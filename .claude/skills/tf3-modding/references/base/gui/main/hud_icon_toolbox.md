# gui.main.hud_icon_toolbox

Definition file: `base/tealdef/gui/main/hud_icon_toolbox.d.tl` (in the game folder).

Recipes for the HUD icons drawn above entities in the 3D view (vehicles with their cargo,
stocks, waiting passengers at stations, towns, perk icons, notifications), and the
parameter types that custom HUD icon scripts receive.

## HudIconToolbox (record)

```teal
global record HudIconToolbox
```
Load: `local hud_icon_toolbox = ug_require "::/gui/main/hud_icon_toolbox.tl" as HudIconToolbox`

HUD icon recipes and their parameter types.

- `PerkHudIcon : Recipe<PerkHudIconParam>`: Recipe that shows a single icon image as HUD icon; used by landmarks, the headquarters and other perk buildings in their `hudIconScript`.

    ```lua
    -- landmark_hud.script.tl, referenced by a construction as
    -- hudIconScript = { fileName = "::/landmarks/landmark_hud.script@hudIconFn" }
    return {
        hudIconFn = function(_captureParams, _params, _userParam)
            return hud_icon_toolbox.PerkHudIcon{
                class = "landmark",
                path = "::/gui/hud/icons/building_temple_32.tga",
            }
        end,
    }
    ```

    - used 9x in the base game, e.g. `base/content/game_mechanics/game_mechanics/emission/pollution_cleanup_facility_hud.script.tl:20`: `return hud_icon_toolbox.PerkHudIcon{`
- `HudIconMasterBase : Recipe3<Builtin.HudIconParam, HudIconToolbox.HudIconMasterUserParam, boolean>`: Minimal HUD icon used by layers. It shows the entity's notification icons plus, unless the third parameter is `true`, the town HUD icon or the engine's default HUD icon.
    - used 11x in the base game, e.g. `base/content/gui/gui/layers/layer_cargo_flow.tl:170`: `hud_icon_toolbox.HudIconMasterBase(param, userParam, false),`
- `HudIconMasterGame : Recipe2<Builtin.HudIconParam, HudIconToolbox.HudIconMasterUserParam>`: The main HUD icon recipe of the game view.
    - notes: Depending on the entity it shows: for vehicles their stop/depot status and cargo; for stock lists (not of town buildings) the stocks; for stations the waiting passengers and the terminal number; for towns the town HUD icon; for the first subconstruction of a construction with a `hudIconScript` the result of that script function, called as `fn(captureParams, hudIconParam, userParam)`. Notification icons are added in front. Missions replace this recipe with `ReplaceRecipe` to hide or change HUD icons.
    - used 8x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_hud.tl:249`: `children[#children + 1] = hud_icon_toolbox.HudIconMasterGame(param, userParam)`

### HudIconToolbox.HudIconMasterUserParam (interface)

```teal
interface HudIconMasterUserParam
```

User parameter passed to HUD icon recipes and to custom `hudIconScript` functions.

- `accessNotificationCacheFn : function(Engine.Entity) : {Notification}`: Returns the notifications of an entity; when set, notification icons are added in front of the HUD icon.
- `selectedEntity : Engine.Entity`: Currently selected entity; the terminal number of a station is only shown when it is selected.
- `hideEmptyVehicles : boolean`: When `true`, empty vehicles get no HUD icon; otherwise the default HUD icon is shown for them.

### HudIconToolbox.Happiness (record)

```teal
record Happiness
```

Waiting passengers at a station group or terminal, shown by the station HUD icon.

- `totalWaiting : integer`: Number of passengers waiting.
- `unhappy : integer`: Number of waiting passengers with bad quality (unhappy).

### HudIconToolbox.PerkHudIconParam (record)

```teal
record PerkHudIconParam
```

Parameters of `PerkHudIcon`.

- `path : string`: Path of the icon image (e.g. a `.tga` file).
- `class : string`: Style class of the image view (e.g. `"landmark"`).
