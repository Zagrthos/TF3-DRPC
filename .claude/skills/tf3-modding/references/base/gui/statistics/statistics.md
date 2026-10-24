# gui.statistics.statistics

Definition file: `base/tealdef/gui/statistics/statistics.d.tl` (in the game folder).

The statistics window (lists of lines, vehicles, stations, towns, industries, warehouses and depots) and the parameter types of its tabs, loaded with `ug_require "/gui/statistics/statistics.tl"`.

## StatisticsRecipeParams (record)

```teal
global record StatisticsRecipeParams
```

Parameters the statistics window passes to the recipe of each tab (for example `gui/statistics/statistic_lines.tl`).

- `searchString : string`: Current text of the search field (`""` for none).
- `filterShowOnlyVisible : boolean`: Whether only objects visible in the 3D view should be listed.
- `filterShowCategories : {integer : boolean}`: Enabled category buttons, by index into the categories the tab declared.
- `initialSortColumn : {integer, boolean}`: Sort column to start with, as `{columnIndex, ascending}`; remembered while the game runs.
- `onSortColumnChange : function(columnIndex : integer, ascending : boolean)`: Called when the player changes the sort column, so the window can remember it.
- `filterIaComp : ReactRefWrap`: Reference to the filter bar's input action component; tabs use it as source of the keybinding hint for focusing the filters.
- `deleteLineFeedback : function(message : string, mode : string, dialogData : FeedbackListUtil.FeedbackDialogParam, id : number)`
- `declareFilterOnMount : function(categories : {Category}, onlyVisible : OnlyVisible)`: Tells the window which category buttons and which "show only visible" toggle the tab needs. Tabs call it on mount.
    - param `categories`: Category buttons of the tab.
    - param `onlyVisible`: Object kind for the "show only visible" toggle; `nil` for no toggle.

### StatisticsRecipeParams.TabKey (enum)

```teal
enum TabKey
```

Tab of the statistics window.

In game: "Statistics" (German: „Statistiken“)

- `"Line"`: Lines tab.
  In game: "Lines" (German: „Linien“)
- `"Vehicle"`: Vehicles tab.
  In game: "Vehicles" (German: „Fahrzeuge“)
- `"Station"`: Stations tab.
  In game: "Stations" (German: „Stationen“)
- `"Town"`: Towns tab.
  In game: "Towns" (German: „Städte“)
- `"Industry"`: Industries tab.
  In game: "Industries" (German: „Industrien“)
- `"Warehouse"`: Warehouses tab.
  In game: "Warehouses" (German: „Lager“)
- `"Depot"`: Depots tab.
  In game: "Depots" (German: „Depots“)

### StatisticsRecipeParams.Category (record)

```teal
record Category
```

A category toggle button in the filter bar of a tab.

- `imageFile : string`: Icon of the button.
- `tooltip : string`: Tooltip of the button.
- `carrier : Carrier`: Carrier the category stands for, used to filter the line display in the 3D view; `nil` for categories that are not carriers.

### StatisticsRecipeParams.OnlyVisible (enum)

```teal
enum OnlyVisible
```

Kind of objects named in the tooltip of the "show only visible" toggle of a tab.

- `"Lines"`: Lines.
  In game: "Lines" (German: „Linien“)
- `"Vehicles"`: Vehicles.
  In game: "Vehicles" (German: „Fahrzeuge“)
- `"Depots"`: Depots.
  In game: "Depots" (German: „Depots“)
- `"Stations"`: Stations.
  In game: "Stations" (German: „Stationen“)
- `"Industries"`: Industries.
  In game: "Industries" (German: „Industrien“)
- `"Towns"`: Towns.
  In game: "Towns" (German: „Städte“)
- `"Warehouses"`: Warehouses.
  In game: "Warehouses" (German: „Lager“)

## StatisticsRecipeApi (record)

```teal
global record StatisticsRecipeApi
```

API of the lines tab, used by the statistics window to draw the listed lines in the 3D view.

- `getVisualizeLines : function() : {Builtin.Type.LineVisualization}`: Returns the line visualizations of the lines currently listed (after filtering), or `nil`.

## Statistics (record)

```teal
global record Statistics
```
Load: `local statistics = ug_require "::/gui/statistics/statistics.tl" as Statistics`

The statistics module.

- `StatisticsEntryPoint : Recipe<StatisticsEntryPointParam>`: Invisible recipe that opens the statistics window as a tool on the `openStatisticsWindow` event and the configured input actions, and keeps the filter settings of the tabs between openings.
    - used 2x in the base game, e.g. `base/content/gui/gui/main/game.tl:253`: `local statisticsEntryPointRef = react.useNodeRef(statistics.StatisticsEntryPoint)`

### Statistics.StatisticsEntryPointParam (record)

```teal
record StatisticsEntryPointParam
```

Parameters of `StatisticsEntryPoint`.

- `gameCtx : GameContext`: The in-game GUI context; its tool stack is used to open the window.
- `statisticsInputActionMapping : {string : StatisticsRecipeParams.TabKey}`: Input action ids mapped to the tab they open, for example `["IA_SELECT_STATISTICS_LINES"] = "Line"`. Pressing the action of the open tab closes the window.
- `statisticsEnabled : {StatisticsRecipeParams.TabKey : boolean}`: Tabs that are shown. The base game disables the line, vehicle and station tabs in the map editor.
