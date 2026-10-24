
# Mod structure

This page takes apart the mods that ship with the game: the script mods in `mods/release/`, the eight campaign missions, and the two DLC packages in `dlcs/`. The base game in `base/` has the same layout. Where the official [modding wiki](https://wiki.transportfever3.com/doku.php?id=modding:start) documents a file, this page follows it and says where the shipped files differ.

## Folder layout

The wiki's [mod definition page](https://wiki.transportfever3.com/doku.php?id=modding:general:moddefinition) shows the files a mod can have:

```
my_mod/
├── mod.json                     technical definition: id, revision, hooks, flags (required)
├── strings.json                 translations of the mod's own strings (optional)
├── _metadata/
│   ├── modinfo.json             name, summary, description, authors, tags (required)
│   ├── description.html         description for the mod browser and mod.io (optional)
│   ├── 0.png, 1.png, ...        cover (0.png) and gallery images, 1920 x 1080 (optional)
│   └── mod.io_fileid.txt        written by the game after the first upload
└── content/                     everything the game loads
    └── mod.script.tl            pre/run/post script functions (optional)
```

Inside `content/`, the game tells resources apart by their file ending, not by their folder ([resource types](https://wiki.transportfever3.com/doku.php?id=modding:general:resourcetypes)). The wiki recommends the base game's folder layout anyway, with one subfolder per asset and a `shared` folder for files several assets use ([guidelines](https://wiki.transportfever3.com/doku.php?id=modding:general:guidelines#content_guidelines)).

A complete mod from the game files, `mods/release/urbangames_campaign_mission_01/` (archives unpacked, content shortened):

```
urbangames_campaign_mission_01/
├── mod.json                     mod id, mod script hooks, flags
├── _metadata/modinfo.json       name, description, authors, tags
├── _content.json                list of content archives and loose files
├── texture_metadata_cache.lua   size and mipmap data per texture
├── tealdef/                     Teal type definitions for this mod
│   ├── urbangames_campaign_mission_01_def.d.tl
│   └── mission/mission_params_01.d.tl
└── content/                     everything the game loads
    ├── mod.script.tl            pre/run/post script functions
    ├── info.mission.lua         mission description
    ├── animal.zip  assets.zip  audio.zip  cargos.zip  clima.zip
    ├── gui.zip  guide_system.zip  industries.zip  init.zip
    ├── mission.zip  terrain.zip  vehicle.zip  voiceover.zip
```

Not every mod has every part:

| Mod | Contents |
|---|---|
| `urbangames_no_costs`, `urbangames_sandbox`, `urbangames_tycoon` | `mod.json`, `_metadata/modinfo.json`, `content/mod.script.tl` |
| `urbangames_vehicles_no_end_year` | the same, with `content/mod.script.lua` in Lua |
| `urbangames_campaign` | `mod.json`, `_metadata`, `tealdef`, `content/info.campaign.lua` |
| `urbangames_campaign_mission_01` … `_08` | full layout as above |
| `dlcs/urbangames_preorder_pack` | `mod.json`, `_metadata`, content archives `animal`, `landmarks`, `vehicle` |
| `dlcs/urbangames_deluxe_upgrade_pack` | the same plus `tealdef/fun_elements/balloon.d.tl` and archives `characters`, `fun_elements`, `music` |

The DLC packages are built like mods. They have no mod script hooks and differ only in flags (`"autoActivate": true`, `"cosmetic": true`, `"severityRemove": "Warning"`).

## mod.json

`mod.json` is the technical description of the mod. The keys found in the shipped files:

| Key | Values in the shipped mods | Meaning |
|---|---|---|
| `modId` | `"urbangames_no_costs_1"`, `"urbangames_campaign_mission_01"`, `"urbangames_deluxe_upgrade_pack"` | Identifier, only `a-z`, `0-9` and `_`. Script references use it as prefix: `urbangames_no_costs_1::/mod.script@preRunFn`. The base game's `base/mod.json` has an empty `modId`. |
| `revision` | `1` | Version of the mod, shown on the mod's detail page. Raise it with every published update. |
| `preRunScript`, `runScript`, `postRunScript` | `{ "fileName": "<modId>::/mod.script@<fn>" }` or `""` | Script functions called while the game loads, see [mod script hooks](#mod-script-hooks) |
| `dependencies` | always `null` | Mods this mod needs, see [dependencies](#dependencies) |
| `incompatibilities` | always `null` | Mods this mod cannot run with |
| `params` | `null`, except in `base/mod.json` | Parameters the player sets for a game, see [mod parameters](#mod-parameters) |
| `options` | always `null` | Documented neither in the definitions nor in the wiki |
| `severityAdd`, `severityRemove` | `"None"`, `"Warning"` | Warning level when the mod is added to or removed from a savegame: `None`, `Warning` or `Critical`. Unset, they act as `None` (add) and `Warning` (remove). |
| `cosmetic` | `true` for the campaign, missions and DLCs | Achievements stay available if all active mods are cosmetic. Leave it `false` if the mod affects the simulation. |
| `visible` | `false` for the campaign missions | `false` hides the mod from the mod list. Default `true`. A hidden mod cannot auto-activate. |
| `autoActivate` | `true` for the DLCs | "If the mod is automatically activate by the game" |
| `url` | `""` in `base/mod.json` | "URL to the mod page" |

The wiki describes the three severity values like this ([mod.json](https://wiki.transportfever3.com/doku.php?id=modding:general:moddefinition#modjson)): `None` when adding or removing does no harm, for example a cosmetic change; `Warning` when the savegame loses something, such as vehicle models, but stays stable; `Critical` when it can break, for example because cargo types go missing. The [technical requirements](#technical-requirements) make a correct `severityRemove` mandatory.

The wiki says that, unlike in the earlier Transport Fever games, the `modId` has no major-version suffix such as `_3`. The shipped script mods still end in `_1` (`urbangames_no_costs_1`, `urbangames_sandbox_1`, `urbangames_tycoon_1`, `urbangames_vehicles_no_end_year_1`), while their folders, the campaign and the DLCs have no suffix. For an update that breaks savegames, the wiki recommends a new `modId` and a separate mod, not a higher `revision`.

The game finds a mod by its `modId`, wherever it is installed. With the same id installed twice, the staging area wins over a manual installation, and that wins over a mod.io subscription.

The quoted descriptions come from [`Mod.ModDesc`](../api/api/type/mod.md) (symbol: `Mod.ModDesc`, see [symbol index](../index.tsv)) in `api/tealdef/api/type/mod.d.tl`, which is the type the game uses for a loaded mod description. `ModDesc` has `severityAdd`/`severityRemove` as integers ("0: None, 1: Warning, 2:Critical") while the JSON files use the names. The game reads installed mods through [`ModRep`](../api/api/type.md) (symbol: `ModRep`, see [symbol index](../index.tsv)) (`app.getUserProfile():getModRep()` in `base/content/gui/gui/menu/mod_util.tl`), whose `getModDesc` returns this type.

### Dependencies

No shipped mod declares a dependency. The wiki ([dependencies and incompatibilities](https://wiki.transportfever3.com/doku.php?id=modding:general:moddefinition#dependencies_incompatibilities)) shows the JSON form. A repaint that needs the mod with the original vehicle would declare:

```json
{
    "dependencies": [
        {
            "mod": { "modId": "jdoe_base_vehicles", "revisionMin": 2, "revisionMax": -1 },
            "modInfo": { "displayName": "Base Vehicles", "url": "https://example.com/base-vehicles" },
            "loadBefore": true,
            "optional": false
        }
    ],
    "incompatibilities": [
        { "mod": { "modId": "jdoe_hard_mode" } }
    ]
}
```

| Field | Meaning | Default |
|---|---|---|
| `mod.modId` | `modId` from the other mod's `mod.json` | required |
| `mod.revisionMin` | lowest accepted revision | none (`-1` or left out) |
| `mod.revisionMax` | highest accepted revision; rarely needed, since updates should stay compatible | none (`-1` or left out) |
| `modInfo.displayName` | name shown in the "Mod Dependencies Not Satisfied" window when the dependency is not installed | |
| `modInfo.url` | optional download link, for example on a third-party site | |
| `loadBefore` | load the dependency before this mod | `true` |
| `optional` | recommended, not required | `false` |

An incompatibility has only the `mod` part. The game shows both while the player activates mods. It activates dependencies first, offers to download missing ones, and warns before a game starts with a missing dependency or an active incompatible mod.

The types match: [`Mod.ModDependency`](../api/api/type/mod.md) (symbol: `Mod.ModDependency`, see [symbol index](../index.tsv)) has `mod`, `optional`, `loadBefore` and `modInfo`; [`Mod.ModRef`](../api/api/type/mod.md) (symbol: `Mod.ModRef`, see [symbol index](../index.tsv)) holds `modId`, `revisionMin` and `revisionMax`; [`Mod.ModRefInfo`](../api/api/type/mod.md) (symbol: `Mod.ModRefInfo`, see [symbol index](../index.tsv)) holds `displayName` and `url`; an incompatibility ([`Mod.ModIncompatibility`](../api/api/type/mod.md) (symbol: `Mod.ModIncompatibility`, see [symbol index](../index.tsv))) is only a `ModRef`. The mod selector in `base/content/gui/gui/menu/mod_util.tl` checks these fields: it reports dependencies as missing with reason `"NotFound"` or `"IncorrectVersion"`, honours `optional` and `loadBefore`, and lists active incompatible mods.

## _metadata/modinfo.json

The player-facing information sits in a separate file. From `mods/release/urbangames_campaign_mission_01/_metadata/modinfo.json`:

```json
{
    "authors": [
        {
            "name": "Urban Games",
            "role": "CREATOR"
        }
    ],
    "description": "...",
    "name": "Campaign Mission 01",
    "tags": [
        "Campaign"
    ],
    "url": ""
}
```

The script mods and DLCs use translation keys such as `"MOD_NO_COSTS_NAME"` as values. Their translations come with the game's own strings (see [translated strings](#translated-strings)). A mod brings its translations along in the same file, as the wiki's [modinfo.json section](https://wiki.transportfever3.com/doku.php?id=modding:general:moddefinition#modinfojson) shows:

```json
{
    "name": "Soft Brakes",
    "summary": "Trains brake more gently.",
    "description": "Lowers the global train brake deceleration.",
    "localization": {
        "de": {
            "name": "Sanfte Bremsen",
            "summary": "Züge bremsen sanfter.",
            "description": "Senkt die globale Bremsverzögerung von Zügen."
        }
    },
    "authors": [ { "name": "jdoe", "role": "CREATOR" } ],
    "tags": [ "Script Mod" ],
    "dependencies": [ "5499706" ],
    "url": "https://example.com/soft-brakes"
}
```

| Key | Rules |
|---|---|
| `name` | Display name, at most 32 characters, no line breaks. The guidelines ask for title case. |
| `summary` | Tooltip on the mod's tile in the Mod Hub, at most 100 characters, no line breaks |
| `description` | Long text with line breaks. Ignored if `_metadata/description.html` exists. Must be in English. |
| `localization` | `name`, `summary` and `description` per language code; the top-level values apply to all other languages |
| `authors` | list of `name` and `role`; the shipped files only use `"CREATOR"` |
| `tags` | officially supported tags, used as filters in the Mod Hub (`"Script Mod"`, `"Campaign"`, `"Town Building"`, `"Depot"` appear in the files and the wiki) |
| `dependencies` | mod.io ids (not `modId`s) of the mods this one needs, so mod.io links them. Removing an entry here does not remove it on mod.io; do that on the website. |
| `url` | link for more information. The game does not show it, and consoles can't open it, so the description must contain everything a player needs. |

On upload, mod.io gets the English name and a description with all languages joined, because mod.io has no per-language texts yet; the wiki expects that to change by Q2/2027. The upload overwrites name and description on mod.io with the values from `modinfo.json`.

The mod hub side has its own record, [`Modhub.ModInfo`](../api/api/type/modhub.md) (symbol: `Modhub.ModInfo`, see [symbol index](../index.tsv)), with `title`, `summary`, `description`, `tags`, `logoImage`, download and rating counters and release information. The wiki doesn't map the fields one by one. It says the English name is uploaded as the title, and that the `_metadata/<number>.png` files are the cover (`0.png`) and gallery images.

## Mod script hooks

The mod script hooks are the three `mod.json` keys `preRunScript`, `runScript` and `postRunScript`. Each names one function. The base game and the official mods use them for different jobs:

| Hook | When it runs (wiki) | Signature in the shipped scripts | What the shipped mods do there |
|---|---|---|---|
| `preRunScript` | before game and mod resources are loaded | `preRunFn(captureParams, configDict, allModParams, baseConfig)` | Change the base config ([`BaseConfig`](../api/api/type.md)). `urbangames_no_costs` sets `baseConfig.noCosts = true`; the base game fills almost all of it here. |
| `runScript` | when the mod is loaded; registered modifiers and filters apply while resources load, after all `runFn` have run | `runFn(captureParams, configDict, allModParams)` (`base/content/base/base/base_mod.lua`; `urbangames_vehicles_no_end_year` names the second argument `settings` and leaves out the third, mission 01 takes only `captureParams`) | Register resource modifiers with `addModifier` and file filters with `addFileFilter`. |
| `postRunScript` | once, after all resources are loaded | `postRunFn(captureParams, configDict, allModParams)` | Edit, add or hide resources through `api.res`. |

Each hook runs for the base game first and then for the mods in activation order ([script functions](https://wiki.transportfever3.com/doku.php?id=modding:general:modscripts#script_functions)). Dependencies with `loadBefore` are activated before the mods that need them. The wiki recommends doing filtering and changes in `postRunFn` where possible and keeping `runFn` modifiers for what can't be done there.

The arguments are:

| Argument | Content |
|---|---|
| `captureParams` | the `params` table of the script reference ([resource types](https://wiki.transportfever3.com/doku.php?id=modding:general:resourcetypes#static_resources_scripts)); the hook references in `mod.json` have none |
| `configDict` | the selected climate, economy and name list |
| `allModParams` | parameter values of all mods, keyed by mod id; `allModParams[getCurrentModId()]` gives this mod's own ([`getCurrentModId`](../api/main.md) (symbol: `getCurrentModId`, see [symbol index](../index.tsv))) |
| `baseConfig` | the base config, `preRunFn` only |

The wiki lists the hook parameters without `captureParams`. Every shipped `mod.script` function takes it as the first parameter, which matches the wiki's own rule for script references, so write your functions with it. The base game reads its own parameters from `allModParams[""]` (`base/content/base/base/base_mod.lua`).

A Teal module returns a table of functions (`urbangames_no_costs`, shown in [Getting started](getting-started.md#a-minimal-mod)); a Lua file defines `data()` and returns the table from it (`mods/release/urbangames_vehicles_no_end_year/content/mod.script.lua`). How `runFn` registers resource modifiers and file filters, with the list of categories, is in [Resource modifiers](scripting.md#resource-modifiers) and [File filters](scripting.md#file-filters). [Load-time code](scripting.md#load-time-code) shows what each phase is used for.

A `postRunFn` works on the repositories in [`api.res`](../api/api/res.md). From `mods/release/urbangames_campaign_mission_01/content/mod.script.tl`:

```teal
mod.postRunFn = function(captureParams)
	local baseConfig = api.res.getBaseConfig()
	baseConfig.advancedOptions.subventionMode = 0

	local model = api.res.modelRep.get(api.res.modelRep.find("::/vehicle/truck/horse_cart_medium/horse_cart_medium.mdl"))
	local tvMetadata = model.metadata["transportVehicle"] as ModelMetadata.TransportVehicle
	tvMetadata.loadSpeed = tvMetadata.loadSpeed * 10

	-- ...
	local allBriges = api.res.bridgeTypeRep.getAll()
	for id, name in pairs(allBriges) do
		if name ~= "::/infrastructure/bridge/trestle.bridge" then
			api.res.bridgeTypeRep.setVisible(id, false)
		end
	end
end
```

[`ResTypeRep`](../api/api/res.md) (symbol: `ResTypeRep`, see [symbol index](../index.tsv)) is the interface of every repository: `find(resName)` returns an id (`-1` if missing), `get(id)` the resource, `getAll()` a map of ids to names, `setVisible` hides a resource from the player, and `add` registers a new one. The base game's `postRunFn` uses `api.res.moduleRep.add` to create one station module per track type.

For edits, the wiki's examples use the table form: [`getAsTable(id)`](../api/api/res.md) (symbol: `ResTypeRep.getAsTable`, see [symbol index](../index.tsv)) returns a Lua table you can change freely, [`setAsTable(id, data)`](../api/api/res.md) (symbol: `ResTypeRep.setAsTable`, see [symbol index](../index.tsv)) writes it back, and [`addAsTable(name, data)`](../api/api/res.md) (symbol: `ResTypeRep.addAsTable`, see [symbol index](../index.tsv)) adds it as a new resource. [`modelRep.forEachModelWithMetadata`](../api/api/res.md) (symbol: `ModelRep.forEachModelWithMetadata`, see [symbol index](../index.tsv)) loops over models with a given metadata key. A capacity multiplier from a mod parameter:

```teal
mod.postRunFn = function(captureParams, configDict : {{string, string}}, allModParams : {string : {string : integer}})
	local factor = allModParams[getCurrentModId()].capacityFactor
	api.res.modelRep.forEachModelWithMetadata("transportVehicle", function(name : string)
		local id = api.res.modelRep.find(name)
		local model = api.res.modelRep.getAsTable(id)
		local tv = model.metadata["transportVehicle"] as ModelMetadata.TransportVehicle
		for _, compartment in ipairs(tv.compartments) do
			for _, loadConfig in ipairs(compartment.loadConfigs) do
				if loadConfig.cargoEntry and loadConfig.cargoEntry.capacity then
					loadConfig.cargoEntry.capacity = loadConfig.cargoEntry.capacity * factor
				end
			end
		end
		api.res.modelRep.setAsTable(id, model)
	end)
end
```

This follows the wiki's [resource modification example](https://wiki.transportfever3.com/doku.php?id=modding:general:modscripts#resource_modification). Scripts can't add new 3D data: a new model added this way can only point at existing `.mdl` files through its `modelPath`.

### Mod parameters

`base/mod.json` declares the new-game settings as `params`. One entry:

```json
{
    "defaultIndex": 1,
    "key": "advancedOptions.reforestation",
    "name": "Reforestation",
    "tooltip": "Enable or disable reforestation.\n\nIf reforestation is on, bald spaces after removing constructions will regrow their initial trees.",
    "uiType": "Button",
    "values": [
        "Off",
        "On"
    ],
    "yearFrom": 0,
    "yearTo": 0
}
```

The fields match [`ScriptParam`](../api/api/type.md) (symbol: `ScriptParam`, see [symbol index](../index.tsv)) and the wiki's [mod parameters section](https://wiki.transportfever3.com/doku.php?id=modding:general:modscripts#mod_params):

| Field | Meaning |
|---|---|
| `key` | name under which scripts read the value |
| `name` | label in the UI; a translation key, see [Translations](other-content.md#translations-in-a-mod-stringsjson) |
| `tooltip` | optional mouse-over text; a translation key, see [Translations](other-content.md#translations-in-a-mod-stringsjson) |
| `uiType` | `Button`, `Slider`, `ComboBox`, `IconButton` or `CheckBox` |
| `values` | labels, or image paths for `IconButton`; labels are translation keys, see [Translations](other-content.md#translations-in-a-mod-stringsjson) |
| `numbers` | optional values the scripts receive per option, one per entry in `values` (`script_param_util.tl` drops the list when the counts differ); default 1, 2, 3, .... [`ScriptParam`](../api/api/type.md) (symbol: `ScriptParam`, see [symbol index](../index.tsv)) types the list as `{number}`, the hook signatures type the received values as `integer`, and no shipped parameter uses a fraction, so check a fractional value in game. |
| `defaultIndex` | default option, counted from 0 (unlike Lua indices); default the first option |

So `"defaultIndex": 1` in the reforestation entry above selects `"On"`. Without `numbers`, a script receives `1` for the first option, `2` for the second and so on. The player sets the values in the mod selection when starting a new game or loading a savegame. Constructions use a similar parameter list in Lua, where `defaultIndex` is 1-based (see [Parameters](constructions.md#parameters)). Some entries carry `tags` such as `"desktop"`, `"console"` or `"experimentalMapFeatures"`, and the same key appears several times with different value lists per tag. The script functions receive the chosen values in `allModParams`, a table keyed by mod id; the base game reads its own from `allModParams[""]`. At runtime, game scripts read them with `api.engine.config.getModParams()`, for example `api.engine.config.getModParams()[""]["isMapEditor"]` in `base/content/game_mechanics/game_mechanics/towns/towns.script.tl`. No shipped game script reads its own mod's parameters. [`getOwningModId()`](../api/main.md) (symbol: `getOwningModId`, see [symbol index](../index.tsv)), called in the game script file itself, should give the key for `getModParams()`, but no shipped file confirms it.

## How content is packaged

### How the zips map to folders

The game ships `content/` as a set of zip archives plus loose files. `_content.json` lists both. From `mods/release/urbangames_campaign_mission_01/_content.json` (shortened):

```json
{
    "archives": [
        { "fileName": "animal.zip", "entries": [ { "fileName": "animal/alligator/alligator.mdl", ... } ] },
        { "fileName": "mission.zip", "entries": [ { "fileName": "mission/mission_story.tl", ... } ] }
    ],
    "files": [ "info.mission.lua", "mod.script.tl", "mission01conlua.txt" ]
}
```

Entry names are relative to the folder the archive sits in. `mission.zip` is at the top of `content/` and contains `mission/mission_story.tl`, which the game addresses as `urbangames_campaign_mission_01::/mission/mission_story.tl`. Archive names can contain folders too: the base game's `assets/buildings/ind.zip` has the entry `ind/mat/assets_ind_1_body.mtl`, so the file's path is `/assets/buildings/ind/mat/assets_ind_1_body.mtl`. The base game has 1,092 archives, among them `assets/stations.zip` and `buildings/a/c1/1x1_01.zip`, and loose files such as `industries/industry_workers.gs.lua`. In the unpacked tree these docs quote from, each archive became a folder named after the zip, which gives doubled paths such as `content/mission/mission/mission_story.tl`.

A mod you write yourself doesn't need archives: the wiki's mod layout has loose files in `content/`. Whether the game builds the archives and `_content.json` when it cooks a mod is covered [below](#files-generated-by-the-game).

### File references inside content

The wiki's [file names and references section](https://wiki.transportfever3.com/doku.php?id=modding:general:resourcetypes#file_names_references) gives the rules. A path starting with `/` is absolute from the `content` folder of the current mod (for base game files, the base game). A path without a leading `/` is relative to the referencing file. A prefix picks another mod:

| Form | Resolves to | Example from the game files |
|---|---|---|
| `/path` | absolute, in the current mod | `ug_require "/mission/mission_params_01.tl"` in mission 01; `ug_require "/scripts/vec3.tl"` in the base game |
| `path` | relative to the current file, in the current mod | `"small_stops.script@updateFn"`, `resolve("small_new.mdl")` |
| `::/path` | absolute, in the base game | `"::/gui/construction/construction_desc_hud_icons.script@configureStationHudIconsFn"` |
| `::path` | relative to the current file, but in the base game's folders | `"::buildings/ind/mat/assets_ind_1_body.mtl"` in mission 04's `/assets/instrument_box.mdl`, which is the base file `/assets/buildings/ind/mat/assets_ind_1_body.mtl` |
| `<modId>::/path` | absolute, in mod `<modId>` | `"urbangames_campaign_mission_01::/mission/hud_replacement.script"` |
| `<modId>::path` | relative to the current file, in mod `<modId>` | the wiki's `"ug_wiki_example_mod::mod.script@preRunFn"` in `mod.json` |

Base game files often load other base files with `/...`, because for them `/` means the base game. When you copy such a line into a mod, write `::/...`: `require "/scripts/mat4.tl"` in a base file becomes `require "::/scripts/mat4.tl"` in yours.

`../` is not allowed, so a file can't refer to its parent folder. [`resolve(relativePath)`](../api/main.md) (symbol: `resolve`, see [symbol index](../index.tsv)) "Resolves a file paths relative to the current file".

The shipped `mod.json` files write the hook references with a slash (`urbangames_no_costs_1::/mod.script@preRunFn`), the wiki's examples without one. Since `mod.json` sits at the mod root and `mod.script` directly in `content/`, both forms point at the same file.

File and folder names may only use lowercase `a-z`, `0-9`, `_` and `-`, with no spaces, and should stay short. Text files (`.lua`, `.tl`, `.json`, `.mdl`, `.msh`, ...) are UTF-8 without a byte order mark.

Script references point at a function: `file.script@function`. The `.lua` or `.tl` extension is left out, and the part after `@` can be a dotted path into the returned table (`"modules.script@bulk.updateFn"` in `base/content/warehouses/warehouses/wh_bulk.module.lua`). [`ScriptRef`](../api/api/type.md) (symbol: `ScriptRef`, see [symbol index](../index.tsv)) describes the same idea for an older `models/model/animal/animal.flock.crane` notation.

Code is loaded with [`ug_require`](../api/main.md) (symbol: `ug_require`, see [symbol index](../index.tsv)), "Used instead of the regular lua `require` to include game related script". The base scripts cast the result to its type: `local mathutil = ug_require "::/scripts/mathutil.lua" as MathUtil`.

The base game's Lua files load modules with plain `require` and the same paths, for example `require "/scripts/soundsetutil.lua"` or `require "::/scripts/construction/constructionutil.lua"`. Both functions work, because `base/content/base/base/init.lua` replaces `require` so that a path ending in `.lua` or `.tl` is resolved like any other file reference, and `ug_require` only calls that `require`. In Teal files, use `ug_require ... as Type`: only `ug_require` is declared in `main.d.tl`, and the `as` cast is Teal syntax that fails in a `.lua` file. `init.lua` keeps the original `require` for paths without those extensions.

### What the base content folders hold

The top-level folders of `base/content` and typical file types in them. The wiki's [directory structure table](https://wiki.transportfever3.com/doku.php?id=modding:general:resourcetypes#directory_structure) describes the same folders, and its [file endings table](https://wiki.transportfever3.com/doku.php?id=modding:general:resourcetypes#file_endings) lists every resource type by extension.

| Folder | Typical files |
|---|---|
| `animal`, `vehicle`, `characters` | `.mdl` models with `.mtl` materials, animations (`.ani`) |
| `assets`, `buildings` | asset and town building models; buildings also have `.con.lua` |
| `stations`, `depots`, `industries`, `landmarks`, `warehouses` | `.con.lua` constructions, `.module.lua` modules, `.metacon.tl` metaconstructions, their `.script.lua`/`.script.tl` |
| `infrastructure` | streets (`.street.lua`, `.street_template.lua`), tracks, bridges, tunnels, signals |
| `game_mechanics` | game scripts (`.gs.lua`) for towns, company, finance, notifications, ... (see [Game mechanics](game-mechanics.md#the-base-game-scripts)) |
| `gui` | the user interface in Teal, stylesheets (`.css.lua`), [GUI resources](gui.md#gui-resources) (`.gres.lua`) |
| `mission` | mission framework, tasks, guide system |
| `scripts` | shared libraries (`vec3`, `mat4`, `util`, construction utilities, `react.d.tl`) |
| `rendering` | material types (`.mat.lua`), shader programs (`.prog.lua`), techniques, properties |
| `climates`, `terrain`, `economy`, `cargos`, `names`, `music`, `locale` | climates (`.clima.lua`), terrain materials, [economies](game-mechanics.md#economies) (`.eco.lua`), cargo types, name lists, playlists, languages |

Binary files (`.msh` meshes, `.dds` and `.tga` textures, sounds) are listed in `_content.json` but are not part of the unpacked sources these guides were written from.

### Generic resources

Configuration that is not a model or construction is often a `.res.lua` file with a `type` and a `data` table. From `base/content/game_mechanics/game_mechanics/game_time/game_time_config.res.lua`:

```lua
function data()
	return {
		type = "game_script_config",
		data = {
			timeOfDayMode = "Dynamic"
		}
	}
end
```

Scripts read them through `api.res.genericRep`, either by name (`api.res.genericRep.find("::/game_mechanics/game_time/game_time_config.res")`) or by type (`api.res.genericRep.getAllOfType("notification")`). The 295 `.res.lua` files in the game files use these types (paths below `base/content/` unless a mod is named):

| Type | Files | Example | What it adds | See |
|---|---|---|---|---|
| `menu_filter_category` | 75 | `gui/gui/construction/menu/filter_categories/gravel.res.lua` | a filter in the build menu | [The build menu](constructions.md#the-build-menu) |
| `menu_category` | 49 | `gui/gui/construction/menu/menu_categories/category_road_buildings.res.lua` | a build menu tab or module tab | [The build menu](constructions.md#the-build-menu) |
| `react-plugin ::<ExtensionPoint>` | 44 | `gui/gui/entity_window/warehouse/warehouse_eow_stocks.res.lua` | a GUI plugin at an extension point | [GUI](gui.md#extending-the-base-gui-plugins) |
| `notification` | 33 | `game_mechanics/game_mechanics/notifications/types/stuck_vehicle.res.lua`; mission 05 and the deluxe DLC add their own | a notification type | [Notifications](game-mechanics.md#notifications) |
| `construction_tool` | 22 | `gui/gui/construction/tools/asset_eraser_tool.res.lua` | a tool in the build menu | [Tools in the build menu](constructions.md#tools-in-the-build-menu) |
| `company_exploration` | 12 | `game_mechanics/game_mechanics/company/explorations/exploration_coal.res.lua` | an exploration perk | [Company perks](game-mechanics.md#company-perks) |
| `stamp` | 11 | `gui/gui/construction/shared/tex/stamps/desert_hill1.res.lua` | a heightmap (`.png` plus icon) for the "Roughen Terrain" tool in the build menu, which only the Map Editor and sandbox mode offer | [Terrain stamps](environment.md#terrain-stamps) |
| `missiontask_boot` | 8 | `mission/mission/mission.res.lua` in each campaign mission | starts a mission | [How a mission starts](missions.md#how-a-mission-starts) |
| `guide_system_boot` | 6 | `mission/mission/guide_system/guide_system.res.lua`; missions 01, 02, 03 and 07 | starts a guide system | [How a mission starts](missions.md#how-a-mission-starts) |
| `game_script_config` | 6 | `game_mechanics/game_mechanics/game_time/game_time_config.res.lua` | settings that a base game script reads by name | [The base game scripts](game-mechanics.md#the-base-game-scripts) |
| `subvention` | 5 | `game_mechanics/game_mechanics/subventions/deliver_cargo/deliver_cargo.res.lua`; mission 06 adds `deliver_visitors` | a subsidy type | [Subsidies](game-mechanics.md#subsidies) |
| `rename_scheme_component` | 5 | `gui/gui/line_vehicle_mgmt/auto_rename/scheme_components/scheme_component_town.res.lua` | one part of an automatic name, with its `renameFn` | none |
| `react-replacement-config` | 4 | `mission/mission/hud_replacement.res.lua` in mission 01 | replaces a base GUI recipe | [Replacing a base recipe](gui.md#replacing-a-base-recipe) |
| `firstStopToSend_scheme` | 4 | `gui/gui/line_vehicle_mgmt/first_stop_to_send/scheme_firststop.res.lua` | a rule for the stop a new line vehicle goes to first | none |
| `rename_scheme` | 3 | `gui/gui/line_vehicle_mgmt/auto_rename/schemes/scheme_firststop_laststop.res.lua` | an automatic naming scheme (`formatString = "{key1} - {key2}"`) | none |
| `drag_and_drop` | 3 | `gui/gui/line_vehicle_mgmt/dnd_train_composition.res.lua` | an empty marker for something the GUI can drag, read by name | none |
| `company_permit_key` | 3 | `game_mechanics/game_mechanics/company/permitKeys/hq.res.lua` | a permit key perk | [Company perks](game-mechanics.md#company-perks) |
| `tutorial_boot` | 1 | `mission/mission/tutorial/mission.res.lua` | starts the tutorial | [How a mission starts](missions.md#how-a-mission-starts) |
| `game_config` | 1 | `infrastructure/traffic_light/traffic_light/traffic_light_config.res.lua` | `trafficLightYearFrom`, read by name | none |

Only `notification`, `subvention`, `missiontask_boot`, `guide_system_boot` and `react-replacement-config` appear in a shipped mod or DLC. For the other types that the base game collects with `getAllOfType` (`menu_category`, `menu_filter_category`, `react-plugin`, `construction_tool`, `company_exploration`, `company_permit_key`, `stamp`, `rename_scheme`, `rename_scheme_component`, `firstStopToSend_scheme`, `tutorial_boot`), a mod's file of the same type should show up next to the base ones, but no shipped mod confirms it. `game_script_config`, `game_config` and `drag_and_drop` resources are read by their fixed names, so a new file of these types is not picked up.

### Placeholders for missing files

`base/content/placeholders/placeholders/` holds stand-ins for missing content: `generic.con.lua` (name `MISSING_CONSTRUCTION`, drawn as the exclamation mark marker `/assets/markers/marker_exclamation.mdl`), `missing_signal.con.lua`, `missing_street_terminal.con.lua`, `missing_gtex.gtex.lua` and one `missing_*.mdl` per model kind (`missing_rail_vehicle.mdl`, `missing_road_vehicle.mdl`, `missing_animal.mdl`, `missing_person.mdl`, ...). When a savegame or construction refers to a resource that no longer exists, for example after a mod was removed, the game should show one of these stand-ins in its place. Only the file names and `MISSING_CONSTRUCTION` point to this; it is not confirmed in game. The base scripts also use them directly: `base/content/scripts/scripts/construction/constructionutil.lua` falls back to `"::/placeholders/missing_generic.mdl"` when a model group is empty, and 48 files use `/placeholders/generic.tga` as their icon.

### Translated strings

Text in scripts is wrapped in `_()`. A mod translates its keys in `strings.json` at the mod root, one table per language code. Key functions, the `strings.json` format, the language codes and the `.lang.lua` files are explained in [Translations](other-content.md#translations). The mod's name, summary and description are translated in `modinfo.json` instead (see [above](#_metadatamodinfojson)).

## Files generated by the game

`_content.json` and `texture_metadata_cache.lua` (texture sizes and mipmap counts per `.dds`) are present in every shipped mod. The modhub types mention cooking: [`Modhub.ModValidatorResult`](../api/api/type/modhub.md) (symbol: `Modhub.ModValidatorResult`, see [symbol index](../index.tsv)) has `cookedPcPath` and `cookedConsolePath`, and `ModPublishHelper.publish` "Validates and cooks the mod once more first and uploads the cooked folders". Neither the files nor the wiki say whether cooking produces these two files or whether a hand-made mod needs them. The wiki's mod layout doesn't list them, so a mod without them is the documented starting point.

The one file the wiki names as generated is `_metadata/mod.io_fileid.txt`, written after the first upload to mod.io. It holds the mod.io id and links later uploads to the same entry (see [publishing](getting-started.md#publishing-on-modio)).

## Technical requirements

Every upload goes through an automatic validation for PC and consoles, and a mod must pass it to be released. The wiki's [technical requirements](https://wiki.transportfever3.com/doku.php?id=modding:general:guidelines#technical_requirements):

| Area | Requirement |
|---|---|
| Integrity | all resources are syntactically correct and load without errors or warnings |
| Security | no batch files, shell scripts or other executables |
| Encoding | text files in UTF-8 without BOM |
| Size | at most 500 MB unpacked per mod |
| Names | lowercase files and folders, no spaces or special characters |
| Ballast | no source files (`.psd`, `.fbx`) or temporary files |
| Meshes | lowest LOD mesh blobs at most 256 KB |
| Textures | DDS with mipmaps, power-of-two sizes (512, 1024, 2048, ...) |
| Scripts | only the official scripting API |
| Audio | `.wav` with 16-bit samples at 48,000 Hz |
| Savegames | `severityRemove` set correctly |
| Consoles only | no custom shaders, no changes to core gameplay that hurt performance |

The guidelines also give content targets, for example 20,000 to 50,000 triangles for a typical vehicle (the largest have up to 100,000), a texel density of 128 to 170 pixels per metre and 1 to 3 LODs. Those belong to the [models guide](models-and-assets.md#levels-of-detail).

Official wiki: [Mod definition](https://wiki.transportfever3.com/doku.php?id=modding:general:moddefinition), [Mod parameters and scripts](https://wiki.transportfever3.com/doku.php?id=modding:general:modscripts), [Resource types and structure](https://wiki.transportfever3.com/doku.php?id=modding:general:resourcetypes), [Syntax](https://wiki.transportfever3.com/doku.php?id=modding:general:syntax), [Resource modifiers and file filters](https://wiki.transportfever3.com/doku.php?id=modding:scripting:modifiersfilters), [Localizations](https://wiki.transportfever3.com/doku.php?id=modding:misc:localizations), [Guidelines and requirements](https://wiki.transportfever3.com/doku.php?id=modding:general:guidelines), [Best practices](https://wiki.transportfever3.com/doku.php?id=modding:general:bestpractice), [Publish a mod](https://wiki.transportfever3.com/doku.php?id=modding:general:publishing).
