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
    Compatibility preflight: compiles every active KFS plugin against a Rock clone's assemblies,
    and reports the results by plugin.

.DESCRIPTION
    Finds code that no longer compiles against a new Rock version, for all active plugins at
    once, without a running site or database.

    A plugin is a RockShop package in the shared Rock Packages folder; its code is what its
    newest package ships (PluginCatalog.ps1). A plugin is active when it ships code from our
    repos: DLLs built by KFSRockAssemblies or RockAttendedCheckin projects, and WebForms
    pages (.ascx/.aspx/.ashx) under Plugins\rocks_kfs or Plugins\cc_newspring. Code that no
    package ships is not tested; the report lists those plugins that are not tested and why.

      1. Junctions   - runs CreateLinks.bat only if the KFS junctions are missing.
      2. Plugin DLLs - builds each active plugin's projects against RockWeb\Bin (their
                       post-build step copies the DLLs into RockWeb\Bin).
      3. Pages       - precompiles each active plugin's pages with aspnet_compiler, the same
                       compiler IIS uses on first page load.

    aspnet_compiler stops at the first failing file, so failing pages are set aside and the
    compile is repeated until what remains is clean. Every failure is recorded.

    Also reports calls to Rock APIs marked [Obsolete] (CS0618/CS0612): those are the breaks
    coming in a future Rock version.

    Exit code: 0 = every plugin compiles, 1 = compile errors found, 2 = environment problem.

.PARAMETER RockRoot
    The Rock clone to test against (e.g. C:\KFSRepo\Rock\Rock20). Defaults to the current
    directory. Rock core must already be built into RockWeb\Bin.

.PARAMETER RockPackagesRoot
    The shared Rock Packages folder, which defines the plugins (read only).

.PARAMETER Plugin
    Test only these plugins (Rock Packages folder names, with or without "(Unreleased)").
    Default: every active plugin.

.PARAMETER ListPlugins
    List the plugins and the code each one ships, then stop. Builds and compiles nothing.

.PARAMETER SkipBuild
    Skip building the plugin DLLs (reuse whatever DLLs are already in RockWeb\Bin).

.EXAMPLE
    .\Invoke-CompatPreflight.ps1 -RockRoot C:\KFSRepo\Rock\Rock20

.EXAMPLE
    .\Invoke-CompatPreflight.ps1 -RockRoot C:\KFSRepo\Rock\Rock20 -Plugin "Steps to Care", "Eventbrite"
#>
[CmdletBinding()]
param(
    [string] $RockRoot = ( Get-Location ).Path,
    [string] $RockPackagesRoot = ( Join-Path $env:USERPROFILE "OneDrive - Kingdom First Solutions Inc\Shared Documents - Kingdom First Solutions Inc. Team Site\Rock Packages" ),
    [string[]] $Plugin,
    [switch] $ListPlugins,
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

. ( Join-Path $PSScriptRoot "PluginCatalog.ps1" )

# The repo a project or page comes from. Project paths are relative to the Rock clone
# ("RockAttendedCheckin\cc.newspring.AttendedCheckIn.csproj"); page paths to RockWeb
# ("Plugins\cc_newspring\AttendedCheckin\Admin.ascx").
function Get-CodeRepo( [string] $path ) {
    if ( $path -match '^(RockAttendedCheckin|Plugins\\cc_newspring)\\' ) {
        return "rock-attended-checkin"
    }
    if ( $path -match '^Plugins\\' ) {
        return "KFSRockBlocks"
    }
    return "KFSRockAssemblies"
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

#region Plugins

Write-Step "Plugins"

if ( -not ( Test-Path $RockPackagesRoot ) ) {
    Exit-Environment "Rock Packages folder not found: $RockPackagesRoot. It defines the plugins; pass -RockPackagesRoot."
}

$catalog = @( Get-PluginCatalog -RockRoot $RockRoot -PackagesRoot $RockPackagesRoot )

if ( $Plugin ) {
    $unknown = @( $Plugin | Where-Object { $name = $_; -not ( $catalog | Where-Object { $_.Name -eq $name -or $_.Folder -eq $name } ) } )
    if ( $unknown.Count -gt 0 ) {
        Exit-Environment "Unknown plugin(s): $( $unknown -join ', ' ). Plugins: $( ( $catalog | ForEach-Object { $_.Name } ) -join ', ' )"
    }
    $catalog = @( $catalog | Where-Object { $_.Name -in $Plugin -or $_.Folder -in $Plugin } )
}

$activePlugins = @( $catalog | Where-Object { $_.Status -eq "Active" } )
$untestedPlugins = @( $catalog | Where-Object { $_.Status -ne "Active" } )

foreach ( $entry in $catalog ) {
    $label = if ( $entry.Unreleased ) { "$( $entry.Name ) (unreleased)" } else { $entry.Name }
    Write-Host ( "   {0,-13} {1,-38} {2,-10} {3} DLL project(s), {4} page(s)" -f $entry.Status, $label, $entry.Version, $entry.Projects.Count, $entry.Pages.Count )
    if ( $ListPlugins ) {
        $entry.Projects | ForEach-Object { Write-Host "                   project  $_" }
        $entry.Pages | ForEach-Object { Write-Host "                   page     $_" }
        $entry.Missing | ForEach-Object { Write-Host "                   missing  $_" -ForegroundColor Yellow }
    }
}

if ( $ListPlugins ) {
    exit 0
}

if ( $activePlugins.Count -eq 0 ) {
    Exit-Environment "No active plugins to test (none of the selected packages ships code from our repos)."
}

# The code under test: every project and page an active plugin ships, each tested once.
$projects = @( $activePlugins | ForEach-Object { $_.Projects } | Sort-Object -Unique )
$pages = @( $activePlugins | ForEach-Object { $_.Pages } | Sort-Object -Unique )
Write-Host "Testing $( $activePlugins.Count ) active plugin(s): $( $projects.Count ) DLL project(s), $( $pages.Count ) page(s)."

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

    # $projects: the active plugins' projects (see Plugins). Archived projects never appear there.
    Write-Host "Building $( $projects.Count ) projects with $msbuild"

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

        $dllResults += [pscustomobject]@{ Project = $name; Path = $project; Repo = ( Get-CodeRepo $project ); Succeeded = $succeeded; Errors = $errors }

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

Write-Step "Pages"

$aspnetCompiler = Join-Path $env:WINDIR "Microsoft.NET\Framework64\v4.0.30319\aspnet_compiler.exe"
if ( -not ( Test-Path $aspnetCompiler ) ) {
    Exit-Environment "aspnet_compiler.exe not found at $aspnetCompiler (.NET Framework 4.x is required)."
}

# Stage only the active plugins' pages (blocks, .aspx pages, .ashx handlers) and their code-behind.
foreach ( $page in $pages ) {
    $destination = Join-Path $app $page
    New-Item -ItemType Directory -Force ( Split-Path $destination -Parent ) | Out-Null
    Copy-Item ( Join-Path $rockWeb $page ) $destination
    if ( Test-Path ( Join-Path $rockWeb "$page.cs" ) ) {
        Copy-Item ( Join-Path $rockWeb "$page.cs" ) "$destination.cs"
    }
}

# Page paths relative to the staged app, e.g. "Plugins\rocks_kfs\Crm\PersonAttributeForms.ascx".
$allBlocks = $pages

Write-Host "Compiling $( $allBlocks.Count ) pages"

$blockErrors = @{}
$obsoleteUsages = @{}
$errorPattern = '(?m)^(?<file>[A-Za-z]:\\[^\r\n(]+?\.(?:(?:ascx|aspx|ashx)(?:\.cs)?|cs))\((?<line>\d+)\): (?<kind>error|warning) (?<code>\w+): (?<msg>[^\r\n]+)'

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

# A page whose errors are all "type or namespace not found" for a KFS assembly is blocked by a DLL build failure, not broken itself.
$failedDllNames = @( $dllResults | Where-Object { -not $_.Succeeded } | ForEach-Object { $_.Project } )
$kfsNamespacePattern = 'rocks\.kfs|''rocks''|EventbriteDotNetFramework|ZoomDotNetFramework|VimeoDotNet|cc\.newspring'

# Every page's result, keyed by its path relative to RockWeb ("Plugins\...").
$pageResults = @{}
foreach ( $page in $allBlocks ) {
    $errors = if ( $blockErrors.ContainsKey( $page ) ) { @( $blockErrors[ $page ] ) } else { @() }
    $missingKfsTypes = @( $errors | Where-Object { $_ -match '(CS0246|CS0234|CS0103)' -and $_ -match $kfsNamespacePattern } )
    $result = if ( $errors.Count -eq 0 ) { "compiles" }
        elseif ( $failedDllNames.Count -gt 0 -and $missingKfsTypes.Count -gt 0 ) { "not checked" }
        else { "broken" }
    $pageResults[ $page ] = [pscustomobject]@{ Page = $page; Repo = ( Get-CodeRepo $page ); Result = $result; Errors = $errors }
}

# Obsolete-API warnings, by page ("Plugins\...\X.ascx.cs(12): CS0618 ..." -> "Plugins\...\X.ascx").
$obsoleteByPage = @{}
foreach ( $usage in $obsoleteUsages.Keys ) {
    $page = ( $usage -replace '\(\d+\):.*$', '' ) -replace '\.cs$', ''
    if ( -not $obsoleteByPage.ContainsKey( $page ) ) {
        $obsoleteByPage[ $page ] = @()
    }
    $obsoleteByPage[ $page ] += $usage
}

function Get-ObsoleteApiGroups( [string[]] $usages ) {
    return @( $usages | ForEach-Object {
        $api = if ( $_ -match "'(?<api>[^']+)' is obsolete" ) { $Matches.api } else { "(unparsed)" }
        $note = if ( $_ -match "is obsolete: '(?<note>[^']*)'" ) { $Matches.note } else { "" }
        [pscustomobject]@{ Api = $api; Note = $note; Page = ( ( $_ -replace '\(\d+\):.*$', '' ) -replace '\.cs$', '' ) }
    } | Group-Object Api | Sort-Object Count -Descending )
}

$icon = @{
    Pass       = [char]::ConvertFromUtf32( 0x2705 )
    Fail       = [char]::ConvertFromUtf32( 0x274C )
    Incomplete = [char]::ConvertFromUtf32( 0x26A0 )
}

# Each active plugin's result, from its own projects and pages.
$pluginResults = @( foreach ( $entry in $activePlugins ) {
    $dlls = @( $entry.Projects | ForEach-Object {
        $path = $_
        $dllResults | Where-Object { $_.Path -eq $path } | Select-Object -First 1
    } | Where-Object { $_ } )
    $pluginPages = @( $entry.Pages | ForEach-Object { $pageResults[ $_ ] } )
    $obsolete = @( $entry.Pages | ForEach-Object { if ( $obsoleteByPage.ContainsKey( $_ ) ) { $obsoleteByPage[ $_ ] } } )
    $projectNames = @( $entry.Projects | ForEach-Object { [IO.Path]::GetFileNameWithoutExtension( $_ ) } )
    $conflicts = @( $dependencyConflicts | Where-Object { @( $_.CopiedBy | Where-Object { $_ -in $projectNames } ).Count -gt 0 } )

    $failedDllCount = @( $dlls | Where-Object { -not $_.Succeeded } ).Count
    $brokenPageCount = @( $pluginPages | Where-Object { $_.Result -eq "broken" } ).Count
    $uncheckedPageCount = @( $pluginPages | Where-Object { $_.Result -eq "not checked" } ).Count

    $result = if ( $failedDllCount + $brokenPageCount + $conflicts.Count -gt 0 ) { "Fail" }
        elseif ( $uncheckedPageCount -gt 0 ) { "Incomplete" }
        else { "Pass" }

    $notes = @()
    if ( $entry.Unreleased ) { $notes += "unreleased" }
    if ( $failedDllCount ) { $notes += "$failedDllCount DLL(s) do not build" }
    if ( $brokenPageCount ) { $notes += "$brokenPageCount page(s) broken" }
    if ( $uncheckedPageCount ) { $notes += "$uncheckedPageCount page(s) not checked" }
    if ( $conflicts.Count ) { $notes += "overwrites $( $conflicts.Count ) Rock dependency(ies)" }
    if ( $entry.Missing.Count ) { $notes += "$( $entry.Missing.Count ) shipped file(s) not on this branch" }
    if ( $entry.Obsidian.Count ) { $notes += "Obsidian files not compiled" }

    [pscustomobject]@{
        Plugin = $entry.Name; Folder = $entry.Folder; Version = $entry.Version; Unreleased = $entry.Unreleased
        Result = $result; Notes = $notes
        Dlls = $dlls; Pages = $pluginPages; Obsolete = $obsolete; Conflicts = $conflicts
        Missing = $entry.Missing; Obsidian = $entry.Obsidian; Dependencies = $entry.Dependencies
    }
} )

$passing = @( $pluginResults | Where-Object { $_.Result -eq "Pass" } ).Count
$failing = @( $pluginResults | Where-Object { $_.Result -eq "Fail" } ).Count
$incomplete = @( $pluginResults | Where-Object { $_.Result -eq "Incomplete" } ).Count
$failedDlls = @( $dllResults | Where-Object { -not $_.Succeeded } )
$cleanCount = @( $pageResults.Values | Where-Object { $_.Result -eq "compiles" } ).Count

$resultsDir = Join-Path $PSScriptRoot ( "results\" + $versionLabel )
New-Item -ItemType Directory -Force $resultsDir | Out-Null

$scope = if ( $Plugin ) { "selected plugins: $( $Plugin -join ', ' )" } else { "every plugin in Rock Packages" }

$lines = @()
$lines += "# KFS compatibility preflight"
$lines += ""
$lines += "- Rock clone: ``$RockRoot``"
$lines += "- Rock.dll: $rockVersion"
$lines += "- Run: $( $started.ToString( 's' ) ), $( [int]( ( Get-Date ) - $started ).TotalSeconds ) s"
$lines += "- Scope: $scope (plugins are defined by their packages in ``$RockPackagesRoot``)"
$lines += ""
$lines += "| Check | Result |"
$lines += "|---|---|"
$lines += "| Plugins | $passing of $( $pluginResults.Count ) active plugins pass; $failing fail; $incomplete incomplete; $( $untestedPlugins.Count ) not tested |"
if ( $SkipBuild ) {
    $lines += "| Plugin DLLs | skipped |"
}
else {
    $lines += "| Plugin DLLs | $( $dllResults.Count - $failedDlls.Count ) of $( $dllResults.Count ) build |"
}
$lines += "| Pages | $cleanCount of $( $allBlocks.Count ) compile |"
$lines += "| Rock dependencies overwritten by a plugin | $( $dependencyConflicts.Count ) |"
$lines += "| Obsolete Rock APIs in use | $( $obsoleteUsages.Count ) call sites |"

$summaryLines = @()
$summaryLines += ""
$summaryLines += "## Results by plugin"
$summaryLines += ""
$summaryLines += "| Result | Plugin | Version | DLLs build | Pages compile | Obsolete calls | Notes |"
$summaryLines += "|---|---|---|---|---|---|---|"
foreach ( $p in $pluginResults ) {
    $dllCell = if ( $SkipBuild ) { "skipped" } elseif ( $p.Dlls.Count ) { "$( @( $p.Dlls | Where-Object { $_.Succeeded } ).Count ) of $( $p.Dlls.Count )" } else { "-" }
    $pageCell = if ( $p.Pages.Count ) { "$( @( $p.Pages | Where-Object { $_.Result -eq 'compiles' } ).Count ) of $( $p.Pages.Count )" } else { "-" }
    $summaryLines += "| $( $icon[ $p.Result ] ) $( $p.Result ) | [$( $p.Plugin )](#$( ( $p.Plugin.ToLower() -replace '[^a-z0-9 -]', '' ) -replace ' ', '-' )) | $( $p.Version ) | $dllCell | $pageCell | $( $p.Obsolete.Count ) | $( $p.Notes -join '; ' ) |"
}
$lines += $summaryLines

if ( $untestedPlugins.Count -gt 0 ) {
    $lines += ""
    $lines += "## Not tested"
    $lines += ""
    $lines += "These packages ship no WebForms or DLL code from our repos, so the preflight has nothing to compile for them."
    $lines += ""
    $lines += "| Plugin | Version | Why |"
    $lines += "|---|---|---|"
    foreach ( $entry in $untestedPlugins ) {
        $why = switch ( $entry.Status ) {
            "Obsidian only" { "Ships only built Obsidian files, which this preflight does not compile" }
            "No KFS code" { "Ships no code from our repos (a vendor plugin)" }
            default { "No package found in its folder" }
        }
        if ( $entry.Missing.Count ) {
            $missingFiles = ( $entry.Missing | ForEach-Object { ( $_ -split ' \(' )[ 0 ] } ) -join ", "
            $why += "; not on this branch: $missingFiles"
        }
        $lines += "| $( $entry.Name ) | $( $entry.Version ) | $why |"
    }
}

foreach ( $p in $pluginResults ) {
    $lines += ""
    $lines += "## $( $p.Plugin )"
    $lines += ""
    $lines += "$( $icon[ $p.Result ] ) **$( $p.Result )**. Package $( $p.Version ) (``$( $p.Folder )``)."
    $lines += ""
    $lines += "| Result | Code | Kind | Repo |"
    $lines += "|---|---|---|---|"
    foreach ( $dll in $p.Dlls ) {
        $cell = if ( $dll.Succeeded ) { "$( $icon.Pass ) builds" } else { "$( $icon.Fail ) does not build" }
        $lines += "| $cell | $( $dll.Project ) | DLL project | $( $dll.Repo ) |"
    }
    if ( $SkipBuild -and $p.Dlls.Count -eq 0 -and $activePlugins.Where( { $_.Name -eq $p.Plugin } )[ 0 ].Projects.Count ) {
        $lines += "| skipped | $( ( $activePlugins.Where( { $_.Name -eq $p.Plugin } )[ 0 ].Projects | ForEach-Object { [IO.Path]::GetFileNameWithoutExtension( $_ ) } ) -join ', ' ) | DLL project | (-SkipBuild) |"
    }
    foreach ( $page in $p.Pages ) {
        $cell = switch ( $page.Result ) { "compiles" { "$( $icon.Pass ) compiles" } "broken" { "$( $icon.Fail ) broken" } default { "$( $icon.Incomplete ) not checked (a DLL it needs failed to build)" } }
        $kind = switch -Regex ( $page.Page ) { '\.ascx$' { "block" } '\.aspx$' { "page" } default { "handler" } }
        $lines += "| $cell | $( $page.Page -replace '^Plugins\\', '' ) | $kind | $( $page.Repo ) |"
    }

    foreach ( $dll in @( $p.Dlls | Where-Object { -not $_.Succeeded } ) ) {
        $lines += ""
        $lines += "**$( $dll.Project ) does not build:**"
        $dll.Errors | Select-Object -First 15 | ForEach-Object { $lines += "- ``$_``" }
        if ( @( $dll.Errors ).Count -gt 15 ) {
            $lines += "- ... and $( @( $dll.Errors ).Count - 15 ) more"
        }
    }
    foreach ( $page in @( $p.Pages | Where-Object { $_.Result -ne "compiles" } ) ) {
        $lines += ""
        $lines += "**$( $page.Page -replace '^Plugins\\', '' ) ($( $page.Result )):**"
        $page.Errors | ForEach-Object { $lines += "- ``$( $_ -replace '^Plugins\\', '' )``" }
    }
    foreach ( $conflict in $p.Conflicts ) {
        $lines += ""
        $lines += "**Overwrites Rock's ``$( $conflict.Dll )``:** Rock needs $( $conflict.RockVersion ), the build copies $( $conflict.FoundVersion ) (by $( $conflict.CopiedBy -join ', ' )). On a site this replaces Rock's copy."
    }
    if ( $p.Missing.Count ) {
        $lines += ""
        $lines += "Shipped by the package but not on this branch (not tested):"
        $p.Missing | ForEach-Object { $lines += "- ``$_``" }
    }
    if ( $p.Obsidian.Count ) {
        $lines += ""
        $lines += "Obsidian files the package ships (not compiled by the preflight): $( ( $p.Obsidian | ForEach-Object { Split-Path $_ -Leaf } ) -join ', ' )"
    }
    if ( $p.Obsolete.Count ) {
        $lines += ""
        $lines += "Obsolete Rock APIs used ($( $p.Obsolete.Count ) call sites):"
        foreach ( $group in Get-ObsoleteApiGroups $p.Obsolete ) {
            $lines += "- ``$( $group.Name )`` x$( $group.Count ): $( $group.Group[ 0 ].Note )"
        }
    }
}

# Conflicts no active plugin's project causes (e.g. left in Bin by an earlier build of other code).
$otherConflicts = @( $dependencyConflicts | Where-Object { $c = $_; -not ( $pluginResults | Where-Object { $c -in $_.Conflicts } ) } )
if ( $otherConflicts.Count -gt 0 ) {
    $lines += ""
    $lines += "## Rock dependencies overwritten by code outside the active plugins"
    $lines += ""
    $lines += "| DLL | Rock needs | Bin has | Copied by |"
    $lines += "|---|---|---|---|"
    foreach ( $conflict in $otherConflicts ) {
        $lines += "| ``$( $conflict.Dll )`` | $( $conflict.RockVersion ) | $( $conflict.FoundVersion ) | $( ( @( $conflict.CopiedBy ) -join ', ' ) -replace '^$', '(unknown)' ) |"
    }
}

$lines += ""
$lines += "Note: the compiler stops at the first failing file, and a file can hide further errors behind the ones listed. Re-run after fixing."

# A run limited with -Plugin must not replace the all-plugins report.
$reportName = if ( $Plugin ) { "preflight-selected" } else { "preflight" }
$reportPath = Join-Path $resultsDir "$reportName.md"
$lines | Set-Content -Encoding UTF8 $reportPath

[pscustomobject]@{
    rockRoot = $RockRoot
    rockVersion = $rockVersion
    started = $started.ToString( 's' )
    scope = $scope
    plugins = @( $pluginResults | ForEach-Object {
        [pscustomobject]@{
            plugin = $_.Plugin; folder = $_.Folder; version = $_.Version; unreleased = $_.Unreleased; result = $_.Result; notes = $_.Notes
            dlls = @( $_.Dlls | ForEach-Object { [pscustomobject]@{ project = $_.Project; path = $_.Path; succeeded = $_.Succeeded; errors = $_.Errors } } )
            pages = @( $_.Pages | ForEach-Object { [pscustomobject]@{ page = $_.Page; result = $_.Result; errors = $_.Errors } } )
            obsoleteUsages = $_.Obsolete; dependencyConflicts = $_.Conflicts; missing = $_.Missing; obsidian = $_.Obsidian
        }
    } )
    notTested = @( $untestedPlugins | ForEach-Object { [pscustomobject]@{ plugin = $_.Name; version = $_.Version; status = $_.Status; missing = $_.Missing } } )
    dependencyConflicts = $dependencyConflicts
} | ConvertTo-Json -Depth 6 | Set-Content -Encoding UTF8 ( Join-Path $resultsDir "$reportName.json" )

# Keep each run: the files above are overwritten by the next run against this Rock version.
$historyDir = Join-Path $resultsDir "history"
New-Item -ItemType Directory -Force $historyDir | Out-Null
$stamp = $started.ToString( "yyyy-MM-dd_HHmmss" )
Copy-Item $reportPath ( Join-Path $historyDir "$( $reportName )_$stamp.md" )
Copy-Item ( Join-Path $resultsDir "$reportName.json" ) ( Join-Path $historyDir "$( $reportName )_$stamp.json" )

Write-Step "Summary"
$lines | Select-Object -Skip 7 -First 7 | ForEach-Object { Write-Host $_ }
$summaryLines | Select-Object -Skip 3 | ForEach-Object { Write-Host ( $_ -replace '\[([^\]]+)\]\(#[^)]*\)', '$1' ) }
Write-Host ""
Write-Host "Report: $reportPath"

if ( $failing -gt 0 -or $incomplete -gt 0 -or $otherConflicts.Count -gt 0 ) {
    exit 1
}
exit 0

#endregion
