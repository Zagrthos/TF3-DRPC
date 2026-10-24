# mission.tutorial.tutorial_story_util

Definition file: `base/tealdef/mission/tutorial/tutorial_story_util.d.tl` (in the game folder).

Helpers of the base-game tutorial story (capital town, industries, areas, vehicle lists).

## TutorialStoryUtil (record)

```teal
global record TutorialStoryUtil
```
Load: `local tutorial_story_util = ug_require "::/mission/tutorial/tutorial_story_util.tl" as TutorialStoryUtil`

Helpers of the base-game tutorial story.

- `TownDistrictRadius : number`: Radius in metres around a district centre used to find its streets (100).
    - used 31x in the base game, e.g. `base/content/mission/mission/tutorial/tutorial_story.tl:688`: `circle = tutorial_story_util.toPosRadius(tutorial_story_util.getTownDistrictCenterSafe(tutorial_story_util.getCapital(), api.type["enum"].LandUseType.RESIDENTIA`
- `getCapital : function() : Engine.Entity`  (call as `tutorial_story_util.getCapital(…)`): Returns the tutorial's capital, read from the `store_capital` task.
    - returns: Town entity, or `-1`.
    - used 69x in the base game, e.g. `base/content/mission/mission/tutorial/tasks/tutorial_init.tl:138`: `industryEntity = tutorial_story_util.getClosestIndustryToTown("::/industries/farm/farm.con", tutorial_story_util.getCapital())`
- `getCapitalName : function() : string`  (call as `tutorial_story_util.getCapitalName(…)`): Returns the name of the capital.
    - returns: Name, or `"ERROR"`.
    - used 35x in the base game, e.g. `base/content/mission/mission/tutorial/tutorial_story.tl:558`: `TOWN_NAME = tutorial_story_util.getCapitalName(),`
- `getSecondTown : function() : Engine.Entity`  (call as `tutorial_story_util.getSecondTown(…)`): Returns the tutorial's second town, read from the `store_town2` task.
    - returns: Town entity, or `-1`.
    - used 13x in the base game, e.g. `base/content/mission/mission/tutorial/tutorial_story.tl:322`: `addEntity(tutorial_story_util.getSecondTown())`
- `getSecondTownName : function() : string`  (call as `tutorial_story_util.getSecondTownName(…)`): Returns the name of the second town.
    - returns: Name, or `"ERROR"`.
    - used 8x in the base game, e.g. `base/content/mission/mission/tutorial/tutorial_story.tl:1222`: `TOWN_NAME_2 = tutorial_story_util.getSecondTownName(),`
- `getPositionSafe : function(entity : Engine.Entity) : Vec3f`  (call as `tutorial_story_util.getPositionSafe(…)`): Returns the position of an entity, or `nil` if the entity is `nil` or no longer exists.
    - param `entity`: Entity to locate.
    - returns: Position, or `nil`.
    - used 20x in the base game, e.g. `base/content/mission/mission/tutorial/tutorial_story.tl:109`: `local townCenter = tutorial_story_util.getPositionSafe(town)`
- `getTownDistrictCenterSafe : function(town : Engine.Entity, landUseType : LandUseType) : Vec3f`  (call as `tutorial_story_util.getTownDistrictCenterSafe(…)`): Returns the centre of a town's district of one land-use type, or `nil` if the town is `nil` or no longer exists.
    - param `town`: Town entity.
    - param `landUseType`: Residential, commercial or industrial district.
    - returns: Position of the district centre, or `nil`.
    - used 23x in the base game, e.g. `base/content/mission/mission/tutorial/tutorial_story.tl:688`: `circle = tutorial_story_util.toPosRadius(tutorial_story_util.getTownDistrictCenterSafe(tutorial_story_util.getCapital(), api.type["enum"].LandUseType.RESIDENTIA`
- `getCapitalCargoDemand : function(cargoTypeRes : string) : number`  (call as `tutorial_story_util.getCapitalCargoDemand(…)`): Returns how much of a cargo the capital demands.
    - param `cargoTypeRes`: Cargo type file, as `api.res.cargoTypeRep.find` takes it.
    - returns: The demand (the second value of `townBuildingSystem.getCargoSupplyAndLimit`), or 0 without a capital or for a cargo the town does not use.
    - used 4x in the base game, e.g. `base/content/mission/mission/tutorial/tutorial_story_branch_a.tl:383`: `local demands = tutorial_story_util.getCapitalCargoDemand("::/cargos/vegetables/vegetables.cargo")`
- `getStoredEntity : function(taskKey : string) : Engine.Entity`  (call as `tutorial_story_util.getStoredEntity(…)`): Returns the entity stored by the `store_entity` task with the given key. The tutorial uses it for the oil platform, refinery, fishing grounds and forest.
    - param `taskKey`: Key of the `store_entity` task.
    - returns: The entity, or `nil` if the task is missing, has not stored one yet, or the entity no longer exists (an error is logged once).
    - used 8x in the base game, e.g. `base/content/mission/mission/tutorial/tutorial_story.tl:324`: `addEntity(tutorial_story_util.getStoredEntity("a_store_oil_platform"))`
- `toPosRadius : function(pos : Vec3f, radius : number) : MissionInterface.PosRadius`  (call as `tutorial_story_util.toPosRadius(…)`): Builds an area from a position and a radius.
    - param `pos`: Centre.
    - param `radius`: Radius in metres.
    - returns: Area.
    - used 37x in the base game, e.g. `base/content/mission/mission/tutorial/tutorial_story.tl:688`: `circle = tutorial_story_util.toPosRadius(tutorial_story_util.getTownDistrictCenterSafe(tutorial_story_util.getCapital(), api.type["enum"].LandUseType.RESIDENTIA`
- `getBaseEdgesInDistrictType : function(landUseType : LandUseType) : {Engine.Entity}`  (call as `tutorial_story_util.getBaseEdgesInDistrictType(…)`): Returns the street edges within `TownDistrictRadius` of the capital's district centre of a land-use type.
    - param `landUseType`: District type.
    - returns: Edge entities.
    - used 1x in the base game, e.g. `base/content/mission/mission/tutorial/tutorial_story.tl:697`: `proposalHandler = tutorial_story_util.makeStopConfig(tutorial_story_util.getBaseEdgesInDistrictType(api.type["enum"].LandUseType.RESIDENTIAL)),`
- `makeStopConfig : function(edges : {Engine.Entity}) : BuildConstructionUtil.ProposalHandlerConfig`  (call as `tutorial_story_util.makeStopConfig(…)`): Builds a proposal check that accepts only street stops placed on the given edges.
  In game: "Not a Stop!", "Not a Station!", "Only possible in the highlighted area." (German: „Keine Haltestelle!“, „Keine Station!“, „Nur im markierten Gebiet möglich.“)
    - param `edges`: Allowed edges.
    - returns: Proposal handler config.
    - used 1x in the base game, e.g. `base/content/mission/mission/tutorial/tutorial_story.tl:697`: `proposalHandler = tutorial_story_util.makeStopConfig(tutorial_story_util.getBaseEdgesInDistrictType(api.type["enum"].LandUseType.RESIDENTIAL)),`
- `getDepotPosition : function() : {number, number, number}`  (call as `tutorial_story_util.getDepotPosition(…)`): Returns the depot position stored by the `store_depot_position` task.
    - returns: Position `{x, y, z}`, or `nil`.
    - used 8x in the base game, e.g. `base/content/mission/mission/tutorial/tutorial_story.tl:937`: `links = { entity_depot = { getLocation = function() : MissionInterface.PosRadius return { pos = tutorial_story_util.getDepotPosition(), radius = DepotRadius } e`
- `getAllBusTypes : function() : {string : integer}`  (call as `tutorial_story_util.getAllBusTypes(…)`): Returns every land vehicle model that carries passengers, each with limit 10.
    - returns: Model name -> 10.
    - used 1x in the base game, e.g. `base/content/mission/mission/tutorial/tutorial_story.tl:940`: `allowBuyVehicles = tutorial_story_util.getAllBusTypes() as {string : integer},`
- `getAllTrucksWithClasses : function(allowedClasses : {string}) : {string : integer}`  (call as `tutorial_story_util.getAllTrucksWithClasses(…)`): Returns every land vehicle model that carries one of the given cargo classes, each with limit 10.
    - param `allowedClasses`: Cargo classes.
    - returns: Model name -> 10.
    - used 2x in the base game, e.g. `base/content/mission/mission/tutorial/tutorial_story_branch_a.tl:263`: `allowBuyVehicles = tutorial_story_util.getAllTrucksWithClasses({"UNIVERSAL", "GOODS"}) as {string: integer},`
- `makeDisabledListExcept : function(exceptions? : {string}) : {string}`  (call as `tutorial_story_util.makeDisabledListExcept(…)`): Returns the list of construction and management menu ids the tutorial disables, minus the exceptions.
    - param `exceptions`: Menu ids to keep enabled.
    - returns: Menu ids.
    - used 35x in the base game, e.g. `base/content/mission/mission/tutorial/tutorial_story.tl:395`: `disabled = tutorial_story_util.makeDisabledListExcept(),`
- `getClosestIndustryToTown : function(conName : ResName, townEntity : Engine.Entity) : Engine.Entity`  (call as `tutorial_story_util.getClosestIndustryToTown(…)`): Returns the industry of a construction type closest to a town; one in the town's area wins.
    - param `conName`: Construction file of the industry.
    - param `townEntity`: Town.
    - returns: Industry entity, or `-1`.
    - used 8x in the base game, e.g. `base/content/mission/mission/tutorial/tasks/tutorial_init.tl:138`: `industryEntity = tutorial_story_util.getClosestIndustryToTown("::/industries/farm/farm.con", tutorial_story_util.getCapital())`
- `getIndustryToConnect : function() : Engine.Entity`  (call as `tutorial_story_util.getIndustryToConnect(…)`): Returns the industry whose road the tutorial init task removed.
    - returns: Industry entity, or `-1`.
    - used 21x in the base game, e.g. `base/content/mission/mission/tutorial/tutorial_story.tl:323`: `addEntity(tutorial_story_util.getIndustryToConnect())`
- `calcAnnualShipmentSafe : function(industryEntity : Engine.Entity) : integer`  (call as `tutorial_story_util.calcAnnualShipmentSafe(…)`): Returns the annual shipment of an industry, or 0 if the entity is `nil` or no longer exists.
    - param `industryEntity`: Industry entity.
    - returns: Annual shipment, or 0.
    - used 4x in the base game, e.g. `base/content/mission/mission/tutorial/tutorial_story_branch_a.tl:395`: `return tutorial_story_util.calcAnnualShipmentSafe(tutorial_story_util.getIndustryToConnect()) > 0`
- `makeAreaForIndustry : function(entity : Engine.Entity, radius : number) : MissionInterface.PosRadius`  (call as `tutorial_story_util.makeAreaForIndustry(…)`): Builds a circle at an industry construction's position.
    - param `entity`: Industry entity.
    - param `radius`: Radius in metres.
    - returns: Area, or `nil`.
    - used 8x in the base game, e.g. `base/content/mission/mission/tutorial/tutorial_story_branch_a.tl:190`: `tutorial_story_util.makeAreaForIndustry(tutorial_story_util.getIndustryToConnect(), IndustryRadius),`
- `makeAreaForIndustryRoadSegment : function(entity : Engine.Entity) : MissionInterface.PosRadius`  (call as `tutorial_story_util.makeAreaForIndustryRoadSegment(…)`): Builds a 20 m circle at the start of the industry construction's first frozen edge.
    - param `entity`: Industry entity.
    - returns: Area, or `nil`.
    - used 4x in the base game, e.g. `base/content/mission/mission/tutorial/tutorial_story_branch_a.tl:120`: `local industryZone = tutorial_story_util.makeAreaForIndustryRoadSegment(tutorial_story_util.getIndustryToConnect())`
- `constructPolygonForWaterIndustry : function(entity : Engine.Entity) : PolygonUtil.Polygon`  (call as `tutorial_story_util.constructPolygonForWaterIndustry(…)`): Builds a convex polygon around the water body of a water industry, extended 200 m onto land.
    - param `entity`: Industry entity.
    - returns: Polygon.
    - used 2x in the base game, e.g. `base/content/mission/mission/tutorial/tutorial_story_branch_a.tl:91`: `WaterPolygonCache = tutorial_story_util.constructPolygonForWaterIndustry(industry)`
- `hasAnySingleVehicleSupplyLine : function(pos1 : {MissionInterface.PosRadius}, pos2 : {MissionInterface.PosRadius}, cargoTypeRes : string) : boolean`  (call as `tutorial_story_util.hasAnySingleVehicleSupplyLine(…)`): Tells whether a player road line between any of the start and end areas has exactly one vehicle that can carry the cargo.
    - param `pos1`: Start areas.
    - param `pos2`: End areas.
    - param `cargoTypeRes`: Cargo type resource.
    - returns: `true` if such a line exists.
    - used 2x in the base game, e.g. `base/content/mission/mission/tutorial/tutorial_story_branch_a.tl:921`: `return tutorial_story_util.hasAnySingleVehicleSupplyLine(`
