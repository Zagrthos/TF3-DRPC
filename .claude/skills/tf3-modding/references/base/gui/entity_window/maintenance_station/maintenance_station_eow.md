# gui.entity_window.maintenance_station.maintenance_station_eow

Definition file: `base/tealdef/gui/entity_window/maintenance_station/maintenance_station_eow.d.tl` (in the game folder).

Plugin types of the maintenance station (maintenance building) window (`maintenance_station_eow.script.tl`).

## MaintenanceStationEow (record)

```teal
global record MaintenanceStationEow
```
Load: `local maintenance_station_eow = ug_require "::/gui/entity_window/maintenance_station/maintenance_station_eow.script.tl" as MaintenanceStationEow`

Plugin types of the maintenance station window (depots and buildings with a maintenance pool).

- `MaintenanceStationEowExtensionPoint : ExtensionPoint<MaintenanceStationWidgetPluginParams>`: Extension point of the maintenance station window; register plugins with a resource of type `"react-plugin ::MaintenanceStationEowExtensionPoint"`.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/maintenance_station/maintenance_station.tl:76`: `local pluginWidgets = gui_react_util.usePlugins(maintenance_station_eow.MaintenanceStationEowExtensionPoint, {lastRef = lastInScrollAreaRef}, extensionParams)`
- `MaintenanceStationMaintenancePoolPlugin : Recipe<MaintenanceStationEow.MaintenanceStationWidgetPluginParams>`: "Maintenance Pool" card with the pool usage bar; registered with order 20.
  In game: "Maintenance Pool" (German: „Wartungspool“)
- `conditionMaintenanceStationBalance : function(extensionParams : MaintenanceStationEow.MaintenanceStationWidgetPluginParams) : boolean`  (call as `maintenance_station_eow.conditionMaintenanceStationBalance(…)`): Plugin condition of `MaintenanceStationBalancePlugin`; true if the station is owned by the player, `state.constructionHasAccount` is set and `state.infiniteMoney` is not.
- `MaintenanceStationBalancePlugin : Recipe<MaintenanceStationEow.MaintenanceStationWidgetPluginParams>`: Card with the yearly vehicle maintenance balance of the station; registered with order 30 and condition `conditionMaintenanceStationBalance`.
  In game: "Balance" (German: „Ertrag“)
- `MaintenanceStationVehiclesPlugin : Recipe<MaintenanceStationEow.MaintenanceStationWidgetPluginParams>`: "Maintained Vehicles" (or "Vehicles") card with a sortable table of the vehicles assigned to the station and their condition; registered with order 50.
  In game: "Maintained Vehicles", "Condition" (German: „Gewartete Fahrzeuge“, „Zustand“)
- `conditionMaintenanceStationStatistics : function(extensionParams : MaintenanceStationEow.MaintenanceStationWidgetPluginParams) : boolean`  (call as `maintenance_station_eow.conditionMaintenanceStationStatistics(…)`): Plugin condition; true if the station is owned by the player and has a logbook.
- `MaintenanceStationStatisticsPlugin : Recipe<MaintenanceStationEow.MaintenanceStationWidgetPluginParams>`: "Statistics" chart of the maximum and average pool usage (`maxPoolUsage`, `averagePoolUsage` logs); registered with order 40 and condition `conditionMaintenanceStationStatistics`.
  In game: "Statistics", "Average Usage", "Maximum Usage" (German: „Statistiken“, „Durchschnittliche Nutzung“, „Maximale Nutzung“)

### MaintenanceStationEow.MaintenanceStationEowState (record)

```teal
record MaintenanceStationEowState
```

State of the maintenance station window, recomputed every step and passed to the plugins.

- `poolSize : integer`: Maintenance pool size of the depot (`VEHICLE_DEPOT.maintenancePool`), 0 without depot component.
- `hasDepot : boolean`: True if the entity is a depot with in or out nodes (vehicles can be bought there).
- `hasLogbook : boolean`: True if the entity has a `LOG_BOOK` component.
- `constructionEntity : Engine.Entity`: Construction of the station; nil if it is owned by the player but cannot be modified. Used for the "Configure" button.
- `constructionHasAccount : boolean`: Required by `conditionMaintenanceStationBalance`; the base game window does not set it, so the balance card stays hidden.
- `constructionTimeBuilt : integer`
- `constructionIsModifiable : boolean`
- `infiniteMoney : boolean`: Hides the balance card in `conditionMaintenanceStationBalance` when true; the base game window does not set it.

### MaintenanceStationEow.MaintenanceStationWidgetPluginParams (record)

```teal
record MaintenanceStationWidgetPluginParams is EowExtensionUtil.IEowWidgetsExtensionParams
```

Parameters passed to the plugins of the maintenance station window.

- `state : MaintenanceStationEowState`: Window state.
