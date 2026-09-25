---
name: kfs-plugin-migration
description: >-
  Create or review a KFS plugin migration. Use when the user says "plugin migration", "write a
  migration", "hotfix", "data fix", "add a page/block/attribute", or describes any schema or
  configuration change for a KFS plugin. Prefer this over the core `plugin-migration`, which writes to
  Rock core's own hotfix stream, and over `migration`, which is core EF and does not apply here.
argument-hint: "Describe what the migration should do, or say 'review'"
---

# KFS Plugin Migration

The mechanism is Rock's — `Rock.Plugin.Migration`, `[MigrationNumber]`, `Up()`/`Down()`,
`RockMigrationHelper`, the SQL rules, string escaping, the block-type safety warning, the review
severity model. Only the location, numbering and a few method choices differ.

## Step 1 — Read Rock's material

The core `plugin-migration` skill is loaded alongside this one. Read its
`references/hotfix-patterns.md` for patterns and worked examples, `references/methods-and-sql.md`
for method signatures and the stored-procedure pattern, and `references/common-pitfalls.md` before
finalising.

Note those examples use Rock core's namespace and numbering. The mapping is below; the examples are
otherwise correct.

Also read `.claude/rules/kfs/kfs-migrations.md`, the authority for these deltas.

## Step 2 — Locate it correctly

`Rock/Plugin/HotFixes/` is **Rock core's own** hotfix stream. We never write there.

| | Rock core | KFS plugin |
|---|---|---|
| Location | `Rock/Plugin/HotFixes/[NNN]_[Name].cs` | `KFSRockAssemblies/rocks.kfs.[Plugin]/Migrations/[NNN]_[Name].cs` |
| Numbering | one global sequence | **restarts at 001 per plugin** |
| Namespace | `Rock.Plugin.HotFixes` | `rocks.kfs.[Plugin].Migrations` |
| Copyright | Spark / RCL | KFS / Apache — `kfs-licensing.md` |

Identify the owning plugin first, then scan only that folder:

```bash
ls KFSRockAssemblies/rocks.kfs.[Plugin]/Migrations/*.cs
```

Twelve plugins have migrations; `StepsToCare` is highest at 025. If a plugin has no `Migrations/`
folder, migration `001` creates it.

**Minimum version string** uses Rock's current scheme — `"17.0"`, `"18.0"` — not the legacy `1.x`
form that existing KFS migrations use because they predate v17.

## Step 3 — Write it

**Use the typed schema API.** `Rock.Plugin.Migration` exposes `CreateTable<TColumns>`, `AddTable`,
`AddColumn`, `AddPrimaryKey`, `AddForeignKey`, `AddIndex`, `DropColumn`, `DropIndex`,
`DropPrimaryKey`, `DropForeignKey`, `DropTable` alongside `Sql()`. Prefer them for schema exactly as
Rock core does; reserve `Sql()` for data migrations, platform entities the helper does not cover,
and stored procedures.

> Several existing KFS migrations build tables with raw `Sql( "CREATE TABLE ..." )`. That is drift —
> do not copy it forward.

**Make it replayable.** Migrations run against live customer databases. Prefer the `AddOrUpdate*`
form where one exists; prefer `UpdateDefinedValue()` over `AddDefinedValue()` when the value may
already be present; guard bare `Add*` calls with an existence check. `UpdateDefinedType()` and
`AddSecurityAuthForBlockType()` are documented in Rock's reference but **do not exist** in
`MigrationHelper` — calling either is a compile error.

**Table and PK names** carry the plugin prefix — `_rocks_kfs_[Plugin]_[Entity]`,
`PK__rocks_kfs_[Plugin]_[Entity]`.

**Block registration** uses the plugin path and a matching category:

```csharp
RockMigrationHelper.UpdateBlockType( "Care Entry", "…",
    "~/Plugins/rocks_kfs/StepsToCare/CareEntry.ascx",
    "KFS > Steps To Care",   // must match the block's [Category] exactly
    "4F0F9ED7-9F74-4152-B27F-D9B2A458AFBE" );
```

`AddOrUpdateEntityBlockType()` for entity-based blocks. **Never** `UpdateBlockTypeByGuid()` for one.

Page and block icons are version-dependent — `kfs-rock-versions.md` before writing one.
