
# Game mechanics

`base/content/game_mechanics/game_mechanics/` holds the gameplay systems of the base game: time and weather, towns, the company, loans, industries, emissions, notifications, subsidies, achievements, celebrations and the fun elements (blimps, UFOs, fireworks). Each system is a game script plus resources that the script collects at run time, and a mod can add resources of the same kind. This page lists the base game scripts and explains the systems a mod is most likely to extend: notifications, subsidies, company perks and economies. How to write a game script yourself is on [Scripting](scripting.md#game-scripts).

## The base game scripts

The base game has 28 game scripts (`.gs.lua`), the deluxe DLC adds one. Folders are below `base/content/` (DLC: `dlcs/urbangames_deluxe_upgrade_pack/content/`), shortened by the doubled zip folder. The descriptions come from the events each script subscribes to and from its state records.

| Folder | File | What it runs |
|---|---|---|
| `game_mechanics/achievements/` | `achievements.gs.lua` | achievements; listens to construction, delivery, level-up, line and build proposal events |
| `game_mechanics/arrival_tracker/` | `arrivaltracker.gs.lua` | counts people and cargo arriving at their destinations, for filters that other code creates with the `onCreatePersonFilter` and `onCreateCargoFilter` events |
| `game_mechanics/celebrations/` | `celebrations.gs.lua` | celebrations when a town or the company ranks up (`OnLevelUp`) and for new line connections |
| `game_mechanics/company/` | `company.gs.lua` | the company state: permits, greening industries (`MakeGreen`), marketing campaigns, spawning industries (`spawnIndustry`) |
| `game_mechanics/company/` | `company_progression.gs.lua` | company experience and rank (`company_growth.script`), and the ticket price multiplier (`OnCalcTicketPrice`) |
| `game_mechanics/company/explorations/` | `exploration_plane.gs.lua` | the Prospection Plane of an [exploration](#company-perks) (`startProspection`, `endProspection`) |
| `game_mechanics/emission/` | `emissions.gs.lua` | noise and pollution of industries, stations and warehouses |
| `game_mechanics/finance/` | `loan.gs.lua` | loans (`Obtain`, `Repay`) |
| `game_mechanics/fun_elements/` | `fireworks.gs.lua` | fireworks over towns (`startFireworkForTown`) |
| `game_mechanics/fun_elements/` | `fun_elements.gs.lua` | blimps for marketing campaigns and UFOs every two to four years |
| `game_mechanics/game_time/` | `game_time.gs.lua` | time of day and cloud cover (`SetMode`, `SkipPhase`) |
| `game_mechanics/industries/` | `industries.gs.lua` | industry extensions and closures |
| `game_mechanics/notifications/` | `notifications.gs.lua` | the list of [notifications](#notifications) |
| `game_mechanics/notifications/` | `availability_notifications.gs.lua` | once a year, notifications about newly available vehicles and constructions |
| `game_mechanics/subventions/` | `subventions.gs.lua` | [subsidies](#subsidies): spawning, progress, completion and failure |
| `game_mechanics/towns/` | `town.gs.lua` | town rating, marketing campaigns and reputation (`towns.script`) |
| `game_mechanics/towns/` | `town_cargo.gs.lua` | town growth and cargo demand (`town_growth.script`) |
| `gui/context_helper/` | `context_helper.gs.lua` | one flag: whether to show the welcome screen after `initNewGameFromMap` |
| `gui/hud/` | `cargo_flow.gs.lua` | cargo loaded and unloaded, for the HUD display |
| `gui/hud/` | `greenification_display.gs.lua` | HUD display of greened industries |
| `gui/hud/` | `happy_change_display.gs.lua` | HUD display of changes in happiness (`OnHappyChange`) |
| `gui/hud/` | `marketing_display.gs.lua` | HUD display of running marketing campaigns |
| `industries/` | `industry_workers.gs.lua` | industry productivity boost from arriving workers |
| `landmarks/` | `landmarks.gs.lua` | landmark progress from delivered cargo (`ItemsAboutToBeConsumed`) |
| `mission/` | `mission.gs.lua` | the [mission script](missions.md#how-a-mission-starts), `mission_sim.script` with `gameScriptMode = "Mission"` |
| `mission/` | `guide_system.gs.lua` | the same script with `gameScriptMode = "GuideSystem"`, for the guide system |
| `terrain/` | `reforestation.gs.lua` | regrowing trees, when `advancedOptions.isReforestationActive` is set |
| `vehicle/` | `vehicle_modifier.gs.lua` | speed, noise, pollution and comfort modifiers on vehicles (`ChangeModifier`) |
| DLC `fun_elements/` | `balloon.gs.lua` | the hot air balloon (`spawnBalloon`) and its notification |

Many of these scripts list console commands in a comment at the top, for example `api.cmd.sendCommand(api.cmd.makeScriptingSendEventCmd("", "Subvention", "_debugSpawn", { }))` in `subventions.script.tl`. These are a quick way to try a system in the in-game console.

## Notifications

A notification type is two files with the same base name: a generic resource of type `notification` and a `.script.tl` that returns a [`NotificationType`](../base/game_mechanics/notifications/notifications.md) (symbol: `NotificationType`, see [symbol index](../index.tsv)). The game has 33 of them. 21 are in `game_mechanics/notifications/types/`, the others next to the system that sends them (`company/`, `landmarks/`, `towns/`, `mission/`); mission 05 adds three and the deluxe DLC one.

The deluxe DLC's balloon is a complete example. The resource, `dlcs/urbangames_deluxe_upgrade_pack/content/fun_elements/fun_elements/balloon_notification.res.lua`:

```lua
function data()
	return {
		type = "notification",
		data = {
			guiType = "Info",
			label = _("Hot Air Balloon Sighting"),
			initiallyIgnoredType = false,
		},
	}
end
```

| Field | Meaning |
|---|---|
| `guiType` | `"Info"`, `"Caution"`, `"Problem"`, `"Opportunity"` or `"Achievement"` ([`NotificationGuiData.Type`](../base/game_mechanics/notifications/notifications.md) (symbol: `NotificationGuiData.Type`, see [symbol index](../index.tsv))); a missing resource counts as `"Unknown"` |
| `label` | name of the type in the filter of the Notifications log (`notification_log.tl`) |
| `initiallyIgnoredType` | `true` makes the type start out ignored in a new game |
| `simUpdateScript` | optional, `"<file>@updateData"`: a [`NotificationSimType`](../base/game_mechanics/notifications/notifications.md) (symbol: `NotificationSimType`, see [symbol index](../index.tsv)) function that the notifications game script calls in the engine state; its result is stored as the notification's `simParams` |

`industry_close.res.lua` is a base example with `simUpdateScript = "::/game_mechanics/notifications/types/industry_close.sim.script@updateData"`; its `updateData` remembers the industry's name while the industry still exists. Mission 05's resources have only `guiType`.

The script, shortened from `balloon_notification.script.tl` in the same folder:

```teal
local data : NotificationType<BalloonSpottedParam> = {}

data.useDataState = function(notificationParam : BalloonSpottedParam) : NotificationGuiData
	local nameState = engine_react_util.useStepStateTimer(function() : string
		-- ... the name of notificationParam.townEntity ...
	end)

	return {
		title = _("Hot Air Balloon Sighting"),
		description = lang_util.format(_("People report that they have spotted a hot air balloon near {townName}"), {
			townName = nameState:old()
		}),
		icon = resolve("vehicle_balloon.tga"),
		wouldClick = notification_util.makeDefaultWouldClick({notificationParam.townEntity}),
		onClick = notification_util.makeDefaultOnClick({notificationParam.townEntity}),
	}
end

return data
```

`useDataState(params, simParams)` runs in the GUI as part of a recipe and returns a [`NotificationGuiData`](../base/game_mechanics/notifications/notifications.md) (symbol: `NotificationGuiData`, see [symbol index](../index.tsv)): title, text, icon, and optionally a progress bar, a HUD icon, sounds and the click handlers. The comment in the definition says to use it as the last state in a recipe and with no other dependent state.

To show a notification, send the event `Notifications`/`add`. From `balloon.script.tl`:

```teal
api.cmd.sendCommand(api.cmd.makeScriptingSendEventCmd("", "Notifications", "add", {
	type = resolve("balloon_notification.script"),
	params = {
		entity = entity_util.makeEntityAndRevision0(balloon.entity),
		townEntity = entity_util.makeEntityAndRevision0(townEntity),
	},
	autoDismissDuration = notification_util.defaultAutoDismissDurationMs,
}))
```

`type` is the game path of the script without `.lua`. `notification_util.tl` finds the resource by replacing `.script` with `.res`, which is why both files need the same base name. `params` arrive in `useDataState`. `autoDismissDuration` hides the notification after that many milliseconds (60000 by default); according to the comment in [`Notification`](../base/game_mechanics/notifications/notifications.md) (symbol: `Notification`, see [symbol index](../index.tsv)) that is not the same as dismissing it. The fields are in [`Notification`](../base/game_mechanics/notifications/notifications.md) (symbol: `Notification`, see [symbol index](../index.tsv)).

The notifications game script drops `add` events while it is paused. It starts paused when a mission begins (`initMission`), and the mission game script sends `Notifications`/`pause` with `true` and then with `false` in its first two updates. A notification sent before that is lost. Missions can also hide types with `notificationFilter` ([Missions](missions.md#defining-tasks)), which the mission script passes on as `updateIgnoredTypes`.

Mission 05's drilling notification shows the persistent kind, which stays while a condition lasts and is bound to entities. `mods/release/urbangames_campaign_mission_05/content/mission/mission/tasks/drilling/drilling.tl` sends it with `updatePersistent` and removes it with `removePersistent`:

```teal
local persistentNotifications : PersistentNotifications = {
	type = "urbangames_campaign_mission_05::/mission/tasks/drilling/mission_drilling_notification.script",
	entitiesAndParam = {{
		entities = entity_util.makeEntityAndRevision0Array(getEntititesForNotification(entity)),
		param = notificationParams,
	}},
	passOnlyParam = true,
}
api.cmd.sendCommand(api.cmd.makeScriptingSendEventCmd("", "Notifications", "updatePersistent", persistentNotifications))
```

When the drilling ends, the task removes the persistent notification and sends a normal one with `add`. The records are [`PersistentNotifications`](../base/game_mechanics/notifications/notifications.md) (symbol: `PersistentNotifications`, see [symbol index](../index.tsv)) and [`RemovePersistentNotifications`](../base/game_mechanics/notifications/notifications.md) (symbol: `RemovePersistentNotifications`, see [symbol index](../index.tsv)); the helpers are in [`notification_util`](../base/game_mechanics/notifications/notification_util.md).

## Subsidies

A subsidy type is a generic resource of type `subvention` that points at a script. The base game has four in `game_mechanics/subventions/` (`deliver_cargo`, `deliver_cargo_town`, `deliver_passengers`, `deliver_workers`), and mission 06 adds `deliver_visitors`. From `mods/release/urbangames_campaign_mission_06/content/subventions/subventions/deliver_visitors/deliver_visitors.res.lua`:

```lua
function data()
return {
	type = "subvention",
	data = {
		scriptFile = "urbangames_campaign_mission_06::/subventions/deliver_visitors/deliver_visitors.script@deliver_visitors",
		icons = {
			fallback = "urbangames_campaign_mission_06::/subventions/deliver_visitors/deliver_visitors.tga",
		}
	}
}
end
```

`scriptFile` names a table in the script file; the script ends with `return { deliver_visitors = ret }`. `icons` ([`SubventionIcons`](../base/game_mechanics/subventions/subvention.md) (symbol: `SubventionIcons`, see [symbol index](../index.tsv))) can also have `proposed`, `active`, `complete` and `failed`; the shipped files only set `fallback`.

The table is a [`SubventionScript`](../base/game_mechanics/subventions/subvention.md) (symbol: `SubventionScript`, see [symbol index](../index.tsv)). The subsidies game script calls its functions by name:

| Function | Called |
|---|---|
| `canSpawn()` | to decide whether the type may spawn now; `deliver_visitors` returns `true` while a landmark has no subsidy |
| `onSpawn()` | for a random spawn; returns the subsidy data and a uid |
| `spawnSpecific(param)` | when a script asks for this type with given targets (see below) |
| `isComplete`, `onComplete`, `onFailure`, `onAccept`, `onUpdate`, `shouldDespawn`, `onDespawn` | while the subsidy is proposed or active |
| `eventsToHandle`, `handleEvent` | for game events the type needs, for example arrivals |
| `getNotificationParams`, `getNotificationData`, `getNotificationMissedData` | for the notifications about the subsidy |
| `getCardData`, `getRelevantEntities`, `getRelevantEntityNames` | for the subsidy window |

The data a type returns follows `ISubvention.ISubventionData` ([`ISubvention`](../base/game_mechanics/subventions/subvention.md) (symbol: `ISubvention`, see [symbol index](../index.tsv))): `upfront`, `complete` and `failure` rewards or penalties (money, reputation, town experience), `toDeliver`, `delivered` and the durations.

The game script spawns subsidies at random. It collects every `subvention` resource with `getAllOfType("subvention")`, keeps those whose `canSpawn()` returns `true` and picks one. The interval depends on `advancedOptions.subventionMode` in the [base config](scripting.md#base-config) (0 turns subsidies off; 1, 2, 3 and 4 give about 12, 6, 3 and 1 months, up to twice as long at the highest company rank; `subvention_util.tl`) and doesn't run in the Map Editor. Mission 06 shows that a mod's type joins this pool, since `deliver_visitors` has a working `canSpawn`.

A script can also spawn one type on purpose. The base mission task `subsidies` (`base/content/mission/mission/tasks/subsidies/subsidies.tl`) does it like this, and mission 06 passes it `deliver_visitors` with a landmark as `target`:

```teal
api.cmd.sendCommand(api.cmd.makeScriptingSendEventCmd("", "Subvention", "subventionSpawn", { name = subsidy[1], specific = subsidy[2] }))
```

`name` is the resource (`"urbangames_campaign_mission_06::/subventions/deliver_visitors/deliver_visitors.res"`), `specific` the parameters for `spawnSpecific`; `expireDurationProposed = -1` means the offer does not expire. The parameter records of the base types are in [`specific_subvention`](../base/game_mechanics/subventions/specific_subvention.md). The event `Subvention`/`setPause` with `true` or `false` stops and resumes random spawning.

## Company perks

The Perks menu unlocks things by company rank. Each perk has a permit key, and `rankAndPermits` lists `{rank, permits}` pairs: at each rank the company gets that many more permits, counted together (`company_util.tl`). A permit is used up when the player builds or starts the perk. Landmarks and the headquarters are constructions with this metadata and are described under [Landmarks](constructions.md#landmarks). Three other kinds exist.

Explorations are generic resources of type `company_exploration`, 12 of them in `game_mechanics/company/explorations/`. From `exploration_clay.res.lua`:

```lua
function data()
return {
	type = "company_exploration",
	data = {
		cargoType = "::/cargos/clay/clay.cargo",
		icon = "game_mechanics/company/explorations/exploration_clay.tga",
		previewIcon = nil,
		lockedIcon = "game_mechanics/company/explorations/exploration_clay_locked.tga",
		rankAndPermits = {
			{3, 1},
		},
		requiredTags = {
			"OUTPUT_CLAY",
		},
		menuCategory = {
            categories = {
                {
                    category = "prospections",
                    order = 2000,
                },
            },
        },
	}
}
end
```

The build menu (`construction_react_util.tl`) offers an exploration only if an industry in the current [economy](#economies) has every tag in `requiredTags` in its `placementParams` `tags`. The exploration then works as a perk of category `PROSPECTION`, and `exploration_plane.gs.lua` flies the Prospection Plane. Instead of its own `rankAndPermits`, an exploration can share a permit key: `exploration_grain.res.lua` and `exploration_vegetables.res.lua` set `permitKey = "::/game_mechanics/company/permitKeys/permit_key_farm.res"`. The fields are in [`Exploration`](../base/game_mechanics/company/company.md) (symbol: `Exploration`, see [symbol index](../index.tsv)).

Permit keys are generic resources of type `company_permit_key`, three of them in `game_mechanics/company/permitKeys/`. A key gives several perks one shared count of permits. From `marketing.res.lua`:

```lua
function data()
return {
	type = "company_permit_key",
	data = {
		name = _("Marketing"),
		rankAndPermits = {{4,1}},
		cooldownInSeconds = 5 * 4 * 365.25, -- 5 years
	}
}
end
```

`hq.res.lua` (name "Headquarters", `{{1,1}}`) is the key of `landmarks/hq/hq/headquarter.con.lua`, `marketing.res.lua` that of the Marketing Campaign tool, `permit_key_farm.res.lua` (name "Industry") that of the two farm explorations. `cooldownInSeconds` makes a used permit come back after that time. A perk names its key with `permitKey` in the company metadata; without one, the perk's own resource name is its key ([`PermitKeyDesc`](../base/game_mechanics/company/company_static_util.md) (symbol: `CompanyStaticUtil.PermitKeyDesc`, see [symbol index](../index.tsv))).

Mechanics perks are tools or constructions with `perkCategory = "MECHANICS"` in their company metadata ([`CompanyMetadata.ConstructionDesc`](../base/game_mechanics/company/company_metadata.md) (symbol: `CompanyMetadata.ConstructionDesc`, see [symbol index](../index.tsv))). The Cleaning Plant is a construction, from `game_mechanics/game_mechanics/emission/pollution_cleanup_facility.con.lua`:

```lua
constr.metadata = {
    company = {
		perkCategory = "MECHANICS",
		rankAndPermits = {
			{10, 15},
		},
    },
}
```

The Industry Greenification and Marketing Campaign tools (`gui/gui/construction/tools/industry_greenify_tool.script.tl`, `marketing_campaign_tool.script.tl`) are construction tools that set the same metadata with `company_metadata.constructionDesc.make{ perkCategory = "MECHANICS", ... }`. The Industry Greenification tool uses a plain string as its key (`permitKey = "ECO_INDUSTRY"`) and gains a permit at ranks 3, 6, 9, 12 and 15.

The base game collects explorations and permit keys with `getAllOfType`, so a mod's resource of the same type should appear next to the base ones. No shipped mod adds one, so this is untested.

## Economies

An economy (`.eco.lua`) decides which industries and cargo types a game uses and how the map generator places industries. The base game has five in `base/content/economy/economy/`: `all`, `temperate`, `dry`, `tropic` and `subarctic`. Missions 02, 04, 06, 07 and 08 ship their own. A climate names its default economy in its `economy` field ([Climates](environment.md#climates)); the economy chosen for the game ends up in `baseConfig.economyId` (`base/content/base/base/mod.script.tl`). Scripts read it with `api.res.economyRep.get(api.res.economyRep.find(api.res.getBaseConfig().economyId))`.

The end of `base/content/economy/economy/temperate.eco.lua`:

```lua
return {
	desc = {
		name = _("Temperate"),
		icon = "temperate.tga"
	},
	category = "temperate.eco",
	cargoCategories = cargoCategories,
	waterIndustryFraction = 6.0 / 41.0,
	placementParams = placementParams,
	cargoRequiredIndustries = cargoRequiredIndustries,
	companyRankRequiredIndustries = companyRankRequiredIndustries,
	townLevels2CargoInfos = placementparamsutil.getDefaultTownLevels2CargoInfos(),
	defaultIndustryProductivity = 0.58,
	industryPlacementFallbacks = {
		oil_platform = "oil_well",
		sand_excavator = "sand_pit",
	},
}
```

| Field | Meaning |
|---|---|
| `desc`, `order`, `visible` | name and icon, sort order and whether the economy can be chosen in the menu; mission 04's economy sets `visible = false` |
| `category` | the tag that industries and cargo types list in their `categoryList` to exist in this economy |
| `placementParams` | one entry per industry, keyed by the industry's `placementTag` (see below) |
| `cargoCategories` | one entry per cargo category: `tier`, `landUses`, `requiredIndustryTags`, `townConsumptionFactor` |
| `townLevels2CargoInfos` | per town level, the cargo tiers and land use that towns start to demand |
| `cargoRequiredIndustries` | per cargo, the industries of its supply chain with a factor each |
| `companyRankRequiredIndustries` | per company rank, industries that the rank needs on the map |
| `industryPlacementFallbacks` | industry to place instead when one can't be placed, here a land oil well for the oil platform |
| `waterIndustryFraction`, `defaultIndustryProductivity` | share of industries on water, starting productivity |

The meaning of the last five rows comes from the field names and values; the definition ([`EconomyDesc`](../api/api/type.md) (symbol: `EconomyDesc`, see [symbol index](../index.tsv))) has comments only for `desc`, `order`, `visible` and `category`.

`placementParams` comes from `placementparamsutil.getAllParams()` in `base/content/economy/economy/placementparamsutil.lua`. Its first entry:

```lua
brewery = {
	buildOrder = 190,
	initWeight = 10,
	tags = { "INPUT_GRAIN", "INPUT_GLASS" },
	distanceWeights = {
		TOWN = CLOSE,
	},
	weights = {
		biome0 = BIOME_VERY_FAR,
		biome1 = BIOME_CLOSE,
		biome2 = BIOME_CLOSE,
		biome3 = BIOME_CLOSE,
		biome4 = BIOME_VERY_FAR,
	},
	requiredIndustryTags = {"glass_works", "farm"},
	tier = 1,
},
```

The key `brewery` is the `placementTag` of `industries/brewery/brewery/brewery.con.lua` ([Industries](constructions.md#industries)). `tags` describe what the industry takes and gives (`INPUT_*`, `OUTPUT_*`); explorations match against them. `distanceWeights` pull the industry towards towns or tagged industries (positive) or push it away (negative); the file defines `VERY_CLOSE = 2` down to `VERY_FAR = -2`. `weights` rate the biome masks `biome0` to `biome4`, from `BIOME_BEST = -8000` to `BIOME_VERY_FAR = 4000`. `requiredIndustryTags` names the industries that must exist for this one, here its suppliers. `buildOrder` and `initWeight` set the order and the starting weight of placement; `allowedOnLand` and `allowedOnWater` mark water industries such as `oil_platform`. What `tier` does is not documented. The record is [`PlacementParams`](../api/api/type.md) (symbol: `PlacementParams`, see [symbol index](../index.tsv)).

Each economy starts from the full list and removes what it doesn't use. From `temperate.eco.lua`:

```lua
local placementParams = placementparamsutil.getAllParams()

placementParams.cement_plant = nil
placementParams.paper_mill = nil
-- ...
local cargoCategories = placementparamsutil.getCargoCategories()
cargoCategories.cement = nil
cargoCategories.books = nil
-- ...
placementparamsutil.removeTags(placementParams, {"OUTPUT_RUBBER"})
```

Removing an entry keeps the map generator from placing that industry. Which constructions and cargo types the player sees is decided separately: the base game's `postRunFn` hides every industry and cargo type whose `categoryList` doesn't contain the economy's `category`, except in the `all` economy (`mod.script.tl`). A cargo type's `categoryList` is described under [Cargo types](other-content.md#cargo-types).

A new industry needs both: its `.con.lua` with `isIndustry = true`, a `placementTag` and the economy categories in `categoryList`, and a `placementParams` entry under that tag. Without the entry the industry exists but the economy doesn't place it; mission 04's `pharmacy.con.lua` has `placementTag = "pharmacy"` and no entry in the mission's economy. No shipped mod adds an entry to a base economy. Judging by the file shape, a [`loadEconomy` modifier](scripting.md#resource-modifiers) in `runFn` should work; this sketch is not tested:

```lua
function data()
return {
	runFn = function(captureParams, configDict, allModParams)
		addModifier("loadEconomy", function(fileName, data)
			if data.category == "temperate.eco" and data.placementParams then
				data.placementParams.jdoe_cider_press = {
					buildOrder = 195,
					initWeight = 10,
					tags = { "INPUT_FRUIT" },
					distanceWeights = { TOWN = 1 },
					requiredIndustryTags = { "farm" },
					tier = 1,
				}
			end
			return data
		end)
	end,
}
end
```

Mission 04's own economy keeps `category = "temperate.eco"`, so the base industries and cargo types that list this category stay visible in it. A mod that ships a whole economy of its own, as the missions do, can start from `placementparamsutil.getAllParams()` with `require "::/economy/placementparamsutil.lua"` and change the table. The repository is [`api.res.economyRep`](../api/api/res.md) (symbol: `Res.economyRep`, see [symbol index](../index.tsv)).

Official wiki: [Climates](https://wiki.transportfever3.com/doku.php?id=modding:environment:climates), [Construction types](https://wiki.transportfever3.com/doku.php?id=modding:constructions:types), [Cargo](https://wiki.transportfever3.com/doku.php?id=modding:misc:cargo).
