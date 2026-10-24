# mission.tasks.transfer_ownership.mission_task_transfer_ownership

Definition file: `base/tealdef/mission/tasks/transfer_ownership/mission_task_transfer_ownership.d.tl` (in the game folder).

Task that gives lines and constructions to the player or takes them away.

## MissionTaskTransferOwnership (record)

```teal
global record MissionTaskTransferOwnership is MissionInterface.TaskScript<MissionTaskTransferOwnership.State, MissionTaskTransferOwnership.Params>
```
Load: `local transfer_ownership = ug_require "::/mission/tasks/transfer_ownership/transfer_ownership.tl" as MissionTaskTransferOwnership`

Task (`transfer_ownership`) that gives lines and constructions to the player or moves them to a newly spawned AI player, then completes.

Example:
```lua
-- urbangames_campaign_mission_01
taskScript = transfer_ownership as MissionInterface.TaskScript<any, any>,
taskParams = {
    removeLineFromPlayer = params.entityList_nonPlayerOwnedLines,
    removeEntityFromPlayer = { params.entity_relevantHarbor, params.entity_newOrleansRoadDepot },
    assignEntityToPlayer = { params.entity_stationHotel, params.entity_newOrleansPort },
},
followup = { { taskKey = "transfer_ownership_depots" } },
```

### MissionTaskTransferOwnership.Params (record)

```teal
record Params
```

Params of the task.

- `assignLineToPlayer : {Engine.Entity}`: Lines given to the player, with their vehicles and stops.
- `removeLineFromPlayer : {Engine.Entity}`: Lines moved to an AI player, with their vehicles and stops.
- `assignEntityToPlayer : {Engine.Entity}`: Entities (and their constructions, stations, station groups) given to the player.
- `removeEntityFromPlayer : {Engine.Entity}`: Entities (and their constructions, stations, station groups) moved to an AI player.
- `getTransferLineAndPlayerOwnership : function(readOnlyApi : MissionInterface.MissionTaskReadOnlyApi) : {Engine.Entity}, boolean`: Second return value, if true: assign, else: remove
- `getTransferEntityAndPlayerOwnership : function(readOnlyApi : MissionInterface.MissionTaskReadOnlyApi) : {Engine.Entity}, boolean`: Second return value, if true: assign, else: remove

### MissionTaskTransferOwnership.State (record)

```teal
record State
```

State of the task.

- `isComplete : boolean`: The transfer was done.
