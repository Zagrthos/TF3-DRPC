# mission.tasks.auto_builder.mission_task_track_builder

Definition file: `base/tealdef/mission/tasks/auto_builder/mission_task_track_builder.d.tl` (in the game folder).

Task that builds a prepared track segment by segment.

## MissionTaskTrackBuilder (record)

```teal
global record MissionTaskTrackBuilder is MissionInterface.TaskScript<MissionTaskTrackBuilder.State, MissionTaskTrackBuilder.Params>
```
Load: `local track_builder = ug_require "::/mission/tasks/auto_builder/track_builder.tl" as MissionTaskTrackBuilder`

Task (`track_builder`) that builds a list of track segments (`simple_catenary` template) one after the other up to a target count, then completes.

Notes:

The target count is all segments, or the progress of `ownerTask` (a `transport_cargo` task) times the
number of segments, or (with `onEvent`) the value of a `mission` / `track_builder` / `target-segment`
script event.

### MissionTaskTrackBuilder.Segment (record)

```teal
record Segment
```

One track segment.

- `position0 : {number, number, number}`: Start position.
- `position1 : {number, number, number}`: End position.
- `tangent0 : {number, number, number}`: Tangent at the start.
- `tangent1 : {number, number, number}`: Tangent at the end.

### MissionTaskTrackBuilder.Params (record)

```teal
record Params
```

Params of the task.

- `roadTemplate : ResName`
- `segments : {Segment}`: Segments to build, in order.
- `firstNode : Engine.Entity`: Existing node the first segment starts at.
- `lastNode : Engine.Entity`: Existing node the last segment ends at.
- `ownerTask : ResName`: Unique key of a `transport_cargo` task whose progress sets how many segments are built.
- `onEvent : boolean`: The target count comes from `target-segment` events (starts at 0).
- `inverted : boolean`: Builds from the last segment backwards, starting at `lastNode`.
- `tunnel : boolean`: Builds the segments as tunnel.

### MissionTaskTrackBuilder.State (record)

```teal
record State
```

State of the task.

- `isComplete : boolean`: All target segments are built.
- `previousNodes : {Engine.Entity}`: Nodes built so far; the last one is the start of the next segment.
- `builtSegmentCount : integer`: Segments built.
- `targetSegmentCount : integer`: Segments to build.
