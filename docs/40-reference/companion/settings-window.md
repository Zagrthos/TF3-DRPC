---
title: Settings window
type: reference
audience: [contributor, agent]
area: [app, windows, macos, accessibility]
covers:
  - companion/Tf3Drpc.App/Views/**
  - companion/Tf3Drpc.App/App.xaml
  - companion/Tf3Drpc.App/Constants/AppConstants.cs
  - companion/Tf3Drpc.App/ViewModels/**
  - companion/Tf3Drpc.App/Platforms/MacCatalyst/MacWindowService.cs
  - companion/Tf3Drpc.App/Platforms/MacCatalyst/MacPickerHandler.cs
  - companion/Tf3Drpc.App/Platforms/MacCatalyst/MacCheckBoxHandler.cs
  - companion/Tf3Drpc.App/Platforms/Windows/WindowsWindowService.cs
  - companion/Tf3Drpc.App/Platforms/Windows/WindowsPickerHandler.cs
last-reviewed: 2026-11-11
verified:
  - windows 2026-10-22
  - macos 2026-10-24 build 40425
---

# Settings window

The window is only for the settings.
The tray / menu bar icon has "Language", "Settings…" and "Exit"; closing the window keeps the app running.
On macOS the app menu's "Quit TF3-DRPC" (Cmd+Q), which Mac Catalyst shows while the window is focused, is replaced by a command with the same title and shortcut that leaves through `IAppLifecycleService`, like Exit; the system's own Quit would end the process without the exit steps.
The platform design is in [companion app platform](platform.md); what differs between Windows and macOS is decided in [Windows and macOS parity](../platform-parity.md).

## Placement and size

- The window opens centred on the primary display, never on the nearest one or the one with the active app.
  On macOS the primary display is the one with the menu bar (`NSScreen.screens[0]`); `MacWindowService` centres the window in its visible frame itself, because AppKit's `center` uses the display of the active app.
- It opens at 520 × 780 and can't be made smaller than 400 × 500 (device-independent pixels).
- If the tray icon can't be created, the window is shown at start so the app stays reachable.
  The same happens when the data folder or the log folder can't be written (also in a translocated copy on macOS), so the warning is seen.
- The sizes are `AppConstants.SettingsWindow…`, used by the XAML and, on macOS, by `MacWindowService`, because Mac Catalyst ignores the XAML start size.

## macOS window behaviour

- `MacWindowService` takes over the AppKit window behind the MAUI window when it first appears (`NSWindowDidUpdateNotification`; at `SceneOnActivated` it doesn't exist yet), hides it, gives it the start size, centres it and points its close button at an action that only hides it.
- "Settings…" activates the app and brings the window to the front; macOS allows that activation only after a user action, such as the click in the menu bar.
- The game runs in a full-screen Space of its own, so the window moves to the active Space and may join a full-screen one (`collectionBehavior` `MoveToActiveSpace` and `FullScreenAuxiliary`); without that it opened on the desktop Space, unseen while the game was in front.
  Over the game it stays in front until it is closed, as an auxiliary window over a full-screen app does, even when the game is the active app again.

## XAML

The window, its size and its title bar are defined in `Views/SettingsWindow.xaml`, the content in `Views/SettingsPage.xaml`.
C# is left only where XAML has no access (DI, the backdrop, the picker handler, centring and hiding).
On Windows the title bar is MAUI's `TitleBar`, 48 high, with the tray icon, the app name and the version as subtitle.
macOS keeps its native title bar, "TF3-DRPC – <version>" (`MacWindowService` sets the subtitle).

## Layout

From the top:

- Up to two red warnings: the settings aren't saved, because the data folder isn't writable or the last save failed; the log file can't be written.
- The game status.
- The game build ("Transport Fever 3 · Build 40420", only while the log named it; not translated).
- The Discord connection and the preview (with the store button as a button-like frame when it is on).
- The settings: theme, language, preset, the custom templates with the placeholder list, the store button check box with its hint.
- At the bottom the plain buttons "GitHub" and "mod.io" on the left (untranslated because they are names; the tooltip shows the URL; they open the default browser through one `OpenUrl` command) and Exit on the right.

Further facts:

- The mod.io URL is `AppConstants.ModIoUrl`.
- A link the system won't open is logged as a warning, one that throws as an error; neither reaches the window.
- The `OpenUrl` command allows concurrent runs, because a command that disables its button while it runs makes WinUI move the keyboard focus to Exit.
- MAUI has no hyperlink control on every platform, and WinUI's `HyperlinkButton` alone would make the two platforms differ.
- Only the settings scroll, so the status stays visible at any size.

## Long texts

The header has its own height limit (240 device-independent pixels) and scrolls beyond it, so two warnings and long previews can't push the settings and Exit out of the minimum window.
The game status and the preview's first line, second line and tooltip show at most two lines each and end in an ellipsis.

## Theme

- The first picker, above the language, sets `Application.UserAppTheme`: "Same as the system" is `Unspecified`, so the window follows the system and its live switches; "Light mode" and "Dark mode" are `Light` and `Dark` whatever the system is set to.
- `CompanionApplication` applies it at start, before the window is shown, and on every `SettingsChangedMessage`, in shared code on both platforms.
- The page's colours, the native controls, the title bar and on Windows the Mica backdrop follow it.
- On macOS the native title bar is AppKit's and doesn't follow `UserAppTheme`, so `MacWindowService` gives the window the Aqua or DarkAqua appearance, or none for "Same as the system", when it takes the window over and on each theme change (`RequestedThemeChanged`).
  Without it "Light mode" on a dark system drew a white title on a light bar.
- The tray and menu bar menus belong to the system and keep following it.

## Pickers

- The mouse wheel over a closed picker scrolls the settings instead of changing the theme, language or preset (a focused WinUI combo box would otherwise step through its items).
- Hovering an entry of the preset picker shows a tooltip with what that preset would put on Discord with the values the app already has, like the preview (Alternating: its three layouts).
- Both come from `Platforms/Windows/WindowsPickerHandler.cs`: its combo box attaches a tooltip to each entry as WinUI creates it, and asks the item for its text (`IDescribedOption`) when the tooltip opens; only `PresetOption` implements it.
- On macOS `MacPickerHandler` builds the pop-up button's menu again each time it opens (`UIControlEvent.MenuActionTriggered`) and gives each entry its item's description as `DiscoverabilityTitle`, which Mac Catalyst shows as the entry's tooltip, line breaks included.
  The tooltip follows the in-game date while the game runs.

## Accessibility

- The pickers, the three template entries, the store check box and the Exit button have an accessible name (`SemanticProperties.Description`, bound to their translated labels).
- The "GitHub" and "mod.io" buttons have their text as name and their URL as hint.
- The secondary text (the Discord line, hints, placeholder descriptions) follows the theme: `#5C5C5C` in the light and `#A6A6A6` in the dark theme, at least 6:1 against the Mica backgrounds.

## Backdrop

Mica on Windows 11 and Desktop Acrylic on Windows 10, in the light or dark variant of the chosen theme; the page itself is transparent so the backdrop shows through.

## macOS look

- The app runs in the Mac idiom (`UIDeviceFamily` 6), so UIKit draws macOS controls instead of scaled iPad ones.
- Two Mac-only handlers replace MAUI's iPad controls: `MacPickerHandler` (a `UIButton` whose menu changes the selection, which is AppKit's pop-up button) and `MacCheckBoxHandler` (a `UISwitch` in the check box style).
- The page groups its two areas in rounded boxes on a grey background, like System Settings, and the template fields get a rounded border, since MAUI's text field draws none in the Mac idiom; the separator lines are hidden.
- The styles are in `App.xaml` (`MacPage`, `MacGroup`, `MacField`), chosen per platform through typed `OnPlatform<T>` resources (`PageStyle`, `GroupStyle`, `FieldStyle`); on Windows the plain variants keep the layout unchanged.
