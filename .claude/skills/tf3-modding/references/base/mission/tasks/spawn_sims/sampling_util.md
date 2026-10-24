# mission.tasks.spawn_sims.sampling_util

Definition file: `base/tealdef/mission/tasks/spawn_sims/sampling_util.d.tl` (in the game folder).

Poisson-disk sampling of points in a rectangle.

## SamplingUtil (record)

```teal
global record SamplingUtil
```
Load: `local sampling_util = ug_require "::/mission/tasks/spawn_sims/sampling_util.tl" as SamplingUtil`

Point sampling helper.

- `poissonDiskSampling : function(width : number, height : number, radius : number, sampleLimit? : integer, seed? : integer) : {Vec2f}`  (call as `sampling_util.poissonDiskSampling(…)`): Returns random points in a rectangle that are at least `radius` apart (Poisson-disk sampling).
    - param `width`: Width of the rectangle.
    - param `height`: Height of the rectangle.
    - param `radius`: Minimum distance between points.
    - param `sampleLimit`: Attempts per point (default 30).
    - param `seed`: Random seed.
    - returns: Points with `0 <= x < width`, `0 <= y < height`.
    - used 1x in the base game, e.g. `base/content/mission/mission/tasks/spawn_sims/spawn_sims.tl:50`: `local samplePoints = sampling_util.poissonDiskSampling(len1, len2, minDistanceToNeighbor, 30, seed)`
