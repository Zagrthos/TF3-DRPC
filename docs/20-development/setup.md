---
title: Set up a development machine
type: guide
audience: [contributor, agent]
area: [app, mod, tools]
covers:
  - companion/global.json
  - companion/nuget.config
last-reviewed: 2026-11-13
---

# Set up a development machine

What to install before you build the app, work on the mod or run the scripts.
A Mac needs the extra steps in [set up a Mac](setup-macos.md).

## Before you start

| For | You need |
|---|---|
| The companion app | the .NET SDK and MAUI workload set pinned in [companion/global.json](../../companion/global.json) (.NET 11) |
| Building on Windows | Windows 10 version 2004 or later |
| Building on macOS | macOS 26 or later, Xcode 27.2, and the `maui-maccatalyst` workload |
| The mod | Transport Fever 3; optionally the [Lua language server](https://luals.github.io/) |
| The scripts in `tools/` | PowerShell 7 (`pwsh`) |
| Manual tests | Transport Fever 3 on Steam and the Discord desktop app |
| Scripted test runs | additionally WinApp CLI 0.7 on Windows |

A Windows machine builds only the Windows app and a Mac only the macOS app, so a pull request names the platform it couldn't check.

## Steps

1. Install the .NET SDK whose version is `sdk.version` in `companion/global.json`.
   `dotnet --version` in `companion/` prints that version.
2. Install the pinned workload set, once per machine and again whenever `global.json` moves it or the SDK is reinstalled, from a folder outside the repository:

   ```sh
   cd ~
   dotnet workload update --version <workloadVersion from global.json>
   ```

   Inside `companion/`, `dotnet workload` refuses `--version`, because `global.json` already pins one.

   Visual Studio keeps its own older manifests, so its updates don't do this.
   `dotnet workload list` then shows the `maui` workloads.
3. Build once to check the setup ([build and publish](build-and-publish.md)).

## If it fails

| Symptom | Fix |
|---|---|
| Every project fails restore with "workload version … not found", and C# Dev Kit shows "Failed to restore NuGet packages for the solution" | Step 2 is missing or out of date, often after a new or reinstalled SDK: run it with the version from `global.json`, then reload the VS Code window. |
| `dotnet workload update --version` says a version is already set in `global.json` | It ran inside `companion/`: run it from a folder outside the repository. |
| C# Dev Kit keeps restoring a project that was removed | Dev Kit opened at the repository root caches its own solution in VS Code's `workspaceStorage/<id>/ms-dotnettools.csdevkit/`; delete that `.sln` and reload the window. |
