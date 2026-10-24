# mission.arrival_tracker

Definition file: `base/tealdef/mission/arrival_tracker.d.tl` (in the game folder).

Client of the arrival-tracker game script, which counts passengers and cargo delivered between two places.

## ArrivalTracker (record)

```teal
global record ArrivalTracker
```
Load: `local arrival_tracker = ug_require "::/mission/arrival_tracker.tl" as ArrivalTracker`

Counts passengers and cargo that arrive by line between two places, through the `::/game_mechanics/arrival_tracker/arrivaltracker.gs` game script. Used by the transport-cargo tasks.

Example:
```lua
-- mission/tasks/transport_cargo/transport_cargo.tl
arrival_tracker.track(id, { cargotype = ctx.params.cargoTypeRes, to = target, from = source })
local delivered = arrival_tracker.get(id)
arrival_tracker.track(id) -- stop tracking
```

- `initStateIfEmpty : function(trackerState : TrackerState) : TrackerState`  (call as `arrival_tracker.initStateIfEmpty(…)`): Returns an empty tracker state if the given one is not initialised.
    - param `trackerState`: Current state.
    - returns: The state, or a new empty one.
    - used 4x in the base game, e.g. `base/content/game_mechanics/game_mechanics/arrival_tracker/arrivaltracker.script.tl:80`: `trackerState = arrival_tracker.initStateIfEmpty(trackerState)`
- `track : function(filtername : string, options? : ArrivalTracker.FilterData)`  (call as `arrival_tracker.track(…)`): Starts counting arrivals under a filter name, or stops and deletes it when no options are given.
    - param `filtername`: Name of the filter.
    - param `options`: What to count; `nil` deletes the filter.
    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/arrival_tracker/arrivaltracker.script.tl:84`: `arrival_tracker.track(filtername, options)`
- `get : function(filtertname : string) : integer`  (call as `arrival_tracker.get(…)`): Returns the number of arrivals counted for a filter.
    - param `filtertname`: Name of the filter.
    - returns: Count, or `nil` for an unknown filter.
    - used 2x in the base game, e.g. `base/content/mission/mission/tasks/transport_cargo/transport_cargo.tl:69`: `local c = arrival_tracker.get(id)`

### ArrivalTracker.FilterData (record)

```teal
record FilterData
```

What a tracking filter counts.

- `to : integer`: Destination entity (a town, a construction or the target entity); `nil` matches any.
- `from : integer`: Source entity (for persons the source town or building, for cargo the source entity); `nil` matches any.
- `cargotype : string`: Cargo type resource; `"::/cargos/passengers/passengers.cargo"` tracks persons, anything else cargo.

### ArrivalTracker.TrackerState (record)

```teal
record TrackerState
```

State of the arrival-tracker game script.

- `filterdata : {string : FilterData}`: Filter name -> filter.
- `data : {string : integer}`: Filter name -> number of arrivals counted.
- `filters : Filters`: Filter names by kind.

#### ArrivalTracker.TrackerState.Filters (record)

```teal
record Filters
```

Filter names by kind.

- `cargo : {string : boolean}`: Names of cargo filters.
- `person : {string : boolean}`: Names of person filters.
