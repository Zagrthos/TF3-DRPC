# gui.main.selection_colors

Definition file: `base/tealdef/gui/main/selection_colors.d.tl` (in the game folder).

Colours of the selection highlight in the 3D view, as a GUI resource (`::/gui/main/selection_colors.gres`).

## SelectionGuiConfig (record)

```teal
global record SelectionGuiConfig
```

Selection highlight colour sets; read with `api.gui.genericRep.get(api.gui.genericRep.find("::/gui/main/selection_colors.gres")).data`.

- `default : ColorConfig`: Normal colours (white outline).
- `inverted : ColorConfig`: Dark colours (black outline), used for example by the terrain layer and by `makeDefaultSelector` with `invertedSelectionColors`.
- `trafficLight : ColorConfig`: Colours for selecting signals and switches in the entity window (only fill and outline, both invisible).

### SelectionGuiConfig.ColorConfig (record)

```teal
record ColorConfig
```

One set of selection highlight colours.

- `selectionColor : GResColor`: Fill colour of the selected object (invisible in the base sets).
- `selectionOutlineColor : GResColor`: Outline colour of the selected object.
- `selectionCircleColor : GResColor`: Colour of the selection circle.
- `crosshairColor : GResColor`: Colour of the crosshair.
