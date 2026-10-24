# gui.hud.hud_react_util

Definition file: `base/tealdef/gui/hud/hud_react_util.d.tl` (in the game folder).

Helpers for configuring HUD icons in the 3D view: building and editing `Builtin.Type.VisibilityThresholds`, the default line-viewer snap factors and per-carrier vehicle icon offsets.

## HudReactUtil (record)

```teal
global record HudReactUtil
```
Load: `local hud_react_util = ug_require "::/gui/hud/hud_react_util.tl" as HudReactUtil`

Helpers to build and modify the `Builtin.Type.VisibilityThresholds` that control when and in which draw order HUD icons appear.

Notes:

All `set*`/`reset*` functions return a new thresholds object and leave the input unchanged.
The result goes into `visibilityThresholds` of a `builtin.HudIconManagerConfig` (see the layer windows in `gui/layers`).

- `DefaultVisDistDetailThreshold : number`: Default camera distance (350) beyond which a station shows its main icon instead of the per-terminal sub icons; used as `detailsThreshold` of the station entry.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/station_group/station_group.tl:933`: `stationDetailsThresholds.detailsThreshold = hud_react_util.DefaultVisDistDetailThreshold * 2`
- `DefaultCameraHeightThreshold : number`: Default `cameraHeightThreshold` of the visibility thresholds (150).
- `DefaultCameraHeightLowerCutoff : number`: Default `cameraHeightLowerCutoff` of the visibility thresholds (30).
- `DefaultHudLodFarRangeThreshold : number`: Default `hudLodFarRangeThreshold` of the visibility thresholds (3000).

convert the config formulated with lua tables into the actual userdata type
- `makeVisibilityThresholdsUserdata : function(visibilityThresholds : Builtin.Type.VisibilityThresholds) : Builtin.Type.VisibilityThresholds`  (call as `hud_react_util.makeVisibilityThresholdsUserdata(…)`)
- `makeDefaultVisibilityThresholds : function() : Builtin.Type.VisibilityThresholds`  (call as `hud_react_util.makeDefaultVisibilityThresholds(…)`): Builds the base game's default HUD icon visibility thresholds for lines, towns, industries, warehouses, depots, stations, signals, vehicles, dead ends, town buildings and sub-constructions.
    - returns: A new `Builtin.Type.VisibilityThresholds` userdata.
    - notes: The per-type `scale` values come from `HudGuiConfig.renderDistances`. Order matters: the first matching component entry applies to an entity with several components. By default, town districts are hidden (`detailsThreshold = -1` for `SelectionDetails.Type.Town`); see `resetSubEntityThresholds`.

    ```lua
    local visibilityThresholds = hud_react_util.makeDefaultVisibilityThresholds()
    -- show town districts as well (from gui/layers/layer_cargo_flow.tl)
    visibilityThresholds = hud_react_util.resetSubEntityThresholds(visibilityThresholds, api.gui.SelectionDetails.Type.Town)
    ```

    - used 25x in the base game, e.g. `base/content/game_mechanics/game_mechanics/emission/pollution_cleanup_facility_hud.script.tl:99`: `visibilityThresholds = hud_react_util.makeDefaultVisibilityThresholds(),`
- `findSubEntityThresholds : function(thresholds : Builtin.Type.VisibilityThresholds, detailsType : api.gui.SelectionDetails.Type, makeIfNotExisting? : boolean) : Builtin.Type.VisibilityThresholds.OfSubEntity`  (call as `hud_react_util.findSubEntityThresholds(…)`): Looks up the sub-entity thresholds for a selection-details type.
    - param `thresholds`: Thresholds to search.
    - param `detailsType`: Sub-entity type, e.g. `api.gui.SelectionDetails.Type.Station`.
    - param `makeIfNotExisting`: If true, return a new empty `OfSubEntity` instead of `nil` when there is no entry.
    - returns: The entry from `thresholds.forSubEntities` (not a copy), a new empty object, or `nil`.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/station_group/station_group.tl:932`: `local stationDetailsThresholds = hud_react_util.findSubEntityThresholds(visibilityThresholds, api.gui.SelectionDetails.Type.Station, true)`
- `setSubEntityThresholds : function(thresholds : Builtin.Type.VisibilityThresholds, detailsType : api.gui.SelectionDetails.Type, value : Builtin.Type.VisibilityThresholds.OfSubEntity) : Builtin.Type.VisibilityThresholds`  (call as `hud_react_util.setSubEntityThresholds(…)`): Returns a copy of the thresholds with the sub-entity entry for `detailsType` replaced by `value`.
    - param `thresholds`: Thresholds to copy.
    - param `detailsType`: Sub-entity type whose entry is replaced.
    - param `value`: New sub-entity thresholds.
    - returns: New thresholds object.
    - notes: If `forSubEntities` is not set, a list with only the new entry is created. If the list exists but has no entry for `detailsType`, no entry is added.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/station_group/station_group.tl:934`: `visibilityThresholds = hud_react_util.setSubEntityThresholds(visibilityThresholds, api.gui.SelectionDetails.Type.Station, stationDetailsThresholds)`
- `resetSubEntityThresholds : function(thresholds : Builtin.Type.VisibilityThresholds, detailsType : api.gui.SelectionDetails.Type) : Builtin.Type.VisibilityThresholds`  (call as `hud_react_util.resetSubEntityThresholds(…)`): Returns a copy of the thresholds where the sub-entity entry for `detailsType` has no `detailsThreshold`, so main icon and sub-entity icons are shown together.
    - param `thresholds`: Thresholds to copy.
    - param `detailsType`: Sub-entity type, e.g. `api.gui.SelectionDetails.Type.Town` to show town districts.
    - returns: New thresholds object; `layer` and `physicsImpact` of the entry are kept.
    - used 4x in the base game, e.g. `base/content/gui/gui/construction/construction_react_util.tl:1268`: `visibilityThresholds = hud_react_util.resetSubEntityThresholds(`
- `setMainIconsLayer : function(thresholds : Builtin.Type.VisibilityThresholds, componentType : Engine.ComponentType, layer : integer) : Builtin.Type.VisibilityThresholds`  (call as `hud_react_util.setMainIconsLayer(…)`): Returns a copy of the thresholds with a new draw-order `layer` for the main icons of one component type.
    - param `thresholds`: Thresholds to copy.
    - param `componentType`: Component type of the entry to change.
    - param `layer`: New draw order (lower is drawn in front).
    - returns: New thresholds object; other values of the entry are kept.
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/line_util.tl:1144`: `visibilityThresholds = hud_react_util.setMainIconsLayer(`
- `setSubAndMainIconsLayer : function(thresholds : Builtin.Type.VisibilityThresholds, componentType : Engine.ComponentType, detailsType : api.gui.SelectionDetails.Type, layer : integer) : Builtin.Type.VisibilityThresholds`  (call as `hud_react_util.setSubAndMainIconsLayer(…)`): Like `setMainIconsLayer`, but also sets the same `layer` on the sub-entity entry for `detailsType`.
    - param `thresholds`: Thresholds to copy.
    - param `componentType`: Component type of the main-icon entry to change.
    - param `detailsType`: Sub-entity type of the sub-icon entry to change.
    - param `layer`: New draw order (lower is drawn in front).
    - returns: New thresholds object.
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/line_util.tl:1138`: `visibilityThresholds = hud_react_util.setSubAndMainIconsLayer(`
- `findComponentEntityThresholds : function(thresholds : Builtin.Type.VisibilityThresholds, type : api.engine.ComponentType, makeIfNotExisting? : boolean) : Builtin.Type.VisibilityThresholds.OfComponent`  (call as `hud_react_util.findComponentEntityThresholds(…)`): Looks up the thresholds entry for a component type.
    - param `thresholds`: Thresholds to search.
    - param `type`: Component type, e.g. `api.type.ComponentType.STATION_GROUP`.
    - param `makeIfNotExisting`: If true, return a new empty `OfComponent` instead of `nil` when there is no entry.
    - returns: The entry from `thresholds.forComponents` (not a copy), a new empty object, or `nil`.
- `setComponentEntityThresholds : function(thresholds : Builtin.Type.VisibilityThresholds, type : api.engine.ComponentType, value : Builtin.Type.VisibilityThresholds.OfComponent) : Builtin.Type.VisibilityThresholds`  (call as `hud_react_util.setComponentEntityThresholds(…)`): Returns a copy of the thresholds with the entry for `type` replaced by `value`.
    - param `thresholds`: Thresholds to copy.
    - param `type`: Component type whose entry is replaced.
    - param `value`: New component thresholds.
    - returns: New thresholds object.
    - notes: Only replaces an existing entry; if there is no entry for the component type, none is added.
- `makeDefaultLineViewerSnapFactors : function() : LineViewerSnapFactors`  (call as `hud_react_util.makeDefaultLineViewerSnapFactors(…)`): Returns the default line viewer snap factors (`view = 0.05`, `world = 10.0`).
    - used 6x in the base game, e.g. `base/content/gui/gui/entity_window/line/line.tl:173`: `local snapFactorRef = react.useRefLazy(function() : HudReactUtil.LineViewerSnapFactors return hud_react_util.makeDefaultLineViewerSnapFactors() end)`
- `makeVehicleIconOffsets : function(vehicleIconOffsets : {string : {number}}) : {Carrier : Vec3f}`  (call as `hud_react_util.makeVehicleIconOffsets(…)`): Converts `HudGuiConfig.vehicleIconOffsets` (carrier name to `{x, y, z}`) into a table keyed by `Carrier` enum value with `Vec3f` values.
    - param `vehicleIconOffsets`: Offsets keyed by carrier name; names that are not a `Carrier` enum value are skipped.
    - returns: Offsets keyed by `api.type.enum.Carrier` value, ready for the `vehicleIconOffsets` of the HUD icon manager config.
    - used 13x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/subventions_gui.tl:442`: `local vehicleIconOffsets = hud_react_util.makeVehicleIconOffsets(hudGuiConfig.vehicleIconOffsets)`

### HudReactUtil.LineViewerSnapFactors (record)

```teal
record LineViewerSnapFactors
```

Snap factors for the line viewer, passed as `maxSnapFactorView` and `maxSnapFactorWorld` to the line renderer config (see `gui/line_vehicle_mgmt/line_util.tl`).

- `view : number`: Value for `maxSnapFactorView` (default 0.05).
- `world : number`: Value for `maxSnapFactorWorld` (default 10.0).
