---
title: 0013 The store button is off by default
type: decision
audience: [player, contributor, maintainer, agent]
area: [app, discord]
status: accepted
decided: 2026-10-24
last-reviewed: 2026-11-11
---

# 0013 The store button is off by default

## Context

A button in the player's status is advertising that the player has to choose.
Discord doesn't show the button to the player, so it can't be checked from their own client.

## Decision

The store button is off by default.

## Consequences

- The button is described in [presence.md](../40-reference/presence/presets-and-templates.md#store-button).
