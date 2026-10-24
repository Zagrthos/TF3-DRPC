# game_mechanics.fun_elements.blimp_util

Definition file: `base/tealdef/game_mechanics/fun_elements/blimp_util.d.tl` (in the game folder).

Airship that circles a town during a marketing campaign.

## BlimpUtil (record)

```teal
global record BlimpUtil
```

Airship that flies to a town when a marketing campaign starts there, circles it while the campaign runs, and then leaves the map.

Notes:

The fun elements game script spawns one on the `Towns` event `startMarketingCampaign` and calls `update` for it every frame.

- `spawnNew : function(townEntity : Engine.Entity, callback : function(BlimpData))`: Spawns an airship near a town, with a model chosen by the current year.
    - param `townEntity`: Town to fly to.
    - param `callback`: Receives the new airship's data once the entity exists.
- `update : function(blimpData : BlimpData, dt : number) : boolean`: Moves the airship one step and advances its flight stage.
    - param `blimpData`: Airship to update; changed in place.
    - param `dt`: Time step in seconds.
    - returns: True when the airship was removed and its data can be dropped.

### BlimpUtil.BlimpState (enum)

```teal
enum BlimpState
```

Flight stage of an airship.

- `"Approaching"`: Flying to the town at 4 times cruise speed.
  In game: "Approaching" (German: „Nähert sich“)
- `"Circling"`: Flying to random points around the town while the campaign runs.
- `"Leaving"`: Flying to the nearest map edge.
  In game: "Leaving" (German: „Verlässt“)
- `"Despawn"`: Shrunk at the map edge, waiting to be removed.

### BlimpUtil.Era (enum)

```teal
enum Era
```

Era that decides the airship models.

- `"EraA"`: Up to 1950; ZMC-2 only.
- `"EraB"`: 1951 to 1990; adds the LZ 127 Graf Zeppelin.
- `"EraC"`: After 1990; adds the Zeppelin NT.

### BlimpUtil.BlimpData (record)

```teal
record BlimpData
```

Saved state of one airship. It is also sent to the entity as custom state `blimpData`.

- `blimpEntity : EntityUtil.EntityAndRevision`: Airship entity.
- `townEntity : EntityUtil.EntityAndRevision`: Town the airship circles.
- `era : Era`: Era at spawn time.
- `state : BlimpState`: Current flight stage.
- `velocity : {number}`: Velocity as `{x, y, z}` in metres per second.
- `velocityNormalized : number`: Current speed (length of `velocity`).
- `velocityMin : number`: Minimum cruise speed (5).
- `velocityMax : number`: Maximum cruise speed (15).
- `roll : number`: Turn rate estimated from the last velocity change; negative when turning right.
- `elevator : number`: Vertical speed divided by the maximum cruise speed.
- `flyHeight : number`: Height above the town at spawn, 105 to 150 metres.
- `flyRadius : number`: Distance from the town centre at spawn, 125 to 250 metres.
- `target : {number}`: Current target position as `{x, y, z}`.
- `targetSetTime : number`: Game time (ms) when `target` was set. After 20 seconds the airship picks a new target even if it has not reached it.
