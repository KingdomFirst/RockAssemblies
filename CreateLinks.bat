rd .\KFSRockAssemblies 2>nul
rd .\RockAttendedCheckin 2>nul
rd .\RockWeb\Plugins\rocks_kfs 2>nul
rd .\RockWeb\Plugins\cc_newspring 2>nul
rd .\RockWeb\Content\KFSRockAssets 2>nul
@echo off
REM Run this from the root of a Rock clone (Rock17, Rock18, ...).
REM Junctions the KFS repos into the Rock tree so plugins build against Rock's assemblies.

mklink /J KFSRockAssemblies c:\KFSRepo\Rock\KFSRockAssemblies
mklink /J RockAttendedCheckin c:\KFSRepo\Rock\RockAttendedCheckin
mklink /J RockWeb\Plugins\rocks_kfs c:\KFSRepo\Rock\KFSRockBlocks
mklink /J RockWeb\Plugins\cc_newspring c:\KFSRepo\Rock\RockAttendedCheckin\cc_newspring
mklink /J RockWeb\Content\KFSRockAssets c:\KFSRepo\Rock\KFSRockAssets

REM --- Claude Code guardrails -------------------------------------------------
REM Not set up here. The guardrails come from two sources (a Rock20 clone for
REM Rock's, and KFSRockAssemblies\.guardrails for ours), so they need more than
REM a junction. Run this once per Rock clone instead:
REM
REM     powershell -ExecutionPolicy Bypass -File ^
REM         c:\KFSRepo\Rock\KFSRockAssemblies\.guardrails\setup.ps1 -Target %CD%
REM
REM It adds CLAUDE.local.md and entries under .claude\ without writing over
REM anything the clone tracks, lists them in .git\info\exclude, and wires the
REM skills into ~\.claude\skills. Re-run it after pulling guardrail changes.
