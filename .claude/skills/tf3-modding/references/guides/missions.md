
# Missions

The campaign is built from mods: `urbangames_campaign` defines the campaign, and each of `urbangames_campaign_mission_01` to `_08` is one mission. A mission is a savegame plus a set of tasks. The tasks run inside the base game's mission framework (`base/content/mission/`), which is a game script. This page walks through mission 01: its description file, how it boots, how tasks are defined, and one task script.

A mod can add a whole campaign or single missions to an existing one; both show up under Campaign in the main menu ([wiki: Missions](https://wiki.transportfever3.com/doku.php?id=modding:scripting:missions)). The wiki sums a mission up as an orchestrating game script plus the savegame it plays on.

## The pieces of a mission mod

| File in `mods/release/urbangames_campaign_mission_01/` | Role |
|---|---|
| `mod.json` | `"visible": false`, `"cosmetic": true`, pre/run/post scripts in `mod.script` |
| `content/savegames/savegame.sav` | the savegame the mission plays on (`savegame = "/savegames/savegame.sav"` in `info.mission.lua`; see [The savegame](#the-savegame)) |
| `content/info.mission.lua` | the mission description: name, images, stars, medals, characters, music, campaign |
| `content/mod.script.tl` | `preRunFn` and `postRunFn` adjust `BaseConfig` and resources for this mission |
| `content/mission/mission/mission.res.lua` | registers the mission's boot function (`missiontask_boot`) |
| `content/mission/mission/mission.script.lua` | exposes the mission module to script references |
| `content/mission/mission/mission.tl` | builds the module with `mission_boot_util.makeMission` |
| `content/mission/mission/mission_story.tl` | defines every task by key (3,019 lines) |
| `content/mission/mission/mission_params_01.tl` | entity ids, zones and other constants of the savegame |
| `content/mission/mission/tasks/everything/everything.tl` | a task script written for this mission |
| `content/mission/mission/keyframes/languages.script.tl` | the `languageScript`: per language, which keyframe file each dialogue and cutscene uses |
| `content/mission/mission/keyframes/en/*.lua`, `de/*.lua` | dialogue and cutscene keyframes (portraits, subtitles, voice-overs, camera) per language |
| `content/mission/mission/hud_replacement.*` | a GUI recipe replacement (see [GUI](gui.md#replacing-a-base-recipe)) |
| `content/init/init/init.gs.lua` | a game script that fixes up the savegame on `initMission` |
| `content/guide_system/guide_system/guide_system.res.lua` | registers the mission's guide system (`guide_system_boot`) |
| `tealdef/mission/mission_params_01.d.tl` | the Teal type of the parameter table |

The wiki names the minimum set for a mission folder: `info.mission.lua`, `savegame.sav`, `mission.res.lua`, `mission.script.lua`, `mission.tl` and `mission_story.tl`. Everything else in the table is specific to the official missions.

The official mission mods set `"visible": false` in `mod.json`. The wiki's [mod definition page](https://wiki.transportfever3.com/doku.php?id=modding:general:moddefinition) explains why: a hidden mod does not appear in the mod list, which suits mods that only provide content for a campaign savegame and should not be used in free games. `"cosmetic": true` keeps achievements available; the wiki leaves it to the modder to judge whether a mod affects the simulation. See [Mod structure](mod-structure.md) for the other `mod.json` fields.

## The description files

The campaign itself is one file, `mods/release/urbangames_campaign/content/info.campaign.lua`:

```lua
function data()
return {
	name = _("CAMPAIGN_NAME"),
	icon = "campaign.tga",
	description = _("CAMPAIGN_DESC"),
	order = 1,
	unlockingProgression = true,
}
end
```

The wiki says the campaign file needs the suffix `.campaign.lua` and can sit in one of the mission mods or in a mod of its own; the official campaign uses a separate mod, `urbangames_campaign`. Its fields, as the [wiki](https://wiki.transportfever3.com/doku.php?id=modding:scripting:missions) describes them:

| Field | Meaning |
|---|---|
| `name`, `description` | title and text in the campaign selection menu |
| `icon` | image button for selecting the campaign; the double-size `<name>@2x.tga` should be 675 × 1080 pixels |
| `order` | sort position in the campaign menu; the official campaign has 1 |
| `unlockingProgression` | `false` unlocks all missions; `true` unlocks them one at a time |

For `unlockingProgression = true` the wiki says the next mission unlocks once the player earns at least one star. The base game code checks completion instead: `base/content/gui/gui/menu/savegame_react_util.tl` enables a mission when `index <= highestUnlocked + 1`, where `highestUnlocked` is the last mission for which `app.getUserProfile():isMissionCompleted(campaign, missionName)` is true. The files don't show whether "completed" means "at least one star".

Each mission links to the campaign from its `info.mission.lua` with `campaign = "urbangames_campaign::/info.campaign"`. From mission 01 (shortened):

```lua
function data()
	return {
		name = _("MISSION_01_NAME"),
		description = _("MISSION_01_DESC"),
		image = "/gui/mission/m01_preview.tga",
		languageScript = "urbangames_campaign_mission_01::/mission/keyframes/languages.script@get",
		loadscreen = "/gui/mission/m01_loadscreen.tga",
		savegame = "/savegames/savegame.sav",
		stars = {
			{
				id = "MISSION_01_STAR_1",
				name = _("MISSION_01_STAR_1"),
				iconLocked = "::/gui/menu/icons/star_1.tga",
				iconCompleted = "::/gui/menu/icons/star_1.tga",
			},
			-- STAR_2, STAR_3
		},
		medals = {
			{ id = "MISSION_01_MEDAL_1", name = _("MISSION_01_MEDAL"), nameLocked = _("MISSION_01_MEDAL_LOCKED"), ... },
		},
		characters = {
			{ name = _("MISSION_01_CHARACTER_1_NAME"), info = _("MISSION_01_CHARACTER_1_INFO"), portrait = "urbangames_campaign_mission_01::/mission/dialogue/major_neutral.tga", },
			-- ...
		},
		location = _("MISSION_01_LOCATION"),
		year = 1906,
		campaign = "urbangames_campaign::/info.campaign",
		order = 1,
		musicTracks = {
			{ "urbangames_campaign_mission_01::/audio/music/m1_intro.ogg", false },
			{ "urbangames_campaign_mission_01::/audio/music/m1_theme_chapter_1.ogg", true },
		},
	}
end
```

Mission 01 also sets `imagePreview`, `imageSuccess` and `imageFailure` (end-window images), `loadscreenDescription` and `loadscreenVoiceOver` (text and voice-over on the loading screen) and `starsDescription`. The wiki points out three fields in particular: every mission must name its `campaign`; `stars` are needed if the campaign should track progress; and `languageScript` is needed for dialogue tasks, with at least one language (see [Translations](#translations)).

The fields match [`MissionDesc`](../api/api/type.md) (symbol: `MissionDesc`, see [symbol index](../index.tsv)) and [`CampaignDesc`](../api/api/type.md) (symbol: `CampaignDesc`, see [symbol index](../index.tsv)) in `api/tealdef/api/type.d.tl`. The application keeps them in `app.res.campaignRep` and `app.res.missionRep`, and `app.loadMission(saveId, campaign, mission)` starts one ([App](../api/app.md)).

## The savegame

The wiki's recipe: start a new game or load an existing one with the mission mod active, so that the mission's game scripts are loaded, then save and copy the savegame into the mission mod. All eight official missions reference `savegame = "/savegames/savegame.sav"`. A path starting with `/` points into the mod's `content` folder (mission 01's `image = "/gui/mission/m01_preview.tga"` is `content/gui/mission/m01_preview.tga`), so the file belongs at `content/savegames/savegame.sav`. The unpacked official mods don't contain it, and their `_content.json` doesn't list it. The tasks refer to entities of this savegame by id (see [Mission parameters](#mission-parameters)).

## Translations

Mission texts come in two kinds.

Strings wrapped in `_()`, such as `_("MISSION_01_NAME")` or the task names in `mission_story.tl`, are ordinary translation keys. How a mod translates them is in [Translations in a mod](other-content.md#translations-in-a-mod-stringsjson).

Dialogues and cutscenes are keyframe files, and those exist once per language because subtitle timing follows the voice-over. The `languageScript` in `info.mission.lua` returns a table from language code to a map of script name to keyframe file. From `content/mission/mission/keyframes/languages.script.tl` (one entry each):

```teal
local function data() : any
return {
	["en"] = {
		["urbangames_campaign_mission_01::/mission/keyframes/MISSION_01_CUTSCENE_INTRO.script@get"] = "urbangames_campaign_mission_01::/mission/keyframes/en/MISSION_01_CUTSCENE_INTRO.lua",
		-- ...
	},
	["de"] = {
		["urbangames_campaign_mission_01::/mission/keyframes/MISSION_01_CUTSCENE_INTRO.script@get"] = "urbangames_campaign_mission_01::/mission/keyframes/de/MISSION_01_CUTSCENE_INTRO.lua",
		-- ...
	},
}
end
return { get = data() }
```

A keyframe file returns `camera`, `environmentParams`, `fades`, `music`, `portraits`, `subtitles` and `voiceOvers`; the subtitles are `_()` keys again. The English and German copies of `MISSION_01_CUTSCENE_BRIDGE_COMPLETE.lua` differ only in their durations. `base/content/mission/mission/mission_react_util.tl` picks the table for `app.getUserProfile():getLanguage().code`, falls back to `"en"`, and logs an error and uses legacy dialogue handling when `languageScript` is missing.

## How a mission starts

The base game's mission script is a game script, `base/content/mission/mission/mission.gs.lua`, whose functions all live in `mission_sim.script` and run with `params = { gameScriptMode = "Mission" }`. On start it looks for boot resources. From `base/content/mission/mission/mission_sim.script.tl`:

```teal
if gameScriptMode == "Mission" then
	local missionTaskBoot = api.res.genericRep.getAllOfType("missiontask_boot")
	if #missionTaskBoot > 0 then
		for __, missionTaskId in ipairs(missionTaskBoot) do
			subscribeIfNeeded()
			registerTask(missionTaskId)
		end
	else
		for __, missionTaskId in ipairs(api.res.genericRep.getAllOfType("tutorial_boot")) do
			subscribeIfNeeded()
			registerTask(missionTaskId)
		end
	end
elseif gameScriptMode == "GuideSystem" then
	for __, guideId in ipairs(api.res.genericRep.getAllOfType("guide_system_boot")) do
		subscribeIfNeeded()
		registerTask(guideId)
	end
end
```

`registerTask` reads the resource's `bootstrapScriptFn` ([`MissionInterface.BootstrapDesc`](../base/mission/mission_framework/mission_interface.md) (symbol: `MissionInterface.BootstrapDesc`, see [symbol index](../index.tsv))), calls it once and remembers it in `state.bootTypes`. Mission 01 registers its boot function in `content/mission/mission/mission.res.lua`:

```lua
function data()
return {
	type = "missiontask_boot",
	data = {
		bootstrapScriptFn = "urbangames_campaign_mission_01::/mission/mission.script@mission.spawnBootTasks",
		bootstrapScriptFnParams = nil,
	}

}
end
```

`mission.script.lua` exposes the module under the name `mission`, which is why the reference ends in `@mission.spawnBootTasks`:

```lua
local mission = require "mission.tl"

function data()
return {
	mission = mission
}
end
```

and `mission.tl` builds that module:

```teal
local mission_boot_util = ug_require "::/mission/mission_boot_util.tl" as MissionBootUtil
local taskDataFactory = ug_require "mission_story.tl" as function(taskKey : string) : MissionTaskUtil.StaticTaskInfo

return mission_boot_util.makeMission({
	scriptKey = "urbangames_campaign_mission_01::/mission/mission.script",
	taskDataFactory = taskDataFactory,
	bootTasks = {
		{ taskKey = "check_entities" },
		{ taskKey = "everything" },
		{ taskKey = "intro" },
		{ taskKey = "disableMostUiStuff" },
		{ taskKey = "intro_cutscene" },
		{ taskKey = "non_bulldozable" },
		{ taskKey = "transfer_ownership" },
		{ taskKey = "town_development" },
		{ taskKey = "task_1_spawn_alligators" },
		{ taskKey = "industry_activity_1" },
		{ taskKey = "protect_important_entities" },
	},
})
```

[`MissionBootUtil.makeMission`](../base/mission/mission_boot_util.md) (symbol: `MissionBootUtil.makeMission`, see [symbol index](../index.tsv)) returns a module with `spawnBootTasks` and `getVTable`. In `base/content/mission/mission/mission_boot_util.tl`, `spawnBootTasks` adds the `bootTasks` through `missionApi.addTask`, each with a task config whose `getVTableScriptFn` is `scriptKey .. "@mission.getVTable"` and whose `uniqueKey` comes from `mission_task_util.makeTaskKey(key, scriptKey)`. The framework later calls `getVTable(taskKey)` to get a task's script and parameters from `taskDataFactory`.

The guide system works the same way with `makeGuide`, the resource type `guide_system_boot` and the `GuideSystem` mode.

## Defining tasks

`mission_story.tl` returns one function that maps a task key to a [`MissionTaskUtil.StaticTaskInfo`](../base/mission/mission_task_util.md) (symbol: `MissionTaskUtil.StaticTaskInfo`, see [symbol index](../index.tsv)). Each branch picks a task script from the base library and configures it. A typical one, the task that asks the player to build a road depot (shortened):

```teal
if taskKey == "buildStreetDepot" then
	return {
		taskScript = build_construction as MissionInterface.TaskScript<any, any>,
		taskParams : MissionTaskBuildConstruction.Params = {
			zones = {{
				key = "buildStreetDepot",
				polygon = polygon_util.transform(api.type.Mat4f.rotZTransl(params.streetDepotAngle, api.type.Vec3f.new(params.streetDepotZone.pos[1], params.streetDepotZone.pos[2], 0)), mission_outline.streetDepotOutline()),
				draw = true,
				playAnimation = true;
			}},
			initialAngle = params.streetDepotAngle,
			proposalHandler = build_construction_util.makeProposalHandlerFilterApply({
					checkConstruction = build_construction_util.makeCheckConstruction("::/depots/road/road_depot/road_depot.con", nil, { error = _("MISSION_PROPOSAL_FEEDBACK_NOT_A_ROAD_DEPOT") }),
					checkLocation = build_construction_util.makeCheckLocation(params.streetDepotZone.pos[1], params.streetDepotZone.pos[2], 20, { error = _("MISSION_PROPOSAL_FEEDBACK_NOT_IN_OUTLINED_AREA") }),
					checkSnapping = build_construction_util.makeCheckSnapping({}, { error = _("MISSION_PROPOSAL_FEEDBACK_NOT_CONNECTED_TO_STREET") }),
				}, true),
			},
		showInHistory = true,
		taskInfo = {
			name = _("MISSION_01_TASK_BUILD_STREET_FIRST_DEPOT_HOTEL_NAME"),
			paragraphs = {
				{ text = _("MISSION_01_TASK_BUILD_STREET_FIRST_DEPOT_HOTEL_PARAGRAPH") },
			},
			instructionData = {
				subtitle = _("MISSION_01_TASK_BUILD_STREET_FIRST_DEPOT_HOTEL_INSTRUCTION"),
				portrait = "urbangames_campaign_mission_01::/mission/dialogue/andrew_neutral.tga",
				voiceOver = "urbangames_campaign_mission_01::/audio/voice_over/MISSION_01_TASK_BUILD_STREET_FIRST_DEPOT_HOTEL_INSTRUCTION.wav",
				duration = 7,
			},
		},
		enabled = { "menu.construction.road", },
		highlightRules = { {"menu.construction.road", "Circle"}, },
		followup = {
			{ taskKey = "buyTruck" },
			{ taskKey = "highlightDepotBuy" },
			{ taskKey = "buy_vehicle_guide" },
		},
		menuFilter = {
			enabledItems = {
				"::/depots/road/road_depot/road_depot.con@0",
				"::/infrastructure/street/country/country_old_small.street_template"
			},
			disabledItems = {},
		},
	}
end
```

The fields fall into four groups:

| Group | Fields in this task | Meaning |
|---|---|---|
| The task | `taskScript`, `taskParams` | which task script runs and its specific parameters (here [`MissionTaskBuildConstruction.Params`](../base/mission/tasks/build_construction/mission_task_build_construction.md)) |
| Presentation | `taskInfo` (`name`, `paragraphs`, `instructionData`), `showInHistory` | text in the task window, voice-over and portrait; [`MissionInterface.TaskInfo`](../base/mission/mission_framework/mission_interface.md) (symbol: `MissionInterface.TaskInfo`, see [symbol index](../index.tsv)) |
| Restrictions while active | `enabled`, `highlightRules`, `menuFilter`, `vehicleFilter`, `notificationFilter`, `disableFeatures` | which UI elements and build menu items are available or highlighted; `notificationFilter` names [notification](game-mechanics.md#notifications) types (by script path, such as `"::/landmarks/landmarks_notification.script"` in mission 05) that the mission script sends to the notifications script as ignored types |
| Flow | `followup` | tasks added when this one completes; each entry can have a `condition` |

When a task completes, the framework calls the vtable's `onComplete`, which in `mission_boot_util.tl` spawns the `followup` tasks. The mission is a graph of task keys: `intro` leads to `findAlligators`, `buildStreetDepot` to `buyTruck`, and so on, until `missionEnd` uses the `finish` task. `mission_story.tl` ends with a warning for unknown keys:

```teal
	log.warning("task not found! taskKey = " .. taskKey)
	return nil
end
```

### Branching on a button

`TaskInfo.options` adds buttons to the mission window, and `followup` entries can carry a `condition`. Together they branch the graph. The wiki's example uses the `page` task, which has no logic of its own (shortened):

```teal
if taskKey == "branching_point" then
	return {
		taskScript = page as MissionInterface.TaskScript<any, any>,
		taskInfo = {
			name = _("Branching point"),
			paragraphs = { { text = _("Make your decision") } },
			options = {
				{ text = _("Choose A"), key = "finish-a" },
				{ text = _("Choose B"), key = "finish-b" },
			},
		},
		followup = {
			{ taskKey = "a", condition = function(ctx : MissionInterface.TaskContext<any, any>) : boolean
				return ctx.genericState.optionDecision == "finish-a" end },
			{ taskKey = "b", condition = function(ctx : MissionInterface.TaskContext<any, any>) : boolean
				return ctx.genericState.optionDecision == "finish-b" end },
		},
	}
end
```

A button click stores its `key` in [`genericState.optionDecision`](../base/mission/mission_framework/mission_interface.md) (symbol: `MissionInterface.GenericTaskState.optionDecision`, see [symbol index](../index.tsv)). `page.tl` defines no `isComplete`; `mission_sim.script.tl` completes any task whose `optionDecision` is `"finish"` or starts with `"finish"`, which is why both keys above begin with it. In `paragraphs`, the wiki says the first entry is the task description and later ones are hints.

### Ending the mission

A mission ends with the [`finish`](../base/mission/tasks/finish/mission_task_finish.md) (symbol: `MissionTaskFinish`, see [symbol index](../index.tsv)) task. Without parameters it is a success; `taskParams = { success = false }` makes it a failure:

```teal
if taskKey == "failure" then
	return { taskScript = finish as MissionInterface.TaskScript<any, any>, taskParams = { success = false } }
end
if taskKey == "success" then
	return { taskScript = finish as MissionInterface.TaskScript<any, any> }
end
```

`finish.tl` defaults `success` to `true` in `onSpawn` and moves to state `Complete` when the end window sends `completeMission`.

### More task fields

`StaticTaskInfo` has more options than this task uses: `stars` and `medals` (which stars a task awards), `isBonusTask`, `financialReward`, `requiredTaskToComplete`, `allowSimultaneousUiTasks`, `allowBuyVehicles`, `lockedVehicles`, `spawnGuide`, `soundtrack` and others. In `mission_sim.script.tl`, a `financialReward` books money with `makeJournalBookAssetCmd` and sends a [notification](game-mechanics.md#notifications).

## Task scripts

A task script implements [`MissionInterface.TaskScript<State, Params>`](../base/mission/mission_framework/mission_interface.md) (symbol: `MissionInterface.TaskScript`, see [symbol index](../index.tsv)). The framework only calls the callbacks a script defines (`mission_sim.script.tl` checks, for example, `if scriptOnComplete then`). The engine-side ones are `onSpawn`, `onStart`, `isComplete`, `onComplete`, `onUpdate`, `handleEvent` and `handleLegacy`; the GUI-side ones are `guiUpdate`, `guiHandleEvent`, `guiHandleProposal` and `onGuiKickOff`; and `getInfo`, `getMarkers`, `getZones`, `getNonBulldozableEntities`, `getProtectedEntities` and the rule getters describe the task to the UI. The wiki says which state each side runs in: `onUpdate` and `handleEvent` run in the engine state (the wiki calls it the engine thread), where engine functions work and `api.gui` is unavailable; `guiUpdate` and `guiHandleEvent` run in the GUI state, where it is the other way round (see [Scripting](scripting.md#the-script-states)). `onUpdate` runs every simulation step and `guiUpdate` every frame. They receive a [`TaskContext`](../base/mission/mission_framework/mission_interface.md) (symbol: `MissionInterface.TaskContext`, see [symbol index](../index.tsv)) with the task's `state`, its `params`, the `genericState` and `guiTimeSeconds`.

The `wait` task is a short complete example. From `base/content/mission/mission/tasks/wait/wait.tl` (without `handleLegacy`):

```teal
local type S = MissionTaskWait.State
local type P = MissionTaskWait.Params

local ret : MissionTaskWait = {

	taskScriptName = "wait",

	onSpawn = function(params : P, guiTimeSeconds : number) : S
		local result : S = {}
		if params.simTimeSeconds then
			local gt : Engine.Component.GameTime = api.engine.getComponent(api.engine.util.getWorld(), api.type.ComponentType.GAME_TIME)
			local currentGameTimeMs : number = gt.gameTime
			result.endSimTimeMs = currentGameTimeMs + 1000 * params.simTimeSeconds
		end
		if params.guiTimeSeconds then
			result.endGuiTimeSeconds = guiTimeSeconds + params.guiTimeSeconds
		end
		return result
	end,

	isComplete = function(ctx : MissionInterface.TaskContext<S, P>) : boolean
		if ctx.params.guiTimeSeconds then
			return ctx.guiTimeSeconds >= ctx.state.endGuiTimeSeconds
		end
		if ctx.state.endSimTimeMs then
			local gt : Engine.Component.GameTime = api.engine.getComponent(api.engine.util.getWorld(), api.type.ComponentType.GAME_TIME)
			local currentGameTimeMs : number = gt.gameTime
			return currentGameTimeMs >= ctx.state.endSimTimeMs
		end
		return false
	end,
}

return ret
```

Its type is declared in `base/tealdef/mission/tasks/wait/mission_task_wait.d.tl`, with the comments that tell the two time bases apart:

```teal
global record MissionTaskWait is MissionInterface.TaskScript<MissionTaskWait.State, MissionTaskWait.Params>
	record Params
		simTimeSeconds : number -- in simulation time (affected by game speed)
		guiTimeSeconds : number -- in approximated wall clock time (not affected by game speed, except that it is delayed when the game lags)
	end
	record State
		endSimTimeMs : number
		endGuiTimeSeconds : number
	end
end
```

`onSpawn` "prepare[s] the initial state when attempting to spawn (return nil to prevent spawning)". After every update, `mission_sim.script.tl` checks `isComplete(ctx)`; a task can also be finished through an option with key `"finish"` (mission 01 adds "Debug: Skip" options this way when `DEBUGMODE` is set).

### The base task library

The wiki describes tasks as "triggered sequentially". The official missions start several at once: mission 01 has eleven `bootTasks`, and `buildStreetDepot` above has three `followup` entries, which are all added when it completes.

The task scripts under `base/content/mission/mission/tasks/` (definitions in `base/tealdef/mission/tasks/`, reference under [base/mission/tasks](../base/mission/tasks/wait/mission_task_wait.md)):

| Folder | Tasks |
|---|---|
| `build_construction`, `build_path`, `connect_nodes`, `bulldoze`, `auto_builder` | building, connecting and removing things, automatic track building and electrification |
| `buy_vehicle` ([`Params`](../base/mission/tasks/buy_vehicle/mission_task_buy_vehicle.md) (symbol: `MissionTaskBuyVehicle.Params`, see [symbol index](../index.tsv)): `carrier`, `cargoTypeRes`, `minCount`, `allowBuyVehicles`), `assign_vehicle`, `create_line`, `vehicle_util` | buying and cloning vehicles, assigning them to lines, creating lines, selling, replacing, starting and modifying vehicles |
| `transport_cargo`, `town_size`, `town_rating`, `subsidies` | goals: deliver cargo or passengers, grow a town, reach a rating, complete [subsidies](game-mechanics.md#subsidies) that the task spawns |
| `dialogue`, `cutscene`, `page`, `guide`, `waypoints`, `observe_entity`, `click_entity`, `click_and_collect_custom_entity` | presentation and interaction |
| `wait`, `join`, `finish`, `page` | flow: wait for time or an event, wait for several tasks, end the mission, wait for a button |
| `utility` | `send_event`, `send_command`, `highlight`, `protected_entities`, `town_development`, `industry_activity`, `check_entities`, `store_any`, `store_entity`, `play_sound`, ... |
| `non_bulldozable`, `transfer_ownership`, `spawn_sims` | savegame setup |

### A mission's own task

When the library is not enough, a mission ships its own task script. Mission 01's `everything` task runs for the whole mission. In `onStart` it renames entities, sets the date and calendar speed, replaces the player's money and empties a forest's stock. From `content/mission/mission/tasks/everything/everything.tl` (shortened):

```teal
local ret : MissionInterface.TaskScript<table, string> = {
	taskScriptName = "everything",

	isComplete = function(ctx : MissionInterface.TaskContext<table, string>) : boolean
		local waitForTask = mission_framework_util.findTask(ctx.params)
		return waitForTask and waitForTask.genericState and waitForTask.genericState.completedTime ~= nil
	end,

	onStart = function(_ctx : MissionInterface.TaskContext<table, string>)
		calendar.syncDate(params.missionStartYear)
		api.cmd.sendCommand(api.cmd.makeGameSetCalendarSpeedCmd(8000))

		local cmd = api.cmd.makeJournalClearAllCmd()
		api.cmd.sendCommand(cmd)

		local entry = api.type.JournalEntry.new()
		entry.amount = 10000000
		entry.time = -1
		entry.category.type = api.type.JournalEntry.Type.OTHER
		api.cmd.sendCommand(api.cmd.makeJournalBookAssetCmd(api.engine.util.getPlayer(), entry, api.type.Vec3f.new(0, 0, 0)))
		-- ...
	end,

	onUpdate = function()
		local gameTimeComponent : Engine.Component.GameTime
				= api.engine.getComponent(api.engine.util.getWorld(), api.type.ComponentType.GAME_TIME)
		local date = api.engine.util.getCalendarDate(gameTimeComponent.gameTime)
		if date.year == params.missionEndYear - 1 and date.month == 12 and date.day == 31 then
			api.cmd.sendCommand(api.cmd.makeGameSetCalendarSpeedCmd(0))
		end
	end,
	-- handleEvent, guiUpdate ...
}
```

Its parameter is the unique key of the `missionEnd` task (`taskParams = mission_task_util.makeTaskKey("missionEnd", ...)`), so `everything` completes when the mission does. Missions 02, 03 and 05 declare their own task types in `tealdef/mission/tasks/`, for example `MissionTaskLineRate` in mission 02 with `Params { targetRate, lineEntity }` and `State { rate }`.

## Mission parameters

The tasks refer to the savegame's content by entity id. `mission_params_01.tl` collects them in one typed table:

```teal
local missionStartYear = 1900
local missionEndYear = 1910

local t : MissionParams01 = {
	crocodileSound = "urbangames_campaign_mission_01::/audio/sound/mission01_alligator.wav",

	missionStartYear = missionStartYear,
	missionEndYear = missionEndYear,

	entityList_nonPlayerOwnedLines = {
		51165, -- tram-
		33105, -- tram-
		45546, -- ship-
		44294, -- train-
		1368, -- new orleans bus line-
	},
	-- ...
	entity_relevantShipyard = 2397,--
	entity_relevantHarbor = 25264,--
```

The ids only make sense together with the mission's savegame, which is why the first boot task, `check_entities`, receives the whole table (`taskParams = { params = params }`). The type `MissionParams01` is in `tealdef/mission/mission_params_01.d.tl` and uses framework types such as `MissionInterface.PosRadius` for zones.

## Mission-wide setup outside the tasks

Two more mechanisms in mission 01 act on the whole game:

- `content/mod.script.tl` sets maintenance effects to zero in `preRunFn` and, in `postRunFn`, turns off [subsidies](game-mechanics.md#subsidies), speeds up the horse cart, hides the standard guide system and tutorial resources and hides every bridge type except the trestle bridge.
- `content/init/init/init.gs.lua` is a small game script whose `handleEvent` reacts to `initMission` and calls `mission_savegame_util.updateMaintenanceCost()` and `updateIndustriesPlayerOwned()`.

Official wiki: [Missions](https://wiki.transportfever3.com/doku.php?id=modding:scripting:missions), [Mod definition](https://wiki.transportfever3.com/doku.php?id=modding:general:moddefinition) (`visible`, `cosmetic`, `strings.json`), [Localizations](https://wiki.transportfever3.com/doku.php?id=modding:misc:localizations).
