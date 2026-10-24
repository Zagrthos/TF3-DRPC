# scripts.construction.street_util_serialized

Definition file: `base/tealdef/scripts/construction/street_util_serialized.d.tl` (in the game folder).

Types of `/scripts/construction/street_util_serialized.tl` (`StreetUtilSerialized`): helpers that append
street/track edges (position, tangent, optional tag) to an edge list.

## StreetUtilSerialized (record)

```teal
global record StreetUtilSerialized
```
Load: `local street_util_serialized = require "::/scripts/construction/street_util_serialized.tl" as StreetUtilSerialized`

Street/track edge helpers returned by `ug_require "/scripts/construction/street_util_serialized.tl"`.

- `Edge : {Vec3Serialized.Table3, Vec3Serialized.Table3, string}`: One node of an edge list: `{position, tangent, tag}`. Two consecutive entries form one edge
(start and end node).
    - used 5x in the base game, e.g. `base/content/scripts/scripts/construction/street_util_serialized.tl:18`: `street_util_serialized.addStraightEdge = function(edges : {StreetUtilSerialized.Edge},`
- `calcScale : function(dist : number, angle : number) : number`  (call as `street_util_serialized.calcScale(…)`): Calculate the scale factor for a street segment based on distance and angle.
    - param `dist`: distance between endpoints
    - param `angle`: angle between tangents in radians
    - returns: scale factor
    - used 2x in the base game, e.g. `base/content/infrastructure/street_construction/street_construction/street_constructions.script.lua:41`: `local scale = street_util_serialized.calcScale(length, angle) * 0.8`
- `addStraightEdge : function(edges : {Edge}, p0 : Vec3Serialized.Vec3, p1 : Vec3Serialized.Vec3, tag0? : string, tag1? : string)`  (call as `street_util_serialized.addStraightEdge(…)`): Add a straight edge between two points.
    - param `edges`: edge list to append to
    - param `p0`: start position
    - param `p1`: end position
    - used 29x in the base game, e.g. `base/content/infrastructure/street_construction/street_construction/street_constructions.script.lua:119`: `street_util_serialized.addStraightEdge(ledges, vec3.new(x[1],  y[4], 0), vec3.new(x[2], y[4], 0), nil, "we-west2")`
- `addRamp : function(edges : {Edge}, p0 : Vec3Serialized.Vec3, p1 : Vec3Serialized.Vec3, tag0? : string, tag1? : string)`  (call as `street_util_serialized.addRamp(…)`): Add a ramp edge with slope 0 at both endpoints.
    - param `edges`: edge list to append to
    - param `p0`: start position
    - param `p1`: end position
    - used 14x in the base game, e.g. `base/content/infrastructure/street_construction/street_construction/street_constructions.script.lua:120`: `street_util_serialized.addRamp(ledges, vec3.new(x[2],  y[4], 0), vec3.new(x[3],  y[4], z[1]), "we-west2", "we-west")`
- `addEdge : function(edges : {Edge}, p0 : Vec3Serialized.Vec3, p1 : Vec3Serialized.Vec3, t0 : Vec3Serialized.Vec3, t1 : Vec3Serialized.Vec3, tag0? : string, tag1? : string)`  (call as `street_util_serialized.addEdge(…)`): Add an edge with explicit tangents at both endpoints.
    - param `edges`: edge list to append to
    - param `p0`: start position
    - param `p1`: end position
    - param `t0`: tangent at start
    - param `t1`: tangent at end
    - used 2x in the base game, e.g. `base/content/infrastructure/street_construction/street_construction/street_constructions.script.lua:61`: `street_util_serialized.addEdge(`
- `addEdgeAutoTangents : function(edges : {Edge}, p0 : Vec3Serialized.Vec3, p1 : Vec3Serialized.Vec3, t0 : Vec3Serialized.Vec3, t1 : Vec3Serialized.Vec3, tag0? : string, tag1? : string)`  (call as `street_util_serialized.addEdgeAutoTangents(…)`): Add an edge with automatically computed tangents.
    - param `edges`: edge list to append to
    - param `p0`: start position
    - param `p1`: end position
    - param `t0`: tangent hint at start
    - param `t1`: tangent hint at end
    - used 68x in the base game, e.g. `base/content/infrastructure/street_construction/street_construction/street_constructions.script.lua:29`: `street_util_serialized.addEdgeAutoTangents(dir.z > 0 and edges or edgesBridge, p0, pI, t0, dir, tag0, nil)`
- `freeAllNodes : function(edges : {Edge}) : {integer}`  (call as `street_util_serialized.freeAllNodes(…)`): Get indices of all nodes in the edge list as free (unconnected) node indices (0-based).
    - param `edges`: edge list
    - returns: list of free node indices
    - used 27x in the base game, e.g. `base/content/infrastructure/street_construction/street_construction/street_constructions.script.lua:228`: `freeNodes = street_util_serialized.freeAllNodes(ledges),`
