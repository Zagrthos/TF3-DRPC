# Scripts for test runs

The scripts that drove Transport Fever 3 and the TF3-DRPC companion app in the scripted test
runs of [docs/testing.md](../../docs/testing.md#runs-driven-by-script): they start and quit the
game, click and type in its window, take screenshots, and check the app's settings window
against the running game. They are tools for a developer's machine, not part of the app.

The game scripts (`run-game.ps1`, `tf3ctl.ps1`, `winshot.ps1`, `laststatus.ps1`) run on Windows and
macOS, with the screen checks (`screen.ps1`, `tf3ctl.ps1 state`, `photo`, `classify` and `measure`).
**`game-tests.ps1`, `ui-tests.ps1`, `hovershot.ps1` and `apptext.ps1` are Windows only**: they drive the
app's settings window through Windows UI Automation (WinApp CLI), which has no counterpart here for macOS.
The scripts work with the game and the app in any of the 13 game languages: no UI name or status
pattern is written in one language, see [Languages](#languages). The click points and the boxes of the screen
checks are the same in every language (the layout doesn't depend on the text), but they belong to a platform
and an aspect ratio at UI scale 100%: they are all in [`calibration.psd1`](calibration.psd1), calibrated for
Windows at 16:9 and for macOS at 1280x804 (all but the download bar), see [Calibrating](#calibrating).

## Prerequisites

- Windows, or macOS for the game scripts, and PowerShell 7 (`pwsh`).
- On macOS: the Xcode command line tools (`swiftc`), and for the app that runs the scripts (Terminal,
  VS Code) the **Accessibility** permission for the input and **Screen Recording** for the screenshots
  (System Settings → Privacy & Security). `common.ps1` compiles `tf3mac.swift` into
  `artifacts/bin/tf3mac` on first use. The screen checks and the calibration need no further tool: the
  pixels of a screenshot are read by `tf3mac pixels` (ImageIO).
- WinApp CLI 0.7 on the `PATH` (`winapp ui ...`),
  for `apptext.ps1`, `hovershot.ps1`, `ui-tests.ps1` and `game-tests.ps1`.
- Transport Fever 3 through Steam, with the mod linked (`tools/install-mod.ps1`, a symbolic link on
  macOS) and activated in a savegame, as in [docs/testing.md](../../docs/testing.md#before-a-run).
- Discord desktop, running and logged in.
- A published copy of TF3-DRPC outside the repository, started from there, with its own `DATA`
  folder next to the exe (`dotnet publish` in `companion/`, then copy the output).

## The scripts

Run them from this folder, or give the path. `$PSScriptRoot` is used everywhere, so the
working directory doesn't matter. Relative output paths count from where you are.

| Script | What it does | Example |
|---|---|---|
| `common.ps1` | Not run by itself. Finds the game's user data folder like the app (the Steam folder from the registry, on macOS `~/Library/Application Support/Steam`, then `userdata/<id>/3493540/local` of the account whose `crash_dump/stdout.txt` was written last), reads the game log, resolves output paths, compiles the macOS helper, picks the calibration set (`Get-Calibration`, `Get-CalibrationValue`). Also the language helpers (`Set-TestLanguage`, `Get-AppText`, `Get-PresetText`, `Format-AppDate`, `Get-PresencePatterns`, see [Languages](#languages)). Dot-sourced by the others. | |
| `calibration.psd1` | Not run by itself. Every click point and every box and threshold of the screen checks, one set per platform and screenshot height (`windows-720`, `macos-804`); `$null` is "not calibrated". Read by `common.ps1`. See [Calibrating](#calibrating). | |
| `screen.ps1` | Not run by itself. Pixel checks on a `tf3ctl.ps1` screenshot (Windows and macOS): the Mod Hub download bar, the main menu, the game menu, the Quit flyout, photo mode. They look at the layout, not at text, so they work in every language, and take their boxes from `calibration.psd1`. Dot-sourced by `run-game.ps1` and `tf3ctl.ps1`. | |
| `tf3mac.swift` | Not run by itself. The macOS helper behind `tf3ctl.ps1` and `winshot.ps1`: finds a window, brings an app to the front, sends clicks, keys and text through CoreGraphics. Text is typed with the keys of the current keyboard layout (Shift and Option included), as a person types it; characters off the layout go as Unicode. In one run a Mac Catalyst text field took real keys but not Unicode-only events; the cause wasn't found, and later runs took both. `tf3mac pixels <png> <out> [width]` writes a PNG's pixels (sRGB, B, G, R, A bytes, optionally scaled to a width) for `screen.ps1`. Formatted with `swift-format` ("Swift: Format document" in VS Code) by the rules in `.swift-format` next to it: 4 spaces, lines up to 140 characters. | |
| `run-game.ps1` | Game life cycle: `prepare [-Language <code>]`, `start`, `load [-SaveName <name>]`, `quit [-Editor]`, `restore`. Without a command it prints the usage. | `./run-game.ps1 start` |
| `tf3ctl.ps1` | Drives the game window: `shot`, `size`, `click`, `rclick`, `key`, `type`, `photo <png>`, `state`; and for a saved screenshot, without the game, `classify <png>` (add `-Verbose` for the numbers behind every check) and `measure <png> <x> <y> <w> <h>` (box statistics, for [Calibrating](#calibrating)). Screenshots are 1280 px wide and click points are in that picture. Refuses input unless TF3 is in front. | `./tf3ctl.ps1 shot game.png` |
| `appctl.ps1` | The same for another process by name (700 px scale), plus `wheel` and `wheeldown`. | `./appctl.ps1 TF3-DRPC shot app.png` |
| `winshot.ps1` | Screenshot of a window without focusing it or sending input; `-Width` sets the width in pixels (the app window at its native size, for example). On macOS only for a window on the current Space, so not for the game behind another app; use `tf3ctl.ps1 shot` there. | `./winshot.ps1 TF3-DRPC app.png` |
| `shot.ps1` | Screenshot of the TF3-DRPC window by its DWM bounds (a screen copy), optionally resized first. | `./shot.ps1 small.png 400 500` |
| `resize.ps1` | Resizes a window and reports the size it got. | `./resize.ps1 TF3-DRPC 400 500` |
| `hovershot.ps1` | Windows only. Opens the preset picker, hovers an entry (a preset key such as `network`, or a literal text), captures the screen region with its tooltip. | `./hovershot.ps1 network tip.png` |
| `apptext.ps1` | Windows only. Prints the settings window's texts in tree order. | `./apptext.ps1 -Limit 20` |
| `laststatus.ps1` | Prints the last `[drpc-status]` line's values (rank, date, lines, balance, save name, ...). Reads the log only. | `./laststatus.ps1` |
| `ui-tests.ps1` | Windows only. Switches the presets through UI Automation without the game; checks the Custom section, the preview's second line, `settings.json` and screenshots. Needs an existing `DATA\settings.json` (a fresh app writes it only after a setting changes). | `./ui-tests.ps1 -AppFolder <your app folder>` |
| `game-tests.ps1` | Windows only. The in-game checks, in phases `presets`, `alternating`, `rerun` and `custom`: all presets, the custom templates and both pause paths. | `./game-tests.ps1 -Phase presets -AppFolder <your app folder>` |

Each script has a comment header with its parameters. Screenshots and results go to
`artifacts/` in this folder (`-OutFolder` where there is one), which git ignores.

## Languages

The game and the app each come in 13 languages (`de`, `en`, `es`, `fr`, `it`, `ja`, `ko`, `nl`, `pl`,
`pt_BR`, `ru`, `zh_CN`, `zh_TW`). The two settings are independent, but the app follows the game by default
("Same as the game").

- **The game's language** is in `<userdata>/profile.lua` (`language = { code = "de", ... }`), not in
  `settings.lua`. In the menu a change needs a restart, and in a running game the field is locked, so the
  clean way is `run-game.ps1 prepare -Language <code>` with the game closed. It backs up `profile.lua` next
  to the `settings.lua` backup, and `restore` puts both back.
- **The app's language** is `language` in its `DATA\settings.json`: a code, or `game`.
- **The language under test** is what `ui-tests.ps1`, `game-tests.ps1` and `hovershot.ps1` use for the app's
  texts. `-Language <code>` sets it; without it the script reads `settings.json` (`-AppFolder`, or the folder
  of the running app for `hovershot.ps1`), and for `game` the code in `profile.lua`. It is printed at the start.
  If you passed a code that is not the language the app really shows, the checks fail.
- **The app's own texts** (the picker's name and entries, the labels, the store button, the Discord
  connection) are looked up by key in `companion/Tf3Drpc.Core/Resources/Texts*.resx` (`Get-AppText`), with
  English as the fallback like in the app. The scripts hold no German or English UI name.
- **The status patterns** (`Get-PresencePatterns`) are built from the structure of what `PresenceBuilder`
  composes. What they check in every language: the app's own word "In game" exactly; the number and order of the
  parts, joined by ` · ` (preview lines by ` | `); digits where a count or the year stands; the date in the
  culture's year-and-month format of the app (`September 1905`, `septiembre de 1905`, `1905年9月`,
  `1905년 9월`; it is the culture's pattern turned into a regex, so Japanese, Korean and Chinese without spaces
  work); amounts as `$` and a number with optional `K`, `M`, `B` or `T`. What they cannot check: the game's own
  words (labels such as Lines or Paused, ranks, modes, climates, month names) stand as "words without digits",
  because the app takes them from the game's text files. A wrong translation of those is not caught here.
- **Click points** don't depend on the language: the main menu "Load Game" tile, the game menu entries and the
  Quit flyout were the same in German and English.

## A typical run

1. `./run-game.ps1 prepare -Language de` with the game closed: backs up `settings.lua` and switches
   `screenMode` to `BORDERLESS` (in exclusive fullscreen the screenshots show only the splash), and with
   `-Language` backs up `profile.lua` and sets the game language. Leave `-Language` out to keep the language.
   On macOS the game starts in `BORDERLESS` already.
2. Start the published `TF3-DRPC.exe` (from its own folder), then start it a second time to show the
   settings window.
3. `./run-game.ps1 start`: starts the game through Steam, waits for `Main menu is ready` written after the
   process start, then waits until the Mod Hub downloads are finished: while they run the main menu shows a
   progress bar under the account box, and loading a game then must be avoided. The game log has no line for
   the end of the downloads, so the script looks at the bar on screenshots (every 10 s, up to
   `-DownloadTimeoutSeconds`, default 900) and prints the progress. On macOS the download bar isn't calibrated, so
   it prints a warning and doesn't wait (wait by hand).
4. `./run-game.ps1 load -SaveName <your save>`: clicks the "Load Game" tile, types the name into the filter
   box, clicks the first card's Load Game button, waits for `Loading game from file`, and from 15 s on clicks
   "Start Game" every 20 s until `Game is ready` arrives; the game writes that line only after the click. Without
   `-SaveName` it loads the first card, the save written last. It prints the log line with the file that
   loads. It checks that the main menu is showing, and waits for downloads again (both skipped with a warning where the
   screen checks are not calibrated). `continue` is the old name and
   runs `load`: the scripts don't use the "Continue" tile, which build 40423 dropped and 40425 shows again on macOS.
5. `./game-tests.ps1 -Phase presets -AppFolder <your app folder>`, then `alternating`; then
   `custom -SaveName <your save>` (the save name is read from the last status line if left out).
   `./ui-tests.ps1` needs no game. Add `-Language <code>` if the app's setting is not the language you test.
6. `./tf3ctl.ps1 photo game.png` (optional): photo mode for a picture of the game without HUD, see below.
7. `./run-game.ps1 quit`: looks at the screen, opens the game menu if it is closed, clicks Quit and Return to
   Desktop; waits for the process to end and reports the `Normal shutdown` or `Ungraceful exit` line. Where the
   screen checks are not calibrated it warns and goes blind: Escape, then the Quit and Return to Desktop points.
8. `./run-game.ps1 restore` with the game closed: puts the original `settings.lua` and `profile.lua` back. The
   game rewrites both when it quits, so restore only after the game has ended.

### The screen checks

`quit` doesn't press Escape blindly: the game menu has state (Escape opens and closes it, and also closes
open windows first). `screen.ps1` tells the screens apart by their layout in a screenshot (the numbers here are the Windows 16:9 ones
of `calibration.psd1`):

| State | How it is recognised |
|---|---|
| `game-menu` | The first four of the five dark grey button panels at the left are flat dark grey (x 75..410, centres at y 235, 297, 360, 422) and carry a white icon. |
| `quit-flyout` | The game menu, and two dark buttons beside Quit (Return to Main Menu, Return to Desktop). |
| `main-menu` | The settings, achievements, news and exit icons at the top right are white on the dark background. |
| `other` | Anything else: the game with the menu closed, a window, a loading screen, the settings screen. |

`quit` repeats up to eight times: in `other` it presses Escape, then (alternating) clicks the game menu button,
because the game sometimes ignored Escape after another window was in front; in `game-menu` it clicks Quit; in
`quit-flyout` it clicks Return to Desktop; in `main-menu` it clicks the exit icon. If the menu doesn't open it
fails with the last state instead of clicking blind. `./tf3ctl.ps1 state` prints the state of the game now,
`./tf3ctl.ps1 classify <png>` that of a saved screenshot (no game, no input; `-Verbose` shows the mean colour,
spread and bright pixels of every box), which is how to check the classifiers after a game update.

`tf3ctl.ps1 photo <png>` needs the running game with the menu closed. It clicks the eye icon (Points.Eye, 1006,710 on
Windows), which hides the HUD, checks that the hint box (Esc Back, F9 Screenshot) is there, presses F9, waits for the new file in
`<userdata>/screenshots`, copies it to `<png>` (full resolution, no HUD, none of the external overlays that a window
screenshot shows) and presses Escape to leave photo mode. F9 outside photo mode does nothing, which is why the hint
box is checked first.

## Limits

- **Coordinates.** The click points and the screen check boxes use the 1280 px wide screenshot as the coordinate
  system and are all in `calibration.psd1`, one set per platform and screenshot height (`tf3ctl.ps1 size`). The
  **Windows set (16:9, UI scale 100%, measured on build 40423, checked on 40425 on 2026-10-24)**: main menu "Load Game" tile 370,627; filter box of the Load Game
  screen 180,112; the first card's Load Game button 176,322 (its gear is at 278,322); "Start Game" 1192,653; game menu
  button at the bottom right 1259,686; game menu entries at x=240: Resume y=235, Save 297, Load 360, Settings 422, Quit
  485; Quit flyout "Return to Main Menu" 540,485 and "Return to Desktop" 788,485; main menu settings gear 1079,50
  and exit icon 1188,50; photo mode eye icon 1006,710. The **macOS set (1280x804, build 40425)**, measured on
  2026-10-24: "Load Game" tile 372,670 (build 40425 has a "Continue" tile again, above it); filter box 180,153; the
  first card's Load Game button 177,366; "Start Game" 1192,736; game menu button 1259,769; game menu entries at x=240:
  Resume y=277, Save 339, Load 402, Settings 464, Quit 527; Return to Desktop 788,527; exit icon 1188,50; eye icon
  1007,794. The map editor's points (240,558 and 788,558) are from older runs and weren't measured again, and the
  download bar isn't calibrated (no Mod Hub download ran), so `start` doesn't wait for downloads there and
  `tf3ctl.ps1 state` and `classify` print `downloads=not-calibrated`. On Windows, build 40425 left the "Load Game"
  tile where it was: `start`, `load`, `classify` of the game menu and the Quit flyout, `photo` and `quit` worked
  with the set unchanged on 2026-10-24 (the map editor's points and the download bar weren't exercised). A script that needs
  a value that is `$null` stops before the first click with a message naming it; `-QuitPoint` and the other
  parameters of `run-game.ps1` (`-LoadGameTilePoint`, `-FilterPoint`, `-SaveCardPoint`, `-StartGamePoint`,
  `-MenuButtonPoint`, `-QuitPoint`, `-ToDesktopPoint`, `-ExitPoint`) and `tf3ctl.ps1 photo`'s `-EyePoint` override the
  table, which is also how to try a point before it goes into the file. While the screen checks of a set are `$null`,
  `start` doesn't wait for the downloads, `load` doesn't check the main menu and `quit` goes blind, each with a warning.
  The map editor's menu has one more entry ("Save and Play"), so its Quit and Return to Desktop are lower
  (EditorQuit and EditorToDesktop in the table: 240,517 and 788,517 on Windows, 240,558 and 788,558 on macOS; "Return
  to Main Menu" 540,517 on Windows): `quit -Editor` uses them without looking at the screen, because the menu check is
  calibrated for the normal game menu. Another aspect ratio or UI scale needs a set of its own; without one the
  scripts stop with "No calibration for ...".
- **The in-game menu has state.** Escape opens and closes it, and also closes only the Quit flyout when that is
  open, and an open window first. `quit` handles that by looking (see above). In a campaign mission Escape first
  skips the mission's dialogue and closes its tutorial pop-ups, and during the opening dialogue the menu button
  doesn't open the menu either; closing the window (`CloseMainWindow`) does. In those cases use `-NoMenuCheck` or
  open the menu by hand.
- **Without `-SaveName`, `load` loads the save written last.** After a test save is deleted it can load another
  one (a tutorial, on 2026-10-21); `load` prints the `Loading game from file` line, so check it. With a name, the
  first card of the filtered list loads; a filter that matches several saves gives the newest of them.
- **The download bar** sits under the account box at about x 80..310, y 105..120 (blue fill, grey track; Windows). The
  check scans y 100..130, because a longer text can wrap the account box.
- **The language of the game can't change in a running game**; set it with `prepare -Language` while the game is
  closed. The app follows the game language only when its setting is "Same as the game", and then also changes.
- **One display.** Clicks go to screen coordinates and screenshots to the window's own picture;
  other layouts weren't tried.
- **The game keeps running** between two checks, so the date, the money and even the rank change.
  The checks use patterns and the latest status line, not fixed values.
- **Input needs the foreground.** An input script refuses to act when another
  window is in front. Do not touch the keyboard or the mouse during a run. On macOS the TF3-DRPC
  settings window opens over the full-screen game and stays in front until it is closed, so close
  it before sending input to the game.
- **Overlays.** An external performance overlay (FPS, CPU at the top centre, for example) can appear in the
  `tf3ctl.ps1 shot` pictures; the pixel checks look elsewhere. Photo mode pictures are free of it.
- **Never screenshot the Discord window**; it shows content unrelated to the test. Read the presence text
  in the app's settings window (`apptext.ps1`) instead. `shot.ps1` and `hovershot.ps1` copy a
  screen region, so keep Discord out of that region.

## Calibrating

Do this for any other aspect ratio or UI scale, or after a game update moves the menus: copy a set of
[`calibration.psd1`](calibration.psd1) under the key `<windows|macos>-<height>`, the height being what
`./tf3ctl.ps1 size` prints with the game running (`windows-720` and `macos-804` are the templates: same keys, same
meaning, see the comments in the file). Nothing here needs a clicked guess: every
number is read off a saved screenshot. The tools are `./tf3ctl.ps1 shot <png>` (1280 px wide, the picture the
numbers refer to), `./tf3ctl.ps1 measure <png> <x> <y> <w> <h>` and `./tf3ctl.ps1 classify <png> -Verbose`.

1. **Capture** with the game in BORDERLESS mode (macOS starts that way), UI scale 100%, and the language you
   will test in (the layout is the same in every language); name the pictures so that you know them again:
   `main-bar.png` (main menu while a Mod Hub download runs, if you can arrange one; else skip), `main.png` (main
   menu, no download), `load.png` (the Load Game screen with at least one save, once with the filter box clicked
   if the box looks different), `loading.png` (the loading screen with "Start Game", if it shows one), `game.png`
   (in game, menu closed), `menu.png` (game menu open, Escape), `quit.png` (game menu open and the Quit flyout
   open), `editor-menu.png` (the map editor's menu, if you use it), and `photo.png` (photo mode: take it with
   `shot` after clicking the eye icon by hand). Take every picture with `./tf3ctl.ps1 shot x.png` while the screen
   is showing it (the command brings the game to the front, so open the menus by hand or with `key ESCAPE` first).
2. **Points.** Open a picture in an image viewer that shows the pixel position of the cursor, or draw a selection (Preview
   shows its position and size), and read the centre of the thing to click: `LoadGameTile` and `Exit` (and
   the settings gear) from `main.png`, `Filter` and `SaveCard` (the first card's Load Game button) from `load.png`,
   `StartGame` from `loading.png`, `MenuButton` (bottom right) from `game.png`, `Quit` from `menu.png`, `ToDesktop`
   from `quit.png`, `EditorQuit` and `EditorToDesktop` from `editor-menu.png`, `Eye` from `game.png`. The numbers are
   in the 1280 px picture, whatever the real resolution. Try a point once with the parameter before it goes into the
   file (`./run-game.ps1 quit -QuitPoint 240,527 ...`, or `./tf3ctl.ps1 click x y` on a harmless screen).
3. **Boxes.** Each screen check compares a few boxes with limits. Find the same elements as in the Windows picture
   (the game menu panels, the white icons, the download bar) in your picture, place the box inside a uniform area, and read
   the numbers: `./tf3ctl.ps1 measure menu.png 300 223 100 24` prints the mean colour, the largest channel, the spread
   (largest minus smallest channel), the standard deviation and the number of bright pixels (every channel at least
   200) of a box, using the same sampling as the checks (every second pixel). Choose each limit between the values
   of the screens that must match and the highest value of the screens that must not:

   | Entry | Box | Must match | Must not match | Windows limit |
   |---|---|---|---|---|
   | `DarkPanel` | the panels of `GameMenu` and `QuitFlyout` | `menu.png`, `quit.png` | (used by both) | `MaxHigh` 70, `MaxSpread` 14, `MaxDeviation` 8 (measured 37..53, spread up to 7, deviation up to 4) |
   | `GameMenu` | `RowYs` the y of the centres of the first four panels, `PanelX0..X1` a patch of panel, `IconX0..X1` the white icon, `HalfHeight` | `menu.png`, `quit.png`: all four panels dark and each icon box with `MinIconBright` or more bright pixels | `main.png`, `game.png`, the settings screens | `MinIconBright` 25 (measured 62..70) |
   | `QuitFlyout` | two patches (`Button1`, `Button2`) of the dark buttons beside Quit, in the row of `Points.Quit` | `quit.png` | `menu.png` | same dark limits (measured 41..53) |
   | `MainMenu` | `IconXs` x0, x1 of the four icons at the top right, rows `Y0..Y1` | `main.png`, `main-bar.png`: `MinBright` or more bright pixels in each box | `menu.png` (the game menu has white text in that corner), `game.png` | `MinBright` 35 (icons 46 or more, menu text at most 25) |
   | `DownloadBar` | rows `Y0..Y1`, columns `X0..X1` every `XStep`; the `Track` colour and `TrackTolerance`; the fill is blue (`FillMinBlue`, blue over red by `FillBlueOverRed`); a row counts at `RowFraction` of its samples, the bar needs `MinRows` rows | `main-bar.png` | `main.png`, `load.png` (its cards have a few such rows) | track 46,54,61 +-6, 6 rows of 11 |
   | `PhotoMode` | `Boxes`: three patches (x0, x1, y0, y1) in the translucent hint box (Esc Back, F9 Screenshot), `MaxHigh`, `MaxSpread`, optionally `MinHigh` | `photo.png` | `game.png`, `loading.png` (its black band passes without `MinHigh`) | `MaxHigh` 62, `MaxSpread` 16 (measured 41..54); macOS `MinHigh` 30 |

   Measure the colour of a single pixel with a 1x1 box (`measure x.png 100 108 1 1`) for the download bar's track.
4. **Write** the numbers into the new set of `calibration.psd1`, replacing `$null`: a point is
   `@(x, y)`, a screen check is a block like the Windows one, copied and changed.
5. **Verify** with no game running: `./tf3ctl.ps1 classify <png>` on every picture must answer
   as follows, and `-Verbose` shows the numbers when one does not:

   | Picture | `state` | `downloads` | `photo` |
   |---|---|---|---|
   | `main-bar.png` | `main-menu` | `True` | `False` |
   | `main.png`, `load.png` (a screen that is not the menu: `other`) | `main-menu` / `other` | `False` | `False` |
   | `game.png` | `other` | `False` | `False` |
   | `menu.png` | `game-menu` | `False` | `False` |
   | `quit.png` | `quit-flyout` | `False` | `False` |
   | `photo.png` | `other` | `False` | `True` |

   Then with the game running, `./tf3ctl.ps1 state` in the main menu and in game, `./run-game.ps1 load` and
   `quit` (watch that the warnings about missing calibration are gone), and `./tf3ctl.ps1 photo x.png`.

## Placeholders to replace

No script holds a value of a particular machine. Where the scripts need one, it is a parameter or
a marker in angle brackets in the comments. Replace these with your own values:

| Placeholder | Meaning |
|---|---|
| `-AppFolder <your app folder>` | The folder of your published TF3-DRPC copy; its `DATA\settings.json` is checked. |
| `-SaveName <your save>` | The name of the savegame that is loaded, for `load` and the `custom` phase. |
| `-Language <code>` | A game language code (`de`, `en`, `es`, `fr`, `it`, `ja`, `ko`, `nl`, `pl`, `pt_BR`, `ru`, `zh_CN`, `zh_TW`): the game's language for `prepare`, the app's for the test scripts. |
| `-UserDataFolder <Steam>\userdata\<your Steam id>\3493540\local` | Only when the automatic lookup finds the wrong account or Steam isn't registered (the lookup falls back to the default Steam folder if the registry has no entry). |
| `-OutFolder <folder>` | Where screenshots and the `settings.lua` and `profile.lua` backups go; the default is the ignored `artifacts/` folder here. |
| `-StoreButtonText <text>` | Only to override the store button's label; by default it is the app's text in the language under test. |
