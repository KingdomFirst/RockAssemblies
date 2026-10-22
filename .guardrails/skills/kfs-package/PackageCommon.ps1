# Shared helpers for Find-PackageChanges.ps1 and New-PluginPackage.ps1. Dot-source only.

# Parses a version folder name ("v2.2", "2_10", "1.8.7 for Rock v12", "1.2_v15") into a
# [version], or $null when the folder is not a version (Screenshots, Documentation, ...).
function ConvertTo-PackageVersion( [string] $folderName ) {
    $normalized = ( $folderName -replace '^[vV]', '' ) -replace '_', '.'
    if ( $normalized -notmatch '^(\d+(\.\d+)*)' ) { return $null }
    $parts = $Matches[1].Split( '.' )
    while ( $parts.Count -lt 2 ) { $parts += "0" }
    return [version]( $parts[0..( [Math]::Min( 3, $parts.Count - 1 ) )] -join '.' )
}

# Returns the newest version folder of a plugin: its name, parsed version, and the folder that
# directly holds content\, install\ and uninstall\ (the version folder itself, or a single
# child such as "rocks_kfs" in older packages).
function Get-LatestPackageVersion( [string] $PluginFolder ) {
    if ( -not ( Test-Path $PluginFolder ) ) { throw "Plugin folder not found: $PluginFolder" }

    $latest = Get-ChildItem $PluginFolder -Directory |
        ForEach-Object { [pscustomobject]@{ Name = $_.Name; Path = $_.FullName; Version = ConvertTo-PackageVersion $_.Name } } |
        Where-Object { $_.Version } |
        Sort-Object Version, Name |
        Select-Object -Last 1
    if ( -not $latest ) { throw "No version folders under $PluginFolder" }

    $root = $latest.Path
    if ( -not ( Test-Path ( Join-Path $root "content" ) ) ) {
        $nested = Get-ChildItem $root -Directory | Where-Object { Test-Path ( Join-Path $_.FullName "content" ) } | Select-Object -First 1
        if ( -not $nested ) { throw "No content folder in $root" }
        $root = $nested.FullName
    }

    $latest | Add-Member PackageRoot $root
    $latest | Add-Member PluginFile ( Get-ChildItem $root -Filter *.plugin -File | Select-Object -First 1 )
    return $latest
}

# Maps a path relative to a package's content\ folder to where it comes from.
#   Plugins\rocks_kfs\X  -> Block, RepoPath X in RockBlocks
#   bin\rocks.kfs.*.dll  -> KfsDll, a build output
#   bin\*.dll            -> Dll, a third-party dependency shipped from build output
#   anything else        -> Other
function Resolve-ContentSource( [string] $Relative, [string] $BlocksRepo ) {
    if ( $Relative -match '^Plugins\\rocks_kfs\\(.+)$' ) {
        return [pscustomobject]@{ Kind = "Block"; RepoPath = ( $Matches[1] -replace '\\', '/' ) }
    }
    if ( $Relative -match '^bin\\(rocks\.kfs\.[^\\]+\.dll)$' ) {
        return [pscustomobject]@{ Kind = "KfsDll"; FileName = $Matches[1] }
    }
    if ( $Relative -match '^bin\\([^\\]+\.dll)$' ) {
        return [pscustomobject]@{ Kind = "Dll"; FileName = $Matches[1] }
    }
    return [pscustomobject]@{ Kind = "Other" }
}
