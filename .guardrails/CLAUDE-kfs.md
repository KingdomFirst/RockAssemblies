# KFS Rock Plugin Development

Loaded after Rock v20's `CLAUDE.md`, which is never modified. A generated `CLAUDE.local.md`
imports this file, and outside the `Rock20` clone it imports Rock20's `CLAUDE.md` first.

**Rock's conventions are the standard.** Where this project departs from them, the departure is
recorded in `.claude/rules/kfs/` and nowhere else — `kfs-precedence.md` lists every conflict
explicitly. If something is not in that list, follow Rock, **including when existing KFS code does
otherwise**. Our plugins predate these guardrails and a conformance project will assess them
separately.

## What you are working on

KFS builds **plugins** for Rock. Rock core is read-only reference here; we do not fork it.

Rock's `CLAUDE.md`, loaded above, describes Rock core development — its Project Architecture
table, commit format and copyright header are Spark's and are **overridden** for our work. See
`kfs-repos.md` for our layout, branches and commit convention, and `kfs-licensing.md` for the
header.

## Before writing code

1. **Establish the Rock version** from the clone, not the branch name — `kfs-rock-versions.md`.
   Icons, styling, several framework APIs and the build path all differ between v17 and v18+.
2. **Establish which repository** you are in. Four are junctioned into this working tree and a bare
   `git` command hits Rock's, not ours — `kfs-repos.md` § Git.
3. **Use the `kfs-*` skill where one exists**: `/kfs-plugin-migration` for anything touching the
   database, and `/kfs-build`, `/kfs-check` and `/kfs-test` to verify. `kfs-precedence.md`
   § Skill routing lists them.

## The rules

| File | Loads |
|---|---|
| `kfs-precedence.md` | always — the override contract |
| `kfs-licensing.md` | always — copyright and licence |
| `kfs-repos.md` | always — repos, git, paths, build, branches, commits |
| `kfs-rock-versions.md` | always — the v17 / v18+ gate |
| `kfs-domains.md` | always — the three things called "domain" |
| `kfs-plugin-model.md` | in the KFS plugin and block trees |
| `kfs-migrations.md` | in plugin `Migrations/` and `sql/` |
| `kfs-obsidian.md` | in the Obsidian project |

Rock's own rules load beside these, unmodified: under `.claude/rules/rock/`, or directly under
`.claude/rules/` in the `Rock20` clone. These rules call them `rock/…` either way.
