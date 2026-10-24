# game_mechanics.subventions.specific_subvention

Definition file: `base/tealdef/game_mechanics/subventions/specific_subvention.d.tl` (in the game folder).

Parameters for spawning one of the four base game subsidy types for a given target, as missions do.

## DeliverCargoSubventionParam (record)

```teal
global record DeliverCargoSubventionParam is ISubvention.ISpecificSubventionParam
```

Spawn parameters of the `deliver_cargo` subsidy ("Supply Industry").

- `target : Engine.Entity`: Industry that needs its input cargo. It must not have produced anything yet.

## DeliverCargoTownSubventionParam (record)

```teal
global record DeliverCargoTownSubventionParam is ISubvention.ISpecificSubventionParam
```

Spawn parameters of the `deliver_cargo_town` subsidy ("Supply Town").

- `target : Engine.Entity`: Town to supply.
- `cargoTypeId : CargoTypeId`: Cargo type to deliver, e.g. `api.res.cargoTypeRep.find("::/cargos/fuel/fuel.cargo")`.

## DeliverPassengersSubventionParam (record)

```teal
global record DeliverPassengersSubventionParam is ISubvention.ISpecificSubventionParam
```

Spawn parameters of the `deliver_passengers` subsidy ("Connect Towns"), a transfer-free passenger line between two towns.

- `target : Engine.Entity`: Destination town.
- `source : Engine.Entity`: Origin town.

## DeliverWorkersSubventionParam (record)

```teal
global record DeliverWorkersSubventionParam is ISubvention.ISpecificSubventionParam
```

Spawn parameters of the `deliver_workers` subsidy ("Transport Workers").

- `target : Engine.Entity`: Industry that needs workers transported from nearby towns.
