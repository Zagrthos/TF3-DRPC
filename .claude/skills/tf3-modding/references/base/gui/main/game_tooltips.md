# gui.main.game_tooltips

Definition file: `base/tealdef/gui/main/game_tooltips.d.tl` (in the game folder).

Tooltip recipes for the 3D view, loaded with `ug_require "/gui/main/game_tooltips.tl"`.

## GameTooltips (record)

```teal
global record GameTooltips
```
Load: `local tooltips = ug_require "::/gui/main/game_tooltips.tl" as GameTooltips`

Tooltip recipes for hovered entities and terrain in the 3D view.

- `DefaultEntityToolTip : Recipe<DefaultEntityTooltipParam>`: Tooltip with the name of the hovered entity and the explanation texts of its persisting notifications.
  In game: "Crossing", "Rail Crossing", "Headquarters" (German: „Kreuzung“, „Bahnübergang“, „Hauptquartier“)
    - notes: The name is the entity name, or for unnamed entities a description such as the owning vehicle (carriages), "Crossing", "Rail Crossing", the bridge or tunnel type, "Headquarters", a fun element type or the model's `metadata.entityTooltip`; otherwise the name of the owning construction. The world entity gets no tooltip.
    - used 8x in the base game, e.g. `base/content/game_mechanics/game_mechanics/emission/pollution_cleanup_facility_hud.script.tl:106`: `recipe = tooltips.DefaultEntityToolTip,`
- `TerrainTooltip : Recipe<TerrainTooltipParam>`: Tooltip that shows the terrain height, noise or pollution value under the mouse, used by the terrain, noise and pollution layers.
    - notes: The terrain value is shown only if `filter` is set and the hovered entity does not pass it; otherwise the `DefaultEntityToolTip` is shown instead.
    - used 3x in the base game, e.g. `base/content/gui/gui/layers/layer_noise.tl:207`: `recipe = game_tooltips.TerrainTooltip,`

### GameTooltips.DefaultEntityTooltipParam (interface)

```teal
interface DefaultEntityTooltipParam
```

Parameters of the entity tooltip.

- `entityRef : ReactRefT<Engine.Entity>`: Reference holding the hovered entity (typically the one the selector writes on hover); `-1` or `nil` for none.
- `filter : function(entity : Engine.Entity) : boolean`: Optional filter; entities for which it returns `false` get no tooltip.

### GameTooltips.TerrainTooltipMode (enum)

```teal
enum TerrainTooltipMode
```

Value the terrain tooltip shows.

- `"Height"`: Terrain altitude at the mouse position.
- `"Noise"`: Noise level at the mouse position, as a step index of the noise layer.
- `"Pollution"`: Pollution level at the mouse position, as a step index of the pollution layer.

### GameTooltips.TerrainTooltipParam (record)

```teal
record TerrainTooltipParam is DefaultEntityTooltipParam
```

Parameters of the terrain tooltip.

- `mode : TerrainTooltipMode`: Value to show.
