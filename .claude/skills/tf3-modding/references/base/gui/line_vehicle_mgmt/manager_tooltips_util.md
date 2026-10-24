# gui.line_vehicle_mgmt.manager_tooltips_util

Definition file: `base/tealdef/gui/line_vehicle_mgmt/manager_tooltips_util.d.tl` (in the game folder).

Tooltip recipes shown next to the cursor while the Line & Vehicle Manager tool is active (prefix `LM` = line management, `VM` = vehicle management).

## ManagerTooltipsUtil (record)

```teal
global record ManagerTooltipsUtil
```
Load: `local manager_tooltips_util = ug_require "::/gui/line_vehicle_mgmt/manager_tooltips_util.tl" as ManagerTooltipsUtil`

Cursor tooltips of the Line & Vehicle Manager, set with `ContextBuilder.tooltip`.

- `Debug : Recipe<string>`: Debug tooltip showing the given message.
- `Default : Recipe<any>`: "Select a line, vehicle, station or depot."
  In game: "Select a line, vehicle, station or depot." (German: „Wähle eine Linie, Fahrzeug, Station oder Depot.“)
- `makeSelectLineTooltip : function(entity : Engine.Entity) : string`  (call as `manager_tooltips_util.makeSelectLineTooltip(…)`): Returns "Select <name>." for the entity.
  In game: "Select {name}." (German: „Wähle {name}.“)
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_hud_util.tl:136`: `tooltip = manager_tooltips_util.makeSelectLineTooltip(lineEntity),`
- `SelectLine : Recipe<LineUtil.LineSelectionDetails>`: "Select <line name>." for a picked line (nothing if the line does not exist).
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:322`: `ctx.tooltip(manager_tooltips_util.SelectLine, details.line)`
- `makeAddVehicleToSelectionTooltip : function(Engine.Entity) : string`  (call as `manager_tooltips_util.makeAddVehicleToSelectionTooltip(…)`): Returns "Add <name> to selection.".
  In game: "Add {name} to selection." (German: „{name} zur Auswahl hinzufügen.“)
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_hud_util.tl:1286`: `tooltips[vehicleEntity] = manager_tooltips_util.makeAddVehicleToSelectionTooltip(vehicleEntity)`
- `AddVehicleToSelection : Recipe<Engine.Entity>`: "Add <name> to selection."
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:197`: `ctx.tooltip(manager_tooltips_util.AddVehicleToSelection, entity)`
- `makeRemoveVehicleFromSelectionTooltip : function(Engine.Entity) : string`  (call as `manager_tooltips_util.makeRemoveVehicleFromSelectionTooltip(…)`): Returns "Remove <name> from selection.".
  In game: "Remove {name} from selection." (German: „{name} aus der Auswahl entfernen.“)
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_hud_util.tl:1283`: `tooltips[vehicleEntity] = manager_tooltips_util.makeRemoveVehicleFromSelectionTooltip(vehicleEntity)`
- `RemoveVehicleFromSelection : Recipe<Engine.Entity>`: "Remove <name> from selection."
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:195`: `ctx.tooltip(manager_tooltips_util.RemoveVehicleFromSelection, entity)`
- `makeAddVehicleToListTooltip : function(Engine.Entity) : string`  (call as `manager_tooltips_util.makeAddVehicleToListTooltip(…)`): Returns "Add <name> to list.".
  In game: "Add {name} to list." (German: „{name} zur Liste hinzufügen.“)
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_hud_util.tl:1289`: `tooltips[vehicleEntity] = manager_tooltips_util.makeAddVehicleToListTooltip(vehicleEntity)`
- `AddVehicleToList : Recipe<Engine.Entity>`: "Add <name> to list."
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:200`: `ctx.tooltip(manager_tooltips_util.AddVehicleToList, entity)`
- `RemoveVehicleFromList : Recipe<Engine.Entity>`: "Remove <name> from list."
  In game: "Remove {name} from list." (German: „{name} aus der Liste entfernen.“)
- `OpenDepot : Recipe<Engine.Entity>`: "Buy vehicles at <depot name>."
  In game: "Buy vehicles at {name}." (German: „Fahrzeuge kaufen in {name}.“)
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:341`: `ctx.tooltip(manager_tooltips_util.OpenDepot, details.vehicleDepot.entity)`
- `AddOrRemoveDepotVehiclesToVehicleList : Recipe<LineUtil.VehicleDepotDetails>`: "Add vehicles in <depot> to list." or "Remove vehicles in <depot> from list.", depending on `removeFromList`.
  In game: "Remove vehicles in {name} from list.", "Add vehicles in {name} to list." (German: „Fahrzeuge in {name} von der Liste entfernen.“, „Fahrzeuge in {name} zur Liste hinzufügen.“)
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:339`: `ctx.tooltip(manager_tooltips_util.AddOrRemoveDepotVehiclesToVehicleList, details.vehicleDepot)`
- `OpenWarehouse : Recipe<Engine.Entity>`: "Open <warehouse name>."
  In game: "Open {name}." (German: „{name} öffnen“)
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:370`: `ctx.tooltip(manager_tooltips_util.OpenWarehouse, entity)`
- `CreateLine : Recipe<LineUtil.StationSelectionDetails>`: "Create new line starting at <station name>."
  In game: "Create new line starting at {name}." (German: „Neue Linie erstellen beginnend bei {name}.“)
    - used 2x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:308`: `ctx.tooltip(manager_tooltips_util.CreateLine, sgDetails)`
- `CreateLineError : Recipe<any>`: "Start new line is currently disabled."
  In game: "Start new line is currently disabled." (German: „Neue Linie starten ist derzeit deaktiviert.“)
    - used 2x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:306`: `ctx.tooltip(manager_tooltips_util.CreateLineError, nil)`
- `LMDefault : Recipe<any>`: "Select a station to add stops."
  In game: "Select a station to add stops." (German: „Eine Station auswählen, um Haltestellen hinzuzufügen.“)
- `LMDefaultMoveStop : Recipe<any>`: "Select the station to which the stop should be moved."
  In game: "Select the station to which the stop should be moved." (German: „Die Station auswählen, zu der die Haltestelle verschoben werden soll.“)
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:924`: `ctx.tooltip(manager_tooltips_util.LMDefaultMoveStop, nil)`
- `LMDefaultMoveWaypoint : Recipe<any>`: "Select the location to which the waypoint should be moved."
  In game: "Select the location to which the waypoint should be moved." (German: „Den Ort auswählen, an den der Wegpunkt verschoben werden soll.“)
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:1105`: `ctx.tooltip(manager_tooltips_util.LMDefaultMoveWaypoint, nil)`
- `LMSelectSection : Recipe<integer>`: "Select line section <index>." for the given stop number.
  In game: "Select line section {index}." (German: „Linienabschnitt {index} auswählen.“)
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:600`: `ctx.tooltip(manager_tooltips_util.LMSelectSection, details.line.stopNumber)`
- `LMSelectSectionFirst : Recipe<any>`: "Select a segment."
  In game: "Select a segment." (German: „Ein Segment auswählen.“)
- `LMMoveStopTarget : Recipe<LineUtil.StationSelectionDetails>`: "Move stop to <station> [terminal <n>]."
  In game: "{name} terminal {index}", "Move stop to {name}." (German: „{name} Terminal {index}“, „Haltestelle zu {name} verschieben.“)
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:933`: `ctx.tooltip(manager_tooltips_util.LMMoveStopTarget, sDetails)`
- `LMMoveStopTargetError : Recipe<any>`: "Cannot move stop here."
  In game: "Cannot move stop here." (German: „Kann die Haltestelle nicht hierher verschieben.“)
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:935`: `ctx.tooltip(manager_tooltips_util.LMMoveStopTargetError, sDetails)`
- `LMMoveWaypoint : Recipe<LineUtil.WaypointSelectionDetails>`: "Move waypoint <stop number>."
  In game: "Move waypoint {index}." (German: „Wegpunkt {index} verschieben.“)
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:749`: `ctx.tooltip(manager_tooltips_util.LMMoveWaypoint, details.waypoint)`
- `LMSelectDepotForLine : Recipe<LineUtil.DepotAndLineParams>`: "Buy vehicles for <line> at <depot>."
  In game: "Buy vehicles for {lineName} at {depotName}." (German: „Fahrzeuge für Linie {lineName} in {depotName} kaufen.“)
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:684`: `ctx.tooltip(manager_tooltips_util.LMSelectDepotForLine, depotAndLine)`
- `LMMoveWaypointTarget : Recipe<Waypoint>`: "Move waypoint to <edge name>." or "Move waypoint here."
  In game: "Move waypoint to {name}.", "Move waypoint here." (German: „Wegpunkt zu {name} verschieben.“, „Wegpunkt hierher verschieben.“)
    - used 2x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:1112`: `ctx.tooltip(manager_tooltips_util.LMMoveWaypointTarget, apiWaypoint)`
- `LMWaypointTooClose : Recipe<Waypoint>`: "Too close to other waypoint."
  In game: "Too close to other waypoint." (German: „Zu nah an anderem Wegpunkt.“)
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:1128`: `ctx.tooltip(manager_tooltips_util.LMWaypointTooClose, apiWaypoint)`
- `LMWaterWaypointOnLand : Recipe<Waypoint>`: "Ship waypoints must be on water."
  In game: "Ship waypoints must be on water." (German: „Schiffswegpunkte müssen auf dem Wasser liegen.“)
    - used 2x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:722`: `ctx.tooltip(manager_tooltips_util.LMWaterWaypointOnLand, apiWaypoint)`
- `LMAddStop : Recipe<LineUtil.StationSelectionDetails>`: "Add stop at <station> [terminal <n>]."
  In game: "Add stop at {name} terminal {index}.", "Add stop at {name}." (German: „Haltestelle bei {name} Terminal {index} hinzufügen.“, „Haltestelle bei {name} hinzufügen.“)
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:659`: `ctx.tooltip(manager_tooltips_util.LMAddStop, sDetails)`
- `LMAddWaypoint : Recipe<Waypoint>`: "Add waypoint here."
  In game: "Add waypoint here." (German: „Wegpunkt hier hinzufügen.“)
    - used 2x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:702`: `ctx.tooltip(manager_tooltips_util.LMAddWaypoint, apiWaypoint)`
- `LMPromptWaypointModifier : Recipe<Waypoint>`: Hint to hold the precision-mode input action to add a waypoint.
  In game: "(Hold) Add Waypoint" (German: „(Halten) Wegpunkt hinzufügen“)
    - used 2x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:714`: `ctx.tooltip(manager_tooltips_util.LMPromptWaypointModifier, apiWaypoint)`
- `VMDefault : Recipe<integer>`: "Select a line, station or depot to send <count> vehicles to."
  In game: "Select a line, station or depot to send {count} vehicles to." (German: „Linie, Station oder Depot auswählen, an welche {count} Fahrzeuge zugewiesen werden sollen.“)
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:1207`: `ctx.tooltip(manager_tooltips_util.VMDefault, #selectedVehicles)`
- `VMSendToLine : Recipe<SendToLineOrDepotParams>`: "Send <count> vehicle(s) to <line>."
  In game: "Send {count} vehicle to {lineName}. / Send {count} vehicles to {lineName}." (German: „Fahrzeug an {lineName} senden. / {count} Fahrzeuge an {lineName} senden.“)
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:1247`: `ctx.tooltip(manager_tooltips_util.VMSendToLine, sendToLineParams)`
- `VMSendToNewLine : Recipe<integer>`: "Create new line from this stop and assign <count> vehicle(s)."
  In game: "Create new line from this stop and assign {count} vehicle. / Create new line from this stop and assign {count} vehicles." (German: „Neue Linie ab dieser Haltestelle erstellen und Fahrzeug zuweisen. / Neue Linie ab dieser Haltestelle erstellen und {count} Fahrzeuge zuweisen.“)
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:1275`: `ctx.tooltip(manager_tooltips_util.VMSendToNewLine, #selectedVehicles)`
- `VMSendToDepot : Recipe<SendToLineOrDepotParams>`: "Send <count> vehicle(s) to <depot>."
  In game: "Send {count} vehicle to {depotName}. / Send {count} vehicles to {depotName}." (German: „Fahrzeug zu {depotName} senden. / {count} Fahrzeuge zu {depotName} senden.“)
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:1261`: `ctx.tooltip(manager_tooltips_util.VMSendToDepot, sendToDepotParams)`

### ManagerTooltipsUtil.SendToLineOrDepotParams (record)

```teal
record SendToLineOrDepotParams
```

Parameters of the `VMSendToLine` and `VMSendToDepot` tooltips.

- `numVehicles : integer`: Number of vehicles to send.
- `lineOrDepotEntity : Engine.Entity`: Target line or depot.
