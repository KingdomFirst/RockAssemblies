---
name: kfs-compat-test
description: >-
  Run the KFS plugin end-to-end compatibility suite against a Rock site and report which plugins
  break on it. Use when a new Rock version drops, or when the user says "compat test", "test
  plugins against Rock X", or "vet our plugins".
---

Run `KFSRockAssemblies/rocks.kfs.Tests.EndToEnd` against the configured Rock site and report
the results. Read that project's `README.md` first if you have not already. The suite is not
Rock core's `Rock.Tests`.

**Ignore `ZZZ_Archive`.** Plugins under a `ZZZ_Archive` folder (in `KFSRockAssemblies` or
`KFSRockBlocks`) are retired and obsolete by KFS practice. Never test, report or fix them, and
never add end-to-end tests for them.

## 0. Compile preflight

Run `/kfs-compat-preflight` against the Rock clone matching the site's Rock version, unless it
has already been run for that version this session. If the plugin you are about to test is on
its **broken** list, stop and report that instead: the end-to-end tests cannot pass while its
block does not compile, and the preflight already names the cause.

## 1. Setup checks

```bash
cd KFSRockAssemblies/rocks.kfs.Tests.EndToEnd
test -f .env && echo ".env present" || echo ".env MISSING"
npm install
npm run typecheck
```

- If `.env` is missing, **stop** and ask the user to create it from `.env.example`. Do not
  create it yourself, and **never read, print, or echo `.env`**: it holds a REST API key.
- Confirm with the user which site `ROCK_BASE_URL` points at before the first run in a session
  (ask them; do not read `.env`). The suite writes to that site. Refuse to run against
  anything the user describes as a customer production site.

## 2. Run

```bash
npx playwright test                         # whole suite
npx playwright test tests/Crm/PersonAttributeForms   # one plugin, if the user named one
```

The first line of output shows `Rock version under test: …`. Include it in the report.

## 3. Triage every failure

Open the failure's `error-context.md`, screenshot and trace under `test-results/artifacts/`.
Classify each failure as one of the following before reporting it:

| Class | Signs | Meaning |
|---|---|---|
| **Plugin break** | Rock error page / "Compilation Error" / block exception; a result assertion fails (value not saved, member not added, workflow not launched); a block setting key missing | Real incompatibility. This is what the suite exists to find. |
| **Test drift** | Selector not found although the block rendered fine and the feature works in the screenshot | The test needs updating for new markup. Say so; do not call it a plugin break. |
| **Environment** | 401/403 from the API, fixture setup failure, parent page or zone missing, timeouts across every test | Configuration problem. Report what to fix. No verdict on the plugin. |

For a plugin break, find the cause in the plugin source (`RockWeb/Plugins/rocks_kfs/...` or
`KFSRockAssemblies/rocks.kfs.*`) against the new Rock source. Say which Rock API or behavior
changed. **Do not fix it in this skill.** Offer `/bugfix`.

## 4. Report

- Site and Rock version tested.
- Per plugin: pass/fail counts, then each failure with its class, a one-line cause, and the
  manual-plan step it corresponds to (the README maps tests to steps).
- Anything cleanup reported it could not delete (`Cleanup of … failed`).
- What the suite does **not** cover for each plugin (listed in the README), so the user knows
  what still needs a manual pass.
