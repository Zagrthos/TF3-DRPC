
# Vehicles: advanced topics

This page continues [Vehicles](vehicles.md) with the parts that most vehicle mods only need now and then: custom transformator scripts, push-pull trains, grouped entries in the Vehicle Store and fake bogies for articulated vehicles. The transformator part applies to any animated model, not only vehicles.

## Transformator scripts

A transformator computes the animation states and particle settings of a model every frame. The model names it in `transformatorConfig` (see [Transformator config](models-and-assets.md#transformator-config)); the transformator itself is a `.trf.lua` file with up to four script references. The DLC Concorde has its own, `dlcs/urbangames_deluxe_upgrade_pack/content/vehicle/vehicle/plane/concorde/concorde.trf.lua`:

```lua
function data()

return {
	updateScript = {
		fileName = "transformator_concorde.script@concorde.updateFn",
		params = {}
	},
	updateParticleSystemScript = {
		fileName = "transformator_concorde.script@concorde.updateParticleSystemFn",
		params = {}
	},
}

end
```

and its model names it with the mod prefix:

```lua
transformatorConfig = { 
	skipFromLod = 2,
	transformator = { 
		name = "urbangames_deluxe_upgrade_pack::/vehicle/plane/concorde/concorde.trf",
	},
},
```

`fileName` is `<script file without .tl>@<table>.<function>`, relative to the `.trf.lua`. A key-value table in `params` reaches the function as its first argument (`captureParams`). The four scripts are:

| Key | Called | Third argument |
|---|---|---|
| `updateScript` | to move and animate the model | [`TransfOutput`](../api/api/type/transformator.md) (symbol: `Transformator.TransfOutput`, see [symbol index](../index.tsv)) |
| `updateParticleSystemScript` | once per `particleSystem` | [`ParticleSystem`](../api/api/type/transformator.md) (symbol: `Transformator.ParticleSystem`, see [symbol index](../index.tsv)) |
| `getEmittableModelsScript` | returns the models that `computeEmittedModelsScript` may spawn | none |
| `computeEmittedModelsScript` | spawns extra models relative to this one | [`ModelEmitter`](../api/api/type/transformator.md) (symbol: `Transformator.ModelEmitter`, see [symbol index](../index.tsv)) |

The second argument is always [`TransformatorParams`](../base/scripts/transformator_util.md) (symbol: `TransformatorParams`, see [symbol index](../index.tsv)). Inside these functions only `api.type` is available, no other `api` functions; the wiki gives performance as the reason.

The shipped transformators:

| File | Used by |
|---|---|
| `vehicle/shared/default_road.trf` | buses, trucks, cars (125 models) |
| `vehicle/train/shared/default_train.trf` | trains and wagons (110) |
| `vehicle/train/shared/tilting_train.trf` | tilting trains (7) |
| `vehicle/tram/shared/default_tram.trf` | trams (56) |
| `vehicle/ship/shared/default_ship.trf`, `hovercraft_ship.trf` | ships (26), hovercraft (2) |
| `vehicle/shared/default_air.trf` | aircraft and helicopters (42) |
| `vehicle/zeppelin/shared/zeppelin.trf` | the three zeppelins |
| `concorde.trf`, `hot_air_balloon.trf` | the DLC Concorde and hot air balloon |

### Update function

The default transformators combine helpers from `base/content/scripts/scripts/transformator_util.tl` (the wiki calls it `transformatorutil.tl`), documented as [`TransformatorUtil`](../base/scripts/transformator_util.md) (symbol: `TransformatorUtil`, see [symbol index](../index.tsv)). The Concorde script, `transformator_concorde.script.tl` next to the `.trf.lua`, calls the standard aircraft helpers and then adds its drooping nose (shortened):

```lua
local transformator_util = ug_require "::/scripts/transformator_util.tl" as TransformatorUtil

local updateFnConcorde = function(
	__captureParams : NativeLuaTable,
	params : TransformatorParams,
	transfsOutput : Transformator.TransfOutput
)
	if params.currentInfo.aircraft ~= nil then
		transformator_util.addFlapsAnimation(
			params.currentInfo.aircraft.flaps,
			params.aircraftStaticInfo.flapsDuration,
			transfsOutput
		)
		transformator_util.addDoorAnimationState(params.currentInfo.vehicle.doorAnimationInfo, transfsOutput)
		-- addGearAnimation, addAircraftControlSurfacesAnimation, addAircraftLightsAnimation, ...

		-- state2fraction maps each FlightState to a nose position from 0 to 1
		local timeNose = math.ceil(state2fraction[params.currentInfo.aircraft.flightState] * 11000)
		transfsOutput:addAnimationState("nose_up", -1, timeNose, false, false)
	end
end

return {
	concorde = {
		updateFn = updateFnConcorde,
		updateParticleSystemFn = transformator_util.updateParticleSystemFn
	},
}
```

`TransfOutput` has [`getUserTransfs`](../api/api/type/transformator.md) (symbol: `Transformator.TransfOutput.getUserTransfs`, see [symbol index](../index.tsv)) and [`setUserTransf`](../api/api/type/transformator.md) (symbol: `Transformator.TransfOutput.setUserTransf`, see [symbol index](../index.tsv)) for the per-node transformations (position and rotation on the track, for example; `setUserTransf` takes the node index, a matrix and whether it is absolute, default relative), [`getAnimationStates`](../api/api/type/transformator.md) (symbol: `Transformator.TransfOutput.getAnimationStates`, see [symbol index](../index.tsv)), [`addAnimationState`](../api/api/type/transformator.md) (symbol: `Transformator.TransfOutput.addAnimationState`, see [symbol index](../index.tsv)) (event name, start time, parameter, loop, reversed) to jump to a point of an animation, and [`triggerAnimation`](../api/api/type/transformator.md) (symbol: `Transformator.TransfOutput.triggerAnimation`, see [symbol index](../index.tsv)) (event name, optional start and end time) to play one. The wiki writes the first function as `getUserTransf`; the definition has `getUserTransfs`. The wiki adds that simpler transformators run faster, and that some helpers rely on values the simulation already computed (door timing, for instance).

### Particle function

`updateParticleSystemFn` gets the particle system of the model. The wiki's order is: check that the state data you need exists in `TransformatorParams`, compute the values once, then loop over the emitters. [`getSize`](../api/api/type/transformator.md) (symbol: `Transformator.ParticleSystem.getSize`, see [symbol index](../index.tsv)) returns the number of emitters and [`getParticleId`](../api/api/type/transformator.md) (symbol: `Transformator.ParticleSystem.getParticleId`, see [symbol index](../index.tsv)) the `particleId` of one emitter, so a script can treat `"exhaust"` and `"brake"` emitters differently. Emitter indices start at 0:

```lua
for i = 0, particleSystem:getSize() - 1 do
	if particleSystem:getParticleId(i) == "exhaust" then
		particleSystem:setLifeTimeScale(i, lifeTimeScale)
	end
end
```

The setters (`setLifeTimeScale`, `setColorOffset`, `setFrequencyScale`, `setVelocity`, ...) are listed under [`Transformator.ParticleSystem`](../api/api/type/transformator.md) (symbol: `Transformator.ParticleSystem`, see [symbol index](../index.tsv)).

### Emitted models

`computeEmittedModelsFn` can place models relative to the vehicle, for custom passengers, number plates and similar. [`getModelId`](../api/api/type/transformator.md) (symbol: `Transformator.ModelEmitter.getModelId`, see [symbol index](../index.tsv)) turns a model path into an id (a path without a mod prefix is taken from the base game), and [`emitModel`](../api/api/type/transformator.md) (symbol: `Transformator.ModelEmitter.emitModel`, see [symbol index](../index.tsv)) / [`emitModels`](../api/api/type/transformator.md) (symbol: `Transformator.ModelEmitter.emitModels`, see [symbol index](../index.tsv)) place one or several models with a matrix, animation states, a parent node and an align-to-terrain flag. The wiki says only models returned by `getEmittableModelsFn` can be emitted, and suggests passing the model list through `transformatorConfig.params`.

The zeppelins do this for their crew. `base/content/vehicle/zeppelin/zeppelin_nt/zeppelin_nt/zeppelin_nt.mdl` passes the paths:

```lua
transformatorConfig = { 
	params = { 
		crewPath = "characters/era_c_driver_air/era_c_driver_air.mdl",
		passengerPathsA = { },
		passengerPathsB = { },
		passengerPathsC = { },
	},
	skipFromLod = -1,
	transformator = { name = "/vehicle/zeppelin/shared/zeppelin.trf", },
},
```

and `transformator_zeppelin.script.tl` emits a crew model at every crew seat:

```lua
local crewModelId = modelEmitter:getModelId(transformatorConfigParamsZeppelin.crewPath)
-- for each seat of params.modelMetadataInfo.seats on this instance:
modelEmitter:emitModel(crewModelId, seat.transf, {a}, seat.group, false)
```

Its `getEmittableModelsZeppelinFn` returns an empty list, so this shipped script does not follow the wiki's rule. The files don't show whether the list is needed or only recommended. The wiki's downloadable example (custom number plates on an EMD F unit) collects all models of its `randomGroups` parameter in `getEmittableModelsFn` and picks one per group with `math.randomseed(transformatorParams.entityId + index)`, so every vehicle keeps its number.

### Transformator parameters

[`TransformatorParams`](../base/scripts/transformator_util.md) (symbol: `TransformatorParams`, see [symbol index](../index.tsv)) always has `entityId`. Everything else depends on the model and may be missing, so check before use.

Static data: `modelMetadataInfo` (seats), `vehicleStaticInfo` (axles, purchase time), `aircraftStaticInfo` (aircraft only) and `transformatorConfigParams` (the `params` of the model's `transformatorConfig`).

Dynamic data, for the current frame in `currentInfo` and for the previous one in `previousInfo` (when the model existed and was in view): `world` (cloud cover, game time, date, time of day, real time), `vehicle` (speed, power output, acceleration, maintenance, consist), the carrier-specific `roadVehicle`, `railVehicle`, `tram`, `landVehicle`, `aircraft`, `ship` and `airWaterVehicle`, and for static models `industry`, `station` and `townBuilding`. For the game time (`world.gameTime`) the wiki says "ticks since map start". The definitions give no unit, and the base mission task `wait` (`base/content/mission/mission/tasks/wait/wait.tl`) treats the engine's `GameTime.gameTime` as milliseconds. The Concorde compares `previousInfo.aircraft.flightState` with the current one to start its nose animation when the flight state changes.

## Reversible trains

Reversible trains and trams change direction in a station instead of running round. For that, the first and the last vehicle of the consist and every motorized vehicle in it need `reversible = true` in `transportVehicle`. 75 base models have it, among them the Traxx locomotive `base/content/vehicle/train/br_185_traxx/br_185_traxx/br_185_traxx.mdl` and the ICE 1 parts.

### Hiding meshes by position

The default train and tram transformators show and hide nodes by position in the consist and direction of travel. Each node needs both the `_on` and the `_off` animation. The Traxx headlights:

```lua
{ 
	animations = { 
		front_forward_parts_off = { 
			params = { id = "/vehicle/shared/ani/front_forward_parts_off.ani", },
			type = "FILE_REF",
		},
		front_forward_parts_on = { 
			params = { id = "/vehicle/shared/ani/front_forward_parts_on.ani", },
			type = "FILE_REF",
		},
	},
	materials = { "/vehicle/train/emissive/train_all_lights.mtl", },
	mesh = "msh/headlights_fwd_lod0.msh",
	name = "headlights_fwd",
	transf = { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, },
},
```

| Event pair | Shown when the vehicle is |
|---|---|
| `front_forward_parts_*` | at the front of the consist, driving forward |
| `inner_forward_parts_*` | neither first nor last, driving forward |
| `back_forward_parts_*` | at the back, driving forward |
| `front_backward_parts_*` | at the front, driving backward (reversible consists only) |
| `inner_backward_parts_*` | neither first nor last, driving backward (reversible only) |
| `back_backward_parts_*` | at the back, driving backward (reversible only) |
| `forward_parts_*`, `backward_parts_*` | driving forward or backward, wherever it is |
| `has_previous_*`, `has_no_previous_*` | with or without a vehicle in front |
| `has_next_*`, `has_no_next_*` | with or without a vehicle behind |

A mesh instance can serve only one of these cases; add a second instance of the mesh for another. The position events are for end-of-train markers, pantographs that depend on the position in the train and gangways between coaches. The shared animations are in `vehicle/shared/ani/`. The Traxx excerpt is base game code; in a mod, write `::/vehicle/shared/ani/front_forward_parts_on.ani` and `::/vehicle/train/emissive/train_all_lights.mtl`. Brake and blink lights of road vehicles and trams work with events of their own, see [Lights](vehicles.md#lights).

### Driver seats

`forward` on a crew seat limits it to one direction. A seat whose `group` is a node that is only shown at the front appears only in the leading vehicle. The Traxx does both:

```lua
seatProvider = { 
	crewModels = { },
	drivingLicense = "RAIL",
	seats = {
		{ animation = "driving_upright", crew = true, group = "headlights_fwd",
		  transf = { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 7.336, -0.673, 1.761, 1, }, },
		{ animation = "driving_upright", crew = true, forward = false, group = "headlights_bwd",
		  transf = { -1, 0, 0, 0, 0, -1, 0, 0, 0, 0, 1, 0, -7.342, 0.705, 1.761, 1, }, },
	},
},
```

## Vehicle store groups

The Vehicle Store can show several vehicles as one entry with "n variants" and a list of the variants; the technical data then belongs to the selected variant. This suits shape variants, repaints and multiple units of different lengths, for every carrier. If only one variant is available in a given year, it appears as a normal entry; the group appears once two or more variants are available. When variants of a rail group have different engine types, the store's engine filter (Steam, Diesel, Electric) shows only the matching subset.

A variant points to the group's parent with `groupFileName` in `transportVehicle`, relative to the variant's `.mdl` (the wiki calls the store the buy menu):

```lua
transportVehicle = {
	carrier = "RAIL",
	-- ...
	multipleUnitOnly = false,
	groupFileName = "menu_1020.mdl",
},
```

A multiple unit sets `groupFileName` in its `.mu.lua`, relative to that file. `base/content/gui/gui/line_vehicle_mgmt/vehicle_store_util.tl` reads the value as the vehicle's `variantGroup`. No base or DLC vehicle uses `groupFileName`; the examples here are the wiki's. A relative path only reaches models in the same mod; for a variant of a base or other mod's vehicle, see [Repaints: Model](vehicles.md#model).

For a group with its own name and icon, make a menu model that is never sold: give it the group's name and icon, set `multipleUnitOnly = true` and do not set `groupFileName` on it. As long as no multiple unit uses it, it only appears as the head of its group. The wiki's icon guidelines for such a menu model: up to four variants side by side, 30 px apart with the rightmost in front and a gap of about 2 px between them; for multiple units half to all of the second coach as well; at most 327 px wide at normal menu size (wider icons stretch the menu, from 700 px the right edge is cut off). A diagonal arrangement is the other common style.

## Fake bogies

The game aligns vehicle parts to the track or lane from their axles:

- An axle's origin sits centred above the lane.
- A node with axle meshes as children is a bogie. Its pivot is its mesh origin, which should be centred between its axles.
- A node with two bogies pivots in the middle between them.
- A node with more than two axles or bogies aligns to the outermost ones.

An articulated tram whose end sections rest on one bogie each would turn badly in curves. A fake bogie is a scripted pivot point attached to a node in place of a missing real bogie. `fakeBogies` holds one list per LOD, each entry with:

| Field | Meaning |
|---|---|
| `group` | node the fake bogie belongs to |
| `position` | distance along the lane from the model's root node; positive towards the front. The game takes the tangent to the lane at this point. |
| `offset` | distance along that tangent from `position` to the actual pivot |
| `upright` | optional; `true` keeps the node standing or hanging upright |

A node can have up to two fake bogies. If real and fake bogies together are more than two, the fake ones override the real ones, and the node aligns along the line between its two pivots. The Be 4/6 tram (`base/content/vehicle/tram/be4_6mirage/be4_6mirage/be4_6mirage.mdl`, shown on the [Vehicles](vehicles.md#rail-vehicles) page) gives its end sections `front_grp` and `back_grp` one fake bogie each and its middle frame `middle_grp` two. Road vehicles usually put theirs between the two axles; articulated buses put one on the rear axle of the front section, offset towards the joint.

In the base files `fakeBogies` sits directly in `config` as a list of LOD lists (`config = { axles = {...}, fakeBogies = { { ... }, { ... } } }`), as on the wiki's vehicle types page. The sample on the wiki's advanced topics page wraps `axles` and `fakeBogies` in an extra table inside `config` and lists the bogies without the per-LOD level. No base model sets `upright`.

Official wiki: [Vehicle advanced topics](https://wiki.transportfever3.com/doku.php?id=modding:vehicles:advancedtopics), [Vehicle types](https://wiki.transportfever3.com/doku.php?id=modding:vehicles:types), [Vehicle basics](https://wiki.transportfever3.com/doku.php?id=modding:vehicles:basics), [Scripting API](https://wiki.transportfever3.com/doku.php?id=modding:scripting:api).
