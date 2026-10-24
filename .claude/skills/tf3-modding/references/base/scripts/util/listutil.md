# scripts.util.listutil

Definition file: `base/tealdef/scripts/util/listutil.d.tl` (in the game folder).

Types of `/scripts/util/listutil.tl` (`ListUtil`).

## ListUtil (record)

```teal
global record ListUtil
```
Load: `local listutil = ug_require "::/scripts/util/listutil.tl" as ListUtil`

List helper returned by `ug_require "/scripts/util/listutil.tl"`.

- `listAssign : function<T>(n : integer, value : T) : {T}`  (call as `listutil.listAssign(…)`): Returns a list of length `n` with every element set to `value`.
    - used 2x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_react_util.tl:339`: `newPart.autoLoadConfig = listutil.listAssign(#vehiclePart.part.compartment2loadConfig, true)`
