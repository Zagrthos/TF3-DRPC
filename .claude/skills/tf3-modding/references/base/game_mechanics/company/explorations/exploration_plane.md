# game_mechanics.company.explorations.exploration_plane

Definition file: `base/tealdef/game_mechanics/company/explorations/exploration_plane.d.tl` (in the game folder).

State of the plane that flies over a town during a prospection.

## ExplorationPlaneCustomState (record)

```teal
global record ExplorationPlaneCustomState
```

Custom state of the plane entity that circles a town during a prospection.

- `townEntity : Engine.Entity`: Town the plane flies to.
- `townRevision0 : integer`: Revision of the town entity at the start.
- `initiatedTimestamp : integer`: Game time (ms) at which the prospection started.
- `cargoType : ResName`: Cargo type of the prospection.
