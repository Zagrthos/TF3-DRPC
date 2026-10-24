# game_mechanics.fun_elements.custom_entity_util

Definition file: `base/tealdef/game_mechanics/fun_elements/custom_entity_util.d.tl` (in the game folder).

Helpers to spawn, move and remove custom entities (script-controlled models), plus movement maths for flying ones.

## CustomEntityUtil (record)

```teal
global record CustomEntityUtil
```
Load: `local custom_entity_util = ug_require "::/game_mechanics/fun_elements/custom_entity_util.tl" as CustomEntityUtil`

Helpers for custom entities (models placed and moved by script) and movement maths for flying ones.

- `convertMatrixToTable : function(Mat4f) : {{number, number, number, number}, {number, number, number, number}, {number, number, number, number}, {number, number, number, number}}`  (call as `custom_entity_util.convertMatrixToTable(…)`): Converts a matrix to a plain table that can be saved in game script state or sent as event parameter.
    - param `#1`: Matrix.
    - returns: Table of rows; see `MatCols`.
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/fun_elements/fireworks.script.tl:126`: `local transf = custom_entity_util.convertMatrixToTable(`
- `convertTableToMatrix : function({{number, number, number, number}, {number, number, number, number}, {number, number, number, number}, {number, number, number, number}}) : Mat4f`  (call as `custom_entity_util.convertTableToMatrix(…)`): Converts a table from `convertMatrixToTable` back to a matrix.
    - param `#1`: Table of rows.
    - returns: The matrix.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/fun_elements/rocket_util.tl:29`: `local mat = custom_entity_util.convertTableToMatrix(transf)`
- `spawn : function(string, transf : Mat4f, callback : function(Engine.Entity))`  (call as `custom_entity_util.spawn(…)`): Creates a custom entity of a model and sets its transformation. Logs an error and does nothing if the model does not exist.
    - param `#1`: Model res name.
    - param `transf`: Transformation.
    - param `callback`: Receives the new entity once it is placed.
    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/fun_elements/rocket_util.tl:33`: `custom_entity_util.spawn(model, mat, function(rocketEntity:Engine.Entity)`
- `getTransf : function(entity : Engine.Entity) : Mat4f`  (call as `custom_entity_util.getTransf(…)`): Transformation of the entity's first model instance.
    - param `entity`: Custom entity.
    - returns: The transformation.
- `setTransf : function(entity : Engine.Entity, transf : Mat4f)`  (call as `custom_entity_util.setTransf(…)`): Sets the transformation of a custom entity.
    - param `entity`: Custom entity.
    - param `transf`: New transformation.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/fun_elements/rocket_util.tl:106`: `custom_entity_util.setTransf(rocketData.rocketEntity.entity, newTransf)`
- `destroy : function(entity : Engine.Entity)`  (call as `custom_entity_util.destroy(…)`): Removes a custom entity.
    - param `entity`: Custom entity.
    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/fun_elements/rocket_util.tl:62`: `custom_entity_util.destroy(rocketData.rocketEntity.entity)`
- `getPos : function(entity : Engine.Entity) : Vec3f`  (call as `custom_entity_util.getPos(…)`): Position of a custom entity.
    - param `entity`: Custom entity.
    - returns: World position.
- `setPos : function(entity : Engine.Entity, pos : Vec3f)`  (call as `custom_entity_util.setPos(…)`): Moves a custom entity and keeps its rotation.
    - param `entity`: Custom entity.
    - param `pos`: New world position.
- `findSpawnPoint : function(pos : Vec3f, xyOffset : number, zOffset : number) : Vec3f`  (call as `custom_entity_util.findSpawnPoint(…)`): Random point in a square around a position, at a height above the terrain.
    - param `pos`: Centre of the square.
    - param `xyOffset`: Half the side of the square (metres).
    - param `zOffset`: Height above the terrain (metres).
    - returns: The point.
    - used 2x in the base game, e.g. `base/content/mission/mission/tasks/click_and_collect_custom_entity/spawn_entity.tl:24`: `local pos = custom_entity_util.findSpawnPoint(vec, 0.5, params.alignTerrain and 0 or position[3])`
- `findSpawnPointClosestOffMap : function(pos : Vec3f, xyOffset : number, zOffset : number) : Vec3f, Vec3f`  (call as `custom_entity_util.findSpawnPointClosestOffMap(…)`): Returns a position and direction (pointing inside the map) at the edge of the map that is closest to pos
    - param `pos`: the world position for which to find a closest edge position
    - param `xyOffset`: the offset from the border of the map for the point (in meters)
    - param `zOffset`: the altitude of the point
    - returns: a position and direction
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/explorations/exploration_plane.script.tl:87`: `local startPos, startDir = custom_entity_util.findSpawnPointClosestOffMap(nextTarget3D, offmap, altitude)`
- `approach : function(current : number, target : number, shift : number) : number`  (call as `custom_entity_util.approach(…)`): Moves a number towards a target by at most `shift`, without overshooting.
    - param `current`: Start value.
    - param `target`: Target value.
    - param `shift`: Maximum change; the sign is ignored.
    - returns: The new value.
- `approachVec3f : function(current : Vec3f, target : Vec3f, shift : number) : Vec3f`  (call as `custom_entity_util.approachVec3f(…)`): Moves a point towards a target by about `shift` along the straight line, without overshooting.
    - param `current`: Start point.
    - param `target`: Target point.
    - param `shift`: Step length.
    - returns: The new point.
- `approachAngularVec2f : function(current : Vec2f, target : Vec2f, shift : number) : Vec2f`  (call as `custom_entity_util.approachAngularVec2f(…)`): Turns a 2D direction towards a target direction along the shorter way.
    - param `current`: Current direction.
    - param `target`: Target direction.
    - param `shift`: Share of the angle difference to turn.
    - returns: The new unit direction.
- `approachAngularVec3f : function(current : Vec3f, target : Vec3f, shift : number) : Vec3f`  (call as `custom_entity_util.approachAngularVec3f(…)`): Turns a 3D direction towards a target direction; heading as in `approachAngularVec2f`, the z part as in `approach`.
    - param `current`: Current direction.
    - param `target`: Target direction.
    - param `shift`: Turn amount.
    - returns: The new unit direction.
- `vec2fToYaw : function(v : Vec2f) : number`  (call as `custom_entity_util.vec2fToYaw(…)`): Heading of a 2D vector.
    - param `v`: Vector.
    - returns: Angle in radians (`atan2(y, x)`).
    - used 2x in the base game, e.g. `base/content/mission/mission/tasks/spawn_sims/spawn_sims.tl:90`: `local angle = custom_entity_util.vec2fToYaw(api.type.Vec2f.new(-normal.x, -normal.y))`
- `yawToVec2f : function(angle : number) : Vec2f`  (call as `custom_entity_util.yawToVec2f(…)`): Unit vector for a heading.
    - param `angle`: Angle in radians.
    - returns: The unit vector.
- `calcAirshipMovement : function(position : Vec3f, velocity : Vec3f, targetPos : Vec3f, minGroundOffset : number, minSpeed : number, maxSpeed : number, acceleration : number, steering : number, speedLerpMult : number, dt : number) : MovementData`  (call as `custom_entity_util.calcAirshipMovement(…)`): Computes one movement step of a flying entity that steers towards a target and slows down near it.
    - param `position`: Current position.
    - param `velocity`: Current velocity.
    - param `targetPos`: Target position.
    - param `minGroundOffset`: Minimum height above the terrain (metres).
    - param `minSpeed`: Speed used within 50 metres of the target.
    - param `maxSpeed`: Speed used further away.
    - param `acceleration`: Maximum speed change per second.
    - param `steering`: Turn rate; also sets the reach distance (`2 / steering` metres).
    - param `speedLerpMult`: Smoothing of the speed change; 0 uses the target speed directly.
    - param `dt`: Time step in seconds.
    - returns: The new movement state; apply it with `applyAirshipMovement`.
- `applyAirshipMovement : function(entity : Engine.Entity, data : MovementData)`  (call as `custom_entity_util.applyAirshipMovement(…)`): Places the entity at the new position, turned to the heading of the new direction.
    - param `entity`: Custom entity.
    - param `data`: Result of `calcAirshipMovement`.
- `getModelInstanceList : function(entity : Engine.Entity) : Engine.Component.ModelInstanceList`  (call as `custom_entity_util.getModelInstanceList(…)`): Model instance list of an entity, or of the first carriage of a vehicle that has none of its own.
    - param `entity`: Entity.
    - returns: The model instance list.
    - used 2x in the base game, e.g. `base/content/gui/gui/entity_window/make_entity_window.tl:708`: `local mil = custom_entity_util.getModelInstanceList(entity)`

### CustomEntityUtil.MovementData (record)

```teal
record MovementData
```

Result of one movement step, from `calcAirshipMovement`.

- `velocity : Vec3f`: New velocity.
- `position : Vec3f`: New position, kept at least the minimum ground offset above the terrain.
- `direction : Vec3f`: New unit direction.
- `distanceToTarget : number`: Distance from the new position to the target.
- `targetReached : boolean`: True when the target counts as reached.
