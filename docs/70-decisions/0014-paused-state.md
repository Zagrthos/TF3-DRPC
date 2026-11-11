---
title: 0014 Paused means speedup 0 or no status line for more than 20 s
type: decision
audience: [contributor, maintainer, agent]
area: [mod, app]
status: accepted
decided: 2026-10-24
last-reviewed: 2026-11-10
---

# 0014 Paused means speedup 0 or no status line for more than 20 s

## Context

The pause button leaves `guiUpdate` running and the status says `speedup` 0.
The game menu (Escape) stops `guiUpdate`, so no line arrives.
In the map editor time doesn't run, and the mod always writes `speedup` 0.

## Decision

Paused means `speedup` 0, or no status line for more than 20 s.
The map editor is never paused.
