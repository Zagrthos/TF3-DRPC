# scripts.util.config_util

Definition file: `base/tealdef/scripts/util/config_util.d.tl` (in the game folder).

Types of `/scripts/util/config_util.tl` (`ConfigUtil`) for editing the data of a generic script resource.

## ConfigUtil (record)

```teal
global record ConfigUtil
```
Load: `local config_util = ug_require "::/scripts/util/config_util.tl" as ConfigUtil`

Helper returned by `ug_require "/scripts/util/config_util.tl"`.

- `changeScriptConfig : function<T>(configRes : ResName, fn : function(config : T))`  (call as `config_util.changeScriptConfig(…)`): Loads a generic script resource, lets `fn` modify its data table and writes the data back.
    - param `configRes`: Resource name, for example `"::/gui/construction/tools/traffic_light_tool.res"`.
    - param `fn`: Function that receives the resource's data and changes it in place.

    ```lua
    -- from base/content/base/base/mod.script.tl
    config_util.changeScriptConfig("::/gui/construction/tools/traffic_light_tool.res", function(config : ConstructionToolData)
    	config.getDefinitionFn.params.yearFrom = trafficLightConfig.trafficLightYearFrom
    end)
    ```

    - used 7x in the base game, e.g. `base/content/base/base/mod.script.tl:397`: `config_util.changeScriptConfig("::/gui/construction/tools/traffic_light_tool.res", function(config : ConstructionToolData)`
