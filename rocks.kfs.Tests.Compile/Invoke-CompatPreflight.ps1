# <copyright>
# Copyright 2026 by Kingdom First Solutions
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
# http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
# </copyright>
#

<#
.SYNOPSIS
    Compatibility preflight: compiles every KFS plugin against a Rock clone's assemblies.

.DESCRIPTION
    Finds code that no longer compiles against a new Rock version, across all plugins at once,
    without a running site or database:

      1. Junctions  - runs CreateLinks.bat only if the KFS junctions are missing.
      2. Plugin DLLs - builds every project in KFSRock.sln.kfs from each source ($sources:
                       KFSRockAssemblies and the Attended Check-in fork, RockAttendedCheckin)
                       against RockWeb\Bin (their post-build step copies the DLLs into RockWeb\Bin).
      3. Blocks     - precompiles every WebForms block in each source's tree (rocks_kfs and
                      cc_newspring) with aspnet_compiler, the same compiler IIS uses on first
                      page load.

    The report's Coverage table gives DLL and block counts per source. A source that contributes
    no DLL project or no blocks stops the run (exit 2) instead of being skipped silently.

    aspnet_compiler stops at the first failing file, so failing blocks are set aside and the
    compile is repeated until what remains is clean. Every failure is recorded.

    Also reports calls to Rock APIs marked [Obsolete] (CS0618/CS0612): those are the breaks
    coming in a future Rock version.

    Exit code: 0 = everything compiles, 1 = compile errors found, 2 = environment problem.

.PARAMETER RockRoot
    The Rock clone to test against (e.g. C:\KFSRepo\Rock\Rock20). Defaults to the current
    directory. Rock core must already be built into RockWeb\Bin.

.PARAMETER SkipBuild
    Skip building the plugin DLLs (reuse whatever rocks.kfs.*.dll are already in RockWeb\Bin).

.EXAMPLE
    .\Invoke-CompatPreflight.ps1 -RockRoot C:\KFSRepo\Rock\Rock20
#>
[CmdletBinding()]
param(
    [string] $RockRoot = ( Get-Location ).Path,
    [switch] $SkipBuild
)

$ErrorActionPreference = "Stop"
$started = Get-Date

function Write-Step( [string] $message ) {
    Write-Host ""
    Write-Host "== $message" -ForegroundColor Cyan
}

function Exit-Environment( [string] $message ) {
    Write-Host "ENVIRONMENT PROBLEM: $message" -ForegroundColor Red
    exit 2
}

# The code this preflight covers. Every source must contribute DLL projects and blocks; one that
# silently drops out (e.g. removed from KFSRock.sln.kfs, or a missing junction) stops the run.
$sources = @(
    [pscustomobject]@{ Name = "KFS plugins"; Repo = "KFSRockAssemblies, KFSRockBlocks"; ProjectRoot = "KFSRockAssemblies"; BlockRoot = "rocks_kfs" },
    [pscustomobject]@{ Name = "Attended Check-in"; Repo = "rock-attended-checkin (NewSpring fork)"; ProjectRoot = "RockAttendedCheckin"; BlockRoot = "cc_newspring" }
)

# Project paths are relative to the Rock clone, e.g. "RockAttendedCheckin\cc.newspring.AttendedCheckIn.csproj".
function Get-ProjectSource( [string] $project ) {
    return ( $sources | Where-Object { $project.StartsWith( $_.ProjectRoot + "\", "OrdinalIgnoreCase" ) } | Select-Object -First 1 ).Name
}

# Block paths are relative to the staged app, e.g. "Plugins\cc_newspring\AttendedCheckin\Admin.ascx".
function Get-BlockSource( [string] $block ) {
    $relative = $block -replace '^Plugins\\', ''
    return ( $sources | Where-Object { $relative.StartsWith( $_.BlockRoot + "\", "OrdinalIgnoreCase" ) } | Select-Object -First 1 ).Name
}

#region Validate the Rock clone

$RockRoot = ( Resolve-Path $RockRoot ).ProviderPath.TrimEnd( "\" )
$rockWeb = Join-Path $RockRoot "RockWeb"
$rockDll = Join-Path $rockWeb "Bin\Rock.dll"

if ( -not ( Test-Path $rockDll ) ) {
    Exit-Environment "$rockDll not found. Point -RockRoot at a Rock clone whose core has been built."
}

$rockVersion = ( Get-Item $rockDll ).VersionInfo.ProductVersion
$versionLabel = ( $rockVersion -replace '^Rock\s+\w+\s+', '' ) -replace '\+.*$', ''
Write-Host "Rock clone: $RockRoot"
Write-Host "Rock.dll:   $rockVersion"

#endregion

#region 1. Junctions

Write-Step "Junctions"

$junctions = @(
    "KFSRockAssemblies",
    "RockAttendedCheckin",
    "RockWeb\Plugins\rocks_kfs",
    "RockWeb\Plugins\cc_newspring",
    "RockWeb\Content\KFSRockAssets"
)

$missingJunctions = @( $junctions | Where-Object { -not ( Test-Path ( Join-Path $RockRoot $_ ) ) } )

if ( $missingJunctions.Count -eq 0 ) {
    Write-Host "All KFS junctions present; CreateLinks.bat not needed."
}
else {
    Write-Host "Missing: $( $missingJunctions -join ', ' ). Running CreateLinks.bat."

    # The script lives in the real KFSRockAssemblies folder, so this works even before the junction exists.
    $createLinks = Join-Path ( Split-Path $PSScriptRoot -Parent ) "CreateLinks.bat"
    if ( -not ( Test-Path $createLinks ) ) {
        Exit-Environment "CreateLinks.bat not found at $createLinks."
    }

    # mklink reports an error for links that already exist (e.g. a v20 clone's own .claude); that is expected.
    Push-Location $RockRoot
    try {
        cmd /c "`"$createLinks`"" 2>&1 | ForEach-Object { Write-Host "   $_" }
    }
    finally {
        Pop-Location
    }

    $stillMissing = @( $junctions | Where-Object { -not ( Test-Path ( Join-Path $RockRoot $_ ) ) } )
    if ( $stillMissing.Count -gt 0 ) {
        Exit-Environment "Junctions still missing after CreateLinks.bat: $( $stillMissing -join ', ' )"
    }
}

#endregion

#region Staging

Write-Step "Staging Rock's assemblies"

<#
    9/24/2026 - CLAUDE

    Staging is under %TEMP% with a short name on purpose. .NET Framework's compiler cannot
    read paths over 260 characters, and Rock's Bin has deep codeBase paths such as
    Bin\System.IdentityModel.Tokens.Jwt\4.0.4\System.IdentityModel.Tokens.Jwt.dll.

    Reason: MAX_PATH limit in aspnet_compiler.
#>
# %TEMP% is often an 8.3 short path; compiler output uses long paths, so resolve to the long form.
$tempRoot = ( Get-Item $env:TEMP ).FullName
$stage = Join-Path $tempRoot ( "kfsc-" + ( Split-Path $RockRoot -Leaf ) )
$app = Join-Path $stage "app"
$parked = Join-Path $stage "parked"

if ( Test-Path $stage ) {
    Remove-Item -Recurse -Force $stage -Confirm:$false
}
New-Item -ItemType Directory -Force $app, $parked | Out-Null
$stagedBin = Join-Path $app "Bin"

# Rock's own web.config, so blocks compile with Rock's compiler settings, binding redirects and tag prefixes.
Copy-Item ( Join-Path $rockWeb "web.config" ) ( Join-Path $app "web.config" )
Set-Content -Encoding UTF8 ( Join-Path $app "web.ConnectionStrings.config" ) `
    '<connectionStrings><add name="RockContext" connectionString="Data Source=none;Initial Catalog=none" providerName="System.Data.SqlClient" /></connectionStrings>'

Write-Host "Staging Rock's Bin at $app"
robocopy ( Join-Path $rockWeb "Bin" ) ( Join-Path $app "Bin" ) /S /XF *.refresh /NFL /NDL /NJH /NJS /NP | Out-Null

<#
    A clone's RockWeb\Bin holds only .refresh pointer files for NuGet-supplied DLLs; Visual
    Studio copies the real DLL in when it builds the website. Resolve each pointer from
    <RockRoot>\packages, then the global NuGet cache, restoring any still missing.
#>
$nugetCache = Join-Path $env:USERPROFILE ".nuget\packages"

<#
    9/30/2026 - CLAUDE

    Always stage Rock's declared version of a .refresh-managed DLL, even when a real copy
    already sits in RockWeb\Bin. A plugin's post-build step can copy its own older version of
    a Rock dependency into Bin (Microsoft365Utilities did this with Microsoft.Identity.Client
    4.47.2 over Rock 20's 4.80.0), and staging that copy stops aspnet_compiler before any block
    is checked. The mismatch is reported separately by Test-DependencyConflicts.

    Reason: A plugin-overwritten Rock dependency must be reported, not crash the preflight.
#>
$rockManagedDlls = @{}

function Get-AssemblyVersion( [string] $path ) {
    try {
        return [Reflection.AssemblyName]::GetAssemblyName( $path ).Version.ToString()
    }
    catch {
        return $null
    }
}

function Resolve-RefreshFiles {
    $unresolved = @()
    foreach ( $refresh in Get-ChildItem ( Join-Path $rockWeb "Bin" ) -Filter *.refresh ) {
        $dllName = $refresh.Name -replace '\.refresh$', ''
        if ( $rockManagedDlls.ContainsKey( $dllName ) ) {
            continue
        }

        $relative = ( Get-Content $refresh.FullName -Raw ).Trim()
        $candidates = @( [IO.Path]::GetFullPath( ( Join-Path $rockWeb $relative ) ) )
        if ( $relative -match '^\.\.\\packages\\(?<pkg>[^\\]+?)\.(?<ver>\d[^\\]*)\\(?<rest>.+)$' ) {
            $candidates += Join-Path $nugetCache ( "{0}\{1}\{2}" -f $Matches.pkg.ToLower(), $Matches.ver, $Matches.rest )
        }

        $found = $candidates | Where-Object { Test-Path $_ } | Select-Object -First 1
        if ( $found ) {
            Copy-Item $found ( Join-Path $stagedBin $dllName ) -Force
            $rockManagedDlls[ $dllName ] = Get-AssemblyVersion $found
        }
        else {
            $unresolved += $relative
        }
    }

    return , $unresolved
}

<#
    Finds real DLLs in RockWeb\Bin whose version differs from the version Rock declares for
    them (its .refresh pointer). On a site, that copy replaces Rock's and ASP.NET fails to start
    or load. Attributes each one to the KFS projects whose post-build step copies it into Bin.
#>
function Test-DependencyConflicts {
    $conflicts = @()
    $projectFiles = @( Get-ChildItem ( Join-Path $RockRoot "KFSRockAssemblies" ), ( Join-Path $RockRoot "RockAttendedCheckin" ) -Recurse -Filter *.csproj -ErrorAction SilentlyContinue |
        Where-Object { $_.FullName -notmatch '\\(ZZZ_Archive|node_modules)\\' } )

    foreach ( $dllName in ( $rockManagedDlls.Keys | Sort-Object ) ) {
        $inBin = Join-Path $rockWeb "Bin\$dllName"
        $expected = $rockManagedDlls[ $dllName ]
        if ( -not ( Test-Path $inBin ) -or -not $expected ) {
            continue
        }

        $found = Get-AssemblyVersion $inBin
        if ( $found -and $found -ne $expected ) {
            $copiedBy = @( $projectFiles | Where-Object { Select-String -Path $_.FullName -SimpleMatch -Quiet -Pattern $dllName } |
                ForEach-Object { $_.BaseName } | Sort-Object -Unique )
            $conflicts += [pscustomobject]@{ Dll = $dllName; RockVersion = $expected; FoundVersion = $found; CopiedBy = $copiedBy }
        }
    }

    return , $conflicts
}

$unresolved = Resolve-RefreshFiles
if ( $unresolved.Count -gt 0 ) {
    Write-Host "Restoring $( $unresolved.Count ) RockWeb packages missing from the NuGet cache"
    $packages = @{}
    foreach ( $relative in $unresolved ) {
        if ( $relative -match '^\.\.\\packages\\(?<pkg>[^\\]+?)\.(?<ver>\d[^\\]*)\\' ) {
            $packages[ $Matches.pkg ] = $Matches.ver
        }
    }

    $restoreDir = Join-Path $stage "restore"
    New-Item -ItemType Directory -Force $restoreDir | Out-Null
    $items = ( $packages.GetEnumerator() | ForEach-Object { "    <PackageDownload Include=`"$( $_.Key )`" Version=`"[$( $_.Value )]`" />" } ) -join "`n"
    Set-Content -Encoding UTF8 ( Join-Path $restoreDir "restore.csproj" ) @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup><TargetFramework>net472</TargetFramework></PropertyGroup>
  <ItemGroup>
$items
  </ItemGroup>
</Project>
"@
    dotnet restore ( Join-Path $restoreDir "restore.csproj" ) --verbosity quiet 2>&1 | Out-Null
    $unresolved = Resolve-RefreshFiles
}

if ( $unresolved.Count -gt 0 ) {
    Write-Host "   Not resolvable (only matters if a block uses them): $( ( $unresolved | ForEach-Object { Split-Path $_ -Leaf } ) -join ', ' )" -ForegroundColor DarkGray
}

#endregion

#region 2. Plugin DLLs

$dllResults = @()

if ( $SkipBuild ) {
    Write-Step "Plugin DLLs (skipped: -SkipBuild)"
}
else {
    Write-Step "Plugin DLLs"

    $vswhere = Join-Path ${env:ProgramFiles(x86)} "Microsoft Visual Studio\Installer\vswhere.exe"
    if ( -not ( Test-Path $vswhere ) ) {
        Exit-Environment "vswhere.exe not found; Visual Studio (with MSBuild) is required to build the plugin projects."
    }
    $msbuild = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -find "MSBuild\**\Bin\MSBuild.exe" | Select-Object -First 1
    if ( -not $msbuild ) {
        Exit-Environment "MSBuild not found through vswhere."
    }

    # The solution template lists the active KFS projects (archived ones are excluded from it).
    $template = Join-Path $RockRoot "KFSRockAssemblies\KFSRock.sln.kfs"
    # Anything under a ZZZ_Archive folder is retired by KFS practice and never tested.
    $projectRoots = ( $sources | ForEach-Object { [regex]::Escape( $_.ProjectRoot ) } ) -join "|"
    $projects = @( [regex]::Matches( ( Get-Content $template -Raw ), "`"((?:$projectRoots)\\[^`"]+\.csproj)`"" ) |
        ForEach-Object { $_.Groups[ 1 ].Value } |
        Where-Object { $_ -notmatch '\\ZZZ_Archive\\' } |
        Sort-Object -Unique )

    foreach ( $source in $sources ) {
        if ( -not ( $projects | Where-Object { ( Get-ProjectSource $_ ) -eq $source.Name } ) ) {
            Exit-Environment "KFSRock.sln.kfs lists no $( $source.Name ) project under $( $source.ProjectRoot )\. The preflight would silently skip it."
        }
    }

    Write-Host "Building $( $projects.Count ) projects from KFSRock.sln.kfs with $msbuild"

    foreach ( $project in $projects ) {
        $projectPath = Join-Path $RockRoot $project
        $name = [IO.Path]::GetFileNameWithoutExtension( $project )

        # SolutionDir makes packages.config restore to <RockRoot>\packages (where HintPaths point)
        # and makes the post-build step copy the DLL into <RockRoot>\RockWeb\bin.
        # ReferencePath: a clone's RockWeb\Bin has only .refresh pointers for NuGet DLLs such as
        # EntityFramework; the staged Bin has the real files, so HintPaths that miss fall back to it.
        $common = @( $projectPath, "/nologo", "/v:q", "/p:Configuration=Debug", "/p:SolutionDir=$RockRoot\", "/p:ReferencePath=$stagedBin" )
        $restoreOutput = & $msbuild @common "/t:Restore" "/p:RestorePackagesConfig=true" 2>&1 | Out-String
        $buildOutput = & $msbuild @common "/t:Build" "/clp:ErrorsOnly" 2>&1 | Out-String
        $succeeded = ( $LASTEXITCODE -eq 0 )

        $errors = @( [regex]::Matches( $restoreOutput + $buildOutput, '(?m)^\s*(?<text>[^\r\n]*?: error [^\r\n]+)' ) |
            ForEach-Object { ( $_.Groups[ 'text' ].Value -replace '\s*\[[^\]]+\.csproj\]\s*$', '' ).Replace( "$RockRoot\", "" ).Trim() } |
            Sort-Object -Unique )

        $dllResults += [pscustomobject]@{ Project = $name; Source = ( Get-ProjectSource $project ); Succeeded = $succeeded; Errors = $errors }

        if ( $succeeded ) {
            Write-Host ( "   ok     {0}" -f $name )
        }
        else {
            Write-Host ( "   FAILED {0} ({1} errors)" -f $name, $errors.Count ) -ForegroundColor Yellow
        }
    }

    # Pick up the freshly built plugin DLLs for the block compile.
    robocopy ( Join-Path $rockWeb "Bin" ) $stagedBin rocks.kfs.*.dll cc.newspring.*.dll EventBriteDotNetFramework.dll ZoomDotNetFramework.dll /NFL /NDL /NJH /NJS /NP | Out-Null
}

# After the build, because plugin post-build steps are what put conflicting copies into Bin.
$dependencyConflicts = Test-DependencyConflicts
foreach ( $conflict in $dependencyConflicts ) {
    Write-Host ( "   CONFLICT {0}: Rock needs {1}, Bin has {2} (copied by {3})" -f $conflict.Dll, $conflict.RockVersion, $conflict.FoundVersion, ( $conflict.CopiedBy -join ", " ) ) -ForegroundColor Yellow
}

#endregion

#region 3. Blocks

Write-Step "Blocks"

$aspnetCompiler = Join-Path $env:WINDIR "Microsoft.NET\Framework64\v4.0.30319\aspnet_compiler.exe"
if ( -not ( Test-Path $aspnetCompiler ) ) {
    Exit-Environment "aspnet_compiler.exe not found at $aspnetCompiler (.NET Framework 4.x is required)."
}

# Blocks: each source's block tree, minus archived code and built Obsidian output.
foreach ( $source in $sources ) {
    robocopy ( Join-Path $rockWeb "Plugins\$( $source.BlockRoot )" ) ( Join-Path $app "Plugins\$( $source.BlockRoot )" ) *.ascx *.ascx.cs *.cs *.lava /S /XD ZZZ_Archive .git Obsidian node_modules /NFL /NDL /NJH /NJS /NP | Out-Null
}

$allBlocks = @( Get-ChildItem -Recurse ( Join-Path $app "Plugins" ) -Filter *.ascx | ForEach-Object { $_.FullName.Substring( $app.Length + 1 ) } )

foreach ( $source in $sources ) {
    if ( -not ( $allBlocks | Where-Object { ( Get-BlockSource $_ ) -eq $source.Name } ) ) {
        Exit-Environment "No $( $source.Name ) blocks found under RockWeb\Plugins\$( $source.BlockRoot ). The preflight would silently skip them."
    }
}

Write-Host "Compiling $( $allBlocks.Count ) blocks ($( ( $sources | ForEach-Object { $name = $_.Name; '{0}: {1}' -f $name, @( $allBlocks | Where-Object { ( Get-BlockSource $_ ) -eq $name } ).Count } ) -join ', ' ))"

$blockErrors = @{}
$obsoleteUsages = @{}
$errorPattern = '(?m)^(?<file>[A-Za-z]:\\[^\r\n(]+?\.(?:ascx|ascx\.cs|cs))\((?<line>\d+)\): (?<kind>error|warning) (?<code>\w+): (?<msg>[^\r\n]+)'

for ( $pass = 1; $pass -le $allBlocks.Count + 1; $pass++ ) {
    $output = & $aspnetCompiler -v "/kfscheck$pass" -p $app 2>&1 | Out-String
    $exitCode = $LASTEXITCODE

    foreach ( $match in [regex]::Matches( $output, $errorPattern ) ) {
        $file = $match.Groups[ 'file' ].Value.Substring( $app.Length + 1 )
        $entry = "{0}({1}): {2} {3}" -f $file, $match.Groups[ 'line' ].Value, $match.Groups[ 'code' ].Value, $match.Groups[ 'msg' ].Value

        if ( $match.Groups[ 'kind' ].Value -eq "warning" ) {
            if ( $match.Groups[ 'code' ].Value -in @( "CS0618", "CS0612" ) ) {
                $obsoleteUsages[ $entry ] = $true
            }
            continue
        }

        $block = $file -replace '\.cs$', ''
        if ( -not $blockErrors.ContainsKey( $block ) ) {
            $blockErrors[ $block ] = @()
        }
        if ( $blockErrors[ $block ] -notcontains $entry ) {
            $blockErrors[ $block ] += $entry
        }
    }

    if ( $exitCode -eq 0 ) {
        break
    }

    # Park the files that failed this pass and compile the rest again.
    $failedThisPass = @( [regex]::Matches( $output, $errorPattern ) |
        Where-Object { $_.Groups[ 'kind' ].Value -eq "error" } |
        ForEach-Object { $_.Groups[ 'file' ].Value -replace '\.cs$', '' } | Sort-Object -Unique )

    if ( $failedThisPass.Count -eq 0 ) {
        $firstError = ( $output -split "`r?`n" | Where-Object { $_ -match 'error' } | Select-Object -First 3 ) -join " | "
        Exit-Environment "aspnet_compiler failed without a file-level error (staging problem, not a plugin problem): $firstError"
    }

    foreach ( $failed in $failedThisPass ) {
        foreach ( $path in @( $failed, "$failed.cs" ) ) {
            if ( Test-Path $path ) {
                $relative = $path.Substring( $app.Length + 1 )
                $destination = Join-Path $parked $relative
                New-Item -ItemType Directory -Force ( Split-Path $destination -Parent ) | Out-Null
                Move-Item $path $destination -Force
            }
        }
    }

    Write-Host ( "   pass {0}: {1} block(s) failed, retrying without them" -f $pass, $failedThisPass.Count )
}

#endregion

#region Report

# A block whose errors are all "type or namespace not found" for a KFS assembly is blocked by a DLL build failure, not broken itself.
$failedDllNames = @( $dllResults | Where-Object { -not $_.Succeeded } | ForEach-Object { $_.Project } )
$kfsNamespacePattern = 'rocks\.kfs|''rocks''|EventbriteDotNetFramework|ZoomDotNetFramework|VimeoDotNet|cc\.newspring'

$brokenBlocks = @()
$blockedBlocks = @()
foreach ( $block in ( $blockErrors.Keys | Sort-Object ) ) {
    $errors = $blockErrors[ $block ]
    $missingKfsTypes = @( $errors | Where-Object { $_ -match '(CS0246|CS0234|CS0103)' -and $_ -match $kfsNamespacePattern } )
    $record = [pscustomobject]@{ Block = $block.Replace( "Plugins\", "" ); Source = ( Get-BlockSource $block ); Errors = $errors }

    if ( $failedDllNames.Count -gt 0 -and $missingKfsTypes.Count -gt 0 ) {
        $blockedBlocks += $record
    }
    else {
        $brokenBlocks += $record
    }
}

$cleanCount = $allBlocks.Count - $blockErrors.Count
$failedDlls = @( $dllResults | Where-Object { -not $_.Succeeded } )

$resultsDir = Join-Path $PSScriptRoot ( "results\" + $versionLabel )
New-Item -ItemType Directory -Force $resultsDir | Out-Null

$lines = @()
$lines += "# KFS compatibility preflight"
$lines += ""
$lines += "- Rock clone: ``$RockRoot``"
$lines += "- Rock.dll: $rockVersion"
$lines += "- Run: $( $started.ToString( 's' ) ), $( [int]( ( Get-Date ) - $started ).TotalSeconds ) s"
$lines += ""
$lines += "| Check | Result |"
$lines += "|---|---|"
if ( $SkipBuild ) {
    $lines += "| Plugin DLLs | skipped |"
}
else {
    $lines += "| Plugin DLLs | $( $dllResults.Count - $failedDlls.Count ) of $( $dllResults.Count ) build |"
}
$lines += "| Blocks | $cleanCount of $( $allBlocks.Count ) compile; $( $brokenBlocks.Count ) broken; $( $blockedBlocks.Count ) blocked by a failed DLL |"
$lines += "| Rock dependencies overwritten by a plugin | $( $dependencyConflicts.Count ) |"
$lines += "| Obsolete Rock APIs in use | $( $obsoleteUsages.Count ) call sites |"

# Per-source counts, so every repo the preflight covers is visibly accounted for.
$coverageLines = @()
$coverageLines += ""
$coverageLines += "## Coverage"
$coverageLines += ""
$coverageLines += "| Source | Repo | Plugin DLLs | Blocks |"
$coverageLines += "|---|---|---|---|"
foreach ( $source in $sources ) {
    $sourceDlls = @( $dllResults | Where-Object { $_.Source -eq $source.Name } )
    $dllCell = if ( $SkipBuild ) { "skipped" } else { "$( @( $sourceDlls | Where-Object { $_.Succeeded } ).Count ) of $( $sourceDlls.Count ) build" }
    $sourceBlocks = @( $allBlocks | Where-Object { ( Get-BlockSource $_ ) -eq $source.Name } )
    $sourceFailed = @( $blockErrors.Keys | Where-Object { ( Get-BlockSource $_ ) -eq $source.Name } )
    $coverageLines += "| $( $source.Name ) | $( $source.Repo ) | $dllCell | $( $sourceBlocks.Count - $sourceFailed.Count ) of $( $sourceBlocks.Count ) compile |"
}
$lines += $coverageLines

if ( $dependencyConflicts.Count -gt 0 ) {
    $lines += ""
    $lines += "## Plugin DLLs that overwrite a Rock dependency"
    $lines += ""
    $lines += "A plugin's build copies its own version of a library Rock depends on into RockWeb\Bin. On a site this replaces Rock's copy, and ASP.NET fails to start or load the assembly."
    $lines += ""
    $lines += "| DLL | Rock needs | Plugin supplies | Copied by |"
    $lines += "|---|---|---|---|"
    foreach ( $conflict in $dependencyConflicts ) {
        $lines += "| ``$( $conflict.Dll )`` | $( $conflict.RockVersion ) | $( $conflict.FoundVersion ) | $( $conflict.CopiedBy -join ', ' ) |"
    }
}

if ( $failedDlls.Count -gt 0 ) {
    $lines += ""
    $lines += "## Plugin DLLs that do not build"
    foreach ( $dll in $failedDlls ) {
        $lines += ""
        $lines += "### $( $dll.Project )"
        $dll.Errors | Select-Object -First 15 | ForEach-Object { $lines += "- ``$_``" }
        if ( $dll.Errors.Count -gt 15 ) {
            $lines += "- ... and $( $dll.Errors.Count - 15 ) more"
        }
    }
}

foreach ( $section in @( @{ Title = "Broken blocks"; Items = $brokenBlocks }, @{ Title = "Blocks that could not be checked (depend on a plugin DLL that failed to build)"; Items = $blockedBlocks } ) ) {
    if ( $section.Items.Count -eq 0 ) {
        continue
    }
    $lines += ""
    $lines += "## $( $section.Title )"
    foreach ( $item in $section.Items ) {
        $lines += ""
        $lines += "### $( $item.Block )"
        $item.Errors | ForEach-Object { $lines += "- ``$( $_.Replace( 'Plugins\', '' ) )``" }
    }
}

if ( $obsoleteUsages.Count -gt 0 ) {
    $lines += ""
    $lines += "## Obsolete Rock APIs in use (will break in a future Rock version)"
    $lines += ""
    $lines += "Grouped by API, most-used first. Only blocks that compiled are included; the DLL builds are not scanned for these."
    $lines += ""
    $lines += "| API | Uses | Blocks | Rock's note |"
    $lines += "|---|---|---|---|"

    $groups = $obsoleteUsages.Keys | ForEach-Object {
        $usage = $_
        $api = if ( $usage -match "'(?<api>[^']+)' is obsolete" ) { $Matches.api } else { "(unparsed)" }
        $note = if ( $usage -match "is obsolete: '(?<note>[^']*)'" ) { $Matches.note } else { "" }
        $block = ( $usage -replace '\(\d+\):.*$', '' ) -replace '^Plugins\\', '' -replace '\.cs$', ''
        [pscustomobject]@{ Api = $api; Note = $note; Block = $block }
    } | Group-Object Api | Sort-Object Count -Descending

    foreach ( $group in $groups ) {
        # Folder\File, so blocks with common names (Admin.ascx, Confirm.ascx) say whose they are.
        $blocks = @( $group.Group | ForEach-Object { ( $_.Block -split '\\' | Select-Object -Last 2 ) -join '\' } | Sort-Object -Unique )
        $blockList = ( $blocks | Select-Object -First 4 ) -join ", "
        if ( $blocks.Count -gt 4 ) {
            $blockList += " +$( $blocks.Count - 4 ) more"
        }
        $lines += "| ``$( $group.Name )`` | $( $group.Count ) | $blockList | $( $group.Group[ 0 ].Note.Replace( '|', '/' ) ) |"
    }
}

$lines += ""
$lines += "Note: the compiler stops at the first failing file, and a file can hide further errors behind the ones listed. Re-run after fixing."

$reportPath = Join-Path $resultsDir "preflight.md"
$lines | Set-Content -Encoding UTF8 $reportPath

[pscustomobject]@{
    rockRoot = $RockRoot
    rockVersion = $rockVersion
    started = $started.ToString( 's' )
    dlls = $dllResults
    blocksTotal = $allBlocks.Count
    blocksClean = $cleanCount
    brokenBlocks = $brokenBlocks
    blockedBlocks = $blockedBlocks
    dependencyConflicts = $dependencyConflicts
    obsoleteUsages = @( $obsoleteUsages.Keys | Sort-Object )
} | ConvertTo-Json -Depth 5 | Set-Content -Encoding UTF8 ( Join-Path $resultsDir "preflight.json" )

Write-Step "Summary"
$lines | Select-Object -Skip 6 -First 6 | ForEach-Object { Write-Host $_ }
$coverageLines | Select-Object -Skip 3 | ForEach-Object { Write-Host $_ }
Write-Host ""
Write-Host "Report: $reportPath"

if ( $failedDlls.Count -gt 0 -or $brokenBlocks.Count -gt 0 -or $blockedBlocks.Count -gt 0 -or $dependencyConflicts.Count -gt 0 ) {
    exit 1
}
exit 0

#endregion
