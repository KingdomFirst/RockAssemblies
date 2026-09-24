# Rock Version Targets

KFS ships against more than one Rock major at a time. This file is the single source of truth for
what differs between them.

**The guardrails are sourced from a `Rock20` clone regardless of what you are building against.**
That is deliberate — Rock's conventions are current, and the version-specific deltas live here. Do
not conclude from the guardrail source that you are targeting v20.

**Never state a version-specific pattern as if it were universal.** If a row below differs, the
answer depends on the clone you are in — establish that first.

---

## Which version am I on?

```bash
grep AssemblyInformationalVersion Rock.Version/AssemblySharedInfo.cs
```

Read it from the clone, not from the branch name — a `hotfix-17.9` branch can still stamp `17.8.2`
while a release is in preparation.

| KFS branch | Rock clone |
|---|---|
| `hotfix-17` | `Rock17` |
| `hotfix-18` | `Rock18` |
| `hotfix-19` | `Rock19` |
| `master` | latest |

If the KFS branch and the clone disagree — `hotfix-17` checked out inside `Rock18` — **stop and tell
the user.** That combination builds against the wrong assemblies and every decision below becomes
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

### v20-only — do not use on v17 or v18

| Topic | Note |
|---|---|
| `getCombinedFilterValue` on `HighlightDetailColumn` | Absent in **both** v17 and v18. There a name+subtitle column still needs its own `filterValue` / `quickFilterValue` / `sortValue` props — do not delete them. |
| `Rock.Frontend.Styles` project | v20 relocated the SCSS sources here. |
| `Rock/Model/AI/`, `Rock.Blocks/AI/`, MCP integration | v20 only. `[RockDomain( "AI" )]` is v18+. |

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
