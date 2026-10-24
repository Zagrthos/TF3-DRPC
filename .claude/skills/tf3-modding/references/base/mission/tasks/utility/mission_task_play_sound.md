# mission.tasks.utility.mission_task_play_sound

Definition file: `base/tealdef/mission/tasks/utility/mission_task_play_sound.d.tl` (in the game folder).

Task that plays a sound effect.

## MissionTaskPlaySound (record)

```teal
global record MissionTaskPlaySound is MissionInterface.TaskScript<MissionTaskPlaySound.State, MissionTaskPlaySound.Params>
```
Load: `local play_sound = ug_require "::/mission/tasks/utility/play_sound.tl" as MissionTaskPlaySound`

Task (`play_sound`) that plays a sound effect in the GUI when it starts and completes at once.

### MissionTaskPlaySound.Params (record)

```teal
record Params
```

Params of the task.

- `file : FilePath`: Sound file to play.

### MissionTaskPlaySound.State (record)

```teal
record State
```

Empty state.
