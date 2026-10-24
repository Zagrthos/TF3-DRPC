# mission.tasks.transport_cargo.mission_task_empty_stocks

Definition file: `base/tealdef/mission/tasks/transport_cargo/mission_task_empty_stocks.d.tl` (in the game folder).

Task that completes when every target stock list is empty. With `countdown` it also completes when the time limit runs out.

## MissionTaskEmptyStocks (record)

```teal
global record MissionTaskEmptyStocks is MissionInterface.TaskScript<MissionTaskEmptyStocks.State, MissionTaskEmptyStocks.Params>
```
Load: `local empty_stocks = ug_require "::/mission/tasks/transport_cargo/empty_stocks.tl" as MissionTaskEmptyStocks`

Task (`empty_stocks`) that completes when all target stocks are empty, or when the countdown runs out. Shows the amount removed and, for several targets, how many are empty.

### MissionTaskEmptyStocks.Params (record)

```teal
record Params
```

Params of the task.

- `targets : {Engine.Entity}`: note: targets need to be stockList entities
- `getTargets : function(readOnlyApi : MissionInterface.MissionTaskReadOnlyApi) : {Engine.Entity}`: Returns the stock-list entities when `targets` is not set; called until it returns a list.
    - param `readOnlyApi`: Read-only mission API.
    - returns: Stock-list entities.
- `simTimeMinutesToComplete : integer`: Time limit in simulation minutes (game time).
- `countdown : boolean`: Ends the task when the time limit runs out and shows the remaining time.

### MissionTaskEmptyStocks.State (record)

```teal
record State
```

State of the task.

- `targets : {Engine.Entity}`: Stock-list entities watched.
- `result : {Engine.Entity : boolean}`: Stock-list entity -> has been empty.
- `currentSimTimeMs : integer`: Game time (ms) of the last update.
- `initialSimTimeMs : integer`: Game time (ms) at which the time limit started.
- `endSimTimeMs : integer`: Game time (ms) at which the time limit ends.
- `initialTotalStored : integer`: Total stored amount at the first update.
