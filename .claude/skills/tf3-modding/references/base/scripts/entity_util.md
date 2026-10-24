# scripts.entity_util

Definition file: `base/tealdef/scripts/entity_util.d.tl` (in the game folder).

Types of `/scripts/entity_util.tl` (`EntityUtil`): helpers to remember an entity together with its
revision (so a script can detect later that it changed or was removed), and ownership/position lookups.

## EntityUtil (record)

```teal
global record EntityUtil
```
Load: `local entity_util = ug_require "::/scripts/entity_util.tl" as EntityUtil`

Entity helpers returned by `ug_require "/scripts/entity_util.tl"`.

- `makeEntityAndRevision : function(Engine.Entity) : EntityAndRevision`  (call as `entity_util.makeEntityAndRevision(…)`): Records an entity with its full current revision.
    - returns: The record, or nil if the entity does not exist.
    - used 54x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company.script.tl:151`: `resultEntity = entity_util.makeEntityAndRevision(resultEntities[1][1]),`
- `makeEntityAndRevision0 : function(Engine.Entity) : EntityAndRevision`  (call as `entity_util.makeEntityAndRevision0(…)`): Records an entity with only the first revision number (the other two set to 0).
    - returns: The record, or nil if the entity does not exist.
    - notes: Use with `entityChanged0`, which compares only the first revision number.
    - used 34x in the base game, e.g. `base/content/game_mechanics/game_mechanics/celebrations/celebrations.script.tl:189`: `local entityAndRev0 = entity_util.makeEntityAndRevision0(townBuildingComponent.town)`
- `makeEntityAndRevision0Array : function({Engine.Entity}) : {EntityUtil.EntityAndRevision}`  (call as `entity_util.makeEntityAndRevision0Array(…)`): Applies `makeEntityAndRevision0` to each entity and returns the records of those that exist.
    - used 6x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notifications.script.tl:206`: `entities = entity_util.makeEntityAndRevision0Array(entitiesAndProblem[1]),`
- `entityEquals : function(EntityAndRevision, EntityAndRevision) : boolean`  (call as `entity_util.entityEquals(…)`): True if both records have the same entity id and the same first revision number.
    - used 6x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company.script.tl:491`: `if ecoLevel and ecoLevel[1] and entity_util.entityEquals(ecoLevel[1], entityAndRev) then`
- `entityChanged0 : function(EntityAndRevision) : boolean`  (call as `entity_util.entityChanged0(…)`): True if the entity no longer exists or its first revision number has increased since the record was made.
    - used 87x in the base game, e.g. `base/content/game_mechanics/game_mechanics/celebrations/celebration_react_util.tl:202`: `if celebration.entity and entity_util.entityChanged0(celebration.entity) then`
- `entityChanged : function(EntityAndRevision) : boolean`  (call as `entity_util.entityChanged(…)`): True if the entity no longer exists or its first or second revision number has increased since the record was made.
    - used 2x in the base game, e.g. `base/content/gui/gui/entity_window/bridge_and_tunnel.tl:199`: `if not entity_util.entityChanged({ entity = entityRevision[1], revision = entityRevision[2] }) then`
- `getPosition : function(Engine.Entity) : Vec3f`  (call as `entity_util.getPosition(…)`): Centre of the entity's bounding box (`BOUNDING_VOLUME` component).
    - returns: The centre position, or nil if the entity has no bounding volume.
    - used 54x in the base game, e.g. `base/content/game_mechanics/game_mechanics/achievements/achievements.script.tl:412`: `local position = entity_util.getPosition(headCarriage)`
- `sameRevision : function(Engine.Revision, Engine.Revision) : boolean`  (call as `entity_util.sameRevision(…)`): True if all three revision numbers are equal.
- `getEntityName : function(Engine.Entity) : string`  (call as `entity_util.getEntityName(…)`): Display name of the entity (`api.engine.util.getEntityName`).
    - used 34x in the base game, e.g. `base/content/game_mechanics/game_mechanics/celebrations/celebration_react_util.tl:31`: `townName = entity_util.getEntityName(target.entity)`
- `getPlayerOwned : function(Engine.Entity) : Engine.Component.PlayerOwned`  (call as `entity_util.getPlayerOwned(…)`): The entity's `PLAYER_OWNED` component, or nil if no player owns it.
    - used 1x in the base game, e.g. `base/content/gui/gui/debug_panel/make_entity_debug_panel.tl:389`: `local po = entity_util.getPlayerOwned(e)`
- `isOwnedByPlayer : function(Engine.Entity) : boolean`  (call as `entity_util.isOwnedByPlayer(…)`): True if the entity is owned by the current player (`api.engine.util.getPlayer()`).
    - used 30x in the base game, e.g. `base/content/game_mechanics/game_mechanics/achievements/achievements.script.tl:403`: `if entity_util.isOwnedByPlayer(vehicleEntity) then`
- `isOwnedByPlayerOrNotOwned : function(Engine.Entity) : boolean`  (call as `entity_util.isOwnedByPlayerOrNotOwned(…)`): True if the entity is owned by the current player or has no owner.
    - used 12x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notifications.script.tl:522`: `if entity_util.isOwnedByPlayerOrNotOwned(constructionEntity) then`
- `getBoundingVolumePosition : function(Engine.Component.BoundingVolume) : Vec3f`  (call as `entity_util.getBoundingVolumePosition(…)`): Centre of the given bounding volume's box.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util.tl:1202`: `local location = entity_util.getBoundingVolumePosition(proposalData.toAddData_native:at(index).boundingVolume)`

### EntityUtil.EntityAndRevision (record)

```teal
record EntityAndRevision
```

An entity id stored together with the revision it had when it was recorded. Scripts keep these in
their state and later call `entityChanged` / `entityChanged0` to see whether the entity was modified,
rebuilt or removed in the meantime.

- `entity : Engine.Entity`: The entity id.
- `revision : Engine.Revision`: The revision of the entity at the time it was recorded.
