# The companion app

A portable .NET MAUI app for Windows and macOS that follows the [game log](game-log.md)
and shows the [presence](presence.md) in Discord. Name: `TF3-DRPC`. Decisions and open work
are in [README.md](README.md#decisions).

## Platform

.NET 11 with the MAUI workload: RC2 now (go-live licence), GA in November 2026.

| | Windows | macOS |
|---|---|---|
| Target | WinUI 3, `net11.0-windows10.0.19041.0` | Mac Catalyst, `net11.0-maccatalyst` |
| Native AOT | `PublishAot` with `UseOptimizedPublish=false` (RC2's optimised publish skips reference resolution and fails with `WMC1007`); works, one known MAUI trim warning and 20 informational ILC messages about the left-out WebView2 (see [AGENTS.md](../AGENTS.md#native-aot-rules)) | `PublishAot`, `RuntimeIdentifier` `maccatalyst-arm64`, the optimised publish (a full build first would leave the CoreCLR runtime and a 70 MB R2R image in the `.app`); works, one informational ILC message about `UIKit.NSLayoutAnchor`1` |
| Tray / menu bar | no MAUI API; WinUIEx's `TrayIcon` with a WinUI `MenuFlyout` (no AOT or trim warnings) | no MAUI or UIKit API; AppKit's `NSStatusItem` through the Objective-C runtime, as `Drastic.Tray` does |
| Minimum OS | Windows 10 2004 (`SupportedOSPlatformVersion` 10.0.19041.0) | macOS 26 (`SupportedOSPlatformVersion` 26.0) |
| Shipped as | portable zip: unpackaged, self-contained (`WindowsPackageType=None`, `WindowsAppSDKSelfContained=true`); not code-signed yet, so SmartScreen warns at the first start | portable zip with the `.app`, not signed or notarised yet, so Gatekeeper stops the first start; never through the Mac App Store |
| Built on | Windows | a Mac with Xcode 27.0; Xcode 27.1 works with `ValidateXcodeVersion=false` until GA |

No release has been published yet. `tools/build-release.ps1` builds the zip of the platform it
runs on (`TF3-DRPC-<version>-windows-x64.zip`, `TF3-DRPC-<version>-macos-arm64.zip`) into
`artifacts/`. CI runs the same script: a push to `main` keeps both zips as workflow
artifacts, and a `v<VersionPrefix>` tag on `main` publishes them as a GitHub release
(`.github/workflows/release.yml`).
Each zip holds `LICENSE.txt` (the MIT licence of this project) and `THIRD-PARTY-NOTICES.txt` next to
the exe or the `.app`. `tools/new-third-party-notices.ps1` generates the notices at build time from the
publish's restore output (`project.assets.json`): the licence texts of every package and runtime pack
of the target, so they follow the package versions.
The first release (step 3.3, see [README.md](README.md#next-steps)) is unsigned on both
platforms, and signing comes later.

- **Native AOT rules:** see [AGENTS.md](../AGENTS.md#native-aot-rules).
- **Window:** only for the settings. The tray / menu bar icon has "Language",
  "Settings…" and "Exit"; closing the window keeps the app running. On macOS the app menu's
  "Quit TF3-DRPC" (Cmd+Q), which Mac Catalyst shows while the window is focused, is replaced
  by a command with the same title and shortcut that leaves through `IAppLifecycleService`,
  like Exit; the system's own Quit would end the process without the exit steps.
  - **Placement and size:** the window opens centred on the primary display, never on the
    nearest one or the one with the active app. On macOS the primary display is the one with
    the menu bar (`NSScreen.screens[0]`); `MacWindowService` centres the window in its visible
    frame itself, because AppKit's `center` uses the display of the active app. It opens at 520 × 780 and can't be made smaller than 400 × 500
    (device-independent pixels). If the tray icon can't be created, the window is shown at
    start so the app stays reachable; the same happens when the data folder or the log folder
    can't be written (also in a translocated copy on macOS), so the warning is seen. The sizes are `AppConstants.SettingsWindow…`, used by the
    XAML and, on macOS, by `MacWindowService`, because Mac Catalyst ignores the XAML start size.
  - **macOS:** `MacWindowService` takes over the AppKit window behind the MAUI window when it
    first appears (`NSWindowDidUpdateNotification`; at `SceneOnActivated` it doesn't exist
    yet), hides it, gives it the start size, centres it and points its close button at an
    action that only hides it. "Settings…" activates the app and brings the window to the
    front; macOS allows that activation only after a user action, such as the click in the
    menu bar. The game runs in a full-screen Space of its own, so the window moves to the
    active Space and may join a full-screen one (`collectionBehavior` `MoveToActiveSpace` and
    `FullScreenAuxiliary`); without that it opened on the desktop Space, unseen while the game
    was in front. Over the game it stays in front until it is closed, as an auxiliary window
    over a full-screen app does, even when the game is the active app again.
  - **XAML:** the window, its size and its title bar are defined in `Views/SettingsWindow.xaml`,
    the content in `Views/SettingsPage.xaml`; C# is left only where XAML has no access (DI,
    the backdrop, the picker handler, centring and hiding). On Windows the title bar is MAUI's
    `TitleBar`, 48 high, with the tray icon, the app name and the version as subtitle; macOS
    keeps its native title bar, "TF3-DRPC – <version>" (`MacWindowService` sets the subtitle).
  - **Layout, from the top:** up to two red warnings (settings aren't saved, because the data
    folder isn't writable or the last save failed; the log file can't be written), the game
    status, the game build ("Transport Fever 3 · Build 40420", only while the log named it;
    not translated), the Discord connection and the preview (with the store button as a button-like
    frame when it is on); then the settings (theme, language, preset, the custom
    templates with the placeholder list, the store button check box with its hint); at the
    bottom the plain buttons "GitHub" and "mod.io" on the left (untranslated because they are
    names; the tooltip shows the URL; they open the default browser through one `OpenUrl`
    command) and Exit on the right. The mod.io URL (`AppConstants.ModIoUrl`) is the expected
    one; its last part is set at the first upload, so check it then. A link the system won't
    open is logged as a warning, one that throws as an error; neither reaches the window. The
    command allows concurrent runs, because a command that disables its button while it runs
    makes WinUI move the keyboard focus to Exit.
    MAUI has no hyperlink control on every platform, and WinUI's `HyperlinkButton` alone would
    make the two platforms differ. Only the settings scroll, so the status stays visible at any size.
  - **Long texts:** the header has its own height limit (240 device-independent pixels) and
    scrolls beyond it, so two warnings and long previews can't push the settings and Exit out
    of the minimum window. The game status and the preview's first line, second line and
    tooltip show at most two lines each and end in an ellipsis.
  - **Theme:** the first picker, above the language, sets `Application.UserAppTheme`: "Same as
    the system" is `Unspecified`, so the window follows the system and its live switches;
    "Light mode" and "Dark mode" are `Light` and `Dark` whatever the system is set to.
    `CompanionApplication` applies it at start, before the window is shown, and on every
    `SettingsChangedMessage`, in shared code on both platforms. The page's colours, the native
    controls, the title bar and on Windows the Mica backdrop follow it (checked on Windows
    2026-10-20, with Windows set to dark). On macOS the native title bar is AppKit's and
    doesn't follow `UserAppTheme`, so `MacWindowService` gives the window the Aqua or DarkAqua
    appearance, or none for "Same as the system", at start and on each theme change; without it
    "Light mode" on a dark system drew a white title on a light bar (checked 2026-10-20 with
    macOS set to dark and to light). The tray and menu bar menus belong to the system and keep
    following it.
  - **Pickers:** the mouse wheel over a closed picker scrolls the settings instead of changing
    the theme, language or preset (a focused WinUI combo box would otherwise step through its
    items).
    Hovering an entry of the preset picker shows a tooltip with what that preset would put on
    Discord with the values the app already has, like the preview (Alternating: its three
    layouts). Both come from `Platforms/Windows/WindowsPickerHandler.cs`: its combo box
    attaches a tooltip to each entry as WinUI creates it, and asks the item for its text
    (`IDescribedOption`) when the tooltip opens; only `PresetOption` implements it. On macOS
    `MacPickerHandler` builds the pop-up button's menu again each time it opens
    (`UIControlEvent.MenuActionTriggered`) and gives each entry its item's description as
    `DiscoverabilityTitle`, which Mac Catalyst shows as the entry's tooltip, line breaks
    included (checked 2026-10-20: the tooltip follows the in-game date while the game runs).
  - **Accessibility:** the pickers, the three template entries, the store check box and the
    Exit button have an accessible name (`SemanticProperties.Description`, bound to their
    translated labels); the "GitHub" and "mod.io" buttons have their text as name and their URL as hint. The secondary text (the Discord line, hints, placeholder
    descriptions) follows the theme: `#5C5C5C` in the light and `#A6A6A6` in the dark theme,
    at least 6:1 against the Mica backgrounds.
  - **Backdrop:** Mica on Windows 11 and Desktop Acrylic on Windows 10, in the light or dark
    variant of the chosen theme; the page itself is transparent so the backdrop shows through.
  - **macOS look:** the app runs in the Mac idiom (`UIDeviceFamily` 6), so UIKit draws macOS
    controls instead of scaled iPad ones. Two Mac-only handlers replace MAUI's iPad controls:
    `MacPickerHandler` (a `UIButton` whose menu changes the selection, which is AppKit's pop-up
    button) and `MacCheckBoxHandler` (a `UISwitch` in the check box style). The page groups its
    two areas in rounded boxes on a grey background, like System Settings, and the template
    fields get a rounded border, since MAUI's text field draws none in the Mac idiom; the
    separator lines are hidden. The styles are in `App.xaml` (`MacPage`, `MacGroup`,
    `MacField`), chosen per platform through typed `OnPlatform<T>` resources (`PageStyle`,
    `GroupStyle`, `FieldStyle`); on Windows the plain variants keep the layout unchanged.
- **One instance:** Windows through MAUI's `OnAppInstanceActivated` hook with
  `AppInstance.FindOrRegisterForKey` (a second start hands over to the running app, which
  shows the settings window). macOS per user session as well, without a file: a new process
  that finds an older one (a lower process id, not terminated) with the same bundle
  identifier (`NSRunningApplication`) posts a distributed notification and exits, and the
  running app shows its settings window. Only the younger process yields, so of two starts at
  the same moment exactly one keeps running (review R02, 2026-10-20); a start
  that LaunchServices hands to the running app as a reopen event does the same. A lock file in
  `DATA/` would only cover one folder, so two copies in different folders could run at once.
  The reopen handler is registered when the window scene first activates; one set earlier is
  replaced while the app finishes launching. Limit: when a second copy hands over, the
  running one shows its window but may stay behind the active app, because macOS grants the
  activation to the process the user started, which exits (seen 2026-10-20 with another app in
  front). Over the full-screen game the window does come to the front, since it joins the
  game's Space.
- **Icons:** app and tray icons are made from `assets/discord/cover.png`; the tray icon is a
  committed multi-size `Tf3Drpc.App/Resources/Tray/tray.ico` (16–64 px), the menu bar icon on
  macOS a committed `Tray/tray.png` (36 px for 18 points), bundled as `tray.png`. The window's
  title bar takes the one of its platform (`OnPlatform`).
- **Menu bar (macOS):** `MacTrayService` creates the `NSStatusItem`, its menu and a small
  Objective-C target object for the menu actions through `ObjCRuntimeUtils` (`objc_msgSend`
  through `[LibraryImport]`, one declaration per signature); the menu matches the Windows tray.
- **macOS project settings:**
  - `LSUIElement=true` (no Dock icon) as an `AppManifestEntry` item in the project file.
  - No App Sandbox: the template's `Platforms/MacCatalyst/Entitlements.plist` enables it,
    so the entry is removed (see [game-log.md](game-log.md#finding-the-game-and-its-log)).
  - Planned: `RuntimeIdentifiers` `maccatalyst-arm64;maccatalyst-x64` in the app project
    for a universal build (macOS 26 still runs on some Intel Macs).
  - The UIKit scene lifecycle, required with the Xcode 27 SDKs and included in the RC2
    templates.

**.NET 11 MAUI features we use:**

| Feature | Use |
|---|---|
| Native AOT as opt-in for publishing (CoreCLR otherwise) | `PublishAot` for releases |
| XAML source generation (`MauiXamlInflator=SourceGen`) | AOT-safe XAML with lazy resource dictionaries; every binding is compiled (`XC0022`–`XC0025` are errors) |
| `OnAppInstanceActivated` / `AppInstance` | single instance on Windows |
| `AppManifestEntry` items | `LSUIElement` |

## Portable

No installer. The app runs from the extracted zip and writes only into a `DATA` folder next
to its own files:

```
TF3-DRPC/                    ← extracted zip, anywhere the user likes
├── TF3-DRPC.exe + DLLs      (Windows)  /  TF3-DRPC.app  (macOS)
├── LICENSE.txt, THIRD-PARTY-NOTICES.txt
└── DATA/                    ← created on first start
    ├── settings.json
    └── logs/TF3-DRPC_<yyyy-MM-dd>.log
```

- **Where `DATA` is:** next to the executable on Windows (`AppContext.BaseDirectory`), next
  to the `.app` bundle on macOS. Never inside the bundle: changing files there breaks the
  code signature.
- **Nothing outside:** no registry, no `%APPDATA%`/`%LOCALAPPDATA%`, no
  `~/Library/Preferences` or `~/Library/Application Support`. So no MAUI `Preferences`,
  `SecureStorage` or `FileSystem.AppDataDirectory`, and no start with the system (registry
  `Run` key or login item).
- **Not writable** (for example extracted into `Program Files`): the app shows a message
  and runs with defaults, without saving. The same message appears when saving a setting
  failed; changing any setting again retries the save. A save probes the data folder again
  when it wasn't writable, so a folder repaired while the app runs is noticed and the message
  goes away; a save that wasn't written counts as failed. A template edit still waiting for its
  save is kept on both Exit routes: the lifecycle service raises `Exiting` on the UI thread
  before it quits, and the settings view model saves the edit then.
- **macOS App Translocation:** macOS starts a quarantined app downloaded in a zip from a
  hidden read-only copy, where `DATA` can't be written. The app detects this
  (`SecTranslocateIsTranslocatedURL` on the bundle URL), opens the settings window at start
  and shows "Move TF3-DRPC.app once with Finder…" in the place of the not-writable warning;
  moving the app ends the translocation. Checked 2026-10-20 with an ad-hoc-signed copy carrying
  a user-approved quarantine flag (the process ran from `…/AppTranslocation/…`). An unsigned
  or ad-hoc-signed download is first stopped by Gatekeeper ("Open Anyway" in Privacy &
  Security); a signed and notarised release avoids that.
- **Operating-system data** the app doesn't write itself (for example macOS crash reports
  or the Windows tray icon cache) is outside its control. On macOS that includes two small
  files the system frameworks write before the app can prevent it (seen 2026-10-20):
  `~/Library/Preferences/com.zagrthos.tf3drpc.plist` with the window's first frame (AppKit's
  frame autosave; it can also hold the frames of AppKit's spelling panels) and `~/Library/Saved Application State/com.zagrthos.tf3drpc~iosmac.savedState`
  with UIKit's list of scene sessions. The app turns off window state restoration and further
  frame saves; stopping the first writes would need private APIs.
- **Check:** a bounded check on Windows found nothing outside `DATA/` (see
  [testing.md](testing.md#other-checks)); on macOS the only files seen outside `DATA/` are the
  two above, and the bounded check over a 45-minute game run on 2026-10-20 found nothing
  written outside `DATA/` (both files were older than its marker). A full trace
  with Process Monitor or `fs_usage` isn't planned (see [README.md](README.md#decisions)).
- **Logs:** NReco.Logging.File, one file per day,
  `DATA/logs/TF3-DRPC_<yyyy-MM-dd>.log`, files older than 7 days deleted at start (a locked
  file is skipped). The log folder is probed on its own: if it can't be written, or the file logger
  fails to start, the app runs without a log file. A log file that can't be written (for
  example because another program holds today's file open) is reported once by NReco's
  `HandleFileError`; the settings window then shows its own warning, separate from the one
  about settings, through `IPathService.IsLogFileAvailable` and a message. NReco can't retry
  with another name and still report a second failure (it doesn't call the handler
  recursively), so there is no retry. A log folder that was fixed later isn't picked up: the
  file logger is created only at start.

## Projects

| Project | Content |
|---|---|
| `Tf3Drpc.Core` (`net11.0`, `IsAotCompatible`) | log reader and parser, states, the monitor tick (`GameMonitor`), presence texts, `.mo` reader, settings file, Discord IPC client |
| `Tf3Drpc.App` (MAUI; Windows and Mac Catalyst) | settings window, tray / menu bar icon, platform code (paths, `DATA` location, single instance) |
| `Tf3Drpc.Core.Tests` (`net11.0`) | every Core part: parser and follower on `docs/log-samples/`, state machine, the monitor tick (`GameMonitor`, with fakes for its services), presence texts, `.mo` reader, languages, settings, Discord frames and client (with an in-memory fake Discord) |

**Folders** (in both projects, as far as they apply):

| Folder | Content |
|---|---|
| `Constants/` | `<Area>Constants` classes with shared limits and tuning values |
| `Converters/` | XAML value converters (app): `IsNotNullConverter` shows a label only while its value is set (a `DataTrigger` on `{x:Null}` doesn't fire for a value that is null from the start) |
| `Enums/` | enums; their texts and keys come from `Extensions/EnumExtensions.cs` |
| `Extensions/` | `ServiceCollectionExtensions` (app), `EnumExtensions`, as C# 14 `extension` blocks |
| `Logging/` | `LogMessages.cs` with all `[LoggerMessage]` methods of the project |
| `Messages/` | messenger records (app) |
| `Serialization/` | the one `JsonSourceGenContext` of the project, and the lenient converters that let a value of the wrong type cost only that value (`LenientInt64Converter` for the status, `LenientStringConverter`, `LenientBooleanConverter` and `LenientTemplatesConverter` for the settings) |
| `Services/`, `Services/Interfaces/` | services and their interfaces (app); `PathServiceBase` holds what the two path services share |
| `Structs/` | `…Struct` result types (Core) |
| `Utilities/` | `…Utils` static helpers, such as `RegexUtils`; `ObjCRuntimeUtils` (Mac Catalyst only) |
| `Platforms/Windows/`, `Platforms/MacCatalyst/` | the platform implementations of the path, single instance, tray and window services and the picker and check box handlers (app); `PlatformServices.cs` names them for the shared code |
| `Views/`, `ViewModels/` | the settings window and page in XAML, the settings view model and the picker items (`LanguageOption`, `ThemeOption`, `PresetOption`, `PlaceholderOption`, `IDescribedOption`) (app) |
| `Discord/`, `GameLog/`, `Languages/`, `Monitor/`, `Presence/`, `Settings/`, `State/`, `Status/`, `Translation/` | the Core's parts, one folder each (Core) |
| `Resources/` | the own texts as `.resx` (Core); the tray and menu bar icons (app) |

`Tf3Drpc.Core` needs no UI, so it builds and tests anywhere without the MAUI workload.

**Build setup** (the rules are in [AGENTS.md](../AGENTS.md)):

- `global.json`: `sdk.version` `11.0.100-rc.2.26504.105` (`allowPrerelease: true`,
  `rollForward: latestPatch`), `sdk.workloadVersion` `11.0.100-rc.2.26509.2` (both move to
  GA in November), `test.runner` `Microsoft.Testing.Platform`.
- All in `companion/`, where the commands run:
  - `TF3-DRPC.slnx`.
  - `Directory.Build.props`: the build policy, Debug and Release settings, trim analysis, our
    13 satellite languages, file properties, and `EnableWindowsTargeting` so the Windows
    projects restore on macOS.
  - `Directory.Build.targets`: keeps the `.xml` doc files out of the output.
  - `Directory.Packages.props`: central package versions. The analysers Meziantou, Roslynator
    and Sonar and `ThisAssembly.AssemblyInfo` are `GlobalPackageReference`s.
  - `nuget.config`: nuget.org only. Feeds from a user- or machine-wide NuGet configuration
    would otherwise cause NU1507.
  - The app project: MAUI, Windows, Native AOT, size and security settings, and the build
    targets listed under **Publish size**.
- Every props and project file uses the same layout: topic groups with a comment header,
  alphabetical inside a group, a doc link after a non-obvious property.
- Build policy: `LangVersion` latest, `Nullable` enable, `ImplicitUsings` disable,
  `Features` Strict, `CheckForOverflowUnderflow`, `AnalysisLevel` latest-all,
  `EnforceCodeStyleInBuild`, `GenerateDocumentationFile`, nullable warnings as errors.
- Configurations Debug and Release only, no `Testing`. Release has no symbols
  (`DebugType none`); Debug embeds them. `UseArtifactsOutput` puts all output in `artifacts/`.
- Tests: xunit v3 on Microsoft.Testing.Platform.

**Publish size.** The Windows publish is about 83 MB, and `TF3-DRPC.exe` about 25 MB. It was
305 MB before these measures:

| Measure | Saving |
|---|---|
| No `.pdb` in Release | 158 MB |
| `ExcludeAssets="all"` for parts of the Windows App SDK that MAUI pulls in but the app doesn't use: `Microsoft.WindowsAppSDK.AI`, `.ML`, `.Widgets`, `Microsoft.Windows.AI.MachineLearning` (`onnxruntime.dll`, `DirectML.dll`) and `Microsoft.Web.WebView2` | ~40 MB |
| Target `RemoveUnusedMuiFromPublish`: WinUI `.mui` resources only for our 13 languages, not 119 | ~3.5 MB |
| Target `RemoveWindowsTileImages`: no MAUI tile and logo images; the unpackaged app uses the `.ico` | a few MB |
| AOT switches in the app project: `OptimizationPreference Size`, `TrimMode Full`, unused runtime features off, C#/WinRT size options | ~7 MB of the exe |

- The rest is mostly the self-contained Windows App SDK, which a portable app has to carry.
  Going much lower would mean pure WinUI 3 without MAUI.
- The five excluded packages are pinned to the versions MAUI resolves. Raise them only
  together with MAUI.
- Leaving out `Microsoft.Web.WebView2` makes the publish print 20 ILC messages "will always
  throw because: Failed to load assembly 'Microsoft.Web.WebView2.Core.Projection'", for the
  web view code in MAUI and WinUI. The app has no web view, so that code never runs; the
  messages are expected (see [AGENTS.md](../AGENTS.md#native-aot-rules)).
- The macOS `.app` is about 21 MB (10 MB zipped), nearly all of it the Native AOT executable:
  macOS brings AppKit and UIKit, so nothing like the Windows App SDK has to ship. It needs
  the optimised publish; with `UseOptimizedPublish=false` the full build first leaves the
  CoreCLR runtime, a 70 MB R2R image and the managed assemblies in `Contents/MonoBundle`
  (99 MB in all), which the executable never loads.

Two more app targets are needed for a working build:

- `AddReferenceSatellitesToIlc`: Native AOT can't load satellite assemblies at runtime, and
  ILC only compiles in those of NuGet packages. This target adds the Core's translations.
  Without it the app's own texts are English only, while the tests still pass.
- `MarkWindowsAppSdkSourcesAsGenerated`: marks the Windows App SDK's
  `UndockedRegFreeWinRT-AutoInitializer.cs` as generated code, so its CA5392 isn't ours.

## Architecture

Services do the work; the UI only presents.

- **Builder:** MAUI's `MauiAppBuilder` provides DI and logging. No Generic Host and no
  hosted services: MAUI doesn't run them, and the background work is one loop.
- **Services:** one interface per service in `Services/Interfaces/`, the implementation in
  `Services/`, registered in `ServiceCollectionExtensions` in dependency tiers, injected
  through primary constructors. Platform-specific implementations (tray, single instance,
  paths) sit behind the same interfaces.
- **Background loop:** `IMonitorService` runs one `PeriodicTimer` (1 s) from app start
  until exit (`CancellationToken`) and sends an `AppStatusChangedMessage` when a tick changed
  the status. The tick itself is Core's `GameMonitor` (tested in `Tf3Drpc.Core.Tests`): it
  checks the game process, reads new log lines and lets the Discord service send or reconnect.
  It reaches the services through narrow Core interfaces (`IGameProcessSource`,
  `IGamePathResolver`, `ISettingsSource`, `ILanguageResolver`, `IPresenceBuilder`,
  `IPresenceSink`) that the app's service interfaces inherit, and owns the log reader and the
  state machine. While the game runs without a known log path it asks for the paths again
  every 15 s (`MonitorConstants.PathRetryInterval`); the warning "The game's files were not
  found yet" comes once per lost log path, not on every retry, and a found log path re-arms it.
  "Game paths: …" is logged only when the paths change.
- **Messages:** `IMessenger` (`WeakReferenceMessenger`) with immutable records in
  `Messages/`. `AppStatusChangedMessage` carries the game state, the Discord connection,
  the language and the presence texts; `SettingsChangedMessage` carries the settings after a
  change; `LogFileUnavailableMessage` tells the settings window that the daily log file can't
  be written. Receivers use the status of the message, because the loop updates the shared one.
- **ViewModels:** CommunityToolkit.Mvvm (`[ObservableProperty]`, `[RelayCommand]`).

| Service | Does |
|---|---|
| `IMonitorService` | the background loop: the timer and the messenger around Core's `GameMonitor` |
| `IPathService` | `DATA` folder (probed again on a save when it wasn't writable), Steam folder, user data (the account whose log isn't dated in the future), game log, game folder; whether the daily log file is written |
| `ISettingsService` | `DATA/settings.json`: language, theme, preset, templates, store button |
| `IGameProcessService` | whether the game runs, its executable path |
| `ITranslationService` | game texts from the `.mo` files in the chosen language and in the game's language (for "New Game"), own texts, the chosen language |
| `IPresenceService` | builds the presence from state, status, settings and texts, and the preview from the last status (also for the preset tooltips) |
| `IDiscordService` | Discord connection, reconnect, rate limit; closes the connection when there is nothing to show |
| `ITrayService` | tray icon (Windows) / menu bar icon (macOS) |
| `ISingleInstanceService` | one running instance |
| `IWindowService` | the settings window: starts hidden, closing hides it, shown centred on the primary display |
| `IAppLifecycleService` | exit: stops the loop, clears the presence, raises `Exiting` (the tray icon goes, a pending template edit is saved), quits |

**Source generators**, used wherever they exist:

| Generator | For |
|---|---|
| `System.Text.Json` source generation (`JsonSerializerContext`) | settings, the status line, Discord frames; reflection is off (`JsonSerializerIsReflectionEnabledByDefault=false`) |
| `[LoggerMessage]` | all logging, in `Logging/LogMessages.cs` |
| `[GeneratedRegex]` (with a timeout, in `Utilities/RegexUtils.cs`) | log line header, the `Campaign/mission:` line, the language code in `profile.lua` (after `LuaUtils.StripComments` removed its Lua comments) |
| `[LibraryImport]` | native calls on macOS: the translocation check and the Objective-C runtime |
| CommunityToolkit.Mvvm | properties and commands of the ViewModels |
| MAUI XAML source generation | the settings window |
| `ThisAssembly.AssemblyInfo` | app name and version without reflection |
| `.resx` strongly typed resources | own texts in 13 languages |

## Discord connection

- **Application ID:** `1559680023458484244` (not secret).
- **Large image:** Art Asset `cover` (`assets/discord/cover.png`, a square crop of a
  harbour in our savegame from the game's photo mode, 1024 × 1024), hover text "Transport Fever 3". No small image.
- **Endpoint:** Windows named pipe `\\.\pipe\discord-ipc-0` … `-9`; macOS Unix socket
  `$TMPDIR/discord-ipc-0` … `-9`.
- **Protocol:** frames of `opcode:uint32 LE, length:uint32 LE, JSON`. Handshake (opcode 0,
  `{"v":1,"client_id":"…"}`), then `SET_ACTIVITY` (opcode 1); answer `PING` with `PONG`;
  handle `CLOSE`.
- **Client:** our own, in `Tf3Drpc.Core`: `NamedPipeClientStream` on Windows, a `Socket` with
  `UnixDomainSocketEndPoint` on macOS, frames serialised through the source-generated JSON
  context. Third-party libraries such as Lachee's `DiscordRichPresence` depend on
  Newtonsoft.Json and aren't Native AOT compatible.
- **Rate:** Discord allows 5 updates per 20 s; the app sends only on change, at most every
  ~15 s.
- **Text limits** (as Lachee's library enforces them): `details`, `state` and image texts
  up to 128 characters, button labels up to 31 UTF-8 bytes, URLs up to 512. The app
  shortens texts before sending.
- **Store button:** `SET_ACTIVITY` carries `"buttons":[{"label":"…","url":"https://store.steampowered.com/app/3493540"}]`
  only while the [setting](#settings) is on; without it the `buttons` key is absent. The label
  is cut to 31 UTF-8 bytes at a character boundary. Discord doesn't show the player their own
  button. A refused button would come back as an `ERROR` reply (see below); the live test checks that.
- **Reconnect:** if Discord isn't running or restarts, the app retries every ~15 s.
- **Write deadline:** a write to Discord that doesn't finish within 5 s drops the connection, so a
  Discord that stopped reading can't stall the monitor loop; the next update reconnects.
- **Rejected presence:** an `ERROR` reply carrying the nonce of the last `SET_ACTIVITY` means the
  presence isn't shown. The app sends the same content again after 15 s and doubles the wait per
  further rejection, up to 5 min; changed content is sent at the normal rate.
- When the game exits, the app clears the presence.

## States

Built from the process check and the [log lines](game-log.md#lines-that-matter):

| State | Entered on | Discord |
|---|---|---|
| Not running | no game process | cleared |
| Starting | process found, `Application startup. Hello!` | "Starting the game" |
| Main menu | `Main menu is ready` | "In the main menu" |
| Loading | `Loading game from file`, `Campaign/mission:` | the game's `Loading...` |
| Mission | `Game is ready` after `Campaign/mission:` | "Campaign · <mission name>" |
| Playing | `Game is ready` otherwise | design A with a fresh mod status, otherwise "Playing" |
| Playing (fallback) | the process runs for 2 min and no usable log line arrived | "Playing", timer since the process start |
| Closing | `Normal shutdown. Bye!`, `Ungraceful exit` (not `Quit requested`, after which the game can stay open) | cleared |

- In Playing the latest mod status is kept. The mod writes every 10 s, with `speedup` 0 while
  paused with the pause button, and nothing while the game menu is open, during loading or in
  missions. So the game counts as paused when `speedup` is 0 or no new status line came for
  more than 20 s. The next line with a speed ends the pause.
- Without the game process the state is "Not running", whatever the log says.
- A game process that replaces the old one between two checks (a different start time) counts as a
  new start: the state resets and the app finds the paths and reads the log from the start again.
- Lines from before the game process started (more than 2 s) belong to the previous session
  and are ignored; see [game-log.md](game-log.md#reading-it).
  Once a line of the current session was accepted, the check ends for that process, so a clock
  set back later doesn't make the running session look old. A log dated more than a minute in
  the future isn't accepted as the running session's (and loses the account choice to a
  current log), so the monitor keeps resolving the paths until the right one turns up.
- **Timer:** since `Game is ready` of the current game; in the main menu since the game
  start.

## Settings

`DATA/settings.json` (see [Portable](#portable)), read and written with a source-generated
`System.Text.Json` context. It stores the language (`"game"` or a language code), the
[theme](#platform) of the settings window (`"theme"`: `"system"`, `"light"` or `"dark"`,
case-sensitive, `"system"` by default) and the [preset](presence.md#presets) (`"career"`, `"network"`, `"era"`, `"alternating"` or
`"minimal"` or `"custom"`), and the [custom templates](presence.md#custom-templates-and-preview)
(`"templates"` with `"details"`, `"state"` and `"hover"`), and whether the presence carries the
[store button](presence.md#store-button) (`"storeButton"`, a boolean, `false` by default). An
invalid value falls back to its default without resetting the others, whatever its JSON type
(a `language`, `theme` or `preset` that isn't a string, a `storeButton` that isn't `true` or
`false`, `templates` that isn't an object); a template over 512 characters is cut. A missing or
broken file means defaults; the app writes to a temporary file and renames it, so a crash can't leave a half-written file.

## References

- .NET 11 MAUI: [What's new](https://learn.microsoft.com/dotnet/maui/whats-new/dotnet-11),
  [release notes Preview 1 – RC2](https://github.com/dotnet/core/tree/main/release-notes/11.0/preview)
- .NET MAUI: [Native AOT on iOS and Mac Catalyst](https://learn.microsoft.com/dotnet/maui/deployment/nativeaot),
  [Publish an unpackaged Windows app](https://learn.microsoft.com/dotnet/maui/windows/deployment/publish-unpackaged-cli),
  [Windows App SDK 1.6 (PublishAot for WinUI 3)](https://learn.microsoft.com/windows/apps/windows-app-sdk/release-notes-archive/stable-channel-1.6)
- .NET SDK: [workload sets and `global.json`](https://learn.microsoft.com/dotnet/core/tools/dotnet-workload-sets)
- Tray: [WinUIEx `TrayIcon`](https://dotmorten.github.io/WinUIEx/concepts/TrayIcon.html),
  [Drastic.Tray](https://www.nuget.org/packages/Drastic.Tray)
- Discord: <https://discord.com/developers/docs>,
  [Rich Presence overview](https://docs.discord.com/developers/rich-presence/overview)
- Lachee's [discord-rpc-csharp](https://github.com/Lachee/discord-rpc-csharp): reference for
  the pipe locations, frames and text limits
- Logging: [NReco.Logging.File](https://github.com/nreco/logging)
