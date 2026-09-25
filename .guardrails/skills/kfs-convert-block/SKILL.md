---
name: kfs-convert-block
description: >-
  Convert a KFS WebForms block to Obsidian, or review such a conversion. Use when the user says
  "convert block", "obsidian conversion", or names a block under RockWeb/Plugins/rocks_kfs.
  Prefer this over the core `convert-block`, whose paths, branch convention and validator all target
  Rock core.
argument-hint: "Domain/BlockName (e.g. Finance/BatchExportDetails)"
---

# KFS Block Conversion

**Confirm the repo first.** KFS Obsidian block conversion largely happens in
`KingdomFirst/RockPlugins`, which is a separate repository with its own guardrails and its own
layout. If that is what the user means, stop and say so — nothing here applies there.

This skill covers conversions in the junctioned split-repo tree only.

## Step 1 — Read Rock's material

The core `convert-block` skill is loaded alongside this one. Its conversion philosophy,
classification, base-class selection, bag rules, grid patterns, IdKey resolution and the
`PersonPicker` warning all apply unchanged — **a converted KFS block should look like the core Rock
block of similar function.** Read its `references/common-patterns.md` after classifying, plus the
type-specific reference, and read its canonical reference blocks in Rock core freely.

## Step 2 — Version gate

Check `.claude/rules/kfs/kfs-rock-versions.md` before designing. On **v17** specifically:

- `ContentSection` / `ContentStack` / `ContentSectionContainer` **do not exist** — the edit panel
  uses `<fieldset>`. Ignore `references/detail-block-patterns.md` § "UI Layout".
- Icons are `fa fa-`, not `ti ti-`.
- `safeParseJson` is absent, though `list-block-patterns.md` recommends it.
- `HighlightDetailColumn` has no `getCombinedFilterValue` on v17 **or** v18 — keep the explicit
  `filterValue` / `quickFilterValue` / `sortValue` props that reference tells you to delete.
- Rock utility classes do not exist; use CSS variables in scoped styles.

## Step 3 — Paths

| Rock core | KFS |
|---|---|
| `RockWeb/Blocks/[Category]/[Block].ascx` | `RockWeb/Plugins/rocks_kfs/[Domain]/[Block].ascx` |
| `Rock.Blocks/[Category]/[Block].cs` | within the owning `KFSRockAssemblies/rocks.kfs.[Plugin]` |
| `Rock.ViewModels/Blocks/[Category]/[Block]/` | within the owning plugin project |
| `Rock.JavaScript.Obsidian.Blocks/src/[Category]/[block].obs` | `rocks.kfs.JavaScript.Obsidian/src/` |
| generated `.d.ts` | no generator here — see below |

Namespace `RockWeb.Plugins.rocks_kfs.[Domain]`; `[Category]` is `"KFS > [Area]"` and must match the
migration's `UpdateBlockType()` argument exactly.

## Step 4 — Replace Rock's plan steps 1, 9 and 10

**Branch.** Rock's `feature-v[N]-claude-[name]` is not ours, and `git branch` at the working root
lists **Rock's** branches. Use:

```bash
git -C RockWeb/Plugins/rocks_kfs branch --show-current   # not master/hotfix-*
git -C RockWeb/Plugins/rocks_kfs checkout -b feature/[initials]-[Block]Obsidian [base]
```

`[base]` is `master` for the generally available Rock version, or `hotfix-NN` for an Early Access
version that has one. See `kfs-rock-versions.md` § Version branches.

**Chopping.** Deleting the `.ascx` removes a block customers have deployed. Do **not** chop without
explicit confirmation, and pair it with a migration that re-registers the block type against the
entity-based registration (`AddOrUpdateEntityBlockType()`, never `UpdateBlockTypeByGuid()`).

**Validation.** `scripts/validate-conversion.js` checks Rock core paths only and **refuses to run**
inside a KFS tree. Walk `references/implementation-details.md`'s file checklist by hand.

## Known blockers

`rocks.kfs.JavaScript.Obsidian` has one control and no blocks — there is no local precedent. Two
toolchain problems to check before relying on generated types: `build/build-types.js` is pinned to a
`Rock17` sibling clone, and `tsconfig.base.json` maps `@Obsidian/*` one level short of the clone
root. `Rock.CodeGeneration` does not run over plugin assemblies. Raise all of this before planning.
