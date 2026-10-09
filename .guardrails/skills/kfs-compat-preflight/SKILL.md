---
name: kfs-compat-preflight
description: >-
  Compile every KFS plugin (DLL projects and WebForms blocks) against a Rock clone and list
  everything that breaks. Use as the first check when a new Rock version drops, or when the user
  says "preflight", "compile check", or "which plugins break on Rock X".
---

Run the compatibility preflight in `KFSRockAssemblies/rocks.kfs.Tests.Compile` and report the
results. Read that project's `README.md` if you have not already.

**Ignore `ZZZ_Archive`.** Plugins under a `ZZZ_Archive` folder (in `KFSRockAssemblies` or
`KFSRockBlocks`) are retired and obsolete by KFS practice. Never test, report or fix them. The
script already skips them; if one ever shows up in a report, treat it as a script bug.

## 1. Pick the Rock clone

Use the clone the user named. Otherwise use the newest `C:\KFSRepo\Rock\Rock<major>` that has
`RockWeb\Bin\Rock.dll`, and say which one you chose. If Rock core is not built there, stop and
say so. Do not build Rock core yourself.

## 2. Run

```powershell
KFSRockAssemblies\rocks.kfs.Tests.Compile\Invoke-CompatPreflight.ps1 -RockRoot C:\KFSRepo\Rock\Rock<major>
```

It runs `CreateLinks.bat` only if junctions are missing, and it builds the plugin DLLs into
that clone's `RockWeb\Bin`. Allow 10 to 15 minutes; run it in the background.
`-SkipBuild` reuses already-built DLLs.

## 3. Report

Read `results\<version>\preflight.md` and report:

- The Rock version, and counts: DLLs building, blocks clean, broken, and not checked.
- The **Coverage** table: counts for each source (KFS plugins, and the Attended Check-in fork
  in `RockAttendedCheckin` + `RockWeb\Plugins\cc_newspring`). Report Attended Check-in breaks
  separately; it is a NewSpring fork, so ask before proposing changes to it.
- **Every broken block and DLL**, grouped by plugin, each with the Rock API that changed. For
  the ones that are not obvious, check the Rock source (`git log -S"<name>"` in the clone) to
  say what replaced it.
- Blocks that could not be checked, and which DLL failure blocks them.
- The obsolete-API count, plus the few that matter most.

Exit code 2 is an environment problem: report what to fix, and give no verdict on the plugins.

**Do not fix anything in this skill.** Offer `/bugfix` for specific items.
