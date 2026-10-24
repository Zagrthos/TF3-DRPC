# gui.entity_window.entity_window_hud

Definition file: `base/tealdef/gui/entity_window/entity_window_hud.d.tl` (in the game folder).

HUD icon masters, pre-filters and tooltip filters used while an entity window is open: they decide
which world entities get HUD icons (and hover tooltips) related to the selected entity, e.g. the
suppliers and consumers of a selected industry.

## EntityWindowHud (record)

```teal
global record EntityWindowHud
```
Load: `local entity_window_hud = ug_require "::/gui/entity_window/entity_window_hud.tl" as EntityWindowHud`

HUD icon configuration of the entity windows (industry, perk, line, subsidy, town building, station, town, maintenance, vehicle).

Notes:

Each kind comes as a set: a `get...HudIconPreFilter` function listing the component types of the
entities that can get an icon, a `...HudIconMaster` recipe (called with the `Builtin.HudIconParam` and
a user parameter) that returns the icon or nil, and for some a `filter...HudActionTooltip` function
that decides which hovered entities show a tooltip. The windows pass them to
`builtin.HudIconManagerConfig{ masterRecipe = ..., userParam = ..., preFilter = ... }` and
`builtin.ActionTooltip`.

- `filterIndustryHudActionTooltip : function(entity : Engine.Entity, userParam : EntityWindowHudUserParam) : boolean`  (call as `entity_window_hud.filterIndustryHudActionTooltip(…)`): Tooltip filter of the industry window.
    - param `entity`: Hovered entity.
    - param `userParam`: User parameter with the selected industry and its cargo types.
    - returns: True for towns consuming the selected industry's cargo and for industries trading matching cargo (or the selected industry itself); nil if the entity does not exist or the selected entity is no industry.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/industry/industry.tl:395`: `return entity_window_hud.filterIndustryHudActionTooltip(entity, userParamInplace)`
- `getIndustryHudIconPreFilter : function() : {Engine.ComponentType}`  (call as `entity_window_hud.getIndustryHudIconPreFilter(…)`): Component types that can get an icon in the industry window.
    - returns: `TOWN` and `INDUSTRY`.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/industry/industry.tl:381`: `preFilter = entity_window_hud.getIndustryHudIconPreFilter(),`
- `IndustryHudIconMaster : Recipe2<Builtin.HudIconParam, EntityWindowHudUserParam>`: HUD icon master of the industry window; shows the cargo input/output indicator of related industries, the town icon of consuming towns and the notification icons of the selected industry.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/industry/industry.tl:379`: `masterRecipe = entity_window_hud.IndustryHudIconMaster,`
- `filterPerkHudActionTooltip : function(entity : Engine.Entity, userParam : EntityWindowHudUserParam) : boolean`  (call as `entity_window_hud.filterPerkHudActionTooltip(…)`): Tooltip filter of the perk (landmark, headquarters) window.
    - param `entity`: Hovered entity.
    - param `userParam`: User parameter with the selected perk construction.
    - returns: True for the closest town of the selected perk, the selected perk construction itself, and industries producing its cargo types.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/emission/pollution_cleanup_facility_hud.script.tl:109`: `return entity_window_hud.filterPerkHudActionTooltip(entity, userParamInplace)`
- `getPerkHudIconPreFilter : function() : {Engine.ComponentType}`  (call as `entity_window_hud.getPerkHudIconPreFilter(…)`): Component types that can get an icon in the perk window.
    - returns: `TOWN`, `CONSTRUCTION`, `SUBCONSTRUCTION`, `INDUSTRY` and `STOCK_LIST`.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/emission/pollution_cleanup_facility_hud.script.tl:93`: `preFilter = entity_window_hud.getPerkHudIconPreFilter(),`
- `PerkHudIconMaster : Recipe2<Builtin.HudIconParam, EntityWindowHudUserParam>`: HUD icon master of the perk window; shows the town icon of the perk's closest town, the perk's own icon and the cargo indicators of supplying industries.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/emission/pollution_cleanup_facility_hud.script.tl:91`: `masterRecipe = entity_window_hud.PerkHudIconMaster,`
- `getLineHudIconPreFilter : function() : {Engine.ComponentType}`  (call as `entity_window_hud.getLineHudIconPreFilter(…)`): Component types that can get an icon in the line window.
    - returns: `STATION_GROUP`, `TOWN` and `TRANSPORT_VEHICLE`.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/line/line.tl:192`: `preFilter = entity_window_hud.getLineHudIconPreFilter(),`
- `LineHudIconMaster : Recipe2<Builtin.HudIconParam, EntityWindowHudUserParam>`: HUD icon master of the line window; shows the cargo of the line's vehicles, the town icons of towns the line stops in, and default icons for the line's stations.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/line/line.tl:187`: `masterRecipe = entity_window_hud.LineHudIconMaster,`
- `getSubventionHudIconPreFilter : function() : {Engine.ComponentType}`  (call as `entity_window_hud.getSubventionHudIconPreFilter(…)`): Component types that can get an icon in the subsidy window.
    - returns: `TOWN` and `INDUSTRY`.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/subventions_gui.tl:449`: `preFilter = entity_window_hud.getSubventionHudIconPreFilter(),`
- `SubventionHudIconMaster : Recipe2<Builtin.HudIconParam, SubventionHudUserParam>`: HUD icon master of the subsidy window; shows town icons and cargo indicators for the towns and industries relevant to the subsidy.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/subventions_gui.tl:445`: `masterRecipe = entity_window_hud.SubventionHudIconMaster,`
- `filterTownBuildingHudActionTooltip : function(entity : Engine.Entity, userParam : EntityWindowHudUserParam) : boolean`  (call as `entity_window_hud.filterTownBuildingHudActionTooltip(…)`): Tooltip filter of the town building window.
    - param `entity`: Hovered entity.
    - param `userParam`: User parameter with the selected town building.
    - returns: True for the building's town, vehicles of lines stopping in that town, and industries producing the building's cargo types.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/town_building/town_building.tl:485`: `return entity_window_hud.filterTownBuildingHudActionTooltip(entity, userParamInplace)`
- `getTownBuildingHudIconPreFilter : function() : {Engine.ComponentType}`  (call as `entity_window_hud.getTownBuildingHudIconPreFilter(…)`): Component types that can get an icon in the town building window.
    - returns: `TOWN`, `TRANSPORT_VEHICLE` and `INDUSTRY`.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/town_building/town_building.tl:471`: `preFilter = entity_window_hud.getTownBuildingHudIconPreFilter(),`
- `TownBuildingHudIconMaster : Recipe2<Builtin.HudIconParam, EntityWindowHudUserParam>`: HUD icon master of the town building window; shows cargo indicators of supplying industries, the cargo of vehicles serving the town and the town icon.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/town_building/town_building.tl:469`: `masterRecipe = entity_window_hud.TownBuildingHudIconMaster,`
- `getStationGroupHudIconPreFilter : function() : {Engine.ComponentType}`  (call as `entity_window_hud.getStationGroupHudIconPreFilter(…)`): Component types that can get an icon in the station window.
    - returns: `TRANSPORT_VEHICLE` and `STATION_GROUP`.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/station_group/station_group.tl:947`: `preFilter = entity_window_hud.getStationGroupHudIconPreFilter(),`
- `StationGroupHudIconMaster : Recipe2<Builtin.HudIconParam, EntityWindowHudUserParam>`: HUD icon master of the station window; shows the selected station's icon and the cargo of vehicles on lines stopping at the station.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/station_group/station_group.tl:946`: `masterRecipe = entity_window_hud.StationGroupHudIconMaster,`
- `filterTownHudActionTooltip : function(entity : Engine.Entity, userParam : TownHudUserParam) : boolean`  (call as `entity_window_hud.filterTownHudActionTooltip(…)`): Tooltip filter of the town window.
    - param `entity`: Hovered entity.
    - param `userParam`: User parameter with the selected town and tab.
    - returns: True for the town; on the "Bonuses" tab for perks near the town; otherwise for related industries, vehicles and stations of lines stopping in the town, and warehouses storing matching cargo.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/town/town.tl:1090`: `return entity_window_hud.filterTownHudActionTooltip(entity, userParamInplace)`
- `getTownHudIconPreFilter : function(currentTab : TownReactUtil.TownInfoTabType) : {Engine.ComponentType}`  (call as `entity_window_hud.getTownHudIconPreFilter(…)`): Component types that can get an icon in the town window for a tab.
    - param `currentTab`: Selected tab of the town window.
    - returns: The component types; for the layer tabs (traffic, happiness, delivery, noise, pollution) the pre-filter of the matching layer.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/town/town.tl:1072`: `preFilter = entity_window_hud.getTownHudIconPreFilter(activeTabState:old()),`
- `TownHudIconMaster : Recipe2<Builtin.HudIconParam, TownHudUserParam>`: HUD icon master of the town window; the icons depend on the selected tab (cargo, vehicles and town on "Overview", perks on "Bonuses", the layer icons on the layer tabs).
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/town/town.tl:1070`: `masterRecipe = entity_window_hud.TownHudIconMaster,`
- `MaintenanceHudIconMaster : Recipe2<Builtin.HudIconParam, MaintenanceHudUserParam>`: HUD icon master for maintenance; shows vehicles and the pool usage bar of maintenance stations (depots with a maintenance pool) of the given transport modes.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/maintenance_station/maintenance_station.tl:151`: `masterRecipe = entity_window_hud.MaintenanceHudIconMaster,`
- `getMaintenanceHudIconPreFilter : function() : {Engine.ComponentType}`  (call as `entity_window_hud.getMaintenanceHudIconPreFilter(…)`): Component types that can get a maintenance icon.
    - returns: `VEHICLE_DEPOT` and `TRANSPORT_VEHICLE`.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/maintenance_station/maintenance_station.tl:157`: `preFilter = entity_window_hud.getMaintenanceHudIconPreFilter(),`
- `VehicleHudIconMaster : Recipe2<Builtin.HudIconParam, VehicleHudUserParam>`: HUD icon master of the vehicle window; shows icons for vehicles on the same line and for the selected vehicle while it goes to a depot.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/vehicle/vehicle.tl:572`: `masterRecipe = entity_window_hud.VehicleHudIconMaster,`
- `getVehicleHudIconPreFilter : function() : {Engine.ComponentType}`  (call as `entity_window_hud.getVehicleHudIconPreFilter(…)`): Component types that can get an icon in the vehicle window.
    - returns: `LINE` and `TRANSPORT_VEHICLE`.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/vehicle/vehicle.tl:574`: `preFilter = entity_window_hud.getVehicleHudIconPreFilter(),`

### EntityWindowHud.EntityWindowHudUserParam (interface)

```teal
interface EntityWindowHudUserParam is HudIconToolbox.HudIconMasterUserParam
```

User parameter of the entity window HUD icon masters.

- `cargoTypes : {CargoTypeId : CargoUtil.StockProperties}`: Cargo types of the selected entity with input/output flags; entities producing or consuming matching cargo get icons.

### EntityWindowHud.SubventionHudUserParam (record)

```teal
record SubventionHudUserParam
```

User parameter of `EntityWindowHud.SubventionHudIconMaster`.

- `subvention : ISubvention`: Subsidy whose relevant entities (from its script's `getRelevantEntities`) get icons.

### EntityWindowHud.TownHudUserParam (record)

```teal
record TownHudUserParam is EntityWindowHudUserParam
```

User parameter of the town window HUD icons.

- `currentTab : TownReactUtil.TownInfoTabType`: Selected tab of the town window; decides which icons are shown (e.g. `"Overview"`, `"Bonuses"`, `"Traffic"`, `"Noise"`).
- `showAllTowns : boolean`: Shows the icons of all towns, not only the selected one.

### EntityWindowHud.MaintenanceHudUserParam (record)

```teal
record MaintenanceHudUserParam is HudIconToolbox.HudIconMasterUserParam
```

User parameter of `EntityWindowHud.MaintenanceHudIconMaster`.

- `transportModes : {TransportMode}`: Transport modes to show; only vehicles and maintenance stations supporting one of them get an icon.

### EntityWindowHud.VehicleHudUserParam (record)

```teal
record VehicleHudUserParam is HudIconToolbox.HudIconMasterUserParam
```

User parameter of `EntityWindowHud.VehicleHudIconMaster`.

- `lineEntity : Engine.Entity`: Line of the selected vehicle; vehicles on the same line get icons.
