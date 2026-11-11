---
title: Known traps in test runs
type: reference
audience: [contributor, agent]
area: [tools, app, mod]
covers:
  - tools/test-run/run-game.ps1
  - tools/test-run/tf3ctl.ps1
last-reviewed: 2026-11-11
verified:
  - windows 2026-10-18
---

# Known traps in test runs

The traps of [testing by hand](manual-test.md) and of [scripted test runs](scripted-runs.md), each with what to do.

- **PowerShell variable names are case-insensitive:** `$P` and `$p` (or `$s` and `$S`) are the same variable, which breaks scripts that use both.
- **The loading screen can wait:** after a savegame or a new game has loaded, the game may show "Spiel starten" and write `Game is ready` only after the click.
  It happened with a new game and with "Fortsetzen", and "Fortsetzen" also started without it.
  A script that waits for that line should take a screenshot and click it when it is there.
- **The game keeps running in the background of a script:** at 4× speed the date, the money and even the rank change between two checks, so compare with the latest status line, not with fixed values.
- **Stutter in the first 30 s after loading** comes from the game spawning industries (`Spawned industry … Connect industry: ~415 ms` in its log), not from the mod or the app.
- **Build and publish traps** (a locked icon file, an old `.pdb` in publish) are in [build pitfalls](../20-development/build-pitfalls.md).
- **Screenshots of the game show only the start splash** while it runs in exclusive fullscreen.
  For a run driven by script, set `screenMode = "BORDERLESS"` in `<userdata>\settings.lua` while the game is closed, and set it back to `"FULLSCREEN"` afterwards; the game rewrites the file when it quits (`run-game.ps1 prepare` and `restore` do both, and refuse while the game runs).
- **Never take screenshots of the Discord window**; it shows content that has nothing to do with the test.
  Check the presence text in the app's settings window instead; it shows what the app sends.
