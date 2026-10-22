---
name: kfs-compat-preflight
description: >-
  Compile every active KFS RockShop plugin (DLL projects and WebForms pages) against a Rock
  clone and report what breaks, by plugin. Use as the first check when a new Rock version drops,
  or when the user says "preflight", "compile check", or "which plugins break on Rock X".
---

Run the compatibility preflight in `KFSRockAssemblies/rocks.kfs.Tests.Compile` and report the
results. Read that project's `README.md` if you have not already.

**Plugins are RockShop packages.** A plugin's code is what its newest package in the shared Rock
Packages folder ships (`PluginCatalog.ps1`), the same plugins the end-to-end suite and
`/kfs-package` use. Code that no package ships is not tested; do not add it back. If the user
expects code to be covered and it is not, the answer is to ship it in a plugin's package.
Folders listed in `.guardrails/skills/kfs-package/excluded-plugins.txt` are not KFS plugins (e.g.
no longer our IP): they are skipped entirely and must not appear in a report.

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

One run tests every active plugin. It runs `CreateLinks.bat` only if junctions are missing, and
it builds the plugins' DLLs into that clone's `RockWeb\Bin`. Allow 5 to 15 minutes; run it in
the background.

- `-Plugin "<name>", ...` tests only the plugins the user named (Rock Packages folder names).
- `-ListPlugins` shows what each plugin ships, without building. Use it to answer "is X
  covered?".
- `-SkipBuild` reuses already-built DLLs.

## 3. Report

Read `results\<version>\preflight.md` (`preflight-selected.md` for a `-Plugin` run) and give the
user its path: it is the record of the run, with every plugin's DLLs and pages and their
results, and a dated copy is kept in `results\<version>\history\`. Then report **by plugin**:

- The Rock version, and how many active plugins pass, fail, and are incomplete.
- **Each failing plugin**: the DLL or page that breaks and the Rock API that changed. For the
  ones that are not obvious, check the Rock source (`git log -S"<name>"` in the clone) to say
  what replaced it. Attended Check-in is a NewSpring fork: ask before proposing changes to it.
- Incomplete plugins, and which DLL failure keeps their pages from being checked.
- Rock dependencies a plugin's build overwrites, and the plugin responsible.
- The *Not tested* plugins, in one line each (Obsidian only, vendor plugin, no package), and
  any shipped files that are not on this branch.
- Obsolete-API calls per plugin, plus the few that matter most.

Exit code 2 is an environment problem: report what to fix, and give no verdict on the plugins.

**Do not fix anything in this skill.** Offer `/bugfix` for specific items.
