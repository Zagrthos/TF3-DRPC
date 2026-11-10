---
title: 0018 No autoActivate in mod.json
type: decision
audience: [player, contributor, maintainer, agent]
area: [mod]
status: accepted
decided: 2026-10-17
last-reviewed: 2026-11-10
---

# 0018 No autoActivate in mod.json

## Context

Tested 2026-10-17: `autoActivate` doesn't turn the mod on, neither in a new game nor in an old save.

## Decision

`mod.json` has no `autoActivate`.

## Consequences

- Players turn the mod on per savegame and save once, as the [README](../../README.md#2-turn-the-mod-on-for-your-game) describes.
