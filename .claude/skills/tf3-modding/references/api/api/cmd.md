# api.cmd

Definition file: `api/tealdef/api/cmd.d.tl` (in the game folder).

The module is available on both the GUI State and the Engine State (and the Console State as well). However,
it works in a slightly different manner depending on where it is used from.
On the engine states, commands are executed immediately and the effects are immediately noticeable. Moreover, the callback function
will be called immediately.
On the gui and console states, commands are executed in the next simulation step, and the effects are noticeable only after a few frames.
The callback function will be called the frame after the command has been executed, after which the changes are visible on the gui and engine
state.

## Globals

- `JournalEntry` (type): Journal entry used by journal-related commands.

One finance journal booking (`api.type.JournalEntry`). Create it with `api.type.JournalEntry.new()`,
set `amount`, `time` and `category`, and book it with `api.cmd.makeJournalBookAssetCmd`.

## Command (record)

```teal
global record Command<T is ICommandData>
```

Command object sent via `cmd.sendCommand`.

## ICommandData (interface)

```teal
global interface ICommandData
```

Marker interface for command data payloads.

## AnimalSetStateCommandData (record)

```teal
global record AnimalSetStateCommandData is ICommandData
```

Sends a command object to be executed later or immediately depending on the context.

- `animalEntity : Engine.Entity`: Entity id of the animal that should be updated.
- `movementType : integer`: the index of the configuration in the animals metadata movementTypes list
- `targetChangedElapsed : number`: Time since the current target destination was selected.
- `invalidTileElapsed : number`: Time since the animal got into an invalid area.
- `movementSpeed : number`: speed that should be used.
- `angularSpeed : number`: angularSpeed angular speed that should be used.

## AnimalSpawnAtCommandData (record)

```teal
global record AnimalSpawnAtCommandData is ICommandData
```

Command data for `cmd.makeAnimalSpawnAtCmd`.

- `modelId : integer`: reference to the animal model that should be spawned.
- `position : Vec2f`: position coordinates of the position where the animal should be spawned.
- `lookAt : Vec2f`: lookAt the direction the animal is looking.
- `resultEntity : Engine.Entity`: resultEntity The entity id of the spawned animal, used as a result only.

## CustomEntityDestroyCommandData (record)

```teal
global record CustomEntityDestroyCommandData is ICommandData
```

Command data for `cmd.makeCustomEntityDestroyCmd`.

- `entity : Engine.Entity`: Entity id of the custom entity.

## CustomEntityCreateCommandData (record)

```teal
global record CustomEntityCreateCommandData is ICommandData
```

Command data for `cmd.makeCustomEntityCreateCmd`.

- `modelId : integer`: Model id of the custom entity.
- `resultEntity : Engine.Entity`: Created entity id (result only).

## CustomEntityUpdateStateCommandData (record)

```teal
global record CustomEntityUpdateStateCommandData is ICommandData
```

Command data for `cmd.makeCustomEntityUpdateStateCmd`.

- `entity : Engine.Entity`: Entity id of the custom entity.
- `customState : table`: New custom state.

## CustomEntityUpdateTransformationCommandData (record)

```teal
global record CustomEntityUpdateTransformationCommandData is ICommandData
```

Command data for `cmd.makeCustomEntityUpdateTransformationCmd`.

- `entity : Engine.Entity`: Entity id of the custom entity.
- `transf : Mat4f`: New transformation.

## CustomVehicleCreateCommandData (record)

```teal
global record CustomVehicleCreateCommandData is ICommandData
```

Command data for `cmd.makeCustomVehicleCreateOrUpdateCmd`.

- `vehicleEntity : Engine.Entity`: Vehicle entity to update, nil will create a new entity
- `carrier : Carrier`: Model id of the custom entity.
- `vehicles : {{integer, boolean}}`: List of carriages to add to the vehicles (model id and reversed status)
- `movePathAircraft : Engine.Component.MovePathAircraft`: The move path for aircraft and ships (not all members are utilized, path0 and 2 must be empty and must be in flight state)
- `resultEntity : Engine.Entity`: Created entity id (result only).

## EntitySetColorCommandData (record)

```teal
global record EntitySetColorCommandData is ICommandData
```

Command data for `cmd.makeEntitySetColorCmd`.

- `entity : Engine.Entity`: Entity id.
- `color : Vec3f`: RGB color.

## EntitySetEmissionsCommandData (record)

```teal
global record EntitySetEmissionsCommandData is ICommandData
```

Command data for `cmd.makeEntitySetEmissionsCmd`.

- `entity : Engine.Entity`: Entity id of the emission emitter.
- `noisePower : number`: Noise in power.
- `pollutionPower : number`: Pollution in power.
- `radius : number`: Emission radius in meters.

## EntitySetNameCommandData (record)

```teal
global record EntitySetNameCommandData is ICommandData
```

Command data for `cmd.makeEntitySetNameCmd`.

- `entity : Engine.Entity`: id of the entity that should be renamed.
- `name : string`: the name that should be set.

## EntitySetPlayerCommandData (record)

```teal
global record EntitySetPlayerCommandData is ICommandData
```

Command data for `cmd.makeEntitySetPlayerCmd`.

- `entity : Engine.Entity`: Entity id for which the player will be assigned.
- `playerEntity : Engine.Entity`: Player entity id.

## GameAddPlayerCommandData (record)

```teal
global record GameAddPlayerCommandData is ICommandData
```

Command data for `cmd.makeGameAddPlayerCmd`.

- `name : string`: Player name.
- `color : Vec3f`: Player color.
- `resultEntity : Engine.Entity`: Created player entity id (result only).

## GameSetCalendarSpeedCommandData (record)

```teal
global record GameSetCalendarSpeedCommandData is ICommandData
```

Command data for `cmd.makeGameSetCalendarSpeedCmd`.

- `millisPerDay : integer`: Day length in milliseconds.

## GameSetDateCommandData (record)

```teal
global record GameSetDateCommandData is ICommandData
```

Command data for `cmd.makeGameSetDateCmd`.

- `date : Date`: New date.

## GameSaveCommandData (record)

```teal
global record GameSaveCommandData is ICommandData
```

Command data for saving the game.

- `modDescs : {Mod.GameModDesc}`: Descriptions of the mods stored with the savegame.
- `configData : table`
- `guiSaveData : table`: GUI state stored in the savegame (compare `game.getGuiSaveData` / `game.setGuiSaveData` in the base GUI scripts).
- `fileName : string`: File name of the savegame.
- `isAutosave : boolean`: True for an automatic save.
- `isMapsave : boolean`: True when the game is saved as a map (savemap namespace) instead of a savegame.
- `screenshot : GameScreenshot`: Preview screenshot stored with the savegame (the image `app.getSavegameImage` returns).
- `error : integer`

## GameSetSpeedCommandData (record)

```teal
global record GameSetSpeedCommandData is ICommandData
```

Command data for `cmd.makeGameSetSpeedCmd`.

- `speedup : integer`: Simulation speed.

## GameSetTimeOfDayCommandData (record)

```teal
global record GameSetTimeOfDayCommandData is ICommandData
```

Command data for `cmd.makeGameSetTimeOfDayCmd`.

- `timeOfDaySec : integer`: Time of day in seconds.

## GameSetCloudCoverageCommandData (record)

```teal
global record GameSetCloudCoverageCommandData is ICommandData
```

Command data for `cmd.makeGameSetCloudCoverageCmd`.

- `cloudCoverage : number`: new cloud coverage.

## IndustrySetDespawnTimeCommandData (record)

```teal
global record IndustrySetDespawnTimeCommandData is ICommandData
```

Command data for `cmd.makeIndustrySetDespawnTimeCmd`._

- `entity : Engine.Entity`: of the building entity of which the despawn timeStamp should change.
- `timeStamp : integer`: time after which the building should be closed.

## IndustrySetManualDevelopmentCommandData (record)

```teal
global record IndustrySetManualDevelopmentCommandData is ICommandData
```

Command data for `cmd.makeIndustrySetManualDevelopmentCmd`.

- `entity : Engine.Entity`: Industry entity id.
- `manual : boolean`: manual whether the entity should manually be developed.

## CreateIndustryExtendProposalCommandData (record)

```teal
global record CreateIndustryExtendProposalCommandData is ICommandData
```

Command data for `cmd.makeCreateIndustryExtendProposalCmd`.

- `extendConstruction : Engine.Entity`: Entity id of the industry that should be extended.
- `numTrySlots : integer`: Number of slots to try to extend.
- `numKeepSlots : integer`: Number of slots to keep after the extension.
- `resultProposal : Proposal`: The resulting proposal generated by the command (result only).
- `resultCollidingSubconstructions : {integer}`: The resulting colliding subconstructions generated by the command (result only).

## JournalBookAssetCommandData (record)

```teal
global record JournalBookAssetCommandData is ICommandData
```

Command data for `cmd.makeJournalBookAssetCmd`.

- `playerEntity : Engine.Entity`: Player entity id.
- `journalEntry : JournalEntry`: Journal entry to add.
- `position : Vec3f`: World position of the journal entry.

## JournalClearAllCommandData (record)

```teal
global record JournalClearAllCommandData is ICommandData
```

Command data for `cmd.makeJournalClearAllCmd`.

## JournalLogEntryCommandData (record)

```teal
global record JournalLogEntryCommandData is ICommandData
```

Command data for `cmd.makeJournalLogEntryCmd`.

- `entity : Engine.Entity`: Entity to log on.
- `logName : string`: Log name.
- `time : number`: Log time.
- `value : integer`: Log value.

## LineCreateCommandData (record)

```teal
global record LineCreateCommandData is ICommandData
```

Command data for `cmd.makeLineCreateCmd`.

- `line : Engine.Component.Line`: Line configuration.
- `name : Engine.Component.Name`: new name component of the line.
- `color : Engine.Component.Color`: new color component of the line.
- `playerOwned : Engine.Component.PlayerOwned`: new player owned component of the line.
- `resultEntity : Engine.Entity`: Created line entity id (result only).

## LineDestroyCommandData (record)

```teal
global record LineDestroyCommandData is ICommandData
```

Command data for `cmd.makeLineDestroyCmd`.

- `lineEntity : Engine.Entity`: Line entity id.

## LineUpdateCommandData (record)

```teal
global record LineUpdateCommandData is ICommandData
```

Command data for `cmd.makeLineUpdateCmd`.

- `lineEntity : Engine.Entity`: Line entity id.
- `line : Engine.Component.Line`: New line component.

## ScriptingSendEventCommandData (record)

```teal
global record ScriptingSendEventCommandData is ICommandData
```

Command data for `cmd.makeScriptingSendEventCmd`.

- `fileName : string`: Filename of the game script that should be the pretended origin of the event.
- `id : string`: Identifier of the element that caused the event.
- `name : string`: Identifier of the event type.
- `param : any`: Parameters that depend on the context of the event.

## StockListDiscardCargoCommandData (record)

```teal
global record StockListDiscardCargoCommandData is ICommandData
```

Command data for `cmd.makeStockListDiscardCargoCmd`.

- `stockListEntity : Engine.Entity`: Stock list entity id.
- `stockIds : {StockId}`: Stock ids to discard.
- `remainingTimeToDelivery : number`: Time until the cargo is delivered.

## StockListSetStocksCargoTypeCommandData (record)

```teal
global record StockListSetStocksCargoTypeCommandData is ICommandData
```

Command data for `cmd.makeStockListSetStocksCargoTypeCmd`.

- `stockListEntity : Engine.Entity`: Stock list entity id.
- `stockIds : {StockId}`: a list of stock ids to set the cargo type
- `cargoType : CargoTypeId`: cargoType the cargo type to set, empty to reset

## StockListSetModifiersCommandData (record)

```teal
global record StockListSetModifiersCommandData is ICommandData
```

Command data for `cmd.makeStockListSetModifiersCmd`.

- `entity : Engine.Entity`: Id of the industry entity of which the departure mode should change.
- `modifiers : Engine.Component.StockList.Modifiers`: a struct containing all modifiers applied to the industry.

## TownConnectWithIndustriesCommandData (record)

```teal
global record TownConnectWithIndustriesCommandData is ICommandData
```

Command data for `cmd.makeTownConnectWithIndustriesCmd`.

- `townEntities : {Engine.Entity}`: Town entity ids to connect.
- `connections : {{integer, integer}}`: Connections as pairs of indices into `townEntities` for start and end destinations
- `keep : boolean`: Whether to keep existing street connections.

## TownAutoDetectConnectionsCommandData (record)

```teal
global record TownAutoDetectConnectionsCommandData is ICommandData
```

Command to automatically detect connection existing between towns.

- `townEntities : {Engine.Entity}`: Entity ids of the towns to detect connections for.

## TownCreateCommandData (record)

```teal
global record TownCreateCommandData is ICommandData
```

Command data for `cmd.makeTownCreateCmd`.

- `towns : {TownInfo}`: Town creation information.
- `updateInitialLandUseCapacities : boolean`: Whether to update initial land use capacities.

## TownDestroyCommandData (record)

```teal
global record TownDestroyCommandData is ICommandData
```

Command data for `cmd.makeTownDestroyCmd`.

- `townEntity : Engine.Entity`: Town entity id.

## TownDevelopAtCommandData (record)

```teal
global record TownDevelopAtCommandData is ICommandData
```

Command data for `cmd.makeTownDevelopAtCmd`.

- `position : Vec2f`: World position where development should be triggered.
- `developStreets : boolean`: Whether to develop streets.
- `upgradeBuildings : boolean`: Whether to upgrade buildings.

## TownCustomDistributionWeightsCommandData (record)

```teal
global record TownCustomDistributionWeightsCommandData is ICommandData
```

Command data for `cmd.makeTownCustomDistributionWeightsCmd`.

- `townEntity : Engine.Entity`: Town entity id.
- `levelToCapacityDistributionWeights : {{number, number, number, number}, {number, number, number, number}, {number, number, number, number}}`: Level-to-capacity distribution weights for each land use type.

## TownSetDevelopmentActiveCommandData (record)

```teal
global record TownSetDevelopmentActiveCommandData is ICommandData
```

Command data for `cmd.makeTownSetDevelopmentActiveCmd`.

- `townEntity : Engine.Entity`: Town entity id.
- `developmentActive : boolean`: Whether development should be set to active or inactive.

## TownSetInitialLandUseCapacitiesCommandData (record)

```teal
global record TownSetInitialLandUseCapacitiesCommandData is ICommandData
```

Command data for `cmd.makeTownSetInitialLandUseCapacitiesCmd`.

- `townEntity : Engine.Entity`: Town entity id.
- `initialLandUseCapacities : {integer}`: Initial capacities for the three land use types (residential, commercial and industrial).

## TownUpdateCargoNeedsCommandData (record)

```teal
global record TownUpdateCargoNeedsCommandData is ICommandData
```

Command data for `cmd.makeTownUpdateCargoNeedsCmd`.

- `townEntity : Engine.Entity`: Town entity id.
- `cargoNeeds : {{CargoTypeId}}`: Cargo needs per district (residential/commercial/industrial).
- `updateTownBuildings : boolean`: Whether to update all buildings to their target capacity.

## TownUpdateSizeCommandData (record)

```teal
global record TownUpdateSizeCommandData is ICommandData
```

Command data for `cmd.makeTownUpdateSizeCmd`.

- `townEntity : Engine.Entity`: Town entity id.
- `landUseSizeFactors : {number}`: Size factors for residential/commercial/industrial.
- `updateAllBuildings : boolean`: Whether to update all buildings to their target capacity.

## TownBuildingSetBlockedDevelopment (record)

```teal
global record TownBuildingSetBlockedDevelopment is ICommandData
```

- `townBuildingEntity : Engine.Entity`: Town building entity id.
- `blockedDevelopment : boolean`: Whether to prevent the town developer from changing the town building

## VehicleBuyCommandData (record)

```teal
global record VehicleBuyCommandData is ICommandData
```

Command data for `cmd.makeVehicleBuyCmd`.

- `playerEntity : Engine.Entity`: Player entity id.
- `depotEntity : Engine.Entity`: Depot entity id.
- `config : TransportVehicleConfig`: config data containing information about the vehicle being purchased.
- `resultVehicleEntity : Engine.Entity`: the entity id of the new vehicle, used only as output when the command is successful.

## VehicleReplaceCommandData (record)

```teal
global record VehicleReplaceCommandData is ICommandData
```

Command data for `cmd.makeVehicleReplaceCmd`.

- `vehicleEntity : Engine.Entity`: Vehicle entity id.
- `config : TransportVehicleConfig`: Bew ransport vehicle configuration.
- `applyPurchaseCost : boolean`: Whether to apply the purchase cost of the new vehicle.

## VehicleReverseCommandData (record)

```teal
global record VehicleReverseCommandData is ICommandData
```

Command data for `cmd.makeVehicleReverseCmd`.

- `vehicleEntity : Engine.Entity`: Vehicle entity id of the vehicle that should be reversed

## VehicleSellCommandData (record)

```teal
global record VehicleSellCommandData is ICommandData
```

Command data for `cmd.makeVehicleSellCmd`.

- `vehicleEntities : {Engine.Entity}`: Vehicle entity ids.

## VehicleSendToDepotCommandData (record)

```teal
global record VehicleSendToDepotCommandData is ICommandData
```

Command data for `cmd.makeVehicleSendToDepotCmd`.

- `vehicleEntity : Engine.Entity`: Vehicle entity id.
- `sellOnArrival : boolean`: if set to true, the vehicle will be sold upon arrival at the depot
- `jumpToDepotEntity : Engine.Entity`: Optional depot entity; if set, the vehicle is teleported to this depot instead of driving there (see `makeVehicleSendToDepotCmd`).

## VehicleSetLineCommandData (record)

```teal
global record VehicleSetLineCommandData is ICommandData
```

Command data for `cmd.makeVehicleSetLineCmd`.

- `vehicleEntity : Engine.Entity`: Vehicle entity id.
- `lineEntity : Engine.Entity`: id of the line on which the vehicle should be sent
- `stopIndex : integer`: the position of the stop on the line to which the vehicle should be sent

## VehicleSetManualDepartureCommandData (record)

```teal
global record VehicleSetManualDepartureCommandData is ICommandData
```

Command data for `cmd.makeVehicleSetManualDepartureCmd`.

- `entity : Engine.Entity`: Id of the vehicle entity of which the departure mode should change
- `manual : boolean`: manual whether the entity should wait at the terminal or not

## VehicleSetModifiersCommandData (record)

```teal
global record VehicleSetModifiersCommandData is ICommandData
```

Command data for `cmd.makeVehicleSetModifiersCmd`.

- `entity : Engine.Entity`: Id of the vehicle entity of which the departure mode should change
- `modifiers : Engine.Component.TransportVehicle.Modifiers`: a struct containing all modifiers applied to the vehicle

## VehicleSetStoppedByUserCommandData (record)

```teal
global record VehicleSetStoppedByUserCommandData is ICommandData
```

Command data for `cmd.makeVehicleSetStoppedByUserCmd`.

- `vehicleEntity : Engine.Entity`: id of the vehicle that should be stopped or started
- `userStopped : boolean`: true if the vehicle should be stopped

## VehicleTryToDepartCommandData (record)

```teal
global record VehicleTryToDepartCommandData is ICommandData
```

Command data for `cmd.makeVehicleTryToDepartCmd`.

- `vehicleEntity : Engine.Entity`: id of the vehicle that should be stopped or started

## WorldBuildProposalCommandData (record)

```teal
global record WorldBuildProposalCommandData is ICommandData
```

Command data for `cmd.makeWorldBuildProposalCmd`.

- `proposal : Proposal`: The definition of the proposal
- `context : Context`: A context with extra options for the build
- `withCostRep : boolean`: tells to ingore costs if set to true
- `ignoreErrors : boolean`: tells to ingore costs if set to true
- `resultProposalData : ProposalData`: The resulting data generated by the proposal
- `resultEntities : {{Engine.Entity, Engine.Revision}}`: only relevant when obtained in callback result, after command has been ran
- `doDust : boolean`: whether dust effect should be created

## WorldChangeWindCommandData (record)

```teal
global record WorldChangeWindCommandData is ICommandData
```

Command data for `cmd.makeWorldChangeWindCmd`.

- `emissionGridEntity : Engine.Entity`: id of the emission grid affected (noise or pollution)
- `wind : Vec2f`: the new wind velocity to set

## WorldReplaceTerrainCommandData (record)

```teal
global record WorldReplaceTerrainCommandData is ICommandData
```

Command data for `cmd.makeWorldReplaceTerrainCmd`.

- `map : GameMap`: Data for the new terrain (heightmap, assets, water level, ...).
- `terrainConfig : BaseConfig.Terrain`: Configuration parameters for the new terrain.
- `seedText : string`: a seed for randomization purposes
- `worldEntity : Engine.Entity`: id of the world, usually 0
- `keepAssets : boolean`: if set to true, existing landscape assets are conserved

## WorldSetBulldozableCommandData (record)

```teal
global record WorldSetBulldozableCommandData is ICommandData
```

Command data for `cmd.makeWorldSetBulldozableCmd`.

- `entity : Engine.Entity`: Id of the entity
- `value : boolean`: true if entity is bulldozable

## SimPersonSetStateCommandDataDEBUG (record)

```teal
global record SimPersonSetStateCommandDataDEBUG is ICommandData
```

Debug command data for `api.cmd.debug.makeSimPersonSetStateCmd`, which forces a new state on a simulated person.

- `entity : Engine.Entity`: Entity id of the simulated person.
- `type : integer`: The state to set.
    - notes: The base debug panel (`gui/debug_panel/make_entity_debug_panel.tl`) has buttons for these values: 0 pick new destinations, 1 new path, 2 jump home, 3 jump shopping, 4 jump working, 5 go home, 6 go shopping, 7 go working.

## ComponentExchangeCommandDataDEBUG (record)

```teal
global record ComponentExchangeCommandDataDEBUG is ICommandData
```

Debug command data for `api.cmd.debug.makeComponentExchangeCmd`, which replaces a component (EmissionEmitter or ModelInstanceList) on an entity.

- `entity : Engine.Entity`: Entity whose component is replaced.
- `component : Engine.Component`: Engine.Component.EmissionEmitter|Engine.Component.ModelInstanceList

## StockSetCargoAmountCommandDataDEBUG (record)

```teal
global record StockSetCargoAmountCommandDataDEBUG is ICommandData
```

Debug command data for `api.cmd.debug.makeStockSetCargoAmountCmd`, which sets the cargo amount of a stock.

- `stockListEntity : Engine.Entity`: Entity that holds the stock list.
- `stock : StockId`: Id of the stock in the stock list.
- `amount : integer`: New cargo amount.
- `cargoType : ResName`: Cargo type resource name (optional in `makeStockSetCargoAmountCmd`).

## GamePerformSimulationStepsCommandDataDEBUG (record)

```teal
global record GamePerformSimulationStepsCommandDataDEBUG is ICommandData
```

Debug command data for `api.cmd.debug.makeGamePerformSimulationStepsCmd`, which advances the simulation.

- `amount : integer`: Number of simulation steps to perform.

## MaintenanceCostUpdateCommandDataDEBUG (record)

```teal
global record MaintenanceCostUpdateCommandDataDEBUG is ICommandData
```

Command data to set new maintenance cost (debug).

- `maintenanceCostEntity : Engine.Entity`: The entity id of the maintenance cost
- `amount : integer`: The new maintenance cost amount
- `resetAccumulatedCost : boolean`: Whether to clean the accumulated cost

## Cmd (record)

```teal
global record Cmd
```
Access: `api.cmd`

contains all commmand factory methods.

- `sendCommand : function<T is ICommandData>(cmd : Command<T>, callback? : function(data : T, success : boolean, resultEntities : {{Engine.Entity, Engine.Revision}}), progress? : CommandProgress)`  (call as `api.cmd.sendCommand(…)`): Sends a command object to be executed later or immediately depending on the state context. After the execution of the command, the callback function is called.
The callback function receives two parameters:
**cmd** the command that has been executed, depending on the context this might contain additional information.
**success** a boolean value to indicate whether the command was successful
    - param `cmd`: the command object to be sent
    - param `callback`: an optional function that is called after the execution of the command
    - param `progress`: an optional progress object to receive progress updates for long running commands

    ```lua
    -- build a construction and get the new entity (from mission_framework_util_entity.tl)
    local proposal = api.type.SimpleProposal.new()
    local ce = api.type.SimpleProposal.ConstructionEntity.new()
    ce.fileName = conFileName  -- name of a .con resource
    ce.transf = transf
    ce.params = params
    ce.name = "My construction"
    ce.playerEntity = -1
    proposal.constructionsToAdd = {ce}
    api.cmd.sendCommand(api.cmd.makeWorldBuildProposalCmd(proposal, nil, true, false),
      function(data, success, resultEntities)
        if success then print("new entity", resultEntities[1][1]) end
      end)
    ```

    - used 293x in the base game, e.g. `base/content/game_mechanics/game_mechanics/celebrations/celebrations.script.tl:193`: `api.cmd.sendCommand(api.cmd.makeScriptingSendEventCmd("", "Towns", "decreaseReputation", {`
- `makeAnimalSetStateCmd : function(animalEntity : Engine.Entity, movementType : integer, targetChangedElapsed : number, invalidTileElapsed : number, movementSpeed : number, angularSpeed : number) : Command<AnimalSetStateCommandData>`  (call as `api.cmd.makeAnimalSetStateCmd(…)`): Makes an AnimalSetState command.
When executed, the animal with the given entity Id will be updated.
    - param `animalEntity`: the entity Id of the animal that should be updated
    - param `movementType`: the index of the configuration in the animals metadata movementTypes list
    - param `targetChangedElapsed`: time since the current target destination was selected
    - param `invalidTileElapsed`: time since the animal got into an invalid area
    - param `movementSpeed`: speed that should be used
    - param `angularSpeed`: angular speed that should be used
    - returns: a newly created AnimalSetState command
- `makeAnimalSpawnAtCmd : function(fileName : string, position : Vec2f, lookAt? : Vec2f) : Command<AnimalSpawnAtCommandData>`  (call as `api.cmd.makeAnimalSpawnAtCmd(…)`): Makes an AnimalSpawnAt command.
When executed, a new animal entity is spawned at the specified position.
    - param `fileName`: reference to the animal model for the animal that should be spawned
    - param `position`: world coordinates of the position where the animal should be spawned
    - param `lookAt`: the direction the animal is facing in
    - returns: a newly created AnimalSpawnAt command
- `makeCustomEntityDestroyCmd : function(entity : Engine.Entity) : Command<CustomEntityDestroyCommandData>`  (call as `api.cmd.makeCustomEntityDestroyCmd(…)`): custom entity
Makes a CustomEntityDestroy command.
When executed, the command removes the custom entity with the given entity Id.
    - param `entity`: the entity Id of the custom entity
    - returns: a newly created CustomEntityDestroy command
    - used 4x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/explorations/exploration_plane.script.tl:192`: `api.cmd.sendCommand(api.cmd.makeCustomEntityDestroyCmd(plane.entity))`
- `makeCustomEntityCreateCmd : function(modelId : integer) : Command<CustomEntityCreateCommandData>`  (call as `api.cmd.makeCustomEntityCreateCmd(…)`): Makes a CustomEntityCreate command.
When executed, the command creates a new custom entity with the given model Id.
    - param `modelId`: the model id of the custom entity
    - returns: a newly created CustomEntityCreate command
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/fun_elements/custom_entity_util.tl:29`: `local cmd = api.cmd.makeCustomEntityCreateCmd(modelId)`
- `makeCustomEntityUpdateStateCmd : function(entity : Engine.Entity, customState : ComponentCustomState) : Command<CustomEntityUpdateStateCommandData>`  (call as `api.cmd.makeCustomEntityUpdateStateCmd(…)`): Makes a CustomEntityUpdateState command.
When executed, the command updates the state of the custom entity.
    - param `entity`: the entity Id of the custom entity
    - param `customState`: the new state of the custom entity
    - returns: a newly created CustomEntityUpdateState command
    - used 6x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/explorations/exploration_plane.script.tl:111`: `api.cmd.sendCommand(api.cmd.makeCustomEntityUpdateStateCmd(plane.entity, {`
- `makeCustomEntityUpdateTransformationCmd : function(entity : Engine.Entity, transf : Mat4f) : Command<CustomEntityUpdateTransformationCommandData>`  (call as `api.cmd.makeCustomEntityUpdateTransformationCmd(…)`): Makes a CustomEntityUpdateTransformation command.
When executed, the command updates the transformation of the custom entity.
    - param `entity`: the entity Id of the custom entity
    - param `transf`: the new transformation of the custom entity
    - returns: a newly created CustomEntityUpdateTransformation command
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/fun_elements/custom_entity_util.tl:38`: `api.cmd.sendCommand(api.cmd.makeCustomEntityUpdateTransformationCmd(spawnedEntity, transf), function()`
- `makeCustomVehicleCreateOrUpdateCmd : function(vehicleEntity : Engine.Entity, carrier : Carrier, vehicles : {{integer, boolean}}, movePathAircraft : Engine.Component.MovePathAircraft | nil) : Command<CustomVehicleCreateCommandData>`  (call as `api.cmd.makeCustomVehicleCreateOrUpdateCmd(…)`): Makes a CustomVehicleCreateOrUpdateCmd command.
When executed, the command updates or creates a custom vehicle entity.
    - param `vehicleEntity`: the entity Id of the custom entity, if nil a new entity will be created
    - param `carrier`: the carrier for the vehicleEntity (currently must be Air)
    - param `vehicles`: a list of carriages, a pair of modelId and whether the model shall be reversed (currently only a single entry)
    - param `movePathAircraft`: optional move path for air and water vehicles (currently path0 and path2 must be empty and the state must be in flight)
    - returns: a newly created CustomEntityUpdateTransformation command
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/explorations/exploration_plane.script.tl:99`: `api.cmd.sendCommand(api.cmd.makeCustomVehicleCreateOrUpdateCmd(nil, api.type["enum"].Carrier.AIR, {{modelId, false}}, mpa),`
- `makeEntitySetColorCmd : function(entity : Engine.Entity, color : Vec3f) : Command<EntitySetColorCommandData>`  (call as `api.cmd.makeEntitySetColorCmd(…)`): entity
Makes an EntitySetColor command.
When executed, the entity Id is set to the given color.
    - param `entity`: the entity Id of the entity that should be colored
    - param `color`: the rgb color
    - returns: a newly created EntitySetColor command
    - used 5x in the base game, e.g. `base/content/gui/gui/entity_window/line/line.tl:85`: `return api.cmd.makeEntitySetColorCmd(entityId, color)`
- `makeEntitySetEmissionsCmd : function(entity : Engine.Entity, noisePower : number, pollutionPower : number, radius : number, pollutionRadius : number) : Command<EntitySetEmissionsCommandData>`  (call as `api.cmd.makeEntitySetEmissionsCmd(…)`): Makes an EntitySetEmissions command.
When executed, the emission values of the entity are set to the given values.
    - param `entity`: the entity Id of the entity that should be updated
    - param `noisePower`: the noise power of the entity
    - param `pollutionPower`: the pollution power of the entity
    - param `radius`: the radius of the noise emissions
    - param `pollutionRadius`: the radius of the pollution emissions
    - returns: a newly created EntitySetEmissions command
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/emission/emissions.script.tl:130`: `api.cmd.sendCommand(api.cmd.makeEntitySetEmissionsCmd(subEntity, noisePower * missionEmissionsFactor, pollutionPower * missionEmissionsFactor, radius, pollution`
- `makeEntitySetNameCmd : function(entity : Engine.Entity, name : string, forceSameEntity? : boolean) : Command<EntitySetNameCommandData>`  (call as `api.cmd.makeEntitySetNameCmd(…)`): Makes an EntitySetName command.
When executed, the entity Id is renamed with the given name.
    - param `entity`: the entity Id of the entity that should be renamed
    - param `name`: the new name that should be set
    - param `forceSameEntity`: (optional, default false) Unless true, things like stations that don't have a name on their own will set the stem name instead.
    - returns: a newly created EntitySetName command
    - used 14x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company.tl:941`: `return api.cmd.makeEntitySetNameCmd(playerEntity, name)`
- `makeEntitySetPlayerCmd : function(entity : Engine.Entity, player : Engine.Entity) : Command<EntitySetPlayerCommandData>`  (call as `api.cmd.makeEntitySetPlayerCmd(…)`): Makes an EntitySetPlayer command.
When executed, the entity is set or unset to the player
    - param `entity`: the entity Id of the entity
    - param `player`: the entity Id of the player
    - returns: a newly created EntitySetPlayer command
    - used 15x in the base game, e.g. `base/content/gui/gui/debug_panel/make_entity_debug_panel.tl:422`: `api.cmd.sendCommand(api.cmd.makeEntitySetPlayerCmd(e, -1))`
- `makeGameAddPlayerCmd : function(name : string, color : Vec3f) : Command<GameAddPlayerCommandData>`  (call as `api.cmd.makeGameAddPlayerCmd(…)`): game
Makes a GameAddPlayer command.
When executed, a new player entity is created.
    - param `name`: the player's name
    - param `color`: the player's color
    - returns: a newly created GameAddPlayer command
    - used 1x in the base game, e.g. `base/content/mission/mission/tasks/transfer_ownership/transfer_ownership_util.tl:5`: `local cmd = api.cmd.makeGameAddPlayerCmd("aiPlayer", api.type.Vec3f.new(1.0, 1.0, 1.0))`
- `makeGameSetCalendarSpeedCmd : function(millisPerDay : integer) : Command<GameSetCalendarSpeedCommandData>`  (call as `api.cmd.makeGameSetCalendarSpeedCmd(…)`): Makes a GameSetCalendarSpeed command.
When executed, the calendar speed is set to the given value (this affects only the speed of the calendar, and not the simulation speed).
    - param `millisPerDay`: day length in milliseconds
    - returns: a newly created GameSetCalendarSpeed command
    - used 17x in the base game, e.g. `base/content/gui/gui/game_bar/game_bar_widgets.tl:344`: `return api.cmd.makeGameSetCalendarSpeedCmd(newState)`
- `makeGameSetDateCmd : function(date : Date) : Command<GameSetDateCommandData>`  (call as `api.cmd.makeGameSetDateCmd(…)`): Makes a GameSetDate command.
When executed, the calendar date (for vehicle availability, etc.) is set to the given date.
    - param `date`: the new date that should be set
    - returns: a newly created GameSetDate command
    - used 4x in the base game, e.g. `base/content/gui/gui/game_bar/game_bar_widgets.tl:680`: `return api.cmd.makeGameSetDateCmd(date)`
- `makeGameSetSpeedCmd : function(speedup : integer) : Command<GameSetSpeedCommandData>`  (call as `api.cmd.makeGameSetSpeedCmd(…)`): Makes a GameSetSpeed command.
When executed, the game (simulation) speed is set to the given value.
    - param `speedup`: a positive number: the speed that the simulation will run at
    - returns: a newly created GameSetSpeed command
    - used 13x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company.script.tl:403`: `api.cmd.sendCommand(api.cmd.makeGameSetSpeedCmd(1))`
- `makeGameSetTimeOfDayCmd : function(timeOfDaySec : integer) : Command<GameSetTimeOfDayCommandData>`  (call as `api.cmd.makeGameSetTimeOfDayCmd(…)`): Makes a GameSetTimeOfDay command.
    - param `timeOfDaySec`: a positive number: the time of day in seconds
    - returns: a newly created GameSetTimeOfDay command
    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/game_time/game_time.script.tl:195`: `api.cmd.sendCommand(api.cmd.makeGameSetTimeOfDayCmd(60 * (60 * date.hour + date.min) + date.sec))`
- `makeGameSetCloudCoverageCmd : function(cloudCoverage : number) : Command<GameSetCloudCoverageCommandData>`  (call as `api.cmd.makeGameSetCloudCoverageCmd(…)`): Makes a GameSetCloudCoverage command.
    - param `cloudCoverage`: the current cloud coverage
    - returns: a newly created GameSetCloudCoverage command
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/game_time/game_time.script.tl:259`: `api.cmd.sendCommand(api.cmd.makeGameSetCloudCoverageCmd(newCloudCoverage))`
- `makeIndustrySetDespawnTimeCmd : function(industryEntity : Engine.Entity, timeStamp : integer) : Command<IndustrySetDespawnTimeCommandData>`  (call as `api.cmd.makeIndustrySetDespawnTimeCmd(…)`): industry
Makes an IndustrySetDespawnTime command.
When executed, the command sets a new value to the closure time stamp of the given industry.
    - param `industryEntity`: the entity Id of the industry
    - param `timeStamp`: sets the despawn time of the industry
    - returns: a newly created IndustrySetDespawnTime command
    - used 4x in the base game, e.g. `base/content/game_mechanics/game_mechanics/industries/industries.script.tl:101`: `api.cmd.sendCommand(api.cmd.makeIndustrySetDespawnTimeCmd(entityId, 0))`
- `makeIndustrySetManualDevelopmentCmd : function(industryEntity : Engine.Entity, manual : boolean) : Command<IndustrySetManualDevelopmentCommandData>`  (call as `api.cmd.makeIndustrySetManualDevelopmentCmd(…)`): Makes an IndustrySetManualDevelopment command.
When executed, the command sets the industry to manual mode if manual is true, or to automatic development mode if manual is set to false (default).
    - param `industryEntity`: the entity Id of the industry
    - param `manual`: set to true if industries do not start to close/stop countdown and do not level up or down
    - returns: a newly created IndustrySetManualDevelopment command
- `makeCreateIndustryExtendProposalCmd : function(extendConstruction : Engine.Entity, numTrySlots : integer, numKeepSlots : integer) : Command<CreateIndustryExtendProposalCommandData>`  (call as `api.cmd.makeCreateIndustryExtendProposalCmd(…)`): Makes a CreateIndustryExtendProposal command.
When executed, the command generates a new proposal to extend the given industry.
    - param `extendConstruction`: the entity Id of the industry that should be extended
    - param `numTrySlots`: the number of slots to try to extend
    - param `numKeepSlots`: the number of slots to keep after the extension
    - returns: a newly created CreateIndustryExtendProposal command
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/industry/industry.tl:95`: `api.cmd.sendCommand(api.cmd.makeCreateIndustryExtendProposalCmd(constructionEntity, 0, 1), function(res : CreateIndustryExtendProposalCommandData, success : boo`
- `makeJournalBookAssetCmd : function(player : Engine.Entity, entry : JournalEntry, position? : Vec3f) : Command<JournalBookAssetCommandData>`  (call as `api.cmd.makeJournalBookAssetCmd(…)`): journal
Makes a JournalBookAsset command.
When executed, the new journal entry is added to the players journal.
    - param `player`: the entity Id of the player
    - param `entry`: the entry to add
    - param `position`: a position for the event
    - returns: a newly created JournalBookAsset command
    - used 18x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/loan.script.tl:101`: `local loanCmd = api.cmd.makeJournalBookAssetCmd(api.engine.util.getPlayer(), loanEntry, api.type.Vec3f.new(0, 0, 0))`
- `makeJournalClearAllCmd : function() : Command<JournalClearAllCommandData>`  (call as `api.cmd.makeJournalClearAllCmd(…)`): Makes a JournalClearAll command.
When executed, the journal and logbook is cleared
    - returns: a newly created JournalClearAll command
    - used 8x in the base game, e.g. `mods/release/urbangames_campaign_mission_01/content/mission/mission/tasks/everything/everything.tl:39`: `local cmd = api.cmd.makeJournalClearAllCmd()`
- `makeJournalLogEntryCmd : function(entity : Engine.Entity, logNamesAndValues : {{string, integer, boolean}}) : Command<JournalLogEntryCommandData>`  (call as `api.cmd.makeJournalLogEntryCmd(…)`): Makes a JournalLogEntry command.
When executed, the log book value is logged.
    - param `entity`: the entity Id of the logbook
    - param `logNamesAndValues`: a list of log names and values to log, where each entry contains the log name, value and whether the value is incremental
    - returns: a newly created JournalLogEntry command
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/industries/industries.script.tl:219`: `api.cmd.sendCommand(api.cmd.makeJournalLogEntryCmd(api.engine.util.getWorld(), logsToAdd))`
- `makeLineCreateCmd : function(name : string, color : Vec3f, player : Engine.Entity, line : Engine.Component.Line) : Command<LineCreateCommandData>`  (call as `api.cmd.makeLineCreateCmd(…)`): line
Makes a LineCreate command.
When executed, creates a new line with the given parameters.
    - param `name`: the name of the new line
    - param `color`: the color of the new line
    - param `player`: the entity Id of the player owning the line
    - param `line`: the configuration data for the line
    - returns: a newly created LineCreate command
    - used 2x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:6537`: `cmd = api.cmd.makeLineCreateCmd(`
- `makeLineDestroyCmd : function(lineEntity : Engine.Entity) : Command<LineDestroyCommandData>`  (call as `api.cmd.makeLineDestroyCmd(…)`): Makes a LineDestroy command.
When executed, the line with the given entity Id is removed.
    - param `lineEntity`: the line that should be removed
    - returns: a newly created LineDestroy command
    - used 4x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:2248`: `local destroyCmd = api.cmd.makeLineDestroyCmd(entity)`
- `makeLineUpdateCmd : function(lineEntity : Engine.Entity, data : Engine.Component.Line) : Command<LineUpdateCommandData>`  (call as `api.cmd.makeLineUpdateCmd(…)`): Makes a LineUpdate command.
When executed, the line with the given entity Id is updated to the new line configuration.
    - param `lineEntity`: the entity Id of the line which should be updated
    - param `data`: the new line configuration
    - returns: a newly created LineUpdate command
    - used 5x in the base game, e.g. `base/content/gui/gui/entity_window/line/line.tl:33`: `return api.cmd.makeLineUpdateCmd(params.lineEntityId, lineData)`
- `makeScriptingSendEventCmd : function(src : string, id : string, name : string, param : any) : Command<ScriptingSendEventCommandData>`  (call as `api.cmd.makeScriptingSendEventCmd(…)`): scripting
Makes a ScriptingSendEvent command.
When executed, a script event will be broadcasted to all scripts.
    - param `src`: the filename of the game_script that should be the pretended origin of the event
    - param `id`: identifier of the element that caused the event
    - param `name`: identifier of the event type
    - param `param`: parameters that depend on the context of the event
    - returns: a newly created ScriptingSendEvent command
    - used 111x in the base game, e.g. `base/content/game_mechanics/game_mechanics/celebrations/celebrations.script.tl:193`: `api.cmd.sendCommand(api.cmd.makeScriptingSendEventCmd("", "Towns", "decreaseReputation", {`
- `makeStockListDiscardCargoCmd : function(stockListEntity : Engine.Entity, stockIds : {StockId}, remainingTimeToDelivery : number) : Command<StockListDiscardCargoCommandData>`  (call as `api.cmd.makeStockListDiscardCargoCmd(…)`): stocklist
Makes a StockListDiscardCargo command.
When executed, discards the cargo of the given stock list.
    - param `stockListEntity`: the entity Id of the stock list
    - param `stockIds`: the list of stock ids
    - param `remainingTimeToDelivery`: the time remaining to delivery
    - returns: a newly created StockListDiscardCargo command
    - used 2x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_util.tl:427`: `return api.cmd.makeStockListDiscardCargoCmd(params.entity, params.config.ids, 1.0)`
- `makeStockListSetStocksCargoTypeCmd : function(stockListEntity : Engine.Entity, stockIds : {StockId}, cargoType? : CargoTypeId) : Command<StockListSetStocksCargoTypeCommandData>`  (call as `api.cmd.makeStockListSetStocksCargoTypeCmd(…)`): Makes a StockListSetStocksCargoType command.
When executed, sets or resets the cargo type of the given stocks.
    - param `stockListEntity`: the entity Id of the stock list
    - param `stockIds`: the list of stock ids
    - param `cargoType`: the cargo type to set, empty if the cargo type should be reset
    - returns: a newly created StockListSetStocksCargoType command
- `makeStockListSetModifiersCmd : function(industryEntity : Engine.Entity, modifiers : Engine.Component.StockList.Modifiers) : Command<StockListSetModifiersCommandData>`  (call as `api.cmd.makeStockListSetModifiersCmd(…)`): Makes a StockListSetModifiers command.
Allows changing some properties of industries
    - param `industryEntity`: the entity Id of the industry of which the departure mode is to be changed
    - param `modifiers`: a struct with the changes to the industry parameters
    - returns: a newly created StockListSetModifiers command
    - used 1x in the base game, e.g. `base/content/industries/industry_workers.script.tl:13`: `api.cmd.sendCommand(api.cmd.makeStockListSetModifiersCmd(entity, newModifiers))`
- `makeTownConnectWithIndustriesCmd : function(townEntities : {Engine.Entity}, connections : {{integer, integer}}, keep : boolean) : Command<TownConnectWithIndustriesCommandData>`  (call as `api.cmd.makeTownConnectWithIndustriesCmd(…)`): town
Makes a TownConnectWithIndustries command.
When executed, the command generates a new street network connecting the given towns and all existing industries.
    - param `townEntities`: the entity Ids of the towns to connect together
    - param `connections`: list with indices into the vector townEntities specifying which towns are to be connected
    - param `keep`: set to true if existing street connections should be kept, otherwise the streets will be removed
    - returns: a newly created TownConnectWithIndustries command
    - used 1x in the base game, e.g. `base/content/gui/gui/map_editor/map_editor.tl:2632`: `api.cmd.sendCommand(api.cmd.makeTownConnectWithIndustriesCmd(townEntities, conns, fullState:old().keep), makeTownConnectWithIndustriesCmdCallback, a)`
- `makeTownAutoDetectConnectionsCmd : function(townEntities : {Engine.Entity}) : Command`  (call as `api.cmd.makeTownAutoDetectConnectionsCmd(…)`): Makes a TownAutoDetectConnections command.
When executed, the command tries to detect if an existing connections between towns exists
    - param `townEntities`: the entity Ids of the towns to check for connections
    - returns: a newly created TownAutoDetectConnections command
    - used 1x in the base game, e.g. `base/content/gui/gui/map_editor/map_editor.tl:2504`: `api.cmd.sendCommand(api.cmd.makeTownAutoDetectConnectionsCmd(townEntities), autoDetectCallback, a)`
- `makeTownCreateCmd : function(towns : {TownInfo}) : Command<TownCreateCommandData>`  (call as `api.cmd.makeTownCreateCmd(…)`): Makes a TownCreate command.
When executed, the command initiates the generation of new towns using information contained in TownInfo.
    - param `towns`: a list of town creation information, one for each new town
    - returns: a newly created TownCreate command
    - used 1x in the base game, e.g. `base/content/gui/gui/map_editor/map_editor.tl:1636`: `api.cmd.sendCommand(api.cmd.makeTownCreateCmd(townResult.toAdd), onTownCreateCallback, a)`
- `makeTownDestroyCmd : function(townEntity : Engine.Entity) : Command<TownDestroyCommandData>`  (call as `api.cmd.makeTownDestroyCmd(…)`): Makes a TownDestroy command.
When executed, the town with the given entity Id is removed (all streets and buildings belonging to the town will be removed as well).
    - param `townEntity`: the entity Id of the town that should be removed
    - returns: a newly created TownDestroy command
    - used 2x in the base game, e.g. `base/content/gui/gui/map_editor/map_editor.tl:772`: `api.cmd.sendCommand(api.cmd.makeTownDestroyCmd(townEntity), (i == #townsToRemove) and commandCallback or nil)`
- `makeTownDevelopAtCmd : function(position : Vec2f, developStreets : boolean, upgradeBuildings : boolean) : Command<TownDevelopAtCommandData>`  (call as `api.cmd.makeTownDevelopAtCmd(…)`): Makes a TownDevelopAt command.
When executed, the command triggers development of towns at the given position.
By developing towns, new buildings or street segments are added at the given position.
    - param `position`: the position where development should be triggered
    - param `developStreets`: develop streets
    - param `upgradeBuildings`: upgrade buildings
    - returns: a newly created TownDevelopAt command
- `makeTownCustomDistributionWeightsCmd : function(townEntitiy : Engine.Entity, levelToCapacityDistributionWeights? : {{number}}) : Command<TownCustomDistributionWeightsCommandData>`  (call as `api.cmd.makeTownCustomDistributionWeightsCmd(…)`): Makes a TownCustomDistributionWeights command.
When executed, the command sets the level to capacities distribution weights for the town.
    - param `townEntitiy`: the entity Id of the town
    - param `levelToCapacityDistributionWeights`: level to capacities distribution weights for each land use type
    - returns: a newly created TownCustomDistributionWeights command
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/town/town_eow.script.tl:731`: `return api.cmd.makeTownCustomDistributionWeightsCmd(params.keyEntity, state.levelToCapacityDistributionWeights)`
- `makeTownSetDevelopmentActiveCmd : function(entity : Engine.Entity, developmentActive : boolean) : Command<TownSetDevelopmentActiveCommandData>`  (call as `api.cmd.makeTownSetDevelopmentActiveCmd(…)`): Makes a TownSetDevelopmentActive command.
When executed, the command enables/disables the development of a town.
    - param `entity`: the entity Id of the town
    - param `developmentActive`: signals whether the town development should be enabled/disabled
    - returns: a newly created TownSetDevelopmentActive command
    - used 2x in the base game, e.g. `base/content/gui/gui/entity_window/town/town_eow.script.tl:1035`: `return api.cmd.makeTownSetDevelopmentActiveCmd(params.keyEntity, state.isDevelopmentActive)`
- `makeTownSetInitialLandUseCapacitiesCmd : function(entity : Engine.Entity, landUseCapacities? : {integer}) : Command<TownSetInitialLandUseCapacitiesCommandData>`  (call as `api.cmd.makeTownSetInitialLandUseCapacitiesCmd(…)`): Makes a TownSetInitialLandUseCapacities command.
When executed, the town with the given entity Id is set to the given initial capacities.
    - param `entity`: the entity Id of the town that should be updated
    - param `landUseCapacities`: initial capacities for the residential, commercial and industrial. If not provided, capacities are automatically updated
    - returns: a newly created TownSetInitialLandUseCapacities command
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/town/town_eow.script.tl:1020`: `return api.cmd.makeTownSetInitialLandUseCapacitiesCmd(params.keyEntity, newInitialLandUseCapacities)`
- `makeTownUpdateCargoNeedsCmd : function(entity : Engine.Entity, cargoNeeds : {{CargoTypeId}}, updateTownBuildings : boolean) : Command<TownUpdateCargoNeedsCommandData>`  (call as `api.cmd.makeTownUpdateCargoNeedsCmd(…)`): Makes a TownUpdateCargoNeeds command.
When executed, the cargo needs of the town districts are updated to the provided needs.
    - param `entity`: the entity Id of the town that should be updated
    - param `cargoNeeds`: a list of cargo type Ids for the needs of each of the three town districts residential, commercial and industrial
    - param `updateTownBuildings`: whether to update all buildings immediately
    - returns: a newly created TownUpdateCargoNeeds command
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_growth.script.tl:279`: `api.cmd.sendCommand(api.cmd.makeTownUpdateCargoNeedsCmd(townEntity, oldCargoNeeds, true))`
- `makeTownUpdateSizeCmd : function(townEntity : Engine.Entity, sizeFactors : {number}, updateAllBuildings : boolean) : Command<TownUpdateSizeCommandData>`  (call as `api.cmd.makeTownUpdateSizeCmd(…)`): Makes a TownUpdateSize command.
When executed, the town with the given entity Id is set to the given initial capacities.
    - param `townEntity`: the entity Id of the town that should be updated
    - param `sizeFactors`: initial capacities for the residential, commercial and industrial districts of the town
    - param `updateAllBuildings`: whether to update all buildings immediately
    - returns: a newly created TownUpdateSize command
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_growth.script.tl:550`: `api.cmd.sendCommand(api.cmd.makeTownUpdateSizeCmd(townEntity, baseTownSize, false))`
- `makeTownBuildingSetBlockedDevelopmentCmd : function(townBuildingEntity : Engine.Entity, blockedDevelopment : boolean) : Command<TownBuildingSetBlockedDevelopment>`  (call as `api.cmd.makeTownBuildingSetBlockedDevelopmentCmd(…)`): Makes a TownBuildingSetBlockedDevelopment command.
When executed, makes buildings "Historic" thatn cannot be changed
    - param `townBuildingEntity`: the entity Id of the town building that should be updated
    - param `blockedDevelopment`: decides whether to prevent the town developer from changing building
    - returns: a newly created TownBuildingSetBlockedDevelopment command
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/town_building/town_building.tl:206`: `return api.cmd.makeTownBuildingSetBlockedDevelopmentCmd(params.entityId, newState)`
- `makeVehicleBuyCmd : function(playerEntity : Engine.Entity, depotEntity : Engine.Entity, tvc : TransportVehicleConfig) : Command<VehicleBuyCommandData>`  (call as `api.cmd.makeVehicleBuyCmd(…)`): vehicle
Makes a VehicleBuy command.
When executed, a new vehicle is bought and placed in the given depot.
    - param `playerEntity`: the entity Id of the player
    - param `depotEntity`: the entity Id of the depot in which the vehicle will appear
    - param `tvc`: data containing information about the vehicle being purchased
    - returns: a newly created VehicleBuy command
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_react_util.tl:350`: `api.cmd.makeVehicleBuyCmd(`
- `makeVehicleReplaceCmd : function(vehicleEntity : Engine.Entity, tvc : TransportVehicleConfig) : Command<VehicleReplaceCommandData>`  (call as `api.cmd.makeVehicleReplaceCmd(…)`): Makes a VehicleReplace command.
When executed, the vehicle with the given entity Id will be replaced by the new vehicle configuration.
    - param `vehicleEntity`: the entity Id of the vehicle that should be replaced
    - param `tvc`: data containing information about the new vehicle being purchased
    - returns: a newly created VehicleReplace command
    - used 2x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_react_util.tl:407`: `api.cmd.sendCommand(api.cmd.makeVehicleReplaceCmd(change.vehicleEntity, change.config))`
- `makeVehicleReverseCmd : function(vehicleEntity : Engine.Entity) : Command<VehicleReverseCommandData>`  (call as `api.cmd.makeVehicleReverseCmd(…)`): Makes a VehicleReverse command.
When executed, the vehicle with the given entity Id will be reversed.
    - param `vehicleEntity`: the entity Id of the vehicle that should be reversed
    - returns: a newly created VehicleReverse command
    - used 2x in the base game, e.g. `base/content/gui/gui/entity_window/vehicle/vehicle.tl:317`: `return api.cmd.makeVehicleReverseCmd(vehicleEntityId)`
- `makeVehicleSellCmd : function(vehicleEntities : {Engine.Entity}) : Command<VehicleSellCommandData>`  (call as `api.cmd.makeVehicleSellCmd(…)`): Makes a VehicleSell command.
When executed, the vehicle with the given entity Id will be sold.
    - param `vehicleEntities`: the entity ids of the vehicles which should be sold
    - returns: a newly created VehicleSell command
    - used 9x in the base game, e.g. `base/content/gui/gui/entity_window/vehicle/vehicle.tl:361`: `return api.cmd.makeVehicleSellCmd({vehicleEntityId})`
- `makeVehicleSendToDepotCmd : function(vehicleEntity : Engine.Entity, sellOnArrival : boolean, jumpToDepoEntity? : Engine.Entity) : Command<VehicleSendToDepotCommandData>`  (call as `api.cmd.makeVehicleSendToDepotCmd(…)`): Makes a VehicleSendToDepot command.
When executed, the vehicle with the given entity Id will be sent to the nearest reachable depot and (optionally) be sold.
    - param `vehicleEntity`: the entity Id of the vehicle that should be sent to the depot
    - param `sellOnArrival`: if set to true, the vehicle will be sold upon arrival at the depot
    - param `jumpToDepoEntity`: optional. If provided, then vehicle gets "teleported" to depot
    - returns: a newly created VehicleSendToDepot command
    - used 4x in the base game, e.g. `base/content/gui/gui/entity_window/vehicle/vehicle.tl:343`: `return api.cmd.makeVehicleSendToDepotCmd(vehicleEntityId, false)`
- `makeVehicleSetLineCmd : function(vehicleEntity : Engine.Entity, lineEntity : Engine.Entity, stopIndex : integer) : Command<VehicleSetLineCommandData>`  (call as `api.cmd.makeVehicleSetLineCmd(…)`): Makes a VehicleSetLine command.
When executed, the vehicle with the provided entity Id is assigned to the line with given entity Id and sent to the stop with the given index.
    - param `vehicleEntity`: the entity Id of the vehicle that should be assigned to the line
    - param `lineEntity`: the entity Id of the line on which the vehicle should be sent
    - param `stopIndex`: the position of the stop on the line to which the vehicle should be sent to
    - returns: a newly created VehicleSetLine command
    - used 5x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:4753`: `api.cmd.makeVehicleSetLineCmd(`
- `makeVehicleSetManualDepartureCmd : function(vehicleEntity : Engine.Entity, manual : boolean) : Command<VehicleSetManualDepartureCommandData>`  (call as `api.cmd.makeVehicleSetManualDepartureCmd(…)`): Makes a VehicleSetManualDeparture command.
When an entity with component type TRANSPORT_VEHICLE is set to manual departure
it will not depart from the terminal under any circumstance (the callback will always return success).
If the entity is not a transport vehicle, the command callback will return False.
    - param `vehicleEntity`: the entity Id of the vehicle of which the departure mode is to be changed
    - param `manual`: whether the entity is managed or not
    - returns: a newly created VehicleSetManualDeparture command
- `makeVehicleSetStoppedByUserCmd : function(vehicleEntity : Engine.Entity, stopped : boolean) : Command<VehicleSetStoppedByUserCommandData>`  (call as `api.cmd.makeVehicleSetStoppedByUserCmd(…)`): Makes a VehicleSetStoppedByUser command.
When executed, the given vehicle is stopped or started as if the user controlled it in the vehicle dialog.
    - param `vehicleEntity`: the entity Id of the vehicle that should be stopped or started
    - param `stopped`: stopped true if the vehicle should be stopped
    - returns: a newly created VehicleSetStoppedByUser command
    - used 6x in the base game, e.g. `base/content/gui/gui/entity_window/vehicle/vehicle.tl:330`: `return api.cmd.makeVehicleSetStoppedByUserCmd(vehicleEntityId, not isStopped)`
- `makeVehicleSetModifiersCmd : function(vehicleEntity : Engine.Entity, modifiers : Engine.Component.TransportVehicle.Modifiers) : Command<VehicleSetModifiersCommandData>`  (call as `api.cmd.makeVehicleSetModifiersCmd(…)`): Makes a VehicleSetModifiers command.
Allows changing some properties of transport vehicles
    - param `vehicleEntity`: the entity Id of the vehicle of which the departure mode is to be changed
    - param `modifiers`: a struct with the changes to the transport vehicle parameters
    - returns: a newly created VehicleSetModifiers command
    - used 2x in the base game, e.g. `base/content/vehicle/vehicle_modifier.script.tl:97`: `api.cmd.sendCommand(api.cmd.makeVehicleSetModifiersCmd(param.entity, api.type.TransportVehicle.Modifiers.new()))`
- `makeVehicleTryToDepartCmd : function(vehicleEntity : Engine.Entity) : Command<VehicleTryToDepartCommandData>`  (call as `api.cmd.makeVehicleTryToDepartCmd(…)`): Makes a VehicleTryToDepart command.
When executed, and the vehicle with entity Id is at the terminal, the vehicle will immediately close
the door and depart as soon as the doors are closed. The vehicle will ignore any departure condition and depart immediately.
The command callback will return success if the entity has a TRANSPORT_VEHICLE component and
is currently waiting at the terminal.
    - param `vehicleEntity`: the entity Id of the vehicle at terminal that should depart
    - returns: a newly created VehicleTryToDepart command
- `makeWorldBuildProposalCmd : function(proposal : Proposal, context : any, ignoreErrors : boolean, playerInitiated : boolean, doDust? : boolean) : Command<WorldBuildProposalCommandData>`  (call as `api.cmd.makeWorldBuildProposalCmd(…)`): world
Makes a WorldBuildProposal command. This command is used to create new constructions and new streets via scripting.
When executed, the given build proposal is built. The command might fail resulting in the proposal not being built.
    - param `proposal`: a proposal containing a preview of the changes that will be applied with this command
    - param `context`: a context with extra options for the proposal
    - param `ignoreErrors`: a flag enabling the building of the proposal even if non-critical errors are triggered
    - param `playerInitiated`: whether the command was initiated by a player
    - param `doDust`: whether dust effect should be created
    - returns: a newly created WorldBuildProposal command
    - notes: Two overloads: one takes a full `Proposal`, the other a `SimpleProposal` (`api.type.SimpleProposal.new()`), which lists constructions to add (`constructionsToAdd`, built from `SimpleProposal.ConstructionEntity`), constructions to remove (`constructionsToRemove`) and the old-to-new mapping (`old2new`). The base game uses the `SimpleProposal` form to build, replace and bulldoze constructions from scripts. `context` may be `nil`.

    ```lua
    -- bulldoze a construction (mission_framework_util_entity.tl)
    local proposal = api.type.SimpleProposal.new()
    proposal.constructionsToRemove = {entity}
    api.cmd.sendCommand(api.cmd.makeWorldBuildProposalCmd(proposal, nil, true, false))
    ```

    - used 37x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company.script.tl:212`: `api.cmd.sendCommand(api.cmd.makeWorldBuildProposalCmd(proposal, nil, false, false), callback)`
- `makeWorldBuildProposalCmd : function(proposal : SimpleProposal, context : any, ignoreErrors : boolean, playerInitiated : boolean, doDust? : boolean) : Command<WorldBuildProposalCommandData>`  (call as `api.cmd.makeWorldBuildProposalCmd(…)`)
    - notes: Two overloads: one takes a full `Proposal`, the other a `SimpleProposal` (`api.type.SimpleProposal.new()`), which lists constructions to add (`constructionsToAdd`, built from `SimpleProposal.ConstructionEntity`), constructions to remove (`constructionsToRemove`) and the old-to-new mapping (`old2new`). The base game uses the `SimpleProposal` form to build, replace and bulldoze constructions from scripts. `context` may be `nil`.

    ```lua
    -- bulldoze a construction (mission_framework_util_entity.tl)
    local proposal = api.type.SimpleProposal.new()
    proposal.constructionsToRemove = {entity}
    api.cmd.sendCommand(api.cmd.makeWorldBuildProposalCmd(proposal, nil, true, false))
    ```

    - used 37x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company.script.tl:212`: `api.cmd.sendCommand(api.cmd.makeWorldBuildProposalCmd(proposal, nil, false, false), callback)`
- `makeWorldChangeWindCmd : function(emissionGridEntity : Engine.Entity, wind : Vec2f) : Command<WorldChangeWindCommandData>`  (call as `api.cmd.makeWorldChangeWindCmd(…)`): Makes a WorldChangeWind command.
When executed, the EmissionGrid with the given entity Id is updated to run with the new wind given.
    - param `emissionGridEntity`: the entity Id of the game's emission grid
    - param `wind`: the new wind
    - returns: a newly created WorldChangeWind command
- `makeWorldReplaceTerrainCmd : function(map : GameMap, terrainConfig : BaseConfig.Terrain, seedText : string, worldEntity : Engine.Entity, keepAssets : boolean) : Command<WorldReplaceTerrainCommandData>`  (call as `api.cmd.makeWorldReplaceTerrainCmd(…)`): Makes a WorldReplaceTerrain command.
When executed, the terrain is replaced. This is similar to the generation in map editor.
    - param `map`: an object that specifies the data for the new terrain (heightmap, assets, water level...)
    - param `terrainConfig`: configuration parameters for the new terrain
    - param `seedText`: a seed for randomization purposes
    - param `worldEntity`: the entity Id of the world, usually 0
    - param `keepAssets`: if set to true, existing landscape assets are conserved
    - returns: a newly created WorldReplaceTerrain command
    - used 3x in the base game, e.g. `base/content/gui/gui/map_editor/map_editor.tl:796`: `api.cmd.sendCommand(api.cmd.makeWorldReplaceTerrainCmd(mapCopy, api.res.getBaseConfig().terrain, fullState:old().seed, api.engine.util.getWorld(), fullState:old`
- `makeWorldSetBulldozableCmd : function(entity : Engine.Entity, bulldozable : boolean) : Command<WorldSetBulldozableCommandData>`  (call as `api.cmd.makeWorldSetBulldozableCmd(…)`): Makes a WorldSetBulldozable command.
Sets whether an entity can be bulldozed or not.
    - param `entity`: entity ID
    - param `bulldozable`: whether bulldozeable or not
    - returns: a newly created WorldSetBulldozable command
    - used 2x in the base game, e.g. `base/content/mission/mission/mission_sim.script.tl:783`: `api.cmd.sendCommand(api.cmd.makeWorldSetBulldozableCmd(actual, false))`
- `debug : Debug`: Debug command helpers (`api.cmd.debug`), e.g. `api.cmd.debug.makeStockSetCargoAmountCmd`.

    ```lua
    api.cmd.sendCommand(api.cmd.debug.makeSimPersonSetStateCmd(personEntity, 2)) -- 2: jump home
    ```

### Cmd.Debug (record)

```teal
record Debug
```
Access: `api.cmd.debug`

Debug command helpers.

- `makeMaintenanceCostUpdateCmd : function(maintenanceCostEntity : Engine.Entity, amount : integer, resetAccumulatedCost : boolean) : Command`  (call as `api.cmd.debug.makeMaintenanceCostUpdateCmd(…)`): Makes a MaintenanceCostUpdate command.
When executed, the maintenance cost amount is updated
    - param `maintenanceCostEntity`: the entity Id of the maintenance cost about to change
    - param `amount`: of the new maintenance cost
    - param `reset`: the accumulated cost
    - returns: a newly created MaintenanceCostUpdate command
    - used 1x in the base game, e.g. `base/content/mission/mission/mission_savegame_util.tl:19`: `api.cmd.sendCommand(api.cmd.debug.makeMaintenanceCostUpdateCmd(entity, 0, true))`
- `makeSimPersonSetStateCmd : function(entity : Engine.Entity, simPersonState : integer) : Command<SimPersonSetStateCommandDataDEBUG>`  (call as `api.cmd.debug.makeSimPersonSetStateCmd(…)`): Sets the state of a simulated person.
    - param `entity`: the entity Id
    - param `simPersonState`: the new state
    - returns: a newly created SimPersonSetState command
    - used 2x in the base game, e.g. `base/content/gui/gui/debug_panel/make_entity_debug_panel.tl:676`: `api.cmd.sendCommand(api.cmd.debug.makeSimPersonSetStateCmd(simEntity, index))`
- `makeSimPersonSetHappiness : function(entity : Engine.Entity, happiness : number) : Command`  (call as `api.cmd.debug.makeSimPersonSetHappiness(…)`): Sets the happiness value of a simulated person.
    - param `entity`: the entity Id
    - param `happiness`: the new happiness value
    - returns: a newly created SimPersonSetHappiness command
- `makeComponentExchangeCmd : function(entity : Engine.Entity, component : Engine.Component) : Command<ComponentExchangeCommandDataDEBUG>`  (call as `api.cmd.debug.makeComponentExchangeCmd(…)`): Replaces a component on an entity for debugging purposes.
component is Engine.Component.EmissionEmitter|Engine.Component.ModelInstanceList
    - param `entity`: the entity Id
    - param `component`: the component (EmissionEmitter or ModelInstanceList)
    - returns: a newly created ComponentExchange command
- `makeStockSetCargoAmountCmd : function(entity : Engine.Entity, stockId : StockId, amount : integer, cargoType? : string) : Command<StockSetCargoAmountCommandDataDEBUG>`  (call as `api.cmd.debug.makeStockSetCargoAmountCmd(…)`): Sets the cargo amount for a stock.
    - param `entity`: stocklist entity
    - param `stockId`: the stock ID
    - param `amount`: the cargo amount
    - param `cargoType`: optional cargo type URI
    - returns: a newly created StockSetCargoAmount command
    - used 4x in the base game, e.g. `base/content/gui/gui/debug_panel/make_entity_debug_panel.tl:52`: `return api.cmd.debug.makeStockSetCargoAmountCmd(params.entity, params.stockId - 1, state.current)`
- `makeGamePerformSimulationStepsCmd : function(amount : integer) : Command<GamePerformSimulationStepsCommandDataDEBUG>`  (call as `api.cmd.debug.makeGamePerformSimulationStepsCmd(…)`): Advances the simulation by a given number of steps.
    - param `amount`: the number of steps
    - returns: a newly created GamePerformSimulationSteps command
- `makeClearLogbooksCmd : function(towns : boolean) : Command`  (call as `api.cmd.debug.makeClearLogbooksCmd(…)`): Clears either town or global logbooks.
    - param `towns`: whether to clear town logbooks
    - returns: a newly created ClearLogbooks command
