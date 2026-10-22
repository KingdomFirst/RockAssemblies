# rocks.kfs.Tests.Compile — compatibility preflight

Compiles every **active KFS plugin** against a Rock clone's assemblies and reports, plugin by
plugin, everything that no longer compiles. It is the first check to run when a new Rock
version drops. It needs no site, no database and no test data, and one run covers every
plugin.

## Running

From PowerShell, pointing at the Rock clone to test against:

```powershell
KFSRockAssemblies\rocks.kfs.Tests.Compile\Invoke-CompatPreflight.ps1 -RockRoot C:\KFSRepo\Rock\Rock20
```

Or in Claude Code: `/kfs-compat-preflight`.

| Option | Does |
|---|---|
| *(none)* | Tests every active plugin. |
| `-Plugin "Steps to Care", "Eventbrite"` | Tests only these plugins (Rock Packages folder names; "(Unreleased)" is optional). |
| `-ListPlugins` | Lists every plugin and the code it ships, then stops. Builds nothing. |
| `-SkipBuild` | Reuses the DLLs already in `RockWeb\Bin` instead of building them. |
| `-RockPackagesRoot <path>` | The Rock Packages folder, if it is not at the usual OneDrive path. |

Prerequisites: Rock core built into `RockWeb\Bin` in that clone, Visual Studio (for MSBuild),
.NET Framework 4.x (for `aspnet_compiler`), the .NET SDK (to restore any missing RockWeb
packages), and the shared Rock Packages folder synced locally.

## What a plugin is

A plugin is a **RockShop package** in the shared *Rock Packages* folder: the same plugins the
end-to-end suite (`rocks.kfs.Tests.EndToEnd`) and `/kfs-package` work with. Its code is what
its newest package ships (`PluginCatalog.ps1`, using the same "newest version" rule as
`/kfs-package`). Folders listed in `.guardrails/skills/kfs-package/excluded-plugins.txt` are
not KFS plugins (e.g. QuickBooks Online, no longer our IP) and are skipped entirely: not
tested, listed or reported.

| Shipped file | Tested as |
|---|---|
| `bin\<assembly>.dll` built by a project in `KFSRockAssemblies` or `RockAttendedCheckin` | That project is built |
| `.ascx`, `.aspx`, `.ashx` under `Plugins\rocks_kfs` or `Plugins\cc_newspring` | Precompiled with its code-behind |
| Other `bin\*.dll` (third-party libraries) | Not compiled; they ship as-is |
| Built Obsidian files (`.obs.js`) | Not compiled (the preflight does not cover Obsidian yet) |
| Files under another vendor's `Plugins\` folder | Not ours; not tested |

A plugin is **active** when its package ships at least one project or page from our repos.
The rest are listed under *Not tested* with the reason: Obsidian only, a vendor plugin, or no
package. Code that no package ships is **not tested at all**; to test it, it has to be in a
plugin's package. A shipped file that is not on this branch (e.g. its project was archived) is
listed under its plugin and not tested.

When a project or page is shipped by more than one plugin, it is built or compiled once and
its result appears under each plugin.

**Retired code is never tested.** Anything under a `ZZZ_Archive` folder, in either
`KFSRockAssemblies` or `KFSRockBlocks`, is retired by KFS practice; a package that still ships
it reports it as archived.

## What it does

1. **Junctions.** If any KFS junction is missing from the clone, it runs `CreateLinks.bat`.
   Otherwise it leaves the clone alone.
2. **Plugins.** It reads the plugin definitions from Rock Packages (`-ListPlugins` stops here).
3. **Plugin DLLs.** It builds each active plugin's projects against the clone's `RockWeb\Bin`.
   Each project's post-build step copies its DLL into `RockWeb\Bin`, exactly as a normal
   build would.
4. **Pages.** It precompiles each active plugin's pages with `aspnet_compiler`, the compiler
   IIS runs on first page load. This happens in a staging copy under `%TEMP%\kfsc-<clone>`
   holding only those pages, so the clone's `RockWeb` is not touched.

`aspnet_compiler` stops at the first failing file. The script sets each failing page aside
and compiles the rest again until what remains is clean, so one run lists every broken page.

It also lists every call to a Rock API marked `[Obsolete]`, by plugin. Those calls will break
in a future Rock version: Person Attribute Forms' `PERSON_CONNECTION_STATUS_WEB_PROSPECT` was
flagged obsolete in Rock 1.13, seven years before v20 removed it.

The Attended Check-in plugin is a NewSpring fork (`RockAttendedCheckin`). It is tested here,
but fixes to it are asked about first, never made to "conform" it.

## Output

`results\<rock version>\preflight.md` (for people) and `preflight.json` (for tools):

1. **Summary**: plugins passing, DLLs building, pages compiling, Rock dependencies
   overwritten, obsolete calls.
2. **Results by plugin**: one row per active plugin with its result, package version, DLL and
   page counts, obsolete calls and notes.
3. **Not tested**: the packages with no code of ours to compile, and why.
4. **One section per plugin**: every DLL project and page with its own result, then the
   errors for anything that failed, Rock dependencies it overwrites, shipped files not on this
   branch, and the obsolete Rock APIs it uses.

A plugin's result:

| Result | Meaning |
|---|---|
| Pass | Every project builds and every page compiles |
| Fail | A project does not build, a page is broken, or its build overwrites a Rock dependency |
| Incomplete | Nothing is broken, but a page could not be checked because a DLL it needs failed to build. Fix the DLL first. |

A run with `-Plugin` writes `preflight-selected.md` / `.json` instead, so `preflight.md` always
holds the latest all-plugins run. Every run also keeps a dated copy in
`results\<rock version>\history\`. Exit codes:

| Code | Meaning |
|---|---|
| 0 | Every tested plugin passes |
| 1 | A plugin fails or is incomplete, or a Rock dependency is overwritten (see the report) |
| 2 | Environment problem (missing tools, Rock not built, Rock Packages not found, staging failure). No verdict on the plugins. |

**Rock dependencies overwritten.** A plugin's post-build step can copy its own version of a
library Rock depends on into `RockWeb\Bin`, replacing Rock's. On a site, ASP.NET then fails to
start or to load that assembly (Microsoft 365 Utilities 1.0.1–1.2 did this with MSAL). Each
such DLL is reported under the plugin whose project copies it, with the version Rock needs and
the version found. The page compile always uses Rock's declared version, so one of these does
not stop the scan.

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
