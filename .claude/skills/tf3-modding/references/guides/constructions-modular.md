
# Modular constructions

Modular constructions (rail stations, airports, harbours, warehouses, the Headquarters, and industries with their fields) can be changed after building: the player adds and removes modules in slots. The construction itself is an ordinary `.con.lua` with an update function, described on [Constructions](constructions.md). This page covers slots, modules, the order in which their scripts run, and templates, which give a new construction its first modules. A small construction with one module that you can start from is at the end, in [A minimal modular construction](#a-minimal-modular-construction). The official wiki covers the same ground on [Modular Constructions](https://wiki.transportfever3.com/doku.php?id=modding:constructions:modular) and [Construction Templates](https://wiki.transportfever3.com/doku.php?id=modding:constructions:templates).

## How the scripts run

Each time a modular construction is previewed or built, `construction.constructWithModules` in `base/content/scripts/scripts/construction/construction.script.tl` runs the scripts in this order:

1. The construction's `updateFn(captureParams, params)` builds the base result, including `result.slots`.
2. The construction's `cost` and `bulldozeCost` are multiplied by `costs.infrastructureCostScale` from the script params, which the base game sets from the Infrastructure Costs setting (`base/content/base/base/mod.script.tl`). If `maintenanceCost` is 0 or unset, it becomes `cost / 6`, computed before the scaling. Module prices are added later and are not scaled.
3. If `params.modules` is set and `result.slots` exists, the modules are sorted by ascending slot id. For each one the slot with that id is looked up in `result.slots`. If there is none and `result.callInvalidModules` is not `true`, the module is skipped.
4. If the module has an update script, its `updateFn` is called with the result, the slot's transform and a tag `"__module_<slotId>"`. While it runs, `params.seed` is the construction's seed plus the slot id.
5. Right after that call, `modulesutil.addCosts(result, module)` adds the module's `metadata.price`, `metadata.maintenanceCost` and `metadata.bulldozePrice` to the result.
6. After the last module, `result.terminateConstructionHook()` runs if the construction set one. Like steps 3 to 5, this only happens when `params.modules` and `result.slots` both exist.

The base config's `constructionScript` points at `constructWithModules` (`base/content/base/base/mod.script.tl`). The config has no separate script for constructions without modules, so the game most likely takes this path for all of them, cost scaling included.

Because modules run after the construction and in slot id order, a module sees the slots and data that the construction and all earlier modules put into `result`, and it can add new slots for later modules. Constructions also store their own helper functions in `result` for the modules to call, as the rail station does with `GetCoord` and `GetRoofAt`.

## The construction's update function

Next to the player's parameters, `params.modules` maps each slot id to the module placed there ([`ModuleUpdateFnParams.Module`](../base/scripts/construction/construction_util_serialized.md) (symbol: `ConstructionUtilSerialized.ModuleUpdateFnParams.Module`, see [symbol index](../index.tsv))):

```lua
modules = {
	[10001000] = {
		metadata = {},
		name = "airfield_main_building.module",
		updateScript = { fileName = "", params = {} },
		variant = 0,
	},
},
```

`metadata` is a copy of the module's `metadata` table, `name` the module file and `updateScript` its update function. `variant` changes when the player presses M and N while placing a module and can be negative; the default is `0`. The Headquarters uses it as a rotation in quarter turns (`base/content/landmarks/hq/hq/headquarter_addon.script.tl`).

`result.cost` is the cost of the construction alone; the game scales it in step 2 and adds the module prices in step 5 above.

### Slots

`result.slots` lists the slots that currently exist ([`Slot`](../base/scripts/construction/construction_util_serialized.md) (symbol: `ConstructionUtilSerialized.Slot`, see [symbol index](../index.tsv))):

| Field | Meaning |
|---|---|
| `id` | non-negative unique id; `params.modules` is keyed by it |
| `type` | only modules with the same `type` can be built here |
| `transf` | position relative to the construction origin |
| `spacing` | size of the slot around `transf`, `{ -x, x, -y, y }` |
| `height` | height above `transf`; with `spacing` used for selection and bulldozing |
| `shape` | marker symbol: `0` square, `1` triangle, `2` transverse rectangle, `3` longitudinal rectangle |
| `autofill` | slot counts as filled when an industry is first built (fields and satellites) |
| `alignToTerrain` | slot follows the terrain height |
| `replaceable` | a module of the same type can replace the one in the slot |

The wiki and the shipped code (`base/content/industries/fieldutil.lua`) write `autofill`; the Teal definition spells it `autoFill`.

Many base constructions encode a grid position and the slot type into the id. The warehouse, from `base/content/warehouses/warehouses/warehouse.script.lua`:

```lua
local EncodeSlotId = function(coord, type)
    return typesNum * (coordNum * (coord.y + coordOffset) + (coord.x + coordOffset)) + type2id[type]
end
```

and creates its slots in `updateFn` with `table.insert(result.slots, { id = ..., transf = ..., type = type, spacing = {...}, height = ..., replaceable = true })`.

`result.slotConfig` sets limits per slot type ([`SlotConfig`](../base/scripts/construction/construction_util_serialized.md) (symbol: `ConstructionUtilSerialized.SlotConfig`, see [symbol index](../index.tsv))): `maxModules` is the most modules of that type in the whole construction, `message` the text shown on the module in the menu when it can't be built, and `skipCollisionCheck = true` stops the slot markers turning red when something is in the way (the module's colliders still count). The wiki gives `maxModules = -2` a special meaning: the module button is enabled while `message` is empty and disabled when a message is set. The airport uses this to require a second runway before terminal B (`base/content/stations/air/air/airport.script.lua`):

```lua
result.slotConfig = {
	["2nd_runway"] = { maxModules = -1, message = "" },
	terminal_B = {
		maxModules = -2,
		message = hasSecondRunway and "" or _("You Need a 2nd Runway"),
	},
}
```

`result.callInvalidModules = true` lets modules run even if their slot id is not in `result.slots`; their `transform` is then `nil`.

## Modules

A module is a `.module.lua` file (121 in the base game) described by [`ModuleDesc`](../api/api/type.md) (symbol: `ModuleDesc`, see [symbol index](../index.tsv)). From `base/content/warehouses/warehouses/wh_bulk.module.lua` (shortened):

```lua
function data()

return {
	menuCategory = {
		categories = {
			{ category = "modules_warehouse", order = 100, },
		},
	},
	description = {
		name = _("WAREHOUSE_BULK_NAME"),
		description = _("WAREHOUSE_BULK_DESCRIPTION"),
		icon = "::/warehouses/wh_bulk_module.tga",
	}, 
	availability = { yearFrom = 1850, yearTo = 0, },
	type = "warehouse_large",
	metadata = {
		type = "bulk",
		specialization = "BULK",
		price = 300000,
		maintenanceCost = 50000,
	},
	updateScript = {
		fileName = "modules.script@bulk.updateFn",
		params = {}
	},
	builderAudioRes = "::/gui/construction/sound/buildoze_module.builder_audio",
	getModelsScript = {
		fileName = "modules.script@bulk.getModelsFn",
		params = {}
	},
}

end
```

| Field | Meaning |
|---|---|
| `description`, `availability`, `menuCategory` | as for constructions; module tabs are the `modules_*` categories (see [build menu](constructions.md#the-build-menu)) |
| `type` | "a module of a specific type can be built only on slots with the same type" (definition comment) |
| `metadata` | free data for the construction and other modules; `price`, `maintenanceCost` and `bulldozePrice` are added to the costs |
| `autoRemovable` | removed when something else is built over it, as for farm fields (default `false`) |
| `updateScript` | "Script function that gets called once per build module after the updateScript of the construction has run" |
| `getModelsScript` | "Script function used to generate module preview when building with module builder" |
| `builderAudioRes` | the [builder audio set](other-content.md#builder-audio-sets) played while building; all 121 base modules use `::/gui/construction/sound/buildoze_module.builder_audio` (the file name is spelled that way) |

All 121 base modules set `menuCategory`; without it the module has no tab in the module menu.

The wiki's module example also has `buildMode = "SINGLE"`. That field is not in `ModuleDesc` and no shipped module uses it.

### getModelsFn

`getModelsFn(captureParams)` returns the models that follow the mouse cursor while the module is not over a slot: a list of `{ id = "<model>.mdl", transf = ... }`. If the module's models are placed relative to a slot that is offset from the construction, shift them back here. The wiki's roundhouse example returns its three shed models with `transf.transl(vec3.new(-30, 0, 0))` because its slots sit 30 m from the turntable centre.

### updateFn

The game calls the module's update function with seven arguments:

| Argument | Content |
|---|---|
| `captureParams` | the `params` of the module's `updateScript` reference |
| `result` | the result of the construction and all modules before this one |
| `transform` | the slot transform; apply it to everything placed relative to the slot. `nil` with `callInvalidModules` |
| `tag` | `"__module_<slotId>"`; put it on the module's models so they are removed with the module |
| `slotId` | the slot id |
| `addModelFn` | `addModel(name, transf, tag?, models?)`: multiplies `transf` with the slot transform, sets the tag and appends to `models` (default `result.models`); returns the index |
| `constrParams` | the `params` the construction's update function got, including `modules` |

Trailing arguments can be left out. The wiki says the function should return `result`; `constructWithModules` ignores the return value, so what counts is that the module changes `result` in place. The wiki's hall roof example for the rail station declares an eighth argument `params`, which `constructWithModules` does not pass.

A short version from the wiki, adding a hangar model and a ground face:

```lua
updateFn = function(captureParams, result, transform, tag)
	local sub = result.subconstructions[1]
	sub.models[#sub.models + 1] = { id = "hangar.mdl", transf = transform, tag = tag }

	local faces = { {-25.0, -25.0, 0.0, 1.0}, {25.0, -25.0, 0.0, 1.0}, ... }
	modulesutil.TransformFaces(transform, faces)
	sub.groundFaces[#sub.groundFaces + 1] = {
		face = faces,
		modes = { { type = "FILL", key = "airfield_hangar.gtex.lua" } },
	}
end,
```

In the warehouse modules the signature is `(captureParams, result, transform, tag, slotId, addModelFn, params)`, and they add models and new slots to the `result` that the construction built.

Modules can react to their neighbours through `constrParams.modules`. The wiki's roundhouse example computes the neighbour slot ids from `slotId` and adds a side wall only where the neighbour is not a shed:

```lua
local moduleLeft = constrParams.modules[leftIndex]
if not (moduleLeft ~= nil and moduleLeft.metadata ~= nil and moduleLeft.metadata.type == "shed") then
	addModel("shed_wall_left.mdl", transf.transl(vec3.new(-30, 0, 0)), tag, result.subconstructions[1].models)
end
```

A script file can hold the functions of several modules in a nested table. The module then names them as `"roundhouse_modules.script@shed.updateFn"`:

```lua
function data()
  return {
    shed = { updateFn = updateFnShed, getModelsFn = getModelsFnShed },
    empty = { updateFn = updateFnEmpty, getModelsFn = getModelsFnEmpty },
  }
end
```

A new module for a base construction only has to use a slot `type` of that construction. The wiki's hall roof example has `type = "passenger_platform_roof"`, the type of the vanilla platform roofs, so it can go wherever those go.

## The preprocess function

When a module is added or removed, a preprocess function updates the construction's parameters before the update runs. Neither `ConstructionDesc` nor the wiki documents it; 60 base `.con.lua` files set `preProcessScript`. The default one is in `base/content/base/base/base_config.lua`:

```lua
defaultPreprocessFn = function(captureParams, params, change)
	local modules = params.modules
	if change.added then
		modules[change.slotId] = change.module
	else
		modules[change.slotId] = nil
	end
	return params
end
```

`warehouse.con.lua` sets its own with `preProcessScript = { fileName = "warehouse.script@preProcessFn" }`, as do `airport.con.lua` and `modular_terminal.con.lua`.

## Upgrade script

For a modular construction, `upgradeFn(captureParams, params, slotId, constructionConfig)` gets the id of the slot the cursor pointed at when the player applied the tool. Otherwise it works as described on [Constructions](constructions.md#construction-scripts).

## Templates

A template is a menu entry that builds a construction with a starting set of modules. The game files have two forms.

In the base game, a modular construction gets into the build menu through its templates. `airfield.con.lua`, `airport.con.lua`, `harbor_modular.con.lua` and `warehouse.con.lua` have no `menuCategory`; their entries are the `.metacon.tl` files. In the 11 files with `constructionTemplates`, each template carries its own `menuCategory`, and the build menu ignores the `.con.lua`'s own one: it makes one entry per template (`getConstructionDefinitions` in `base/content/gui/gui/construction/construction_react_util.tl`). A `.con.lua` without templates gets its own entry if it has a `menuCategory`, as the industries do, and a metaconstruction that builds it adds a second one. The industries' own `createTemplateFn` is empty, so the files don't show what a construction's own entry places when its `createTemplateScript` returns modules.

A metaconstruction is a `.metacon.tl` file (26 in the base game) described by [`MetaConstructionDesc`](../api/api/type.md) (symbol: `MetaConstructionDesc`, see [symbol index](../index.tsv)), whose comment says it "replaces the deprecated construction templates". It has the menu fields of a construction (`availability`, `description`, `menuCategory`, `soundConfig`, `params`, `order`, `heightModView`, ...) and a `createTemplateScript`. The airfields are one example; `base/content/stations/air/air/airfield_era_a.metacon.tl`:

```teal
local airfield = ug_require "::airfield.tl"

return airfield.makeTemplate(airfield.eraAStart, airfield.eraBStart)
```

A `.metacon.tl` returns its description table at top level, without `function data()`; all 26 in the base game end with such a `return`. There are no `.metacon.lua` files in the game, so write these in Teal.

`airfield.tl` returns one description table per era with `createTemplateScript = { fileName = "airfield.script@createTemplateFn", params = { } }`. The warehouse entries work the same way through `warehouse.makeTemplate`.

The older form is a `constructionTemplates` list inside the `.con.lua`, each entry a [`ConstructionTemplate`](../api/api/type.md) (symbol: `ConstructionTemplate`, see [symbol index](../index.tsv)) with `type = "DYNAMIC"`, `availability`, `description`, `data.params` and `menuCategory`. `ConstructionDesc` marks the field "[DEPRECATED] Use metaconstructions instead", but 11 base files still use it, among them the rail station (`modular_station.con.lua`), the depots and the modular street station. These `.con.lua` files also set `createTemplateScript` themselves, as do industries such as `forest.con.lua`; 68 base `.con.lua` files have one.

The wiki's [Construction Templates](https://wiki.transportfever3.com/doku.php?id=modding:constructions:templates) page mixes the two: it says templates are defined in `metacon.lua` files and shows them as a list of entries with `type = "DYNAMIC"`. In the base game the files are `.metacon.tl`, and each returns a single description table without `type`; the list with `type = "DYNAMIC"` is the shape of the deprecated `constructionTemplates`.

### createTemplateFn

`createTemplateFn(captureParams, params)` decides which construction to build with which modules. After the wiki, `params` contains `paramX` and `paramY` (the hotkeys), a `seed` that increases with every placement or proposal, `year`, `templateIndex` (which template was used) and the template's own parameters. The airfield, from `base/content/stations/air/air/airfield.script.lua`:

```lua
createTemplateFn = function(captureParams, params)
	local result = {}
	local hangar = params.hangar == 1
	local terminals = params.terminals and params.terminals or 1

	if hangar then 
		result[terminalSlotId       + 0 ] = "airfield/af_hangar.module"
	end
	local offset = hangar and 2 or 0

	result[terminalSlotId + offset  ] = "airfield/af_main.module"
	result[terminalSlotId     + offset + 2 ] = "airfield/af_terminal.module"
	-- ... more terminals for params.terminals > 1 ...

	return {
		constructions = {
			{
				modules = result,
				constructionFileName = "airfield.con",
			},
		}
	}
end,
```

Each entry of `constructions` has `modules` (slot id to module file; `{}` for a construction without modules), the `constructionFileName`, and optionally `transf` (offset from the template origin, default none) and `params` (parameters for the construction, default the template's `params`). The warehouse's `createTemplateFn` in `warehouse.script.lua` puts one module from its `captureParams` into one slot:

```lua
result[EncodeSlotId({x = 0, y = 3}, "warehouse_large")] = captureParams.initialModule
```

## A minimal modular construction

The base game's modular constructions are large (`modular_terminal.script.lua` has over 3,000 lines). The files below fit together into the smallest working shape: a construction with one slot, one module for that slot, and a metaconstruction that puts the construction into the build menu with the module already placed. They follow the warehouse (`base/content/warehouses/warehouses/`), which splits its files the same way. The mod id `jdoe_yard` is made up, and the example has not been tested in game. All five files sit in one folder, `content/jdoe_yard/`, so the relative names between them work as they do in the warehouse folder.

`yard.con.lua` has no menu entry of its own and only names the update function:

```lua
function data()
	return {
		order = 1000,
		params = {},
		updateScript = {
			fileName = "yard.script@updateFn",
			params = {},
		},
	}
end
```

`yard.script.lua` builds the base: one subconstruction with a model, a ground face and terrain alignment, and one slot of a new type. The ground face and the alignment matter: a construction with one subconstruction and nothing but models is placed as loose models, which can't take modules (see [construction types](constructions.md#construction-types)). `createTemplateFn` names the module for slot 1:

```lua
local transf = require "::/scripts/mat4.tl"
local vec3 = require "::/scripts/vec3.tl"

local SHED_SLOT = 1
local area = { { -12, -6, 0 }, { 12, -6, 0 }, { 12, 6, 0 }, { -12, 6, 0 } }

function data()
return {
	createTemplateFn = function(captureParams, params)
		return {
			constructions = {
				{
					modules = { [SHED_SLOT] = "yard_shed.module" },
					constructionFileName = "yard.con",
				},
			}
		}
	end,
	updateFn = function(captureParams, params)
		return {
			subconstructions = {
				{
					tag = 1,
					models = {
						{ id = "::/assets/buildings/ind/aeration/aeration_06.mdl", transf = transf.transl(vec3.new(-8, 0, 0)) },
					},
					groundFaces = {
						{ face = area, modes = { { type = "FILL", key = "::/warehouses/wh_universal/wh_universal.gtex" } } },
					},
					terrainAlignmentLists = {
						{ type = "EQUAL", faces = { area } },
					},
				},
			},
			slots = {
				{
					id = SHED_SLOT,
					type = "jdoe_yard_shed",
					transf = transf.transl(vec3.new(5, 0, 0)),
					spacing = { -5, 5, -5, 5 },
					height = 6,
				},
			},
			cost = 50000,
		}
	end,
}
end
```

`yard_shed.module.lua` declares the module. Its `type` matches the slot, `menuCategory` gives it a tab in the module menu, and `metadata.price` is added to the construction's cost:

```lua
function data()
return {
	menuCategory = {
		categories = {
			{ category = "modules_misc", order = 100, },
		},
	},
	description = {
		name = _("JDOE_YARD_SHED_NAME"),
		description = _("JDOE_YARD_SHED_DESCRIPTION"),
		icon = "yard_shed.tga",
	},
	availability = { yearFrom = 0, yearTo = 0, },
	type = "jdoe_yard_shed",
	metadata = {
		price = 20000,
		maintenanceCost = 2000,
	},
	updateScript = {
		fileName = "yard_modules.script@shed.updateFn",
		params = {},
	},
	getModelsScript = {
		fileName = "yard_modules.script@shed.getModelsFn",
		params = {},
	},
	builderAudioRes = "::/gui/construction/sound/buildoze_module.builder_audio",
}
end
```

`yard_modules.script.lua` places the module's model with `addModelFn`, which multiplies the transform with the slot transform and sets the module's tag. Pass the subconstruction's model list as the fourth argument: without it, `addModelFn` appends to `result.models`, a list the construction result doesn't have.

```lua
local transf = require "::/scripts/mat4.tl"
local vec3 = require "::/scripts/vec3.tl"

local shedModel = "::/warehouses/wh_universal/wh_universal_shack.mdl"

function data()
return {
	shed = {
		updateFn = function(captureParams, result, transform, tag, slotId, addModelFn, params)
			addModelFn(shedModel, transf.transl(vec3.new(0, 0, 0)), tag, result.subconstructions[1].models)
		end,
		getModelsFn = function(captureParams)
			return { { id = shedModel, transf = transf.transl(vec3.new(0, 0, 0)) } }
		end,
	},
}
end
```

`yard.metacon.tl` is the build menu entry. It returns its table at top level and points at `createTemplateFn`:

```teal
local desc : MetaConstructionDesc = {
	description = {
		name = _("JDOE_YARD_NAME"),
		description = _("JDOE_YARD_DESCRIPTION"),
		icon = resolve("yard.tga"),
	},
	availability = { yearFrom = 0, yearTo = 0 },
	menuCategory = {
		categories = {
			{ category = "road_buildings", order = 9000 },
		},
	},
	order = 9000,
	params = {},
	createTemplateScript = {
		fileName = "yard.script@createTemplateFn",
		params = {},
	},
}

return desc
```

The two `.tga` icons and the translation keys (see [Translations](other-content.md#translations)) are yours to add. The models and the ground texture are base game files, addressed with `::/`.

The shipped example of a mod adding a module to a base construction is `dlcs/urbangames_deluxe_upgrade_pack/content/landmarks/landmarks/hq/modules/hq_maze.module.lua`. It brings no script of its own: it uses the base Headquarters' slot type `hq_fountain_l` and the base module script, and hands its generated model and ground texture data to that script through `captureParams`:

```lua
type = "hq_fountain_l",
-- ...
updateScript = {
	fileName = "::/landmarks/hq/headquarter_addon.script@updateFn",
	params = {
		generatedData = generatedData
	}
},
```

Paths inside `generatedData` carry the DLC's id (`"urbangames_deluxe_upgrade_pack::/landmarks/hq/modules/hq_maze.mdl"`), probably because the base script resolves them and a path without the id would point into the base game.

Official wiki: [Modular Constructions](https://wiki.transportfever3.com/doku.php?id=modding:constructions:modular), [Construction Templates](https://wiki.transportfever3.com/doku.php?id=modding:constructions:templates), [Construction Basics](https://wiki.transportfever3.com/doku.php?id=modding:constructions:basics), [Construction Menu](https://wiki.transportfever3.com/doku.php?id=modding:constructions:menu).
