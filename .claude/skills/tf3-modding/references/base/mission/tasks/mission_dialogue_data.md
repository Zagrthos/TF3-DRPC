# mission.tasks.mission_dialogue_data

Definition file: `base/tealdef/mission/tasks/mission_dialogue_data.d.tl` (in the game folder).

Data format of dialogue and cutscene keyframe files and of task instructions.

## MissionDialogueData (record)

```teal
global record MissionDialogueData
```

Data format of dialogue and cutscene keyframe files (`function data() return { ... } end`) and of task instructions. Times and durations are in seconds.

### MissionDialogueData.CameraFrame (record)

```teal
record CameraFrame
```

Camera keyframe of a cutscene.

- `angle : number`: Camera yaw.
- `fovY : number`: Vertical field of view.
- `pitch : number`: Camera pitch.
- `position : Vec3f`: Camera position.
- `roll : number`: Camera roll.
- `skyRotation : number`: Rotation of the sky.
- `speed : number`: Speed of the camera movement.
- `time : number`: Time of the keyframe in seconds.

### MissionDialogueData.Fade (record)

```teal
record Fade
```

Fade to and from black.

- `duration : number`: Length of the fade in seconds.
- `fadeIn : number`: Fade-in time.
- `fadeOut : number`: Fade-out time.
- `time : number`: Start time in seconds.

### MissionDialogueData.Subtitle (record)

```teal
record Subtitle
```

Subtitle line.

- `duration : number`: Display time in seconds.
- `text : string`: Text, usually a translation key passed through `_()`.
- `time : number`: Start time in seconds.

### MissionDialogueData.VoiceOver (record)

```teal
record VoiceOver
```

Voice-over sound.

- `fileName : string`: Sound file (`.wav`).
- `time : number`: Start time in seconds.

### MissionDialogueData.Portrait (record)

```teal
record Portrait
```

Character portrait shown next to the subtitles.

- `duration : number`: Display time in seconds.
- `path : string`: Image file.
- `time : number`: Start time in seconds.

### MissionDialogueData.DialogueData (record)

```teal
record DialogueData
```

Content of a dialogue or cutscene keyframe file.

Example:
```lua
-- urbangames_campaign_mission_01: keyframes/en/MISSION_01_CUTSCENE_BRIDGE_COMPLETE.lua
function data()
return {
    camera = {},
    fades = {},
    music = "",
    portraits = {
        { duration = 8.77, path = "urbangames_campaign_mission_01::/mission/dialogue/katie_baker_neutral.tga", time = 0 },
    },
    subtitles = {
        { duration = 8.27, text = _("MISSION_01_CUTSCENE_BRIDGE_COMPLETE_1"), time = 0.5 },
    },
    voiceOvers = {
        { fileName = "urbangames_campaign_mission_01::/audio/voice_over/MISSION_01_CUTSCENE_BRIDGE_COMPLETE_1.wav", time = 0.5 },
    },
}
end
```

- `camera : {CameraFrame}`: Camera keyframes.
- `fades : {Fade}`: Fades.
- `music : string`: Music file; empty for none.
- `subtitles : {Subtitle}`: Subtitle lines.
- `voiceOvers : {VoiceOver}`: Voice-over sounds.
- `portraits : {Portrait}`: Portraits.

### MissionDialogueData.InstructionData (record)

```teal
record InstructionData
```

Single voice-over instruction of a task, played from the task window.

- `subtitle : string`: Subtitle text.
- `voiceOver : string`: Sound file.
- `portrait : string`: Portrait image of the speaker.
- `duration : number`: Length in seconds.
