# CLAUDE.md — KFS Rock Plugin Development Guidelines

> **Provenance.** This file and `.claude/rules/*` are Spark Development Network's Rock v20
> guardrails, kept as close to verbatim as possible so they can be diffed against upstream.
> **Rock's conventions are the standard.** Departures are confined to
> `.claude/rules/plugin-deviations.md`, which is a closed list — if something is not in it,
> follow Rock, including when existing KFS code does otherwise. Our plugins predate these
> guardrails and a conformance project will evaluate them later.

## Project Overview

Rock RMS is an open-source church management system. The codebase is C# (.NET) + TypeScript/Vue 3.
- **Obsidian** = the Vue 3 + C# block framework replacing legacy WebForms (.ascx) blocks.
- **Lava** = Rock's DotLiquid-based templating language.
- **WebForms** = legacy ASP.NET block system being phased out.

KFS builds **plugins** for Rock, across two repositories junctioned into a Rock clone:

`CreateLinks.bat` junctions **four** KFS repositories into the Rock clone:

| Repo | Junctioned to | Contains |
|---|---|---|
| `KingdomFirst/RockAssemblies` | `KFSRockAssemblies` | `rocks.kfs.*` projects — entities, jobs, workflow actions, field types, gateways, Obsidian source |
| `KingdomFirst/RockBlocks` | `RockWeb/Plugins/rocks_kfs` | WebForms blocks and built Obsidian output |
| `KingdomFirst/rock-attended-checkin` | `RockAttendedCheckin` **and** `RockWeb/Plugins/cc_newspring` | Attended check-in — **one repo, two junction points**: the C# project, and its 5 `.ascx` blocks under `cc_newspring/AttendedCheckin` |
| `KFS/rockassets` (self-hosted, `repo.kingdomfirstsolutions.com`) | `RockWeb/Content/KFSRockAssets` | Content library — ~278 Lava templates, 93 SQL scripts, 24 workflow exports, plus Less, XSL, ZPL |

**These guardrails were written for the first two.** The other two are in the working tree and
build alongside them (`rock-attended-checkin` is in `KFSRock*.sln`), but their conventions have
not been worked through:

- **`rock-attended-checkin` is a NewSpring fork.** Namespaces are `cc.newspring.AttendedCheckIn.*`
  and `RockWeb.Plugins.cc_newspring.*`, and its `.cs` files carry **no copyright header**. Our
  `rocks.kfs.*` naming, `_rocks_kfs_` table prefix and KFS/Apache header do **not** apply to it
  as they stand. Do not "conform" it — ask first.
- **`rockassets` is content, not compiled code.** Lava, SQL, workflow exports. The SQL
  formatting rules in `.claude/rules/code-conventions.md` and the Lava guidance in Critical
  Rules are the relevant parts; most of the rest of these guardrails is not.

A fifth repo, `KingdomFirst/RockPlugins` (branch `main`), holds the newer `rocks.kfs.Next.*`
products and the Obsidian block conversions already done there. It is **not** junctioned, uses
its own layout, and gets its own guardrails in a separate project.

**If a task turns out to belong to any repo other than the first two, say so** rather than
applying these conventions to it.

---

## Working Root

**Run Claude Code from the Rock clone root**, never from inside `KFSRockAssemblies\`.
`CreateLinks.bat` junctions `.claude` and links `CLAUDE.md` into that root, so the config is
version-controlled in our repo but loads with the whole tree visible. Plugin projects
reference `..\..\RockWeb\Bin\*.dll` and the Obsidian project builds through Rock's toolchain —
neither resolves outside the junction.

Every path in these rules and skills is relative to the Rock clone root. **Rock core trees are
read-only reference** — we do not fork Rock.

### Clone layout — a standard, not a preference

Rock clones and the KFS repos are siblings under one root:

```
C:\KFSRepo\Rock\
    Rock16\  Rock17\  Rock18\  Rock20\   <- Rock clones, named Rock<major>, no "V"
    KFSRockAssemblies\                    <- junctioned into whichever clone you work in
    KFSRockBlocks\
    RockAttendedCheckin\
    KFSRockAssets\
```

**Clone folders are `Rock<major>` — `Rock17`, not `RockV17`.** Tooling depends on it:
`rocks.kfs.JavaScript.Obsidian/build/build-types.js` reaches a sibling clone by the relative
path `../../../Rock17/…`, which resolves only under this naming. If a clone is named
differently, rename the folder rather than patching the tooling.

### Git: three repositories, one working tree

The working root is the Rock clone, but our code lives in two *other* repositories reached
through junctions. A bare `git` command run from the working root operates on **Rock's** repo,
which does not track our files — it reports `?? KFSRockAssemblies/` and shows none of your
actual changes.

**Always target the right repo with `-C`:**

| Working on | Command prefix | Repo |
|---|---|---|
| Plugins, and this config | `git -C KFSRockAssemblies ...` | `KingdomFirst/RockAssemblies` |
| Blocks | `git -C RockWeb/Plugins/rocks_kfs ...` | `KingdomFirst/RockBlocks` |
| Attended check-in (either junction) | `git -C RockAttendedCheckin ...` | `KingdomFirst/rock-attended-checkin` |
| Lava / SQL / workflow assets | `git -C RockWeb/Content/KFSRockAssets ...` | `KFS/rockassets` (self-hosted) |
| Rock core (read-only) | `git ...` | `SparkDevNetwork/Rock` |

This applies to `status`, `diff`, `log`, `branch`, `add`, `commit` — everything. A change that
spans two KFS repos needs a branch, PR and merge in each; they are versioned independently.

Note the third row: `RockAttendedCheckin` and `RockWeb/Plugins/cc_newspring` are two junctions
into the **same** repo, so a change touching both is still one commit. `git -C` against either
path resolves there.

Note also that the two repos keep their own branches. `git branch` from the working root lists
**Rock's** branches (including `develop` and Rock's own `feature-*`), not ours.

---

## Rock Version

KFS ships against several Rock majors at once. **Before writing code, establish which version
you are on** and read `.claude/rules/rock-version-targets.md`. Icons, styling, several
framework APIs and the build path differ between v17 and v18+.

---

## The Prime Directive

**Follow established patterns in the existing codebase.** Do not invent your own patterns. If you are aware of an alternative or newer pattern, state it explicitly but default to what already exists.

---

## When the Request Is Ambiguous

Before implementing, if the request has multiple reasonable interpretations, **stop and ask**. Don't silently pick.

- State assumptions you're making so the user can correct them.
- If you see two or more ways to read the task (e.g., "make it faster" could mean latency, throughput, or perceived UX), present the options and ask which one matters.
- If a simpler approach than what was asked fits the goal, say so before coding it the long way.

This does **not** apply to questions already settled by Rock conventions (rules, skills, or memory). Ask about **goals and scope**, not about conventions — looking up the answer yourself is faster than a round-trip.

---

## Project Architecture

Rock core layout, for reference when reading core code (read-only for us):

| What you're reading | Where it lives |
|---|---|
| C# block class | `Rock.Blocks/[Domain]/` |
| ViewModels / bags | `Rock.ViewModels/Blocks/[Domain]/[BlockName]/` |
| Obsidian Vue component | `Rock.JavaScript.Obsidian.Blocks/src/[Domain]/` |
| Obsidian partials | `Rock.JavaScript.Obsidian.Blocks/src/[Domain]/[blockName]/` |
| Entity model | `Rock/Model/[Domain]/[EntityName]/` |
| Enums | `Rock.Enums/[Domain]/` |
| Migrations | `Rock.Migrations/Migrations/` |
| SystemGuid constants | `Rock/SystemGuid/` |

KFS plugin layout — the same structures, relocated. Paths from the Rock clone root:

| What you're creating | Where it goes |
|---|---|
| WebForms block | `RockWeb/Plugins/rocks_kfs/[Domain]/[BlockName].ascx` + `.ascx.cs` |
| Obsidian source | `KFSRockAssemblies/rocks.kfs.JavaScript.Obsidian/src/` |
| C# block class / bags | within the owning `KFSRockAssemblies/rocks.kfs.[Plugin]/` |
| Entity model | `KFSRockAssemblies/rocks.kfs.[Plugin]/Model/[EntityName].cs` |
| Enums | `KFSRockAssemblies/rocks.kfs.[Plugin]/` — Rock's structural rules, plugin assembly |
| Migrations | `KFSRockAssemblies/rocks.kfs.[Plugin]/Migrations/[NNN]_[Name].cs` |
| SystemGuid constants | `KFSRockAssemblies/rocks.kfs.[Plugin]/SystemGuid/` |
| Jobs / workflow actions / field types | `KFSRockAssemblies/rocks.kfs.[Plugin]/Jobs/`, `rocks.kfs.Workflow.Action.[Domain]/`, `[Plugin]/Field/Types/` |

**A KFS plugin block follows the architecture of the core Rock block of similar function.**
Only the path, namespace, `[Category]` prefix and registration call differ. See
`.claude/rules/plugin-deviations.md`.

---

## Critical Rules

- **Never break backward compatibility** unless explicitly instructed.
- Use `RockDateTime` instead of `DateTime`. Format as ISO 8601: `RockDateTime.ToString("s")`.
- Do not add optional parameters that change a public method's signature — add a new overload and keep the original intact. (Plugins are not recompiled as often as core.)
- `System.Web` is the enemy — wrap any usage in `#if WEBFORMS` blocks. Obsidian blocks and shared code must not reference `System.Web`.
- Do not use delimiters for persisting configuration — use **JSON**.
- All page parameters (query string params) must be **PascalCase** (e.g., `AccountId`).
- Be intentional with `public`. Prefer `internal`, `protected`, or `private` to reduce breaking-change risk.
- Avoid `lock()` without first consulting the prompter. Use database-level unique constraints instead — clustered/web-farm environments make in-process locking unreliable.
- When passing custom objects to Lava, use `LavaDataObject` (not `RockDynamic`). Name custom Lava objects with an `Info` suffix (e.g., `CampusInfo`).
- Avoid `Guid` in LINQ `.Where()` clauses when `Id` is available (e.g., from cached items).
- Do not declare class variables on singletons — not thread-safe. Rock has many singletons (Workflow Actions, FieldTypes, Cache types, etc.).

---

## Naming Conventions

**C#:** PascalCase for classes/methods, camelCase for variables/params, `I` prefix for interfaces, underscore prefix for private fields (no Hungarian notation). Meaningful names — no abbreviations, no single-char variables (except `i` in loops).

**TypeScript:** PascalCase for classes/interfaces/enums/types, camelCase for functions/variables/params/**filenames**. The leading-underscore convention (`_unusedArg`) silences the unused-vars warning; do not use `_` as a general private-field marker. See `.claude/rules/obsidian-conventions.md` for the full Obsidian/TypeScript style guide.

---

## Booleans

- Names must answer a question: `IsActive`, not `Active`; `IsCategoryFieldVisible`, not `ShowCategoryField`.
- For Obsidian components, the **default value must be `false`** and the name must reflect that default (e.g., `IsPanelShown = false` for a normally-hidden panel).
- `Has` is acceptable instead of `Is` when it reads more naturally.

---

## Code Style

- Always use braces — even for single-line `if`, `for`, `else`.
- Use early returns to avoid nested `if` statements.
- Use variables to document intent of complex conditions rather than inline logic.
- Use `var` for consistency (except when the type cannot be inferred).

---

## Engineering Notes

When code is confusing or non-obvious, add a note explaining **why**:

```
/*
    3/5/26 - CLAUDE

    <Why this code exists or why this change was made.>

    Reason: <One-line summary for scanning.>
*/
```

---

## Copyright Headers

Always include the appropriate copyright header at the top of every new file. See `.claude/rules/code-conventions.md` for the exact templates.

---

## Commit Messages

> **KFS: the format below is Rock's, and applies only to patches contributed upstream to
> `SparkDevNetwork/Rock`.** Our own repositories use plain descriptive subject lines — see
> **KFS commits** at the end of this section. Do not draft a `+ (Domain)` message for a
> `RockAssemblies` or `RockBlocks` commit.
>
> **The domain list below is the release-note taxonomy — nothing else.** Three unrelated
> things are called "domain" here, and this is the one that applies to us least:
>
> | Concept | Applies to KFS? | Values |
> |---|---|---|
> | Release-note domain (**below**) | **No** — upstream Rock commits only | the list below |
> | `[RockDomain]` on an entity | **Yes** | a **different** set — `.claude/rules/rock-domains.md` § "Valid `[RockDomain]` values" |
> | Block `[Category]` | **Yes** | `KFS > [Area]` — `.claude/rules/code-conventions.md` |
>
> Do not source a `[RockDomain]` or a `[Category]` from the list below.

Commits use `+` (release notes) or `-` (trivial):

### Release note commits (`+`)

```
+ ([Domain]) [Message]. (Fixes #0000)
```

**Domain** must be exactly one of the following (wrapped in parentheses):
`AI`, `API`, `CMS`, `Check-in`, `Communication`, `Connection`, `Core`, `CRM`, `Engagement`, `Event`, `Farm`, `Finance`, `Group`, `Lava`, `LMS`, `Mobile`, `Prayer`, `Reporting`, `Workflow`, `Other`
IMPORTANT: You may only use one of the domains listed. You may not make new ones up.

**Starting word determines classification:**

| Starting Word | Classification |
|---|---|
| `Fixes` / `Fixed` | Bug Fix |
| `Improve` / `Improved` / `Updated` | Improvement |
| `Add` / `Added` | New Feature |

The message should be descriptive enough to serve as the full release note text. Append `(Fixes #0000)` if it resolves a tracked issue.

### Examples

```
+ (Core) Fixed the friendly schedule text display for single-date schedules. (Fixes #6694)
+ (Finance) Added support for ACH refunds on the NMI gateway.
+ (CRM) Improved the duplicate detection merge process to preserve giving records.
- Fixed typo in variable name.
- Removed unused using statement.
```

### KFS commits

**Our repositories do not use the `+ (Domain)` format.** Write a plain, descriptive subject
line that stands on its own in a PR:

```
Fix auto-assign worker skipping inactive campuses in Steps to Care
Add project mode support to Shelby Financials export
Update obsolete methods for Rock 18 compatibility
```

- Imperative or past tense, either is fine. No domain, no `+` / `-` prefix.
- **Name the plugin** when the change is scoped to one — it is the main way we scan history.
- Reference an issue with `(Fixes #123)` when there is one.
- PRs are squash-merged; the PR title is what lands on the version branch.

**Branches** — `type/initials-Description`, branched from the version branch you are targeting:

```
bug/gem-StepsToCare_AutoAssignWorker
feature/nbh-PageExportImportProcessor
release/nbh-CybersourceUpdate_v15
```

Version branches are `hotfix-17`, `hotfix-18`, `hotfix-19`, `master`. **Never commit directly
to one** — work goes through a PR. Remember that each KFS repo has its own branches and its
own PR; see § Git: three repositories, one working tree.
