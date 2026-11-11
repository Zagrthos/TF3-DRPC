---
title: 0021 Publish size measures cut the Windows publish from 305 MB to 83 MB
type: decision
audience: [contributor, maintainer, agent]
area: [app, windows]
status: accepted
decided: 2026-10-24
last-reviewed: 2026-11-11
---

# 0021 Publish size measures cut the Windows publish from 305 MB to 83 MB

## Context

The Windows publish was 305 MB.

## Decision

Unused Windows App SDK parts and symbols are left out of the publish, which brought it down to 83 MB.

## Consequences

- The measures and what each one saves are listed under Publish size in [companion.md](../40-reference/companion/projects-and-build.md#publish-size).
