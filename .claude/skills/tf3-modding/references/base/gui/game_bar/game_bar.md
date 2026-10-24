# gui.game_bar.game_bar

Definition file: `base/tealdef/gui/game_bar/game_bar.d.tl` (in the game folder).

The game bar at the bottom of the game screen (money, construction and tool buttons, statistics, calendar, speed controls) and the main radial menu. Both are created in `gui/main/game.tl`.

## GameBar (record)

```teal
global record GameBar
```
Load: `local game_bar = ug_require "::/gui/game_bar/game_bar.tl" as GameBar`

Module of the game bar recipe and the main radial menu recipe.

- `GameBar : Recipe<GameBarParam>`: Recipe of the game bar at the bottom of the game screen.
  In game: "Company Finances", "Line Manager", "Bulldozer" (German: „Finanzübersicht“, „Linien-Manager“, „Bulldozer“)
    - used 1x in the base game, e.g. `base/content/gui/gui/main/game.tl:421`: `item = game_bar.GameBar(react.ref{gameBarRef, params.gameBarRef}, {`
- `MainRadialMenu : Recipe<MainRadialMenuParam>`: Recipe of the main radial menu (gamepad menu with notifications, statistics, calendar and more); its node has the API `GameBar.RadialMenuApi`.
  In game: "Layers", "Statistics", "Finances" (German: „Layers“, „Statistiken“, „Finanzen“)
    - used 2x in the base game, e.g. `base/content/gui/gui/main/game.tl:381`: `local radialMenuRef = react.useNodeRef(game_bar.MainRadialMenu)`

### GameBar.RadialMenuApi (record)

```teal
record RadialMenuApi
```

API of the `MainRadialMenu` node (set with `react.provideApi`).

- `show : function()`: Shows the radial menu and gives it focus.

### GameBar.GameBarParam (record)

```teal
record GameBarParam
```

Parameters of the `GameBar` recipe; `gui/main/game.tl` fills them depending on map editor, sandbox and mission mode.

- `setup : Setup`: Feature switches, see `GameBar.GameBarParam.Setup`.
- `radialMenuRef : ReactRefWrapApiT<Builtin.RadialMenuParam, RadialMenuApi>`: Node reference of the `MainRadialMenu`; the menu button and the `IA_RADIAL_MENU` input action call its `show()`.
- `gameCtx : GameContext`: Game GUI context (tool stack, window container and so on).
- `gameSpeedHelper : ReactRefWrapApi0<GameSpeedApi>`: Game speed API, passed to `GameBarWidgets.GameSpeedControl`.
- `gameUiProxyRootRef : ReactRefWrap`: Root node used as source for the keybinding hints in the speed control.
- `activateInspector : function()`: Called by the inspector button (`IA_INSPECTOR`); the base game pushes the inspector tool.
- `deactivateInspector : function()`: Callback that pops the inspector tool; passed in by `gui/main/game.tl`.

#### GameBar.GameBarParam.Setup (record)

```teal
record Setup
```

Feature switches that decide which buttons and tools the game bar shows.

- `money : Money`: Settings of the money display.
- `hasCompany : boolean`: Shows the company button (off in the map editor and in missions).
- `hasMapGeneratorNodeEditor : boolean`: Shows the map generator node editor button (map editor with the experimental terrain generator editor enabled, not in console mode).
- `hasHeightMapGenerator : boolean`: Shows the height map generator button (map editor).
- `hasHeightMapImporter : boolean`: Offers height map import in the map editor tools (not in console mode).
- `hasHeightMapExporter : boolean`: Offers height map export in the map editor tools (not in console mode).
- `hasHeightMapTextureTool : boolean`: Set to true in the map editor and sandbox mode; the base game's game bar does not read it.
- `hasTownGenerator : boolean`: Offers the town generator in the map editor tools.
- `hasIndustryGenerator : boolean`: Offers the industry generator in the map editor tools.
- `hasTownIndustryImporter : boolean`: Offers the town and industry import in the map editor tools (map editor, not in console mode).
- `hasTownIndustryExporter : boolean`: Offers the town and industry export in the map editor tools (map editor, not in console mode).
- `hasStreetMapGenerator : boolean`: Shows the street map generator button (map editor).
- `mapEditorGlobalStateRef : ReactRefT<MapEditor.GlobalState>`: Shared map editor state, passed on to the map editor tools.
- `hasTownBuilder : boolean`: Shows the town builder button (map editor and sandbox mode).
- `hasIndustryBuilder : boolean`: Shows the industry builder button (map editor and sandbox mode).
- `hasPerksMenu : boolean`: Shows the perks button (not in the map editor or in missions).
- `hasLineVehicleManager : boolean`: Shows the line and vehicle manager button (not in the map editor).
- `hasInfrastructures : boolean`: Shows the infrastructure construction buttons, including warehouses (not in the map editor).
- `hasTracksMenu : boolean`: Shows the tracks construction button (not in the map editor).
- `statisticsEnabled : {StatisticsRecipeParams.TabKey : boolean}`: Which statistics tabs get a button (keys such as `"Line"`, `"Vehicle"`, `"Station"`, `"Warehouse"`, `"Depot"`, `"Town"`, `"Industry"`).
- `dateEditable : boolean`: Lets the calendar editor change the current date (map editor and sandbox mode).
- `environmentEditable : boolean`: Lets the calendar editor change the environment (map editor and sandbox mode).
- `isWelcomeScreenEnabled : boolean`: Set to true outside the map editor and missions; the base game's game bar does not read it.

##### GameBar.GameBarParam.Setup.Money (record)

```teal
record Money
```

Settings of the money display.

- `show : boolean`: Shows the money display (off in the map editor).
- `editable : boolean`: Lets the player edit the money amount in the money display.

### GameBar.MainRadialMenuParam (record)

```teal
record MainRadialMenuParam
```

Parameters of the `MainRadialMenu` recipe.

- `gameCtx : GameContext`: Game GUI context.
- `modAreaEnabled : boolean`: Adds a "Mods" entry that focuses the main mod button area; the base game sets it when plugins for that area exist.
- `statisticsEnabled : {StatisticsRecipeParams.TabKey : boolean}`: Which statistics tabs get an entry in the radial menu.
- `dateEditable : boolean`: Passed to the calendar editor tool opened from the radial menu.
- `environmentEditable : boolean`: Passed to the calendar editor tool opened from the radial menu.
