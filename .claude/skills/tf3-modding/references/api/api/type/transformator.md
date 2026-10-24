# api.type.transformator

Definition file: `api/tealdef/api/type/transformator.d.tl` (in the game folder).

Types used by transformator scripts: the per-frame scripting info a model's update script reads (vehicle, aircraft, ship, industry, world, ...), and the output objects it writes to (`Transformator.TransfOutput`, `Transformator.ParticleSystem`, `Transformator.ModelEmitter`).

## Globals

- `Engine` (type): Contains all type definitions for transformators

## Transformator (record)

```teal
global record Transformator
```

Transformator types

### Transformator.ModelEmitter (record)

```teal
record ModelEmitter
```

EmitModel in transformator

- `getModelId : function(modelEmitter : ModelEmitter, resName : string) : integer`  (call as `modelEmitter:getModelId(…) (method)`): Returns the model id of a model resource, for use with `emitModel`.
    - param `resName`: Model file, e.g. a path from the script's config params.
    - notes: The base scripts only request models that their `getEmittableModelsFn` returned.
    - used 5x in the base game, e.g. `base/content/scripts/scripts/transformator_util.tl:86`: `local modelId = modelEmitter:getModelId(lane.model)`
- `emitModel : function(modelEmitter : ModelEmitter, modelId : integer, transfMat : Mat4f, animationStates : {Transformator.AnimationState}, group : string, alignToTerrain : boolean)`  (call as `modelEmitter:emitModel(…) (method)`): Emits one extra model instance (e.g. a person on a seat) for this frame.
    - param `modelId`: Model id from `getModelId`.
    - param `transfMat`: Transformation of the emitted model.
    - param `animationStates`: Animations to play on the emitted model.
    - param `group`: Group (model part) the model is attached to, e.g. `seat.group`; `""` for none.
    - param `alignToTerrain`: Whether to align the model to the terrain.

    ```lua
    local a = api.type.AnimationState.new()
    a.animation = seat.animation
    a.param = params.currentInfo.world.gameTime
    a.loop = true
    a.startTime = 0
    modelEmitter:emitModel(modelId, seat.transf, {a}, seat.group, false)
    ```

    - used 6x in the base game, e.g. `base/content/scripts/scripts/transformator_util.tl:77`: `modelEmitter:emitModel(v.modelId, seat.transf, {a}, seat.group, false)`
- `emitModels : function(modelEmitter : ModelEmitter, modelIds : {integer}, gridSize : Vec2i, transfMat : Mat4f, distances : Vec2f, animationStates : {Transformator.AnimationState}, alignToTerrain : boolean)`

### Transformator.ParticleSystem (record)

```teal
record ParticleSystem
```

ParticleSystem of transformator

- `getModelIndices : function(particleSystem : ParticleSystem) : {integer}`
- `setLifeTime : function(particleSystem : ParticleSystem, index : integer, lifeTime : ParticleParamFloat)`: Sets the particle life time parameter of entry `index` (0-based, below `getSize()`) of the particle system.
    - param `index`: 0-based entry index.
- `getSize : function(particleSystem : ParticleSystem) : integer`  (call as `particleSystem:getSize(…) (method)`): Returns the number of entries of the particle system; the base scripts loop `for i = 0, particleSystem:getSize() - 1`.
    - used 4x in the base game, e.g. `base/content/buildings/transformator_townbuilding.script.tl:8`: `for i = 0, particleSystem:getSize() - 1 do`
- `setFrequencyScale : function(particleSystem : ParticleSystem, index : integer, frequencyScale : number)`  (call as `particleSystem:setFrequencyScale(…) (method)`): Scales the emission frequency of entry `index` (0-based, below `getSize()`) of the particle system; `0` stops emission.
    - param `index`: 0-based entry index.
    - param `frequencyScale`: Factor; the base scripts use `0.0` in tunnels or when an industry state does not match.
    - used 6x in the base game, e.g. `base/content/buildings/transformator_townbuilding.script.tl:9`: `particleSystem:setFrequencyScale(i, transformatorParams.entityId % 100 < 33 and 1.0 or 0.0)`
- `setLifeTimeScale : function(particleSystem : ParticleSystem, index : integer, lifeTimeScale : number)`  (call as `particleSystem:setLifeTimeScale(…) (method)`): Scales the particle life time of entry `index` (0-based, below `getSize()`) of the particle system.
    - param `index`: 0-based entry index.
    - used 1x in the base game, e.g. `base/content/scripts/scripts/transformator_util.tl:40`: `particleSystem:setLifeTimeScale(i, lifeTimeScale)`
- `setVelocityDampingFactor : function(particleSystem : ParticleSystem, index : integer, velocity : number)`  (call as `particleSystem:setVelocityDampingFactor(…) (method)`): Sets the velocity damping factor of entry `index` (0-based, below `getSize()`) of the particle system.
    - param `index`: 0-based entry index.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/fun_elements/firework/firework.script.tl:47`: `particleSystem:setVelocityDampingFactor(0, math_util.mapClamp(state.size or 0.5, 0, 1, 10, 40))`
- `setColorOffset : function(particleSystem : ParticleSystem, index : integer, colorOffset : Vec3f)`  (call as `particleSystem:setColorOffset(…) (method)`): Sets a color offset added to the particles of entry `index` (0-based, below `getSize()`) of the particle system.
    - param `index`: 0-based entry index.
    - param `colorOffset`: RGB offset; the base vehicle script scales it with maintenance.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/fun_elements/firework/firework.script.tl:44`: `particleSystem:setColorOffset(0, color)`
- `setVelocity : function(particleSystem : ParticleSystem, index : integer, velocity : Vec3f)`  (call as `particleSystem:setVelocity(…) (method)`): Sets the base velocity of the particles of entry `index` (0-based, below `getSize()`) of the particle system.
    - param `index`: 0-based entry index.
    - param `velocity`: Velocity vector; the base vehicle script passes the vehicle's velocity.
    - used 2x in the base game, e.g. `base/content/scripts/scripts/transformator_util.tl:41`: `particleSystem:setVelocity(i, velocity)`
- `setSizeScale01 : function(particleSystem : ParticleSystem, index : integer, scale0 : number, scale1 : number)`  (call as `particleSystem:setSizeScale01(…) (method)`): Sets two size scale factors of entry `index` (0-based, below `getSize()`) of the particle system.
    - param `index`: 0-based entry index.
    - notes: The base vehicle script uses `scale0 = max(powerOutput / power, 0.25)` and `scale1 = scale0 + maintenance`.
    - used 1x in the base game, e.g. `base/content/scripts/scripts/transformator_util.tl:38`: `particleSystem:setSizeScale01(i, sizeScale0, sizeScale1)`
- `setAlbedoTexture : function(particleSystem : ParticleSystem, index : integer, albedoTexture : string)`: Sets the albedo (color) texture of entry `index` (0-based, below `getSize()`) of the particle system.
    - param `index`: 0-based entry index.
    - param `albedoTexture`: Texture file.
- `setNormalMapTexture : function(particleSystem : ParticleSystem, index : integer, normalMapTexture : string)`: Sets the normal map texture of entry `index` (0-based, below `getSize()`) of the particle system.
    - param `index`: 0-based entry index.
    - param `normalMapTexture`: Texture file.
- `setNumFrames : function(particleSystem : ParticleSystem, index : integer, numFrames : Vec2i)`: Sets the number of animation frames (columns and rows of the texture atlas) of entry `index` (0-based, below `getSize()`) of the particle system.
    - param `index`: 0-based entry index.
- `setFrame : function(particleSystem : ParticleSystem, index : integer, frame : ParticleParamFloat)`: Sets the animation frame parameter of entry `index` (0-based, below `getSize()`) of the particle system.
    - param `index`: 0-based entry index.
- `setFrameProgressOverLifeTime : function(particleSystem : ParticleSystem, index : integer, frameProgressOverLifeTime : ParticleParamFloat)`: Sets the frame progress over the particle life time parameter of entry `index` (0-based, below `getSize()`) of the particle system.
    - param `index`: 0-based entry index.
- `setFrameSpeed : function(particleSystem : ParticleSystem, index : integer, frameSpeed : ParticleParamFloat)`: Sets the animation frame speed parameter of entry `index` (0-based, below `getSize()`) of the particle system.
    - param `index`: 0-based entry index.
- `setGravity : function(particleSystem : ParticleSystem, index : integer, gravity : ParticleParamFloat)`: Sets the gravity parameter of entry `index` (0-based, below `getSize()`) of the particle system.
    - param `index`: 0-based entry index.
- `setRotation : function(particleSystem : ParticleSystem, index : integer, rotation : ParticleParamFloat)`: Sets the rotation parameter of entry `index` (0-based, below `getSize()`) of the particle system.
    - param `index`: 0-based entry index.
- `setRotationByDirection : function(particleSystem : ParticleSystem, index : integer, rotationByDirection : ParticleParamFloat)`: Sets the rotation by movement direction parameter of entry `index` (0-based, below `getSize()`) of the particle system.
    - param `index`: 0-based entry index.
- `setRotationSpeed : function(particleSystem : ParticleSystem, index : integer, rotationSpeed : ParticleParamFloat)`: Sets the rotation speed parameter of entry `index` (0-based, below `getSize()`) of the particle system.
    - param `index`: 0-based entry index.
- `getParticleId : function(particleSystem : ParticleSystem, index : integer) : string`  (call as `particleSystem:getParticleId(…) (method)`): Returns the id of an entry; the base scripts match substrings of it (e.g. `"steam_chimney"`, `"diesel"`, `"|idle"`, `"|boost"`) to treat entries differently.
    - param `index`: 0-based entry index.
    - used 2x in the base game, e.g. `base/content/industries/shared/shared/transformator_industry.script.tl:60`: `local id = particleSystem:getParticleId(i)`

### Transformator.UserTransf (record)

```teal
record UserTransf
```

Wheel information for user transformations

- `transf : Mat4f`: Transformation matrix of the part.
- `type : integer`

### Transformator.StateInfo (record)

```teal
record StateInfo
```

Information about the state for animations

- `state : integer`
- `stateTime : integer`

### Transformator.DepotInfo (record)

```teal
record DepotInfo
```

Information about depot for animations

- `stateInfo : StateInfo`

### Transformator.SignalListInfo (record)

```teal
record SignalListInfo
```

Information about signals for animations

- `stateInfo : StateInfo`

### Transformator.TrafficLightInfo (record)

```teal
record TrafficLightInfo
```

Information about traffic lights for animations

- `stateInfo : StateInfo`

### Transformator.RailRoadCrossingInfo (record)

```teal
record RailRoadCrossingInfo
```

Information of rail road crossings for animations

- `stateInfo : StateInfo`
- `trafficDelay : integer`

### Transformator.ModelPersonInfo (record)

```teal
record ModelPersonInfo
```

Information about persons for animations

- `stateInfo : StateInfo`
- `movementType : integer`
- `distance : number`

### Transformator.ModelPersonIdleInfo (record)

```teal
record ModelPersonIdleInfo
```

Information of idle persons for animations

- `arriveTime : integer`

### Transformator.AnimalAnimationStateInfo (record)

```teal
record AnimalAnimationStateInfo
```

State information of animals for animations

- `movementTypeIndex : integer`
- `targetChangedElapsed : number`
- `instIndex : integer`

### Transformator.GroupInfo (record)

```teal
record GroupInfo
```

Information about group

- `children : {integer}`

### Transformator.BogieInfo (record)

```teal
record BogieInfo
```

Information about bogie

- `posMin : number`
- `posMax : number`
- `offset : number`
- `zOffset : number`
- `upright : boolean`

### Transformator.VehicleScriptingInfo (record)

```teal
record VehicleScriptingInfo
```

Vehicle Scripting Information

- `speed : number`: Current speed of the vehicle.
- `topSpeed : number`: Top speed of the vehicle.
- `speed01 : number`: Current speed normalised to 0..1; the base sound sets use it as the input of their sample curves.
- `powerOutput : number`: Current power output; `powerOutput / power` is the engine load used for smoke size and life time.
- `power : number`: Maximum power of the vehicle.
- `power01 : number`: Power output normalised to 0..1, used by the base sound sets.
- `cargoCapacityVehicle01 : number`: -1.0 if no capacity in entire vehicle
- `cargoCapacities01 : {number}`: -1.0 for each carriage with no capacity
- `loadState : Engine.Component.TransportVehicle.LoadState`: Loading state of the vehicle.
- `doorAnimationInfo : DoorAnimationInfo`: State of the door animation, see `transformator_util.addDoorAnimationState`.
- `doorsLeft : boolean`
- `carriageListLength : integer`: Number of carriages in the vehicle's carriage list.
- `positionInCarriageList : integer`: 0 index based
- `maintenance : number`: 0 is best, 1 is worst
- `velocity : Vec3f`: Velocity vector; the base scripts pass it to `ParticleSystem.setVelocity`.
- `direction : Vec3f`: Direction vector of the vehicle.
- `acceleration : number`: Current acceleration.
- `brakeDecel : number`: Current braking deceleration; the base sound scripts switch between acceleration and deceleration tracks at `1`.
- `inTunnel : boolean`: Whether the vehicle is in a tunnel; the base scripts stop smoke emission then.
- `timeStanding : number`: How long the vehicle has been standing still.
- `approachingStation : boolean`: Whether the vehicle is approaching a station.
- `indicesLoadConfig : {integer}`: User-transformation indices that the base scripts scale to zero (hide) for the current load configuration.
- `color : Vec3f`: Color of the vehicle (RGB); the base scripts contain commented-out code that writes it to the color instance attribute.

### Transformator.AircraftScriptingInfo (record)

```teal
record AircraftScriptingInfo
```

Aircraft Scripting Information

- `flightState : FlightState`: Current flight phase (taxi, take-off, flight, approach, landing).
- `gearTime : number`: Time of the last gear change, used (divided by 1000) as the start time of the gear animation.
- `gear : boolean`: Whether the landing gear is down; the base script plays `open_wheels` or `close_wheels` accordingly.
- `hasPropsBlurred : boolean`: Whether the model has blurred propeller parts (`props_blurred_on`/`props_on` animations).
- `gearDuration : number`: Duration of the gear animation.
- `bodyAngle : number`: Body angle of a helicopter; the base script rotates user transformation 1 by `-bodyAngle` around Y.
- `wheelAngleZ : number`: Steering angle of the nose wheels, applied around Z.
- `totalDist : number`: Distance rolled on the ground, used to rotate the wheels (`totalDist / radius`).
- `touchesGround : boolean`: Whether the aircraft is on the ground.
- `waiting : boolean`: Whether the aircraft is waiting.
- `velocity : Vec2f`: Velocity of the aircraft; its length drives the pitch animation of planes.
- `lastAccel : Vec2f`
- `pitch : number`: Current pitch; drives the `pitch` animation of helicopters.
- `pitchV : number`
- `roll : number`: Current roll; drives the `rudder` animation.
- `rollV : number`
- `thrust : number`: Current thrust; compared with `VehicleScriptingInfo.power` to decide on blurred propellers.
- `thrustV : number`
- `elevator : number`: Elevator deflection; drives the `elevator` animation.
- `aileronLeft : number`: Left aileron deflection; drives the `aileron_left` and `aileron_right` animations.
- `propPhase : number`: Rotation phase of the propellers in radians.
- `flaps : number`: Flap position, combined with `AircraftStaticScriptingInfo.flapsDuration` for the `flaps` animation.
- `alt : number`: Current altitude.

### Transformator.AircraftStaticScriptingInfo (record)

```teal
record AircraftStaticScriptingInfo
```

Aircraft Static Scripting Information

- `isHelicopter : boolean`: Whether the aircraft is a helicopter.
- `powerIdle : number`
- `hasFlaps : boolean`: Whether the model has a `flaps` animation.
- `flapsDuration : number`: Duration of the `flaps` animation.

### Transformator.ShipScriptingInfo (record)

```teal
record ShipScriptingInfo
```

Ship Scripting Information

- `rudderAngleZ : number`: Rudder angle in radians; drives the `rudder` animation.
- `taxiState : boolean`: Whether the ship is manoeuvring; the base scripts do not play the `drive` animation then.
- `lastAccel : number`
- `tractiveEffort : number`: Current tractive effort.
- `tractiveEffortV : number`
- `roll : number`
- `rollV : number`
- `powerOutput : number`: Current power output.
- `approachingStation : boolean`: Whether the ship is approaching a station.
- `paddlePhase : number`: Rotation phase of paddle wheels in radians; drives the `paddles` animation.
- `actualPos : PathPosData`: Current position of the ship on its path.

### Transformator.TramScriptingInfo (record)

```teal
record TramScriptingInfo
```

Tram Scripting Information

### Transformator.RailVehicleScriptingInfo (record)

```teal
record RailVehicleScriptingInfo
```

Rail Vehicle Scripting Information

- `axleRefWeight : number`
- `brakeDecelSmoothed : number`: Smoothed braking deceleration, used for the brake sound.
- `chuffStep : number`: Speed step per steam chuff, used for the chuff sound of steam engines.
- `curvature : number`: Curvature of the track at the vehicle.
- `curveSpeedLimit : number`: Speed limit of the current curve.
- `gameSpeedUp : number`: Current game speed factor; the clack sound gets quieter above 1 and 2.
- `maxSideForce : number`: Side force limit; the wheel squeal gets louder as `sideForce` approaches it.
- `numAxles : number`: Number of axles.
- `sideForce : number`: Current side force in curves, used for the wheel squeal sound.
- `weight : number`: Weight of the vehicle, used for the clack and chuff sounds.

### Transformator.RoadVehicleScriptingInfo (record)

```teal
record RoadVehicleScriptingInfo
```

Road Vehicle Scripting Information

- `wheelRotation : number`: Steering angle of the wheels, applied to the steering parts around Z.

### Transformator.LandVehicleScriptingInfo (record)

```teal
record LandVehicleScriptingInfo
```

Land Vehicle Scripting Information

- `wheelAnimationInfo : WheelAnimationInfo`: Distance and wheel data for the drive and wheel animations.
- `leftStationSince : number`: Time since the vehicle left a station; the base road and tram scripts blink for a while after leaving (`< 5.0`).
- `reversed : boolean`: Whether the vehicle drives reversed; swaps front and back animations and the blinker sides.
- `carriagesReversed : {boolean}`: Per carriage, whether it is reversed.
- `side : {boolean, boolean}`: Pair of flags: whether this carriage is the first (`side[1]`) and the last (`side[2]`) of the train; drives the `front_*`, `back_*`, `has_previous_*` and `has_next_*` animations.
- `brakingTimer : number`: Braking timer; brake lights are on while it is above 0.
- `turnLeft : boolean`: Whether the left turn signal is on.
- `turnRight : boolean`: Whether the right turn signal is on.

### Transformator.AirWaterVehicleScriptingInfo (record)

```teal
record AirWaterVehicleScriptingInfo
```

Air Water Vehicle Scripting Information

### Transformator.IndustryScriptingInfo (record)

```teal
record IndustryScriptingInfo
```

Industry Scripting Information

- `boosted01 : number`
- `doubleBoosted01 : number`
- `idle01 : number`
- `level : number`
- `production01 : number`
- `workplaces : number`: Workplace level of the industry; the base scripts treat values above `0.7` as high production.
    - notes: The base industry script picks the animation `boost` (producing, boosting and `workplaces > 0.7`), `prod_high` (producing and boosting, or `workplaces > 0.7`), `prod_low` (producing) or `idle`/`stopped`. The same tags select particle entries (`|boost`, `|prod_high`, ...) and emitted lane models.
- `progress : {number}`
- `producing : boolean`: Whether the industry is producing; otherwise the base script plays `idle` and `stopped`.
- `boosting : boolean`: Whether production is boosted.

### Transformator.StationScriptingInfo (record)

```teal
record StationScriptingInfo
```

Station Scripting Information

- `cargo : number`
- `cargo01 : number`
- `crowd : number`
- `crowd01 : number`
- `terminals : number`

### Transformator.TownBuildingScriptingInfo (record)

```teal
record TownBuildingScriptingInfo
```

Town Building Scripting Information

- `underConstruction : number`
- `scaffold : boolean`

### Transformator.ModelMetadataInfo (record)

```teal
record ModelMetadataInfo
```

Model metadata available to transformator scripts.

- `seats : {Seat}`: Seats of the model, from the `seatProvider.seats` metadata of the `.mdl` file.

#### Transformator.ModelMetadataInfo.Seat (record)

```teal
record Seat
```

A seat of the model, where a person model can be emitted.

- `transf : Mat4f`: Transformation of the seat.
- `animation : string`: Animation played by a person on this seat, e.g. `"driving_upright"`.
- `group : string`: Model group (part) the seat belongs to.
- `instanceIndex : number`: Model instance the seat belongs to; the base scripts only emit persons for seats whose index equals `instIndex`.
- `crew : boolean`: Whether the seat is for crew rather than passengers.

### Transformator.WheelAnimationInfo (record)

```teal
record WheelAnimationInfo
```

Information about wheel animation

- `totalDist : number`: Total distance travelled; the `drive` animation runs at `1000 * totalDist` and the wheels rotate by `totalDist / radius`.
- `drivingWheelRadius : number`: Radius of the driving wheels; the `wheels` animation is only played when it is above 0.
- `wheelDuration : number`: Duration of the `wheels` animation.

### Transformator.DoorAnimationInfo (record)

```teal
record DoorAnimationInfo
```

Information for door animation

- `name : string`: Name of the door animation to play.
- `doorTime : number`: Start time of the door animation; when negative, the animation is shown at its end (`duration`).
- `duration : number`: Duration of the door animation.

### Transformator.SimPersonInfo (record)

```teal
record SimPersonInfo
```

Information about town building simulation persons for animations

- `entity : Engine.Entity`: Entity of the person.
- `modelId : integer`: Model id of the person, passed to `ModelEmitter.emitModel`.

### Transformator.PersonCapacityInfo (record)

```teal
record PersonCapacityInfo
```

Information about person capacity for animations

- `simPersons : {SimPersonInfo}`: Persons currently in the model; the base scripts emit one person model per seat.

### Transformator.ModelInfo (record)

```teal
record ModelInfo
```

Information about a model

- `axles : {WheelInfo}`: Axles of an aircraft, rotated around Y while it rolls on the ground.
- `wheels : {WheelInfo}`: Wheels of the model.
- `wheelSigns : {Vec2f}`: Per wheel, sign factors for steering (`x`) and rolling (`y`).
- `steeringParts : {integer}`: User-transformation indices of steering parts; when present, the wheels only roll and the steering parts are rotated.

#### Transformator.ModelInfo.WheelInfo (record)

```teal
record WheelInfo
```

A wheel or axle of the model.

- `index : integer`: User-transformation index of the wheel, for `TransfOutput.setUserTransf`.
- `x : number`
- `radius : number`: Wheel radius; rolling angle is `distance / radius`.
- `z : number`
- `animated : boolean`

### Transformator.VehicleStaticScriptingInfo (record)

```teal
record VehicleStaticScriptingInfo
```

Vehicle Static Scripting Information

- `carriageEntity : Engine.Entity`: entity of the individual member of the carriage list
- `modelInfo : ModelInfo`: Wheel and steering data of the model.
- `purchaseTime : integer`: Time the vehicle was purchased.

### Transformator.Calendar (record)

```teal
record Calendar
```

Calendar Information

- `year : integer`: Year.
- `month : integer`: Month.
- `day : integer`: Day of the month.

### Transformator.WallClock (record)

```teal
record WallClock
```

Wall Clock Information

- `hour : integer`: Hour.
- `minute : integer`: Minute.
- `second : integer`: Second.

### Transformator.WorldScriptingInfo (record)

```teal
record WorldScriptingInfo
```

World Scripting Information

- `cloudCoverage : number`: Current cloud coverage, from 0 to 1.
- `gameTime : integer`: Current game time in milliseconds; used as the time base for looping animations.
- `timeOfDaySec : integer`: Time of day in seconds.
- `calendar : Calendar`: Current in-game date.
- `wallClock : WallClock`: Current in-game time of day.

### Transformator.VehicleEdgeInfo (record)

```teal
record VehicleEdgeInfo
```

Info containing values for the edge at the requested position, some values are only valid at that exact point, others for the entire edge

- `offset : number`
- `offsetClampedToPathEnd : number`: The requested offset, limited to the end of the vehicle's path.
- `width : number`: 0 for rail tracks
- `curvature : number`: Curvature of the path at the requested position.
- `slope : number`: 0 to 1.0 at 45 degrees slope
- `speedLimit : number`: Speed limit at the requested position.
- `curveSpeedLimit : number`: Curve speed limit at the requested position.
- `hasElectricTrack : boolean`: Whether the track or street is electrified.
- `catenaryHeight : number`: Height of the catenary.

### Transformator.EdgeObjectInfo (record)

```teal
record EdgeObjectInfo
```

An object on an edge: a stop or a signal.

- `type : integer`: 0 stop left, 1 stop right, 2 signal

### Transformator.EdgeDecorationInfo (record)

```teal
record EdgeDecorationInfo
```

A decoration on an edge.

- `categories : {string}`: Categories of the decoration.

### Transformator.BaseEdgeInfo (record)

```teal
record BaseEdgeInfo
```

Information related to one specified base edge or base node

- `edgeOffset : integer`
- `edgeOffsetClampedToEdgesCount : integer`: < 0 if not a valid BaseEdgeInfo result
- `length : number`: Length of the edge.
- `onBaseEdge : boolean`: otherwise on BaseNode (crossing, ...) or construction (switch, depot, ...)
- `onBaseNode : boolean`: Whether the position is on a base node.
- `roadType : integer`: 0 track, 1 street
- `baseEdgeType : integer`: 0 normal, 1 bridge, 2 tunnel
- `typeIndex : integer`: bridge/tunnel type
- `edgeObjectInfos : {EdgeObjectInfo}`: Stops and signals on the edge.
- `edgeDecorationInfos : {EdgeDecorationInfo}`: Decorations on the edge.
- `catenaryHeight : number`: Height of the catenary.
- `railBase : number`
- `railHeight : number`
- `railWidth : number`
- `streetCategories : {string}`: Categories of the street.

### Transformator.LandVehicleScriptingApi (record)

```teal
record LandVehicleScriptingApi
```

Mini Api to request data related to land vehicles

- `getVehicleEdgeInfo : function(landVehicleScriptingApi : LandVehicleScriptingApi, offset : number) : VehicleEdgeInfo`: Returns path information at a position relative to the vehicle.
    - param `offset`: Distance along the path from the vehicle.
- `getBaseEdgeInfo : function(landVehicleScriptingApi : LandVehicleScriptingApi, edgeIndexOffset : integer) : BaseEdgeInfo`: Returns information about a base edge or base node of the vehicle's path.
    - param `edgeIndexOffset`: Index offset of the edge relative to the vehicle's current edge.
    - returns: Edge information; `edgeOffsetClampedToEdgesCount < 0` means the result is not valid.
- `getAverageCurvature : function(landVehicleScriptingApi : LandVehicleScriptingApi, distanceFront : number, distanceBack : number) : number`  (call as `landVehicleScriptingApi:getAverageCurvature(…) (method)`): Returns the average curvature of the path around the vehicle.
    - param `distanceFront`: Distance to sample ahead of the vehicle.
    - param `distanceBack`: Distance to sample behind the vehicle.
    - notes: The tilting train script calls it with 80 for both distances and derives the `tilt` animation from `speed * speed * curvature`.
    - used 1x in the base game, e.g. `base/content/scripts/scripts/transformator_util.tl:531`: `local avgCurvature = landVehicleApi:getAverageCurvature(distance, distance)`
- `getAverageSlope : function(landVehicleScriptingApi : LandVehicleScriptingApi, distanceFront : number, distanceBack : number) : number`: Returns the average slope of the path around the vehicle (0 to 1, 1 at 45 degrees, as in `VehicleEdgeInfo.slope`).
    - param `distanceFront`: Distance to sample ahead of the vehicle.
    - param `distanceBack`: Distance to sample behind the vehicle.
- `getAverageSpeedLimits : function(landVehicleScriptingApi : LandVehicleScriptingApi, distanceFront : number, distanceBack : number) : {number, number}`: speedLimit, curveSpeedLimit
- `getElectrificationChange : function(landVehicleScriptingApi : LandVehicleScriptingApi, distanceFront : number) : {number, number}`: distance to next change, change tendency - < 0 changes to non-electrified, > 0 changes to electrified, == 0 no change
- `getAverageCatenaryHeight : function(landVehicleScriptingApi : LandVehicleScriptingApi, distanceFront : number, distanceBack : number) : number`  (call as `landVehicleScriptingApi:getAverageCatenaryHeight(…) (method)`): Returns the average catenary height around the vehicle.
    - param `distanceFront`: Front end of the sampled range.
    - param `distanceBack`: Back end of the sampled range.
    - notes: The tram script maps the result between `pantographMin` and `pantographMax` to the frame of the `pantograph` animation.
    - used 1x in the base game, e.g. `base/content/scripts/scripts/transformator_util.tl:443`: `local catenaryHeight = landVehicleApi:getAverageCatenaryHeight(distanceFront, distanceBack)`

### Transformator.AnimationState (record)

```teal
record AnimationState
```
Access: `api.type.AnimationState`

An animation to play on an emitted model; create it with `api.type.AnimationState.new()` and pass it to `ModelEmitter.emitModel`.

- `new : function() : AnimationState`  (call as `api.type.AnimationState.new(…)`): Creates a new, empty `AnimationState`.
    - used 5x in the base game, e.g. `base/content/scripts/scripts/transformator_util.tl:72`: `local a = api.type.AnimationState.new()`
- `animation : string`: Name of the animation.
- `loop : boolean`: Whether the animation loops.
- `param : integer`: Animation time; the base scripts set it to the current game time.
- `reversed : boolean`: Whether the animation plays backwards.
- `startTime : integer`: Start time of the animation.

### Transformator.TransfOutput (record)

```teal
record TransfOutput
```

Transformation output, which contains user transformations and animations of transformation lua scripts

- `getUserTransfs : function(transfOutput : TransfOutput) : {Transformator.UserTransf}`: Returns the user transformations set so far.
- `setUserTransf : function(transfOutput : TransfOutput, index : integer, transfMat : Mat4f, absolute? : boolean)`  (call as `transfOutput:setUserTransf(…) (method)`): Sets the transformation of a model part (user transformation).
    - param `index`: User-transformation index, e.g. `WheelInfo.index`, a steering part or an entry of `indicesLoadConfig`.
    - param `transfMat`: Transformation; `api.type.Mat4f.scale(api.type.Vec3f.new(0, 0, 0))` hides the part.

    ```lua
    -- hide part 0
    transfsOutput:setUserTransf(0, api.type.Mat4f.scale(api.type.Vec3f.new(0)))
    ```

    - used 13x in the base game, e.g. `base/content/industries/shared/shared/transformator_industry.script.tl:26`: `transfsOutput:setUserTransf(0, api.type.Mat4f.scale(api.type.Vec3f.new(0)))`
- `getAnimationStates : function(transfOutput : TransfOutput) : {AnimationState}`: Returns the animation states added so far.
- `addAnimationState : function(transfOutput : TransfOutput, animation : string, startTime : number, param : number, loop : boolean, reversed : boolean)`  (call as `transfOutput:addAnimationState(…) (method)`): Sets a specific animation to a specific frame
    - param `transfOutput`: Handle to self
    - param `animation`: Name of the animation to play
    - param `startTime`: If non-negative, will be used as an offset into global time, i.e. param = gameTime - startTime
    - param `param`: Only used if startTime is < 0, in  which case is the time (in milliseconds) of the animation frame
    - param `loop`: Whether the animation should loop around or should stop at the end
    - param `reversed`: Whether the animation has to loop backwards
    - used 47x in the base game, e.g. `base/content/industries/shared/shared/transformator_industry.script.tl:16`: `transfsOutput:addAnimationState(animation, -1, startTime, true, false)`
- `triggerAnimation : function(transfOutput : TransfOutput, animation : string, startTime? : integer, endTime? : integer)`  (call as `transfOutput:triggerAnimation(…) (method)`): Start an animation for the running in the given interval (and then gets removed)
    - param `transfOutput`: Handle to self
    - param `animation`: Name of the animation to play
    - param `startTime`: Initial keyframe (in milliseconds) of the animation, or nil for beginning of the animation
    - param `endTime`: Final keyframe (in milliseconds) of the animation, or nil to run until animation end
    - used 1x in the base game, e.g. `dlcs/urbangames_deluxe_upgrade_pack/content/vehicle/vehicle/plane/concorde/transformator_concorde.script.tl:87`: `transfsOutput:triggerAnimation("nose_up", timeNosePrev, timeNose) -- NOTE overrides animation state`
- `setModelInstanceAttributeVec4f : function(transfOutput : TransfOutput, position : integer, value : Vec4f)`: Sets custom values for instance attributes of the model instances as Vec4f
Used to set colors
    - param `transfOutput`: Handle to self
    - param `position`: Position of the instance attribute (see below for builtin positions)
    - param `value`: Value to set the attribute to
- `setModelInstanceAttributeVec3f : function(transfOutput : TransfOutput, position : integer, value : Vec3f)`  (call as `transfOutput:setModelInstanceAttributeVec3f(…) (method)`): Sets custom values for instance attributes of the model instances as Vec3f
    - param `transfOutput`: Handle to self
    - param `position`: Position of the instance attribute (see below for builtin positions)
    - param `value`: Value to set the attribute to
    - used 1x in the base game, e.g. `base/content/vehicle/zeppelin/shared/shared/transformator_zeppelin.script.tl:66`: `transfsOutput:setModelInstanceAttributeVec3f(transformator_util.colorAttributePostition, color)`
- `setModelInstanceAttributeVec2f : function(transfOutput : TransfOutput, position : integer, value : Vec2f)`: Sets custom values for instance attributes of the model instances as Vec2f
    - param `transfOutput`: Handle to self
    - param `position`: Position of the instance attribute (see below for builtin positions)
    - param `value`: Value to set the attribute to
- `setModelInstanceAttributeFloat : function(transfOutput : TransfOutput, position : integer, value : number)`  (call as `transfOutput:setModelInstanceAttributeFloat(…) (method)`): Sets custom values for instance attributes of the model instances as floats
Used to set age values
    - param `transfOutput`: Handle to self
    - param `position`: Position of the instance attribute (see below for builtin positions)
    - param `value`: Value to set the attribute to
    - used 1x in the base game, e.g. `base/content/vehicle/zeppelin/shared/shared/transformator_zeppelin.script.tl:65`: `transfsOutput:setModelInstanceAttributeFloat(transformator_util.ageAttributePostition, 0)`
