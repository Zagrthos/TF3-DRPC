---
title: Project status
type: explanation
audience: [player, contributor, maintainer, agent]
area: [mod, app, docs]
last-reviewed: 2026-11-11
---

# Project status

The single place for where the project stands, its known limits and the work ahead.
Reference documents describe the current behaviour only; plans and open work live here.

## Current state

- The mod and the companion app are done on Windows and macOS and tested end to end with the game and Discord.
- The first release, app `0.1.0` and mod revision 1, is public: the app on GitHub, unsigned, and the mod on mod.io.
- The app is built on .NET 11 GA; the next release brings it to players ([CHANGELOG.md](../../CHANGELOG.md)).
- The macOS app is experimental and runs on Apple silicon with macOS 26 or later.
- The app's own texts in the 12 languages other than English are machine-translated and wait for a check by native speakers.

## Known limits

These are accepted as they are.

- The edge cases of the mod's hardening (NaN or infinity, a clock set back, a balance above 2^31, a failing date field, values of an unexpected type, a malformed configuration pair) are covered only by the reviews' Lua harnesses in [reviews/mod/](../../reviews/mod/); none occurred in the game runs.
- A real second Steam account with a log dated in the future is covered by unit tests only.
- While a new game's map is generated (up to a minute) the app shows the main menu: the log has `Seed text:` there, but no line the app uses for Loading ([the game log](../40-reference/game-log.md#lines-that-matter)).
- The game folder is looked for in the default Steam library only (no `libraryfolders.vdf`); a game in another library is found through its running process.

## To do

Each item becomes a GitHub issue with the `enhancement` label and its `area-` labels; until its issue exists, this list is where it is tracked.
When an issue is opened, its item here is replaced by a link to it.

- **macOS universal build** (`area-macos`): `maccatalyst-arm64;maccatalyst-x64`, because macOS 26 still runs on some Intel Macs; only `maccatalyst-arm64` is built today.
- **Windows code signing** (`area-windows`): a code-signing certificate, so SmartScreen stops warning; then the README's SmartScreen section goes.
- **macOS signing and notarisation** (`area-macos`): an Apple Developer account (99 USD per year); then the README's "Open Anyway" steps go.
- **Translation checks** (`area-translations`): the app's own texts in 12 languages, the translocation text included (DeepL, 2026-10-20), checked by native speakers; each language ships once its check is done, independent of a release.
- **CI image** (`area-tools`): move macOS from the `xcode-27` preview image to a regular image once one has Xcode 27.2.
- **Pinned actions** (`area-tools`): raise the commit-pinned actions from time to time.
- **Documentation restructure** (`area-docs`): move every document into the layout of the [documentation guide](../00-meta/documentation-guide.md); the files not yet moved are the `Legacy` list in `tools/docs/docs-config.psd1`.
