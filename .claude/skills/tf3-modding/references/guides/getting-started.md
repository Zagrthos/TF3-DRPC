
# Getting started

This page sets up an editor for Teal and Lua, shows where mods live on disk, builds a small working mod, and walks through testing and publishing it. The examples come from the mods that ship with the game and from the official [modding wiki](https://wiki.transportfever3.com/doku.php?id=modding:start).

The wiki's [introduction](https://wiki.transportfever3.com/doku.php?id=modding:introduction) gives the ground rule: don't change files in the installation folder. Put every change in a mod, which is a folder with a `mod.json`, a `_metadata/modinfo.json` and a `content/` folder.

## Tools

Any text editor works for `.lua`, `.tl` and `.json` files. The wiki's [external tools page](https://wiki.transportfever3.com/doku.php?id=modding:tools:external#vs_code_setup) recommends Visual Studio Code with two extensions:

| Extension | Purpose |
|---|---|
| Lua (sumneko) | Lua syntax, completion and diagnostics |
| Teal (Patrick Desaulniers, `pdesaulniers.vscode-teal`) | Teal syntax and type checking against the game's definition files |

The Teal extension needs the Teal compiler (`tl`) from the official Teal repository. After installing it, set the path to the `tl` executable in the extension settings.

Teal is optional. You can write mods in plain Lua, and the official mods mix both: `mods/release/urbangames_vehicles_no_end_year/content/mod.script.lua` is Lua, while `mods/release/urbangames_no_costs/content/mod.script.tl` is Teal. The wiki's [syntax page](https://wiki.transportfever3.com/doku.php?id=modding:general:syntax) is a short Lua and Teal primer. It says the game does not support global variables, so declare every variable `local`. The one global that Lua resource files define is `function data()`, as in the `mod.script.lua` above.

## The development workspace

The wiki says the [Model Editor](https://wiki.transportfever3.com/doku.php?id=modding:tools:modeleditor) creates a ready workspace the first time it starts: `tlconfig.lua`, `workspace_def.tl` and `.vscode/extensions.json`, placed in the staging area (see [where mods live](#where-mods-live) and [Model editor](model-editor.md#setup)). The wiki's file list says `workspace_def.lua`, its later steps say `workspace_def.tl`. The file is the Teal global definition file that `global_env_def` in `tlconfig.lua` names, so this page calls it `workspace_def.tl`. If that fails, it says to copy the files from `<tf3_install_folder>/dev-workspace` and fix the paths in `tlconfig.lua`.

The game files these docs are built from have the template under a different name. The folder is `vscode-template/`, and the global definition file in it is `all_def.tl`, not `workspace_def.tl`:

```
vscode-template/
├── .vscode/extensions.json
├── tlconfig.lua
└── all_def.tl
```

Use whichever your installation has. The content is the same idea in both cases. `.vscode/extensions.json` recommends the Teal extension:

```json
{
	"recommendations": [
		"pdesaulniers.vscode-teal"
	]
}
```

`tlconfig.lua` tells the Teal compiler where the definitions are:

```lua
return {
	include_dir = {
		-- ensure to set proper paths for the definitions for the base game
		"<tf3_install_folder>api/tealdef",
		"<tf3_install_folder>base/tealdef",

		-- list your mods here, if you provide your own d.tl definitions
		-- "example_mod_1",
	},
	global_env_def = "all_def"
}
```

Replace `<tf3_install_folder>` with the path of your game installation. The two folders are:

| Folder | Content | Reference |
|---|---|---|
| `api/tealdef` | The engine API: `api.engine`, `api.cmd`, `api.gui`, `api.res`, `api.type`, `api.util`, `api.modhub`, `app` and the global functions | [Engine API](../api/api.md) |
| `base/tealdef` | Definitions of the base game's own script libraries | [Base game scripts](../index.tsv) (symbol: `base-game-scripts`, see [symbol index](../index.tsv)) |

`global_env_def` names the file Teal loads as the global environment of every file it checks. The template's `all_def.tl` is short:

```teal
require "api_def"
require "content_def"

-- list your mods here, if you provide your own d.tl definitions
-- require "example_mod_1"
```

`api_def` is `api/tealdef/api_def.d.tl`, which requires all engine API definitions (`api.type`, `api.res`, `api.engine`, `api.cmd`, `app`, `api.gui`, `main`, ...). `content_def` is `base/tealdef/content_def.d.tl`, which requires the base game definitions (colours, sound effects, GUI resources, ...). After that, globals such as `api`, `app`, `ug_require`, `_` and `debugPrint` are known to the type checker. They are documented in [main](../api/main.md) and [api](../api/api.md).

To check the setup, the wiki suggests a file `example.tl` in your mod with:

```teal
local function myFirstTealFunction()
	log.warning("Hello World!")
end
```

Ctrl+click or F12 on `warning` should jump to its definition ([`log.warning`](../base/scripts/log.md) (symbol: `log.warning`, see [symbol index](../index.tsv))).

### Adding definitions of your own mod

Your own type definitions go into `*.d.tl` files in your mod. Add the mod's folder to `include_dir` in `tlconfig.lua` and a `require` for its main definition file to the global definition file (`all_def.tl` or `workspace_def.tl`). The wiki's trial run uncomments `"example_mod_1"` in both files and adds `example_mod_1/example_mod_1.d.tl`:

```teal
global record MyFirstRecord
	myFirstField : string
end
```

`MyFirstRecord` can then be used as a type in `example.tl`. Since the workspace lives in the staging area, the entry `"example_mod_1"` is the mod folder next to `tlconfig.lua`.

The campaign mods keep their definitions in a separate `tealdef/` folder next to `content/`:

```
mods/release/urbangames_campaign_mission_01/tealdef/
├── urbangames_campaign_mission_01_def.d.tl
└── mission/mission_params_01.d.tl
```

`urbangames_campaign_mission_01_def.d.tl` contains a single line, `require "mission.mission_params_01"`, so this `tealdef/` folder is used as an include root, the same way `base/tealdef` is. With that layout, the `include_dir` entry is `"my_mod/tealdef"` and `all_def.tl` gets `require "my_mod_def"`.

## Where mods live

The game knows several mod sources. The Mod Hub (`base/content/gui/gui/menu/mod_manager_react_util.tl`) groups mods by `Mod.GameModDesc.modSource`, whose values are listed in the commented-out `ModSource` enum in `api/tealdef/api/type/mod.d.tl`:

| Source id | Shown in the game as | Folder |
|---|---|---|
| `StagingArea` | "My Mods" | `staging_area` in the user data folder |
| `UserMods` | "User Mod" | manually installed mods; the wiki doesn't give the path |
| `BuiltInMods` | "Built-In" | the official mods, shipped in `mods/release/` of the installation |
| `DLC` | "DLC" | shipped in `dlcs/` of the installation |
| a mod hub backend | the backend's name and icon, for example mod.io | subscribed mods, managed by the game |

The staging area is where you develop a mod and where you publish it from. The [publishing page](https://wiki.transportfever3.com/doku.php?id=modding:general:publishing#publish) gives the usual location for the Steam version on Windows:

```
C:\Program Files (x86)\Steam\userdata\<your Steam ID>\3493540\local\staging_area\
```

The user data folder on your system opens from the main menu under Settings > Advanced > Open User Data Folder, which calls [`app.openUserDataFolder()`](../api/app.md) (symbol: `App.openUserDataFolder`, see [symbol index](../index.tsv)). In the staging view, each mod has a button that opens its folder through [`ModPublishHelper.openFolder`](../api/api/type/modhub.md) (symbol: `Modhub.ModPublishHelper`, see [symbol index](../index.tsv)).

The Model Editor also needs this folder: on its first start you enter it as `userDataPath` in its configuration file ([Model editor](model-editor.md#setup) has the file locations).

The game identifies a mod by its `modId`, and a copy in the staging area wins over a published copy with the same id, so your working copy replaces the published version while you test ([install order](mod-structure.md#modjson)).

## A minimal mod

The wiki's "Demo Config Change" and the shipped mods `urbangames_no_costs` and `urbangames_sandbox` are the smallest kind of mod: a script that changes the base config. They need three files:

```
my_mod/
├── mod.json
├── _metadata/modinfo.json
└── content/mod.script.tl
```

(The shipped folders also contain `_content.json`, a list of the content files and archives. See [Mod structure](mod-structure.md#files-generated-by-the-game) for that file.)

Folder and id follow a few rules from the [mod definition page](https://wiki.transportfever3.com/doku.php?id=modding:general:moddefinition):

- The folder name has no function. The mod definition page allows `A-Z`, `a-z`, `0-9`, `_` and spaces, but the upload requirements ask for lowercase names without spaces for all files and folders, so use the `modId` as folder name.
- The `modId` may only contain `a-z`, `0-9` and `_`. The recommended form is `<author>_<modname>`.
- Unlike in the earlier Transport Fever games, the `modId` has no major version suffix such as `_3`. Version changes go into `revision`.

The official script mods have a `_1` suffix anyway (`urbangames_no_costs_1`); leave it out of your own ids ([mod.json](mod-structure.md#modjson) lists the cases).

`mod.json` declares the id and points to the script functions the game calls while loading. From `mods/release/urbangames_no_costs/mod.json`:

```json
{
    "dependencies": null,
    "incompatibilities": null,
    "modId": "urbangames_no_costs_1",
    "options": null,
    "params": null,
    "postRunScript": {
        "fileName": ""
    },
    "preRunScript": {
        "fileName": "urbangames_no_costs_1::/mod.script@preRunFn"
    },
    "revision": 1,
    "runScript": {
        "fileName": ""
    },
    "severityAdd": "None",
    "severityRemove": "None"
}
```

The keys with `null` or `""` can be left out. The wiki's minimal `mod.json` has only `modId`, `revision`, `severityAdd`, `severityRemove`, `visible` and `cosmetic`. All keys are described in [Mod structure](mod-structure.md#modjson).

`_metadata/modinfo.json` holds the text the player sees in the mod browser. The shipped mods put translation keys here. In your own mod, write the English text directly or add a `localization` block (see [modinfo.json](mod-structure.md#_metadatamodinfojson)):

```json
{
    "authors": [
        {
            "name": "Urban Games",
            "role": "CREATOR"
        }
    ],
    "description": "MOD_NO_COSTS_DESCRIPTION",
    "name": "MOD_NO_COSTS_NAME",
    "summary": "MOD_NO_COSTS_SUMMARY",
    "tags": [
        "Script Mod"
    ],
    "url": ""
}
```

`content/mod.script.tl` is the script that `"urbangames_no_costs_1::/mod.script@preRunFn"` refers to. The reference names the mod (`urbangames_no_costs_1::`), the file inside its `content/` folder without the `.tl` extension (`/mod.script`) and the function in the table that the file returns (`@preRunFn`):

```teal
local mod = {}

mod.preRunFn = function(captureParams, configDict : {{string, string}}, allModParams : {string : {string : integer}}, baseConfig : BaseConfig)
	baseConfig.noCosts = true
end

return mod
```

To make your own mod from this, copy the three files, pick a new `modId`, use the same id in the script references, and change what `preRunFn` does. [`BaseConfig`](../api/api/type.md) lists every field of the base config, the wiki's [game config page](https://wiki.transportfever3.com/doku.php?id=modding:scripting:baseconfig) gives defaults and meanings, and [Base config](scripting.md#base-config) shows where the base game sets and reads it.

The sketch below does what the wiki's demo does, plus a mod parameter: the player picks how hard trains brake, and the script writes the choice into `trainBrakeDeceleration` (wiki default `2.5`, lower values mean slower deceleration). `jdoe_soft_brakes` is a made-up id:

=== "mod.json"

    ```json
    {
        "modId": "jdoe_soft_brakes",
        "revision": 1,
        "severityAdd": "None",
        "severityRemove": "None",
        "cosmetic": false,
        "preRunScript": { "fileName": "jdoe_soft_brakes::/mod.script@preRunFn" },
        "params": [
            {
                "key": "brake_factor",
                "name": "brake_intensity_name",
                "tooltip": "brake_intensity_tooltip",
                "uiType": "Slider",
                "values": [ "50%", "75%", "100%" ],
                "numbers": [ 1.25, 1.875, 2.5 ],
                "defaultIndex": 2
            }
        ]
    }
    ```

=== "strings.json"

    ```json
    {
        "en": {
            "brake_intensity_name": "Brake Intensity",
            "brake_intensity_tooltip": "Adjust the global brake intensity."
        },
        "de": {
            "brake_intensity_name": "Bremsintensität",
            "brake_intensity_tooltip": "Passen Sie die globale Bremsintensität an."
        }
    }
    ```

=== "_metadata/modinfo.json"

    ```json
    {
        "name": "Soft Brakes",
        "summary": "Trains brake more gently.",
        "description": "Lowers the global train brake deceleration.",
        "authors": [ { "name": "jdoe", "role": "CREATOR" } ],
        "tags": [ "Script Mod" ],
        "url": ""
    }
    ```

=== "content/mod.script.tl"

    ```teal
    local mod = {}

    mod.preRunFn = function(captureParams, configDict : {{string, string}}, allModParams : {string : {string : integer}}, baseConfig : BaseConfig)
    	baseConfig.trainBrakeDeceleration = allModParams[getCurrentModId()].brake_factor
    end

    return mod
    ```

`params` declares one slider with three options. `defaultIndex` counts from 0 in `mod.json`, so `2` preselects "100%", the wiki's default value. The script finds the `numbers` entry of the chosen option under the parameter's `key` in `allModParams[getCurrentModId()]`. `name` and `tooltip` are translation keys whose texts are in `strings.json` at the mod root ([Translations](other-content.md#translations-in-a-mod-stringsjson)). No shipped mod has a `strings.json`, so that file follows the wiki's example. No shipped parameter has fractional `numbers`, so check in game that the script receives `1.875` and not a rounded value. Every field is explained in [Mod parameters](mod-structure.md#mod-parameters).

Every base train, tram and wagon model also sets its own `brakeDeceleration = 2.5` in its `landVehicle` block. The game files don't say how this per-model value and the global `trainBrakeDeceleration` combine. If the slider has no visible effect in a test, change the models in a `postRunFn` instead (see [Mod script hooks](mod-structure.md#mod-script-hooks)).

`cosmetic` is `false` because braking changes the simulation. The wiki asks modders to set `cosmetic: true` only for mods with no effect on gameplay, since it keeps achievements available.

The three mod script hooks (`preRunScript`, `runScript`, `postRunScript`), their arguments and the order in which the game calls them are described in [Mod script hooks](mod-structure.md#mod-script-hooks), and [Scripting](scripting.md#load-time-code) shows what each phase is used for.

## Testing a mod

Put the mod folder into the staging area. It then shows up under "My Mods" in the Mod Hub and in the mod list when you start a new game.

- Mods are activated per game. `app.startGame2` receives the list of mods, the game settings (`configDict`) and the mod parameters (`modParams`) ([App](../api/app.md)), and the New Game page in `base/content/gui/gui/menu/new_game_or_map_settings_page.tl` calls it. The wiki adds that the player can also pick mods and mod parameters when loading a savegame.
- `debugPrint(...)` "Prints text to the console/log file" ([main](../api/main.md) (symbol: `debugPrint`, see [symbol index](../index.tsv))) and prints tables too. The wiki's [API page](https://wiki.transportfever3.com/doku.php?id=modding:scripting:api) recommends the in-game console for trying out commands. The base scripts also use the global `log` table with `log.verbose`, `log.message`, `log.warning` and `log.error` ([log](../base/scripts/log.md)).
- Run the validator often, not only before publishing ([best practices](https://wiki.transportfever3.com/doku.php?id=modding:general:bestpractice#some_final_tips)). In the game, `ModPublishHelper.validate` "Validates a staging mod, this is an async operation ... Does not cook, only publish does." The result is a [`Modhub.ModValidatorResult`](../api/api/type/modhub.md) (symbol: `Modhub.ModValidatorResult`, see [symbol index](../index.tsv)) whose messages carry the file, the severity level and whether the issue only affects consoles. The Export button in the Mod Hub saves this report to a file. The checks are listed in [Mod structure](mod-structure.md#technical-requirements).
- Set `severityAdd` and `severityRemove` in `mod.json` to warn players before they add your mod to a savegame or remove it. The values are `None`, `Warning` and `Critical`; see [Mod structure](mod-structure.md#modjson).

!!! note
    The wiki lists "Ingame Tools" as coming soon. Neither the wiki nor the game files describe a hot-reload workflow or how to open the console. The "Console State" is mentioned in `api/tealdef/api/cmd.d.tl` without further detail.

## Publishing on mod.io

The official mod browser runs on mod.io and reaches Windows, macOS, Linux, PS5 and Xbox Series. The [publishing page](https://wiki.transportfever3.com/doku.php?id=modding:general:publishing) describes the workflow:

1. Put the mod into the staging area and start the game. The Mod Hub in the main menu then has a "My Mods" tab.
2. Select the mod. You can set the name, tags, cover and gallery images, visibility and a changelog message there.
3. Click Upload. The game validates the mod first. If the mod also meets the stricter console rules, the game prepares a version with adapted textures, and that version is approved for PS5 and Xbox automatically.
4. After the upload, the mod opens on mod.io. Profile, media, file history, dependencies, preview access for beta testers, team members, reports and statistics are edited there.

After the first upload, the game writes `_metadata/mod.io_fileid.txt`. It holds the mod.io id and tells the game which entry to update. If you lose it, create it again with the id from the info box on the mod's mod.io page. When the update target is recognised, the publish view says "The existing mod on mod.io will be updated." next to the Update button.

For an update, raise `revision` in `mod.json` and write a changelog message. If an update breaks existing savegames, the wiki recommends a new `modId` and a separate mod instead. Name and description from `modinfo.json` overwrite whatever you edited on mod.io, so keep a copy of such edits.

The rules for publishing: only content you have the rights to, no offensive content, mod.io's terms apply, and mods must be free. Donations, commissions and supporter programs such as Patreon are allowed, with early access for supporters limited to 31 days. Selling mods or keeping them behind a permanent paywall is not. Files from Transport Fever, Transport Fever 2 and Transport Fever 3 may be used in Transport Fever 3 mods if you name the source.

Other sites, such as the filebase on transportfever.net, take uploads by hand under their own terms. Put the download URL into `modinfo.json`'s `url` so players can find the mod if it is missing.

The wiki's [guidelines](https://wiki.transportfever3.com/doku.php?id=modding:general:guidelines#mod_metadata) also cover the presentation: a title-case name of at most 32 characters, a 1920 x 1080 cover of at most 8 MB with no text, an English description, and tags.

Official wiki: [Modding manual](https://wiki.transportfever3.com/doku.php?id=modding:start), [Introduction](https://wiki.transportfever3.com/doku.php?id=modding:introduction), [Mod definition](https://wiki.transportfever3.com/doku.php?id=modding:general:moddefinition), [Mod parameters and scripts](https://wiki.transportfever3.com/doku.php?id=modding:general:modscripts), [Syntax](https://wiki.transportfever3.com/doku.php?id=modding:general:syntax), [External tools](https://wiki.transportfever3.com/doku.php?id=modding:tools:external), [Model Editor](https://wiki.transportfever3.com/doku.php?id=modding:tools:modeleditor), [Publish a mod](https://wiki.transportfever3.com/doku.php?id=modding:general:publishing), [Guidelines and requirements](https://wiki.transportfever3.com/doku.php?id=modding:general:guidelines), [Best practices](https://wiki.transportfever3.com/doku.php?id=modding:general:bestpractice).
