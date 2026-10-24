
# GUI

The base game's interface is written in Teal with a React-style framework: components are functions ("recipes") that return a tree of built-in widgets, keep state in hooks and are re-run when that state changes. `api.gui` sits underneath and "Contains all GUI functionality apart from react" (`api/tealdef/api/gui.d.tl`). All GUI code runs in the GUI state (see [Scripting](scripting.md#the-script-states)).

The official wiki has no GUI page yet; its modding index lists "User Interface" as coming soon. This page is built from the base game files, and the wiki only adds a few details, noted below.

## The pieces

| Module | Loaded with | Definition | Content |
|---|---|---|---|
| React | `ug_require "::/gui/main/react.lua" as React` | [react](../base/scripts/react.md) | `RegisterRecipe`, hooks (`useState`, `useRef`, `onStep`, `onEvent`, ...), extension points and plugins |
| Builtin | `ug_require "::/gui/main/builtin.lua" as Builtin` | [builtin](../base/scripts/builtin.md) | the widgets: `BoxLayout`, `FlowLayout`, `FloatingLayout`, `TextView`, `RichTextView`, `ImageView`, `Button`, `ToggleButton`, `CheckBox`, `ComboBox`, `Slider`, `TextInputField`, `List`, `ScrollArea`, `TabWidget`, `TableLayout`, `Window`, `Chart`, ... |
| GUI helpers | `ug_require "::/gui/main/gui_react_util.tl"` and others in `gui/main` | [gui/main](../base/gui/main/gui_react_util.md) | layout shortcuts, `usePlugins`, colours, tooltips |
| `api.gui` | global | [api.gui](../api/api/gui.md) | camera, sound, input actions, `byId`, `genericRep` for [GUI resources](#gui-resources), `fireGuiScriptEvent` |

## A recipe

`React.RegisterRecipe(name, fn)` turns a function into a recipe that can be called like a widget. The function returns a node tree built from `builtin.*` calls, each taking a parameter table. The game bar's Earnings display is a small, complete example. From `base/content/gui/gui/game_bar/game_bar_display_earnings_plugin/game_bar_display_earnings.script.tl`:

```teal
local react = ug_require "/gui/main/react.lua" as React
local builtin = ug_require "/gui/main/builtin.lua" as Builtin
local engine_react_util = ug_require "/gui/main/engine_react_util.tl" as EngineReactUtil

local game_bar_widgets = ug_require "/gui/game_bar/game_bar_widgets.tl" as GameBarWidgets

local GameBarEarningsPlugin = react.RegisterPluginRecipe(game_bar_widgets.GameBarInfoDisplayExtension, "GameBarEarningsPlugin", function() : TreeNodeId
	local isMapEditorRef : ReactRefT<boolean> = react.useRefLazy(function() : boolean return api.gui.game.isMapEditor() end)

	local makeState = function() : integer
		return api.engine.util.finance.calculateEarnings(api.engine.util.getPlayer())
	end
	local earningsAndIsMapEditor = engine_react_util.useStepStateTimer(makeState)

	if isMapEditorRef:get() then
		return nil
	end

	local className = earningsAndIsMapEditor:old() >= 0 and "positive" or "negative"
	return builtin.BoxLayout{
		orientation = builtin.type.Orientation.Horizontal,
		children = {
			builtin.Component {
				meta = { tooltip = _("Total Earnings") },
				layout = builtin.BoxLayout{
					orientation = builtin.type.Orientation.Horizontal,
					children = {
						builtin.TextView{
							meta = { class = "font-scale-headline" },
							text = _("Earnings"),
						},
						builtin.TextView{
							meta = { class = "font-scale-headline, " .. className },
							text = api.util.formatMoney(earningsAndIsMapEditor:old())
						},
					},
				},
			},
		},
	}
end)

local result = {
	GameBarEarningsPlugin = GameBarEarningsPlugin,
}

return result
```

This file is part of the base game, so `/` in its `ug_require` lines means the base game. In a mod, write `::/gui/main/react.lua` and so on (see [File references inside content](mod-structure.md#file-references-inside-content)).

What it shows:

- Returning `nil` renders nothing.
- `meta` is on every widget ([`Meta`](../base/scripts/react.md) (symbol: `Meta`, see [symbol index](../index.tsv))): `id`, `class` (style classes, comma-separated), `tooltip`, `enabled`, `onMouseOver`, `onFocusChange` and more.
- The recipe reads the engine with `api.engine.util.finance.calculateEarnings`. `engine_react_util.useStepStateTimer(makeState)` re-runs `makeState` on a timer and returns a state whose `:old()` is the last value ([engine_react_util](../base/gui/main/engine_react_util.md)).
- The module returns its recipes in a table, so a `.res` file can reference them as `...script@GameBarEarningsPlugin`.

### Hooks

The hooks are on the React module and are called at the top of a recipe, as in React for the web:

| Hook | Definition |
|---|---|
| `useState(initial)` | returns a [`ReactStateT<T>`](../base/scripts/react.md) (symbol: `ReactStateT`, see [symbol index](../index.tsv)) with `old()`, `set(v)` and `transform(fn)`; setting it re-runs the recipe |
| `useRef(initial)`, `useRefLazy(fn)` | a [`ReactRefT<T>`](../base/scripts/react.md) (symbol: `ReactRefT`, see [symbol index](../index.tsv)) with `get`/`set`, without re-running |
| `useMirrorState`, `useDependentState` | state derived from another ref or state |
| `onStep(fn)`, `onStepTimer(fn, intervalSeconds?, jitter?)` | run code every step or on an interval |
| `onMount(fn)`, `onUnmount(fn)` | lifecycle |
| `onEvent(name, fn)`, `fireEvent(source, name, param?)` | GUI events between recipes |
| `useNodeRef(recipe?)`, `ref(nodeRef)` | handle to a child node, for focus and position |
| `useInputAction(name, handler)`, `iaHandler(fn, ...)` | keyboard and gamepad input actions |

Buttons call back into Lua with `onClick`, and that is where scripts usually send commands or events. From the game bar (`base/content/gui/gui/game_bar/game_bar_widgets.tl`):

```teal
builtin.Button{
	meta = {
		class = "secondary, skip-to-next",
		tooltip = _("Skip to Next Time of Day"),
		enabled = stateMode:old() == "Dynamic",
	},
	onClick = function()
		api.cmd.sendCommand(api.cmd.makeScriptingSendEventCmd("", "GameTime", "SkipPhase", { }))
	end,
	content = builtin.ImageView{
		path = "::/gui/game_bar/icons/gametime_cycle.tga"
	}
}
```

### Styles

Each GUI module has a stylesheet next to it, a `.css.lua` file that builds a table of selectors with `stylesheetutil.lua`. From `game_bar_display_earnings.css.lua`:

```lua
local ssu = require "::/gui/main/stylesheetutil.lua"

function data()
	local result = { }

	local a = ssu.makeAdder(result)

	a("R::GameBarEarningsPlugin", {
		gravity = { 0.5, 0.5 },
	})

	a("R::GameBarEarningsPlugin BoxLayout", {
		innerSpacing = { 8, 0 },
		outerSpacing = { 14, 0 },
	})

	return result
end
```

`R::<RecipeName>` selects the nodes produced by a recipe; widget names (`BoxLayout`) and classes from `meta.class` narrow it down. The style properties a widget accepts are in [`StyleSheet`](../api/api/gui.md) (symbol: `StyleSheet`, see [symbol index](../index.tsv)) in `api.gui` (`backgroundColor`, `borderWidth`, `padding`, `size`, ...). The full selector syntax is not documented in the definitions or the wiki. The wiki's [resource types](https://wiki.transportfever3.com/doku.php?id=modding:general:resourcetypes) table confirms `.css.lua` as the stylesheet type, and the base loader has a `loadStyleSheet` modifier key, so a mod can change existing stylesheets at load time (see [Resource modifiers](scripting.md#resource-modifiers)).

### GUI resources

Colours, GUI sounds and a few display settings are not written into the stylesheets and recipes. They live in GUI resources, `.gres.lua` files whose `data()` returns `type = "gui_res"` and a plain table. The base game has 29 of them, in three kinds:

| Kind | Examples | Content |
|---|---|---|
| Colours | `::/gui/main/default_colors.gres.lua`, `::/mission/mission_colors.gres.lua`, `::/game_mechanics/towns/district_colors.gres.lua` | named colours such as `Good`, `Bad`, `Highlight` |
| Sound effects | `::/gui/sound/gui_comp_sfx.gres.lua`, `::/game_mechanics/notifications/gui/sound/notification_sfx.gres.lua` | lists of `.wav` files per event, such as `ButtonClick` or `Hover` |
| Settings | `::/gui/hud/hud_gui_config.gres.lua`, `::/gui/main/transparency.gres.lua` | HUD render distances and icon offsets, transparency levels |

A colour table, from `base/content/mission/mission/mission_colors.gres.lua`:

```lua
local internal = ug_require "::/gui/main/color_internal.lua" -- do NOT use outside of gres!!

local colors = {}

colors.Medal = internal.gold

colors.Good = internal.green
colors.Target = internal.yellow
colors.SecondaryTarget = internal.darkOrange
colors.Bad = internal.fireEngineRed

colors.Highlight = internal.daffodil
colors.Guide = internal.gold
colors.GuideBlink = internal.goldenrod

function data()
	return {
		type = "gui_res",
		data = colors,
	}
end
```

The comment on the first line is the base game's: the raw colours in `color_internal.lua` are only meant for `.gres` files, everything else reads the named colours. A sound table has the same shape, with `resolve(...)` around each file name. From `base/content/game_mechanics/game_mechanics/notifications/gui/sound/notification_sfx.gres.lua`:

```lua
local sfx = {}

sfx.Initialize = { resolve("initialize.wav") }
sfx.Resolve = { resolve("resolve.wav") }

function data()
	return {
		type = "gui_res",
		data = sfx,
	}
end
```

GUI resources are not in `api.res.genericRep`; the definitions send you to the GUI's own repository, [`api.gui.genericRep`](../api/api/gui.md) (symbol: `Gui.GenericRep`, see [symbol index](../index.tsv)), whose `getAllOfType` "Only allows typeName in { "gui_res_overwrite", "gui_res" }". Stylesheets and GUI scripts look a file up by its game path without `.lua`. From `base/content/game_mechanics/game_mechanics/notifications/gui/notifications.css.lua`:

```lua
local colorDefault = api.gui.genericRep.get(api.gui.genericRep.find("::/gui/main/default_colors.gres")).data
local colorNotification = api.gui.genericRep.get(api.gui.genericRep.find("::/game_mechanics/notifications/gui/notification_colors.gres")).data
local transparency = api.gui.genericRep.get(api.gui.genericRep.find("::/gui/main/transparency.gres")).data
```

The notification popups read the sound table the same way and play one entry with `api.gui.sound.playRandomSoundEffect(notificationSFX.Initialize)` (`notification_popups.tl`). 25 of the 29 files have a Teal type in a `.d.tl` file of the same name, for example `NotificationSFX` in `notification_sfx.d.tl`, which the script uses as a cast.

!!! note "Open question: changing a GUI resource from a mod"
    The files don't show how a mod is meant to change colours or GUI sounds. The type name `gui_res_overwrite` and the record [`OverwriteGuiRes`](../base/gui/main/gui_res_modding.md) (symbol: `OverwriteGuiRes`, see [symbol index](../index.tsv)) (`base/content/gui/gui/main/gui_res_modding.d.tl`, a single function `overwrite(params : table)`) suggest a resource type for exactly that, but no base file, mod or DLC uses it. `mod.lua` also has a `loadGameRes` modifier key; whether it receives `.gres.lua` files is not visible in the files either.

## Extending the base GUI: plugins

The base GUI leaves named slots, extension points, where extra recipes can be inserted. A plugin is two files: a recipe and a generic resource that registers it. The resource for the recipe above, `game_bar_display_earnings.res.lua`:

```lua
function data()
	return {
		type = "react-plugin ::GameBarInfoDisplayExtension",
		data = {
			filePath = "::/gui/game_bar/game_bar_display_earnings_plugin/game_bar_display_earnings.script@GameBarEarningsPlugin",
			priority = -2, -- note: this is negative so default 0 will be appended to the right
		}
	}
end
```

The parts fit together like this:

1. The base GUI declares the extension point: `game_bar_widgets.GameBarInfoDisplayExtension = react.RegisterExtensionPoint0(getOwningModId(), "GameBarInfoDisplayExtension")`. In `react.lua` the id becomes `modId .. "::" .. name`; for the base game, whose mod id is empty, that is `::GameBarInfoDisplayExtension`.
2. Where the slot is drawn, the base GUI calls `gui_react_util.usePlugins(extensionPoint)` or `react.getPlugins(extensionPoint)`.
3. `getPlugins` (in `base/content/gui/gui/main/react.lua`) loads every generic resource of type `"react-plugin " .. extensionPoint.id`, resolves `filePath` to a recipe and an optional `condition` to a function, and sorts by `order` ([`ReactPluginDesc`](../base/scripts/react.md) (symbol: `ReactPluginDesc`, see [symbol index](../index.tsv)): `filePath`, `condition`, `order`).
4. The recipe registers itself with `react.RegisterPluginRecipe(extensionPoint, name, fn)`.

The entity window plugins use `order` and `condition`, for example `base/content/gui/gui/entity_window/warehouse/warehouse_eow_stocks.res.lua` with `order = 20`, or the town window plugins with `condition = "::/gui/entity_window/town/town_eow.script@isSandboxMode"`.

!!! note
    The two game bar plugins set `priority` instead of `order`. `getPlugins` in `react.lua` only reads `order`, so how `priority` is used is unclear from the files.

Extension points registered in the base GUI:

| Extension point | Where |
|---|---|
| `ModEntryPointExtension` | `gui/main/mod_entry_point.tl`; the main game view adds every plugin as a floating child (`gui/main/game.tl`) |
| `MainModButtonAreaExtension` | `gui/main/main_mod_button_area.tl`; the Radial Menu gets a mod area when plugins exist |
| `GameBarInfoDisplayExtension` | `gui/game_bar/game_bar_widgets.tl` |
| [`RadialMenuExtension`](../base/gui/main/radial_menu_extension.md) (symbol: `RadialMenuExtension.RadialMenuExtension`, see [symbol index](../index.tsv)) | `gui/main/radial_menu_extension.tl`; extra entries of the Radial Menu in the game bar (`gui/game_bar/game_bar.tl`). It is the only extension point registered with `RegisterWrapperExtensionPoint0`, so each plugin recipe is registered as a wrapper around `builtin.RadialMenuChild` |
| [`AnimalEowExtensionPoint`](../base/gui/entity_window/animal/animal_eow.md) (symbol: `AnimalEow.AnimalEowExtensionPoint`, see [symbol index](../index.tsv)), [`EmptyConstructionEowExtensionPoint`](../base/gui/entity_window/empty_construction/empty_construction_eow.md) (symbol: `EmptyConstructionEow.EmptyConstructionEowExtensionPoint`, see [symbol index](../index.tsv)), [`FunElementsEowExtensionPoint`](../base/gui/entity_window/fun_elements/fun_elements_eow.md) (symbol: `FunElementsEow.FunElementsEowExtensionPoint`, see [symbol index](../index.tsv)), [`IndustryEowExtensionPoint`](../base/gui/entity_window/industry/industry_eow.md) (symbol: `IndustryEow.IndustryEowExtensionPoint`, see [symbol index](../index.tsv)), [`LineEowExtensionPoint`](../base/gui/entity_window/line/line_eow.md) (symbol: `LineEow.LineEowExtensionPoint`, see [symbol index](../index.tsv)), [`MaintenanceStationEowExtensionPoint`](../base/gui/entity_window/maintenance_station/maintenance_station_eow.md) (symbol: `MaintenanceStationEow.MaintenanceStationEowExtensionPoint`, see [symbol index](../index.tsv)), [`PerkEowExtensionPoint`](../base/gui/entity_window/perk/perk_eow.md) (symbol: `PerkEow.PerkEowExtensionPoint`, see [symbol index](../index.tsv)), [`SimPersonEowExtensionPoint`](../base/gui/entity_window/sim_person/sim_person_eow.md) (symbol: `SimPersonEow.SimPersonEowExtensionPoint`, see [symbol index](../index.tsv)), [`StationGroupEowExtensionPoint`](../base/gui/entity_window/station_group/station_group_eow.md) (symbol: `StationGroupEow.StationGroupEowExtensionPoint`, see [symbol index](../index.tsv)), [`TownBuildingEowExtensionPoint`](../base/gui/entity_window/town_building/town_building_eow.md) (symbol: `TownBuildingEow.TownBuildingEowExtensionPoint`, see [symbol index](../index.tsv)), [`TownEowExtensionPoint`](../base/gui/entity_window/town/town_eow.md) (symbol: `TownEow.TownEowExtensionPoint`, see [symbol index](../index.tsv)), [`TownEowEditorExtensionPoint`](../base/gui/entity_window/town/town_eow.md) (symbol: `TownEow.TownEowEditorExtensionPoint`, see [symbol index](../index.tsv)), [`VehicleEowExtensionPoint`](../base/gui/entity_window/vehicle/vehicle_eow.md) (symbol: `VehicleEow.VehicleEowExtensionPoint`, see [symbol index](../index.tsv)), [`WarehouseEowExtensionPoint`](../base/gui/entity_window/warehouse/warehouse_eow.md) (symbol: `WarehouseEow.WarehouseEowExtensionPoint`, see [symbol index](../index.tsv)) | the entity windows in `gui/entity_window/` |

### Entity window plugins

The game bar plugin above takes no parameters, because its extension point is registered with `RegisterExtensionPoint0`. The entity window extension points are registered with `RegisterExtensionPoint` and pass a params table to each plugin recipe and to its `condition` (`usePluginsUntyped` in `gui/main/gui_react_util.tl` calls the condition with the same arguments). Every entity window's params type extends [`EowExtensionUtil.IEowWidgetsExtensionParams`](../base/gui/entity_window/eow_extension_util.md) (symbol: `EowExtensionUtil.IEowWidgetsExtensionParams`, see [symbol index](../index.tsv)):

| Field | Content |
|---|---|
| `entityId` | the entity the window shows |
| `ownershipState` | `"None"`, `"Player"` or `"Foreign"` |
| `gameCtx` | the game context |
| `showCalloutOnRightSide` | which side callouts open on |
| `setCollapsibleExpanded`, `getCollapsibleExpanded` | store whether a collapsible card is open, by a key |
| `firstWidgetRef`, `lastWidgetRef` | refs to the first and last widget of the window |

Each window adds its own fields; the warehouse window's [`WarehouseWidgetPluginParams`](../base/gui/entity_window/warehouse/warehouse_eow.md) (symbol: `WarehouseEow.WarehouseWidgetPluginParams`, see [symbol index](../index.tsv)) has `state` with `isModifiable`. The warehouse window's Stocks card is such a plugin. Its resource is `warehouse_eow_stocks.res.lua` (`type = "react-plugin ::WarehouseEowExtensionPoint"`, `order = 20`), and the recipe, shortened, from `base/content/gui/gui/entity_window/warehouse/warehouse_eow.script.tl`:

```teal
warehouse_eow.WarehouseStocksPlugin = react.RegisterPluginRecipe(warehouse_eow.WarehouseEowExtensionPoint, "WarehouseStocksPlugin", function(params : WarehouseEow.WarehouseWidgetPluginParams) : TreeNodeId
	local uniqueCardKey = tostring(react.getCurrentRecipeId())
	return builtin.BoxLayout{
		child = content_card.ContentCard{
			title = _("Stocks"),
			recipeAndParamPermanent = content_card.makeRecipeAndParam(
				WarehouseCargoInfo,
				{
					warehouseEntity = params.entityId,
					mode = "EntityWindow",
					-- ...
				}
			),
			-- ...
			setCollapsibleExpanded = params.state.isModifiable and function(v : boolean) params.setCollapsibleExpanded(uniqueCardKey, v) end or nil,
			collapsibleExpanded = params.state.isModifiable and params.getCollapsibleExpanded(uniqueCardKey) or nil,
			gameCtx = params.gameCtx,
			showOnRightSide = params.showCalloutOnRightSide,
		}
	}
end)
```

The diagram card next to it, `warehouse_eow_diagram.res.lua`, adds `condition = "::/gui/entity_window/warehouse/warehouse_eow.script@isModifiable"`; `isModifiable(params)` gets the same params table and returns `params.state.isModifiable`, so the diagram only shows for warehouses the player may change.

The names `ModEntryPointExtension` and `MainModButtonAreaExtension` suggest they are meant for mods. None of the shipped mods use them, so there is no official example; a mod would follow the pattern above with `type = "react-plugin ::ModEntryPointExtension"`. A mod can also declare its own extension points; the id then starts with its mod id.

In a mod, write `filePath` and `condition` with your mod id, for example `"jdoe_station_button::/gui/station_button.script@StationButtonPlugin"`. The path is resolved later by the base GUI (`util.useFn(desc.filePath)` in `react.lua`), not by your file, and the campaign mods always qualify it this way, as in the replacement below.

## Replacing a base recipe

A mod can swap out an existing recipe for its own. Campaign mission 01 does this to hide the HUD icons when a mission task disables them. The resource, `mods/release/urbangames_campaign_mission_01/content/mission/mission/hud_replacement.res.lua`:

```lua
function data()
	return {
		type = "react-replacement-config",
		data = {
			filePath = "urbangames_campaign_mission_01::/mission/hud_replacement.script",
			doReplaceFn = "doReplaceFn"
		}
	}
end
```

and the script, `hud_replacement.script.tl`:

```teal
local builtin = ug_require "::/gui/main/builtin.lua" as Builtin
local game_react_globals = ug_require "::/gui/main/game_react_globals.tl" as GameReactGlobals
local hud_icon_toolbox = ug_require "::/gui/main/hud_icon_toolbox.tl" as HudIconToolbox
local react = ug_require "::/gui/main/react.lua" as React

local HudIconMasterGameReplacement = react.RegisterRecipe("HudIconMasterGame", function(params : Builtin.HudIconParam, userParam : HudIconToolbox.HudIconMasterUserParam) : TreeNodeId
	if game_react_globals.getDisableFeatures()["HudIconMaster"] then
		return nil
	end
	return builtin.BoxLayout{
		children = { react.CallOriginalRecipe(hud_icon_toolbox.HudIconMasterGame, params, userParam) }
	}
end)

local function doReplaceFn(replacementApi : ReactReplacementApi)
	replacementApi.ReplaceRecipe(hud_icon_toolbox.HudIconMasterGame, HudIconMasterGameReplacement)
end

local result = {
	doReplaceFn = doReplaceFn
}

return result
```

`base/content/gui/gui/main/bootstrap_game.tl` collects all `react-replacement-config` resources, sorts them by `order` ([`ReactReplacementConfigDesc`](../base/scripts/react.md) (symbol: `ReactReplacementConfigDesc`, see [symbol index](../index.tsv))), calls each `doReplaceFn` with a [`ReactReplacementApi`](../base/scripts/react.md) (symbol: `ReactReplacementApi`, see [symbol index](../index.tsv)), and then sets `recipeReplacementAllowed = false`, so replacements only work in that phase. The definition warns: "the replacement must be fully compatible, e.g. if the original provides an Api, the replacement must provide the same one!" Inside the replacement, `react.CallOriginalRecipe` renders the original; the definition notes that this "will *not* work for builtins".

## GUI code in construction files

Constructions can bring their own GUI through script references in the `.con.lua`. The Cleaning Plant (`base/content/game_mechanics/game_mechanics/emission/pollution_cleanup_facility.con.lua`) sets four of them:

```lua
constr.hudIconScript = {
	fileName = "::pollution_cleanup_facility_hud.script@hudIconFn",
}
constr.configureHudIconsScript = {
	fileName = "::/gui/construction/construction_desc_hud_icons.script@configureLandmarkConstructionHudIconsFn",
}
constr.entityWindowScript = {
	fileName = "::pollution_cleanup_facility_hud.script@entityWindowFn",
}
constr.configureLayerScript = {
	fileName = "pollution_cleanup_facility_layer.script@configureLayerFn",
	params = {
	}
}
```

What each function receives and returns is listed in [Construction scripts](constructions.md#construction-scripts).

## Talking to game scripts

GUI code does not change the engine directly. It reads with `api.engine` and sends commands or script events (see [Scripting](scripting.md#events)). For GUI-only logic the game scripts have `guiUpdate` and `guiHandleEvent`, which receive events fired with `api.gui.fireGuiScriptEvent(id, name, param)` and can return a value to the caller. The Calendar widget in `game_bar_widgets.tl` uses it as `api.gui.fireGuiScriptEvent("calendar", "onManualTimeChanged", value)`. The wiki's [Missions](https://wiki.transportfever3.com/doku.php?id=modding:scripting:missions) page says the same for mission tasks: `handleEvent` receives the game script events and `guiHandleEvent` the GUI events.

To look at a GUI value while you work, the wiki's [API Reference](https://wiki.transportfever3.com/doku.php?id=modding:scripting:api) page suggests the in-game console and `debugPrint(<someTable>)`.

Official wiki: [Construction Basics](https://wiki.transportfever3.com/doku.php?id=modding:constructions:basics) (HUD icon, entity window and layer scripts), [Resource types](https://wiki.transportfever3.com/doku.php?id=modding:general:resourcetypes), [Missions](https://wiki.transportfever3.com/doku.php?id=modding:scripting:missions), [API Reference](https://wiki.transportfever3.com/doku.php?id=modding:scripting:api). The wiki's user interface page is not published yet.
