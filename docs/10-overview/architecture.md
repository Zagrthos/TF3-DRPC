---
title: How TF3-DRPC works
type: explanation
audience: [player, contributor, maintainer, agent]
area: [mod, app, discord]
last-reviewed: 2026-11-10
---

# How TF3-DRPC works

- **The mod** reads the game state (date, mode, company rank, money, network statistics) and writes it as one JSON line to the game log every ~10 s.
- **The companion app**, a portable .NET 11 MAUI app for Windows and macOS published with Native AOT, sits in the tray or menu bar, follows the game log and shows the result in Discord.
  It runs from the extracted zip and writes only into a `DATA` folder next to it.

```
┌──────────────────────── Transport Fever 3 ────────────────────────┐
│  zagrthos_drpc mod (Lua, official API only)                       │
│  game script guiUpdate ── every ~10 s ──► "[drpc-status] {json}"   │
│  game itself ──► main menu, loading, mission lines                │
└───────────────────────────────────────────┬───────────────────────┘
                                            │  game log (stdout.txt in the user data)
                                            ▼
┌──────── Companion app (.NET MAUI, Native AOT; Windows + macOS) ───┐
│  follows the game log + watches the game process                  │
│  ──► Discord IPC (pipe / Unix socket) ──► Discord desktop client  │
└───────────────────────────────────────────────────────────────────┘
```

The mod goes to mod.io through the in-game mod hub; the companion app is released separately (for example on GitHub).
It can't be one part: the scripting API can't reach another process, mods may contain no executables, and the game's Lua 5.2.2 exports no C API for native extensions.
The decision is recorded in [0001](../70-decisions/0001-mod-and-companion-app.md).

## Sources

The TF3 install (Steam on Windows, build 40420 from 2026-10-08, later 40423 and 40425) with its Teal definitions and base-game scripts, the TF3 modding reference (version `2026.10.23.2`, online at <https://tf3-docs.transportfever.net>), test runs in the game on Windows (2026-10-13 to 2026-10-24) and on macOS 27 (2026-10-20 to 2026-10-24), the .NET MAUI documentation and the .NET 11 release notes up to GA.
Each document lists its own references.
