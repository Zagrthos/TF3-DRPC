# The game log

The companion learns everything from the game's log: the mod's
[status lines](status-line.md) and the game's own lines for main menu, loading, missions
and shutdown.

## Finding the game and its log

| | Windows (measured) | macOS (measured 2026-10-20, macOS 27) |
|---|---|---|
| Game process | `TransportFever3` (`Process.GetProcessesByName`) | `TransportFever3` as well, an arm64 executable started by Steam as `./TransportFever3` (`Process.GetProcessesByName` finds it from the Mac Catalyst app, checked 2026-10-20) |
| Steam folder | `HKCU\Software\Valve\Steam` → `SteamPath` | `~/Library/Application Support/Steam` |
| User data | `<Steam>\userdata\<steamid>\3493540\local\` | `<Steam>/userdata/<steamid>/3493540/local/`, created at the first game start |
| Game log | `<userdata>\crash_dump\stdout.txt` | `<userdata>/crash_dump/stdout.txt`, the same lines as on Windows |
| Game folder (translation files) | from the process path, `…\steamapps\common\Transport Fever 3\` | `<Steam>/steamapps/common/Transport Fever 3/`, no app bundle; translations in `base/strings/<lang>/LC_MESSAGES/base.mo` |

- `3493540` is TF3's Steam app id. With several Steam accounts, use the one whose log was
  written last. A log dated more than a minute in the future (after a clock set back) counts
  only when every account's log is. Other stores aren't covered.
- **Game language:** `<userdata>/profile.lua` → `language.code` (for example `"de"`). The app
  reads the file as text without running it, and ignores Lua comments (`-- …`, `--[[ … ]]`).
- **No App Sandbox on macOS:** a sandboxed app can read neither the Steam folder nor
  Discord's socket. The MAUI template enables the sandbox in
  `Platforms/MacCatalyst/Entitlements.plist` (`com.apple.security.app-sandbox`); the app
  removes that entry.

## Reading it

Measured on Windows:

- **Encoding:** UTF-8 without BOM, CRLF; non-ASCII text occurs
  (`Industry 'Pune Holzfällerlager': connected`). The parser also accepts LF.
- **Sharing:** the game keeps the file open for writing. On Windows open it with
  `FileShare.ReadWrite | FileShare.Delete` (`FileShare.Read` alone fails); this doesn't
  disturb the game, also not while it starts.
- **New session:** at every start the game truncates the same file and writes
  `Application startup. Hello!` first. The app keeps the first 256 bytes it read; when the file
  is shorter than the read position or those bytes differ, it starts over from 0 and resets its
  state (this also catches a file that has already regrown past the old position).
- **Long lines:** a line is at most 1 MiB (a status line is about 1.3 KB). A longer one is
  dropped and reading resumes after the next line feed. A poll reads at most 4 MiB, so a
  large log (or many short lines) is read over several polls and memory stays bounded; an
  unfinished line is kept for the next poll.
- **Old session:** until the game truncates the file, it still holds the previous session.
  The app doesn't read a log last written before the game process started, and ignores
  lines stamped more than 2 s before the process start (log times have whole seconds).
  Both checks end for a process once the file or a line passed them, so a clock set back
  later doesn't make the running session look old. A file dated more than a minute in the
  future can't be the running session's (an old account's log after a clock rollback), so it
  isn't accepted until the clock has caught up; the app keeps looking for the right paths
  meanwhile.
- **Following:** poll every ~1 s from the last position and keep an unfinished last line
  until its line break arrives. Lines appear right away; the main menu may write nothing
  for a long time. Started while the game runs, the app reads the whole file, a few MB at
  most, in a few polls.

## Line format

```
[2026-10-13 21:15:24Z - MESSAGE  - Main Thread      - Main           ]  Game is ready
 └ time (UTC)          └ level    └ thread, 16 chars └ category, 15 chars
```

- Level is `VERBOSE`, `MESSAGE`, `WARNING` or `ERROR`, padded to 8 characters; thread and
  category are cut or padded to fixed widths (`Simulation Threa`, `UpdateBaseFileS`).
- After `]` follow two spaces and the message. Multi-line entries have `]+ ` instead and
  continue on lines starting with `    | `, the last with `    + `. The app reads only the
  first line of an entry, also of a multi-line one (`Ungraceful exit` is such a first line),
  and skips the continuation lines.

## Lines that matter

All seen in the test runs:

| Message | Meaning |
|---|---|
| `Application startup. Hello!` | the game started (first line of the file) |
| `Starting up build version: <number>, build name "Build"` | the game's build number, shown in the settings window while the game runs. The next line, `Build: Build <number> Windows 64-bit`, repeats it and is ignored. seen on Windows 2026-10-21; macOS writes the same line (build 40425 shown on 2026-10-24) |
| `Main menu is ready` | in the main menu, after the start or after leaving a game |
| `Loading game from file <name>` | a savegame is loading; `<name>` is `savegame` for a mission |
| `Campaign/mission: urbangames_campaign::/info.campaign/urbangames_campaign_mission_01::/info.mission` | a campaign mission is loading; its mod id is before `::/info.mission` |
| `Game is ready` | the game is running |
| `[drpc-status] {...}` | the mod's [status](status-line.md) |
| `Game is stopped` | the player left the game |
| `Normal shutdown. Bye!` | the game is closing |
| `Quit requested` | a close of the window was asked for; the game can stay open after it (seen 2026-10-17), so the app ignores it. A real quit logs `Normal shutdown. Bye!` a second later; quitting through the game menu logs `Game is stopped` and the shutdown without it |
| `Ungraceful exit` (`ERROR`) | the game crashed |

A new game (and the map editor) logs no `Loading game from file`; the app treats `Game is ready`
without it as a new game. Seen on 2026-10-20: while the new-game settings are open the game
repeats `Autoactivating mods:`; the map is generated after `Seed text: <seed>` and `Active
mods:`, which can take a minute. The app shows the main menu until `Game is ready`.

The log isn't a documented interface and may change with game updates; the app skips
unknown lines. If the process has run for 2 minutes without one usable line (the format
changed, or the log wasn't found), the app shows "Playing" until a usable line arrives. States:
[companion.md](companion.md#states).

## Samples

Real lines from the Windows test runs for the parser tests, kept byte for byte
(`.gitattributes` protects their line endings); timestamps come from several runs:

| File | Content |
|---|---|
| [log-samples/session.log](log-samples/session.log) | start, main menu, loading a save with the mod, three status lines, a multi-line entry, a non-ASCII line, leaving the game, shutdown |
| [log-samples/startup.log](log-samples/startup.log) | the first, sixth and seventh line of a real start: `Application startup. Hello!` and the two build lines. The other start lines hold a session ID, hardware details and the user data path, so they are left out |
| [log-samples/mission.log](log-samples/mission.log) | starting campaign mission 1 |
| [log-samples/crash.log](log-samples/crash.log) | a crash ending in `Ungraceful exit` |
