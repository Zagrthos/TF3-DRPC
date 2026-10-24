# mission.tasks.dialogue.mission_task_dialogue

Definition file: `base/tealdef/mission/tasks/dialogue/mission_task_dialogue.d.tl` (in the game folder).

Task that plays a dialogue (voice-overs, subtitles, portraits).

## MissionTaskDialogue (record)

```teal
global record MissionTaskDialogue is MissionInterface.TaskScript<MissionTaskDialogue.State, MissionTaskDialogue.Params>
```
Load: `local dialogue = ug_require "::/mission/tasks/dialogue/dialogue.tl" as MissionTaskDialogue`

Task (`dialogue`) that plays a dialogue in the dialogue overlay when it starts and completes when the dialogue ends. If the GUI reloads while it plays, it is started again.

Example:
```lua
-- urbangames_campaign_mission_01
taskScript = dialogue as MissionInterface.TaskScript<any, any>,
taskParams = {
    filePath = "urbangames_campaign_mission_01::/mission/keyframes/MISSION_01_CUTSCENE_BUILD_PATH.script@get",
    syncWithCutscene = true,
},
followup = { { taskKey = "bulldoze" } },
```

### MissionTaskDialogue.Params (record)

```teal
record Params
```

Params of the task.

- `filePath : string`: Dialogue script. The campaign mods pass a `languages.script` key that maps to a per-language keyframe file.
- `syncWithCutscene : boolean`: Plays the dialogue in sync with a cutscene running at the same time.

### MissionTaskDialogue.State (enum)

```teal
enum State
```

Playback state.

- `"Initial"`: Not started (or restarted after a GUI reload).
- `"Playing"`: Playing.
- `"Complete"`: Ended.
