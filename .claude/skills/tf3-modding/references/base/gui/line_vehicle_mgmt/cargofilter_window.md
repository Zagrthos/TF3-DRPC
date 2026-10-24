# gui.line_vehicle_mgmt.cargofilter_window

Definition file: `base/tealdef/gui/line_vehicle_mgmt/cargofilter_window.d.tl` (in the game folder).

The per-stop cargo filter popover of the line manager (`/gui/line_vehicle_mgmt/cargofilter_window.tl`, which returns the window recipe directly).

## CargoFilterUtil (record)

```teal
global record CargoFilterUtil
```
Load: `local CargoFilterWindow = ug_require "::/gui/line_vehicle_mgmt/cargofilter_window.tl" as CargoFilterUtil`

Per-stop cargo filter popover of the line manager (`CargoFilterWindow`).

- `CargoFilterWindow : Recipe<CargoFilterParams>`: Window recipe of the cargo filter popover, opened by the line manager as a singleton window.

    ```lua
    -- from manager_window.tl
    local params = {}
    params.lineEntity = where.line
    params.stopIndex = where.apiStopIndex0
    params.stopNumber = where.stopNumber
    params.gameCtx = gameCtx
    params.pos = pos + api.type.Vec2f.new(0.002, -0.4)
    params.onClose = function() gameCtx.windowContainer:get():getApi().removeAllWindows(CargoFilterWindow) end
    gameCtx.windowContainer:get():getApi().addSingletonWindow(CargoFilterWindow, params)
    ```

### CargoFilterUtil.CargoFilterParams (record)

```teal
record CargoFilterParams
```

Parameters of the cargo filter popover for one stop of a line.

Notes:

The popover edits the stop's `stopConfig` (cargo to load, max. fill level, force unload,
destroy-cargo options), its load mode and its min./max./additional waiting times, and
applies them with `api.cmd.makeLineUpdateCmd` (setting `customFilters = true` on the line).

- `lineEntity : Engine.Entity`: Line whose stop is configured.
- `stopNumber : integer`: Stop number shown in the window title (`#<number> - <station group name>`).
- `pos : Vec2f`: Initial window position (`initialX`/`initialY` of the window).
- `stopIndex : integer`: zero-based
- `onClose : function()`: Called when the popover is closed.
- `gameCtx : GameContext`: Game context; its renderer component is used to bind `action`.
- `action : function() : TreeNodeId`: Optional renderer action bound to the popover content while it is mounted.
