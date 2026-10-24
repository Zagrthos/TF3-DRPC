# gui.main.input_action_colors

Definition file: `base/tealdef/gui/main/input_action_colors.d.tl` (in the game folder).

Colours of the four face buttons of a gamepad, as a GUI resource (`::/gui/main/input_action_colors.gres`), used for key binding hints.

## InputActionColors (record)

```teal
global record InputActionColors
```

Gamepad face-button colours; read with `api.gui.genericRep.get(api.gui.genericRep.find("::/gui/main/input_action_colors.gres")).data`.

- `Up : GResColor`: Colour of the upper face button (yellow, Xbox Y).
- `Right : GResColor`: Colour of the right face button (red, Xbox B).
- `Down : GResColor`: Colour of the lower face button (green, Xbox A).
- `Left : GResColor`: Colour of the left face button (blue, Xbox X).
