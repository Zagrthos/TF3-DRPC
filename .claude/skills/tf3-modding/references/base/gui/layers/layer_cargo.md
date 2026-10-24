# gui.layers.layer_cargo

Definition file: `base/tealdef/gui/layers/layer_cargo.d.tl` (in the game folder).

The "Cargo Satisfaction" map layer (button `menu.layers.cargoButton`).

## LayerCargo (record)

```teal
global record LayerCargo
```
Load: `local layer_cargo = ug_require "::/gui/layers/layer_cargo.tl" as LayerCargo`

The "Cargo Satisfaction" layer, which colors warehouses and network edges by cargo satisfaction.

- `getConfig : function(cargoIndex : CargoTypeId) : LayerConfig`  (call as `layer_cargo.getConfig(…)`): Builds the layer config for one cargo type or for all cargo types.
    - param `cargoIndex`: Cargo type id, or `-1` for all cargo types except passengers.
    - returns: A new `LayerConfig` with warehouse coloring, satisfaction edges, signs and town areas.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/town/town.tl:1000`: `layerConfig = layer_cargo.getConfig(-1)`
- `Legend : Recipe0`: Recipe of the layer legend.
  In game: "Delivery Time" (German: „Lieferzeit“)
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/town/town.tl:999`: `legendRecipe = layer_cargo.Legend`
- `Layer : Recipe<LayersButtonRidge.LayerContentParam>`: Recipe of the layer window content, with a cargo type selector.
  In game: "Cargo Filter", "All Cargo" (German: „Güterfilter“, „Alle Güter“)
    - used 1x in the base game, e.g. `base/content/gui/gui/layers/layers_button_ridge.tl:447`: `content = layer_cargo.Layer,`
- `HudIconMaster : Recipe2<Builtin.HudIconParam, HudIconUserParam>`: HUD icon recipe of the layer; shows town cargo ratings, the matching cargo of vehicles and warehouse stocks.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_hud.tl:581`: `children[#children + 1] = layer_cargo.HudIconMaster(param, cargoLayerHudIconUserParam)`
- `getHudIconPreFilter : function() : {Engine.ComponentType}`  (call as `layer_cargo.getHudIconPreFilter(…)`): Component types that get HUD icons in this layer.
    - returns: `TRANSPORT_VEHICLE`, `TOWN` and `WAREHOUSE`.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_hud.tl:611`: `filter = layer_cargo.getHudIconPreFilter()`

### LayerCargo.HudIconUserParam (record)

```teal
record HudIconUserParam is LayersButtonRidge.HudIconUserParam
```

User parameter of `LayerCargo.HudIconMaster`.

- `getSelectedCargoTypeId : function() : CargoTypeId`: Returns the cargo type selected in the layer window, or `-1` for all.
