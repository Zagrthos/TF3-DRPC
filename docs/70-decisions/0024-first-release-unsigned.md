---
title: 0024 The first release is unsigned on Windows and macOS
type: decision
audience: [player, contributor, maintainer, agent]
area: [app, windows, macos]
status: accepted
decided: 2026-10-24
last-reviewed: 2026-11-10
---

# 0024 The first release is unsigned on Windows and macOS

## Context

A code-signing certificate and the Apple Developer account cost money every year before anyone uses the app.

## Decision

The first release is unsigned on Windows and macOS.
The README walks players through the SmartScreen warning and Gatekeeper's "Open Anyway".

## Consequences

- Signing is listed in [status.md](../10-overview/status.md#to-do).
