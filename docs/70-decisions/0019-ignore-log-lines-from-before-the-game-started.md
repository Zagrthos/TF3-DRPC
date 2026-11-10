---
title: 0019 Log lines from before the game process started are ignored
type: decision
audience: [contributor, maintainer, agent]
area: [app]
status: accepted
decided: 2026-10-24
last-reviewed: 2026-11-10
---

# 0019 Log lines from before the game process started are ignored

## Context

Until the game truncates its log, the file still holds the previous session.

## Decision

Log lines from before the game process started are ignored.
