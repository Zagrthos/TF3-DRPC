# game_mechanics.fun_elements.rocket_util

Definition file: `base/tealdef/game_mechanics/fun_elements/rocket_util.d.tl` (in the game folder).

Rockets for the fireworks and for the rocket launch in campaign mission 8.

## Globals

- `VecCol : {number, number, number, number}`: Four numbers of one matrix row in a `MatCols` table.
- `MatCols : {VecCol, VecCol, VecCol, VecCol}`: 4x4 matrix as a plain Lua table, as made by `CustomEntityUtil.convertMatrixToTable`.
    - notes: Despite the name, the outer table holds rows; entry `[r][c]` is component `r` of column `c`, so the translation is in `[1][4]`, `[2][4]` and `[3][4]`.

## RocketParams (record)

```teal
global record RocketParams
```

Flight parameters of a rocket.

Example:
```lua
-- fireworks.script.tl
local rocketParams : RocketParams = {
    thrust = 1607,
    mass = 5,
    gravity = api.type.Vec3f.new(0, 0, -9.81),
    timeOnGround = 0.0,
    animationStartDelay = 0.0,
    totalFlightTime = 5,
    finalPitchAngle = math.pi / 4,
    verticalPhaseTime = 0.0,
    maxAltitude = 200.0,
}
```

- `thrust : number`: Thrust force; divided by `mass` for the acceleration along the rocket axis.
- `mass : number`: Mass of the rocket.
- `gravity : Vec3f`: Gravity acceleration, e.g. `(0, 0, -9.81)`.
- `timeOnGround : number`: Seconds after spawning before the rocket starts to move.
- `animationStartDelay : number`: Seconds after spawning when the launch animation starts (sent to the model as `launchAnimationGameTime`).
- `totalFlightTime : number`: Seconds after spawning when the rocket is removed.
- `finalPitchAngle : number`: Pitch (radians) reached at the end of the flight; the rocket tilts linearly after the vertical phase.
- `verticalPhaseTime : number`: Seconds of vertical flight after `timeOnGround`.
- `maxAltitude : number`: Height above the launch point (metres) at which the rocket stops and explodes. Optional; the fireworks use 200 plus up to 100.
- `rotZ : number`: Heading (radians) of the pitch-over.

## RocketUtil (record)

```teal
global record RocketUtil
```
Load: `local rocket_util = ug_require "::/game_mechanics/fun_elements/rocket_util.tl" as RocketUtil`

Spawning and flight of rockets.

- `spawnNew : function(model : string, transf : MatCols, rocketParams : RocketParams, callback : function(RocketData))`  (call as `rocket_util.spawnNew(…)`): Spawns a rocket model and sends its launch time, colour and size to the entity as custom state.
    - param `model`: Model res name, e.g. `::/game_mechanics/fun_elements/firework/firework1.mdl`.
    - param `transf`: Launch transformation.
    - param `rocketParams`: Flight parameters.
    - param `callback`: Receives the new rocket's data once the entity exists.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/fun_elements/fireworks.script.tl:74`: `rocket_util.spawnNew(models[math.random(1, #models)], transf, copy, function(rocketData : RocketUtil.RocketData)`
- `update : function(rocketData : RocketData, dt : number, rocketParams : RocketParams) : boolean`  (call as `rocket_util.update(…)`): Advances the rocket's flight by one step.
    - param `rocketData`: Rocket to update; changed in place.
    - param `dt`: Time step in seconds.
    - param `rocketParams`: Flight parameters; pass the same ones as to `spawnNew`.
    - returns: True when the flight time is over and the rocket was removed.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/fun_elements/fireworks.script.tl:137`: `updateHelper(rocketState.rockets, dt, rocket_util.update)`

### RocketUtil.RocketData (record)

```teal
record RocketData
```

Saved state of one rocket.

- `rocketEntity : EntityUtil.EntityAndRevision`: Rocket entity.
- `initialTransf : MatCols`: Launch transformation.
- `velocity : {number}`: Velocity as `{x, y, z}`.
- `position : {number}`: Offset from the launch point as `{x, y, z}`.
- `flightTime : number`: Seconds since spawning.
- `spawnTime : number`: Game time (ms) of spawning.
- `atMaxAltitude : boolean`: True once the rocket reached `maxAltitude`; the model then plays the explosion.
- `rotZ : number`: Heading (radians) copied from the params.
- `color : {number, number, number}`: Random normalised RGB colour of the explosion.
- `size : number`: Random value from 0 to 1 that sets the velocity damping of the explosion particles.

## RocketState (record)

```teal
global record RocketState
```

Saved state of the rocket game script of campaign mission 8.

- `rockets : {RocketUtil.RocketData}`: Rockets in flight.
- `spawnTime : integer`
