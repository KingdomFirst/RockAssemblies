# KFS Claude Code Guardrails

The curated KFS layer. Rock's own guardrails are **not** stored here — they are loaded unmodified
from a `Rock20` clone at setup time.

## Setup

Once per Rock clone:

```powershell
powershell -ExecutionPolicy Bypass -File `
    C:\KFSRepo\Rock\KFSRockAssemblies\guardrails\setup.ps1 -Target C:\KFSRepo\Rock\Rock17
```

That builds `<clone>\.claude\` and `<clone>\CLAUDE.md`, adds both to the clone's
`.git\info\exclude`, and wires all skills into `~\.claude\skills\`. Re-run it after pulling
guardrail changes, adding a skill, or updating the `Rock20` clone. It is idempotent.

Then start Claude Code from the **Rock clone root** — never from inside `KFSRockAssemblies`.

**Approve the external imports once per clone.** The generated `CLAUDE.md` imports files outside
the clone, and Claude Code silently skips those until you approve them. The desktop app's Code tab
never shows that prompt, so run the `claude` CLI once from the clone root and accept it. Or set
`hasClaudeMdExternalIncludesApproved` to `true` under `projects["C:/KFSRepo/Rock/<clone>"]` in
`~\.claude.json`. To check, ask a fresh session (no tools) whether its context contains "Rock RMS is
an open-source church management system". If it doesn't, the imports did not load.

Prerequisites: a `Rock20` clone at `C:\KFSRepo\Rock\Rock20` (guardrail source only — nobody builds
it), and the repo junctions from `CreateLinks.bat`.

## How it fits together

```
Rock20\.claude\rules\     ──┐
                            ├──> <clone>\.claude\rules\{rock,kfs}\   (junctions)
guardrails\rules\         ──┘

Rock20\CLAUDE.md          ──┐
                            ├──> <clone>\CLAUDE.md   (generated, @imports both)
guardrails\CLAUDE-kfs.md  ──┘

Rock20\.claude\skills\    ──┐
                            ├──> ~\.claude\skills\   (junctions, machine-wide)
guardrails\skills\        ──┘
```

Nothing is copied into a KFS repo, so pulling `Rock20` refreshes Rock's guardrails with no merge.

## What's here

| | |
|---|---|
| `rules/kfs-*.md` | The curated delta. `kfs-precedence.md` is the override contract. |
| `skills/kfs-*/` | Variants for the three skills where Rock's would misfire. |
| `commands/` | `/build`, `/test`, `/check` |
| `hooks/` | `prevent-destructive.sh` |
| `CLAUDE-kfs.md` | Imported after Rock's `CLAUDE.md` |
| `setup.ps1` | Builds the overlay |
| `specs/`, `docs/` | Where `/spec` and `/docs` write. Not part of the overlay — content, not config. |

Rock's own specs and docs are **not** copied here. They are read-only reference in the `Rock20`
clone (`Rock20/specs/`, `Rock20/docs/`); 276 duplicated files were removed from this repo. See the
README in each folder.

## The principle

**Rock's conventions are the standard.** Departures live in `kfs-precedence.md` and nowhere else —
if a pattern is not listed there, follow Rock, *including when existing KFS code does otherwise*.
Our plugins predate these guardrails; a conformance project will assess them separately.

Rock's files are never edited, so they stay diffable against upstream. When adding a deviation, add
it to `kfs-precedence.md` as well as the rule that carries it.

## Editing

These files are tracked in `RockAssemblies`, so they move with the branch. If you switch to a
branch that predates them, the overlay's `kfs` junction goes empty. Either land `guardrails/` on the
branches you work from, or pin the overlay to a worktree:

```
git worktree add C:\KFSRepo\Rock\KFSRockAssemblies-guardrails master
setup.ps1 -Target ... -Guardrails C:\KFSRepo\Rock\KFSRockAssemblies-guardrails\guardrails
```

`setup.ps1` prints which branch the guardrails source is on, so a stale pin is visible.

## Team rules

- Don't edit `settings.json` without review — it governs permissions for everyone.
- When Claude makes a repeatable mistake, add it to the relevant rule, not to a one-off prompt.
- `RockPlugins` has its own guardrails. Don't cross-apply.
