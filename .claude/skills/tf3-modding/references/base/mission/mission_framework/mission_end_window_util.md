# mission.mission_framework.mission_end_window_util

Definition file: `base/tealdef/mission/mission_framework/mission_end_window_util.d.tl` (in the game folder).

GUI content of the window shown at the end of a mission (stars, medals, success).

## MissionEndWindowUtil (record)

```teal
global record MissionEndWindowUtil
```
Load: `local mission_end_window_util = ug_require "::/mission/mission_framework/mission_end_window_util.tl" as MissionEndWindowUtil`

GUI of the mission-end window.

- `MissionEndWindowContent : Recipe<MissionEndWindowContentParam>`: Recipe of the mission-end window content.
  In game: "Congratulations, you have successfully completed the mission "{mission}"!", "The mission "{mission}" was unsuccessful.", "Try Again!" (German: „Glückwunsch, du hast die Mission "{mission}" erfolgreich abgeschlossen!“, „Die Mission "{mission}" ist fehlgeschlagen.“, „Versuche es nochmals!“)
    - used 1x in the base game, e.g. `base/content/mission/mission/mission_framework/mission_framework_react.tl:827`: `mission_end_window_util.MissionEndWindowContent{`

### MissionEndWindowUtil.MissionEndWindowContentParam (record)

```teal
record MissionEndWindowContentParam
```

Params of the mission-end window content.

- `stars : {string : boolean}`: Star ids and whether they were earned.
- `medals : {string : boolean}`: Medal ids and whether they were earned.
- `success : boolean`: The mission was won.
- `isTutorial : boolean`: The finished mission is the tutorial.
- `onClose : function(?boolean)`: Closes the window.
