# mission.mission_framework.mission_framework_react_util

Definition file: `base/tealdef/mission/mission_framework/mission_framework_react_util.d.tl` (in the game folder).

GUI building blocks of the mission window (task progress, option buttons, instruction buttons, links).

## MissionFrameworkReactUtil (record)

```teal
global record MissionFrameworkReactUtil
```
Load: `local mission_framework_react_util = ug_require "::/mission/mission_framework/mission_framework_react_util.tl" as MissionFrameworkReactUtil`

GUI building blocks of the mission window.

- `makeTaskProgressItem : function(progress : MissionInterface.TaskProgressData) : TreeNodeId`  (call as `mission_framework_react_util.makeTaskProgressItem(…)`): Builds the GUI node that shows one progress entry (see `MissionInterface.TaskProgressData` for the forms).
  In game: "Progress", "{currentProgress}/{maxProgress} per Year" (German: „Fortschritt“, „{currentProgress}/{maxProgress} pro Jahr“)
    - param `progress`: Progress to show.
    - returns: GUI node, or `nil` if nothing applies.
- `followLink : function(link : MissionInterface.TaskInfo.LinkDef)`  (call as `mission_framework_react_util.followLink(…)`): Moves the camera to a link's entity or area.
    - param `link`: Link to follow.
    - used 1x in the base game, e.g. `base/content/mission/mission/mission_framework/mission_framework_react.tl:1432`: `mission_framework_react_util.followLink(locateTarget)`
- `handleLinkClick : function(taskInfo : MissionInterface.TaskInfo, target : string)`  (call as `mission_framework_react_util.handleLinkClick(…)`): Follows the link with the given key of a task info.
    - param `taskInfo`: Task info with `links`.
    - param `target`: Link key clicked in a paragraph text.
    - used 2x in the base game, e.g. `base/content/mission/mission/mission_framework/mission_framework_react.tl:260`: `mission_framework_react_util.handleLinkClick(taskInfo, target)`
- `fixParagraphsForInputMode : function(original : MissionInterface.TaskInfo) : MissionInterface.TaskInfo`  (call as `mission_framework_react_util.fixParagraphsForInputMode(…)`): Replaces paragraphs that only have `textMouse` / `textGamepad` with the text for the current input mode.
    - param `original`: Task info.
    - returns: The task info, copied if a paragraph changed.
    - used 1x in the base game, e.g. `base/content/mission/mission/mission_framework/mission_framework_react.tl:70`: `result.info = mission_framework_react_util.fixParagraphsForInputMode(`
- `MissionTaskOptionButtons : Recipe<MissionTaskOptionButtonsParam>`: Recipe that shows option buttons; clicking sends a `Decision` event with the option key to the task.
    - used 1x in the base game, e.g. `base/content/mission/mission/mission_framework/mission_framework_react.tl:315`: `optionsItem = mission_framework_react_util.MissionTaskOptionButtons(react.ref(initialFocusRef), {`
- `MissionTaskProgress : Recipe2<MissionInterface.TaskInfo, boolean>`: Recipe that shows a task's progress and sub-task progress, optionally with its name.
    - used 2x in the base game, e.g. `base/content/mission/mission/mission_framework/mission_framework_react.tl:272`: `detailsChildren[#detailsChildren + 1] = MissionTaskDetailsCard(mission_framework_react_util.MissionTaskProgress(taskInfo, false))`
- `MissionTaskInstructionsOverlayButton : Recipe<MissionTaskInstructionsOverlayButtonParam>`: Recipe of the portrait button that plays or stops a task's instruction.
    - used 2x in the base game, e.g. `base/content/mission/mission/mission_framework/mission_framework_react.tl:247`: `detailsChildren[#detailsChildren + 1] = MissionTaskDetailsCard(mission_framework_react_util.MissionTaskInstructionsOverlayButton(param))`

### MissionFrameworkReactUtil.MissionTaskOptionButtonsParam (record)

```teal
record MissionTaskOptionButtonsParam
```

Params of the option-button recipe.

- `taskUid : string`: Task that receives the `Decision` event.
- `options : {MissionInterface.TaskInfo.Option}`: Options to show.
- `onSelectOption : function(option : MissionInterface.TaskInfo.Option)`: Called after an option was clicked.

### MissionFrameworkReactUtil.MissionTaskInstructionsOverlayButtonParam (record)

```teal
record MissionTaskInstructionsOverlayButtonParam is IRecipeParamWithMeta
```

Params of the instruction (voice-over) button.

- `gameCtx : GameContext`: Game context (gives access to the dialogue overlay).
- `taskUid : string`: Task the instruction belongs to.
- `instructionData : MissionDialogueData.InstructionData`: Instruction to play.
- `paragraph : MissionInterface.TaskInfo.Paragraph`: Paragraph shown next to the button.
- `taskInfo : MissionInterface.TaskInfo`: Task info used to resolve paragraph links.
- `autoPlay : boolean`: Starts the instruction when the button appears.
- `isTaskComplete : boolean`: Disables paragraph links for completed tasks.
