---
title: 0001 Mod plus separate companion app
type: decision
audience: [contributor, maintainer, agent]
area: [mod, app]
status: accepted
decided: 2026-10-24
last-reviewed: 2026-11-10
---

# 0001 Mod plus separate companion app

## Context

The scripting API can't reach another process, mods may contain no executables, and the game's Lua exports no C API.

## Decision

The project is a mod plus a separate companion app.

## Consequences

- The mod reports the game state and the companion app shows it in Discord; how the two work together is in [How TF3-DRPC works](../10-overview/architecture.md).
