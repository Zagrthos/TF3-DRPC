# mission.guide_system.guides.guide_event

Definition file: `base/tealdef/mission/guide_system/guides/guide_event.d.tl` (in the game folder).

Guide task triggered by a matching script event.

## GuideEvent (record)

```teal
global record GuideEvent is MissionInterface.TaskScript<GuideEvent.State, GuideEvent.Params>
```
Load: `local guide_event = ug_require "::/mission/guide_system/guides/guide_event.tl" as GuideEvent`

Guide task triggered by an engine-side script event that matches all given fields.

### GuideEvent.Params (record)

```teal
record Params
```

Event to wait for; `nil` fields match anything, `param` is compared deeply.

- `src : string`: Event source.
- `id : string`: Event id.
- `name : string`: Event name.
- `param : any`: Event parameter.

### GuideEvent.State (record)

```teal
record State is IGuideState
```

Guide state.
