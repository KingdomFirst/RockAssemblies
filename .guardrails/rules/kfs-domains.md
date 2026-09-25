# KFS Domains

Three unrelated things are called "domain" in this codebase and they are **not** interchangeable.
`rock/rock-domains.md` and `rock/code-conventions.md` each publish a list; neither is the right
source for `[RockDomain]`.

| # | Concept | Applies to KFS? | Correct values |
|---|---|---|---|
| 1 | **Release-note domain** — the `+ (Domain)` commit subject | **No.** Upstream Rock commits only; our repos use plain subjects | the list in `rock/rock-domains.md` |
| 2 | **`[RockDomain]` attribute** — entity grouping | **Yes**, on plugin entities | the measured list below |
| 3 | **Block `[Category]`** — where the block appears in Rock's UI | **Yes**, on plugin blocks | `KFS > [Area]`, below |

---

## Valid `[RockDomain]` values

Both inherited lists are wrong for this purpose. `rock/code-conventions.md` § Rock Domain Names
publishes Rock's *namespace* list (`Cms`, `Crm`, `Lms`, `Blocks`, `Controls`, `Geography`, `Net`,
`Observability`); no Rock entity uses those, and the casing is wrong for the four that do exist.

These are the values actually present on Rock's entities, measured from `Rock/Model` in the v17,
v18 and v20 clones:

```
AI (v18+)   CMS       CRM        Check-in   Communication   Core
Engagement  Event     Finance    Group      LMS             Meta
Prayer      Reporting Security   WebFarm    Workflow
```

Differences that will bite:

- **In the release-note list but not valid `[RockDomain]` values:** `API`, `Connection`, `Farm`,
  `Lava`, `Mobile`, `Other`. Two are actively misleading — Rock's Connection entities are tagged
  `[RockDomain( "Engagement" )]`, and `Farm` is `WebFarm`.
- **Valid but absent from the release-note list:** `Meta`, `Security`, `WebFarm`.
- **`AI` is v18+.** No v17 entity uses it.
- **v17 quirk:** v17's sole `Security` usage is `[RockDomain( "Security " )]` — trailing space, in
  `Rock/Model/Security/History/HistoryLogin.cs`, with no clean usages. Corrected upstream by v18.
  Write `"Security"` without the space.

## Block categories

KFS blocks are namespaced under a `KFS > ` prefix. The `[Category]` attribute must use one of:

```
KFS > Bulldozer          KFS > Event              KFS > Prayer
KFS > CMS                KFS > Eventbrite         KFS > Reporting
KFS > CRM                KFS > Finance            KFS > RSVP Groups
KFS > Check-in           KFS > Financial Edge     KFS > Security
KFS > Check-in > Manager KFS > Fundraising        KFS > Shelby Financials
KFS > Communication      KFS > Groups             KFS > Steps To Care
KFS > Connection         KFS > Import             KFS > Utility
KFS > Core               KFS > Intacct
```

The string in the `[Category]` attribute and in the migration's `UpdateBlockType()` call must match
exactly, or the block registers into a duplicate tree node. Adding a category means adding a
product area — confirm first.
