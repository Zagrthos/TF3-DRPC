# gui.layers.layer_pollution

Definition file: `base/tealdef/gui/layers/layer_pollution.d.tl` (in the game folder).

The "Pollution" map layer (button `menu.layers.pollutionButton`).

## LayerPollution (record)

```teal
global record LayerPollution
```
Load: `local layer_pollution = ug_require "::/gui/layers/layer_pollution.tl" as LayerPollution`

The "Pollution" layer, which shows pollution emission, the averaged pollution map, wind and town areas.

- `getConfig : function() : LayerConfig`  (call as `layer_pollution.getConfig(…)`): Builds the layer config; the pollution cleanup facility also uses it.
    - returns: A new `LayerConfig`.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/emission/pollution_cleanup_facility_layer.script.tl:5`: `return layerPollution.getConfig()`
- `Legend : Recipe0`: Recipe of the layer legend.
  In game: "Pollution Emission", "Pollution Absorbtion", "Affected Region" (German: „Verschmutzungsemission“, „Verschmutzungsabsorption“, „Betroffene Region“)
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/town/town.tl:1007`: `legendRecipe = layer_pollution.Legend`
- `Layer : Recipe<LayersButtonRidge.LayerContentParam>`: Recipe of the layer window content.
    - used 1x in the base game, e.g. `base/content/gui/gui/layers/layers_button_ridge.tl:469`: `content = layer_pollution.Layer,`
- `getHudIconPreFilter : function() : {Engine.ComponentType}`  (call as `layer_pollution.getHudIconPreFilter(…)`): Component types that get HUD icons in this layer.
    - returns: `TRANSPORT_VEHICLE` and `TOWN`.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_hud.tl:615`: `filter = layer_pollution.getHudIconPreFilter()`
- `HudIconMaster : Recipe2<Builtin.HudIconParam, LayersButtonRidge.HudIconUserParam>`: HUD icon recipe of the layer; default icons for vehicles, notifications and the pollution rating for towns.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_hud.tl:585`: `children[#children + 1] = layer_pollution.HudIconMaster(param, layerHudIconUserParam)`
