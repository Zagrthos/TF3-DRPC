---
title: Test by hand on macOS
type: guide
audience: [contributor, agent]
area: [app, mod, discord, macos]
covers:
  - tools/install-mod.ps1
  - tools/test-run/tf3mac.swift
  - companion/Tf3Drpc.App/Tf3Drpc.App.csproj
last-reviewed: 2026-11-13
verified:
  - macos 2026-10-24 build 40425
---

# Test by hand on macOS

What differs from [test by hand on Windows](manual-test.md) when you test on a Mac.
Every step of [the table of steps](manual-test.md#steps-and-what-discord-shows) applies on macOS, run on macOS 27 with the Native AOT publish, except the double-click on the tray icon (see the menu bar below).

## What differs

- `./tools/install-mod.ps1` runs under `pwsh` and makes a symbolic link.
  The game's user data and log are at the same place under `~/Library/Application Support/Steam` ([the game log](../40-reference/game-log.md#finding-the-game-and-its-log)).
- Publish with `-r maccatalyst-arm64`; the bundle `TF3-DRPC.app` is in `artifacts/bin/Tf3Drpc.App/release_maccatalyst-arm64/`.
  Copy it outside the repository and start it there; `DATA` is created next to the `.app`.
- A copy that carries the quarantine flag (a download) is started from a hidden read-only copy (App Translocation): the settings window opens at start and asks to move the app once with Finder.
  An unsigned or ad-hoc-signed download is first stopped by Gatekeeper ("Open Anyway" in Privacy & Security).
- The tray is the menu bar icon, with the same menu.
  A click or a double-click on it opens the menu, as for any menu bar icon; the settings window opens with "Settings…" there.
  A second start hands over to the running app, which shows its settings window.
  Cmd+Q in the settings window quits like Exit; Cmd+W only hides it, like the close button.
- The preset tooltips appear when the pointer rests on an entry of the open "Status appearance" menu, beside the menu.
  The menu is a window of its own, so capture the screen region; find the entry's frame through the Accessibility API (its `AXMenuItem`).
- The theme follows the macOS appearance (System Settings → Appearance) with "Same as the system"; with "Light mode" or "Dark mode" the native title bar has to match the page.
- The game runs in a full-screen Space of its own.
  The settings window opens over it and stays in front until it is closed, so close it before you send input to the game by script (`tf3ctl.ps1` refuses input while another window is in front).
- "Close the game window" is Cmd+Q in the game: it logs `Quit requested` and the game stays in its menu, as on Windows.
- The scripted steps of [scripted test runs](scripted-runs.md) cover only the game on macOS (`ui-tests.ps1`, `game-tests.ps1`, `hovershot.ps1` and `apptext.ps1` are Windows only).
  On macOS the window can be driven through the Accessibility API instead: System Events for the menu bar menu (`click menu bar item 1 of menu bar 2` of process "TF3-DRPC"), and the window's controls by their positions, which change when the preview's height changes, so look them up again after each change.
  `tf3mac` clicks and types there; the status line is the window's first static text.

## Check

The game scripts run on a Mac (macOS 27, build 40425) with the measured set `macos-804`: `run-game.ps1 start`, `load -SaveName`, `quit` (from the game and from the Quit flyout), `tf3ctl.ps1 state` on every screen and `tf3ctl.ps1 photo`.
The screen checks find each screen.
The download bar isn't measured, because no Mod Hub download ran.
