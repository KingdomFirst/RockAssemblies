<#
.SYNOPSIS
    Builds a new RockShop package version from the previous package's file list.

.DESCRIPTION
    Uses the newest existing version of the plugin under Rock Packages as the manifest:

    - Every file in its content\ folder is re-sourced from the current code. Block files come
      from the RockBlocks working tree; dlls come from the -DllSource folders (build output).
    - install\ and uninstall\ are copied unchanged. Edit them afterwards if the file list
      changed (see the warnings this script prints).
    - The new folder is named in the previous folder's style ("v2.3", "2_7", "1.3"), and the
      .plugin zip in the previous zip's style, with the version swapped.

    The zip is written with forward-slash entry names and explicit directory entries, the same
    shape as the existing packages. Compress-Archive in Windows PowerShell 5.1 writes
    backslashes, which is why it is not used.

    Refuses to overwrite an existing version folder.

.EXAMPLE
    .\New-PluginPackage.ps1 -Plugin "Advanced Check-in Monitor" -Version 2.3

.EXAMPLE
    .\New-PluginPackage.ps1 -Plugin "Microsoft 365 Utilities" -Version 1.3 `
        -DllSource C:\KFSRepo\Rock\KFSRockAssemblies\rocks.kfs.Microsoft365Utilities\bin\Release `
        -Exclude 'bin\Microsoft.Identity.Client.dll','bin\Microsoft.IdentityModel.Abstractions.dll'
#>
[CmdletBinding()]
param(
    [Parameter( Mandatory = $true )]
    [string] $Plugin,

    # Major.minor (or longer) of the new package. For a plugin with a KFS dll, this must match
    # that dll's AssemblyVersion major.minor.
    [Parameter( Mandatory = $true )]
    [string] $Version,

    # Folders to take dlls from, searched in order. Usually the plugin project's bin\Release.
    [string[]] $DllSource = @(),

    # Paths relative to content\ to stop shipping (e.g. a dependency Rock now provides).
    [string[]] $Exclude = @(),

    # Extra files to ship, as @{ 'Plugins\rocks_kfs\Domain\New.ascx' = 'C:\path\to\New.ascx' }.
    [hashtable] $Add = @{},

    [string] $PackagesRoot = "$env:USERPROFILE\OneDrive - Kingdom First Solutions Inc\Shared Documents - Kingdom First Solutions Inc. Team Site\Rock Packages",
    [string] $BlocksRepo = "C:\KFSRepo\Rock\KFSRockBlocks"
)

$ErrorActionPreference = "Stop"
. "$PSScriptRoot\PackageCommon.ps1"
Add-Type -AssemblyName System.IO.Compression, System.IO.Compression.FileSystem

$excludedPlugins = Get-ExcludedPlugins
if ( $excludedPlugins.ContainsKey( $Plugin ) ) {
    throw "'$Plugin' is not a KFS plugin: $( $excludedPlugins[ $Plugin ] ) (excluded-plugins.txt)"
}

$pluginFolder = Join-Path $PackagesRoot $Plugin
$previous = Get-LatestPackageVersion -PluginFolder $pluginFolder
$newVersion = ConvertTo-PackageVersion $Version
if ( $newVersion -le $previous.Version ) { throw "Version $Version is not newer than the latest package ($( $previous.Name ))." }

# Name the folder in the previous folder's style.
$prefix = if ( $previous.Name -match '^[vV]' ) { $previous.Name.Substring( 0, 1 ) } else { "" }
$separator = if ( $previous.Name -match '_' ) { "_" } else { "." }
$folderName = $prefix + ( $Version -replace '[._]', $separator )
$target = Join-Path $pluginFolder $folderName
if ( Test-Path $target ) { throw "$target already exists. Remove it yourself if you mean to rebuild it." }

# Re-source every content file.
$contentRoot = Join-Path $previous.PackageRoot "content"
$plan = @()
foreach ( $file in Get-ChildItem $contentRoot -Recurse -File ) {
    $relative = $file.FullName.Substring( $contentRoot.Length + 1 )
    if ( $Exclude | Where-Object { $_ -ieq $relative } ) { Write-Host "Excluding $relative"; continue }

    $source = Resolve-ContentSource -Relative $relative -BlocksRepo $BlocksRepo
    $from = $null
    switch ( $source.Kind ) {
        "Block" { $from = Join-Path $BlocksRepo $source.RepoPath }
        { $_ -in "KfsDll", "Dll" } {
            $from = $DllSource | ForEach-Object { Join-Path $_ $source.FileName } | Where-Object { Test-Path $_ } | Select-Object -First 1
            if ( -not $from ) { throw "No -DllSource folder contains $( $source.FileName ) (needed for $relative)." }
        }
        default { throw "Don't know where $relative comes from. Pass it with -Add, or -Exclude it." }
    }
    if ( -not ( Test-Path $from ) ) { throw "Source for $relative not found: $from" }
    $plan += [pscustomobject]@{ Relative = $relative; From = $from }
}
foreach ( $key in $Add.Keys ) {
    if ( -not ( Test-Path $Add[$key] ) ) { throw "Source for $key not found: $( $Add[$key] )" }
    $plan += [pscustomobject]@{ Relative = $key; From = $Add[$key] }
}

# Lay out the folder.
New-Item -ItemType Directory $target | Out-Null
foreach ( $item in $plan ) {
    $dest = Join-Path ( Join-Path $target "content" ) $item.Relative
    New-Item -ItemType Directory -Force ( Split-Path $dest ) | Out-Null
    Copy-Item $item.From $dest
}
foreach ( $section in "install", "uninstall" ) {
    $sectionPath = Join-Path $previous.PackageRoot $section
    if ( Test-Path $sectionPath ) { Copy-Item $sectionPath ( Join-Path $target $section ) -Recurse }
}

# Name the zip in the previous zip's style.
$pluginFileName = $null
if ( $previous.PluginFile ) {
    $old = $previous.PluginFile.Name
    $v = $previous.Version
    $oldParts = @( $v.Major, $v.Minor ) + @( $v.Build, $v.Revision | Where-Object { $_ -ge 0 } )
    foreach ( $sep in "_", "." ) {
        # Longest form first so "1_8_5" is replaced whole rather than as "1_8".
        for ( $n = $oldParts.Count; $n -ge 2 -and -not $pluginFileName; $n-- ) {
            $token = $oldParts[0..( $n - 1 )] -join $sep
            $pattern = '(?<![\d])' + [regex]::Escape( $token ) + '(?![\d])'
            if ( $old -match $pattern ) { $pluginFileName = [regex]::Replace( $old, $pattern, ( $Version -replace '[._]', $sep ) ) }
        }
    }
}
if ( -not $pluginFileName ) { $pluginFileName = ( $Plugin -replace '[^A-Za-z0-9]', '' ) + "_v" + ( $Version -replace '[._]', '_' ) + ".plugin" }
$zipPath = Join-Path $target $pluginFileName

# Zip: install/, uninstall/, content/ with forward slashes and directory entries.
$zip = [System.IO.Compression.ZipFile]::Open( $zipPath, [System.IO.Compression.ZipArchiveMode]::Create )
try {
    foreach ( $section in "install", "uninstall", "content" ) {
        $sectionPath = Join-Path $target $section
        if ( -not ( Test-Path $sectionPath ) ) { continue }
        $items = @( Get-Item $sectionPath ) + @( Get-ChildItem $sectionPath -Recurse | Sort-Object FullName )
        foreach ( $item in $items ) {
            $entryName = $item.FullName.Substring( $target.Length + 1 ) -replace '\\', '/'
            if ( $item.PSIsContainer ) { $zip.CreateEntry( "$entryName/" ) | Out-Null }
            else { [System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile( $zip, $item.FullName, $entryName, [System.IO.Compression.CompressionLevel]::Optimal ) | Out-Null }
        }
    }
}
finally { $zip.Dispose() }

# Report.
Write-Host ""
Write-Host "Created $target"
Write-Host "  $pluginFileName"
foreach ( $item in $plan ) {
    $line = "  content\$( $item.Relative )"
    if ( $item.Relative -like '*.dll' ) {
        try { $line += "  [$( [Reflection.AssemblyName]::GetAssemblyName( $item.From ).Version )]" } catch { }
    }
    Write-Host $line
}

# Flag install/uninstall lists that no longer match what ships.
$shipped = $plan | ForEach-Object { $_.Relative }
$uninstallList = Join-Path $target "uninstall\deletefile.lst"
if ( Test-Path $uninstallList ) {
    $listed = Get-Content $uninstallList | Where-Object { $_.Trim() } | ForEach-Object { $_.Trim() }
    foreach ( $entry in $listed ) {
        if ( -not ( $shipped | Where-Object { $_ -ieq $entry } ) ) { Write-Warning "uninstall\deletefile.lst lists $entry, which this package does not ship." }
    }
    foreach ( $entry in $Add.Keys ) {
        if ( -not ( $listed | Where-Object { $_ -ieq $entry } ) ) { Write-Warning "New file $entry is not in uninstall\deletefile.lst." }
    }
}
foreach ( $entry in $Exclude ) { Write-Warning "Excluded $entry. Decide whether install\deletefile.lst should remove it from existing sites. Never list a dll Rock itself ships." }
