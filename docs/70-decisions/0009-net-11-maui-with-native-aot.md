---
title: 0009 .NET 11 MAUI with Native AOT on both platforms
type: decision
audience: [contributor, maintainer, agent]
area: [app, windows, macos]
status: accepted
decided: 2026-10-24
last-reviewed: 2026-11-10
---

# 0009 .NET 11 MAUI with Native AOT on both platforms

## Context

One app should serve Windows and macOS, and Native AOT gives a faster start-up and a smaller size.

## Decision

The app is a .NET 11 MAUI app, published with Native AOT on both platforms.

## Consequences

- The Native AOT rules are in [native-aot.md](../30-conventions/native-aot.md).
