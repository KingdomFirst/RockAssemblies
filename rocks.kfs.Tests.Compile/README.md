# rocks.kfs.Tests.Compile — compatibility preflight

Compiles **every** KFS plugin against a Rock clone's assemblies and lists everything that no
longer compiles. It is the first check to run when a new Rock version drops. It needs no
site, no database and no test data, and it covers all plugins in one pass.

It catches the most common kind of break: Rock removed or changed an API the plugin calls
(a system Guid constant, a method overload, a type). It does **not** catch behavior changes;
that is what each plugin's end-to-end tests in `rocks.kfs.Tests.EndToEnd` are for.

## Running

From PowerShell, pointing at the Rock clone to test against:

```powershell
KFSRockAssemblies\rocks.kfs.Tests.Compile\Invoke-CompatPreflight.ps1 -RockRoot C:\KFSRepo\Rock\Rock20
```

Or in Claude Code: `/kfs-compat-preflight`.

Prerequisites: Rock core built into `RockWeb\Bin` in that clone, Visual Studio (for MSBuild),
.NET Framework 4.x (for `aspnet_compiler`), and the .NET SDK (to restore any missing
RockWeb packages).

**Retired plugins are never tested.** Anything under a `ZZZ_Archive` folder, in either
`KFSRockAssemblies` or `KFSRockBlocks`, is retired and obsolete by KFS practice. The preflight
skips those projects and blocks, and their errors are never reported or fixed.

## What it does

1. **Junctions.** If any KFS junction is missing from the clone, it runs `CreateLinks.bat`.
   Otherwise it leaves the clone alone.
2. **Plugin DLLs.** It builds every project listed in `KFSRock.sln.kfs` from both sources
   below against the clone's `RockWeb\Bin`. Each project's post-build step copies its DLL
   into `RockWeb\Bin`, exactly as a normal build would. `-SkipBuild` skips this step and uses
   whatever `rocks.kfs.*.dll` and `cc.newspring.*.dll` are already there.
3. **Blocks.** It precompiles every WebForms block under `RockWeb\Plugins\rocks_kfs` and
   `RockWeb\Plugins\cc_newspring` (excluding `ZZZ_Archive`) with `aspnet_compiler`, the
   compiler IIS runs on first page load. This happens in a staging copy under
   `%TEMP%\kfsc-<clone>`, so the clone's `RockWeb` is not touched.

It covers two sources, listed in `$sources` at the top of the script:

| Source | Repo | DLL projects | Blocks |
|---|---|---|---|
| KFS plugins | KFSRockAssemblies, KFSRockBlocks | `KFSRockAssemblies\…` in `KFSRock.sln.kfs` | `RockWeb\Plugins\rocks_kfs` |
| Attended Check-in | rock-attended-checkin (NewSpring fork) | `RockAttendedCheckin\cc.newspring.AttendedCheckIn.csproj` | `RockWeb\Plugins\cc_newspring` |

The report's **Coverage** table gives the DLL and block counts for each. If a source has no
DLL project in `KFSRock.sln.kfs` or no blocks under its folder, the run stops with exit code 2
instead of quietly skipping it. The Attended Check-in fork is tested here, but fixes to it are
asked about first, never made to "conform" it.

`aspnet_compiler` stops at the first failing file. The script sets each failing block aside
and compiles the rest again until what remains is clean, so one run lists every broken
block.

It also lists every call to a Rock API marked `[Obsolete]`. Those calls will break in a
future Rock version: Person Attribute Forms' `PERSON_CONNECTION_STATUS_WEB_PROSPECT` was
flagged obsolete in Rock 1.13, seven years before v20 removed it.

## Output

`results\<rock version>\preflight.md` (for people) and `preflight.json` (for tools). Exit
codes:

| Code | Meaning |
|---|---|
| 0 | Everything compiles |
| 1 | Compile errors found (see the report) |
| 2 | Environment problem (missing tools, Rock not built, staging failure). No verdict on the plugins. |

Blocks are classified as:

- **Broken**: the block's own code no longer compiles against this Rock.
- **Could not be checked**: the block depends on a KFS plugin DLL that failed to build.
  Fix the DLL first; the block may be fine.

It also reports **Rock dependencies overwritten by a plugin**. A plugin's post-build step can
copy its own version of a library Rock depends on into `RockWeb\Bin`, replacing Rock's. On a
site, ASP.NET then fails to start or to load that assembly. Each such DLL is listed with the
version Rock needs, the version the plugin supplies, and the project that copies it. The block
compile always uses Rock's declared version, so one of these does not stop the scan.

Because plugin builds copy into the clone's real `RockWeb\Bin`, a conflicting DLL stays there
after a preflight, as it would after any normal build. Restore Rock's copy from the path in the
matching `.refresh` file if you run that clone as a local site.

## Beta packages

`New-CompatPackages.ps1` rebuilds RockShop plugin packages from the current branch for manual
deployment to a beta site. Each package uses its newest release in the shared *Rock Packages*
folder as the template: same plugin folder name, the release's version folder plus a suffix,
and the same `content\`, `install\` and `uninstall\` files, with content refreshed from the
branch. Nothing is written to the shared folder.

```powershell
.\Invoke-CompatPreflight.ps1 -RockRoot C:\KFSRepo\Rock\Rock17   # builds the DLLs against the target Rock
.\New-CompatPackages.ps1                                          # writes results\packages\compat-beta\
```

- `$plugins` in the script names the RockShop plugins (shared-folder names) to rebuild; edit
  it for the next round. A changed file that no RockShop package contains is not packaged.
- `bin\*.dll` come from the building project's `bin\Debug`; `Plugins\rocks_kfs\*` from
  `KFSRockBlocks`. Build against the Rock version the site will run. Rock's assemblies are not
  strong-named, so it is the site's Rock version that must have the APIs, not a file version.
- Rock-owned DLLs (`$rockOwned`, e.g. `Microsoft.Identity.Client.dll`) are left out even if a
  release shipped them.
- It stops before writing anything if a content file cannot be found on the branch or a
  packaged file has uncommitted changes.
- `-Suffix` (default `compat-beta`) is a placeholder, not a release version: set real
  versions before anything goes into the shared Rock Packages folder.

## Limits

- A file can hide more errors behind the ones reported. Re-run after fixing.
- Obsidian blocks (`rocks.kfs.JavaScript.Obsidian`) are not covered here yet.
- A plugin DLL that compiles from source can still fail at runtime if a site runs an
  older build of it. Rebuild and redeploy DLLs for each new Rock version.
