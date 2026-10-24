# game_mechanics.finance.finances_react_util

Definition file: `base/tealdef/game_mechanics/finance/finances_react_util.d.tl` (in the game folder).

Chart, legend and slider recipes of the finances window.

## FinancesReactUtil (record)

```teal
global record FinancesReactUtil
```
Load: `local finances_react_util = ug_require "::/game_mechanics/finance/finances_react_util.tl" as FinancesReactUtil`

Recipes of the finances window.

- `FinancesChart : Recipe<FinancesChartParam>`: Chart recipe.
    - used 4x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_assets.tl:389`: `local chart = finances_react_util.FinancesChart{`
- `FinancesChartLegend : Recipe<FinancesChartLegendParam>`: Legend recipe.
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_charts.tl:157`: `finances_react_util.FinancesChartLegend{`
- `FinancesCargoLegend : Recipe<FinancesCargoLegendParam>`: Cargo legend recipe.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_assets.tl:444`: `finances_react_util.FinancesCargoLegend{`
- `FinancesSlider : Recipe<FinancesSliderParam>`: Slider recipe.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_charts.tl:163`: `finances_react_util.FinancesSlider{`

### FinancesReactUtil.FinancesChartParam (record)

```teal
record FinancesChartParam
```

Params of the finances chart.

- `chartData : ChartResult`: Series to draw.
- `labels : {string}`: Series labels.
- `colors : {Vec4f}`: Series colours.
- `chartType : Builtin.SeriesParam.Type`: Chart type of the series.
- `lineWidth : number`: Line width.
- `xFormatter : (function(number) : string)`: Formats x-axis labels.
- `yFormatter : (function(number) : string)`: Formats y-axis labels.

### FinancesReactUtil.FinancesChartLegendParam (record)

```teal
record FinancesChartLegendParam
```

Params of the chart legend.

- `chartData : ChartResult`: Series of the chart.
- `labels : {string}`: Series labels.
- `colors : {Vec4f}`: Series colours.

### FinancesReactUtil.FinancesCargoLegendParam (record)

```teal
record FinancesCargoLegendParam
```

Params of the cargo legend, whose check boxes show or hide series.

- `chartData : ChartResult`: Series of the chart.
- `labels : {string}`: Series labels.
- `colors : {Vec4f}`: Series colours.
- `icons : {string}`: Cargo icons.
- `logNames : {string}`: Log name of each series, passed to `setVisible`.
- `yFormatter : function(number) : string`: Formats values.
- `setVisible : function(logName : string, show : boolean)`: Shows or hides a series.
    - param `logName`: Log name of the series.
    - param `show`: `true` to show.

### FinancesReactUtil.FinancesSliderParam (record)

```teal
record FinancesSliderParam
```

Params of the slider with text before and after it.

- `value : integer`: Current value.
- `min : integer`: Minimum.
- `max : integer`: Maximum.
- `onValueChange : (function(integer) : string)`: Called with the new value.
- `prefix : string`: Text before the slider.
- `postfix : string`: Text after the slider.
