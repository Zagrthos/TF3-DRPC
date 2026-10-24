# gui.main.free_cam_options

Definition file: `base/tealdef/gui/main/free_cam_options.d.tl` (in the game folder).

The free camera tool of the game UI: a tool for the tool stack that switches to the free
camera (or the cockpit camera of a vehicle) and a recipe that handles leaving it.

## FreeCamOptions (record)

```teal
global record FreeCamOptions
```
Load: `local free_cam_options = ug_require "::/gui/main/free_cam_options.tl" as FreeCamOptions`

Free camera tool and its input-handling recipe.

- `FreeCamOptions : Recipe<FreeCamOptionsParam>`: Recipe without visible content that pops `FreeCamTool` on `IA_BACK`, `IA_MENU_BACK` or when the engine reports `stoppedFollowAction`; on PC it also takes screenshots on `IA_GAME_SCREENSHOT`.
    - used 1x in the base game, e.g. `base/content/gui/gui/main/game.tl:901`: `item = free_cam_options.FreeCamOptions(react.ref(gameFreeCamRef), {`
- `FreeCamTool : ReactToolDefinition<FreeCamToolParam>`: Tool (name `"FreeCam"`) that enters the cockpit camera or toggles the free camera when pushed, and leaves the cockpit camera when popped.

    ```lua
    -- from entity_window_util.tl
    gameCtx.toolStack:get():getApi().push(free_cam_options.FreeCamTool, nil,
        {gameCtx = gameCtx, cockpitCameraForEntity = entityId}, true)
    ```

    - used 3x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_util.tl:1733`: `gameCtx.toolStack:get():getApi().push(free_cam_options.FreeCamTool, nil, {gameCtx = gameCtx, cockpitCameraForEntity = entityId}, true)`

### FreeCamOptions.FreeCamOptionsParam (record)

```teal
record FreeCamOptionsParam
```

Parameters of the `FreeCamOptions` recipe.

- `getGameContext : function() : GameContext`: Returns the game context; its tool stack is used to pop `FreeCamTool`.

### FreeCamOptions.FreeCamToolParam (record)

```teal
record FreeCamToolParam
```

Tool parameter of `FreeCamTool`.

- `cockpitCameraForEntity : Engine.Entity`: Vehicle entity whose cockpit camera to enter; when `nil`, the free camera is toggled instead.
