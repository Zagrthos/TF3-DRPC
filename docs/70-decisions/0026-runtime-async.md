---
title: 0026 Runtime async for all our assemblies
type: decision
audience: [contributor, maintainer, agent]
area: [app, windows, macos]
status: accepted
decided: 2026-11-13
last-reviewed: 2026-11-15
---

# 0026 Runtime async for all our assemblies

## Context

.NET 11 lets the runtime run async methods itself (runtime async), in place of the state machines the compiler generates.
The .NET 11 libraries are already built that way, and runtime async works with CoreCLR, which runs Debug builds on both platforms, and with Native AOT, which runs the releases.
It is still a preview feature in .NET 11: the [GA announcement](https://devblogs.microsoft.com/dotnet/announcing-dotnet-11/) says so, and the compiler switch needs no `EnablePreviewFeatures`.

## Decision

Every project compiles with `runtime-async=on`, which `companion/Directory.Build.targets` adds to the `Features` that `companion/Directory.Build.props` sets.

## Consequences

- Our assemblies hold no async state machines, so live stack traces and the debugger show the real call chain, and async calls cost less.
  Exception stack traces look the same as before.
- `async void` event handlers and async iterators still get a state machine, because runtime async covers only methods that return a `Task` or `ValueTask`.
- As a preview, it may change in a .NET 11 servicing update or in .NET 12; a problem in it shows up in our async code paths, the Discord connection and the monitor loop first.
- A project can opt out with `<UseRuntimeAsync>false</UseRuntimeAsync>` in its project file.
  The SDK doesn't know that property: our `Directory.Build.targets` reads it, as the dotnet/runtime repository does, which is why the switch is in the targets file and not in the props file, which is imported before the project file sets anything.

## Alternatives considered

- Keep the compiler's state machines: they work everywhere, but they forgo the cheaper calls and clearer stacks that the .NET 11 libraries already use.
- Wait for runtime async to leave preview: the Native AOT releases and the Debug builds on both platforms run with it, and the opt-out is one property.
