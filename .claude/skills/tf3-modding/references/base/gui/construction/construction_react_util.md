# gui.construction.construction_react_util

Definition file: `base/tealdef/gui/construction/construction_react_util.d.tl` (in the game folder).

Helper module of the construction menu (`::/gui/construction/construction_react_util.tl`). Builds the
menu entries from the resource repositories (constructions, streets, tracks, modules, terrain
materials, `construction_tool` resources, edge decorations), converts script parameters into menu
parameters and creates the parameters of the construction action.

## StampData (record)

```teal
global record StampData
```

Data of a `stamp` resource, a heightmap stamp for the terrain heightmap tool.

Example:
```lua
-- base/content/gui/gui/construction/shared/tex/stamps/temperate_hill1.res.lua
function data()
	return {
		type = "stamp",
		data = {
			heightmap = resolve("temperate_hill1.png"),
			icon = resolve("temperate_hill1.tga")
		}
	}
end
```

- `heightmap : string`: Heightmap image of the stamp.
- `icon : string`: Icon shown for the stamp in the tool's "Heightmap" parameter.

## ConstructionReactUtil (record)

```teal
global record ConstructionReactUtil
```
Load: `local construction_react_util = ug_require "::/gui/construction/construction_react_util.tl" as ConstructionReactUtil`

Helper module of the construction menu. Load it with `ug_require "::/gui/construction/construction_react_util.tl"`.

- `getConstructionDefinitions : function() : {ConstructionDefinition}`  (call as `construction_react_util.getConstructionDefinitions(…)`): Creates menu entries for all constructions and meta constructions; constructions with templates get one entry per template.
    - notes: Constructions without a menu category are skipped. Edge object constructions become `ACTION_STREET_TERMINAL_BUILDER` entries, all others `ACTION_CONSTRUCTION_BUILDER`.
- `convertScriptParamsToMenuParams : function(descParams : {ScriptParam}, filterByPostconstructionModifiable? : boolean) : {ConstructionMenuParam}`  (call as `construction_react_util.convertScriptParamsToMenuParams(…)`): Converts script parameters of a construction description into menu parameters.
    - param `descParams`: Script parameters.
    - param `filterByPostconstructionModifiable`: Keeps only parameters flagged `postConstructionModifiable`.
    - used 3x in the base game, e.g. `base/content/gui/gui/construction/construction.tl:1245`: `local menuParams = construction_react_util.convertScriptParamsToMenuParams(desc.params, true)`
- `getPerksProspectionDefinitions : function() : {ConstructionDefinition}`  (call as `construction_react_util.getPerksProspectionDefinitions(…)`): Creates the prospection perk entries from the `company_exploration` resources (with required tags) of the current economy.
  In game: "Prospect for Resources: {cargoType}", "Prospect for unexploited occurrences of resource "{cargoType}" near a specific town.", "Start Prospecting Here" (German: „Prospektion nach Ressourcen: {cargoType}“, „Eine Prospektion nach unerschlossenen Vorkommen von "{cargoType}" in der Nähe einer bestimmten Stadt auslösen.“, „Hier eine Prospektion starten“)
- `getModuleDefinitions : function() : {ConstructionDefinition}`  (call as `construction_react_util.getModuleDefinitions(…)`): Creates `ACTION_MODULE_BUILDER` entries for all modules that have a menu category.
- `getStreetDefinitions : function(tramOnly? : boolean) : {ConstructionDefinition}`  (call as `construction_react_util.getStreetDefinitions(…)`): Creates street builder entries for all street templates of road type `STREET`.
  In game: "Straight Road", "Curved Road", "Replace Road" (German: „Gerade Strasse“, „Kurvige Strasse“, „Strasse ersetzen“)
    - param `tramOnly`: Returns only tram-only roads.
- `getStreetToolsHudIcons : function(depotType : ConstructionActionHudIconConstructionType) : ConstructionActionHudIcons`  (call as `construction_react_util.getStreetToolsHudIcons(…)`): HUD icon configuration for street tools (industries, towns, stations, depots, warehouses).
    - param `depotType`: Carrier filter for depot icons.
    - used 6x in the base game, e.g. `base/content/gui/gui/construction/tools/bus_lane_tool.script.tl:31`: `hudIcons = construction_react_util.getStreetToolsHudIcons("ROAD"),`
- `getTrackDefinitions : function() : {ConstructionDefinition}`  (call as `construction_react_util.getTrackDefinitions(…)`): Creates track builder entries for all street templates of road type `TRACK`, sorted by speed.
  In game: "Track Builder Mode", "Build Track", "Replace Track" (German: „Gleisbaumodus“, „Gleis bauen“, „Gleis ersetzen“)
- `getMaterialsDefinitions : function(terrainToolData : ConstructionToolParam.TerrainToolData, includeEraser : boolean) : {ConstructionDefinition}`  (call as `construction_react_util.getMaterialsDefinitions(…)`): Creates terrain painter entries for all terrain materials, sorted by their order.
  In game: "Eraser Tool", "Erase the terrain painting and restore the default material." (German: „Radiergummi“, „Die Gelände-Bemalung entfernen (den Ursprungszustand wiederherstellen).“)
    - param `terrainToolData`: Size data for the brush parameters.
    - param `includeEraser`: Adds the material eraser entry (`resIndex` 0).
- `forEachDefinition : function(fn : function(definition : ConstructionDefinition), repository : ConstructionActionRepository)`  (call as `construction_react_util.forEachDefinition(…)`): SLOW!
    - used 1x in the base game, e.g. `base/content/gui/gui/construction/construction.tl:4295`: `construction_react_util.forEachDefinition(append, params.repository)`
- `makeItemDisabledCache : function(allDefinitions : {ConstructionDefinition}) : ItemDisableCache`  (call as `construction_react_util.makeItemDisabledCache(…)`): Creates the cache for `checkItemDisabledDefault` and `checkItemRemainingNum` from all menu entries.
    - used 2x in the base game, e.g. `base/content/gui/gui/construction/construction.tl:4312`: `itemDisableCacheRef:set(construction_react_util.makeItemDisabledCache(refAllDefinitions:get()))`
- `checkItemDisabledDefault : function(ConstructionDefinition, cache : ItemDisableCache) : ItemDisableReason`  (call as `construction_react_util.checkItemDisabledDefault(…)`): Checks the company permits of an entry.
    - returns: The reason the entry is disabled, or `nil` if it is usable.
    - used 3x in the base game, e.g. `base/content/gui/gui/construction/construction.tl:277`: `local disabledReason = params.itemDisableCacheRef and construction_react_util.checkItemDisabledDefault(definition, params.itemDisableCacheRef:get()) or nil`
- `checkItemRemainingNum : function(definition : ConstructionDefinition, cache : ItemDisableCache) : integer | nil`  (call as `construction_react_util.checkItemRemainingNum(…)`): Returns how many uses of an entry's company permit remain, or `nil` if not limited.
    - used 1x in the base game, e.g. `base/content/gui/gui/construction/construction.tl:286`: `construction_react_util.checkItemRemainingNum(definition, params.itemDisableCacheRef:get()) or nil`
- `makeUndergroundParam : function(stepValueFn : (function(value : integer, direction : integer, precise : boolean) : integer), checkEnabledFn? : (function(params : {string : number}) : ConstructionMenuParam.CheckEnabledResult), defaultUnderground? : boolean) : ConstructionMenuParam`  (call as `construction_react_util.makeUndergroundParam(…)`): Creates the toolbar parameter `undergroundMode` ("Underground", Yes/No) that shows and snaps only to underground objects. Its value is preserved across categories.
    - param `stepValueFn`: Step function of the parameter.
    - param `checkEnabledFn`: Enabled check of the parameter.
    - param `defaultUnderground`: Makes "Yes" the default.
    - used 7x in the base game, e.g. `base/content/gui/gui/construction/tools/bus_lane_tool.script.tl:27`: `construction_react_util.makeUndergroundParam(function(value : integer, __ : integer, __ : boolean) : integer`
- `getBrushConstructionParams : function(args : ConstructionReactUtil.BrushConstructionArgs) : {ConstructionMenuParam}`  (call as `construction_react_util.getBrushConstructionParams(…)`): Creates the toolbar parameters of a brush tool (size, strength, rotation, brush shape and the optional parameters selected in `args`).
  In game: "Brush Mode", "Brush Size", "Strength" (German: „Pinselmodus“, „Werkzeuggrösse“, „Stärke“)
    - used 6x in the base game, e.g. `base/content/gui/gui/construction/tools/asset_eraser_tool.script.tl:24`: `params = construction_react_util.getBrushConstructionParams({`
- `getMenuCategories : function(repository : ConstructionActionRepository) : {ConstructionMenu : {{ConstructionMenuCategory, {ConstructionDefinition}}}}`  (call as `construction_react_util.getMenuCategories(…)`): Groups all menu entries by construction menu tab and `menu_category`, sorted by category order and entry order.
    - returns: For each menu tab, a list of `{category, definitions}` pairs.
    - used 1x in the base game, e.g. `base/content/gui/gui/construction/construction.tl:4374`: `return initCategories(refEntity, params.bulldozerHudIconFilter, construction_react_util.getMenuCategories(params.repository))`
- `getConstructionActionRepository : function(terrainToolData : ConstructionToolParam.TerrainToolData, hasHeightMapTextureTool : boolean) : ConstructionActionRepository`  (call as `construction_react_util.getConstructionActionRepository(…)`): Creates the repository with all heightmap stamps, ordered cargo types and land-use cargo types.
    - used 1x in the base game, e.g. `base/content/gui/gui/construction/construction.tl:5047`: `repository = construction_react_util.getConstructionActionRepository(param.payload.toolParam.terrainToolData, param.payload.toolParam.hasHeightMapTextureTool),`
- `getActionParams : function(definition : ConstructionDefinition, params : {string : number}, repository : ConstructionActionRepository, isGamepadMode : boolean, refParams : ReactRefWrapApiT<any, ConstructionMenuParamsAPI>, entity : Engine.Entity, accessNotificationCacheFn : (function(Engine.Entity) : {Notification}), sublistParamsApi : ReactRefWrapApiT<any, ConstructionMenuParamsAPI>, disableLayersIA : boolean) : ConstructionMenuActionParams`  (call as `construction_react_util.getActionParams(…)`): Creates the construction action, HUD icon and layer parameters for a menu entry and its current parameter values, including the input action handlers of its toolbar parameters.
  In game: "Brush Mode", "Reputation: -{value} {townsString}", "Underground Mode" (German: „Pinselmodus“, „Ruf: -{value} {townsString}“, „Untergrundmodus“)
    - param `disableLayersIA`: When `true`, the layer ridge input action of the construction menu is not registered. The construction window passes `gameCtx.disableFeatures:get()["Layers"]`.
    - used 1x in the base game, e.g. `base/content/gui/gui/construction/construction.tl:4449`: `local actionParams = construction_react_util.getActionParams(`

Very slow -> Do not use! If needed, get allCompanyDefinitionsCacheRef from game.tl
- `getAllCompanyDefinitions : function(terrainToolData : ConstructionToolParam.TerrainToolData) : {ConstructionDefinition}`  (call as `construction_react_util.getAllCompanyDefinitions(…)`)
    - used 1x in the base game, e.g. `base/content/gui/gui/main/game.tl:314`: `return construction_react_util.getAllCompanyDefinitions(terrainToolDataRef:get())`
- `constructIcon : function(icon : IconWithAvailability, locked : boolean) : TreeNodeId`  (call as `construction_react_util.constructIcon(…)`): Creates the icon node of a menu entry (the year-dependent or locked icon if applicable).
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company.tl:159`: `} or construction_react_util.constructIcon(currentPerk.icon, false)) or nil,`
- `mergeLayerConfig : function(fromConstructionItem : LayerConfig, preferredLayerConfig : LayerConfig) : LayerConfig`  (call as `construction_react_util.mergeLayerConfig(…)`): Returns the preferred layer configuration (the one chosen in the layers menu) if set, otherwise the configuration of the construction entry.
    - used 1x in the base game, e.g. `base/content/gui/gui/construction/construction.tl:4486`: `local finalLayerConfig = construction_react_util.mergeLayerConfig(actionParams.layerConfig, layerToolLayerConfig:old())`
- `makeWhiteScreen : function(layerConfig : LayerConfig, layerColors : LayerColors)`  (call as `construction_react_util.makeWhiteScreen(…)`): Sets a color pass on the layer configuration that renders the scene in the neutral construction fallback colors.
    - used 1x in the base game, e.g. `base/content/gui/gui/construction/construction.tl:4239`: `construction_react_util.makeWhiteScreen(layerConfig, layerColors)`
- `SimpleTooltipRecipe : Recipe<ConstructionReactUtil.SimpleTooltipRecipeParams>`: Recipe of a simple action tooltip with a message line and an optional cost line.
  In game: "Cost: {cost}" (German: „Kosten: {cost}“)
    - used 4x in the base game, e.g. `base/content/gui/gui/construction/tools/industry_greenify_tool.script.tl:36`: `recipe = construction_react_util.SimpleTooltipRecipe,`
- `RegisterCustomSelectorBasedActionRecipe : function<P>(recipeName : string, outerParams : RegisterCustomSelectorBasedActionRecipeParams<P>) : Recipe<ConstructionCustomActionParams<P>>`  (call as `construction_react_util.RegisterCustomSelectorBasedActionRecipe(…)`): Registers a custom action recipe that lets the player pick an entity on the map, with tooltip, cursor and HUD icons. The base game uses it for the prospection, marketing and industry greenification perks.
    - param `recipeName`: Name of the recipe.
    - param `outerParams`: Callbacks and selector filter.
    - returns: A recipe for `ConstructionCustomAction.recipe`.

    ```lua
    -- abridged from base/content/gui/gui/construction/tools/industry_greenify_tool.script.tl
    local IndustryGreenifyRecipe = construction_react_util.RegisterCustomSelectorBasedActionRecipe("IndustryGreenifyAction", {
    	onHover = function(__, entity : Engine.Entity) : SelectorBasedActionHoverResult
    		return {
    			param = {
    				recipe = construction_react_util.SimpleTooltipRecipe,
    				param = { message = { message = _("Select an Industry to Greenify") } },
    			},
    			mouseCursor = gui_react_util.getMouseCursorInteger("CompanyMechanics"),
    		}
    	end,
    	onSelect = function(params : ConstructionCustomActionParams<string>, entity : Engine.Entity)
    		-- send a command for the selected entity
    	end,
    	filter = componentFilter(api.type.ComponentType.INDUSTRY),
    })
    ```

    - used 2x in the base game, e.g. `base/content/gui/gui/construction/tools/industry_greenify_tool.script.tl:16`: `local IndustryGreenifyRecipe = construction_react_util.RegisterCustomSelectorBasedActionRecipe("IndustryGreenifyAction", {`

### ConstructionReactUtil.ItemDisableCache (record)

```teal
record ItemDisableCache
```

Cached company data used to decide whether menu entries are locked or used up; create it with `makeItemDisabledCache`.

- `company : CompanyUtil.ConstructionDisableCacheData`: Company permit data of all entries.

### ConstructionReactUtil.ItemDisableReason (record)

```teal
record ItemDisableReason
```

Why a menu entry is disabled.

- `reason : string`: Message shown for the disabled entry.
- `elapsedCooldownFraction : number`: Elapsed part of a cooldown (0 to 1), shown as progress on the entry.
- `category : string`: Category of the reason; `"company-rank"` shows the entry as locked.

### ConstructionReactUtil.ConstructionMenuSetTabEventParam (record)

```teal
record ConstructionMenuSetTabEventParam
```

Parameter of the GUI event `constructionMenuSetTab`, which opens a construction menu tab.

- `tabIndex : integer`: Index of the tab to open.
- `allowStacking : boolean`: Passed to the tool stack when the construction tool is pushed.
- `sublistId : string`: Id of the sub tab to select.

### ConstructionReactUtil.ConstructionMenuSelectConstructionEventParam (record)

```teal
record ConstructionMenuSelectConstructionEventParam
```

Parameter of the GUI event `constructionMenuSelectTabForConstruction`, which opens the menu tab containing a construction.

Example:
```lua
-- base/content/game_mechanics/game_mechanics/notifications/types/con_availability.script.tl
react.fireEvent(nil, "constructionMenuSelectTabForConstruction", {
	resName = notificationParams.resName,
})
```

- `resName : ResName`: Resource name of the entry to find; an entry available in the current year is preferred.

### ConstructionReactUtil.SimpleTooltipRecipeParams (record)

```teal
record SimpleTooltipRecipeParams
```

Parameters of `SimpleTooltipRecipe`.

- `message : MessageGoodOrBad`: Message line.
- `cost : integer`: Cost in game currency; adds a "Cost" line, shown as bad when the player cannot afford it.

#### ConstructionReactUtil.SimpleTooltipRecipeParams.MessageGoodOrBad (record)

```teal
record MessageGoodOrBad
```

A tooltip message with its style.

- `message : string`: Text of the message.
- `good : boolean`: Shows the message in the success style.
- `bad : boolean`: Shows the message in the error style (if not `good`); otherwise it uses the information style.

### ConstructionReactUtil.DefinitionWithFilterCategories (record)

```teal
record DefinitionWithFilterCategories
```

A menu entry together with the filter categories of one of its menu categories.

- `definition : ConstructionDefinition`: The menu entry.
- `filterCategories : {string}`: Filter categories of the entry in that menu category.

### ConstructionReactUtil.BrushConstructionArgs (record)

```teal
record BrushConstructionArgs
```

Options for `getBrushConstructionParams`.

- `numSizes : integer`: Number of brush sizes.
- `minSize : number`: Smallest brush size.
- `maxSize : number`: Largest brush size.
- `scale : number`: Factor applied to the sizes for display.
- `prefix : string`: Prefix of the parameter keys (for example `terrainPainter` gives `terrainPainter_size`).
- `defaultStrength : number`: Default strength, from 0 to 1.
- `single : boolean`: Adds a mode switch between single placement and brush mode, with a height parameter for single mode.
- `slope : boolean`: Adds the incline threshold and "Paint Steep" parameters.
- `invert : boolean`: Adds an "Invert" parameter.
- `invertTooltip : string`: Tooltip of the "Invert" parameter.
- `randomize : boolean`: Adds a "Random Rotation" parameter.
- `ignoreCollision : boolean`: Adds a "No Collision" parameter (single mode).
- `eraseWhenEmpty : boolean`: Makes eraser mode the default.
- `heightMaps : {integer}`: Stamp resource ids; adds a "Heightmap" parameter with their icons.
