@echo off
REM Run this from the root of a Rock clone (Rock17, Rock18, ...).
REM Junctions the KFS repos into the Rock tree so plugins build against Rock's
REM assemblies and so Claude Code loads the shared guardrails with the Rock
REM clone as its project root.

mklink /J KFSRockAssemblies c:\KFSRepo\Rock\KFSRockAssemblies
mklink /J RockAttendedCheckin c:\KFSRepo\Rock\RockAttendedCheckin 
mklink /J RockWeb\Plugins\rocks_kfs c:\KFSRepo\Rock\KFSRockBlocks
mklink /J RockWeb\Plugins\cc_newspring c:\KFSRepo\Rock\RockAttendedCheckin\cc_newspring
mklink /J RockWeb\Content\KFSRockAssets c:\KFSRepo\Rock\KFSRockAssets

REM --- Claude Code guardrails -------------------------------------------------
REM The config is tracked in KingdomFirst/RockAssemblies but must load with the
REM Rock clone as the project root, because every skill path is relative to it.
REM Junction (/J) and hard link (/H) both work without admin rights.
REM Skip these two in a Rock v20 clone -- it ships its own .claude and CLAUDE.md.

mklink /J .claude c:\KFSRepo\Rock\KFSRockAssemblies\.claude
mklink /H CLAUDE.md c:\KFSRepo\Rock\KFSRockAssemblies\CLAUDE.md

REM Keep the links from showing up as untracked in the Rock clone.
findstr /x /c:".claude" .git\info\exclude >nul 2>&1 || echo .claude>> .git\info\exclude
findstr /x /c:"CLAUDE.md" .git\info\exclude >nul 2>&1 || echo CLAUDE.md>> .git\info\exclude
