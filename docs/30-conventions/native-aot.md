---
title: Native AOT
type: rules
audience: [contributor, agent]
area: [app]
covers:
  - companion/Tf3Drpc.App/Tf3Drpc.App.csproj
  - companion/Tf3Drpc.Core/Tf3Drpc.Core.csproj
  - companion/Directory.Build.props
last-reviewed: 2026-11-10
---

# Native AOT

The app and `Tf3Drpc.Core` are published with Native AOT, and these rules hold in both without exception; `Tf3Drpc.Core.Tests` is exempt.
Why the app is AOT-compiled is in [docs/README.md](../README.md#decisions).

## Code

- **Loops, not LINQ:** use loops and pre-sized collections (`new List<T>(capacity)`).
- **Compile-time metadata, not reflection:** the app name and version come from `ThisAssembly.Info`; enum texts and keys come from switch-expression `extension(EnumType value)` members in `Extensions/EnumExtensions.cs`, never from attributes read by reflection.
- **Source generation wherever it exists:**
  - JSON only through the one source-generated `JsonSourceGenContext` per project in `Serialization/`; reflection-based JSON is switched off, and a differing option (such as indentation) comes from a second instance with its own `JsonSerializerOptions`.
  - Logging only through `[LoggerMessage]` partial methods in `Logging/LogMessages.cs`: extension methods on `ILogger<TheClass>`, grouped in regions by level, then by logger class, in place of `LogInformation` and the like.
  - Regular expressions only as `[GeneratedRegex]` partial properties with a match timeout in `Utilities/RegexUtils.cs`, used through its helper methods.
  - Native calls through `[LibraryImport]`.
  - ViewModels through CommunityToolkit.Mvvm, XAML through MAUI's source generator.
- Invariant data is formatted with `string.Create(CultureInfo.InvariantCulture, $"...")`.
- The build is detected with `#if DEBUG`, not with runtime environment checks.

## Warnings

- Every trim or AOT warning is fixed, never suppressed; only `dotnet publish` shows them all, and `TrimmerSingleWarn=false` in `Directory.Build.props` shows each on its own.
- The one known exception is not ours and stays unsuppressed: an `IL2070` in `Microsoft.Maui.Controls` (`MauiItemsView.DragDrop.cs`, the Windows CollectionView drag and drop, which the app doesn't use), present in MAUI 11.0.0.
  Check it again with each MAUI update.
- The publish also prints ILC messages "Method '…' will always throw because: Failed to load …".
  They are information, not warnings, and need no fix: ILC compiled code that can never run into a throw.
  - On Windows there are 20, for the web view code of MAUI (`WebViewHandler`, `HybridWebViewHandler`, `MauiWebView`, `WebViewExtensions`) and WinUI (`WebView2`), because `Microsoft.Web.WebView2` is left out of the publish with the other unused Windows App SDK parts ([docs/companion.md](../companion.md#projects)).
    The app has no web view, and its links open the system browser through `Launcher`, so the WebView2 package stays out.
  - On macOS there is one, for ``UIKit.NSLayoutAnchor`1`` in the Mac Catalyst SDK's type map.

## Translations

Native AOT compiles satellite assemblies into the exe, but ILC only takes those of NuGet packages, so the app project adds those of project references (`AddReferenceSatellitesToIlc`).
Check a new translation in the published exe as well as in `dotnet test`.
