# mission.tasks.utility.mission_task_substitute_constructions

Definition file: `base/tealdef/mission/tasks/utility/mission_task_substitute_constructions.d.tl` (in the game folder).

Task that replaces or refreshes constructions.

## MissionTaskSubstituteConstructions (record)

```teal
global record MissionTaskSubstituteConstructions is MissionInterface.TaskScript<MissionTaskSubstituteConstructions.State, MissionTaskSubstituteConstructions.Params>
```
Load: `local substitute_constructions = ug_require "::/mission/tasks/utility/substitute_constructions.tl" as MissionTaskSubstituteConstructions`

Task (`substitute_constructions`) that replaces, refreshes or rebuilds constructions with a new file or new params, optionally giving them to the player, then completes.

### MissionTaskSubstituteConstructions.Params (record)

```teal
record Params
```

Params of the task.

- `newConstruction : ResName`: Construction file of the replacement.
- `baseName : string`: Name of the new constructions, numbered `"<baseName> <n>"` (default "Construction").
- `getEntities : function(readOnlyApi : MissionInterface.MissionTaskReadOnlyApi) : {Engine.Entity}`: Returns the constructions to substitute.
    - param `readOnlyApi`: Read-only mission API.
    - returns: Construction entities.
- `getRuleCapacityMultiplier : function(entity : Engine.Entity, readOnlyApi : MissionInterface.MissionTaskReadOnlyApi) : {integer : number}`: Returns the `ruleCapacityMultiplier` construction param of a new construction.
    - param `entity`: Construction being replaced.
    - param `readOnlyApi`: Read-only mission API.
    - returns: Index -> multiplier.
- `transferOwnershipToPlayer : boolean`: Gives the constructions to the player afterwards.
- `useRefreshFn : boolean`: Refreshes each construction with `newConstruction` (`api.engine.util.proposal.refreshConstruction`) instead of rebuilding it.
- `useReplaceFn : boolean`: Replaces each construction with new params (`createProposalReplaceConstruction`) instead of rebuilding it.
- `makeReplaceParams : function(params : table) : table`: Builds the new construction params for `useReplaceFn`.
    - param `params`: Current construction params.
    - returns: New construction params.

### MissionTaskSubstituteConstructions.State (record)

```teal
record State
```

State of the task.

- `isComplete : boolean`: The substitution was done.
- `entities : {Engine.Entity}`: Constructions that were substituted.
