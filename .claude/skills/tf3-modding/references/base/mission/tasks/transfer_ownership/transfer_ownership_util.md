# mission.tasks.transfer_ownership.transfer_ownership_util

Definition file: `base/tealdef/mission/tasks/transfer_ownership/transfer_ownership_util.d.tl` (in the game folder).

Ownership helpers used by mission tasks.

## TransferOwnershipUtil (record)

```teal
global record TransferOwnershipUtil
```
Load: `local transfer_ownership_util = ug_require "::/mission/tasks/transfer_ownership/transfer_ownership_util.tl" as TransferOwnershipUtil`

Ownership helpers used by mission tasks.

- `spawnAiPlayer : function(callback : function(Engine.Entity))`  (call as `transfer_ownership_util.spawnAiPlayer(…)`): Adds a new player named `aiPlayer`.
    - param `callback`: Called with the new player entity.
    - used 9x in the base game, e.g. `base/content/gui/gui/debug_panel/make_entity_debug_panel.tl:281`: `transfer_ownership_util.spawnAiPlayer(onAiPlayerSpawned)`
- `transferLineEntities : function(entity : Engine.Entity, targetPlayer : Engine.Entity)`  (call as `transfer_ownership_util.transferLineEntities(…)`): Gives a line, its vehicles, its stop station groups, their stations and their constructions to a player.
    - param `entity`: Line entity.
    - param `targetPlayer`: New owner.
    - used 5x in the base game, e.g. `base/content/mission/mission/tasks/transfer_ownership/transfer_ownership.tl:20`: `transfer_ownership_util.transferLineEntities(entity, aiPlayerEntity)`
- `transferEntity : function(entity : Engine.Entity, targetPlayer : Engine.Entity)`  (call as `transfer_ownership_util.transferEntity(…)`): Gives an entity, its construction and the construction's stations and station groups to a player.
    - param `entity`: Entity.
    - param `targetPlayer`: New owner.
    - used 9x in the base game, e.g. `base/content/gui/gui/debug_panel/make_entity_debug_panel.tl:400`: `transfer_ownership_util.transferEntity(e, aiPlayerState:old())`
