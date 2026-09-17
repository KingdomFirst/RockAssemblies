# Claude Code for KFS Rock RMS Plugins

A developer guide for using [Claude Code](https://code.claude.com/docs/en/overview) across the
KFS plugin repositories.

> **Provenance.** This configuration is adapted from the guardrails Spark Development Network
> ships in Rock RMS v20 (`SparkDevNetwork/Rock`, `.claude/`). This is a copy that has been
> been rewritten and adapted for plugin development. When you pull improvements from
> upstream, diff against Rock's version and re-apply our adaptations — do not overwrite.

---

## 1. Setup

### Install

```bash
npm install -g @anthropic-ai/claude-code
```

### Wire up the working tree

Claude Code must run from a **Rock clone root** (`RockV17\`, `RockV18\`), not from inside
`KFSRockAssemblies\`. Every path in these rules and skills is relative to that root, and the
plugin projects themselves only build there (`..\..\RockWeb\Bin\*.dll`).

From the Rock clone root, once per clone:

```
CreateLinks.bat
```

That junctions the KFS repos in and links `.claude` + `CLAUDE.md` from `RockAssemblies` into
the Rock root, so the config is version-controlled in our repo but loads with the whole tree
visible. It also adds both to `.git/info/exclude` so they do not show as untracked in the Rock
clone.

Then:

```bash
cd C:\KFSRepo\Rock\RockV17
claude
```

### LSP (optional but worth it)

```
/plugin install csharp-lsp@claude-plugins-official
/plugin install typescript-lsp@claude-plugins-official
```

- **TypeScript LSP** -- Type-aware navigation across the Obsidian frontend (Vue 3 + TypeScript)
- **C# LSP** -- Cross-project resolution across Rock.Blocks, Rock.ViewModels, Rock.Model, and the rest of the solution

> **Why it matters:** With LSP, Claude can resolve types, follow inheritance chains, and find all usages -- the same way your IDE does. Without it, it relies on grep and file reads alone.
>
> **Troubleshooting:** If LSP isn't working, run `/plugin` and check the **Errors** tab. The most common issue is the language server binary not being found in your PATH.

### Prompting tips

- **Plan before you build.** For non-trivial tasks, press `Shift+Tab` to toggle Plan Mode -- Claude explores the code without making changes. Once you're aligned on the approach, switch back and let it execute.
- **Be specific.** "Fix the login bug in `AuthenticationService.cs` -- session tokens aren't refreshing after expiry" beats "fix the login bug."
- **Include test cases.** "Implement `ValidateEmail`. Test: `user@example.com` returns true, `invalid` returns false. Run tests after."
- **Scope your requests.** One task per prompt. If you need three things, do them sequentially or break them into separate prompts.
- **Provide context.** Paste error messages, stack traces, or screenshots directly into the prompt. The more Claude can verify its own work, the better the output.

---

## 2. Know Your Rock Version First

KFS ships against several Rock major versions at once. **Before any code task**, establish which one:

```bash
grep AssemblyInformationalVersion Rock.Version/AssemblySharedInfo.cs
```

| KFS branch | Rock clone |
|---|---|
| `hotfix-17` | `RockV17` |
| `hotfix-18` | `RockV18` |
| `hotfix-19` | — |
| `master` | latest |

Icons, styling, several framework APIs and the build path all differ between v17 and v18+.
`.claude/rules/rock-version-targets.md` is the single source of truth. A `hotfix-17` branch
checked out inside `RockV18` builds against the wrong assemblies — Claude is instructed to stop
and flag that combination.

---

## 3. Project Configuration

The `.claude/` directory is checked into the repo. Everything here is shared -- treat it like production code.

```
.claude/
  README.md              -- This guide
  settings.json          -- Shared permissions and hooks (team-wide)
  settings.local.json    -- Personal overrides (gitignored)
  commands/              -- Slash commands (/build, /test, /check)
  hooks/                 -- Safety hooks (block destructive git operations)
  rules/                 -- Contextual rules (auto-loaded based on file paths)
    plugin-deviations.md      always -- KFS: the closed list of departures from Rock
    rock-version-targets.md   always -- KFS: v17 vs v18+ differences
    code-conventions.md       always -- Rock's, verbatim except Copyright Headers
    data-model.md             always -- Rock's, verbatim
    rock-domains.md           always -- Rock's, verbatim
    block-architecture.md     Rock's, verbatim; paths extended to the KFS block trees
    obsidian-conventions.md   Rock's, verbatim; KFS eslintrc added as source-of-truth
  skills/                -- multi-step workflows
```

| Component | When it loads |
|---|---|
| `CLAUDE.md` (repo root, hard-linked into the Rock clone) | Always |
| `rules/*.md` without `paths:` frontmatter | Always |
| `rules/*.md` with `paths:` frontmatter | Only when working in those directories |
| `skills/` | On demand — `/name` or keyword trigger |
| `commands/` | On demand — `/name` |
| `settings.json` | Always |

---

## 4. Rock Is the Standard

The rules in `.claude/rules/` are Spark's Rock v20 guardrails, kept verbatim so they can be
diffed against upstream. **Follow Rock's patterns.** Departures live in one closed list —
`.claude/rules/plugin-deviations.md` — and if something is not in it, follow Rock.

Our plugins predate these guardrails and some do not conform. **Existing KFS code is not
evidence of a convention.** A conformance project will evaluate them separately; until then,
do not infer a pattern from surrounding code. Report, but do not "fix" unrelated non-conforming code
while doing other work.

Two deviations are worth knowing before you read the list:

**Copyright and licensing.** New KFS files get `Copyright <year> by Kingdom First Solutions`
under **Apache 2.0**. Files derived from Rock core keep Spark's header and the **Rock Community
License** — 35 of the 76 blocks in `RockBlocks` are in that second category and are correct as
they stand. Getting this wrong is a licensing defect, not a style nit.

**Plugin entities need three additions.** `IRockEntity` (Rock's own documented plugin hook),
`[DataContract]` (required by `RockContext`'s registration filter), and `HasEntitySetName()`
(core gets this from its `DbSet` properties; plugins are registered dynamically). Table name,
file location and namespace change. Everything else in `data-model.md` — including
`[RockDomain]` and `[Rock.SystemGuid.EntityTypeGuid]`, which do work on plugin entities —
applies unchanged.

---

## 5. Commands

| Command | Purpose |
|---|---|
| `/build` | `nuget restore` + `msbuild` on the version-matched `KFSRock*.sln` |
| `/test` | Reports what verification actually exists — **there is no test suite** |
| `/check` | Pre-commit: build, branch sanity, diff review (licensing, data model, migrations, version-sensitive APIs) |

`dotnet build` does not work here. All 37 plugin projects are legacy-format csproj on every
Rock version, and `Rock.sln` fails with `MSB4249` regardless.

---

## 6. Permissions & Safety

`settings.json` pre-approves file operations, read-only git, `msbuild`/`nuget restore`, and the
npm lint/test scripts. Everything else prompts.

Two permissions from Rock's original config were **deliberately removed**:

- `Bash(git push origin*)` — our version branches take changes through PRs, and pushing is
  outward-facing. Confirm each push.
- `Bash(git remote*)` — our remote URLs currently embed a PAT in cleartext, so this would print
  a credential on every call.

A `PreToolUse` hook (`hooks/prevent-destructive.sh`) blocks force push, hard reset, rebase onto
main/develop, `--amend`, `--no-verify`, force-delete branch, stash drop/clear, and blanket
discard. If you need one of those, do it manually outside Claude Code.

### Team guidelines

- **Do not edit `settings.json` without team review** — it governs permissions for everyone.
- **Do not modify a skill you did not author** without coordinating.
- When Claude makes a repeatable mistake, add it to the relevant `references/common-pitfalls.md`:

```markdown
### Pitfall: [Short name]
**Symptom:** What Claude does wrong
**Cause:** Why it happens
**Fix:** What to do instead
**Added:** [date] by [your initials]
```

---

## 7. Context Management

- **`/clear` between unrelated tasks** — leftover context reduces accuracy.
- **`/compact` with a focus** — `/compact keep the migration changes`.
- **`@filename`** to reference a file instead of pasting it.
- **`/context`** to see what is consuming space.

Note that this tree is large: the Rock clone plus both KFS repos. Scope requests to a plugin
("in `rocks.kfs.StepsToCare`, ...") rather than letting Claude search the whole tree.

---

## 8. Upstream

Rock's guardrails: `SparkDevNetwork/Rock` → `.claude/`. Worth watching for new skills and
pitfalls. Two standing caveats when pulling from it:

1. Its content targets Rock core on the **current** version. Check anything you copy against
   `.claude/rules/rock-version-targets.md` before trusting it on v17.
2. Its conventions are Spark's — copyright, entity attributes, migration layout, commit format
   and branch naming all differ from ours.
