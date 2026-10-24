# base.model_metadata_util

Definition file: `base/tealdef/base/model_metadata_util.d.tl` (in the game folder).

Types of `/base/model_metadata_util.lua` (`ModelMetadataUtil`). Each `(fileName, data)` function is a
`loadModel` modifier that the base mod registers with `addModifier("loadModel", ...)`: it receives a
model file's data while the model loads, fills in defaults or converts older metadata formats, and
returns the data.

## ModelMetadataUtil (record)

```teal
global record ModelMetadataUtil
```
Load: `local model_metadata_util = require "::/base/model_metadata_util.lua" as ModelMetadataUtil`

`loadModel` modifiers from `/base/model_metadata_util.lua`. Each `(fileName, data)` function gets the
model's file name and its data table and returns the (changed) data.

- `convertEmissionData : function(fileName : string, data : table) : table`  (call as `model_metadata_util.convertEmissionData(…)`): Converts the legacy `metadata.emission` block (`idleEmission`, `speedEmission`, `powerEmission`) into `metadata.emissions.noise` and `metadata.emissions.pollution`.
    - used 1x in the base game, e.g. `base/content/model_editor/model_editor/editor_base_mod.lua:200`: `addModifier("loadModel", model_metadata_util.convertEmissionData)`
- `addEmissionMetadata : function(fileName : string, data : table) : table`  (call as `model_metadata_util.addEmissionMetadata(…)`): Converts legacy emission data and fills in default noise and pollution values for trees, cars and transport vehicles.
    - used 1x in the base game, e.g. `base/content/base/base/base_mod.lua:85`: `addModifier("loadModel", model_metadata_util.addEmissionMetadata)`
- `addCostMetadata : function(fileName : string, data : table) : table`  (call as `model_metadata_util.addCostMetadata(…)`): For transport vehicles with `cost.price < 0` or `maintenance.runningCosts < 0`, computes the price and
running costs from capacity, average speed and (for rail) engine power.
    - notes: The price becomes six times the computed running costs, multiplied by `cost.priceScale`; running costs are multiplied by `maintenance.runningCostScale`. Set the values to `-1` in a model to use this.
    - used 1x in the base game, e.g. `base/content/base/base/base_mod.lua:81`: `addModifier("loadModel", model_metadata_util.addCostMetadata)`
- `addTransportVehicleTransportModes : function(fileName : string, data : table) : table`  (call as `model_metadata_util.addTransportVehicleTransportModes(…)`): Sets `transportVehicle.transportModes` and `engineTransportModes` when the model does not define them:
from the legacy `tags`, or from the carrier (bus or truck by cargo, train/tram with electric variants
by engine type, aircraft, small aircraft, helicopter, ship or small ship).
    - used 2x in the base game, e.g. `base/content/base/base/base_mod.lua:83`: `addModifier("loadModel", model_metadata_util.addTransportVehicleTransportModes)`
- `addFilterTags : function(fileName : string, data : table) : table`  (call as `model_metadata_util.addFilterTags(…)`): Sets `transportVehicle.filterTags` to `{"default"}` if missing, or to `{}` for vehicles with `multipleUnitOnly`.
    - used 2x in the base game, e.g. `base/content/base/base/base_mod.lua:86`: `addModifier("loadModel", model_metadata_util.addFilterTags)`
- `newLoadSpeedsAndMaintenanceFactor : function(fileName : string, data : table) : table`  (call as `model_metadata_util.newLoadSpeedsAndMaintenanceFactor(…)`): Sets `transportVehicle.maintenanceFactor` to 1 if it is missing.
    - used 1x in the base game, e.g. `base/content/base/base/base_mod.lua:87`: `addModifier("loadModel", model_metadata_util.newLoadSpeedsAndMaintenanceFactor)`
- `turnRoadAndRailToLandVehicle : function(fileName : string, data : table) : table`  (call as `model_metadata_util.turnRoadAndRailToLandVehicle(…)`): Builds `metadata.landVehicle` from a legacy `roadVehicle` or `railVehicle` block (engines, sound set,
top speed, weight), with default brake deceleration and friction (road 5 and 0.1, rail 2.5 and 0.02).
    - used 2x in the base game, e.g. `base/content/base/base/base_mod.lua:69`: `addModifier("loadModel", model_metadata_util.turnRoadAndRailToLandVehicle)`
- `addVehicleExtent : function(fileName : string, data : table) : table`  (call as `model_metadata_util.addVehicleExtent(…)`): Sets `metadata.extent` of land, air and water vehicles to the model's bounding info if it is missing.
    - used 1x in the base game, e.g. `base/content/base/base/base_mod.lua:70`: `addModifier("loadModel", model_metadata_util.addVehicleExtent)`
- `addTransformatorConfig : function(fileName : string, data : table) : table`  (call as `model_metadata_util.addTransformatorConfig(…)`): Gives vehicle models without `transformatorConfig` the default transformator for their type
(`default_road.trf`, `default_train.trf`, `default_tram.trf`, `default_air.trf` or `default_ship.trf`).
    - notes: Models with `version = 2` are left unchanged.
    - used 2x in the base game, e.g. `base/content/base/base/base_mod.lua:17`: `addModifier("loadModel", model_metadata_util.addTransformatorConfig)`
- `toCompartmentList : function(fileName : string, data : table) : table`  (call as `model_metadata_util.toCompartmentList(…)`): Converts the legacy `capacities`, `loadConfigs` and `compartmentsList` fields of `transportVehicle` into `compartments`.
    - used 2x in the base game, e.g. `base/content/base/base/base_mod.lua:68`: `addModifier("loadModel", model_metadata_util.toCompartmentList)`
- `replaceMeshIdWithNodeName : function(fileName : string, data : table) : table`  (call as `model_metadata_util.replaceMeshIdWithNodeName(…)`): Replaces numeric node indices and `.msh` paths in vehicle configs (axles, wheels, fake bogies, steering parts), seats, camera positions, cargo slots and particle emitters with node names.
    - used 2x in the base game, e.g. `base/content/base/base/base_mod.lua:71`: `addModifier("loadModel", model_metadata_util.replaceMeshIdWithNodeName)`
- `squeezeConfigsToOneConfigForVehicles : function(fileName : string, data : table) : table`  (call as `model_metadata_util.squeezeConfigsToOneConfigForVehicles(…)`): Merges the per-LOD `configs` of road, rail and air vehicles into one `config`, keeping each axle and wheel once.
    - used 2x in the base game, e.g. `base/content/base/base/base_mod.lua:72`: `addModifier("loadModel", model_metadata_util.squeezeConfigsToOneConfigForVehicles)`
- `sortLods : function(fileName : string, data : table) : table`  (call as `model_metadata_util.sortLods(…)`): For `version = 1` models, sorts the LODs by `visibleTo` and reorders the fake bogies to match.
    - used 2x in the base game, e.g. `base/content/base/base/base_mod.lua:73`: `addModifier("loadModel", model_metadata_util.sortLods)`
- `addTextureLods : function(fileName : string, data : table) : table`  (call as `model_metadata_util.addTextureLods(…)`): Marks LODs 2 and 3 as `textureLodBase` for buildings under `::/buildings/` with four dynamic LODs, unless a LOD already sets it.
    - used 2x in the base game, e.g. `base/content/base/base/base_mod.lua:74`: `addModifier("loadModel", model_metadata_util.addTextureLods)`
- `addTreeReputationFactor : function(fileName : string, data : table) : table`  (call as `model_metadata_util.addTreeReputationFactor(…)`): For trees without a `reputationFactor` (or with -1), derives it from the collider volume divided by 2750.
    - used 1x in the base game, e.g. `base/content/base/base/base_mod.lua:91`: `addModifier("loadModel", model_metadata_util.addTreeReputationFactor)`
- `makeLoadIndicators : function(fileName : string, data : table) : table`  (call as `model_metadata_util.makeLoadIndicators(…)`): Moves the `cargoBay` and `customCargoModels` of cargo entries, and `cargoSlotProvider.slots`, into a `metadata.loadIndicator` block and links the entries to it by name.
    - used 2x in the base game, e.g. `base/content/base/base/base_mod.lua:75`: `addModifier("loadModel", model_metadata_util.makeLoadIndicators)`
- `makeLoadIndicatorFormats : function(fileName : string, data : table) : table`  (call as `model_metadata_util.makeLoadIndicatorFormats(…)`): Fills `cargoFormats` of load indicator cargo bays from `cargoFormat`, or with defaults (`{"MEDIUM4x1", "MEDIUM2x1"}` for `LEVEL` bays, `{"BIG", "SMALL"}` otherwise).
    - used 2x in the base game, e.g. `base/content/base/base/base_mod.lua:76`: `addModifier("loadModel", model_metadata_util.makeLoadIndicatorFormats)`
- `makeCargoEntry : function(fileName : string, data : table) : table`  (call as `model_metadata_util.makeCargoEntry(…)`): Replaces the `cargoEntries` list of each load config with a single `cargoEntry` (the first one).
    - used 2x in the base game, e.g. `base/content/base/base/base_mod.lua:77`: `addModifier("loadModel", model_metadata_util.makeCargoEntry)`
- `makeCargoTypeSet : function(fileName : string, data : table) : table`  (call as `model_metadata_util.makeCargoTypeSet(…)`): Converts a cargo entry's legacy `type` into `cargoTypeSet = { cargoTypesIncluded = { type } }`.
    - used 2x in the base game, e.g. `base/content/base/base/base_mod.lua:78`: `addModifier("loadModel", model_metadata_util.makeCargoTypeSet)`
- `convertCargoType : function(oldCargoType : string, debugContext : string) : string`  (call as `model_metadata_util.convertCargoType(…)`): Hook for converting an old cargo type name; the base game returns the name unchanged.
    - param `oldCargoType`: The cargo type name.
    - param `debugContext`: Context for log messages.
    - used 7x in the base game, e.g. `base/content/base/base/base_mod.lua:179`: `local newCargoType = model_metadata_util.convertCargoType(stock.cargoType, fileName)`
- `convertCargoTypeSet : function(oldCargoTypeSet : table, debugContext : string) : table`  (call as `model_metadata_util.convertCargoTypeSet(…)`): Runs `convertCargoType` on the included and excluded cargo types of a cargo type set.
    - returns: The original set if nothing changed, otherwise a new set.
    - used 1x in the base game, e.g. `base/content/base/base/base_mod.lua:185`: `local newCargoTypes = model_metadata_util.convertCargoTypeSet(stock.cargoTypes, fileName)`
- `convertCargoTypes : function(fileName : string, data : table) : table`  (call as `model_metadata_util.convertCargoTypes(…)`): Runs `convertCargoTypeSet` on the cargo type set of every cargo entry of a transport vehicle.
    - used 2x in the base game, e.g. `base/content/base/base/base_mod.lua:80`: `addModifier("loadModel", model_metadata_util.convertCargoTypes)`
- `addIconMetadata : function(fileName : string, data : table) : table`  (call as `model_metadata_util.addIconMetadata(…)`): For `version = 2` transport vehicles, fills missing icon paths in `description` from the model file
name, e.g. `icons/<name>_icon_small.tga`, `icons/<name>_store.tga` and `icons/<name>_icon20.tga`.
    - used 1x in the base game, e.g. `base/content/base/base/base_mod.lua:82`: `addModifier("loadModel", model_metadata_util.addIconMetadata)`
- `convertParticleSystem : function(fileName : string, data : table) : table`  (call as `model_metadata_util.convertParticleSystem(…)`): Renames old particle emitter fields (`frameSpeedByAge`, `rotationSpeedByAge`, `frameSpeedByLifetime`) and folds the removed `rotationSpeedByLifetime` into `rotationSpeed`, with a log message.
    - used 2x in the base game, e.g. `base/content/base/base/base_mod.lua:79`: `addModifier("loadModel", model_metadata_util.convertParticleSystem)`
