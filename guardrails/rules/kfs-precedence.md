# KFS Precedence

**Rock's conventions are the standard.** The `rock/` rules loaded alongside these are Spark
Development Network's Rock v20 guardrails, junctioned in unmodified from a `Rock20` clone. Follow
them for everything except the points listed below.

Rules are context, not enforced configuration, and two rules that contradict each other resolve
arbitrarily. So the conflicts are named here explicitly rather than left to inference.

---

## Where a `kfs-*` rule beats a `rock/` rule

| Conflict | `rock/` says | We do | Authority |
|---|---|---|---|
| Copyright header | Spark / Rock Community License | KFS / Apache 2.0 on new KFS files; Spark's retained on core-derived files | `kfs-licensing.md` |
| Entity class attributes | five, incl. `[CodeGenerateRest]` | four + `IRockEntity` + `HasEntitySetName()`; omit `[CodeGenerateRest]` | `kfs-plugin-model.md` |
| Table names | singular PascalCase, no prefix | `_rocks_kfs_[Plugin]_[Entity]` — prefix mandatory | `kfs-plugin-model.md` |
| `[RockDomain]` values | the namespace list in `rock/code-conventions.md` | a different, measured list | `kfs-domains.md` |
| Plugin migrations | `Rock/Plugin/HotFixes/`, one global sequence | `rocks.kfs.[Plugin]/Migrations/`, restarting at 001 per plugin | `kfs-migrations.md` |
| Commit format | `+ (Domain) Message.` | plain descriptive subject | `kfs-repos.md` |
| File locations | `Rock/Model/`, `Rock.Blocks/`, … | the KFS trees | `kfs-repos.md` |
| Build | `dotnet build Rock.sln` | `nuget restore` + `msbuild KFSRock*.sln` | `kfs-repos.md` |

Everything not in that table follows Rock: naming, code style, logging, SQL formatting, Obsidian
and TypeScript style, cascade rules, GUID casing, `RockDateTime`, `#if WEBFORMS`, the Options POCO
pattern, deprecation, and the `UpdateBlockTypeByGuid()` data-loss warning.

## Skill routing

`/migration` is Rock core's EF migration skill and opens by saying so. For KFS work use
`/plugin-migration`. Rock's skills load as `rock-*`; where a `kfs-*` variant exists, prefer it.

## Existing KFS code is not evidence

Our plugins predate these guardrails and some do not conform. A conformance project will assess
them separately. Do not infer a convention from surrounding code when this file and the `rock/`
rules say otherwise, and do not "fix" unrelated non-conforming code while doing other work.

## This list is closed

If something seems to need a new deviation, raise it rather than inventing one.
