# Windows and macOS parity

What the companion app does differently on Windows and macOS, and what is decided for each
difference. Compiled on 2026-10-20 from the code of both platforms (`Platforms/Windows/`,
`Platforms/MacCatalyst/`, the `OnPlatform` resources and the platform checks in Core). The
platform design itself is in [companion.md](companion.md#platform).

Mac code can't be built on Windows, so every item marked **to do** is done and checked on a
Mac. A pull request that changes Mac code from Windows says that it wasn't built there. Every
item that was open is now done or checked on a Mac (2026-10-20, macOS 27, the Native AOT
publish); the game build line added on 2026-10-22 was checked on 2026-10-24.

## Summary

| Feature | Windows | macOS | Decision |
|---|---|---|---|
| Game build in the settings window | Read from `Starting up build version: <number>` in the game log; checked 2026-10-22 | The same line in `stdout.txt`; the window showed "Transport Fever 3 · Build 40425" on 2026-10-24 | **Checked** |
| Preset tooltips | Hovering an entry of "Status appearance" shows what the preset would put on Discord | The same, as the entry's tooltip in the pop-up menu | **Done** ([below](#preset-tooltips)) |
| Minimum window size 400 × 500 | Enforced by the XAML | Holds as well | **Checked**, nothing to fix ([below](#minimum-window-size)) |
| Cmd+W | No equivalent | Only hides the window, like the close button | **Checked**, nothing to fix ([below](#cmdw)) |
| Theme setting | Checked: light, dark and "Same as the system" switch the window at once, also at start | The same; the native title bar needed the window's appearance | **Fixed and checked** ([below](#theme-setting)) |
| Game process and its path | `Process.GetProcessesByName`, `MainModule` | Same code, works in the Mac Catalyst app | **Checked** with the end-to-end test |
| Discord connection | Named pipe | Unix socket in `$TMPDIR` | **Checked** with the end-to-end test |
| Click on the tray icon | Double-click opens the settings, right-click the menu | A click opens the menu, with Settings… in it | Keep: each follows its platform |
| Mouse wheel over a closed picker | Scrolls the page instead of changing the setting | The pop-up button ignores the wheel | Keep: nothing to fix on macOS |
| Window look | Mica or Acrylic, MAUI title bar with icon and version | Native title bar with the version as subtitle, grouped boxes | Keep: each looks native |
| Cmd+Q, App Translocation, Gatekeeper | Not applicable | macOS only | Keep |

## The same on both

Checked in the code; nothing to do:

- **Tray / menu bar:** the same menu (Language with "Same as the game" and the 13 languages,
  checkmarks, Settings…, Exit), texts that follow the language at once, the tooltip
  "TF3-DRPC", and the settings window shown at start when the icon can't be created.
- **Settings window:** start size 520 × 780, centred on the primary display (on macOS the one
  with the menu bar, also when another app on a second display is active), closing only hides it, the header's
  240 height limit, the accessible names, light and dark theme, the warnings (data folder,
  log file, and on macOS the translocated copy), Exit.
- **Single instance:** a second start exits and the running app shows its settings window
  (on the desktop macOS may leave it behind the active app, over the full-screen game it comes
  to the front; see [companion.md](companion.md#platform)).
- **Exit:** the tray, the window's Exit and Cmd+Q all go through `IAppLifecycleService`, so a
  pending template edit is saved.
- **Paths and data:** `DATA` next to the app, the Steam account choice, `profile.lua`, the
  `.mo` translations, the game folder (from the process, otherwise the default Steam library),
  the file log with 7 days of retention.
- **Core:** the only platform check is the Discord transport in `DiscordConnector`.

## Preset tooltips

**Done.** On Windows, `WindowsPickerHandler` attaches a tooltip to each entry of the preset
picker and asks the item for its text (`IDescribedOption.Description`) when the tooltip opens,
so it shows the current values. `MacPickerHandler` used to build the native pop-up button's
menu from the display texts only.

- Each menu action's `DiscoverabilityTitle` is the item's description when the item is an
  `IDescribedOption`; Mac Catalyst shows it as the entry's tooltip, also in a pop-up button's
  menu.
- The menu is built again each time it opens (`UIControlEvent.MenuActionTriggered`), not only
  when the picker's items change, so the values are as current as on Windows and nothing new is
  read.
- The selection behaviour of the pop-up button (`ChangesSelectionAsPrimaryAction`) and the check
  mark on the selected entry stay.

**Checked on the Mac** (2026-10-20, with the game running): the tooltip appears beside the
entry; in French it showed "janvier 1900" and, after the in-game month changed, "février 1900" without
any other change to the picker; "Alternating" shows its three layouts separated by a blank
line; choosing an entry still changes the preset and moves the check mark; the theme and
language pickers, which have no descriptions, work as before.

## Minimum window size

**Checked, nothing to fix.** The minimum of 400 × 500 is set only as `MinimumWidth` and
`MinimumHeight` in `Views/SettingsWindow.xaml`, and Mac Catalyst passes it on: asked for
300 × 300, the window became 400 × 500 (2026-10-20). The minimum-window check of
[testing.md](testing.md#other-checks) passes on macOS too, with the theme picker: the preview
lines end in "…" after two lines and the pickers and Exit stay visible.

## Cmd+W

**Checked, nothing to fix.** Cmd+W in the settings window goes the way of the close button,
which `MacWindowService` points at an action that only hides the window: the app keeps running,
and "Settings…" and a second start show the same window again (its AppKit window number stays
the same), with the close button still only hiding it (2026-10-20).

## Theme setting

**Fixed and checked.** The theme reaches the page and the native controls through
`Application.UserAppTheme`, but the AppKit window kept the system's appearance for its title
bar: "Light mode" on a dark system drew a white title on a light title bar. `MacWindowService`
now gives the window the Aqua or DarkAqua appearance, or none for "Same as the system", when it
takes the window over and on each `RequestedThemeChanged`.

**Checked on the Mac** (2026-10-20): "Light mode" and "Dark mode" switch the page, the pop-up
buttons and the title bar at once, also against the opposite system setting; a saved "Light
mode" applies at start; with "Same as the system" the window follows when macOS switches
between light and dark.

## Checked by the end-to-end test

The macOS end-to-end test (2026-10-20) confirmed:

- `Process.GetProcessesByName("TransportFever3")` finds the game from the Mac Catalyst app, and
  `MainModule` gives its path: the app log has the start with the executable
  (`…/steamapps/common/Transport Fever 3/TransportFever3`) and the end of the game process.
- The presence reaches Discord over the Unix socket, and Discord accepts it.

## Kept different

- **Click on the tray icon:** a Windows tray icon opens its window on a double-click; a macOS
  menu bar icon opens its menu on a click, and the settings are one entry away. Adding a
  double-click on macOS would go against the platform.
- **Mouse wheel over a picker:** WinUI's combo box changes the selection with the wheel, so
  `WindowsPickerHandler` stops that; macOS's pop-up button doesn't, so there is nothing to
  match.
- **Window look:** Mica or Acrylic and the MAUI title bar on Windows; the native title bar,
  grouped boxes and native controls on macOS (`MacPage`, `MacGroup`, `MacField`,
  `MacPickerHandler`, `MacCheckBoxHandler`). Both are meant to look native.
- **macOS only:** Cmd+Q through the exit steps, the App Translocation warning, the menu bar
  app without a Dock icon (`LSUIElement`), no App Sandbox.
- **Windows only:** hiding the window from Alt+Tab while it's hidden; a macOS menu bar app
  isn't in the app switcher anyway.
