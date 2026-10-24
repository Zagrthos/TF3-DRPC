# mission.mission_types

Definition file: `base/tealdef/mission/mission_types.d.tl` (in the game folder).

`MissionOutline`: helpers that build zone polygons and circles for mission tasks.

## MissionOutline (record)

```teal
global record MissionOutline
```
Load: `local mission_outline = ug_require "::/mission/mission_outline.tl" as MissionOutline`

Helpers that build zone shapes (polygons around the origin, circles) for mission tasks.

- `boundingCircle : function(positions : {{number, number, number}}, extension : number) : MissionInterface.PosRadius`  (call as `mission_outline.boundingCircle(…)`): Builds a circle around a set of points, centred on their centroid.
    - param `positions`: Points `{x, y, z}`.
    - param `extension`: Extra radius in metres.
    - returns: 2D circle.
    - used 3x in the base game, e.g. `mods/release/urbangames_campaign_mission_03/content/mission/mission/mission_story.tl:827`: `excavation_pos = { location = mission_outline.boundingCircle(params.bonesPositions, 30) },`
- `busStationOutline : function() : PolygonUtil.Polygon`  (call as `mission_outline.busStationOutline(…)`): Returns the outline of a street bus stop (polygon around the origin, metres).
- `streetDepotOutline : function() : PolygonUtil.Polygon`  (call as `mission_outline.streetDepotOutline(…)`): Returns the outline of a street depot (polygon around the origin, metres).
    - used 3x in the base game, e.g. `mods/release/urbangames_campaign_mission_01/content/mission/mission/mission_story.tl:766`: `polygon = polygon_util.transform(api.type.Mat4f.rotZTransl(params.streetDepotAngle, api.type.Vec3f.new(params.streetDepotZone.pos[1], params.streetDepotZone.pos`
- `warehouseOutline : function() : PolygonUtil.Polygon`  (call as `mission_outline.warehouseOutline(…)`): Returns the outline of a warehouse (polygon around the origin, metres).
    - used 1x in the base game, e.g. `mods/release/urbangames_campaign_mission_01/content/mission/mission/mission_story.tl:1262`: `polygon = polygon_util.transform(api.type.Mat4f.rotZTransl(params.warehouseAngle, api.type.Vec3f.new(params.warehouseZone.pos[1], params.warehouseZone.pos[2], 0`
- `railStationHeadOutline : function() : PolygonUtil.Polygon`  (call as `mission_outline.railStationHeadOutline(…)`): Returns the outline of a rail station head (polygon around the origin, metres).
- `railStationOutline : function() : PolygonUtil.Polygon`  (call as `mission_outline.railStationOutline(…)`): Returns the outline of a rail station (polygon around the origin, metres).
- `railDepotOutline : function() : PolygonUtil.Polygon`  (call as `mission_outline.railDepotOutline(…)`): Returns the outline of a rail depot (polygon around the origin, metres).
- `cargoStationOutline : function() : PolygonUtil.Polygon`  (call as `mission_outline.cargoStationOutline(…)`): Returns the outline of a cargo station (136 x 48 m rectangle around the origin).
    - used 1x in the base game, e.g. `mods/release/urbangames_campaign_mission_02/content/mission/mission/mission_story.tl:1282`: `polygon = polygon_util.transform(api.type.Mat4f.rotZTransl(params.stationAngle, api.type.Vec3f.new(params.stationZone.pos[1], params.stationZone.pos[2], 0)), mi`
- `getPosRadiusFromEntityWithBoundingVolume : function(entity : Engine.Entity, radius : number) : MissionInterface.PosRadius`  (call as `mission_outline.getPosRadiusFromEntityWithBoundingVolume(…)`): Builds a circle with the given radius at the centre of an entity's bounding volume.
    - param `entity`: Entity.
    - param `radius`: Radius in metres.
    - returns: 2D circle; an empty table if the entity does not exist, `nil` if it has no bounding volume.
    - used 1x in the base game, e.g. `base/content/mission/mission/tasks/build_construction/build_construction.tl:14`: `circle = mission_outline.getPosRadiusFromEntityWithBoundingVolume(entity, 5),`
