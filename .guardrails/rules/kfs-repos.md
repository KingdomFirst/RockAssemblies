# KFS Repositories and Environment

Environment facts for the split-repo (junctioned) setup. Overrides the file locations, build
commands and commit format in the `rock/` rules and in Rock's `CLAUDE.md`.

---

## Working root

**Run Claude Code from the Rock clone root** (`Rock17\`, `Rock18\`, …), never from inside
`KFSRockAssemblies\`. Every path below is relative to that root. Plugin projects reference
`..\..\RockWeb\Bin\*.dll`, so nothing resolves outside the junction.

**Clone folders are `Rock<major>` — `Rock17`, not `RockV17`.** Tooling depends on it: the Obsidian
project's `build/build-types.js` reaches a sibling clone by `../../../Rock17/…`. Rename a
misnamed folder rather than patching the tooling.

**Rock core trees are read-only reference** — `Rock/`, `Rock.Blocks/`, `Rock.ViewModels/`,
`Rock.Enums/`, `Rock.Migrations/`, `RockWeb/Blocks/`. We do not fork Rock. If a task appears to
require editing core, stop and raise it.

## The repositories

`CreateLinks.bat` junctions four KFS repositories into the Rock clone:

| Repo | Junctioned to | Contains |
|---|---|---|
| `KingdomFirst/RockAssemblies` | `KFSRockAssemblies` | `rocks.kfs.*` projects, and these guardrails |
| `KingdomFirst/RockBlocks` | `RockWeb/Plugins/rocks_kfs` | WebForms blocks, built Obsidian output |
| `KingdomFirst/rock-attended-checkin` | `RockAttendedCheckin` **and** `RockWeb/Plugins/cc_newspring` | One repo, two junction points. A NewSpring fork: `cc.newspring.*` namespaces, **no copyright headers**. Our conventions do not apply — ask before conforming it. |
| `KFS/rockassets` (self-hosted) | `RockWeb/Content/KFSRockAssets` | Lava, SQL, workflow exports, Less, XSL, ZPL |

`KingdomFirst/RockPlugins` is a separate repo with its own guardrails and its own layout. It is not
junctioned here. If a task belongs there, say so rather than applying these conventions.

## Git: several repositories, one working tree

A bare `git` command at the working root operates on **Rock's** repo, which tracks none of our
files — it reports `?? KFSRockAssemblies/` and shows none of your changes.

| Working on | Prefix |
|---|---|
| Plugins, and these guardrails | `git -C KFSRockAssemblies …` |
| Blocks | `git -C RockWeb/Plugins/rocks_kfs …` |
| Attended check-in (either junction) | `git -C RockAttendedCheckin …` |
| Lava / SQL / workflow assets | `git -C RockWeb/Content/KFSRockAssets …` |
| Rock core (read-only) | `git …` |

Applies to `status`, `diff`, `log`, `branch`, `add`, `commit` — everything. A change spanning two
KFS repos is two commits and two PRs. `git branch` at the root lists **Rock's** branches, not ours.

## Where things go

| What you're creating | Where |
|---|---|
| Plugin entity | `KFSRockAssemblies/rocks.kfs.[Plugin]/Model/[Entity].cs` |
| GUID constants | `KFSRockAssemblies/rocks.kfs.[Plugin]/SystemGuid/[Type].cs` |
| Plugin migration | `KFSRockAssemblies/rocks.kfs.[Plugin]/Migrations/[NNN]_[Name].cs` |
| Job / workflow action / field type | `rocks.kfs.[Plugin]/Jobs/`, `rocks.kfs.Workflow.Action.[Domain]/`, `[Plugin]/Field/Types/` |
| WebForms block | `RockWeb/Plugins/rocks_kfs/[Domain]/[Block].ascx` + `.ascx.cs` |
| Obsidian source | `KFSRockAssemblies/rocks.kfs.JavaScript.Obsidian/src/` |
| Shipped SQL object | `rocks.kfs.[Plugin]/sql/_rocks_kfs_sp*.sql`, deployed by a migration |
| KFS spec | `KFSRockAssemblies/.guardrails/specs/` — **not** `specs/` at the project root |
| KFS doc | `KFSRockAssemblies/.guardrails/docs/[plugin-kebab]/` — **not** `docs/` at the project root |

Those last two matter because the project root is a Rock clone: `specs/` and `docs/` there are
untracked and lost on a clean. Rock's own specs and docs are read-only reference in the `Rock20`
clone. See `kfs-precedence.md` § Skill routing.

Enums follow Rock's structural rules but live in the owning plugin project — `Rock.Enums` is core.
Omit `[Enums.EnumDomain]`; it is `internal` to that project.

## Build

`nuget restore` + `msbuild` on the version-matched `KFSRock*.sln` at the clone root. All 37 plugin
projects are legacy-format csproj on every Rock version, so `dotnet build` will not build them, and
`Rock.sln` fails regardless (`MSB4249` — it contains the `RockWeb` website project; the .NET CLI
refuses a whole solution over one such entry). See `/build`.

There is no unit-test suite. See `/test` for what verification actually exists.

## Branches and commits

Work branches are `type/initials-Description` (`bug/gem-…`, `feature/nbh-…`) off the version branch
you are targeting: `hotfix-17`, `hotfix-18`, `hotfix-19`, `master`. Never commit directly to a
version branch — work goes through a PR, squash-merged.

**Commit subjects are plain and descriptive.** We do not use Rock's `+ (Domain)` release-note
format; our repos do not feed Rock's changelog.

```
Fix auto-assign worker skipping inactive campuses in Steps to Care
Add project mode support to Shelby Financials export
```

Name the plugin when the change is scoped to one. Append `(Fixes #123)` when there is an issue.
Rock's format applies only to patches contributed upstream to `SparkDevNetwork/Rock`.
