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

The trailing `//` is intentional. The year is the year the file is **created** — it should be bumped on
later edits.

`.obs` / `.partial.obs` — one line, before `<template>`:

```html
<!-- Copyright by Kingdom First Solutions; Licensed under the Apache License -->
```

## Case 2 — File copied or derived from Rock core

Keep Spark's header and the Rock Community License **exactly as they are**. Do not relicense, do
not add a KFS line alongside. Use the verbatim block in `rock/code-conventions.md`.

This is common: 35 of the 76 blocks in `RockBlocks` began as core blocks and correctly retain
Spark's header. When you fork a core block into `RockWeb/Plugins/rocks_kfs/`, the header travels
with it.

## Deciding

Did this file's code originate in Rock core? If yes — even partially, even heavily modified —
Case 2. If it is original KFS work, Case 1. When a KFS file grows to contain a substantial verbatim
block from core, note the provenance in a comment at that block rather than changing the file
header.

A handful of files carry `Copyright by the Central Christian Church`. Same reasoning; leave them.

## Not ours

`rock-attended-checkin` is a NewSpring fork whose `.cs` files carry **no** copyright header at all.
Do not add one. See `kfs-repos.md`.
