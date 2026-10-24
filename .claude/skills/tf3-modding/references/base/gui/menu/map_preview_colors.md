# gui.menu.map_preview_colors

Definition file: `base/tealdef/gui/menu/map_preview_colors.d.tl` (in the game folder).

Colour resource for the town and industry markers of the map preview (`MapPreviewComp`) in the new-game menu.

## MapPreviewColors (record)

```teal
global record MapPreviewColors
```

Colours of the town and industry markers in the map preview (`MapPreviewComp`).

- `TownMarker : GResColor`: Colour of town markers (white in the base game, drawn with low transparency).
- `IndustryMarker : GResColor`: Colour of industry markers (goldenrod in the base game, drawn with low transparency).
- `TownMarkerExisting : GResColor`: Colour of town markers with the `existing` style class (gray in the base game).
- `IndustryMarkerExisting : GResColor`: Colour of industry markers with the `existing` style class (walnut in the base game).
