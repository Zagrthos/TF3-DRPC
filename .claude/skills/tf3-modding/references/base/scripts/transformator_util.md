# scripts.transformator_util

Definition file: `base/tealdef/scripts/transformator_util.d.tl` (in the game folder).

Types for transformator scripts, the per-frame scripts that animate vehicle and building models
(wheels, doors, lights, pantographs, emitted person models, particles). `TransformatorParams` is what
an update function receives; `TransformatorUtil` (`/scripts/transformator_util.tl`) holds the shared
animation helpers the base game's transformators call.

## Globals

- `UpdateFn : function(captureParams : any, transformatorParams : TransformatorParams, transfsOutput : Transformator.TransfOutput)`: Signature of a transformator update function: receives the capture parameters, the
`TransformatorParams` and the output object, on which it sets animation states and user transforms.
- `UpdateParticleSystemFn : function(captureParams : any, transformatorParams : TransformatorParams, Transformator.ParticleSystem)`: Signature of a transformator particle update function (see `TransformatorUtil.updateParticleSystemFn`).
- `GetEmittableModelsFn : function(captureParams : any, transformatorParams : TransformatorParams) : {string}`: Signature of a function that lists the model files a transformator may emit.
- `ComputeEmittedModelsFn : function(captureParams : any, transformatorParams : TransformatorParams, modelEmitter : Transformator.ModelEmitter)`: Signature of a function that emits models through the `ModelEmitter` in each update.

## TransformatorParams (record)

```teal
global record TransformatorParams
```

Everything a transformator update function receives about the model instance it animates: current and
previous dynamic info, static vehicle info, model metadata and the `params` table from the model's
`transformatorConfig`.

- `previousInfo : DynamicInfo`: Dynamic info of the previous update.
- `currentInfo : DynamicInfo`: Dynamic info of the current update.
- `landVehicleApi : Transformator.LandVehicleScriptingApi`: Queries along the vehicle's path, e.g. `getAverageCatenaryHeight` and `getAverageCurvature`.
- `vehicleStaticInfo : Transformator.VehicleStaticScriptingInfo`: Static (non-changing) info of the vehicle.
- `aircraftStaticInfo : Transformator.AircraftStaticScriptingInfo`: Static info of the aircraft.
- `modelMetadataInfo : Transformator.ModelMetadataInfo`: Model metadata available to the transformator, e.g. `seats`.
- `entityId : Engine.Entity`: Entity id of the model instance.
- `instIndex : integer`: Index of the model instance within the entity; compared with `seat.instanceIndex`.
- `transformatorConfigParams : any`: The `transformatorConfig.params` table from the model's metadata (e.g. `ModelParamsTram`).

### TransformatorParams.TransformatorLanes (record)

```teal
record TransformatorLanes
```

`transformatorConfig.params` layout for town buildings that emit moving models along curves; read by
`TransformatorUtil.getEmittableModelsTownbuildingFn` and `computeEmittedModelsTownbuildingFn`.

- `lanes : {Curve}`: The lanes.

#### TransformatorParams.TransformatorLanes.Curve (record)

```teal
record Curve
```

One lane of emitted models.

- `curves : ConstructionUtilSerialized.GeneratedDataGroup.GeneratedCurve`: Curves (start point, tangents, end point) along which the models move.
- `speed : number`: Travel time per metre of curve; one pass along a curve takes `speed * length` game-time units.
- `density : number`: Number of emitted models per metre of curve.
- `model : string`: Model file to emit.
- `tags : string`: Optional industry-state filter: contains `idle`, `prod_low`, `prod_high` and/or `boost`; the lane is
shown only in the matching production state.

### TransformatorParams.ModelParamsHelicopter (record)

```teal
record ModelParamsHelicopter
```

`transformatorConfig.params` of helicopters.

- `vtol : boolean`: True for aircraft that keep rotor pitch up at low horizontal speed (vertical take-off).

### TransformatorParams.ModelParamsTram (record)

```teal
record ModelParamsTram
```

`transformatorConfig.params` of trams and other vehicles with a pantograph.

- `pantographMin : number`: Catenary height (metres) at which the pantograph animation is at its first frame.
- `pantographMax : number`: Catenary height (metres) at which the pantograph animation is at its last frame.
- `pantographOffsetFromCenter : number`: Distance of the pantograph from the vehicle centre in driving direction, in metres.

### TransformatorParams.ModelParamsZeppelin (record)

```teal
record ModelParamsZeppelin
```

`transformatorConfig.params` of zeppelins: models emitted on the crew and passenger seats.

- `crewPath : string`: Model file emitted on seats marked as crew.
- `passengerPathsA : {string}`: Passenger model files used in era A.
- `passengerPathsB : {string}`: Passenger model files used in era B.
- `passengerPathsC : {string}`: Passenger model files used in era C (and as fallback).

### TransformatorParams.DynamicInfo (record)

```teal
record DynamicInfo
```

Dynamic Info that will update every render step

- `world : Transformator.WorldScriptingInfo`: World info such as `gameTime`.
- `vehicle : Transformator.VehicleScriptingInfo`: Info of any vehicle (speed, velocity, doors, maintenance, ...); nil for non-vehicles.
- `roadVehicle : Transformator.RoadVehicleScriptingInfo`: Road vehicle info; nil for other models.
- `railVehicle : Transformator.RailVehicleScriptingInfo`: Rail vehicle info (weight, axles, brake deceleration, ...); nil for other models.
- `tram : Transformator.TramScriptingInfo`: Tram info; nil for other models.
- `landVehicle : Transformator.LandVehicleScriptingInfo`: Info shared by land vehicles (reversed, side, wheel animation, braking, turn signals).
- `aircraft : Transformator.AircraftScriptingInfo`: Aircraft info; nil for other models.
- `ship : Transformator.ShipScriptingInfo`: Ship info; nil for other models.
- `airWaterVehicle : Transformator.AirWaterVehicleScriptingInfo`: Info shared by aircraft and ships.
- `industry : Transformator.IndustryScriptingInfo`: Industry info (producing, boosting, workplaces) for industry buildings.
- `station : Transformator.StationScriptingInfo`: Station info for station models.
- `townBuilding : Transformator.TownBuildingScriptingInfo`: Town building info for town buildings.
- `personCapacity : Transformator.PersonCapacityInfo`: Persons currently in the model (`simPersons`), used to emit person models on seats.
- `modelPerson : any`
- `modelPersonIdle : any`
- `customState : any`: Script-defined custom state of the entity (the zeppelin transformator reads its era from it).

## TransformatorUtil (record)

```teal
global record TransformatorUtil
```
Load: `local transformator_util = ug_require "::/scripts/transformator_util.tl" as TransformatorUtil`

Shared transformator helpers returned by `ug_require "::/scripts/transformator_util.tl"`.

- `updateParticleSystemFn : UpdateParticleSystemFn`: Default particle update for vehicles: scales smoke size, life time and frequency with power output and
maintenance, tints particles by type (`steam_chimney`, `cylinder`, `diesel`) and stops emission in tunnels.
    - used 10x in the base game, e.g. `base/content/vehicle/shared/shared/transformator_air.script.tl:96`: `updateParticleSystemFn = transformator_util.updateParticleSystemFn`
- `getEmittableModelsTownbuildingFn : GetEmittableModelsFn`: Returns the `model` of every lane in `TransformatorLanes`, so the engine can load them.
    - used 3x in the base game, e.g. `base/content/buildings/transformator_townbuilding.script.tl:25`: `getEmittableModelsFn = transformatorutil.getEmittableModelsTownbuildingFn,`
- `computeEmittedModelsTownbuildingFn : ComputeEmittedModelsFn`: Emits person models on the model's seats for the persons in `personCapacity`, and moving models along
the `TransformatorLanes` curves (filtered by industry state via `tags`).
    - used 3x in the base game, e.g. `base/content/buildings/transformator_townbuilding.script.tl:26`: `computeEmittedModelsFn = transformatorutil.computeEmittedModelsTownbuildingFn,`
- `getEntityTime : function(gameTime : number, entityId : Engine.Entity) : number`  (call as `transformator_util.getEntityTime(…)`): Per-entity time offset: `gameTime + entityId`, so identical models do not animate in sync.
    - used 9x in the base game, e.g. `base/content/industries/shared/shared/transformator_industry.script.tl:20`: `local time = transformator_util.getEntityTime(transformatorParams.currentInfo.world.gameTime, transformatorParams.entityId)`
- `stableRandomColor : function(entityId : Engine.Entity) : Vec3f`  (call as `transformator_util.stableRandomColor(…)`): Pseudo-random colour derived from the entity id; the same entity always gets the same colour.
    - used 1x in the base game, e.g. `base/content/vehicle/zeppelin/shared/shared/transformator_zeppelin.script.tl:64`: `local color = transformator_util.stableRandomColor(blimpData.blimpEntity.entity)`
- `scaleUserTransfIndicesLoadConfig : function(indicesLoadConfig : {integer}, transfsOutput : Transformator.TransfOutput)`  (call as `transformator_util.scaleUserTransfIndicesLoadConfig(…)`): Hides the model parts with the given user-transform indices by scaling them to zero.
    - param `indicesLoadConfig`: Indices of the parts to hide (the vehicle's `indicesLoadConfig`); nil does nothing.
    - used 8x in the base game, e.g. `base/content/vehicle/shared/shared/transformator_air.script.tl:82`: `transformator_util.scaleUserTransfIndicesLoadConfig(`
- `setWheelUserTransf : function(modelInfo : Transformator.ModelInfo, wheelRotation : number, totalDist : number, transfsOutput : Transformator.TransfOutput)`  (call as `transformator_util.setWheelUserTransf(…)`): Sets user transforms that rotate the wheels by the distance driven and steer them by `wheelRotation`.
    - param `modelInfo`: Wheel, steering part and wheel sign info of the model.
    - param `wheelRotation`: Steering angle in radians.
    - param `totalDist`: Total distance driven, in metres.
    - used 1x in the base game, e.g. `base/content/vehicle/shared/shared/transformator_road.script.tl:45`: `transformator_util.setWheelUserTransf(`
- `addDoorAnimationState : function(doorAnimationInfo : Transformator.DoorAnimationInfo, transfsOutput : Transformator.TransfOutput)`  (call as `transformator_util.addDoorAnimationState(…)`): Adds the door animation described by `doorAnimationInfo` (name, door time, duration); nil does nothing.
    - used 8x in the base game, e.g. `base/content/vehicle/shared/shared/transformator_air.script.tl:39`: `transformator_util.addDoorAnimationState(`
- `addBrakeLightsAnimationState : function(brakingTimer : number, time : number, transfsOutput : Transformator.TransfOutput)`  (call as `transformator_util.addBrakeLightsAnimationState(…)`): Plays `brake_lights_on` while `brakingTimer` is positive, otherwise `brake_lights_off`.
    - used 4x in the base game, e.g. `base/content/vehicle/shared/shared/transformator_road.script.tl:25`: `transformator_util.addBrakeLightsAnimationState(`
- `addAnimationStatesRailVehicles : function(side : {boolean}, reversed : boolean, transfsOutput : Transformator.TransfOutput)`  (call as `transformator_util.addAnimationStatesRailVehicles(…)`): Sets the on/off animations that depend on the vehicle's position in the train and direction:
`front_/back_/inner_/single_` `forward_/backward_parts_on|off`, `forward_parts_*`, `backward_parts_*`,
`has_previous_*`, `has_no_previous_*`, `has_next_*` and `has_no_next_*`.
    - param `side`: `{isFront, isBack}`: whether the vehicle is at the front and/or back end of the train.
    - param `reversed`: True if the vehicle is driving backwards.
    - used 3x in the base game, e.g. `base/content/vehicle/train/shared/shared/transformator_tiltingTrain.script.tl:14`: `transformator_util.addAnimationStatesRailVehicles(`
- `addDriveAnimationState : function(totalDist : number, reversed : boolean, transfsOutput : Transformator.TransfOutput)`  (call as `transformator_util.addDriveAnimationState(…)`): Adds the looping `drive` animation driven by distance (1 metre = 1000 animation units).
    - used 6x in the base game, e.g. `base/content/vehicle/shared/shared/transformator_road.script.tl:14`: `transformator_util.addDriveAnimationState(`
- `addBlinkLightsAnimationState : function(turnLeft : boolean, turnRight : boolean, reversed : boolean, leftStationSince : number, time : number, transfsOutput : Transformator.TransfOutput)`  (call as `transformator_util.addBlinkLightsAnimationState(…)`): Turn-signal animations `blink_lights_left_on/off` and `blink_lights_right_on/off` (swapped when
reversed); the left signal also blinks for 5 time units after leaving a station.
    - used 2x in the base game, e.g. `base/content/vehicle/shared/shared/transformator_road.script.tl:31`: `transformator_util.addBlinkLightsAnimationState(`
- `addPantographAnimationState : function(landVehicleApi : Transformator.LandVehicleScriptingApi, pantographMin : number, pantographMax : number, pantographOffsetFromCenter : number, reversed : boolean, distance : number, transfsOutput : Transformator.TransfOutput)`  (call as `transformator_util.addPantographAnimationState(…)`): Sets the `pantograph` animation frame from the average catenary height around the pantograph position.
    - param `landVehicleApi`: Path query API of the vehicle.
    - param `pantographMin`: Catenary height for the first frame.
    - param `pantographMax`: Catenary height for the last frame.
    - param `pantographOffsetFromCenter`: Pantograph position from the vehicle centre in driving direction, in metres.
    - param `reversed`: True if the vehicle is driving backwards.
    - param `distance`: Half-length of the sampled stretch (applied in both directions), in metres; the tram transformator uses 15.
    - used 1x in the base game, e.g. `base/content/vehicle/tram/shared/shared/transformator_tram.script.tl:48`: `transformator_util.addPantographAnimationState(`
- `addDrivingWheelAnimationState : function(drivingWheelRadius : number, totalDist : number, wheelDuration : number, reversed : boolean, transfsOutput : Transformator.TransfOutput)`  (call as `transformator_util.addDrivingWheelAnimationState(…)`): Adds the looping `wheels` animation for driving wheels (steam locomotives), synchronised to the distance driven.
    - param `drivingWheelRadius`: Radius of the driving wheel in metres; nothing happens if 0 or less.
    - param `totalDist`: Total distance driven in metres.
    - param `wheelDuration`: Length of one wheel turn in the animation.
    - used 2x in the base game, e.g. `base/content/vehicle/train/shared/shared/transformator_tiltingTrain.script.tl:26`: `transformator_util.addDrivingWheelAnimationState(`
- `addPaddleAnimationState : function(paddlePhase : number, transfsOutput : Transformator.TransfOutput)`  (call as `transformator_util.addPaddleAnimationState(…)`): Adds the looping `paddles` animation of paddle steamers from the paddle phase (radians).
    - used 2x in the base game, e.g. `base/content/vehicle/ship/shared/shared/transformator_hovercraft.script.tl:22`: `transformator_util.addPaddleAnimationState(`
- `addRudderAnimationState : function(rudderAngleZ : number, transfsOutput : Transformator.TransfOutput)`  (call as `transformator_util.addRudderAnimationState(…)`): Sets the `rudder` animation from the rudder angle (radians; 0 is the middle frame).
    - used 2x in the base game, e.g. `base/content/vehicle/ship/shared/shared/transformator_hovercraft.script.tl:27`: `transformator_util.addRudderAnimationState(`
- `addHelicopterAnimationState : function(pitchDyn : number, velocityX : number, touchesGround : boolean, bodyAngle : number, vtol : boolean, transfsOutput : Transformator.TransfOutput)`  (call as `transformator_util.addHelicopterAnimationState(…)`): Sets the rotor `pitch` animation and tilts the body (user transform 1) by `bodyAngle`.
    - used 1x in the base game, e.g. `base/content/vehicle/shared/shared/transformator_air.script.tl:15`: `transformator_util.addHelicopterAnimationState(`
- `addPitchAnimationState : function(velocity : Vec2f, topSpeed : number, transfsOutput : Transformator.TransfOutput)`  (call as `transformator_util.addPitchAnimationState(…)`): Sets the `pitch` animation from the current speed relative to 75 % of the top speed.
    - used 2x in the base game, e.g. `base/content/vehicle/shared/shared/transformator_air.script.tl:24`: `transformator_util.addPitchAnimationState(`
- `addFlapsAnimation : function(flapsDuration : number, flaps : number, transfsOutput : Transformator.TransfOutput)`  (call as `transformator_util.addFlapsAnimation(…)`): Sets the `flaps` animation position from the flap setting.
    - used 2x in the base game, e.g. `base/content/vehicle/shared/shared/transformator_air.script.tl:32`: `transformator_util.addFlapsAnimation(`
- `addGearAnimation : function(gear : boolean, gearTime : number, gearDuration : number, transfsOutput : Transformator.TransfOutput)`  (call as `transformator_util.addGearAnimation(…)`): Plays `open_wheels` or `close_wheels` for the landing gear.
    - used 2x in the base game, e.g. `base/content/vehicle/shared/shared/transformator_air.script.tl:44`: `transformator_util.addGearAnimation(`
- `addAircraftControlSurfacesAnimation : function(aileronLeft : number, elevator : number, roll : number, transfsOutput : Transformator.TransfOutput)`  (call as `transformator_util.addAircraftControlSurfacesAnimation(…)`): Sets the `aileron_left`, `aileron_right`, `elevator` and `rudder` animations from the control angles (radians).
    - used 3x in the base game, e.g. `base/content/vehicle/shared/shared/transformator_air.script.tl:51`: `transformator_util.addAircraftControlSurfacesAnimation(`
- `addAircraftLightsAnimation : function(gear : boolean, gearTime : number, gameTime : number, time : number, transfsOutput : Transformator.TransfOutput)`  (call as `transformator_util.addAircraftLightsAnimation(…)`): Plays `beacon_lights` and `strobe_lights` and switches `landing_light_on/off` with the gear.
    - used 3x in the base game, e.g. `base/content/vehicle/shared/shared/transformator_air.script.tl:58`: `transformator_util.addAircraftLightsAnimation(`
- `addBlurredPropsAnimation : function(propPhase : number, hasPropsBlurred : boolean, thrust : number, maxThrust : number, transfsOutput : Transformator.TransfOutput)`  (call as `transformator_util.addBlurredPropsAnimation(…)`): Switches between `props_on` and `props_blurred_on` (and the matching `_off` states) depending on thrust;
blurred props are used above 5 % of maximum thrust if the model has them.
    - used 3x in the base game, e.g. `base/content/vehicle/shared/shared/transformator_air.script.tl:66`: `transformator_util.addBlurredPropsAnimation(`
- `addAircraftWheelTransformation : function(modelInfo : Transformator.ModelInfo, touchesGround : boolean, totalDist : number, wheelAngleZ : number, transfsOutput : Transformator.TransfOutput)`  (call as `transformator_util.addAircraftWheelTransformation(…)`): While on the ground, rotates axles and wheels by the distance driven and steers the steering parts.
    - used 2x in the base game, e.g. `base/content/vehicle/shared/shared/transformator_air.script.tl:74`: `transformator_util.addAircraftWheelTransformation(`
- `calculateTilt : function(landVehicleApi : Transformator.LandVehicleScriptingApi, speed : number, distance : number, trainReversed : boolean) : number`  (call as `transformator_util.calculateTilt(…)`): Tilt value for tilting trains, from speed and the average track curvature around the vehicle.
    - param `landVehicleApi`: Path query API of the vehicle.
    - param `speed`: Current speed.
    - param `distance`: Half-length of the sampled stretch (applied in both directions), in metres.
    - param `trainReversed`: True if the train is driving backwards.
    - returns: Value between 0 and 1; 0.5 means no tilt.
    - used 1x in the base game, e.g. `base/content/vehicle/train/shared/shared/transformator_tiltingTrain.script.tl:52`: `local tilt = transformator_util.calculateTilt(`
- `colorAttributePostition : integer`: Attribute position for color instance attribute
    - used 1x in the base game, e.g. `base/content/vehicle/zeppelin/shared/shared/transformator_zeppelin.script.tl:66`: `transfsOutput:setModelInstanceAttributeVec3f(transformator_util.colorAttributePostition, color)`
- `ageAttributePostition : integer`: Attribute position for age instance attribute
    - used 1x in the base game, e.g. `base/content/vehicle/zeppelin/shared/shared/transformator_zeppelin.script.tl:65`: `transfsOutput:setModelInstanceAttributeFloat(transformator_util.ageAttributePostition, 0)`
- `color0AttributePostition : integer`: Attribute position for the first color of cblend 4 instance attribute
- `color1AttributePostition : integer`: Attribute position for the seconn color of cblend 4 instance attribute
- `color2AttributePostition : integer`: Attribute position for the third color of cblend 4 instance attribute
- `color3AttributePostition : integer`: Attribute position for the fourth color of cblend 4 instance attribute
