# gui.main.game_react_globals

Definition file: `base/tealdef/gui/main/game_react_globals.d.tl` (in the game folder).

Global access to the main parts of the in-game GUI for code that has no `GameContext`, loaded with `ug_require "/gui/main/game_react_globals.tl"`.

## GameReactGlobals (record)

```teal
global record GameReactGlobals
```
Load: `local game_react_globals = ug_require "::/gui/main/game_react_globals.tl" as GameReactGlobals`

Functions returning the in-game renderer component, window container, tool stack and disabled features.

Notes:

The values exist only after the in-game GUI root (`GameUIRoot`) has been created.

- `getDefaultRendererComponent : function() : ReactRefWrapApiT<Builtin.RendererComponentParam, Builtin.RenderComponentAPI>`  (call as `game_react_globals.getDefaultRendererComponent(…)`): Returns the reference to the main 3D view (same as `GameContext.rendererComponent`).
- `getDefaultWindowContainer : function() : ReactRefWrapApiT<Builtin.WindowContainerDelegateParam, Builtin.WindowAPI>`  (call as `game_react_globals.getDefaultWindowContainer(…)`): Returns the reference to the in-game window container (same as `GameContext.windowContainer`).
- `getDefaultToolStack : function() : ReactRefWrapApiT<Builtin.ToolStackParam, Builtin.ToolStackAPI>`  (call as `game_react_globals.getDefaultToolStack(…)`): Returns the reference to the in-game tool stack (same as `GameContext.toolStack`).
- `getDefaultWindowApi : function() : Builtin.WindowAPI`  (call as `game_react_globals.getDefaultWindowApi(…)`): Returns the window API of the in-game window container.
    - returns: The `Builtin.WindowAPI`, or `nil` if the container does not exist (any more).
- `getDefaultToolStackApi : function() : Builtin.ToolStackAPI`  (call as `game_react_globals.getDefaultToolStackApi(…)`): Returns the API of the in-game tool stack.
    - returns: The `Builtin.ToolStackAPI`, or `nil` if the tool stack does not exist (any more).
- `getDisableFeatures : function() : {DisableFeatures : boolean}`  (call as `game_react_globals.getDisableFeatures(…)`): Returns the set of currently disabled features (the value of `GameContext.disableFeatures`).
    - used 4x in the base game, e.g. `mods/release/urbangames_campaign_mission_01/content/mission/mission/hud_replacement.script.tl:7`: `if game_react_globals.getDisableFeatures()["HudIconMaster"] then`
