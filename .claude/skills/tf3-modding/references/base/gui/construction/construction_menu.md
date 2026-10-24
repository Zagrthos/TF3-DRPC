# gui.construction.construction_menu

Definition file: `base/tealdef/gui/construction/construction_menu.d.tl` (in the game folder).

Shared types of the construction menu: menu tabs and categories, menu entries
(`ConstructionDefinition`), their parameters (`ConstructionMenuParam`), the parameter API of the
menu, HUD icon configuration and custom tool actions.

## ConstructionMenu (enum)

```teal
global enum ConstructionMenu
```

Top-level tab of the construction menu. Menu categories (`menu_category` resources) name the tab they belong to.

- `"ROAD"`: Road transport buildings and tools (stations, depots; categories `road_buildings`, `road_tools`, `road_assets`).
- `"RAIL"`: Rail buildings (category `rail_buildings`).
- `"WATER"`: Water transport buildings, tools and assets.
- `"AIR"`: Air transport buildings, tools and assets.
- `"ROADS"`: Streets and street tools (categories `roads_small`, `roads_medium`, `roads_large`, `roads_country`, `roads_highway`, `roads_tools`, `street_constructions`).
- `"TRACKS"`: Tracks, signals and rail tools (categories `tracks`, `rail_signals`, `rail_tools`, `rail_constructions`, `rail_assets`).
- `"WAREHOUSE"`: Warehouses (category `warehouses`).
- `"PERKS"`: Company perks (categories `perks_mechanics`, `landmarks`, `prospections`).
- `"LANDSCAPING"`: Terrain, ground, vegetation, assets and town buildings (categories `landscaping_*`, `town_buildings_*`).
- `"TOWN"`: Town tools (category `town_tools`).
- `"INDUSTRY"`: Industries (categories `industries`, `industries_assets`).
- `"BULLDOZER"`: The bulldozer.
- `"MODULES"`: Modules of a modular construction (categories `modules_*`).
- `"MODULE_BULLDOZER"`: The module bulldozer.

## ConstructionMenuCategory (record)

```teal
global record ConstructionMenuCategory
```

Data of a `menu_category` resource, a sub tab of a construction menu tab.

Example:
```lua
-- base/content/gui/gui/construction/menu/modules_categories/landing.res.lua
function data()
	return {
		type = "menu_category",
		data = {
			category = "modules_landing",
			order = 4000,
			name = _("Landing"),
			icon = resolve("landing.tga"),
			menu = "MODULES",
		}
	}
end
```

- `category : string`: Category id; menu entries reference it in `menuCategory.categories[].category`.
- `menu : ConstructionMenu`: Construction menu tab the category is shown in.
- `name : string`: Display name of the category.
- `icon : ResName`: Icon of the category tab.
- `order : integer`: Sort key within the menu tab (ascending); categories without `order` come after, sorted by name.

## ConstructionAvailability (record)

```teal
global record ConstructionAvailability
```

Years in which a construction menu entry is available.

- `yearFrom : integer`: First year the entry is available; `0` means no lower limit.
- `yearTo : integer`: Year from which the entry is no longer available; `0` means no upper limit.

## ConstructionMenuParamsAPI (record)

```teal
global record ConstructionMenuParamsAPI
```

API of the parameter list of the construction menu (side window or bottom bar), passed to parameter callbacks such as `ConstructionMenuParam.onChangeFn`.

Notes:

Parameter values are stored per category index. Parameters with `preserveValueInAllCategories`
are stored under the category index `-1`.

- `setActiveDefinition : function(definition : ConstructionDefinition, category : integer, categoryId : string) : {string : number}`: Makes a definition the active one, resets parameters flagged `resetOnCategoryChange`/`resetOnDefinitionChange` as needed, and returns the resulting parameter values.
    - param `definition`: Menu entry to activate.
    - param `category`: Index of the category (tab).
    - param `categoryId`: Id of the category; `"bulldozer"` and `"module-bulldozer"` defer the category reset until another category is selected.
    - returns: Parameter values by key.
- `getParamsForDefinition : function(definition : ConstructionDefinition, category : integer) : {string : number}`: Computes the parameter values a definition would get in a category, without activating it.
    - returns: Parameter values by key.
- `getStoredValues : function(category : integer) : {string : number}`: Returns the stored parameter values of a category.
    - param `category`: Category index, or `-1` for values preserved in all categories.
    - returns: Values by parameter key, or `nil` if none are stored.
- `changeParam : function(index : integer, value : number)`: Sets the value of a parameter of the active definition.
    - param `index`: Position of the parameter in the current parameter list.
    - param `value`: New value (option index, or slider value).
- `setParamDisabled : function(index : integer, disabled : boolean)`: Disables or enables a parameter in addition to its own `checkEnabledFn`. Cleared when another definition is activated.
    - param `index`: Position of the parameter in the current parameter list.
    - param `disabled`: `true` to disable the parameter.
- `isParamActive : function(key : string) : boolean`: Tells whether the parameter with the given key exists for the active definition and is neither hidden nor disabled.
- `getParam : function(index : integer) : ConstructionMenuParam`: Returns the parameter at a position of the current parameter list, or `nil`.
- `getCategory : function() : integer`: Returns the index of the current category.
- `getParamByKey : function(key : string) : ConstructionMenuParamInfo`: Looks up a parameter of the active definition by key.
    - returns: Its position, definition and current value, or `nil` if the active definition has no such parameter.
- `updateVisibility : function()`: Shows or hides the parameter window depending on whether there are parameters to show (no effect for the bottom bar).
- `getCurrentParams : function() : {string : number}`: Returns the current parameter values by key.
- `onInfoPopupVisibilityChange : function(visible : boolean)`

## ConstructionMenuParam (record)

```teal
global record ConstructionMenuParam
```

A parameter of a construction menu entry, shown in the parameter window or the toolbar.

Notes:

Script parameters of construction descriptions are converted into this type by
`ConstructionReactUtil.convertScriptParamsToMenuParams`; built-in tools create them directly.
A parameter's value is a 1-based index into `values` (or `numbers`).

- `new : function() : ConstructionMenuParam`: Creates a new, empty `ConstructionMenuParam`.
- `key : string`: Key of the parameter in the parameter value table.
- `name : string`: Display name.
- `tooltip : string`: Tooltip of the parameter.
- `values : {string}`: Display values (or image paths for icon buttons), one per option.
- `numbers : {number}`: Numeric values for slider parameters, used instead of `values`.
- `defaultIndex : Index`: Index of the default value.
- `uiType : ScriptParamType`: Widget type of the parameter.
- `iconStyle : ScriptParamUtil.ParamForUi.IconStyle`
- `location : ScriptParamLocation`: Where the parameter is shown (parameter window or toolbar).
- `displayMode : ScriptParamDisplayMode`: Display mode of the parameter widget.
- `yearFrom : integer`: First year the parameter is available; `0` means no lower limit.
- `yearTo : integer`: Year from which the parameter is no longer available; `0` means no upper limit.
- `group : string`: Group name; consecutive parameters of the same group are shown together, with a separator between groups.
- `tooltips : {string}`: Tooltip per value.
- `onChangeFn : function(newValue : number, paramsApi : ConstructionMenuParamsAPI, oldValue? : number)`: Called after the parameter's value changed.
- `checkEnabledFn : function(params : {string : number}) : ConstructionMenuParam.CheckEnabledResult`: Decides from the current parameter values whether the parameter is enabled, disabled or hidden. For script parameters it calls the parameter's `checkEnabledScript`.
- `makeCompactButtonFn : function(value : number) : TreeNodeId`: Creates the compact button for a value; for script parameters with `images` it shows the matching image.
- `formatValueFn : function(value : number) : string`: Formats a value for display; for script parameters it calls `formatValueScript`.
- `getImageFn : function(value : integer) : string`: Returns the image path for a value.
- `stepValueFn : function(value : number, direction : integer, precise : boolean) : number`: Returns the next value when the parameter is stepped (key binding or gamepad) in `direction`; `precise` is set while the precision modifier is held. For script parameters it calls `stepValueScript`, otherwise it steps through the values.
- `getFilterObjectsFn : function() : {MenuCategoryUtil.FilterObject}, integer`: Returns the selectable objects (and the default index) of an object-filter parameter, such as the bridge, tunnel or railroad crossing type of the street and track builders.
- `hideLabel : boolean`: Hides the parameter's label.
- `resetOnCategoryChange : boolean`: Resets the value to the default when the category changes.
- `resetOnMenuClose : boolean`: Resets the value to the default when the construction menu closes.
- `resetOnDefinitionChange : boolean`: Resets the value to the default when another entry is selected.
- `preserveValueInAllCategories : boolean`: Shares one stored value across all categories (stored under category `-1`).
- `constructionParam : boolean`: Marks a parameter as a parameter of the construction itself; only such parameters are passed to the construction builder. Set for all converted script parameters.
- `id : string`

### ConstructionMenuParam.CheckEnabledResult (enum)

```teal
enum CheckEnabledResult
```

Result of `checkEnabledFn`, the state of a parameter for the current parameter values.

- `"Enabled"`: Shown and usable.
- `"Hidden"`: Not shown.
- `"Disabled"`: Shown but not usable.
- `"InputActionOnly"`: Not shown in the parameter list; the base game uses it for parameters that are only changed through input actions (for example the brush eraser mode).

## ConstructionMenuParamInfo (record)

```teal
global record ConstructionMenuParamInfo
```

A parameter of the active definition with its position and current value, as returned by `ConstructionMenuParamsAPI.getParamByKey`.

- `index : integer`: Position in the current parameter list (use with `changeParam`).
- `param : ConstructionMenuParam`: The parameter.
- `value : number`: Its current value.

## ConstructionMenuAction (enum)

```teal
global enum ConstructionMenuAction
```

Kind of action a construction menu entry starts; selects which built-in construction action is configured.

- `"ACTION_CONSTRUCTION_BUILDER"`: Places a construction (`constructions`, optionally `constructionTemplate`).
- `"ACTION_STREET_BUILDER_UPGRADER"`: Builds or upgrades streets.
- `"ACTION_STREET_TERMINAL_BUILDER"`: Places an edge object construction (street terminal) on a street.
- `"ACTION_BUS_LANE_TOOL"`: Bus lane tool (upgrades roads with bus lanes).
- `"ACTION_TRAM_TRACK_TOOL"`: Tram track tool (upgrades roads with tram tracks).
- `"ACTION_PLAYER_OWNED_TOOL"`: Locking tool (marks roads as locked and player owned).
- `"ACTION_TRAFFIC_LIGHT_TOOL"`: Traffic light tool (enables or disables traffic lights on crossings).
- `"ACTION_LANE_MODIFIER_TOOL"`: Crossing tool (changes road and tram lanes at crossings).
- `"ACTION_CROSSWALK_MODIFIER_TOOL"`: Crosswalk tool (enables or disables crosswalks).
- `"ACTION_TRACK_BUILDER_UPGRADER"`: Builds or upgrades tracks.
- `"ACTION_TRACK_ELECTRIFICATION_TOOL"`: Electrification tool (adds catenary to tracks).
- `"ACTION_TRACK_EDGE_DECORATION_TOOL"`: Applies an edge decoration (`resIndex`) to tracks.
- `"ACTION_TERRAIN_MODIFIER_SMOOTH_TOOL"`: Smooth terrain tool.
- `"ACTION_TERRAIN_MODIFIER_FLATTEN_TOOL"`: Flatten terrain tool.
- `"ACTION_TERRAIN_MODIFIER_RAISE_TOOL"`: Raise terrain tool.
- `"ACTION_TERRAIN_MODIFIER_LOWER_TOOL"`: Lower terrain tool.
- `"ACTION_TERRAIN_MODIFIER_HEIGHTMAP_TOOL"`: Roughen terrain tool (modifies the terrain with a heightmap texture).
- `"ACTION_TERRAIN_PAINTER"`: Paints a terrain material (`resIndex`, `0` is the eraser).
- `"ACTION_ASSET_BRUSH"`: Brush that places or erases assets such as trees, plants and rocks.
- `"ACTION_TOWN_BUILDER"`: Town builder.
- `"ACTION_TOWN_BUILDER_EXPERIMENTAL"`: Experimental town builder.
- `"ACTION_BULLDOZER"`: The bulldozer.
  In game: "Bulldozer", "Remove constructions and terraform the terrain.", "Underground" (German: „Bulldozer“, „Konstruktionen entfernen und das Gelände bearbeiten.“, „Untergrund“)
- `"ACTION_MODULE_BUILDER"`: Places a module of a modular construction.
- `"ACTION_MODULE_BULLDOZER"`: Removes modules.
  In game: "Bulldozer" (German: „Bulldozer“)
- `"ACTION_MARKETING"`: Marketing campaign perk.
- `"ACTION_ECO_INDUSTRY"`: Industry greenification perk.
- `"ACTION_PROSPECTION"`: Prospection perk.
- `"ACTION_CUSTOM"`

## ConstructionActionCustomHudIcons (enum)

```teal
global enum ConstructionActionCustomHudIcons
```

Extra HUD icons a construction action can show.

- `"MAINTENANCE_STATION"`: Shows the maintenance pool progress bar on vehicle depots that have a maintenance pool.

## ConstructionActionHudIconStationType (enum)

```teal
global enum ConstructionActionHudIconStationType
```

Restricts station HUD icons to cargo or passenger stations.

- `"CARGO"`: Only cargo stations.
- `"PASSENGER"`: Only passenger stations.

## ConstructionActionHudIconConstructionType (enum)

```teal
global enum ConstructionActionHudIconConstructionType
```

Restricts station and depot HUD icons to a carrier.

- `"ROAD"`: also includes TRAM
- `"TRAM"`: Tram stations and depots.
- `"RAIL"`: Rail stations and depots.
- `"WATER"`: Water stations and depots.

## ConstructionActionHudIconParams (record)

```teal
global record ConstructionActionHudIconParams
```

User parameters of the HUD icon master used while a construction action is active.

- `defaultIcons : {Engine.ComponentType}`: Component types whose entities get HUD icons (filled from `ConstructionActionHudIcons.componentTypes`).
- `customIcons : {ConstructionActionCustomHudIcons}`: Extra icons to show.
- `stationType : ConstructionActionHudIconStationType`: Shows station icons only for stations of this type.
- `constructionType : ConstructionActionHudIconConstructionType`: Shows station and depot icons only for this carrier.
- `depotType : ConstructionActionHudIconConstructionType`: Carrier filter for depots; overrides `constructionType` for depots.
- `accessNotificationCacheFn : function(entity : Engine.Entity) : {Notification}`: Returns the notifications of an entity (from the game context).
- `showPerksAndTowns : boolean`: Shows only town icons and custom HUD icons of landmark constructions (`hudIconScript`) instead of the default icons.

## ConstructionActionHudIcons (record)

```teal
global record ConstructionActionHudIcons
```

HUD icon configuration of a construction menu entry. Construction descriptions set it through `configureHudIconsScript`.

Example:
```lua
-- base/content/gui/gui/construction/construction_desc_hud_icons.script.tl
configureIndustryHudIconsFn = function() : ConstructionActionHudIcons
	return {
		componentTypes = {
			api.type.ComponentType.INDUSTRY,
			api.type.ComponentType.TOWN,
		},
	}
end,
```

- `componentTypes : {Engine.ComponentType}`: Component types whose entities show HUD icons while the action is active.
- `showDistricts : boolean`: default nil/false -- note: true is only working, when componentTypes contains TOWN!!
- `showTownBorders : boolean`: default nil/false -- note: true is only working, when componentTypes contains TOWN!!
- `showPerksAndTowns : boolean`: default nil/false -- note: true is only working, when componentTypes contains TOWN!!
- `params : ConstructionActionHudIconParams`: Extra parameters for the HUD icon master.

## ConstructionCustomActionParams (record)

```teal
global record ConstructionCustomActionParams<P>
```

Parameters passed to the recipe of a custom construction action (`ConstructionDefinition.customAction`).

- `gameCtx : GameContext`: Game context.
- `definition : ConstructionDefinition`: The menu entry the action belongs to.
- `customParam : P`: The `customParam` of the custom action.
- `isActive : boolean`: Whether the action is currently active.
- `abort : function()`: Aborts the action.
- `setActionFn : function(fn : (function() : TreeNodeId), key2? : string)`: Binds the function that creates the action's tree node (for example an `ActionDescriptor`) to the menu entry; `key2` identifies the bound function.
- `getCurrentParams : function() : {string : number}`: Returns the current parameter values of the menu.

## ConstructionCustomAction (record)

```teal
global record ConstructionCustomAction<P>
```

A custom action of a construction menu entry; create one for perk tools with `ConstructionReactUtil.RegisterCustomSelectorBasedActionRecipe`.

- `recipe : Recipe<ConstructionCustomActionParams<P>>`: Recipe that implements the action.
- `customParam : P`: Value passed to the recipe as `customParam`.

## SelectorBasedActionHoverResult (record)

```teal
global record SelectorBasedActionHoverResult
```

Result of `RegisterCustomSelectorBasedActionRecipeParams.onHover`.

- `param : Builtin.ActionTooltipParam<any>`: Tooltip to show (recipe and parameters), for example with `ConstructionReactUtil.SimpleTooltipRecipe`.
- `mouseCursor : integer`: Mouse cursor id, for example from `gui_react_util.getMouseCursorInteger("CompanyMechanics")`.

## RegisterCustomSelectorBasedActionRecipeParams (record)

```teal
global record RegisterCustomSelectorBasedActionRecipeParams<P>
```

Callbacks of a custom action that selects an entity on the map, for `ConstructionReactUtil.RegisterCustomSelectorBasedActionRecipe`.

- `isAvailable : function(params : ConstructionCustomActionParams<P>) : boolean`: Returns whether selecting is currently allowed; without it the action is always available.
- `isActive : boolean`
- `onHover : function(ConstructionCustomActionParams<P>, entity : Engine.Entity, indices : {integer}, details : Gui.SelectionDetails) : SelectorBasedActionHoverResult`: Called when an entity is hovered; returns the tooltip and cursor to show, or `nil` to clear them.
- `onSelect : function(ConstructionCustomActionParams<P>, entity : Engine.Entity)`: Called when the hovered entity is selected (click or `IA_APPLY`) and the action is available.
- `filter : function(entity : Engine.Entity)`: Filter of the selector; decides which entities can be hovered.

## IconWithAvailability (record)

```teal
global record IconWithAvailability
```

Icon of a construction menu entry.

- `icon : string`: Icon path.
- `lockedIcon : string`: Icon shown while the entry is locked (company rank too low).
- `yearFrom : {{integer, string}}`: List of `{year, iconPath}` pairs in ascending year order; the last entry whose year has been reached is shown. Overrides `icon`.

## ConstructionDefinition (record)

```teal
global record ConstructionDefinition
```

An entry of the construction menu. The menu builds these from constructions, streets, tracks, modules, terrain materials, edge decorations and `construction_tool` resources.

Notes:

A `construction_tool` resource returns its entry from `getDefinitionFn` (see
`ConstructionToolData`); the base game's perk tools in `gui/construction/tools/` are examples.

- `action : ConstructionMenuAction`: Kind of action the entry starts.
- `resName : ResName`: Resource name of the entry (construction, street type, module, ...).
- `constructions : {ResName}`: Constructions placed by `ACTION_CONSTRUCTION_BUILDER`.
- `builderAudioRes : {ResName}`: Builder audio resources (`*.builder_audio`) used for the action's sounds.
- `resIndex : integer`: for terrainPainter
- `icon : IconWithAvailability`: Icon in the menu list.
- `previewIcon : IconWithAvailability`: Preview image in the info panel.
- `name : string`: Display name.
- `description : string`: Description text.
- `params : {ConstructionMenuParam}`: Parameters of the entry.
- `categories : {string}`: Filter categories of the entry within its menu category.
- `availability : ConstructionAvailability`: Years in which the entry is available.
- `placementTag : string`
- `metadata : ConstructionDescMetadata`: Construction description metadata (company perks, emissions, track description, ...); read by the info panel.
- `constructionTemplate : integer`: Zero-based index of the construction template, or `-1` for none.
- `hudIcons : ConstructionActionHudIcons`: HUD icons shown while the action is active.
- `showInAllCategories : boolean`: Shows the entry regardless of the selected filter category.
- `disabled : DefinitionDisabled`: When set, the entry is shown as disabled with its message, and its action is not started unless `keepDisabledActive` is set.
- `terrainContours : boolean`: Shows terrain contour lines while the action is active.
- `navigableWater : boolean`: Colors navigable water while the action is active.
- `configureLayerScript : ScriptRef`: Script that adjusts the layer configuration of the action (called with its params, the layer config and the current parameter values).
- `configureHudIconsScript : ScriptRef`: Script that returns the `ConstructionActionHudIcons`.
- `customAction : ConstructionCustomAction<any>`: Custom action implemented by a recipe; when set, the menu runs this recipe for the entry. The base game uses it for the prospection, marketing and industry greenification perks.
- `order : integer`: Sort order (set for edge decorations).
- `attributes : {string : {number, number}}`: Attribute ranges shown in the info panel (for example `cost`, `maintenanceCost`, `noise`), each `{min, max}`.
- `attributesCostPerKm : boolean`: Shows costs and emissions per km (per mile in imperial units), as for streets and tracks.
- `costsYearProgression : boolean`: Adjusts the displayed costs to the current year.
- `cargoTypeSet : CargoTypeSet`: Cargo types the construction handles, shown in the info panel.
- `keepDisabledActive : boolean`: Keeps the action running while the entry is disabled (used for modules).
- `showNumRemaining : boolean`: Shows how many uses of the company permit remain (landmarks and headquarters).
- `numRemaining : integer`: Fixed number of remaining uses to show, instead of computing it.
- `menuCategory : ModelMetadata.MenuCategory`: Menu categories (with order and filter categories) the entry is listed in.
- `moduleType : string`: Module type, for module entries.
- `categoryList : ModelMetadata.CategoryList`: Category list of the entry; climate categories (`*clima*`) are shown as climate specializations in the info panel.

### ConstructionDefinition.DefinitionDisabled (record)

```teal
record DefinitionDisabled
```

Marks an entry as disabled.

- `message : string`: Reason shown for the disabled entry.

## ConstructionMenuActionParams (record)

```teal
global record ConstructionMenuActionParams
```

Everything the construction menu needs to run the action of an entry, returned by `ConstructionReactUtil.getActionParams`.

- `constructionActionParams : Builtin.ConstructionActionParam`: Parameters of the built-in construction action.
- `hudIconManagerParams : Builtin.HudIconManagerConfigParam<ConstructionActionHudIconParams>`: HUD icon manager configuration, or `nil` if the entry has no HUD icons.
- `layerConfig : LayerConfig`: Layer configuration while the action is active.

## ConstructionMenuModuleCategory (record)

```teal
global record ConstructionMenuModuleCategory
```

- `categoryName : string`
- `category : string`
- `icon : string`
- `modules : {{string, ModuleDesc}}`
- `typeAvailability : {string : ConstructionAvailability}`

## ConstructionActionRepository (record)

```teal
global record ConstructionActionRepository
```

Data shared by all construction menu entries, created by `ConstructionReactUtil.getConstructionActionRepository`.

- `terrainModifierHeightmaps : {integer}`: Resource ids of all `stamp` resources (heightmap stamps).
- `cargoTypes : {CargoTypeId}`: Cargo type ids in menu order.
- `landUse2CargoTypes : {LandUseType : {CargoTypeId}}`: Cargo types per land use type (from `api.engine.util.town.getLandUse2CargoTypes()`).
- `terrainToolData : ConstructionToolParam.TerrainToolData`: Size data of the terrain tools.
- `hasHeightMapTextureTool : boolean`: Whether the heightmap texture tool is available.

## ConstructionToolData (record)

```teal
global record ConstructionToolData
```

Data of a `construction_tool` resource, which adds an entry to the construction menu.

Example:
```lua
-- base/content/gui/gui/construction/tools/industry_greenify_tool.res.lua
function data()
	return {
		type = "construction_tool",
		data = {
			getDefinitionFn = {
				fileName = resolve("industry_greenify_tool.script@getDefinition"),
			}
		}
	}
end
```

- `getDefinitionFn : ScriptRef`: Script function `(resName, repository, params) -> ConstructionDefinition` that returns the menu entry; a `nil` result adds nothing.
- `isDisabledFn : ScriptRef`: Optional script function `(resName, repository, params) -> boolean`; when it returns `true` the tool is left out of the menu.
