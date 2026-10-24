# gui.layers.layer_public_transport

Definition file: `base/tealdef/gui/layers/layer_public_transport.d.tl` (in the game folder).

The "Passenger Satisfaction" map layer (button `menu.layers.publicTransportButton`).

## LayerPublicTransport (record)

```teal
global record LayerPublicTransport
```
Load: `local layer_public_transport = ug_require "::/gui/layers/layer_public_transport.tl" as LayerPublicTransport`

The "Passenger Satisfaction" layer, which colors stations, waiting passengers and network edges by passenger satisfaction.

- `getConfig : function() : LayerConfig`  (call as `layer_public_transport.getConfig(…)`): Builds the layer config.
    - returns: A new `LayerConfig`.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/town/town.tl:996`: `layerConfig = layer_public_transport.getConfig()`
- `Legend : Recipe0`: Recipe of the layer legend.
  In game: "Happiness" (German: „Zufriedenheit“)
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/town/town.tl:995`: `legendRecipe = layer_public_transport.Legend`
- `Layer : Recipe<LayersButtonRidge.LayerContentParam>`: Recipe of the layer window content.
    - used 1x in the base game, e.g. `base/content/gui/gui/layers/layers_button_ridge.tl:425`: `content = layer_public_transport.Layer,`
- `getHudIconPreFilter : function() : {Engine.ComponentType}`  (call as `layer_public_transport.getHudIconPreFilter(…)`): Component types that get HUD icons in this layer.
    - returns: `STATION_GROUP`, `TRANSPORT_VEHICLE` and `TOWN`.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_hud.tl:609`: `filter = layer_public_transport.getHudIconPreFilter()`
- `HudIconMaster : Recipe2<Builtin.HudIconParam, LayersButtonRidge.HudIconUserParam>`: HUD icon recipe of the layer; town happiness rating, passengers in vehicles and unhappy waiting passengers at stations.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_hud.tl:573`: `children[#children + 1] = layer_public_transport.HudIconMaster(param, layerHudIconUserParam)`
