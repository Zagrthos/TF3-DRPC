# mission.tasks.utility.mission_task_store_any

Definition file: `base/tealdef/mission/tasks/utility/mission_task_store_any.d.tl` (in the game folder).

Task that stores a value computed at runtime for other tasks to read.

## MissionTaskStoreAny (record)

```teal
global record MissionTaskStoreAny is MissionInterface.TaskScript<MissionTaskStoreAny.State, MissionTaskStoreAny.Params>
```
Load: `local store_any = ug_require "::/mission/tasks/utility/store_any.tl" as MissionTaskStoreAny`

Task (`store_any`) that stores the first non-`nil` result of `storeFn` in its state and then completes. Other tasks read it through `getTaskState(key).specific.data`.

Notes:

Use `MissionTaskStoreEntity` for entities: it survives the engine swapping entity ids.

### MissionTaskStoreAny.Params (record)

```teal
record Params
```

Params of the task.

- `storeFn : function(readOnlyApi : MissionInterface.MissionTaskReadOnlyApi) : any`: Computes the value to store; called every update until it returns a value.
    - param `readOnlyApi`: Read-only mission API.
    - returns: Value to store, or `nil` to try again.

### MissionTaskStoreAny.State (record)

```teal
record State
```

State of the task.

- `data : any`: Stored value.
- `waitForEngineSwap : boolean`: Delays completion by one update after the value was stored.
