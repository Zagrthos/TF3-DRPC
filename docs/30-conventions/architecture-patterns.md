---
title: Architecture patterns
type: rules
audience: [contributor, agent]
area: [app]
covers:
  - companion/Tf3Drpc.App/Extensions/ServiceCollectionExtensions.cs
  - companion/Tf3Drpc.App/PlatformServices.cs
  - companion/Tf3Drpc.App/App.xaml
last-reviewed: 2026-11-13
---

# Architecture patterns

The patterns every part of the companion app follows.
How the app is built from them is in [companion architecture](../40-reference/companion/architecture.md).

## Services and dependency injection

- DI and logging come from MAUI's `MauiAppBuilder`; the app has no separate Generic Host and no hosted services.
- Background work runs in the one loop of `IMonitorService`.
- Each service has an interface in `Services/Interfaces/` and its implementation in `Services/`, injected through primary constructors.
- The services do the work; the UI only presents.
- Services return identifiers and values, and the presentation layer turns them into display strings.
- Registration is `Extensions/ServiceCollectionExtensions.cs`, an `internal static` class with a C# 14 `extension(IServiceCollection services)` block: `AddAppServices()` and `AddAppViewModels()`.
  Its comments name the dependency tier ("These services do not depend on anything", "These services depend on the above services"), not the service.
- Logic that can be tested lives in `Tf3Drpc.Core`, because the tests can't reference the MAUI app ([testing strategy](../50-testing/strategy.md)).

## Messages and results

- Messages are `internal sealed record class ...Message(...)` in `Messages/`, with a one-line `<summary>` and `<param>` docs, sent through `IMessenger`.
- A multi-value result on a service surface is an `internal readonly struct NameStruct : IEquatable<NameStruct>` in `Structs/`, in place of a tuple: with `[StructLayout(LayoutKind.Auto)]`, `init` properties, `Equals([NotNullWhen(true)] object? obj)` and `==`/`!=` operators with `in` parameters.
  Build it with an object initialiser.

## XAML first

- UI that XAML can express (window and title bar properties, visibility rules, text composition, styles) goes into XAML, not into C# or view-model properties.
- C# stays for DI, services and the platform APIs XAML can't reach.
- Platform-specific XAML values with resources go through typed `OnPlatform<T>` resources in `App.xaml`, because the `{OnPlatform …}` markup extension with `StaticResource` values isn't trim safe (IL2026).
- The Mac-only look comes from `MacPickerHandler`, `MacCheckBoxHandler` and the `Mac…` styles.

## Platform code

- The platform service types are named once in `PlatformServices.cs` (global using aliases); shared code uses those names.
- Windows-only parts of `Tf3Drpc.App.csproj` carry the condition `'$([MSBuild]::GetTargetPlatformIdentifier($(TargetFramework)))' == 'windows'`.
- The app is portable: see [portable app](portable-app.md).
