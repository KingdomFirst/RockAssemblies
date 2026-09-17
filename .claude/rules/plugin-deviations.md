# Plugin Deviations from Rock Core

**Rock core conventions are the standard.** Every other file in `.claude/rules/` is Spark's
Rock v20 guardrail content, kept verbatim so it can be diffed against upstream. This file is
the complete, closed list of places where KFS plugin development must depart from it, and why.

Always loaded.

---

## How to use this file

1. **Default to Rock.** If a pattern is not listed below, follow Rock's rule exactly — for
   entity annotations, block architecture, naming, logging, code style, SQL formatting,
   Obsidian conventions, all of it.
2. **A KFS plugin block should look like the core Rock block of similar function.** Find the
   nearest core equivalent (a list block, a detail block, a settings block) and follow its
   architecture. The plugin-specific parts are the path, namespace, category, and registration
   call — not the design.
3. **Existing KFS code is not evidence.** Our plugins predate these guardrails and some do not
   conform. A conformance project will evaluate them later. Until then, do not infer a
   convention from surrounding code when this file and Rock's rules say otherwise — and do not
   "fix" unrelated non-conforming code while doing other work.
4. **This list is closed.** If you believe something needs a new deviation, stop and raise it
   with the user rather than inventing one.

---

## 1. Licensing

**The only deviation with legal consequences. Get it right on every new file.**

| | Rock core | KFS plugin |
|---|---|---|
| New file | `Copyright by the Spark Development Network` / Rock Community License | `Copyright <year> by Kingdom First Solutions` / **Apache License 2.0** |

Full templates are inline in `.claude/rules/code-conventions.md` § Copyright Headers, which is
the one section of that file rewritten rather than inherited.

**Exception — files derived from Rock core keep Spark's header.** If a file's code originated
in Rock core, even heavily modified, Spark's copyright and the Rock Community License travel
with it. Do not relicense it and do not add a KFS line alongside. 35 of the 76 blocks in
`RockBlocks` are in this category and are correct as they stand.

A handful of files carry `Copyright by the Central Christian Church`. Same reasoning; leave
them alone.

---

## 2. Entity Model

Rock's `.claude/rules/data-model.md` applies in full — `Model<T>` vs `Entity<T>`, standard
columns, PersonAlias vs Person, cascade conventions, GUID format, RockContext usage, LINQ
guidance. These are the plugin-specific departures.

### Required additions

| Requirement | Why |
|---|---|
| Implement `Rock.Data.IRockEntity` | Rock's own documented plugin hook. Its doc comment: *"Apply this to your plugin entities so that they can be loaded using RockContext… should **not** be used in Rock core, only plugins."* `RockContext` reflects over `IRockEntity` to register plugin types. |
| `[DataContract]` on the class | Not optional for plugins. It is in `RockContext`'s filter predicate — an `IRockEntity` without `[DataContract]` is silently never registered. |
| `this.HasEntitySetName( "[Entity]" )` in the `EntityTypeConfiguration` constructor | Core entities are registered through `DbSet` properties on `RockContext`, which supply the entity set name. Plugin entities are registered dynamically via `modelBuilder.RegisterEntityType()`, which does not. Rock core never needs this; every KFS entity does. Pass the **unprefixed** entity name. |

### Changed

| | Rock core | KFS plugin |
|---|---|---|
| Table name | `[Table( "CareNeed" )]` — singular PascalCase, no prefix | `[Table( "_rocks_kfs_[Plugin]_[Entity]" )]` — the prefix marks it as plugin-owned so Rock upgrades leave it alone |
| PK constraint name | `PK_dbo.CareNeed` | `PK__rocks_kfs_[Plugin]_[Entity]` |
| File location | `Rock/Model/[Domain]/[Entity]/[Entity].cs` | `KFSRockAssemblies/rocks.kfs.[Plugin]/Model/[Entity].cs` |
| Namespace | `Rock.Model` | `rocks.kfs.[Plugin].Model` |
| GUID constants | `Rock/SystemGuid/[Type].cs` | `KFSRockAssemblies/rocks.kfs.[Plugin]/SystemGuid/[Type].cs`, namespace `rocks.kfs.[Plugin].SystemGuid` |
| Options POCO | `Rock/Model/[Domain]/[Entity]/Options/` | `rocks.kfs.[Plugin]/Model/Options/`, namespace `rocks.kfs.[Plugin].Model.Options` |

### Omit

`[CodeGenerateRest]` — consumed only by `Rock.CodeGeneration`, a WPF tool that runs over Rock
core. It has no effect on a plugin assembly.

### Keep — these are NOT deviations

`[RockDomain( "..." )]` and `[Rock.SystemGuid.EntityTypeGuid( "..." )]` both work on plugin
entities and should be used. `EntityTypeService.RegisterEntityTypes()` reflects over all loaded
assemblies including plugins, and `CreateFromType` reads `[EntityTypeGuid]` to set the
`EntityType.Guid`. Registering an EntityType *only* through a migration's `UpdateEntityType()`
call is historical KFS practice, not a requirement — declare the attribute as Rock does.

> **Do not take the `[RockDomain]` value from the commit-message domain list in `CLAUDE.md`.**
> They are different sets — that list has `API`, `Connection`, `Farm`, `Lava`, `Mobile` and
> `Other`, none of which are legal `[RockDomain]` values, and omits `Meta`, `Security` and
> `WebFarm`, which are. The valid list is in `.claude/rules/rock-domains.md`
> § "Valid `[RockDomain]` values", measured from Rock's own entities.

Also keep, unchanged: `partial` class keyword, `[Required]`, `[MaxLength]`, `[DataMember]` /
`[DataMember( IsRequired = true )]`, XML doc comments on every property, `virtual` navigation
properties, a complete `EntityTypeConfiguration` with explicit `WillCascadeOnDelete( false )`,
uppercase hyphenated GUIDs, and `IOrdered` / `IHasActiveFlag` / `ICacheable` where they apply.

---

## 3. Migrations

Rock core has two migration systems; plugins use a third.

| System | Location | Applies to us? |
|---|---|---|
| EF migrations | `Rock.Migrations/Migrations/Version NN.0/` | **No** — Rock core only |
| Core hotfixes | `Rock/Plugin/HotFixes/NNN_*.cs` | **No** — that is Rock's own hotfix stream, not ours |
| **Plugin migrations** | `KFSRockAssemblies/rocks.kfs.[Plugin]/Migrations/[NNN]_[Name].cs` | **Yes** |

- Base class `Rock.Plugin.Migration`, namespace `rocks.kfs.[Plugin].Migrations`.
- `[MigrationNumber( N, "minRockVersion" )]` where `N` matches the filename prefix.
- **Numbering restarts at 001 per plugin** and is independent of every other plugin and of
  Rock's hotfix numbers.
- `Down()` carries Rock's standard plugin-migration comment; down migrations are not supported
  in the plugin system.

### Use the typed schema API — this is not a deviation

`Rock.Plugin.Migration` exposes the full EF-style surface: `CreateTable<TColumns>`,
`AddTable`, `AddColumn`, `AddPrimaryKey`, `AddForeignKey`, `AddIndex`, `DropColumn`,
`DropIndex`, `DropPrimaryKey`, `DropForeignKey`, `DropTable` — plus `Sql()`, `SqlScalar()` and
the full `RockMigrationHelper`.

Prefer the typed methods for schema, exactly as Rock core does. Reserve `Sql()` for data
migrations, platform entity work the helper does not cover, and stored procedures.

> Several existing KFS migrations build tables with raw `Sql( "CREATE TABLE ..." )`. That is
> drift, not a plugin requirement. Do not copy it into new migrations.

---

## 4. Blocks

**Follow the architecture of the core Rock block of similar function.** Rock's
`.claude/rules/block-architecture.md` applies verbatim: `FieldAttribute`s declared vertically
with property assignment, keys as constants in nested `AttributeKey` / `PageParameterKey` /
`AttributeCategory` / `PersonPreferenceKey` classes, `PageParameter( PageParameterKey.X )`
never `Request.Params`, `LinkedPageUrl()` for navigation.

Deviations are structural only:

| | Rock core | KFS plugin |
|---|---|---|
| WebForms path | `RockWeb/Blocks/[Domain]/` | `RockWeb/Plugins/rocks_kfs/[Domain]/` |
| WebForms namespace | `RockWeb.Blocks.[Domain]` | `RockWeb.Plugins.rocks_kfs.[Domain]` |
| Obsidian C# | `Rock.Blocks/[Domain]/` | `KFSRockAssemblies/rocks.kfs.[Plugin]/` |
| Obsidian Vue | `Rock.JavaScript.Obsidian.Blocks/src/[Domain]/` | `KFSRockAssemblies/rocks.kfs.JavaScript.Obsidian/src/` |
| Bags | `Rock.ViewModels/Blocks/[Domain]/[Block]/` | within the owning `rocks.kfs.[Plugin]` project |
| `[Category]` | `"Core"`, `"Finance"` | `"KFS > Core"`, `"KFS > Finance"` — must match the category in the migration's `UpdateBlockType()` call exactly |
| Registration path | `~/Blocks/[Domain]/X.ascx` | `~/Plugins/rocks_kfs/[Domain]/X.ascx` |

`UpdateBlockType()` for path-based (WebForms), `AddOrUpdateEntityBlockType()` for entity-based
(Obsidian). The `UpdateBlockTypeByGuid()` data-loss warning in `data-model.md` applies to us
identically.

Most KFS blocks are still WebForms. Converting one is a product decision, not cleanup.

---

## 5. Paths and Projects

- **Working root is the Rock clone**, not `KFSRockAssemblies`. See `CLAUDE.md` § Working Root.
- **Rock core trees are read-only reference** — `Rock/`, `Rock.Blocks/`, `Rock.ViewModels/`,
  `Rock.Enums/`, `Rock.Migrations/`, `RockWeb/Blocks/`. We do not fork Rock. If a task appears
  to require editing core, stop and raise it.
- **Enums** follow Rock's structural rules (domain folder, XML docs on every member, no root
  namespace) but live in the owning plugin project, because `Rock.Enums` is core. Omit
  `[Enums.EnumDomain]` — it is `internal` to `Rock.Enums` and drives core's TypeScript
  generation.
- **`[RockInternal]`** is a core-only visibility-staging attribute. Not applicable.

---

## 6. Build and Verification

| | Rock core | KFS plugin |
|---|---|---|
| Solution | `Rock.sln` | `KFSRock[Version].sln` at the Rock clone root |
| Build | `dotnet build` (v18+) | `nuget restore` + `msbuild` — all 37 plugin projects are legacy csproj on every Rock version |
| Tests | `Rock.Tests` | None exist. See `.claude/commands/test.md`. |

---

## 7. Commit Messages and Branches

Rock's `+ (Domain) Message.` format drives **Rock's** release-note generation. Our repositories
do not feed it, so we do not use it.

| | Rock core | KFS |
|---|---|---|
| Subject | `+ (Domain) Fixed …` / `- trivial` | plain descriptive subject, no prefix, no domain |
| Scope marker | the domain | **name the plugin** — it is how we scan history |
| Issue ref | `(Fixes #0000)` | `(Fixes #123)`, same |
| Branches | Rock's own | `type/initials-Description` off a version branch |

```
Fix auto-assign worker skipping inactive campuses in Steps to Care
Add project mode support to Shelby Financials export
```

Version branches (`hotfix-17`, `hotfix-18`, `hotfix-19`, `master`) take changes only through a
PR, squash-merged. Each KFS repo has its own branches and its own PR — see `CLAUDE.md` § Git:
three repositories, one working tree.

Rock's format still applies verbatim to patches contributed **upstream** to
`SparkDevNetwork/Rock`, including picking a domain from `.claude/rules/rock-domains.md`.

---

## 8. Common false deviations

Things that look plugin-specific but are not. Follow Rock on all of these:

| Trap | The rule |
|---|---|
| "Half our blocks use literal attribute keys" | Use the nested `AttributeKey` / `PageParameterKey` constant classes. Always. |
| "Our plugins log with `ExceptionLogService` everywhere" | Rock's guidance stands: `RockLogger.Log.<Level>( RockLogDomains.X, ... )` for structured logging, `ExceptionLogService.LogException()` for exceptions that belong in the Exception Log. |
| "Our migrations build tables with raw SQL" | Use the typed schema API. See § 3. |
| "Our entities register EntityTypes in migrations" | Declare `[Rock.SystemGuid.EntityTypeGuid]`. See § 2. |
| "Our entities have no `[RockDomain]`" | Add it. It works on plugin entities. |
| "`[RockObsolete]` is a Rock attribute" | It is `public` in `Rock.Common` and usable. Follow Rock's deprecation pattern: `[Obsolete()]` + `[RockObsolete( "X.Y" )]` with the Rock version you were targeting, plus an engineering note. |
| "Our `.obs` files skip the region structure" | Follow `.claude/rules/obsidian-conventions.md`. Our `.eslintrc.js` is a copy of Rock's, so the rules are identical. |
| "This existing file does it differently" | Not evidence. See § How to use this file, item 3. |
