
# Environment

A map in Transport Fever 3 comes from a chain of files. The climate (`.clima.lua`) names the default [economy](game-mechanics.md#economies), name list and environment and holds the node tree that paints terrain materials. A terrain generator (`.gen.lua`) belongs to one climate and runs its own node tree to build the heightmap, the biome masks and the forests. The environment (`.env.lua`) sets light, atmosphere, sky and water. Terrain materials (`.tmat.lua`), grass (`.grass.lua`), trees, rocks and animals (`.mdl` with special metadata) fill in the details, and the climate's `category` decides which of them a game can use.

The base game has four climates in `base/content/climates/climates/`: `temperate`, `subarctic`, `tropical` and `dry`. Each folder holds the same set of files:

| File | Example | Purpose |
|---|---|---|
| `.clima.lua` | `temperate.clima.lua` | climate |
| `_clima.tree.lua` | `temperate_clima.tree.lua` | node tree for terrain material distribution |
| `.gen.lua` | `temperate.gen.lua`, `temperate_import.gen.lua` | terrain generators |
| `_gen.tree.lua` | `temperate_gen.tree.lua`, `temperate_import_gen.tree.lua` | node trees of the generators |
| `.env.lua` | `temperate.env.lua`, `temperate_old.env.lua` | environments |

The doubled folder name comes from the unpacked zips (see [Mod structure](mod-structure.md#how-the-zips-map-to-folders)). Inside the game the climate is `::/climates/temperate/temperate.clima`.

Mods can change all of these while the game loads. `base/content/base/base/mod.lua` has modifier keys for each type, among them `loadClimate`, `loadEnvironment`, `loadTerrainGenerator`, `loadNodeTree`, `loadTerrainMaterial`, `loadGrass`, `loadGroundTex` and `loadAutoGroundTex` (see [Resource modifiers](scripting.md#resource-modifiers)). At runtime the loaded resources are in [`api.res.climateRep`](../api/api/res.md) (symbol: `Res.climateRep`, see [symbol index](../index.tsv)), [`environmentRep`](../api/api/res.md) (symbol: `Res.environmentRep`, see [symbol index](../index.tsv)), [`terrainMaterialTypeRep`](../api/api/res.md) (symbol: `Res.terrainMaterialTypeRep`, see [symbol index](../index.tsv)), [`groundTextureRep`](../api/api/res.md) (symbol: `Res.groundTextureRep`, see [symbol index](../index.tsv)) and [`autoGroundTexRep`](../api/api/res.md) (symbol: `Res.autoGroundTexRep`, see [symbol index](../index.tsv)).

## Climates

A climate is a `.clima.lua` file whose `data()` returns a [`ClimateDesc`](../api/api/type.md) (symbol: `ClimateDesc`, see [symbol index](../index.tsv)). The [wiki page on climates](https://wiki.transportfever3.com/doku.php?id=modding:environment:climates) describes the fields. From `base/content/climates/climates/temperate/temperate.clima.lua` (shortened, audio and data maps follow below):

```lua
function data()
return { 
		nodeTree = "temperate_clima.tree",
		audio = { environment = { ... } },
		dataMaps = { biomes = { ... }, layers = { ... } },
		desc = { 
			description = _("Mixed Forest Vegetation"),
			name = _("Temperate"),
		},
		category = "temperate.clima",
		economy = "/economy/temperate.eco",
		environment = "temperate.env",
		defaultNames = "/names/europe/europe.names",
		groundTextures = { 
			waterGround = "/climates/shared/gtex/water_ground.gtex",
			waterShore = "/climates/shared/gtex/water_shore.gtex",
		},
		id = "temperate",
		mapColoring = { 
			ambientColor = { 0.804, 0.859, 1, },
			levels = {
				{ color = { 0.365, 0.439, 0.259, }, height = 0, },
				{ color = { 0.251, 0.306, 0.196, }, height = 100, },
				{ color = { 0.949, 0.922, 0.863, }, height = 550, },
			},
			sunColor = { 1, 0.961, 0.941, },
			treeColor = { 0.094, 0.141, 0.063, },
			waterColor0 = { 0.392, 0.529, 0.620, },
			waterColor1 = { 0.235, 0.314, 0.357, },
		},
		order = 10,
		skirt = { 
			colorTex = { fileName = "/climates/shared/skirt_color.dds", scaleDownAllowed = false, type = "TWOD", wrapS = "REPEAT", },
			detailTex = { fileName = "/climates/shared/skirt_detail.dds", scaleDownAllowed = false, type = "TWOD", wrapS = "REPEAT", wrapT = "REPEAT", },
			waterTex = { fileName = "/climates/shared/water_skirt.dds", scaleDownAllowed = false, type = "TWOD", },
		},
		updateScript = { fileName = "", params = { }, },
	}
end
```

This is a base file, so its `/...` paths point into the base game. A mod climate writes `::/economy/dry.eco` for a base economy (mission 07) and `urbangames_campaign_mission_04::/economy/temperate.eco` for an economy the mod ships itself (mission 04).

| Field | Meaning |
|---|---|
| `id` | technical identifier |
| `desc` | `name` and `description`, translatable. The wiki also lists an `icon` (600×400 px at @2 resolution, shown in menu dialogs); none of the four base climates sets one. |
| `order` | sort order in the dialogs. The base climates use 10 (temperate), 20 (subarctic), 30 (tropical) and 40 (dry). |
| `category` | the tag that filters content. Models, terrain materials and other resources list it in `categoryList.categories` to be available in this climate. |
| `defaultNames`, `economy`, `environment` | default name list, economy (`.eco`, see [Economies](game-mechanics.md#economies)) and environment (`.env`) for new games |
| `nodeTree` | node tree that distributes terrain materials, see [below](#terrain-material-distribution) |
| `audio.environment` | environment sounds, see [below](#environment-sounds) |
| `mapColoring` | colours of the map preview in the generator dialogs: height `levels` (interpolated between), `waterColor0` (shallow) and `waterColor1` (deep), `ambientColor`, `sunColor`. The base files also set `treeColor`, which the wiki doesn't list. |
| `groundTextures` | `.gtex` ground textures for the ground under water (`waterGround`) and the shoreline (`waterShore`) |
| `skirt` | textures for the edge faces of the world: `colorTex`, `detailTex`, `waterTex` |
| `dataMaps` | biomes and layers that the terrain generator exports, see below. Not on the wiki. |

The wiki spells the skirt water texture `waterText`; all four base climates use `waterTex`. The wiki calls the skirt entries texture maps; in the files each one is a texture table with `fileName`, `type` and wrap modes. `subarctic.clima.lua` also sets `visible = true`, which [`ClimateDesc.visible`](../api/api/type.md) (symbol: `ClimateDesc.visible`, see [symbol index](../index.tsv)) describes as the visibility in the selection screen.

`dataMaps` ([`ClimateDesc.DataMap`](../api/api/type.md) (symbol: `ClimateDesc.DataMap`, see [symbol index](../index.tsv))) declares the biomes and layers of the climate. The API notes that biomes exclude each other while layers can overlap each other and the biomes. Temperate has five biomes (`biome0` to `biome4`) and two layers:

```lua
dataMaps = {
	biomes = {
		{ key = "biome0", displayName = _("Biome 0"), color = { 0, 0.63, 0.9 }, id = 0, },
		-- biome1 to biome4
	},
	layers = {
		{ key = "mountains", displayName = _("Mountains"), color = { 0.9, 0.5, 0.6 }, id = 1 },
		{ key = "rivers", displayName = _("Rivers"), color = { 0, 0.63, 0.9 }, id = 0 },
	}
},
```

The generator trees write these with `output_biomes` nodes (`type = "Biomes"`, or `type = "Layer"` with `layerKey = "rivers"` or `"mountains"`), and the import generators read them back with `input_biomes`.

### Environment sounds

Each entry in `audio.environment` has a `sounds` list and the properties `refDist` (distance of normal loudness), `height` (emitter height above ground), `layer`, `altitudeRange` (camera height range) and `ambient`. The audio system prefers the lowest `layer` that has a playing sound and mixes the sounds within one layer. `ambient = true` marks a fallback that plays when no other sound does. From `temperate.clima.lua`:

```lua
forest = {
	sounds = { 
		{ tags = {"day", "clear"}, file = "::/climates/sound/environment/nature/forest.wav" },
		{ tags = {"night", "clear"}, file = "::/climates/sound/environment/nature/forest_night.wav" },
		{ tags = {"day", "rain"}, file = "::/climates/sound/environment/nature/forest_rain.wav" },
		{ tags = {"night", "rain"}, file = "::/climates/sound/environment/nature/forest_night_rain.wav" },
	},
	refDist = 10.0,
	height = 0.0,
	layer = 2,
},
```

The keys used are `water`, `forest`, `lowland`, `mountain`, `crowd`, `traffic`, `atmosphere`, `rain`, and `residential`, `commercial` and `industrial` (each a list of four entries, one per building level). The base climate uses `layer = -1` for `atmosphere` and `rain`, which the wiki describes as always active (wind at 4,000 m, rain at the camera). Tags select one sound from the list: `day` (6:00 to 19:00 game time), `night`, `clear`, `rain`, and the eras `year1900` (1900 to 1919), `year1920` (1920 to 1989) and `year1990`. The wiki writes the rain tag as `"rain:"`; the base climates use `"rain"`.

`terrainMasks` adds sounds for masks that the terrain generator saved with a data output node. The wiki says it holds a list of structs; the base file uses a table keyed by the mask key:

```lua
terrainMasks = {
	river_mask = {
		sounds = { 
			{ file = "::/climates/sound/environment/nature/river.wav" }
		},
		refDist = 10.0,
		height = 0.0,
		layer = 0,
		altitudeRange = { -4000.0, 0.0 },
	},
}
```

`river_mask` is the key of an `output_map_data` node in `temperate_gen.tree.lua`.

### Terrain material distribution

The climate's `nodeTree` runs after every terrain change and recalculates the default terrain materials of the map. It is edited in the [Terrain Generator Editor](#terrain-generator-editor) in Terrain Material Distribution mode. In the `.tree.lua` file each node is a table with `layerType`, `name`, `inputs`, `params`, `color` and `position`. The node names the wiki uses in the editor map to these layer types:

| Editor node (wiki) | `layerType` in the file | Parameters |
|---|---|---|
| Heightmap input | `height_map_input` | none |
| Map data | `input_data` | none; outputs seed, map bounds |
| Data input | `input_map_data` | `key` of a mask saved by the generator |
| Material output | `material_index_output` | `order`, `key`, `materialType`, `dither` |
| Material background output | `material_background_index_output` | `materialType` |

The wiki notes that the water level output of the map data node is not provided yet. From `temperate_clima.tree.lua`, the background and one material:

```lua
{ 
	layerType = "material_background_index_output",
	name = "layer_background",
	params = { 
		materialType = "::/terrain/materials/grass_light_green/grass_light_green.tmat",
	},
},
{ 
	inputs = { 
		material = { key = "out", nodeName = "New Multiply map  #0", },
	},
	layerType = "material_index_output",
	name = "grass_brown",
	params = { 
		dither = true,
		key = "grass brown",
		materialType = "::/terrain/materials/grass_brown/grass_brown.tmat",
		order = 20,
	},
},
```

The `material` input is a map with values from 0 to 1 that says where the material is drawn. `dither` mixes the material with its neighbours. The temperate tree has twelve material outputs and reads the masks `biome0` to `biome4`, `forest_mask`, `biome4_mountains` and `biome4_no_mountains` from the generator.

## Terrain generators

A terrain generator is a `.gen.lua` file ([`TerrainGeneratorDesc`](../api/api/type.md) (symbol: `TerrainGeneratorDesc`, see [symbol index](../index.tsv))) that points at a node tree. The tree defines biome regions, builds the heightmap and places trees and rocks ([wiki](https://wiki.transportfever3.com/doku.php?id=modding:environment:terraingenerators)). From `base/content/climates/climates/temperate/temperate.gen.lua` (two of three parameters left out):

```lua
function data()
return { 
		nodeTree = "temperate_gen.tree",
		climate = "temperate.clima",
		desc = { 
			cargoTypeSet = { ... },
			name = _("Temperate"),
		},
		editorOnly = false,
		order = 0,
		params = {
			{ 
				defaultIndex = 3,
				key = "mountains",
				name = _("Mountains"),
				tooltip = _("Adjust the number and height of mountains on the map."),
				uiType = "Slider",
				values = { _("Sparse"), _("Scattered"), _("Medium"), _("Dense"), _("Packed"), },
			},
			-- "oceans" (Lakes), "water" (Rivers)
		},
		previewSeed = "yQKiK",
		updateScript = { fileName = "", params = { }, },
	}
end
```

`climate` names the climate the generator belongs to. `desc.name` is shown in the dropdown when a climate has more than one generator, sorted by `order`. `editorOnly = true` hides the generator from the main menu. `params` are [`ScriptParam`](../api/api/type.md) (symbol: `ScriptParam`, see [symbol index](../index.tsv)) entries, the type that [mod parameters](mod-structure.md#mod-parameters) use, and the tree reads them by key. They are written in Lua, so `defaultIndex` is 1-based: `defaultIndex = 3` selects Medium. `previewSeed` is the starting seed in the Terrain Generator Editor. The wiki example writes the references with extension (`"temperate.clima.lua"`, `"temperate_gen.tree.lua"`); the base files leave `.lua` out.

Every base climate also has an import generator. `temperate_import.gen.lua` differs from `temperate.gen.lua` in four fields:

```lua
nodeTree = "temperate_import_gen.tree",
name = _("Temperate (Import)"),
editorOnly = true,
isImportGenerator = true,
order = 2,
```

[`TerrainGeneratorDesc.isImportGenerator`](../api/api/type.md) (symbol: `TerrainGeneratorDesc.isImportGenerator`, see [symbol index](../index.tsv)) marks "a generator used when importing biomes/heightmap". The import trees have no heightmap output; they start from `height_map_input` and `input_biomes` nodes instead. The wiki does not describe import generators.

### Generator node tree

| Editor node (wiki) | `layerType` in the file | Parameters |
|---|---|---|
| Map data | `input_data` | none; outputs seed, map bounds |
| Input parameter | `param_number` | `key`, `dummy` (placeholder value in the editor) |
| Data output | `output_map_data` | `key`, `resolutionx`, `resolutiony`, `persistent` |
| Heightmap output | `height_map_output` | none; the input map is in metres |
| Asset output | `assets_output` | `assets` and `forest` mappings |

The wiki calls the resolution parameters "x-Resolution scale" and "y-Resolution scale", metres per pixel of the mask. The base trees use 32 and set `persistent = true`, which the wiki doesn't explain. Data outputs are saved with the savegame and are read by the climate's material tree and by `terrainMasks` sounds. From `temperate_gen.tree.lua`:

```lua
{ 
	inputs = { 
		output = { key = "out", nodeName = "New Multiply map  #0", },
	},
	layerType = "output_map_data",
	name = "biome 2 mask output",
	params = { 
		key = "biome2",
		persistent = true,
		resolutionx = 32,
		resolutiony = 32,
	},
},
```

The forest mask (`forest_mask` in all base generators) is also used for reforestation: trees grow back over time in forest areas after the player removed what had cleared them. The base game runs this from `base/content/terrain/reforestation.gs.lua` and `reforestation.script.tl`, which call [`makeReforestationProposal`](../api/api/engine/util.md) (symbol: `UtilProposal.makeReforestationProposal`, see [symbol index](../index.tsv)).

The asset output maps the values of two maps to tags. Trees and rocks of the current climate whose `categoryList.categories` contains the tag are picked at random. From `temperate_gen.tree.lua`:

```lua
{ 
	inputs = { 
		assets = { key = "out", nodeName = "New Constant map #275", },
		forest = { key = "out", nodeName = "add temperate river trees", },
	},
	layerType = "assets_output",
	name = "assets_out",
	params = { 
		assets = {
			{ 1, "granite", },
		},
		forest = {
			{ 5, "beech_weeping", },
			{ 7, "temperate_conifer", },
			{ 2, "shrub", },
			{ 255, "temperate_single", },
			{ 3, "temperate_hills", },
			{ 4, "broadleaf", },
			{ 6, "temperate_plains", },
			{ 8, "temperate_river", },
		},
	},
},
```

The temperate tree reads the parameters `oceans`, `water`, `mountains` and also `forest`. `forest` is not in the `params` of `temperate.gen.lua`, and the files don't show where its value comes from. The temperate tree names the node behind it "Humidity".

Besides the built-in layer types, a tree can use script nodes such as `gui/node_editor/remap_number.node` and `gui/node_editor/split_interval.node`. They are defined in `base/content/gui/gui/node_editor/*.node.lua` (18 files) with inputs, outputs, an editor category and an `applyScript`. From `split_interval.node.lua`:

```lua
function data()
return {
	inputs = {
		{ key = "interval", type = "Point", displayName ="Interval", desc = "Interval with Lower and Upper Bound", optional = true },
		{ key = "percentage", type = "Float", displayName ="Percentage", desc = "Portion of the First Part between 0 and 1", optional = true },
	},
	outputs = {
		{ key = "interval_1", type = "Point", displayName ="Interval 1", desc = "Interval from Lower to Middle Bound"  },
		{ key = "interval_2", type = "Point", displayName ="Interval 2", desc = "Interval from Middle to Upper Bound"  },
	},
	params = { },
	def = {
		displayName = "Split Interval",
		category = "point_feat",
		description = "Split one Interval in two Weighted Intervals",
		order = 540
	},
	withSeed = false,
	applyScript = {
		fileName = "::/gui/node_editor/layer_nodes.script@splitInterval.applyFn",
		params = {}
	},
}
end
```

The apply functions in `layer_nodes.script.tl` receive the node's parameters and a table of inputs (each with `point`, `value`, `pointCloud` or `map`) and return the outputs. The river and ridge nodes there use `climates/gen/mapgenutil.tl` and `rivermapgenutil.tl`, whose types are in [map_gen_util_serialized](../base/climates/gen/map_gen_util_serialized.md), for example [`RiverBuilderConfig`](../base/climates/gen/map_gen_util_serialized.md) (symbol: `RiverBuilderConfig`, see [symbol index](../index.tsv)). Neither the wiki nor the files say whether mods can add their own `.node.lua` files.

## Terrain generator editor

The Terrain Generator Editor is a node editor inside the Map Editor. It edits generator trees and material distribution trees ([wiki](https://wiki.transportfever3.com/doku.php?id=modding:tools:terraingeneratoreditor)). The wiki says it is switched on with `experimentalTerrainGeneratorEditor = true` in `settings.lua`; the game files don't contain that key.

The head bar switches between the Terrain Generator and Terrain Material Distribution modes, opens and saves trees, undoes and redoes, checks the tree for the current mode (Validate), and has settings for the seed, the preview resolution and the parallelism of the preview. The board moves with the right mouse button or W, A, S, D, zooms with the wheel, and R resets the view. Ctrl+C and Ctrl+V copy nodes, Shift plus left click selects several.

Connectors are colour-coded by data type: blue for a number, red for a point (two numbers), green for a point cloud (a list of points) and yellow for a map. Some inputs can be set as a local parameter instead of being connected. Each node can generate a preview of the tree up to that node, show it, and export it as `.tga`. Nodes can be renamed and recoloured, which is where the `name` and `color` fields in the `.tree.lua` files come from. The node catalogue has the tabs Special (inputs, parameters, outputs), Numbers, Points, Point Clouds, Map Generators, Noise Maps, Map Operations 1 and 2, Combine Maps, and Ridges & Rivers.

### Terrain stamps

Stamps are the heightmaps of the "Roughen Terrain" tool in the build menu's Terrain tools. They are not part of the generator trees. The tool exists only in the Map Editor and in sandbox mode (`hasHeightMapTextureTool = params.isMapEditor or params.isSandboxMode` in `base/content/gui/gui/main/game.tl`). Each stamp is a generic resource of type `stamp` with a heightmap `.png` and a `.tga` icon next to it. `base/content/gui/gui/construction/shared/tex/stamps/desert_hill1.res.lua`:

```lua
function data()
	return {
		type = "stamp",
		data = {
			heightmap = resolve("desert_hill1.png"),
			icon = resolve("desert_hill1.tga")
		}
	}
end
```

The base game ships 11 stamps in that folder (`desert_hill1`, `temperate_noise`, `tropical_hill2` and so on). The tool collects them with `api.res.genericRep.getAllOfType("stamp")` in `construction_react_util.tl`, shows each `icon` as a button and passes the chosen `heightmap` to the terrain modifier. The `data` table is [`StampData`](../base/gui/construction/construction_react_util.md) (symbol: `StampData`, see [symbol index](../index.tsv)). A mod's own `stamp` resource should appear in the same list, since the tool reads every resource of the type; no shipped mod adds one to confirm it. The size and pixel format the heightmap needs are open questions: the `.png` files are listed in `base/_content.json` but not unpacked.

## Environments

An environment (`.env.lua`, [`Environment`](../api/api/type.md) (symbol: `Environment`, see [symbol index](../index.tsv))) sets the sun and moon, the atmosphere model, clouds, shadows, sky textures and water ([wiki](https://wiki.transportfever3.com/doku.php?id=modding:environment:environments)). The climate names its environment in `environment`. Each base climate has a current one and an `_old` variant, subarctic also a `subarctic2.env.lua`, and `base/content/model_editor/model_editor/environments/` has environments for the model editor. From `base/content/climates/climates/temperate/temperate.env.lua` (shortened):

```lua
function data()
return { 
		atmosphere = { 
			bottomRadius = 6360,
			atmosphereHeight = 100,
			miePhaseG = 0.8,
			mieScatteringScale = 0.00692128,
			rayScatteringCoeff = { 0.143, 0.38, 0.816, },
			rayScatteringScale = 0.0362366,
			sunIlluminance = 15.0,
			sunIntensity = 12500.0,
			sunSizeAngle = 0.9,
			cloudCoverage = 0.0,
			dayOfYear = 171,
			-- 25 more
		},
		light = { 
			ambientDayColor = { 0.047, 0.047, 0.047, },
			hdrContrastDay ={ 1.3, 0.95, 6.0 },
			hdrExposureDay = 1.0,
			hdrExposureNight = 2.0,
			latitude = 0.7068436145782470703125,
			refBrightness = 1.0,
			-- 18 more
		},
		name = _("Temperate"),
		shadows = { numCascades = 4, lambda = 0.5, ... },
		skyBox = { 
			environmentTex = { fileName = "sun.dds", scaleDownAllowed = false, type = "CUBE_MAP", },
			moonTex = { fileName = "/climates/shared/moon.tga", type = "TWOD", },
			starsTex = { fileName = "/climates/shared/stars.dds", type = "TWOD", wrapS = "MIRROR_REPEAT", wrapT = "MIRROR_REPEAT", },
			starsTexMask = { fileName = "/climates/shared/starsMask.tga", type = "TWOD", },
		},
		useSkyBoxOverwrite = false,
		water = { 
			depthColorTex = "depthColor.dds",
			extinctionCoeff = 0.05,
			normalScale = 0.4,
			normalTex = "/climates/shared/water_normal.dds",
			particleDensity = 1,
			preferLowFreq = false,
			roughness = 0.5,
			waveFoamTex = "",
		},
	}
end
```

The wiki groups the `atmosphere` fields like this:

| Group | Fields | Notes from the wiki |
|---|---|---|
| simulation | `cloudCoverage`, `dayOfYear`, `timeOfDaySec` | `cloudCoverage` from 0.0 to 0.5, rain from about 0.45; `dayOfYear` 1 to 365 |
| sun and moon | `sunBloom`, `sunBloomScale`, `sunIlluminance`, `sunIntensity`, `sunSizeAngle`, `moonIlluminance`, `moonIntensity` | `sunIntensity` is the direct light (0 to 100000), `sunIlluminance` the indirect light (0 to 100) |
| clouds | `cloudBase`, `cloudTop`, `cloudSpeed`, `cloudPhase`, `cloudScattering` | heights in metres |
| mie (haze, fog) | `miePhaseG`, `mieScatteringScale`, `mieAbsorptionScale`, `mieScaleHeight` | dry climates have little haze |
| rayleigh (blue sky, red sunset) | `rayScatteringCoeff`, `rayScatteringScale`, `rayScaleHeight` | |
| ozone | `absorptionCoeff`, `absorptionScale` | absorbs, does not scatter |
| advanced | `bottomRadius`, `atmosphereHeight`, `heightOffset`, `fogScaleLow`, `fogScaleHigh` | radius and heights in km; fog scales 0.1 to 25 |

The base file sets more fields than the wiki lists: overcast variants (`mieAbsorptionScaleOvercast`, `miePhaseGOvercast`, `mieScatteringScaleOvercast`), `mieAbsorptionCoeff`, `mieScatteringCoeff`, `sunsetBoostScale`, `sunsetBoostParam`, `sunsetOffset`, `envTexScale`, `skylightScale` and `mirrorSky`. It leaves out the cloud fields except `cloudCoverage`.

In `light`, `latitude` (radians) sets the path of the sun and `refBrightness` the overall brightness. The wiki describes `hdrContrastDay` as a number; `temperate.env.lua` sets it to three numbers, `{ 1.3, 0.95, 6.0 }`, and also has transition, overcast and curve fields (`hdrExposureOvercastCurve`, `hdrTargetCurve`, `hdrMax`, `globalEmissiveScale` and others) that the wiki doesn't cover.

In `water`, `normalTex` and `normalScale` make the waves (0 is still water), `preferLowFreq` calms the movement, `roughness` reduces reflections, `depthColorTex` colours the depth, `extinctionCoeff` sets the clarity (0 clear, 1 opaque), and a `waveFoamTex` adds animated foam on the shore. The wiki recommends trying values with the in-game debug tools.

## Terrain materials

Terrain materials (`.tmat.lua`, [`TerrainMaterialType`](../api/api/type.md) (symbol: `TerrainMaterialType`, see [symbol index](../index.tsv))) colour the ground ([wiki](https://wiki.transportfever3.com/doku.php?id=modding:environment:terrainmaterials)). The base game has 86 of them in `base/content/terrain/materials/`. From `base/content/terrain/materials/grass_cutted_01/grass_cutted_01/grass_cutted_01.tmat.lua`:

```lua
function data()
return {
	name = _("Grass Cutted Light"),
	
	icon = "grass_cutted_01.tga",
	previewIcon = "grass_cutted_01_preview.tga",
	detailColorTexture = "tex/grass_cutted_01_albedo.dds",
	detailMetalGlossAoHTexture = "tex/grass_cutted_01_metal_gloss_ao_h.dds",
	detailNormalTexture = "tex/grass_cutted_01_nrml.dds",
	overlayTexture = "/terrain/materials/shared/tex/overlay_0.dds",
	desc = _("Paint the ground with the materials brush."),
	
	grass = "grass_cutted_01.grass",

	detailSize = 1 / 32,
	overlaySize = 0.03,
	overlayStrength = 0.25,
	hOffset = 0.3,

	categories = {"grass"},
	order = 635,
	priority = 1
,	categoryList = {
		categories = { "all.clima", }
	},
}
end
```

The wiki shows the same material in a different form: the menu data in a `menuCategory` struct (`category = "landscaping_ground"`, `filterCategories = {"grass"}`, `order = 635`) and the texts in a `description` struct. All 86 base materials use the flat form above, with `name`, `desc`, `icon`, `previewIcon`, `categories` and `order` at the top level. The reference type has both the flat fields and [`menuCategory`](../api/api/type.md) (symbol: `TerrainMaterialType.menuCategory`, see [symbol index](../index.tsv)).

| Field | Meaning |
|---|---|
| `detailColorTexture`, `detailNormalTexture` | colour and normal map |
| `detailMetalGlossAoHTexture` | metal, gloss and ambient occlusion; the alpha channel is a heightmap |
| `overlayTexture`, `overlaySize`, `overlayStrength` | overlay that breaks up the tiling of the detail texture |
| `detailSize` | scale of the detail texture; 48 of the 86 base materials use `1 / 32` |
| `hOffset` | added to the texture height; at the border between two materials the higher one shows |
| `priority` | the highest priority is painted on top (asphalt over dirt); ignored when the player paints by hand |
| `grass` | `.grass` resource with grass sprites and small plants |
| `categoryList.categories` | climates the material is available in; 30 base materials use `"all.clima"` |

The wiki requires 4096×4096 pixels for the detail textures and advises using few extra textures for that reason. It also limits `name` to 32 and the description to 320 characters, with a 240×150 px `@2x.tga` icon and a 720×405 px preview. `priority` is set in 33 base materials, with values from -1 (one material) to 1000000 (the asphalts and others). The reference type has `hScale` but no `hOffset`; four base materials use `hOffset`, none uses `hScale`.

### Grass

A `.grass.lua` file adds 2D grass sprites and small plant models on top of a material. From `base/content/terrain/materials/grass_green/grass_green/grass_green.grass.lua`:

```lua
function data()
return {
	colorTexture = "tex/grass_green_albedo_opacity.dds",
	metalGlossAoTexture = "/terrain/materials/grass_light_green/tex/grass_light_green_metal_gloss_ao.dds",		-- same as light green
	normalTexture = "/terrain/materials/grass_light_green/tex/grass_light_green_normal.dds",					-- same as light green
	translucencyTexture = "tex/grass_green_translucency.dds",

	width = 1.0,
	height = 1.0,
	density = 6.0, -- stems per square meter (range allowed 0 to 16)
	lodDistance = 300.0,
	
	assets = {
        {
            filePath = "/assets/vegetation/plants/flower_01/flower_01.mdl",
            density = 0.08,
			scaleMin = 1.1,
			scaleMax = 1.3,
        },
        -- flower_02, flower_06
    }
}
end
```

`width` and `height` are the sprite size in metres, `density` the sprites per square metre (0 to 16) and `lodDistance` the distance up to which they are drawn. `translucencyTexture` sets how much sun shines through thin parts of the plant. Each `assets` entry places a model with its own density and a random scale between `scaleMin` and `scaleMax`. Grass sprites can be switched off by setting Grass to Off under Settings > Graphics.

## Landscape assets

Trees and rocks are ordinary models with an extra metadata struct: an empty `tree = { }` or `rock = { }` ([wiki](https://wiki.transportfever3.com/doku.php?id=modding:environment:landscapeassets)). Both kinds also appear in the mixed brush tool. From `base/content/assets/vegetation/trees/trees/brleaf_01/brleaf_01.mdl` (metadata only):

```lua
metadata = { 
	categoryList = { 
		categories = { "broadleaf", "large", "temperate_single", "temperate_hills", "town", "temperate.clima", },
	},
	description = { 
		description = _("ASSET_VEGETATION_TREES_BRLEAF_01_DESCRIPTION"),
		icon = "brleaf_01.tga",
		name = _("ASSET_VEGETATION_TREES_BRLEAF_01_NAME"),
		previewIcon = "/assets/vegetation/tmp_vegetation.tga",
	},
	emissions = { 
		noise = { },
		pollution = { idle = -0.1, },
	},
	menuCategory = { 
		categories = {
			{ category = "landscaping_vegetation", filterCategories = { "tree", }, order = 200, },
		},
	},
	soundConfig = { 
		builderAudioRes = "/gui/construction/sound/buildoze_tree.builder_audio",
		soundSet = { name = "", },
	},
	tree = { },
},
```

`categoryList.categories` does two jobs. The entry that matches a climate's `category` (`temperate.clima`) makes the tree available in that climate only. The other entries are the tags that the generator's asset output picks by: `broadleaf` is forest value 4 and `temperate_single` value 255 in `temperate_gen.tree.lua`. `menuCategory` puts the asset into the Landscaping menu ([`ModelMetadata.MenuCategory`](../api/api/type.md) (symbol: `ModelMetadata.MenuCategory`, see [symbol index](../index.tsv))). The trees also have a negative idle pollution emission. [`ModelMetadata.Tree`](../api/api/type.md) (symbol: `ModelMetadata.Tree`, see [symbol index](../index.tsv)) has a `reputationFactor`, which `town_util.tl` reads.

### Auto ground textures

The ground around trees and rocks gets a texture that depends on how many of them stand close together. An `.agt.lua` file ([`AutoGroundTex`](../api/api/type.md) (symbol: `AutoGroundTex`, see [symbol index](../index.tsv))) defines one rule, and a model joins it through `autoGroundTex.categories`. From `base/content/assets/vegetation/shared/shared/gtex/tree_1.agt.lua`:

```lua
function data()
return {
	category = "auto.model.tree",
	gridSize = 18.0,
	countFrom = 2,
	countTo = 5,
	groundTex = "forest_floor.gtex",
	individual = true,
	individualCategory = "forestTree",
}
end
```

`category` is `"auto.model.tree"` or `"auto.model.rock"`. The models are counted in grid cells of `gridSize` metres, and the texture shows in cells with `countFrom` to `countTo` models. With `individual = true` the texture goes around single models (the roots of a tree) instead of patches between them, and models select it by naming `individualCategory` in their `autoGroundTex.categories`. The tree `eu_lnd_01.mdl` has `autoGroundTex = { categories = { "forestTree", }, }`, the rock `cr_rck_01.mdl` has `"rockCrackedMedium"`. The rock rules use `countTo = -1`, for example `base/content/assets/rocks/shared/shared/gtex/rock_0_medium.agt.lua`; the wiki doesn't explain the value, and it probably means no upper limit.

## Animals

Animals are models with a `metadata.animal` struct ([`ModelMetadata.Animal`](../api/api/type.md) (symbol: `ModelMetadata.Animal`, see [symbol index](../index.tsv))) and a `cameraConfig` for the first-person view ([wiki](https://wiki.transportfever3.com/doku.php?id=modding:environment:animals)). The base game has them in `base/content/animal/animal/`, among them `wildlife_fox`, `wildlife_bear`, `wildlife_deer`, `wolf`, `cougar`, `bird_eagle`, `bird_crane`, `bird_gull`, `fish_salmon`, `cr_fish` and farm animals (`cow`, `sheep`, `horse`). From `base/content/animal/animal/wildlife_fox/wildlife_fox.mdl` (metadata only, three movement types left out):

```lua
metadata = { 
	animal = { 
		config = { 
			density = 1,
			fish = false,
			idleTime = 10,
			predator = false,
			targetDistance = 64,
		},
		flockFormationFn = { fileName = "", params = { }, },
		flockName = "",
		movement = { 
			alignToTerrain = true,
			heightOffset = 0,
			rollWhileTurning = false,
			stickToWater = false,
			wobbleWhenIdle = false,
		},
		movementTypes = {
			{ 
				angularSpeed = 60,
				description = _("Wandering"),
				despawnAtEnd = false,
				duration = 4,
				eventName = "walk",
				playOnce = false,
				playRandomly = true,
				playWhenSelected = false,
				speed = 1.9,
			},
			-- idle, run, sitdown (playWhenSelected), die (despawnAtEnd)
		},
		suitableAreas = { 
			bias = 5,
			height = { mapFrom = { 400, 2000, }, mapTo = { 0, -1000, }, },
			noise = { mapFrom = { 60, 150, }, mapTo = { 0, -1000, }, },
			scores = { 
				civilisation = -1000, fish = 0, forest = -4, predator = 0,
				ship = -1000, shore = -1000, water = -1000,
			},
			slope = { mapFrom = { 0.3, 2, }, mapTo = { 0, -1000, }, },
			waterDepth = { mapFrom = { 0, 1, }, mapTo = { 0, 0, }, },
		},
	},
	cameraConfig = { 
		positions = {
			{ fov = 60, group = "RootNode", transf = { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 1, 0, 1, 1, }, },
		},
	},
	categoryList = { 
		categories = { "temperate.clima", "subarctic.clima", "tropical.clima", },
	},
	description = { name = _("Fox"), },
},
```

`config` holds `density`, `fish` (restricted to water), `idleTime` (time spent idle before moving on), `predator` (other animals avoid it, as deer avoid bears) and `targetDistance` (how far it looks for a new target). The wiki gives density 1 for land animals and birds and 4 for fish. `fish_salmon.mdl` has 4; the four tropical `cr_fish` models have 2.

`movement` decides how the animal follows the ground. Land animals have `alignToTerrain = true`. Fish and birds have it false, and their `heightOffset` is then measured from the water level: negative for fish (salmon: -1.6), positive for birds. Fish also set `stickToWater` and `wobbleWhenIdle`, birds `rollWhileTurning`.

Each `movementTypes` entry plays the animation named in `eventName` for `duration` at `speed` (m/s) and turns at `angularSpeed`. The animal's normal behaviour is a chain of the types with `playRandomly = true`. `playWhenSelected` plays when the player clicks the animal (the fox sits down, the bear roars), `playOnce` stops the loop, and `despawnAtEnd` removes the animal afterwards, which is how `die` works. The `description` is shown in the animal window. Common event names are `idle`, `idle_standing`, `walk`, `fly`, `swim`, `run`, `die`, and random or click events such as `roar`, `eat` or `sitdown`.

`suitableAreas` scores each spot. `bias` is a fixed offset, `scores` adds fixed values near civilisation, fish, forest, predators, ships, shores and water, and `height`, `noise`, `slope` and `waterDepth` map a range of values (`mapFrom`) to a range of scores (`mapTo`), clamped at both ends. An animal in a negative area tries to leave it and dies if it can't. The fox above avoids water, shores, towns and ships completely and scores negative above about 400 m. The wiki says the score maps can be inspected with the in-game debug tools.

Birds and fish can move in flocks. `flockName` is the translatable flock name and `flockFormationFn` points at a function without parameters that returns the member `positions` relative to the flock origin, plus optional `overrides` with other models per member. From `base/content/animal/animal/animal.script.lua`:

```lua
salmon = function()
	local f = {}
	local n = math.random(5,7)
	for i=1, n do
	  f[i] = { math.random(-2,2), math.random(-2,2), math.random(-3,0) }
	end
	return { positions = f }
end,
```

`fish_salmon.mdl` references it with `flockName = _("Salmon School")` and `fileName = "/animal/animal.script@flock.salmon"`. The `discus` function in the same file uses `overrides` to mix the four `cr_fish` colour variants. The `cr_fish` models also set `idForAchievement = "animal/cr_fish"`, so all four count as one animal for the spotted-animals achievement ([`ModelMetadata.Animal.idForAchievement`](../api/api/type.md) (symbol: `ModelMetadata.Animal.idForAchievement`, see [symbol index](../index.tsv))). The wiki's example has `version = 1`; the base animal models have `version = 2`.

Official wiki: [Climates](https://wiki.transportfever3.com/doku.php?id=modding:environment:climates), [Environments](https://wiki.transportfever3.com/doku.php?id=modding:environment:environments), [Terrain Materials](https://wiki.transportfever3.com/doku.php?id=modding:environment:terrainmaterials), [Terrain Generators](https://wiki.transportfever3.com/doku.php?id=modding:environment:terraingenerators), [Terrain Generator Editor](https://wiki.transportfever3.com/doku.php?id=modding:tools:terraingeneratoreditor), [Landscape Assets](https://wiki.transportfever3.com/doku.php?id=modding:environment:landscapeassets), [Animals](https://wiki.transportfever3.com/doku.php?id=modding:environment:animals).
