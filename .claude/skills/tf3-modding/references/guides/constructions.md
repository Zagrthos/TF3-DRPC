
# Constructions

Stations, depots, industries, landmarks, warehouses, interchanges, signals, street stops, decorative assets and town buildings are constructions. Each one is a `.con.lua` file that describes the construction for the build menu and points to a script function that generates its geometry. The base game has 586 of them, grouped in folders such as `depots/`, `industries/` and `stations/`.

This page covers the `.con.lua` file, the update function and its result, the construction types, the build menu and ground textures. Slots, modules and templates are on [Modular constructions](constructions-modular.md). The official wiki describes the same format on [Construction Basics](https://wiki.transportfever3.com/doku.php?id=modding:constructions:basics).

## The two halves of a construction

A `.con.lua` file returns a description table from `data()`. The fields correspond to [`ConstructionDesc`](../api/api/type.md) (symbol: `ConstructionDesc`, see [symbol index](../index.tsv)) in `api/tealdef/api/type.d.tl`. The geometry is not in this file: `updateScript` names a function in a `.script.lua` or `.script.tl` file, and the game calls that function to get the actual models, lanes, terminals and costs.

The helipad shows the split. `base/content/stations/air/air/helipad.con.lua` (description shortened):

```lua
function data()
	return {
		configureHudIconsScript = {
				fileName = "::/gui/construction/construction_desc_hud_icons.script@configureStationHudIconsFn",
				params = {
					cargo = true,
				}
		},
		namePrefix = _("{townName} Heliport"),
		description = {
				name = _("STATIONS_AIR_HELIPAD_NAME"),
				description = _("STATIONS_AIR_HELIPAD_DESCRIPTION"),
				icon = "helipad/helipad.tga",
				previewIcon = "helipad_preview.tga",
				attributes = {
					noise =  { 5, -1 },
					pollution = { 40, -1 },
				},
				cargoTypeSet = {
					cargoClassesIncluded = { "PASSENGERS", "UNIVERSAL", },
				},
		},
		availability = {
				yearFrom = 1950,
				notificationGroup = "helicopterBuildings",
		},
		order = 7000,
		params = {
		},
		updateScript = {
				fileName = "helipad/helipad.script@updateFn",
				params = {
				}
		},
		menuCategory = {
				categories = {
					{ category = "air_buildings", order = 20000, },
				},
		},
	}
end
```

## Fields of the description

| Field | Purpose |
|---|---|
| `description` | Menu entry: `name`, `description`, `icon`, `previewIcon`, the `attributes` shown in the info box and the accepted cargo ([`ModelMetadata.Description`](../api/api/type.md) (symbol: `ModelMetadata.Description`, see [symbol index](../index.tsv))) |
| `availability` | `yearFrom`, `yearTo` and notification settings ([`ModelMetadata.Availability`](../api/api/type.md) (symbol: `ModelMetadata.Availability`, see [symbol index](../index.tsv))) |
| `menuCategory` | Tabs of the build menu the construction appears in, see [build menu](#the-build-menu) |
| `order` | Sort order in the menu |
| `soundConfig` | `soundSet.name` (a [sound set](other-content.md#sound-sets) file), `effects` (event name to sound files, e.g. `select`) and `builderAudioRes`, the [builder audio set](other-content.md#builder-audio-sets) played while building. The wiki says paths are relative to the `.con.lua` |
| `params` | Parameters the player can set, see [parameters](#parameters) |
| `updateScript` | The function that builds the result, with fixed `params` passed to it. The only mandatory script |
| other `...Script` fields | See [construction scripts](#construction-scripts) |
| `namePrefix`, `subConstructionNamePrefix` | Name templates for the window of the built construction, e.g. `_("{townName} Train Depot")` and `"{constructionName}"` |
| `edgeObject` | Turns the construction into an edge object such as a signal or street stop, see [edge objects](#edge-objects) |
| `placementTag`, `isIndustry` | Industries: tag used by the economy, and whether the economy places the construction ([industries](#industries)) |
| `townBuildingParams` | Town buildings: land use, parcel size and level ([town buildings](#town-buildings)) |
| `metadata` | Free table; landmarks put their unlock rules here ([landmarks](#landmarks)) |
| `snapping` | Assets: `rail`, `road`, `water` flags for snapping along tracks, streets or the water surface |
| `heightModView` | `false` hides the grid that previews terrain changes; the wiki says this is usually off for very large constructions |
| `undergroundView`, `heightAdjustable` | Enable the Underground Mode and the height adjustment while building |

The wiki says `availability.yearFrom` below 1900 or unset means "from the start", and `yearTo` unset or `0` means no end; a `yearTo` below 1900 makes the construction never available. It also gives limits for the menu entry: `name` up to 32 characters, `description` up to 320 characters, `icon` a 240×150 `@2x.tga`, `previewIcon` a 720×405 `.tga` for the info box. In `attributes`, each of `noise`, `pollution`, `maintenanceCost` and `cost` is a `{ from, to }` interval; with `-1` as second value only one number is shown.

The wiki's basics page lists two more fields that are not in `ConstructionDesc` and that no shipped file uses: `keepBuildingsAndFields` (do not bulldoze town buildings, fields and other satellites) and, for assets, `buildMode`. `preProcessScript`, which 60 base `.con.lua` files use, is neither in `ConstructionDesc` nor on the wiki; see [Modular constructions](constructions-modular.md#the-preprocess-function).

## Parameters

`params` is a list of [`ConstructionScriptParam`](../api/api/type.md) (symbol: `ConstructionScriptParam`, see [symbol index](../index.tsv)). Each entry needs a `key` and a list of `values`; the selected option reaches the update function as `params.<key>`.

```lua
params = {
	{
		key = "hangar",
		name = _("Depot"),
		group = "hangar",
		values = { _("Yes"), _("No") },
		defaultIndex = 1,
		displayMode = "Horizontal",
	},
},
```

(from `base/content/stations/air/air/airfield.tl`). Its `createTemplateFn` then tests `params.hangar == 1` for "Yes".

The values are 1-based: the first option returns `1`, and a check box returns `1` when unchecked and `2` when checked. The wiki points out that Transport Fever 2 used 0-based values, so old scripts need adjusting. `numbers` replaces the default return values `1, 2, 3, ...`.

| Field | Meaning |
|---|---|
| `key` | Unique identifier. The wiki recommends keys that are unique across all mods, because a value the player picked for another construction with the same key can be preselected for yours |
| `name`, `tooltip`, `tooltips` | Label, tooltip of the parameter, tooltips per option |
| `values` | Labels of the options; for `IconButton` the paths of `.tga` images relative to the `.con.lua` |
| `numbers` | Return values per option instead of `1, 2, 3, ...` |
| `defaultIndex` | Preselected option, 1-based (default 1). In `mod.json` it is 0-based instead, see [Mod parameters](mod-structure.md#mod-parameters) |
| `uiType` | `"Button"` (default), `"Slider"`, `"ComboBox"`, `"IconButton"` or `"CheckBox"`. A check box still needs two `values`, which are not shown |
| `group` | Parameters of one group are separated from others by a line |
| `hideLabel` | Hide the name |
| `location` | `"Default"` or `"Toolbar"` (left or right part of the build menu) |
| `displayMode` | The wiki names `"Default"` and `"Compact"`; the definition also has `"Horizontal"`, `"Vertical"` and `"Sparse"`, and the base game uses `"Horizontal"` (31 times) and `"Sparse"` |
| `yearFrom`, `yearTo` | Years in which the parameter is shown |
| `postConstructionModifiable` | The parameter can be changed after the construction is built |
| `checkEnabledScript` | Function that shows, greys out or hides the parameter |
| `stepValueScript`, `formatValueScript` | Functions for custom stepping and value display (only in the definition, not on the wiki) |

`checkEnabledScript` names a function in a script file. The function returns `"Enabled"`, `"Disabled"` (shown greyed out) or `"Hidden"`; the definition knows a fourth value, `"InputActionOnly"`. It must not call other scripting interfaces. The wiki says the function receives three arguments, but the game calls it with two: in `base/content/gui/gui/construction/construction_react_util.tl` the call is `fn(v.checkEnabledScript.params, params2)`, so the first argument is the `params` table written next to `fileName` and the second holds the current values of all parameters. The wiki's own example uses it the same way. From `base/content/scripts/scripts/construction/assetbuilderutil.script.lua`:

```lua
checkEnabledRotationXYFn = function(captureParams, params)
	return params["randomRotation"] ~= 1 and "Enabled" or "Hidden"
end,
```

The wiki explains the rotation hotkeys: the player turns a construction with two pairs of hotkeys (by default O/P and Ü/+ or [/]), each step is 2π/32. To use them for rotation around X and Y, add `param_util.makeRotationParam("constructOpt56", _("Rotation X"))` and `makeRotationParam("constructOpt78", _("Rotation Y"))` ([`ParamUtil.makeRotationParam`](../base/scripts/construction/param_util.md) (symbol: `ParamUtil.makeRotationParam`, see [symbol index](../index.tsv)), `require "::/scripts/construction/param_util.tl"`) and wrap the model transforms with `constructionutil.rotateTransf(params, transf)` from `::/scripts/construction/constructionutil.lua`.

## Construction scripts

Besides `updateScript`, a construction can name these functions. Each is a script reference `{ fileName = "file.script@function", params = { ... } }`, and the `params` table reaches the function as its first argument (`captureParams`).

| Field | Called | In base `.con.lua` files |
|---|---|---|
| `updateScript` | to build the construction, see [the update function](#the-update-function) | all |
| `createTemplateScript` | to place the starting modules, see [templates](constructions-modular.md#templates) | 68 |
| `preProcessScript` | when a module is added or removed (not on the wiki) | 60 |
| `isAffectedByToolScript` | while a street or track tool is active: may the tool change this construction? | 5 |
| `upgradeScript` | when such a tool changes the construction | 5 |
| `hudIconScript` | to get the HUD icon of the construction | 38 |
| `configureHudIconsScript` | which HUD icons to show while placing | 51 |
| `configureLayerScript` | which map layer to show while the player places the construction | 44 |
| `entityWindowScript` | to build the window shown when the construction is selected | 38 |

If a UI script is missing, the game falls back to its default, which can be nothing.

`isAffectedByToolFn(captureParams, params, conConfigAdd, conConfigRevert)` returns `true` if the tool may be applied; without the function `true` is assumed. `params` holds the values of the last update, `conConfigAdd` what the tool wants to do and `conConfigRevert` what it would do if it had already been applied. The wiki lists the keys: `catenary` (electrification and track tool, 1 = off, 2 = on), `streetTemplate` (track and street tools, resource name), `busLane`, `tramTrack` and `tramCatenary` (1 = off, 2 = on). `upgradeFn(captureParams, params, slotId, constructionConfig)` gets the same kind of data in `constructionConfig`, returns the changed `params`, and these go into the update function. `slotId` is `-1` for non-modular constructions. Both functions may use `api.res`, but no other part of `api`. The rail station in `base/content/stations/rail/rail/modular_station/modular_station.script.lua` has both:

```lua
isAffectedByToolFn = function(captureParams, params, conConfigAdd, conConfigRemove)
	if conConfigAdd.streetTemplate then
		local streetTemplate = api.res.streetTemplateRep.get(api.res.streetTemplateRep.find(conConfigAdd.streetTemplate))
		return streetTemplate.roadType == 0 -- 0: "TRACK"
	end
	return true
end,
```

The UI scripts in short, after the wiki:

- `hudIconFn(captureParams, params, userParam)` returns a layout; helpers are in `::/gui/main/hud_icon_toolbox.tl`. The landmarks use `::/landmarks/landmark_hud.script@hudIconFn`.
- `configureHudIconsScript` returns a `ConstructionActionHudIcons` table: `componentTypes` (e.g. `api.type.ComponentType.TOWN`, `STATION_GROUP`, `INDUSTRY`) and, with `TOWN`, the flags `showDistricts`, `showTownBorders` and `showPerksAndTowns`. The base defaults are in `::/gui/construction/construction_desc_hud_icons.script.tl`.
- `configureLayerScript` gets `(captureParams, layerConfig, params)`, where `params` holds the current parameter values, and returns a layer configuration. The build menu calls it while the player places the construction (`base/content/gui/gui/construction/construction_react_util.tl`); the base game uses `::/gui/construction/construction_desc_layers.script@configureDefaultLayerFn`. The wiki also says the layer applies while the entity window of the construction is open, but nothing in the base entity window code reads `configureLayerScript`.
- `entityWindowScript` returns a window description, usually with `recipe = make_entity_window.ConstructionWindow` from `::/gui/entity_window/make_entity_window.tl`, its `param`, and a `fallbackTitle`. See [GUI](gui.md) for how windows are built.

## The update function

The function behind `updateScript` receives two arguments. The first, `captureParams`, is the `params` table written into `updateScript` in the `.con` file. The second, `params`, holds the player's parameter choices, the modules of a modular construction, and extra values that depend on the [construction type](#construction-types). The small bus stops show the first part clearly. `base/content/stations/street/street/small_stops/small_new.con.lua` passes the model and the catchment radius:

```lua
updateScript = {
	fileName = "small_stops.script@updateFn",
	params = {
		twoSided = false,
		modelId = resolve("small_new.mdl"),
		maintenanceCost = 5000,
		catchmentAreaRadius = catchmentAreaRadius,
	}
},
```

and `small_stops.script.lua` in the same folder reads them back:

```lua
function data()

return {
	updateFn = function(captureParams, params)
		local result = {}
		result.streetTerminal = {
			capacity = 50,
			-- ... cargo settings ...
			catchmentArea = { 
				position = { 0, 0, 0, },
				radius = captureParams.catchmentAreaRadius,
			},
			passengers = true,
			wantSymmetric = captureParams.twoSided,
		}
		result.edgeModels = {
			{
				model = {
					id = captureParams.modelId,
					transf = { -1,0,0,0, 0,-1,0,0, 0,0,1,0, 0,0,0,1, },
				}
			}
		}
		result.cost = captureParams.buildCost or captureParams.maintenanceCost * 6
		result.maintenanceCost = captureParams.maintenanceCost

		return result
	end
}

end
```

Six bus stop variants share this script and differ only in the `params` of their `.con` files. The stop is an edge object, so its result has `edgeModels` instead of subconstructions; see [edge objects](#edge-objects).

### The result table

The returned table is described in Teal as [`ConstructionUtilSerialized.ConstructionResult`](../base/scripts/construction/construction_util_serialized.md) (symbol: `ConstructionUtilSerialized.ConstructionResult`, see [symbol index](../index.tsv)) in `base/tealdef/scripts/construction/construction_util_serialized.d.tl`. The file comment says what these types are:

```teal
-- Plain lua table type definitions for construction resource scripts.
-- These types describe the format of lua tables produced by construction scripts
-- and read by C++ via converters. They must NOT reference C++ userdata types.
```

The top level ([`IConstructionResult`](../base/scripts/construction/construction_util_serialized.md) (symbol: `ConstructionUtilSerialized.IConstructionResult`, see [symbol index](../index.tsv))) and the fields the wiki adds:

| Field | Content |
|---|---|
| `subconstructions` | list of [`SubConstruction`](../base/scripts/construction/construction_util_serialized.md) (symbol: `ConstructionUtilSerialized.SubConstruction`, see [symbol index](../index.tsv)), see below |
| `edgeLists` | streets and tracks, see [edge lists](#edge-lists) |
| `edgeObjects` | models such as signals attached to those edges |
| `snapPoint` | how the construction snaps while placing, see [snap point](#snap-point) |
| `slots`, `slotConfig` | module slots, see [Modular constructions](constructions-modular.md#slots) |
| `cost`, `bulldozeCost`, `maintenanceCost` | paid when built, when demolished, and as running cost. The wiki calls the running cost monthly here and yearly for the menu `attributes` |
| `costMultiplier` | factor on the costs, e.g. by year (wiki; one shipped file uses it) |
| `noCostAtAll` | disables all costs of the proposal, terrain alignment included |
| `terminalGroups`, `stations` | group model terminals into stations, see [stations](#stations) |
| `streetTerminal` | terminal of a street-side stop (edge objects only) |
| `metadata` | free table; landmarks keep their building stages here |

The bus stop's `edgeModels` and the signals' `signal` are not in these Teal types; the wiki documents them for edge objects.

### Subconstructions

Nearly everything that appears in the world or has a game function goes into a subconstruction. The wiki's list of fields:

| Field | Content |
|---|---|
| `tag` | integer that maps the subconstruction to the same one after an update |
| `models` | `{ id = "x.mdl", transf = ..., tag = "..." }` entries; `id` is relative to the `.con.lua`, `transf` a 16-number matrix or a value from `mat4.tl` |
| `groundFaces` | painted terrain, see [ground faces](#ground-faces) |
| `colliders` | areas where nothing else can be built |
| `terrainAlignmentLists` | terrain shaping |
| `laneLists` | paths for people, cargo and vehicles |
| `runways` | landing and take-off paths for aircraft and ships |
| `labelText` | texts for `"CUSTOM"` labels of models: `[modelIndex] = { "first label", "second label" }` |
| `metadata`, `emissionEmitter` | free data, emissions |
| `station`, `depot`, `industry`, `field`, `warehouse`, `maintenanceStation`, `townBuilding` | the game function of the subconstruction, see [construction types](#construction-types) |
| `stocks`, `rules`, `personCapacity`, `scaffold` | cargo stocks and production rules, workplaces or inhabitants, scaffolding of town buildings |

In practice one subconstruction holds most of this and others hold only models. Some game functions cannot share a subconstruction, so a construction with a station and a depot needs one subconstruction for each. When the player clicks the construction, its window has one tab per functional subconstruction.

Colliders have a `type` of `"BOX"`, `"CYLINDER"` (both with `params.halfExtents`, three half sizes) or `"POINT_CLOUD"` (with `params.points`, in which case `transf` is ignored), plus a `transf` for the centre. [`collider_util_serialized`](../base/scripts/construction/collider_util_serialized.md) has the format and `colliderutil.createBox(...)` builds one.

Terrain alignment lists have a `type` (`"EQUAL"` sets the terrain to the faces, `"LESS"` only cuts higher ground, `"GREATER"` only fills lower ground), `faces` (polygons) or `triangles` (a flat point list, a multiple of three), the embankment slopes `slopeLow` and `slopeHigh`, and `optional = true` to avoid collision errors with other alignments of the same construction. If the alignment is impossible because of something nearby, the construction cannot be built.

Lane lists have `transf`, `nodes` (every two nodes form one edge, each node is `{ position, tangent, width }`; the wiki notes that the tangent length must equal the edge length or vehicles look squeezed or stretched, and the width is only used for capacity), `speedLimit` in m/s and `transportModes`. Vehicle lanes also take `needsReservation` and `forwardOnly`, pedestrian lanes `linkable` (reachable by the automatic short footpath links) and `pedestrianWalkPenalty`. `laneutil.createLane(curve, modes, speed, width, linkable)` builds one (see [`lane_util_serialized`](../base/scripts/construction/lane_util_serialized.md)).

Runways are `{ type = "LANDING" | "TAKEOFF", node = { laneList, node }, edges = { { laneList, lane }, ... } }`. The `node` is where the runway joins the other lanes; there an aircraft has slowed down after landing or starts to speed up for take-off. The direction comes from the tangent of the first or last edge. A first index of `-1` refers to the construction's `edgeLists` instead of a lane list. The same index pairs are used by terminals, pools and depots.

### Ground faces

```lua
groundFaces = {
	{
		face = { { -10, -10, 0 }, { 10, -10, 0 }, { 10, 10, 0 } },
		modes = {
			{ type = "FILL", key = "industry_floor.gtex" },
		},
		loop = true,
	},
},
```

`face` is a counter-clockwise polygon of at least three points (z is ignored). Each mode has a `type` (`"FILL"`, `"STROKE"`, `"STROKE_INNER"` or `"STROKE_OUTER"`), the [ground texture](#ground-textures) `key` relative to the `.con.lua`, and optional `texCoords` (one 0 to 1 coordinate per face point). `loop = false` leaves the edge from the last to the first point without a stroke. `alignmentOffsetMode`/`alignmentDirMode` (`"OBJECT"` or `"WORLD"`) with `alignmentOffset`/`alignmentDir` move and rotate the texture. Base scripts usually call `modulesutil.addGroundFaces("file.gtex", generatedData, subconstruction)` instead.

### Edge lists

Edge lists add streets and tracks to the transport network. Shortened from the wiki:

```lua
result.edgeLists = {
	{
		type = "STREET",
		params = {
			type = "::/infrastructure/street/country/country_new_small.street_template",
			tramTrackType = "YES",
		},
		edges = {
			{ { .0, -79.0, .0 }, { .0, 15.0, .0 } },  -- node 0
			{ { .0, -64.0, .0 }, { .0, 15.0, .0 } },  -- node 1
		},
		snapNodes = { 0 },
		freeNodes = {},
	},
}
```

`type` is `"STREET"` or `"TRACK"`; `params.type` names the street template (for tracks too) and `tramTrackType` is `"NO"`, `"YES"` or `"ELECTRIC"`. `edgeType` `"BRIDGE"` or `"TUNNEL"` with `edgeTypeName` makes a bridge or tunnel; the wiki recommends absolute paths for both. Every two `edges` entries are one edge, each `{ position, tangent }` with an optional third string tag that merges nodes at different positions. `snapNodes` are the node indices that later street or track building can connect to, `freeNodes` nodes that do not belong to the construction, and `alignTerrain = true` shapes the terrain as normal street building does. The node groups for tags are called `tag2Nodes` on the wiki and `tag2nodes` in the definition and in `base/content/stations/rail/rail/trainstationutil.lua`. For traffic lights the wiki describes `trafficLightNodePreference` (node index and `"YES"`, `"NO"` or `"AUTO"`); the definition has `trafficLightYes` and `trafficLightNo` instead, and no shipped construction uses either. [`street_util_serialized`](../base/scripts/construction/street_util_serialized.md) has helpers such as `addStraightEdge`.

`result.edgeObjects` attaches models to these edges: `{ edge = 1, param = .5, left = false, model = "::/stations/airport/asset/signal_runway_old.mdl" }`. Edge `n` is the one between nodes `2n` and `2n+1`, `param` is the distance from the start node in metres, and only one object per edge is possible from a script.

Neither the wiki nor the definitions say how a construction's own vehicle lanes join its streets. The road depot shows what the base game does (`base/content/depots/road/road/road_depot/road_depot.script.lua`). Its street stub comes from the generated curve `lane1` and runs from `(0, -35, 0)`, the snap node, to `(0, -20.26, 0)` at the depot front:

```lua
local snapNodes = { 0 }
constructionutil.addEdges(generatedData["base"].curves["lane1"], result, "STREET", { type = streetType, }, false, snapNodes, freeNodesList)
-- ...
local lane = laneutil.createLane(generatedData["base"].curves["in"], transportModes , 30, 30, true)
```

The street template, `::/infrastructure/street/constructions/entrance_new.street_template`, has lanes 3, 5, 5 and 3 m wide, so the two vehicle lanes run 2.5 m left and right of the street centre. The depot's `BUS`/`TRUCK` lanes end and start at `(-2.5, -20.26, 0)` and `(2.5, -20.26, 0)`, which is where those street lanes end. The likely rule is that a construction lane connects to a street lane when their end points are at the same position. No file says so, and whether the game tolerates a small gap is an open question.

### Snap point

```lua
result.snapPoint = {
	transf = transf.rotZTransl(0, vec3.new(0, -25, 0)),
	snapToBaseEdgeTypes = {"STREET"},
}
```

`transf` is the point of the construction under the mouse cursor. `transportModes` and `snapToBaseEdgeTypes` (`"TRACK"`, `"STREET"`) choose what it snaps to, `allowSnapToBaseEdgeEnds` allows snapping at crossings and ends, `alignWithCoast` snaps along the coast, `placeAtTerrainHeight` keeps it at terrain height when snapping. For harbours there are `forceSnapToWaterSurfaceHeight`, `snapHeightOffsetAboveWater` and `onlySnapToWaterSurfaceHeightWhenInWater`. `allowSnapToMesh` lets the player snap to model geometry by pressing ALT.

### Annotated example: the helipad

`base/content/stations/air/air/helipad/helipad.script.lua` builds a complete small station. It starts with a block of generated data, then the update function (shortened, comments added):

```lua
local transf = require "/scripts/mat4.tl"  -- in a mod: require "::/scripts/mat4.tl"
local constructionutil = require "::/scripts/construction/constructionutil.lua"
local laneutil = require "::/scripts/construction/laneutil.lua"
local modulesutil = require "/scripts/construction/modulesutil.lua"  -- in a mod: require "::/scripts/construction/modulesutil.lua"

--Begin Generated
local generatedData = {
	["base"] = {
		models = {
			["::/assets/buildings/ind/aeration/aeration_06.mdl"] = {
				{ 0.0, -1.0, 0.0, 0.0,  1.0, 0.0, 0.0, 0.0,  -0.0, -0.0, 1.0, 0.0,  10.0, 5.18, 0.0, 1.0, },
			},
			-- ... one list of 4x4 transforms per model ...
--End Generated

function data()
    return {
        updateFn = function(captureParams, params)
            local result = {}

            -- one subconstruction holds everything; tag identifies it
            local stationSubconstruction = {
                runways = {},
                laneLists = {},
                models = {},
                groundFaces = {},
                tag = 1000,
            }

            -- place the generated models
            constructionutil.addModelsAndGroups(params, generatedData["base"], stationSubconstruction)

            -- a station with a person/cargo pool
            local station = {
                terminals = {},
                tag = 1,
                pool = { edges = {}, capacity = 100 },
                catchmentAreas = {{ radius = 100 }},
            }

            -- lanes: a walkway for PERSON/CARGO, then two HELICOPTER lanes
            local lane = laneutil.createLane(lanes["lanes"].curves["in"], { "PERSON", "CARGO"} , 10, 5, true)
            table.insert(stationSubconstruction.laneLists, lane)
            -- ... two more createLane calls for "HELICOPTER" ...

            -- the terminal refers to lanes by {laneList index, node index}
            local terminal = {
                passengers = true,
                cargo = true,
                personEdges = { { #stationSubconstruction.laneLists-3, 0},  { #stationSubconstruction.laneLists-3, 1},  },
                personNodes = { { #stationSubconstruction.laneLists-3, 2}, },
                vehicleNode = { #stationSubconstruction.laneLists-1, 0},
                transportModes = {"HELICOPTER"},
                capacity = 30,
            }
            table.insert(station.terminals, terminal)

            -- runways for landing and take-off
            stationSubconstruction.runways[#stationSubconstruction.runways + 1] = {
                type = "LANDING",
                node = { #stationSubconstruction.laneLists-2, 0},
                edges = { {#stationSubconstruction.laneLists-2, 0}, },
            }
            -- ... TAKEOFF runway ...
            stationSubconstruction.station = station

            -- ground texture from helipad.gtex
            modulesutil.addGroundFaces( "helipad/helipad.gtex", generatedData, stationSubconstruction)

            result.subconstructions = { stationSubconstruction }

            -- snap the station 25 m in front to a street
            result.snapPoint = {
                transf = transf.rotZTransl(0, vec3.new(0, -25, 0)),
                snapToBaseEdgeTypes = {"STREET"},
            }

            result.cost = 180000
            result.maintenanceCost = 25000
            return result
        end,
    }
end
```

The `--Begin Generated` and `--End Generated` markers appear in many base construction and module scripts. The Model Editor's construction data import fills the block between them from an `.fbx` file, see [Model editor](model-editor.md#construction-data).

The helipad is a base game file, so `/scripts/mat4.tl` means the base game here. In a mod, `/` points into the mod itself, so write `::/` as the comments show ([file references](mod-structure.md#file-references-inside-content)).

## Construction types

A construction has no type field. What it does depends on the subconstructions in its result, and one construction can combine several of them. The wiki's [Construction Types](https://wiki.transportfever3.com/doku.php?id=modding:constructions:types) page describes each. It also notes a shortcut: a simple construction with one subconstruction and no ground faces, terrain alignment, station, depot, industry or town building is not placed as a construction at all, only as its models. The player then cannot click it, configure it later or remove it in one go, and `labelText` does not work for it.

### Stations

A subconstruction with a `station` table is a station ([`StationDef`](../base/scripts/construction/construction_util_serialized.md) (symbol: `ConstructionUtilSerialized.StationDef`, see [symbol index](../index.tsv))):

```lua
subconstruction.station = {
	tag = 1,
	terminals = { ... },
	pool = { edges = { { 1, 1 } }, capacity = 100 },
	catchmentAreas = { { position = { 0, 0, 0 }, radius = 200 } },
}
```

Each terminal ([`TerminalDef`](../base/scripts/construction/construction_util_serialized.md) (symbol: `ConstructionUtilSerialized.TerminalDef`, see [symbol index](../index.tsv))) refers to lanes of the same subconstruction by `{ laneListIndex, index }` pairs, or to the construction's `edgeLists` with a first value of `-1`:

| Field | Meaning |
|---|---|
| `tag` | maps the terminal to the same one after an update |
| `personEdges` | waiting edges for passengers and cargo; their length and width give the capacity. They must allow `"PERSON"` or `"CARGO"` |
| `personNodes` | where alighting passengers and cargo appear |
| `vehicleNode` | where the vehicle stops (front or middle depends on the vehicle type) |
| `vehicleEdges` | alternative to `vehicleNode` that lets several vehicles load at once |
| `passengers`, `cargo` | what the terminal handles |
| `comfortFactor` | how fast waiting passengers get impatient |
| `cargoTransferSpeeds` | list of `{ cargoTypeSet, loadSpeedModifier, length }` for specialised sections, relevant for mixed trains |

The `pool` is an overflow waiting area shared by the station. Inside a catchment area, places count as reachable if passengers have a complete walking path; other cargo is teleported.

Stations built from models that bring their own terminals (the rail station's platforms) group them in `result.terminalGroups`: `terminals` lists `{ modelIndex, terminalIndex }` pairs and `vehicleNodeOverride` replaces the vehicle node with a node of `result.edgeLists`. The wiki notes that for street edges the vehicle node lands on the first lane, which is usually the sidewalk, and that once `terminalGroups` exists, ungrouped terminals are ignored. `result.stations` then assigns groups to stations, for example to split passenger and cargo parts: `{ { terminals = { 0, 1 }, tag = 0 }, { terminals = { 2, 3 }, tag = 1 } }`. The `tag` keeps lines attached when the order changes.

### Depots

```lua
depot = {
	transportModes = { "TRAIN", "ELECTRIC_TRAIN", },
	filterTags = { "default", },
	inNodes = { { 1, 0 }, },
	outNodes = { { 0, 0 }, },
},
```

`transportModes` are the vehicles sold here and `filterTags` limits the shop to vehicles with a matching tag. `inNodes` and `outNodes` are the entry and exit nodes and must each have only one lane attached. How the road depot's lanes reach the street is shown under [edge lists](#edge-lists). Models of the depot can have `open` and `close` animations for the doors. A maintenance building uses the same `depot` table with a `catchmentArea` (`radius` in metres around `position`) and `maintenancePool` (how many vehicles can be maintained at once); vehicles inside the radius are maintained ([`Depot`](../base/scripts/construction/construction_util_serialized.md) (symbol: `ConstructionUtilSerialized.Depot`, see [symbol index](../index.tsv))).

### Industries

A subconstruction with an `industry` table is an industry. In the `.con.lua`, `placementTag` links it to the economy and `isIndustry = true` lets the economy place it to balance supply and demand. Which industries a map gets is decided by the climate's economy file, see [Economies](game-mechanics.md#economies). The cargo flow is in `stocks` and `rules`:

```lua
subconstruction.stocks = {
	{ type = "INPUT_STOCK", cargoType = "::/cargos/grain/grain.cargo", capacity = 400, loadIndicators = { 10, 11 }, tag = 1 },
}
subconstruction.rules = {
	{ input = { { 8, 3 } }, output = { ["::/cargos/beverages/beverages.cargo"] = 8 }, capacity = 45, priority = 1 },
}
```

A stock `type` is `"INPUT_STOCK"`, `"OUTPUT_STOCK"` or `"STORAGE_STOCK"` (warehouses). `loadIndicators` are indices of models in the same subconstruction that show the stored amount, and `tag` keeps the stored items across updates. A rule's `input` lists an amount per input stock; each inner list is one alternative. `capacity` limits how often the rule runs per year, and with several runnable rules the one with the higher `priority` wins, otherwise they alternate.

Boosters: a rule with `booster = true` and a `boostFactor` (e.g. `1.4`) runs on its own and speeds up the normal rules while it runs. A `personCapacity` of type `"INDUSTRIAL"` adds workplaces, which also boost production. The wiki recommends both boosters for raw material industries and only workers for processing industries.

Fields and satellites are modules in slots (see [Modular constructions](constructions-modular.md)). `industry.maxLevel` is the number of fields when fully upgraded; `0` means none, as in most processing industries. The update function then receives `params.sizeFactor`, which scripts use to scale the rules, e.g. `consumptionFactor = params.sizeFactor`.

### Warehouses

A warehouse has `subconstruction.warehouse = { }` and stocks of type `"STORAGE_STOCK"`. These accept a set of cargo types through `cargoTypes` (a cargo type set as on vehicles) instead of one `cargoType`. Normally the first stored item fixes the stock to that cargo; with `mixedTypes = true` it stays open for all accepted types. The wiki adds that unmixed stocks extend the lifetime of stored cargo. `loadSpeedModifier` scales loading speed.

### Assets

Assets are decorative constructions; they can contain edges. The wiki names `type = "ASSET_DEFAULT"` (placed freely) and `"ASSET_TRACK"` (snaps along tracks, SHIFT suppresses it). These match [`ConstructionType`](../api/api/type.md) (symbol: `ConstructionType`, see [symbol index](../index.tsv)), but no shipped `.con.lua` sets such a `type`. Other asset fields: `skipCollision` (models may overlap), `autoRemovable` (removed when something else is built over it), `categories` for filtering, and `snapping`. Asset update functions get `paramX` and `paramY` (the hotkeys), a `seed` that increases with each placement, `state` (cached asset and track data) and `year`, plus the custom parameters of this and earlier asset constructions. `assetbuilderutil.makeAssetCon(...)` from `::/scripts/construction/assetbuilderutil.lua` builds a whole asset collection; seven campaign missions ship such a file as `content/mission0Xconlua.txt` (a `.txt`, so presumably not loaded as a construction).

### Town buildings

The town picks buildings by `townBuildingParams` in the `.con.lua`: `landUseType` (`"RESIDENTIAL"`, `"COMMERCIAL"` or `"INDUSTRIAL"`), `parcelSize` in 8 m steps (`{ 1, 2 }` is 8 m along the street and 16 m deep) and `level` 1 to 4. When the town places one, the update function also receives `capacity`, `cargoTypes`, `width`, `depth` and `parcelFace`. The subconstruction returns `townBuilding = { level, width, depth }`, a `personCapacity` (`type`, `capacity`, the `personNodes` where people enter) and a `scaffold` (`buildingFace` polygon and `height` offset below the roof) for the construction phase. Cargo demand is an `INPUT_STOCK` per entry in `params.cargoTypes` and a rule whose inputs are alternatives (`{ {1,0,0}, {0,1,0}, {0,0,1} }`), so one delivered cargo type is enough.

The base game writes all of this through `base/content/buildings/townbuildingutil.lua`. From `base/content/buildings/a/c1/1x1_01/1x1_01/a_com_l1_1x1_01.con.lua`:

```lua
local townbuildingutil = require "/buildings/townbuildingutil.lua"  -- in a mod: require "::/buildings/townbuildingutil.lua"

function data()
	return townbuildingutil.makeBuildingStatic(
		{ { -4.60, -0.10 }, { 4.60, -0.10 }, { 4.60, 9.10 }, { -4.60, 9.10 } },
		"COMMERCIAL", 
		"A", 
		1,
		{ 1, 1 },
		"a_com_l1_1x1_01.script"
	)
end
```

`makeBuildingStatic(buildingFace, landUseType, era, level, parcelSize, scriptFile)` sets availability by era, the sounds and `townBuildingParams`, and passes `townBuildingParams` and `buildingFace` to the update script. `townbuildingutil.makeBuildingScript(modelData, groundTexture)` returns a complete update function.

### Landmarks

Landmarks are unique buildings the player unlocks. The unlock rule is in the `.con.lua` `metadata`:

```lua
metadata = {
	company = {
		perkCategory = "LANDMARK",
		rankAndPermits = {{2, 1}},
	},
},
```

`perkCategory` is `"LANDMARK"`, `"HQ"` (Headquarters modules), `"PROSPECTION"` or `"MECHANICS"`. Each pair in `rankAndPermits` is a company rank and the number of instances unlocked at that rank. Building stages and area effects go into `result.metadata.constructionConfig` of the update function ([`construction_config_metadata`](../base/scripts/construction/construction_config_metadata.md)):

```lua
result.metadata = {
	constructionConfig = {
		numLevels = 4,
		level = 0,
		underConstructionMessage = _("Supply the construction site with resources."),
		requirements = { { cargoType = "::/cargos/stone/stone.cargo", amount = 800 } },
		effects = { { carrier = "ROAD", type = "noise", value = 0.75 } },
	},
}
```

`numLevels` counts the stages including the finished one. With `level = 0` the landmark game script advances the stage once the `requirements` are delivered. `underConstructionDescription` is shown in the window until completion. An effect has a `carrier` (`"ROAD"`, `"RAIL"`, `"TRAM"`, `"AIR"`, `"WATER"`), a `type` (`"topSpeed"`, `"noise"`, `"pollution"`, `"comfort"`) and a multiplier `value` for vehicles nearby.

The same `company` block unlocks the other perks. Mechanics perks are tools or constructions with `perkCategory = "MECHANICS"`, such as the Cleaning Plant `base/content/game_mechanics/game_mechanics/emission/pollution_cleanup_facility.con.lua` and the tool `industry_greenify_tool` (see [tools in the build menu](#tools-in-the-build-menu)). Prospecting perks and permit keys are generic resources, not constructions. All three are described on [Game mechanics](game-mechanics.md#company-perks).

The Headquarters is itself a modular construction: `base/content/landmarks/hq/hq/headquarter.con.lua` with 14 modules in `landmarks/hq/hq/modules/`, each with `perkCategory = "HQ"` in its `metadata.company`. The DLC module `hq_maze.module.lua` adds a fifteenth from outside the base game; it is the example in [A minimal modular construction](constructions-modular.md#a-minimal-modular-construction).

### Edge objects

Signals and street-side stops are constructions with an `edgeObject` table ([`ConstructionDesc.EdgeObject`](../api/api/type.md) (symbol: `ConstructionDesc.EdgeObject`, see [symbol index](../index.tsv))) in the `.con.lua`: `snapToStreet`, `snapToTrack`, `minDistToCrossing`, and for stops the catchment preview settings `catchmentCargo`, `catchmentPassengers` and `catchmentAreaRadius`. Their update function also receives `params.baseEdgeType` (`NORMAL`, `BRIDGE` or `TUNNEL`) and `params.roadType` (`STREET` or `TRACK`).

The result of an edge object accepts only a few fields: `cost`, `maintenanceCost`, `edgeModels`, `signal`, `streetTerminal` and `edgeObjectEmissionEmitter` (`noise`, `pollution`). This answers why the bus stop above returns `edgeModels`. Each entry of `edgeModels` is `{ edgeOffset = ..., model = { id, transf } }`, where the optional `edgeOffset` moves the model along the edge and follows its curve and slope. `signal` has a `type` (`"PATH_SIGNAL"` for two-way or `"ONE_WAY_PATH_SIGNAL"`), an optional `soundevent` and `upright` (`true` keeps the model vertical, `false` follows the gradient). A complete signal, with its animation events, is on [Infrastructure](infrastructure.md#signals).

`streetTerminal` ([`StreetTerminal`](../base/scripts/construction/construction_util_serialized.md) (symbol: `ConstructionUtilSerialized.StreetTerminal`, see [symbol index](../index.tsv))) has `capacity`, `catchmentArea`, `passengers`, `cargoLoad`, `cargoUnload`, `cargoTransferSpeeds` and `wantSymmetric` (build the same stop on the other side of the street).

## The build menu

`menuCategory.categories` lists the tabs a construction, module or metaconstruction appears in. Each entry has a `category` tag, optional `filterCategories` and an optional `order` ([`ModelMetadata.MenuCategory`](../api/api/type.md) (symbol: `ModelMetadata.MenuCategory`, see [symbol index](../index.tsv))):

```lua
menuCategory = {
	categories = {
		{ category = "rail_buildings", filterCategories = {"building"}, order = 5000 },
	},
},
```

The base game defines the tabs in `base/content/gui/gui/construction/menu/menu_categories/*.res.lua` (each with `category`, `order`, `name`, `icon` and `menu`), the street tabs in `base/content/infrastructure/street/street/shared/category_roads_*.res.lua`, the module tabs in `.../menu/modules_categories/` and the filters in `.../menu/filter_categories/` (75 files). The wiki's [Construction Menu](https://wiki.transportfever3.com/doku.php?id=modding:constructions:menu) page lists the tabs:

| Menu | Tags |
|---|---|
| Road | `road_buildings`, `road_tools`, `road_assets` |
| Rail | `rail_buildings` |
| Tracks | `tracks`, `rail_tools`, `rail_constructions`, `rail_signals`, `rail_assets` |
| Water | `water_buildings`, `water_tools`, `water_assets` |
| Air | `air_buildings`, `air_tools`, `air_assets` |
| Roads | `roads_small`, `roads_medium`, `roads_large`, `roads_country`, `roads_highway`, `street_constructions`, `roads_tools` |
| Warehouses | `warehouses` |
| Perks | `landmarks`, `prospections`, `perks_mechanics` |
| Industry | `industries`, `industries_assets` (both only in sandbox mode) |
| Landscaping | `landscaping_terrain`, `landscaping_ground`, `landscaping_vegetation`, `landscaping_animals`, `landscaping_people`, `landscaping_vehicles`, `landscaping_assets`, `town_buildings_res`, `town_buildings_com`, `town_buildings_ind`, `town_buildings_other` |
| Town | `town_tools` |
| Modules | `modules_building`, `modules_platforms`, `modules_plots`, `modules_decoration`, `modules_street_access`, `modules_tracks`, `modules_landing`, `modules_warehouse`, `modules_misc` |

Several of these come from the game files and are not in the wiki's table: `roads_tools` (menu `ROADS`, 17 uses), `town_tools` (menu `TOWN`) and the nine `modules_*` tabs (menu `MODULES`, used by `.module.lua` files). The wiki puts `rail_assets` in the Rail menu; its `.res.lua` file says `menu = "TRACKS"`. The wiki marks the asset tabs, `rail_constructions`, `rail_signals`, the landscaping animal, people, vehicle and asset tabs and the four town building tabs as "mod only", meaning the base game puts nothing there.

Filter categories narrow a tab. The wiki groups them by topic:

| Topic | Tags |
|---|---|
| Ground | `asphalt`, `ground`, `gravel`, `grass`, `plant`, `rock`, `sand`, `misc` |
| Vegetation | `tree`, `bush`, `plant`, `flower` |
| Animals | `bird`, `land`, `fish` |
| People | `residential`, `worker` |
| Vehicles | `bike`, `car`, `bus`, `truck`, `tram`, `train`, `passenger_wagon`, `cargo_wagon`, `plane`, `helicopter`, `ship` |
| Buildings | `culture`, `sport`, `public`, `emergency`, `tower`, `lvl_1` to `lvl_4` |
| Industries | `farm`, `mine`, `factory`, `water` |
| Infrastructure | `stop`, `station`, `depot`, `tracks`, `bridge`, `tunnel`, `intersection` |
| Tracks | `600mm`, `750mm`, `1000mm`, `1435mm`, `1500mm`, `3rd_rail`, `catenary`, `cogwheel`, `high_speed`, `wood`, `concrete` |
| Bridges, tunnels | `bridge_viaduct`, `bridge_arch`, `bridge_beam`, `bridge_truss`, `bridge_suspension`, `tunnel_round`, `tunnel_rectangular` |
| Misc | `cargo`, `passenger`, `building`, `barrier`, `construction`, `lamp`, `sign`, `spline`, `tools` |

Custom tabs and filters are possible, but the wiki asks modders to use the predefined ones and to contact Urban Games before adding new ones.

### Tools in the build menu

Not every build menu entry is a construction. Brushes, terrain tools, the Bus Lane Tool and Tram Track Tool, the Town Builder and the Marketing Campaign are `construction_tool` resources: 22 `.res.lua` files in `base/content/gui/gui/construction/tools/`. Each one points at a function that returns the menu entry. `asset_eraser_tool.res.lua`:

```lua
function data()
	return {
		type = "construction_tool",
		data = {
			getDefinitionFn = {
				fileName = resolve("asset_eraser_tool.script@getDefinition"),
			}
		}
	}
end
```

`data` is a [`ConstructionToolData`](../base/gui/construction/construction_menu.md) (symbol: `ConstructionToolData`, see [symbol index](../index.tsv)): `getDefinitionFn` is required, and an optional `isDisabledFn` that returns `true` leaves the entry out (no base tool sets it). The build menu collects all resources of this type with `api.res.genericRep.getAllOfType("construction_tool")` and calls `getDefinitionFn(resName, repository, params)` (`base/content/gui/gui/construction/construction_react_util.tl`). The function returns a [`ConstructionDefinition`](../base/gui/construction/construction_menu.md) (symbol: `ConstructionDefinition`, see [symbol index](../index.tsv)). From `asset_eraser_tool.script.tl` (shortened):

```teal
getDefinition = function(constructionDefinition : ResName, _repository : ConstructionActionRepository, _params : table) : ConstructionDefinition
	return {
		resName = constructionDefinition,
		constructions = {},
		builderAudioRes = { "::/gui/construction/sound/terraform_erase.builder_audio" },
		icon = { icon = resolve("asset_reset_material.tga") },
		previewIcon = { icon = resolve("asset_reset_material_preview.tga") },
		name = _("Asset Eraser Tool"),
		description = _("Erase assets with a brush."),
		action = "ACTION_ASSET_BRUSH",
		availability = { yearFrom = 0, yearTo = 0, },
		-- ... hudIcons, brush params ...
		menuCategory = {
			categories = {
				{ category = "landscaping_assets", filterCategories = {"*"}, order = -10000, },
			},
		},
	}
end,
```

`action` picks one of the built-in tools of [`ConstructionMenuAction`](../base/gui/construction/construction_menu.md) (symbol: `ConstructionMenuAction`, see [symbol index](../index.tsv)), so a mod can put an existing tool with other settings into another tab. The mechanics perks `industry_greenify_tool` and `marketing_campaign_tool` add a `customAction` with their own GUI recipe and put `perkCategory = "MECHANICS"` into their `metadata`. No shipped mod or DLC adds a `construction_tool`, so it is not confirmed that a mod's file of this type shows up in the menu, although the menu code reads every resource of the type.

## Ground textures

A ground face paints the terrain with a ground texture, a `.gtex.lua` file (710 in the base game). It maps the grey values of a mask image to terrain materials. `base/content/stations/air/air/helipad/helipad.gtex.lua` (shortened):

```lua
local tu = require "/scripts/tex_util.tl"  -- in a mod: require "::/scripts/tex_util.tl"

function data()
return {
	texture = tu.makeMaterialIndexTexture("helipad.tga", "CLAMP_TO_EDGE", "CLAMP_TO_EDGE"),

	texSize = { 36, 38 },
	materialIndexMap = {
		[95] = "::/terrain/materials/asphalt_02/asphalt_02.tmat",
		[8] = "::/terrain/materials/asphalt_03/asphalt_03.tmat",
		[199] = "::/terrain/materials/gravel_02/gravel_02.tmat",
		-- ...
	}
}
end
```

| Field | Meaning |
|---|---|
| `texture` | the mask, from [`TexUtil.makeMaterialIndexTexture(fileName, wrapS, wrapT)`](../base/scripts/tex_util.md) (symbol: `TexUtil.makeMaterialIndexTexture`, see [symbol index](../index.tsv)); the file is relative to the `.gtex.lua`. `"CLAMP_TO_EDGE"` stretches the mask over the face, `"REPEAT"` tiles it |
| `texSize` | size of the pattern in metres |
| `materialIndexMap` | grey value (1 nearly black to 255 white) to [terrain material](environment.md#terrain-materials) `.tmat`. Values not in the map stay transparent and show the normal terrain |
| `priority` | which texture wins where several overlap |
| `indices` | instead of `texture`: `data` (a string with one digit per pixel) and `size` |

The wiki's example loads `::/scripts/texutil.lua`. That file still exists, but 699 of the 710 base `.gtex.lua` files use `tex_util.tl` and none uses `texutil.lua`. The wiki says the material paths are relative to the `.gtex.lua`; the base files use absolute `::/terrain/materials/...` paths. A one-pixel example from `base/content/buildings/shared/shared/building_paving_fill.gtex.lua`:

```lua
indices = {
	data = "1",
	size = { 1, 1 },
},
texSize = { 8.0, 2.0 },
materialIndexMap = {
	"::/terrain/materials/asphalt_03/asphalt_03.tmat",
},
priority = 10
```

## Construction utilities

`base/tealdef/scripts/construction/` contains the definitions of the helpers. The implementations are Lua or Teal files in `base/content/scripts/scripts/construction/`.

| Definition | Contents |
|---|---|
| [construction_util_serialized](../base/scripts/construction/construction_util_serialized.md) | All result types (`ConstructionResult`, `SubConstruction`, `LaneList`, `StationDef`, `TerminalDef`, `Slot`, `SnapPoint`, `EdgeList`, `Stocks`, ...) and `addModelsAndGroups(params, group, subconstruction, tag?, transf2?)` |
| [lane_util_serialized](../base/scripts/construction/lane_util_serialized.md) | `createLane(curve, transportModes, number, number, boolean)`, returns a `LaneList` |
| [street_util_serialized](../base/scripts/construction/street_util_serialized.md) | Edge helpers: `addStraightEdge`, `addRamp`, `addEdge`, `addEdgeAutoTangents`, `freeAllNodes`, `calcScale` |
| [collider_util_serialized](../base/scripts/construction/collider_util_serialized.md) | The collider format: type `NONE`, `BOX`, `CYLINDER` or `POINT_CLOUD`, `transf`, `params.halfExtents` |
| [modules_util_serialized](../base/scripts/construction/modules_util_serialized.md) | `addCosts(result, module)`, `TransformAlignmentFaces`, `addGroundFaces` |
| [param_util](../base/scripts/construction/param_util.md) | Ready-made parameters: `makeTrackTypeDefaultParam`, `makeTrackCatenaryParam`, `makeTramTrackDefaultParam`, `makeRotationParam`, `getRailTrackTypes` |
| [asset_util](../base/scripts/construction/asset_util.md) | `assets`: map of asset group names to lists of `.mdl` paths |
| [construction_config_metadata](../base/scripts/construction/construction_config_metadata.md) | Level, requirements, stocks and effects stored in construction metadata |
| [construction_specialization_metadata](../base/scripts/construction/construction_specialization_metadata.md) | Specialisation metadata |

## Changing constructions from a mod

Two ways are visible in the shipped code. At load time, `addModifier("loadConstruction", fn)` (and `loadModule`, `loadMetaConstruction`) receives each file's data (see [Resource modifiers](scripting.md#resource-modifiers)). After loading, `api.res.constructionRep` and `api.res.moduleRep` are ordinary repositories; the base game's `postRunFn` adds modules with `api.res.moduleRep.add(mod.fileName, mod, true)`.

To place a construction from a script, the commands are `api.cmd.makeWorldBuildProposalCmd` and related proposal types; the mission framework's `build_construction` task checks player proposals against a construction file name (see [Missions](missions.md)). The proposal format is documented in the [api.type reference](../api/api/type.md), not here.

Official wiki: [Construction Basics](https://wiki.transportfever3.com/doku.php?id=modding:constructions:basics), [Construction Types](https://wiki.transportfever3.com/doku.php?id=modding:constructions:types), [Construction Menu](https://wiki.transportfever3.com/doku.php?id=modding:constructions:menu), [Ground Textures](https://wiki.transportfever3.com/doku.php?id=modding:constructions:groundtextures), [Modular Constructions](https://wiki.transportfever3.com/doku.php?id=modding:constructions:modular), [Construction Templates](https://wiki.transportfever3.com/doku.php?id=modding:constructions:templates).
