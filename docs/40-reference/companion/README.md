---
title: The companion app
type: index
audience: [contributor, agent]
area: [app]
last-reviewed: 2026-11-11
---

# The companion app

A portable .NET MAUI app for Windows and macOS that follows the [game log](../game-log.md) and shows the [presence](../presence/presets-and-templates.md) in Discord.
Name: `TF3-DRPC`.
Decisions are in [70-decisions/](../../70-decisions/README.md), open work in [status.md](../../10-overview/status.md).

## Documents

- [platform.md](platform.md): the targets, the release zip, icons, the macOS project settings and the MAUI features in use.
- [settings-window.md](settings-window.md): the settings window, its placement, layout, theme, pickers, accessibility and macOS look.
- [single-instance.md](single-instance.md): how a second start hands over to the running app.
- [portable.md](portable.md): the `DATA` folder, what the app never writes, translocation and the logs.
- [projects-and-build.md](projects-and-build.md): the projects, the folders, the build setup and the publish size.
- [architecture.md](architecture.md): the builder, the services, the messages and the source generators.
- [discord-connection.md](discord-connection.md): the application, the transport, the update limits and the reconnects.
- [states.md](states.md): the game states and what Discord shows in each.
- [settings-file.md](settings-file.md): `DATA/settings.json`.

## References

- .NET 11 MAUI: [What's new](https://learn.microsoft.com/dotnet/maui/whats-new/dotnet-11), [release notes](https://github.com/dotnet/core/tree/main/release-notes/11.0)
- .NET MAUI: [Native AOT on iOS and Mac Catalyst](https://learn.microsoft.com/dotnet/maui/deployment/nativeaot), [Publish an unpackaged Windows app](https://learn.microsoft.com/dotnet/maui/windows/deployment/publish-unpackaged-cli), [Windows App SDK 1.6 (PublishAot for WinUI 3)](https://learn.microsoft.com/windows/apps/windows-app-sdk/release-notes-archive/stable-channel-1.6)
- .NET SDK: [workload sets and `global.json`](https://learn.microsoft.com/dotnet/core/tools/dotnet-workload-sets)
- Tray: [WinUIEx `TrayIcon`](https://dotmorten.github.io/WinUIEx/concepts/TrayIcon.html), [Drastic.Tray](https://www.nuget.org/packages/Drastic.Tray)
- Discord: <https://discord.com/developers/docs>, [Rich Presence overview](https://docs.discord.com/developers/rich-presence/overview)
- Lachee's [discord-rpc-csharp](https://github.com/Lachee/discord-rpc-csharp): reference for the pipe locations, frames and text limits
- Logging: [NReco.Logging.File](https://github.com/nreco/logging)
