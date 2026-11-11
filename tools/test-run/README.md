# Scripts for test runs

The scripts that drive Transport Fever 3 and the TF3-DRPC companion app in scripted test runs.
They are tools for a developer's machine, not part of the app.
How to run them, the order of a run, their limits and the values to replace are in [scripted test runs](../../docs/50-testing/scripted-runs.md); how to measure the click points and screen check boxes is in [calibrate the test-run scripts](../../docs/50-testing/calibration.md).

## Scripts

- `common.ps1`: shared helpers (user data lookup, game log, calibration set, language helpers); dot-sourced by the others.
- `calibration.psd1`: every click point and screen check box, one set per platform and screenshot height.
- `screen.ps1`: pixel checks on a screenshot (download bar, main menu, game menu, Quit flyout, photo mode).
- `tf3mac.swift`: the macOS helper for clicks, keys, text and screenshot pixels.
- `run-game.ps1`: the game's life cycle (`prepare`, `start`, `load`, `quit`, `restore`).
- `tf3ctl.ps1`: drives the game window and classifies or measures saved screenshots.
- `appctl.ps1`: the same for another process by name.
- `winshot.ps1`: screenshot of a window without focusing it.
- `shot.ps1`: screenshot of the TF3-DRPC window by its DWM bounds.
- `resize.ps1`: resizes a window and reports its size.
- `hovershot.ps1`: captures a preset's tooltip (Windows only).
- `apptext.ps1`: prints the settings window's texts in tree order (Windows only).
- `laststatus.ps1`: prints the values of the last `[drpc-status]` line.
- `ui-tests.ps1`: switches the presets through UI Automation without the game (Windows only).
- `game-tests.ps1`: the in-game checks of presets, custom templates and pause (Windows only).

Each script has a comment header with its parameters.
