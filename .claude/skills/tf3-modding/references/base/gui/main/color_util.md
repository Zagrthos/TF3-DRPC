# gui.main.color_util

Definition file: `base/tealdef/gui/main/color_util.d.tl` (in the game folder).

Colour types of GUI resource files (`*.gres`) and helpers that turn them into
`Vec3f`/`Vec4f` values, add transparency, mix colours and derive hover/active shades.

## GResColor (record)

```teal
global record GResColor is {number}
```

{number} is expected to have length 3 or 4

## GResColorRange (record)

```teal
global record GResColorRange
```

A pair of GUI resource colours (minimum and maximum), e.g. for a colour gradient.

- `min : GResColor`: Colour at the lower end of the range.
- `max : GResColor`: Colour at the upper end of the range.

## GResColorArray (record)

```teal
global record GResColorArray is {GResColor}
```

A list of GUI resource colours.

## GResTransparency (record)

```teal
global record GResTransparency
```

Standard alpha values of the GUI, from the resource `::/gui/main/transparency.gres`.

Notes:

Read it with `api.gui.genericRep.get(api.gui.genericRep.find("::/gui/main/transparency.gres")).data`
and pass the values as `alpha` to `color_util.withTransparency`.

- `__index : function(GResTransparency, string) : number`: Lookup of the alpha values by name (`transparency["Low"]`).
- `VeryLow : number`: Alpha 0.9 (almost opaque).
- `Low : number`: Alpha 0.75.
- `Medium : number`: Alpha 0.5.
- `High : number`: Alpha 0.25.
- `VeryHigh : number`: Alpha 0.1 (almost transparent).

## ColorUtil (record)

```teal
global record ColorUtil
```
Load: `local color_util = require "::/gui/main/color_util.tl" as ColorUtil`

Conversion and mixing helpers for GUI resource colours (`GResColor`, a list of 3 or 4 numbers in 0..1).

- `toVec3 : function(color : GResColor) : Vec3f`  (call as `color_util.toVec3(…)`): Converts a colour to a `Vec3f` (red, green, blue); alpha is dropped.
    - used 5x in the base game, e.g. `base/content/gui/gui/main/particle_react_util.tl:19`: `api.type.ParticleParamCurveVec3f.CurvePoint.new(0.0, color_util.toVec3(colors.LevelUpStars)),`
- `toVec4 : function(color : GResColor) : Vec4f`  (call as `color_util.toVec4(…)`): Converts a colour to a `Vec4f`; a missing alpha becomes 1.0.
    - used 163x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_charts.tl:199`: `colors = { color_util.toVec4(defaultColorsRef:get().Positive), color_util.toVec4(defaultColorsRef:get().Negative) },`
- `toRange3 : function(colorRange : GResColorRange) : {Vec3f, Vec3f}`  (call as `color_util.toRange3(…)`): {min, max}
- `toRange4 : function(colorRange : GResColorRange) : {Vec4f, Vec4f}`  (call as `color_util.toRange4(…)`): {min, max}
    - used 11x in the base game, e.g. `base/content/gui/gui/entity_window/town/town.tl:54`: `local passengerTransportColor = color_util.toRange4(layerColors.PassengerFlow.PublicTransport)`
- `toArray3 : function(colorArray : GResColorArray) : {Vec3f}`  (call as `color_util.toArray3(…)`): Converts each colour of the list with `toVec3`.
    - used 7x in the base game, e.g. `base/content/gui/gui/construction/construction_react_util.tl:3326`: `laneModifier.lineColors = color_util.toArray3(api.gui.genericRep.get(api.gui.genericRep.find("::/gui/line_vehicle_mgmt/line_colors.gres")).data as GResColorArra`
- `toArray4 : function(colorArray : GResColorArray) : {Vec4f}`  (call as `color_util.toArray4(…)`): Converts each colour of the list with `toVec4`.
    - used 16x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_charts.tl:233`: `local chartColorsRef = react.useRef(color_util.toArray4(api.gui.genericRep.get(api.gui.genericRep.find("::/gui/main/chart_colors.gres")).data as GResColorArray)`
- `toArray4WithTransparency : function(colorArray : GResColorArray, alpha : number) : {Vec4f}`  (call as `color_util.toArray4WithTransparency(…)`): Converts each colour of the list to a `Vec4f` with the given alpha (the colour's own alpha is replaced).
    - used 6x in the base game, e.g. `base/content/gui/gui/entity_window/industry/industry.tl:332`: `local potentialColors = color_util.toArray4WithTransparency(layerColors.CargoFlow.Suppliers3D.Potential, transparency.VeryLow)`
- `withTransparency : function(gResColor : GResColor, alpha : number) : Vec4f`  (call as `color_util.withTransparency(…)`): Returns the colour as `Vec4f` with its alpha replaced by `alpha`.
    - used 30x in the base game, e.g. `base/content/gui/gui/construction/construction_desc_layers.script.tl:38`: `mapColors.water = color_util.withTransparency(layerColors.Terrain.Water, transparency.Low)`
- `withTransparencyRaw : function(gResColor : GResColor, alpha : number) : GResColor`  (call as `color_util.withTransparencyRaw(…)`): Returns a new `GResColor` with its alpha replaced by `alpha`.
    - used 323x in the base game, e.g. `base/content/game_mechanics/game_mechanics/achievements/achievements.css.lua:103`: `backgroundColor1 = color_util.withTransparencyRaw(`
- `mixColors : function(colorA : GResColor, colorB : GResColor, amount : number) : Vec4f`  (call as `color_util.mixColors(…)`): Linearly interpolates between two colours (including alpha) and returns a `Vec4f`.
    - param `amount`: 0 gives `colorA`, 1 gives `colorB`.
- `mixColorsRaw : function(colorA : GResColor, colorB : GResColor, amount : number) : GResColor`  (call as `color_util.mixColorsRaw(…)`): Like `mixColors`, but returns a `GResColor`.
    - used 3x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/line_vehicle_mgmt.css.lua:37`: `color = color_util.mixColorsRaw(`
- `getHoverFrom : function(enabledColor : GResColor) : Vec4f`  (call as `color_util.getHoverFrom(…)`): Derives the hover shade of a colour (brightness +0.2, slightly less saturation, in HSV) as `Vec4f`.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_react_util.tl:133`: `backgroundColor1 = color_util.getHoverFrom({color.x, color.y, color.z, color.w}),`
- `getHoverFromRaw : function(enabledColor : GResColor) : GResColor`  (call as `color_util.getHoverFromRaw(…)`): Like `getHoverFrom`, but returns a `GResColor`.
    - used 54x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company.css.lua:139`: `color_util.getHoverFromRaw(colorDefault.AccentLight),`
- `getActiveFrom : function(enabledColor : GResColor) : Vec4f`  (call as `color_util.getActiveFrom(…)`): Derives the pressed/active shade of a colour (brightness +0.1, slightly less saturation, in HSV) as `Vec4f`.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_react_util.tl:125`: `backgroundColor1 = color_util.getActiveFrom({color.x, color.y, color.z, color.w}),`
- `getActiveFromRaw : function(enabledColor : GResColor) : GResColor`  (call as `color_util.getActiveFromRaw(…)`): Like `getActiveFrom`, but returns a `GResColor`.
    - used 57x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company.css.lua:146`: `color_util.getActiveFromRaw(colorDefault.AccentLight),`
