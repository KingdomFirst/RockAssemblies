<#
.SYNOPSIS
    Builds the Claude Code guardrail overlay in a Rock clone.

.DESCRIPTION
    Joins three sources into a gitignored overlay at the Rock clone root:

        Rock v20 guardrails   <Rock20>\.claude\        (junctioned, never copied)
        KFS curated delta     <Guardrails>\            (this folder)
        Overlay               <Target>\.claude\        (generated)

    Also creates machine-wide skill junctions in ~\.claude\skills\ so skills are
    available in every clone without per-repo wiring.

    Idempotent. Re-run after changing sources or adding skills.

.EXAMPLE
    .\setup.ps1 -Target C:\KFSRepo\Rock\Rock17

.EXAMPLE
    .\setup.ps1 -Target C:\KFSRepo\Rock\Rock18 `
                -Rock20 C:\KFSRepo\Rock\Rock20 `
                -Guardrails C:\KFSRepo\Rock\KFSRockAssemblies-guardrails\guardrails
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string] $Target,

    [string] $Rock20 = 'C:\KFSRepo\Rock\Rock20',

    [string] $Guardrails,

    [switch] $SkipSkills
)

$ErrorActionPreference = 'Stop'

# $PSScriptRoot is not reliably populated as a parameter default, so resolve it here.
if ([string]::IsNullOrWhiteSpace($Guardrails)) {
    $Guardrails = Split-Path -Parent $MyInvocation.MyCommand.Path
}

function Fail($message) {
    Write-Host ''
    Write-Host "SETUP FAILED: $message" -ForegroundColor Red
    Write-Host ''
    Write-Host 'Nothing was left half-linked. Fix the above and re-run.' -ForegroundColor Yellow
    exit 1
}

function Confirm-Source($path, $what) {
    if (-not (Test-Path -LiteralPath $path)) {
        Fail "$what not found: $path"
    }
}

# Replace a junction or directory in place. Uses Directory.Delete on reparse points so we
# never recurse into the junction target and delete the source.
function Set-Junction($linkPath, $targetPath) {
    if (Test-Path -LiteralPath $linkPath) {
        $item = Get-Item -LiteralPath $linkPath -Force
        if ($item.Attributes -band [IO.FileAttributes]::ReparsePoint) {
            [IO.Directory]::Delete($linkPath, $false)
        }
        else {
            Remove-Item -LiteralPath $linkPath -Recurse -Force
        }
    }
    New-Item -ItemType Junction -Path $linkPath -Target $targetPath | Out-Null
    Write-Host ("  link  {0,-34} -> {1}" -f (Split-Path $linkPath -Leaf), $targetPath)
}

function Add-GitExclude($repoRoot, $pattern) {
    $excludeFile = Join-Path $repoRoot '.git\info\exclude'
    if (-not (Test-Path -LiteralPath $excludeFile)) { return }
    $lines = @(Get-Content -LiteralPath $excludeFile -ErrorAction SilentlyContinue)
    if ($lines -notcontains $pattern) {
        Add-Content -LiteralPath $excludeFile -Value $pattern
        Write-Host "  git   excluded $pattern"
    }
}

# --- Validate every source up front -----------------------------------------

Write-Host ''
Write-Host 'Validating sources' -ForegroundColor Cyan

Confirm-Source $Target                                  'Target Rock clone'
Confirm-Source (Join-Path $Target 'Rock.Version')       'Target does not look like a Rock clone (no Rock.Version)'
Confirm-Source $Rock20                                  'Rock20 guardrail source clone'
Confirm-Source (Join-Path $Rock20 '.claude\rules')      "Rock20 guardrails (.claude\rules)"
Confirm-Source (Join-Path $Rock20 '.claude\skills')     "Rock20 skills (.claude\skills)"
Confirm-Source (Join-Path $Rock20 'CLAUDE.md')          'Rock20 CLAUDE.md'
Confirm-Source $Guardrails                              'KFS guardrails folder'
Confirm-Source (Join-Path $Guardrails 'rules')          'KFS rules'
Confirm-Source (Join-Path $Guardrails 'CLAUDE-kfs.md')  'KFS CLAUDE-kfs.md'

$targetVersion = 'unknown'
$sharedInfo = Join-Path $Target 'Rock.Version\AssemblySharedInfo.cs'
if (Test-Path -LiteralPath $sharedInfo) {
    $m = Select-String -LiteralPath $sharedInfo -Pattern 'AssemblyInformationalVersion\(\s*"([^"]+)"' |
         Select-Object -First 1
    if ($m) { $targetVersion = $m.Matches[0].Groups[1].Value }
}

Write-Host "  ok    target      $Target  ($targetVersion)"
Write-Host "  ok    rock20      $Rock20"
Write-Host "  ok    guardrails  $Guardrails"

# Surface the guardrail source's checkout so stale content is visible.
Push-Location $Guardrails
$grHead = (& git rev-parse --abbrev-ref HEAD 2>$null)
$grDesc = (& git log -1 --format='%h %s' 2>$null)
Pop-Location
if ($grHead) { Write-Host "  ok    guardrails on '$grHead' -> $grDesc" }

# --- Overlay ----------------------------------------------------------------

Write-Host ''
Write-Host "Building overlay in $Target" -ForegroundColor Cyan

$claude = Join-Path $Target '.claude'

# The previous layout linked .claude as a junction and CLAUDE.md as a hard link, both pointing
# into KFSRockAssemblies. Writing the overlay through either would modify the source repo, so
# clear them first.
if (Test-Path -LiteralPath $claude) {
    $existing = Get-Item -LiteralPath $claude -Force
    if ($existing.Attributes -band [IO.FileAttributes]::ReparsePoint) {
        [IO.Directory]::Delete($claude, $false)
        Write-Host '  note  removed legacy .claude junction'
    }
}

$targetClaudeMd = Join-Path $Target 'CLAUDE.md'
if (Test-Path -LiteralPath $targetClaudeMd) {
    # Unlink rather than overwrite: a hard link shares content with the source file.
    Remove-Item -LiteralPath $targetClaudeMd -Force
    Write-Host '  note  removed existing CLAUDE.md link'
}

New-Item -ItemType Directory -Path (Join-Path $claude 'rules') -Force | Out-Null

Set-Junction (Join-Path $claude 'rules\rock') (Join-Path $Rock20 '.claude\rules')
Set-Junction (Join-Path $claude 'rules\kfs')  (Join-Path $Guardrails 'rules')
Set-Junction (Join-Path $claude 'commands')   (Join-Path $Guardrails 'commands')
Set-Junction (Join-Path $claude 'hooks')      (Join-Path $Guardrails 'hooks')

Copy-Item -LiteralPath (Join-Path $Guardrails 'settings.json') `
          -Destination (Join-Path $claude 'settings.json') -Force
Write-Host '  copy  settings.json'

# --- Generated CLAUDE.md ----------------------------------------------------

$rockClaude = (Join-Path $Rock20 'CLAUDE.md')       -replace '\\', '/'
$kfsClaude  = (Join-Path $Guardrails 'CLAUDE-kfs.md') -replace '\\', '/'

@"
<!-- GENERATED by guardrails\setup.ps1 - do not edit, do not commit. -->
<!-- Sources: $Rock20 (Rock v20, unmodified) + $Guardrails (KFS delta) -->

Rock's guidelines are imported first and are the standard. KFS departures follow and override
them; every conflict is named explicitly in ``.claude/rules/kfs/kfs-precedence.md``.

@$rockClaude

@$kfsClaude
"@ | Set-Content -LiteralPath $targetClaudeMd -Encoding UTF8 -NoNewline

Write-Host '  gen   CLAUDE.md'

Add-GitExclude $Target '.claude'
Add-GitExclude $Target 'CLAUDE.md'

# --- Machine-wide skills ----------------------------------------------------

if (-not $SkipSkills) {
    Write-Host ''
    Write-Host 'Wiring skills into ~\.claude\skills' -ForegroundColor Cyan

    $userSkills = Join-Path $HOME '.claude\skills'
    New-Item -ItemType Directory -Path $userSkills -Force | Out-Null

    $n = 0
    foreach ($src in Get-ChildItem -Directory (Join-Path $Rock20 '.claude\skills')) {
        Set-Junction (Join-Path $userSkills $src.Name) $src.FullName
        $n++
    }
    $k = 0
    $kfsSkills = Join-Path $Guardrails 'skills'
    if (Test-Path -LiteralPath $kfsSkills) {
        foreach ($src in Get-ChildItem -Directory $kfsSkills) {
            Set-Junction (Join-Path $userSkills $src.Name) $src.FullName
            $k++
        }
    }
    Write-Host "  $n Rock skills, $k KFS skills"
}

# --- Done -------------------------------------------------------------------

Write-Host ''
Write-Host 'Done.' -ForegroundColor Green
Write-Host "  Start Claude Code from $Target"
Write-Host '  Verify with /context: Rock rules under rules/rock, KFS under rules/kfs.'
Write-Host ''
