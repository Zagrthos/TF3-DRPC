# mission.guide_system.guides.guide_ui_event

Definition file: `base/tealdef/mission/guide_system/guides/guide_ui_event.d.tl` (in the game folder).

Guide task triggered by a matching GUI event.

## GuideUiEvent (record)

```teal
global record GuideUiEvent is MissionInterface.TaskScript<GuideUiEvent.State, GuideUiEvent.Params>
```

Guide task triggered by a GUI event that matches all given fields.

### GuideUiEvent.Params (record)

```teal
record Params
```

GUI event to wait for; `nil` fields match anything, `param` is compared deeply.

- `src : string`: Event source.
- `id : string`: Event id.
- `name : string`: Event name.
- `param : any`: Event parameter.

### GuideUiEvent.State (record)

```teal
record State is IGuideState
```

Guide state.
