---
title: 0005 The mod is for PC only
type: decision
audience: [player, contributor, maintainer, agent]
area: [mod]
status: accepted
decided: 2026-10-24
last-reviewed: 2026-11-10
---

# 0005 The mod is for PC only

## Context

The companion app runs only on Windows and macOS, so on PS5 and Xbox the mod does nothing.
Neither the game nor the mod files can exclude consoles: an upload that passes the console checks is approved for PS5 and Xbox automatically.
mod.io offers no way to take it off consoles either.
The game leaves consoles out only after a critical console error in its validation (`mod_manager_react_util.tl`).

## Decision

The mod is for PC only.
The description says "PC only" and explains that the mod does nothing on consoles.

## Consequences

- The description is the only notice on consoles, because neither the game nor mod.io can leave them out.

## Alternatives considered

- Causing a critical console error in the validation to leave consoles out: it would mean breaking a console rule on purpose.
