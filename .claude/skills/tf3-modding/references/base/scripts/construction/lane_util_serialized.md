# scripts.construction.lane_util_serialized

Definition file: `base/tealdef/scripts/construction/lane_util_serialized.d.tl` (in the game folder).

Types of the lane helper (`/scripts/construction/laneutil.lua`, `LaneUtilSerialized`) that turns
generated curve data into lane lists.

## LaneUtilSerialized (record)

```teal
global record LaneUtilSerialized
```

Lane helper from `/scripts/construction/laneutil.lua`.

- `createLane : function(curve : {{{number}}}, transportModes : {ConstructionUtilSerialized.TransportMode}, number, number, boolean) : ConstructionUtilSerialized.LaneList`: Builds a lane list from curves (each a list of points) for the given transport modes.
    - param `curve`: List of curves, each a list of `{x, y, z}` points: 2 points give a straight lane, 3 a quadratic
curve, and 4 (or 3n+1) points cubic segments whose tangents come from the control points.

    - param `transportModes`: Transport modes allowed on the lane, e.g. `{"PERSON"}`.
    - param `#3`: Speed limit of the lane (left unset if nil).
    - param `#4`: Width written as the third element of every node (0 if nil).
    - param `#5`: Whether the lane can be linked to other lanes (`linkable`).
    - returns: A lane list with `nodes`, `transportModes` and the optional `speedLimit` / `linkable`.

    ```lua
    -- from base/content/stations/street/street/underground_station/underground_station.script.lua
    local lane = laneutil.createLane(generatedData["base"].curves["passenger_entrance_link"], {"PERSON",}, 2, 2, true)
    ```

