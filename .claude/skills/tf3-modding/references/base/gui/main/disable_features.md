# gui.main.disable_features

Definition file: `base/tealdef/gui/main/disable_features.d.tl` (in the game folder).

Names of game UI features that a mission can switch off. Missions return them as
`disableFeatures = { ["Layers"] = true }`; the mission script fires the
`setDisableFeatures` React event and the game UI keeps the set in `GameContext.disableFeatures`.

## DisableFeatures (enum)

```teal
global enum DisableFeatures
```

Keys of the feature-switch table `{DisableFeatures : boolean}`; a key set to `true` disables that UI feature.

Notes:

Read the current set with `game_react_globals.getDisableFeatures()` or
`gameCtx.disableFeatures:get()`. Missions set it through the `disableFeatures` table of
their task info (see `mission_sim.script.tl`).

Example:
```lua
-- from a campaign mission (mission_story.tl)
disableFeatures = {
    ["HudIconMaster"] = true,
},
```

- `"CreateNewLine"`: Disables creating new lines in the line manager.
- `"GameSpeedControl"`: Disables changing the game speed (speed buttons and input actions).
- `"GameSpeedPause"`: Disables pausing the game (game speed 0).
- `"HudIconMaster"`: Intended to hide the default HUD icons above entities.
- `"Layers"`: Disables the layer buttons (map overlays) of the game bar.
- `"OpenEntityWindow"`: Disables opening entity windows by selecting an entity.
- `"PerkHudIcons"`: Intended to hide perk HUD icons.
