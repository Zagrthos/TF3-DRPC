# Testing

How to check the mod and the companion app by hand, on Windows; what differs on macOS is in [On macOS](#on-macos). The automated tests (`dotnet test` in `companion/`) cover the Core, including the monitor's tick (`GameMonitor`); they can't cover the game, Discord or the MAUI app (its timer loop and UI).

## Before a run

1. Link the mod into the game: `./tools/install-mod.ps1`. The game then loads it from this repository, so the branch you have checked out is the mod you test.
2. **Activate the mod in the savegame, and save once.** Load dialogue → gear button next to the savegame → tick "Discord Rich Presence" → load → save right away under the same name.
   - Without the save, the activation counts only for that one load. The mod list printed when a savegame loads (`Active mods:` in the game log) shows whether `zagrthos_drpc` is active.
   - Campaign missions always start with only their own mod, so the mod never runs there.
3. Publish the app: `dotnet publish Tf3Drpc.App/Tf3Drpc.App.csproj -c Release -r win-x64` in `companion/`.
4. Copy the publish folder somewhere outside the repository, delete any `DATA` folder in the copy, and start `TF3-DRPC.exe` from there. The app writes its `DATA` folder next to the exe.
5. Discord desktop must be running and logged in.

### On macOS

The end-to-end run with the game and Discord was done on a Mac on 2026-10-20 (macOS 27, the
Native AOT publish), with every step of the table below except the preset tooltips (Windows
only then; checked on macOS afterwards, see below) and the double-click on the tray icon (see
the menu bar below). What differs from the steps above:

- `./tools/install-mod.ps1` runs under `pwsh` and makes a symbolic link. The game's user data
  and log are at the same place under `~/Library/Application Support/Steam`
  ([game-log.md](game-log.md#finding-the-game-and-its-log)).
- Publish with `-r maccatalyst-arm64`; the bundle `TF3-DRPC.app` is in
  `artifacts/bin/Tf3Drpc.App/release_maccatalyst-arm64/`. Copy it outside the repository and
  start it there; `DATA` is created next to the `.app`.
- A copy that carries the quarantine flag (a download) is started from a hidden read-only copy
  (App Translocation): the settings window opens at start and asks to move the app once with
  Finder. An unsigned or ad-hoc-signed download is first stopped by Gatekeeper ("Open Anyway"
  in Privacy & Security).
- The tray is the menu bar icon, with the same menu. A click or a double-click on it opens the
  menu, as for any menu bar icon; the settings window opens with "Settings…" there. A second
  start hands over to the running app, which shows its settings window. Cmd+Q in the settings
  window quits like Exit; Cmd+W only hides it, like the close button.
- The preset tooltips appear when the pointer rests on an entry of the open "Status appearance"
  menu, beside the menu. The menu is a window of its own, so capture the screen region; find
  the entry's frame through the Accessibility API (its `AXMenuItem`).
- The theme follows the macOS appearance (System Settings → Appearance) with "Same as the
  system"; with "Light mode" or "Dark mode" the native title bar has to match the page.
- The game runs in a full-screen Space of its own. The settings window opens over it and stays
  in front until it is closed, so close it before you send input to the game by script
  (`tf3ctl.ps1` refuses input while another window is in front).
- "Close the game window" is Cmd+Q in the game: it logs `Quit requested` and the game stays
  in its menu, as on Windows.
- The scripted steps of [Runs driven by script](#runs-driven-by-script) cover only the game
  on macOS (`ui-tests.ps1`, `game-tests.ps1`, `hovershot.ps1` and `apptext.ps1` are Windows only). On macOS the window can be driven
  through the Accessibility API instead: System Events for the menu bar menu (`click menu bar
  item 1 of menu bar 2` of process "TF3-DRPC"), and the window's controls by their positions,
  which change when the preview's height changes, so look them up again after each change.
  `tf3mac` clicks and types there; the status line is the window's first static text.
- On 2026-10-24 (macOS 27, build 40425) the game scripts ran on a Mac with the measured set
  `macos-804`: `run-game.ps1 start`, `load -SaveName`, `quit` (from the game and from the Quit
  flyout), `tf3ctl.ps1 state` on every screen and `tf3ctl.ps1 photo`; the screen checks found
  each screen. The download bar couldn't be measured, because no Mod Hub download ran.

## Steps and what Discord shows

The texts follow the game language ("Same as the game"); in German, for example:

| Step | Discord |
|---|---|
| Start the game | "Spiel wird gestartet", then "Im Hauptmenü"; the timer counts from the game start (not in Discord: the settings window shows "Transport Fever 3 · Build <n>" under the game status) |
| Load a savegame with the mod ("Spiel laden" in the main menu and the save's card; click "Spiel starten" if the loading screen shows it) | "Laden...", then about 10–15 s after the game is ready the rank (e.g. "Mechaniker") and below it month and year (e.g. "September 1905"); the timer starts at 0 |
| Pause with the pause button (Space), wait about 15 s | " · Pausiert" after the date; rank and date stay |
| Resume, then open the game menu (Escape) and wait at least 40 s | " · Pausiert" again, now because no status line arrives |
| Close the menu and resume, wait about 20 s | " · Pausiert" disappears |
| Tray menu → Language → English | The tray menu at once, and Discord within about 15 s, switch to English ("Mechanic", "September 1905") |
| Double-click the tray icon | The settings window opens centred on the main display, showing the game state and the Discord connection; closing it only hides it |
| Start a second `TF3-DRPC.exe` | It exits at once, and the running app shows its window |
| Settings window → "Statusanzeige" → each preset in turn | Within about 15 s: Netzwerk "Linien 44 · Fahrzeuge 118" / "Stationen 74 · 1905", Epoche "Jahr 1905 · Tropisch" / "Mechaniker", Minimal "Im Spiel · 1905", Abwechselnd Karriere, Netzwerk and Epoche in turn about every 45 s (values from the save). The preview shows the same without "Beispielwerte", and `settings.json` keeps the preset |
| Settings window → "Design" → "Heller Modus", then "Dunkler Modus", then "Wie im System" | The window switches at once; "Wie im System" follows the Windows setting (Settings → Personalisation → Colours); `settings.json` keeps `"theme"` as `"light"`, `"dark"` or `"system"` |
| Hover an entry of the open "Statusanzeige" list | A tooltip shows that preset's two lines with the current values; Abwechselnd lists its three layouts |
| "Benutzerdefiniert", type `{rank} · {balance}`, `{companyValue} · {saveName}` and `{climate} · {passengers}` | The preview at once, Discord within about 15 s: "Mechaniker · $25M" / "$79M · <savegame name>" (values from the save); `settings.json` has the templates a second after typing. The 🔒 marks the private placeholders |
| Pause with a second line of 150 characters | The second line is shortened and ends in "… · Pausiert" |
| Type a template and press Exit within a second | The app quits and `settings.json` has the new template (the same for the tray's Exit) |
| Settings window → tick "Schaltfläche zum Steam-Store zeigen" | The preview shows a frame "Hol dir Transport Fever 3" under its lines; within about 15 s others see the button "Hol dir Transport Fever 3" below your status (it opens the Steam page). You can't see your own button, so check from a second account, or note that you couldn't. Untick it and the button disappears |
| Start a new game on a generated map ("Neues Spiel" → a climate) and look before the first save | With the preset Custom and `{saveName}` in a line, Discord shows "New Game" (or the chosen language's text, e.g. "Nouvelle partie" after switching to French) instead of the game language's "Neues Spiel"; after saving it shows the real name. A new game on a ready-made map ("Benutzerdefinierte Karte spielen") shows the map's name until it is saved, because the game names the savegame after the map |
| Leave the savegame | "Im Hauptmenü" |
| Start campaign mission 1 | "Kampagne · Mardi Gras retten" |
| Load a map in the map editor ("Karten-Editor") with the mod ticked under "Mods hinzufügen" in its gear dialogue, then open the editor's menu for 30 s | The rank, the mode and the date, e.g. "Junior · Map Editor · June 1960" in English; never " · Pausiert", also not with the menu open, because time doesn't run there and the mod always writes `speedup` 0 |
| Start a new game with the built-in mod "Tycoon" and this mod ticked in its "Mods" tab | "Tycoon · Juni 1960": the Tycoon mod fixes the rank at Tycoon, so the mode isn't shown again ("Tycoon · Tycoon") |
| Start a new game with the built-in mod "Sandkastenmodus" and this mod ticked | The rank, the game's text for the sandbox mode and the date |
| Load a copy of the savegame with the mod unticked in its gear dialogue | The game log shows `Removing entity … for GameScript zagrthos_drpc::/drpc/drpc.gs`; with no status line the app shows "Spielt" once the game is ready. Delete the copy afterwards |
| Close the game window while a game runs | `Quit requested` in the game log; if the game stays open in its menu, the presence stays too |
| Quit the game | The presence disappears within a few seconds |
| Tray menu → Exit | The tray icon disappears and the process ends |
| Settings window → Exit | Same as the tray menu's Exit |

Discord accepts an update at most every 15 s, so each change can take that long to show.

### Why the timing is like this

- **Status lines:** the mod writes one every 10 s of real time.
- **Pause:** with the pause button the mod keeps writing, with `speedup` 0. While the game menu is open its `guiUpdate` doesn't run and no line arrives, so the app counts the game as paused after 20 s without a status line.
- **Previous session:** at start the game log still holds the previous session until the game truncates it. The app ignores lines stamped before the game process started.

## Where to look

| What | Where |
|---|---|
| Game log, with the mod's `[drpc-status]` lines | `<Steam>\userdata\<id>\3493540\local\crash_dump\stdout.txt`; `<Steam>` is `~/Library/Application Support/Steam` on macOS |
| Active mods of the loaded savegame | `Active mods:` in the same log |
| App log | `DATA\logs\TF3-DRPC_<yyyy-MM-dd>.log` next to the exe (next to the `.app` on macOS): start, paths found, state changes, the game build, language, setting changes (theme, preset, templates, store button), Discord connection |
| Mod errors | `[drpc] reading <name> failed`, `[drpc] <key> has an unexpected type <type>` or `[drpc] writing the status failed` in the game log; each is logged once per name until the game is restarted |

Useful one-liners in Git Bash on Windows or in a shell on macOS; set `L` with the line for your system:

```sh
L="/c/program files (x86)/steam/userdata/<id>/3493540/local/crash_dump/stdout.txt"              # Windows
L="$HOME/Library/Application Support/Steam/userdata/<id>/3493540/local/crash_dump/stdout.txt"  # macOS
grep -c "zagrthos_drpc (rev" "$L"                                    # mod active in this load?
grep -n -E "Game is ready|\[drpc|Game is stopped" "$L" | cut -c1-200  # status cadence and errors
```

## Other checks

- **Live Discord test without the game:** `TF3DRPC_DISCORD_LIVE=1`, then `dotnet test TF3-DRPC.slnx -- --filter-method '*LiveDiscord*'`. It shows "Test / Test" with the button "Get Transport Fever 3" on your profile for 5 s, and fails if Discord answers with an error.
- **Translations in the exe:** `grep -c -a "Spiel wird gestartet" TF3-DRPC.exe` must be 1 (on macOS in `TF3-DRPC.app/Contents/MacOS/TF3-DRPC`). Native AOT drops them silently if `AddReferenceSatellitesToIlc` breaks.
- **Writes outside `DATA`:** set a marker file, run the app for a while, then list files newer than the marker. Keep it bounded: about three levels deep in `%LOCALAPPDATA%`, `%APPDATA%`, `Documents` and `%TEMP%`, with a time limit. Also query the registry keys `HKCU\Software\TF3-DRPC` and `HKCU\Software\Microsoft\Windows\CurrentVersion\Run`. On macOS: three levels deep in `~/Library` (`Application Support`, `Preferences`, `Caches`, `Containers`, `Saved Application State`, `HTTPStorages`, `WebKit`, `Logs`), `~/Documents` and `$TMPDIR`, filtered for `tf3drpc` and `zagrthos`. This bounded check is the one used; a full trace isn't planned (see [README.md](README.md#decisions)).
- **Locked daily log:** create `DATA/logs/TF3-DRPC_<today>.log` and hold it open with `FileShare.None` (for example from a PowerShell `[System.IO.FileStream]`), start the app and open the settings window with a second start. A red message says the log file can't be written, and no message about settings appears. Release the file and the next start logs normally.
- **Accessible names:** with the settings window open, `winapp ui inspect -a <pid> --interactive` (WinApp CLI 0.7) lists the pickers, the template entries, the store check box and Exit with their names ("Design", "Sprache", "Statusanzeige", "Erste Zeile", …), not empty. On macOS read `AXDescription` of the window's controls through the Accessibility API; a control scrolled out of view takes `AXPress` only after `AXScrollToVisible`, as VoiceOver does.
- **Minimum window with long texts:** set all three custom templates to 150 characters and make the window as small as it goes (400 × 500); the preview lines end in "…" after two lines, and the pickers and Exit stay on screen.
- **Discord quitting and starting:** quit Discord while a game runs. The app logs one `DiscordConnectionLost` warning and shows that it isn't connected, with no warning repeated after it; start Discord again and the app connects within about 15 s and sends the presence again.
- **Screen reader:** start Narrator (Ctrl+Win+Enter), bring the settings window to the front and press Tab through it; Narrator speaks each control's name ("Design", "Sprache", "Statusanzeige", "Erste Zeile", …, "Beenden"). Only a person can listen to it.
- **The image tooltip in Discord:** with a custom "Bild-Tooltip" template, open your own profile in Discord and hover the large image; it shows the template's text. Check this yourself; never screenshot the Discord window (see [Known traps](#known-traps)).
- **Smoke test of a publish:** start the exe, check `TrayCreated` in the app log after a few seconds, start it a second time (it must exit), stop the first with `Stop-Process`, and delete the publish folder's `DATA` again.

## Runs driven by script

Since 2026-10-17 the game runs have been driven by scripts instead of by hand. The
scripts are in [tools/test-run/](../tools/test-run/); its
[README](../tools/test-run/README.md) lists them, the order of a run and what to replace for
your machine. These are the parts that matter:

- **The game:** `run-game.ps1 prepare` switches it to `BORDERLESS` first (see below),
  `run-game.ps1 start` starts it with `steam://rungameid/3493540` and waits for
  `Main menu is ready` in the game log. `tf3ctl.ps1` sends clicks and keys through `SendInput`
  (keys as scan codes) after `SetForegroundWindow`; it takes screenshots with `PrintWindow`, and
  you click by the coordinates in them (the 1280 px wide picture at 16:9; the points are the
  same in German and English, because they follow the layout, not the text).
  On macOS the same commands go through a Swift helper (`tf3mac.swift`, CoreGraphics events, the
  Accessibility API to bring the game to the front, ImageIO to read the screenshot's pixels) and
  `screencapture`. Every click point and screen check box is in `tools/test-run/calibration.psd1`,
  one set per platform and screenshot height: Windows 16:9 and macOS (1280x804, 2026-10-24) are
  calibrated, except the macOS download bar (a value that is `$null` stops a script before it
  clicks, and its screen check is skipped with a warning). The README's
  [Calibrating](../tools/test-run/README.md#calibrating) says how to measure a set or a missing
  value, and how to check a set after a game update.
- **The language:** the game keeps its language in `<userdata>/profile.lua`, not in
  `settings.lua`, and the field is locked in a running game. `run-game.ps1 prepare -Language <code>`
  sets it while the game is closed (and `restore` puts it back), so a run can cover any of the 13
  languages. The app follows the game ("Same as the game") or has its own language in
  `DATA\settings.json`. `ui-tests.ps1`, `game-tests.ps1` and `hovershot.ps1` look up the app's texts
  by key in the resx files and build the status patterns from the structure, so the same scripts
  work in every language (`-Language <code>` if the app's setting isn't the language to test; see
  the [README](../tools/test-run/README.md#languages) for what the patterns can and cannot check).
- **Loading and leaving:** build 40423's main menu had no "Continue" tile; build 40425 on macOS
  has one again, above "Load Game", which the scripts don't use. On Windows build 40425 shows the
  tutorial tile there instead, and "Load Game" stays where it was (checked 2026-10-24).
  `run-game.ps1 start` waits for `Main menu is ready` and then until the Mod Hub download bar under
  the account box is gone (a game must not load while mods download). `run-game.ps1 load` clicks
  the "Load Game" tile, types `-SaveName` into the filter box if given, clicks the first card's
  Load Game button, clicks "Start Game" if the loading screen shows it, and waits for
  `Game is ready`. Without a name the first card, the save written last, loads, so after a test
  save is deleted it can open another one (on 2026-10-21 it opened `tutorial_temperate_1`, where
  the mod doesn't run); give `-SaveName` or check the `Loading game from file` line it prints.
  `run-game.ps1 quit` looks at the screen (the game menu has state: Escape closes an open menu, and
  closes open windows first), opens the menu if needed and leaves through Quit → Return to Desktop
  without saving; it fails with a message instead of clicking blind (where the screen checks are
  not calibrated it warns and leaves blind, as before).
- **Photo mode:** `tf3ctl.ps1 photo <png>` (needs the eye icon's point and the photo mode check
  of the set) clicks the eye icon, which hides the HUD, presses F9 and
  copies the full-resolution picture, without HUD and without external overlays, from
  `<userdata>/screenshots` (F9 does nothing outside photo mode); Escape leaves it.
- **The foreground lock:** Windows refuses `SetForegroundWindow` to a process that didn't
  send the last input, for example after UI Automation calls on the app. A short Alt key
  press through `SendInput` before the call lifts it. Even then the game sometimes ignored the
  first Escape after the settings window had been in front (seen twice on 2026-10-21); a click
  on its menu button at the bottom right (1259,686 in the screenshot) opens the menu reliably.
- **The settings window:** drive it with UI Automation through WinApp CLI 0.7 (`winapp ui`):
  start the app a second time to show the window, open a picker with
  `invoke <combo box> --action expand`, choose with `invoke <list item> --action select`
  (the open list is a popup window of its own: `search "<entry>" -a <pid>` names its handle,
  and `invoke` then needs `-w <handle>`; seen 2026-10-20), type templates with `set-value`, and read the texts with `inspect --json`. Selectors such
  as `cmb-7391` change when a combo box opens or closes, so look them up again each time;
  a text selector that matches both a list item and its text fails, so search and take the
  `ListItem`. Find a picker by its accessible name ("Design", "Sprache", "Statusanzeige"), not
  by its position: the theme picker came first on 2026-10-20 and the scripts that took the
  second combo box opened the language picker. Read the status in tree order: it is the text
  right above the Discord line. `ui-tests.ps1` (presets without the game), `game-tests.ps1`
  (the in-game checks) and `apptext.ps1` do this.
- **The tray:** the icon usually sits in the overflow area. Invoke the taskbar's "Show Hidden
  Icons" button (in the `Shell_TrayWnd` window; it toggles, so check first whether the
  overflow window `TopLevelWindowForOverflowXamlIsland` already shows the icon), find the
  `TF3-DRPC` button there, and right-click or double-click its position with real mouse input.
  The menu and its language submenu are `PopupHost` windows of the app, driven like the
  pickers. Typing into the settings window takes the focus and closes an open tray menu, so for
  the tray's Exit with a pending edit type first, then open the menu and invoke Exit at once.
- **Tooltips:** a tooltip opens in its own popup window, so capture the screen region, not
  the window, while `winapp ui hover` keeps the pointer on the entry (`hovershot.ps1`).
- **What Discord gets:** read it from the settings window (the game status line) and the app
  log, not from the Discord window (see below); `laststatus.ps1` prints the last status line's
  values to compare with.

## Known traps

- **PowerShell variable names are case-insensitive:** `$P` and `$p` (or `$s` and `$S`) are the same variable, which breaks scripts that use both.
- **The loading screen can wait:** after a savegame or a new game has loaded, the game may show "Spiel starten" and write `Game is ready` only after the click (seen on 2026-10-17 with a new game and on 2026-10-18 with "Fortsetzen"; an earlier "Fortsetzen" started without it). A script that waits for that line should take a screenshot and click it when it is there.
- **The game keeps running in the background of a script:** at 4× speed the date, the money and even the rank change between two checks, so compare with the latest status line, not with fixed values.
- **Stutter in the first 30 s after loading** comes from the game spawning industries (`Spawned industry … Connect industry: ~415 ms` in its log), not from the mod or the app.
- **Build and publish traps** (a locked icon file, an old `.pdb` in publish) are in [AGENTS.md](../AGENTS.md#quick-reference).
- **Screenshots of the game show only the start splash** while it runs in exclusive fullscreen. For a run driven by script, set `screenMode = "BORDERLESS"` in `<userdata>\settings.lua` while the game is closed, and set it back to `"FULLSCREEN"` afterwards; the game rewrites the file when it quits (`run-game.ps1 prepare` and `restore` do both, and refuse while the game runs).
- **Never take screenshots of the Discord window**; it shows content that has nothing to do with the test. Check the presence text in the app's settings window instead; it shows what the app sends.
