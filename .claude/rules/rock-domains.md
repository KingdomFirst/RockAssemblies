# Rock Domains

Canonical list of Rock domains and their casing variants. Always loaded as a project instruction.

> **KFS:** This file is Rock v20's, verbatim. Read the disambiguation below before using any
> list in it — "domain" means three different things in this codebase and they are not
> interchangeable.

## KFS: Three things called "domain"

| # | Concept | Applies to KFS? | Correct values |
|---|---|---|---|
| 1 | **Release-note domain** — the `+ (Domain)` commit subject | **No.** Upstream Rock commits only; our repos use plain subjects | The List below |
| 2 | **`[RockDomain]` attribute** — entity grouping | **Yes**, on plugin entities | the measured list below — **not** The List |
| 3 | **Block `[Category]`** — where the block appears in Rock's UI | **Yes**, on plugin blocks | `KFS > [Area]`, see `.claude/rules/code-conventions.md` |

See `CLAUDE.md` § Commit Messages for #1 and `.claude/rules/plugin-deviations.md` § 2 and § 4
for #2 and #3.

### Valid `[RockDomain]` values

The table below implies `[RockDomain]` takes the PascalCase "Namespace / Enum form"
(`Cms`, `Crm`, `Lms`). **It does not.** These are the values actually present on Rock's
entities, measured from `Rock/Model` in the v17, v18 and v20 clones:

```
AI (v18+)   CMS       CRM        Check-in   Communication   Core
Engagement  Event     Finance    Group      LMS             Meta
Prayer      Reporting Security   WebFarm    Workflow
```

Differences from The List that will bite:

- **In The List but not valid `[RockDomain]` values:** `API`, `Connection`, `Farm`, `Lava`,
  `Mobile`, `Other`. Two are actively misleading — Rock's Connection entities are tagged
  `[RockDomain( "Engagement" )]`, and `Farm` is `WebFarm`.
- **Valid but absent from The List:** `Meta`, `Security`, `WebFarm`.
- **`AI` is v18+.** No v17 entity uses it.
- **v17 quirk:** v17's sole `Security` usage is `[RockDomain( "Security " )]` — with a trailing
  space, in `Rock/Model/Security/History/HistoryLogin.cs`. There are no clean `"Security"`
  usages in v17; it is corrected upstream by v18. Write `"Security"` without the space.

This is the single source of truth referenced by:
- The `bugfix` skill (release-note classification, path-to-domain mapping).
- The `spec` skill (`specs/completed/{folder}/` directory structure, INDEX.md `Domain` column).
- The `docs` skill (`docs/{folder}/` directory structure, README headings).
- Any future commit skill.
- The "Commit Messages" section of `CLAUDE.md`.

Do not duplicate this list anywhere else. Reference this file.

---

## The List

Three casings exist for the same domain. Use the form appropriate to the context.

| Release-note form | Folder name | Namespace / Enum form |
|---|---|---|
| `AI` | `ai` | `AI` |
| `API` | `api` | (no direct namespace; use `Net` or domain-specific) |
| `CMS` | `cms` | `Cms` |
| `Check-in` | `check-in` | `CheckIn` |
| `Communication` | `communication` | `Communication` |
| `Connection` | `connection` | `Connection` |
| `Core` | `core` | `Core` |
| `CRM` | `crm` | `Crm` |
| `Engagement` | `engagement` | `Engagement` |
| `Event` | `event` | `Event` |
| `Farm` | `farm` | `WebFarm` |
| `Finance` | `finance` | `Finance` |
| `Group` | `group` | `Group` |
| `Lava` | `lava` | `Lava` (under `Rock.Lava`) |
| `LMS` | `lms` | `Lms` |
| `Mobile` | `mobile` | `Mobile` |
| `Prayer` | `prayer` | `Prayer` |
| `Reporting` | `reporting` | `Reporting` |
| `Workflow` | `workflow` | `Workflow` |
| `Other` | `other` | (n/a) |

### Where each form is used

- **Release-note form** — commit messages (`+ (Domain) ...`), the `Domain` column of `specs/completed/INDEX.md`, the H1 of every `docs/{folder}/README.md`. Human-facing.
- **Folder name** — every directory under `specs/completed/` and `docs/`. Lowercase, hyphens for spaces, no exceptions. Path-safe and case-insensitive-filesystem-safe.
- **Namespace / Enum form** — C# `[RockDomain]` and `[Enums.EnumDomain]` attributes, file paths under `Rock.Enums/{Domain}/`, namespace placement. PascalCase. Includes some domains that do NOT appear in the release-note list (`Blocks`, `Controls`, `Geography`, `Net`, `Observability`, `Security`) because those are code organization, not user-visible feature areas. See `.claude/rules/code-conventions.md` for the complete namespace list.

---

## Path-to-Domain Mapping

When a change touches code at a known path, the domain is usually inferable. This is the same table the `bugfix` skill uses for picking a domain from a bug location.

| Path contains | Release-note domain |
|---|---|
| `/AI/` | `AI` |
| `/Api/` or REST controllers | `API` |
| `/Cms/` or `/Blocks/Cms/` | `CMS` |
| `/CheckIn/` | `Check-in` |
| `/Communication/` | `Communication` |
| `/Connection/` | `Connection` |
| `/Core/` | `Core` |
| `/Crm/` | `CRM` |
| `/Engagement/` | `Engagement` |
| `/Event/` | `Event` |
| `/WebFarm/` | `Farm` |
| `/Finance/` | `Finance` |
| `/Group/` | `Group` |
| `/Lava/` | `Lava` |
| `/Lms/` | `LMS` |
| `/Mobile/` | `Mobile` |
| `/Prayer/` | `Prayer` |
| `/Reporting/` | `Reporting` |
| `/Workflow/` | `Workflow` |
| Cross-cutting or unclear | `Other` |

When a change spans two paths, pick the one closest to the user-visible feature, not the supporting infrastructure. A bug in a `Finance` block that happens to live in a `Core` cache layer is a `Finance` change.

---

## Hard Rules

- **Do not invent new domains.** The list above is exhaustive for release-note and folder use. If a topic genuinely does not fit, use `Other` / `other`.
- **Do not mix forms within one context.** A folder name is always lowercase. A release-note domain is always release-note casing. Picking the wrong form is a bug.
- **Do not silently translate.** When a skill needs to convert between forms, it should be explicit about which form it is using and why.
