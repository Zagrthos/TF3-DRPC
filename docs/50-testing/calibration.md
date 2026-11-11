---
title: Calibrate the test-run scripts
type: guide
audience: [contributor, agent]
area: [tools, windows, macos]
covers:
  - tools/test-run/calibration.psd1
  - tools/test-run/screen.ps1
  - tools/test-run/tf3ctl.ps1
last-reviewed: 2026-11-11
verified:
  - windows 2026-10-24 build 40425
  - macos 2026-10-24 build 40425
---

# Calibrate the test-run scripts

How to measure the click points and screen check boxes of the [scripted test runs](scripted-runs.md) for another aspect ratio or UI scale, or after a game update moves the menus.

## Before you start

Copy a set of [`calibration.psd1`](../../tools/test-run/calibration.psd1) under the key `<windows|macos>-<height>`, the height being what `./tf3ctl.ps1 size` prints with the game running.
`windows-720` and `macos-804` are the templates: same keys, same meaning, see the comments in the file.
Nothing here needs a clicked guess: every number is read off a saved screenshot.
The tools are `./tf3ctl.ps1 shot <png>` (1280 px wide, the picture the numbers refer to), `./tf3ctl.ps1 measure <png> <x> <y> <w> <h>` and `./tf3ctl.ps1 classify <png> -Verbose`.

## Steps

1. **Capture** with the game in BORDERLESS mode (macOS starts that way), UI scale 100%, and the language you will test in (the layout is the same in every language); name the pictures so that you know them again:
   - `main-bar.png`: main menu while a Mod Hub download runs, if you can arrange one; else skip.
   - `main.png`: main menu, no download.
   - `load.png`: the Load Game screen with at least one save, once with the filter box clicked if the box looks different.
   - `loading.png`: the loading screen with "Start Game", if it shows one.
   - `game.png`: in game, menu closed.
   - `menu.png`: game menu open, Escape.
   - `quit.png`: game menu open and the Quit flyout open.
   - `editor-menu.png`: the map editor's menu, if you use it.
   - `photo.png`: photo mode; take it with `shot` after clicking the eye icon by hand.

   Take every picture with `./tf3ctl.ps1 shot x.png` while the screen is showing it (the command brings the game to the front, so open the menus by hand or with `key ESCAPE` first).
2. **Points:** Open a picture in an image viewer that shows the pixel position of the cursor, or draw a selection (Preview shows its position and size), and read the centre of the thing to click:
   - `LoadGameTile` and `Exit` (and the settings gear) from `main.png`.
   - `Filter` and `SaveCard` (the first card's Load Game button) from `load.png`.
   - `StartGame` from `loading.png`.
   - `MenuButton` (bottom right) from `game.png`.
   - `Quit` from `menu.png`.
   - `ToDesktop` from `quit.png`.
   - `EditorQuit` and `EditorToDesktop` from `editor-menu.png`.
   - `Eye` from `game.png`.

   The numbers are in the 1280 px picture, whatever the real resolution.
   Try a point once with the parameter before it goes into the file (`./run-game.ps1 quit -QuitPoint 240,527 ...`, or `./tf3ctl.ps1 click x y` on a harmless screen).
3. **Boxes:** Each screen check compares a few boxes with limits.
   Find the same elements as in the Windows picture (the game menu panels, the white icons, the download bar) in your picture, place the box inside a uniform area, and read the numbers: `./tf3ctl.ps1 measure menu.png 300 223 100 24` prints the mean colour, the largest channel, the spread (largest minus smallest channel), the standard deviation and the number of bright pixels (every channel at least 200) of a box, using the same sampling as the checks (every second pixel).
   Choose each limit between the values of the screens that must match and the highest value of the screens that must not:

   | Entry | Box | Must match | Must not match | Windows limit |
   |---|---|---|---|---|
   | `DarkPanel` | the panels of `GameMenu` and `QuitFlyout` | `menu.png`, `quit.png` | (used by both) | `MaxHigh` 70, `MaxSpread` 14, `MaxDeviation` 8 (measured 37..53, spread up to 7, deviation up to 4) |
   | `GameMenu` | `RowYs` the y of the centres of the first four panels, `PanelX0..X1` a patch of panel, `IconX0..X1` the white icon, `HalfHeight` | `menu.png`, `quit.png`: all four panels dark and each icon box with `MinIconBright` or more bright pixels | `main.png`, `game.png`, the settings screens | `MinIconBright` 25 (measured 62..70) |
   | `QuitFlyout` | two patches (`Button1`, `Button2`) of the dark buttons beside Quit, in the row of `Points.Quit` | `quit.png` | `menu.png` | same dark limits (measured 41..53) |
   | `MainMenu` | `IconXs` x0, x1 of the four icons at the top right, rows `Y0..Y1` | `main.png`, `main-bar.png`: `MinBright` or more bright pixels in each box | `menu.png` (the game menu has white text in that corner), `game.png` | `MinBright` 35 (icons 46 or more, menu text at most 25) |
   | `DownloadBar` | rows `Y0..Y1`, columns `X0..X1` every `XStep`; the `Track` colour and `TrackTolerance`; the fill is blue (`FillMinBlue`, blue over red by `FillBlueOverRed`); a row counts at `RowFraction` of its samples, the bar needs `MinRows` rows | `main-bar.png` | `main.png`, `load.png` (its cards have a few such rows) | track 46,54,61 +-6, 6 rows of 11 |
   | `PhotoMode` | `Boxes`: three patches (x0, x1, y0, y1) in the translucent hint box (Esc Back, F9 Screenshot), `MaxHigh`, `MaxSpread`, optionally `MinHigh` | `photo.png` | `game.png`, `loading.png` (its black band passes without `MinHigh`) | `MaxHigh` 62, `MaxSpread` 16 (measured 41..54); macOS `MinHigh` 30 |

   Measure the colour of a single pixel with a 1x1 box (`measure x.png 100 108 1 1`) for the download bar's track.
4. **Write** the numbers into the new set of `calibration.psd1`, replacing `$null`: a point is `@(x, y)`, a screen check is a block like the Windows one, copied and changed.
5. **Verify** with no game running: `./tf3ctl.ps1 classify <png>` on every picture must answer as follows, and `-Verbose` shows the numbers when one does not:

   | Picture | `state` | `downloads` | `photo` |
   |---|---|---|---|
   | `main-bar.png` | `main-menu` | `True` | `False` |
   | `main.png`, `load.png` (a screen that is not the menu: `other`) | `main-menu` / `other` | `False` | `False` |
   | `game.png` | `other` | `False` | `False` |
   | `menu.png` | `game-menu` | `False` | `False` |
   | `quit.png` | `quit-flyout` | `False` | `False` |
   | `photo.png` | `other` | `False` | `True` |

   Then with the game running, `./tf3ctl.ps1 state` in the main menu and in game, `./run-game.ps1 load` and `quit` (watch that the warnings about missing calibration are gone), and `./tf3ctl.ps1 photo x.png`.

## Check

`./tf3ctl.ps1 classify <png>` answers as in the table of step 5 for every picture, and a run with the game starts, loads and quits without a warning about missing calibration.
