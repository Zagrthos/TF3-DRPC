
# Guides

These guides explain how Transport Fever 3 mods are put together. They are based on the files that ship with the game (the Teal type definitions of the engine API, the base game's own scripts and content, the official VS Code template, the campaign mods and the DLC packages) and on the [official modding wiki](https://wiki.transportfever3.com/doku.php?id=modding:start). Where the wiki and the game files disagree, the guide gives both and names the source. Where neither answers a question, the guide says so instead of guessing.

The [API reference](../index.tsv) is generated from the same definition files. The guides link into it whenever a type or function comes up.

## How the site is organised

| Section | Content |
|---|---|
| Guides (this section) | Hand-written explanations with excerpts from the game files |
| [Reference: Engine API](../api/api.md) | `api.engine`, `api.cmd`, `api.gui`, `api.res`, `api.type`, `api.util`, `api.modhub`, `app` and the globals from `main.d.tl` |
| [Reference: Base game scripts](../index.tsv) (symbol: `base-game-scripts`, see [symbol index](../index.tsv)) | Definitions of the base game's Lua/Teal libraries (construction utilities, React GUI framework, mission framework, ...) |
| [Globals A–Z](../index.tsv) | Every global name in one list |

The guides in reading order:

1. [Getting started](getting-started.md): editor setup with the official VS Code template, where mods live, a minimal mod, testing and publishing.
2. [Mod structure](mod-structure.md): `mod.json`, `_metadata/modinfo.json`, dependencies, the mod script hooks, file paths and generic resources.
3. [Scripting](scripting.md): the GUI and engine states, reading entities and components, commands, game scripts, the base config, modifiers and file filters.
4. [Constructions](constructions.md): `.con` files, the `updateFn` result, construction types, the build menu and ground textures; [modular constructions](constructions-modular.md) covers slots, modules, templates and a minimal modular construction.
5. [Models and assets](models-and-assets.md): `.mdl`, `.msh`, `.mtl`, model metadata and textures; the [model editor](model-editor.md) page covers the in-game tool.
6. [Vehicles](vehicles.md): vehicle metadata, the vehicle types and repaints; [advanced topics](vehicles-advanced.md) covers transformators, reversible trains, vehicle store groups and fake bogies.
7. [Infrastructure](infrastructure.md): tracks, streets, edge add-ons, bridges, tunnels, signals, crossings and traffic lights.
8. [Environment](environment.md): climates, terrain generators, terrain materials, landscape assets and animals.
9. [Cargo, names, people and sounds](other-content.md): cargo types, translations, name sets, characters, playlists and sound sets.
10. [GUI](gui.md): the React-style framework the base game GUI is built with, plugins and recipe replacement.
11. [Game mechanics](game-mechanics.md): the base game scripts, notifications, subsidies, company perks and economies.
12. [Missions](missions.md): the mission framework and a walk-through of campaign mission 01.

## What a TF3 mod consists of

A mod is a folder with a `mod.json`, a `_metadata/modinfo.json` with the texts and images for the mod browser, and a `content/` folder. The game loads models, vehicles, constructions, scripts and the in-game windows from files in `content/`. The base game itself has the same layout: `base/mod.json` lists the game settings (map size, town density, costs, ...) as mod parameters and points to `::/base/mod.script` for its setup code.

Most content files are Lua scripts that return a table from a `data()` function. A construction (`.con.lua`), a model (`.mdl`), a material (`.mtl`), a game script (`.gs.lua`) or a generic resource (`.res.lua`) all follow this pattern. Behaviour lives in `.script.lua` or `.script.tl` files, and content files point at functions inside them with references such as `"small_stops.script@updateFn"`.

Scripts can be written in Lua or in [Teal](https://github.com/teal-language/tl), a typed dialect of Lua. The base game uses both: its content has 813 Teal (`.tl`) files, 151 of them `.script.tl`, next to 3,611 plain Lua files, plus 316 `.d.tl` definition files. The game bundles the Teal compiler (`base/content/base/base/tl.lua`), and the campaign mods ship `.tl` sources directly, so Teal files are loaded without a separate build step. The `.d.tl` files are type definitions only; they are what the VS Code template and this site's reference use.

## The script states

Scripts run in separate script states: the engine state runs the simulation, the GUI state runs the user interface, and both can read the whole engine through `api.engine`. Changes always go through commands. The [scripting guide](scripting.md#the-script-states) explains the states and which code runs where.

## Further reading

The [official modding wiki](https://wiki.transportfever3.com/doku.php?id=modding:start) by Urban Games covers the same topics, and every guide ends with links to its matching wiki pages. Some wiki pages (base config, modifiers and filters) still describe Transport Fever 2; the guides point out where the TF3 files differ.

!!! note "About the source paths"
    Excerpts are labelled with their path inside the unpacked game files. `api/tealdef/...` and `base/tealdef/...` are the definition folders of the game installation. `base/content/...`, `mods/release/<mod>/...` and `dlcs/<dlc>/...` are content folders. The game ships most content in zip archives; in the unpacked tree each archive became a folder of the same name, which is why paths such as `base/content/scripts/scripts/gamescript.d.tl` repeat a folder name. The [mod structure guide](mod-structure.md#how-content-is-packaged) explains the mapping.
