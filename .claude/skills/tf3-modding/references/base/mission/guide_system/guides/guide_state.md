# mission.guide_system.guides.guide_state

Definition file: `base/tealdef/mission/guide_system/guides/guide_state.d.tl` (in the game folder).

State interface shared by all guide task scripts.

## IGuideState (interface)

```teal
global interface IGuideState
```

State shared by all guide task scripts. A guide is shown once `triggeredTime` is set and completes when the GUI sends the `guide-system` / `guide-complete` event for it.

- `triggeredTime : number`: GUI time (seconds) at which the guide was triggered; `nil` while waiting.
- `complete : boolean`: The guide was dismissed.
