# game_mechanics.towns.district_colors

Definition file: `base/tealdef/game_mechanics/towns/district_colors.d.tl` (in the game folder).

Colour ranges of the three district types, loaded from `district_colors.gres`.

## DistrictColors (record)

```teal
global record DistrictColors
```

Colour ranges (`min`, `max`) for residential, commercial and industrial districts. The town window styles, the finance charts and the cargo flow and civilization layers use them.

Example:
```lua
local districtColors = api.gui.genericRep.get(api.gui.genericRep.find("::/game_mechanics/towns/district_colors.gres")).data as DistrictColors
```

- `Commercial : GResColorRange`: Colour range of commercial districts (pale sky to true blue).
- `Residential : GResColorRange`: Colour range of residential districts (celadon to dark pastel green).
- `Industrial : GResColorRange`: Colour range of industrial districts (icterine to goldenrod).
