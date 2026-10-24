
# Infrastructure

Tracks, streets, edge add-ons, bridges, tunnels, signals, railroad crossings and traffic lights are separate resource types, each with its own file extension. The base game keeps them under `base/content/infrastructure/` (the doubled folder names such as `track/track/` come from unpacking the zips, see [Mod structure](mod-structure.md#how-the-zips-map-to-folders)). Each file returns a table from `data()`. The game loads it into a repository under `api.res`, and a mod can change it while loading through `addModifier`.

| What | File | Base game count | Repository | Type |
|---|---|---|---|---|
| Track or street template | `.street_template.lua` | 87 (6 tracks) | [`streetTemplateRep`](../api/api/res.md) (symbol: `Res.streetTemplateRep`, see [symbol index](../index.tsv)) | [`StreetTemplate`](../api/api/type.md) (symbol: `StreetTemplate`, see [symbol index](../index.tsv)) |
| Track or street style | `.street.lua` | 28 | [`streetStyleRep`](../api/api/res.md) (symbol: `Res.streetStyleRep`, see [symbol index](../index.tsv)) | [`StreetStyle`](../api/api/type.md) (symbol: `StreetStyle`, see [symbol index](../index.tsv)) |
| Edge add-on | `.edge.lua` | 6 | [`edgeDecorationRep`](../api/api/res.md) (symbol: `Res.edgeDecorationRep`, see [symbol index](../index.tsv)) | [`EdgeDecoration`](../api/api/type.md) (symbol: `EdgeDecoration`, see [symbol index](../index.tsv)) |
| Bridge | `.bridge.lua` | 9 | [`bridgeTypeRep`](../api/api/res.md) (symbol: `Res.bridgeTypeRep`, see [symbol index](../index.tsv)) | [`BridgeType`](../api/api/type.md) (symbol: `BridgeType`, see [symbol index](../index.tsv)) |
| Tunnel | `.tunnel.lua` | 6 | [`tunnelTypeRep`](../api/api/res.md) (symbol: `Res.tunnelTypeRep`, see [symbol index](../index.tsv)) | [`BridgeType`](../api/api/type.md) (symbol: `BridgeType`, see [symbol index](../index.tsv)) as well |
| Signal | `.con.lua` with `edgeObject` | 2 | [`constructionRep`](../api/api/res.md) (symbol: `Res.constructionRep`, see [symbol index](../index.tsv)) | [`ConstructionDesc`](../api/api/type.md) (symbol: `ConstructionDesc`, see [symbol index](../index.tsv)) |
| Railroad crossing | `.rcr.lua` | 10 | [`railroadCrossingTypeRep`](../api/api/res.md) (symbol: `Res.railroadCrossingTypeRep`, see [symbol index](../index.tsv)) | [`RailroadCrossingType`](../api/api/type.md) (symbol: `RailroadCrossingType`, see [symbol index](../index.tsv)) |
| Traffic light | `.trl.lua` | 4 | [`trafficLightTypeRep`](../api/api/res.md) (symbol: `Res.trafficLightTypeRep`, see [symbol index](../index.tsv)) | [`TrafficLightType`](../api/api/type.md) (symbol: `TrafficLightType`, see [symbol index](../index.tsv)) |

The counts are for `base/content/infrastructure/`; the 81 street templates include airport runways and taxiways and the short entrance streets used inside constructions.

The modifier keys for these files are `loadStreetTemplate`, `loadStreet`, `loadTrack`, `loadEdgeDecoration`, `loadBridge`, `loadTunnel`, `loadRailroadCrossing` and `loadTrafficLight` (listed in `base/content/base/base/mod.lua`; signals go through `loadConstruction`). The base game's own modifiers in `base/content/base/base/base_mod.lua` translate older field names, which explains why both spellings of some fields work:

- `loadStreetTemplate` builds `availability` from top-level `yearFrom`/`yearTo` and `description` from `name`, `desc`, `icon`, `previewIcon` when those tables are missing. A lane without `speed` gets the template's `speed` divided by 3.6, so the old field was in km/h. `streetFillGroundTex` is copied to `fillGroundTex`.
- `loadStreet` turns a style's `materials.ballastMaterial` string into `materials.ballast = { name = ..., size = { 8.0, 8.0 } }`.
- `loadBridge`, `loadTunnel` and `loadRailroadCrossing` do the same `availability`/`description` conversion.
- `loadConstruction` turns an old `isEdgeObject = true` into `edgeObject = { snapToStreet = true, snapToTrack = true, minDistToCrossing = 10 }`.

`description`, `availability` and `menuCategory` work as for constructions in all of these files (see [Constructions](constructions.md)). In the wiki's words, an availability `yearTo` below 1900 gives a type that is never available, and `0` or unset means no end.

## Tracks and streets

Tracks and streets use the same two file types. The `.street_template.lua` holds what the simulation needs: lanes, speeds, costs, slopes, emissions. The `.street.lua` it points to with `streetStyle` holds the looks: materials, models, catenary. Several templates can share one style: 15 `town_old_*` templates (sizes plus their `_barriers`, `_trees` and `_tram` variants) use `town_old.street`. `roadType` is `"STREET"` or `"TRACK"` in both files. The [wiki page](https://wiki.transportfever3.com/doku.php?id=modding:infrastructure:tracksstreets) describes every field; this section covers the ones a new type needs.

### A track template

`base/content/infrastructure/track/track/standard/standard.street_template.lua` (shortened, floats rounded):

```lua
function data()
return {
		availability = { yearFrom = 1930, yearTo = 0, },
		catenaryAdd = "::/infrastructure/track/standard/standard_catenary.street_template",
		cost = 180,
		maintenanceCost = 30,
		laneConfigs = {
			{
				forward = true,
				height = 0.53,
				offset = 0,
				speed = 44.44,
				transportModes = { "TRAIN", "TRAM_TRACK", },
				width = 4,
			},
		},
		maxSlope = 0.13,
		maxSlopeBuild = 0.085,
		maxSlopeShape = 0.16,
		minCurveRadius = 40,
		minCurveRadiusBuild = 55,
		roadType = "TRACK",
		speedCoeffs = { 0.85, 30, 0.6, },
		streetStyle = "::/infrastructure/track/standard/standard.street",
		trackDistance = 5,
		-- description, emissions, modifiers, ground textures, menuCategory, builderAudioRes
	}
end
```

The electrified version is a second template, `standard_catenary.street_template.lua`, whose lane adds `"ELECTRIC_TRAIN"` and `"ELECTRIC_TRAM_TRACK"` and which points back with `catenaryRemove`. Building or removing catenary swaps one template for the other.

| Field | Meaning (from the wiki) |
|---|---|
| `laneConfigs` | One entry per lane: `speed` in m/s, `width`, `offset`, `height` (raises the sidewalk on streets), `forward` (same direction as the player dragged), `transportModes` |
| `transportModes` | `"PERSON"`, `"CARGO"` for sidewalks; `"CAR"`, `"BUS"`, `"TRUCK"` for road lanes; `"TRAM"`, `"ELECTRIC_TRAM"` for street tram; `"TRAM_TRACK"`, `"ELECTRIC_TRAM_TRACK"` for trams on rail track; `"TRAIN"`, `"ELECTRIC_TRAIN"` |
| `cost`, `maintenanceCost` | per meter |
| `maxSlopeBuild`, `maxSlope`, `maxSlopeShape` | slope of the straight line between the end points; of the curve that follows the end slopes; of the built shape when crossings push it around |
| `speedCoeffs` | curve speed limit is `a * (radius + b) ^ c` |
| `minCurveRadiusBuild`, `minCurveRadius` | smallest radius when dragging freely, and when snapping to existing track |
| `trackDistance` | distance for parallel tracks; the wiki warns that values other than 5 can break ballast and catenary gantries |
| `emissions`, `modifiers` | noise and pollution of the edge, and factors for top speed, noise and pollution of the vehicles on it |
| `defaultEdgeDecorations` | `.edge` references with a boolean for the left side, see [Edge add-ons](#edge-add-ons) |

### A street template

A street lists its sidewalks as lanes too. `base/content/infrastructure/street/street/town/town_old_small.street_template.lua` has four lanes, from left to right in build direction:

```lua
		laneConfigs = {
			{ forward = false, height = 0.15, offset = 0.15, speed = 11.11, transportModes = { "PERSON", "CARGO", }, width = 3, },
			{ forward = false, height = 0,    offset = -0.5, speed = 11.11, transportModes = { "CAR", "BUS", "TRUCK", }, width = 5, },
			{ forward = true,  height = 0,    offset = 0.5,  speed = 11.11, transportModes = { "CAR", "BUS", "TRUCK", }, width = 5, },
			{ forward = true,  height = 0.15, offset = -0.15, speed = 11.11, transportModes = { "PERSON", "CARGO", }, width = 3, },
		},
		priority = 510,
		roadType = "STREET",
		simBuildable = true,
		streetStyle = "::/infrastructure/street/town/town_old.street",
```

Fields only streets use, per the wiki: `simBuildable` (towns may upgrade to this type), `country` (no houses spawn along it), `priority` (whose markings win at an intersection), `busAndTramRight`, `defaultWithCrosswalk`, `pedestrianWalkPenalty` and `transportModesStreet`. The wiki also lists `aiLock`, which keeps town development from changing built segments. No base game file sets `aiLock`; the six highway templates and the six construction entrance streets set `initiallyLocked = true` instead, which the reference describes as the [initial value of the Locking Tool](../api/api/type.md) (symbol: `StreetTemplate.initiallyLocked`, see [symbol index](../index.tsv)).

### Styles

A track style (`base/content/infrastructure/track/track/standard/standard.street.lua`, shortened) names the models and the dimensions of the procedural parts:

```lua
	return {
			roadType = "TRACK",
			ballastHeight = 0.3,
			ballastCutOff = 0.35,
			trackStraightModel = { "2m_standard.mdl", "4m_standard.mdl", "8m_standard.mdl", "16m_standard.mdl", },
			sleeperModel = "single_sleeper_standard.mdl",
			railModel = "::/infrastructure/track/shared/single_rail.mdl",
			railTrackWidth = 1.435,
			shapeStep = 2.0,
			bumperModel = "bumper_standard.mdl",
			switchSignalModel = "switch_box_standard.mdl",
			catenary = {
				catenaryPoleDistance = 24,
				catenaryMaxPoleDistance = 3.33,
				catenaryMinPoleDistance = 0.9,
				catenaryPoleModel = "power_pole_2_standard.mdl",
				catenaryMultiPoleModel = "power_pole_1_standard.mdl",
				catenaryMultiGirderModel = "power_pole_1a_standard.mdl",
				catenaryMultiInnerPoleModel = "power_pole_1b_standard.mdl",
				-- catenaryBase, catenaryHeight
			},
			materials = {
				ballast = { name = "::/infrastructure/track/shared/mat/ballast.mtl", size = { 7.0, 7.0 } },
				sleeperMaterial = "mat/sleeper_standard_tileable.mtl",
				railMaterial = "::/infrastructure/track/shared/mat/rail_tileable.mtl",
				catenaryMaterial = "::/infrastructure/shared/mat/catenary.mtl",
			},
			-- sleeper and rail dimensions
		}
```

`trackStraightModel` holds pieces of 2, 4, 8 and 16 m with sleepers. Short sections and tight curves are built from one `sleeperModel` and two `railModel`s, and switches and crossings get procedural sleepers and rails from the dimension fields. The wiki says all of these models have their origin at terrain height, centered in length and width. The catenary uses the single pole for one track, turned outward for two, multi poles with girders for three or four, and extra inner poles beyond four.

The wiki and the base files disagree on two names here. The wiki writes the pole limits as `catenaryMaxPoleDistanceFactor` and `catenaryMinPoleDistanceFactor`; all four base track styles (`standard`, `simple`, `high_speed`, `industries/shared/shared/industry_track/industry.street.lua`) use `catenaryMaxPoleDistance` and `catenaryMinPoleDistance` with factor-sized values (3.33, 0.9). The wiki writes `ballastMaterial = "..."`; the base files use `ballast = { name, size }`, and the `loadStreet` modifier above converts the wiki form with an 8 m tile.

A street style has a `materials` table of `{ name = "....mtl", size = { x, y } }` entries for paving, lanes, borders, stripes, sidewalks, tram tracks, bus lanes, crossings and junctions (the wiki lists all 25 keys). A key that is left out is invisible in the game; `town_old.street.lua` leaves out `sidewalkBorderOuter`. Base paving materials need `polygon_offset = { factor = -3, units = -1.2, forceDepthWrite = true }` and overlays `{ factor = -4, units = -1.3 }`, according to the wiki. The `assets` table places models along the street, keyed by name so that bridges and tunnels can replace them. From `base/content/infrastructure/street/street/town/town_old.street.lua`:

```lua
	assets = {
		["street_light"] = {
			name = "/assets/streets/street_light_eu_a.mdl",
			offset = 6.0,
			distance = 12.0,
			prob = 1.0,
			offsetOrth = 2.4,
			randRot = false,
			oneSideOnly = false,
			alignToElevation = false,
			avoidFaceEdges = false,
			placeOnBridge = true,
		},
		-- "fireplug", "mailbox", "trash"
	},
	categoryList = {
		categories = { "withSidewalk", "town", "old", }
	},
```

The style's `categoryList` adds tags to the edge that edge add-ons can require (see the next section).

### Track modules in stations

The base game's `postRunFn` in `base/content/base/base/mod.script.tl` creates one station track module for every template with `roadType` TRACK, including the catenary variants, and adds it with `api.res.moduleRep.add`. The module's file name is `"::/trainstation_"` followed by the template name with `::` replaced by `__`. A new track type therefore shows up in the modular station automatically.

The wiki shows how to hide such modules in a mod's own `postRunFn` with [`api.res.moduleRep.setVisible`](../api/api/res.md) (symbol: `ResTypeRep.setVisible`, see [symbol index](../index.tsv)). Its sample names the modules `::trainstation_<tracktypename>.lua` and `...luacatenary`, which is not the naming in the base script above, and loops over `getAll()` with `ipairs` and `index - 1`. [`getAll`](../api/api/res.md) (symbol: `ResTypeRep.getAll`, see [symbol index](../index.tsv)) returns a table from id to name, so iterating with `pairs` and using the key as the id, as the base script does, avoids the off-by-one guess:

```lua
postRunFn = function()
	local hide = { ["::/trainstation___/my_track/my_track.street_template"] = true } -- check the real name in the log or with getAll()
	for id, name in pairs(api.res.moduleRep.getAll()) do
		if hide[name] then api.res.moduleRep.setVisible(id, false) end
	end
end,
```

The exact module name for a mod's track depends on how its template name is written in `streetTemplateRep`; the files don't show a mod track, so print `getAll()` once before relying on a name.

## Edge add-ons

Edge add-ons (the reference calls them edge decorations) are the noise barriers and tree alleys that the player paints onto existing edges. `base/content/infrastructure/edge_addons/edge_addons/barrier_a.edge.lua` (shortened):

```lua
function data()
return {
    availability = { yearFrom = 0, yearTo = 1940, },
    cost = { price = 100, priceScale = 1, },
    maintenance = { runningCosts = 10, },
    desc = {
        icon = "barrier_a.tga",
        previewIcon = "barrier_a_preview.tga",
        name = _("INFRA_EDGE_ADDON_BARRIER_A_NAME"),
        description = _("INFRA_EDGE_ADDON_BARRIER_A_DESCRIPTION"),
    },
    categoryList = { categories = { "barrier" } },
    menuCategory = { categories = { { category = "roads_tools", order = 400, }, } },

    repModel = "barrier_a.mdl",
    length = 3.0,
    stretch = true,
    verticalAlignment = "Shear", -- "Vertical", "Rotate"
    offset = 0.0,
    snapMode = "Side", -- "Side", "Center"
    skipSameInParallelStrip = true,

    incompatibleTags = {"track", "tunnel", "decorated"},
    modifiers = { noiseModifier = -0.65, },
    preventParcels = true,
}
end
```

The description table is `desc` here, not `description`. An add-on can be placed when the edge has every tag in `requiredTags` and none in `incompatibleTags`. The engine sets `track`, `street`, `bridge`, `tunnel` and `decorated` (at least one add-on present); the street style's `categoryList` and the `categoryList` of add-ons already on the edge add more. `alley.edge.lua` uses that: it requires `withSidewalk`, which `town_old.street`, `town_new.street` and `town_new_large.street` provide. `barrier_track_a.edge.lua` is the same wall for tracks, with `"street"` in place of `"track"` in `incompatibleTags`.

The model `repModel` is placed once per `length` meters at `offset` from the edge axis. `snapMode` is `"Side"` (each side separately) or `"Center"`. `modifiers` change noise, pollution and top speed of vehicles on the edge, and `preventParcels` stops town buildings on that side.

The wiki doesn't describe `stretch` and `verticalAlignment`, which the four base barriers set, or `order`, which all six set. The reference explains them: [`stretch`](../api/api/type.md) (symbol: `EdgeDecoration.stretch`, see [symbol index](../index.tsv)) scales the model to close gaps, and [`verticalAlignment`](../api/api/type.md) (symbol: `EdgeDecoration.verticalAlignment`, see [symbol index](../index.tsv)) keeps the model vertical, shears it along the slope, or rotates it to the street. The reference types [`snapMode`](../api/api/type.md) (symbol: `EdgeDecoration.snapMode`, see [symbol index](../index.tsv)) and `verticalAlignment` as integers; the base files write the strings `"Side"`, `"Center"`, `"Shear"`. The alleys (`alley.edge.lua`, `alley_old.edge.lua`) use `modelRotation = "RandomXY"` for random rotation of the trees around the z axis, as the wiki describes; the reference's `EdgeDecoration` has no such field.

A street template can preset add-ons, for example `town_old_small_barriers.street_template.lua`:

```lua
		defaultEdgeDecorations = {
			{ "::/infrastructure/edge_addons/barrier_a.edge", false, },
			{ "::/infrastructure/edge_addons/barrier_a.edge", true, },
		},
```

The `_trees` variants do the same with `alley.edge`.

## Bridges and tunnels

A bridge type describes the costs, the pillar spacing and the replacements for the street or track materials on top. The geometry comes from an `updateScript` function that receives the pillar heights and the railing sections and returns model lists. `base/content/infrastructure/bridge/bridge/stone.bridge.lua` (model tables shortened):

```lua
function data()
local dir = "stone/"
local pillar_btm_rep = {
	dir .. "pillar_btm_rep.mdl",
	{ { -2.0, 0.0, 0.0, }, { 2.0, 4.0, 4.0, } },   -- bounding box
}
-- ... about 40 more model entries
local config = {
	pillarBase = { pillar_btm_side, pillar_btm_rep, pillar_btm_side2 },
	pillarRepeat = { pillar_rep_side, pillar_rep_rep, pillar_rep_side2 },
	pillarTop = { pillar_top_side, pillar_top_rep, pillar_top_side2 },
	railingBegin = { ... },   -- 8 models
	railingRepeat = { ... },
	railingEnd = { ... },
	abutmentBase = { stone_abut_bot_l, stone_abut_bot_rep, stone_abut_bot_r },
	abutmentRepeat = { stone_abut_rep_l, stone_abut_rep_rep, stone_abut_rep_r },
	abutmentTop = { stone_abut_top_l, stone_abut_top_rep, stone_abut_top_r },
}

return {
	description = { name = _("INFRA_BRIDGE_STONE_NAME"), icon = "stone.tga", },
	availability = { yearFrom = 0, yearTo = 0, },
	carriers = { "RAIL" , "ROAD"},
	speedLimit = 90.0 / 3.6,
	pillarLen = 3,
	pillarMinDist = 12.0,
	pillarMaxDist = 48.0,
	pillarTargetDist = 12.0,
	abutmentLen = 5.0,
	cost = 200.0,
	costFactors = { 10.0, 2.5, 1.0 },
	pillarGroundTexture = "/terrain/materials/dirt/dirt.gtex",
	pillarGroundTextureOffset = 2.0,
	materialsToReplace = {
		streetPaving = { name = "::/infrastructure/street/town/mat/old_town_paving.mtl", size = { 6.0, 6.0 } },
		streetStripe = { },
		-- ...
	},
	updateScript = {
		fileName = "bridge.script@stone.updateFn",
		params = config
	},
}
end
```

`bridge.script.lua` next to it maps `stone`, `steel`, `trestle` and `placeholder` to `bridgeutil2.makeDefaultUpdateFn()`, and `tarch`, `cable` and `suspension` to their own functions in `bridgeutil2.lua`. Each model entry is `{ path, bounding box }`. A layer list with one model places it in the middle, two models use the first for both edges and repeat the second, three give left edge, repeated middle and right edge. A railing list has 5 or 8 models for the side, side-with-collision, middle and other-side cases (with 5, the first three are reused rotated for the other side). The wiki has the details, including the `params` the update function receives (`pillarHeights`, `railingIntervals`, `hasAbutment`, ...) and the `pillarModels`/`railingModels` it must return.

What the wiki adds to the fields: `cost` and `maintenanceCost` are multiplied by the area under the bridge, which is computed every ~5 m as `5 * (a * heightDiff ^ b + c)` from `costFactors` (with `a = 1 / (costFactors[1] ^ costFactors[2])`). The street or track on top costs extra. The reference describes [`BridgeType.cost`](../api/api/type.md) (symbol: `BridgeType.cost`, see [symbol index](../index.tsv)) as "Cost per unit length", which is shorter than the wiki's area rule. `speedLimit` only applies when the edge itself is faster. Bridge segments are at most 100 m long; `noParallelStripSubdivision = true` builds the whole bridge in one call, which `cable`, `suspension` and `trestle` set. `cable` and `suspension` also set `autoGeneration = false` (not used for map generation and town roads) and `ignoreWaterCollision = true` (pillars spread evenly even into water). In `materialsToReplace` an empty table removes a material (the stone bridge drops `streetStripe`); in `assetsToReplace` an empty table drops the street asset with that key.

The wiki gives the helper's path as `/infrastructure/bridges/bridgeutil2.lua`. In the base game it is `base/content/infrastructure/bridge/bridge/bridgeutil2.lua`, which `tunnel.script.lua` loads as `"::/infrastructure/bridge/bridgeutil2.lua"`.

Tunnels use the same type, [`BridgeType`](../api/api/type.md) (symbol: `BridgeType`, see [symbol index](../index.tsv)), with `padding` (distance from the outer lanes to the wall, used to find where diverging tunnels split) and `height` (the terrain above can't be lowered below it). `base/content/infrastructure/tunnel/tunnel/tunnel_a.tunnel.lua` (model tables shortened):

```lua
	local config = {
		railingBegin = startrailing,
		railingRepeat = railing,
		railingEnd = endrailing,
		portalsConfig = { tunnel_a_startportal_rgt, tunnel_a_startportal_rep, tunnel_a_startportal_lft },
		endPortalsConfig= { tunnel_a_endportal_rgt, tunnel_a_endportal_rep, tunnel_a_endportal_lft },
		deadConfig = { tunnel_a_dead_rgt, tunnel_a_dead_rep, tunnel_a_dead_lft, tunnel_a_dead_rgt_end, tunnel_a_dead_lft_end },
		assetsConfig = {
			{ tunnel_a_m1_side_lft[1], tunnel_a_m1_add_lamp_lft1[1], 15, 3},-- first value = each n step ,second value = initial offset step
			-- ...
		}
	}
return {
	description = { name = _("INFRA_TUNNEL_ROCK_NAME"), icon = newdir .."icon/a.tga", },
	carriers = { "RAIL"},
	cost = 600.0,
	updateScript = {
		fileName = "tunnel.script@tunnel.updateFn",
		params = config
	},
	padding = 2,
	height = 8.66 + 1.0,
	version = 1,
}
```

`portalsConfig` and `endPortalsConfig` are right, repeated and left portal models, `deadConfig` covers dead-end walls, and each `assetsConfig` entry adds an asset model to every n-th instance of a parent model, starting at an offset. Road tunnels are separate files (`tunnel_a_car.tunnel.lua`, `tunnel_b_car.tunnel.lua`).

## Signals

The [signals wiki page](https://wiki.transportfever3.com/doku.php?id=modding:infrastructure:signals) only covers the animation events. In the base game a signal is a construction placed on a track. `base/content/infrastructure/signal/signal/signal_path_a.con.lua` (shortened):

```lua
		description = {
			name = _("INFRA_SIGNAL_PATH_A_NAME"),
			description = _("INFRA_SIGNAL_PATH_A_DESCRIPTION"),
			icon = "signal_path_a.tga",
			previewIcon = "signal_path_a_preview.tga",
			attributes = { cost = { 9000, -1, }, maintenanceCost = { 1500, -1, }, },
		},
		menuCategory = { categories = { { category = "rail_tools", order = 1000, }, }, },
		updateScript = {
			fileName = "signal_path_a.script@updateFn",
			params = { }
		},
		edgeObject = {
			snapToTrack = true,
		},
		params = {
			{ key = "oneWay", name = _("One-Way"), values = { _("Yes"), _("No") }, defaultIndex = 2, },
		},
```

`edgeObject` is [`ConstructionDesc.EdgeObject`](../api/api/type.md) (symbol: `ConstructionDesc.EdgeObject`, see [symbol index](../index.tsv)), which also has `snapToStreet`, `minDistToCrossing` and catchment fields. The result fields an edge object may return are listed in [Edge objects](constructions.md#edge-objects). The update function in `signal_path_a.script.lua` returns a `signal` table and the model on the edge:

```lua
	updateFn = function(captureParams, params)
		local result = {}
		result.signal = {
			soundevent = "",
			type = "PATH_SIGNAL",
		}
		result.edgeModels = {
			{ model = { id = resolve("signal_path_a.mdl"), transf = { 1,0,0,0, 0,1,0,0, 0,0,1,0, 0,0,0,1, }, } }
		}
		result.cost = 9000
		result.maintenanceCost = 1500
		return result
	end
```

The signal types are [`SignalType`](../api/api/type.md) (symbol: `SignalType`, see [symbol index](../index.tsv)): `PATH_SIGNAL`, `ONE_WAY_PATH_SIGNAL` and `WAYPOINT`. The base signal script returns `PATH_SIGNAL` whatever the `oneWay` parameter says. The airport signs show how to switch: `base/content/assets/stations/stations/air/signs/sign.script.lua` returns `captureParams.oneWay and "ONE_WAY_PATH_SIGNAL" or "PATH_SIGNAL"`, with `oneWay` set in the `.con.lua`'s `updateScript.params`. How the player's `oneWay` choice reaches the engine for the rail signals is not visible in the files.

The model switches state through animation events on its mesh nodes. The wiki lists `forever`, `idle_red`, `green`, `idle_green` and `red`: the signal loops `idle_red`, plays `green` once when it clears, loops `idle_green`, and plays `red` once when it closes again. `signal_path_a.mdl` only uses `green` and `red`:

```lua
							animations = {
								green = { params = { id = "ani/green/signal_path_temperate_a_light_red.ani", }, type = "FILE_REF", },
								red = { params = { id = "ani/red/signal_path_temperate_a_light_red.ani", }, type = "FILE_REF", },
							},
							mesh = "msh/signal_path_temperate_a_light_red_lod0.msh",
```

## Railroad crossings

A railroad crossing type picks left and right models by street width. `base/content/infrastructure/railroad_crossing/railroad_crossing/era_b_eu/era_b_eu.rcr.lua`:

```lua
function data()
return {
	description = {
		name = _("INFRA_RCR_ERA_B_NAME"),
		icon = "era_b_eu.tga",
		description = _("INFRA_RCR_ERA_B_NAME")
	},
	soundFileName = "/infrastructure/railroad_crossing/shared/railroad_crossing.wav",
	config = {
		{ modelLeft = "/infrastructure/railroad_crossing/shared/eu_a_simple.mdl", modelRight = "eu_b_barrier_small.mdl", streetWidth = 12.0 },
		{ modelLeft = "/infrastructure/railroad_crossing/shared/eu_a_simple.mdl", modelRight = "eu_b_barrier_small.mdl", streetWidth = 18.0 },
		{ modelLeft = "/infrastructure/railroad_crossing/shared/eu_a_simple.mdl", modelRight = "eu_b_barrier_medium.mdl", streetWidth = 22.0 },
		{ modelLeft = "/infrastructure/railroad_crossing/shared/eu_a_simple.mdl", modelRight = "eu_b_barrier_large.mdl", streetWidth = 28.0 },
	},
	speedLimit = 160.0 / 3.6,
	availability = { yearFrom = 1925, yearTo = 1975, },
	menuCategory = { categories = { { order = 40, }, }, },
	cost = 16000,
	trafficDelay = 2500,
	categoryList = { categories = { "temperate.clima", }, },
}
end
```

The game uses the `config` entry whose `streetWidth` is closest to the real street; `modelLeft` and `modelRight` stand on the left and right of the street as seen when driving toward the crossing. Per the wiki, `trafficDelay` is the time in milliseconds from the start of the open animation until road traffic moves again (2500 if unset, and all ten base crossings set 2500). `soundFileName` plays while the crossing closes or is closed, `crossingMaterial` is the surface between the rails (default `infrastructure/railroad_crossing/shared/mat/railroad_crossing.mtl`), and a `categoryList` entry that equals a climate file name limits the type to that climate. The base files also set [`speedLimit`](../api/api/type.md) (symbol: `RailroadCrossingType.speedLimit`, see [symbol index](../index.tsv)) (120 or 160 km/h), which the wiki doesn't mention.

The model events are `forever`, `idle_open`, `close`, `idle_closed` and `open`. The barrier model `eu_b_barrier_small.mdl` in the same folder uses `close` and `open` on its barrier and wheel nodes:

```lua
							animations = {
								close = { params = { id = "ani/close/crossing_eu_b_barrier_small.ani", }, type = "FILE_REF", },
								open = { params = { id = "ani/open/crossing_eu_b_barrier_small.ani", }, type = "FILE_REF", },
							},
```

Some US crossing models also use `idle_closed`, for example `era_b_us/us_b_barrier_small.mdl`. At runtime the state is [`Engine.Component.RailroadCrossing`](../api/api/engine.md) (symbol: `Engine.Component.RailroadCrossing`, see [symbol index](../index.tsv)).

## Traffic lights

A traffic light type has no fixed models. Its `modelScript` is called for every street at an intersection and returns the models to place. `base/content/infrastructure/traffic_light/traffic_light/traffic_light_a.trl.lua`:

```lua
local trafficlightutil = require "/infrastructure/traffic_light/trafficlightutil.lua"
-- in a mod: require "::/infrastructure/traffic_light/trafficlightutil.lua"

function data()
	local input = {
		models = {
			trafficLightPole = "traffic_light_a/pole.mdl",
			pedestrianPole = "traffic_light_a/pedestrian_pole.mdl",
			beam = { "traffic_light_a/beam_1.mdl", "traffic_light_a/beam_r.mdl", },          -- last model is repeated
			trafficLight = { "traffic_light_a/traffic_light_1.mdl", "traffic_light_a/traffic_light_r.mdl", },
			pedestrianLight = "traffic_light_a/pedestrian_light.mdl",
		},
		params = {
			offset = -0.25,
			beamWidth = { 6 + .25, 3 },
			lightOffset = { 4.5, 1.5 },
			poleTrafficLight = true
		}
	}

	return {
		availability = { yearFrom = 0, },
		description = { name = _("Type A"), icon = "traffic_light_a_eu.tga" },
		tryOpposite = false,
		modelScript = {
			fileName = "traffic_light.script@traffic_light_a.modelFn",
			params = input
		},
		categoryList = { categories = { "temperate.clima", "subarctic.clima" }, },
	}
end
```

`traffic_light.script.lua` sets `traffic_light_a.modelFn` and `traffic_light_c.modelFn` to `trafficlightutil.standardLights()`. That helper reads the `models` and `params` above: one pole on the right, beams from right to left with the last one repeated, a light per lane at `lightOffset`, and pedestrian lights on their own poles where no traffic light pole stands. `poleTrafficLight = true` mounts the rightmost lane's light on the pole instead of the beam. `tryOpposite = true` puts the lights on the far side of the intersection, as in the US, when there is a street there.

The wiki places this helper at `res/scripts/trafficlightutil.lua`. In the base game it is `base/content/infrastructure/traffic_light/traffic_light/trafficlightutil.lua`, loaded as shown above.

A custom `modelScript` gets `captureParams` (the `params` from the reference) and `args` for one street: `needsLight` and `pedestrian` (per lane, right to left, sidewalks included), `widths`, `position` (right side of the street, next to the outermost lane) and `oppositePosition`. It returns one list per lane that needs a light, each a list of `{ id = "....mdl", transf = { ... } }` with global transforms. The wiki has a full example result.

Traffic light and pedestrian light models use the events `idle_red`, `green`, `idle_green` and `red`, in the same cycle as signals. The year from which towns get traffic lights is not in the `.trl.lua` files: `traffic_light_config.res.lua` sets it as a game config, `data = { trafficLightYearFrom = 1950 }`, read into [`TrafficLightConfig`](../base/infrastructure/traffic_light/traffic_light.md) (symbol: `TrafficLightConfig`, see [symbol index](../index.tsv)).

Official wiki: [Tracks and streets](https://wiki.transportfever3.com/doku.php?id=modding:infrastructure:tracksstreets), [Edge addons](https://wiki.transportfever3.com/doku.php?id=modding:infrastructure:edgeaddons), [Bridges and tunnels](https://wiki.transportfever3.com/doku.php?id=modding:infrastructure:bridgestunnels), [Signals](https://wiki.transportfever3.com/doku.php?id=modding:infrastructure:signals), [Railroad crossings](https://wiki.transportfever3.com/doku.php?id=modding:infrastructure:railroadcrossings), [Traffic lights](https://wiki.transportfever3.com/doku.php?id=modding:infrastructure:trafficlights). The old [Infrastructure](https://wiki.transportfever3.com/doku.php?id=modding:infrastructure) page only redirects to Tracks and streets.
