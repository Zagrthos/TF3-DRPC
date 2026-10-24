# gui.entity_window.town.town_eow

Definition file: `base/tealdef/gui/entity_window/town/town_eow.d.tl` (in the game folder).

Plugin types and charts of the town window (`town_eow.script.tl`).

## TownEow (record)

```teal
global record TownEow
```
Load: `local town_eow = ug_require "::/gui/entity_window/town/town_eow.script.tl" as TownEow`

Plugin types and charts of the town window.

- `TownCharts : Recipe<TownChartsParam>`: Chart of a town's logbook for one information tab, e.g. residential, commercial and industrial counts on "Overview"; can be selected and inspected with the gamepad.
  In game: "District Sizes", "Reputation", "Traffic Rating" (German: „Gebietsgrössen“, „Ruf“, „Verkehrsbewertung“)
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/town/town.tl:970`: `(activeTabState:old() ~= "Bonuses") and town_eow.TownCharts{`
- `TownEowExtensionPoint : ExtensionPoint<TownWidgetPluginParams>`: Extension point of the town window's main tab; register plugins with a resource of type `"react-plugin ::TownEowExtensionPoint"`.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/town/town.tl:1272`: `local pluginWidgets = gui_react_util.usePlugins(town_eow.TownEowExtensionPoint, {`
- `TownEowEditorExtensionPoint : ExtensionPoint<TownWidgetPluginParams>`: Extension point of the town window's editor tab; the tab is only shown if at least one plugin is active. Register plugins with a resource of type `"react-plugin ::TownEowEditorExtensionPoint"`.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/town/town.tl:1289`: `local editorTabChildren : {TreeNodeId} = gui_react_util.usePlugins(town_eow.TownEowEditorExtensionPoint, {}, extensionParams)`
- `NotificationPlugin : Recipe<TownWidgetPluginParams>`: Notification cards of the town; registered with order 0.
- `TownLevelPlugin : Recipe<TownWidgetPluginParams>`: Card with town level, population, progress towards the next level and growth state, with the population widget as collapsible part; registered with order 10.
  In game: "Town Development Inactive", "Current Population", "{percentage} Towards {nextTownLevelName}" (German: „Stadtentwicklung inaktiv“, „Aktuelle Bevölkerung“, „{percentage} zu {nextTownLevelName}“)
- `conditionTownCharts : function(params : TownEow.TownWidgetPluginParams) : boolean`  (call as `town_eow.conditionTownCharts(…)`): Plugin condition; true if the district sizes are visible and the window is not in the map editor.
- `TownChartsPlugin : Recipe<TownWidgetPluginParams>`: "District Sizes" chart card; registered with order 40 and condition `conditionTownCharts`.
  In game: "District Sizes" (German: „Gebietsgrössen“)
- `isMapEditor : function(params : TownEow.TownWidgetPluginParams) : boolean`  (call as `town_eow.isMapEditor(…)`): Plugin condition; true in the map editor.
- `MapEditorTownSuppliesPlugin : Recipe<TownWidgetPluginParams>`: "Supplies" card with the editable town supplies in the map editor; registered with order 50 and condition `isMapEditor`.
  In game: "Supplies" (German: „Versorgung“)
- `conditionTownDebugGraph : function(params : TownEow.TownWidgetPluginParams) : boolean`  (call as `town_eow.conditionTownDebugGraph(…)`): Plugin condition of the debug graph; always returns false in the base game.
- `TownDebugGraphPlugin : Recipe<TownWidgetPluginParams>`: "Debug Graph" card with charts of all town rating logs; registered with order 70, but disabled by `conditionTownDebugGraph`.
- `isSandboxMode : function(params : TownEow.TownWidgetPluginParams) : boolean`  (call as `town_eow.isSandboxMode(…)`): Plugin condition; true in sandbox mode.
- `TownEditorCargoDemandPlugin : Recipe<TownWidgetPluginParams>`: "Cargo Demand" editor card; registered on the editor extension point with order 10 and condition `isSandboxMode`.
  In game: "Cargo Demand", "Residential District" (German: „Güterbedarf“, „Wohngebiet“)
- `TownEditorInitialCapacityPlugin : Recipe<TownWidgetPluginParams>`: "Target District Capacities" editor card; registered on the editor extension point with order 20 and condition `isSandboxMode`.
  In game: "Target District Capacities", "Enable Town Development" (German: „Angestrebte Gebietskapazitäten“, „Stadtentwicklung aktivieren“)
- `TownEditorDistributionWeightsPlugin : Recipe<TownWidgetPluginParams>`: "District Tool" editor card; registered on the editor extension point with order 30 and condition `isSandboxMode`.
  In game: "District Tool", "Enable Custom Distribution Weights" (German: „Gebiets-Werkzeug“, „Benutzerdefinierte Gewichtungsverteilung aktivieren“)
- `TownEditorRatingsSensitivityPlugin : Recipe<TownWidgetPluginParams>`: "Ratings Sensitivity" editor card; registered on the editor extension point with order 40 and condition `isSandboxMode`.
  In game: "Ratings Sensitivity", "Reset to Default" (German: „Bewertungsempfindlichkeit“, „Auf Standard zurücksetzen“)

### TownEow.TownEowState (record)

```teal
record TownEowState
```

State of the town window passed to the plugins.

- `isSandboxMode : boolean`: True in sandbox mode; enables the editor plugins.
- `isMapEditor : boolean`: True in the map editor.
- `districtSizesVisible : boolean`: Whether the town information widget currently shows the district sizes chart.

### TownEow.TownWidgetPluginParams (record)

```teal
record TownWidgetPluginParams is EowExtensionUtil.IEowWidgetsExtensionParams
```

Parameters passed to the plugins of the town window.

- `state : TownEowState`: Window state.

### TownEow.TownEditorWidgetPluginParams (record)

```teal
record TownEditorWidgetPluginParams is EowExtensionUtil.IEowWidgetsExtensionParams
```

Parameters for the plugins of the town editor tab.

- `state : TownEowState`: Window state.

### TownEow.TownChartsParam (record)

```teal
record TownChartsParam
```

Parameters of `TownEow.TownCharts`.

- `entityId : Engine.Entity`: Town entity.
- `tabType : TownReactUtil.TownInfoTabType`: Town information tab whose chart is shown (e.g. `"Overview"` for district sizes, `"Reputation"`, `"Supplies"`, `"Traffic"`, `"Happiness"`).
- `chartStepTimer : boolean`
- `styleClass : string`: Style class of the chart.
- `showTimeScaleSlider : boolean`: Shows the time scale slider (not used for every tab).
- `legendOrientation : Builtin.Type.Orientation`: Orientation of the chart widget's layout.
