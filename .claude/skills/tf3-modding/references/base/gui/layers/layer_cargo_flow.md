# gui.layers.layer_cargo_flow

Definition file: `base/tealdef/gui/layers/layer_cargo_flow.d.tl` (in the game folder).

The "Cargo Flow" map layer (button `menu.layers.cargoFlowButton`).

## LayerCargoFlow (record)

```teal
global record LayerCargoFlow
```
Load: `local layer_cargo_flow = ug_require "::/gui/layers/layer_cargo_flow.tl" as LayerCargoFlow`

The "Cargo Flow" layer, which shows where cargo is supplied and demanded.

- `getConfig : function(includePassengers : boolean) : LayerConfig`  (call as `layer_cargo_flow.getConfig(…)`): Builds the layer config.
    - param `includePassengers`: Include passengers in the coloring and the cargo edges; the layer window passes `false`.
    - returns: A new `LayerConfig`.
- `Legend : Recipe0`: Recipe of the layer legend.
  In game: "Cargo Transport Rate", "Commercial Supply" (German: „Gütertransportrate“, „Versorgung Gewerbegebiet“)
- `Layer : Recipe<LayersButtonRidge.LayerContentParam>`: Recipe of the layer window content.
    - used 1x in the base game, e.g. `base/content/gui/gui/layers/layers_button_ridge.tl:436`: `content = layer_cargo_flow.Layer,`
- `getHudIconPreFilter : function() : {Engine.ComponentType}`  (call as `layer_cargo_flow.getHudIconPreFilter(…)`): Component types that get HUD icons in this layer.
    - returns: `TRANSPORT_VEHICLE`, `TOWN`, `INDUSTRY` and `WAREHOUSE`.
- `HudIconMaster : Recipe2<Builtin.HudIconParam, LayersButtonRidge.HudIconUserParam>`: HUD icon recipe of the layer; shows supply and demand of town districts, industry and warehouse stocks, and vehicle cargo.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_hud.tl:569`: `children[#children + 1] = layer_cargo_flow.HudIconMaster(param, layerCargoFlowHudIconUserParam)`

### LayerCargoFlow.HudIconUserParam (record)

```teal
record HudIconUserParam is LayersButtonRidge.HudIconUserParam
```

User parameter of `LayerCargoFlow.HudIconMaster`.

- `includePassengers : boolean`: For town districts, include passenger demand from residential line usage.
- `districtsDemandsShowPercentage : boolean`: Show district demands as percentages.
