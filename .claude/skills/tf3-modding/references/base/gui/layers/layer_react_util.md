# gui.layers.layer_react_util

Definition file: `base/tealdef/gui/layers/layer_react_util.d.tl` (in the game folder).

Shared building blocks of the map layers: gradient legend colors, town rating and demand HUD icons, filtered entity tooltips, legend containers and fallback colors for a `LayerConfig`.

## LayerReactUtil (record)

```teal
global record LayerReactUtil
```
Load: `local layer_react_util = ug_require "::/gui/layers/layer_react_util.tl" as LayerReactUtil`

Shared helpers of the map layers.

- `makeGradientColorsOf2 : function(GradientColorsOf2) : {GradientColor}`  (call as `layer_react_util.makeGradientColorsOf2(…)`): Interpolates `numberOfLevels` colors from `colorA` to `colorB` for a `builtin.GradientLegend`.
    - returns: List of colors with labels; only the first and last step are labelled unless `iterativeLabels` is set.

    ```lua
    -- from gui/layers/layer_noise.tl
    builtin.GradientLegend{
    	colors = layer_react_util.makeGradientColorsOf2{
    		colorA = averageColors[1],
    		colorB = emissionMinMaxColor[2],
    		numberOfLevels = emission_config.pollutionNumSteps,
    		labelLeft = pGetText("layer-rating-label", "Low"),
    		labelRight = pGetText("layer-rating-label", "High"),
    	},
    	horizontal = true,
    	discrete = true,
    	internalLabels = false,
    },
    ```

    - used 3x in the base game, e.g. `base/content/gui/gui/entity_window/town/town.tl:96`: `colors = layer_react_util.makeGradientColorsOf2({`
- `makeGradientColorsOfN : function(GradientColorsOfN) : {GradientColor}`  (call as `layer_react_util.makeGradientColorsOfN(…)`): Interpolates colors through all entries of `colors` for a `builtin.GradientLegend`.
    - returns: List of colors with labels; the list ends with the last color exactly.
- `buildMouseEmissionInfo : function(Engine.Component.EmissionGrid.Type) : TreeNodeId`  (call as `layer_react_util.buildMouseEmissionInfo(…)`): Builds a "Mouse Position Info" checkbox that, when ticked, shows the current and average emission at the mouse position, raw and in dB.
    - notes: Must be called inside a recipe function (uses React state and `onStep`).
- `TownRatingHudIcon : Recipe2<string, Engine.Entity>`: HUD icon recipe that shows a town's name and a rating bar for one rating; parameters are the rating key and the town entity.
    - notes: Rating keys used by the base game: `"traffic_congestion"`, `"people_happiness"`, `"cargo_delivery"`, `"noise"`, `"pollution"` (from `town_util.GetRatings()`). Returns nothing for entities without a `TOWN` component.
- `TownDemandsHudIcon : Recipe<Engine.Entity>`: HUD icon recipe that shows a town's name and its residential, industrial and commercial cargo supply against demand.
- `makeFilteredSelectorTooltips : function(selectedEntityRef : ReactRefT<Engine.Entity>, preFilter : {Engine.ComponentType}) : TreeNodeId`  (call as `layer_react_util.makeFilteredSelectorTooltips(…)`): Creates an action tooltip with the default entity tooltip, limited to entities with one of the given component types.
    - param `selectedEntityRef`: Reference that holds the hovered or selected entity.
    - param `preFilter`: Component types to show tooltips for, usually the layer's `getHudIconPreFilter()`.
- `LayerLegendWrapper : Recipe<LayerLegendWrapperParam>`: Box layout around a layer's legend or window content; all layer recipes wrap their content with it.
- `LayerLegendFakeWindow : Recipe<LayerLegendFakeWindowParam>`: Shows a layer legend styled like a window (class `fake-builtin-window`) without being a window; the town window uses it for its layer views.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/town/town.tl:147`: `item = layer_react_util.LayerLegendFakeWindow(param),`
- `createDefaultFallbackColor : function(surface : GResColor, details : GResColor, overwriteMapSurface? : boolean, overwriteMapDetails? : boolean) : LayerConfig.ColorPassFn.FallbackColor`  (call as `layer_react_util.createDefaultFallbackColor(…)`): Creates the fallback color of a layer's color pass, used for everything that no specific painter colors.
    - param `surface`: Color for terrain, water and other surfaces.
    - param `details`: Color for edges, nodes and parallel strips.
    - param `overwriteMapSurface`: Passed to `BaseColor.fromColor` for the surface color.
    - param `overwriteMapDetails`: Passed to `BaseColor.fromColor` for the details color.
    - returns: A new `LayerConfig.ColorPassFn.FallbackColor`.

    ```lua
    colorPassFn.fallbackColor = layer_react_util.createDefaultFallbackColor(layerColors.Noise.Fallback.Surface, layerColors.Noise.Fallback.Details)
    ```

    - used 8x in the base game, e.g. `base/content/gui/gui/construction/construction_react_util.tl:610`: `colorPassFn.fallbackColor = layer_react_util.createDefaultFallbackColor(layerColors.Construction.Fallback.Surface, layerColors.Construction.Fallback.Details)`
- `ColorIndicator : Recipe<string>`: Small color swatch (style class `color-indicator-inner`) for legends; the pollution legend uses it with classes `odd` and `event`.

### LayerReactUtil.GradientColorsOf2 (record)

```teal
record GradientColorsOf2
```

Input of `makeGradientColorsOf2`, a gradient between two colors.

- `colorA : Vec4f`: Color of the first step.
- `colorB : Vec4f`: Color of the last step.
- `numberOfLevels : integer`: Number of steps.
- `labelLeft : string`: Label of the first step (default "Low").
- `labelRight : string`: Label of the last step (default "High").
- `iterativeLabels : boolean`: If true, every step is labelled with its number instead of using `labelLeft` and `labelRight`.

### LayerReactUtil.GradientColorsOf3 (record)

```teal
record GradientColorsOf3
```

Gradient definition with three colors; no function in the base game takes it.

- `colorA : Vec4f`
- `colorB : Vec4f`
- `colorC : Vec4f`
- `numberOfLevels : integer`
- `labelLeft : string`
- `labelRight : string`
- `iterativeLabels : boolean`

### LayerReactUtil.GradientColorsOfN (record)

```teal
record GradientColorsOfN
```

Input of `makeGradientColorsOfN`, a gradient through several colors.

- `colors : {Vec4f}`: Colors to interpolate between, in order.
- `numberOfLevels : integer`: Total number of steps; split evenly between each pair of neighbouring colors.
- `labels : {string}`: Optional labels, one per color, placed on the step where that color starts.

### LayerReactUtil.GradientColor (record)

```teal
record GradientColor
```

One step of a legend gradient.

- `color : Vec4f`: Color of the step.
- `label : string`: Label of the step (may be empty).

### LayerReactUtil.LayerLegendWrapperParam (record)

```teal
record LayerLegendWrapperParam
```

Parameter of `LayerLegendWrapper`.

- `content : TreeNodeId`: Node to wrap.

### LayerReactUtil.LayerLegendFakeWindowParam (record)

```teal
record LayerLegendFakeWindowParam
```

Parameter of `LayerLegendFakeWindow`.

- `title : string`: Optional title shown above the legend.
- `legendRecipe : Recipe0`: Legend recipe to show, e.g. `LayerTraffic.Legend`.
