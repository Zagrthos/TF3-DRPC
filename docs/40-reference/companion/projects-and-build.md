---
title: Projects and build setup
type: reference
audience: [contributor, agent]
area: [app, tools]
covers:
  - companion/TF3-DRPC.slnx
  - companion/global.json
  - companion/Directory.Build.props
  - companion/Directory.Build.targets
  - companion/Directory.Packages.props
  - companion/nuget.config
  - companion/Tf3Drpc.App/Tf3Drpc.App.csproj
  - companion/Tf3Drpc.Core/Tf3Drpc.Core.csproj
  - companion/Tf3Drpc.Core.Tests/Tf3Drpc.Core.Tests.csproj
last-reviewed: 2026-11-13
---

# Projects and build setup

The projects, folders and build settings of the [companion app](README.md).
The commands are in [build and publish](../../20-development/build-and-publish.md), the traps in [build pitfalls](../../20-development/build-pitfalls.md).

## Projects

| Project | Content |
|---|---|
| `Tf3Drpc.Core` (`net11.0`, `IsAotCompatible`) | log reader and parser, states, the monitor tick (`GameMonitor`), presence texts, `.mo` reader, settings file, Discord IPC client |
| `Tf3Drpc.App` (MAUI; Windows and Mac Catalyst) | settings window, tray / menu bar icon, platform code (paths, `DATA` location, single instance) |
| `Tf3Drpc.Core.Tests` (`net11.0`) | every Core part: parser and follower on `docs/40-reference/log-samples/`, state machine, the monitor tick (`GameMonitor`, with fakes for its services), presence texts, `.mo` reader, languages, settings, Discord frames and client (with an in-memory fake Discord) |

`Tf3Drpc.Core` needs no UI, so it builds and tests anywhere without the MAUI workload.

## Folders

In both projects, as far as they apply:

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

## Build setup

The rules are in [C# style](../../30-conventions/csharp.md) and [Native AOT](../../30-conventions/native-aot.md).

- `global.json`: `sdk.version` `11.0.100` (`allowPrerelease: false`, `rollForward: latestPatch`), `sdk.workloadVersion` `11.0.100.1` (MAUI `11.0.0`, Mac Catalyst SDK `27.2.11039`), `test.runner` `Microsoft.Testing.Platform`.
- All in `companion/`, where the commands run:
  - `TF3-DRPC.slnx`.
  - `Directory.Build.props`: the build policy, Debug and Release settings, trim analysis, our 13 satellite languages, file properties, and `EnableWindowsTargeting` so the Windows projects restore on macOS.
  - `Directory.Build.targets`: keeps the `.xml` doc files out of the output.
  - `Directory.Packages.props`: central package versions.
    The analysers Meziantou, Roslynator and Sonar and `ThisAssembly.AssemblyInfo` are `GlobalPackageReference`s.
  - `nuget.config`: nuget.org only.
    Feeds from a user- or machine-wide NuGet configuration would otherwise cause NU1507.
  - The app project: MAUI, Windows, Native AOT, size and security settings, and the build targets listed under [publish size](#publish-size).
- Every props and project file uses the same layout: topic groups with a comment header, alphabetical inside a group, a doc link after a non-obvious property.
- Build policy: `LangVersion` latest (C# 15), `Nullable` enable, `ImplicitUsings` disable, `Features` Strict and `runtime-async=on` ([decision 0026](../../70-decisions/0026-runtime-async.md)), `CheckForOverflowUnderflow`, `AnalysisLevel` latest-all, `EnforceCodeStyleInBuild`, `GenerateDocumentationFile`, nullable warnings as errors.
- Configurations Debug and Release only, no `Testing`.
  Release has no symbols (`DebugType none`); Debug embeds them.
  `UseArtifactsOutput` puts all output in `artifacts/`.
- Tests: xunit v3 on Microsoft.Testing.Platform.

## Publish size

The Windows publish is about 83 MB, and `TF3-DRPC.exe` about 25 MB; without the measures below it was 305 MB.

| Measure | Saving |
|---|---|
| No `.pdb` in Release | 158 MB |
| `ExcludeAssets="all"` for parts of the Windows App SDK that MAUI pulls in but the app doesn't use: `Microsoft.WindowsAppSDK.AI`, `.ML`, `.Search` (since MAUI 11.0.0, 3.5 MB), `.Widgets`, `Microsoft.Windows.AI.MachineLearning` (`onnxruntime.dll`, `DirectML.dll`) and `Microsoft.Web.WebView2` | ~43 MB |
| Target `RemoveUnusedMuiFromPublish`: WinUI `.mui` resources only for our 13 languages, not 119 | ~3.5 MB |
| Target `RemoveWindowsTileImages`: no MAUI tile and logo images; the unpackaged app uses the `.ico` | a few MB |
| AOT switches in the app project: `OptimizationPreference Size`, `TrimMode Full`, unused runtime features off, C#/WinRT size options | ~7 MB of the exe |

- The rest is mostly the self-contained Windows App SDK, which a portable app has to carry.
  Going much lower would mean pure WinUI 3 without MAUI.
- The six excluded packages are pinned to the versions MAUI resolves.
  Raise them only together with MAUI.
- Leaving out `Microsoft.Web.WebView2` makes the publish print 20 ILC messages "will always throw because: Failed to load assembly 'Microsoft.Web.WebView2.Core.Projection'", for the web view code in MAUI and WinUI.
  The app has no web view, so that code never runs; the messages are expected (see [Native AOT](../../30-conventions/native-aot.md#warnings)).
- The macOS `.app` is about 21 MB (10 MB zipped), nearly all of it the Native AOT executable: macOS brings AppKit and UIKit, so nothing like the Windows App SDK has to ship.
  The publish runs the full build first (`UseOptimizedPublish` is `false`, the .NET 11 default), and from clean output folders the `.app` holds only the executable and its resources ([build pitfalls](../../20-development/build-pitfalls.md#if-it-fails)).
  `NoDSymUtil` keeps the 44 MB `.dSYM` that a Native AOT build writes by default since .NET 11 out of `artifacts/`, because Release has no symbols.

## Extra app targets

Two more app targets are needed for a working build:

- `AddReferenceSatellitesToIlc`: Native AOT can't load satellite assemblies at runtime, and ILC only compiles in those of NuGet packages.
  This target adds the Core's translations.
  Without it the app's own texts are English only, while the tests still pass.
- `MarkWindowsAppSdkSourcesAsGenerated`: marks the Windows App SDK's `UndockedRegFreeWinRT-AutoInitializer.cs` as generated code, so its CA5392 isn't ours.
