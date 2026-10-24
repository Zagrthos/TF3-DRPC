---
name: tf3-modding
description: Reference and workflow for Transport Fever 3 mods (Lua/Teal game scripts, mod.json, constructions, models, GUI, missions). Use when creating, changing or debugging a TF3 mod, or when code calls the TF3 scripting API (api.engine, api.cmd, api.gui, api.res, api.type, api.util) or the base game's script libraries.
---

# Transport Fever 3 modding

This skill holds an offline copy of the TF3 modding reference from https://tf3-docs.transportfever.net:
every record, function, field and enum the game's Teal definition files declare, with
descriptions, how to call each function, and where the base game uses it. The guides
explain how a mod is put together.

## Look things up before you write code

1. Search `references/index.tsv` for the name you need. It has one row per symbol:
   `qualname`, `kind`, `definition` (file and line in the game folder), `signature`,
   `call_as`, `base_game_uses` and `description`. For example:
   `grep -i "getComponent" references/index.tsv`.
2. Open the module file for the row's `definition` path: `api/tealdef/api/engine.d.tl` is
   `references/api/api/engine.md`, `base/tealdef/scripts/vec3.d.tl` is
   `references/base/scripts/vec3.md`.
   It lists the record's fields and functions, parameters, notes, examples and one line of
   real base-game code per symbol.
3. Read the guide for the task before you start a new kind of content:

   | Task | Guide |
   |---|---|
   | Set up tools, first mod, publishing | `references/guides/getting-started.md` |
   | mod.json, dependencies, script hooks, paths, translations | `references/guides/mod-structure.md` |
   | Reading the game state, commands, game scripts, modifiers | `references/guides/scripting.md` |
   | Constructions (`.con`), build menu | `references/guides/constructions.md`, `constructions-modular.md` |
   | Models, meshes, materials, textures, model editor | `references/guides/models-and-assets.md`, `model-editor.md` |
   | Vehicles and repaints | `references/guides/vehicles.md`, `vehicles-advanced.md` |
   | Tracks, streets, bridges, signals | `references/guides/infrastructure.md` |
   | Climates, terrain, landscape assets, animals | `references/guides/environment.md` |
   | Cargo, translations, names, people, sounds | `references/guides/other-content.md` |
   | GUI, React recipes, plugins | `references/guides/gui.md` |
   | Missions and campaigns | `references/guides/missions.md` |

## Use the player's game folder as the source of truth

The game ships its own definitions and content. Ask for the install folder if you don't
know it (on Steam usually `steamapps/common/Transport Fever 3`).

- Definitions: `<game>/api/tealdef/**/*.d.tl` and `<game>/base/tealdef/**/*.d.tl`. Check a
  signature there when the reference and your code disagree.
- To find the code behind a text from the game, search the `in_game` and `in_game_de`
  columns of `references/index.tsv`, for example `grep -i "time to celebrate" references/index.tsv`.
- Base game content is packed in zip files (`<game>/base/content/<name>.zip`). A path such
  as `base/content/stations/air/...` in the reference means the file inside
  `base/content/stations/air.zip`. Inside the archive the path starts again with the archive
  name, so `base/content/stations/air/air/helipad.con.lua` has the game path
  `::/stations/air/helipad.con`. Unzip one to a temporary folder to read real examples.
- The official campaign mods in `<game>/mods/release/` are complete examples of mod folders.
- `<game>/vscode-template/` is the official setup for type-checking with Teal
  (`tlconfig.lua` and `all_def.tl`).

## The official wiki

Urban Games documents the concepts (mod definition, resource types, constructions, vehicles,
infrastructure, scripting) at https://wiki.transportfever3.com/doku.php?id=modding:start. The
guides link the matching wiki pages. When this reference and the wiki disagree, check the
game files, and tell the user which source says what.

## Rules

- Only call functions and fields that exist in the reference or the definition files. If
  something isn't there, say so instead of guessing an API from Transport Fever 2.
- Descriptions come from two sources. Text from the game files is copied as is. The rest
  was written for the site from how the base game uses the symbol. Where the definitions
  and the game's code disagree, the description says so; follow the code.
- Base-game patterns beat invented ones. Before you write a construction, GUI recipe or
  game script, find a base-game file that does something similar (the `base_game_uses`
  column and the "used ... e.g." lines point to them) and follow its structure.
- Methods marked "(method)" are called with a colon on an instance (`result:isSuccess()`).
- The reference is from a specific game build. After a game update, trust the
  definition files in the game folder over this copy.

## Keep the skill current

The `VERSION` file in this folder holds this copy's version (for example `2026.10.08.1`).
Once per session, before the first lookup, fetch
https://tf3-docs.transportfever.net/agents/skill-version.txt if you have web access. If the
version there is different from `VERSION`:

1. Use the online files for this session. https://tf3-docs.transportfever.net/llms.txt lists them;
   they have the same layout as `references/` (`.txt` instead of `.md`), and the index is
   https://tf3-docs.transportfever.net/agents/index.tsv.
2. Tell the user that a newer version of the skill is available and offer to update it:
   download https://tf3-docs.transportfever.net/downloads/tf3-modding-skill.zip and replace this
   folder with the `tf3-modding` folder from the zip. Replace it only after the user agrees.

Without web access, keep using the local copy.
