---
title: Decisions
type: index
audience: [contributor, maintainer, agent]
area: [docs]
last-reviewed: 2026-11-11
---

# Decisions

One file per decision, from the [template](../00-meta/templates/decision.md); the rules are in the [documentation guide](../00-meta/documentation-guide.md#decisions).
Decisions 0001 to 0025 were moved from the table of the first release; their `decided` date is 2026-10-24 unless the decision names an earlier day.
Rows of that table that gave no reason, or that only restated the privacy rules of [SECURITY.md](../../SECURITY.md#keeping-personal-data-out), were left out, and so were the decisions about the setup of the GitHub repository itself (issue forms, labels), the decisions that applied only to the first release, and those that only repeated what a reference document describes.

| Number | Decision | Status | Decided |
|---|---|---|---|
| [0000](0000-documentation-system.md) | One documentation system for people and agents | accepted | 2026-11-10 |
| [0001](0001-mod-and-companion-app.md) | Mod plus separate companion app | accepted | 2026-10-24 |
| [0002](0002-mod-writes-to-game-log.md) | The mod writes to the game log | accepted | 2026-10-24 |
| [0003](0003-own-images-only.md) | Every published image is our own | accepted | 2026-10-24 |
| [0004](0004-mod-hub-cover-with-name-and-status-card.md) | The Mod Hub cover shows the mod's name and a status card | accepted | 2026-10-24 |
| [0005](0005-mod-for-pc-only.md) | The mod is for PC only | accepted | 2026-10-24 |
| [0006](0006-career-preset-timer-and-languages.md) | Design A as default, the timer, 13 languages with a language choice | accepted | 2026-10-24 |
| [0007](0007-portable-app-writing-only-to-data.md) | Portable app writing only to DATA, checked with a bounded check | accepted | 2026-10-24 |
| [0008](0008-macos-26-minimum-never-in-mac-app-store.md) | macOS 26 minimum, never in the Mac App Store | accepted | 2026-10-24 |
| [0009](0009-net-11-maui-with-native-aot.md) | .NET 11 MAUI with Native AOT on both platforms | accepted | 2026-10-24 |
| [0010](0010-own-source-generated-discord-ipc-client.md) | Our own source-generated Discord IPC client | accepted | 2026-10-24 |
| [0011](0011-tray-icon-through-winuiex.md) | Tray icon through WinUIEx TrayIcon | accepted | 2026-10-24 |
| [0012](0012-theme-setting-for-the-settings-window.md) | A theme setting for the settings window only | accepted | 2026-10-24 |
| [0013](0013-store-button-off-by-default.md) | The store button is off by default | accepted | 2026-10-24 |
| [0014](0014-paused-state.md) | Paused means speedup 0 or no status line for more than 20 s | accepted | 2026-10-24 |
| [0015](0015-quit-requested-does-not-end-the-session.md) | Quit requested does not end the session | accepted | 2026-10-17 |
| [0016](0016-macos-settings-window-on-the-current-space.md) | On macOS the settings window comes to the Space the player is on | accepted | 2026-10-24 |
| [0017](0017-no-mode-next-to-the-rank-in-tycoon.md) | No mode next to the rank in a Tycoon game | accepted | 2026-10-24 |
| [0018](0018-no-autoactivate-in-mod-json.md) | No autoActivate in mod.json | accepted | 2026-10-17 |
| [0019](0019-ignore-log-lines-from-before-the-game-started.md) | Log lines from before the game process started are ignored | accepted | 2026-10-24 |
| [0020](0020-translations-compiled-into-the-exe.md) | Translations compiled into the exe by an extra build target | accepted | 2026-10-24 |
| [0021](0021-publish-size-measures.md) | Publish size measures cut the Windows publish from 305 MB to 83 MB | accepted | 2026-10-24 |
| [0022](0022-windows-app-sdk-parts-follow-maui.md) | The excluded Windows App SDK parts stay on the versions MAUI resolves | accepted | 2026-10-24 |
| [0023](0023-ci-on-github-actions.md) | CI on GitHub Actions with an immutable release built through the gh CLI | accepted | 2026-10-24 |
| [0024](0024-first-release-unsigned.md) | The first release is unsigned on Windows and macOS | accepted | 2026-10-24 |
| [0025](0025-game-build-in-the-settings-window-only.md) | The game build shows in the settings window only, untranslated | accepted | 2026-10-24 |
