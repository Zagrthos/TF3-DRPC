# mission.tasks.auto_builder.prepare_track_building_util

Definition file: `base/tealdef/mission/tasks/auto_builder/prepare_track_building_util.d.tl` (in the game folder).

Development helper that writes track segments to a file for the track-builder task.

## PrepareTrackBuildingUtil (record)

```teal
global record PrepareTrackBuildingUtil
```
Load: `local prepare_track_building_util = ug_require "::/mission/tasks/auto_builder/prepare_track_building_util.tl" as PrepareTrackBuildingUtil`

Development helper for the track-builder task.

- `writeSegments : function(baseEdges : {Engine.Entity}, path : string)`  (call as `prepare_track_building_util.writeSegments(…)`): Writes the geometry of existing edges as `MissionTaskTrackBuilder.Segment` list to a file.
    - param `baseEdges`: Edges to export.
    - param `path`: File to write.
