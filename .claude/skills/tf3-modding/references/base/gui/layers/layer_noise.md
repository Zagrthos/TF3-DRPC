# gui.layers.layer_noise

Definition file: `base/tealdef/gui/layers/layer_noise.d.tl` (in the game folder).

The "Noise" map layer (button `menu.layers.noiseButton`).

## LayerNoise (record)

```teal
global record LayerNoise
```
Load: `local layer_noise = ug_require "::/gui/layers/layer_noise.tl" as LayerNoise`

The "Noise" layer, which shows noise emission and the affected town buildings.

- `getConfig : function() : LayerConfig`  (call as `layer_noise.getConfig(…)`): Builds the layer config.
    - returns: A new `LayerConfig` with the averaged noise map, emitter coloring and town building impact coloring.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/town/town.tl:1004`: `layerConfig = layer_noise.getConfig()`
- `Legend : Recipe0`: Recipe of the legend (noise emission and affected buildings).
  In game: "Noise Emission", "Affected Buildings" (German: „Lärmemission“, „Betroffene Gebäude“)
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/town/town.tl:1003`: `legendRecipe = layer_noise.Legend`
- `Layer : Recipe<LayersButtonRidge.LayerContentParam>`: Recipe of the layer window content.
    - used 1x in the base game, e.g. `base/content/gui/gui/layers/layers_button_ridge.tl:458`: `content = layer_noise.Layer,`
- `getHudIconPreFilter : function() : {Engine.ComponentType}`  (call as `layer_noise.getHudIconPreFilter(…)`): Component types that get HUD icons in this layer.
    - returns: `TRANSPORT_VEHICLE` and `TOWN`.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_hud.tl:613`: `filter = layer_noise.getHudIconPreFilter()`
- `HudIconMaster : Recipe2<Builtin.HudIconParam, LayersButtonRidge.HudIconUserParam>`: HUD icon recipe of the layer; default icons for vehicles, notifications and the noise rating for towns.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_hud.tl:583`: `children[#children + 1] = layer_noise.HudIconMaster(param, layerHudIconUserParam)`
