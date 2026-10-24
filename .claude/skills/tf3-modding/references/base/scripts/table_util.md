# scripts.table_util

Definition file: `base/tealdef/scripts/table_util.d.tl` (in the game folder).

Types of `/scripts/table_util.tl` (`TableUtil`): deep comparison and copying of tables and simple
array search helpers. Load it with `local table_util = ug_require "/scripts/table_util.tl" as TableUtil`.

## TableUtil (record)

```teal
global record TableUtil
```
Load: `local table_util = ug_require "::/scripts/table_util.tl" as TableUtil`

Table helpers returned by `ug_require "/scripts/table_util.tl"`.

- `deepEquals : function(arg1 : any, arg2 : any, allowFunctions? : boolean, key? : any) : boolean`  (call as `table_util.deepEquals(…)`): Recursively compares two values; tables are equal when they have the same keys with deeply equal values.
    - param `arg1`: First value.
    - param `arg2`: Second value.
    - param `allowFunctions`: If true, functions are compared by identity. Otherwise comparing functions logs an error, unless
both tables carry a `<key>_compareProxy` sibling value (compared instead) or the key ends in `ecipe`.

    - param `key`: Key under which the values are stored; used for function comparison and error messages.
    - returns: True if the values are deeply equal.
    - notes: Userdata values must implement an `equals` method; it is called for the comparison.
    - used 140x in the base game, e.g. `base/content/base/base/difficulty_util.tl:213`: `if table_util.deepEquals(current, preset) then`
- `copy : function<T>(obj : T) : T`  (call as `table_util.copy(…)`): Deep copy of a table (keys and values copied recursively); non-table values are returned unchanged.
    - notes: Metatables are not copied, so vectors from `vec3.new` lose their operators.
    - used 238x in the base game, e.g. `base/content/climates/climates/gen/mapgenutil.tl:126`: `local riverMapBackup = table_util.copy(config.map.riverMap)`
- `shallowCopy : function<T>(obj : T) : T`  (call as `table_util.shallowCopy(…)`): Copies the top-level key/value pairs of a table into a new table; nested tables are shared.
    - used 67x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_charts.tl:48`: `local newState = table_util.shallowCopy(state)`
- `arrayFind : function<T>(arr : {T}, value : T) : integer`  (call as `table_util.arrayFind(…)`): Index of the first element equal (`==`) to `value`.
    - returns: The 1-based index, or `0` if the value is not in the array.
    - used 13x in the base game, e.g. `base/content/gui/gui/entity_window/double_slip_switch.tl:118`: `local locked = table_util.arrayFind(selectedState.lockedLanes, lane) > 0`
- `arrayFindIf : function<T>(arr : {T}, pred : function(T) : boolean) : integer`  (call as `table_util.arrayFindIf(…)`): Index of the first element for which `pred` returns true.
    - returns: The 1-based index, or `0` if no element matches.
    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/explorations/exploration_plane.script.tl:300`: `if table_util.arrayFindIf(explorationPlaneState.explorationPlanes,`
- `arrayRemove : function<T>(arr : {T}, value : T) : boolean`  (call as `table_util.arrayRemove(…)`): Removes the first element equal to `value` from the array, in place.
    - returns: True if an element was removed.
    - used 1x in the base game, e.g. `base/content/economy/economy/placementparamsutil.lua:665`: `table_util.arrayRemove(placementParam.tags, tag)`
- `arrayContains : function<T>(arr : {T}, value : T) : boolean`  (call as `table_util.arrayContains(…)`): True if the array contains an element equal (`==`) to `value`.
    - used 104x in the base game, e.g. `base/content/game_mechanics/game_mechanics/achievements/achievements.script.tl:230`: `if IsDefaultModCargo(cargoName) and not table_util.arrayContains(achievementState.cargoTypesDelivered, cargoName) then`
- `find : function<K, V>(dict : {{K, V}}, key : K) : V`  (call as `table_util.find(…)`): Looks up `key` in a list of `{key, value}` pairs.
    - returns: The value of the first pair whose first element equals `key`, or nil.
    - used 5x in the base game, e.g. `base/content/gui/gui/menu/new_game_or_map_settings_page.tl:94`: `map.climateId = table_util.find(configDict, "climate")`
- `concatenate : function<T>(... : {T}) : {T}`  (call as `table_util.concatenate(…)`): Returns a new array with the elements of all given arrays, in order.
    - used 28x in the base game, e.g. `base/content/gui/gui/entity_window/industry/industry.tl:300`: `widgets = table_util.concatenate(widgets, gui_react_util.usePlugins(industry_eow.IndustryEowExtensionPoint, { firstRef = initialFocusRef, }, extensionParams))`
- `merge : function<T, V>(... : {T : V}) : {T : V}`  (call as `table_util.merge(…)`): if same key is in multiple tables, the last one wins.
    - used 11x in the base game, e.g. `base/content/gui/gui/menu/graphics_presets_util.tl:169`: `consolePresetsSettings[i] = table_util.merge(proSettings, preset)`
- `removeDuplicates : function<T>(arr : {T}) : {T}`  (call as `table_util.removeDuplicates(…)`): Returns a new array with repeated elements removed, keeping the first occurrence and the order.
    - used 2x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_store_util.tl:367`: `vehicleData.cargoClasses = table_util.removeDuplicates(vehicleData.cargoClasses)`
