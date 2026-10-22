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
    Rebuilds RockShop plugin packages from the current branch, for manual deployment to a beta
    site, using the released packages in the shared "Rock Packages" folder as templates.

.DESCRIPTION
    For each plugin in $plugins, the newest released version folder (the one whose .plugin file
    is newest) is the template. The output mirrors it exactly:

      <OutRoot>\<Plugin folder>\<template version>-<Suffix>\
          content\...                   every file the release ships, refreshed from the branch
          install\  uninstall\          copied as released (deletefile.lst, run.sql, ...)
          <release .plugin name>_<Suffix>.plugin

    Content files are refreshed from the branch: bin\*.dll from the KFSRockAssemblies project
    that builds it (bin\Debug), Plugins\rocks_kfs\* from KFSRockBlocks. Files in $rockOwned are
    left out even if a release shipped them: they belong to Rock, and shipping them overwrites
    Rock's copy (Microsoft365Utilities 1.0.1-1.2 did this with MSAL).

    Build the DLLs first against the target Rock clone, normally with
    Invoke-CompatPreflight.ps1 (the projects' post-build steps copy from bin\Debug, so a Release
    build fails). The script stops before writing anything if a content file cannot be
    resolved, or if a packaged file has uncommitted changes.

.PARAMETER RockPackagesRoot
    The shared "Rock Packages" folder (read only; nothing is written there).

.PARAMETER OutRoot
    Where to write the packages. Defaults to results\packages\<Suffix> (git-ignored).

.PARAMETER Suffix
    Appended to the version folder and .plugin name. A placeholder, not a release version.

.PARAMETER Plugins
    Rock Packages folder names of the plugins to package. Defaults to $defaultPlugins.

.PARAMETER Version
    A real version for the new package (e.g. "v1.1"). Replaces the release's version in the
    version folder and .plugin name, instead of appending -Suffix.

.PARAMETER AllowUncommitted
    Package files with uncommitted changes (listed as a warning) instead of stopping. For a
    beta deployment of a fix that is not committed yet.

.EXAMPLE
    .\Invoke-CompatPreflight.ps1 -RockRoot C:\KFSRepo\Rock\Rock17
    .\New-CompatPackages.ps1 -OutRoot "C:\...\Claude\packages\Rock17.8-compat-beta"

.EXAMPLE
    .\New-CompatPackages.ps1 -Plugins "KFS Fundraising Progress" -Version v1.1 -OutRoot "C:\...\Claude\packages\beta"
#>
[CmdletBinding()]
param(
    [string] $RockPackagesRoot = ( Join-Path $env:USERPROFILE "OneDrive - Kingdom First Solutions Inc\Shared Documents - Kingdom First Solutions Inc. Team Site\Rock Packages" ),
    [string] $OutRoot,
    [string] $Suffix = "compat-beta",
    [string[]] $Plugins,
    [string] $Version,
    [switch] $AllowUncommitted
)

$ErrorActionPreference = "Stop"
Add-Type -AssemblyName System.IO.Compression, System.IO.Compression.FileSystem

# RockShop plugins (folder names in Rock Packages) whose released files the Rock 20
# compatibility fixes changed. Edit for the next round.
$defaultPlugins = @(
    "Advanced Check-in Monitor",
    "Microsoft 365 Utilities",
    "Person Attributes Form Advanced"
)
$Plugins = if ( $Plugins ) { $Plugins } else { $defaultPlugins }

# Rock's own assemblies: never shipped in a plugin package.
$rockOwned = @( "Microsoft.Identity.Client.dll", "Microsoft.IdentityModel.Abstractions.dll" )

# This script lives in KFSRockAssemblies\rocks.kfs.Tests.Compile; the repos sit beside KFSRockAssemblies.
$repoRoot = Split-Path ( Split-Path $PSScriptRoot -Parent ) -Parent
$assemblies = Join-Path $repoRoot "KFSRockAssemblies"
$blocks = Join-Path $repoRoot "KFSRockBlocks"

if ( -not ( Test-Path $RockPackagesRoot ) ) {
    throw "Rock Packages folder not found: $RockPackagesRoot"
}
if ( -not $OutRoot ) {
    $OutRoot = Join-Path $PSScriptRoot "results\packages\$Suffix"
}

# Maps a released content path (e.g. "bin\rocks.kfs.X.dll") to its source on the branch.
# Returns $null if it cannot be found. Tracked is what the uncommitted-change check looks at.
function Resolve-ContentFile( [string] $relative ) {
    if ( $relative -like "bin\*" ) {
        $name = Split-Path $relative -Leaf
        $built = @( Get-ChildItem $assemblies -Directory | Where-Object Name -ne "ZZZ_Archive" |
            ForEach-Object { Join-Path $_.FullName "bin\Debug\$name" } | Where-Object { Test-Path $_ } )
        if ( $built.Count -eq 0 ) {
            return $null
        }
        # Prefer the project that produces the DLL (rocks.kfs.X.dll -> rocks.kfs.X); third-party
        # DLLs come from the first project that copies them.
        $own = $built | Where-Object { ( Split-Path ( Split-Path ( Split-Path $_ -Parent ) -Parent ) -Leaf ) -eq [IO.Path]::GetFileNameWithoutExtension( $name ) } | Select-Object -First 1
        $source = if ( $own ) { $own } else { $built[ 0 ] }
        return [pscustomobject]@{ Source = $source; Repo = $assemblies; Tracked = ( Split-Path ( Split-Path ( Split-Path $source -Parent ) -Parent ) ) }
    }

    if ( $relative -like "Plugins\rocks_kfs\*" ) {
        $source = Join-Path $blocks $relative.Substring( "Plugins\rocks_kfs\".Length )
        if ( -not ( Test-Path $source ) ) {
            return $null
        }
        return [pscustomobject]@{ Source = $source; Repo = $blocks; Tracked = $source }
    }

    return $null
}

# Plan every package before writing anything.
$plans = @()
$problems = @()
foreach ( $plugin in $plugins ) {
    $pluginDir = Join-Path $RockPackagesRoot $plugin
    $release = Get-ChildItem $pluginDir -Recurse -Filter *.plugin |
        Where-Object { Test-Path ( Join-Path $_.DirectoryName "content" ) } |
        Sort-Object LastWriteTime -Descending | Select-Object -First 1
    if ( -not $release ) {
        $problems += "${plugin}: no released version with a content folder"
        continue
    }

    $templateDir = $release.DirectoryName
    $contentDir = Join-Path $templateDir "content"
    $files = @()
    $skipped = @()
    foreach ( $file in Get-ChildItem $contentDir -Recurse -File ) {
        $relative = $file.FullName.Substring( $contentDir.Length + 1 )
        if ( ( Split-Path $relative -Leaf ) -in $rockOwned ) {
            $skipped += $relative
            continue
        }
        $resolved = Resolve-ContentFile $relative
        if ( -not $resolved ) {
            $problems += "${plugin}: cannot find a branch source for content\$relative"
            continue
        }
        $files += [pscustomobject]@{ Relative = $relative; Source = $resolved.Source; Repo = $resolved.Repo; Tracked = $resolved.Tracked }
    }

    $versionPath = $templateDir.Substring( $pluginDir.Length ).TrimStart( "\" )
    $releaseName = [IO.Path]::GetFileNameWithoutExtension( $release.Name )
    $releaseVersion = ( $versionPath -split '\\' )[ 0 ]
    $pluginName = if ( -not $Version ) {
        "${releaseName}_$( $Suffix -replace '[^A-Za-z0-9]', '_' ).plugin"
    }
    elseif ( $releaseName.Contains( $releaseVersion ) ) {
        "$( $releaseName.Replace( $releaseVersion, $Version ) ).plugin"
    }
    elseif ( $releaseName.Contains( $releaseVersion.Replace( ".", "_" ) ) ) {
        # Some releases spell the version with underscores (Eventbrite_1_8.plugin in folder 1.8).
        "$( $releaseName.Replace( $releaseVersion.Replace( ".", "_" ), $Version.Replace( ".", "_" ) ) ).plugin"
    }
    else {
        "$releaseName-$Version.plugin"
    }
    $plans += [pscustomobject]@{
        Plugin = $plugin
        Template = $templateDir
        TemplateVersion = $versionPath
        PluginName = $pluginName
        Files = $files
        Skipped = $skipped
    }
}

if ( $problems.Count -gt 0 ) {
    throw "Cannot build packages:`n  " + ( $problems -join "`n  " )
}

# Fail if a packaged file (or a DLL's project source, ignoring bin/obj) has uncommitted changes.
$dirty = @()
foreach ( $group in $plans.Files | Group-Object Repo ) {
    $paths = @( $group.Group | ForEach-Object { $_.Tracked } | Sort-Object -Unique )
    $dirty += @( git -C $group.Name status --porcelain -- $paths | Where-Object { $_ -notmatch '(^|[\\/])(bin|obj)[\\/]' -and $_ -notmatch '^\?\?' } )
}
if ( $dirty.Count -gt 0 ) {
    if ( -not $AllowUncommitted ) {
        throw "Uncommitted changes in packaged files; commit or stash them first (or pass -AllowUncommitted):`n  " + ( $dirty -join "`n  " )
    }
    Write-Warning ( "Packaging uncommitted changes:`n  " + ( $dirty -join "`n  " ) )
}

$summary = @()
foreach ( $plan in $plans ) {
    # Keep the release's version folder path (e.g. "v2.6" or "v1.0\rocks_kfs"), with its first
    # segment suffixed, or replaced by -Version.
    $segments = $plan.TemplateVersion -split '\\'
    $segments[ 0 ] = if ( $Version ) { $Version } else { "$( $segments[ 0 ] )-$Suffix" }
    $dir = Join-Path ( Join-Path $OutRoot $plan.Plugin ) ( $segments -join "\" )
    if ( Test-Path $dir ) {
        Get-ChildItem $dir -Force | Remove-Item -Recurse -Force
    }

    # Mirror the release: every folder (even empty ones) plus install\ and uninstall\ as shipped.
    foreach ( $folder in Get-ChildItem $plan.Template -Recurse -Directory ) {
        New-Item -ItemType Directory -Force ( Join-Path $dir $folder.FullName.Substring( $plan.Template.Length + 1 ) ) | Out-Null
    }
    foreach ( $file in Get-ChildItem $plan.Template -Recurse -File | Where-Object { $_.Extension -ne ".plugin" -and -not $_.FullName.StartsWith( ( Join-Path $plan.Template "content\" ) ) } ) {
        Copy-Item $file.FullName ( Join-Path $dir $file.FullName.Substring( $plan.Template.Length + 1 ) )
    }
    foreach ( $file in $plan.Files ) {
        Copy-Item $file.Source ( Join-Path $dir "content\$( $file.Relative )" )
    }

    # Zip with forward-slash entries, including folder entries, as the released packages do.
    $pluginPath = Join-Path $dir $plan.PluginName
    $zip = [IO.Compression.ZipFile]::Open( $pluginPath, [IO.Compression.ZipArchiveMode]::Create )
    try {
        foreach ( $item in Get-ChildItem $dir -Recurse | Where-Object { $_.FullName -ne $pluginPath } | Sort-Object FullName ) {
            $entryName = $item.FullName.Substring( $dir.Length + 1 ).Replace( "\", "/" )
            if ( $item.PSIsContainer ) {
                $zip.CreateEntry( "$entryName/" ) | Out-Null
            }
            else {
                [IO.Compression.ZipFileExtensions]::CreateEntryFromFile( $zip, $item.FullName, $entryName, [IO.Compression.CompressionLevel]::Optimal ) | Out-Null
            }
        }
    }
    finally {
        $zip.Dispose()
    }

    $summary += [pscustomobject]@{
        Plugin = $plan.Plugin
        Template = $plan.TemplateVersion
        Package = $pluginPath.Substring( $OutRoot.Length + 1 )
        Files = $plan.Files.Count
        LeftOut = $plan.Skipped -join ", "
    }
}

$summary
Write-Host ""
Write-Host "$( $summary.Count ) packages written to $OutRoot"
