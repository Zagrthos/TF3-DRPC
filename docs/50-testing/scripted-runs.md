---
title: Scripted test runs
type: guide
audience: [contributor, agent]
area: [tools, app, mod, windows, macos]
covers:
  - tools/test-run/**
last-reviewed: 2026-11-11
verified:
  - windows 2026-10-24 build 40425
  - macos 2026-10-24 build 40425
---

# Scripted test runs

How to run the scripts in [`tools/test-run/`](../../tools/test-run/README.md) that drive Transport Fever 3 and the TF3-DRPC companion app in place of the [manual steps](manual-test.md): they start and quit the game, click and type in its window, take screenshots, and check the app's settings window against the running game.
They are tools for a developer's machine, not part of the app.

The game scripts (`run-game.ps1`, `tf3ctl.ps1`, `winshot.ps1`, `laststatus.ps1`) run on Windows and macOS, with the screen checks (`screen.ps1`, `tf3ctl.ps1 state`, `photo`, `classify` and `measure`).
**`game-tests.ps1`, `ui-tests.ps1`, `hovershot.ps1` and `apptext.ps1` are Windows only**: they drive the app's settings window through Windows UI Automation (WinApp CLI), which has no counterpart here for macOS.
The scripts work with the game and the app in any of the 13 game languages: no UI name or status pattern is written in one language, see [languages](#languages).
The click points and the boxes of the screen checks are the same in every language (the layout doesn't depend on the text), but they belong to a platform and an aspect ratio at UI scale 100%.
They are all in [`calibration.psd1`](../../tools/test-run/calibration.psd1), calibrated for Windows at 16:9 and for macOS at 1280x804 (all but the download bar); [calibrate the test-run scripts](calibration.md) says how to measure a set or a missing value, and how to check a set after a game update.

## Before you start

- Windows, or macOS for the game scripts, and PowerShell 7 (`pwsh`).
- On macOS: the Xcode command line tools (`swiftc`), and for the app that runs the scripts (Terminal, VS Code) the **Accessibility** permission for the input and **Screen Recording** for the screenshots (System Settings → Privacy & Security).
  `common.ps1` compiles `tf3mac.swift` into `artifacts/bin/tf3mac` on first use.
  The screen checks and the calibration need no further tool: the pixels of a screenshot are read by `tf3mac pixels` (ImageIO).
- WinApp CLI 0.7 on the `PATH` (`winapp ui ...`), for `apptext.ps1`, `hovershot.ps1`, `ui-tests.ps1` and `game-tests.ps1`.
- Transport Fever 3 through Steam, with the mod linked (`tools/install-mod.ps1`, a symbolic link on macOS) and activated in a savegame, as in [before a run](manual-test.md#before-a-run).
- Discord desktop, running and logged in.
- A published copy of TF3-DRPC outside the repository, started from there, with its own `DATA` folder next to the exe (`dotnet publish` in `companion/`, then copy the output).

## The scripts

Run them from `tools/test-run/`, or give the path.
`$PSScriptRoot` is used everywhere, so the working directory doesn't matter.
Relative output paths count from where you are.

| Script | What it does | Example |
|---|---|---|
| `common.ps1` | Not run by itself. Finds the game's user data folder like the app (the Steam folder from the registry, on macOS `~/Library/Application Support/Steam`, then `userdata/<id>/3493540/local` of the account whose `crash_dump/stdout.txt` was written last), reads the game log, resolves output paths, compiles the macOS helper, picks the calibration set (`Get-Calibration`, `Get-CalibrationValue`). Also the language helpers (`Set-TestLanguage`, `Get-AppText`, `Get-PresetText`, `Format-AppDate`, `Get-PresencePatterns`, see [languages](#languages)). Dot-sourced by the others. | |
| `calibration.psd1` | Not run by itself. Every click point and every box and threshold of the screen checks, one set per platform and screenshot height (`windows-720`, `macos-804`); `$null` is "not calibrated". Read by `common.ps1`. See [calibrate the test-run scripts](calibration.md). | |
| `screen.ps1` | Not run by itself. Pixel checks on a `tf3ctl.ps1` screenshot (Windows and macOS): the Mod Hub download bar, the main menu, the game menu, the Quit flyout, photo mode. They look at the layout, not at text, so they work in every language, and take their boxes from `calibration.psd1`. Dot-sourced by `run-game.ps1` and `tf3ctl.ps1`. | |
| `tf3mac.swift` | Not run by itself. The macOS helper behind `tf3ctl.ps1` and `winshot.ps1`: finds a window, brings an app to the front, sends clicks, keys and text through CoreGraphics. Text is typed with the keys of the current keyboard layout (Shift and Option included), as a person types it; characters off the layout go as Unicode. In one run a Mac Catalyst text field took real keys but not Unicode-only events; the cause wasn't found, and later runs took both. `tf3mac pixels <png> <out> [width]` writes a PNG's pixels (sRGB, B, G, R, A bytes, optionally scaled to a width) for `screen.ps1`. Formatted with `swift-format` ("Swift: Format document" in VS Code) by the rules in `.swift-format` next to it: 4 spaces, lines up to 140 characters. | |
| `run-game.ps1` | Game life cycle: `prepare [-Language <code>]`, `start`, `load [-SaveName <name>]`, `quit [-Editor]`, `restore`. Without a command it prints the usage. | `./run-game.ps1 start` |
| `tf3ctl.ps1` | Drives the game window: `shot`, `size`, `click`, `rclick`, `key`, `type`, `photo <png>`, `state`; and for a saved screenshot, without the game, `classify <png>` (add `-Verbose` for the numbers behind every check) and `measure <png> <x> <y> <w> <h>` (box statistics, for [calibrating](calibration.md)). Screenshots are 1280 px wide and click points are in that picture. Refuses input unless TF3 is in front. | `./tf3ctl.ps1 shot game.png` |
| `appctl.ps1` | The same for another process by name (700 px scale), plus `wheel` and `wheeldown`. | `./appctl.ps1 TF3-DRPC shot app.png` |
| `winshot.ps1` | Screenshot of a window without focusing it or sending input; `-Width` sets the width in pixels (the app window at its native size, for example). On macOS only for a window on the current Space, so not for the game behind another app; use `tf3ctl.ps1 shot` there. | `./winshot.ps1 TF3-DRPC app.png` |
| `shot.ps1` | Screenshot of the TF3-DRPC window by its DWM bounds (a screen copy), optionally resized first. | `./shot.ps1 small.png 400 500` |
| `resize.ps1` | Resizes a window and reports the size it got. | `./resize.ps1 TF3-DRPC 400 500` |
| `hovershot.ps1` | Windows only. Opens the preset picker, hovers an entry (a preset key such as `network`, or a literal text), captures the screen region with its tooltip. | `./hovershot.ps1 network tip.png` |
| `apptext.ps1` | Windows only. Prints the settings window's texts in tree order. | `./apptext.ps1 -Limit 20` |
| `laststatus.ps1` | Prints the last `[drpc-status]` line's values (rank, date, lines, balance, save name, ...). Reads the log only. | `./laststatus.ps1` |
| `ui-tests.ps1` | Windows only. Switches the presets through UI Automation without the game; checks the Custom section, the preview's second line, `settings.json` and screenshots. Needs an existing `DATA\settings.json` (a fresh app writes it only after a setting changes). | `./ui-tests.ps1 -AppFolder <your app folder>` |
| `game-tests.ps1` | Windows only. The in-game checks, in phases `presets`, `alternating`, `rerun` and `custom`: all presets, the custom templates and both pause paths. | `./game-tests.ps1 -Phase presets -AppFolder <your app folder>` |

Each script has a comment header with its parameters.
Screenshots and results go to `artifacts/` in the `tools/test-run/` folder (`-OutFolder` where there is one), which git ignores.

## How the scripts work

- **The game:** `run-game.ps1 prepare` switches it to `BORDERLESS` first (see [known traps](known-traps.md)), and `run-game.ps1 start` starts it with `steam://rungameid/3493540` and waits for `Main menu is ready` in the game log.
  `tf3ctl.ps1` sends clicks and keys through `SendInput` (keys as scan codes) after `SetForegroundWindow`; it takes screenshots with `PrintWindow`, and you click by the coordinates in them.
  On macOS the same commands go through a Swift helper (`tf3mac.swift`, CoreGraphics events, the Accessibility API to bring the game to the front, ImageIO to read the screenshot's pixels) and `screencapture`.
- **The foreground lock:** Windows refuses `SetForegroundWindow` to a process that didn't send the last input, for example after UI Automation calls on the app.
  A short Alt key press through `SendInput` before the call lifts it.
  Even then the game sometimes ignored the first Escape after the settings window had been in front; a click on its menu button at the bottom right (1259,686 in the screenshot) opens the menu reliably.
- **The settings window:** drive it with UI Automation through WinApp CLI 0.7 (`winapp ui`): start the app a second time to show the window, open a picker with `invoke <combo box> --action expand`, choose with `invoke <list item> --action select`, type templates with `set-value`, and read the texts with `inspect --json`.
  - The open list is a popup window of its own: `search "<entry>" -a <pid>` names its handle, and `invoke` then needs `-w <handle>`.
  - Selectors such as `cmb-7391` change when a combo box opens or closes, so look them up again each time.
  - A text selector that matches both a list item and its text fails, so search and take the `ListItem`.
  - Find a picker by its accessible name ("Design", "Sprache", "Statusanzeige"), not by its position: the theme picker comes first, so a script that takes the second combo box opens the language picker.
  - Read the status in tree order: it is the text right above the Discord line.
  - `ui-tests.ps1` (presets without the game), `game-tests.ps1` (the in-game checks) and `apptext.ps1` do this.
- **The tray:** the icon usually sits in the overflow area.
  Invoke the taskbar's "Show Hidden Icons" button (in the `Shell_TrayWnd` window; it toggles, so check first whether the overflow window `TopLevelWindowForOverflowXamlIsland` already shows the icon), find the `TF3-DRPC` button there, and right-click or double-click its position with real mouse input.
  The menu and its language submenu are `PopupHost` windows of the app, driven like the pickers.
  Typing into the settings window takes the focus and closes an open tray menu, so for the tray's Exit with a pending edit type first, then open the menu and invoke Exit at once.
- **Tooltips:** a tooltip opens in its own popup window, so capture the screen region, not the window, while `winapp ui hover` keeps the pointer on the entry (`hovershot.ps1`).
- **What Discord gets:** read it from the settings window (the game status line) and the app log, not from the Discord window (see [known traps](known-traps.md)); `laststatus.ps1` prints the last status line's values to compare with.

## Languages

The game and the app each come in 13 languages (`de`, `en`, `es`, `fr`, `it`, `ja`, `ko`, `nl`, `pl`, `pt_BR`, `ru`, `zh_CN`, `zh_TW`).
The two settings are independent, but the app follows the game by default ("Same as the game").

- **The game's language** is in `<userdata>/profile.lua` (`language = { code = "de", ... }`), not in `settings.lua`.
  In the menu a change needs a restart, and in a running game the field is locked, so the clean way is `run-game.ps1 prepare -Language <code>` with the game closed, which lets a run cover any of the 13 languages.
  It backs up `profile.lua` next to the `settings.lua` backup, and `restore` puts both back.
- **The app's language** is `language` in its `DATA\settings.json`: a code, or `game`.
- **The language under test** is what `ui-tests.ps1`, `game-tests.ps1` and `hovershot.ps1` use for the app's texts.
  `-Language <code>` sets it; without it the script reads `settings.json` (`-AppFolder`, or the folder of the running app for `hovershot.ps1`), and for `game` the code in `profile.lua`.
  It is printed at the start.
  If you passed a code that is not the language the app really shows, the checks fail.
- **The app's own texts** (the picker's name and entries, the labels, the store button, the Discord connection) are looked up by key in `companion/Tf3Drpc.Core/Resources/Texts*.resx` (`Get-AppText`), with English as the fallback like in the app.
  The scripts hold no German or English UI name.
- **The status patterns** (`Get-PresencePatterns`) are built from the structure of what `PresenceBuilder` composes.
  What they check in every language: the app's own word "In game" exactly; the number and order of the parts, joined by ` · ` (preview lines by ` | `); digits where a count or the year stands; the date in the culture's year-and-month format of the app (`September 1905`, `septiembre de 1905`, `1905年9月`, `1905년 9월`; it is the culture's pattern turned into a regex, so Japanese, Korean and Chinese without spaces work); amounts as `$` and a number with optional `K`, `M`, `B` or `T`.
  What they cannot check: the game's own words (labels such as Lines or Paused, ranks, modes, climates, month names) stand as "words without digits", because the app takes them from the game's text files.
  A wrong translation of those is not caught here.
- **Click points** don't depend on the language: the main menu "Load Game" tile, the game menu entries and the Quit flyout were the same in German and English.

## A typical run

1. `./run-game.ps1 prepare -Language de` with the game closed: backs up `settings.lua` and switches `screenMode` to `BORDERLESS` (see [known traps](known-traps.md)), and with `-Language` backs up `profile.lua` and sets the game language.
   Leave `-Language` out to keep the language.
   On macOS the game starts in `BORDERLESS` already.
2. Start the published `TF3-DRPC.exe` (from its own folder), then start it a second time to show the settings window.
3. `./run-game.ps1 start`: starts the game through Steam, waits for `Main menu is ready` written after the process start, then waits until the Mod Hub downloads are finished.
   While they run the main menu shows a progress bar under the account box, and loading a game then must be avoided.
   The game log has no line for the end of the downloads, so the script looks at the bar on screenshots (every 10 s, up to `-DownloadTimeoutSeconds`, default 900) and prints the progress.
   On macOS the download bar isn't calibrated, so it prints a warning and doesn't wait (wait by hand).
4. `./run-game.ps1 load -SaveName <your save>`: clicks the "Load Game" tile, types the name into the filter box, clicks the first card's Load Game button, waits for `Loading game from file`, and from 15 s on clicks "Start Game" every 20 s until `Game is ready` arrives; the game writes that line only after the click.
   Without `-SaveName` it loads the first card, the save written last.
   It prints the log line with the file that loads.
   It checks that the main menu is showing, and waits for downloads again (both skipped with a warning where the screen checks are not calibrated).
   `continue` is the old name and runs `load`: the scripts don't use the "Continue" tile, which build 40423 dropped and which build 40425 shows again above "Load Game" on macOS; on Windows build 40425 shows the tutorial tile there instead, and "Load Game" stays where it was.
5. `./game-tests.ps1 -Phase presets -AppFolder <your app folder>`, then `alternating`; then `custom -SaveName <your save>` (the save name is read from the last status line if left out).
   `./ui-tests.ps1` needs no game.
   Add `-Language <code>` if the app's setting is not the language you test.
6. `./tf3ctl.ps1 photo game.png` (optional): photo mode for a picture of the game without HUD, see below.
7. `./run-game.ps1 quit`: looks at the screen, opens the game menu if it is closed, clicks Quit and Return to Desktop (without saving); waits for the process to end and reports the `Normal shutdown` or `Ungraceful exit` line.
   Where the screen checks are not calibrated it warns and goes blind: Escape, then the Quit and Return to Desktop points.
8. `./run-game.ps1 restore` with the game closed: puts the original `settings.lua` and `profile.lua` back.
   The game rewrites both when it quits, so restore only after the game has ended.

### The screen checks

`quit` doesn't press Escape blindly: the game menu has state (Escape opens and closes it, and also closes open windows first).
`screen.ps1` tells the screens apart by their layout in a screenshot (the numbers here are the Windows 16:9 ones of `calibration.psd1`):

| State | How it is recognised |
|---|---|
| `game-menu` | The first four of the five dark grey button panels at the left are flat dark grey (x 75..410, centres at y 235, 297, 360, 422) and carry a white icon. |
| `quit-flyout` | The game menu, and two dark buttons beside Quit (Return to Main Menu, Return to Desktop). |
| `main-menu` | The settings, achievements, news and exit icons at the top right are white on the dark background. |
| `other` | Anything else: the game with the menu closed, a window, a loading screen, the settings screen. |

`quit` repeats up to eight times: in `other` it presses Escape, then (alternating) clicks the game menu button, because the game sometimes ignored Escape after another window was in front; in `game-menu` it clicks Quit; in `quit-flyout` it clicks Return to Desktop; in `main-menu` it clicks the exit icon.
If the menu doesn't open it fails with the last state instead of clicking blind.
`./tf3ctl.ps1 state` prints the state of the game now, `./tf3ctl.ps1 classify <png>` that of a saved screenshot (no game, no input; `-Verbose` shows the mean colour, spread and bright pixels of every box), which is how to check the classifiers after a game update.

`tf3ctl.ps1 photo <png>` needs the running game with the menu closed, the eye icon's point and the photo mode check of the set.
It clicks the eye icon (Points.Eye, 1006,710 on Windows), which hides the HUD, checks that the hint box (Esc Back, F9 Screenshot) is there, presses F9, waits for the new file in `<userdata>/screenshots`, copies it to `<png>` (full resolution, no HUD, none of the external overlays that a window screenshot shows) and presses Escape to leave photo mode.
F9 outside photo mode does nothing, which is why the hint box is checked first.

## Limits

- **Coordinates:** The click points and the screen check boxes use the 1280 px wide screenshot as the coordinate system and are all in `calibration.psd1`, one set per platform and screenshot height (`tf3ctl.ps1 size`).
  - The **Windows set (16:9, UI scale 100%, measured on build 40423, checked on build 40425)**: main menu "Load Game" tile 370,627; filter box of the Load Game screen 180,112; the first card's Load Game button 176,322 (its gear is at 278,322); "Start Game" 1192,653; game menu button at the bottom right 1259,686; game menu entries at x=240: Resume y=235, Save 297, Load 360, Settings 422, Quit 485; Quit flyout "Return to Main Menu" 540,485 and "Return to Desktop" 788,485; main menu settings gear 1079,50 and exit icon 1188,50; photo mode eye icon 1006,710.
  - On Windows, build 40425 left the "Load Game" tile where it was: `start`, `load`, `classify` of the game menu and the Quit flyout, `photo` and `quit` worked with the set unchanged (the map editor's points and the download bar weren't exercised).
  - The **macOS set (1280x804, measured on build 40425)**: "Load Game" tile 372,670 (build 40425 has a "Continue" tile again, above it); filter box 180,153; the first card's Load Game button 177,366; "Start Game" 1192,736; game menu button 1259,769; game menu entries at x=240: Resume y=277, Save 339, Load 402, Settings 464, Quit 527; Return to Desktop 788,527; exit icon 1188,50; eye icon 1007,794.
  - The map editor's points (240,558 and 788,558) are from older runs and weren't measured again, and the download bar isn't calibrated (no Mod Hub download ran), so `start` doesn't wait for downloads there and `tf3ctl.ps1 state` and `classify` print `downloads=not-calibrated`.
  - A script that needs a value that is `$null` stops before the first click with a message naming it.
  - `-QuitPoint` and the other parameters of `run-game.ps1` (`-LoadGameTilePoint`, `-FilterPoint`, `-SaveCardPoint`, `-StartGamePoint`, `-MenuButtonPoint`, `-QuitPoint`, `-ToDesktopPoint`, `-ExitPoint`) and `tf3ctl.ps1 photo`'s `-EyePoint` override the table, which is also how to try a point before it goes into the file.
  - While the screen checks of a set are `$null`, `start` doesn't wait for the downloads, `load` doesn't check the main menu and `quit` goes blind, each with a warning.
  - The map editor's menu has one more entry ("Save and Play"), so its Quit and Return to Desktop are lower (EditorQuit and EditorToDesktop in the table: 240,517 and 788,517 on Windows, 240,558 and 788,558 on macOS; "Return to Main Menu" 540,517 on Windows): `quit -Editor` uses them without looking at the screen, because the menu check is calibrated for the normal game menu.
  - Another aspect ratio or UI scale needs a set of its own; without one the scripts stop with "No calibration for ...".
- **The in-game menu has state:** Escape opens and closes it, and also closes only the Quit flyout when that is open, and an open window first.
  `quit` handles that by looking (see [the screen checks](#the-screen-checks)).
  In a campaign mission Escape first skips the mission's dialogue and closes its tutorial pop-ups, and during the opening dialogue the menu button doesn't open the menu either; closing the window (`CloseMainWindow`) does.
  In those cases use `-NoMenuCheck` or open the menu by hand.
- **Without `-SaveName`, `load` loads the save written last.**
  After a test save is deleted it can load another one (a tutorial such as `tutorial_temperate_1`, where the mod doesn't run); `load` prints the `Loading game from file` line, so check it.
  With a name, the first card of the filtered list loads; a filter that matches several saves gives the newest of them.
- **The download bar** sits under the account box at about x 80..310, y 105..120 (blue fill, grey track; Windows).
  The check scans y 100..130, because a longer text can wrap the account box.
- **The language of the game can't change in a running game**; set it with `prepare -Language` while the game is closed.
  The app follows the game language only when its setting is "Same as the game", and then also changes.
- **One display:** Clicks go to screen coordinates and screenshots to the window's own picture; other layouts weren't tried.
- **The game keeps running** between two checks, so the date, the money and even the rank change; the checks use patterns and the latest status line, not fixed values ([known traps](known-traps.md)).
- **Input needs the foreground:** An input script refuses to act when another window is in front.
  Do not touch the keyboard or the mouse during a run.
  On macOS the TF3-DRPC settings window opens over the full-screen game and stays in front until it is closed, so close it before sending input to the game.
- **Overlays:** An external performance overlay (FPS, CPU at the top centre, for example) can appear in the `tf3ctl.ps1 shot` pictures; the pixel checks look elsewhere.
  Photo mode pictures are free of it.
- **Keep Discord out of the pictures** ([known traps](known-traps.md)): `shot.ps1` and `hovershot.ps1` copy a screen region, so keep Discord out of that region, and read the presence text in the app's settings window (`apptext.ps1`) instead.

## Placeholders to replace

No script holds a value of a particular machine.
Where the scripts need one, it is a parameter or a marker in angle brackets in the comments.
Replace these with your own values:

| Placeholder | Meaning |
|---|---|
| `-AppFolder <your app folder>` | The folder of your published TF3-DRPC copy; its `DATA\settings.json` is checked. |
| `-SaveName <your save>` | The name of the savegame that is loaded, for `load` and the `custom` phase. |
| `-Language <code>` | A game language code (`de`, `en`, `es`, `fr`, `it`, `ja`, `ko`, `nl`, `pl`, `pt_BR`, `ru`, `zh_CN`, `zh_TW`): the game's language for `prepare`, the app's for the test scripts. |
| `-UserDataFolder <Steam>\userdata\<your Steam id>\3493540\local` | Only when the automatic lookup finds the wrong account or Steam isn't registered (the lookup falls back to the default Steam folder if the registry has no entry). |
| `-OutFolder <folder>` | Where screenshots and the `settings.lua` and `profile.lua` backups go; the default is the ignored `artifacts/` folder there. |
| `-StoreButtonText <text>` | Only to override the store button's label; by default it is the app's text in the language under test. |
