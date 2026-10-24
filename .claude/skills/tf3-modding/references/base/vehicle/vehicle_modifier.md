# vehicle.vehicle_modifier

Definition file: `base/tealdef/vehicle/vehicle_modifier.d.tl` (in the game folder).

Types of the `VehicleModifier` game script (`/vehicle/vehicle_modifier.script.tl`), which combines
speed, noise, pollution and comfort modifiers from several sources per vehicle.

## VehicleModifierChanger (record)

```teal
global record VehicleModifierChanger
```

Payload of the `ChangeModifier` event of the `VehicleModifier` game script.

Notes:

Each source (`id`) sets its own modifiers for a vehicle. The script adds up the deviations from 1.0
of all sources and applies the result with `api.cmd.makeVehicleSetModifiersCmd`.

Example:
```lua
-- from base/content/landmarks/landmarks.script.tl
local vehicleChanger = { entity = entity, modifications = { Comfort = 1.25 }, id = "landmarks.script" }
api.cmd.sendCommand(api.cmd.makeScriptingSendEventCmd("", "VehicleModifier", "ChangeModifier", vehicleChanger))
```

- `entity : Engine.Entity`: The vehicle.
- `modifications : {Modification : number}`: Factors by property, 1.0 = unchanged; an empty table removes this source's modifiers.
- `id : string`: Name of the source, e.g. `"landmarks.script"`; a later event with the same id replaces the earlier modifiers.

### VehicleModifierChanger.Modification (enum)

```teal
enum Modification
```

Vehicle property a modifier changes.

- `"TopSpeed"`: Top speed scale.
- `"Noise"`: Noise scale.
- `"Pollution"`: Pollution scale.
- `"Comfort"`: Comfort scale.

## VehicleModifierState (record)

```teal
global record VehicleModifierState
```

State of the `VehicleModifier` game script.

- `modificationMap : {Engine.Entity : {string : {VehicleModifierChanger.Modification : number}}}`: Modifiers per vehicle and source id.
