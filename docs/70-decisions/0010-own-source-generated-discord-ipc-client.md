---
title: 0010 Our own source-generated Discord IPC client
type: decision
audience: [contributor, maintainer, agent]
area: [app, discord]
status: accepted
decided: 2026-10-24
last-reviewed: 2026-11-11
---

# 0010 Our own source-generated Discord IPC client

## Context

The known libraries depend on Newtonsoft.Json and aren't AOT-compatible.

## Decision

The app has its own source-generated Discord IPC client.

## Consequences

- The client and the protocol are described in [companion.md](../40-reference/companion/discord-connection.md).

## Alternatives considered

- Third-party libraries such as Lachee's `DiscordRichPresence`: they depend on Newtonsoft.Json and aren't Native AOT compatible.
