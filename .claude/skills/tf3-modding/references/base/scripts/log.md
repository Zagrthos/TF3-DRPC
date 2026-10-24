# scripts.log

Definition file: `base/tealdef/scripts/log.d.tl` (in the game folder).

Types of the global `log` table, the game's script logging functions.

## log (record)

```teal
global record log
```

Global logging functions available to all game scripts.

- `verbose : function(string)`: Writes a verbose (debug) message to the game log.
    - used 75x in the base game, e.g. `base/content/base/base/mod.script.tl:498`: `log.verbose(debugString)`
- `message : function(string)`: Writes an informational message to the game log.
    - used 58x in the base game, e.g. `base/content/base/base/init.lua:127`: `log.message(table.concat(parts))`
- `warning : function(string)`: Writes a warning to the game log.
    - used 122x in the base game, e.g. `base/content/base/base/difficulty_util.tl:239`: `log.warning(lang_util.format("Did not find {modParam}, hence no difficulty scale can be provided.", { modParam = modParam }))`
- `error : function(string, ?string)`: Writes an error to the game log.
    - param `#1`: The error message.
    - param `#2`: Optional extra context, which the base game uses for the values involved.

    ```lua
    log.error("Could not find construction", "resName = " .. tostring(resName))
    ```

    - used 218x in the base game, e.g. `base/content/base/base/init.lua:246`: `log.error("Creating globals by assignment is not allowed (variable was )\nDid you forget to add 'local' before your variable?", "key = " .. tostring(key))`
