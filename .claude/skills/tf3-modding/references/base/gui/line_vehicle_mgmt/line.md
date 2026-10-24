# gui.line_vehicle_mgmt.line

Definition file: `base/tealdef/gui/line_vehicle_mgmt/line.d.tl` (in the game folder).

Types for the GUI-side copy of a line that the Line & Vehicle Manager edits before it is sent to the engine with `api.cmd.makeLineCreateCmd` / `api.cmd.makeLineUpdateCmd`.

## Line (record)

```teal
global record Line
```

Namespace for the GUI-side line types used by the Line & Vehicle Manager.

### Line.PassThroughStopData (record)

```teal
record PassThroughStopData
```

Stop settings that the line editor does not change itself but must keep, so that the update command can carry them over to the engine `Line.Stop`.

- `minWaitingTime : number`: Minimum waiting time at the stop, copied from / to `Engine.Component.Line.Stop.minWaitingTime`.
- `maxWaitingTime : number`: Maximum waiting time at the stop, copied from / to `Engine.Component.Line.Stop.maxWaitingTime`.
- `maxAdditionalWaitingTime : number`: Maximum additional waiting time at the stop, copied from / to `Engine.Component.Line.Stop.maxAdditionalWaitingTime`.
    - notes: `LineUtil.makeNewStop` sets it to 60 for new stops, or 30 when the station group serves road or tram but not rail.
- `stopConfig : Engine.Component.Line.StopConfig`: Cargo load configuration of the stop (`load`, `maxLoad`, `forceUnload` per cargo type), as in `Engine.Component.Line.Stop.stopConfig`.
- `loadMode : Engine.Component.Line.LoadMode`: Load mode of the stop, copied from / to `Engine.Component.Line.Stop.loadMode`.

### Line.ReactTerminal (record)

```teal
record ReactTerminal
```

One stop of a `Line.ReactLine`, using 1-based station and terminal indices (the engine uses 0-based ones).

- `stationGroup : Engine.Entity`: Entity of the station group the stop belongs to.
- `station1 : integer`: 1-based index in stationGroup
- `terminal1 : integer`: 1-based index in station
- `alternativeTerminals : {StationTerminal}`: Additional terminals (0-based `StationTerminal` values) the vehicle may use at this stop.
- `loadMode : Engine.Component.Line.LoadMode`: Load mode of the stop; overridden by `passThroughStopData.loadMode` when that is set.
- `passThroughStopData : PassThroughStopData`: not used by line editor, but need to store them so we can create the updateline command

### Line.ReactVia (record)

```teal
record ReactVia
```

One entry of a line path; either `stop` or `waypoint` is set.

- `waypoint : Waypoint`: nil if not a waypoint
- `stop : ReactTerminal`: The stop, or nil if this entry is a waypoint.

### Line.ReactLine (record)

```teal
record ReactLine
```

Editable copy of a line as the Line & Vehicle Manager holds it. Create one from the game state with `LineUtil.getReactLineFromGameState`.

Notes:

Unlike the engine `LINE` component, waypoints are separate entries of `path` instead of being
attached to the preceding stop. `LineUtil.autoAssignTerminals` converts the path back to
`Engine.Component.Line.Stop` entries.

- `entityAndRevision : EntityUtil.EntityAndRevision`: Line entity and the revision the copy was made from; nil for a line that is not created yet.
- `path : {ReactVia}`: Stops and waypoints of the line in driving order ("stop number" = 1-based index into this list).
- `customFilters : boolean`: Whether the cargo loading of the stops was configured by hand. When true, `LineUtil.autoLoadConfig` leaves the stop configuration unchanged.
    - notes: The cargo filter window sets it to true when the player changes a stop configuration.
- `reservationPriority : number`: Reservation priority of the line, passed to `Engine.Component.Line.reservationPriority`; new lines use 1.0.
- `maxWaypointTag : integer`: Highest waypoint `tag` used in the line (-1 if there are none); new waypoints get the next tag.
- `color : Vec3f`: Colour of a new line; filled with `LineUtil.makeLineColor` when the line is created.
- `carrierFilter : {Carrier : boolean}`: Carriers served by the line's stops, as computed by `LineUtil.getLineCarriers`.

### Line.DnDLineStop (record)

```teal
record DnDLineStop
```

Drag-and-drop payload used when a stop is dragged in the line panel of the Line & Vehicle Manager (resource `::/dnd_line_stop.res`).

- `lineEntity : Engine.Entity`: Line the dragged stop belongs to.
- `lineRevision : Engine.Revision`: Revision of the line when the drag started.
- `stopNumber : integer`: 1-based index of the dragged entry in `Line.ReactLine.path`.
- `apiStopIndex0 : integer`: 0-based index of the stop in the engine `LINE` component.
