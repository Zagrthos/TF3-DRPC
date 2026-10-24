# mission.mission_pathfinding_util

Definition file: `base/tealdef/mission/mission_pathfinding_util.d.tl` (in the game folder).

Path finding between entities and areas on the transport network, used by mission tasks.

## MissionPathfindingUtil (record)

```teal
global record MissionPathfindingUtil
```
Load: `local mission_pathfinding_util = ug_require "::/mission/mission_pathfinding_util.tl" as MissionPathfindingUtil`

Path finding on the transport network for mission checks.

- `getNodesForEntity : function(entity : Engine.Entity) : {NodeId}`  (call as `mission_pathfinding_util.getNodesForEntity(…)`): Returns the network nodes of a station (its terminal vehicle nodes) or of an entity with a transport network; for constructions the first such subconstruction is used.
    - param `entity`: Station, construction or network entity.
    - returns: Node ids (empty if none apply).
- `findPathEntityToEntity : function(entity1 : Engine.Entity, entity2 : Engine.Entity, transportModes : {api.type.enum.TransportMode}) : {{EdgeId, boolean}}`  (call as `mission_pathfinding_util.findPathEntityToEntity(…)`): Finds a path between the network nodes of two entities.
    - param `entity1`: Start entity.
    - param `entity2`: Goal entity.
    - param `transportModes`: Allowed transport modes; `nil` allows all.
    - returns: Path as a list of `{edge id, direction}` pairs.
    - used 4x in the base game, e.g. `base/content/mission/mission/tasks/auto_builder/electrify.tl:88`: `local path = mission_pathfinding_util.findPathEntityToEntity(nodePair[1], nodePair[2], {api.type["enum"].TransportMode.TRAIN})`
- `findPathEntityToCircle : function(entity1 : Engine.Entity, position : api.type.Vec2f, radius : number, transportModes : {api.type.enum.TransportMode}) : {{EdgeId, boolean}}`  (call as `mission_pathfinding_util.findPathEntityToCircle(…)`): Finds a path from the network nodes of an entity to any network node inside a circle.
    - param `entity1`: Start entity.
    - param `position`: Centre of the goal circle.
    - param `radius`: Radius in metres.
    - param `transportModes`: Allowed transport modes; `nil` allows all.
    - returns: Path as a list of `{edge id, direction}` pairs.
    - used 2x in the base game, e.g. `mods/release/urbangames_campaign_mission_01/content/mission/mission/mission_story.tl:142`: `local path1 = mission_pathfinding_util.findPathEntityToCircle(params.entity_newOrleansForest, street1, params.connectStreets_1_1_zone.radius + 5, {api.type["enu`
