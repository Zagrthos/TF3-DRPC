
# Model editor

The Model Editor ships with the game. It imports `.fbx` files into TF3 models, edits model metadata and materials, previews animations, particles, labels, cargo and ageing, checks models for errors and renders the vehicle icons. This page summarises the official [Model Editor](https://wiki.transportfever3.com/doku.php?id=modding:tools:modeleditor) wiki page; the file formats it writes are described in [Models and assets](models-and-assets.md).

## Setup

Start `ModelEditor.bat` (Windows), `ModelEditor.sh` (Linux) or `ModelEditor.command` (macOS) in the game's installation directory. Two windows open: a console with debug output and the editor itself.

On the first start the editor writes a configuration file and asks you to fill in your user data folder as `userDataPath`. The file is in:

| System | Folder |
|---|---|
| Windows | `%APPDATA%/Transport Fever 3/` |
| Linux | `~/.local/share/Transport Fever 3` |
| macOS | `/Users/<username>/Library/Application Support/Transport Fever 3/` |

The game shows the user data folder under main menu > Settings > Advanced > Open User Data Folder. Write the path with forward slashes only, or the editor may not start. The editor finds the staging area (your mods in progress) and the `screenshots/` folder from this path. Starting it once also writes a Teal workspace (`tlconfig.lua`, a global definition file and `.vscode/extensions.json`) into the staging area; [Getting started](getting-started.md#the-development-workspace) explains the files and the name differences between the wiki and the shipped `vscode-template/`.

## The window

The toolbar has five groups: open and save, render mode and LOD, animation preview, the detail windows (model, metadata, materials, animations), and validation, screenshots and options. Most buttons stay disabled until a model is loaded.

In the viewport, drag with the left mouse button to rotate, with the right button to pan, and scroll to zoom. Ctrl plus left drag rotates the skybox.

### Opening and saving

The open dialog browses the base game and every mod in the staging area, with a search over all subfolders of the selected folder and a list of recently opened models. Saving writes the model into the mod chosen in the dropdown next to the save button, without asking before it overwrites files. The save dropdown limits what is written:

| Option | Writes |
|---|---|
| MODEL | the `.mdl` |
| MESH | `.msh` and `.msh.blob`, only for freshly imported meshes |
| MATERIAL | the `.mtl` files |
| ANIMATION | `.ani` files, only for freshly imported animations |

### Viewport modes and animation preview

The render buttons switch between textured, plain white and wireframe. The LOD selector is either Auto (by zoom, as in the game) or a fixed level. The animation group plays one animation event of the model, once or looped.

### Model window

The Model window shows the model path, buttons to recalculate the bounding box and the simulation extent, and fields to edit the bounding box. One tab per LOD sets `visibleFrom`, `visibleTo` and the texture LOD base flag. Below is the node tree with names and mesh files. Ticking a node or hovering it shows that mesh alone, which helps to find parts. A button per node opens a transform dialog. The last columns count materials and animations per node; Extended View lists them.

### Metadata window

The Metadata window shows the whole `metadata` table as a tree. The + button adds a block, the gear menu deletes all metadata, and Reset returns to the loaded state. Shift-click on an arrow unfolds a block completely. Each field and block can be reset or removed, list entries can be added or duplicated, and the gear button on the left of a field offers templates where they exist.

The field names, types and templates come from `base/content/model_editor/model_editor/meta_metadata.lua` in the game files, e.g. `["transformatorConfig.skipFromLod"] = { name = "Skip LODs from", type = "INTEGER", order = 6 }`.

### Materials window

The Materials window lists every material of the model with its type prefix. Materials referenced by relative path are highlighted, because absolute ones are probably shared with other models and should be edited with care. Hovering a material hides the rest of the geometry. For the selected material you can change `order`, convert it to another material type, reset it, and edit its properties; texture paths can be typed or picked from a file dialog. Templates exist for some properties. In the game files they are in `base/content/model_editor/model_editor/meta_material.lua`: "Default dirt/rust" for `dirt_rust`, "Interior / Exterior" and "Interior / Exterior Glass" for `alpha_test`, and "Interior" and "Exterior" for `light_receiver`.

## Validation

The validate button in the toolbar checks the loaded model and stops at the first problem; run it again after each fix. The Validate tool under Options > Tools checks many models at once. The wiki lists the messages it can show. They cover:

| Area | Checks |
|---|---|
| `loadIndicator` | `cargoBay.childId` names a missing node; cargo bay box too small; a configuration references a missing slot |
| `roadVehicle`, `railVehicle` | `fakeBogies` must have one list per LOD (empty lists allowed) or be empty |
| `waterVehicle` | `area` too large for `availPower`, so the ship never reaches top speed |
| `airVehicle` | `wheels` and `axles` must both be set; landing gear with zero distance between gears or a ground pitch that is too low or too high |
| `seatProvider` | a seat names a missing node |
| `transportVehicle` | empty `loadConfigs`; a cargo entry references a missing load indicator or seat; negative capacity; unknown cargo type or cargo class |
| `categoryList` | unknown cargo type or cargo format |
| Mesh | the material type needs a vertex attribute the mesh lacks, usually `uv1` or skinning data |
| Material | missing texture file; number of materials or skin materials differs from the submesh count (the editor patches it); skinned mesh with an unskinned material |
| Animation | number of times and transforms differ; no keyframes; first keyframe after 0 |

## Options

The Options window keeps its settings between sessions. Most tabs have a Reset to default button.

| Tab | Settings |
|---|---|
| Rendering | overlays for axes, nodes, node names, bounding box, extent box and collider; locators for seats, lights, particle emitters, labels (with demo texts), ship water line, cargo bays, cargo slots, axles and wheels, bogies, fake bogies; vertex normals and tangents |
| Config | auto reload on file change (use On Focus if textures load half-written from your image editor), animation speed, LOD scale, a reference model for comparison, colour for colour-blend materials, age for dirt and rust; passenger and cargo amount, random seed and cargo type per compartment; particle preview with a simulated speed and a tag filter; label test texts |
| Graphics | texture quality, anti-aliasing, bloom, SSR, SSAO, shadows, legacy (TF2) HDR rendering, VSync; some need a restart |
| Environment | field of view and background colour; ground and sky on or off, the environment, time of day, day of year, brightness, sun intensity, and day and night ambient colour, HDR exposure and contrast |
| Icons | screenshot format (the game needs `.tga`), stage models for road and rail vehicles, environment and animation state for screenshots, icon width and pixels per metre, 3D screenshot camera, which images the screenshot button makes, bulk generation for a folder |
| Im-/Export | overwrite meshes, animations, materials, `PHYS_TRANSPARENT_DIFFUSE` placeholder materials and textures on re-import; Blender coordinate system; `.tga` to `.dds` conversion quality, or skip compression of existing textures |
| Tools | target mod and bulk folder; Rename (search and replace in file names); Convert (TF2 `res` models to the TF3 `content` layout, `.mtl` files with the legacy flag set, metadata that still needs new values added by hand); Copy (model into another mod); Validate (many models) |

The stage models and environments for screenshots are in the game files under `base/content/model_editor/model_editor/stage/` (for example `stage_track_standard.mdl`, `stage_street_country_new.mdl`) and `.../environments/` (`vehicles.env.lua`, `vehicles_3d.env.lua`, `temperate_model_editor.env.lua`, `substance_3d_model_editor.env.lua`).

### Icons and screenshots

The Screenshot button in the toolbar renders the UI icons and puts them in the right place in the mod. It can also write colour-blend masks, perspective views, large and small orthographic views and a viewport shot into `screenshots/` under `userDataPath`. Bulk Generate does this for every model in a folder and can take a long time. [Best practice](https://wiki.transportfever3.com/doku.php?id=modding:general:bestpractice) asks mods to ship all icons, including the cblend masks.

## Importing

The editor imports three kinds of files: `.import.lua` templates, `.fbx` models and `.fbx` construction data. FBX files from Modo and Blender are supported; for Blender, turn on Use Blender Coordinate System in the Im-/Export tab.

### Templates (.import.lua)

An `.import.lua` file dropped into the editor before the models sets defaults for the following imports. It defines up to three functions in a `fbxLoader` table:

```lua
fbxLoader = {
	predictMaterial = function(params)
		return "::physical_nrml_map_op_uv1_ao_op_mask.mat.lua"
	end,
	overrideProperties = function(params)
		return params.properties
	end,
	getMetadataMap = function()
		return { availability = { yearFrom = 1900, yearTo = 0, }, }
	end,
}
```

`predictMaterial` runs per material and returns a material type file or `nil`. Its `params` has `fbxPath` (folder of the `.fbx`), `fbxName` (file name without `_lodX` and extension), `fbxMaterialName` and `textures` (texture files assigned in the FBX). `overrideProperties` gets the same plus `materialType` and `properties` (the material parameters as in an `.mtl`) and returns the changed `properties`. `getMetadataMap` takes no arguments and returns the `metadata` table for the models imported afterwards. The wiki has a downloadable window material override and a bus metadata template.

### FBX models

Drag an `.fbx` file into the editor to import it, or a folder to import all files in it. One `.fbx` holds one LOD and ends in `_lod0.fbx`, `_lod1.fbx` and so on; `_lod0` is required. All LODs of a model go into one folder, and the folder name sets where the model lands in the mod:

| Folder name | Result |
|---|---|
| `vehicle-bus-my_bus/` | `vehicle/bus/my_bus/my_bus.mdl` |
| `vehicle-train+my_unit_a/` | `vehicle/train/my_unit_a.mdl`, so several models can share one folder, e.g. a multiple unit |

#### Nodes and meshes

Node names have the form `<elementname>[#<comment>]`. The element name becomes the node name and the mesh name; it may not contain `|`, `%` or `#`, and the editor adds the `_lodX` suffix itself. Everything after `#` is ignored, which removes Blender's `.001` suffixes and lets two nodes share a name: `middle_interior` and `middle_interior#borrowed_from_middle` both become node `middle_interior` with mesh `middle_interior_lodX.msh`. In Blender the object name is the node name and the mesh data name is the `.msh` file name.

The element name can also be a path, which reuses an existing mesh or creates it at that place:

| Element name | Mesh |
|---|---|
| `w1` | `msh/w1_lodX.msh` next to the model |
| `/vehicle/waggon/bay_05/msh/w1` | `vehicle/waggon/bay_05/msh/w1_lodX.msh` in the current mod, created if missing |
| `::/vehicle/waggon/bay_05/msh/w1` | the same path in the base game |
| `master_mod::/vehicle/waggon/bay_05/msh/w1` | the same path in mod `master_mod`, which must be a dependency in `mod.json` |

#### Materials

Material names have the form `<materialname>[|<prefix>][#<comment>]`. The name works like a mesh name: a plain name creates the material next to the model, and an absolute path such as `/infrastructure/tracks/standard/mat/ballast` reuses that material or creates it there. `|<prefix>` picks the material type by its short code, e.g. `window|PNOL` for `PHYSICAL_NRML_MAP_OP_LIGHT`. The prefixes are listed in [Models and assets](models-and-assets.md#material-types) and in each type's `rendering/*.mat.lua` (`prefix = "PNOL"`).

#### Textures

The importer finds the albedo texture if it is assigned to the material in the FBX or has the material's name plus an albedo suffix. All other maps must have the albedo texture's base name plus one of these suffixes:

| Map | Suffixes |
|---|---|
| `map_albedo` | `_alb`, `_albedo` |
| `map_albedo_opacity` | `_albo`, `_albedo_opacity` |
| `map_normal` | `_nrm`, `_normal` |
| `map_metal_gloss_ao` | `_mga`, `_metal_gloss_ao` |
| `map_cblend_dirt_rust` | `_cdr`, `_cblend_dirt_rust` |
| `map_cblend` | `_cblend` |
| `map_ao` | `_uv1_ao` |
| `map_id` | `_id`, `_emissive_ID_lod1` |
| `map_lgt` | `_e` |
| `map_op_1`, `map_op_2` | `_op1`, `_op2` |

The base game textures use the same short suffixes (`_mga`, `_nrm`, `_alb`, `_albo`, `_id`, `_e`, `_cdr` are the most common), but also `_albedo`, `_normal`, `_ao`, `_opacity` and `_texture`. When the model is saved, `.tga` textures are converted to `.dds`.

#### Animations

Keyframe animations on the meshes become `.ani` files. In Blender, store them as NLA strips named after the animation event. Several strips (one per animated mesh) can serve the same event when the `.001` suffixes are cut off with `#`.

#### Metadata from the FBX

| FBX element | Becomes |
|---|---|
| Point, Spot or Directional light (Blender: Point, Spot, Sun) | a light source of type `POINT`, `SPOT` or `PARALLEL` with `lightId` from the object name, `transf`, `color`, `strength` (Blender power × 100), `angleMin`/`angleMax` from Blender's Angle and Blend, and the parent node |
| locator `…\|seat_<anim>` or `…\|seat_crew_<anim>` | a passenger or crew seat; `<anim>` is `idle`, `sitting`, `driving_upright` or `driving`. Meshes under it are dropped. |
| locator `…\|draft_animal_<mdl>[(<index>)]` | a draft animal, e.g. `something\|draft_animal_mymod::/animal/draft_horse/draft_horse.mdl(2)#something` for the second colour config |
| locator with `emitter` in its name | a particle emitter with `particleId`, `position` and `velocity` (from the x scale); `smoke_bright` or `smoke_dark` in the name adds standard smoke values |
| mesh `…\|bounding_box`, `…\|extent`, `…\|collider_list` | the bounding box, the extent or a box collider; the mesh itself is dropped |
| two nested locators `…\|label` | a label: the parent is the lower left corner, the child the upper right. Both need the same rotation, must lie in one x/y plane and face along z. |

### Bulk import

For many models at once, e.g. bridges and tunnels, drag a `.txt` or `.log` file into the editor:

```
Scene;<scenename>
ModRefName;<modid>
DeleteTargetFolders;<False or True>
Path;<path to folder with fbx>
Path;<another path to folder with fbx>
```

`ModRefName` is the target mod. `Scene` is an optional note about the source file. `DeleteTargetFolders = True` overwrites everything in the mod. Each `Path` line is one folder with `_lod0.fbx`, `_lod1.fbx` and so on.

### Construction data

An `.fbx` can also fill a construction script. Drop it onto the box in the Im-/Export tab. Its top-level `Empty` nodes are named after the target script, e.g. `/industries/brewery/brewery.script`, and the script must contain the two marker lines below; the import replaces everything between them:

```lua
--Begin Generated

--End Generated
```

Under that, `Empty` nodes named `<name>.id` (with an optional `#comment`) form the top levels the construction util expects, such as `base`, `static` or `level1`. Inside them:

| Node name | Meaning |
|---|---|
| `collider.curve[#comment]` | the construction collider |
| `less.mesh`, `greater.mesh`, `equal.mesh` | terrain alignment: upper limit, lower limit, exact |
| `<mod>::<path>/<name>.mdl[#comment]` | a placed model, with the node's transform |
| `<key>.grp` | a random asset group from `/scripts/construction/asset_util.tl` |
| `seed(<n>)\|…` | shared random seed, e.g. `seed(123)\|era_a_garden_chair.grp` |
| `align\|…` | keep the height relative to the construction origin instead of following the terrain |
| `ignore(<percent>)\|…` | show the model only sometimes, e.g. `ignore(50)\|::/assets/buildings/ind/boxes/wood_box_long.mdl#` |

`align`, `seed` and `ignore` also work on an `Empty` parent and then apply to all its children. How constructions use this data is in [Constructions](constructions.md).

## Notes from the game files

- The wiki's setup text says the start scripts are in the installation directory "of Transport Fever 2" for Windows; the scripts belong to the TF3 installation.
- The editor runs the base game's model loading chain from its own mod script, `base/content/model_editor/model_editor/editor_base_mod.lua` (`toCompartmentList`, `addTransformatorConfig`, `makeLoadIndicators`, `sortLods`, `addTextureLods`, ...), so a model looks the same in the editor as in the game. As in the game, `addTransformatorConfig`, `sortLods` and the `createAnimationEvents*` functions only convert models older than `version = 2`, so a new model needs those blocks in their final form (see [Vehicles](vehicles.md#which-metadata-blocks-a-vehicle-needs)).
- The material field list in `meta_material.lua` includes a `compressionAllowed` sampler flag that the wiki's material page does not describe.

Official wiki: [Model Editor](https://wiki.transportfever3.com/doku.php?id=modding:tools:modeleditor), [External tools](https://wiki.transportfever3.com/doku.php?id=modding:tools:external), [Best practice](https://wiki.transportfever3.com/doku.php?id=modding:general:bestpractice), [Material definition (.mtl)](https://wiki.transportfever3.com/doku.php?id=modding:general:resourcetypes:mtl).
