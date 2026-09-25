---
name: kfs-entity-model
description: >-
  Scaffold or review a KFS plugin entity model. Use when the user says "create an entity",
  "new entity model", "scaffold entity", "add a new table", or names a plugin entity —
  in any KFS plugin under KFSRockAssemblies/rocks.kfs.*. Prefer this over the core `entity-model`,
  which scaffolds Rock core entities and will produce the wrong attributes, table name and paths.
argument-hint: "Describe the entity (e.g. 'a CareFollowUp entity in rocks.kfs.StepsToCare with a CareNeedId FK and a FollowUpDate'), or say 'review'"
---

# KFS Plugin Entity Model

Plugin entities are **not** core entities. Work from Rock's skill, then apply the deltas here.

## Step 1 — Read Rock's material

The core `entity-model` skill is loaded alongside this one. Read its
`references/entity-patterns.md` for the entity template and annotation requirements, and its
`references/common-pitfalls.md` before finalising. Everything in them applies except where
contradicted below.

Also read `.claude/rules/kfs/kfs-plugin-model.md`, which is the authority for these deltas.

## Step 2 — Apply the deltas

**Three required additions** — a plugin entity missing any is broken, two of them silently:
`Rock.Data.IRockEntity` on the class, `[DataContract]` (it is in `RockContext`'s registration
filter), and `this.HasEntitySetName( "[Entity]" )` in the `EntityTypeConfiguration` constructor.

| Rock's step | Core | KFS plugin |
|---|---|---|
| Does it exist | `Rock/Model/**/{Entity}.cs` | `KFSRockAssemblies/rocks.kfs.*/Model/{Entity}.cs` |
| SystemGuid | `Rock/SystemGuid/EntityType.cs` | `rocks.kfs.[Plugin]/SystemGuid/[Type].cs`, namespace `rocks.kfs.[Plugin].SystemGuid`. **Named constant only** — not the inline-GUID variant. |
| File path | `Rock/Model/[Domain]/[Entity]/[Entity].cs` | `rocks.kfs.[Plugin]/Model/[Entity].cs` |
| Namespace | `Rock.Model` | `rocks.kfs.[Plugin].Model` |
| Class attributes | five, incl. `[CodeGenerateRest]` | four — **omit `[CodeGenerateRest]`** |
| Table name | singular, no prefix | `_rocks_kfs_[Plugin]_[Entity]`; PK `PK__rocks_kfs_[Plugin]_[Entity]` |
| Enums | `Rock.Enums/[Domain]/`, `[EnumDomain]` | owning plugin project, **no `[EnumDomain]`** |
| Options POCO | `Rock/Model/[Domain]/[Entity]/Options/` | `rocks.kfs.[Plugin]/Model/Options/` |
| Next steps | run `Rock.CodeGeneration` | **skip it** — it does not run over plugin assemblies. Hand-write the service class if needed. |

**Keep `[RockDomain]` and `[Rock.SystemGuid.EntityTypeGuid]`.** Both work on plugin entities —
`EntityTypeService.RegisterEntityTypes()` reflects over plugin assemblies and `CreateFromType` reads
the GUID attribute. Registering only via a migration's `UpdateEntityType()` is historical practice,
not a requirement.

**`[RockDomain]` values come from `kfs-domains.md`**, not from the list in
`rock/code-conventions.md` § Rock Domain Names and not from the commit-message domain list — all
three differ.

## Corrections to Rock's pitfalls

- **Pitfall 2** lists five required attributes. For plugins it is the four above plus `IRockEntity`;
  `[CodeGenerateRest]` is not one of them.
- **Pitfall 8** says table names carry no prefix. Core-only — plugin tables must carry
  `_rocks_kfs_[Plugin]_`.

## Step 3 — Next

The table comes from `/kfs-plugin-migration`, never `/plugin-migration` or `/migration`. Use the typed schema API there, not
raw `CREATE TABLE` SQL.
