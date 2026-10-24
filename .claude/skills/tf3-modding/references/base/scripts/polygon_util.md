# scripts.polygon_util

Definition file: `base/tealdef/scripts/polygon_util.d.tl` (in the game folder).

Types of `/scripts/polygon_util.tl` (`PolygonUtil`) for 2D polygons given as lists of `Vec2f` points.

## PolygonUtil (record)

```teal
global record PolygonUtil
```
Load: `local polygon_util = ug_require "::/scripts/polygon_util.tl" as PolygonUtil`

Polygon helpers returned by `ug_require "/scripts/polygon_util.tl"`.

- `transform : function(mat : Mat4f, polygon : Polygon) : Polygon`  (call as `polygon_util.transform(…)`): Applies the X/Y part of a 4x4 transform (rotation, scale and translation in the ground plane) to each point.
    - returns: A new polygon.
    - used 5x in the base game, e.g. `mods/release/urbangames_campaign_mission_01/content/mission/mission/mission_story.tl:766`: `polygon = polygon_util.transform(api.type.Mat4f.rotZTransl(params.streetDepotAngle, api.type.Vec3f.new(params.streetDepotZone.pos[1], params.streetDepotZone.pos`
- `makeQuad : function(bottomLeft : Vec2f, topRight : Vec2f) : Polygon`  (call as `polygon_util.makeQuad(…)`): Axis-aligned rectangle from its bottom-left and top-right corners, in counter-clockwise order starting at bottom-left.
- `contains : function(polygon : Polygon, pos : Vec2f) : boolean`  (call as `polygon_util.contains(…)`): True if `pos` is inside the polygon (even-odd rule) or exactly on one of its corner points.
    - used 2x in the base game, e.g. `base/content/mission/mission/tasks/build_construction/build_construction_variant.tl:82`: `if polygon_util.contains(polygon, api.type.Vec2f.new(pos.x, pos.y)) then`
- `offset : function(polygon : Polygon, offset : number) : Polygon`  (call as `polygon_util.offset(…)`): Moves every edge of the polygon outwards or inwards by `offset` and returns the new corner points.
    - notes: The direction depends on the winding order of the polygon; try the sign on your polygon.

### PolygonUtil.Polygon (record)

```teal
record Polygon is {Vec2f}
```

A 2D polygon as a list of `Vec2f` corner points.
