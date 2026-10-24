# gui.main.game_context

Definition file: `base/tealdef/gui/main/game_context.d.tl` (in the game folder).

Types for the shared in-game GUI context (`GameContext`), the dialogue overlay and callout APIs, the game speed API and the base game's GUI save data.

## Globals

- `Notification` (type): A notification as sent with the `Notifications`/`add` script event and stored in `NotificationsState`.
    - notes: `type` names a notification type script. The `.res` file with the same base name (`type = "notification"`) holds its `guiType`, `label`, `simUpdateScript` and `initiallyIgnoredType`; the script itself returns a `NotificationType` whose `useDataState(params, simParams)` builds the GUI text and icons. A notification expires (and is hidden from pop-ups) when `autoDismissDuration` has passed, or when `params.entities` is a non-empty list of `EntityUtil.EntityAndRevision` and every one of those entities has changed or is gone. `add` events are dropped while the notifications state is paused, and notifications of a fully ignored type are not stored at all (see `NotificationsState.Ignored`).

    ```lua
    -- urbangames_deluxe_upgrade_pack, fun_elements/balloon.script.tl
    api.cmd.sendCommand(api.cmd.makeScriptingSendEventCmd("", "Notifications", "add", {
        type = resolve("balloon_notification.script"),
        params = {
            entity = entity_util.makeEntityAndRevision0(balloon.entity),
            townEntity = entity_util.makeEntityAndRevision0(townEntity),
        },
        autoDismissDuration = notification_util.defaultAutoDismissDurationMs,
    }))
    ```

## DialogueOverlayApi (record)

```teal
global record DialogueOverlayApi
```

API of the mission dialogue overlay that plays voice-over dialogues, instructions and the credits.

Notes:

Provided by the `DialogueOverlay` recipe in `mission/mission_react_util.tl`. Get it from the game context with
`gameCtx.dialogueOverlayRef:get():getApi()`.

- `isPlaying : function() : boolean`: Whether a dialogue or instruction is currently shown.
- `getPlayingForTask : function() : string`: Mission task UID of the dialogue or instruction currently shown.
    - returns: The task UID, or `nil` if nothing is playing.
- `areCreditsPlaying : function() : boolean`: Whether the credits are currently shown.
- `stopCredits : function() : boolean`: Stops the credits if they are playing.
- `abortCurrent : function()`: Stops the credits and the current voice-over and ends the current dialogue or instruction; the next queued instruction, if any, starts.
    - notes: The base game calls it from the `uiCloseAll` input action.
- `skipCurrentLine : function()`: Stops the current voice-over line. An instruction is ended (the next queued one starts); in a dialogue only the current line is skipped.
    - notes: The base game calls it from the `IA_SKIP_VOICEOVER_AND_CUTSCENE` input action.
- `enqueueUnlessExisting : function(taskUid : string, instructionKey : string, function() : MissionDialogueData.InstructionData)`: Shows a mission instruction now, or queues it if something is already playing, unless the same task UID and instruction key are already queued.
    - param `taskUid`: UID of the mission task the instruction belongs to.
    - param `instructionKey`: Key identifying the instruction within the task.
    - param `#3`: Function that builds the instruction data; it is only called when the instruction is actually added. Returning `nil` logs an error and adds nothing.

## CalloutApi (record)

```teal
global record CalloutApi
```

API of the callout container, a floating layer that shows one callout (a popup recipe placed next to a GUI element or at a position) at a time.

Notes:

In the game, get it with `gameCtx.calloutContainerRef:get():getApi()`; the main menu has its own container
(`commonParams.calloutContainerRef`). Opening a new callout replaces the current one.

Example:
```lua
-- from gui/construction/construction.tl
gameCtx.calloutContainerRef:get():getApi().open({
    recipe = ConstructionInfoPanelCallout,
    recipeParam = { info = nameAndDescription },
    anchor = api.type.Vec2f.new(0.0, 1.0),
    parent = params.listRef,
    getPositionFn = function() return nameAndDescription.position end,
    delay = 0.5,
})
```

- `open : function<R is IRecipeParamWithMeta>(CalloutParamsT<R>)`: Shows a callout, replacing the current one. Calling it again with equal parameters keeps the open callout unchanged.
    - notes: The position comes from `getPositionFn` if given, otherwise from `parent`'s position at `gravity`. It is recalculated every frame. The callout closes by itself when its parent node is destroyed.
- `close : function(any)`: any=ReactRefWrap (interfaces don't work)

### CalloutApi.CalloutParamsT (record)

```teal
record CalloutParamsT<R is IRecipeParamWithMeta>
```

Parameters of `CalloutApi.open`.

- `recipe : Recipe<R>`: Recipe rendered as the callout content.
- `recipeParam : R`: Parameter passed to `recipe`. It is copied, and its style sheet gets `anchorPoint = anchor`.
- `anchor : Vec2f`: Anchor point of the callout content, in relative coordinates (0..1 per axis), placed at the computed position.
- `parent : any`: any=ReactRefWrap (interfaces don't work)
- `gravity : Vec2f`: requires parent
- `getPositionFn : function() : Vec2f`: overrides automatic placement
- `recipeParamCompareFn : function(R, R) : boolean`: overrides default compare
- `delay : number`: default 0 seconds
- `noRepeatedDelay : boolean`: default false, if true, the delay will not be applied if the callout is already open

## GameContext (record)

```teal
global record GameContext
```

Shared context of the in-game GUI. Holds references to the main renderer component, window container and tool stack, plus mission filters, disabled features and a few shared states.

Notes:

Created once by `GameUIRoot` in `gui/main/game.tl` and passed to tools, windows and HUD recipes as `gameCtx`
(for example `toolParam.gameCtx`). Code that has no `gameCtx` can reach the most important parts through
`GameReactGlobals` (`gui/main/game_react_globals.tl`).

Example:
```lua
-- open a singleton window and push a tool through the context
gameCtx.windowContainer:get():getApi().addSingletonWindow(ContextHelperWindow, { gameCtx = gameCtx, onClose = onClose })
gameCtx.toolStack:get():getApi().push(FinanceTool, nil, toolParam)
```

- `rendererComponent : ReactRefWrapApiT<Builtin.RendererComponentParam, Builtin.RenderComponentAPI>`: Reference to the main 3D view (`builtin.RendererComponent`). Used, for example, as the target of `react.fireEvent(gameCtx.rendererComponent, "selectEntity", ...)`.
- `windowContainer : ReactRefWrapApiT<Builtin.WindowContainerDelegateParam, Builtin.WindowAPI>`: Reference to the in-game window container. `gameCtx.windowContainer:get():getApi()` returns the `Builtin.WindowAPI` used to add and remove windows.
- `toolStack : ReactRefWrapApiT<Builtin.ToolStackParam, Builtin.ToolStackAPI>`: Reference to the in-game tool stack. `gameCtx.toolStack:get():getApi()` returns the `Builtin.ToolStackAPI` used to push and pop tools.
- `accessNotificationCacheFn : function(Engine.Entity) : {Notification}`: Returns the persisting notifications attached to an entity, from a cache that is refreshed every frame.
    - param `#1`: Entity id.
    - returns: List of notifications of the entity (each with `notificationId` set); empty if there are none.
- `metroMode : ReactStateT<boolean>`: Shared "metro mode" state, initially `false`. The line view shows only vehicle HUD icons while it is `true` and the camera is more than 500 m above the terrain.
- `preferredLayerConfig : ReactRefT<LayerConfig>`: Layer configuration last requested through the `preferredLayerConfig` React event (`nil` when cleared).
- `dialogueOverlayRef : ReactRefWrapApi0<DialogueOverlayApi>`: Reference to the mission dialogue overlay; `getApi()` returns a `DialogueOverlayApi`.
- `calloutContainerRef : ReactRefWrapApi0<CalloutApi>`: Reference to the in-game callout container; `getApi()` returns a `CalloutApi`.
- `filters : ReactRefT<Filters>`: Current mission filters (see `GameContext.Filters`). Read with `gameCtx.filters:get()`.
- `disableFeatures : ReactRefT<{DisableFeatures : boolean}>`: Set of currently disabled GUI features (`DisableFeatures` names mapped to `true`). Replaced by the `setDisableFeatures` React event, which the mission system fires.

    ```lua
    if gameCtx.disableFeatures:get()["GameSpeedPause"] == true then
        -- pausing is not allowed right now
    end
    ```

- `focusGameBar : function()`: Moves keyboard/gamepad focus to the game bar.

### GameContext.Filters (record)

```teal
record Filters
```

GUI restrictions set by the mission system (construction menu, vehicle store, protected entities, selectable entities).

Notes:

Filled by the `setMenuFilter`, `setVehicleFilter`, `setProtectedEntities` and `setSelectorEntityFilter` React
events, which `mission/mission_sim.script.tl` fires when the active mission tasks change. All filters are empty
by default, which means no restriction.

- `menuFilter : MenuFilter`: Construction menu restriction.
- `vehicleFilter : VehicleFilter`: Vehicle store restriction.
- `protectedEntities : {Engine.Entity : ProtectionConfig | boolean}`: Entities (lines, vehicles) the player must not change, for example in the line manager. `true` protects the entity completely; a `ProtectionConfig` protects only the listed stops of a line.
- `selectorEntityFilter : {Engine.Entity : boolean}`: If not empty, only these entities can be hovered and selected with the default selector.

#### GameContext.Filters.MenuFilter (record)

```teal
record MenuFilter
```

Restriction of the construction menu.

Notes:

Items are matched by construction tag: the definition's `resName`, with `@<constructionTemplate>` appended for
template constructions (or the action name if there is no resource name).

- `enabledItems : {string}`: Construction tags allowed in the construction menu. If not empty, only these items are shown and `disabledItems` is ignored.
- `disabledItems : {string}`: Construction tags hidden from the construction menu (used when `enabledItems` is empty).
- `disabledParams : {string}`: Ids of construction parameters to hide from the construction parameter controls.

#### GameContext.Filters.VehicleFilter (record)

```teal
record VehicleFilter
```

Restriction of the vehicle store.

- `enabledVehicles : {string}`: Model paths of the vehicles offered in the vehicle store. If not empty, other vehicles are hidden.
- `disabledVehicles : {string}`: Model paths of vehicles hidden from the vehicle store.

#### GameContext.Filters.ProtectionConfig (record)

```teal
record ProtectionConfig
```

Partial protection of an entity, used instead of `true` in `protectedEntities`.

- `line : Line`: Protection of individual stops of a line. Required when the protected entity is a line.

##### GameContext.Filters.ProtectionConfig.Line (record)

```teal
record Line
```

Protection settings for a line.

- `stationGroups : {Engine.Entity : boolean}`: Station groups whose stops on the line are protected (set to `true`).

## GameSpeedApi (record)

```teal
global record GameSpeedApi
```

API of the game speed helper used by the game bar to read and change the simulation speed.

Notes:

Provided by the `GameSpeedHelper` recipe in `gui/main/game.tl`. Speeds are speed-up factors: `0` is paused,
`1`, `2` and `4` are the normal steps. Changes are sent with `api.cmd.makeGameSetSpeedCmd` and are clamped to
`api.gui.game.getEstimatedMaximumGameSpeed()`.

- `isEnabled : function() : boolean`: Whether game speed control is enabled, that is the `GameSpeedControl` feature is not disabled.
- `getSpeed : function() : integer`: Current speed-up factor.
    - returns: `0` when paused, otherwise the last non-zero speed-up.
- `setSpeed : function(integer)`: Sets the speed-up factor (`0` pauses). Pausing is ignored while the `GameSpeedPause` feature is disabled.
- `togglePause : function()`: Pauses the game, or resumes it at the last non-zero speed.
- `cycleSpeed : function()`: Switches to the next speed step (1, 2, 4, then back to 1); when paused, resumes at speed 1.

## BaseGUISaveData (record)

```teal
global record BaseGUISaveData
```

GUI data the base game stores in the savegame, read with `api.gui.game.getGuiSaveData("")` and written with `api.gui.game.setGuiSaveData("", data)`.

Example:
```lua
-- from gui/hud/cargo_flow.script.tl
local hudFilter = (api.gui.game.getGuiSaveData("") as BaseGUISaveData).hudFilter
if not hudFilter or hudFilter.showCargoFlow then
    -- draw cargo flow
end
```

- `hudFilter : HUDFilter`: HUD icon filter settings; `nil` until the player changes a filter.

### BaseGUISaveData.FilterState (enum)

```teal
enum FilterState
```

Visibility of one kind of HUD icon.

- `"Hidden"`: The icons are hidden.
- `"Default"`: The icons are shown (default).

### BaseGUISaveData.HUDFilter (record)

```teal
record HUDFilter
```

Settings of the HUD icon filter layer (`gui/layers/layer_hud_filter.tl`). Missing entries count as shown.

- `preFilters : {Engine.ComponentType : FilterState}`: Visibility of HUD icons per component type (stations, industries, warehouses, towns, landmarks).
- `preFilterTvCarriers : {Carrier : FilterState}`: Visibility of vehicle HUD icons per carrier (rail, road, tram, water, air). If all carriers are hidden, vehicle icons are hidden completely.
- `showIncome : boolean`: Whether income HUD icons are shown (the "Income" toggle).
- `showCosts : boolean`: Whether cost HUD icons are shown (the "Costs" toggle).
- `showEventIcons : boolean`: Whether event icons (for example happiness changes, marketing and greening effects) are shown (the "Events" toggle).
- `showCargoFlow : boolean`: Whether cargo flow is shown (the "Cargo Flow" toggle).
