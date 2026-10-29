# KFS Docs

KFS-authored documentation lives here. The `docs` skill writes to this folder, not to `docs/` at the
project root — the project root is a Rock clone we do not own, and anything written there is
untracked and lost on a clean.

From the project root (a Rock clone), that path is:

```
KFSRockAssemblies/.guardrails/docs/
```

Organise by **plugin**, not by Rock domain — that is the unit we work in:

```
docs/
  steps-to-care/
  intacct/
  shelby-financials/
```

Everything else in Rock's `docs` skill applies: the frontmatter block, the `related_files:` list
(pointing at `KFSRockAssemblies/rocks.kfs.[Plugin]/…`), the code-anchored voice, and the
specs-vs-docs distinction.

## Rock's own docs are not here

Spark's 119 "as built" documents covering Rock core live in the `Rock20` clone at `Rock20/docs/`.
They were previously duplicated into this repo and have been removed — read-only reference, already
present wherever `setup.ps1` can run, and not ours to carry.

Two cautions when citing them:

- They are written against **v20**. Check `kfs-rock-versions.md` before trusting anything on v17 or
  v18. Measured against v17, 60 of the 119 were fully valid, 40 partially stale, and 7 described
  subsystems v17 does not have (`ai/agent-skills-authoring`, `ai/mcp-integration`,
  `communication/communication-flows`, `cms/lava-applications`, `engagement/outreach-toolbox`,
  `core/composite-field-type-pattern`, `core/cascade-picker-pattern`).
- Do **not** run the skill's Audit or Update-From-Spec mode against them. Auditing Spark's v20 docs
  against our clone reports real drift that is not ours to fix, and "correcting" them would fork
  their content into our repo under our name.
