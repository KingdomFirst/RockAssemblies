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
# Maps KFS RockShop plugins to the code in our repos that they ship. Dot-source only.

<#
    10/22/2026 - CLAUDE

    A KFS plugin is defined by its RockShop package in the shared Rock Packages folder: the
    files its newest version ships are the plugin's code. The end-to-end suite is organized
    by those same plugins, and the preflight now tests and reports by them too, so code that
    no package ships is not tested. "Newest version" comes from the kfs-package skill's
    Get-LatestPackageVersion, so packaging and testing agree on what a plugin contains.

    Reason: One definition of "a plugin" for packaging, functional tests and the preflight.
#>

. ( Join-Path $PSScriptRoot "..\.guardrails\skills\kfs-package\PackageCommon.ps1" )

# Where each kind of shipped file lives in a Rock clone set up by CreateLinks.bat.
$script:catalogProjectRoots = @( "KFSRockAssemblies", "RockAttendedCheckin" )
$script:catalogBlockRoots = @( "rocks_kfs", "cc_newspring" )

# Indexes every C# project in our repos by the assembly it builds. Archived projects are kept
# apart so a package that ships one can be reported as archived rather than missing.
function Get-ProjectIndex( [string] $RockRoot ) {
    $active = @{}
    $archived = @{}

    foreach ( $root in $script:catalogProjectRoots ) {
        $rootPath = Join-Path $RockRoot $root
        if ( -not ( Test-Path $rootPath ) ) {
            continue
        }

        foreach ( $csproj in Get-ChildItem $rootPath -Recurse -Filter *.csproj -ErrorAction SilentlyContinue |
            Where-Object { $_.FullName -notmatch '\\(node_modules|obj|bin)\\' } ) {
            $text = Get-Content $csproj.FullName -Raw
            $assembly = if ( $text -match '<AssemblyName>\s*([^<\s]+)\s*</AssemblyName>' ) { $Matches[ 1 ] } else { $csproj.BaseName }
            $relative = $csproj.FullName.Substring( $RockRoot.Length + 1 )

            if ( $relative -match '\\ZZZ_Archive\\' ) {
                $archived[ $assembly ] = $relative
            }
            elseif ( -not $active.ContainsKey( $assembly ) ) {
                $active[ $assembly ] = $relative
            }
        }
    }

    return [pscustomobject]@{ Active = $active; Archived = $archived }
}

# Finds a plugin's newest package and lists the files under its content\ folder, as paths
# relative to content\ (e.g. "bin\rocks.kfs.X.dll"). Most packages are kept unpacked beside
# their .plugin file; one kept only as a .plugin file is read from the zip.
function Get-PluginPackage( [string] $PluginFolder ) {
    $package = $null
    try {
        $package = Get-LatestPackageVersion -PluginFolder $PluginFolder
    }
    catch {
        if ( Test-Path ( Join-Path $PluginFolder "content" ) ) {
            $package = [pscustomobject]@{ Name = "(unversioned)"; Version = $null; PackageRoot = $PluginFolder }
        }
    }

    if ( $package ) {
        $contentRoot = Join-Path $package.PackageRoot "content"
        $files = @( Get-ChildItem $contentRoot -Recurse -File | ForEach-Object { $_.FullName.Substring( $contentRoot.Length + 1 ) } )
        return [pscustomobject]@{ Name = $package.Name; Files = $files }
    }

    $pluginFile = Get-ChildItem $PluginFolder -Recurse -Filter *.plugin -File | Sort-Object LastWriteTime | Select-Object -Last 1
    if ( -not $pluginFile ) {
        return $null
    }

    Add-Type -AssemblyName System.IO.Compression.FileSystem
    $zip = [System.IO.Compression.ZipFile]::OpenRead( $pluginFile.FullName )
    try {
        $files = @( $zip.Entries | Where-Object { $_.Name -and $_.FullName -match '^content/' } |
            ForEach-Object { ( $_.FullName -replace '^content/', '' ).Replace( '/', '\' ) } )
    }
    finally {
        $zip.Dispose()
    }
    $name = if ( $pluginFile.BaseName -match '(\d+(?:[._]\d+)+)' ) { $Matches[ 1 ] } else { $pluginFile.Name }
    return [pscustomobject]@{ Name = $name; Files = $files }
}

<#
.SYNOPSIS
    Lists every plugin in the Rock Packages folder with the code it ships.

.DESCRIPTION
    For each plugin folder (not folders starting with "_", nor those listed in the kfs-package
    skill's excluded-plugins.txt), reads the newest
    package's content\ and sorts each file into:

      Projects      bin\<assembly>.dll built by a project in our repos (KFSRockAssemblies or
                    RockAttendedCheckin), as a path relative to the Rock clone.
      Pages         .ascx / .aspx / .ashx under Plugins\rocks_kfs or Plugins\cc_newspring,
                    present in the clone; compiled with their code-behind.
      Obsidian      built Obsidian files (.obs.js). Not compiled by the preflight.
      Dependencies  other bin\*.dll (third-party libraries the package ships).
      External      files under another Plugins\ folder (a vendor's code, not ours).
      Missing       shipped files whose source is not on this branch (archived or removed).

    Status: "Active" when the plugin ships at least one project or page from our repos;
    "Obsidian only"; "No KFS code" (e.g. a vendor plugin); or "No package".
#>
function Get-PluginCatalog {
    param(
        [Parameter( Mandatory = $true )] [string] $RockRoot,
        [Parameter( Mandatory = $true )] [string] $PackagesRoot
    )

    if ( -not ( Test-Path $PackagesRoot ) ) {
        throw "Rock Packages folder not found: $PackagesRoot"
    }

    $projects = Get-ProjectIndex $RockRoot
    $pluginsRoot = Join-Path $RockRoot "RockWeb\Plugins"
    $blockRootPattern = ( $script:catalogBlockRoots | ForEach-Object { [regex]::Escape( $_ ) } ) -join "|"

    # Folders that are not KFS plugins (excluded-plugins.txt, e.g. no longer our IP) are skipped entirely.
    $excluded = Get-ExcludedPlugins

    foreach ( $folder in Get-ChildItem $PackagesRoot -Directory | Where-Object { $_.Name -notlike "_*" -and -not $excluded.ContainsKey( $_.Name ) } | Sort-Object Name ) {
        $plugin = [pscustomobject]@{
            Name         = ( $folder.Name -replace '\s*\(Unreleased\)\s*$', '' )
            Folder       = $folder.Name
            Unreleased   = $folder.Name -match '\(Unreleased\)'
            Version      = $null
            Status       = "No package"
            Projects     = @()
            Pages        = @()
            Obsidian     = @()
            Dependencies = @()
            External     = @()
            Missing      = @()
        }

        $package = Get-PluginPackage $folder.FullName
        if ( -not $package ) {
            $plugin
            continue
        }
        $plugin.Version = $package.Name

        foreach ( $relative in $package.Files ) {

            if ( $relative -match '^bin\\(?<asm>[^\\]+)\.dll$' ) {
                $assembly = $Matches.asm
                if ( $projects.Active.ContainsKey( $assembly ) ) {
                    $plugin.Projects += $projects.Active[ $assembly ]
                }
                elseif ( $projects.Archived.ContainsKey( $assembly ) ) {
                    $plugin.Missing += "$relative (project archived: $( $projects.Archived[ $assembly ] ))"
                }
                elseif ( $assembly -match '^(rocks\.kfs|cc\.newspring)\.' ) {
                    $plugin.Missing += "$relative (no project builds it on this branch)"
                }
                else {
                    $plugin.Dependencies += "$assembly.dll"
                }
            }
            elseif ( $relative -match "^Plugins\\(?:$blockRootPattern)\\.+\.obs\.js$" ) {
                $plugin.Obsidian += $relative
            }
            elseif ( $relative -match "^Plugins\\(?:$blockRootPattern)\\.+\.(ascx|aspx|ashx)$" ) {
                if ( Test-Path ( Join-Path $pluginsRoot $relative.Substring( "Plugins\".Length ) ) ) {
                    $plugin.Pages += $relative
                }
                else {
                    $plugin.Missing += "$relative (not on this branch)"
                }
            }
            elseif ( $relative -match '^Plugins\\' -and $relative -notmatch "^Plugins\\(?:$blockRootPattern)\\" ) {
                $plugin.External += $relative
            }
        }

        $plugin.Projects = @( $plugin.Projects | Sort-Object -Unique )
        $plugin.Pages = @( $plugin.Pages | Sort-Object -Unique )
        $plugin.Dependencies = @( $plugin.Dependencies | Sort-Object -Unique )

        $plugin.Status = if ( $plugin.Projects.Count + $plugin.Pages.Count -gt 0 ) { "Active" }
            elseif ( $plugin.Obsidian.Count -gt 0 ) { "Obsidian only" }
            else { "No KFS code" }

        $plugin
    }
}
