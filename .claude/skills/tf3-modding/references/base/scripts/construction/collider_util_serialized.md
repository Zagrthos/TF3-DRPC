# scripts.construction.collider_util_serialized

Definition file: `base/tealdef/scripts/construction/collider_util_serialized.d.tl` (in the game folder).

Types of `/scripts/construction/collider_util_serialized.tl` (`ColliderUtilSerialized`): plain-table
colliders (box, cylinder, point cloud) for construction results.

## ColliderUtilSerialized (record)

```teal
global record ColliderUtilSerialized
```
Load: `local collider_util_serialized = require "::/scripts/construction/collider_util_serialized.tl" as ColliderUtilSerialized`

Collider helpers returned by `ug_require "/scripts/construction/collider_util_serialized.tl"`.

- `createBox : function(center : Vec3Serialized.Vec3, halfExtents : Vec3Serialized.Vec3) : Collider`  (call as `collider_util_serialized.createBox(…)`): Create a box collider.
    - param `center`: center position of the box
    - param `halfExtents`: half-extents along each axis
    - returns: box collider
    - used 6x in the base game, e.g. `base/content/stations/air/air/airfield.script.lua:188`: `collider_util_serialized.createBox(vec3.new(midPoint, -13, 10), vec3.new(halfSize, 54, 20))`
- `createCylinder : function(direction? : ColliderUtilSerialized.Direction, center? : Vec3Serialized.Vec3, halfExtents? : Vec3Serialized.Vec3) : Collider`  (call as `collider_util_serialized.createCylinder(…)`): Create a cylinder collider.
    - param `direction`: axis direction of the cylinder, default (nil) is z-direction
    - param `center`: center position of the cylinder, default (nil) is origin
    - param `halfExtents`: half-extents along each axis, default (nil) is none
    - returns: cylinder collider
- `createPointCloud : function(points : {Vec3Serialized.Vec3}) : Collider`  (call as `collider_util_serialized.createPointCloud(…)`): Create a point cloud collider.
    - param `points`: list of points forming the cloud
    - returns: point cloud collider
    - used 1x in the base game, e.g. `base/content/scripts/scripts/construction/constructionutil.lua:338`: `local collider = collider_util_serialized.createPointCloud(points)`
- `extrudePoints : function(points : {Vec3Serialized.Vec3}, offset1 : number, offset2 : number) : {Vec3Serialized.Vec3}`  (call as `collider_util_serialized.extrudePoints(…)`): Extrude a list of points by offsetting them along the z-direction. Each point is hence added twice once with offset1 and once with offset2.
    - param `points`: input points to extrude
    - param `offset1`: offset for first set of extruded points
    - param `offset2`: offset for second set of extruded points
    - returns: extruded point list

### ColliderUtilSerialized.Collider (record)

```teal
record Collider
```

A collision shape of a construction, as a plain table with `type`, `transf` and `params`.

- `type : Type`: Shape of the collider.
- `transf : Mat4Serialized.Mat4`: Transform matrix of collider
- `params : Params`: Shape parameters of collider

#### ColliderUtilSerialized.Collider.Type (enum)

```teal
enum Type
```

Shape of a collider.

- `"NONE"`: No collider shape
- `"BOX"`: Box-shaped collider
- `"CYLINDER"`: Cylinder-shaped collider
- `"POINT_CLOUD"`: Point cloud collider

#### ColliderUtilSerialized.Collider.Params (record)

```teal
record Params
```

Shape parameters; which field is used depends on `type`.

- `halfExtents : Vec3Serialized.Table3`: Half-extents of the box or cylinder (x, y, z)
- `points : {Vec3Serialized.Table3}`: Points used for a point cloud collider

### ColliderUtilSerialized.Direction (enum)

```teal
enum Direction
```

Axis of a cylinder collider; without one (nil) the cylinder points along Z.

- `"X"`: Along the X axis
- `"Y"`: Along the Y axis
