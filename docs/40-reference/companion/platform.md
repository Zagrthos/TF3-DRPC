---
title: Companion app platform
type: reference
audience: [contributor, agent]
area: [app, windows, macos]
covers:
  - companion/Tf3Drpc.App/Tf3Drpc.App.csproj
  - companion/Tf3Drpc.App/Platforms/**
  - companion/Tf3Drpc.App/Resources/Tray/**
last-reviewed: 2026-11-13
---

# Companion app platform

The targets, the platform features and the icons of the [companion app](README.md); the window is in [settings window](settings-window.md), the second start in [single instance](single-instance.md).
What differs between Windows and macOS, and what is decided for each difference, is in [Windows and macOS parity](../platform-parity.md).

## Targets

.NET 11 GA with the MAUI workload (MAUI `11.0.0`).

| | Windows | macOS |
|---|---|---|
| Target | WinUI 3, `net11.0-windows10.0.19041.0` | Mac Catalyst, `net11.0-maccatalyst` |
| Native AOT | `PublishAot` with `UseOptimizedPublish=false` (the optimised publish skips `PrepareForRun`, where the Windows App SDK's MSIX tooling writes the `.pri` of an unpackaged app, and fails with `MSB3030`; generating the `.pri` explicitly fills `bin/` again, so it saves nothing); works, one known MAUI trim warning and 20 informational ILC messages about the left-out WebView2 (see [Native AOT](../../30-conventions/native-aot.md)) | `PublishAot`, `RuntimeIdentifier` `maccatalyst-arm64`, `NoDSymUtil` (no `.dSYM`); works, one informational ILC message about `UIKit.NSLayoutAnchor`1` |
| Tray / menu bar | no MAUI API; WinUIEx's `TrayIcon` with a WinUI `MenuFlyout` (no AOT or trim warnings) | no MAUI or UIKit API; AppKit's `NSStatusItem` through the Objective-C runtime, as `Drastic.Tray` does |
| Minimum OS | Windows 10 2004 (`SupportedOSPlatformVersion` 10.0.19041.0) | macOS 26 (`SupportedOSPlatformVersion` 26.0) |
| Shipped as | portable zip: unpackaged, self-contained (`WindowsPackageType=None`, `WindowsAppSDKSelfContained=true`); not code-signed, so SmartScreen warns at the first start | portable zip with the `.app`, not signed or notarised, so Gatekeeper stops the first start; never through the Mac App Store |
| Built on | Windows | a Mac with Xcode 27.2, the version the .NET 11 Mac Catalyst SDK requires |

- **Native AOT rules:** see [Native AOT](../../30-conventions/native-aot.md).
- **Window:** [settings window](settings-window.md).
- **One instance:** [single instance](single-instance.md).

## Release zip

`tools/build-release.ps1` builds the zip of the platform it runs on ([build and publish](../../20-development/build-and-publish.md#steps)), and CI publishes both ([CI and releases](../ci-and-release.md#release)).
Each zip holds `LICENSE.txt` (the MIT licence of this project) and `THIRD-PARTY-NOTICES.txt` next to the exe or the `.app`.
`tools/new-third-party-notices.ps1` generates the notices at build time from the publish's restore output (`project.assets.json`): the licence texts of every package and runtime pack of the target, so they follow the package versions.
The first release ([decision 0024](../../70-decisions/0024-first-release-unsigned.md)) is unsigned on both platforms.

## Icons

App and tray icons are made from `assets/discord/cover.png`.
The tray icon is a committed multi-size `Tf3Drpc.App/Resources/Tray/tray.ico` (16–64 px), the menu bar icon on macOS a committed `Tray/tray.png` (36 px for 18 points), bundled as `tray.png`.
The window's title bar takes the one of its platform (`OnPlatform`).

## Menu bar (macOS)

`MacTrayService` creates the `NSStatusItem`, its menu and a small Objective-C target object for the menu actions through `ObjCRuntimeUtils` (`objc_msgSend` through `[LibraryImport]`, one declaration per signature).
The menu matches the Windows tray.

## macOS project settings

- `LSUIElement=true` (no Dock icon) as an `AppManifestEntry` item in the project file.
- No App Sandbox: the template's `Platforms/MacCatalyst/Entitlements.plist` enables it, so the entry is removed (see [the game log](../game-log.md#finding-the-game-and-its-log)).
- The UIKit scene lifecycle, which the Xcode 27 SDKs require and the MAUI templates include.

## .NET 11 MAUI features we use

| Feature | Use |
|---|---|
| Native AOT as opt-in for publishing (CoreCLR otherwise) | `PublishAot` for releases |
| XAML source generation (`MauiXamlInflator=SourceGen`) | AOT-safe XAML with lazy resource dictionaries; every binding is compiled (`XC0022`–`XC0025` are errors) |
| `OnAppInstanceActivated` / `AppInstance` | single instance on Windows |
| `AppManifestEntry` items | `LSUIElement` |
