# gui.layers.layer_hud_filter

Definition file: `base/tealdef/gui/layers/layer_hud_filter.d.tl` (in the game folder).

The "HUD Filters" layer window, where the player hides HUD icons per building type and per vehicle carrier.

## HUDFilter (record)

```teal
global record HUDFilter
```
Load: `local layer_hud_filter = ug_require "::/gui/layers/layer_hud_filter.tl" as HUDFilter`

The "HUD Filters" layer window.

Notes:

The filters are stored in the GUI save data (`hudFilter`: `preFilters` per component type, `preFilterTvCarriers`
per carrier, plus `showCosts`, `showIncome`, `showEventIcons`, `showCargoFlow`).

- `Layer : Recipe<LayersButtonRidge.LayerContentParam>`: Recipe of the window content with toggles for stations, industries, warehouses, towns, landmarks and each vehicle carrier.
  In game: "Buildings", "Vehicles", "Miscellaneous" (German: „Gebäude“, „Fahrzeuge“, „Verschiedenes“)
    - used 1x in the base game, e.g. `base/content/gui/gui/layers/layers_button_ridge.tl:481`: `content = layer_hud_filter.Layer,`
