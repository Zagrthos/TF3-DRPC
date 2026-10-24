# mission.builder_util

Definition file: `base/tealdef/mission/builder_util.d.tl` (in the game folder).

Small geometry helpers for checking build proposals in mission tasks.

## BuilderUtil (record)

```teal
global record BuilderUtil
```
Load: `local builder_util = ug_require "::/mission/builder_util.tl" as BuilderUtil`

Geometry helpers for checking build proposals.

- `anglediff : function(alpha : number, beta : number) : number`  (call as `builder_util.anglediff(…)`): Returns the absolute difference between two angles, wrapped to 0..pi.
    - param `alpha`: Angle in radians.
    - param `beta`: Angle in radians.
    - returns: Difference in radians.
- `closeEnough : function(transf : Mat4f, goal : Vec2f, tol? : number) : boolean`  (call as `builder_util.closeEnough(…)`): Tells whether the translation of a transformation is near a 2D point.
    - param `transf`: Transformation.
    - param `goal`: Target position.
    - param `tol`: Tolerance in metres (default 12).
    - returns: `true` if closer than the tolerance.
    - used 1x in the base game, e.g. `base/content/mission/mission/tasks/build_construction/mission_task_build_construction_util.tl:313`: `local ce = builder_util.closeEnough(con.transf, api.type.Vec2f.new(targetPosX, targetPosY), targetMaxDist)`
- `segments2nodes : function(segments : {Proposal.SegmentAndEntity}, newnodes? : {Proposal.NodeAndEntity}) : {Engine.Entity : Engine.Component.BaseNode}`  (call as `builder_util.segments2nodes(…)`): Collects the end nodes of proposal segments with their node components; new (negative) nodes are looked up in `newnodes`.
    - param `segments`: Proposal segments.
    - param `newnodes`: New nodes of the proposal.
    - returns: Node entity -> base-node component.
    - used 1x in the base game, e.g. `base/content/mission/mission/tasks/build_construction/mission_task_build_construction_util.tl:290`: `local nodes = builder_util.segments2nodes(proposal.proposal.addedSegments, proposal.proposal.addedNodes)`
