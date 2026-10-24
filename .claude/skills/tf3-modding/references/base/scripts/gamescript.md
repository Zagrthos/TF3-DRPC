# scripts.gamescript

Definition file: `base/tealdef/scripts/gamescript.d.tl` (in the game folder).

Types for game scripts (`*.gs.lua` files whose `updateScript`, `postUpdateScript`, `handleEventScript`
and `guiHandleEventScript` entries point to functions as `"file.script@functionName"`), their persistent
state object and the payloads of engine events such as `OnCalcTicketPrice`.

## CalcTicketPriceEvent (record)

```teal
global record CalcTicketPriceEvent
```

One element of the `param` list of the `OnCalcTicketPrice` event sent by `SimEntityAtVehicleSystem` for
passengers. A handler can return a table `{index : multiplier}` to scale the ticket price of selected
elements (index `0` applies to all).

- `vehicleEntity : Engine.Entity`: The vehicle the passenger travels in.
- `lineEntity : Engine.Entity`: The line the vehicle serves.
- `simEntity : Engine.Entity`: The simulated person paying the ticket.
- `basePrice : number`: Ticket price before script multipliers.
- `distance : number`: Travelled distance the price is based on.

## CalcTicketPriceCargoEvent (record)

```teal
global record CalcTicketPriceCargoEvent
```

One element of the `param` list of the `OnCalcTicketPrice` event sent by `TransportVehicleSystem` for
cargo. A handler can return a table `{index : multiplier}` to scale the price of selected elements.

- `vehicleEntity : Engine.Entity`: The vehicle carrying the cargo.
- `lineEntity : Engine.Entity`: The line the vehicle serves.
- `simEntity : Engine.Entity`: The simulated cargo item.
- `stockListEntity : Engine.Entity`: Stock list the cargo comes from (the base game's delivery subsidies match it against an industry's stock list).
- `basePrice : number`: Price before script multipliers.
- `distance : number`: Travelled distance the price is based on.

## StartedLineUsage (record)

```teal
global record StartedLineUsage
```

Payload of the `OnStartedLineUsage` event sent by `SimPersonSystem` when persons start a trip on a line.

- `entities : {{Engine.Entity, Engine.Entity}}`: List of pairs; the first element of each pair is the sim person entity.

## ArriveAtStop (record)

```teal
global record ArriveAtStop
```

Payload of a vehicle-arrives-at-stop event.

- `vehicleEntity : Engine.Entity`: The arriving vehicle.
- `lineEntity : Engine.Entity`: The vehicle's line.
- `stopIndex : integer`: Index of the stop the vehicle arrived at.
- `lastStopIndex : integer`

## EventData (record)

```teal
global record EventData<T, U>
```

Generic pair of an event parameter `param` and the handler's `result`.

- `param : T`: The event parameter.
- `result : U`: The handler's result.

## GameScriptState (record)

```teal
global record GameScriptState<T>
```

Persistent state object of a game script, passed to `update`, `postUpdate` and `handleEvent`. The state
table is saved with the game; change it by reading with `get`, modifying and writing back with `set`.

- `get : function(GameScriptState<T>) : T`  (call as `gameScriptState:get(…) (method)`): Returns the script's current state table.
    - used 78x in the base game, e.g. `base/content/game_mechanics/game_mechanics/achievements/achievements.script.tl:496`: `local achievementGuiState : AchievementGuiState = guiState:get()`
- `get_native : function(GameScriptState<T>) : NativeLuaTable`: Returns the state as a native Lua table without Teal typing.
- `set : function(GameScriptState<T>, T)`  (call as `gameScriptState:set(…) (method)`): Replaces the script's state with the given table.

    ```lua
    local scriptState : CompaniesGrowthState = state:get()
    applyLevel(scriptState, api.engine.util.getPlayer(), (param as table).level as integer)
    state:set(scriptState)
    ```

    - used 87x in the base game, e.g. `base/content/game_mechanics/game_mechanics/achievements/achievements.script.tl:781`: `guiState:set(achievementGuiState)`
- `set_native : function(GameScriptState<T>, NativeLuaTable)`: Replaces the state with a native Lua table.
- `hasEventSubscriptions : function(GameScriptState<T>) : boolean`  (call as `gameScriptState:hasEventSubscriptions(…) (method)`): True if the script has subscribed to at least one event.
    - used 12x in the base game, e.g. `base/content/game_mechanics/game_mechanics/arrival_tracker/arrivaltracker.script.tl:70`: `if not state:hasEventSubscriptions() then`
- `subscribeToAllEvents : function(GameScriptState<T>)`  (call as `gameScriptState:subscribeToAllEvents(…) (method)`): Makes `handleEvent` receive every event.
    - used 1x in the base game, e.g. `base/content/mission/mission/mission_sim.script.tl:186`: `scriptState:subscribeToAllEvents()`
- `subscribeToNoEvents : function(GameScriptState<T>)`: Removes all event subscriptions.
- `subscribeToEvent : function(GameScriptState<T>, string)`  (call as `gameScriptState:subscribeToEvent(…) (method)`): Subscribes `handleEvent` to events with the given name.
    - notes: The base game subscribes in `handleEvent` when it receives `initNewGame` or `handleLegacy` (sent with empty `src` and `id`).

    ```lua
    -- from base/content/game_mechanics/game_mechanics/company/company_growth.script.tl
    if src == "" and id == "" then
    	if name == "initNewGame" or name == "handleLegacy" then
    		state:subscribeToEvent("applyLevel")
    		state:subscribeToEvent("OnCalcTicketPrice")
    	end
    end
    ```

    - used 42x in the base game, e.g. `base/content/game_mechanics/game_mechanics/arrival_tracker/arrivaltracker.script.tl:71`: `state:subscribeToEvent("onDeleteState")`

## GameScriptStateReadOnly (record)

```teal
global record GameScriptStateReadOnly<T>
```

Read-only view of a game script's state, given to the GUI functions.

- `get : function(GameScriptStateReadOnly<T>) : T`: Returns the script's current state table.

## GameScript (record)

```teal
global record GameScript<T>
```

The functions a game script file exports; `*.gs.lua` names them as `"file.script@functionName"`.

- `update : function(userParams : table, state : GameScriptState<T>, dt : number) : any`: Called regularly by the game script system.
    - param `userParams`: User parameters of the game script.
    - param `state`: The script's persistent state.
    - param `dt`: Time step of this update.
    - returns: Any value; it is passed to `postUpdate` as `updateResult`.
- `postUpdate : function(userParams : table, state : GameScriptState<T>, dt : number, updateResult : any)`: Called after `update` with its result; the base game sends commands from here (for example building proposals).
    - param `updateResult`: The value returned by `update`.
- `handleEvent : function(userParams : table, state : GameScriptState<T>, src : string, id : string, name : string, param : any) : any`: Receives events the script subscribed to.
    - param `src`: Source of the event; empty for game lifecycle events such as `initNewGame`.
    - param `id`: Receiver/system id, e.g. `"TransportVehicleSystem"` or the id used in `api.cmd.makeScriptingSendEventCmd`.
    - param `name`: Event name, e.g. `"OnCalcTicketPrice"`.
    - param `param`: Event payload.
    - returns: Event-specific result, e.g. ticket price multipliers for `OnCalcTicketPrice` or metadata for `storeSavegameMetadata`.

## GameScriptWithGui (record)

```teal
global record GameScriptWithGui<StateT, GuiStateT>
```

A game script that also exports GUI-side functions, with separate engine state `StateT` and GUI state `GuiStateT`.

- `update : function(userParams : table, state : GameScriptState<StateT>, dt : number) : any`: Called regularly by the game script system; the result is passed to `postUpdate`.
- `postUpdate : function(userParams : table, state : GameScriptState<StateT>, dt : number, updateResult : any)`: Called after `update` with its result.
- `handleEvent : function(userParams : table, state : GameScriptState<StateT>, src : string, id : string, name : string, param : any) : any`: Receives events the script subscribed to (see `GameScript.handleEvent`).
- `guiUpdate : function(userParams : table, state : GameScriptStateReadOnly<StateT>, guiState : GameScriptState<GuiStateT>)`: GUI-side update; reads the engine state read-only and may write its own GUI state.
- `guiHandleEvent : function(userParams : table, state : GameScriptStateReadOnly<StateT>, guiState : GameScriptState<GuiStateT>, src : string, id : string, name : string, param : any) : any`: GUI-side event handler; reads the engine state read-only and may write its own GUI state.
