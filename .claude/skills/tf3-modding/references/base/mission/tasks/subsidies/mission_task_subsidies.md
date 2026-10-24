# mission.tasks.subsidies.mission_task_subsidies

Definition file: `base/tealdef/mission/tasks/subsidies/mission_task_subsidies.d.tl` (in the game folder).

Task that offers subsidies (subventions) and waits until all are completed or failed.

## MissionTaskSubsidies (record)

```teal
global record MissionTaskSubsidies is MissionInterface.TaskScript<MissionTaskSubsidies.State, MissionTaskSubsidies.Params>
```
Load: `local subsidies = ug_require "::/mission/tasks/subsidies/subsidies.tl" as MissionTaskSubsidies`

Task (`subsidies`) that spawns subventions through the `Subvention` script and completes when none of them is still proposed or being verified. Its progress counts completed subsidies against those not failed.

### MissionTaskSubsidies.Params (record)

```teal
record Params
```

Params of the task.

- `subsidies : {{ResName, ISubvention.ISpecificSubventionParam}}`: List of `{ subvention type, specific params }` to spawn at start.
- `getOptionalSubsidies : function(readOnlyApi : MissionInterface.MissionTaskReadOnlyApi) : {{ResName, ISubvention.ISpecificSubventionParam}}`: Returns more subsidies to spawn once; the task waits until it has returned a list.
    - param `readOnlyApi`: Read-only mission API.
    - returns: List of `{ subvention type, specific params }`, or `nil` to try again.

### MissionTaskSubsidies.Status (enum)

```teal
enum Status
```

Status of one spawned subsidy.

- `"Verifying"`: Spawn sent; not yet found in the subventions game script.
- `"Proposed"`: Offered to the player.
- `"Failed"`: Declined or failed.
- `"Completed"`: Completed (or no longer present when checked).

### MissionTaskSubsidies.State (record)

```teal
record State
```

State of the task.

- `subsidies : {{number, Status}}`: List of `{ uid, status }` of the spawned subsidies.
- `optionalSubsidiesSpawned : boolean`: The optional subsidies were spawned.
