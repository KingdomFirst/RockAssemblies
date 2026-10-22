---
name: kfs-package
description: >-
  Create RockShop plugin packages (.plugin files, release notes and screenshots) for KFS plugins
  changed by a branch or merge. Use when the user says "package", "create plugin files",
  "RockShop update", "new plugin version", or "release notes for the RockShop".
---

Build a new RockShop package for every KFS RockShop plugin a change touched, plus a
`RockShopSubmission.md` with the RockShop description and screenshots.

Helper scripts live beside this file (`.guardrails/skills/kfs-package/`):

| Script | Does |
|---|---|
| `Find-PackageChanges.ps1 -Plugin "<folder>"` | Read-only. For each file the latest package ships, finds the commit it was built from and lists the commits and PRs since, plus uncommitted changes |
| `New-PluginPackage.ps1 -Plugin "<folder>" -Version x.y` | Builds the next version folder and `.plugin` zip, re-sourcing every file the previous package shipped |

## Definitions

- **Rock Packages**: `%USERPROFILE%\OneDrive - Kingdom First Solutions Inc\Shared Documents -
  Kingdom First Solutions Inc. Team Site\Rock Packages`. One folder per plugin, one subfolder
  per version, plus `Screenshots` (some plugins spell it `ScreenShots`) and `Documentation`.
- **A KFS RockShop plugin** is a plugin with a folder there. A changed project or block with no
  package folder is not one. Skip it and say so. `_Package Icons` is not a plugin, and neither
  is any folder listed in `excluded-plugins.txt` beside this file (e.g. a plugin that is no
  longer KFS intellectual property): never package, change or report those. The scripts refuse
  them. When a plugin stops being ours, add its folder there; the preflight and the end-to-end
  reports read the same list.
- **A version folder** holds the `.plugin` zip and its unpacked `content\`, `install\` and
  `uninstall\`. A few old ones nest these one level down (`v1.0\rocks_kfs\`). New versions use
  the flat layout. Folder and zip names follow the previous version's style (`v2.3`, `2_7`,
  `1.3`; `CheckInMonitor_v2_3.plugin`). The script does this for you.
- **Ignore `ZZZ_Archive`** in both repos. Archived code is never packaged.

## 1. Find the affected plugins

Collect the changed files from whatever the user names: usually the current branch of each repo
against `master`, and/or the last merge to `master` (`git diff <merge>^1 <merge>`). **Exclude
uncommitted changes unless the user says to include them.** When they do, use the working tree
for both the file list and the build.

Map each changed file to a plugin by what the plugin's latest package ships:

- `RockBlocks` path `X` → a package containing `content\Plugins\rocks_kfs\X`.
- `RockAssemblies` project `rocks.kfs.Y` → a package containing `content\bin\rocks.kfs.Y.dll`.

Shipped file lists come from each plugin's newest version folder; `Find-PackageChanges.ps1`
prints them. Report the plugins found **and** the changed code that maps to no package.

## 2. Find every change since the last package

Run `Find-PackageChanges.ps1` for each affected plugin. It goes back to the commit the last
package was really built from, not to the last PR, so it catches changes that were merged but
never packaged. KingdomFirst/RockBlocks#17 sat unpackaged from 2020 to 2026 this way.

If a block file reports "matches no commit", the package was built from uncommitted or
hand-edited code. Diff the packaged file against the repo yourself before trusting the date
fallback.

## 3. Version

**A minor bump over the previous package** (`2.2` → `2.3`, `2.14` → `2.15`) unless the user
says otherwise.

- **Plugin with a KFS dll**: the package version is the dll's `AssemblyVersion` major.minor.
  Check the project's `Properties/AssemblyInfo.cs`. If it is not already one minor above the
  previous package, stop and ask the user. Do not bump it yourself; it is a code change.
- **Plugin without a dll**: the previous package's version plus one minor.

## 4. Build any dll

Build the plugin project in **Release** in the Rock clone that `master` targets
(`kfs-rock-versions.md` § Version branches). Build only that project, not the whole solution.
Use the MSBuild from `vswhere`, as `/kfs-build` describes:

```
msbuild KFSRockAssemblies\rocks.kfs.Y\rocks.kfs.Y.csproj /t:Rebuild /p:Configuration=Release /p:SolutionDir=<clone>\ /p:PostBuildEvent= /v:m
```

`PostBuildEvent=` stops the post-build `xcopy` into `RockWeb\Bin`. Confirm the built dll's
version starts with the package version.

**Third-party dlls.** Ship one only if the plugin needs it and Rock does not provide it. If
the csproj now references it from `RockWeb\Bin` with `<Private>False</Private>`, Rock ships
it: pass it to `-Exclude`. **Never** list a dll Rock ships in either `deletefile.lst`. Deleting
it breaks Rock. `bin\Release` may hold stale copies of dlls the project no longer copies, so
`-DllSource` takes only the files the previous package listed.

## 5. Assemble

```
.\New-PluginPackage.ps1 -Plugin "<folder>" -Version x.y [-DllSource <bin\Release>] [-Exclude ...] [-Add @{ 'Plugins\rocks_kfs\...' = '<path>' }]
```

It refuses to overwrite an existing version folder. Act on every warning it prints: new or
removed files usually mean `install\deletefile.lst`, `uninstall\deletefile.lst` or
`install\run.sql` need editing. Then verify that each packaged block file's
`git hash-object` matches the repo, and that the zip opens.

## 6. Release notes

For each plugin, gather the **Release Notes** section of every PR from step 2. `gh` may not be
installed, and the KingdomFirst repos are public, so the REST API works without a token:
`https://api.github.com/repos/KingdomFirst/<repo>/pulls/<n>`. Unmerged branches: look the PR up
with `pulls?head=KingdomFirst:<branch>&state=all`. Uncommitted work has no PR. Read the diff.

Write them for church staff, not developers, in this shape:

```html
<dl><dt>Changes</dt><dd><ul><li>First change.</li><li>Second change.</li></ul></dd></dl>
```

- **One plugin's changes only.** A sweep PR spanning many plugins has generic notes. Write
  that plugin's bullets from its own diff.
- **Rock compatibility work is one bullet**: `Updated for compatibility with Rock v20.` (use the
  version in question). Never list the individual API changes it took. This includes dropping
  a dependency Rock now ships. Real bug fixes and features found along the way still get
  their own bullets.
- **Verify every claim against the code.** A commit subject is not evidence. Read the before
  and after.
- Abbreviate. One sentence per bullet, starting with Added / Fixed / Updated / Removed.
- Never include internal names, ticket numbers or client names.

## 7. Screenshots

A change **affects the UI** when it changes what a user sees or sets: `.ascx` markup, block
settings (attributes added, removed, renamed or reordered), rendered output or its visibility,
messages shown on screen, CSS, or `.obs` components. Compatibility-only, logic-only and job
changes do not.

For each UI change, capture the part that changed:

1. **Preferred: the E2E suite** (`rocks.kfs.Tests.EndToEnd`). Find the test that covers the
   change, or add one, and call `releaseScreenshot( locator, "<PluginName>_<WhatItShows>" )`
   after its assertions pass. Ask the user to deploy the new version to the test site, and to
   confirm the site, before running anything. `/kfs-compat-test` has the rules. Then run:
   `RELEASE_SCREENSHOTS_DIR=<scratch dir> npx playwright test tests/<Domain>/<Plugin>`
2. **Otherwise**, open the page on the test site in the browser and screenshot the element.
3. **Never** substitute an image from an old PR or `.screenshots/` for one of the new version.

Copy each image into the plugin's `Screenshots` folder (create it if missing; keep the
existing spelling) as `<PluginName>_<WhatItShows>_v<x_y>.png`. If a screenshot cannot be taken
yet, list it in the markdown file as pending and tell the user. Do not leave it out.

## 8. RockShopSubmission.md

In the version folder:

````markdown
# <Plugin> v<x.y>

Package: `<file>.plugin` (<the KFS dll and its full version, or "block files only; no KFS dll">)
Changes since v<previous>: KingdomFirst/<Repo>#<n>, ...

## Description HTML

```html
<dl><dt>Changes</dt><dd><ul><li>...</li></ul></dd></dl>
```

## New Screenshots

- <file>.png
````

Include **New Screenshots** only when the update changed the UI. Add **Packaging notes (not
for the RockShop description)** only for something a reviewer must know: a dependency dropped,
a `deletefile.lst` change, a run.sql change.

## Report

If the packages were validated on the test site with the E2E suite, give the path to that
run's `summary.html` (`/kfs-compat-test` step 4); it is the test record for the submission.

Per plugin: the new version folder, the dll version where there is one, the PRs covered, the
Description HTML, and screenshots taken or pending. Then list the changed code that maps to no
RockShop plugin. Never upload to the RockShop or change a previous version's folder.

## Collecting this earlier

Notes and screenshots are cheapest at PR time:

- Every PR template has **Release Notes** and **Screenshots** sections. Fill in a
  client-readable Release Notes line *per plugin* touched, and answer Screenshots honestly.
- When a fix or feature changes the UI, add a `releaseScreenshot()` call to the E2E test that
  covers it. The screenshot then comes free with the test run on each release.
