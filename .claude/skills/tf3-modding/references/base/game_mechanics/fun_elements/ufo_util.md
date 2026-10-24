# game_mechanics.fun_elements.ufo_util

Definition file: `base/tealdef/game_mechanics/fun_elements/ufo_util.d.tl` (in the game folder).

UFO that flies in, abducts a cow from a livestock farm and leaves.

## UfoUtil (record)

```teal
global record UfoUtil
```

UFO that flies in from the map edge, lifts a cow from a livestock farm field and flies off again.

Notes:

The fun elements game script spawns one every 2 to 4 game years while no other UFO is in flight.

- `spawnNew : function(callback : function(UfoData)) : boolean`: Spawns a cow on a random cow field of a livestock farm and a UFO at the nearest map edge.
    - param `callback`: Receives the UFO's data once both entities exist.
    - returns: False if no livestock farm with cows exists.
- `update : function(data : UfoData, dt : number) : boolean`: Moves the UFO (or the cow during the abduction) one step and advances the flight stage.
    - param `data`: UFO to update; changed in place.
    - param `dt`: Time step in seconds.
    - returns: True when the UFO was removed and its data can be dropped.

### UfoUtil.UfoState (enum)

```teal
enum UfoState
```

Flight stage of a UFO.

- `"Approach"`: Flying from the map edge towards the farm.
- `"Target"`: Slowing down to hover 25 metres above the cow.
- `"Abduct"`: Lifting the cow.
  In game: "Abducting", "Abducted" (German: „Entführt gerade“, „Entführt“)
- `"Leave"`: Flying to the nearest map edge after the cow is gone.
  In game: "Leaving" (German: „Verlässt“)
- `"Despawn"`: Shrunk at the map edge; removed on the next update.

### UfoUtil.UfoData (record)

```teal
record UfoData
```

Saved state of one UFO.

- `ufoEntity : EntityUtil.EntityAndRevision`: UFO entity.
- `cowEntity : EntityUtil.EntityAndRevision`: Cow entity; empty after the abduction.
- `state : UfoState`: Current flight stage.
- `velocity : {number}`: Velocity as `{x, y, z}` in metres per second.
- `target : {number}`: Current target position as `{x, y, z}`.
- `abductSpeed : number`: Speed at which the cow is pulled up to the UFO in the `Abduct` stage, per `dt` unit of the game script update. It starts at 0 and rises to at most 6.
