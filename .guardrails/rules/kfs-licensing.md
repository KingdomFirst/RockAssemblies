# KFS Licensing

Overrides `rock/code-conventions.md` § Copyright Headers. **This is a licensing decision, not a
formatting one** — getting it wrong puts the wrong copyright and licence on shipped code.

---

## Case 1 — New KFS-authored file (the default)

`.cs` and `.ts`:

```
// <copyright>
// Copyright <CURRENT YEAR> by Kingdom First Solutions
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
```

The trailing `//` is intentional.

**The year is the year of the file's most recent change.**

- **New file:** use the current year.
- **Editing an existing KFS file:** if the header year is earlier than the current year, replace it
  with the current year in the same change. Replace it with a single year, not a range
  (`2021-2026`). If the header already shows the current year, leave it.

This applies only to the KFS header in Case 1. Spark's header in Case 2 has no year, so there is
nothing to update.

`.obs` / `.partial.obs` — one line, before `<template>`:

```html
<!-- Copyright by Kingdom First Solutions; Licensed under the Apache License -->
```

## Case 2 — File copied or derived from Rock core

Keep Spark's header and the Rock Community License **exactly as they are**. Do not relicense, and
do not add a KFS copyright line. Use the verbatim block in `rock/code-conventions.md`.

Directly after the closing `// </copyright>` and its trailing `//`, add a `<notice>` block that
marks the file as a derivative work and lists what KFS changed:

```
// </copyright>
//
// <notice>
// This file contains modifications by Kingdom First Solutions
// and is a derivative work.
//
// Modification (including but not limited to):
// * Added filters to Grid
// * Added sorting to Grid
// * Removed reordering due to sorting
// </notice>
//
```

- The first three lines and the `Modification (including but not limited to):` line are fixed
  wording. Copy them exactly.
- Write one `* ` bullet per behavioural change, describing what changed rather than how. See
  `RockWeb/Plugins/rocks_kfs/Core/DefinedValueList.ascx.cs` and
  `rocks.kfs.Workflow.Action.CheckIn/LoadBalanceLocations.cs`.
- **When you edit a file that already has a notice, add a bullet** for any new behavioural change.
  Don't add bullets for refactors or fixes that don't change behaviour.
- The notice goes in the code-behind (`.cs`), not in the `.ascx` markup.

This is common: 35 of the 76 blocks in `RockBlocks` began as core blocks and correctly retain
Spark's header. When you fork a core block into `RockWeb/Plugins/rocks_kfs/`, the header travels
with it, and you add the notice in the same change.

## Deciding

Did this file's code originate in Rock core? If yes — even partially, even heavily modified —
Case 2. If it is original KFS work, Case 1. When a KFS file grows to contain a substantial verbatim
block from core, note the provenance in a comment at that block rather than changing the file
header.

A handful of files carry `Copyright by the Central Christian Church`. Same reasoning; leave them.

## Not ours

`rock-attended-checkin` is a NewSpring fork whose `.cs` files carry **no** copyright header at all.
Do not add one. See `kfs-repos.md`.
