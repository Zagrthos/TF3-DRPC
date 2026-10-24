# mission.mission_framework.mission_interface

Definition file: `base/tealdef/mission/mission_framework/mission_interface.d.tl` (in the game folder).

Core types of the mission framework, shared by missions, the tutorial and the guide system. It defines task scripts, task state, the task info shown in the mission window and the APIs passed to task callbacks.

## MissionInterface (record)

```teal
global record MissionInterface
```

Namespace of the core mission-framework types. A mission is a list of tasks; each task is run by a `TaskScript` and described by static data.

Notes:

How the pieces fit together (from `mission/mission_sim.script.tl` and the campaign mods):

1. A mod registers a generic resource of type `missiontask_boot` (mission), `tutorial_boot`
   (tutorial) or `guide_system_boot` (guide system). Its `data` is a `BootstrapDesc` whose
   `bootstrapScriptFn` points to the `spawnBootTasks` function of a module built with
   `MissionBootUtil.makeMission` / `makeGuide`.
2. The game script `::mission/mission.gs` (or the guide-system script) calls each bootstrap
   function once per savegame (remembered in `MissionState.bootTypes`); it adds the boot tasks.
3. Each task is identified by a `TaskConfig`. The framework resolves its `TaskVTable` through
   `getVTableScriptFn`, which returns the task's `TaskScript` and its static params from the
   story's task-data factory (`MissionTaskUtil.StaticTaskInfo`).
4. Every update the framework calls `onUpdate` and `isComplete` of each open task. When a task
   completes, `onComplete` runs and its `followup` tasks are added.

State is kept in the game script state (`MissionState`); everything in `StaticTaskInfo`
(functions included) is re-created from the story code and never saved.

Example:
```lua
-- mission.res.lua of urbangames_campaign_mission_01
function data()
return {
    type = "missiontask_boot",
    data = {
        bootstrapScriptFn = "urbangames_campaign_mission_01::/mission/mission.script@mission.spawnBootTasks",
        bootstrapScriptFnParams = nil,
    }
}
end

-- mission.script.lua
local mission = require "mission.tl"
function data()
    return { mission = mission }
end
```

### MissionInterface.GameScriptMode (enum)

```teal
enum GameScriptMode
```

Which framework instance a game script runs, either the mission (missions and the tutorial) or the guide system.

- `"GuideSystem"`: Guide-system instance; boots from `guide_system_boot` resources and only runs when guides are enabled in the app config.
- `"Mission"`: Mission instance; boots from `missiontask_boot` resources, or from `tutorial_boot` when no mission is present.

### MissionInterface.CaptureParams (record)

```teal
record CaptureParams
```

Parameters that the game-script resource (`mission.gs.lua`) passes to the framework's update and event scripts.

- `gameScriptMode : GameScriptMode`: Which instance of the framework this game script is.

### MissionInterface.MissionSavegameUtil (record)

```teal
record MissionSavegameUtil
```
Load: `local mission_savegame_util = ug_require "::/mission/mission_savegame_util.tl" as MissionSavegameUtil`

Helpers that prepare a mission savegame.

- `updateTownBuildings : function()`  (call as `mission_savegame_util.updateTownBuildings(…)`): Rebuilds every construction that contains town buildings, so they are refreshed from their current resources.
    - used 8x in the base game, e.g. `mods/release/urbangames_campaign_mission_01/content/init/init/init.script.tl:16`: `mission_savegame_util.updateTownBuildings()`
- `updateMaintenanceCost : function()`  (call as `mission_savegame_util.updateMaintenanceCost(…)`): Resets the maintenance cost of every entity that has one, and removes the owner of street/track edges that do not belong to a construction.
    - used 8x in the base game, e.g. `mods/release/urbangames_campaign_mission_01/content/init/init/init.script.tl:19`: `mission_savegame_util.updateMaintenanceCost()`
- `updateIndustriesPlayerOwned : function(except? : {Engine.Entity})`  (call as `mission_savegame_util.updateIndustriesPlayerOwned(…)`): Removes the owner (sets player `-1`) of every industry construction.
    - param `except`: Industry entities to leave unchanged.
    - used 8x in the base game, e.g. `mods/release/urbangames_campaign_mission_01/content/init/init/init.script.tl:22`: `mission_savegame_util.updateIndustriesPlayerOwned()`

### MissionInterface.MissionLegacyUtil (record)

```teal
record MissionLegacyUtil
```
Load: `local mission_legacy_util = ug_require "::/mission/mission_framework/mission_legacy_util.tl" as MissionLegacyUtil`

Upgrades a mission state saved by an older game version.

- `currentVersion : integer`: Current version of the mission state layout (4 in the base game).
    - used 1x in the base game, e.g. `base/content/mission/mission/mission_sim.script.tl:75`: `version = mission_legacy_util.currentVersion,`
- `updateLegacyState : function(missionState : MissionState)`  (call as `mission_legacy_util.updateLegacyState(…)`): Fills fields missing in older states, removes tasks whose static data no longer exists (`needTaskDeletion`), calls each task script's `handleLegacy` and sets `version` to `currentVersion`.
    - param `missionState`: Mission state to upgrade in place.
    - used 1x in the base game, e.g. `base/content/mission/mission/mission_sim.script.tl:867`: `mission_legacy_util.updateLegacyState(missionState)`

### MissionInterface.TaskVTable (record)

```teal
record TaskVTable<S, P>
```

Per-task lookup table through which the framework reaches a task's script, its static params and its callbacks. Built by `MissionBootUtil` from the story's `StaticTaskInfo`.

- `getGenericParams : function() : GenericTaskParams`: Returns the framework-level params of the task (filters, rules, medals, rewards, ...).
    - returns: Generic params, cached per task key.
- `getSpecificParams : function() : P`: Returns the task-type specific params (`StaticTaskInfo.taskParams`).
    - returns: The `Params` value of the task's script type.
- `onComplete : function(TaskContext<S, P>, MissionTaskApi)`: Called by the framework after a task completed; the boot util uses it to add the task's `followup` tasks.
    - param `#1`: Context of the completed task.
    - param `#2`: API to add further tasks.
- `getTaskInfo : function(TaskContext<S, P>) : TaskInfo`: Returns what the mission window shows for the task (missions only).
    - param `#1`: Context of the task.
    - returns: The static `taskInfo`, with progress copied in from the task's generic state.
- `getGuideInfo : function(TaskContext<S, P>) : GuideInfo`: Returns the hint shown for a guide task (guide system only).
    - param `#1`: Context of the task.
    - returns: The static `guideInfo` of the task.
- `getTaskScript : function() : TaskScript<S, P>`: Returns the script that implements the task.
    - returns: Task script (`StaticTaskInfo.taskScript`).
- `needTaskDeletion : function() : boolean`: Tells whether the task no longer exists in the story (its static data is `nil`); such saved tasks are removed on load.
    - returns: `true` if the task should be deleted.

### MissionInterface.TaskConfig (record)

```teal
record TaskConfig
```

Saved identification of a task. It names the function that builds its vtable, the argument for it, and the task's unique key.

Notes:

`MissionBootUtil` creates it as
`{ getVTableScriptFn = scriptKey .. "@mission.getVTable", getVTableScriptFnParams = taskKey, uniqueKey = MissionTaskUtil.makeTaskKey(taskKey, scriptKey) }`.

- `getVTableScriptFn : string`: Script function (`"<script>@<path.to.fn>"`) called with `getVTableScriptFnParams` and `uniqueKey` to obtain the task's `TaskVTable`.
- `getVTableScriptFnParams : any`: First argument for `getVTableScriptFn`; the boot util passes the task key from the story.
- `uniqueKey : string`: key created as bootstrapper"||"unique-key-inside-bootstrapper

### MissionInterface.TaskProgressData (record)

```teal
record TaskProgressData
```

Progress of a task (or sub-task) as shown in the mission window. Which fields are set decides how it is drawn.

Notes:

Drawing order in `MissionFrameworkReactUtil.makeTaskProgressItem`: `bool` (check mark or cross,
optionally next to a counter), then `townRatingKey` (rating icon; needs `cargoTypeRes`,
`counter` and `entity`), then `counter` (with cargo icon if `cargoTypeRes`, as
"text current/max" if `max` is set), then `percentage`, `text`, `dynamicText`.
`iconPath` adds an icon in front of the result.

- `iconPath : string`: Path of an icon shown in front of the progress.
- `cargoTypeRes : ResName`: Cargo type resource (e.g. `"::/cargos/fish/fish.cargo"`) whose icon is shown with the counter.
- `bool : boolean`: Done / not done flag, drawn as a check mark or a cross.
- `text : string`: Label of the progress (e.g. "Trees Removed"); a static `text` from the task info overrides the runtime one.
- `dynamicText : string`: Text computed at runtime, shown when no other progress form applies.
- `counter : Counter`: Count towards a target.
- `percentage : number`: range: 0.0 - 1.0
- `progressBar : boolean`
- `style : Style`
- `townRatingKey : TownRatingKey`: Town rating whose icon is shown (needs `entity`, `cargoTypeRes` and `counter`).
- `entity : Engine.Entity`: Town entity for `townRatingKey`.

#### MissionInterface.TaskProgressData.Counter (record)

```teal
record Counter
```

Integer progress counter.

- `current : integer`: Current count.
- `max : integer`: Target count; when `nil` only the current count is shown.

#### MissionInterface.TaskProgressData.Style (enum)

```teal
enum Style
```

- `"Good"`
- `"Neutral"`
- `"Bad"`

### MissionInterface.BootstrapDesc (record)

```teal
record BootstrapDesc
```

`data` of a `missiontask_boot`, `tutorial_boot` or `guide_system_boot` generic resource: the function that spawns the first tasks.

Example:
```lua
function data()
return {
    type = "guide_system_boot",
    data = {
        bootstrapScriptFn = "urbangames_campaign_mission_01::/guide_system/guide_system.script@guide_system.spawnBootTasks",
        bootstrapScriptFnParams = nil,
    }
}
end
```

- `bootstrapScriptFn : string`: Script function (`"<script>@<path.to.fn>"`) called once per savegame as `fn(bootstrapScriptFnParams, nil, missionTaskApi)`.
- `bootstrapScriptFnParams : string`: First argument passed to `bootstrapScriptFn`.

### MissionInterface.MissionSoundtrack (record)

```teal
record MissionSoundtrack
```

Tracklist that a task puts on the music player while it is open.

- `mode : AddMode`: the mode to use when adding the tracklist to the music player
default: "set"
- `enableRepeat : boolean`: when true, repeats the entire tracklist
default: "true"
- `tracks : {Track}`: the list of tracks to play

#### MissionInterface.MissionSoundtrack.Track (record)

```teal
record Track
```

describe a single track of the tracklist

One track of a tracklist.

- `name : string`: the name of the track, this is optional
- `file : string`: the path to the track file, this is required
- `enableRepeat : boolean`: when true, repeats a single track
default: "false"

#### MissionInterface.MissionSoundtrack.AddMode (enum)

```teal
enum AddMode
```

a mode that controls how a tracklist is added to the music player

- `"add"`: pushes the given tracklist on top of the current tracklist; when the tracklist ends, resumes the previous tracklist.
- `"set"`: replaces the current tracklist with the given tracklist.

### MissionInterface.HighlightStyle (enum)

```teal
enum HighlightStyle
```

Shape of the highlight drawn around a GUI element (`api.gui.HighlightStyle`).

- `"Circle"`: Circle highlight.
- `"Square"`: Rectangle highlight.
- `"Mask"`: Mask highlight.

### MissionInterface.GenericTaskParams (record)

```teal
record GenericTaskParams
```

Framework-level params of a task, applied while the task is open. Built from `MissionTaskUtil.StaticTaskInfo` by `MissionTaskUtil.extractGenericParams`.

- `isBonusTask : boolean`: Marks the task as a bonus mission (window title "Bonus Mission").
- `medals : {string : boolean}`: Medal ids (from `info.mission.lua`) noted as earned in the user profile while the task exists.
- `stars : {string : boolean}`: Star ids (from `info.mission.lua`) and their state, noted in the user profile.
- `visibilityRules : {string : boolean}`: GUI element class -> visible flag, applied with `api.gui.byId.setVisibilityByClass` while the task is open.
- `enableRules : {string : boolean}`: GUI element id/tag -> enabled flag while the task is open.
- `highlightRules : {string : HighlightStyle}`: GUI element id/tag -> highlight style while the task is open.
- `omitNotificationOnComplete : boolean`: Suppresses the "task complete" notification of a task shown in the history.
- `startSoundtrack : string`: Music track started with `api.gui.musicPlayer.playTrack` when the task opens.
- `soundtrack : MissionSoundtrack`: Tracklist put on the music player while the task is open.
- `playlistOverride : ResName`: Playlist resource that replaces the music playlist while the task is open.
- `disableGuideSystem : boolean`: Marks the task as one during which the guide system stays inactive (cutscene and dialogue tasks set it).
- `allowBuyVehicles : {string : integer}`: todo, also enable accounting for existing vehicles

Vehicle model -> maximum number the player may own. If any open task sets it, only listed models can be bought.
- `allowBuyVehicleGroups : {string : {{string}, integer}}`: id, -> modelnames, count

Group id -> `{ model names, count }`.
- `lockedVehicles : {Engine.Entity}`: entity ids that may not be sold

Vehicle entities that may not be sold.
- `notificationFilter : {string : boolean}`: Notification types (script paths) hidden while the task is open.
- `menuFilter : GameContext.Filters.MenuFilter`: Menu items enabled or disabled while the task is open.
- `vehicleFilter : GameContext.Filters.VehicleFilter`: Vehicles enabled or disabled in the vehicle store while the task is open.
- `selectorEntityFilter : {Engine.Entity : boolean}`: Entities that may (or may not) be selected in the world while the task is open.
- `disableFeatures : {DisableFeatures : boolean}`: GUI features switched off while the task is open (e.g. `Layers`, `HudIconMaster`).
- `spawnGuide : {string}`: Script events sent when the task is added; with an instruction voice-over they are delayed until it starts (at most 25 s).
- `requiredTaskToComplete : string`: taskkey <- mission_task_util.makeTaskKey

Unique key (`MissionTaskUtil.makeTaskKey`) of a task that must be complete before this one can complete.
- `allowSimultaneousUiTasks : {string}`: taskkey <- mission_task_util.makeTaskKey

Unique keys of tasks whose `onGuiKickOff` may run while this task's UI (cutscene, dialogue) is running.
- `financialReward : integer`: Money booked as income when the task completes, in game currency; also shows a notification.
- `showInHistory : boolean`: Keeps the task in the mission window after completion and sends a completion notification.
- `showInGuiWithoutName : boolean`: Shows the task in the mission window even without a name.
- `suppressAutoOpen : boolean`: Keeps the mission window from opening automatically for this task.

### MissionInterface.GenericTaskState (record)

```teal
record GenericTaskState
```

Framework-managed state of a task, saved with the mission.

- `uid : string`: Runtime id `"<taskScriptName>#<n>"`; also the event id under which the task receives its own events.
- `config : TaskConfig`: Saved identification of the task.
- `acceptedTime : number`
- `spawnTime : number`: Game time (ms) at which the task was added.
- `guiSpawnTime : number`: Mission GUI time (seconds) at which the task was added.
- `completedTime : number`: Game time at which the task completed; `nil` while it is open.
- `optionDecision : any`: decision in case getTaskInfo() offered options

Key of the option button the player clicked (`Decision` event). A value of `"finish"`, or a string starting with `finish`, completes the task.
- `progress : TaskProgressData`: Runtime progress shown in the mission window.
- `subTasksProgress : {TaskProgressData}`: Runtime progress of the sub-tasks, in the order of `TaskInfo.subTasks`.
- `instructionsQueue : {InstructionQueueEntry}`: Extra instructions queued by the task.

the task's own TaskInfo.instructionData, which is played directly by the gui and never enters instructionsQueue
- `taskInstructionState : InstructionQueueEntry.InstructionState`
- `guideQueue : {GuideQueueEntry}`: Pending `spawnGuide` events of the task.
- `guideTimeoutTime : number`: gui time at which a pending guideQueue fires even though the task's instruction never started

#### MissionInterface.GenericTaskState.InstructionQueueEntry (record)

```teal
record InstructionQueueEntry
```

An extra instruction (voice-over) queued by a task, with its playback state.

- `instructionKey : string`: Key passed to `TaskInfo.makeInstructionData` to get the instruction.
- `param : any`: Extra argument passed to `TaskInfo.makeInstructionData`.
- `state : InstructionState`: Playback state, updated from the GUI by the `updateDialogueState` event.

##### MissionInterface.GenericTaskState.InstructionQueueEntry.InstructionState (enum)

```teal
enum InstructionState
```

Playback state of an instruction.

- `"Pending"`: Not started yet.
- `"Playing"`: Playing.
- `"Complete"`: Finished.

#### MissionInterface.GenericTaskState.GuideQueueEntry (record)

```teal
record GuideQueueEntry
```

A `spawnGuide` event waiting for the task's instruction to start.

- `guideKey : string`: Name of the script event to send.
- `state : GuideState`: Whether the event was sent.

##### MissionInterface.GenericTaskState.GuideQueueEntry.GuideState (enum)

```teal
enum GuideState
```

Whether the guide event was sent.

- `"Pending"`: Not sent yet.
- `"Triggered"`: Sent.

### MissionInterface.TaskState (record)

```teal
record TaskState<TaskSpecificState, TaskSpecificParams>
```

Saved state of one task, with its script-specific state and the framework state.

- `type : string`
- `specific : TaskSpecificState`: Script-specific state, as returned by `TaskScript.onSpawn`.
- `genericState : GenericTaskState`: Framework-managed state.

### MissionInterface.MissionState (record)

```teal
record MissionState
```

Saved state of the mission game script (`::mission/mission.gs`).

- `tasks : {TaskState<any, any>}`: All tasks added so far, open and complete.
- `nextUid : integer`: Counter used for the next task `uid`.
- `version : integer`: Layout version of this state (see `MissionLegacyUtil`).
- `bootTypes : {string : boolean}`: Bootstrap functions (`bootstrapScriptFn`) that already ran.
- `nonBulldozableEntities : {Engine.Entity : boolean}`: Entities currently made non-bulldozable by open tasks.
- `notificationFilter : {string : boolean}`: Notification types currently hidden by open tasks.
- `needPauseNotifications : boolean`: Set on `initMission`; notifications are paused for one update.
- `notificationsArePaused : boolean`: Notifications are currently paused by the mission.
- `guiTimeSeconds : number`: Mission GUI clock in seconds, advanced by `dt / speedup` (wall-clock like, independent of game speed).
- `isComplete : boolean`: Set when the mission was completed or aborted; the state is then cleared.
- `lastCompletedTime : number`: GUI time (seconds) at which the last task completed.
- `isTutorialActive : boolean`: The tutorial was started (`initNewGame` / `initTutorial` with a `tutorial_boot` present).

### MissionInterface.MissionTaskApi (record)

```teal
record MissionTaskApi
```

API passed to engine-side task callbacks (`onComplete`, `handleEvent`) and to bootstrap functions.

- `addTask : function(taskConfig : TaskConfig)`: Spawns a task. Calls the script's `onSpawn` (a `nil` result prevents spawning) and then `onStart`.
    - param `taskConfig`: Identification of the task to add.
- `sendTaskEvent : function(eventName : string, params : any)`: Sends a script event to the calling task itself (id = the task's `uid`); it arrives in its `handleEvent`.
    - param `eventName`: Event name.
    - param `params`: Event parameter.
- `getTaskState : function(uniqueTaskKey : string) : TaskState<any, any>`: Looks up a task by its unique key.
    - param `uniqueTaskKey`: Key from `MissionTaskUtil.makeTaskKey`.
    - returns: The task's state, or `nil` if it was not added (yet).

### MissionInterface.MissionTaskReadOnlyApi (record)

```teal
record MissionTaskReadOnlyApi
```

Read-only API passed to `TaskScript.onUpdate`.

- `getTaskState : function(uniqueTaskKey : string) : TaskState<any, any>`: Looks up a task by its unique key.
    - param `uniqueTaskKey`: Key from `MissionTaskUtil.makeTaskKey`.
    - returns: The task's state, or `nil`.
- `getLastCompletedTime : function() : number`: Returns when the last task completed.
    - returns: GUI time in seconds (`MissionState.lastCompletedTime`).
- `hasPendingGuide : function() : boolean`: Tells whether an open guide task has been triggered and is waiting to be shown or dismissed.
    - returns: `true` if a triggered guide is open.

### MissionInterface.DeferredTaskApi (record)

```teal
record DeferredTaskApi
```

API passed to GUI-side task callbacks; changes go through commands to the engine side.

- `sendTaskEvent : function(eventName : string, params : any)`: Sends a script event to the calling task (id = its `uid`) via a command; it arrives in its engine-side `handleEvent`.
    - param `eventName`: Event name.
    - param `params`: Event parameter.
- `getTaskState : function(uniqueTaskKey : string) : TaskState<any, any>`: Looks up a task by its unique key.
    - param `uniqueTaskKey`: Key from `MissionTaskUtil.makeTaskKey`.
    - returns: The task's state, or `nil`.

### MissionInterface.TaskDesc (record)

```teal
record TaskDesc
```

- `scriptFile : string`

### MissionInterface.PosRadius (record)

```teal
record PosRadius
```

Circular area with a centre `{x, y}` (or `{x, y, z}`) and a radius in metres.

- `pos : {number}`: Centre position in world coordinates.
- `radius : number`: Radius in metres.

### MissionInterface.Marker (record)

```teal
record Marker
```

Marker placed in the 3D world by a task (`api.gui.mission.setMarkerAtEntity` / `setMarkerAtPosition`).

- `key : string`: Unique key of the marker; it is removed when no open task returns it any more.
- `position : {number, number, number}`: World position, used when `entity` is not set.
- `entity : Engine.Entity`: Entity to attach the marker to.
- `type : string`: Marker type (e.g. `"arrow"`).
- `scaling : number`: Size factor of the marker.
- `isSelectable : boolean`: The marker can be clicked.
- `fireSelectEntity : boolean`: Clicking the marker fires a select-entity event.

### MissionInterface.Zone (record)

```teal
record Zone
```

Area drawn on the terrain by a task, as a polygon or a circle (`api.gui.mission.setZone` / `setZoneCircle`).

- `key : string`: Unique key of the zone; it is removed when no open task returns it any more.
- `polygon : PolygonUtil.Polygon`: Polygon outline; takes precedence over `circle`.
- `circle : MissionInterface.PosRadius`: Circular outline.
- `draw : boolean`: Draws the zone.
- `getDrawColor : function() : Vec4f`: Returns the zone colour, typically a colour from `MissionColors`.
- `playAnimation : boolean`: Animates the zone outline.
- `prohibitBuilding : boolean`: Forbids building inside the zone.
- `zOffset : number`: Vertical offset of the drawn zone.

### MissionInterface.GuideInfo (record)

```teal
record GuideInfo
```

Hint shown by the guide system for a guide task.

- `name : string`: Title of the hint.
- `text : string`: Hint text.
- `textGamepad : string`: Hint text used in gamepad input mode.
- `image : string`: Image shown in the hint.
- `imageGamepad : string`: Image used in gamepad input mode.
- `caption : string`: Caption passed to the guide spawn call.
- `targets : {Target}`: GUI elements the hint points at.
- `free : boolean`: Shows the hint at a free screen position instead of at a target.
- `freePosition : FreePosition`: Screen position of a free hint.

#### MissionInterface.GuideInfo.Position (enum)

```teal
enum Position
```

Side of the target GUI element where the hint appears.

- `"Above"`: Above the target.
- `"Below"`: Below the target.
- `"Right"`: Right of the target.
- `"Left"`: Left of the target.

#### MissionInterface.GuideInfo.FreePosition (enum)

```teal
enum FreePosition
```

Screen position of a free (untargeted) hint.

- `"High"`: Upper position.
- `"Low"`: Lower position (default).

#### MissionInterface.GuideInfo.Target (record)

```teal
record Target
```

GUI element the hint points at.

- `idOrTag : string`: Id or tag of the GUI element (e.g. `"menu.construction.road"`).
- `position : Position`: Side of the element where the hint appears.

### MissionInterface.TaskInfo (record)

```teal
record TaskInfo
```

What the mission window shows for a task (name, paragraphs, option buttons, progress, links, instruction voice-over).

Notes:

A task without a `name` (and without `options` or `showTaskInGuiWithoutName`) is not listed in the mission window.
Paragraph texts may contain links to keys of `links`; clicking one moves the camera there.

Example:
```lua
-- from urbangames_campaign_mission_01 mission_story.tl
taskInfo = {
    name = _("MISSION_01_TASK_BULLDOZE_NAME"),
    paragraphs = {
        { text = _("MISSION_01_TASK_BULLDOZE_PARAGRAPH") },
        { text = _("MISSION_01_TASK_BULLDOZE_HINT") },
    },
    links = { pos_street = { location = mission_task_util.mergeAreas({params.connectStreets_1_1_zone, params.connectStreets_1_2_zone }, 20) } },
    instructionData = {
        subtitle = _("MISSION_01_TASK_BULLDOZE_INSTRUCTION"),
        portrait = "urbangames_campaign_mission_01::/mission/dialogue/andrew_neutral.tga",
        voiceOver = "urbangames_campaign_mission_01::/audio/voice_over/MISSION_01_TASK_BULLDOZE_INSTRUCTION.wav",
        duration = 6,
    },
    progress = { text = _("Trees Removed") },
},
```

- `links : {string : LinkDef}`: Link key -> camera target. Keys are referenced from paragraph texts; the locate button frames all of them.
- `name : string`: Task title. Tasks without a name are hidden from the mission window.
- `paragraphs : {Paragraph}`: Task description; the first paragraph is shown next to the instruction button. Later paragraphs are hints (wiki).
- `options : {Option}`: Option buttons.
- `camera : {LinkDef}`: Camera targets used by the locate button instead of `links`.
- `progress : TaskProgressData`: Static progress fields (e.g. `text`, `cargoTypeRes`) merged with the runtime progress.
- `subTasks : {SubTaskInfo}`: Sub-tasks with their own progress.
- `instructionData : MissionDialogueData.InstructionData`: Voice-over instruction of the task (portrait, subtitle, sound file, duration).
- `makeInstructionData : function(instructionKey : string, param? : any) : MissionDialogueData.InstructionData`: Returns the instruction for a key queued in `GenericTaskState.instructionsQueue`.
    - param `instructionKey`: Key of the queued instruction.
    - param `param`: Extra parameter of the queue entry.
    - returns: Instruction to play.
- `showTaskInGuiWithoutName : boolean`: Lists the task in the mission window even without a name.

#### MissionInterface.TaskInfo.NotificationInput (record)

```teal
record NotificationInput
```

Params of the mission notification (`::/mission/notification.script`) sent when a task completes.

In game: "Task Completed", "Congratulations! You completed the task '{NAME}' and you received a reward of {REWARD} for completing it." (German: „Aufgabe abgeschlossen“, „Glückwunsch! Du hast die Aufgabe '{NAME}' abgeschlossen und dafür eine Belohnung in Höhe von {REWARD} erhalten.“)

- `missionTaskKey : string`: Unique key of the completed task.
- `financialReward : integer`: Reward paid, in game currency, if any.

#### MissionInterface.TaskInfo.Paragraph (record)

```teal
record Paragraph
```

One paragraph of the task description.

- `text : string`: Paragraph text, used for all input modes.
- `textMouse : string`: Text used with mouse input when `text` is not set.
- `textGamepad : string`: Text used with gamepad input when `text` is not set.

#### MissionInterface.TaskInfo.Option (record)

```teal
record Option
```

Button in the task details; clicking it sends a `Decision` event with `key`.

- `text : string`: Button label.
- `key : any`: Value stored as `GenericTaskState.optionDecision`; `"finish"` (or a string starting with `finish`) completes the task.

#### MissionInterface.TaskInfo.LinkDef (record)

```teal
record LinkDef
```

Camera target of a link. It is an entity or an area, given directly or by a function.

- `entity : Engine.Entity`: Entity the camera focuses on.
- `location : PosRadius`: Area the camera moves to.
- `getEntity : function() : Engine.Entity`: Returns the entity at click time; takes precedence over `entity`.
- `getLocation : function() : MissionInterface.PosRadius`: Returns the area at click time; takes precedence over `location`.

#### MissionInterface.TaskInfo.SubTaskInfo (record)

```teal
record SubTaskInfo
```

A sub-task listed under the task with its own progress.

- `name : string`: Sub-task label.
- `description : string`: Sub-task description.
- `progress : TaskProgressData`: Sub-task progress (filled from `GenericTaskState.subTasksProgress`).

### MissionInterface.ProposalHandlerResult (record)

```teal
record ProposalHandlerResult
```

Answer of a task to a build proposal (`TaskScript.guiHandleProposal`).

- `error : string`: Error message; blocks the proposal.
- `warning : string`: Warning message; the proposal stays possible.
- `priority : number`: When several tasks answer, the message with the highest priority is shown.
- `isWhitelist : boolean`: A whitelist result without message lets the proposal pass this handler.

### MissionInterface.TaskContext (record)

```teal
record TaskContext<TaskSpecificState, TaskSepcificParams>
```

Everything a task callback gets about its task.

- `state : TaskSpecificState`: Script-specific state; may be modified in place (or replaced, the framework commits it).
- `genericState : GenericTaskState`: Framework-managed state.
- `params : TaskSepcificParams`: Task-specific params from the story (read-only).
- `guiTimeSeconds : number`: Current mission GUI time in seconds.

### MissionInterface.TaskScript (interface)

```teal
interface TaskScript<TaskSpecificState, TaskParams>
```

Implementation of a task type. All callbacks are optional; the built-in task types under `mission/tasks/**` implement this interface.

Notes:

Engine side (state can be changed): `onSpawn`, `onStart`, `onUpdate`, `isComplete`, `onComplete`,
`handleEvent`. GUI side (state is read-only; use `DeferredTaskApi.sendTaskEvent` to change it):
`guiUpdate`, `guiHandleEvent`, `guiHandleProposal`, `onGuiKickOff`, `getMarkers`, `getZones`,
`getProtectedEntities`, `getSelectorEntityFilter`, `getVisibilityRules`, `getEnableRules`,
`getHighlightRules`. `getNonBulldozableEntities` runs on the engine side for open tasks.

Example:
```lua
-- base game: mission/tasks/wait/wait.tl (simplified)
local ret : MissionTaskWait = {
    taskScriptName = "wait",
    onSpawn = function(params : MissionTaskWait.Params, guiTimeSeconds : number) : MissionTaskWait.State
        return { endGuiTimeSeconds = params.guiTimeSeconds and guiTimeSeconds + params.guiTimeSeconds }
    end,
    isComplete = function(ctx : MissionInterface.TaskContext<MissionTaskWait.State, MissionTaskWait.Params>) : boolean
        return ctx.guiTimeSeconds >= ctx.state.endGuiTimeSeconds
    end,
}
return ret
```

- `taskScriptName : string`: Type name of the task; prefix of task `uid`s. The name `"finish"` marks the mission-end task.
- `disableGuideSystem : boolean`: Marks the task type as one during which the guide system stays inactive (set by the cutscene and dialogue scripts).
- `handleLegacy : function(ctx : TaskContext<TaskSpecificState, TaskParams>, oldVersion : integer) : TaskContext<TaskSpecificState, TaskParams>`: apply legacy handling for the task (use return value in case state is not a table and thus cannot be updated inplace)

Upgrades the task's saved state from an older mission-state version.
    - param `ctx`: Context with the old state.
    - param `oldVersion`: Version of the saved mission state.
    - returns: A replacement context, or `nil` if the state was updated in place.

prepare the initial state when attempting to spawn (return nil to prevent spawning)
- `onSpawn : function(TaskParams, guiTimeSeconds : number) : TaskSpecificState | nil`: Creates the initial script-specific state when the task is added.
    - param `#1`: Task params from the story.
    - param `guiTimeSeconds`: Current mission GUI time in seconds.
    - returns: Initial state (an empty table is used when the callback is missing).
- `onStart : function(state : TaskContext<TaskSpecificState, TaskParams>)`: Called once right after the task was added.
    - param `state`: Context of the new task.
- `getInfo : function(state : TaskContext<TaskSpecificState, TaskParams>) : TaskInfo`: Returns task info computed by the script. A task built with `TaskDecorator.decorate` uses the wrapped task's `getInfo` and ignores the decorator's.
    - param `state`: Context of the task.
    - returns: Task info.
- `isComplete : function(state : TaskContext<TaskSpecificState, TaskParams>) : boolean`: Checked every update for open tasks; returning `true` completes the task (unless `requiredTaskToComplete` is still open).
    - param `state`: Context of the task.
    - returns: `true` when the task is done.
- `onComplete : function(state : TaskContext<TaskSpecificState, TaskParams>, missionTaskApi : MissionTaskApi)`: Called once when the task completes, before its follow-up tasks are added.
    - param `state`: Context of the task.
    - param `missionTaskApi`: API to add tasks or look up other tasks.
- `onUpdate : function(ctx : TaskContext<TaskSpecificState, TaskParams>, readOnlyApi : MissionTaskReadOnlyApi)`: Called every engine update while the task is open.
    - param `ctx`: Context of the task; state changes are saved.
    - param `readOnlyApi`: Read-only API to look up other tasks.
- `handleEvent : function(src : string, id : string, name : string, param : any, ctx : TaskContext<TaskSpecificState, TaskParams>, missionTaskApi : MissionTaskApi) : any`: Receives every script event on the engine side while the task is open. Events sent to the task itself have `id` equal to its `uid`.
    - param `src`: Event source.
    - param `id`: Event id (target).
    - param `name`: Event name.
    - param `param`: Event parameter.
    - param `ctx`: Context of the task; state changes are saved.
    - param `missionTaskApi`: API to add tasks, send events to itself or look up tasks.
    - returns: Optional result of the event.
- `guiHandleEvent : function(src : string, id : string, name : string, param : any, ctx : TaskContext<TaskSpecificState, TaskParams>, taskApi : DeferredTaskApi) : any`: Receives GUI events while the task is open.
    - param `src`: Event source.
    - param `id`: Event id.
    - param `name`: Event name.
    - param `param`: Event parameter.
    - param `ctx`: Context of the task (read-only on this side).
    - param `taskApi`: Deferred API to send events to the engine side.
    - returns: Optional result of the event.
- `guiUpdate : function(ctx : TaskContext<TaskSpecificState, TaskParams>, taskApi : DeferredTaskApi)`: Called every GUI update while the task is open.
    - param `ctx`: Context of the task (read-only on this side).
    - param `taskApi`: Deferred API to send events to the engine side.
- `guiHandleProposal : function(id : string, name : string, param : ProposalEventData, isApply : boolean, ctx : TaskContext<TaskSpecificState, TaskParams>, taskApi : DeferredTaskApi) : ProposalHandlerResult`: Checks a build proposal (`builder.proposalCreate` / `builder.proposalApply`) and can block it or warn.
    - param `id`: Event id.
    - param `name`: Event name (`builder.proposalCreate` or `builder.proposalApply`).
    - param `param`: Proposal, its data and, on apply, the resulting entities.
    - param `isApply`: `true` for `builder.proposalApply`.
    - param `ctx`: Context of the task.
    - param `taskApi`: Deferred API to send events to the engine side.
    - returns: Error or warning to show, or `nil` to accept.

Opportunity to schedule UI things (i.e. cutscene/dialogue) on task "start", for which other tasks should wait.
If you do so, return true to indicate no other onGuiKickOff shall be called in this frame.
- `onGuiKickOff : function(ctx : TaskContext<TaskSpecificState, TaskParams>, taskApi : DeferredTaskApi) : boolean`: Lets the task start UI (cutscene, dialogue) when it opens; returning `true` stops other tasks from kicking off their UI in the same frame.
    - param `ctx`: Context of the task.
    - param `taskApi`: Deferred API.
    - returns: `true` if the task started UI.
- `getMarkers : function(ctx : TaskContext<TaskSpecificState, TaskParams>) : {Marker}`: Returns the world markers of the task.
    - param `ctx`: Context of the task.
    - returns: Markers to show while the task is open.
- `getZones : function(ctx : TaskContext<TaskSpecificState, TaskParams>) : {Zone}`: Returns the terrain zones of the task.
    - param `ctx`: Context of the task.
    - returns: Zones to draw while the task is open.
- `getNonBulldozableEntities : function(ctx : TaskContext<TaskSpecificState, TaskParams>) : {Engine.Entity}`: Returns entities that cannot be bulldozed while the task is open.
    - param `ctx`: Context of the task.
    - returns: Entities (constructions are resolved from their parts).
- `getProtectedEntities : function(ctx : TaskContext<TaskSpecificState, TaskParams>) : {Engine.Entity : GameContext.Filters.ProtectionConfig | boolean}`: Returns entities protected from player changes while the task is open.
    - param `ctx`: Context of the task.
    - returns: Entity -> protection config, or `true`.
- `getSelectorEntityFilter : function(ctx : TaskContext<TaskSpecificState, TaskParams>) : {Engine.Entity : boolean}`: Returns which entities may be selected while the task is open.
    - param `ctx`: Context of the task.
    - returns: Entity -> selectable flag.
- `getVisibilityRules : function(TaskContext<TaskSpecificState, TaskParams>) : {string : boolean}`: Returns GUI visibility rules computed at runtime; merged over the static `visibilityRules`.
    - param `#1`: Context of the task.
    - returns: GUI class -> visible flag.
- `getEnableRules : function(TaskContext<TaskSpecificState, TaskParams>) : {string : boolean}`: Returns GUI enable rules computed at runtime; merged over the static `enableRules`.
    - param `#1`: Context of the task.
    - returns: GUI id/tag -> enabled flag.
- `getHighlightRules : function(TaskContext<TaskSpecificState, TaskParams>) : {string : MissionInterface.HighlightStyle}`: Returns GUI highlight rules computed at runtime; merged over the static `highlightRules`.
    - param `#1`: Context of the task.
    - returns: GUI id/tag -> highlight style.
