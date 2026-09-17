# Rock Version Targets

KFS plugins ship against more than one Rock major version at a time. This file is the
single source of truth for what differs between them. Always loaded.

**Never state a version-specific pattern as if it were universal.** If a rule below has
different rows for v17 and v18+, the answer depends on the branch you are on — find out
before you write code.

---

## Which version am I on?

Determine it once, at the start of the task, from the Rock clone you are working in:

```bash
grep AssemblyInformationalVersion Rock.Version/AssemblySharedInfo.cs
```

Branch mapping in the KFS repos (`KFSRockAssemblies`, `KFSRockBlocks`):

| KFS branch | Rock clone | Rock version |
|---|---|---|
| `hotfix-17` | `RockV17` | McKinley 17.x |
| `hotfix-18` | `RockV18` | McKinley 18.x |
| `hotfix-19` | — | McKinley 19.x |
| `master` | latest | current development |

If the KFS branch and the Rock clone disagree (e.g. `hotfix-17` checked out inside
`RockV18`), **stop and tell the user** — that combination builds against the wrong
assemblies and every version-gated decision below becomes unreliable.

---

## The Gate

Rock v18 was the modernization release. Nearly everything in this table changed at
17 → 18, not at 18 → 20. Treat "v18+" as one bucket unless a row says otherwise.

| Topic | v17 | v18+ |
|---|---|---|
| **Icon set** | FontAwesome — `fa fa-calendar` | Tabler — `ti ti-calendar` |
| **Icon sizing** | `fa-2x`, `fa-fw` | `ti-2x`, `fw` |
| **Styling system** | LESS (`RockWeb/Styles/*.less`) | SCSS `styles-v2` |
| **Rock utility classes** (`.gap-spacing-xs`, `.mb-spacing-sm`, `.bg-interface-softer`) | **Do not exist** | Available |
| **CSS design tokens** (`--spacing-*`, `--color-interface-*`, `--rounded-*`, `--font-size-*`) | Available (defined in `_rock-core.less`) | Available |
| **`ContentSection` / `ContentStack` / `ContentSectionContainer`** | **Do not exist** — use `<fieldset>` | Available |
| **`safeParseJson`** (`@Obsidian/Utility/stringUtils`) | **Absent** | Available |
| **`AddOrUpdateLavaShortcode()` / `DeleteLavaShortcode()`** | **Absent** from `MigrationHelper` | Available |
| **Rock core project format** | Legacy csproj + `packages.config` (41 of 44) | SDK-style `net472` (35 of 42) |
| **Rock core build** | `nuget restore` + `msbuild` | `dotnet build` works per-project |

> **KFS plugin projects are legacy csproj on every version** — all 37 of them, plus 7
> `packages.config` files. `dotnet build` does not build them regardless of which Rock clone
> they sit in. Always `nuget restore` + `msbuild`. See `.claude/commands/build.md`.

### v20-only (do not use on v17 or v18)

| Topic | Note |
|---|---|
| `getCombinedFilterValue` on `HighlightDetailColumn` | Absent in **both** v17 and v18. On those versions a name+subtitle column still needs its own `filterValue` / `quickFilterValue` / `sortValue` props — do not delete them. |
| `Rock.Frontend.Styles` project | v20 relocated the SCSS sources here. |
| `Rock/Model/AI/`, `Rock.Blocks/AI/`, MCP integration | v20 only. |

---

## Icons

This is the single most common version mistake because it is silent — a wrong prefix
renders nothing, with no error.

```csharp
// Page and block icons in a plugin migration
RockMigrationHelper.AddPage( true, parentGuid, layoutGuid, "Steps to Care", "",
    pageGuid, "fa fa-hand-holding-heart" );   // v17
    // pageGuid, "ti ti-heart-handshake" );   // v18+
```

When a plugin must run on both, prefer an icon that exists in both sets, or drive it
from a block setting rather than hard-coding.

---

## Styling

On **v17**, the `/css-cleanup` decision tree collapses to one reachable branch:

1. ~~Rock utility classes~~ — do not exist
2. ~~Utility combinations~~ — do not exist
3. Block classes — `RockWeb/Styles/_blocks-*.less` (Rock core, read-only for us)
4. **Scoped styles with tokenized values** ← the only option

The tokens are real on v17, so this is still worth doing:

```css
/* correct on every version */
.care-need-header {
    gap: var(--spacing-xsmall);
    color: var(--color-interface-medium);
    border-radius: var(--rounded-medium);
    font-size: var(--font-size-small);
}
```

On **v18+**, prefer the utility class and drop the scoped rule entirely
(`class="d-flex gap-spacing-xs"`).

---

## Assembly-qualified names in migrations

Only relevant when registering an **entity-based** (Obsidian/Mobile) block type. Path-based
`UpdateBlockType()` calls — which is what almost every KFS block uses — take no version.

The scheme changed: `1.17.0.32` (legacy) → `17.1.1.0` → `18.0.9.0` → `20.0.7.0`.
**Never hard-code from memory.** Copy the string from the newest migration on the branch:

```bash
grep -rhoE 'Rock\.Blocks, Version=[0-9.]+' "Rock.Migrations/Migrations/Version "*/ | sort -u | tail -1
```

---

## Writing version-aware guidance

When you add to these rules or skills, and something differs by version:

- Put the difference **here**, in the table above — not inline in five different files.
- In the other file, link to it: `See .claude/rules/rock-version-targets.md § The Gate`.
- If you cannot confirm which versions a pattern applies to, say so rather than guessing.
