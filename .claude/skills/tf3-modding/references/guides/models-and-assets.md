
# Models and assets

Everything visible in the game is a model (`.mdl`) built from meshes (`.msh` plus `.msh.blob`) with materials (`.mtl`) that reference textures (`.dds`, `.tga`, `.hdr`). The base game ships 4,473 models and 6,806 materials. Models, mesh index files, materials and extracted animations (`.ani`) are Lua text files with a `data()` function; only the `.msh.blob` mesh data and the textures are binary ([Resource types](https://wiki.transportfever3.com/doku.php?id=modding:general:resourcetypes)). The unpacked game files these docs quote contain no `.msh`, `.msh.blob` or `.ani` files, so their format comes from the wiki alone.

Models are imported and edited with the Model Editor that ships with the game. Its workflow (setup, FBX import naming rules, metadata and material editors, validation, icons) is on its own page: [Model editor](model-editor.md).

## File names and paths

File and folder names are lowercase without spaces, and text files are UTF-8 without a byte order mark, as described in [File references inside content](mod-structure.md#file-references-inside-content). A model usually references its meshes and materials relative to itself (`msh/...` and `mat/...` in folders next to the `.mdl`), and a material its textures relative to the material (`tex/...`); the absolute forms (`/`, `::/` for base game files, `<modId>::/`) are explained on that page. The bus shelter below uses `mat/...` for its own parts and `/assets/stations/street/...` for parts shared with other stops; a mod that reuses those base parts writes `::/assets/stations/street/...`.

## A model file

A `.mdl` file's `data()` returns a table with the top-level keys `boundingInfo`, `collider`, `lods`, `metadata` and `version`. `version = 2` marks the TF3 format and tells it apart from the old Transport Fever 2 model format ([Model definition](https://wiki.transportfever3.com/doku.php?id=modding:general:resourcetypes:mdl)). From the bus shelter `base/content/stations/street/street/small_stops/small_new.mdl` (shortened):

```lua
function data()
return { 
		boundingInfo = { 
			bbMax = { 3.67032, 1.30928, 2.8806, },
			bbMin = { -3.60838, -1.08571, -0.181345, },
		},
		collider = { 
			params = { 
				halfExtents = { 3.63935, 1.1975, 1.53097, },
			},
			transf = { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, },
			type = "BOX",
		},
		lods = {
			{ 
				node = { 
					children = {
						{ 
							materials = { "mat/bus_era_c_glass.mtl", },
							mesh = "msh/shelter_era_c_left_1_post_glass_lod0.msh",
							name = "shelter_era_c_left_1_post_glass",
							transf = { 1, 0, 0, 0, 0, -1.62921e-07, -1, 0, 0, 1, -1.62921e-07, 0, 2.09323, 0.397267, 1.36608, 1, },
						},
						{ 
							materials = { "/assets/stations/street/mat/streetstation_1_transparent.mtl", "/assets/stations/street/mat/streetstation_1.mtl", },
							mesh = "/assets/stations/street/msh/bus_stop_sign_era_c_1_lod0.msh",
							name = "bus_stop_sign_era_c_1",
							transf = { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, -3.34, 0.9, 0, 1, },
						},
						-- ...
					},
					name = "RootNode",
					transf = { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, },
				},
				visibleFrom = 0,
				visibleTo = 50,
			},
			-- further LODs with visibleFrom/visibleTo 50-250 and 250-850
		},
		metadata = {
			lightSourceList = { ... },
		},
		version = 2,
	}
end
```

### Bounding box and collider

`boundingInfo` is the box around everything in the model, given as distances from the model origin (`bbMin`, `bbMax`). The renderer uses it to decide whether the model is on screen, so the wiki says it should also enclose the volume of the model's lights, or they pop in at the screen edge. The Model Editor recalculates it with one button.

`collider` is used for collisions between models. The wiki lists four types:

| `type` | Shape |
|---|---|
| `"MESH"` | the mesh geometry itself, `params = { }` |
| `"BOX"` | a box of twice `params.halfExtents`, moved by `transf` |
| `"CYLINDER"` | a cylinder of twice `params.halfExtents`, moved by `transf` |
| `"POINT_CLOUD"` | the points in `params.points` (relative to the origin); `transf` is ignored |

The base game files use `BOX` (4,464 colliders), `CYLINDER` (96) and `NONE` (41), which is also accepted by the construction collider format [ColliderUtilSerialized.Collider](../base/scripts/construction/collider_util_serialized.md) (symbol: `ColliderUtilSerialized.Collider`, see [symbol index](../index.tsv)). No base model uses `MESH` or `POINT_CLOUD`, and the wiki does not mention `NONE`.

### Levels of detail

`lods` is a list of levels of detail. Each entry has a `node` tree and a distance range from `visibleFrom` to `visibleTo` in metres. At least one LOD is required. The shelter has three, with separate `_lod0`, `_lod1`, `_lod2` meshes. Two more keys can appear per LOD:

- `textureLodBase = true` marks the first LOD from which textured materials take over, for example buildings that switch to baked textures from LOD 1. 616 base models set it.
- `static` appears in the wiki sample (`static = false`) and in 82 base models; the wiki does not explain it.

The wiki's performance advice for LODs: divide the triangle count by about four per LOD, aim for a few hundred triangles in the last one, and keep the number of meshes, materials and sorted (transparent) materials per LOD low. Typical base game vehicles have 20,000 to 50,000 triangles, the largest up to 100,000 ([guidelines](https://wiki.transportfever3.com/doku.php?id=modding:general:guidelines#technical_requirements)). The draw call rules in [Best practice](https://wiki.transportfever3.com/doku.php?id=modding:general:bestpractice) go further.

### Nodes

A node has a `name`, a 4×4 `transf` written as 16 numbers (relative to its parent), and optionally `mesh`, `materials`, `animations` and `children`. The rules from the wiki:

- `materials` lists one `.mtl` per submesh of the mesh; the sign above uses two. The counts must match. The Model Editor reports a mismatch and patches the group.
- `name` is how metadata refers to the node (`childId`, `group`, `meshId`, `loadIndicator`). It must be unique within a LOD, and the matching node in every other LOD should have the same name.
- Skinned meshes (characters, animals, bellows, some bridge and tunnel parts) use `skin` and `skinMaterials` in place of `mesh` and `materials`, and their `children` are bones with a `name` and `transf`. 303 base models do this.

A vehicle model nests nodes for moving parts: the steam lorry in `base/content/vehicle/truck/steam_lorry_univ/steam_lorry_univ/steam_lorry_univ.mdl` has a `body` node next to a group node whose child `g1` in turn holds `w1`.

### Animations

`animations` on a node maps event names to animations. Transformator scripts (see `transformatorConfig` below) trigger the events by name. There are three forms:

```lua
animations = {
	attack = { params = { id = "ani/attack/root.ani", }, type = "FILE_REF", },
	open = {
		forward = true,  -- false plays the keyframes backwards
		params = {
			keyframes = {
				{ rot = { 0, 0, 0, }, time = 0, transl = { 0, 0, 0, }, },
				{ rot = { 0, 0, 0, }, time = 1200, transl = { 0.75, 0.055, 0, }, },
			},
			origin = { 0, 0, 0, },
		},
		type = "KEYFRAME",
	},
	-- "KEYFRAME_MATRIX": keyframes with time and a 16-number transf
},
```

Times are milliseconds from the start. The wiki recommends `FILE_REF` to keep model files small, and the base game agrees: 539 base models use `FILE_REF`, 6 use `KEYFRAME`, none use `KEYFRAME_MATRIX`. An `.ani` file returns `times` and a list of `transfs` with one matrix per timestamp; the two lists must have the same length, and the first time must be 0.

The wiki's sample comments the `id` as "relative to res/models/animation/", which is the Transport Fever 2 layout. The base game files reference animations relative to the model, e.g. `id = "ani/die/cougar_root.ani"` in `base/content/animal/animal/cougar/cougar.mdl`.

## Meshes

A mesh is two files with the same name: `cube_lod_0.msh` is a text index and `cube_lod_0.msh.blob` holds the binary vertex and index data ([Mesh definition](https://wiki.transportfever3.com/doku.php?id=modding:general:resourcetypes:msh)). The index file returns two lists:

```lua
function data()
return {
	subMeshes = { ... },   -- one entry per submesh, each gets one material in the .mdl
	vertexAttr = { ... },  -- where each vertex attribute sits in the blob
}
end
```

The blob has two sections. The first stores vertex attributes as 4-byte floats; for each attribute the index file gives the `offset`, the number of floats per item (`numComp`) and the number of items (`count`). The second section holds, per submesh, a list of indices into those items, again located by offset and count. The attributes are `position`, `normal`, `tangent`, `uv0`, `uv1`, `jointWeights`, `offset`, `positionAmbient` and `attrOffsetLOD`; a mesh only has the ones it needs. Some material types need specific attributes: `_UV1_` materials need `uv1`, and skinned materials need joint weights. The Model Editor reports a missing attribute as "requires missing mesh attribute".

Meshes are not written by hand. The Model Editor creates them when it imports an `.fbx` file.

## Model metadata

`metadata` decides what a model is in the game: a vehicle, a tree, a person, a station part. Each block is optional. Counted over all base `.mdl` files, the most common blocks are:

| Block | Files | Block | Files |
|---|---|---|---|
| `lightSourceList` | 703 | `transportVehicle` | 311 |
| `transformatorConfig` | 540 | `particleSystem` | 291 |
| `soundConfig` | 514 | `landVehicle` | 288 |
| `availability` | 471 | `cargoModel` | 242 |
| `description` | 469 | `versioning` | 201 |
| `emissions` | 449 | `railVehicle` | 168 |
| `transportNetworkProvider` | 402 | `loadIndicator` | 142 |
| `extent` | 376 | `roadVehicle` | 120 |
| `categoryList` | 357 | [`tree`](environment.md#landscape-assets) | 90 |
| `cost` | 351 | `colorConfig` | 85 |
| `seatProvider` | 343 | `airVehicle`, `waterVehicle` | 42, 28 |
| `maintenance` | 322 | `person`, [`rock`](environment.md#landscape-assets), [`animal`](environment.md#animals) | 38, 23, 15 |

The Teal definition [`ModelMetadata`](../api/api/type.md) in `api/tealdef/api/type.d.tl` ("Contains all metadata of a model. A specific metadata might be nil.") types a subset of them: `Animal`, `Description`, `Category`, `CategoryList`, `MenuCategory`, `Availability`, `SoundConfig`, `Maintenance`, `Emission`/`Emissions`, `VehicleEngine`, `TransportVehicle`, `Cost`, `LandVehicle`, `WaterVehicle`, `AirVehicle`, `Person` and `Tree`. Blocks such as `lightSourceList`, `particleSystem` or `loadIndicator` appear in the files but not in that record. The Model Editor's own list of metadata fields, with display names and value types, is `base/content/model_editor/model_editor/meta_metadata.lua`.

The wiki documents the blocks that most model types share on the model page and the vehicle-specific ones under [Vehicles](https://wiki.transportfever3.com/doku.php?id=modding:vehicles). A vehicle's metadata, from the steam lorry (shortened):

```lua
metadata = { 
	availability = { yearFrom = 1905, yearTo = 1945, },
	cost = { price = -1, },
	description = { 
		description = _("VEHICLE_TRUCK_STEAM_LORRY_UNIV_DESCRIPTION"),
		name = _("VEHICLE_TRUCK_STEAM_LORRY_UNIV_NAME"),
	},
	emissions = { noise = { score = 15, }, pollution = { score = 18, }, },
	landVehicle = { 
		engines = { { power = 14, tractiveEffort = 5, type = "STEAM", }, },
		topSpeed = 6.944,
		weightEmpty = 1000,
		weightMaxPayload = 12000,
	},
	maintenance = { lifespan = 14610, runningCosts = -1, },
	-- in a mod: "::/vehicle/truck/shared/sound/truck_old.snd"
	soundConfig = { soundSet = { name = "/vehicle/truck/shared/sound/truck_old.snd", }, },
	transformatorConfig = { 
		skipFromLod = 2,
		-- in a mod: "::/vehicle/shared/default_road.trf"
		transformator = { name = "/vehicle/shared/default_road.trf", },
	},
	transportVehicle = { 
		carrier = "ROAD",
		compartments = {
			{ 
				loadConfigs = {
					{ 
						cargoEntry = { 
							capacity = 40,
							cargoTypeSet = { cargoClassesIncluded = { "UNIVERSAL", }, },
							loadIndicator = "bay_0",
							seats = { },
						},
						toHide = { },
					},
				},
			},
		},
		engineTransportModes = { "TRUCK", },
		filterTags = { "default", },
		transportModes = { "TRUCK", },
	},
},
```

`compartments` → `loadConfigs` → `cargoEntry` matches [`ModelMetadata.TransportVehicle`](../api/api/type.md) (symbol: `ModelMetadata.TransportVehicle`, see [symbol index](../index.tsv)). `cargoEntry.loadIndicator = "bay_0"` names the entry `metadata.loadIndicator.configs.bay_0`.

### Common blocks

| Block | What it sets |
|---|---|
| `description` | `name` and `description` (both wrapped in `_()` for translation) for the Vehicle Store and entity windows. Inside constructions they are not used. |
| `availability` | `yearFrom`, `yearTo`; leaving one out or setting it to `0` means no start or end year |
| `cost` | `price`; `-1` turns on automatic pricing, a missing `price` is 0. `priceScale` multiplies the computed price. |
| `maintenance` | `runningCosts` (`-1` for automatic) with `runningCostScale`; `lifespan` in quarter days at normal speed (365 × 4 per year, so the lorry's 14610 is ten years); `maintenanceScale` for wear (at 1.0 the vehicle loses all maintenance in a tenth of its lifespan) |
| `extent` | `bbMin`/`bbMax` like the bounding box, used by the simulation, e.g. the vehicle length for coupling |
| `cameraConfig` | `positions`, each with `group` (node name), `transf` and `fov`, for the cockpit camera that otherwise sits at the first crew seat |
| `loadIndicator` | how cargo models are shown, see below |
| `labelList` | dynamic text labels, see below |
| `lightSourceList` | light sources, see below |
| `particleSystem` | smoke, steam, dust and flame emitters, see below |
| `transformatorConfig` | the transformator script that drives animations and particles, see below |

The automatic cost values come from the base scripts: `ModelMetadataUtil` in `base/content/base/base/model_metadata_util.lua` replaces a negative `price` with `round(maintenanceCost * 6 * (priceScale or 1))` and a negative `runningCosts` with `round(maintenanceCost * (runningCostScale or 1))`. The wiki text names the field `runningCost` in one sentence; the files and the wiki's own code sample use `runningCosts`. `maintenanceScale` appears in no base model.

### Load indicators

`loadIndicator.configs` is a map of named indicators (`bay_0`, `slots_0`, ...) that a `cargoEntry` references by name. Each holds one of three kinds:

- `cargoBay`: a box (`bbMin`, `bbMax`, relative to the node `childId`) that fills with cargo models. `type = "DISCRETE"` lays `cargoFormats` such as `"SMALL"` or `"BIG"` in a `gridSize` pattern (x, y and optionally z layers) and scales them by `sizePolicy` (`"STRETCH"`, `"STRETCH_HEIGHT_SCALEY"`, `"BEST_FIT"`, or no scaling). `type = "LEVEL"` shows a bulk cargo surface at a height that depends on the load; use `sizePolicy = "STRETCH"` with it.
- `cargoSlots`: a `capacity` and a list of `configurations`. A configuration is a list of levels from empty to almost full, each listing slot indices from `loadIndicator.slots`. With several configurations, one is picked at random. Each slot has a `group` (node), a list of `models` picked at random, a `transf` and a `randomId`; slots with the same `randomId` pick the same model index. Slot models are real `.mdl` files or a cargo format reference such as `"#RECT_11x2_5"`.
- `cargoLanes`: for static models, `nodes` of position, tangent and width that form lanes of `SMALL`/`BIG` cargo, plus an optional `renderDistance`.

### Labels

`labelList.labels` places dynamic text on a model. A label has a `childId` node, a `transf` at the lower left corner and a `size` (width, height, which also sets the font size). `type` picks the text: `"LINE_NAME"`, `"NEXT_STOP"`, `"NAME"`, `"COMPANY_NAME"`, `"STATION_NAME"`, `"CUSTOM"` (from a `labelText` in the construction) or `"NONE"`. Further keys:

| Key | Values |
|---|---|
| `color`, `alpha`, `alphaMode` | RGB 0 to 1; 0 to 1; `"CUTOUT"`, `"BLEND"`, `"NONE"` |
| `renderMode` | `"EMISSIVE"` for LCD-style displays, `"STD"` otherwise |
| `alignment`, `verticalAlignment` | `"LEFT"`/`"CENTER"`/`"RIGHT"`; `"BOTTOM"`/`"CENTER"`/`"TOP"` |
| `fitting` | `"NONE"` (overflow), `"CUT"`, `"SCALE"` |
| `nLines` | number of lines |
| `filter`, `params` | `"NONE"`, `"NUMBER"` or `"CUSTOM"` with `params.expr` (regular expression), `params.replace` (`\\0`, `\\1`, ...), and for `NEXT_STOP` `offset` and `relative` |
| `font` | only the built-in Lato and Noto fonts; others crash the game |

### Light sources

`lightSourceList.lights` holds lights attached to the node named in `meshId`, placed by `transf`. Scaling the parent node scales the light, so an animation that hides a mesh also turns off its light.

| Key | Meaning |
|---|---|
| `type` | `"SPOT"` (`radius`, `angleMin`, `angleMax`), `"POINT"` (`radius`), `"CYLINDRICAL"` and `"CHEAP_CYLINDRICAL"` (`length`, `radius`), `"PARALLEL"` (`length`, `width`, `height`) |
| `mode` | `"LIGHT"`, `"ABSORBER"` (darkens), `"DISCARD"` (cuts meshes away, used at tunnel and stair entrances) |
| `color`, `strength` | RGB 0 to 1, intensity |
| `mask`, `invertMask` | bit mask against the material's `light_receiver.lightMask` (1 terrain, 2 exterior, 4 water, 8 particles, 32 interior). Exterior lights use `mask = 0` with `invertMask = true`, vehicle interior lights use `32` with `invertMask = false`. |
| `backFaceLit` | also light faces turned away from the light |
| `fromTime`, `toTime` | hours 0 to 24; both `-1` switches on in darkness and heavy rain |
| `visibleFrom`, `visibleTo`, `fadeOutStart` | render distance in metres from the camera, with a fade from `fadeOutStart` to `visibleTo` |

The bus shelter sets `visibleFrom = -1` and `visibleTo = -1`, a value the wiki does not explain. The wiki asks for few, well-placed lights and suggests light materials (`map_id`, `map_lgt`) for building windows.

### Particle emitters

`particleSystem.emitters` is a list of emitters. Each needs `child` (the node), `frequency` (particles per second) and `lifeTime`. Optional keys include `particleId` (for the transformator), `position`, `velocity`, `gravity`, `albedoTexture`, `normalMapTexture`, sprite sheet settings (`numFrames`, `frame`, `frameSpeed`), `animateInvisible` and `maxDistance` (default 100). Numeric values can be fixed (`{ value = 30 }`) or random (`{ randomMinMax = { min = 10, max = 30 } }`). Keys ending in `OverLifeTime` (alpha, color, velocity, size) are curves of `{ time, value, easing }` points with `time` from 0 to 1; `easing` takes the usual easing names from `Linear` to `EaseInOutBounce`. The full key table with defaults is on the [wiki model page](https://wiki.transportfever3.com/doku.php?id=modding:general:resourcetypes:mdl). Keep `frequency`, `lifeTime` and `maxDistance` low, and set `animateInvisible` only on large, long-lived particles such as a steam engine's exhaust.

### Transformator config

In TF3 animation and particle events are not hard-coded; a transformator script triggers them. `transformatorConfig.transformator.name` references a `.trf` file (the base game's defaults are `/vehicle/shared/default_road.trf` and `/vehicle/train/shared/default_train.trf`), and `params` passes model values to it, e.g. `pantographMin`/`pantographMax`. The base game models write these paths with `/`. In a mod, write `::/vehicle/shared/default_road.trf`, because a path that starts with `/` points into your own mod; the campaign's `carnival_waggon_1.mdl` (`mods/release/urbangames_campaign_mission_01/content/vehicle/vehicle/bus/carnival_waggon_1/`) does this. `skipFromLod` is the first LOD that no longer runs the transformator; the wiki gives 1 as the default when it is missing. 438 base models use `skipFromLod = 2`. See [transformator types](../api/api/type/transformator.md) and the wiki's [vehicle advanced topics](https://wiki.transportfever3.com/doku.php?id=modding:vehicles:advancedtopics).

### Metadata is post-processed on load

The base game rewrites model metadata while loading. `base/content/base/base/base_mod.lua` registers a chain of `loadModel` modifiers, among them:

```lua
addModifier("loadModel", model_metadata_util.addTransformatorConfig)
addModifier("loadModel", metadataanimationutil.createAnimationEventsRoadVehicle)
-- ...
addModifier("loadModel", model_metadata_util.toCompartmentList)
addModifier("loadModel", model_metadata_util.turnRoadAndRailToLandVehicle)
addModifier("loadModel", model_metadata_util.addVehicleExtent)
```

The full list of converters is in [`ModelMetadataUtil`](../base/base/model_metadata_util.md) (`addEmissionMetadata`, `addCostMetadata`, `convertCargoTypes`, `makeLoadIndicators`, `sortLods`, `addTextureLods`, ...). Names such as `turnRoadAndRailToLandVehicle` or `convertCargoTypeSet` suggest that older metadata formats are converted into the current one. Several of them (`addTransformatorConfig`, `sortLods` and the `createAnimationEvents*` functions) only convert models older than `version = 2`. A new model must contain those blocks in their final form; others, such as default icons and filter tags, are also filled in for `version = 2` models (see [Vehicles](vehicles.md#which-metadata-blocks-a-vehicle-needs)). The same file also derives `seatProvider` from `transportVehicle.seats` and fills `comfortFactor`/`priceFactor` per carrier when they are `-1`. The Model Editor runs its own copy of most of this chain from `base/content/model_editor/model_editor/editor_base_mod.lua`, so a model looks the same in the editor as in the game.

A mod can hook into the same place. `mods/release/urbangames_vehicles_no_end_year/content/mod.script.lua` sets `data.metadata.availability.yearTo = 0` for every vehicle in a `loadModel` modifier (shown in [Resource modifiers](scripting.md#resource-modifiers)). After loading, models are in `api.res.modelRep`; campaign mission 01 makes the horse cart load faster in its `postRunFn` by editing `model.metadata["transportVehicle"].loadSpeed`.

## Materials

A `.mtl` file picks a material `type` and fills its parameters. `order` sorts materials of the same type for rendering; the default is 0, and an interior material set to `order = -1` renders before the windows of the same type ([Material definition](https://wiki.transportfever3.com/doku.php?id=modding:general:resourcetypes:mtl)). From `base/content/stations/street/street/small_stops/mat/bus_era_c.mtl` (shortened):

```lua
function data()
return { 
		__version = "",
		order = 0,
		params = { 
			map_albedo = { 
				fragmentSamplers = { 
					albedoTex = { 
						fileName = "tex/bus_stop_era_c_alb.dds",
						type = "TWOD",
						wrapS = "REPEAT",
						wrapT = "REPEAT",
					},
				},
			},
			map_metal_gloss_ao = { 
				fragmentSamplers = { 
					metalGlossAoTex = { fileName = "tex/bus_stop_era_c_mga.dds", type = "TWOD", wrapS = "REPEAT", wrapT = "REPEAT", },
				},
			},
			map_normal = { 
				fragmentSamplers = { 
					normalTex = { fileName = "tex/bus_stop_era_c_nrm.dds", redGreen = true, type = "TWOD", wrapS = "REPEAT", wrapT = "REPEAT", },
				},
			},
			window_light_color = { 
				fragmentProperties = {
					{ color1 = { 0.92, 0.81, 0.72, }, color2 = { 0.9, 0.8, 0.7, }, },
				},
			},
			-- map_id, map_lgt, map_op_1, map_op_2, operation_1, operation_2, texcoord_scale, light_receiver ...
		},
		type = "PHYSICAL_NRML_MAP_OP_LIGHT",
	}
end
```

Each parameter wraps its values in `fragmentSamplers` (textures), `fragmentProperties` (most settings) or `vertexProperties` (a few, such as `fade_out_range`). The wiki recommends editing materials in the Model Editor, which handles this nesting.

### How material types are defined

`type` names a material type defined in `base/content/rendering/` (the `files` list of `base/_content.json` has them as loose `rendering/*.mat.lua` files). `rendering/physical_nrml_map_op_light.mat.lua` declares `legacyName = "PHYSICAL_NRML_MAP_OP_LIGHT"`, the short `prefix`, the list of properties the type accepts, its render passes and technique:

```lua
function data()
return {
	hasSmoothLod = false,
	legacyName = "PHYSICAL_NRML_MAP_OP_LIGHT",
	needsTangentVertexAttrib = true,
	prefix = "PNOL",
	properties = { 
		{ name = "map_metal_gloss_ao", id = "properties/map_metal_gloss_ao.prop",  },
		{ name = "map_albedo", id = "properties/map_albedo.prop",  },
		{ name = "map_normal", id = "properties/map_normal.prop",  },
		-- ...
	},
	renderPasses = {
		COLOR = { needsAlphaBlending = true, needsBlend4 = false, programName = "programs/color_nrml_map.prog" },
		DEPTH = { needsAlphaBlending = false, needsBlend4 = false, programName = "programs/depth.prog" },
		NORMAL = { needsAlphaBlending = false, needsBlend4 = false, programName = "programs/phys_nrml_map_op_light.prog" },
	},
	technique = "techniques/standard_nrml_map_op.tec",
	transparent = false,
	order = 10,
}
end
```

Each key under `params` in a `.mtl` matches a property `name` here, and a property file such as `rendering/properties/map_albedo.prop.lua` declares the samplers it binds (`albedoTex`). Shader programs are `.prog.lua` files in `rendering/programs`. The `prefix` is the short code that the Model Editor shows and that FBX material names use to pick a type (`mymaterial|PNOL`, see [Model editor](model-editor.md#materials)).

A material type sits on top of further rendering files, all under `base/content/rendering/`. `technique` names a `.tec.lua` file (32 in the base game), which declares the vertex, instance and storage inputs of each render pass and the topology. Each `programName` names a `.prog.lua` file (223, of which 8 are console variants under `programs/xbox/` and `programs/ps5/`). A program lists its `defines`, its descriptor sets (`.desc.lua` in `programs/descriptors/`, the bindings of samplers and uniform buffers) and its `shaderStages`, whose `sourceFiles` are shader sources such as `shaders/mat/fs/color/std.fs`. A property file (`.prop.lua`, 47) declares each value of a parameter with its type and default. From `rendering/programs/color_nrml_map.prog.lua` (shortened):

```lua
descriptorSets = { "descriptors/phys_world.desc", "descriptors/phys_material_type.desc", "descriptors/set_36.desc", },
shaderStages = {
	{
		sourceFiles = { "shaders/mat/fs/color/std.fs", "shaders/util/lighting.fs", --[[ ... ]] },
		stage = "FRAGMENT",
	},
	{
		sourceFiles = { "shaders/mat/vs/std/color.vs", "shaders/util/fade_out.glsl", "shaders/util/inst_attr.vs", "shaders/util/fog.glsl", },
		stage = "VERTEX",
	},
},
```

The shader sources (`.fs`, `.vs`, `.glsl` and others) are listed in `base/_content.json` but are not among the unpacked files these docs quote. Each of these file types has a [resource modifier](scripting.md#resource-modifiers) (`loadMaterialType`, `loadTechnique`, `loadProgram`, `loadDescriptorSet`, `loadProperty`). A material type that looks different from the existing ones needs its own shaders. The [upload guidelines](https://wiki.transportfever3.com/doku.php?id=modding:general:guidelines) forbid custom shader files on consoles only and set no such rule for PC, but neither they nor any other wiki page explain how to write a shader for the game.

### Material types

The base game defines 51 material types; the wiki tables list 43 and say "over 40". They fall into these groups (prefix in brackets):

| Group | Types | Use |
|---|---|---|
| Physical, opaque | `PHYSICAL` (P), `PHYSICAL_OP` (PO), `PHYSICAL_NRML_MAP` (PN), `PHYSICAL_NRML_MAP_CBLEND` (PNC), `PHYSICAL_NRML_MAP_CBLEND_DIRT` (PNCD), `PHYSICAL_NRML_MAP_OP` (PNO), `PHYSICAL_NRML_MAP_CBLEND_OP` (PNCO), `PHYSICAL_NRML_MAP_UV1_AO` (PNUA), `PHYSICAL_NRML_MAP_OP_UV1_AO` (PNOUA), `..._OP_UV1_AO_MASK` (PNOUAM), `..._OP_DIRT_UV1_AO_MASK_DIRT_MASK` (PNODUAM) | most buildings, assets and vehicle bodies |
| Physical, transparent | `PHYS_TRANSPARENT` (T), `_OP` (TO), `_UV1_OP` (TUO), `_NRML_MAP` (TN), `_NRML_MAP_OP` (TNO), `_NRML_MAP_SMOOTH_LOD` (TNS), `_NRML_MAP_UV1_OP` (TNUO), `_NRML_MAP_UV1_OP_SMOOTH_LOD` (TNUOS), `_NRML_MAP_CBLEND` (TNC), `_NRML_MAP_CBLEND_DIRT` (TNCD) | glass, vehicle windows, ornamented fences |
| Physical with light | `PHYSICAL_NRML_MAP_LIGHT` (PNL), `PHYSICAL_NRML_MAP_OP_LIGHT` (PNOL), `PHYSICAL_NRML_MAP_OP_UV1_AO_LIGHT` (PNOUAL), `PHYS_TRANSPARENT_NRML_MAP_LIGHT` (TNL), `PHYS_TRANSPARENT_NRML_MAP_OP_UV1_AO_MASK_LIGHT` (TNOUAML) | lit windows of town buildings, stations and industries |
| Skinning | `SKINNING_TEXTURED` (S), `SKINNING_PHYS_CBLEND4` (SC), `SKINNING_PHYS_NRML_MAP` (SN), `..._NRML_MAP_OP` (SNO), `..._NRML_MAP_CBLEND4` (SNC), `..._NRML_MAP_CBLEND_DIRT` (SNCD), `SKINNING_PHYS_TRANSPARENT_NRML_MAP` (STN) with `_OP` (STNO), `_CBLEND4` (STNC), `_CBLEND_DIRT` (STNCD) | meshes with bones: people, animals |
| Other | `BILLBOARD` (B), `BILLBOARD_MULTI` (BM), `EMISSIVE` (E), skinned emissive (SE), `LEAF_CARD` (L), `PHYSICAL_GLOSS_ONLY` (PG), `PHYSICAL_OP_GLOSS_ONLY` (POG) | far tree LODs, light-emitting surfaces, tree leaves |

Which maps each type takes is in the wiki's tables; the authoritative list for a type is the `properties` list in its `rendering/*.mat.lua`. Skinned meshes can have up to 40 bones, and each vertex can follow at most four of them with weights set in the modelling software.

The game files differ from the wiki tables in these points:

- The wiki calls the skinned emissive type `SKINNED_EMISSIVE`. The game's `rendering/skinning_emissive.mat.lua` has `legacyName = "SKINNING_EMISSIVE"`, and that is what `.mtl` files use.
- Eight types exist in `rendering/` but not in the wiki: `BILLBOARD2` (B2, 49 base materials), `PHYSICAL_NRML_MAP_CBLEND4` (PNC4), `PHYSICAL_NRML_MAP_CBLEND_DIRT_LOGO` (no prefix), `PHYSICAL_NRML_MAP_OP_UV1_AO_MASK_LIGHT` (PNOUAML), `PHYSICAL_NRML_MAP_OP_UV1_LIGHT` (PNOUL), `PHYS_TRANSPARENT_DIFFUSE` (TD, the importer's placeholder material), `PHYS_TRANSPARENT_NRML_MAP_AO_SMOOTH_LOD` and `PHYS_TRANSPARENT_NRML_MAP_CBLEND_DIRT_UV1_OP` (TNCDUO).
- `PHYS_TRANSPARENT_NRML_MAP_AO_SMOOTH_LOD` is used by 288 base materials and shares the prefix TNS with `PHYS_TRANSPARENT_NRML_MAP_SMOOTH_LOD`, so the prefix alone does not pick one of the two.

### Texture samplers

A map parameter holds one sampler. The full set of sampler keys:

```lua
map_albedo_opacity = { 
	fragmentSamplers = { 
		albedoOpacityTex = { 
			fileName = "tex/bus_d40_albo.dds",
			type = "TWOD",                       -- TWOD, CUBE_MAP, TWOD_ARRAY
			magFilter = "LINEAR",                -- NEAREST, LINEAR
			minFilter = "LINEAR_MIPMAP_LINEAR",  -- NEAREST, LINEAR, LINEAR_MIPMAP_LINEAR, LINEAR_MIPMAP_NEAREST, NEAREST_MIPMAP_NEAREST
			wrapS = "CLAMP_TO_EDGE",             -- REPEAT, CLAMP_TO_EDGE
			wrapT = "CLAMP_TO_EDGE",
			maxDegreeOfAnisotropy = -1,
			mipmapAlphaScale = 0,
			mipmapBaseLevel = 0,
			redGreen = false,
			scaleDownAllowed = true,
			textureLodOffset = 0,
		},
	},
},
```

`wrapS`/`wrapT` choose tiling or clamping. `maxDegreeOfAnisotropy = -1` is the recommended default. `mipmapAlphaScale` scales alpha in mipmaps generated from `.tga` files, which softens cutout edges. `mipmapBaseLevel` forces a lower mipmap of a `.dds`. `redGreen` marks a two-channel normal map. `scaleDownAllowed` allows texture streaming, and `textureLodOffset` shifts the streamed mipmap. The Model Editor's material field list (`base/content/model_editor/model_editor/meta_material.lua`) also has a `compressionAllowed` flag that the wiki does not mention.

### Maps

| Map | Content |
|---|---|
| `map_albedo`, `map_color` | colour, no alpha |
| `map_color_alpha` | colour with on/off transparency |
| `map_albedo_opacity` | colour with 0 to 100 % transparency |
| `map_albedo_gloss` | colour with gloss in alpha (old Train Fever style) |
| `map_emissive` | light colour, only for `EMISSIVE` |
| `map_metal_gloss_ao` | R metal, G gloss, B ambient occlusion (0 = full shading) |
| `map_ao` | separate ambient occlusion on the second UV set |
| `map_normal` | tangent-space normal map |
| `map_cblend` | four colour-blend masks in RGBA, used with `colorConfig` on characters |
| `map_cblend_dirt_rust` | R colour blend, G dirt, B rust intensity |
| `map_dirt`, `map_rust`, `map_dirt_normal`, `map_rust_normal` | custom dirt and rust textures; without them the shared ones in `vehicle/shared/mat/tex/` are used |
| `map_op_1`, `map_op_2` | overlay textures for the two operations |
| `map_id` | 8-bit greyscale window light IDs: faces with the same value switch on together. Values must be 0, 255 or 4 + 8 × n. Use `NEAREST` filters, `CLAMP_TO_EDGE` and `scaleDownAllowed = false`. |
| `map_lgt` | red channel masks the window light intensity (curtains, a darker top edge) |

Colour-blend channels work inversely: the darker the channel, the stronger the recolouring. Set `color_blend.albedoScales` to the brightness of the original texture colour in that area so recoloured parts match other models.

### Operations

`operation_1` and `operation_2` blend `map_op_1`/`map_op_2` onto the albedo, for example to break up repeating wall textures:

```lua
operation_1 = {
	fragmentProperties = {
		{
			op = "OVERLAY",        -- NO_OP, MULTIPLICATION, OVERLAY, LINEAR_BURN, ALPHA_BLEND
			mode = "NORMAL",       -- TEXCOORD, WORLD_XY, NORMAL, OFFSET_TEXCOORD
			scale = { 0.05, 0.5 },
			opacity = 0.5,
		},
	},
},
```

`MULTIPLICATION` multiplies the channels (darker), `OVERLAY` multiplies below 0.5 and screens above it, `LINEAR_BURN` adds and subtracts 1, and `ALPHA_BLEND` lays the overlay over the albedo where it is opaque (moss at the foot of a tree trunk). `mode` chooses the overlay coordinates: model-local (`NORMAL`), world (`WORLD_XY`), UV (`TEXCOORD`) or UV with a random offset (`OFFSET_TEXCOORD`).

### Properties

| Parameter | Keys | Notes |
|---|---|---|
| `light_receiver` | `lightMask`, `isLegacyMaterial`, `disableShadowMapWrite`, `lightMaxDistanceScale` | `lightMask` bits as for lights: 2 for most models, 32 for vehicle interiors, 34 for characters. `isLegacyMaterial` is for converted TF2 textures only. |
| `albedo_scale` | `albedoScale` (RGB) | tints one texture into several colour variants |
| `alpha_scale` | `alphaScale` | scales transparency |
| `emissive_scale` | `emissiveScale` (RGB), `fromTime`, `toTime` | for `EMISSIVE`; times `-1` follow daylight and weather |
| `normal_scale` | `normalScale` | node scaling also changes the normal strength |
| `alpha_test` | `alphaThreshold`, `a2CThreshold`, `cutout`, `sorted`, `disableDepthAndNormalWrite` | shadow threshold, visibility threshold, transparency mode and sorting |
| `color_blend` | `albedoScales`, `color` or `colors` | one value for the red mask, four for `SKINNING_PHYS_NRML_MAP_CBLEND4` |
| `dirt_rust` | `age`, `dirtColor`, `dirtOpacity`, `dirtScale`, `rustColor`, `rustOpacity`, `rustScale` | `age` 0 to 1 for models that get no age from the game |
| `fade_out_range` | `fadeInStartDist`, `fadeInEndDist`, `fadeOutStartDist`, `fadeOutEndDist` | in `vertexProperties`; fades instead of popping out |
| `polygon_offset` | `factor`, `units`, `forceColorWrite`, `forceDepthWrite` | shifts depth; set `factor` and `units` to the same small negative value, and `forceDepthWrite` on the lowest offset material |
| `two_sided` | `twoSided`, `flipNormal` | doubles the rendered faces; set `flipNormal` with normal maps |
| `tex_animation` | `uvSpeed`, `frameSize`, `frameSpeed` | scrolling UVs or a sprite grid |
| `window_light_color` | `color1`, `color2`, `fromTime`, `toTime` | colours of lit windows |

In the base game, 1,162 `.mtl` files set `isLegacyMaterial = true` and 5,585 set it to `false`, although the wiki reserves the flag for old Transport Fever 2 textures. The editor's default dirt template also sets `dirtFactor` and `rustFactor`, which the wiki does not list.

### Transparency

Opaque types render before transparent ones, and anything behind a transparent surface must render first or it disappears. The wiki gives four tools, in this order: `order` within one type; `alpha_test.cutout = true`, which turns transparency into a dither pattern (clean at 0, 25, 50, 75 and 100 %, grainy in between) and suits railings and leaves; `alpha_test.sorted = true`, which sorts meshes by their origin's distance to the camera at the cost of extra draw calls, so not for lower LODs; and `polygon_offset`, which moves the surface back in the depth buffer, e.g. headlights behind glass.

## Textures

`.dds` with compression and mipmaps is the preferred texture format ([Resource types](https://wiki.transportfever3.com/doku.php?id=modding:general:resourcetypes#texture_files)). The supported compressions:

| Format | Use |
|---|---|
| BC1/DXT1 | no alpha or on/off alpha; smallest files |
| BC2/DXT3 | 4-bit alpha |
| BC3/DXT5, BC7 | smooth alpha |
| BC5/3Dc (`ATI2A2XY`) | normal maps; the wiki says this one is required for them |
| BC4/R8/L8 | 8-bit greyscale masks such as `map_id` |

Sizes must be powers of two from 16 to 4096 for ATI and Intel cards. `.tga` is for UI graphics; icons are stored at double size with an `@2x.tga` name for 4K screens. Use `.tga` for model textures only during work: the Model Editor converts `.tga` textures to `.dds` when it saves a model. `.hdr` is listed as a texture format without further detail. Tools that export DDS are on [External tools](https://wiki.transportfever3.com/doku.php?id=modding:tools:external).

Materials reference textures relative to the material file (`tex/...`) or absolutely (`/placeholders/mat/tex/default_map_id.dds`). The base game names them by map with a suffix: `_alb`, `_albo`, `_nrm`, `_mga`, `_cdr`, `_cblend`, `_id`, `_e` (light mask) and so on. The Model Editor relies on these suffixes when it imports an `.fbx`; the full table is on the [Model editor](model-editor.md#textures) page.

`texture_metadata_cache.lua` at the root of each mod and of `base/` lists every texture with `channels`, `extent`, `numMipmaps` and `numStrippedMipmaps`:

```lua
["animal/bird_crane/mat/tex/bird_crane_albedo_opacity.dds"] = { 
	channels = 4,
	extent = { 512, 512, },
	numMipmaps = 10,
	numStrippedMipmaps = 0,
},
```

Whether a mod must ship this cache is an open question, described in [Files generated by the game](mod-structure.md#files-generated-by-the-game).

## Other asset files

| Extension | Example | Content |
|---|---|---|
| `.gtex.lua` | `base/content/stations/air/air/helipad/helipad.gtex.lua` | ground texture: a material index texture plus `materialIndexMap` from colour index to `.tmat` terrain materials |
| `.tmat.lua` | `::/terrain/materials/asphalt_02/asphalt_02.tmat` | terrain material, see [Terrain materials](environment.md#terrain-materials) |
| `.snd.lua` | `::/vehicle/truck/shared/sound/truck_old.snd` | sound set, referenced from `soundConfig.soundSet.name`, see [Sound sets](other-content.md#sound-sets) |
| `.trf.lua` | `::/vehicle/shared/default_road.trf` | transformator, referenced from `transformatorConfig` ([transformator types](../api/api/type/transformator.md)) |
| `.ani` | `ani/die/cougar_root.ani` (next to `::/animal/cougar/cougar.mdl`) | animation: `times` in milliseconds and one `transfs` matrix per time |
| `.wav`, `.ogg` | | sound effects, music |

Official wiki: [Resource types](https://wiki.transportfever3.com/doku.php?id=modding:general:resourcetypes), [Model definition (.mdl)](https://wiki.transportfever3.com/doku.php?id=modding:general:resourcetypes:mdl), [Mesh definition (.msh)](https://wiki.transportfever3.com/doku.php?id=modding:general:resourcetypes:msh), [Material definition (.mtl)](https://wiki.transportfever3.com/doku.php?id=modding:general:resourcetypes:mtl), [Model Editor](https://wiki.transportfever3.com/doku.php?id=modding:tools:modeleditor), [Best practice](https://wiki.transportfever3.com/doku.php?id=modding:general:bestpractice).
