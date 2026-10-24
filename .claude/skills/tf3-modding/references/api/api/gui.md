# api.gui

Definition file: `api/tealdef/api/gui.d.tl` (in the game folder).

Contains all GUI functionality apart from react.

## NinePatch (record)

```teal
global record NinePatch
```
Access: `api.gui.NinePatch`

Nine-patch image description.

- `new : function() : NinePatch`  (call as `api.gui.NinePatch.new(…)`): Creates a new, empty `NinePatch`.
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_react_util.tl:95`: `local backGroundImage1 = api.gui.NinePatch.new()`
- `fileName : string`: Path of the image file (`::/...` resource path, usually a `.tga`).
- `horizontal : {integer}`: Four horizontal pixel coordinates that split the image into left edge, stretchable middle and right edge.
    - notes: The base game writes them as `{ 0, corner, width - corner, width }`, e.g. `horizontal = { 0, 6, 26, 32 }` for a 32-pixel image with 6-pixel corners (`gui/line_vehicle_mgmt/line_react_util.css.lua`). If omitted, the image is used unsplit.
- `vertical : {integer}`: Four vertical pixel coordinates that split the image into top edge, stretchable middle and bottom edge (same layout as `horizontal`).
- `flipHorizontally : boolean`: Draws the image mirrored left to right. Used for vehicle icons that face the other way (`gui/line_vehicle_mgmt/vehicle_react_util.tl`).
- `repeatHorizontally : boolean`: Tiles the middle part horizontally instead of stretching it (see `repeatFactor`). Used for the game bar background (`gui/game_bar/game_bar.css.lua`).
- `repeatVertically : boolean`: Tiles the image vertically instead of stretching it (see `repeatFactor`).
- `repeatFactor : number`: Repeat factor (must be positive).

## StyleSheet (record)

```teal
global record StyleSheet
```
Access: `api.gui.StyleSheet`

Stylesheet used by GUI components.

- `new : function() : StyleSheet`  (call as `api.gui.StyleSheet.new(…)`): Creates a new, empty `StyleSheet`.
    - used 1x in the base game, e.g. `base/content/gui/gui/main/styleutil.tl:4`: `local style =  api.gui.StyleSheet.new()`
- `backgroundColor : Vec4f`: Background colour (RGBA, components 0..1).
- `backgroundColor1 : Vec4f`: Colour of the first background layer; with `backgroundImage1` it tints that image (base game pairs them, e.g. white for an untinted image).
- `backgroundColor2 : Vec4f`: Colour of the second background layer (pairs with `backgroundImage2`).
- `backgroundImage1 : NinePatch`: First background image layer, as a nine-patch.
- `backgroundImage2 : NinePatch`: Second background image layer, drawn in addition to the first (the base game uses it for overlays such as button glyphs and corner decorations).
- `borderWidth : Vec4f`: Border width in pixels, one value per side (CSS files write it as `{ 2, 2, 2, 2 }`).
- `borderColor : Vec4f`: Colour of the border (RGBA, components 0..1).
- `color : Vec4f`: Foreground colour, used for text and to tint images (RGBA, components 0..1).
- `size : Vec2f`: Fixed size of the component in pixels (width, height).
- `padding : Vec4f`: Inner spacing in pixels, one value per side; the first value is the top.
- `fontSize : integer`: Font size in pixels.
- `borderImage : NinePatch`: Nine-patch image drawn as the border / contour of the component.
- `anchorPoint : Vec2f`: Point of the component that is placed at its position, as fractions of its size (`{0.5, 0.5}` = centre, `{0, 0}` = top-left corner).
    - notes: Used by floating / positioned components, e.g. in `gui/context_helper/context_helper.css.lua`.

## Gui (record)

```teal
global record Gui
```
Access: `api.gui`

Main GUI interface

- `fireReactEvent : function(name : string, param : any)`  (call as `api.gui.fireReactEvent(…)`): Fire a react event (same as react.fireEvent() but from outside of react)
    - param `name`: the event name
    - param `param`: the event parameter
    - used 30x in the base game, e.g. `base/content/game_mechanics/game_mechanics/celebrations/celebration_react_util.tl:118`: `api.gui.fireReactEvent("selectEntity", {`
- `fireGuiScriptEvent : function(id : string, name : string, param : any) : any`  (call as `api.gui.fireGuiScriptEvent(…)`): Fire a game gui script event.
This event is meant to be handled by game scripts (guiHandleEvent), not by react.onEvent().
It differs from react.fireEvent in that it allows to return a value (like an error string to prevent the operation)
    - param `id`: the component id
    - param `name`: the event name
    - param `param`: event-specific parameters
    - returns: the result from the event handler

    ```lua
    -- tell game scripts (guiHandleEvent) that a world position was clicked
    api.gui.fireGuiScriptEvent("mainView", "select-world", {
      pos = { world.pos.x, world.pos.y, world.pos.z },
      water = world.water,
    })
    ```

    - used 31x in the base game, e.g. `base/content/gui/gui/construction/construction_react_util.tl:1531`: `local errorMessages = api.gui.fireGuiScriptEvent("", "company.lockPermits", {`
- `spawnEphemeralHudImage : function(iconPath : FilePath, entity : Engine.Entity, color? : Vec4f, position? : Vec3f, overrideDuration? : number, fadeIn? : boolean, fadeOut? : boolean, rev0? : integer)`  (call as `api.gui.spawnEphemeralHudImage(…)`): Spawn a short-lived floating image as a hud-icon
    - param `iconPath`: The icon to be used
    - param `entity`: Optional entity (if not used, you must specify position instead)
    - param `color`: Optional color (if not used, the icon is expected to have color channels)
    - param `position`: Optional position (if not used, position is obtained from the entity)
    - param `overrideDuration`: Optional duration in seconds
    - param `fadeIn`: Optional: Start invisible and fade in (default: false)
    - param `fadeOut`: Optional: Slowly fade out at the end (default: true)
    - param `rev0`: Optional revision0 (if not used, defaults to api.engine.getRevision(entity).num[1])

    ```lua
    -- happy / sad emote above a simulation entity (gui/hud/happy_change_display.script.tl)
    api.gui.spawnEphemeralHudImage("::/gui/main/icons/symbol_emote_happy_colored.tga", simEntity)
    ```

    - used 5x in the base game, e.g. `base/content/gui/gui/hud/greenification_display.script.tl:151`: `api.gui.spawnEphemeralHudImage("::/gui/main/icons/symbol_emote_leaf_colored.tga", entity, nil, api.type.Vec3f.new(pos[1], pos[2], pos[3]), IconLifetime, true)`
- `spawnEphemeralHudImageOnPath : function(iconPath : FilePath, pos0 : Vec3f, pos1 : Vec3f, tan0 : Vec3f, tan1 : Vec3f, speed : number, scale : number, maxDistance : number)`  (call as `api.gui.spawnEphemeralHudImageOnPath(…)`): Spawns a short icon that follows a path
    - param `iconPath`: The icon to be used
    - param `pos0`: The starting tangent of the icon
    - param `pos1`: The end tangent of the icon
    - param `tan0`: Start tangent of the path (the `@param` comments in the game file name it `pos0` by mistake).
    - param `tan1`: End tangent of the path.
    - param `speed`: The speed (in m/s) of the icon
    - param `scale`: A scaling factor for the size of the icon
    - param `maxDistance`: The maximal distance at which the icon will be rendered

    ```lua
    -- cargo icon arcing from pos0 to pos1 (gui/hud/cargo_flow.script.tl)
    local d = api.type.Vec3f.distance(pos0, pos1) / 2
    api.gui.spawnEphemeralHudImageOnPath(cargoType.icon, pos0, pos1,
      api.type.Vec3f.new(0, 0, 1) * d, api.type.Vec3f.new(0, 0, -1) * d, 125, 0.75, 2500.0)
    ```

    - used 1x in the base game, e.g. `base/content/gui/gui/hud/cargo_flow.script.tl:156`: `api.gui.spawnEphemeralHudImageOnPath(cargoType.icon, toVec3f(flow.pos0),`
- `closeAllWindows : function()`  (call as `api.gui.closeAllWindows(…)`): Close all windows
    - used 1x in the base game, e.g. `base/content/gui/gui/main/game.tl:229`: `api.gui.closeAllWindows()`
- `mouse : Mouse`: Mouse input (`Gui.Mouse`).
- `key : Key`: Key input (`Gui.Key`).
- `dnd : DnD`: Drag and drop (`Gui.DnD`).
- `inputAction : InputAction`: Input actions / key bindings (`Gui.InputAction`).
- `byId : ById`: Access to GUI components by id or tag (`Gui.ById`).
- `camera : Camera`: Camera control (`Gui.Camera`).
- `sound : Sound`: Sound effects (`Gui.Sound`).
- `musicPlayer : MusicPlayer`: Music player (`Gui.MusicPlayer`).
- `backend : Backend`: Paradox account backend (`Gui.Backend`).
- `voiceOverManager : VoiceOverManager`: Voice-over playback (`Gui.VoiceOverManager`).
- `byEntity : ByEntity`: Visibility checks for entities (`Gui.ByEntity`).
- `contextHelper : ContextHelper`: Context help data (`Gui.ContextHelper`).
- `construction : Construction`: Construction-tool helpers (`Gui.Construction`).
- `mission : Mission`: Mission zones, markers and cutscenes (`Gui.Mission`).
- `guide : Guide`: Guide system (`Gui.Guide`).
- `game : Game`: Game-session helpers (`Gui.Game`).
- `map : Map`: Map and terrain editor (`Gui.Map`).
- `benchMark : BenchMark`: Timing data (`Gui.BenchMark`).
- `rendering : Rendering`: Rendering helpers (`Gui.Rendering`).
- `debug : Debug`: Debug helpers (`Gui.Debug`).
- `achievements : Achievements`: Achievements and player stats (`Gui.Achievements`).
- `genericRep : GenericRep`: GUI generic resources (`Gui.GenericRep`).
    - used 606x in the base game, e.g. `base/content/game_mechanics/game_mechanics/achievements/achievements.css.lua:9`: `local colorDefault = api.gui.genericRep.get(api.gui.genericRep.find("::/gui/main/default_colors.gres")).data`
- `StyleSheet : StyleSheet`: The `StyleSheet` record, reached as `api.gui.StyleSheet` (use `api.gui.StyleSheet.new()`).
    - notes: The base game usually builds style sheets from a plain table with `styleutil.makeStyle{ ... }` (`gui/main/styleutil.d.tl`) and passes them as `meta.styleSheet` of a builtin component.

    ```lua
    local bg = api.gui.NinePatch.new()
    bg.fileName = data.fileName
    bg.flipHorizontally = data.flippedHorizontally
    
    builtin.ImageView{
      meta = {
        styleSheet = styleutil.makeStyle{
          backgroundImage1 = bg,
          backgroundColor1 = api.type.Vec4f.new(1, 1, 1, 1),
        },
      },
      path = data.cblendFileName,
      scaling = builtin.type.ImageViewScaling.AutoFit,
    }
    ```

- `NinePatch : NinePatch`: The `NinePatch` record, reached as `api.gui.NinePatch` (use `api.gui.NinePatch.new()`).

### Gui.HighlightStyle (record)

```teal
record HighlightStyle
```

Shape of the highlight drawn around a GUI component by `api.gui.byId.setHighlighted`.

Example:
```lua
-- mission/mission_sim.script.tl
api.gui.byId.setHighlighted(idOrTag, true, api.gui.HighlightStyle.Circle, color_util.toVec3(mission_colors.Highlight))
-- remove the highlight again
api.gui.byId.setHighlighted(idOrTag, nil)
```

- `Square : HighlightStyle`: Square highlight around the component.
    - used 1x in the base game, e.g. `base/content/mission/mission/mission_sim.script.tl:1184`: `style = api.gui.HighlightStyle.Square`
- `Circle : HighlightStyle`: Circular highlight (the default of `setHighlighted`).
    - used 1x in the base game, e.g. `base/content/mission/mission/mission_sim.script.tl:1182`: `style = api.gui.HighlightStyle.Circle`
- `Mask : HighlightStyle`: Highlight drawn as a mask of the component's shape.
    - used 1x in the base game, e.g. `base/content/mission/mission/mission_sim.script.tl:1186`: `style = api.gui.HighlightStyle.Mask`

### Gui.SpawnHudImageParams (record)

```teal
record SpawnHudImageParams
```

Description of a floating HUD icon (icon path, optional tint colour, entity or position); the fields mirror the parameters of `spawnEphemeralHudImage`.

- `iconPath : FilePath`: Mandatory path to the icon
- `color : Vec4f`: Optional color. If set, the icon is expected to be grayscale.
- `entity : Engine.Entity`: Optional entity (if not used, specify position)
- `position : Vec3f`: Optional position (if not used, position of the entity is used)

### Gui.SelectionDetails (record)

```teal
record SelectionDetails
```
Access: `api.gui.SelectionDetails`

Carries details about selection event

- `new : function() : SelectionDetails`  (call as `api.gui.SelectionDetails.new(…)`): Creates a new, empty `SelectionDetails`.
- `new : function(copy : SelectionDetails) : SelectionDetails`  (call as `api.gui.SelectionDetails.new(…)`): Creates a new, empty `SelectionDetails`.
- `data : DataType`: variant: nil|Line|Station|Signal|Stop|Waypoint|TransportNetworkEdge|World|Marker|Town

#### Gui.SelectionDetails.Type (record)

```teal
record Type
```
Access: `api.gui.SelectionDetails.Type`

Kind of object under the cursor / selected; the value of `SelectionDetails.data.kind`.

Notes:

Check `data.kind` first, then cast `data` to the matching `Gui.SelectionDetails.*` record.

- `None : Type`: Nothing specific selected (`data` carries no details).
- `Line : Type`: A line segment; `data` is a `SelectionDetails.Line`.
    - used 2x in the base game, e.g. `base/content/gui/gui/hud/hud_react_util.tl:133`: `{ api.gui.SelectionDetails.Type.Line, {`
- `Station : Type`: A station group, station or terminal; `data` is a `SelectionDetails.Station`.
    - used 8x in the base game, e.g. `base/content/gui/gui/entity_window/station_group/station_group.tl:932`: `local stationDetailsThresholds = hud_react_util.findSubEntityThresholds(visibilityThresholds, api.gui.SelectionDetails.Type.Station, true)`
- `Signal : Type`: A signal; `data` is a `SelectionDetails.Signal`.
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/line_util.tl:388`: `elseif apiDetails.data.kind == api.gui.SelectionDetails.Type.Signal then`
- `Stop : Type`: A stop of a line; `data` is a `SelectionDetails.Stop`.
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/line_util.tl:390`: `elseif apiDetails.data.kind == api.gui.SelectionDetails.Type.Stop then`
- `Waypoint : Type`: A waypoint of a line; `data` is a `SelectionDetails.Waypoint`.
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/line_util.tl:392`: `elseif apiDetails.data.kind == api.gui.SelectionDetails.Type.Waypoint then`
- `TransportNetworkEdge : Type`: A point on a transport-network edge (track, street, ...); `data` is a `SelectionDetails.TransportNetworkEdge`.
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/line_util.tl:394`: `elseif apiDetails.data.kind == api.gui.SelectionDetails.Type.TransportNetworkEdge then`
- `World : Type`: A position in the world (terrain or water); `data` is a `SelectionDetails.World`.
    - used 2x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/line_util.tl:396`: `elseif apiDetails.data.kind == api.gui.SelectionDetails.Type.World then`
- `Marker : Type`: A mission marker; `data` is a `SelectionDetails.Marker`.
    - used 2x in the base game, e.g. `base/content/gui/gui/layers/layer_terrain.tl:199`: `elseif apiDetails ~= nil and apiDetails.data ~= nil and apiDetails.data.kind == api.gui.SelectionDetails.Type.Marker then`
- `Town : Type`: A town or one of its districts; `data` is a `SelectionDetails.Town`.
    - used 9x in the base game, e.g. `base/content/gui/gui/construction/constructionHudIconMaster.tl:144`: `if town and param.details.data.kind == api.gui.SelectionDetails.Type.Town then`

#### Gui.SelectionDetails.DataType (interface)

```teal
interface DataType is userdata
```
Access: `api.gui.SelectionDetails.DataType`

Common base of all selection detail records; `kind` tells which one it is.

Example:
```lua
-- in a builtin.Selector onSelect / onHover callback (gui/line_vehicle_mgmt/line_util.tl)
onSelect = function(entity, indices, apiDetails)
  if apiDetails and apiDetails.data then
    if apiDetails.data.kind == api.gui.SelectionDetails.Type.Station then
      local st = apiDetails.data  -- Gui.SelectionDetails.Station
      print(st.stationGroup, st.stationTerminal.station, st.stationTerminal.terminal)
    elseif apiDetails.data.kind == api.gui.SelectionDetails.Type.World then
      local w = apiDetails.data   -- Gui.SelectionDetails.World
      print(w.pos.x, w.pos.y, w.pos.z, w.water)
    end
  end
  return true
end
```

- `kind : Type`: Which kind of details this is; selects the record to cast `data` to.

#### Gui.SelectionDetails.Line (record)

```teal
record Line is DataType
```
Access: `api.gui.SelectionDetails.Line`

Carries details about selection event for a Line.

- `line : Engine.Entity`: The line entity
- `stop : integer`: Stop index where the selected line segment starts
- `waypoint : integer`: Waypoint index identifying the selected subsegment (-1: no waypoints, 0 = begins at stop, numWaypoints = ends at next stop)

#### Gui.SelectionDetails.Station (record)

```teal
record Station is DataType
```
Access: `api.gui.SelectionDetails.Station`

Carries details about selection event for a Station, StationGroup or Terminal

- `new : function() : Station`  (call as `api.gui.SelectionDetails.Station.new(…)`): Creates a new, empty `Station`.
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/line_util.tl:949`: `local apiStation = api.gui.SelectionDetails.Station.new()`
- `stationGroup : Engine.Entity`: The station group entity
- `flatStationTerminalIndex : integer`: Index in the the StationGroup's flattened list of station/terminals
- `stationTerminal : StationTerminal`: Station and terminal indices (zero-based) inside the station group; an index is `-1` when only the group or station, not a terminal, is selected.

#### Gui.SelectionDetails.Signal (record)

```teal
record Signal is DataType
```
Access: `api.gui.SelectionDetails.Signal`

Carries details about selection event for a Signal

- `signalList : Engine.Entity`: The signalList entity
- `signalIndex : integer`: Index of the actual signal in the the SignalList

#### Gui.SelectionDetails.Stop (record)

```teal
record Stop is DataType
```
Access: `api.gui.SelectionDetails.Stop`

Carries details about selection event for a Stop (terminal) inside a line

- `new : function() : Stop`  (call as `api.gui.SelectionDetails.Stop.new(…)`): Creates a new, empty `Stop`.
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/line_util.tl:948`: `local apiStop = api.gui.SelectionDetails.Stop.new()`
- `station : SelectionDetails.Station`: The station / terminal
- `line : Engine.Entity`: The line entity
- `stopIndex : integer`: Index of the stop in the line

#### Gui.SelectionDetails.Waypoint (record)

```teal
record Waypoint is DataType
```
Access: `api.gui.SelectionDetails.Waypoint`

Carries details about selection event for a waypoint inside a line

- `line : Engine.Entity`: The line entity
- `stopIndex : integer`: Index of the stop in the line
- `waypointIndex : integer`: Index of the waypoint in the line (index in stop.waypoints)
- `tag : integer`: Tag of the waypoint; match it against `tag` of the entries in the line stop's `waypoints` to find the waypoint index (`line_util.computeWaypointIndex0FromTag`).

#### Gui.SelectionDetails.TransportNetworkEdge (record)

```teal
record TransportNetworkEdge is DataType
```
Access: `api.gui.SelectionDetails.TransportNetworkEdge`

Carries details about selection event for a Transport Network Edge

- `new : function() : TransportNetworkEdge`  (call as `api.gui.SelectionDetails.TransportNetworkEdge.new(…)`): Creates a new, empty `TransportNetworkEdge`.
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/line_util.tl:965`: `local apiSnap = api.gui.SelectionDetails.TransportNetworkEdge.new()`
- `snap : EdgePos`: Position on the edge closest to the cursor (edge id and parameter along it).

#### Gui.SelectionDetails.World (record)

```teal
record World is DataType
```
Access: `api.gui.SelectionDetails.World`

Carries details about the cursor location in the world.

- `new : function() : World`  (call as `api.gui.SelectionDetails.World.new(…)`): Creates a new, empty `World`.
- `pos : Vec3f`: Position in world coordinates.
- `water : boolean`: Whether the position is on water.

#### Gui.SelectionDetails.Marker (record)

```teal
record Marker is DataType
```
Access: `api.gui.SelectionDetails.Marker`

Carries details about selection event for a Marker.

- `new : function() : Marker`  (call as `api.gui.SelectionDetails.Marker.new(…)`): Creates a new, empty `Marker`.
- `marker : string`: Key of the marker (as given to `api.gui.mission.setMarkerAtPosition` / `setMarkerAtEntity`).
- `fireSelectEntity : boolean`: If true, select the underlying entity of the marker (default = true)
- `pos : Vec3f`: Get the marker position (if it is a position marker)
- `entity : Engine.Entity`: Get the marked entity (if it is an entity marker)

#### Gui.SelectionDetails.Town (record)

```teal
record Town is DataType
```
Access: `api.gui.SelectionDetails.Town`

Carries details about selection event for a Town.

- `new : function() : Town`  (call as `api.gui.SelectionDetails.Town.new(…)`): Creates a new, empty `Town`.
- `townEntity : Engine.Entity`: Entity of the town.
- `districtType : LandUseType`: Land use type of the selected district; `nil` when the town as a whole is selected.
- `districtCenter : Vec3f`: Centre position of the selected district (world coordinates).

### Gui.WorldEvent (record)

```teal
record WorldEvent
```

Parameter of the `select-world` GUI script event, fired when the player clicks a position in the world.

Example:
```lua
-- in a guiHandleEvent handler (campaign mission 05, send_vehicle.tl)
if name == "select-world" then
  if param.water then
    print("water clicked at", param.pos[1], param.pos[2])
  end
end
```

- `pos : {number, number, number}`: Clicked position as `{x, y, z}` in world coordinates.
- `water : boolean`: Whether the clicked position is on water.

### Gui.Mouse (record)

```teal
record Mouse
```
Access: `api.gui.mouse`

Mouse input, reached as `api.gui.mouse`.

Example:
```lua
-- react recipe: close a popup when the user clicks elsewhere (gui/main/callout_react_util.tl)
react.onMouseEvent(function(evt)
  if evt.type == api.gui.mouse.Event.Type.PressedOutside and evt.button == 0 then
    params.onClose()
    return true   -- handled
  end
  return false
end)
```

- `getTerrainPosition : function() : Vec3f`  (call as `api.gui.mouse.getTerrainPosition(…)`): Gets the last valid terrain position pointed by the mouse.
    - returns: coordinates of the terrain position
    - used 2x in the base game, e.g. `base/content/gui/gui/layers/layer_react_util.tl:60`: `local pos =api.gui.mouse.getTerrainPosition()`
- `hasTerrainPosition : function() : boolean`  (call as `api.gui.mouse.hasTerrainPosition(…)`): Returns whether a valid terrain position is currently available.
    - returns: boolean true if the mouse is pointing at the terrain
    - used 1x in the base game, e.g. `base/content/gui/gui/main/game_tooltips.tl:229`: `if not api.gui.mouse.hasTerrainPosition() then`

#### Gui.Mouse.Event (record)

```teal
record Event
```

Represents a mouse event

- `type : Type`: Event type.
- `button : integer`: Mouse button: 0 = left, 1 = middle, 2 = right.
- `x : integer`: Horizontal position of the mouse when event was triggered
- `y : integer`: Vertical position of the mouse when event was triggered
- `xrel : integer`: Relative horizontal movement.
- `yrel : integer`: Relative vertical movement.
- `handled : boolean`: Whether the event was already handled by a child component.

##### Gui.Mouse.Event.Type (record)

```teal
record Type
```

Kind of mouse event (`Gui.Mouse.Event.type`).

- `Pressed : Type`: A mouse button was pressed over the component.
    - used 14x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_react_util.tl:103`: `if evt.type == api.gui.mouse.Event.Type.Pressed then`
- `Released : Type`: A mouse button was released.
    - used 15x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_react_util.tl:106`: `if evt.type == api.gui.mouse.Event.Type.Released then`
- `Clicked : Type`: A mouse button was clicked (pressed and released) on the component.
    - used 6x in the base game, e.g. `base/content/game_mechanics/game_mechanics/celebrations/celebration_react_util.tl:81`: `if evt.type == api.gui.mouse.Event.Type.Clicked and evt.button == 2 then`
- `Entered : Type`: The cursor entered the component (base game uses it for hover states).
    - used 11x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/gui/notification_log.tl:126`: `if evt.type == api.gui.mouse.Event.Type.Entered then`
- `Exited : Type`: The cursor left the component.
    - used 8x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/gui/notification_log.tl:129`: `if evt.type == api.gui.mouse.Event.Type.Exited then`
- `Moved : Type`: The cursor moved over the component.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:2676`: `if (evt.type == api.gui.mouse.Event.Type.Moved) then`
- `Wheel : Type`: The mouse wheel was turned.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/menu_icon_react_util.tl:525`: `if not evt.handled and evt.type == api.gui.mouse.Event.Type.Wheel then`
- `DoubleClicked : Type`: A mouse button was double-clicked.
- `PressedOutside : Type`: A mouse button was pressed outside the component; used to close popups and dialogs.
    - used 6x in the base game, e.g. `base/content/gui/gui/main/callout_react_util.tl:35`: `if evt.type == api.gui.mouse.Event.Type.PressedOutside and evt.button == 0 then`

### Gui.Key (record)

```teal
record Key
```
Access: `api.gui.key`

Keyboard / gamepad key input, reached as `api.gui.key`.

#### Gui.Key.State (record)

```teal
record State
```

Direction of a key event (`InputEvent.dir`).

- `Up : State`: The key was released.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/settings_page.tl:279`: `if evt.data.dir == api.gui.key.State.Up then`
- `Pressed : State`: The key was pressed.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/settings_page.tl:273`: `if evt.data:getKey() == 41 and evt.data.dir == api.gui.key.State.Pressed then -- ESC pressed`

#### Gui.Key.InputEvent (record)

```teal
record InputEvent
```

Raw input event data of a key event.

- `dir : State`: Whether the key was pressed or released.
- `repeated : boolean`: Whether the event is an automatic key repeat.
- `isGamepadEvent : function(InputEvent) : boolean`: Returns whether this is a gamepad event.
- `isKeyboardEvent : function(InputEvent) : boolean`: Returns whether this is a keyboard event.
- `getKey : function(InputEvent) : integer`: Returns the key code.
- `getKeyMod : function(InputEvent) : integer`: Returns key modifiers.

#### Gui.Key.Event (record)

```teal
record Event
```

Represents a key press event.

- `data : InputEvent`: Input data of the key event (direction, key code, modifiers).

    ```lua
    -- builtin component key listener (gui/menu/settings_page.tl)
    meta = {
      keyListener = function(evt)
        if evt.data:getKey() == 41 and evt.data.dir == api.gui.key.State.Pressed then -- ESC
          param.onBack()
        end
        return false
      end,
    }
    ```

- `isPressed : function(Event) : boolean`: Returns whether the key is pressed.

### Gui.DataTransferItem (record)

```teal
record DataTransferItem
```
Access: `api.gui.DataTransferItem`

One item carried by a drag and drop operation, a type resource name plus serializable data.

Example:
```lua
-- vehicle_store_window.tl; the type names a resource of type "drag_and_drop"
local item = api.gui.DataTransferItem.new("::/gui/line_vehicle_mgmt/dnd_train_composition.res",
  { vehicleIndex = vehicleIndex, partIndex = partIndex })
```

- `new : function(ResName, any) : DataTransferItem`  (call as `api.gui.DataTransferItem.new(…)`): Creates a transfer item.
    - param `#1`: Resource name that identifies the item type, e.g. `"::/gui/line_vehicle_mgmt/dnd_line_stop.res"` (a `.res.lua` file with `type = "drag_and_drop"`).
    - param `#2`: Payload; must be serializable (plain tables, numbers, strings).
    - returns: The new item.
    - used 2x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/line_manager_panel.tl:512`: `return api.gui.DataTransferItem.new(DnDLineStopRes, data)`
- `type : ResName`: Resource name that identifies the kind of item; drop targets filter on it.
- `data : any`: type-specific data. Note that this needs to be serializable.

### Gui.DnD (record)

```teal
record DnD
```
Access: `api.gui.dnd`

Drag and drop, reached as `api.gui.dnd`.

Notes:

React code normally does not call it directly but registers handlers with
`react.onDragAndDrop{ grab = ..., filter = ..., drop = ... }`; the base game's
`gui/main/react.lua` translates the `api.gui.dnd` events into those callbacks.

Example:
```lua
react.onDragAndDrop({
  grab = function()
    return { api.gui.DataTransferItem.new(MyDnDRes, { index = i }) }
  end,
  filter = function(typeName, data)
    return typeName == MyDnDRes
  end,
  drop = function(dropCtx)
    for _, item in ipairs(dropCtx:getItems()) do
      print(item.data.index)
    end
    return true
  end,
})
```

- `setCursor : function(cursor : integer)`  (call as `api.gui.dnd.setCursor(…)`): Set cursor while dragging (will get reset when a new drag starts).
    - param `cursor`: the cursor id
    - used 1x in the base game, e.g. `base/content/gui/gui/main/react.lua:713`: `api.gui.dnd.setCursor(cursor)`
- `addTransferData : function(dragStartEvent : Event, item : DataTransferItem)`  (call as `api.gui.dnd.addTransferData(…)`): Add transfer data to the current drag & drop operation.
    - param `dragStartEvent`: drag start event (must be Event.Type.DragStarted)
    - param `item`: transferable item
    - used 1x in the base game, e.g. `base/content/gui/gui/main/react.lua:707`: `api.gui.dnd.addTransferData(event, item, item)`
- `getAllTransferData : function() : {DataTransferItem}`  (call as `api.gui.dnd.getAllTransferData(…)`): Get current drag & drop data.
    - returns: current transfer data list
    - used 2x in the base game, e.g. `base/content/gui/gui/main/react.lua:677`: `return api.gui.dnd.getAllTransferData()`

#### Gui.DnD.Event (record)

```teal
record Event
```

Represents drag & drop event

- `type : Type`: Event type.

##### Gui.DnD.Event.Type (record)

```teal
record Type
```

Kind of drag and drop event.

- `Drop : Type`: Items were dropped on the component.
    - used 1x in the base game, e.g. `base/content/gui/gui/main/react.lua:717`: `elseif event.type == api.gui.dnd.Event.Type.Drop then`
- `DragStarted : Type`: A drag starts on the component; add the dragged items with `addTransferData`.
    - used 1x in the base game, e.g. `base/content/gui/gui/main/react.lua:700`: `if event.type == api.gui.dnd.Event.Type.DragStarted then`
- `DragEntered : Type`: A drag entered the component.
    - used 1x in the base game, e.g. `base/content/gui/gui/main/react.lua:721`: `elseif event.type == api.gui.dnd.Event.Type.DragEntered then`
- `DragExited : Type`: A drag left the component.
    - used 1x in the base game, e.g. `base/content/gui/gui/main/react.lua:725`: `elseif event.type == api.gui.dnd.Event.Type.DragExited then`

### Gui.InputAction (record)

```teal
record InputAction
```
Access: `api.gui.inputAction`

Input actions (key bindings such as `IA_OPTION2`), reached as `api.gui.inputAction`.

- `modifierOnlyActionIsActive : function(inputActionId : string) : boolean`  (call as `api.gui.inputAction.modifierOnlyActionIsActive(…)`): Checks if a modifier-only action is currently active.
    - param `inputActionId`: the input action identifier
    - returns: True if active.
    - used 5x in the base game, e.g. `base/content/gui/gui/construction/construction_react_util.tl:1916`: `local precise = api.gui.inputAction.modifierOnlyActionIsActive("IA_PRECISION_MODE")`
- `resetModifierOnlyActionStates : function()`  (call as `api.gui.inputAction.resetModifierOnlyActionStates(…)`): Resets all modifier-only action states to `false`
    - used 1x in the base game, e.g. `base/content/gui/gui/construction/construction.tl:4693`: `api.gui.inputAction.resetModifierOnlyActionStates()`
- `hasConflicts : function(keyCmdDefinitions : {string : {KeyComboDef}}, inputActionId : string, keyComboDef : KeyComboDef) : string`  (call as `api.gui.inputAction.hasConflicts(…)`): Checks for key binding conflicts.
    - param `keyCmdDefinitions`: Key command definitions.
    - param `inputActionId`: Input action id.
    - param `keyComboDef`: Key combo definition.
    - returns: Conflict description (empty if no conflict).
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/settings_page.tl:213`: `local warning = api.gui.inputAction.hasConflicts(param.appConfig.keyCmdDefinitions, param.inputActionName, currentCombo)`
- `findDuplicatesOf : function(inputActionId : string) : {string}`  (call as `api.gui.inputAction.findDuplicatesOf(…)`): Recursively find list of all IAs that should are duplicates of this one (i.e. duplicateOf == inputActionId and duplicatedKeybindings == true)
    - param `inputActionId`: Input action id.
    - returns: Duplications of the IA
    - used 3x in the base game, e.g. `base/content/gui/gui/menu/settings_page.tl:168`: `local duplicates : {string} = api.gui.inputAction.findDuplicatesOf(param.inputActionName)`
- `getFixedKeyCmdDefs : function(keyCmdDefinitions : {string : {KeyComboDef}}, inputActionId : string, keyComboDef : KeyComboDef) : {string : {KeyComboDef}}`  (call as `api.gui.inputAction.getFixedKeyCmdDefs(…)`): Resolves key binding conflicts and returns updated key command definitions.
    - param `keyCmdDefinitions`: Key command definitions.
    - param `inputActionId`: Input action id.
    - param `keyComboDef`: Key combo definition.
    - returns: Fixed key command definitions  or empty table if change not allowed.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/settings_page.tl:227`: `local keyCmdsFixed = api.gui.inputAction.getFixedKeyCmdDefs(param.appConfig.keyCmdDefinitions, param.inputActionName, currentCombo)`

#### Gui.InputAction.InputActionState (record)

```teal
record InputActionState
```

Enabled state of an input action, returned by the `isEnabled` callback of `react.iaHandler`.

Example:
```lua
react.useInputAction("IA_OPTION2", react.iaHandler(function()
  panelRef:get():focus()
end, function()
  return isListEmpty and api.gui.inputAction.InputActionState.Disabled
    or api.gui.inputAction.InputActionState.Enabled
end))
```

- `Disabled : InputActionState`: disabled, action will not be triggered and no hints or prompts are shown, leaving the keys for other actions
    - used 7x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company.tl:804`: `return groupHasFocusRef:get() and api.gui.inputAction.InputActionState.Disabled`
- `Inactive : InputActionState`: action will not be triggered, but hints and button prompts are generated
    - used 1x in the base game, e.g. `base/content/gui/gui/main/react.lua:669`: `return result and api.gui.inputAction.InputActionState.Enabled or api.gui.inputAction.InputActionState.Inactive`
- `Enabled : InputActionState`: fully enabled, hints and prompts shown (except if disabled/invisible due to hierarchy)
    - used 9x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company.tl:805`: `or api.gui.inputAction.InputActionState.Enabled`

#### Gui.InputAction.InvokeData (record)

```teal
record InvokeData
```

Additional info about the invocation of an input action event

- `status : Status`: status of the action.

##### Gui.InputAction.InvokeData.Status (record)

```teal
record Status
```

Status type of the InputActionInvokeData

- `Triggered : Status`: Start event: when an event key combo first is fully pressed, or when axis values change
    - used 8x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_charts.tl:82`: `if data.status == api.gui.inputAction.InvokeData.Status.Triggered then`
- `Repeat : Status`: When the system reports a repeated key press for this action
    - used 3x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/line_manager_panel.tl:556`: `elseif data.status == api.gui.inputAction.InvokeData.Status.Repeat then`
- `PressedStep : Status`: Invoked every game step, as long as the active is active = pressed
    - used 1x in the base game, e.g. `base/content/gui/gui/main/game.tl:780`: `elseif data.status == api.gui.inputAction.InvokeData.Status.PressedStep then`
- `EndReleased : Status`: "Good" end event: due to a key/button release there no longer is a action combo active
    - used 8x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_charts.tl:84`: `elseif data.status == api.gui.inputAction.InvokeData.Status.EndReleased then`
- `EndAborted : Status`: Component/action became disabled, or another action with higher priority has started
    - used 8x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_charts.tl:86`: `elseif data.status == api.gui.inputAction.InvokeData.Status.EndAborted then`

### Gui.ById (record)

```teal
record ById
```
Access: `api.gui.byId`

Functions that address GUI components by id or tag, reached as `api.gui.byId`.

Example:
```lua
if api.gui.byId.isVisibleRecursive("menu.contexthelper.window") then
  api.gui.byId.resetMovedWindowPosition("menu.contexthelper.window")
end
```

- `setVisibilityByClass : function(idOrTag : string, visible : boolean, idOnly? : boolean)`  (call as `api.gui.byId.setVisibilityByClass(…)`): Shows or hides a component.
visible: nil removes visibility override
    - param `idOrTag`: the component identifier or a tag
    - param `visible`: whether visible or not (nil to reset the rule
    - param `idOnly`: idOnly -> do not consider tags
    - used 2x in the base game, e.g. `base/content/mission/mission/mission_sim.script.tl:1197`: `api.gui.byId.setVisibilityByClass(k, nil)`
- `setEnabled : function(idOrTag : string, enabled : boolean, idOnly? : boolean)`  (call as `api.gui.byId.setEnabled(…)`): Enables or disables a component.
enable: nil re-enables it (and removes the rule)
    - param `idOrTag`: the component identifier or a tag
    - param `enabled`: whether enabled or not (nil sets to enabled and removes the rule)
    - param `idOnly`: idOnly -> do not consider tags
    - used 2x in the base game, e.g. `base/content/mission/mission/mission_sim.script.tl:1166`: `api.gui.byId.setEnabled(k, nil)`
- `setHighlighted : function(idOrTag : string, highlighted? : boolean, style? : HighlightStyle, color? : Vec3f, idOnly? : boolean)`  (call as `api.gui.byId.setHighlighted(…)`): Sets the highlighted state of a component.
    - param `idOrTag`: the component identifier or a tag
    - param `highlighted`: whether highlighted or not (nil sets to not highlighted and removes the rule)
    - param `style`: defaults to HighlightStyle::Circle
    - param `color`: the color of the highlight, defaults to yellow
    - param `idOnly`: idOnly -> do not consider tags
    - used 2x in the base game, e.g. `base/content/mission/mission/mission_sim.script.tl:1176`: `api.gui.byId.setHighlighted(k, nil)`
- `isVisible : function(id : string) : boolean`  (call as `api.gui.byId.isVisible(…)`): Checks if a UI component is visible by its id.
    - param `id`: the component identifier
    - returns: true if the component is visible
    - used 6x in the base game, e.g. `base/content/gui/gui/construction/construction.tl:431`: `if not (params.tabId ~= nil and api.gui.byId.isVisible(params.tabId .. ".tab")) then`
- `isVisibleRecursive : function(id : string) : boolean`  (call as `api.gui.byId.isVisibleRecursive(…)`): Checks if a UI component and all its parents are visible by id.
    - param `id`: the component identifier
    - returns: true if the component and all parents are visible
    - used 6x in the base game, e.g. `base/content/gui/gui/game_bar/game_bar_widgets.tl:139`: `if not api.gui.byId.isVisibleRecursive("cameraManagerWindow") then`
- `setVisible : function(id : string, visible : boolean)`  (call as `api.gui.byId.setVisible(…)`): Sets the visibility of a UI component by its ID.
    - param `id`: the component identifier
    - param `visible`: if the component should be visible
    - used 26x in the base game, e.g. `base/content/gui/gui/construction/construction.tl:1700`: `api.gui.byId.setVisible(constructionParamsWindowId, shouldBeVisible)`
- `resetMovedWindowPosition : function(id : string)`  (call as `api.gui.byId.resetMovedWindowPosition(…)`): Resets a window to its original position if it was moved by the user.
    - param `id`: string id the window identifier
    - used 1x in the base game, e.g. `base/content/gui/gui/context_helper/context_helper_react.tl:831`: `api.gui.byId.resetMovedWindowPosition("menu.contexthelper.window")`
- `getSize : function(id : string) : Vec2f`  (call as `api.gui.byId.getSize(…)`): Get width and height of UI component
    - param `id`: string id the window identifier
    - returns: width and height
    - used 2x in the base game, e.g. `base/content/mission/mission/mission_framework/mission_framework_react.tl:954`: `local constructionMenuSize = api.gui.byId.getSize("menu.construction.react").y`

### Gui.Camera (record)

```teal
record Camera
```
Access: `api.gui.camera`

Camera control, reached as `api.gui.camera`.

Example:
```lua
api.gui.camera.focusEntity(vehicle)        -- jump the view to an entity
api.gui.camera.followEntity(vehicle, false) -- follow it, sliding there
local c = api.gui.camera.getCameraData()   -- {x, y, distance, angle, pitch}
```

- `setManualCamera : function(center : Vec2f, distance : number)`  (call as `api.gui.camera.setManualCamera(…)`): Sets camera to a given position/orientation.
    - param `center`: position on the map where the camera should point to
    - param `distance`: the distance from the position
    - used 1x in the base game, e.g. `base/content/mission/mission/mission_framework/mission_framework_react_util.tl:228`: `api.gui.camera.setManualCamera(api.type.Vec2f.new(location.pos[1], location.pos[2]), location.radius)`
- `getCameraData : function() : Vec5f`  (call as `api.gui.camera.getCameraData(…)`): Obtains the current camera data.
    - returns: the current camera data as a Vec5f {center.x, center.y, distance, angle, pitch}
    - used 9x in the base game, e.g. `base/content/mission/mission/tutorial/tasks/camera_movement.tl:120`: `local cameraData = api.gui.camera.getCameraData()`
- `setCameraData : function(Vec5f)`  (call as `api.gui.camera.setCameraData(…)`): Set the camera data.
    - param `Vec5f`: set the camera data as a Vec5f {center.x, center.y, distance, angle, pitch}
    - used 4x in the base game, e.g. `mods/release/urbangames_campaign_mission_01/content/mission/mission/tasks/everything/everything.tl:85`: `api.gui.camera.setCameraData(camera)`
- `getHeightAboveTerrain : function() : number`  (call as `api.gui.camera.getHeightAboveTerrain(…)`): Obtains the current height above terrain.
    - returns: the height of the camera above the terrain
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/line_util.tl:1120`: `return api.gui.camera.getHeightAboveTerrain() > 500 and gameCtx.metroMode:old()`
- `focusEntity : function(entity : Engine.Entity)`  (call as `api.gui.camera.focusEntity(…)`): Focuses the camera on an entity.
    - param `entity`: the id of the entity that should be focused on
    - used 14x in the base game, e.g. `base/content/game_mechanics/game_mechanics/celebrations/celebration_react_util.tl:116`: `api.gui.camera.focusEntity(param.entity.entity)`
- `focusPosition : function(position : Vec3f, distance : number)`  (call as `api.gui.camera.focusPosition(…)`): Focuses the camera on a position.
    - param `position`: the position that should be focused
    - param `distance`: the distance of the camera from the target
    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/types/line_warning.script.tl:194`: `api.gui.camera.focusPosition(api.type.Vec3f.new(locationOverride[1], locationOverride[2], locationOverride[3]), 15)`
- `takeScreenshot : function(scale : integer)`  (call as `api.gui.camera.takeScreenshot(…)`): Takes a screenshot and stores it in userdata folder
    - param `scale`: image resolution
    - used 1x in the base game, e.g. `base/content/gui/gui/main/free_cam_options.tl:21`: `api.gui.camera.takeScreenshot(1)`
- `getFollowEntity : function() : {Engine.Entity, Engine.Revision}`  (call as `api.gui.camera.getFollowEntity(…)`): Gets the entity currently being followed.
    - returns: the entity being followed (and the entity revision)
- `followEntity : function(entity : Engine.Entity, jump : boolean)`  (call as `api.gui.camera.followEntity(…)`): Sets camera to follow an Entity.
    - param `entity`: the id of the entity that should be followed
    - param `jump`: true if the camera should directly jump to the entity and not slide there
    - used 13x in the base game, e.g. `base/content/gui/gui/entity_window/view_manager.tl:269`: `api.gui.camera.followEntity(params.keyEntity, true)`
- `enterFollowCameraCockpit : function(entity : Engine.Entity)`  (call as `api.gui.camera.enterFollowCameraCockpit(…)`): Enters the cockpit of an entity.
    - param `entity`: entity of the object to enter
    - used 1x in the base game, e.g. `base/content/gui/gui/main/free_cam_options.tl:40`: `api.gui.camera.enterFollowCameraCockpit(toolParam.cockpitCameraForEntity)`
- `leaveFollowCameraCockpit : function()`  (call as `api.gui.camera.leaveFollowCameraCockpit(…)`): Leave the cockpit of an entity.
    - used 1x in the base game, e.g. `base/content/gui/gui/main/free_cam_options.tl:46`: `api.gui.camera.leaveFollowCameraCockpit()`
- `isFreeCameraMouseAttached : function() : boolean`  (call as `api.gui.camera.isFreeCameraMouseAttached(…)`): Tells if mouse is attached to free cam or not
    - returns: if mouse is attached (true) or not (false)
- `toggleFreeCameraMouseAttachment : function()`  (call as `api.gui.camera.toggleFreeCameraMouseAttachment(…)`): Toggles mouse attachment
- `toggleFreeCamera : function()`  (call as `api.gui.camera.toggleFreeCamera(…)`): Toggles free camera
    - used 1x in the base game, e.g. `base/content/gui/gui/main/free_cam_options.tl:42`: `api.gui.camera.toggleFreeCamera()`
- `world2Screen : function(Vec3f) : Vec2i`  (call as `api.gui.camera.world2Screen(…)`): Convert a 3D World position into a position on screen (in pixels)
- `screen2World : function(Vec3f) : Vec3f`  (call as `api.gui.camera.screen2World(…)`): Convert a position on the screen (in pixels) (with depth [0, 1]) into a 3D World position
- `getEye : function() : Vec3f`  (call as `api.gui.camera.getEye(…)`): Get camera origin in world coordinates
- `getCenter : function() : Vec3f`  (call as `api.gui.camera.getCenter(…)`): Get camera center ("look-at" pos) in world coordinates
- `getNearFar : function() : Vec2f`  (call as `api.gui.camera.getNearFar(…)`): Get near and far distances of camera in x and y coordinates repsectively
- `getSize : function() : Vec2i`  (call as `api.gui.camera.getSize(…)`): Width and height of the viewport, in pixels
- `traceToTerrain : function(worldPos : Vec3f, farPos : Vec3f, zOffset : number, numSteps : integer) : Vec3f | nil`  (call as `api.gui.camera.traceToTerrain(…)`): Trace a ray from worldPos to farPos, and find the intersection (if any) with the terrain, offsetted at zOffset
    - param `worldPos`: world position of starting point of ray
    - param `farPos`: world position of end point of ray
    - param `zOffset`: offset to shift the terrain for intersection
    - param `numSteps`: number of steps to perform for finding an intersection. more steps are more precise but cost more, 30 is a good compromise

### Gui.Sound (record)

```teal
record Sound
```
Access: `api.gui.sound`

Sound effects, reached as `api.gui.sound`.

Example:
```lua
api.gui.sound.playRandomSoundEffect({ "::/path/to/effect.wav" })
```

- `letVehicleHorn : function(vehicleEntity : Engine.Entity)`  (call as `api.gui.sound.letVehicleHorn(…)`): Sounds the vehicle horn.
    - param `vehicleEntity`: the vehicle to sound the horn
    - used 2x in the base game, e.g. `base/content/gui/gui/entity_window/sim_person/sim_person.tl:168`: `api.gui.sound.letVehicleHorn(personEntityId)`
- `playRandomSoundEffect : function(paths : {FilePath})`  (call as `api.gui.sound.playRandomSoundEffect(…)`): Plays a single sound effect randomly selected of the provided selection.
    - param `paths`: vector of strings paths of sound effects
    - used 42x in the base game, e.g. `base/content/game_mechanics/game_mechanics/celebrations/celebration_react_util.tl:213`: `api.gui.sound.playRandomSoundEffect(celebrationSFX.Earned)`

### Gui.MusicPlayer (record)

```teal
record MusicPlayer
```
Access: `api.gui.musicPlayer`

Music playback, reached as `api.gui.musicPlayer`.

- `goBack : function()`  (call as `api.gui.musicPlayer.goBack(…)`): Goes back to the previous track in the music player.
    - used 1x in the base game, e.g. `base/content/gui/gui/music_player/music_player.tl:186`: `onClick = function() api.gui.musicPlayer.goBack() end,`
- `goForward : function()`  (call as `api.gui.musicPlayer.goForward(…)`): Goes forward to the next track in the music player.
    - used 3x in the base game, e.g. `base/content/gui/gui/music_player/music_player.tl:196`: `onClick = function() api.gui.musicPlayer.goForward() end,`
- `togglePause : function()`  (call as `api.gui.musicPlayer.togglePause(…)`): Toggles pause/play state of the music player.
    - used 1x in the base game, e.g. `base/content/gui/gui/music_player/music_player.tl:191`: `onClick = function() api.gui.musicPlayer.togglePause() end`
- `getTitle : function() : string`  (call as `api.gui.musicPlayer.getTitle(…)`): Gets the title of the currently playing track.
    - returns: the track title
    - used 1x in the base game, e.g. `base/content/gui/gui/music_player/music_player.tl:40`: `title = api.gui.musicPlayer.getTitle(),`
- `getArtist : function() : string`  (call as `api.gui.musicPlayer.getArtist(…)`): Gets the artist of the currently playing track.
    - returns: the track artist
    - used 1x in the base game, e.g. `base/content/gui/gui/music_player/music_player.tl:39`: `artist = api.gui.musicPlayer.getArtist(),`
- `hasPlaylist : function() : boolean`  (call as `api.gui.musicPlayer.hasPlaylist(…)`): Checks if the music player has tracks loaded.
    - returns: true if the music player has tracks
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/main_menu.tl:936`: `if recipe ~= LicensePage and not api.gui.musicPlayer.hasPlaylist() and api.util.getAppConfig().menuMusicEnabled then`
- `getPlaylistIndex : function() : integer`  (call as `api.gui.musicPlayer.getPlaylistIndex(…)`): Gets the current track index (1-based).
    - returns: the current track index
    - used 1x in the base game, e.g. `base/content/gui/gui/music_player/music_player.tl:44`: `playlistIndex = api.gui.musicPlayer.getPlaylistIndex(),`
- `setTrackListFromPlayList : function(playlist : string, startPaused : boolean, offset? : number)`  (call as `api.gui.musicPlayer.setTrackListFromPlayList(…)`): Sets the current playlist.
The current track will only start after the current track ends. To trigger the new playlist immediately, call `goForward()` right after this call.
    - param `playlist`: the playlist resource name
    - param `startPaused`: if set, does not play the playlist, just prepares it
    - param `offset`: playback offset in seconds (optional)
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/main_menu.tl:914`: `api.gui.musicPlayer.setTrackListFromPlayList("::/music/menu.plist", not api.util.getAppConfig().menuMusicEnabled)`
- `setPlaylist : function(playlist : string)`  (call as `api.gui.musicPlayer.setPlaylist(…)`): Sets the current playlist. Only available while in-game.
The current track will only start after the current track ends. To trigger the new playlist immediately, call `goForward()` right after this call.
    - param `playlist`: the playlist resource name
    - used 2x in the base game, e.g. `base/content/gui/gui/music_player/music_player.tl:209`: `api.gui.musicPlayer.setPlaylist(newValue)`
- `getPlaylist : function() : string`  (call as `api.gui.musicPlayer.getPlaylist(…)`): Gets the current playlist. Only available while in-game.
    - returns: the resource name of the current playlist
    - used 1x in the base game, e.g. `base/content/gui/gui/music_player/music_player.tl:42`: `playlist = api.gui.musicPlayer.getPlaylist(),`
- `isPlaying : function() : boolean`  (call as `api.gui.musicPlayer.isPlaying(…)`): Checks if the music player is currently playing.
    - returns: true if music is playing
    - used 2x in the base game, e.g. `base/content/gui/gui/music_player/music_player.tl:41`: `isPlaying = api.gui.musicPlayer.isPlaying(),`
- `playTrack : function(trackName : string, offset? : number)`  (call as `api.gui.musicPlayer.playTrack(…)`): Plays a track by name.
    - param `trackName`: the name of the track to play
    - param `offset`: playback offset in seconds (optional)
    - used 1x in the base game, e.g. `base/content/mission/mission/mission_sim.script.tl:1212`: `api.gui.musicPlayer.playTrack(newSoundtrack)`
- `playTrackFile : function(filePath : string, offset? : number)`  (call as `api.gui.musicPlayer.playTrackFile(…)`): Plays a specific file that might not be part of the current track list
    - param `filePath`: the path to the track file
    - param `offset`: playback offset in seconds (optional)
    - used 1x in the base game, e.g. `base/content/mission/mission/mission_react_util.tl:314`: `api.gui.musicPlayer.playTrackFile(data.music)`
- `getTrackLength : function() : number`  (call as `api.gui.musicPlayer.getTrackLength(…)`): Gets the length (duration) of the current track
    - returns: the track duration in second
    - used 1x in the base game, e.g. `base/content/gui/gui/music_player/music_player.tl:45`: `trackLength = math.floor(api.gui.musicPlayer.getTrackLength()),`
- `getTrackElapsedTime : function() : number`  (call as `api.gui.musicPlayer.getTrackElapsedTime(…)`): Gets the time of the current track
    - returns: the current time of the current track
    - used 1x in the base game, e.g. `base/content/gui/gui/music_player/music_player.tl:46`: `trackTime = math.floor(api.gui.musicPlayer.getTrackElapsedTime()),`
- `getGain : function() : number`  (call as `api.gui.musicPlayer.getGain(…)`): Gets the current gain (volume) of the music player.
    - returns: the gain value
- `setGain : function(gain : number)`  (call as `api.gui.musicPlayer.setGain(…)`): Sets the gain (volume) of the music player.
    - param `gain`: the gain value
- `setTrackList : function(trackList : {{string, string, boolean}}, enableRepeat : boolean)`  (call as `api.gui.musicPlayer.setTrackList(…)`): Sets the track list for the music player, replaces the current track list.
    - param `trackList`: array of tuples containing track name, file path, and repeat flag
    - param `enableRepeat`: true to enable repeat for the entire list
    - used 3x in the base game, e.g. `base/content/gui/gui/menu/mission_page_content.tl:436`: `api.gui.musicPlayer.setTrackList(trackList, true);`
- `addTrackList : function(trackList : {{string, string, boolean}}, enableRepeat : boolean)`  (call as `api.gui.musicPlayer.addTrackList(…)`): Adds a track list to the music player. This doesn't replace the existing tracks
and once the added tracks are finish, the track previously playing will resume
    - param `trackList`: array of tuples containing track name, file path, and repeat flag
    - param `enableRepeat`: true to enable repeat for the entire list
    - used 1x in the base game, e.g. `base/content/mission/mission/mission_sim.script.tl:1233`: `api.gui.musicPlayer.addTrackList(trackList, enableRepeat)`
- `setContextTag : function(contextTag : string)`  (call as `api.gui.musicPlayer.setContextTag(…)`): Sets the context tag for the music player.
The context tag can be used to change the variant of a playing track.
    - param `contextTag`: the context tag to set
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/main_menu.tl:946`: `api.gui.musicPlayer.setContextTag(state:old())`

### Gui.VoiceOverManager (record)

```teal
record VoiceOverManager
```
Access: `api.gui.voiceOverManager`

Voice-over playback, reached as `api.gui.voiceOverManager`.

- `stop : function()`  (call as `api.gui.voiceOverManager.stop(…)`): Stops the current voice over playing
    - used 4x in the base game, e.g. `base/content/mission/mission/mission_react_util.tl:462`: `api.gui.voiceOverManager.stop()`
- `play : function(voiceOverRes : string, delayInSeconds? : number)`  (call as `api.gui.voiceOverManager.play(…)`): Plays a new voice over
    - param `voiceOverRes`: the file path to the voice over to play
    - param `delayInSeconds`: delay before the voice over starts
    - used 2x in the base game, e.g. `base/content/mission/mission/mission_react_util.tl:83`: `api.gui.voiceOverManager.play(param.instructionData.voiceOver)`
- `isPlaying : function(voiceOverRes : string) : boolean`  (call as `api.gui.voiceOverManager.isPlaying(…)`): Checks whether a specific voice over is playing
    - param `voiceOverRes`: the voice over file to check
    - returns: true if the voice over file is playing
- `isPlayingAny : function() : boolean`  (call as `api.gui.voiceOverManager.isPlayingAny(…)`): Checks whether any voice over is current playing
    - returns: true if any voice over is playing
    - used 6x in the base game, e.g. `base/content/mission/mission/mission_react_util.tl:461`: `if api.gui.voiceOverManager.isPlayingAny() then`
- `getPlaying : function() : ResName`  (call as `api.gui.voiceOverManager.getPlaying(…)`): Checks whether any voice over is current playing and returns the file path if one is playing
    - returns: file path if any voice over is playing

### Gui.ByEntity (record)

```teal
record ByEntity
```
Access: `api.gui.byEntity`

Visibility checks for game entities, reached as `api.gui.byEntity`.

- `isLineEmptyOrVisible : function(Engine.Entity) : boolean`  (call as `api.gui.byEntity.isLineEmptyOrVisible(…)`): Checks if line is empty or visible
    - param `Engine.Entity`: the line entity to check
    - returns: true if the line is empty or visible
    - used 6x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:155`: `if not onlyVisible or api.gui.byEntity.isLineEmptyOrVisible(lineEntity) then`
- `isVisible : function(Engine.Entity) : boolean`  (call as `api.gui.byEntity.isVisible(…)`): Checks if an entity is visible
    - param `Engine.Entity`: the entity to check
    - returns: true if the entity is visible
    - used 10x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:1677`: `if not onlyVisible or api.gui.byEntity.isVisible(depotEAR.entity) then`
- `isVehicleVisible : function(Engine.Entity) : boolean`  (call as `api.gui.byEntity.isVehicleVisible(…)`): Checks if vehicle is visible
    - param `Engine.Entity`: the vehicle entity to check
    - returns: true if the vehicle is visible
    - used 1x in the base game, e.g. `base/content/gui/gui/statistics/statistic_vehicles.tl:419`: `if params.filterShowOnlyVisible and not api.gui.byEntity.isVehicleVisible(key) then`

### Gui.ContextHelper (record)

```teal
record ContextHelper
```
Access: `api.gui.contextHelper`

Data for the context help window, reached as `api.gui.contextHelper`.

- `collectSubjectStates : function(idsOfInterest : {string}) : {string : SubjectState}`  (call as `api.gui.contextHelper.collectSubjectStates(…)`): Collects subject states for given ids
    - param `idsOfInterest`: ids to collect states for
    - used 1x in the base game, e.g. `base/content/gui/gui/context_helper/context_helper_react.tl:822`: `local vis = api.gui.contextHelper.collectSubjectStates(interestingIds)`
- `getIdsOfActiveTool : function() : {string}`  (call as `api.gui.contextHelper.getIdsOfActiveTool(…)`): Ids of the currently active tools in the main view; the context helper uses them to pick the help page for the active tool.
    - returns: List of tool ids (empty when no tool is active).
    - used 2x in the base game, e.g. `base/content/gui/gui/context_helper/context_helper_react.tl:782`: `local tools = api.gui.contextHelper.getIdsOfActiveTool()`

#### Gui.ContextHelper.SubjectState (record)

```teal
record SubjectState
```

State of a GUI component reported by `collectSubjectStates`.

- `Visible : SubjectState`: The component is visible.
    - used 2x in the base game, e.g. `base/content/gui/gui/context_helper/context_helper_react.tl:837`: `if vis[v] == oldVis[v] or vis[v] == api.gui.contextHelper.SubjectState.Visible then`
- `Focused : SubjectState`: The component is visible and has the focus.
    - used 1x in the base game, e.g. `base/content/gui/gui/context_helper/context_helper_react.tl:843`: `if (oldVis[k] == nil or (v == api.gui.contextHelper.SubjectState.Focused and oldVis[k] == api.gui.contextHelper.SubjectState.Visible)) then`

### Gui.Construction (record)

```teal
record Construction
```
Access: `api.gui.construction`

Construction-tool helpers, reached as `api.gui.construction`.

- `setAngle : function(angle : number)`  (call as `api.gui.construction.setAngle(…)`): Sets the default construction angle for constructions.
    - param `angle`: the default construction angle
    - used 1x in the base game, e.g. `base/content/mission/mission/tasks/build_construction/build_construction.tl:51`: `api.gui.construction.setAngle(angle2a + math.pi)`
- `modify : function(construction : Engine.Entity)`  (call as `api.gui.construction.modify(…)`): Modifies a construction
    - param `construction`: the construction entity
- `getBrushIcons : function() : {string}`  (call as `api.gui.construction.getBrushIcons(…)`): Returns brush icons for construction menu
    - returns: paths to brush icons for the construction menu
    - used 2x in the base game, e.g. `base/content/gui/gui/construction/construction_react_util.tl:3124`: `values = api.gui.construction.getBrushIcons(),`
- `provideDustWheneverPossible : function(proposal : Proposal, proposalData : ProposalData)`  (call as `api.gui.construction.provideDustWheneverPossible(…)`): Provides dust effects for proposal
    - param `proposal`: the proposal
    - param `proposalData`: the proposal data
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/double_slip_switch.tl:652`: `api.gui.construction.provideDustWheneverPossible(cmdData.proposal, cmdData.resultProposalData)`
- `getRefundableEntities : function() : {{Engine.Entity, integer}, {Engine.Revision, integer}}`  (call as `api.gui.construction.getRefundableEntities(…)`): Entities whose cost would currently be refunded when they are replaced; pass the result as `refundableEntities` of the command context.
    - returns: Refundable entities with their revisions, or `nil`.

    ```lua
    -- gui/entity_window/bridge_and_tunnel.tl
    local context = api.type.Context.new()
    context.player = api.engine.util.getPlayer()
    local refundables = api.gui.construction.getRefundableEntities()
    if refundables ~= nil then
      context.refundableEntities = refundables
    end
    local cmd = api.cmd.makeWorldBuildProposalCmd(proposal, context, false, true)
    api.cmd.sendCommand(cmd, function(res, success)
      if success then
        api.gui.construction.updateRefundableEntities(resultEntities, res.proposal.proposal)
      end
    end)
    ```

    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/bridge_and_tunnel.tl:189`: `local refundables = api.gui.construction.getRefundableEntities()`
- `updateRefundableEntities : function(resultEntities : {Engine.Entity}, proposal : Proposal.StreetProposal)`  (call as `api.gui.construction.updateRefundableEntities(…)`): Updates refundable entities
    - param `resultEntities`: result entities
    - param `proposal`: the street proposal
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/bridge_and_tunnel.tl:203`: `api.gui.construction.updateRefundableEntities(validResultEntities, cmdData.proposal.proposal)`

### Gui.Mission (record)

```teal
record Mission
```
Access: `api.gui.mission`

Mission and campaign helpers (zones, markers, cutscenes), reached as `api.gui.mission`.

- `getMission : function() : string`  (call as `api.gui.mission.getMission(…)`): Get the current mission
    - returns: the mission name
    - used 7x in the base game, e.g. `base/content/mission/mission/mission_framework/mission_end_window_util.tl:9`: `local missionName = api.gui.mission.getMission()`
- `getCampaign : function() : string`  (call as `api.gui.mission.getCampaign(…)`): Get the current campaign
    - returns: the campaign name
    - used 4x in the base game, e.g. `base/content/mission/mission/mission_framework/mission_end_window_util.tl:272`: `local campaign = api.gui.mission.getCampaign()`
- `setZone : function(key : string, polygon : {Vec2f}, draw : boolean, drawColor : Vec4f, prohibitBuilding : boolean, playAnimation? : boolean, zOffset? : number)`  (call as `api.gui.mission.setZone(…)`): Adds a zone
    - param `key`: the key (name) of the zone
    - param `polygon`: the shape of the zone as a list of 2d position
    - param `draw`: if the zone should be drawn
    - param `drawColor`: the color of the zone
    - param `prohibitBuilding`: if prevented or not to build inside the zone
    - param `playAnimation`: if player zone animation (optional), default to true
    - param `zOffset`: (optional), default to terrain level
    - used 2x in the base game, e.g. `base/content/mission/mission/legacy_ui_util.tl:13`: `api.gui.mission.setZone(key, poly2f, legacyZone.draw, color4f, prohibitBuilding)`
- `setZoneCircle : function(key : string, center : Vec2f, radius : number, draw : boolean, drawColor : Vec4f, prohibitBuilding : boolean, playAnimation? : boolean, zOffset? : number)`  (call as `api.gui.mission.setZoneCircle(…)`): Adds a zone as a cylinder
    - param `key`: the key (name) of the zone
    - param `center`: the center of the zone
    - param `radius`: the radius of the zone
    - param `draw`: if the zone should be drawn
    - param `drawColor`: the color of the zone
    - param `prohibitBuilding`: if prevented or not to build inside the zone
    - param `playAnimation`: if player zone animation (optional), default to true
    - param `zOffset`: (optional), default to terrain level
    - used 1x in the base game, e.g. `base/content/mission/mission/mission_sim.script.tl:1111`: `api.gui.mission.setZoneCircle(zone.key, api.type.Vec2f.new(zone.circle.pos[1], zone.circle.pos[2]),`
- `removeZone : function(key : string)`  (call as `api.gui.mission.removeZone(…)`): Removes a zone
    - param `key`: the key of the zone
    - used 2x in the base game, e.g. `base/content/mission/mission/legacy_ui_util.tl:5`: `api.gui.mission.removeZone(key)`
- `setMarkerAtPosition : function(key : string, pos : Vec3f, type : string, scaling : number, isSelectable? : boolean, fireSelectEntity? : boolean)`  (call as `api.gui.mission.setMarkerAtPosition(…)`): Sets marker at the given position
    - param `key`: the key of the marker
    - param `pos`: the position of the marker
    - param `type`: type of marker (optional)
    - param `scaling`: scaling of marker (optional)
    - param `isSelectable`: whether marker is selectable
    - param `fireSelectEntity`: whether selecting the marker also selects the underlying entity (default is true)
    - used 2x in the base game, e.g. `base/content/mission/mission/legacy_ui_util.tl:24`: `api.gui.mission.setMarkerAtPosition(key, pos, legacyMarker.marker.type)`
- `setMarkerAtEntity : function(key : string, entity : Engine.Entity, type : string, scaling : number, isSelectable? : boolean, fireSelectEntity? : boolean)`  (call as `api.gui.mission.setMarkerAtEntity(…)`): Sets marker at the given entity
    - param `key`: the key of the marker
    - param `entity`: the entity
    - param `type`: optional type
    - param `scaling`: optional scaling
    - param `isSelectable`: whether marker is selectable
    - param `fireSelectEntity`: whether selecting the marker also selects the underlying entity (default is true)
    - used 2x in the base game, e.g. `base/content/mission/mission/legacy_ui_util.tl:21`: `api.gui.mission.setMarkerAtEntity(key, legacyMarker.entity, legacyMarker.type)`
- `removeMarker : function(key : string)`  (call as `api.gui.mission.removeMarker(…)`): Removes a marker
    - param `key`: the key of the marker
    - used 2x in the base game, e.g. `base/content/mission/mission/legacy_ui_util.tl:19`: `api.gui.mission.removeMarker(key)`
- `setMovePathVisualizationAtEntity : function(key : string, entity : Engine.Entity, color? : Vec3f)`  (call as `api.gui.mission.setMovePathVisualizationAtEntity(…)`): Renderes the move path of an entity dynamically
ATTENTION: at the moment, only ships are supported.
    - param `key`: unique key
    - param `entity`: entity
    - param `color`: optional color
    - used 1x in the base game, e.g. `mods/release/urbangames_campaign_mission_05/content/mission/mission/tasks/drilling/send_vehicle.tl:338`: `api.gui.mission.setMovePathVisualizationAtEntity(tostring(ctx.params.vehicle), ctx.params.vehicle, drawColor)`
- `removeMovePathVisualization : function(key : string)`  (call as `api.gui.mission.removeMovePathVisualization(…)`): Removes the move path of an entity dynamically
    - param `key`: unique key
    - used 2x in the base game, e.g. `mods/release/urbangames_campaign_mission_05/content/mission/mission/tasks/drilling/send_vehicle.tl:340`: `api.gui.mission.removeMovePathVisualization(tostring(ctx.params.vehicle))`
- `playCutscene : function(filePath : string, resolveLanguageFolder? : boolean, entityToEntityMap? : {Engine.Entity : Engine.Entity})`  (call as `api.gui.mission.playCutscene(…)`): Plays a cutscene.
    - param `filePath`: the filepath to the cutscene keyframe definition that should be played
    - param `resolveLanguageFolder`: automatically resolve the selected keyframe file by language
    - param `entityToEntityMap`: maps entities from the keyframe file to possible runtime entities
    - used 1x in the base game, e.g. `base/content/mission/mission/tasks/cutscene/cutscene.tl:40`: `api.gui.mission.playCutscene(ctx.params.filePath, true, ctx.params.getEntityToEntityMap and ctx.params.getEntityToEntityMap(taskApi.getTaskState) or nil)`
- `isCutscenePlaying : function() : boolean`  (call as `api.gui.mission.isCutscenePlaying(…)`): Checks if a cutscene is currently playing.
    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/gui/notification_popups.tl:96`: `if api.gui.mission.isCutscenePlaying() then`
- `getCutsceneElapsedTime : function() : number`  (call as `api.gui.mission.getCutsceneElapsedTime(…)`): Return the current cutscene time for syncing mission related systems to the cutscene
    - returns: the actual simulated cutscene time
    - used 3x in the base game, e.g. `base/content/mission/mission/mission_react_util.tl:349`: `elapsedTime = api.gui.mission.getCutsceneElapsedTime()`

### Gui.Game (record)

```teal
record Game
```
Access: `api.gui.game`

Game-session helpers (GUI save data, map editor state, environment), reached as `api.gui.game`.

- `isSavegameUIBlocked : function() : boolean`  (call as `api.gui.game.isSavegameUIBlocked(…)`): Returns if the UI is blocked by a savegame in progress
    - returns: true if the ui is blocked by a savegame
    - used 1x in the base game, e.g. `base/content/gui/gui/main/pause_menu.tl:62`: `return api.gui.game.isSavegameUIBlocked()`
- `setInGameMenuPause : function(pause : boolean)`  (call as `api.gui.game.setInGameMenuPause(…)`): Sets if the game pause menu is open
    - param `pause`: true if the user is in the pause menu
    - used 2x in the base game, e.g. `base/content/gui/gui/main/pause_menu.tl:264`: `api.gui.game.setInGameMenuPause(true)`
- `getGuiSaveData : function(modId : string) : table`  (call as `api.gui.game.getGuiSaveData(…)`): Returns the GUI SaveData
    - param `modId`: the mod to get the user data for
    - returns: the userData as a lua table
    - used 8x in the base game, e.g. `base/content/gui/gui/hud/cargo_flow.script.tl:147`: `local hudFilter = (api.gui.game.getGuiSaveData("") as BaseGUISaveData).hudFilter`
- `setGuiSaveData : function(modId : string, data : table)`  (call as `api.gui.game.setGuiSaveData(…)`): Sets the GUI SaveData
    - param `modId`: the mod to set the user data for
    - param `data`: the user data
    - used 6x in the base game, e.g. `base/content/gui/gui/layers/layer_hud_filter.tl:82`: `api.gui.game.setGuiSaveData("", copy as {string : string})`
- `getEstimatedMaximumGameSpeed : function() : integer`  (call as `api.gui.game.getEstimatedMaximumGameSpeed(…)`): Returns the (estimated) maximum possible game speed based on actual simulation timings.
Only available in GUI thread.
    - returns: maximum game speed (or 0 if not enough data available)
    - used 2x in the base game, e.g. `base/content/gui/gui/main/game.tl:637`: `local maxSpeedup = api.gui.game.getEstimatedMaximumGameSpeed()`
- `isMapEditor : function() : boolean`  (call as `api.gui.game.isMapEditor(…)`): Returns if the game is currently in the map editor
    - returns: true if in the map editor
    - used 6x in the base game, e.g. `base/content/gui/gui/construction/construction_react_util.tl:3305`: `streetEdgeNodeModifier.makePlayerOwnedOnChange = not api.gui.game.isMapEditor()`
- `getDefaultSavegameId : function() : string`  (call as `api.gui.game.getDefaultSavegameId(…)`): Returns the default savegame id
    - returns: the default savegame id
    - used 3x in the base game, e.g. `base/content/gui/gui/main/game.tl:795`: `local name = api.gui.game.getDefaultSavegameId()`
- `setEnvironment : function(environment : ResName)`  (call as `api.gui.game.setEnvironment(…)`): Sets an environment. This value is saved in the GUI SaveData and will be restored on next load.
Changing it will not change the current environment, for that `api.gui.rendering.setEnvironment` needs to be as well called.
    - param `environment`: the resource name for the environment
    - used 1x in the base game, e.g. `base/content/gui/gui/game_bar/game_bar_widgets.tl:665`: `api.gui.game.setEnvironment(newValue as ResName)`
- `getEnvironment : function() : ResName`  (call as `api.gui.game.getEnvironment(…)`): Returns the resource name of the current environment
    - returns: the environment resource name
    - used 1x in the base game, e.g. `base/content/gui/gui/game_bar/game_bar_widgets.tl:639`: `return api.gui.game.getEnvironment()`

### Gui.Guide (record)

```teal
record Guide
```
Access: `api.gui.guide`

The guide system (tutorial "bookmark" buttons attached to GUI components), reached as `api.gui.guide`.

- `trySpawnGuideById : function(targets : {GuideTarget}, tooltip : string, caption : string) : boolean`  (call as `api.gui.guide.trySpawnGuideById(…)`): Tries to attach a guide system button to the first visible target component
    - param `targets`: prioritized list of target components
    - param `tooltip`: text
    - param `caption`: optional caption text
    - used 1x in the base game, e.g. `base/content/mission/mission/guide_system/guide_system_react.tl:256`: `local guideSpawned = api.gui.guide.trySpawnGuideById(apiTargets, desiredGuide.info.name, desiredGuide.info.caption)`
- `trySpawnGuideFree : function(position : FreeGuidePosition, tooltip : string, caption : string) : boolean`  (call as `api.gui.guide.trySpawnGuideFree(…)`): Tries to spawn a free floating guide system button
    - param `tooltip`: text
    - param `caption`: optional caption text
    - used 1x in the base game, e.g. `base/content/mission/mission/guide_system/guide_system_react.tl:246`: `local guideSpawned = api.gui.guide.trySpawnGuideFree(FreePositionTable[desiredGuide.info.freePosition] or api.gui.guide.FreeGuidePosition.Low, desiredGuide.info`
- `completeGuideById : function(targets : {GuideTarget}) : boolean`  (call as `api.gui.guide.completeGuideById(…)`): Completes a guide corresponding to a specific id
    - used 1x in the base game, e.g. `base/content/mission/mission/guide_system/guide_system_react.tl:170`: `api.gui.guide.completeGuideById(apiTargets)`
- `resetGuide : function()`  (call as `api.gui.guide.resetGuide(…)`): Resets the current active guide
    - used 1x in the base game, e.g. `base/content/mission/mission/guide_system/guide_system_react.tl:237`: `api.gui.guide.resetGuide()`

#### Gui.Guide.GuidePosition (record)

```teal
record GuidePosition
```

Gui positioning of the guide "bookmark" with respect to target component

- `Left : GuidePosition`: Guide button to the left of the target component.
    - used 1x in the base game, e.g. `base/content/mission/mission/guide_system/guide_system_react.tl:190`: `["Left"] = api.gui.guide.GuidePosition.Left,`
- `Right : GuidePosition`: Guide button to the right of the target component.
    - used 1x in the base game, e.g. `base/content/mission/mission/guide_system/guide_system_react.tl:189`: `["Right"] = api.gui.guide.GuidePosition.Right,`
- `Above : GuidePosition`: Guide button above the target component (the default).
    - used 1x in the base game, e.g. `base/content/mission/mission/guide_system/guide_system_react.tl:187`: `["Above"] = api.gui.guide.GuidePosition.Above,`
- `Below : GuidePosition`: Guide button below the target component.
    - used 1x in the base game, e.g. `base/content/mission/mission/guide_system/guide_system_react.tl:188`: `["Below"] = api.gui.guide.GuidePosition.Below,`

#### Gui.Guide.FreeGuidePosition (record)

```teal
record FreeGuidePosition
```

Gui positioning of the guide "bookmark" with respect to the entire screen

- `High : FreeGuidePosition`: Free guide button in the upper part of the screen.
    - used 1x in the base game, e.g. `base/content/mission/mission/guide_system/guide_system_react.tl:194`: `["High"] = api.gui.guide.FreeGuidePosition.High,`
- `Low : FreeGuidePosition`: Free guide button in the lower part of the screen (the base game's default).
    - used 2x in the base game, e.g. `base/content/mission/mission/guide_system/guide_system_react.tl:195`: `["Low"] = api.gui.guide.FreeGuidePosition.Low,`

#### Gui.Guide.GuideTarget (record)

```teal
record GuideTarget
```

A target component for the guide system

- `new : function(idOrTag : string, position? : GuidePosition) : GuideTarget`: Creates a guide target.
    - param `idOrTag`: Id or tag of the GUI component to attach the guide to.
    - param `position`: Where to place the guide button; defaults to `GuidePosition.Above`.
    - returns: The new target.

    ```lua
    -- mission/guide_system/guide_system_react.tl
    local targets = { api.gui.guide.GuideTarget.new(target.idOrTag, api.gui.guide.GuidePosition.Below) }
    if not api.gui.guide.trySpawnGuideById(targets, info.name, info.caption) then
      log.verbose("target component not found")
    end
    ```

    - used 2x in the base game, e.g. `base/content/mission/mission/guide_system/guide_system_react.tl:168`: `table.insert(apiTargets, api.gui.guide.GuideTarget.new(target.idOrTag))`
- `idOrTag : string`: Id or tag of the target GUI component.
- `position : GuidePosition`: default is above

### Gui.Rendering (record)

```teal
record Rendering
```
Access: `api.gui.rendering`

Rendering helpers (environment, dust effects, frame interpolation), reached as `api.gui.rendering`.

- `setEnvironment : function(environment : Environment)`  (call as `api.gui.rendering.setEnvironment(…)`): Sets an environment. This will change the current environment, but will not persist the environment resource name.
It might be necessary to as well call `api.gui.game.setEnvironment` if a different environment resource is set.
    - param `environment`: the description for the environment
    - used 3x in the base game, e.g. `base/content/gui/gui/game_bar/game_bar_widgets.tl:555`: `api.gui.rendering.setEnvironment(e)`
- `getEnvironment : function() : Environment`  (call as `api.gui.rendering.getEnvironment(…)`): Returns the current environment data
    - returns: the current environment data
    - used 5x in the base game, e.g. `base/content/gui/gui/game_bar/game_bar_widgets.tl:291`: `local environment = api.gui.rendering.getEnvironment()`
- `spawnDust : function(bbMin : Vec3f, bbMax : Vec3f, transf : Mat4f)`  (call as `api.gui.rendering.spawnDust(…)`): Makes a dust cloud appear
    - param `bbMin`: bounding box min
    - param `bbMax`: bounding box max
    - param `transf`: transformation matrix
    - used 1x in the base game, e.g. `base/content/mission/mission/tasks/click_and_collect_custom_entity/collect_entity.tl:130`: `api.gui.rendering.spawnDust(bi.bbMin, bi.bbMax, modelInstanceList.fatInstances[1].transf)`
- `getFrameTime01 : function() : number`  (call as `api.gui.rendering.getFrameTime01(…)`): Interpolation factor of the current render frame between the previous and the current simulation step.
    - returns: Value from 0 to 1; use it to blend between previous and current positions.

    ```lua
    -- smooth vehicle position for drawing (campaign mission 05, patrol.tl)
    local inst = modelInstanceList.fatInstances[1]
    local pos = api.type.Vec3f.lerp(inst.transf0:getTransl(), inst.transf:getTransl(),
      api.gui.rendering.getFrameTime01())
    ```

    - used 1x in the base game, e.g. `mods/release/urbangames_campaign_mission_05/content/mission/mission/tasks/drilling/patrol.tl:61`: `local frameTime01 = api.gui.rendering.getFrameTime01()`

### Gui.Map (record)

```teal
record Map
```
Access: `api.gui.map`

Map and terrain helpers, reached as `api.gui.map`.

- `setMapSeedText : function(seed : string)`  (call as `api.gui.map.setMapSeedText(…)`): Sets seed for the map. Only available in GUI thread.
    - param `seed`: the seed
    - used 1x in the base game, e.g. `base/content/gui/gui/map_editor/map_editor.tl:753`: `api.gui.map.setMapSeedText(fullState:old().seed)`
- `reloadMaterialIndexManagerPipeline : function()`  (call as `api.gui.map.reloadMaterialIndexManagerPipeline(…)`): Reloads the material index manager pipeline.
    - used 3x in the base game, e.g. `base/content/gui/gui/map_editor/map_editor.tl:754`: `api.gui.map.reloadMaterialIndexManagerPipeline()`
- `openTerrainEditor : function()`  (call as `api.gui.map.openTerrainEditor(…)`): Opens the terrain (node) editor; called by its game-bar toggle.
    - used 1x in the base game, e.g. `base/content/gui/gui/game_bar/game_bar.tl:365`: `api.gui.map.openTerrainEditor()`
- `closeTerrainEditor : function()`  (call as `api.gui.map.closeTerrainEditor(…)`): Closes the terrain (node) editor.
    - used 1x in the base game, e.g. `base/content/gui/gui/game_bar/game_bar.tl:368`: `api.gui.map.closeTerrainEditor()`

### Gui.BenchMark (record)

```teal
record BenchMark
```
Access: `api.gui.benchMark`

Frame and simulation timing data, reached as `api.gui.benchMark`.

- `getBeginFrameTimes : function() : {integer}`  (call as `api.gui.benchMark.getBeginFrameTimes(…)`): Returns the rendering preparation time
- `getEventTimes : function() : {integer}`  (call as `api.gui.benchMark.getEventTimes(…)`): Returns the events processing time
- `getCoreStepTimes : function() : {integer}`  (call as `api.gui.benchMark.getCoreStepTimes(…)`): Returns the gui update time
- `getRenderTimes : function() : {integer}`  (call as `api.gui.benchMark.getRenderTimes(…)`): Returns the rendering time
- `getSwapTimes : function() : {integer}`  (call as `api.gui.benchMark.getSwapTimes(…)`): Returns the rendering finish time
- `getReplicateTimes : function() : {SimTime}`  (call as `api.gui.benchMark.getReplicateTimes(…)`): Returns the sync time between gui and simulation. Only available in GUI thread.
- `getCommandTimes : function() : {SimTime}`  (call as `api.gui.benchMark.getCommandTimes(…)`): Returns commands processing time. Only available in GUI thread.
- `getGameStepTimes : function() : {SimTime}`  (call as `api.gui.benchMark.getGameStepTimes(…)`): Returns the simulation update time. Only available in GUI thread.

#### Gui.BenchMark.SimTime (record)

```teal
record SimTime
```

Simulation time frame along with game time at moment of capture

- `frameTime : integer`: Measured duration of the frame.
- `gameTime : integer`: Game time at the moment of capture.

### Gui.Debug (record)

```teal
record Debug
```
Access: `api.gui.debug`

Debug helpers, reached as `api.gui.debug`.

- `selectEntity : function(entity : Engine.Entity)`  (call as `api.gui.debug.selectEntity(…)`): Set the current debug entity
    - param `entity`: the entity
    - used 5x in the base game, e.g. `base/content/gui/gui/debug_panel/make_entity_debug_panel.tl:150`: `api.gui.debug.selectEntity(comp[v])`

### Gui.Achievements (record)

```teal
record Achievements
```
Access: `api.gui.achievements`

Achievements and player statistics, reached as `api.gui.achievements`.

- `areStatsDisabled : function() : boolean`  (call as `api.gui.achievements.areStatsDisabled(…)`): Checkes whether player stats are disabled
    - returns: true if player stats are disabled
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/achievements/achievements.script.tl:510`: `if not api.gui.achievements.areStatsDisabled() then`
- `unlockAchievement : function(achievement : string)`  (call as `api.gui.achievements.unlockAchievement(…)`): Unlocks an achievement based on its string id
    - param `achievement`: achievement to unlock
    - used 17x in the base game, e.g. `base/content/game_mechanics/game_mechanics/achievements/achievements.script.tl:634`: `api.gui.achievements.unlockAchievement("ACH_FIRST_CARGO_DELIVERED")`
- `isAchievementUnlockable : function(achievement : string) : boolean`  (call as `api.gui.achievements.isAchievementUnlockable(…)`): Checks whether an achievement can be unlocked
    - param `achievement`: achievement to check
    - returns: true if the achievement can be unlocked
    - used 17x in the base game, e.g. `base/content/game_mechanics/game_mechanics/achievements/achievements.script.tl:632`: `if api.gui.achievements.isAchievementUnlockable("ACH_FIRST_CARGO_DELIVERED") then`
- `isAchievementUnlocked : function(achievement : string) : boolean`  (call as `api.gui.achievements.isAchievementUnlocked(…)`): Checks whether an achievement is unlocked (completed)
    - param `achievement`: achievement to check
    - returns: true if the achievement is unlocked
- `updateStat : function(playerStat : string, value : number)`  (call as `api.gui.achievements.updateStat(…)`): Increments a player stat value by the provided value
    - param `playerStat`: the stat id to update
    - param `value`: the value to add to the stat
    - used 8x in the base game, e.g. `base/content/game_mechanics/game_mechanics/achievements/achievements.script.tl:519`: `api.gui.achievements.updateStat("STAT_TRACK_TOTAL_LENGTH", delta)`
- `setStat : function(playerStat : string, value : number)`  (call as `api.gui.achievements.setStat(…)`): Sets a player stat value, replacing the current value by the provided one
    - param `playerStat`: the stat id to update
    - param `value`: the value to add to the stat
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/achievements/achievements.script.tl:585`: `api.gui.achievements.setStat("STAT_TOTAL_INCOME_BILLION", newStatValue)`
- `getStatValue : function(playerStat : string) : number`  (call as `api.gui.achievements.getStatValue(…)`): Gets a player stat value
    - param `playerStat`: the stat id to get
    - returns: the current value of the stat
- `setStatExtraData : function(playerStat : string, data : table)`  (call as `api.gui.achievements.setStatExtraData(…)`): Sets extra data associated with a stat.
Extra data are stored locally in the player profile and allow tracking of more informations if necessary.
    - param `playerStat`: the stat id to set the extra data for
    - param `data`: the extra data to set
    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/achievements/achievements.script.tl:565`: `api.gui.achievements.setStatExtraData("STAT_CLIMATES_TYCOON", extraData)`
- `getStatExtraData : function(playerStat : string) : table`  (call as `api.gui.achievements.getStatExtraData(…)`): Gets extra data associated with a stat.
Extra data are stored locally in the player profile and allow tracking of more informations if necessary.
    - param `playerStat`: the stat id to get the extra data for
    - returns: the extra data
    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/achievements/achievements.script.tl:552`: `local extraData = api.gui.achievements.getStatExtraData("STAT_CLIMATES_TYCOON") as {string:boolean}`
- `setEarnedSFX : function(sfx : {FilePath})`  (call as `api.gui.achievements.setEarnedSFX(…)`): Set sound effect(s) to be played when an achievement is earned.
    - param `sfx`: vector of filepaths pointing to .wav files with according sound effect(s)
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/achievements/achievements.script.tl:166`: `api.gui.achievements.setEarnedSFX(sfxAchievement.Earned or {})`

### Gui.GenericRep (record)

```teal
record GenericRep is ResTypeRep<integer, GenericGameRes>
```
Access: `api.gui.genericRep`

Repository of GUI-side generic resources (`.gres` files of type `gui_res` / `gui_res_overwrite`), reached as `api.gui.genericRep`.

Example:
```lua
local colorDefault = api.gui.genericRep.get(api.gui.genericRep.find("::/gui/main/default_colors.gres")).data
```

- `getAllOfType : function(typeName : string, includeInvisible? : boolean) : {integer}`  (call as `api.gui.genericRep.getAllOfType(…)`): Gets all the Genereic Resource Type of a given type name
Only allows typeName in { "gui_res_overwrite", "gui_res" }, for other types, use `api.res.genericRep`
    - param `typeName`: the type name
    - param `includeInvisible`: whether to include invisible resources

### Gui.Backend (record)

```teal
record Backend
```
Access: `api.gui.backend`

Paradox account and online backend (login, profile, legal documents, third-party account linking), reached as `api.gui.backend`.

- `isInitialized : function() : boolean`  (call as `api.gui.backend.isInitialized(…)`): Whether the online backend is initialized; the mod manager shows an offline message when it is not.
    - returns: `true` when the backend is available.
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/main_menu.tl:585`: `return api.gui.backend.isInitialized()`
- `hasLegalDocuments : function()`  (call as `api.gui.backend.hasLegalDocuments(…)`): Whether there are legal documents the player still has to view; the main menu then shows the licence page first.
    - notes: The definition declares no return value, but the base game uses the result as a boolean (`gui/menu/main_menu.tl`).
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/main_menu.tl:703`: `showLicenseAgreement = api.gui.backend.hasLegalDocuments(),`
- `getLegalDocuments : function() : {string}`  (call as `api.gui.backend.getLegalDocuments(…)`): Types of the legal documents to show; pass each one to `getLegalDocument`.
    - returns: List of document type strings.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/license_page.tl:17`: `local legalDocuments = react.useState(api.gui.backend.getLegalDocuments()) as ReactStateT<{string}>`
- `markLegalDocumentViewed : function(string)`  (call as `api.gui.backend.markLegalDocumentViewed(…)`): Marks a legal document as viewed by the player.
    - param `#1`: Document type, as returned by `getLegalDocuments`.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/license_page.tl:104`: `api.gui.backend.markLegalDocumentViewed(legalDocuments:old()[indexState:old()])`
- `isAccountLoggedIn : function() : boolean`  (call as `api.gui.backend.isAccountLoggedIn(…)`): Whether a Paradox account is logged in.
    - returns: `true` when logged in.
    - used 5x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:1202`: `api.gui.backend.isAccountLoggedIn()`
- `loginAccount : function(username : string, password : string) : PdxFuture<PdxLoginResult>`  (call as `api.gui.backend.loginAccount(…)`): Starts logging in to a Paradox account.
    - param `username`: Account e-mail / user name.
    - param `password`: Account password.
    - returns: Future; poll `getResult()` (it returns `nil` until the result is available).
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/paradox_react_util.tl:1254`: `loginAccountFutureRef:set(api.gui.backend.loginAccount(s.username, s.password))`
- `logoutAccount : function() : PdxFuture<PdxResult>`  (call as `api.gui.backend.logoutAccount(…)`): Logs out of the Paradox account.
    - returns: Future with the result.
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:1268`: `api.gui.backend.logoutAccount()`
- `getAccountInfo : function() : PdxFuture<PdxAccountInfo>`  (call as `api.gui.backend.getAccountInfo(…)`): Requests information about the logged-in account.
    - returns: Future with the account info.
    - used 3x in the base game, e.g. `base/content/gui/gui/menu/paradox_react_util.tl:1033`: `getAccountInfoFutureRef:set(api.gui.backend.getAccountInfo())`
- `getProfile : function(social : boolean) : PdxFuture<PdxProfileResult>`  (call as `api.gui.backend.getProfile(…)`): Requests the player's Paradox profile.
    - param `social`: Which profile to query; the base game passes `true`.
    - returns: Future with the profile result.
    - used 3x in the base game, e.g. `base/content/gui/gui/menu/paradox_react_util.tl:976`: `checkProfileFutureRef:set(api.gui.backend.getProfile(true))`
- `createProfile : function(name : string, social : boolean) : PdxFuture<PdxResult>`  (call as `api.gui.backend.createProfile(…)`): Creates a Paradox profile with the given name.
    - param `name`: Profile name entered by the player.
    - param `social`: The base game passes `true`.
    - returns: Future with the result.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/paradox_react_util.tl:1142`: `createProfileFutureRef:set(api.gui.backend.createProfile(name, true))`
- `renameProfile : function(name : string, social : boolean) : PdxFuture<PdxResult>`  (call as `api.gui.backend.renameProfile(…)`): Renames the Paradox profile.
    - param `name`: New profile name.
    - param `social`: As for `createProfile`.
    - returns: Future with the result.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/paradox_react_util.tl:1610`: `futureRef:set(api.gui.backend.renameProfile(name, true))`
- `getCountries : function() : {PdxCountry}`  (call as `api.gui.backend.getCountries(…)`): Countries / regions selectable when creating an account.
    - returns: List of `PdxCountry` (`name`, `code`); pass `code` to `createAccount`.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/paradox_react_util.tl:543`: `local countries = api.gui.backend.getCountries()`
- `getLegalDocument : function(type : string) : PdxFuture<PdxLegalDocument>`  (call as `api.gui.backend.getLegalDocument(…)`): Requests the content of one legal document.
    - param `type`: Document type, as returned by `getLegalDocuments`.
    - returns: Future with the document (`title`, `text`).

    ```lua
    -- gui/menu/license_page.tl: request, then poll every step
    local future = api.gui.backend.getLegalDocument(api.gui.backend.getLegalDocuments()[1])
    react.onStep(function()
      local doc = future and future:getResult()
      if doc ~= nil then
        future = nil
        title:set(doc.title); text:set(doc.text)
      end
    end)
    ```

    - used 2x in the base game, e.g. `base/content/gui/gui/menu/license_page.tl:26`: `futureRef:set(api.gui.backend.getLegalDocument(legalDocuments:old()[index]))`
- `createAccount : function(email : string, password : string, dateOfBirth : string, countryCode : integer, marketingPermission : boolean) : PdxFuture<PdxResult>`  (call as `api.gui.backend.createAccount(…)`): Creates a new Paradox account.
    - param `email`: E-mail address.
    - param `password`: Password.
    - param `dateOfBirth`: Date of birth, as delivered by the base game's date input.
    - param `countryCode`: Country code from `getCountries`.
    - param `marketingPermission`: Whether the player agrees to marketing e-mails.
    - returns: Future with the result.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/paradox_react_util.tl:421`: `futureRef:set(api.gui.backend.createAccount(s.email, s.password, s.dateOfBirth, s.countryCode, s.marketingPermission))`
- `supportsAccountLinking : function() : boolean`  (call as `api.gui.backend.supportsAccountLinking(…)`): Whether the platform supports linking a third-party (platform) account to the Paradox account.
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/paradox_react_util.tl:1006`: `if api.gui.backend.supportsAccountLinking() then`
- `getThirdPartyLinkState : function() : PdxFuture<PdxThirdPartyLinkState>`  (call as `api.gui.backend.getThirdPartyLinkState(…)`): Requests whether the third-party account is linked.
    - returns: Future with the link state.
    - used 3x in the base game, e.g. `base/content/gui/gui/menu/paradox_react_util.tl:737`: `getThirdPartyLinkStateFutureRef:set(api.gui.backend.getThirdPartyLinkState())`
- `getThirdPartyAutoLoginPromptState : function() : PdxThirdPartyAutoLoginPromptState`  (call as `api.gui.backend.getThirdPartyAutoLoginPromptState(…)`): Current state of the third-party auto-login prompt.
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/paradox_react_util.tl:1038`: `local thirdPartyLoginPromptState = api.gui.backend.getThirdPartyAutoLoginPromptState()`
- `setThirdPartyAutoLoginPromptState : function(state : PdxThirdPartyAutoLoginPromptState)`  (call as `api.gui.backend.setThirdPartyAutoLoginPromptState(…)`): Sets the state of the third-party auto-login prompt (the mod manager resets it to `NotShown` on logout).
    - param `state`: New state, e.g. `api.type.enum.PdxThirdPartyAutoLoginPromptState.NotShown`.
    - used 3x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:1267`: `api.gui.backend.setThirdPartyAutoLoginPromptState(api.type["enum"].PdxThirdPartyAutoLoginPromptState.NotShown)`
- `linkThirdPartyAccount : function() : PdxFuture<PdxResult>`  (call as `api.gui.backend.linkThirdPartyAccount(…)`): Links the third-party account to the logged-in Paradox account.
    - returns: Future with the result.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/paradox_react_util.tl:853`: `linkThirdPartyAccountFutureRef:set(api.gui.backend.linkThirdPartyAccount())`
- `unlinkThirdPartyAccount : function() : PdxFuture<PdxResult>`  (call as `api.gui.backend.unlinkThirdPartyAccount(…)`): Removes the link between the third-party account and the Paradox account.
    - returns: Future with the result.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/paradox_react_util.tl:850`: `linkThirdPartyAccountFutureRef:set(api.gui.backend.unlinkThirdPartyAccount())`
- `loginThirdPartyAccount : function() : PdxFuture<PdxLoginResult>`  (call as `api.gui.backend.loginThirdPartyAccount(…)`): Logs in to Paradox with the linked third-party account.
    - returns: Future with the login result.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/paradox_react_util.tl:919`: `pdxFutureRef:set(api.gui.backend.loginThirdPartyAccount())`

#### Gui.Backend.LegalDocument (record)

```teal
record LegalDocument
```

A legal document (title and text).

- `title : string`: Title of the document.
- `text : string`: Text of the document.
