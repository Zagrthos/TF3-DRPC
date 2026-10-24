# gui.line_vehicle_mgmt.sound.lvm_sfx

Definition file: `base/tealdef/gui/line_vehicle_mgmt/sound/lvm_sfx.d.tl` (in the game folder).

Sound-effect lists of the line and vehicle manager, loaded from `::/gui/line_vehicle_mgmt/sound/lvm_sfx.gres`.

## LvmSFX (record)

```teal
global record LvmSFX
```

Sound-effect file lists of the line and vehicle manager. Each field is a list of sound files; one is picked at random when played.

Notes:

Defined in `gui/line_vehicle_mgmt/sound/lvm_sfx.gres.lua` and read with
`api.gui.genericRep.get(api.gui.genericRep.find("::/gui/line_vehicle_mgmt/sound/lvm_sfx.gres")).data`.
The feedback list plays `Feedback` with `api.gui.sound.playRandomSoundEffect`.

Example:
```lua
local lvmSFX = api.gui.genericRep.get(api.gui.genericRep.find("::/gui/line_vehicle_mgmt/sound/lvm_sfx.gres")).data
api.gui.sound.playRandomSoundEffect(lvmSFX.Feedback)
```

- `NewLine : {FilePath}`: Played when a new line is created (`new_line.wav`).
- `DeleteLine : {FilePath}`: Played when a line is deleted (`delete_line.wav`).
- `AddStation : {FilePath}`: Played when a stop is added to a line (`add_station.wav`).
- `MoveStation : {FilePath}`: Played when a stop of a line is moved (`move_station.wav`).
- `RemoveStation : {FilePath}`: Played when a stop is removed from a line (`remove_station.wav`).
- `AddWaypoint : {FilePath}`: Played when a waypoint is added to a line (`add_waypoint.wav`).
- `MoveWaypoint : {FilePath}`: Played when a waypoint of a line is moved (`move_waypoint.wav`).
- `RemoveWaypoint : {FilePath}`: Played when a waypoint is removed from a line (`remove_waypoint.wav`).
- `Feedback : {FilePath}`: Played whenever a message is added to a feedback list (`feedback.wav`).
- `BuyVehicle : {FilePath}`: Played when vehicles are bought; three variants (`buy_vehicle_0.wav` to `buy_vehicle_2.wav`).
- `SellVehicle : {FilePath}`: Played when vehicles are sold (`sell_vehicle.wav`).
- `VehicleSetLine : {FilePath}`: Played when a vehicle is assigned to a line (`vehicle_set_line.wav`).
- `ReplaceVehicle : {FilePath}`: Played when vehicles are replaced (`replace_vehicle.wav`).
- `SendToDepot : {FilePath}`: Played when a vehicle is sent to a depot (`send_to_depot.wav`).
