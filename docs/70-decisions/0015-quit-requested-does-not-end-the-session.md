---
title: 0015 Quit requested does not end the session
type: decision
audience: [contributor, maintainer, agent]
area: [app]
status: accepted
decided: 2026-10-17
last-reviewed: 2026-11-10
---

# 0015 Quit requested does not end the session

## Context

The game can stay open after `Quit requested` (seen 2026-10-17), which left the presence cleared while the game ran.

## Decision

`Quit requested` doesn't end the session.
Only `Normal shutdown. Bye!`, `Ungraceful exit` or the process exit do.
