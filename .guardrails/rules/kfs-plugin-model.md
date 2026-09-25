---
paths:
  - "KFSRockAssemblies/rocks.kfs.*/**"
  - "RockWeb/Plugins/rocks_kfs/**"
---

# KFS Plugin Model

Overrides the entity and block sections of `rock/data-model.md` and `rock/block-architecture.md`.
Everything in those files not contradicted here still applies in full — `Model<T>` vs `Entity<T>`,
standard columns, PersonAlias vs Person, cascade conventions, GUID format, RockContext usage, LINQ
guidance, and the `UpdateBlockTypeByGuid()` data-loss warning.

---

## Plugin entities

```csharp
[Table( "_rocks_kfs_StepsToCare_CareNeed" )]   // prefix is mandatory
[DataContract]                                  // in RockContext's registration filter
[RockDomain( "Core" )]                          // value list: kfs-domains.md
[Rock.SystemGuid.EntityTypeGuid( "87AC878D-6740-43EB-9389-B8440AC595C3" )]
public partial class CareNeed : Rock.Data.Model<CareNeed>, Rock.Data.IRockEntity
{
```

```csharp
public partial class CareNeedConfiguration : EntityTypeConfiguration<CareNeed>
{
    public CareNeedConfiguration()
    {
        this.HasRequired( cn => cn.PersonAlias ).WithMany()
            .HasForeignKey( cn => cn.PersonAliasId ).WillCascadeOnDelete( false );

        // Required for plugin entities. Core takes its entity set name from RockContext's
        // DbSet properties; plugins are registered dynamically by RegisterEntityType().
        this.HasEntitySetName( "CareNeed" );
    }
}
```

### Three required additions

A plugin entity missing any of these is broken, two of them silently:

| Requirement | Why |
|---|---|
| `Rock.Data.IRockEntity` | Rock's own documented plugin hook: *"Apply this to your plugin entities so that they can be loaded using RockContext… should **not** be used in Rock core, only plugins."* `RockContext` reflects over it to register plugin types. |
| `[DataContract]` | In `RockContext`'s filter predicate — an `IRockEntity` without it is silently never registered. |
| `HasEntitySetName( "[Entity]" )` | Core entities get their set name from `DbSet` properties; plugin entities are registered dynamically by `modelBuilder.RegisterEntityType()`, which does not. Pass the **unprefixed** name. |

### Changed from core

| | Rock core | KFS plugin |
|---|---|---|
| Table name | singular PascalCase, no prefix | `_rocks_kfs_[Plugin]_[Entity]` — marks it plugin-owned so Rock upgrades leave it alone |
| PK constraint | `PK_dbo.CareNeed` | `PK__rocks_kfs_[Plugin]_[Entity]` |
| Namespace | `Rock.Model` | `rocks.kfs.[Plugin].Model` |
| Options POCO | `Rock/Model/[Domain]/[Entity]/Options/` | `rocks.kfs.[Plugin]/Model/Options/` |

**Omit `[CodeGenerateRest]`** — it drives `Rock.CodeGeneration`, a tool that runs over Rock core
only. For the same reason, skip the "run CodeGeneration" step after scaffolding and hand-write the
service class if one is needed.

**Keep `[RockDomain]` and `[EntityTypeGuid]`.** Both work on plugin entities:
`EntityTypeService.RegisterEntityTypes()` reflects over all loaded assemblies including plugins, and
`CreateFromType` reads `[EntityTypeGuid]` to set the `EntityType.Guid`. Registering an EntityType
*only* through a migration's `UpdateEntityType()` call is historical KFS practice, not a requirement.

Also keep unchanged: `partial`, `[Required]`, `[MaxLength]`, `[DataMember]`, XML docs on every
property, `virtual` navigation properties, explicit `WillCascadeOnDelete( false )`, uppercase
GUIDs, and `IOrdered` / `IHasActiveFlag` / `ICacheable` where they apply.

Cascading from a core table into a plugin table is the most dangerous FK case: a routine core delete
silently removes customer data the plugin owns.

---

## Plugin blocks

**A KFS block follows the architecture of the core Rock block of similar function.** Find the
nearest core equivalent and follow it. `rock/block-architecture.md` applies verbatim — vertical
`FieldAttribute` declarations, keys as constants in nested `AttributeKey` / `PageParameterKey` /
`PersonPreferenceKey` classes, `PageParameter( PageParameterKey.X )` never `Request.Params`,
`LinkedPageUrl()` for navigation.

Structural differences only:

| | Rock core | KFS |
|---|---|---|
| WebForms path | `RockWeb/Blocks/[Domain]/` | `RockWeb/Plugins/rocks_kfs/[Domain]/` |
| WebForms namespace | `RockWeb.Blocks.[Domain]` | `RockWeb.Plugins.rocks_kfs.[Domain]` |
| Obsidian C# / bags | `Rock.Blocks/`, `Rock.ViewModels/Blocks/` | within the owning `rocks.kfs.[Plugin]` |
| Obsidian Vue | `Rock.JavaScript.Obsidian.Blocks/src/[Domain]/` | `rocks.kfs.JavaScript.Obsidian/src/` |
| `[Category]` | `"Core"` | `"KFS > Core"` — see `kfs-domains.md` |

Base is `Rock.Web.UI.RockBlock`; add `ISecondaryBlock` when the block should hide with its host,
`ICustomGridColumns` when it exposes an extensible grid.

Use the nested `AttributeKey` / `PageParameterKey` constant classes on **all** new blocks. Roughly a
third of existing blocks use literal strings; that is drift, not a convention. Match the surrounding
block when editing one, but do not convert half of it during unrelated work.

Icons are version-dependent — see `kfs-rock-versions.md` before writing one.
