<#
.SYNOPSIS
    Lists what has changed in a RockShop plugin since its last published package.

.DESCRIPTION
    Reads the newest version folder of a plugin under Rock Packages and, for every file it
    ships, works out the baseline the package was built from and the commits since then:

    - Block files (content\Plugins\rocks_kfs\...): the packaged file's git blob is matched to
      the RockBlocks commit that produced it. Commits after that commit are reported.
    - KFS assemblies (content\bin\rocks.kfs.*.dll): the build date is decoded from the
      assembly version (AssemblyVersion "x.y.*" stamps days since 2000-01-01), and commits to
      the matching rocks.kfs.* project after that date are reported.

    Each commit is mapped to the pull request that merged it ("Merge pull request #N" or a
    squash subject ending "(#N)"), or flagged as not yet merged to master. Uncommitted
    changes to the same paths are reported too.

    Read-only. Never changes either repository or the packages folder.

.EXAMPLE
    .\Find-PackageChanges.ps1 -Plugin "Advanced Check-in Monitor"
#>
[CmdletBinding()]
param(
    [Parameter( Mandatory = $true )]
    [string] $Plugin,

    [string] $PackagesRoot = "$env:USERPROFILE\OneDrive - Kingdom First Solutions Inc\Shared Documents - Kingdom First Solutions Inc. Team Site\Rock Packages",
    [string] $BlocksRepo = "C:\KFSRepo\Rock\KFSRockBlocks",
    [string] $AssembliesRepo = "C:\KFSRepo\Rock\KFSRockAssemblies",
    [string] $MasterBranch = "master"
)

$ErrorActionPreference = "Stop"
. "$PSScriptRoot\PackageCommon.ps1"

$excludedPlugins = Get-ExcludedPlugins
if ( $excludedPlugins.ContainsKey( $Plugin ) ) {
    throw "'$Plugin' is not a KFS plugin: $( $excludedPlugins[ $Plugin ] ) (excluded-plugins.txt)"
}

function Get-MergingPr( [string] $repo, [string] $sha ) {
    $subject = git -C $repo log -1 --format=%s $sha
    if ( $subject -match '\(#(\d+)\)\s*$' ) { return "#$( $Matches[1] )" }

    git -C $repo merge-base --is-ancestor $sha $MasterBranch 2>$null
    if ( $LASTEXITCODE -ne 0 ) { return "not merged to $MasterBranch (current branch)" }

    # The oldest mainline commit that contains $sha is the one that brought it in. (--ancestry-path
    # combined with --first-parent does not find it on current git.)
    $since = git -C $repo log -1 --format=%cI $sha
    foreach ( $mainline in git -C $repo rev-list --first-parent --reverse --since=$since $MasterBranch ) {
        git -C $repo merge-base --is-ancestor $sha $mainline 2>$null
        if ( $LASTEXITCODE -ne 0 ) { continue }
        $subject = git -C $repo log -1 --format=%s $mainline
        if ( $subject -match 'Merge pull request #(\d+)' -or $subject -match '\(#(\d+)\)\s*$' ) { return "#$( $Matches[1] )" }
        return "direct commit on $MasterBranch"
    }
    return "direct commit on $MasterBranch"
}

function Write-Commits( [string] $repo, [string[]] $shas ) {
    if ( -not $shas ) { Write-Host "    (no commits since the package)"; return }
    foreach ( $sha in $shas ) {
        $line = git -C $repo log -1 --format="%h %ad %s" --date=short $sha
        Write-Host ( "    {0}   [PR {1}]" -f $line, ( Get-MergingPr $repo $sha ) )
    }
}

function Write-WorkingChanges( [string] $repo, [string] $path ) {
    $status = git -C $repo status --short -- $path
    if ( $status ) { $status | ForEach-Object { Write-Host "    UNCOMMITTED: $_" } }
}

$previous = Get-LatestPackageVersion -PluginFolder ( Join-Path $PackagesRoot $Plugin )
Write-Host "Plugin:           $Plugin"
Write-Host "Latest package:   $( $previous.Name ) ($( $previous.Version ))"
Write-Host "Package root:     $( $previous.PackageRoot )"
Write-Host ""

$contentRoot = Join-Path $previous.PackageRoot "content"
foreach ( $file in Get-ChildItem $contentRoot -Recurse -File ) {
    $relative = $file.FullName.Substring( $contentRoot.Length + 1 )
    $source = Resolve-ContentSource -Relative $relative -BlocksRepo $BlocksRepo

    if ( $source.Kind -eq "Block" ) {
        Write-Host "[block] $relative"
        $repoPath = $source.RepoPath
        if ( -not ( Test-Path ( Join-Path $BlocksRepo $repoPath ) ) ) {
            Write-Host "    NOT IN REPO at $repoPath (moved, renamed or archived?)"
            continue
        }

        $blob = git -C $BlocksRepo hash-object $file.FullName
        # Newest commit in HEAD's history whose version of the file is exactly what was packaged.
        $baseline = $null
        foreach ( $sha in git -C $BlocksRepo log --format=%H HEAD -- $repoPath ) {
            $atCommit = git -C $BlocksRepo rev-parse "${sha}:$repoPath" 2>$null
            if ( $atCommit -eq $blob ) { $baseline = $sha; break }
        }

        if ( $baseline ) {
            Write-Host ( "    baseline: {0}" -f ( git -C $BlocksRepo log -1 --format="%h %ad %s" --date=short $baseline ) )
            Write-Commits $BlocksRepo ( git -C $BlocksRepo log --format=%H "$baseline..HEAD" -- $repoPath )
        }
        else {
            $since = $file.LastWriteTime.ToString( "yyyy-MM-dd" )
            Write-Host "    baseline: packaged file matches no commit; falling back to its date ($since)"
            Write-Commits $BlocksRepo ( git -C $BlocksRepo log --format=%H --since=$since HEAD -- $repoPath )
        }
        Write-WorkingChanges $BlocksRepo $repoPath
    }
    elseif ( $source.Kind -eq "KfsDll" ) {
        $version = [Reflection.AssemblyName]::GetAssemblyName( $file.FullName ).Version
        $built = ( Get-Date "2000-01-01" ).AddDays( $version.Build ).AddSeconds( $version.Revision * 2 )
        $project = $file.BaseName
        Write-Host "[dll]   $relative  version $version, built $( $built.ToString( 'yyyy-MM-dd HH:mm' ) )"
        if ( -not ( Test-Path ( Join-Path $AssembliesRepo $project ) ) ) {
            Write-Host "    PROJECT NOT FOUND: $AssembliesRepo\$project"
            continue
        }
        Write-Commits $AssembliesRepo ( git -C $AssembliesRepo log --format=%H --since=$( $built.ToString( 's' ) ) HEAD -- $project )
        Write-WorkingChanges $AssembliesRepo $project
        $info = Get-ChildItem ( Join-Path $AssembliesRepo $project ) -Recurse -Filter AssemblyInfo.cs | Select-Object -First 1
        if ( $info -and ( Get-Content $info.FullName -Raw ) -match 'AssemblyVersion\(\s*"([^"]+)"' ) {
            Write-Host "    working-tree AssemblyVersion: $( $Matches[1] )"
        }
    }
    else {
        Write-Host "[other] $relative  (third-party or non-KFS file: check manually whether it changed)"
    }
}
