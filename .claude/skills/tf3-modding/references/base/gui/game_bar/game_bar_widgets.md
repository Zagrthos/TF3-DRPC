# gui.game_bar.game_bar_widgets

Definition file: `base/tealdef/gui/game_bar/game_bar_widgets.d.tl` (in the game folder).

Widgets shown in the game bar: game speed buttons, calendar display and the "Weather and Time" editor, camera and music player buttons, and the info display that mods can extend through `GameBarInfoDisplayExtension`.

## GameBarWidgets (record)

```teal
global record GameBarWidgets
```
Load: `local game_bar_widgets = ug_require "::/gui/game_bar/game_bar_widgets.tl" as GameBarWidgets`

Widgets of the game bar.

- `GameSpeedControl : Recipe<GameSpeedControlParams>`: Recipe of the game speed buttons.
  In game: "Pause", "Speed: 1x", "Speed: 4x" (German: „Pause“, „Geschwindigkeit: 1x“, „Geschwindigkeit: 4x“)
    - used 1x in the base game, e.g. `base/content/gui/gui/game_bar/game_bar.tl:1342`: `game_bar_widgets.GameSpeedControl{`
- `CalendarDisplay : Recipe<DateState>`: Recipe that shows the given date with a weather icon in the game bar.
    - used 1x in the base game, e.g. `base/content/gui/gui/game_bar/game_bar.tl:1371`: `content = game_bar_widgets.CalendarDisplay(dateState:old()),`
- `makeDateState : function() : DateState`  (call as `game_bar_widgets.makeDateState(…)`): Reads the current game date from the world's `GAME_TIME` component.
    - returns: The current date as `DateState`.
    - used 1x in the base game, e.g. `base/content/gui/gui/game_bar/game_bar.tl:1132`: `local dateState = engine_react_util.useStepState(game_bar_widgets.makeDateState)`
- `CalendarEditorTool : ReactToolDefinition<CalendarEditorToolParams>`: Tool with the "Weather and Time" window: weather, time of day and calendar speed, plus date and environment when enabled in `CalendarEditorToolParams`.
    - used 3x in the base game, e.g. `base/content/gui/gui/game_bar/game_bar.tl:30`: `getToolStackApi(gameCtx).pop(game_bar_widgets.CalendarEditorTool, nil)`
- `AdvancedCameraControl : Recipe0`: Toggle button that shows or hides the camera manager window (`cameraManagerWindow`).
  In game: "Advanced Camera" (German: „Erweiterte Kamera“)
    - used 1x in the base game, e.g. `base/content/gui/gui/game_bar/game_bar.tl:1316`: `cameraToolVisibleState:old() and game_bar_widgets.AdvancedCameraControl() or nil,`
- `FreeCameraTool : Recipe<GameContext>`: Button that pushes the free camera tool (input action `selectCameraTool`).
  In game: "Free Camera" (German: „Freie Kamera“)
    - used 1x in the base game, e.g. `base/content/gui/gui/game_bar/game_bar.tl:1315`: `game_bar_widgets.FreeCameraTool(react.ref(freeCameraToolRef), param.gameCtx),`
- `MusicPlayerControl : Recipe<GameContext>`: Tool button that opens the music player (input action `selectMusicPlayer`).
  In game: "Music Player" (German: „Musikplayer“)
    - used 1x in the base game, e.g. `base/content/gui/gui/game_bar/game_bar.tl:1329`: `game_bar_widgets.MusicPlayerControl(react.ref(musicPlayerControlRef), param.gameCtx),`
- `GameBarInfoDisplay : Recipe0`: Horizontal, scrollable area that shows all plugins of `GameBarInfoDisplayExtension`.
    - used 1x in the base game, e.g. `base/content/gui/gui/game_bar/game_bar.tl:1069`: `game_bar_widgets.GameBarInfoDisplay(),`
- `GameBarInfoDisplayExtension : ExtensionPoint0`: Extension point for widgets in the game bar info display; the base game adds the earnings and transported displays through it.
    - notes: A plugin is registered with `react.RegisterPluginRecipe(game_bar_widgets.GameBarInfoDisplayExtension, "Name", fn)` and a resource of type `"react-plugin ::GameBarInfoDisplayExtension"` with `filePath` and `priority` (see `gui/game_bar/game_bar_display_earnings_plugin`).

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

    - used 2x in the base game, e.g. `base/content/gui/gui/game_bar/game_bar_display_earnings_plugin/game_bar_display_earnings.script.tl:7`: `local GameBarEarningsPlugin = react.RegisterPluginRecipe(game_bar_widgets.GameBarInfoDisplayExtension, "GameBarEarningsPlugin", function() : TreeNodeId`

### GameBarWidgets.GameSpeedControlParams (record)

```teal
record GameSpeedControlParams
```

Parameters of `GameSpeedControl`.

- `gameSpeedHelper : ReactRefWrapApi0<GameSpeedApi>`: API used to read and set the game speed.
- `gameUiProxyRootRef : ReactRefWrap`: Source node for the `IA_GAME_PAUSE_OR_CYCLE_SPEED` keybinding hint.
- `gameCtx : GameContext`: Game GUI context.

### GameBarWidgets.DateState (record)

```teal
record DateState
```

A calendar date split into year, month and day.

- `year : integer`: Year.
- `month : integer`: Month.
- `day : integer`: Day of the month.

### GameBarWidgets.CalendarEditorToolParams (record)

```teal
record CalendarEditorToolParams is ToolReactUtil.IToolParam
```

Tool parameters of the "Weather and Time" window (`CalendarEditorTool`).

- `dateEditable : boolean`: Adds the "Current Date" card to change the date.
- `environmentEditable : boolean`: Adds the "Environment" card.
