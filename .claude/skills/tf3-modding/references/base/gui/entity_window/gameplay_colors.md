# gui.entity_window.gameplay_colors

Definition file: `base/tealdef/gui/entity_window/gameplay_colors.d.tl` (in the game folder).

Colours of gameplay concepts used in the GUI, loaded from `/gui/entity_window/gameplay_colors.gres`.

## GameplayColors (record)

```teal
global record GameplayColors
```

GUI colours of gameplay concepts (boosts, growth, maintenance, satisfaction, cargo shipping states), from `gameplay_colors.gres.lua`.

- `IndustryBoost : GResColor`: Colour of industry boosts.
- `TownGrowth : GResColor`: Colour of town growth.
- `Maintenance : GResColor`: Colour of maintenance.
- `Satisfaction : SatisfactionColors`: Colours of the satisfaction levels.
- `ShippingState : ShippingStateColors`: Colours of the places cargo can be in.

### GameplayColors.SatisfactionColors (record)

```teal
record SatisfactionColors
```

Colours of the satisfaction levels.

- `Happy : GResColor`: Colour of happy persons (kept in sync with the fluid traffic colour of the layers).
- `Aloof : GResColor`: Colour of aloof (neutral) persons (kept in sync with the intermediate traffic colour of the layers).
- `Sad : GResColor`: Colour of sad persons (kept in sync with the jammed traffic colour of the layers).

### GameplayColors.ShippingStateColors (record)

```teal
record ShippingStateColors
```

Colours of the places cargo can be in.

- `Warehouse : GResColor`: Cargo stored in a warehouse.
- `WaitingPassenger : GResColor`: Passengers waiting at a station.
- `Vehicle : GResColor`: Cargo in a vehicle.
- `IndustryInput : GResColor`: Cargo in an industry's input stock.
- `IndustryOutput : GResColor`: Cargo in an industry's output stock.
