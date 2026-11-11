---
title: 0002 The mod writes to the game log
type: decision
audience: [contributor, maintainer, agent]
area: [mod]
status: accepted
decided: 2026-10-24
last-reviewed: 2026-11-10
---

# 0002 The mod writes to the game log

## Context

No API function writes a file of our own: `app.saveUserdata` accepts only three fixed folders.

## Decision

The mod writes its status to the game log.

## Consequences

- The mod's output is read from the game log; see [mod.md](../mod.md).
