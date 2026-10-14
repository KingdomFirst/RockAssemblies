---
name: kfs-test
description: >-
  Report what automated verification is available for a KFS change and run it. Use when the user
  says "run tests", "test", or wants to verify changes haven't broken anything. Prefer this over the
  core `test`, which runs Rock core's `Rock.Tests` and says nothing about a plugin change.
---

**There are no unit tests in the KFS repos.** The only automated behavioral coverage is the
end-to-end suite in `KFSRockAssemblies/rocks.kfs.Tests.EndToEnd`, and it covers only the
plugins listed in its README. Do not imply more than that, and do not run Rock core's
`Rock.Tests`: it tests Rock, not us, and a pass there says nothing about a plugin change.

State that plainly, then run whatever verification actually applies to the change at hand.

Plugins under a `ZZZ_Archive` folder are retired and obsolete by KFS practice. Never test them
or propose tests for them.

## What exists

| Check | Command | Covers |
|---|---|---|
| Compile | `/kfs-build` | All 37 plugin projects |
| Compile preflight | `/kfs-compat-preflight` | Every KFS DLL project and WebForms block, compiled against a Rock clone. Catches removed or changed Rock APIs, not behavior. |
| End-to-end | `/kfs-compat-test` (needs a test site and `.env`) | Plugins listed in `rocks.kfs.Tests.EndToEnd/README.md`. Currently Person Attribute Forms Advanced only. |
| TypeScript lint | `cd KFSRockAssemblies/rocks.kfs.JavaScript.Obsidian && npm run lint` | `.ts` under `src` only — **not** `.obs` |
| Jest | `npm test` in the same project | **Zero test files.** `tests/blocks.ts` and `tests/utils.ts` are helpers for tests nobody has written. A green run means nothing. |

If you run `npm test` and it reports success, say explicitly that it matched no tests. Never
present that as evidence the change works.

## What that leaves

For anything beyond "it compiles", verification is manual. Tell the user what to exercise and
be specific — the plugin, the block, the page, the job, the migration path:

> Compiles clean. No automated coverage for this path. To verify manually:
> 1. Run the Steps to Care migration against a v17 database and confirm migration 026 applies.
> 2. Open **KFS > Steps To Care > Care Dashboard** and confirm the worker auto-assign respects
>    inactive campuses.
> 3. Re-run the job once to confirm it is idempotent.

Where a change touches a **migration**, say so loudly — migrations run once against customer
databases and are the highest-risk thing we ship.

## If asked to add tests

For a WebForms block, add it to `rocks.kfs.Tests.EndToEnd` by following "Adding tests for
another plugin" in that project's README.

Otherwise, worth doing, but it is its own piece of work: the jest harness exists and is wired up, so
Obsidian control tests are the cheapest place to start. C# test coverage would need a new test
project added to the solution. Raise it as a separate task rather than bolting it onto an
unrelated change.
