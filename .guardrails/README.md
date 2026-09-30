# KFS Claude Code Guardrails

The curated KFS layer. Rock's own guardrails are **not** stored here — they are loaded unmodified
from a `Rock20` clone.

## Setup

Once per Rock clone:

```powershell
powershell -ExecutionPolicy Bypass -File `
    C:\KFSRepo\Rock\KFSRockAssemblies\.guardrails\setup.ps1 -Target C:\KFSRepo\Rock\Rock17
```

It adds the guardrails **without writing over anything the clone tracks**, lists what it created in
the clone's `.git\info\exclude`, and wires all skills into `~\.claude\skills\`. Re-run it after
pulling guardrail changes, adding a skill, or updating the `Rock20` clone. It is idempotent, and it
migrates a clone set up by an earlier version. It checks everything before changing anything, so a
refusal leaves the clone as it was.

Then start Claude Code from the **Rock clone root** — never from inside `KFSRockAssemblies`.

**Approve the external imports once per clone.** The generated `CLAUDE.local.md` imports files
outside the clone, and Claude Code silently skips those until you approve them. The desktop app's
Code tab never shows that prompt, so run the `claude` CLI once from the clone root and accept it. Or
set `hasClaudeMdExternalIncludesApproved` to `true` under `projects["C:/KFSRepo/Rock/<clone>"]` in
`~\.claude.json`. To check, ask a fresh session (no tools) whether its context contains "Rock RMS is
an open-source church management system" and "Rock's conventions are the standard". If either is
missing, the imports did not load.

Prerequisites: a `Rock20` clone at `C:\KFSRepo\Rock\Rock20` (the guardrail source, whether or not
you also build in it), and the repo junctions from `CreateLinks.bat`.

## How it fits together

Rock's guardrails come from `Rock20` whatever version a clone targets. What `setup.ps1` adds depends
on what the clone already tracks:

| Clone | Rock tracks | `setup.ps1` adds |
|---|---|---|
| v17, v18 | nothing | Rock20's rules and `CLAUDE.md`, and the KFS layer |
| v19 | an early `CLAUDE.md` | the same, and skips the clone's `CLAUDE.md` through `claudeMdExcludes` |
| `Rock20` | `CLAUDE.md` and `.claude\` | the KFS layer only — Rock's guardrails are already there |

```
Rock20\.claude\rules\       ──> <clone>\.claude\rules\rock\    (junction; not in Rock20 itself)
.guardrails\rules\          ──> <clone>\.claude\rules\kfs\     (junction)

Rock20\CLAUDE.md            ──┐
                              ├──> <clone>\CLAUDE.local.md    (generated; imports Rock20's
.guardrails\CLAUDE-kfs.md   ──┘                                 only outside Rock20)

.guardrails\settings.json   ──> <clone>\.claude\settings.local.json   (merged, not replaced)

Rock20\.claude\skills\      ──┐
                              ├──> ~\.claude\skills\          (junctions, machine-wide)
.guardrails\skills\         ──┘
```

`CLAUDE.local.md` and `settings.local.json` are Claude Code's own per-machine files, so setup never
competes with a file Rock tracks. It merges into `settings.local.json` rather than replacing it, so
permission rules Claude Code has saved there survive a re-run. The hook runs from
`.guardrails\hooks\` in place.

Nothing is copied into a KFS repo, so pulling `Rock20` refreshes Rock's guardrails with no merge.

A clone that tracks its own `.claude\rules` but is not the guardrail source is refused, because
loading both would give Claude two versions of Rock's rules. If a later Rock version becomes the
source, point `-Rock20` at that clone.

## What's here

| | |
|---|---|
| `rules/kfs-*.md` | The curated delta. `kfs-precedence.md` is the override contract. |
| `skills/kfs-*/` | KFS variants where Rock's would misfire: `kfs-build`, `kfs-check`, `kfs-test`, `kfs-entity-model`, `kfs-plugin-migration`, `kfs-convert-block` |
| `hooks/` | `prevent-destructive.sh` |
| `settings.json` | Permissions and hooks, merged into each clone's `settings.local.json` |
| `CLAUDE-kfs.md` | Imported after Rock's `CLAUDE.md` |
| `setup.ps1` | Adds all of the above to a clone |
| `specs/`, `docs/` | Where `/spec` and `/docs` write. Not loaded by setup — content, not config. |

In the `Rock20` clone, Rock's own `/build`, `/check` and `/test` commands load beside the `kfs-*`
skills. `kfs-precedence.md` § Skill routing says which to use.

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
branch that predates them, each clone's `rules\kfs` junction goes empty. Either land `.guardrails/`
on the branches you work from, or pin setup to a worktree:

```
git worktree add C:\KFSRepo\Rock\KFSRockAssemblies-guardrails master
setup.ps1 -Target ... -Guardrails C:\KFSRepo\Rock\KFSRockAssemblies-guardrails\.guardrails
```

`setup.ps1` prints which branch the guardrails source is on, so a stale pin is visible.

## Team rules

- Don't edit `settings.json` without review — it governs permissions for everyone.
- When Claude makes a repeatable mistake, add it to the relevant rule, not to a one-off prompt.
- `RockPlugins` has its own guardrails. Don't cross-apply.
