# gui.layers.layer_traffic

Definition file: `base/tealdef/gui/layers/layer_traffic.d.tl` (in the game folder).

The "Traffic" map layer (button `menu.layers.trafficButton`).

## LayerTraffic (record)

```teal
global record LayerTraffic
```
Load: `local layer_traffic = ug_require "::/gui/layers/layer_traffic.tl" as LayerTraffic`

The "Traffic" layer, which colors roads and vehicles by congestion.

- `getConfig : function() : LayerConfig`  (call as `layer_traffic.getConfig(…)`): Builds the layer config.
    - returns: A new `LayerConfig` that colors the network, transport vehicles and land vehicles by congestion (jammed, intermediate, fluid).
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/town/town.tl:992`: `layerConfig = layer_traffic.getConfig()`
- `Legend : Recipe0`: Recipe of the layer legend.
  In game: "Traffic Flow" (German: „Verkehrsfluss“)
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/town/town.tl:991`: `legendRecipe = layer_traffic.Legend`
- `Layer : Recipe<LayersButtonRidge.LayerContentParam>`: Recipe of the layer window content.
    - used 1x in the base game, e.g. `base/content/gui/gui/layers/layers_button_ridge.tl:403`: `content = layer_traffic.Layer,`
- `getHudIconPreFilter : function() : {Engine.ComponentType}`  (call as `layer_traffic.getHudIconPreFilter(…)`): Component types that get HUD icons in this layer.
    - returns: `TRANSPORT_VEHICLE` and `TOWN`.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_hud.tl:607`: `filter = layer_traffic.getHudIconPreFilter()`
- `HudIconMaster : Recipe2<Builtin.HudIconParam, LayersButtonRidge.HudIconUserParam>`: HUD icon recipe of the layer; default icons for vehicles, notifications and the traffic congestion rating for towns.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_hud.tl:571`: `children[#children + 1] = layer_traffic.HudIconMaster(param, layerHudIconUserParam)`
