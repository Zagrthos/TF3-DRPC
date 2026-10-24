
# Scripting

TF3 scripts read the game through `api.engine` and `api.res`, change it by sending commands through `api.cmd`, and keep their own data in game scripts. This page goes through each part with code from the base game.

## The script states

The definitions describe the game as separate script states. `api.engine` is "available as read-only access to the entire engine state (in particular to the entire entity component system). It can be accessed from both the GUI State and the Engine State." (`api/tealdef/api/engine.d.tl`).

Writing works differently depending on the state. From the header of `api/tealdef/api/cmd.d.tl`:

> On the engine states, commands are executed immediately and the effects are immediately noticeable. Moreover, the callback function will be called immediately.
>
> On the gui and console states, commands are executed in the next simulation step, and the effects are noticeable only after a few frames. The callback function will be called the frame after the command has been executed, after which the changes are visible on the gui and engine state.

In practice:

| Code | Runs in |
|---|---|
| Game script `update`, `postUpdate`, `handleEvent` | engine state |
| Game script `guiUpdate`, `guiHandleEvent` | GUI state |
| GUI recipes (`base/content/gui/...`) | GUI state |
| Mod script `preRunFn`, `runFn`, `postRunFn` | load time, before any game script (see [Load-time code](#load-time-code)) |
| Construction `updateFn` and similar content scripts | not stated in the definitions or the wiki |

How often the two sides run is not in the definitions. The wiki's [Missions](https://wiki.transportfever3.com/doku.php?id=modding:scripting:missions) page says it for mission tasks, which are driven by the mission game script: `onUpdate` runs every simulation step and `guiUpdate` every frame. `base/content/mission/mission/mission_sim.script.tl` calls a task's `onUpdate` from its game script `postUpdate` and the task's `guiUpdate` from its game script `guiUpdate`, so the game script functions most likely run at the same rates.

The split of the game script functions follows from [`GameScriptWithGui`](../base/scripts/gamescript.md) (symbol: `GameScriptWithGui`, see [symbol index](../index.tsv)): `guiUpdate` and `guiHandleEvent` get the simulation state as a read-only `GameScriptStateReadOnly` and a separate writable GUI state. Some `api.gui` functions are marked "Only available in GUI thread", and [`api.util`](../api/api/util.md) is the one module whose functions "can be called from anywhere".

## Reading entities

The engine is an entity component system. An entity is a plain integer ([`Engine.Entity`](../api/api/engine.md) (symbol: `Engine.Entity`, see [symbol index](../index.tsv))); its data sits in components that you look up by type. Two entities are always there: the world and the player.

```teal
local gameTimeComponent : Engine.Component.GameTime
		= api.engine.getComponent(api.engine.util.getWorld(), api.type.ComponentType.GAME_TIME)
```

This line is from `base/content/game_mechanics/game_mechanics/game_time/game_time.script.tl`. [`getComponent`](../api/api/engine.md) (symbol: `Engine.getComponent`, see [symbol index](../index.tsv)) returns the component, or `nil` if the entity has none of that type. The component records are listed under [`Engine.Component`](../api/api/engine.md), for example [`Account`](../api/api/engine.md) (symbol: `Engine.Component.Account`, see [symbol index](../index.tsv)) with `balance` and `loan`. For the player's money the base game doesn't read `Account`: it calls [`api.engine.util.finance.getPlayersBalance(player)`](../api/api/engine/util.md) (symbol: `UtilFinance.getPlayersBalance`, see [symbol index](../index.tsv)), which returns `nil` when the player has infinite money (`tutorial_story.tl` and `game_bar.tl` handle that case). The component types are the constants in [`Engine.ComponentType`](../api/api/engine.md) (`GAME_TIME`, `WORLD`, `ACCOUNT`, `PLAYER_OWNED`, `CONSTRUCTION`, `VEHICLE_DEPOT`, `GAME_SCRIPT`, ...).

To find entities, filter by component. From `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl`:

```teal
local allPlayerDepotEntities : {Engine.Entity} = api.engine.getEntitiesWithComponent(api.type.ComponentType.VEHICLE_DEPOT, {
	requireOwnedByPlayer = api.engine.util.getPlayer(),
	filterFn = function(entity : Engine.Entity) : boolean
		local vehicleDepot : Engine.Component.VehicleDepot = api.engine.getComponent(entity, api.type.ComponentType.VEHICLE_DEPOT)
		return vehicleDepot and (#vehicleDepot.inNodes > 0 or #vehicleDepot.outNodes > 0)
	end,
})
```

The definition warns that `filterFn` is the "Most flexible - but very slow" filter. Other functions on `api.engine` are `forEachEntityWithComponent`, `forEachEntity`, `entityExists` and `getRevision`, which "Keeps track of modifications to entities".

Beyond plain components, `api.engine.system` has one object per engine system with query functions, for example `api.engine.system.transportVehicleSystem.getLineVehicles(line)` (used in mission 01) or `api.engine.system.gameScriptSystem.getEntityForGameScript(name)`. They are listed in [system](../api/api/engine/system.md). Further helpers are in `api.engine.util` ([util](../api/api/engine/util.md)), `api.engine.terrain` and `api.engine.config`.

## Resources

Static data loaded from content files (models, constructions, tracks, cargo types, generic resources) is in the repositories of [`api.res`](../api/api/res.md). The comment on [`ResTypeRep`](../api/api/res.md) (symbol: `ResTypeRep`, see [symbol index](../index.tsv)) says "Resources are generally static (initialized when the game starts and don't change)" and "Resource ids are only valid within the same repository".

```teal
local gameTimeConfig = api.res.genericRep.get(api.res.genericRep.find("::/game_mechanics/game_time/game_time_config.res")).data as GameTimeConfig
```

`find` turns a resource name into an id (`-1` if it does not exist), `get` returns the resource. The GUI has a separate repository for [GUI resources](gui.md#gui-resources) (`.gres.lua`), `api.gui.genericRep`.

Resources are static once a game runs, but a mod can change them at load time, in its `postRunFn` (see [Load-time code](#load-time-code)). The wiki's [Mod Parameters and Scripts](https://wiki.transportfever3.com/doku.php?id=modding:general:modscripts) page shows three ways, all on [`ResTypeRep`](../api/api/res.md) (symbol: `ResTypeRep`, see [symbol index](../index.tsv)):

| Function | Effect |
|---|---|
| [`setVisible(id, false)`](../api/api/res.md) (symbol: `ResTypeRep.setVisible`, see [symbol index](../index.tsv)) | hides a resource, e.g. a vehicle from the Vehicle Store |
| [`getAsTable(id)`](../api/api/res.md) (symbol: `ResTypeRep.getAsTable`, see [symbol index](../index.tsv)) and [`setAsTable(id, data)`](../api/api/res.md) (symbol: `ResTypeRep.setAsTable`, see [symbol index](../index.tsv)) | copies a resource into a Lua table and writes the edited table back |
| [`addAsTable(name, data)`](../api/api/res.md) (symbol: `ResTypeRep.addAsTable`, see [symbol index](../index.tsv)) | adds a new resource under a new name, for example a copy of a vehicle with other costs |

[Mod structure](mod-structure.md#mod-script-hooks) has a full `postRunFn` example with these functions. The wiki notes that scripts cannot add new 3D data; a new model has to reference existing mesh files.

### Repositories

`api.res` has 25 repositories ([`Res`](../api/api/res.md) (symbol: `Res`, see [symbol index](../index.tsv))). Each holds one kind of resource file. The `load*` modifier and the file filter category for the same files are the keys in `base/content/base/base/mod.lua`; their names match the repositories, and the pairing below follows those names and the comments in `api/tealdef/api/res.d.tl`.

| Repository | Holds | Modifier | File filter |
|---|---|---|---|
| [`modelRep`](../api/api/res.md) (symbol: `Res.modelRep`, see [symbol index](../index.tsv)) | models (`.mdl`) | `loadModel` | `model/vehicle`, `model/person`, `model/car`, `model/rock`, `model/tree`, `model/signal`, `model/animal`, `model/other` |
| [`multipleUnitRep`](../api/api/res.md) (symbol: `Res.multipleUnitRep`, see [symbol index](../index.tsv)) | multiple units (`.mu.lua`) | `loadMultipleUnit` | `multipleUnit` |
| [`constructionRep`](../api/api/res.md) (symbol: `Res.constructionRep`, see [symbol index](../index.tsv)) | constructions (`.con.lua`) | `loadConstruction` | `construction` |
| [`metaConstructionRep`](../api/api/res.md) (symbol: `Res.metaConstructionRep`, see [symbol index](../index.tsv)) | metaconstructions (`.metacon.tl`) | `loadMetaConstruction` | `metaConstruction` |
| [`moduleRep`](../api/api/res.md) (symbol: `Res.moduleRep`, see [symbol index](../index.tsv)) | modules (`.module.lua`) | `loadModule` | `module` |
| [`buildingTypeRep`](../api/api/res.md) (symbol: `Res.buildingTypeRep`, see [symbol index](../index.tsv)) | town building types; each entry names its `constructionFile`, a `.con.lua` under `buildings/` | none of its own (the constructions go through `loadConstruction`) | none of its own |
| [`streetTemplateRep`](../api/api/res.md) (symbol: `Res.streetTemplateRep`, see [symbol index](../index.tsv)) | track and street templates (`.street_template.lua`) | `loadStreetTemplate` | `streetTemplate` |
| [`streetStyleRep`](../api/api/res.md) (symbol: `Res.streetStyleRep`, see [symbol index](../index.tsv)) | track and street styles (`.street.lua`) | `loadStreet` | `street` |
| [`edgeDecorationRep`](../api/api/res.md) (symbol: `Res.edgeDecorationRep`, see [symbol index](../index.tsv)) | edge add-ons (`.edge.lua`) | `loadEdgeDecoration` | `edgeDecoration` |
| [`bridgeTypeRep`](../api/api/res.md) (symbol: `Res.bridgeTypeRep`, see [symbol index](../index.tsv)) | bridges (`.bridge.lua`) | `loadBridge` | `bridge` |
| [`tunnelTypeRep`](../api/api/res.md) (symbol: `Res.tunnelTypeRep`, see [symbol index](../index.tsv)) | tunnels (`.tunnel.lua`) | `loadTunnel` | `tunnel` |
| [`railroadCrossingTypeRep`](../api/api/res.md) (symbol: `Res.railroadCrossingTypeRep`, see [symbol index](../index.tsv)) | railroad crossings (`.rcr.lua`) | `loadRailroadCrossing` | `railroadCrossing` |
| [`trafficLightTypeRep`](../api/api/res.md) (symbol: `Res.trafficLightTypeRep`, see [symbol index](../index.tsv)) | traffic lights (`.trl.lua`) | `loadTrafficLight` | `trafficLight` |
| [`climateRep`](../api/api/res.md) (symbol: `Res.climateRep`, see [symbol index](../index.tsv)) | climates (`.clima.lua`) | `loadClimate` | `climate` |
| [`environmentRep`](../api/api/res.md) (symbol: `Res.environmentRep`, see [symbol index](../index.tsv)) | environments (`.env.lua`) | `loadEnvironment` | `environment` |
| [`terrainMaterialTypeRep`](../api/api/res.md) (symbol: `Res.terrainMaterialTypeRep`, see [symbol index](../index.tsv)) | terrain materials (`.tmat.lua`) | `loadTerrainMaterial` | `terrainMaterial` |
| [`groundTextureRep`](../api/api/res.md) (symbol: `Res.groundTextureRep`, see [symbol index](../index.tsv)) | ground textures (`.gtex.lua`), keyed by name | `loadGroundTex` | `groundTex` |
| [`autoGroundTexRep`](../api/api/res.md) (symbol: `Res.autoGroundTexRep`, see [symbol index](../index.tsv)) | auto ground textures (`.agt.lua`), keyed by name | `loadAutoGroundTex` | `autoGroundTex` |
| [`economyRep`](../api/api/res.md) (symbol: `Res.economyRep`, see [symbol index](../index.tsv)) | [economies](game-mechanics.md#economies) (`.eco.lua`) | `loadEconomy` | `economy` |
| [`cargoTypeRep`](../api/api/res.md) (symbol: `Res.cargoTypeRep`, see [symbol index](../index.tsv)) | cargo types (`.cargo.lua`) | `loadCargoType` | `cargoType` |
| [`cargoClassRep`](../api/api/res.md) (symbol: `Res.cargoClassRep`, see [symbol index](../index.tsv)) | cargo classes (`.cargoclass.lua`) | `loadCargoClass` | `cargoClass` |
| [`gameScriptRep`](../api/api/res.md) (symbol: `Res.gameScriptRep`, see [symbol index](../index.tsv)) | [game scripts](#game-scripts) (`.gs.lua`) | `loadGameScript` | `gameScript` |
| [`genericRep`](../api/api/res.md) (symbol: `Res.genericRep`, see [symbol index](../index.tsv)) | [generic resources](mod-structure.md#generic-resources) (`.res.lua`) | `loadGameRes` | `gameRes` |
| [`soundSetRep`](../api/api/res.md) (symbol: `Res.soundSetRep`, see [symbol index](../index.tsv)) | sound sets (`.snd.lua`) | `loadSoundSet` | `soundSet` |
| [`builderAudioSetRep`](../api/api/res.md) (symbol: `Res.builderAudioSetRep`, see [symbol index](../index.tsv)) | builder audio sets (`.builder_audio.lua`) | `loadBuilderAudioSet` | `builderAudioSet` |

Track styles are `.street.lua` files with `roadType = "TRACK"`. `mod.lua` also has a `loadTrack` modifier and a `track` filter, and the files don't show which resources go through them. The pairing of `genericRep` with `loadGameRes` is also only inferred from the names; `.gres.lua` files are `gui_res` resources in `api.gui.genericRep`, and which modifier they pass through is not shown either.

The other modifiers have no repository in `api.res`: `loadTransformator` (`.trf.lua`), `loadTerrainGenerator` (`.gen.lua`), `loadLayerGenerator`, `loadNodeTree` (`.node.lua`), `loadGrass` (`.grass.lua`), `loadCargoModelFormat` (`.cmf.lua`), `loadConstructionCategory`, `loadConstructionMenu`, `loadScript`, `loadCampaign`, `loadMission`, `loadNameList`, `loadLanguage`, `loadPlaylist`, `loadSoundEffect`, `loadStyleSheet` and the rendering keys.

Some repositories have extra functions. [`cargoTypeRep`](../api/api/res.md) (symbol: `CargoTypeRep`, see [symbol index](../index.tsv)) has `getPassengerCargoTypeId()`, `forEachCargoType(set, fn)` and `hasCargoType(set, id)`; [`cargoClassRep`](../api/api/res.md) (symbol: `CargoClassRep`, see [symbol index](../index.tsv)) has `getCargoClassId(name)`; [`modelRep`](../api/api/res.md) (symbol: `ModelRep`, see [symbol index](../index.tsv)) has `forEachModelWithMetadata`; [`genericRep`](../api/api/res.md) (symbol: `GenericRep.getAllOfType`, see [symbol index](../index.tsv)) has `getAllOfType(typeName)`, which the base game uses to collect every resource of a type, mod files included.

## Changing the game with commands

Every change to the engine state goes through a command object. `api.cmd` has a factory per command, `make<Name>Cmd`, and [`sendCommand`](../api/api/cmd.md) (symbol: `Cmd.sendCommand`, see [symbol index](../index.tsv)) to run it:

```teal
sendCommand : function<T is ICommandData>(cmd : Command<T>,
	callback? : function(data : T, success : boolean, resultEntities : {{Engine.Entity, Engine.Revision}}),
	progress? : CommandProgress)
```

The callback gets the executed command's data, which "depending on the context ... might contain additional information", a `success` flag and the list of result entities. Each command has a data record whose fields are described in [cmd](../api/api/cmd.md); fields marked "result only" are filled in after execution.

The base game's `custom_entity_util.spawn` chains two commands this way. From `base/content/game_mechanics/game_mechanics/fun_elements/custom_entity_util.tl`:

```teal
custom_entity_util.spawn = function(model : string, transf : Mat4f, callback:function(Engine.Entity))
	local modelId = api.res.modelRep.find(model) as integer

	if modelId < 0 then
		log.error("Model not found:", "model = " .. model)
		return
	end

	local cmd = api.cmd.makeCustomEntityCreateCmd(modelId)

	local onCommandCallback = function(res : CustomEntityCreateCommandData, success : boolean)
		if success == false then
			log.error("Model not spawned")
			return
		end
		local spawnedEntity =  res.resultEntity as Engine.Entity

		api.cmd.sendCommand(api.cmd.makeCustomEntityUpdateTransformationCmd(spawnedEntity, transf), function() 
			callback(spawnedEntity)
		end)
	end

	api.cmd.sendCommand(cmd, onCommandCallback as function(res : ICommandData, success : boolean))
end
```

`CustomEntityCreateCommandData.resultEntity` is documented as "Created entity id (result only)", so the new entity is only known inside the callback.

`api.cmd` has about sixty factories. Grouped by what they touch:

| Area | Examples |
|---|---|
| Game and time | `makeGameSetSpeedCmd`, `makeGameSetCalendarSpeedCmd`, `makeGameSetDateCmd`, `makeGameSetTimeOfDayCmd`, `makeGameSetCloudCoverageCmd` |
| Money | `makeJournalBookAssetCmd`, `makeJournalLogEntryCmd`, `makeJournalClearAllCmd` |
| Vehicles and lines | `makeVehicleBuyCmd`, `makeVehicleSellCmd`, `makeVehicleSetLineCmd`, `makeLineCreateCmd`, `makeLineUpdateCmd` |
| Towns and industries | `makeTownCreateCmd`, `makeTownUpdateSizeCmd`, `makeIndustrySetManualDevelopmentCmd`, `makeCreateIndustryExtendProposalCmd` |
| Building | `makeWorldBuildProposalCmd`, `makeWorldSetBulldozableCmd`, `makeWorldReplaceTerrainCmd` |
| Entities | `makeEntitySetNameCmd`, `makeEntitySetColorCmd`, `makeCustomEntityCreateCmd`, `makeAnimalSpawnAtCmd` |
| Scripts | `makeScriptingSendEventCmd` |

A nested `Debug` record holds a few more (`makeMaintenanceCostUpdateCmd`, `makeSimPersonSetStateCmd`, ...).

## Game scripts

A game script is a script with its own saved state that runs every simulation step and receives events. The base game has 28 of them and the deluxe DLC one more; [Game mechanics](game-mechanics.md#the-base-game-scripts) lists them. Each one consists of a `.gs.lua` file that names the functions, and a `.script.tl` (or `.script.lua`) that implements them. From `base/content/game_mechanics/game_mechanics/game_time/game_time.gs.lua`:

```lua
function data()
	return {
		updateScript = {
			fileName = "game_time.script@update",
		},
		handleEventScript = {
			fileName = "game_time.script@handleEvent",
		},
	}
end
```

The possible keys are those of [`GameScriptDesc`](../api/api/type.md) (symbol: `GameScriptDesc`, see [symbol index](../index.tsv)): `updateScript`, `postUpdateScript`, `handleEventScript`, `guiUpdateScript` and `guiHandleEventScript`. Each entry can carry `params`, which the base game's [mission script](missions.md#how-a-mission-starts) uses (`params = { gameScriptMode = "Mission" }` in `base/content/mission/mission/mission.gs.lua`) and which arrive as the `userParams` argument.

The script file returns a table that matches [`GameScript<T>`](../base/scripts/gamescript.md) (symbol: `GameScript`, see [symbol index](../index.tsv)) or [`GameScriptWithGui<StateT, GuiStateT>`](../base/scripts/gamescript.md) (symbol: `GameScriptWithGui`, see [symbol index](../index.tsv)) from `base/tealdef/scripts/gamescript.d.tl`:

```teal
global record GameScriptWithGui<StateT, GuiStateT>
	update : function(userParams : table, state : GameScriptState<StateT>, dt : number) : any
	postUpdate : function(userParams : table, state : GameScriptState<StateT>, dt : number, updateResult : any)
	handleEvent : function(userParams : table, state : GameScriptState<StateT>, src : string, id : string, name : string, param : any) : any

	guiUpdate : function(userParams : table, state : GameScriptStateReadOnly<StateT>, guiState : GameScriptState<GuiStateT>)
	guiHandleEvent : function(userParams : table, state : GameScriptStateReadOnly<StateT>, guiState : GameScriptState<GuiStateT>,
			src : string, id : string, name : string, param : any) : any
end
```

Nothing registers a `.gs` file by hand. The wiki's [resource types](https://wiki.transportfever3.com/doku.php?id=modding:general:resourcetypes) table lists `.gs.lua` as the file type for game scripts, so the game loads them like any other resource, and the base loader in `base/content/base/base/mod.lua` has a `gameScript` file filter category and a `loadGameScript` modifier key for them. The folder does not matter: the campaign mods keep theirs at the game path `/init/init.gs.lua`, mission 08 has `/mission/rocket/rocket.gs.lua`, and the deluxe DLC has `/fun_elements/balloon.gs.lua`. In the unpacked game files these sit one folder deeper (`content/init/init/init.gs.lua`), because each zip became a folder ([How the zips map to folders](mod-structure.md#how-the-zips-map-to-folders)); a mod with loose files writes `content/mission/rocket/rocket.gs.lua`. Other code finds a game script by its resource name without the `.lua`, for example `getEntityForGameScript("urbangames_campaign_mission_08::/mission/rocket/rocket.gs")`.

The `params` table of a script reference is passed as the first argument. The wiki states this for all script references ("commonly called `captureParams`"); game scripts receive it as `userParams`. The wiki's own game script page is still marked "Coming Soon", so the order in which several game scripts run, and when a mod's game script first starts in an existing savegame, are not documented.

### State

The state object (`GameScriptState<T>`) has `get`, `set` and the event subscription functions. The weather script reads its state, fills it on first use and writes it back at the end of `update`. Shortened from `base/content/game_mechanics/game_mechanics/game_time/game_time.script.tl`:

```teal
local ret : GameScriptWithGui<GameTimeState, GameTimeGUIState> = {
	update = function(_userParams : table, state : GameScriptState<GameTimeState>, dt : number) : nil
		if not state:hasEventSubscriptions() then
			state:subscribeToEvent("SetMode")
		end
		state:subscribeToEvent("SkipPhase")

		local gameTimeState : GameTimeState = state:get()
		gameTimeState = initStateIfEmpty(gameTimeState)

		local gameTimeComponent : Engine.Component.GameTime
				= api.engine.getComponent(api.engine.util.getWorld(), api.type.ComponentType.GAME_TIME)
		-- ... compute the next time of day ...
		api.cmd.sendCommand(api.cmd.makeGameSetTimeOfDayCmd(newTimeOfDaySec))

		state:set(gameTimeState)
	end,
	-- handleEvent = ...
}

return ret
```

`initStateIfEmpty` also stores a `version` number in the state and upgrades older layouts, which is how the base game keeps savegames compatible.

The state is stored in the engine on an entity with a `GAME_SCRIPT` component ([`Engine.Component.GameScript`](../api/api/engine.md) (symbol: `Engine.Component.GameScript`, see [symbol index](../index.tsv)), field `state`). Other scripts read it from there. From `base/content/game_mechanics/game_mechanics/company/company_util.tl`:

```teal
function company_util.externalGetCompaniesState() : CompaniesState
	local entity = api.engine.system.gameScriptSystem.getEntityForGameScript("::/game_mechanics/company/company.gs")
	local gameScript = api.engine.getComponent(entity, api.type.ComponentType.GAME_SCRIPT) as Engine.Component.GameScript<CompaniesState>
	return gameScript.state
end
```

### Events

`handleEvent` receives `src`, `id`, `name` and `param`. The base scripts distinguish three kinds of sender.

Events from the game itself have empty `src` and `id`. The names handled in the base scripts are `initNewGame`, `initNewGameFromMap`, `initMission`, `initTutorial`, `handleLegacy` and `storeSavegameMetadata`. From `base/content/game_mechanics/game_mechanics/company/company_growth.script.tl`:

```teal
handleEvent = function(_userParams : table, state : GameScriptState<CompaniesGrowthState>, src : string, id : string, name : string, param : any) : any
	if src == "" and id == "" then
		if name == "initNewGame" or name == "handleLegacy" then
			state:subscribeToEvent("_debugAddExperience")
			state:subscribeToEvent("applyLevel")
			state:subscribeToEvent("storeSavegameMetadata")
			state:subscribeToEvent("OnCalcTicketPrice")
		end
		if name == "initNewGameFromMap" or name == "initMission" then
			state:set({ companyState = {} } )
		end
		-- ...
```

Events from engine systems carry the system's name as `id`, for example `"TransportVehicleSystem"`, `"SimPersonSystem"` or `"SimCargoSystem"`. Some of them use the return value. The same handler answers the ticket price event with a multiplier table:

```teal
	elseif (id == "TransportVehicleSystem" or id == "SimEntityAtVehicleSystem") and name == "OnCalcTicketPrice" then
		local scriptState : CompaniesGrowthState = state:get()
		local companyData = scriptState.companyState[api.engine.util.getPlayer()]
		if companyData then
			local multiplier = companyData.ticketPriceMultiplier
			if not multiplier or multiplier < 0 then
				return
			end
			local multipliers : {integer : number} = {}
			multipliers[0] = multiplier -- multipliers[0] is used for all
			return multipliers
		end
	end
```

The event payload types that are defined, such as `CalcTicketPriceEvent`, `StartedLineUsage` and `ArriveAtStop`, are in [gamescript](../base/scripts/gamescript.md). Other event names seen in the base scripts are `OnStartedLineUsage`, `OnCompletedLineUsage` and `OnToArriveAtDestination`. The files contain no complete list of engine events.

Events from other scripts and from the GUI are sent with [`api.cmd.makeScriptingSendEventCmd(src, id, name, param)`](../api/api/cmd.md) (symbol: `Cmd.makeScriptingSendEventCmd`, see [symbol index](../index.tsv)): "When executed, a script event will be broadcasted to all scripts." The "Skip to Next Time of Day" button in `base/content/gui/gui/game_bar/game_bar_widgets.tl` does this:

```teal
onClick = function()
	api.cmd.sendCommand(api.cmd.makeScriptingSendEventCmd("", "GameTime", "SkipPhase", { }))
end,
```

and `game_time.script.tl` reacts with `if id == "GameTime" and name == "SkipPhase" then ...`. The scripts subscribe with `state:subscribeToEvent(name)` (or `subscribeToAllEvents`) before they get such events; the base scripts do this in `update` or when `initNewGame`/`handleLegacy` arrives.

For events that only the GUI side should handle, [`api.gui.fireGuiScriptEvent(id, name, param)`](../api/api/gui.md) (symbol: `Gui.fireGuiScriptEvent`, see [symbol index](../index.tsv)) "is meant to be handled by game scripts (guiHandleEvent), not by react.onEvent()". It differs from `react.fireEvent` in that it allows to return a value "like an error string to prevent the operation".

## Load-time code

Code that runs while the game loads, before any game script, is set up in `mod.json` (`preRunScript`, `runScript`, `postRunScript`). Each of the three functions runs for the base game first and then for each mod in activation order. [Mod structure](mod-structure.md#mod-script-hooks) describes the hooks and their arguments, including mod parameters. The scripting side of each phase:

| Phase | Typical job | Tools |
|---|---|---|
| `preRunFn` | change the base config | the `baseConfig` argument |
| `runFn` | change resource files while they load, or hide them | `addModifier`, `addFileFilter` |
| `postRunFn` | change, add or hide loaded resources | `api.res` repositories, `api.res.getBaseConfig()` |

The wiki recommends `postRunFn` with `api.res` over `runFn` modifiers and filters whenever the job can be done there ([Mod Parameters and Scripts](https://wiki.transportfever3.com/doku.php?id=modding:general:modscripts)).

### Base config

[`BaseConfig`](../api/api/type.md) (symbol: `BaseConfig`, see [symbol index](../index.tsv)) holds the global simulation and gameplay settings: costs, town and industry placement, economy, animals, terrain tool limits, the construction script and more. The shipped game sets and reads it in three places:

- `preRunFn` receives it as the fourth argument and writes to it. The base game fills almost all of it there (`base/content/base/base/mod.script.tl`); `urbangames_no_costs` only sets `baseConfig.noCosts = true`.
- `postRunFn` gets it from [`api.res.getBaseConfig()`](../api/api/res.md) (symbol: `Res.getBaseConfig`, see [symbol index](../index.tsv)) and can still change it. Mission 01 sets `advancedOptions.subventionMode = 0` this way.
- Game scripts read it with `api.res.getBaseConfig()`, for example `api.res.getBaseConfig().advancedOptions.subventionRisk` in the [subsidy](game-mechanics.md#subsidies) scripts under `base/content/game_mechanics/game_mechanics/subventions/`.

The wiki's [Base Config](https://wiki.transportfever3.com/doku.php?id=modding:scripting:baseconfig) page is marked as not yet adapted for TF3, and several of its statements don't match the TF3 files:

| The wiki says | The TF3 files do |
|---|---|
| initial values come from `res/config/base_config.lua` | `base/content/base/base/base_config.lua` only creates an empty `game.config`; the values are set in the base game's `preRunFn` |
| write `game.config.<property>` in `runFn`, `api.res.getBaseConfig()` in `postRunFn` | no shipped script uses `game.config`; they write to the `baseConfig` argument of `preRunFn` |
| the `cost.*` values and `ConstructWithModules` are not available through `getBaseConfig()` | the record has `costs` (a map such as `costs.terrainRaise`) and `constructionScript`, which the base game sets to `::scripts/construction/construction.script@constructWithModules` |
| `gui.*`, `audio.*`, `difficulty`, `earnAchievementsWithMods`, `advancedOptions.maximumLoanScale` and others | these fields are not in the TF3 `BaseConfig` record; its `advancedOptions` has subsidy, maintenance, landmark and reforestation settings instead |

The wiki table still helps with the fields that exist in both, such as `locations`, `animal`, `economy.industryDevelopment` and the terrain tool limits. For the TF3 field list use the [`BaseConfig`](../api/api/type.md) (symbol: `BaseConfig`, see [symbol index](../index.tsv)) reference.

### Resource modifiers

A modifier is a function that gets each loaded file of one kind and returns its data, changed or not. Register it in `runFn` with `addModifier(category, fn)`; `fn` receives `fileName` and `data` (the result of the file's `data()` function). From `mods/release/urbangames_vehicles_no_end_year/content/mod.script.lua`:

```lua
function data()
return {
	runFn = function (captureParams, settings)
		addModifier("loadModel", function (fileName, data)
				if data.metadata.transportVehicle and data.metadata.availability then
					data.metadata.availability.yearTo = 0
				end

				return data
			end)
	end
}
end
```

The parameter names in the definition differ from this use. `api/tealdef/main.d.tl` declares `addModifier : function<T>(fileName : string, data : T) : T`, which is the shape of the modifier function, not of `addModifier` itself. The wiki ([Modifiers and Filters](https://wiki.transportfever3.com/doku.php?id=modding:scripting:modifiersfilters)) and the base implementation in `base/content/base/base/mod.lua` (`function addModifier(key, fun)`) both take a category and a function. In Teal you may need a cast to get past the type check.

Modifiers for the same category run one after another in registration order, each on the result of the previous one (`applyModifiers` in `mod.lua`). Several mods can therefore change the same files without overwriting each other's copies.

The categories are the keys of the `modifiers` table in `mod.lua`. An unknown key fails, because `addModifier` inserts into a list that doesn't exist.

| Group | Categories |
|---|---|
| Models and vehicles | `loadModel`, `loadMultipleUnit`, `loadTransformator` |
| Constructions | `loadConstruction`, `loadMetaConstruction`, `loadModule`, `loadConstructionCategory`, `loadConstructionMenu` |
| Infrastructure | `loadStreet`, `loadStreetTemplate`, `loadTrack`, `loadEdgeDecoration`, `loadBridge`, `loadTunnel`, `loadRailroadCrossing`, `loadTrafficLight` |
| Terrain and environment | `loadEnvironment`, `loadClimate`, `loadTerrainMaterial`, `loadTerrainGenerator`, `loadLayerGenerator`, `loadGrass`, `loadGroundTex`, `loadAutoGroundTex`, `loadNodeTree` |
| Economy and cargo | `loadEconomy`, `loadCargoClass`, `loadCargoType`, `loadCargoModelFormat` |
| Scripts and data | `loadScript`, `loadGameScript`, `loadGameRes`, `loadCampaign`, `loadMission`, `loadNameList`, `loadLanguage` |
| Sound and style | `loadSoundSet`, `loadSoundEffect`, `loadBuilderAudioSet`, `loadPlaylist`, `loadStyleSheet` |
| Rendering | `loadMaterialType`, `loadDescriptorSet`, `loadProgram`, `loadTechnique`, `loadProperty` |

The wiki lists 22 of these, all of which exist. `base/content/mission/mission/modifier.lua` has helpers built on top (`modifier.util.disable`, `modifier.util.availability(from, to)`, `modifier.treevisitor(tree)`), which register one modifier per category and apply functions from a tree keyed by path.

### File filters

A file filter decides whether a file is loaded at all. `addFileFilter(category, fn)` is defined in `base/content/base/base/mod.lua`; `fn(fileName, data)` returns `false` to drop the file and `true` to keep it. Several filters can be registered for one category and all of them must return `true`. The wiki's example drops every single-engine rail vehicle whose engine is not steam:

```lua
addFileFilter("model/vehicle", function(fileName, data)
	local rv = data.metadata.railVehicle
	if rv and rv.engines and #rv.engines == 1 and rv.engines[1].type ~= "STEAM" then
		return false
	end
	return true
end)
```

The categories in `mod.lua` are `model/vehicle`, `model/person`, `model/car`, `model/rock`, `model/tree`, `model/signal`, `model/animal`, `model/other`, and one per resource kind with the same names as the modifiers without `load` (`construction`, `module`, `street`, `track`, `bridge`, `cargoType`, `gameScript`, `climate`, ...). As with modifiers, an unknown category fails.

`addFileFilter` is not declared in `api/tealdef/main.d.tl`, so it has no entry in the reference and a Teal mod has to declare it itself. No shipped TF3 mod uses it; the official mods hide resources with `setVisible` in `postRunFn` instead.

The wiki also describes `filefilterutil.lua` with ready-made filters. In TF3 it is `base/content/base/base/filefilterutil.lua`, not `res/scripts/`. It has `util.always`, `util.combineOr`, `util.combineAnd`, `package.base`, `package.mod`, `package.reallyMod` and `model.vehicle`, `model.person`, `model.car`, `model.rock`, `model.tree`, `model.signal`, `model.animal`, `model.other`. `package.base` and `package.mod` still test for the Transport Fever 2 prefix `::/res/`, which TF3 resource names don't have; `package.reallyMod` tests whether the name starts with `::`.

## Useful globals

From `api/tealdef/main.d.tl` ([main](../api/main.md)):

| Global | Use |
|---|---|
| `ug_require(path)` | load a game script module |
| `_(id)`, `pGetText`, `nGetText`, `npGetText` | translated strings |
| `debugPrint(...)` | "Prints text to the console/log file" |
| `resolve(relativePath)` | path relative to the current file |
| `getCurrentModId()` | id of "the outermost mod in the require hierarchy" |
| `getOwningModId()` | id of "the mod owning the current file"; "you can not delegate calls to this function into helper files!" |
| `addModifier(category, fn)` | register a load-time modifier (see [Resource modifiers](#resource-modifiers)) |
| `addFileFilter(category, fn)` | register a load-time file filter; defined in `base/content/base/base/mod.lua`, not in `main.d.tl` (see [File filters](#file-filters)) |

The base scripts log with the global `log` table (`log.verbose`, `log.message`, `log.warning`, `log.error`).

The wiki's [API Reference](https://wiki.transportfever3.com/doku.php?id=modding:scripting:api) page suggests trying commands in the in-game console first and printing tables with `debugPrint(<someTable>)`.

Official wiki: [Mod Parameters and Scripts](https://wiki.transportfever3.com/doku.php?id=modding:general:modscripts), [Modifiers and Filters](https://wiki.transportfever3.com/doku.php?id=modding:scripting:modifiersfilters), [Base Config](https://wiki.transportfever3.com/doku.php?id=modding:scripting:baseconfig), [API Reference](https://wiki.transportfever3.com/doku.php?id=modding:scripting:api), [Resource types](https://wiki.transportfever3.com/doku.php?id=modding:general:resourcetypes), [Missions](https://wiki.transportfever3.com/doku.php?id=modding:scripting:missions).
