# CLAUDE.md

Guidance for Claude Code when working in this repository and the surrounding Rock checkout.

## Repository layout

This folder (`KFSRockAssemblies`) lives inside a full Rock RMS source tree (e.g. `C:\KFSRepo\Rock\Rock16`, `C:\KFSRepo\Rock\Rock17`, etc). Most of that tree is **upstream Rock core code that we do not control** — avoid editing it. The KFS-owned code is junction-linked into the Rock tree (see `CreateLinks.bat`) and lives in:

| Path (relative to Rock root) | What it is |
|---|---|
| `KFSRockAssemblies\` | This repo — KFS Rock assemblies/plugins (class libraries, jobs, workflow actions, financial gateways, integrations) |
| `RockWeb\Plugins\rocks_kfs\` | KFS RockBlocks repo — WebForms blocks (.ascx) |
| `RockAttendedCheckin\` | KFS Attended Check-in |
| `RockWeb\Plugins\cc_newspring\` | Attended Check-in blocks (linked from RockAttendedCheckin) |
| `com.protectmyministry.RockPlugin\` and `RockWeb\Plugins\com.protectmyministry\` | Protect My Ministry background check plugin |
| `RockWeb\Content\KFSRockAssets\` | KFS shared assets |

## Rules of thumb

- **Default to working inside the KFS-owned paths above.** Treat any edit to core Rock files (`Rock\`, `Rock.*\`, `RockWeb\` outside the plugin folders, etc.) as a core patch that is not allowed.
- Each `rocks.kfs.*` folder is its own project/plugin. Follow the conventions of the specific project you're editing (namespaces `rocks.kfs.*`, Apache 2.0 license headers on source files).
- Blocks (UI) generally live in the RockBlocks repo (`RockWeb\Plugins\rocks_kfs`); supporting assemblies, jobs, models, and migrations live here in RockAssemblies. A feature often spans both.

## Solution / build

- `KFSRock.sln.kfs` in this folder is the master solution. It is copied out to the Rock root (e.g. as `KFSRock16.sln`) and used from there so relative project references resolve against the Rock tree.
- This `CLAUDE.md` is copied to the Rock root at the same time (see the root `CreateLinks.bat`) so Claude Code sessions started at the Rock root always load it. Edit the copy in `KFSRockAssemblies` — the root copy is generated.
- Projects target .NET Framework (Rock v16-era). Build with Visual Studio/MSBuild against the copied solution at the Rock root, not from inside this folder.
- Plugin assemblies reference Rock core assemblies by relative path — the sibling folder structure described in `README.md` must be intact for builds to work.
