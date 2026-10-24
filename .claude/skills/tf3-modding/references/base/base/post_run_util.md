# base.post_run_util

Definition file: `base/tealdef/base/post_run_util.d.tl` (in the game folder).

Types of `/base/post_run_util.tl` (`PostRunUtil`), which applies the game's cost scale settings to loaded resources after all mods have run.

## PostRunUtil (record)

```teal
global record PostRunUtil
```

Cost scaling helpers returned by `ug_require "/base/post_run_util.tl"`.

- `applyCostScale : function()`: Multiplies prices and maintenance costs of street templates, bridges, tunnels, railroad crossings,
transport vehicle models and construction modules by the cost scales in `api.res.getBaseConfig().costs`.
    - notes: The base mod calls it from its post-run script, after the description costs were patched with `updateDesc`.
- `updateDesc : function(desc : ModelMetadata.Description, priceScale : number, maintenanceScale : number)`: Scales the `cost` and `maintenanceCost` attributes of a description (the values shown in the GUI); positive values only.
    - param `desc`: The description to patch.
    - param `priceScale`: Factor for `cost`.
    - param `maintenanceScale`: Factor for `maintenanceCost`.
