# KFS Specs

KFS-authored specs live here. The `spec` skill writes to this folder, not to `specs/` at the project
root — the project root is a Rock clone we do not own, and anything written there is untracked and
lost on a clean.

From the project root (a Rock clone), that path is:

```
KFSRockAssemblies/guardrails/specs/
```

Layout follows Rock's `spec` skill: `YYMMDD-topic-kebab.md` here while active, moving to
`completed/{domain}/` or `rejected/{domain}/` with an `INDEX.md` per tree. Create those on first use.

## Rock's own specs are not here

Spark's specs — 65 documents covering Rock core's v19/v20 development, authored by Rock's engineers
with commit hashes into `SparkDevNetwork/Rock` — live in the `Rock20` clone at `Rock20/specs/`.
They were previously duplicated into this repo and have been removed: they are read-only reference,
already present on every machine that can run `setup.ps1`, and they are not ours to carry in a
public KFS repo under KFS commit history.

Read them there when useful. Never run the `spec` skill's completion or rejection mode against
them — both move files and rewrite an `INDEX.md` that belongs to another organisation.
