---
description: Build the KFS plugin solution and report errors. Use when the user says "build", "compile", "check build", or after making code changes that may have introduced errors.
---

Build the KFS plugin solution and report the result.

## Find the solution

KFS plugins build from a `KFSRock*.sln` at the **Rock clone root** — not `Rock.sln`, which
builds Rock core and fails anyway (`MSB4249`, the `RockWeb` website project).

```bash
ls KFSRock*.sln
```

Pick the one matching the Rock clone you are in (`KFSRock17.sln` in `Rock17`,
`KFSRock18.sln` in `Rock18`). If several match, or none do, ask — do not guess.

## Build it

All 37 KFS plugin projects are legacy-format csproj on every Rock version, so `dotnet build`
will not build them. Use MSBuild:

```bash
nuget restore KFSRock17.sln
msbuild KFSRock17.sln /p:Configuration=Debug /v:m /nologo
```

If `msbuild` is not on PATH, it ships with Visual Studio — locate it with `vswhere` rather than
falling back to `dotnet build`.

`nuget restore` is required because 7 of the plugin projects use `packages.config`, which
`dotnet restore` silently skips.

## Report

- Whether the build succeeded or failed.
- Every error with file path and line number.
- Warnings that look like real issues — ignore nullable-reference warnings unless they are in
  files changed in this session.

If the build fails, diagnose the root cause and suggest a fix. **Do not fix anything
automatically — just report.**

## Common failures

| Symptom | Cause |
|---|---|
| Types from `Rock.*` not found | Plugin csproj HintPaths point at `..\..\RockWeb\Bin\`. Either the junctions are missing (run `CreateLinks.bat`) or Rock core has not been built into `RockWeb/Bin` yet. |
| `MSB4249` website project error | You targeted `Rock.sln`. Use `KFSRock*.sln`. |
| Package not found | Run `nuget restore` first, not `dotnet restore`. |
| API missing that you expected to exist | Version mismatch — check `.claude/rules/rock-version-targets.md` and confirm the KFS branch matches the Rock clone. |
