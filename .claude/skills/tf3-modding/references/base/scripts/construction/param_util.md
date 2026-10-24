# scripts.construction.param_util

Definition file: `base/tealdef/scripts/construction/param_util.d.tl` (in the game folder).

Types of `/scripts/construction/param_util.tl` (`ParamUtil`), ready-made construction parameters
(track type, catenary, rotation).

## ParamUtil (record)

```teal
global record ParamUtil
```
Load: `local param_util = require "::/scripts/construction/param_util.tl" as ParamUtil`

Construction parameter helpers returned by `ug_require "::/scripts/construction/param_util.tl"`.

- `makeTrackTypeDefaultParam : function() : ScriptParam`  (call as `param_util.makeTrackTypeDefaultParam(…)`): Create default rail track type parameters.
    - returns: script param for default rail track type selection
    - used 2x in the base game, e.g. `base/content/depots/rail/rail/rail_depot.con.lua:93`: `param_util.makeTrackTypeDefaultParam(),`
- `makeTrackTypeHighSpeedParam : function() : ScriptParam`  (call as `param_util.makeTrackTypeHighSpeedParam(…)`): Create high-speed rail track type parameters.
    - returns: script param for high-speed rail track type selection
    - used 2x in the base game, e.g. `base/content/depots/rail/rail/rail_depot.con.lua:94`: `param_util.makeTrackTypeHighSpeedParam(),`
- `makeTrackCatenaryParam : function() : ScriptParam`  (call as `param_util.makeTrackCatenaryParam(…)`): Create rail catenary parameters.
    - returns: script param for rail catenary toggle
    - used 2x in the base game, e.g. `base/content/depots/rail/rail/rail_depot.con.lua:95`: `param_util.makeTrackCatenaryParam(),`
- `makeTramTrackDefaultParam : function() : ScriptParam`  (call as `param_util.makeTramTrackDefaultParam(…)`): Create default tram track type parameters.
    - returns: script param for default tram track type selection
    - used 1x in the base game, e.g. `base/content/stations/street/street/modular_street_station/modular_terminal.con.lua:189`: `param_util.makeTramTrackDefaultParam(),`
- `makeTramTrackElectricParam : function() : ScriptParam`  (call as `param_util.makeTramTrackElectricParam(…)`): Create electric tram track type parameters.
    - returns: script param for electric tram track type selection
    - used 1x in the base game, e.g. `base/content/stations/street/street/modular_street_station/modular_terminal.con.lua:190`: `param_util.makeTramTrackElectricParam(),`
- `makeTramCatenaryParam : function() : ScriptParam`  (call as `param_util.makeTramCatenaryParam(…)`): Create tram catenary parameters.
    - returns: script param for tram catenary toggle
    - used 3x in the base game, e.g. `base/content/depots/road/road/tram_depot/tram_depot.con.lua:39`: `params = param_util.makeTramCatenaryParam(),`
- `getRailTrackTypes : function(withCaternay : boolean) : {string}`  (call as `param_util.getRailTrackTypes(…)`): Get available rail track type values.
    - param `withCaternay`: whether to include catenary variants
    - returns: list of track type template paths
    - used 2x in the base game, e.g. `base/content/depots/rail/rail/rail_depot.script.lua:278`: `local trackTypes = param_util.getRailTrackTypes(params.catenary == 2)`
- `makeRotationParam : function(key : string, name : string) : ScriptParam`  (call as `param_util.makeRotationParam(…)`): Create rotation parameters.
    - param `key`: unique parameter key
    - param `name`: display name
    - returns: script param for rotation selection
    - used 2x in the base game, e.g. `base/content/scripts/scripts/construction/assetbuilderutil.lua:57`: `local paramX = param_util.makeRotationParam("constructOpt56", _("Rotation X"))`
- `tramCatenaryYearFrom : integer`: Year from which catenaries are available for tram tracks
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/availability_notifications.script.tl:301`: `if year == param_util.tramCatenaryYearFrom then`

### ParamUtil.ScriptRef (record)

```teal
record ScriptRef
```

Reference to a script function and the parameters passed to it.

- `fileName : string`: Path to the script file
- `params : table`: Parameters passed to the script
