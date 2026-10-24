# mission.mission_framework.mission_framework_util_entity

Definition file: `base/tealdef/mission/mission_framework/mission_framework_util_entity.d.tl` (in the game folder).

Commands that change ownership of, build, upgrade or bulldoze entities during a mission.

## MissionFrameworkUtilEntity (record)

```teal
global record MissionFrameworkUtilEntity
```

Commands that change ownership of, build, upgrade or bulldoze entities.

- `setPlayer : function(Engine.Entity, Engine.Entity)`: Sets the owner of one entity.
    - param `#1`: Entity.
    - param `#2`: Player entity (`-1` for none).
- `setPlayerForBaseEdge : function(Engine.Entity, Engine.Entity)`: Sets the owner of a street/track edge and of its edge objects.
    - param `#1`: Edge entity.
    - param `#2`: Player entity.
- `setPlayerForConstruction : function(Engine.Entity, Engine.Entity)`: Sets the owner of a construction and of its frozen edges, depots (with their vehicles), stations, station groups and industries.
    - param `#1`: Construction entity.
    - param `#2`: Player entity.
- `setPlayerForAnyEntity : function(Engine.Entity, Engine.Entity)`: Sets the owner of a construction, a subconstruction (via its construction) or an asset group.
    - param `#1`: Entity.
    - param `#2`: Player entity.
- `upgradeConstruction : function(Engine.Entity, table, string, string)`: Replaces a construction with a new one at the same place (unowned), keeping file, params or name where not given.
    - param `#1`: Construction entity to replace.
    - param `#2`: New construction params (`nil` keeps the old ones).
    - param `#3`: New construction file (`nil` keeps the old one).
    - param `#4`: New name (`nil` keeps the old one).
- `buildConstruction : function(table, string, Mat4f, string, function(Engine.Entity))`: Builds an unowned construction.
    - param `#1`: Construction params (a `seed` is derived from the position if missing).
    - param `#2`: Construction file.
    - param `#3`: Transformation.
    - param `#4`: Name.
    - param `#5`: Called with the new construction entity, or `nil` on failure.
- `bulldoze : function(Engine.Entity)`: Removes a construction.
    - param `#1`: Construction entity.
