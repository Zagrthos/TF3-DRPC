---
title: Test by hand on Windows
type: guide
audience: [contributor, agent]
area: [app, mod, discord, windows]
covers:
  - tools/install-mod.ps1
  - companion/Tf3Drpc.App/Tf3Drpc.App.csproj
last-reviewed: 2026-11-13
---

# Test by hand on Windows

How to check the mod and the companion app by hand, on Windows; what differs on macOS is in [test by hand on macOS](manual-test-macos.md).
The automated tests (`dotnet test` in `companion/`) cover the Core, including the monitor's tick (`GameMonitor`); they can't cover the game, Discord or the MAUI app (its timer loop and UI).
The [testing strategy](strategy.md) says which change needs which test; the scripted version of these steps is in [scripted test runs](scripted-runs.md).

## Before a run

1. Link the mod into the game: `./tools/install-mod.ps1`.
   The game then loads it from this repository, so the branch you have checked out is the mod you test ([work on the mod](../20-development/mod-development.md)).
2. **Activate the mod in the savegame, and save once.**
   Load dialogue → gear button next to the savegame → tick "Discord Rich Presence" → load → save right away under the same name.
   - Without the save, the activation counts only for that one load.
     The mod list printed when a savegame loads (`Active mods:` in the game log) shows whether `zagrthos_drpc` is active.
   - Campaign missions always start with only their own mod, so the mod never runs there.
3. Publish the app (`companion/`, see [build and publish](../20-development/build-and-publish.md#steps)): `dotnet publish Tf3Drpc.App/Tf3Drpc.App.csproj -c Release -r win-x64`.
4. Copy the publish folder somewhere outside the repository, delete any `DATA` folder in the copy, and start `TF3-DRPC.exe` from there.
   The app writes its `DATA` folder next to the exe.
5. Discord desktop must be running and logged in.

## Steps and what Discord shows

The texts follow the game language ("Same as the game"); in German, for example:

| Step | Discord |
|---|---|
| Start the game | "Spiel wird gestartet", then "Im Hauptmenü"; the timer counts from the game start (not in Discord: the settings window shows "Transport Fever 3 · Build <n>" under the game status) |
| Load a savegame with the mod ("Spiel laden" in the main menu and the save's card; click "Spiel starten" if the loading screen shows it) | "Laden...", then about 10–15 s after the game is ready the rank (for example "Mechaniker") and below it month and year (for example "September 1905"); the timer starts at 0 |
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
| Start a new game on a generated map ("Neues Spiel" → a climate) and look before the first save | With the preset Custom and `{saveName}` in a line, Discord shows "New Game" (or the chosen language's text, for example "Nouvelle partie" after switching to French) instead of the game language's "Neues Spiel"; after saving it shows the real name. A new game on a ready-made map ("Benutzerdefinierte Karte spielen") shows the map's name until it is saved, because the game names the savegame after the map |
| Leave the savegame | "Im Hauptmenü" |
| Start campaign mission 1 | "Kampagne · Mardi Gras retten" |
| Load a map in the map editor ("Karten-Editor") with the mod ticked under "Mods hinzufügen" in its gear dialogue, then open the editor's menu for 30 s | The rank, the mode and the date, for example "Junior · Map Editor · June 1960" in English; never " · Pausiert", also not with the menu open, because time doesn't run there and the mod always writes `speedup` 0 |
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
- **Pause:** with the pause button the mod keeps writing, with `speedup` 0.
  While the game menu is open its `guiUpdate` doesn't run and no line arrives, so the app counts the game as paused after 20 s without a status line.
- **Previous session:** at start the game log still holds the previous session until the game truncates it.
  The app ignores lines stamped before the game process started.

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

- **Live Discord test without the game:** `TF3DRPC_DISCORD_LIVE=1`, then `dotnet test TF3-DRPC.slnx -- --filter-method '*LiveDiscord*'`.
  It shows "Test / Test" with the button "Get Transport Fever 3" on your profile for 5 s, and fails if Discord answers with an error.
- **Translations in the exe:** `grep -c -a "Spiel wird gestartet" TF3-DRPC.exe` must be 1 (on macOS in `TF3-DRPC.app/Contents/MacOS/TF3-DRPC`).
  Native AOT drops them silently if `AddReferenceSatellitesToIlc` breaks.
- **Writes outside `DATA`:** set a marker file, run the app for a while, then list files newer than the marker.
  Keep it bounded: about three levels deep in `%LOCALAPPDATA%`, `%APPDATA%`, `Documents` and `%TEMP%`, with a time limit.
  Also query the registry keys `HKCU\Software\TF3-DRPC` and `HKCU\Software\Microsoft\Windows\CurrentVersion\Run`.
  On macOS: three levels deep in `~/Library` (`Application Support`, `Preferences`, `Caches`, `Containers`, `Saved Application State`, `HTTPStorages`, `WebKit`, `Logs`), `~/Documents` and `$TMPDIR`, filtered for `tf3drpc` and `zagrthos`.
  This bounded check is the one used; a full trace is not part of the check ([decision 0007](../70-decisions/0007-portable-app-writing-only-to-data.md)).
- **Locked daily log:** create `DATA/logs/TF3-DRPC_<today>.log` and hold it open with `FileShare.None` (for example from a PowerShell `[System.IO.FileStream]`), start the app and open the settings window with a second start.
  A red message says the log file can't be written, and no message about settings appears.
  Release the file and the next start logs normally.
- **Accessible names:** with the settings window open, `winapp ui inspect -a <pid> --interactive` (WinApp CLI 0.7) lists the pickers, the template entries, the store check box and Exit with their names ("Design", "Sprache", "Statusanzeige", "Erste Zeile", …), not empty.
  On macOS read `AXDescription` of the window's controls through the Accessibility API; a control scrolled out of view takes `AXPress` only after `AXScrollToVisible`, as VoiceOver does.
- **Minimum window with long texts:** set all three custom templates to 150 characters and make the window as small as it goes (400 × 500); the preview lines end in "…" after two lines, and the pickers and Exit stay on screen.
- **Discord quitting and starting:** quit Discord while a game runs.
  The app logs one `DiscordConnectionLost` warning and shows that it isn't connected, with no warning repeated after it; start Discord again and the app connects within about 15 s and sends the presence again.
- **Screen reader:** start Narrator (Ctrl+Win+Enter), bring the settings window to the front and press Tab through it; Narrator speaks each control's name ("Design", "Sprache", "Statusanzeige", "Erste Zeile", …, "Beenden").
  Only a person can listen to it.
- **The image tooltip in Discord:** with a custom "Bild-Tooltip" template, open your own profile in Discord and hover the large image; it shows the template's text.
  Check this yourself; never screenshot the Discord window (see [known traps](known-traps.md)).
- **Smoke test of a publish:** start the exe, check `TrayCreated` in the app log after a few seconds, start it a second time (it must exit), stop the first with `Stop-Process`, and delete the publish folder's `DATA` again.
