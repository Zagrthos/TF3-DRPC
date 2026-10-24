# mission.mission_react_util

Definition file: `base/tealdef/mission/mission_react_util.d.tl` (in the game folder).

GUI parameters of the mission dialogue / voice-over overlay.

## MissionReactUtil (record)

```teal
global record MissionReactUtil
```
Load: `local mission_react_util = ug_require "::/mission/mission_react_util.tl" as MissionReactUtil`

GUI of the mission dialogue and instruction overlay.

- `DialogueOverlay : RecipeWithApi0<DialogueOverlayApi>`: Recipe of the dialogue / voice-over overlay; its API reports what is playing.
    - used 2x in the base game, e.g. `base/content/gui/gui/main/game.tl:731`: `local dialogueOverlayRef = react.useNodeRef(mission_react_util.DialogueOverlay)`

### MissionReactUtil.DialogueEventParam (record)

```teal
record DialogueEventParam
```

Parameter of the `dialogue` and `on-finish-dialogue` React events.

- `id : string`: `start-dialogue`, `start-instruction` or `on-finish-dialogue`.
- `scriptName : string`: Dialogue script to play.
- `instructionData : MissionDialogueData.InstructionData`: Instruction to play.
- `taskUid : string`: Task the dialogue or instruction belongs to.
- `instructionKey : string`: Key of a queued instruction.
- `syncWithCutscene : boolean`: Plays the dialogue in sync with a running cutscene.

### MissionReactUtil.InstructionsOverlayParam (record)

```teal
record InstructionsOverlayParam
```

- `startTime : number`
- `instructionData : MissionDialogueData.InstructionData`
- `taskUid : string`
- `instructionKey : string`
- `setEndState : function()`

### MissionReactUtil.DialogueOverlayContentParam (record)

```teal
record DialogueOverlayContentParam
```

- `startTime : number`
- `scriptName : string`
- `missionDialogueData : MissionDialogueData.DialogueData`
- `taskUid : string`
- `instructionKey : string`
- `syncWithCutscene : boolean`
- `setDeactivateState : function(boolean)`
- `setEndState : function()`
- `getSkipCurrentLineRef : function() : boolean`
- `setSkipCurrentLineRef : function(boolean)`
