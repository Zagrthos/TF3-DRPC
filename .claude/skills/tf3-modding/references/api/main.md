# main

Definition file: `api/tealdef/main.d.tl` (in the game folder).

Basic definitions for all APIs

## Globals

- `ug_require : function(string) : any`: Used instead of the regular lua `require` to include game related script
- `_ : function(string) : string`: Used to mark strings localized string (eg. `_("Some text to localize")`)
- `nGetText : function(singularId : string, pluralId : string, n : integer) : string`: Returns a localized string, with plural handling
    - param `singularId`: singular text (or id)
    - param `pluralId`: plural text (or id)
    - param `n`: number of object referred to in the plural text (for plural)
    - returns: localized string
- `pGetText : function(context : string, id : string) : string`: Returns a localized string associated with a context
    - param `context`: localization context
    - param `id`: the string (or id) to localize
    - returns: localized string
- `npGetText : function(context : string, singularId : string, pluralId : string, n : integer) : string`: Returns a localized string associated with a context, with plural handling
    - param `context`: localization context
    - param `singularId`: singular text (or id)
    - param `pluralId`: plural text (or id)
    - param `n`: number of object referred to in the plural text (for plural)
    - returns: localized string
- `getStringForPlatform : function(playstation : string, xbox : string, desktop : string) : string`: Returns the appropriate string based on the current platform
    - param `playstation`: the string for playstation
    - param `xbox`: the string for xbox
    - param `desktop`: the string for desktop
- `getBuildVersion : function() : string`: Returns the build version
- `debugPrint : function(... : any)`: Prints text to the console/log file
- `resolve : function(relativePath : string) : string`: Resolves a file paths relative to the current file
    - param `relativePath`: a relative path
    - returns: the full filepath
- `getCurrentModId : function() : string`: Returns the mod id of the current mod scope
This is the outermost mod in the require hierarchy (i.e. the current file might belong to a different mod)
    - returns: modId string
- `getOwningModId : function() : string`: Returns the mod id of the mod owning the current file
Note: This means you can not delegate calls to this function into helper files!
    - returns: modId string
- `addModifier : function<T>(fileName : string, data : T) : T`: Registers a modifier function for a resource loader. The modifier receives every loaded resource of that kind and returns it, possibly changed.
    - param `fileName`: In practice the resource category, e.g. `loadModel` or `loadConstruction`.
    - param `data`: In practice the modifier function: it gets the file name and the loaded data and returns the data.
    - notes: Mods call it in the function named by `runScript` in `mod.json` (e.g. `mods/release/urbangames_vehicles_no_end_year/content/mod.script.lua`). The base implementation in `base/content/base/base/mod.lua` takes a resource category and a modifier function, so the parameter names in the definition are misleading. An unknown category raises an error. The wiki recommends `postRunFn` with `api.res` where possible.

    ```lua
    -- from base/base_mod.lua (runFn)
    addModifier("loadModel", function(fileName, data)
      local tv = data.metadata.transportVehicle
      if tv then
        -- adjust the vehicle metadata here
      end
      return data
    end)
    ```

- `_rootData : any`: Start parameters handed to the root GUI. The main menu reads it as its
`MainMenuParams` (e.g. `showNews`); the campaign page clears its `campaign` field.

## game (record)

```teal
global record game
```

Global table under which loaded scripts and their functions are reachable; base script
`scripts/util.tl` (`util.useFn`) resolves a reference `"<file>@<a.b.c>"` by walking
`game[file].a.b.c`.

- `__index : function(game, string) : game`: Lets any key be indexed on `game` for type checking; the result is again typed as `game`.
- `res : Res`

### game.Res (record)

```teal
record Res
```

- `script : table`
