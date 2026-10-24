# gui.statistics.statistics_react_util

Definition file: `base/tealdef/gui/statistics/statistics_react_util.d.tl` (in the game folder).

Helpers shared by the statistics tabs (filter bars, search field, cargo columns, problem column), loaded with `ug_require "/gui/statistics/statistics_react_util.tl"`.

## StatisticsReactUtil (record)

```teal
global record StatisticsReactUtil
```
Load: `local statistics_react_util = ug_require "::/gui/statistics/statistics_react_util.tl" as StatisticsReactUtil`

Helpers for the statistics tabs.

- `calculateCargoColumnData : function(cargoTypeInfos : {CargoUtil.CargoTypeInfo}) : CargoColumnData`  (call as `statistics_react_util.calculateCargoColumnData(…)`): Sums cargo type infos into column data. Values of `-1` (unknown) are skipped.
- `calculateCargoColumnDataForVehicle : function(vehicleEntity : Engine.Entity) : CargoColumnData`  (call as `statistics_react_util.calculateCargoColumnDataForVehicle(…)`): Calculates the column data of a vehicle from the cargo types it carries or can carry.
    - param `vehicleEntity`: Entity id of the vehicle.
    - used 8x in the base game, e.g. `base/content/gui/gui/statistics/statistic_vehicles.tl:110`: `return statistics_react_util.calculateCargoColumnDataForVehicle(entity)`
- `calculateCargoColumnDataForLine : function(lineEntity : Engine.Entity) : CargoColumnData`  (call as `statistics_react_util.calculateCargoColumnDataForLine(…)`): Calculates the column data of a line from the cargo types of its vehicles.
    - param `lineEntity`: Entity id of the line.
    - used 10x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:3943`: `return statistics_react_util.calculateCargoColumnDataForLine(params.lineEntity).demand`
- `getStationGroupType : function(Engine.Entity) : StationGroupType`  (call as `statistics_react_util.getStationGroupType(…)`): Determines whether a station group has passenger stations, cargo stations or both.
    - param `#1`: Entity id of the station group.
    - returns: The type, or `nil` if the group has neither.
    - used 3x in the base game, e.g. `base/content/gui/gui/statistics/statistic_stations.tl:97`: `return statistics_react_util.getStationGroupType(entity)`
- `createFocusBar : function(children : {TreeNodeId}, focusFilters? : function(), defocusFilter? : function(), iaCompRef? : ReactRefWrap, withoutHint? : boolean) : TreeNodeId`  (call as `statistics_react_util.createFocusBar(…)`): Wraps buttons in a horizontal filter bar that can be focused with the `IA_OPTION2` input action and left with `IA_OPTION2` or `IA_BACK`.
    - param `children`: Buttons of the bar.
    - param `focusFilters`: Called by `IA_OPTION2` to focus the bar.
    - param `defocusFilter`: Called by `IA_OPTION2` or `IA_BACK` while the bar has focus.
    - param `iaCompRef`: Reference for the bar's input action component.
    - param `withoutHint`: If set, no keybinding hint is shown.
    - used 1x in the base game, e.g. `base/content/gui/gui/statistics/statistics.tl:586`: `statistics_react_util.createFocusBar({builtin.ToggleButton{`
- `createCarrierCategoryFilter : function({StatisticsRecipeParams.Category}, StatisticsRecipeParams.OnlyVisible, oldState : (function() : FilterState), transformState : function(function(FilterState) : FilterState), focusFilters? : function(), defocusFilter? : function(), iaCompRef? : ReactRefWrap) : TreeNodeId`  (call as `statistics_react_util.createCarrierCategoryFilter(…)`): Creates the filter bar of a tab with a toggle button per category and, if an `OnlyVisible` kind is given, the "show only visible" toggle.
  In game: "Show Only Visible {object}" (German: „Nur sichtbare {object} anzeigen“)
    - param `#1`: Categories of the tab.
    - param `#2`: Object kind for the "show only visible" toggle, or `nil`.
    - param `oldState`: Returns the current filter state.
    - param `transformState`: Applies a change to the filter state.
    - param `focusFilters`: See `createFocusBar`.
    - param `defocusFilter`: See `createFocusBar`.
    - param `iaCompRef`: See `createFocusBar`.
    - used 2x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:1781`: `child = statistics_react_util.createCarrierCategoryFilter(`
- `createSearchField : function(value : string, set : function(string), focusNode : ReactRefWrap) : TreeNodeId`  (call as `statistics_react_util.createSearchField(…)`): Creates the search text field of a tab.
  In game: "Search..." (German: „Suche...“)
    - param `value`: Current text.
    - param `set`: Called with the text while typing.
    - param `focusNode`: Node that gets focus when input is cancelled.
    - used 2x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:5842`: `not params.commonParams.gamepadInputModeState:old() and statistics_react_util.createSearchField(`
- `createFocusDummyForGamepad : function() : TreeNodeId`  (call as `statistics_react_util.createFocusDummyForGamepad(…)`): Creates an empty, focusable component, used as table cell content so that gamepad focus can move through empty cells.
    - used 14x in the base game, e.g. `base/content/gui/gui/statistics/statistic_depots.tl:39`: `} or statistics_react_util.createFocusDummyForGamepad(),`
- `compareProblemsLess : function(notificationsState : {Engine.Entity : {integer}}, entity1 : Engine.Entity, entity2 : Engine.Entity) : boolean`  (call as `statistics_react_util.compareProblemsLess(…)`): Sort comparison for the problems column. An entity with fewer "Problem" notifications sorts first.
    - param `notificationsState`: Persisting notification ids per entity.
    - param `entity1`: First entity.
    - param `entity2`: Second entity.

get value usable to for sorting (so that number of notifiaction is most significant)
- `getProblemsCompareValue : function(notificationsState : {Engine.Entity : {integer}}, entity : Engine.Entity) : {integer, {integer}}`  (call as `statistics_react_util.getProblemsCompareValue(…)`)
    - used 6x in the base game, e.g. `base/content/gui/gui/statistics/statistic_industries.tl:401`: `return statistics_react_util.getProblemsCompareValue(tableState:old().notificationsState or {}, warehouseEntity)`
- `ProblemsCell : Recipe<Builtin.TableCellParam>`: Table cell showing a warning icon (with the count if there are several) for the "Problem" notifications of the row's entity. Clicking it runs the notification's click action.
  In game: "There Are Several Problems" (German: „Es gibt mehrere Probleme“)
    - notes: `params.rowKey` is the entity and `params.userParam` must be a `StatisticsReactUtil.UserParam`.
    - used 6x in the base game, e.g. `base/content/gui/gui/statistics/statistic_industries.tl:406`: `{ path = "::/gui/statistics/icons/alert.tga", tooltip = _("Problems"), recipe = statistics_react_util.ProblemsCell, getCompareValue = getProblemsCompareValue, w`
- `getCarrierFilterFromCategories : function(allCategories : {StatisticsRecipeParams.Category}, activeCategories : {integer : boolean}) : {Carrier}`  (call as `statistics_react_util.getCarrierFilterFromCategories(…)`): Collects the carriers of the enabled categories.
    - param `allCategories`: Categories of the tab.
    - param `activeCategories`: Enabled categories, by index.
    - returns: The carriers, or `nil` if no category is enabled or an enabled category has no carrier.
    - used 8x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:2540`: `local allowedCarriers = statistics_react_util.getCarrierFilterFromCategories(filterCategories, filterShowCategories)`
- `areCarriersContainedInTransportModes : function(transportModes : {TransportMode : boolean}, carriers : {Carrier}) : boolean`  (call as `statistics_react_util.areCarriersContainedInTransportModes(…)`): Checks whether any of the enabled transport modes corresponds to one of the given carriers.
    - param `transportModes`: Transport modes mapped to whether they are enabled.
    - param `carriers`: Carriers to look for.
    - used 7x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:1678`: `if not allowedCarriers or statistics_react_util.areCarriersContainedInTransportModes(depot.transportModes, allowedCarriers) then`

### StatisticsReactUtil.StationGroupType (enum)

```teal
enum StationGroupType
```

Kind of a station group, by the stations it contains.

- `"Passenger"`: Only passenger stations.
  In game: "Passenger" (German: „Passagiere“)
- `"Cargo"`: Only cargo stations.
  In game: "Cargo" (German: „Güter“)
- `"Mixed"`: Both passenger and cargo stations.
  In game: "Mixed" (German: „Gemischt“)

### StatisticsReactUtil.CargoColumnData (record)

```teal
record CargoColumnData
```

Summed load figures for the Load, Capacity, Utilization and Satisfaction columns.

- `supply : integer`: Sum of the cargo counts (load).
- `demand : integer`: Sum of the capacities.
- `coverage : number`: `supply / demand` (utilization); `0` if `supply` is 0.
- `averageQuality : number`: Mean of the known average qualities of the cargo types.

### StatisticsReactUtil.FilterState (record)

```teal
record FilterState
```

Filter settings of one statistics tab.

- `category : {integer : boolean}`: Enabled category buttons, by category index.
- `onlyVisible : boolean`: Whether the "show only visible" toggle is on.
- `search : string`: Search text.
- `initialSortColumn : {integer, boolean}`: Sort column as `{columnIndex, ascending}`.

### StatisticsReactUtil.UserParam (record)

```teal
record UserParam
```

User parameter that the statistics tables pass to their cells.

- `deleteLineFeedback : function(message : string, mode : string, dialogData : FeedbackListUtil.FeedbackDialogParam, id : number)`
- `notificationState : {Engine.Entity : {integer}}`: Persisting notification ids per entity.
