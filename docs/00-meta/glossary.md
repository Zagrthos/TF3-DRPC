---
title: Glossary
type: reference
audience: [contributor, maintainer, agent]
area: [docs]
last-reviewed: 2026-11-11
---

# Glossary

The terms the documents use, with one meaning each.
A document uses the term as it is written here.

| Term | Meaning |
|---|---|
| alternating | the preset that shows Career, Network and Era in turn |
| area label | an `area-` label on issues and pull requests; the same values form the `area` key of the front matter |
| Career | the default preset, also called design A: the rank and the in-game date |
| companion app, app | the .NET MAUI app in `companion/` that follows the game log and talks to Discord |
| custom template | the player's own text for the first line, the second line and the image tooltip, with placeholders |
| `DATA` | the folder next to the app where it writes its settings and logs, and nothing else |
| decision record | one file in `docs/70-decisions/` that states one decision and its reason |
| docs gate | the steps of the [documentation guide](documentation-guide.md#docs-gate) that every change passes before it is committed |
| game log | the game's `stdout.txt` in the user data, which the mod writes to and the app follows |
| game script | the mod's Lua code that the game runs; its `guiUpdate` function writes the status line |
| menu bar icon | the macOS counterpart of the tray icon |
| mod | the game mod in `mod/zagrthos_drpc/` (mod id `zagrthos_drpc`) |
| Mod Hub | the game's own mod browser; it uploads the mod to mod.io |
| monitor, tick | the app's background loop (`IMonitorService`) and one pass of it (Core's `GameMonitor`), once per second |
| placeholder | a `{name}` in a custom template, such as `{rank}`, replaced by a game value |
| presence | what the app sends to Discord and Discord shows in the player's status |
| preset | one of the six looks of the status: Career, Network, Era, Alternating, Minimal, Custom |
| preview | the settings window's view of the presence as it is sent |
| record | a document that states what happened or was found on one date and stays unchanged afterwards |
| session | one run of the game process, from its start to its exit |
| settings window | the app's only window |
| staging area | the game's folder for local mods ("My Mods"), where `tools/install-mod.ps1` links the mod |
| state | the app's view of the game: Not running, Starting, Main menu, Loading, Mission, Playing, Closing |
| status line | the `[drpc-status]` line with one JSON object that the mod writes to the game log; the interface between mod and app |
| store button | the optional "Get Transport Fever 3" button below the presence |
| translocation | macOS starting a quarantined app from a hidden read-only copy |
| user data | the game's folder of profiles, settings and the game log in Steam's user data |
| workload set | the pinned set of .NET workload versions in `companion/global.json` |
