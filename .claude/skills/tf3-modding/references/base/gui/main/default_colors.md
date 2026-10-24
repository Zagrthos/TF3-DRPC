# gui.main.default_colors

Definition file: `base/tealdef/gui/main/default_colors.d.tl` (in the game folder).

The base colour palette of the GUI, from the resource `::/gui/main/default_colors.gres`.
Style sheets (`*.css.lua`) read it with
`api.gui.genericRep.get(api.gui.genericRep.find("::/gui/main/default_colors.gres")).data`.

## DefaultColors (record)

```teal
global record DefaultColors
```

Base GUI colour palette (neutral greys, the blue theme colours, status colours).

Notes:

The theme colours (`Base*`, `Accent*`) are generated in HSV from one hue (210, blue):
the `Base*` colours with low saturation and brightness rising from `BaseVeryDark` to
`BaseVeryLight`, the `Accent*` colours with higher saturation.


basic colors
- `Invisible : GResColor`: Fully transparent.
- `NeutralDarkest : GResColor`: Black.
- `NeutralVeryDark : GResColor`: Grey at 20 % brightness.
- `NeutralDark : GResColor`: Grey at 30 % brightness.
- `NeutralMedium : GResColor`: Grey at 50 % brightness.
- `NeutralLight : GResColor`: Grey at 70 % brightness.
- `NeutralVeryLight : GResColor`: Grey at 80 % brightness.
- `NeutralLightest : GResColor`: White.

theme
- `BaseVeryDark : GResColor`: Darkest theme background colour (blue-grey, hue 210).
- `BaseDark : GResColor`: Theme background colour, one step brighter than `BaseVeryDark`.
- `BaseDarkMedium : GResColor`: Theme background colour, two steps brighter than `BaseVeryDark`.
- `BaseMedium : GResColor`: Theme background colour, three steps brighter than `BaseVeryDark`.
- `BaseLightMedium : GResColor`: Theme background colour, four steps brighter than `BaseVeryDark`.
- `BaseLight : GResColor`: Theme background colour, five steps brighter than `BaseVeryDark`.
- `BaseVeryLight : GResColor`: Brightest theme background colour.
- `AccentVeryDark : GResColor`: Darkest accent colour of the theme.
- `AccentDark : GResColor`: Dark accent colour.
- `AccentMedium : GResColor`: Medium accent colour.
- `AccentLight : GResColor`: Light accent colour.
- `AccentVeryLight : GResColor`: Lightest accent colour.

(very base) game related colors
- `Ok : GResColor`: Success colour (green).
- `Warning : GResColor`: Warning colour (goldenrod).
- `Error : GResColor`: Error colour (red).
- `Info : GResColor`: Information colour (blue).
- `Highlight : GResColor`: Highlight colour (daffodil yellow).
- `Positive : GResColor`: Colour for positive values (light blue).
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/vehicle/vehicle_eow.script.tl:1206`: `color_util.toVec4(DefaultColors.Positive),`
- `Negative : GResColor`: Colour for negative values (light coral).
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/vehicle/vehicle_eow.script.tl:1207`: `color_util.toVec4(DefaultColors.Negative),`
- `Indifferent : GResColor`: Colour for neutral values (celadon green).
- `Debug : DebugColors`: Debug colours.

### DefaultColors.DebugColors (record)

```teal
record DebugColors
```

Extra colours for debugging and development.

- `Quirky : GResColor`: Grape purple.
- `Furious : GResColor`: Folly red.
- `Easy : GResColor`: Jordy blue.
- `Cynic : GResColor`: Daffodil yellow.
- `Hopeful : GResColor`: Soft lime green.
