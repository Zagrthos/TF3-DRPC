# gui.hud.hud_gui_config

Definition file: `base/tealdef/gui/hud/hud_gui_config.d.tl` (in the game folder).

Type of the GUI resource `::/gui/hud/hud_gui_config.gres` (defined in `hud_gui_config.gres.lua`): distance scales, colors and vehicle icon offsets for the HUD icons drawn in the 3D view.

## HudGuiConfig (record)

```teal
global record HudGuiConfig
```

Content of the GUI resource `::/gui/hud/hud_gui_config.gres`, the base settings for HUD icons in the 3D view.

Example:
```lua
local hudGuiConfig = api.gui.genericRep.get(api.gui.genericRep.find("::/gui/hud/hud_gui_config.gres")).data
local offsets = hud_react_util.makeVehicleIconOffsets(hudGuiConfig.vehicleIconOffsets)
```

- `renderDistances : RenderDistances`: Per-entity-type HUD icon scales, see `HudGuiConfig.RenderDistances`.
- `colors : Colors`: Colors used by HUD icons, see `HudGuiConfig.Colors`.
- `vehicleIconOffsets : {string : {number}}`: Offset `{x, y, z}` of the vehicle HUD icon per carrier name (`"ROAD"`, `"RAIL"`, `"TRAM"`, `"AIR"`, `"WATER"`).
    - notes: The base game raises icons by 6 for road, rail and tram and by 18 for air and water vehicles. Convert it with `HudReactUtil.makeVehicleIconOffsets` before passing it to the HUD icon manager.

### HudGuiConfig.RenderDistances (record)

```teal
record RenderDistances
```

Scale multipliers for the HUD icons of each entity type (all `1.0` in the base game).

Notes:

`HudReactUtil.makeDefaultVisibilityThresholds` copies these values into the `scale` of the matching
`forComponents` entry; `Costs` is passed as `costsRenderDistance` to the HUD icon manager.

- `Town : number`: HUD icon scale for towns (`TOWN` component).
- `Industry : number`: HUD icon scale for industries (`INDUSTRY` component).
- `Warehouse : number`: HUD icon scale for warehouses (`WAREHOUSE` component).
- `Depot : number`: HUD icon scale for vehicle depots (`VEHICLE_DEPOT` component).
- `Station : number`: HUD icon scale for stations (`STATION_GROUP` component).
- `Signal : number`: HUD icon scale for signals (`SIGNAL_LIST` component).
- `TransportVehicle : number`: HUD icon scale for vehicles (`TRANSPORT_VEHICLE` component).
- `DeadEndPath : number`: HUD icon scale for dead-end markers on the network (`BASE_NODE` component).
- `TownBuildingCargo : number`: HUD icon scale for cargo icons on town buildings (`TOWN_BUILDING` component).
- `Costs : number`: Passed as `costsRenderDistance` to the HUD icon manager, which draws cost and income pop-ups.

### HudGuiConfig.Colors (record)

```teal
record Colors
```

Colors used by HUD icons and layers.

- `FallbackVehicle : GResColor`: Fallback color for vehicles in layer views; used as both surface and details color of `LayerReactUtil.createDefaultFallbackColor` in the HUD icon manager config (white in the base game).
- `Perk : GResColor`: Background color of perk HUD icons (`PerkHudIcon` in `gui/main/hud_icon_master.css.lua`; true blue in the base game).
- `Capital : GResColor`: Background color of the town HUD icon of the capital city (style `TownHudIcon!capital-city`; true blue in the base game).
