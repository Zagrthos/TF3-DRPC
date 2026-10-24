# mission.mission_framework.mission_framework_react

Definition file: `base/tealdef/mission/mission_framework/mission_framework_react.d.tl` (in the game folder).

GUI of the mission window (task list and task details).

## MissionFrameworkReact (record)

```teal
global record MissionFrameworkReact
```
Load: `local mission_framework_react = ug_require "::/mission/mission_framework/mission_framework_react.tl" as MissionFrameworkReact`

GUI of the mission window.

- `MissionTaskList : Recipe<GameContext>`: Recipe of the list of open tasks.
  In game: "Show Mission Tasks", "End Tutorial", "Are you sure that you want to end the tutorial?" (German: „Missionsaufgaben anzeigen“, „Tutorial beenden“, „Sicher, dass du das Tutorial beenden willst?“)
    - used 1x in the base game, e.g. `base/content/gui/gui/main/game.tl:519`: `item = mission_framework_react.MissionTaskList(react.ref(missionTaskListRef), gameCtx),`
- `makeMissionWindowContent : function(tagPrefix : string, nonEntity : string, gameCtx : GameContext, isMapEditor : boolean, isSandboxMode : boolean, setActionFnAndProxy : {function(actionFn? : (function() : TreeNodeId), key2? : string), any}, closeFnAndProxy : {function(), any}, extraParam : any) : ViewManager.MakeEntityWindowResult`  (call as `mission_framework_react.makeMissionWindowContent(…)`): Builds the content of the mission window (task list and task details).
  In game: "Mission", "Tasks", "Special Award" (German: „Mission“, „Aufgaben“, „Besondere Auszeichnung“)
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/make_non_entity_window.tl:19`: `return mission_framework_react.makeMissionWindowContent(tagPrefix, nonEntity, gameCtx, isMapEditor, isSandboxMode, setActionFnAndProxy, closeFnAndProxy, extraPa`
