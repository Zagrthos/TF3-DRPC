---
title: Work on the mod
type: guide
audience: [contributor, agent]
area: [mod]
covers:
  - tools/install-mod.ps1
  - mod/zagrthos_drpc/mod.json
last-reviewed: 2026-11-11
---

# Work on the mod

How to run the mod from your checkout and see a change in the game.
How the mod works is in [the mod](../40-reference/mod/mod.md), the rules for its code in [Lua style](../30-conventions/lua.md).

## Before you start

Transport Fever 3 is installed, and `pwsh` is available ([set up a development machine](setup.md)).

## Steps

1. Link the mod into the game's staging area once, from the repository root:

   ```sh
   pwsh ./tools/install-mod.ps1           # -Remove unlinks it again
   ```

   The game then loads the mod straight from your checkout ("My Mods" in the game), so the branch you have checked out is the mod you run.
   On Windows the link is a directory junction, on macOS a symbolic link.
2. Turn the mod on in a savegame and save once, as the [README](../../README.md#2-turn-the-mod-on-for-your-game) describes; without the save, the activation counts for that one load only.
3. After a change, load the savegame again; the game needs no restart.
4. Look for `[drpc-status]` lines in the [game log](../40-reference/game-log.md).

## Check

The game log gets one `[drpc-status]` line about every 10 seconds and no `[drpc]` warning, and the Lua language server reports no problems.
The full test with the app and Discord is in [test by hand](../50-testing/manual-test.md).
