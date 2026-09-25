---
paths:
  - "KFSRockAssemblies/rocks.kfs.*/Migrations/**"
  - "KFSRockAssemblies/rocks.kfs.*/sql/**"
---

# KFS Plugin Migrations

Rock core has two migration systems; plugins use a third. `Rock/Plugin/HotFixes/` is **Rock core's
own** hotfix stream — we never write there. `Rock.Migrations/` is core EF and has no surface here.

| | Rock core | KFS plugin |
|---|---|---|
| Location | `Rock/Plugin/HotFixes/[NNN]_[Name].cs` | `KFSRockAssemblies/rocks.kfs.[Plugin]/Migrations/[NNN]_[Name].cs` |
| Numbering | one global sequence | **restarts at 001 per plugin**, independent of every other plugin and of Rock's sequence |
| Namespace | `Rock.Plugin.HotFixes` | `rocks.kfs.[Plugin].Migrations` |
| Large SQL | `HotFixMigrationResource` .resx | no plugin has one; keep inline or add a .resx to that plugin |

Base class `Rock.Plugin.Migration`, `[MigrationNumber( N, "minRockVersion" )]` where `N` matches the
filename prefix. `Down()` carries Rock's standard plugin-migration comment; down migrations are not
supported in the plugin system.

Identify the owning plugin first, then scan only that folder for the next number. Twelve plugins
currently have migrations; `StepsToCare` is highest at 025. If a plugin has no `Migrations/` folder
yet, migration `001` creates it.

**Minimum version string** uses Rock's current scheme, not the legacy `1.x` form. Rock core stamps
`"17.4"`, `"19.5"`. Existing KFS migrations top out at `"1.16.0"` because they predate v17 — do not
copy that form forward.

---

## Use the typed schema API

`Rock.Plugin.Migration` exposes the full EF-style surface: `CreateTable<TColumns>`, `AddTable`,
`AddColumn`, `AddPrimaryKey`, `AddForeignKey`, `AddIndex`, `DropColumn`, `DropIndex`,
`DropPrimaryKey`, `DropForeignKey`, `DropTable` — alongside `Sql()`, `SqlScalar()` and the full
`RockMigrationHelper`.

Prefer the typed methods for schema, exactly as Rock core does. Reserve `Sql()` for data migrations,
platform entities the helper does not cover, and stored procedures.

> Several existing KFS migrations build tables with raw `Sql( "CREATE TABLE ..." )`. That is drift,
> not a plugin requirement — do not copy it forward.

Remember the table prefix and PK naming when creating tables:
`_rocks_kfs_[Plugin]_[Entity]` / `PK__rocks_kfs_[Plugin]_[Entity]`. See `kfs-plugin-model.md`.

---

## `Add*` vs `Update*` vs `AddOrUpdate*`

Migrations replay against live customer databases, often on upgrade paths where a record may or may
not already exist. Picking wrong fails in two silent directions: `Add*` where the record exists
duplicates or throws; `Update*` where it does not exist no-ops and the migration "succeeds" having
done nothing. Neither surfaces at build time.

- **Prefer the `AddOrUpdate*` form where one exists** — `AddOrUpdateEntityBlockType()`,
  `AddOrUpdateBlockTypeAttribute()`, `AddOrUpdatePageRoute()`,
  `AddOrUpdatePersonAttributeByGuid()`. These are idempotent upserts by GUID.
- **For defined values, prefer `UpdateDefinedValue()` over `AddDefinedValue()`** when the value may
  already be present from an earlier migration or a customer edit.
- **Guard bare `Add*` calls** with an existence check, or accept that the migration is not replayable.

Two methods documented in `rock/`'s reference do **not exist** in `MigrationHelper` on any version
we target — `UpdateDefinedType()` and `AddSecurityAuthForBlockType()`. Calling either is a compile
error. That is an upstream documentation error, not ours.

## Block registration

Path-based for WebForms, entity-based for Obsidian:

```csharp
RockMigrationHelper.UpdateBlockType( "Care Entry", "…",
    "~/Plugins/rocks_kfs/StepsToCare/CareEntry.ascx",   // path
    "KFS > Steps To Care",                              // must match the [Category] attribute
    "4F0F9ED7-9F74-4152-B27F-D9B2A458AFBE" );
```

`AddOrUpdateEntityBlockType()` for entity-based blocks. **Never** `UpdateBlockTypeByGuid()` for one —
it runs `DELETE FROM [BlockType] WHERE [Path] = '{path}'`, and entity-based types have an empty path.
See `rock/data-model.md`.

## Shipped SQL objects

Stored procedures and views live in `rocks.kfs.[Plugin]/sql/`, named `_rocks_kfs_sp*`, filename
matching the object name. They are created or altered **by a migration** — a `.sql` file in the repo
runs nowhere on its own. Use the stored-procedure pattern in `rock/`'s
`plugin-migration/references/methods-and-sql.md`, including the ANSI_NULLS / QUOTED_IDENTIFIER
save-and-restore.
