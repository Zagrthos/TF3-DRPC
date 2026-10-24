# mission.guide_system.guide_system_react

Definition file: `base/tealdef/mission/guide_system/guide_system_react.d.tl` (in the game folder).

GUI recipe that shows the hints (guides) of the guide system.

## GuideSystemParams (record)

```teal
global record GuideSystemParams
```

Params of the guide-system recipe.

- `gameCtx : GameContext`: Game context.

## GuideSystemReact (record)

```teal
global record GuideSystemReact
```
Load: `local guide_system_react = ug_require "::/mission/guide_system/guide_system_react.tl" as GuideSystemReact`

GUI of the guide system.

- `GuideSystem : Recipe<GuideSystemParams>`: Recipe that shows the newest triggered, not yet completed guide and sends `guide-complete` when it is dismissed.
    - used 1x in the base game, e.g. `base/content/gui/gui/main/game.tl:514`: `item = guide_system_react.GuideSystem{refRC = refRC, gameCtx = gameCtx },`
