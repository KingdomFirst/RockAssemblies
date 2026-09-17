---
description: Pre-commit verification (build + test + diff review). Use before committing, when the user says "check", "verify", "pre-commit", or wants to validate changes before pushing.
---

Pre-commit verification. Run these checks in order and stop at the first failure:

1. **Build** — run `/build` (must succeed)
2. **Tests** — run `/test`. There is no KFS test suite; report what it says rather than
   treating a green run as verification.
3. **Diff review** — Run `git diff --cached` (or `git diff` if nothing is staged) and scan for:
   - Missing copyright headers on new files
   - `DateTime` usage instead of `RockDateTime`
   - `System.Web` references outside `#if WEBFORMS` blocks
   - `lock()` statements (flag for review)
   - Public methods that should be internal
   - Missing XML doc comments on public methods

Report a pass/fail summary. For failures, list each issue with file and line number.

---

## KFS additions to step 3

Everything above is Rock's list and applies unchanged. Add these, which cover the sanctioned
deviations in `.claude/rules/plugin-deviations.md`.

### Licensing — check first

- New KFS-authored file carrying Spark's header / Rock Community License → should be
  KFS / Apache 2.0.
- File derived from Rock core that has had Spark's header **replaced** with the KFS one →
  that is a relicensing error; restore Spark's.
- Wrong year on a new KFS header (should be the year the file was created).

### Plugin entity

- Missing `IRockEntity`, `[DataContract]`, or `HasEntitySetName( "[Entity]" )`.
- Table name missing the `_rocks_kfs_[Plugin]_` prefix.
- Missing `[RockDomain]` or `[Rock.SystemGuid.EntityTypeGuid]` — these **are** expected on
  plugin entities; their absence is drift, not a deviation.
- FK relationship without an explicit `WillCascadeOnDelete( false )`.
- `PersonId` where `PersonAliasId` belongs.

### Plugin migration

- `[MigrationNumber]` not matching the filename prefix, or colliding with an existing number
  in that plugin's `Migrations/` folder.
- Raw `Sql( "CREATE TABLE ..." )` for schema work — use the typed API (`CreateTable`,
  `AddColumn`, `AddForeignKey`, `AddIndex`). `Sql()` is for data, platform entities and
  stored procedures.
- `UpdateBlockTypeByGuid()` on an entity-based block — **data loss risk**.
- `[Category]` in the block not matching the category in `UpdateBlockType()`.
- Missing `IF NOT EXISTS` / `IF EXISTS` guards, or unescaped quotes/braces in `$@"..."` SQL.

### Version-sensitive

Per `.claude/rules/rock-version-targets.md`, against the branch's target version:

- Icon prefix (`fa fa-` on v17, `ti ti-` on v18+).
- APIs absent on the target: `safeParseJson`, `ContentSection` / `ContentStack`,
  `AddOrUpdateLavaShortcode()`, `styles-v2` utility classes.

### Branch and secrets

- Current branch must not be `master`, `hotfix-17`, `hotfix-18` or `hotfix-19` — those take
  changes only through a PR.
- Branch's target Rock version must match the clone you are in.
- No API keys, connection strings, tokens or credentials in the diff.

---

**Do not fix anything automatically.** This command reports; the user decides.

Close by naming what was *not* verified. Compilation plus the static checks above is the whole
of it — anything behavioural needs manual testing. Call out migrations specifically: they run
once against customer databases.
