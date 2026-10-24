# mission.tasks.utility.mission_task_landmark_requirements

Definition file: `base/tealdef/mission/tasks/utility/mission_task_landmark_requirements.d.tl` (in the game folder).

Task that swaps a landmark construction for a variant chosen at runtime.

## MissionTaskLandmarkRequirements (record)

```teal
global record MissionTaskLandmarkRequirements is MissionInterface.TaskScript<MissionTaskLandmarkRequirements.State, MissionTaskLandmarkRequirements.Params>
```
Load: `local landmark_requirements = ug_require "::/mission/tasks/utility/landmark_requirements.tl" as MissionTaskLandmarkRequirements`

Task (`landmark_requirements`) that replaces a construction with `<baseName>_<n>`, where `n` comes from `getRequirements`, keeping its name and params.

### MissionTaskLandmarkRequirements.Params (record)

```teal
record Params
```

Params of the task.

- `entity : Engine.Entity`: Construction entity to replace.
- `baseName : string`: Construction file name without the `_<n>` suffix.
- `getRequirements : function(readOnlyApi : MissionInterface.MissionTaskReadOnlyApi) : integer`: Returns the variant number appended to `baseName`.
    - param `readOnlyApi`: Read-only mission API.
    - returns: Variant number.

### MissionTaskLandmarkRequirements.State (record)

```teal
record State
```

State of the task.

- `isComplete : boolean`: The construction was replaced.
