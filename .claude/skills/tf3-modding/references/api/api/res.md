# api.res

Definition file: `api/tealdef/api/res.d.tl` (in the game folder).

Repositories containing game resources (models, tracks, streets, ...)

## GameScriptType (record)

```teal
global record GameScriptType
```

Game script resource, the entry type of `api.res.gameScriptRep`.

- `getState : function(GameScriptType) : table`

## ResTypeRep (interface)

```teal
global interface ResTypeRep<K, T>
```

Generic interface for repositories
Resources are generally static (initialized when the game starts and don't change)
Make sure that resource exists before trying to access them
Resource ids are only valid within the same repository

- `get : function(id : K) : T`: Gets the resource for a given id - (can be used to directly modify the resource,
but this is discouraged, except for performance reasons. Use getAsTable and setAsTable instead.
    - param `id`: a resource id
    - returns: the resource
- `getName : function(id : K) : string`: Gets the resource name for a given id
    - param `id`: a resource id
    - returns: the resource name
- `find : function(resName : string) : K`: Finds the id of the given resource name and returns its id
    - param `resName`: a resource name
    - returns: the id of the resource, -1 if the resource doesn't exist
- `findAndGet : function(resName : string) : T`: Finds the resource with the given name and returns it
    - param `resName`: a resource name
    - returns: the resource, and an error if the resource is not found
- `getAll : function(includeInvisible? : boolean) : {K : string}`: Gets all resources
    - param `includeInvisible`: if invisible resources should be included, false by default
    - returns: a dictionary containing all id and resource names
- `add : function(resName : ResName, resource : T, visible : boolean) : K`: Adds a new resource to the repository
    - param `resName`: the resource name of the new resource
    - param `resource`: the resource itself
    - param `visible`: if the resource is visible
    - returns: the id of the newly added resource
- `setVisible : function(id : K, visible : boolean)`: Sets a resource visible
    - param `id`: the id of the resource
    - param `visible`: if the resource should be visible
- `isVisible : function(id : K) : boolean`: Checks if a resource is visible
    - param `id`: the id of the resource
    - returns: if the resource is visible
- `getAsTable : function(id : K) : T`: Gets the resource for a given id as a pure lua table
    - param `id`: a resource id
    - returns: the resource
- `setAsTable : function(id : K, t : T) : boolean`: Updates the resource for a given id from a pure lua table (as if it was the resource file)
    - param `id`: a resource id
    - param `t`: the resource
    - returns: success
- `addAsTable : function(resName : ResName, resource : T) : K`: Add a new resource from a pure lua table (as if it was the resource file)
    - param `the`: name of the resource to add (should be unique)
    - param `t`: the resource to add
    - returns: the newly added id

## ModelRep (record)

```teal
global record ModelRep is ResTypeRep<integer, Model>
```
Access: `api.res.modelRep`

Type of `api.res.modelRep`, the model repository; adds `forEachModelWithMetadata` to the generic repository functions.

Example:
```lua
api.res.modelRep.forEachModelWithMetadata("transportVehicle", function(name)
  local id = api.res.modelRep.find(name)
  local model = api.res.modelRep.get(id)
end)
```

- `forEachModelWithMetadata : function(metadata : string, callback : function(name : string))`  (call as `api.res.modelRep.forEachModelWithMetadata(…)`): Iterates over all the models with a specific metadata
    - param `metadata`: the name of the metadata
    - param `callback`: a callback for every model that contains the specified metadata
    - used 7x in the base game, e.g. `base/content/game_mechanics/game_mechanics/achievements/achievements.script.tl:740`: `api.res.modelRep.forEachModelWithMetadata("transportVehicle", function(resName : string)`

## CargoTypeRep (record)

```teal
global record CargoTypeRep is ResTypeRep<CargoTypeId, CargoType>
```
Access: `api.res.cargoTypeRep`

Type of `api.res.cargoTypeRep`, the cargo type repository; adds cargo type set helpers and `getPassengerCargoTypeId`.

- `forEachCargoType : function(cargoTypeSet : CargoTypeSet, callback : function(cargoTypeId : CargoTypeId))`  (call as `api.res.cargoTypeRep.forEachCargoType(…)`): Iterates over all cargo types in a given cargo type set
    - param `cargoTypeSet`: the cargo type set to iterate over
    - param `callback`: a callback for each cargo type
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_store_util.tl:74`: `api.res.cargoTypeRep.forEachCargoType(loadConfig.cargoEntry.cargoTypeSet, function(cargoTypeId : CargoTypeId)`
- `hasCargoType : function(cargoTypeSet : CargoTypeSet, test : CargoTypeId, inverse? : boolean) : boolean`  (call as `api.res.cargoTypeRep.hasCargoType(…)`): Checks if a cargo type set contains a specific cargo type
    - param `cargoTypeSet`: the cargo type set to check
    - param `test`: the cargo type to test for
    - param `inverse`: if true, the function returns true when any other cargo exists
- `getPassengerCargoTypeId : function() : CargoTypeId`  (call as `api.res.cargoTypeRep.getPassengerCargoTypeId(…)`): Returns the cargo type id for passenger
    - used 9x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/deliver_passengers/deliver_passengers.script.tl:469`: `newData.cargoTypes = {api.res.cargoTypeRep.getPassengerCargoTypeId()}`

## CargoClassRep (record)

```teal
global record CargoClassRep is ResTypeRep<integer, CargoClass>
```
Access: `api.res.cargoClassRep`

Type of `api.res.cargoClassRep`, the cargo class repository; adds lookups by cargo class name.

- `getCargoClassId : function(cargoClass : string) : CargoClassId`  (call as `api.res.cargoClassRep.getCargoClassId(…)`): Gets the CargoClassId for a given CargoClass name
    - param `cargoClass`: cargo class name
    - returns: cargo class id
    - used 9x in the base game, e.g. `base/content/gui/gui/construction/construction.tl:3774`: `local classId = api.res.cargoClassRep.getCargoClassId(specialization)`
- `hasCargoClass : function(scargoClass : string) : boolean`  (call as `api.res.cargoClassRep.hasCargoClass(…)`): Checks if the given cargo class is present, this can be used if some cargo classes exist but are invisible
    - param `cargoClass`: cargo class name
    - returns: true if present

## GenericRep (record)

```teal
global record GenericRep is ResTypeRep<integer, GenericGameRes>
```
Access: `api.res.genericRep`

Type of `api.res.genericRep`, the repository of generic game resources (`GenericGameRes`, a `type` string plus a `data` table); adds `getAllOfType`.

- `getAllOfType : function(typeName : string, includeInvisible? : boolean) : {integer}`  (call as `api.res.genericRep.getAllOfType(…)`): Gets all Generic resource types of a given type name
does not allow typeName in { "gui_res_overwrite", "gui_res" }, for those, use `api.gui.genericRep`
    - param `typeName`: type name
    - param `includeInvisible`: whether to include invisible
    - returns: resource ids
    - used 24x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company_static_util.tl:36`: `for __, id in ipairs(api.res.genericRep.getAllOfType("company_permit_key")) do`

## Res (record)

```teal
global record Res
```
Access: `api.res`

List of all repositories

- `buildingTypeRep : ResTypeRep<integer, BuildingType>`: A resource repository containing data for town buildings.
- `bridgeTypeRep : ResTypeRep<integer, BridgeType>`: A resource repository containing data from the bridge configuration files and from dynamically generated bridges.
    - used 10x in the base game, e.g. `base/content/base/base/post_run_util.tl:50`: `applyCostScale(api.res.bridgeTypeRep)`
- `constructionRep : ResTypeRep<integer, ConstructionDesc>`: A resource repository containing data from the ".con" files and from dynamically generated constructions.
    - used 58x in the base game, e.g. `base/content/base/base/mod.script.tl:326`: `local allConstructions = api.res.constructionRep.getAll()`
- `metaConstructionRep : ResTypeRep<integer, MetaConstructionDesc>`: A resource repository containing data for all custom resources.
    - used 4x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/availability_notifications.script.tl:37`: `local con = api.res.metaConstructionRep.get(id)`
- `genericRep : GenericRep`: Repository of generic game resources (`GenericGameRes`), each with a `type` string and a `data` table.
    - notes: Base-game scripts query it by type, e.g. `getAllOfType("notification")`, `"subvention"`, `"tutorial_boot"`, `"missiontask_boot"`, `"rename_scheme"`, `"stamp"` or `"construction_tool"`, or look up one resource by name with `find`. GUI resources (`gui_res`, `gui_res_overwrite`) live in `api.gui.genericRep` instead.

    ```lua
    local id = api.res.genericRep.find("::/infrastructure/traffic_light/traffic_light_config.res")
    local config = api.res.genericRep.get(id).data
    for _, resId in ipairs(api.res.genericRep.getAllOfType("notification")) do
      local res = api.res.genericRep.get(resId)
    end
    ```

    - used 156x in the base game, e.g. `base/content/base/base/mod.script.tl:383`: `local trafficLightConfig = api.res.genericRep.get(api.res.genericRep.find("::/infrastructure/traffic_light/traffic_light_config.res")).data as TrafficLightConfi`
- `economyRep : ResTypeRep<integer, EconomyDesc>`: A resource repository containing data from the ".eco" economy resources.
    - used 8x in the base game, e.g. `base/content/base/base/mod.script.tl:312`: `local selectedEconomyCategory = api.res.economyRep.get(api.res.economyRep.find(selectedEconomy)).category`
- `climateRep : ResTypeRep<integer, ClimateDesc>`: A resource repository containing data for the climates.
    - used 8x in the base game, e.g. `base/content/base/base/mod.script.tl:316`: `local selectedClimateCategory = api.res.climateRep.get(api.res.climateRep.find(selectedClimate)).category`
- `environmentRep : ResTypeRep<integer, Environment>`: A resource repository containing data for the environment.
    - used 2x in the base game, e.g. `base/content/gui/gui/game_bar/game_bar_widgets.tl:611`: `local environmentRep = api.res.environmentRep`
- `gameScriptRep : ResTypeRep<integer, GameScriptType>`: A resource repository containing game scripts.
- `cargoClassRep : CargoClassRep`: A resource repository containing data with all the cargo classes.
    - used 9x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_util.tl:463`: `local specializationData = api.res.cargoClassRep.get(cargoClassId)`
- `cargoTypeRep : CargoTypeRep`: A resource repository containing data with all the cargo types.
    - used 101x in the base game, e.g. `base/content/base/base/mod.script.tl:336`: `local allCargos = api.res.cargoTypeRep.getAll()`
- `modelRep : ModelRep`: A resource repository containing all the game models and metadata.
    - used 114x in the base game, e.g. `base/content/base/base/mod.script.tl:363`: `local allModels = api.res.modelRep.getAll()`
- `multipleUnitRep : ResTypeRep<integer, MultipleUnit>`: A resource repository containing data from the multiple unit files and from dynamically generated multiple units.
    - used 22x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/availability_notifications.script.tl:147`: `local allMu = api.res.multipleUnitRep.getAll()`
- `streetTemplateRep : ResTypeRep<integer, StreetTemplate>`: A resource repository containing data from the street template files and from dynamically generated street templates.
    - used 31x in the base game, e.g. `base/content/base/base/mod.script.tl:401`: `local tracks = api.res.streetTemplateRep.getAll()`
- `edgeDecorationRep : ResTypeRep<integer, EdgeDecoration>`: Repository of edge decorations; street templates reference them by name in `defaultEdgeDecorations`, and each carries noise and pollution modifiers.

    ```lua
    -- from gui/construction/construction_react_util.tl
    local id = api.res.edgeDecorationRep.find(resName)
    if id >= 0 then
      local deco = api.res.edgeDecorationRep.get(id)
      print(deco.modifiers.noiseModifier, deco.modifiers.pollutionModifier)
    end
    ```

    - used 4x in the base game, e.g. `base/content/gui/gui/construction/construction_react_util.tl:2121`: `local edgeDecorationId = api.res.edgeDecorationRep.find(resName)`
- `terrainMaterialTypeRep : ResTypeRep<integer, TerrainMaterialType>`: A resource repository containing materials.
    - used 8x in the base game, e.g. `base/content/base/base/mod.script.tl:344`: `local allTerrainMaterials = api.res.terrainMaterialTypeRep.getAll()`
- `trafficLightTypeRep : ResTypeRep<integer, TrafficLightType>`: A resource repository containing data from the traffic lights.
    - used 5x in the base game, e.g. `base/content/base/base/mod.script.tl:385`: `local allTraficLights = api.res.trafficLightTypeRep.getAll()`
- `tunnelTypeRep : ResTypeRep<integer, BridgeType>`: A resource repository containing data from the tunnel configuration files and from dynamically generated tunnels.
    - used 7x in the base game, e.g. `base/content/base/base/post_run_util.tl:51`: `applyCostScale(api.res.tunnelTypeRep)`
- `moduleRep : ResTypeRep<integer, ModuleDesc>`: A resource repository containing data from the ".modules" files and from dynamically generated modules.
    - used 12x in the base game, e.g. `base/content/base/base/mod.script.tl:482`: `api.res.moduleRep.add(mod.fileName, mod, true)`
- `railroadCrossingTypeRep : ResTypeRep<integer, RailroadCrossingType>`: A resource repository containing data from the railroad crossing files and from dynamically generated railroad crossing types.
    - used 6x in the base game, e.g. `base/content/base/base/mod.script.tl:656`: `for id, __ in pairs(api.res.railroadCrossingTypeRep.getAll()) do`
- `streetStyleRep : ResTypeRep<integer, StreetStyle>`: A resource repository containing data from the street type files and from dynamically generated street types.
    - used 2x in the base game, e.g. `base/content/base/base/mod.script.tl:416`: `local trackStyle = api.res.streetStyleRep.get(api.res.streetStyleRep.find(track.streetStyle))`
- `builderAudioSetRep : ResTypeRep<integer, BuilderAudioSet>`: A resource repository containing data for audio sets for building
- `soundSetRep : ResTypeRep<integer, SoundSet>`: A resource repository containing data for sound sets
- `groundTextureRep : ResTypeRep<string, GroundTexture>`: A resource repository containing data for ground textures.
- `autoGroundTexRep : ResTypeRep<string, AutoGroundTex>`: A resource repository containing data for automatic ground texture definitions.
- `getBaseConfig : function() : BaseConfig`  (call as `api.res.getBaseConfig(…)`): Gets the base config, it contains most of the data which was read from "base/base_config.lua".
    - returns: Table the configuration table. See https://wiki.transportfever3.com/doku.php?id=modding:baseconfig for a detailed explanation.
    - notes: The base game fills the base configuration in its `preRunFn` (`base/content/base/base/mod.script.tl`); `base/content/base/base/base_config.lua` is only a stub. Mods change it in `preRunFn` (argument `baseConfig`) or read and change it in `postRunFn` through this function. The wiki page https://wiki.transportfever3.com/doku.php?id=modding:scripting:baseconfig still describes Transport Fever 2.
    - used 77x in the base game, e.g. `base/content/base/base/mod.script.tl:309`: `local baseConfig = api.res.getBaseConfig()`
