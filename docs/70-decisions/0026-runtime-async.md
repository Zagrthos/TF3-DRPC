---
title: 0026 Runtime async for all our assemblies
type: decision
audience: [contributor, maintainer, agent]
area: [app, windows, macos]
status: accepted
decided: 2026-11-13
last-reviewed: 2026-11-13
---

# 0026 Runtime async for all our assemblies

## Context

.NET 11 lets the runtime run async methods itself (runtime async), in place of the state machines the compiler generates.
The .NET 11 libraries are already built that way, and runtime async works with CoreCLR, which runs Debug builds on both platforms, and with Native AOT, which runs the releases.

## Decision

Every project compiles with `runtime-async=on`, set next to `Strict` in the `Features` of `companion/Directory.Build.props`.

## Consequences

- Our assemblies hold no async state machines, so live stack traces and the debugger show the real call chain, and async calls cost less.
- `async void` event handlers still get a state machine, because runtime async covers only methods that return a task.
- A project can opt out with `<UseRuntimeAsync>false</UseRuntimeAsync>` if a tool or platform turns out not to support it.

## Alternatives considered

- Keep the compiler's state machines: they work everywhere, but they forgo the cheaper calls and clearer stacks that the .NET 11 libraries already use.
