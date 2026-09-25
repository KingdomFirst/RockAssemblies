# Rock Version Targets

KFS ships against more than one Rock major at a time. This file is the single source of truth for
what differs between them.

**The guardrails are sourced from a `Rock20` clone regardless of what you are building against.**
That is deliberate — Rock's conventions are current, and the version-specific deltas live here. Do
not conclude from the guardrail source that you are targeting v20. A v19 clone tracks an early
`CLAUDE.md` of Rock's own; `setup.ps1` skips it so it does not compete with Rock20's.

**Never state a version-specific pattern as if it were universal.** If a row below differs, the
answer depends on the clone you are in — establish that first.

---

## Which version am I on?

```bash
grep -hE 'AssemblyInformationalVersion|<InformationalVersion>' Rock.Version/AssemblySharedInfo.cs Directory.Build.props 2>/dev/null
```

v17 and v18 stamp the version in `Rock.Version/AssemblySharedInfo.cs`; v19 moved it to
`Directory.Build.props`. The command reads whichever the clone has.

Read it from the clone, not from the branch name — a `hotfix-17.9` branch can still stamp `17.8.2`
while a release is in preparation.

### Version branches

| KFS branch | Targets | Rock clone |
|---|---|---|
| `master` | The latest **generally available** Rock version, as Spark defines it | That version's clone |
| `hotfix-NN` | An **Early Access** Rock version NN | `RockNN` |

- **`master` changes version over time.** When a new Rock version becomes generally available,
  `master` moves to it. As of September 2026 that version is **17.8**, so `master` builds in
  `Rock17`. Check it before relying on it.
- **`hotfix-NN` branches are created only when needed.** One exists only when a repo needs changes
  for Early Access version NN that won't work on `master`. Not every Rock version gets one, and not
  every repo gets one. Check with `git -C <repo> branch -a` rather than assuming.
- **When version NN becomes generally available,** `hotfix-NN` is merged into `master` and then
  deleted. A branch can outlive its merge: `hotfix-17` has been merged but not yet deleted. Never
  target a hotfix branch whose version is now generally available. Use `master`.

If the KFS branch and the clone disagree, **stop and tell the user**. Examples are `hotfix-18`
checked out inside `Rock17`, or `master` inside `Rock18` while 17.8 is the generally available
version. That combination builds against the wrong assemblies, and every decision below becomes
unreliable.

---

## The gate

Rock v18 was the modernization release. Almost everything here changed at 17 → 18, not 18 → 20.
Treat "v18+" as one bucket unless a row says otherwise.

| Topic | v17 | v18+ |
|---|---|---|
| **Icon set** | FontAwesome — `fa fa-calendar` | Tabler — `ti ti-calendar` |
| **Icon sizing** | `fa-2x`, `fa-fw` | `ti-2x`, `fw` |
| **Styling system** | LESS (`RockWeb/Styles/*.less`) | SCSS `styles-v2` |
| **Rock utility classes** (`.gap-spacing-xs`, `.mb-spacing-sm`, `.bg-interface-softer`) | **Do not exist** | Available |
| **CSS design tokens** (`--spacing-*`, `--color-interface-*`, `--rounded-*`, `--font-size-*`) | Available (`_rock-core.less`) | Available |
| **`ContentSection` / `ContentStack` / `ContentSectionContainer`** | **Do not exist** — use `<fieldset>` | Available |
| **`safeParseJson`** (`@Obsidian/Utility/stringUtils`) | **Absent** | Available |
| **`AddOrUpdateLavaShortcode()` / `DeleteLavaShortcode()`** | **Absent** from `MigrationHelper` | Available |
| **Rock core project format** | legacy csproj + `packages.config` | SDK-style `net472` |

> **KFS plugin projects are legacy csproj on every version** — all 37, plus 7 `packages.config`
> files. `dotnet build` does not build them regardless of clone. Always `nuget restore` + `msbuild`.

### v19+ — do not use on v17 or v18

| Topic | Note |
|---|---|
| `getCombinedFilterValue` on `HighlightDetailColumn` | Absent in **both** v17 and v18. There a name+subtitle column still needs its own `filterValue` / `quickFilterValue` / `sortValue` props — do not delete them. |
| `Rock.Frontend.Styles` project | v19 relocated the SCSS sources here. |
| `Rock/Model/AI/`, `Rock.Blocks/AI/`, MCP integration | v19+. `[RockDomain( "AI" )]` is v18+. |

---

## Icons

The most common version mistake, because it is silent — a wrong prefix renders nothing, with no
error and no fallback. Tabler does not exist in the 17.x line at all: no font, no stylesheet, no
`.ti-` class. Rock's own v17 theme CSS carries a developer note anticipating the switch as future
work.

```csharp
RockMigrationHelper.AddPage( true, parentGuid, layoutGuid, "Steps to Care", "",
    pageGuid, "fa fa-hand-holding-heart" );   // v17
    // pageGuid, "ti ti-heart-handshake" );   // v18+
```

For a plugin that must run on both, prefer an icon present in both sets, or drive it from a block
setting.

## Styling

On **v17** the `/css-cleanup` priority order collapses — utility classes do not exist, so tokenized
scoped styles (`gap: var(--spacing-xsmall)`) are the only reachable option. The tokens themselves
are real on v17, so the work is still worth doing. On **v18+** prefer the utility class and drop the
scoped rule entirely.

## Assembly-qualified names in migrations

Only relevant when registering an **entity-based** block type; path-based `UpdateBlockType()` takes
no version. The scheme changed: `1.17.0.32` (legacy) → `17.1.1.0` → `18.0.9.0` → `20.0.7.0`.
**Never hard-code from memory** — copy from the newest migration on the branch:

```bash
grep -rhoE 'Rock\.Blocks, Version=[0-9.]+' "Rock.Migrations/Migrations/Version "*/ | sort -u | tail -1
```

`AddOrUpdateEntityBlockType()` avoids needing the string at all.

When something new differs by version, add it to the gate table above rather than inline in several
files, and link to it. If you cannot confirm which versions a pattern applies to, say so rather than
guessing.
