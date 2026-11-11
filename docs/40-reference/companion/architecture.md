---
title: Companion app architecture
type: reference
audience: [contributor, agent]
area: [app]
covers:
  - companion/Tf3Drpc.App/MauiProgram.cs
  - companion/Tf3Drpc.App/Extensions/ServiceCollectionExtensions.cs
  - companion/Tf3Drpc.App/Services/**
  - companion/Tf3Drpc.App/Messages/**
  - companion/Tf3Drpc.Core/Monitor/**
last-reviewed: 2026-11-11
---

# Companion app architecture

How the [companion app](README.md) is built: services do the work, and the UI only presents.
The rules the parts follow are in [architecture patterns](../../30-conventions/architecture-patterns.md).

## Structure

- **Builder:** MAUI's `MauiAppBuilder` provides DI and logging.
  No Generic Host and no hosted services: MAUI doesn't run them, and the background work is one loop.
- **Services:** one interface per service in `Services/Interfaces/`, the implementation in `Services/`, registered in `ServiceCollectionExtensions` in dependency tiers, injected through primary constructors.
  Platform-specific implementations (tray, single instance, paths) sit behind the same interfaces.
- **Background loop:** `IMonitorService` runs one `PeriodicTimer` (1 s) from app start until exit (`CancellationToken`) and sends an `AppStatusChangedMessage` when a tick changed the status.
  The tick itself is Core's `GameMonitor` (tested in `Tf3Drpc.Core.Tests`): it checks the game process, reads new log lines and lets the Discord service send or reconnect.
  It reaches the services through narrow Core interfaces (`IGameProcessSource`, `IGamePathResolver`, `ISettingsSource`, `ILanguageResolver`, `IPresenceBuilder`, `IPresenceSink`) that the app's service interfaces inherit, and owns the log reader and the state machine.
  While the game runs without a known log path it asks for the paths again every 15 s (`MonitorConstants.PathRetryInterval`); the warning "The game's files were not found yet" comes once per lost log path, not on every retry, and a found log path re-arms it.
  "Game paths: …" is logged only when the paths change.
- **Messages:** `IMessenger` (`WeakReferenceMessenger`) with immutable records in `Messages/`.
  `AppStatusChangedMessage` carries the game state, the Discord connection, the language and the presence texts; `SettingsChangedMessage` carries the settings after a change; `LogFileUnavailableMessage` tells the settings window that the daily log file can't be written.
  Receivers use the status of the message, because the loop updates the shared one.
- **ViewModels:** CommunityToolkit.Mvvm (`[ObservableProperty]`, `[RelayCommand]`).

## Services

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

## Source generators

Used wherever they exist:

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
