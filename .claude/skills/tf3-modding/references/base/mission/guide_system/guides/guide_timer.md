# mission.guide_system.guides.guide_timer

Definition file: `base/tealdef/mission/guide_system/guides/guide_timer.d.tl` (in the game folder).

Guide task triggered after a delay that depends on its importance.

## GuideTimer (record)

```teal
global record GuideTimer is MissionInterface.TaskScript<GuideTimer.State, GuideTimer.Params>
```
Load: `local guide_timer = ug_require "::/mission/guide_system/guides/guide_timer.tl" as GuideTimer`

Guide task triggered after a delay given by its importance, when no other guide is pending and the last task completed at least a minute ago. Completes early when a listed construction is proposed or `shouldSkipGuide` returns true.

### GuideTimer.Importance (enum)

```teal
enum Importance
```

Delay class of a timer guide.

- `"Now"`: No delay.
- `"VeryHigh"`: 1 minute after spawn.
- `"High"`: 5 minutes after spawn.
- `"Medium"`: 15 minutes after spawn.
- `"Low"`: 1.5 hours after spawn.

### GuideTimer.Params (record)

```teal
record Params
```

Params of a timer guide.

- `importance : Importance`: Delay class (GUI time).
- `constructionFileCandidates : {ResName : boolean}`: Construction or model files; proposing one of them completes the guide (the player already knows how).
- `shouldSkipGuide : function(state : MissionInterface.TaskContext<State, Params>) : boolean`: Completes the guide without showing it when it returns true.
    - param `state`: Context of the guide task.
    - returns: `true` to skip the guide.

### GuideTimer.State (record)

```teal
record State is IGuideState
```

Guide state.
