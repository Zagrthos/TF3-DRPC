---
title: 0007 Portable app writing only to DATA, checked with a bounded check
type: decision
audience: [player, contributor, maintainer, agent]
area: [app, windows, macos]
status: accepted
decided: 2026-10-24
last-reviewed: 2026-11-11
---

# 0007 Portable app writing only to DATA, checked with a bounded check

## Context

Players unzip the app anywhere, and nothing is left behind.

## Decision

The app is portable and writes only to `DATA/`.
There is no installer and no start with the system.

## Consequences

- The rules for the app's files are in [portable-app.md](../30-conventions/portable-app.md).
- The bounded check is enough: it found nothing outside `DATA/` on Windows.
- On macOS the only files seen outside `DATA/` are the two the system writes (see [companion.md](../40-reference/companion/portable.md)).

## Alternatives considered

- A full write trace (Process Monitor on Windows, `fs_usage` on macOS): not made, because the bounded check is enough.
