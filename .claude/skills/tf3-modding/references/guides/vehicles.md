
# Vehicles

A vehicle is a model (`.mdl`) whose `metadata` has the vehicle blocks described here. Meshes, materials, LODs and the general metadata blocks (lights, labels, load indicators, particles) work as in any other model, see [Models and assets](models-and-assets.md). This page covers the metadata of buses, trains, ships and planes, how the vehicle types differ, and repaint mods. Transformator scripts, reversible trains, vehicle store groups and fake bogies are on the second page, [Vehicles: advanced topics](vehicles-advanced.md).

The wiki splits vehicles into two groups: vehicles the player buys and runs on lines (buses, trucks, trams, trains, ships, aircraft), and cars, which the game drives on its own as private traffic.

## Where the vehicle files are

The base game keeps its vehicles in `vehicle/<type>/<name>/` inside the content folder. In the unpacked files each vehicle is its own archive, so the path doubles the vehicle name: the eCitaro bus is `base/content/vehicle/bus/ecitaro/ecitaro/ecitaro.mdl`, which the game addresses as `::/vehicle/bus/ecitaro/ecitaro.mdl` (see [How the zips map to folders](mod-structure.md#how-the-zips-map-to-folders)). Next to the model are `mat/` (materials, with textures in `mat/tex/`), `msh/`, `ani/` and `icons/`.

| Folder | Base models | Contents |
|---|---|---|
| `vehicle/bus` | 24 | buses and coaches |
| `vehicle/truck` | 48 | trucks, horse carts |
| `vehicle/car` | 46 | private cars (AI traffic) |
| `vehicle/tram` | 55 | trams, tram wagons, light rail |
| `vehicle/train` | 70 | locomotives and multiple unit parts |
| `vehicle/waggon` | 41 | unpowered rail wagons (the folder is spelled `waggon`) |
| `vehicle/ship` | 27 | ships, ferries, hovercraft |
| `vehicle/plane` | 30 | aircraft |
| `vehicle/helicopter` | 11 | helicopters |
| `vehicle/zeppelin` | 3 | airships, see [below](#air-vehicles) |

The wiki names `vehicle/wagon` for unmotorized rail vehicles; the base game folder is `vehicle/waggon`. Sound sets sit in `vehicle/<type>/shared/sound/` and emissive light materials in `vehicle/<type>/emissive/` (for example `vehicle/bus/emissive/bus_brake_lights.mtl`). `vehicle/shared/` holds the shared animations, the dirt and rust textures and the road and air transformators; the train, tram and ship transformators are in their own `vehicle/<type>/shared/`. The DLCs add 18 more vehicle models under `dlcs/<dlc>/content/vehicle/vehicle/...`, for example the Concorde in `dlcs/urbangames_deluxe_upgrade_pack/content/vehicle/vehicle/plane/concorde/concorde.mdl`.

## Which metadata blocks a vehicle needs

| Block | Used by | Purpose |
|---|---|---|
| `transportVehicle` | every vehicle the player can buy | carrier, transport modes, capacity, loading, depot filter |
| `landVehicle` | road, tram and rail | engines, top speed, weights, braking |
| `roadVehicle` | buses, trucks, cars | axles, steering wheels and fake bogies |
| `railVehicle` | trains, wagons, trams | axles and fake bogies |
| `waterVehicle` | ships | power, drag area, waterline |
| `airVehicle` | aircraft, helicopters | thrust, wing area, landing gear |
| `car` | private cars | marks the model as AI traffic |
| `colorConfig` | private cars | random paint colours |
| `seatProvider` | almost all | driver and passenger positions |
| `draftAnimalProvider` | horse carts, coaches, horse trams | the animals that pull the vehicle, see [Draft animals](#draft-animals) |
| `emissions` | almost all | noise and pollution |
| `soundConfig` | almost all | sound set |
| `transformatorConfig` | all | script that drives animations and particles |
| `availability`, `cost`, `maintenance`, `description` | buyable vehicles | years, price, running costs, name (see [Common blocks](models-and-assets.md#common-blocks)) |

The types are in the reference as [`ModelMetadata.TransportVehicle`](../api/api/type.md) (symbol: `ModelMetadata.TransportVehicle`, see [symbol index](../index.tsv)), [`LandVehicle`](../api/api/type.md) (symbol: `ModelMetadata.LandVehicle`, see [symbol index](../index.tsv)), [`WaterVehicle`](../api/api/type.md) (symbol: `ModelMetadata.WaterVehicle`, see [symbol index](../index.tsv)) and [`AirVehicle`](../api/api/type.md) (symbol: `ModelMetadata.AirVehicle`, see [symbol index](../index.tsv)). `roadVehicle`, `railVehicle`, `seatProvider`, `car` and `colorConfig` appear in the model files but have no Teal record.

All base vehicles are `version = 2` models. For those the base game adds nothing on its own: the loaders that would add a default `transformatorConfig` ([`addTransformatorConfig`](../base/base/model_metadata_util.md) (symbol: `ModelMetadataUtil.addTransformatorConfig`, see [symbol index](../index.tsv))) and default door, wheel and light animations (`createAnimationEvents*Vehicle` in `base/content/base/base/metadataanimationutil.lua`) only run for older model versions. A new vehicle has to name its transformator and carry its animations on the nodes itself. [Lights](#lights) shows how that works for brake and blink lights, and [A complete vehicle file](#a-complete-vehicle-file) has a whole model to start from.

### Draft animals

Horse carts, coaches and horse trams are pulled by animal models that the vehicle names in `draftAnimalProvider`. 13 base models use it, among them `base/content/vehicle/bus/american_post_coach/american_post_coach/american_post_coach.mdl` and the horse tram `double_horse.mdl`. The campaign's carnival wagons do the same, from `mods/release/urbangames_campaign_mission_01/content/vehicle/vehicle/bus/carnival_waggon_4/carnival_waggon_4.mdl` (second horse shortened):

```lua
draftAnimalProvider = { 
	draftAnimals = {
		{ 
			group = "horse_grp_l",
			model = "::/animal/draft_horse/draft_horse.mdl",
			transf = { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, -0.28, 0, 0, 1, },
			transfOverride = {
				{ 
					affectedGroup = "harness_btm_front",
					attachToGroup = "harness_front_attach",
					transf = { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, },
				},
				{ 
					affectedGroup = "harness_btm_back",
					attachToGroup = "harness_back_left_attach",
					transf = { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, },
				},
				{ 
					affectedGroup = "harness_hands",
					attachToGroup = "harness_handle_attach",
					transf = { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, },
				},
			},
		},
		{ group = "horse_grp_r", model = "::/animal/draft_horse/draft_horse.mdl", ... },
	},
},
```

Each entry places one animal: `model` is the animal's `.mdl`, `group` the vehicle node it is attached to (here two empty nodes `horse_grp_l` and `horse_grp_r` under a `horse_grp` node at the front) and `transf` its offset from that node. `transfOverride` moves single nodes of the animal model onto nodes of the vehicle, so the harness reaches the cart: `affectedGroup` is a node in `draft_horse.mdl` (`harness_btm_front`, `harness_btm_back`, `harness_hands`), `attachToGroup` an empty node in the vehicle (`harness_front_attach` under the front bogie mesh `front_b1`, `harness_handle_attach` in the driver's seat group `back_comp_grp`). The base models write the model path as `/animal/draft_horse/draft_horse.mdl`; in a mod, write `::/animal/...`, as the campaign does. The Model Editor creates this block from a `draft_animal_<mdl>` locator in the FBX, see [Metadata from the FBX](model-editor.md#metadata-from-the-fbx).

## Transport vehicle

From the eCitaro, `base/content/vehicle/bus/ecitaro/ecitaro/ecitaro.mdl` (shortened):

```lua
transportVehicle = { 
	carrier = "ROAD",
	comfortFactor = 0.65,
	compartments = {
		{ 
			loadConfigs = {
				{ 
					cargoEntry = { 
						capacity = 80,
						cargoTypeSet = { 
							cargoClassesIncluded = { "PASSENGERS", },
							cargoClassesExcluded = { },
							cargoTypesIncluded = { },
							cargoTypesExcluded = { },
						},
						loadIndicator = "",
						seats = { },
					},
					toHide = { },
				},
			},
		},
	},
	engineTransportModes = { "BUS", },
	entrances = {
		{ path = { { 4.8, -3, 0.3, }, { 4.4, 0, 0.4, }, }, },
		{ path = { { -1.1, -3, 0.3, }, { -1.1, 0, 0.4, }, }, },
	},
	filterTags = { "default", },
	loadSpeed = 5,
	transportModes = { "BUS", },
},
```

### Transport modes

`carrier` picks the means of transport, `transportModes` the networks the vehicle can run on and `engineTransportModes` the networks on which its own engines can move it. An electric locomotive runs on all tracks but drives only under catenary, so it lists `TRAIN` and `ELECTRIC_TRAIN` as transport modes and only `ELECTRIC_TRAIN` as engine mode. A wagon has an empty `engineTransportModes`. The values in the base game files:

| `carrier` | `transportModes` | `engineTransportModes` |
|---|---|---|
| `"ROAD"` | `BUS` or `TRUCK` | the same |
| `"TRAM"` | `TRAM`, `ELECTRIC_TRAM`; light rail adds `TRAM_TRACK`, `ELECTRIC_TRAM_TRACK` | `ELECTRIC_TRAM` (plus `ELECTRIC_TRAM_TRACK`), or `TRAM`, `ELECTRIC_TRAM` for horse and steam trams |
| `"RAIL"` | `TRAIN`, `ELECTRIC_TRAIN` | `ELECTRIC_TRAIN` (electric), `TRAIN`, `ELECTRIC_TRAIN` (steam, diesel), empty (wagons) |
| `"WATER"` | `SMALL_SHIP` or `SHIP` | the same |
| `"AIR"` | `SMALL_AIRCRAFT`, `AIRCRAFT` or `HELICOPTER` | the same |

Light rail vehicles such as `base/content/vehicle/tram/caf_urbos3/caf_urbos3/caf_urbos3.mdl` add the `*_TRAM_TRACK` modes (9 base models); the Vehicle Store counts such parts as light rail compatible for its Light Rail filter (`base/content/gui/gui/line_vehicle_mgmt/vehicle_store_util.tl`). The enums are [`Carrier`](../api/api/type.md) (symbol: `Carrier`, see [symbol index](../index.tsv)) and [`TransportMode`](../api/api/type.md) (symbol: `TransportMode`, see [symbol index](../index.tsv)).

If a model leaves `transportModes` empty, the loader [`addTransportVehicleTransportModes`](../base/base/model_metadata_util.md) (symbol: `ModelMetadataUtil.addTransportVehicleTransportModes`, see [symbol index](../index.tsv)) fills both lists from the carrier: road vehicles get `BUS` or `TRUCK` from the cargo of their first load config, rail and tram vehicles get the electric-only engine mode if all engines are `ELECTRIC`, ships and aircraft get the small or big mode from `waterVehicle.type` or `airVehicle.type` (helicopters from `isHelicopter`). The base game models set the lists explicitly.

### Other transport vehicle fields

| Field | Meaning |
|---|---|
| `loadSpeed` | Loading and unloading speed. For passenger cars the wiki counts one unit per door lane that a person can use at the same time. `-1` lets the game estimate it. |
| `comfortFactor` | How slowly passenger happiness drops during the trip, 0 to 1, 0.5 is average. |
| `priceFactor`, `maintenanceFactor` | The wiki says both are currently unused. |
| `arrivalDelay`, `departureDelay` | Milliseconds between arriving and the start of loading, and between the end of loading and departure. The wiki asks for the length of the longest door animation. |
| `reversible` | The vehicle can be part of a push-pull train, see [Reversible trains](vehicles-advanced.md#reversible-trains). |
| `filterTags` | A depot only offers vehicles that have all of its tags. Missing means `{ "default" }`, the tag of the base depots. An empty list hides the vehicle from every depot; it can still be part of a multiple unit. |
| `multipleUnitOnly` | The vehicle is only sold as part of a multiple unit (see [Multiple units](#multiple-units)). |
| `groupFileName` | Parent entry for grouping variants in the Vehicle Store, see [Vehicle store groups](vehicles-advanced.md#vehicle-store-groups). |
| `entrances` | Paths that boarding passengers walk along, each a list of points relative to the model. 119 base models have them; the wiki does not describe them. |
| `noCblendMask` | `true` when the model has no colour blend mask, so the player cannot recolour it. |

The base game files differ from the wiki on some of these:

- The wiki says `-1` for `comfortFactor` makes the game compute a value from the vehicle's properties. In `base/content/base/base/base_mod.lua` the first `loadModel` modifier replaces `-1` with a fixed value per carrier (0.5 by default, 0.2 for `AIR`, 0.7 for `WATER`) and sets `priceFactor` at the same time (0.5, 0.9 for `AIR`, 0.1 for `WATER`, 0.3 for `ROAD` and `TRAM`). No base vehicle uses `-1`; 115 of them use `comfortFactor = 0`.
- The wiki gives 0.5 as the default of `maintenanceFactor`. [`newLoadSpeedsAndMaintenanceFactor`](../base/base/model_metadata_util.md) in `model_metadata_util.lua` sets it to `1` when it is missing.
- Only one base transport vehicle sets `arrivalDelay` and `departureDelay` (both `0`, the horse tram `base/content/vehicle/tram/double_horse/double_horse/double_horse.mdl`). Cars set `arrivalDelay = 2000` inside their `car` block instead.
- [`addFilterTags`](../base/base/model_metadata_util.md) (symbol: `ModelMetadataUtil.addFilterTags`, see [symbol index](../index.tsv)) gives a vehicle without `filterTags` the tag `default`, except when `multipleUnitOnly` is set: then the list stays empty.

### Compartments

The wiki calls the capacity list `compartmentsList`. 313 base and DLC models use the name `compartments`, 8 DLC models use `compartmentsList`; the loader [`toCompartmentList`](../base/base/model_metadata_util.md) (symbol: `ModelMetadataUtil.toCompartmentList`, see [symbol index](../index.tsv)) renames `compartmentsList` to `compartments`, so both work. It also converts the older `capacities` and `loadConfigs` forms.

The structure has three levels:

- A list of compartments. Most vehicles have one. A vehicle that carries two loads at once (the wiki's example is a double-deck tram with passengers upstairs and cargo below) has one compartment per load.
- Each compartment has `loadConfigs`, the alternative ways to fill it. A ship that carries either coal alone or coal and iron ore half and half needs two load configs. When an empty vehicle loads, the game picks the first config that moves the most cargo under the line's cargo filter.
- Each load config has a `cargoEntry` (`capacity`, `cargoTypeSet`, `loadIndicator`, `seats`) and `toHide`, a list of node names hidden while the config is in use.

`cargoTypeSet` lists included and excluded cargo classes and cargo types (see [Cargo type sets](other-content.md#cargo-type-sets)). `loadIndicator` names an entry in the model's `loadIndicator` block ([Load indicators](models-and-assets.md#load-indicators)). `seats` lists the seat indices that belong to this entry, which matters for early ships that carry passengers and cargo in the same hull.

### Emissions

```lua
emissions = { 
	noise = { score = 15, },
	pollution = { score = 15, },
},
```

`score` is 0 to 100 and rates the vehicle within its carrier; the game maps it to real values per carrier and fills in `idle` (standing), `speed` (at top speed) and `power` (at full thrust) with interpolation in between. `-1` asks the game to estimate the score from the vehicle's other stats; a vehicle without `emissions` gets `score = -1` for both (`addMissingVehicleStats` in `model_metadata_util.lua`). The wiki asks for higher noise and pollution on vehicles with better power and speed, so that every vehicle has a drawback.

## Seats

```lua
seatProvider = { 
	crewModels = { },
	drivingLicense = "BUS",
	seats = {
		{ 
			animation = "driving_upright",
			crew = true,
			group = "comp",
			transf = { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 4.63, 0.79, 0.435, 1, },
		},
		{ animation = "sitting", group = "comp", transf = { ... }, },
		-- ...
	},
},
```

`drivingLicense` picks the default crew models: `BUS`, `TRUCK`, `TRAM`, `RAIL`, `WATER`, `AIR` and `AIR_OUTDOOR` (pilot with helmet) in the wiki. The base helicopters use a further value, `HELICOPTER` (12 base and DLC models, e.g. `base/content/vehicle/helicopter/bell_47/bell_47/bell_47.mdl`). `crewModels` replaces the defaults with your own character models. Crew models are character models whose `person.drivingLicenses` names the license, see [People](other-content.md#people).

Each seat has an `animation` (`driving`, `driving_upright`, `idle`, `sitting` or `walk`), `crew` (crew are visible even with no passengers on board; default `false`), `group` (the node the seat moves and hides with) and `transf`. `forward` limits a crew seat to one direction of travel in reversible trains. A seat on a node that is hidden is hidden too, which is how a second locomotive gets no driver.

## Sound

```lua
soundConfig = { 
	soundSet = { name = "/vehicle/bus/shared/sound/bus_electric.snd", },
},
```

`soundSet.name` points to a sound set (`.snd`, see [Sound sets](other-content.md#sound-sets)). The base game writes `/vehicle/...` for its own sets; in a mod, prefix the base sound sets with `::`, for example `::/vehicle/bus/shared/sound/bus_electric.snd`. `effects` overrides single events of that set. The wiki sample writes it as `"horn" = "audio/horn.wav"`, which is not valid Lua; the type ([`SoundConfig.effects`](../api/api/type.md) (symbol: `ModelMetadata.SoundConfig.effects`, see [symbol index](../index.tsv))) and the zeppelins (`base/content/vehicle/zeppelin/zeppelin_nt/zeppelin_nt/zeppelin_nt.mdl`) use a list of files per event:

```lua
effects = {
	select = { "sound/selected_zeppelin.wav" },
},
```

## Cars

A road model becomes private traffic with a `car` block. The wiki shows an empty `car = { }`. All 48 base and DLC cars set two values in it, from `base/content/vehicle/car/beetle/beetle/beetle.mdl`:

```lua
car = { 
	arrivalDelay = 2000,
	spawnWeight = 1,
},
colorConfig = { 
	configs = {
		{ { 0.601961, 0.572635, 0.496913, }, },
		{ { 0.553856, 0.633333, 0.623564, }, },
		-- ...
	},
},
```

`spawnWeight` is 1 for most cars and lower (down to 0.0001) for rare ones; the wiki does not mention it. A car has `landVehicle`, `roadVehicle`, `seatProvider` and `emissions` like a bus, but no `transportVehicle`. `colorConfig.configs` lists colours (RGB, 0 to 1) that the game picks at random; the car's material needs a colour blend map for this.

The wiki's top speeds for base cars by era, as a guide for balancing: 20 km/h until 1925, 50 km/h until 1950, 80 km/h until 1980, 100 km/h until 2000 and 120 km/h after that. `topSpeed` is in m/s, so divide by 3.6.

## Vehicle types

In TF3 most type-specific metadata names parts of the model by their node name. Give the nodes clear names; the Model Editor shows them in the node tree and copies a name on double click.

### Land vehicles

Road, tram and rail vehicles share a `landVehicle` block. From the eCitaro:

```lua
landVehicle = { 
	curveSpeedScale = 1,
	engines = {
		{ power = 150, tractiveEffort = 30, type = "ELECTRIC", },
	},
	topSpeed = 19.444,
	weightEmpty = 14000,
	weightMaxPayload = 15429,
},
```

| Field | Meaning |
|---|---|
| `engines` | zero or more engines, each with `type` (`HORSE`, `STEAM`, `DIESEL`, `ELECTRIC`), `power` in kW and `tractiveEffort` in kN |
| `topSpeed` | m/s |
| `weightEmpty`, `weightMaxPayload` | kg; the payload is the extra weight when fully loaded |
| `friction` | friction coefficient; the wiki names it, together with `brakeDeceleration`, as the way to tune acceleration and braking. Every base train and tram sets 0.02, road vehicles leave it out. |
| `brakeDeceleration` | how hard the vehicle brakes. Every base train and tram sets 2.5, road vehicles leave it out. The model files and the wiki give no unit; the runtime value [`VehicleData.brakeDeceleration`](../api/api/type.md) (symbol: `VehicleData.brakeDeceleration`, see [symbol index](../index.tsv)) is documented as m/s². |
| `curveSpeedScale` | set in 44 base models, to 1 or, on the parts of the tilting trains Avelia Liberty and ETR 450, to 1.25; the wiki does not describe it |

The Teal record [`ModelMetadata.LandVehicle`](../api/api/type.md) (symbol: `ModelMetadata.LandVehicle`, see [symbol index](../index.tsv)) has no `brakeDeceleration` or `curveSpeedScale`. In Teal, read and write them through a plain table, for example `(model.metadata.landVehicle as {string:any}).brakeDeceleration`.

The wiki puts speed and weight among the properties of `roadVehicle` and `railVehicle`. The base models keep them in `landVehicle`. [`turnRoadAndRailToLandVehicle`](../base/base/model_metadata_util.md) (symbol: `ModelMetadataUtil.turnRoadAndRailToLandVehicle`, see [symbol index](../index.tsv)) builds a `landVehicle` from an older `roadVehicle` or `railVehicle` when the model has none, with a default brake deceleration of 5 (road) or 2.5 (rail) and friction of 0.1 or 0.02.

### Road vehicles

`roadVehicle.config` names the moving parts. From the eCitaro (one fake bogie list per LOD):

```lua
roadVehicle = { 
	config = { 
		axles = { "w2", },
		fakeBogies = {
			{ { group = "RootNode", offset = 0, position = 0, }, },        -- LOD 0
			{ { group = "RootNode", offset = 0, position = 0, }, },        -- LOD 1
			{ { group = "RootNode", offset = 0, position = 0, },
			  { group = "RootNode", offset = 0, position = -2.63, }, },    -- LOD 2
		},
		steeringParts = { },
		wheels = { "w1_lft", "w1_rgt", },
	},
},
```

`axles` are nodes that only roll (turn around the y axis), usually rear axles. `wheels` roll and steer (also turn around the z axis), usually front wheels. `steeringParts` only steer. `fakeBogies` are explained on the [advanced page](vehicles-advanced.md#fake-bogies); a road vehicle usually has one between the steering axle and the first rear axle. Put the mesh origin of an axle at its centre on the x and z axes, or it wobbles in the game.

### Rail vehicles

`railVehicle.config` has `axles` and `fakeBogies`. Simple rail vehicles only need fake bogies in LODs without separate axles; articulated trams and trains with Jacobs bogies need them in more LODs. The Be 4/6 tram, `base/content/vehicle/tram/be4_6mirage/be4_6mirage/be4_6mirage.mdl` (start of LOD 0):

```lua
railVehicle = { 
	config = { 
		axles = { "front_w1", "front_w2", "middle_w1", "middle_w2", "back_w1", "back_w2", },
		fakeBogies = {
			{
				{ group = "front_grp", offset = 0, position = 2.5, },
				{ group = "back_grp", offset = 0, position = -1.81, },
				{ group = "middle_grp", offset = 0, position = 2.5, },
				{ group = "middle_grp", offset = 0, position = -1.5, },
				-- ...
			},
			-- further LODs
		},
	},
},
```

Trams and trains differ by `carrier` (`TRAM` or `RAIL`) and their transformator (`default_tram.trf` or `default_train.trf`).

### Multiple units

A multiple unit is a fixed consist sold as one vehicle. It is a `.mu.lua` file next to its models; the base game has 13. From `base/content/vehicle/train/emd_f/emd_f/emd_f.mu.lua`:

```lua
function data()
	return {
		vehicles = {
			{ name = "emd_fa.mdl", forward = true },
			{ name = "emd_fb.mdl", forward = true },
		},
		name = _("VEHICLE_MULTIPLEUNIT_EMD_F_NAME"),
		desc = _("VEHICLE_MULTIPLEUNIT_EMD_F_DESCRIPTION"),
		filterTags = { "default" },
	}
end
```

`vehicles` lists the models relative to the `.mu.lua` file; `forward = false` turns the model by 180 degrees, as the ICE 1 does with its rear power car in `base/content/vehicle/train/ice1/ice1/ice1.mu.lua`. `name` and `desc` appear in the Vehicle Store (wrapped in `_()` for translation); the technical data is computed from the parts. `filterTags` and an optional `groupFileName` work as on a single vehicle. A multiple unit is available only while all its parts are within their availability years.

The parts of the base multiple units set `multipleUnitOnly = true` and `filterTags = { }` in their `transportVehicle` (31 models, e.g. `ice1_front.mdl`), so they are not sold on their own.

### Water vehicles

Ships have a `waterVehicle` block. From `base/content/vehicle/ship/kakadua/kakadua/kakadua.mdl` (waterline shortened):

```lua
waterVehicle = { 
	area = 5,
	availPower = 600000,
	maxRpm = 133,
	topSpeed = 10.5556,
	waterLine = {
		{ 11.8805, 0, },
		{ 11.8805, 0.125, },
		-- ... 31 points along one side of the hull
		{ -10.96, 0, },
	},
	weightEmpty = 125000,
	weightMaxPayload = 18000,
},
```

| Field | Meaning |
|---|---|
| `area` | largest underwater cross section of the hull in m²; drag grows linearly with it |
| `availPower` | engine power in W (not kW) |
| `weightEmpty`, `weightMaxPayload` | kg |
| `maxRpm` | top revolutions of the paddle wheel or propeller, for the animation |
| `topSpeed` | m/s |
| `type` | `"SMALL"` ships can use small piers, `"BIG"` ships need the large harbour |
| `waterLine` | x/y points of the hull outline at water level, for the foam around the ship |

The base ships do not set `type`; they set `transportModes = { "SMALL_SHIP" }` or `{ "SHIP" }` directly. The loader fills `transportModes` from `type` only when a model leaves it empty. Only the DLC ship `ts_queen_mary.mdl` sets `type`.

### Air vehicles

Aircraft and helicopters have an `airVehicle` block. From `base/content/vehicle/plane/douglas_dc3/douglas_dc3/douglas_dc3.mdl` (shortened):

```lua
airVehicle = { 
	axles = {
		{ position = { 3.38, 0.59, }, radius = 0.59, },
		{ position = { 3.38, 0.59, }, radius = 0.59, },
	},
	config = { 
		axleRadii = { 0.59, 0.59, },
		axles = { "w1_lft", "w1_rgt", },
		fakeBogies = { ... },
		steeringParts = { "g2", },
		wheelRadii = { 0.29, },
		wheels = { "w3", },
	},
	hasFlaps = true,
	maxThrust = 25000,
	timeToFullThrust = 3,
	topSpeed = 83.333,
	weightEmpty = 7700,
	wheels = {
		{ position = { -7.81, 2.56535, }, radius = 0.27, },
	},
	wingArea = 91,
},
```

The outer `axles` and `wheels` lists give the position (x and z from the origin) and radius of every axle and wheel, so the game can place and tilt the aircraft on the ground. `config` names the nodes as for road vehicles; `axleRadii` and `wheelRadii` give one radius in metres per listed node. The flight values are `maxThrust` and `idleThrust` in N, `timeToFullThrust` in seconds, `topSpeed` in m/s, `weightEmpty` and `weightMaxPayload` in kg, `wingArea` in m² (lift grows linearly with it), `hasFlaps`, `isHelicopter` for vertical take-off and landing, and `type` (`"SMALL"` for small airfields, `"BIG"` for airports).

As with ships, the base aircraft give the size through `transportModes` (`SMALL_AIRCRAFT`, `AIRCRAFT`, `HELICOPTER`) and leave out `type`. The Concorde also sets `maxTakeOffWeight = 54400`, a field the wiki does not list. Helicopters such as the Bell 47 set `isHelicopter = true`, `transportModes = { "HELICOPTER" }` and `drivingLicense = "HELICOPTER"`.

The three models in `base/content/vehicle/zeppelin/` are not transport vehicles. They have no `transportVehicle` or `airVehicle`, and their own transformator `vehicle/zeppelin/shared/zeppelin.trf` moves them.

### Animation events by type

With the default transformators, animations named after these events play on their own. Times for `rudder`, `aileron_*` and `elevator` run from 0 to 1000 with 500 as the centre position; `flaps` runs from 0 (retracted) to 1000 (extended).

| Event | Road | Tram | Rail | Water | Air |
|---|---|---|---|---|---|
| `forever` (loops, e.g. fans, radar) | yes | yes | yes | yes | yes |
| `drive` (follows the speed) | yes | yes | yes | yes | yes |
| `wheels` (length mapped to one wheel turn) | | | yes | | |
| `open_all_doors`, `close_all_doors` | yes | yes | yes | yes | yes |
| `open_doors_left`/`_right`, `close_doors_left`/`_right` | yes | yes | yes | yes | yes |
| `brake_lights_on`/`_off` | yes | yes | yes | | |
| `blink_lights_left_on`/`_off`, `blink_lights_right_on`/`_off` | yes | yes | | | |
| `paddles`, `rudder` | | | | yes | `rudder` |
| `aileron_left`, `aileron_right`, `elevator`, `flaps` | | | | | yes |
| `beacon_lights`, `strobe_lights`, `landing_light_on`/`_off` | | | | | yes |
| `props_on`/`_off`, `props_blurred_on`/`_off` | | | | | yes |
| `open_doors_cargo`, `close_doors_cargo`, `open_wheels`, `close_wheels` | | | | | yes |

The door events fire after the vehicle stops and before it leaves; the left and right variants only for stops on that side. For ship propellers, turn the mesh by 90 degrees so that its y axis points along the rotation axis. Reversible trains have further show/hide events, see [Reversible trains](vehicles-advanced.md#hiding-meshes-by-position). The eCitaro keeps its door animations as files per event, e.g. `ani/open_doors_right/d1.ani`.

### Lights

Vehicle lights are meshes with a glowing material, and the light events switch them on and off. Put each group of lights (head, brake, left and right blink lights) on its own node with its own mesh, and give it an emissive material from `vehicle/<type>/emissive/`. The bus folder has `bus_all_lights.mtl`, `bus_add_lights.mtl`, `bus_brake_lights.mtl` and `bus_blink_lights.mtl`; car, truck and tram have matching files, and train has `train_all_lights.mtl` and `train_red_lights.mtl`. For lights that only shine at certain moments, add both animations of the event pair to the node as `FILE_REF` to the shared files in `vehicle/shared/ani/` (`brake_lights_on.ani` and `brake_lights_off.ani`, `blink_lights_left_on.ani` and so on). The eCitaro's brake lights, from `base/content/vehicle/bus/ecitaro/ecitaro/ecitaro.mdl`:

```lua
{ 
	animations = { 
		brake_lights_off = { 
			params = { id = "/vehicle/shared/ani/brake_lights_off.ani", },  -- in a mod: "::/vehicle/shared/ani/brake_lights_off.ani"
			type = "FILE_REF",
		},
		brake_lights_on = { 
			params = { id = "/vehicle/shared/ani/brake_lights_on.ani", },   -- in a mod: "::/vehicle/shared/ani/brake_lights_on.ani"
			type = "FILE_REF",
		},
	},
	materials = { "/vehicle/bus/emissive/bus_brake_lights.mtl", },     -- in a mod: "::/vehicle/bus/emissive/..."
	mesh = "msh/brakelights_lod0.msh",
	name = "brakelights",
	transf = { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, },
},
```

The nodes `blinklights_lft` and `blinklights_rgt` follow the same pattern with `bus_blink_lights.mtl` and the `blink_lights_left_*` and `blink_lights_right_*` pairs. The headlights (`headlights`, with `bus_all_lights.mtl`) have no animation. The beam they throw onto the road comes from the `lightSourceList` block, whose entries name the node in `meshId`, see [Light sources](models-and-assets.md#light-sources). Trains switch their head and tail lights by position in the consist instead, see [Hiding meshes by position](vehicles-advanced.md#hiding-meshes-by-position).

## A complete vehicle file

The page shows the eCitaro in pieces. For a new vehicle in a mod, a whole file from a mod is the better template, because it already uses the paths a mod needs. The campaign's fourth carnival wagon, `mods/release/urbangames_campaign_mission_01/content/vehicle/vehicle/bus/carnival_waggon_4/carnival_waggon_4.mdl`, is a horse-drawn bus with all the vehicle blocks of a road vehicle. It borrows the meshes and materials of the base American post coach with `::/` paths and adds decorations of its own with `/` paths, which point into the campaign mod. Shortened to one LOD with a few nodes, and with numbers rounded:

```lua
function data()
return { 
	boundingInfo = { bbMax = { 4.898, 1.09, 3.395, }, bbMin = { -2.22, -1.09, -0.002, }, },
	collider = { 
		params = { halfExtents = { 3.559, 1.09, 1.698, }, },
		transf = { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, },
		type = "BOX",
	},
	lods = {
		{ 
			node = { 
				children = {
					{ 
						children = {
							{ 
								children = {
									{ name = "harness_front_attach", transf = { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 3.4, 0, 0.85, 1, }, },
									-- harness_back_left_attach, harness_back_right_attach
								},
								materials = { "::/vehicle/bus/american_post_coach/mat/american_post_coach.mtl", },
								mesh = "::/vehicle/bus/american_post_coach/msh/front_b1_lod0.msh",
								name = "front_b1",
								transf = { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, },
							},
							{ 
								materials = { "::/vehicle/bus/american_post_coach/mat/american_post_coach.mtl", },
								mesh = "::/vehicle/bus/american_post_coach/msh/front_w1_lod0.msh",
								name = "front_w1",
								transf = { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, -0.085, 0, 0.625, 1, },
							},
						},
						name = "front_grp",
						transf = { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 1.345, 0, 0.005, 1, },
					},
					{ 
						children = {
							{ 
								materials = { "::/vehicle/bus/american_post_coach/mat/american_post_coach.mtl", },
								mesh = "::/vehicle/bus/american_post_coach/msh/back_body_lod0.msh",
								name = "back_body",
								transf = { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, },
							},
							{ 
								animations = { 
									open_all_doors = { 
										params = { id = "::/vehicle/bus/american_post_coach/ani/open_all_doors/back_d1_lft.ani", },
										type = "FILE_REF",
									},
									-- close_all_doors, open_doors_left, close_doors_left
								},
								materials = { "::/vehicle/bus/american_post_coach/mat/american_post_coach.mtl", "::/vehicle/bus/american_post_coach/mat/american_post_coach_door.mtl", },
								mesh = "::/vehicle/bus/american_post_coach/msh/back_d1_lft_lod0.msh",
								name = "back_d1_lft",
								transf = { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0.277, 0.787, -0.005, 1, },
							},
							{ 
								materials = { "::/vehicle/bus/emissive/bus_all_lights.mtl", },
								mesh = "::/vehicle/bus/american_post_coach/msh/back_headlights_lod0.msh",
								name = "back_headlights",
								transf = { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, -0.005, 1, },
							},
							{ 
								children = {
									{ 
										materials = { "/vehicle/bus/carnival_waggon_all/mat/carnival_waggon_generic.mtl", },
										mesh = "/vehicle/bus/carnival_waggon_all/msh/decoration_ballchain_01_lod0.msh",
										name = "decoration_ballchain_01",
										transf = { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, },
									},
								},
								name = "decoration_ballchain_inst_9",
								transf = { 0.988, 0.147, 0.017, 0, -0.145, 0.985, -0.087, 0, -0.03, 0.083, 0.996, 0, 0.649, -1.041, 2.454, 1, },
							},
							{ 
								children = {
									{ name = "harness_handle_attach", transf = { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 1.899, 0, 2.799, 1, }, },
								},
								name = "back_comp_grp",
								transf = { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, },
							},
							-- back_w2 (rear axle), glass, interior, right door, more decorations
						},
						name = "back_grp",
						transf = { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0.005, 1, },
					},
					{ 
						children = {
							{ children = { { name = "horse_grp_l", transf = { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, }, }, },
							  name = "left_horse", transf = { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0.389, 0, 1, }, },
							{ children = { { name = "horse_grp_r", transf = { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, }, }, },
							  name = "right_horse", transf = { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, -0.389, 0, 1, }, },
						},
						name = "horse_grp",
						transf = { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 3.579, 0, 0, 1, },
					},
				},
				name = "RootNode",
				transf = { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, },
			},
			visibleFrom = 0,
			visibleTo = 100,
		},
		-- LOD 1 (visibleFrom = 100, visibleTo = 400) and LOD 2 (400 to 1000) with the _lod1 and _lod2 meshes
	},
	metadata = { 
		availability = { yearFrom = 1900, yearTo = 1925, },
		cost = { price = -1, },
		description = { 
			description = _("VEHICLE_BUS_CARNIVAL_WAGGON_DESCRIPTION"),
			name = _("VEHICLE_BUS_CARNIVAL_WAGGON_NAME"),
		},
		draftAnimalProvider = { 
			draftAnimals = {
				{ group = "horse_grp_l", model = "::/animal/draft_horse/draft_horse.mdl", transf = { ... }, transfOverride = { ... }, },
				{ group = "horse_grp_r", model = "::/animal/draft_horse/draft_horse.mdl", transf = { ... }, transfOverride = { ... }, },
			},
		},
		emissions = { 
			noise = { score = 8.25, },
			pollution = { score = 4, },
		},
		extent = { bbMax = { 5.898, 1.09, 3.395, }, bbMin = { -2.22, -1.09, -0.002, }, },
		landVehicle = { 
			engines = {
				{ power = 8, tractiveEffort = 3, type = "HORSE", },
			},
			topSpeed = 7.8,
			weight = 1,                       -- older field, in tonnes; current models write weightEmpty in kg
		},
		lightSourceList = { 
			lights = {
				{ 
					angleMax = 30, angleMin = 1,
					color = { 0.796, 0.663, 0.376, },
					fromTime = 18, toTime = 7,
					invertMask = true, lightId = "", mask = 0,
					meshId = "back_headlights",
					mode = "LIGHT", radius = 1.7, strength = 3,
					transf = { 0.985, 0, -0.174, 0, 0, 1, 0, 0, 0.174, 0, 0.985, 0, 1.2, 0.8, 2.2, 1, },
					type = "POINT",
					visibleFrom = -1, visibleTo = -1,
				},
				-- second lamp, interior light
			},
		},
		maintenance = { lifespan = 21915, runningCosts = -1, },
		roadVehicle = { 
			config = { 
				axles = { "front_w1", "back_w2", },
				fakeBogies = {
					{
						{ group = "front_grp", offset = 0, position = 3.58, },
						{ group = "back_grp", offset = 0, position = 1.35, },
						{ group = "horse_grp", offset = 0, position = 3.58, },
					},
					-- LOD 1 the same, LOD 2 with two fake bogies each on front_grp and back_grp
				},
				steeringParts = { },
				wheels = { },
			},
		},
		seatProvider = { 
			crewModels = { },
			drivingLicense = "BUS",
			seats = {
				{ 
					animation = "driving_upright",
					crew = true,
					group = "back_comp_grp",
					transf = { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 1.49, -0.012, 1.999, 1, },
				},
			},
		},
		soundConfig = { 
			soundSet = { name = "::/vehicle/bus/shared/sound/bus_horse.snd", },
		},
		transformatorConfig = { 
			skipFromLod = 2,
			transformator = { name = "::/vehicle/shared/default_road.trf", },
		},
		transportVehicle = { 
			carrier = "ROAD",
			compartmentsList = {
				{ 
					loadConfigs = {
						{ 
							cargoEntry = { 
								capacity = 24,
								cargoTypeSet = { 
									cargoClassesExcluded = { },
									cargoClassesIncluded = { "PASSENGERS", },
									cargoTypesExcluded = { },
									cargoTypesIncluded = { },
								},
								loadIndicator = "",
								seats = { },
							},
							toHide = { },
						},
					},
				},
			},
			engineTags = { "BUS", },          -- older names; current models write engineTransportModes
			loadSpeed = 2,
			tags = { "BUS", },                -- and transportModes
		},
		versioning = { __version = "_v03", },
	},
	version = 2,
}
end
```

The campaign file still uses three older field names, which loaders in `base/content/base/base/model_metadata_util.lua` convert: `landVehicle.weight` in tonnes becomes `weightEmpty` in kg (`convertWeight`), `tags` and `engineTags` become `transportModes` and `engineTransportModes` (`addTransportVehicleTransportModes`), and `compartmentsList` becomes `compartments` (`toCompartmentList`). In a new file, write the current names as in the eCitaro: `weightEmpty = 1000`, a `weightMaxPayload`, `transportModes = { "BUS", }` and `engineTransportModes = { "BUS", }`. The two lamps shine from 18:00 to 7:00 (`fromTime`, `toTime`). The Model Editor lists `versioning.__version` among its metadata fields (`base/content/model_editor/model_editor/meta_metadata.lua`); what the game does with it is not documented.

To turn the file into your own vehicle:

- Replace the meshes, materials and animations with your own (`msh/...`, `mat/...`, `ani/...` relative to the model) or keep base files with `::/`. Your own files can also use a `/` path from the content folder of your mod, as the decorations do.
- Rename the nodes to match your model, and use the same names in `roadVehicle.config`, `seats[].group`, `draftAnimals[].group`, `lightSourceList.lights[].meshId` and `transfOverride`. A bus without horses drops `draftAnimalProvider` and the `horse_grp` node, and lists its front wheels in `wheels`.
- Give the vehicle its own translation keys in `description` and its own icons (see [Icons](#icons)).
- Keep `cost.price = -1` and `maintenance.runningCosts = -1` to let the game compute the price and running costs (see [Common blocks](models-and-assets.md#common-blocks)).
- Pick the sound set and the transformator from the base game with `::/`, as here.

## Repaints

A repaint adds a new livery to an existing vehicle. The wiki names three levels: new textures, new textures with extra meshes, and new textures with changed meshes. Only the last needs copies of the original files, and so the original author's permission. Even for the first two, the community asks the author first; many authors then share their texture work files.

Ship only new or changed files, give the repaint its own `modId` and declare the original mod as a dependency in `mod.json` ([Dependencies](mod-structure.md#dependencies)). A repaint of a base game vehicle needs no dependency. The wiki has an example repaint of the base eCitaro for download.

A texture-only repaint has:

- the usual mod files (`mod.json`, `_metadata/modinfo.json`, see [Mod structure](mod-structure.md)); naming the original author as co-author is good manners
- a `.mtl` for every material that gets new textures
- a `.mdl` for every new variant
- the three icons of every new variant
- the textures

A repaint that keeps the original's stats changes nothing in the simulation. The official DLCs, which add vehicles, set `"cosmetic": true` and `"severityRemove": "Warning"` in their `mod.json`; the wiki asks for `Warning` when removing a mod leaves vehicle models missing. Whether a vehicle with new stats still counts as cosmetic is the modder's call under the wiki's rule (nothing that affects the simulation), see [mod.json](mod-structure.md#modjson).

### Materials

Copy the original `.mtl` and change the texture references. A vehicle material usually has four maps that a repaint may change: `map_albedo_opacity` (colour and transparency), `map_normal` (normal map, e.g. for raised number plates), `map_metal_gloss_ao` (metal, gloss, ambient occlusion) and `map_cblend_dirt_rust` (where the player can recolour, and where dirt and rust show). From `base/content/vehicle/bus/ecitaro/ecitaro/mat/ecitaro.mtl` (shortened):

```lua
map_albedo_opacity = { 
	fragmentSamplers = { 
		albedoOpacityTex = { 
			fileName = "tex/ecitaro_albedo_opacity.dds",
			type = "TWOD",
			wrapS = "REPEAT",
			wrapT = "REPEAT",
		},
	},
},
-- map_normal, map_metal_gloss_ao and map_cblend_dirt_rust look the same
map_dirt = { 
	fragmentSamplers = { 
		dirtTex = { fileName = "/vehicle/shared/mat/tex/dirt_albedo.dds", ... },  -- in the copy: "::/vehicle/shared/mat/tex/dirt_albedo.dds"
	},
},
```

The texture paths are relative to the material. In the copy, every texture you keep must point back to the original: write `"::/vehicle/bus/ecitaro/mat/tex/ecitaro_normal.dds"` for a base game file, or `"<original_modid>::/..."` for another mod. The relative form `"::tex/..."` only works if your file sits at the same path as the original. The shared dirt and rust textures (`/vehicle/shared/mat/tex/dirt_albedo.dds`, `dirt_normal.dds`, `rust_albedo.dds`, `rust_normal.dds`) need the prefix too: without it, a path that starts with `/` points into your mod. The rules are in [File references inside content](mod-structure.md#file-references-inside-content).

Copy every material that reads a texture you change. In the eCitaro all five materials read `tex/ecitaro_albedo_opacity.dds`: `ecitaro.mtl` for the body, `ecitaro_glass.mtl` and `ecitaro_int.mtl` for the windows and the interior, and `ecitaro_lod.mtl` and `ecitaro_int_lod.mtl`, which the two lower LODs use. A material you leave out keeps showing the old livery in its part of the model or from some distance on.

### Model

Copy the original `.mdl` and change three things: the material references in every LOD, `availability`, and `description` (with new translation keys):

```lua
lods = {
	{
		node = {
			children = {
				{
					materials = { "mat/ecitaro_red.mtl", },
					mesh = "::/vehicle/bus/ecitaro/msh/body_lod0.msh",
					-- ...
				},
				-- ...
```

```lua
metadata = {
	availability = { yearFrom = 2019, yearTo = 0, },
	description = {
		description = _("VEHICLE_BUS_ECITARO_RED_DESCRIPTION"),
		name = _("VEHICLE_BUS_ECITARO_RED_NAME"),
	},
	-- everything else as in the original
},
```

Everything you don't copy stays in the original, so every other reference in the copied file needs a prefix. Relative paths such as `msh/body_lod0.msh`, `ani/open_doors_right/d1.ani` and the materials you keep become `::/vehicle/bus/ecitaro/msh/body_lod0.msh` and so on. The eCitaro also has 22 references that start with `/`: the emissive light materials (`/vehicle/bus/emissive/bus_all_lights.mtl`), the shared light animations (`/vehicle/shared/ani/brake_lights_on.ani`), the sound set and the transformator. These become `::/vehicle/...` too, because without a prefix they point into your mod. For a repaint of another mod's vehicle, use `<original_modid>::/...` for its files and `::/` for the base files it uses.

To group the repaint under the original in the Vehicle Store, set `groupFileName` in `transportVehicle`, see [Vehicle store groups](vehicles-advanced.md#vehicle-store-groups). The relative form shown there only reaches models in the same mod; for the eCitaro, write the full name, `groupFileName = "::/vehicle/bus/ecitaro/ecitaro.mdl"`. The wiki's leasing example in its mod script page does the same with the name from the model repository. The Vehicle Store copies the value unchanged (`base/content/gui/gui/line_vehicle_mgmt/vehicle_store_util.tl`) and groups the variant under the model whose repository name equals it. No shipped vehicle uses the field, so whether the `::/` form matches the name of a base model is not confirmed.

### Icons

The wiki lists three icons, and the Model Editor's SCREENSHOT button (top right) creates them in the right folder (see [Icons and screenshots](model-editor.md#icons-and-screenshots)). The base game file names are slightly different from the wiki's: the eCitaro has `icons/ecitaro_store.tga`, `icons/ecitaro_icon_small@2x.tga` and `icons/ecitaro_icon20@2x.tga`, plus `_cblend` masks for the small and the 20 px icon. The wiki writes the middle one as `<vehiclename>_small_icon`. When `description` sets no icons, [`addIconMetadata`](../base/base/model_metadata_util.md) (symbol: `ModelMetadataUtil.addIconMetadata`, see [symbol index](../index.tsv)) uses these defaults, with `<vehicle>` as the model file name:

| `description` field | Default | Used for |
|---|---|---|
| `icon3d` | `icons/<vehicle>_store.tga` | details in the Vehicle Store |
| `iconSmall`, `iconSmallCblend` | `icons/<vehicle>_icon_small.tga`, `..._icon_small_cblend.tga` | compact list in the Vehicle Store |
| `icon20`, `icon20cblend` | `icons/<vehicle>_icon20.tga`, `..._icon20_cblend.tga` | other windows and lists |

Icons are stored at double size under an `@2x.tga` name (see [Textures](models-and-assets.md#textures)), while the defaults name the file without `@2x`. The icon paths are relative to the model, so a repaint keeps its icons in an `icons/` folder next to its `.mdl`.

### Textures

DDS compression loses detail, so don't paint over the shipped `.dds` files; ask the author for work files where possible. The general texture rules are in [Textures](models-and-assets.md#textures).

Official wiki: [Vehicle basics](https://wiki.transportfever3.com/doku.php?id=modding:vehicles:basics), [Vehicle types](https://wiki.transportfever3.com/doku.php?id=modding:vehicles:types), [Repaint mods](https://wiki.transportfever3.com/doku.php?id=modding:vehicles:repaints), [Vehicle advanced topics](https://wiki.transportfever3.com/doku.php?id=modding:vehicles:advancedtopics), [Cargo](https://wiki.transportfever3.com/doku.php?id=modding:misc:cargo), [Model definition (.mdl)](https://wiki.transportfever3.com/doku.php?id=modding:general:resourcetypes:mdl).
