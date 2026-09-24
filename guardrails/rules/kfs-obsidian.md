---
paths:
  - "KFSRockAssemblies/rocks.kfs.JavaScript.Obsidian/**"
  - "RockWeb/Plugins/rocks_kfs/Obsidian/**"
---

# KFS Obsidian Notes

`rock/obsidian-conventions.md` applies **unchanged**. Our `.eslintrc.js` is a copy of Rock's, so
every rule in it — SFC block order, script regions, Stroustrup braces, `space-in-parens: never`,
double quotes, `I`-prefixed interfaces, the naming table — is correct here too. That is useful when
porting a core control into a plugin.

Add only these environment facts.

## Source of truth

`KFSRockAssemblies/rocks.kfs.JavaScript.Obsidian/.eslintrc.js` — alongside the two Rock paths that
file names. If ours and Rock's diverge, ours governs KFS code.

## Linting covers less than it looks

```bash
cd KFSRockAssemblies/rocks.kfs.JavaScript.Obsidian && npm run lint
```

The script is `eslint --ext .ts src` — **`.ts` only**. `.obs` files are configured in `.eslintrc.js`
(the override block covers `src/**/*.obs`) but the default script never reaches them, so an `.obs`
style violation is not caught. Rock fixed this on v18+ by linting `--ext .ts --ext .obs`; matching
that here is a one-line `package.json` change worth doing as its own commit.

`npm test` runs jest and currently matches **zero** test files. `tests/blocks.ts` and `tests/utils.ts`
are helpers for tests nobody has written. If you run it and it reports success, say explicitly that
it matched no tests — never present that as evidence anything works.

## Scope

The KFS Obsidian surface is one control (`src/Controls/cyberSourceGatewayControl.obs`) and no blocks.
Anything added should read as though it came out of Rock's framework, because that is what it will be
compared to.

The build depends on Rock's toolchain (`../../Rock.JavaScript.Obsidian/Build/rollup-mt.js`) and only
resolves inside a junctioned clone. Two known stale paths to check before trusting generated output:
`build/build-types.js` is pinned to a `Rock17` sibling clone, and `tsconfig.base.json` maps
`@Obsidian/*` one level short of the clone root.

Framework APIs differ by version — `safeParseJson`, `ContentSection`, the `styles-v2` utilities and
the icon set. See `kfs-rock-versions.md`.
