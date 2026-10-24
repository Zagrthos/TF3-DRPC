# api.engine

Definition file: `api/tealdef/api/engine.d.tl` (in the game folder).

Contains API for reading the game engine state.
This module is available as read-only access to the entire engine state (in particular to the entire entity component system). It can be accessed from both the GUI State and the Engine State.

## Globals

- `Collider` (type): Forward declaration of the `Collider` type, which is defined in `api/type.d.tl`. Used by `Engine.Component.ColliderList`.
- `EngineUtil` (type): Forward declaration of the `EngineUtil` record (defined in `api/engine/util.d.tl`), the type of `api.engine.util`.
- `System` (type): Forward declaration of the `System` record (defined in `api/engine/system.d.tl`), the type of `api.engine.system`.

## Engine (record)

```teal
global record Engine
```
Access: `api.engine`

Main Engine interface

- `Entity : integer`: Entity id, a plain integer. Every object in the game world (vehicle, station, town, edge, the world itself, ...) is an entity with a set of components.
    - notes: Existing entities have non-negative ids. Negative ids (`-1`, `-2`, ...) are used inside build proposals as temporary ids for entities that do not exist yet; the base game's track builder gives new nodes the ids `-3` and `-4` and refers to them from the new edge's `node0` / `node1`. Check with `api.engine.entityExists(entity)` before reading components of an id you stored earlier.
    - used 1823x in the base game, e.g. `base/content/game_mechanics/game_mechanics/achievements/achievements.script.tl:42`: `stationGroupEntity : api.engine.Entity`
- `getComponent : function<T>(Entity, ComponentType) : T`  (call as `api.engine.getComponent(…)`): Obtains a component from an entity, if available.
    - param `entity`: the entity (must be a valid, existing entity)
    - param `type`: the component type to obtain
    - returns: the component if the entity has one, or nil otherwise

    ```lua
    -- read the base edge of a street or track entity; nil if the entity has no such component
    local baseEdge = api.engine.getComponent(entity, api.type.ComponentType.BASE_EDGE)
    if baseEdge then
      local length = api.type.Vec3f.distance(baseEdge.position0, baseEdge.position1)
      print(length)
    end
    ```

    - used 984x in the base game, e.g. `base/content/game_mechanics/game_mechanics/achievements/achievements.script.tl:227`: `local cargoComponent = api.engine.getComponent(cargoEntity, api.type.ComponentType.SIM_CARGO) as api.engine.Component.SimCargo`
- `forEachEntityWithComponent : function(function(Entity), ComponentType)`  (call as `api.engine.forEachEntityWithComponent(…)`): Loop over each entity with a given component.
    - param `fn`: the callback called for each entity with the given component
    - param `componentType`: the component used to filter the entities

    ```lua
    api.engine.forEachEntityWithComponent(function(entity)
      local industry = api.engine.getComponent(entity, api.type.ComponentType.INDUSTRY)
      if not industry.manualDevelopment then
        print(entity, industry.level)
      end
    end, api.type.ComponentType.INDUSTRY)
    ```

    - used 51x in the base game, e.g. `base/content/game_mechanics/game_mechanics/emission/emissions.script.tl:39`: `api.engine.forEachEntityWithComponent(function(entity : Engine.Entity)`
- `forEachEntity : function(function(Entity))`  (call as `api.engine.forEachEntity(…)`): Loop over each entity in the engine.
    - param `callback`: the callback called for each entity
    - used 4x in the base game, e.g. `base/content/mission/mission/mission_savegame_util.tl:29`: `api.engine.forEachEntity(loopFn)`
- `getEntitiesWithComponent : function(ComponentType, filters? : EntityFilters) : {Engine.Entity}`  (call as `api.engine.getEntitiesWithComponent(…)`): Get each entity with a given component.
    - param `componentType`: the component used to filter the entities
    - param `filters`: optional table with filter configuration (`requireOwnedByPlayer` and `filterFn`)
    - used 9x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:1358`: `local allPlayerDepotEntities : {Engine.Entity} = api.engine.getEntitiesWithComponent(api.type.ComponentType.VEHICLE_DEPOT, {`
- `getRevision : function(Entity) : Revision`  (call as `api.engine.getRevision(…)`): Returns the revision of the entity.
    - param `entity`: the entity
    - returns: the entity revision

    ```lua
    -- remember an entity together with its revision ...
    local stored = { entity, api.engine.getRevision(entity).num[1] }
    -- ... and later check that it is still the same entity
    local valid = api.engine.entityExists(stored[1])
      and stored[2] >= api.engine.getRevision(stored[1]).num[1]
    ```

    - used 27x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/explorations/exploration_plane.script.tl:49`: `api.engine.getRevision(customState.townEntity).num[1] ~= customState.townRevision0 then`
- `entityExists : function(Entity) : boolean`  (call as `api.engine.entityExists(…)`): Checks whether an entity exists.
    - param `entity`: the entity
    - returns: true if the entity exists
    - used 159x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/explorations/exploration_plane.script.tl:48`: `if not api.engine.entityExists(customState.townEntity) or`
- `system : System`: Access to the engine systems (`api.engine.system.<name>System`), which offer queries that are not available from single components, e.g. `api.engine.system.gameScriptSystem.getEntityForGameScript(...)`.
- `terrain : UtilTerrain`: Terrain queries (`api.engine.terrain`), e.g. `getHeightAt` and `isValidCoordinate`.
- `mapgen : MapGen`: Map generation helpers (`api.engine.mapgen`), used by the map editor to create towns and industries and their default connections.
- `util : EngineUtil`: Engine utility functions (`api.engine.util`), e.g. `getWorld`, `getPlayer`, `getCalendarDate`.
- `config : Config`: Game configuration (`api.engine.config`) with the config dictionary and the mod parameters.

### Engine.Revision (record)

```teal
record Revision
```

Keeps track of modifications to entities.

- `num : {integer, integer, integer}`: Revision counters of the entity. The base game compares the first value (`api.engine.getRevision(entity).num[1]`) with a stored value to detect whether an entity was replaced or modified since it was remembered.

### Engine.Component (record)

```teal
record Component
```

Namespace of all component records. A component is read with `api.engine.getComponent(entity, api.type.ComponentType.<NAME>)`; the matching constants are listed in `Engine.ComponentType`.

Notes:

Components are read-only snapshots. To change the game state, build a new component value
(`api.type.<Name>.new()`, for example `api.type.Line.new(oldLine)`) and send the matching command
from `api.cmd` (for example `api.cmd.makeLineUpdateCmd`).

Example:
```lua
local world = api.engine.util.getWorld()
local gameTime = api.engine.getComponent(world, api.type.ComponentType.GAME_TIME)
local date = api.engine.util.getCalendarDate(gameTime.gameTime)
print(date.year)
```

- `Station : Station`: Defines a station, i.e. a group of terminals (places where a vehicle stops and loads cargo)
    - used 19x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notifications.script.tl:605`: `local station : Engine.Component.Station = api.engine.getComponent(stationEntity, api.type.ComponentType.STATION)`

#### Engine.Component.Account (record)

```teal
record Account
```

Contains financial data. Usually attached to the player Entity

- `new : function() : Account`: Creates a new, empty `Account`.
- `balance : integer`: Current balance
- `loan : integer`: Current loan

#### Engine.Component.Aircraft (record)

```teal
record Aircraft
```

Data attached to aircraft

- `new : function() : Aircraft`: Creates a new, empty `Aircraft`.
- `reservedTakeOffRunway : boolean`: Has the runway been reserved for takeoff
- `reservedLandingRunway : boolean`: Has the runway been reserved for landing
- `pathIndex : integer`: Edge index on the path
- `reservedFrom : integer`: Edge index on the path from which the reservation starts
- `reservedTo : integer`: Edge index on the path on which the reservation stops

#### Engine.Component.Animal (record)

```teal
record Animal
```

Data attached to animals. Contains information about position and movement

- `new : function() : Animal`: Creates a new, empty `Animal`.
- `worldPosition : Vec3f`: Current location in the game world
- `lookingAt : Vec2f`: Direction in which the animal is oriented
- `targetCoord : Vec2f`: Destination where the animal is heading
- `movementSpeed : number`: The forward speed
- `angularSpeed : number`: The turn speed
- `lastUpdateElapsed : number`: Time since the last update of the animal
- `targetChangedElapsed : number`: Time since the last reorientation of the animal
- `invalidTileElapsed : number`: Time since the animal is in an invalid location
- `movementType : integer`: Index of the current movement type
- `roll : number`: Current roll value
- `scaling : number`: Scale of the animal
- `flock : {Vec3f}`: Offset for each of the flock animals relative to the worldPosition

#### Engine.Component.AssetGroup (record)

```teal
record AssetGroup
```

Data attached to assets, in particular trees.
Assets are grouped together when built together in order to speed up rendering performance.
Usually comes together with a ModelInstanceList

- `new : function() : AssetGroup`: Creates a new, empty `AssetGroup`.

#### Engine.Component.AssetGroupAutoRemove (record)

```teal
record AssetGroupAutoRemove
```

Data attached to assets entities to enable automatic removal when it collides with newly built objects

- `new : function() : AssetGroupAutoRemove`: Creates a new, empty `AssetGroupAutoRemove`.

#### Engine.Component.AudioEmitter (record)

```teal
record AudioEmitter
```

An entity with an AudioEmitter emits a sound source

- `new : function() : AudioEmitter`: Creates a new, empty `AudioEmitter`.
- `emitter : integer`: Audio emitter reference

#### Engine.Component.BaseEdge (record)

```teal
record BaseEdge
```

Type attached to streets and tracks, specifying the street type and other details.
A BaseEdge usually also has a BaseEdgeStreet and a TransportNetwork

- `new : function() : BaseEdge`: Creates a new, empty `BaseEdge`.
- `clone : function(BaseEdge) : BaseEdge`: Returns a copy of the base edge component.
    - returns: A new `BaseEdge` with the same values.
- `type : BaseEdgeType`: Kind of edge (`api.type.enum.BaseEdgeType`): `NORMAL`, `BRIDGE` or `TUNNEL`.
- `typeIndex : integer`: For `BRIDGE` and `TUNNEL` edges, the index of the bridge or tunnel type in `api.res.bridgeTypeRep` / `api.res.tunnelTypeRep` (the base game looks up the name with `api.res.bridgeTypeRep.get(baseEdge.typeIndex)`).
- `objects : {{Engine.Entity, EdgeObjectType}}`: Objects placed on the edge (stops and signals), as pairs of the object entity and its `EdgeObjectType`.
    - notes: `EdgeObjectType` values below 2 are stops (`STOP_LEFT`, `STOP_RIGHT`); `SIGNAL` marks signals. The base game reads the position of a stop from the object's `MODEL_INSTANCE_LIST`.
- `laneConfigs : {LaneConfig}`: The lanes of the edge (each `LaneConfig` has e.g. a `width`); new edges take them from the street template (`streetTemplate.laneConfigs`).
- `laneConfigs_native : Vector<LaneConfig>`: Native vector view of `laneConfigs`, read with `:size()` and `:at(i)` without converting it to a Lua table.
- `roadDevelopmentLocked : boolean`: prevent towns from updating or extending this road
- `node0 : Engine.Entity`: Entity of the node at the start of the edge.
- `node1 : Engine.Entity`: Entity of the node at the end of the edge.
- `position0 : Vec3f`: World position of the start of the edge (at `node0`).
- `position1 : Vec3f`: World position of the end of the edge (at `node1`).
- `tangent0 : Vec3f`: Tangent of the edge curve at its start (`position0`).
- `tangent1 : Vec3f`: Tangent of the edge curve at its end (`position1`), pointing in the direction of travel from `node0` to `node1`.
- `laneConfig : {LaneConfig}`
- `edgeDecorations : {{integer, boolean}}`
- `distance : number`: Spacing between parallel tracks; the base game uses it to compute the total width of a group of parallel tunnel tracks.
- `roadType : RoadType`: Type of road (STREET or TRACK)
- `roadTemplate : ResName`: Road template resource name
- `roadStyle : ResName`: Road style resource name

#### Engine.Component.EdgeObject (record)

```teal
record EdgeObject
```

Contains details about an object attached to a BaseEdge

- `new : function() : EdgeObject`: Creates a new, empty `EdgeObject`.
- `param : number`: Relative location on the edge, from 0 to 1
- `transf : Mat4f`: Position of the object in world coordinates
- `edgeObjectConstruction : ResName`: The resource name of the object (Construction)
- `params : table`: Arbitrary params

#### Engine.Component.BaseEdgeStreet (record)

```teal
record BaseEdgeStreet
```

Contains details about a base edge when it is a street

- `new : function() : BaseEdgeStreet`: Creates a new, empty `BaseEdgeStreet`.
- `precedenceNode0 : PrecedencePreference`: Precedence preference at first node
- `precedenceNode1 : PrecedencePreference`: Precedence preference at second node

#### Engine.Component.BaseNode (record)

```teal
record BaseNode
```

A node in a street graph, e.g. a railroad crossing, a switch, or street crossing

- `new : function() : BaseNode`: Creates a new, empty `BaseNode`.
- `clone : function(BaseNode) : BaseNode`: Returns a copy of the base node component.
    - returns: A new `BaseNode` with the same values.
- `position : Vec3f`: Position of the node in world coordinates

#### Engine.Component.TrafficLightState (record)

```teal
record TrafficLightState
```
Access: `api.type.TrafficLightState`

A signle state (= phase) of the traffic light

- `new : function() : TrafficLightState`  (call as `api.type.TrafficLightState.new(…)`): Creates a new, empty `TrafficLightState`.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/double_slip_switch.tl:572`: `local tls = api.type.TrafficLightState.new()`
- `clone : function(trafficLightState : TrafficLightState) : TrafficLightState`  (call as `api.type.TrafficLightState.clone(…)`): Returns a copy of the traffic light phase.
    - param `trafficLightState`: The phase to copy.
    - returns: A new `TrafficLightState` with the same values.
- `lockedLanes : {integer}`: Indices (0-based) of the lanes that have red during this phase. Indices `0 .. #laneConnections-1` refer to the node's `laneConnections`; the following indices refer to its `crosswalks`.
- `duration : number`: Duration of the phase in seconds.
- `minDuration : number`: Minimum duration of the phase in seconds; only relevant when `canSkip` is true.
- `canSkip : boolean`: Whether the phase may end early or be skipped when its lanes are empty and traffic waits on another phase.

#### Engine.Component.TrafficLightConfig (record)

```teal
record TrafficLightConfig
```
Access: `api.type.TrafficLightConfig`

Traffic light configuration for the node, including phases (states)

- `new : function() : TrafficLightConfig`  (call as `api.type.TrafficLightConfig.new(…)`): Creates a new, empty `TrafficLightConfig`.
- `clone : function(trafficLightConfig : TrafficLightConfig) : TrafficLightConfig`  (call as `api.type.TrafficLightConfig.clone(…)`): Returns a copy of the traffic light configuration, e.g. to edit it before sending it back with a proposal.
    - param `trafficLightConfig`: The configuration to copy.
    - returns: A new `TrafficLightConfig` with the same values.

    ```lua
    local cfg = api.engine.getComponent(node, api.type.ComponentType.BASE_NODE_CONFIG)
    local tl = cfg.trafficLightConfig:clone()
    local proposal = api.engine.util.proposal.createTrafficLightProposal(node, tl, cfg.trafficLightPreference)
    api.cmd.sendCommand(api.cmd.makeWorldBuildProposalCmd(proposal, nil, false, true))
    ```

- `states : {TrafficLightState}`: The phases of the traffic light, in the order they are cycled.
- `trafficLightType : integer`: Traffic light type, as a key of `api.res.trafficLightTypeRep.getAll()`.

#### Engine.Component.BaseNodeConfig (record)

```teal
record BaseNodeConfig
```

A custom configuration for the lanes

- `new : function() : BaseNodeConfig`: Creates a new, empty `BaseNodeConfig`.
- `laneConnections : {LaneConnection}`: Lane connections across the node (each with `segment0`, `lane0`, `segment1`, `lane1`); their order defines the lane indices used in `TrafficLightState.lockedLanes`.
- `crosswalks : {Entity}`: Crosswalk entities at the node; in `TrafficLightState.lockedLanes` they follow after the lane connections.
- `trafficLightPreference : TrafficLightPreference`: Whether the node should have a traffic light (`api.type.enum.TrafficLightPreference`, e.g. `YES` or `NO`).
- `trafficLightConfig : TrafficLightConfig`: Traffic light phases and type of the node. Change it with `api.engine.util.proposal.createTrafficLightProposal`.
- `doubleSlipSwitch : boolean`: Whether the node (a track crossing) is configured as a double slip switch. Toggle it with `api.engine.util.proposal.createDoubleSlipSwitchProposal`.
- `userModifiedLaneConnections : boolean`
- `userModifiedTrafficLightStates : boolean`

#### Engine.Component.BaseNodeTrafficLight (record)

```teal
record BaseNodeTrafficLight
```

Additional traffic light data for a base node

- `new : function() : BaseNodeTrafficLight`: Creates a new, empty `BaseNodeTrafficLight`.
- `trafficLightData : TrafficLightData`
- `state : integer`: Index (0-based) of the current phase in `BaseNodeConfig.trafficLightConfig.states`.
- `time : number`: Time already spent in the current phase, in seconds; the base game shows `time / duration` as progress.
- `stateTimes : {integer}`

#### Engine.Component.BaseParallelStrip (record)

```teal
record BaseParallelStrip
```

Stores groups of ranges (a range is reflected as a BaseEdgeRange)

- `new : function() : BaseParallelStrip`: Creates a new, empty `BaseParallelStrip`.
- `rangeGroups : {{BaseEdgeRange}}`: Groups of edge ranges; each group holds the ranges of parallel edges (e.g. the tracks of a multi-track bridge or tunnel).
- `rangeGroups_native : Vector<Vector<BaseEdgeRange>>`: Native vector view of `rangeGroups`, read with `:size()` and `:at(i)`.

##### Engine.Component.BaseParallelStrip.BaseEdgeRange (record)

```teal
record BaseEdgeRange
```

Stores a fraction of a base edge (BaseEdge)

- `new : function() : BaseEdgeRange`: Creates a new, empty `BaseEdgeRange`.
- `edge : Engine.Entity`: The base edge entity the range belongs to.
- `bounds : {number, number}`: Start and end of the range as curve parameters on the edge (0 = `position0`, 1 = `position1`).

#### Engine.Component.BoundingVolume (record)

```teal
record BoundingVolume
```

Contains the bounding box of an entity

- `new : function() : BoundingVolume`: Creates a new, empty `BoundingVolume`.
- `bbox : Box3f`: The axis aligned bounding box

#### Engine.Component.Bridge (record)

```teal
record Bridge
```

Contains the full information and the entire spline of a bridge, if this was configured to do so

- `new : function() : Bridge`: Creates a new, empty `Bridge`.
- `intervals : {Interval}`: The subsections (intervals) of the bridge.

##### Engine.Component.Bridge.Interval (record)

```teal
record Interval
```

A subsection of a bridge

- `new : function() : Interval`: Creates a new, empty `Interval`.
- `splines : {{Vec3f}}`
- `railingModels : {{integer, Mat4f}}`: Railing models of the interval, as pairs of model id and transformation.

#### Engine.Component.BuildCost (record)

```teal
record BuildCost
```

Stores the costs of an entity (e.g. Constructions or Edges)

- `new : function() : BuildCost`: Creates a new, empty `BuildCost`.
- `buildCost : integer`: Cost of building the entity, in game currency.
- `bulldozeCost : integer`: Cost of bulldozing the entity, in game currency.
- `noCostAtAll : boolean`: If true, building and bulldozing cost nothing (the headquarters construction sets `noCostAtAll = true` in its result).

#### Engine.Component.BulldozerInfo (record)

```teal
record BulldozerInfo
```

Allows to set certain entities to be not bulldozable

- `new : function() : BulldozerInfo`: Creates a new, empty `BulldozerInfo`.
- `locked : boolean`: If true, the entity cannot be bulldozed.

#### Engine.Component.Carriage (record)

```teal
record Carriage
```

Represents a single waggon of a train (a single carriage or engine).

- `new : function() : Carriage`: Creates a new, empty `Carriage`.
- `reversed : boolean`: true if the train travels reversed

#### Engine.Component.CarriageList (record)

```teal
record CarriageList
```

Represents a list of engines and carriages (usually of a train)

- `new : function() : CarriageList`: Creates a new, empty `CarriageList`.
- `carriages : {Engine.Entity}`: all waggons and engines

#### Engine.Component.ColliderList (record)

```teal
record ColliderList
```

A list of colliders of different shapes, with an extra transformation

- `new : function() : ColliderList`: Creates a new, empty `ColliderList`.
- `colliders : {{Collider, Mat4f}}`: a list of colliders and transformation that belong to the entity

#### Engine.Component.Color (record)

```teal
record Color
```

Specifies the color of an entity

- `new : function() : Color`: Creates a new, empty `Color`.
- `color : Vec3f`: the RGB color of the entity

#### Engine.Component.Construction (record)

```teal
record Construction
```

A construction: usually contains stations, depots, town buildings or industries.

- `new : function() : Construction`: Creates a new, empty `Construction`.
- `fileName : ResName`: the name of the construction (or the id in case of dynamic constructions)
- `params : ConstructionParams`: the parameters, passed to the `updateFn`.
- `params_native : NativeLuaTable`: Native table view of `params`, read with `:find(key)` / `:next()` without converting all parameters to a Lua table.
- `transf : Mat4f`: the world transformation of the construction
- `timeBuilt : integer`: the in-game time when the construction was first placed
- `frozenNodes : {Engine.Entity}`: nodes that belong to the construction and cannot be moved
- `frozenEdges : {Engine.Entity}`: edges that belong to the construction and cannot be moved
- `depots : {Engine.Entity}`: depots that belong to the construction
- `stations : {Engine.Entity}`: stations that belong to the construction
- `industries : {Engine.Entity}`: industries that belong to the construction
- `townBuildings : {Engine.Entity}`: town building that belong to the construction
- `subconstructions : {Engine.Entity}`: all subconstructions generated by the construction
- `persistentMetadata : ConstructionInstanceMetadata`: all metadata that is persisted in the component
- `slots : {Slot}`: all slots of the modular construction
- `slotConfig : {string : SlotConfig}`: configuration by slot type
- `dependentSlots : {integer, {integer}}`
- `tag2modelInstance : {string, {{Engine.Entity, integer}}}`
- `entityAndIndex2labelTexts : {{Engine.Entity, integer}, {string}}`
- `tag2segments : {string, {Engine.Entity}}`

##### Engine.Component.Construction.ConstructionParams (record)

```teal
record ConstructionParams
```

The parameters of a construction as passed to its `updateFn` (`params`), including the module selection of modular constructions.

- `new : function() : ConstructionParams`: Creates a new, empty `ConstructionParams`.
- `seed : integer`: Random seed of the construction instance; the base game seeds `math.randomseed` with it to derive stable per-instance values.
- `year : integer`: Game year passed to the `updateFn`; base-game scripts use it to pick era-dependent models.
- `modules : {integer : any}`: Modules of a modular construction, keyed by slot id; each entry has e.g. `name`, `variant` and `metadata`.

#### Engine.Component.CustomState (record)

```teal
record CustomState
```

The internal state of a custom entity state

- `new : function() : CustomState`: Creates a new, empty `CustomState`.
- `state : table`: the internal state as a generic table

#### Engine.Component.EmissionEmitter (record)

```teal
record EmissionEmitter
```

An emission emitter, which emits pollution and/or noise

- `new : function() : EmissionEmitter`: Creates a new, empty `EmissionEmitter`.
- `position : Vec3f`: the position of the emission emitter
- `radius : number`: the radius in which the emitter is applied (usually 0)
- `noisePower : number`: the noise power of the emission emitter
- `pollutionPower : number`: the pollution power of the emission emitter

#### Engine.Component.EmissionGrid (record)

```teal
record EmissionGrid
```
Access: `api.type.EmissionGrid`

The grid containing the emission data

- `new : function() : EmissionGrid`  (call as `api.type.EmissionGrid.new(…)`): Creates a new, empty `EmissionGrid`.
- `X0 : integer`: the bottom left corner x coordinate
- `Y0 : integer`: the bottom left corner y coordinate
- `width : integer`
- `height : integer`
- `gridPointSize : Vec2f`: the resolution of the grid
- `concentrationBuffer : Gridf`
- `averageConcentration : Gridf`
- `type : Type`: enum type for noise (0) or pollution (1)
- `wind : Vec2f`: global wind direction

##### Engine.Component.EmissionGrid.Type (record)

```teal
record Type
```
Access: `api.type.EmissionGrid.Type`

The emission grid type

- `Noise : Type`: 0
    - used 3x in the base game, e.g. `base/content/gui/gui/entity_window/town_building/town_building.tl:168`: `local emission = api.engine.system.emissionGridSystem.getInterpolatedEmission(api.type.Vec2f.new(pos.x, pos.y), average, api.type.EmissionGrid.Type.Noise)`
- `Pollution : Type`: 1
    - used 3x in the base game, e.g. `base/content/gui/gui/layers/layer_pollution.tl:62`: `colorMap.currentEmissionId = api.type.EmissionGrid.Type.Pollution`

#### Engine.Component.Field (record)

```teal
record Field
```

Contains data representing a farm field on the terrain

- `new : function() : Field`: Creates a new, empty `Field`.

#### Engine.Component.GameScript (record)

```teal
record GameScript<T>
```

The internal state of a single gamescript

- `new : function() : GameScript<T>`: Creates a new, empty `GameScript` component.
- `gameScript : ResName`: the gamescript for this entity
- `addedToExistingSavegame : boolean`
- `state : T`: the internal state as a generic table
- `state_native : NativeLuaTable`: Native table view of `state` (`NativeLuaTable`), read with `:find(key)` without copying the whole script state into Lua.
- `eventSubscriptions : any`: the event subscriptions for this gamescript

#### Engine.Component.GameSpeed (record)

```teal
record GameSpeed
```

Information about the current game simulation and calendar speed.

- `new : function() : GameSpeed`: Creates a new, empty `GameSpeed`.
- `speedup : integer`: multiplier for game simulation speed (non-negative, 0 = paused)
- `millisPerDay : integer`: length of an in-game calendar day in milliseconds (affected by speedup, 0 = paused)

#### Engine.Component.GameTime (record)

```teal
record GameTime
```

Information about the current ingame time.

- `new : function() : GameTime`: Creates a new, empty `GameTime`.
- `gameTime : integer`: the current ingame time
- `gameTime0 : integer`: the ingame time when the last update occured
- `dates : {{integer, Date}}`
- `timeOfDaySec : integer`: Current time of day in seconds since midnight (0 to 86399). Set it with `api.cmd.makeGameSetTimeOfDayCmd`.
- `tickCount : integer`: number of simulation ticks, ticks also when the game is paused (once per frame)
- `updateCount : integer`: number of simulation updates, does not tick when the simulation is paused

#### Engine.Component.Industry (record)

```teal
record Industry
```
Access: `api.type.Industry`

Contains information about industries.

- `new : function() : Industry`  (call as `api.type.Industry.new(…)`): Creates a new, empty `Industry`.
- `stockList : Engine.Entity`: stock list entity
- `level : integer`: the current level of the construction
- `closureTimeStamp : integer`: time after which the building will be closed
- `upgradeProgress : number`: the progress towards upgrade/downgrade
- `manualDevelopment : boolean`: set to true if industries do not start to close/stop countdown and do not level up or down
- `construction : Engine.Entity`: the construction entity
- `maxLevel : integer`: the maximum level of the construction

#### Engine.Component.Landmark (record)

```teal
record Landmark
```

Information about landmarks used for path finding.

- `new : function() : Landmark`: Creates a new, empty `Landmark`.
- `transportNetworkEntity : Engine.Entity`: the entity for the landmark
- `active : boolean`: whether the landmark is currently active

#### Engine.Component.LandVehicle (record)

```teal
record LandVehicle
```

Represents a vehicle travelling on roads (either bus or truck)

- `new : function() : LandVehicle`: Creates a new, empty `LandVehicle`.
- `isTram : boolean`: True if the land vehicle is a tram.
- `dyn : DynState`: the fast-changing component of the LandVehicle

##### Engine.Component.LandVehicle.DynState (record)

```teal
record DynState
```

The fast-changing component of a LandVehicle.

- `new : function() : DynState`: Creates a new, empty `DynState`.
- `brakingTimer : number`: time in seconds that counts down. When positive, braking lights are on
- `turnLeft : boolean`: true if the vehicle is turning left
- `turnRight : boolean`: true if the vehicle is turning right

#### Engine.Component.Line (record)

```teal
record Line
```
Access: `api.type.Line`

An line with stops and configurations

Example:
```lua
-- change a line: copy the component, modify it and send a LineUpdate command
local lineData = api.type.Line.new(api.engine.getComponent(lineEntity, api.type.ComponentType.LINE))
lineData.reservationPriority = 2.0
api.cmd.sendCommand(api.cmd.makeLineUpdateCmd(lineEntity, lineData))
```

- `new : function() : Line`  (call as `api.type.Line.new(…)`): Creates a new, empty `Line`.
    - used 7x in the base game, e.g. `base/content/gui/gui/entity_window/line/line.tl:31`: `local lineData =  api.type.Line.new(api.engine.getComponent(params.lineEntityId, api.type.ComponentType.LINE) as Engine.Component.Line)`
- `new : function(Line) : Line`  (call as `api.type.Line.new(…)`): Creates a new, empty `Line`.
    - used 7x in the base game, e.g. `base/content/gui/gui/entity_window/line/line.tl:31`: `local lineData =  api.type.Line.new(api.engine.getComponent(params.lineEntityId, api.type.ComponentType.LINE) as Engine.Component.Line)`
- `stops : {Stop}`: all line stops in order
- `vehicleInfo : LineVehicleInfo`: information about vehicles of the line
- `customFilters : boolean`: True when the player has edited the cargo filters of the line; the base game then no longer configures the stop load settings automatically.
- `reservationPriority : number`: Track reservation priority of the line's vehicles. The base-game line window offers `1.0` (Standard), `2.0` (High) and `3.0` (Very High).

##### Engine.Component.Line.LoadMode (record)

```teal
record LoadMode
```
Access: `api.type.Line.LoadMode`

Type of loading/waiting for the line.

- `LOAD_IF_AVAILABLE : LoadMode`: Load whatever cargo is available, then depart ("Load if Available").
- `FULL_LOAD_ANY : LoadMode`: Wait until the vehicle is full with any of its cargo types ("Full Load (Any)").
- `FULL_LOAD_ALL : LoadMode`: Wait until the vehicle is full with all of its cargo types ("Full Load (All)").
- `LEGACY_UNLOAD_ONLY : LoadMode`: Legacy mode for unloading only; not offered in the base-game line window.

##### Engine.Component.Line.StopConfig (record)

```teal
record StopConfig
```
Access: `api.type.Line.StopConfig`

Configuration details for a line stop

- `new : function(?StopConfig) : StopConfig`  (call as `api.type.Line.StopConfig.new(…)`): Creates a new stop configuration, optionally as a copy of an existing one. Available as `api.type.Line.StopConfig.new`.
    - param `#1`: Optional stop configuration to copy.
    - returns: The new `StopConfig`.
    - used 4x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/cargofilter_window.tl:403`: `local compStopCfg : Engine.Component.Line.StopConfig = api.type.Line.StopConfig.new()`
- `load : {boolean}`: for each cargo type, if it should be loaded
- `maxLoad : {number}`: for each cargo type, its max allowed fraction of total capacity (from 0 to 1)
- `forceUnload : boolean`: whether to unload cargo at compatible input-stocks, even if there is no space in that stock (cargo will be destroyed)
- `destroyForConfigChange : boolean`: whether to destroy cargo to allow changing the load config of a vehicle (even without compatible input-stock).
This only happens in case a different cargo type is available to be loaded up at this stock.
- `destroyForRefresh : boolean`: whether to destroy cargo that was originally picked up by the vehicle at this output-stock (even without compatible input-stock).
This only happens in case the cargo did a full round trip and new cargo of the same type is available to be loaded up at this stock.

##### Engine.Component.Line.Stop (record)

```teal
record Stop
```
Access: `api.type.Line.Stop`

Configuration for a line stop

- `new : function() : Stop`  (call as `api.type.Line.Stop.new(…)`): Creates a new, empty `Stop`.
    - used 2x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/line_util.tl:171`: `local newStop : Engine.Component.Line.Stop = api.type.Line.Stop.new()`
- `stationGroup : Engine.Entity`: the station group entity to which the line stop belongs
- `station : integer`: the station (within the station group)
- `terminal : integer`: the terminal (within the station) for the stop
- `alternativeTerminals : {StationTerminal}`: Alternative terminals for the stop
- `loadMode : LoadMode`: the configuration of what to load and not to load
- `minWaitingTime : number`: minimum waiting time at this stop
- `maxWaitingTime : number`: maximum waiting time at this stop
- `maxAdditionalWaitingTime : number`: maximum additional waiting time (after unloading/loading) at this stop
- `waypoints : {Waypoint}`: all waypoints after the stop
- `stopConfig : StopConfig`: configuration how much of what cargo type to load and unload

#### Engine.Component.LogBook (record)

```teal
record LogBook
```

A LogBook contains information about the evolution of a variable (e.g. amounts of money) over time.
Be caucious when working with this component, as it can grow quite large over time and lua performance might not be ideal.
Prefer the native utilities in `api.engine.util.logbook` whenever possible.

- `new : function() : LogBook`: Creates a new, empty `LogBook`.
- `refEntity2name2log : {Engine.Entity : {string : Log}}`: some logbooks hold logs relating to other entries (for example "target town entity" where carogo is delivered)
- `name2log : {string : Log}`: A map from a key to a Log
- `name2log_native : Map<string, Log>`: Native map view of `name2log`; look up a log with `:find(name)`.

    ```lua
    -- time of the last delivery to an industry (from the base game's industry closure logic)
    local logBook = api.engine.getComponent(industry.stockList, api.type.ComponentType.LOG_BOOK)
    local log = logBook.name2log_native:find("itemsConsumed")
    if log and log.times_native:size() > 0 then
      print(log.times_native:at(log.times_native:size()))
    end
    ```

##### Engine.Component.LogBook.Log (record)

```teal
record Log
```

A Log is an entry into a LogBook and keeps track of the evolution of data in time.

- `new : function() : Log`: Creates a new, empty `Log`.
- `times : {integer}`: Game times (milliseconds) of the entries, parallel to `values`.
- `values : {integer}`: Logged values, one per entry in `times`.
- `times_native : Vector<integer>`: Native vector view of `times`, read with `:size()` and `:at(i)`.
- `values_native : Vector<integer>`: Native vector view of `values`, read with `:size()` and `:at(i)`.

#### Engine.Component.LotList (record)

```teal
record LotList
```

Information about ground textures that are paining the terrain

- `new : function() : LotList`: Creates a new, empty `LotList`.
- `lots : {Lot}`: a list of lots for the entity

##### Engine.Component.LotList.Lot (record)

```teal
record Lot
```

Actual information of a LotList

- `new : function() : Lot`: Creates a new, empty `Lot`.
- `vertices : {Vec2f}`: list vertices of the lot
- `texCoords : {Vec2f}`: list of texture coordinates for the lot (same length as vertices)
- `triangles : {integer}`: alternative to vertices, a list of triangles can be specified instead (must be a multiple of 3)
- `texKey : ResName`: the ground texture key to use
- `solid : boolean`

#### Engine.Component.MaintenanceCost (record)

```teal
record MaintenanceCost
```

Contains the maintenance cost of objects

- `new : function() : MaintenanceCost`: Creates a new, empty `MaintenanceCost`.
- `maintenanceCost : integer`: the maintenance cost of an object
- `accumulatedCost : {{Carrier, Carrier} : integer}`

#### Engine.Component.ModelInstanceList (record)

```teal
record ModelInstanceList
```

Contains all models rendered by the entity

- `new : function() : ModelInstanceList`: Creates a new, empty `ModelInstanceList`.
- `fatInstances : {ModelInstance}`: A list of models with a full transform attached
- `thinInstances : {ThinModelInstance}`: A list of models with very simple data attached
- `fatInstances_native : Vector<ModelInstance>`: native variant for more performant access
- `thinInstances_native : Vector<ThinModelInstance>`: native variant for more performant access
- `dynamic : boolean`

#### Engine.Component.ModelPerson (record)

```teal
record ModelPerson
```

Additional data for persons walking in the world

- `new : function() : ModelPerson`: Creates a new, empty `ModelPerson`.
- `path : ExtPath`: the current path of the person
- `pathPos : PathPos`: the current position on the path
- `stopped : boolean`: whether a person is currently not walking
- `topSpeed : number`: the maximal speed of a person
- `speed : number`: the current speed of a person
- `brakeDecel : number`: the current deceleration speed of persons
- `offset : number`: an offset perpendicular to the path
- `position : Vec3f`: the position in the world
- `movementType : integer`: the animation used to move the person
- `dist : number`: distance from the target
- `dist0 : number`: distance from the target (last frame)
- `stateTime : integer`: the animation timer
- `startWaitTime : number`: waiting beside a car (before getting in, after coming out) start
- `endWaitTime : number`: waiting in front of a car (before getting in, after coming out) end

#### Engine.Component.ModelPersonIdle (record)

```teal
record ModelPersonIdle
```

Additional data for idle person

- `new : function() : ModelPersonIdle`: Creates a new, empty `ModelPersonIdle`.
- `arrivalTime : integer`: the time since the person stopped

#### Engine.Component.MovePath (record)

```teal
record MovePath
```

Represents a path a vehicle is currently following

- `new : function() : MovePath`: Creates a new, empty `MovePath`.
- `path : Path`: a series of edges the vehicle is following
- `endParam : number`
- `endPos : number`
- `state : VehicleState`: Current movement state of the vehicle (`VehicleState`).
- `blocked : integer`
- `allowEarlyArrival : boolean`
- `reverse : boolean`: True if the vehicle travels the path in reverse.
- `servedFrom : integer`
- `servedTo : integer`
- `occupiedBackIndex : integer`
- `occupiedFrontIndex : integer`
- `dyn : DynState`: the fast-changing component of the position on the path
- `dyn0 : DynState`: the dyn state at the beginning of the simulation frame (optional)

##### Engine.Component.MovePath.DynState (record)

```teal
record DynState
```

The fast-changing component of the position on a path.

- `new : function() : DynState`: Creates a new, empty `DynState`.
- `pathPos : PathPos`: the position on the path
- `speed : number`: Current speed of the vehicle along the path.
- `brakeDecel : number`: Current braking deceleration of the vehicle.
- `accel : number`: Current acceleration of the vehicle.
- `timeUntilAccel : number`
- `timeStanding : number`
- `timeToIgnore : number`
- `approachingStation : boolean`: True while the vehicle is approaching a station.

#### Engine.Component.MovePathAircraft (record)

```teal
record MovePathAircraft
```
Access: `api.type.MovePathAircraft`

Movement path and flight/sailing state of an air or water vehicle (component type `MOVE_PATH_AIRCRAFT`).

Notes:

The path consists of three parts: `path0` (network path before the free movement), `path1`
(free movement, a list of curve geometries such as the result of `api.engine.util.pathfinding.findDubinsPath`)
and `path2` (network path after it). `pathState` tells which part is currently followed and
`pathPos` is the position on it.

Custom air vehicles are created or moved with `api.cmd.makeCustomVehicleCreateOrUpdateCmd`,
which takes a `MovePathAircraft`; per its documentation `path0` and `path2` must be empty there.

Example:
```lua
-- from the base game's exploration plane script
local mpa = api.type.MovePathAircraft.new()
mpa.path1 = api.engine.util.pathfinding.findDubinsPath(startPos, startDir, targetPos, targetDir, turnRadius)
mpa.aircraftState.cruiseAlt = targetPos.z
mpa.aircraftState.dyn.alt = targetPos.z
mpa.aircraftState.dyn.velocity = api.type.Vec2f.new(200, 0)
mpa.aircraftState.dyn.thrust = 60000
api.cmd.sendCommand(api.cmd.makeCustomVehicleCreateOrUpdateCmd(nil, api.type.enum.Carrier.AIR, {{modelId, false}}, mpa))
```

- `new : function() : MovePathAircraft`  (call as `api.type.MovePathAircraft.new(…)`): Creates a new, empty `MovePathAircraft`.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/explorations/exploration_plane.script.tl:91`: `local mpa = api.type.MovePathAircraft.new()`
- `path0 : Path`: First part of the path, a transport network path before the free movement.
- `path1 : {{EdgeGeometry, boolean}}`: Free-movement part of the path, a list of curve geometries with a direction flag (as returned by `api.engine.util.pathfinding.findDubinsPath`).
- `path2 : Path`: Last part of the path, a transport network path after the free movement.
- `pathPos : PathPos`: Current position on the path part selected by `pathState`.
- `pathPos0 : PathPos`
- `state : VehicleState`: Current movement state of the vehicle (`VehicleState`).
- `pathState : PathState`: Which path part (`path0`, `path1` or `path2`) is currently followed.
- `speed : number`: Current speed of the vehicle.
- `speed0 : number`
- `brakeDecel : number`
- `aircraftState : AircraftState`: State specific to aircraft (flight dynamics, altitude, landing gear).
- `shipState : ShipState`: State specific to ships.

##### Engine.Component.MovePathAircraft.AircraftState (record)

```teal
record AircraftState
```
Access: `api.type.MovePathAircraft.AircraftState`

Additional data for aircraft path

- `new : function() : AircraftState`  (call as `api.type.MovePathAircraft.AircraftState.new(…)`): Creates a new, empty `AircraftState`.
- `flightState : FlightState`: current flight state of the aircraft
- `cruiseAlt : number`: Cruising altitude (world z) the aircraft flies at; the exploration plane sets it to the height of its target.
- `waitingDistance : number`
- `waitingPriority : integer`
- `gear : boolean`
- `gearTime : integer`
- `dyn : DynState`: The fast-changing part of the aircraft state.
- `dyn0 : DynState`: The fast-changing part of the aircraft state at the beginning of the simulation frame.

###### Engine.Component.MovePathAircraft.AircraftState.DynState (record)

```teal
record DynState
```
Access: `api.type.MovePathAircraft.AircraftState.DynState`

The fast-changing part of the aircraft state (flight dynamics).

- `new : function() : DynState`  (call as `api.type.MovePathAircraft.AircraftState.DynState.new(…)`): Creates a new, empty `DynState`.
- `velocity : Vec2f`
- `lastAccel : Vec2f`
- `pitch : number`
- `pitchV : number`
- `roll : number`
- `rollV : number`
- `thrust : number`: Current engine thrust of the aircraft.
- `thrustV : number`
- `elevator : number`
- `aileronLeft : number`
- `propPhase : number`
- `flaps : number`
- `alt : number`: Current altitude of the aircraft.

##### Engine.Component.MovePathAircraft.ShipState (record)

```teal
record ShipState
```
Access: `api.type.MovePathAircraft.ShipState`

Additional data for ships following the path.

- `new : function() : ShipState`  (call as `api.type.MovePathAircraft.ShipState.new(…)`): Creates a new, empty `ShipState`.
- `waitingDist : number`
- `clearToLand : boolean`
- `startPathPosData : PathPosData`
- `targetPathPosData : PathPosData`
- `dyn : DynState`: The fast-changing part of the ship state.
- `dyn0 : DynState`: The fast-changing part of the ship state at the beginning of the simulation frame.

###### Engine.Component.MovePathAircraft.ShipState.DynState (record)

```teal
record DynState
```
Access: `api.type.MovePathAircraft.ShipState.DynState`

The fast-changing part of the ship state.

- `new : function() : DynState`  (call as `api.type.MovePathAircraft.ShipState.DynState.new(…)`): Creates a new, empty `DynState`.
- `lastAccel : number`
- `tractiveEffort : number`
- `tractiveEffortV : number`
- `roll : number`
- `rollV : number`
- `powerOutput : number`
- `approachingStation : boolean`
- `paddlePhase : number`
- `actualPos : PathPosData`

##### Engine.Component.MovePathAircraft.PathState (record)

```teal
record PathState
```
Access: `api.type.MovePathAircraft.PathState`

Which of the three path parts (`path0`, `path1`, `path2`) the vehicle currently follows.

- `PATH0 : PathState`: The vehicle follows `path0`.
- `PATH1 : PathState`: The vehicle follows the free-movement part `path1`.
- `PATH2 : PathState`: The vehicle follows `path2`.

#### Engine.Component.Name (record)

```teal
record Name
```

Specifies the name of an entity

- `new : function() : Name`: Creates a new, empty `Name`.
- `name : string`: the name of the entity

#### Engine.Component.Parcel (record)

```teal
record Parcel
```

A parcel is a region of space near a road where a town building can be built.

- `new : function() : Parcel`: Creates a new, empty `Parcel`.
- `streetSegment : Engine.Entity`: the street edge to which the parcel belongs
- `left : boolean`: True if the parcel lies on the left side of its `streetSegment` (compare `ParcelData.leftParcels` / `rightParcels`).
- `curveParam : number`
- `offset : number`
- `faces : {Face}`: Ground polygons that make up the parcel.

##### Engine.Component.Parcel.CollisionType (record)

```teal
record CollisionType
```

Kinds of collision tracked per parcel face.

- `Environment : CollisionType`
- `Parcel : CollisionType`

##### Engine.Component.Parcel.Face (record)

```teal
record Face
```

A ground polygon of the parcel.

- `new : function() : Face`: Creates a new, empty `Face`.
- `points : {Vec3f}`: Corner points of the face, in world coordinates.
- `collision : {CollisionType : boolean}`

#### Engine.Component.PersonCapacity (record)

```teal
record PersonCapacity
```

Contains information about a workplace of household.

- `new : function() : PersonCapacity`: Creates a new, empty `PersonCapacity`.
- `type : LandUseType`: the type of capacity (1 = residential, 2 = commercial, 3 = industrial)
- `capacity : integer`: total capacity (tenants or workers) of the Entity
- `personNodes : {NodeId}`
- `vehicleNodes : {NodeId}`
- `pathfindingPreference : PathfindingPreference`

##### Engine.Component.PersonCapacity.PathfindingPreference (record)

```teal
record PathfindingPreference
```

Pathfinding preferences of the persons belonging to this capacity.

- `new : function() : PathfindingPreference`: Creates a new, empty `PathfindingPreference`.
- `destinationWeight : number`
- `disallowedMoveModes : {MoveMode}`

#### Engine.Component.Player (record)

```teal
record Player
```

Represent a player

- `new : function() : Player`: Creates a new, empty `Player`.
- `headquarters : Engine.Entity`: the current headquarters construction

#### Engine.Component.PlayerOwned (record)

```teal
record PlayerOwned
```

An entity with player owner is property of the owner and can be bulldozed only by him.

- `new : function() : PlayerOwned`: Creates a new, empty `PlayerOwned`.
- `player : Engine.Entity`: the owner of the entity

#### Engine.Component.RailroadCrossing (record)

```teal
record RailroadCrossing
```

A railroad crossing

- `new : function() : RailroadCrossing`: Creates a new, empty `RailroadCrossing`.
- `nodes : {Engine.Entity}`: nodes belonging to the railroad crossing
- `edges : {Engine.Entity}`: edges belonging to the railroad crossing
- `typeIndex : integer`: the type of crossing, as an index in api.res.railroadCrossingTypeRep.
- `state : State`: the current animation
- `stateTime : integer`: the animation timer

##### Engine.Component.RailroadCrossing.State (record)

```teal
record State
```

Animation state of a railroad crossing

- `OPEN : State`: The crossing is open for road traffic.
- `CLOSED : State`: The crossing is closed for road traffic.
- `WAITING : State`

#### Engine.Component.RunwayList (record)

```teal
record RunwayList
```

All runways for the entity transport network.

- `new : function() : RunwayList`: Creates a new, empty `RunwayList`.
- `runways : {Runway}`: The runways of the entity's transport network.

#### Engine.Component.Scaffold (record)

```teal
record Scaffold
```

Construction site of a building that is being built, upgraded or downgraded (scaffolding and crane).

Notes:

Not read by base-game scripts; the description follows from the field names in the definition.

- `new : function() : Scaffold`: Creates a new, empty `Scaffold`.
- `type : Type`: Kind of building work (`NEW`, `UPGRADE` or `DOWNGRADE`).
- `oldModelId : integer`
- `oldTransf : Mat4f`
- `oldBuildingFace : {{Vec2f}}`
- `oldFloorLevel : {{number}}`
- `oldHeight : number`
- `buildingFace : {{Vec2f}}`
- `floorLevel : {{number}}`
- `height : number`
- `constructionStartTime : integer`: Game time when the building work started.
- `constructionEndTime : integer`: Game time when the building work ends.
- `cranePosition : Vec2f`
- `craneAngle : number`
- `craneTerrainHeight : number`
- `craneAngleSpan : {number}`

##### Engine.Component.Scaffold.Type (record)

```teal
record Type
```

Kind of building work.

- `NEW : Type`: A new building is being built.
- `UPGRADE : Type`: An existing building is being upgraded.
- `DOWNGRADE : Type`: An existing building is being downgraded.

#### Engine.Component.ShapeList (record)

```teal
record ShapeList
```

Contains a list procedurally generated shapes.

- `new : function() : ShapeList`: Creates a new, empty `ShapeList`.
- `shapes : {Shape}`: a list of shapes

#### Engine.Component.Ship (record)

```teal
record Ship
```

Contains information about pathing and extra data of ships.

- `new : function() : Ship`: Creates a new, empty `Ship`.
- `pathIndex : integer`: the current position on the path
- `reservedFrom : integer`: the starting edge reserved by the ship
- `reservedTo : integer`: the ending edge reserved by the ship

#### Engine.Component.SignalList (record)

```teal
record SignalList
```

Contains a list of signals for the entity.

- `new : function() : SignalList`: Creates a new, empty `SignalList`.
- `signals : {Signal}`: a list of signals

#### Engine.Component.SimCargo (record)

```teal
record SimCargo
```

Additional data for cargo.

- `new : function() : SimCargo`: Creates a new, empty `SimCargo`.
- `cargoType : CargoTypeId`: The cargo type of the cargo (never 0)
- `targetOrPickupEntity : Engine.Entity`: the entity of the target where the cargo item heads to
- `sourceEntity : Engine.Entity`: the entity of the source where the cargo item came from
- `pickupPoints : {PickupPoint}`: Transport history of the cargo item, one `PickupPoint` (vehicle, line, position, carrier) per transport leg.
- `speed : number`
- `startTime : integer`: the timestamp when the cargo item was produced
- `pickupTime : integer`: the timestamp when the cargo item was picked up
- `deliveryExtensionDuration : integer`: the duration of extra time to deliver the cargo

##### Engine.Component.SimCargo.PickupPoint (record)

```teal
record PickupPoint
```

Historic transport data for cargo items.

- `new : function() : PickupPoint`: Creates a new, empty `PickupPoint`.
- `vehicle : Engine.Entity`: the vehicle the cargo item was transported with
- `line : Engine.Entity`: the line the cargo item was transported on
- `position : Vec3f`: the position of the cargo item on the vehicle
- `carrier : Carrier`: the transport method used to transport the cargo item

#### Engine.Component.SimEntityAtBuilding (record)

```teal
record SimEntityAtBuilding
```

Additional data for entities waiting at buildings.

- `new : function() : SimEntityAtBuilding`: Creates a new, empty `SimEntityAtBuilding`.
- `waitingTime : number`: How long the entity has been waiting

#### Engine.Component.SimEntityAtStock (record)

```teal
record SimEntityAtStock
```

Additional data for entities waiting at industries.

- `new : function() : SimEntityAtStock`: Creates a new, empty `SimEntityAtStock`.
- `stock : {Engine.Entity, StockId}`: The industry the entity is waiting at
- `arrivalTime : integer`: the timestamp when the item arrived at the industry

#### Engine.Component.SimEntityAtTerminal (record)

```teal
record SimEntityAtTerminal
```

Additional data for entities waiting at terminals.

- `new : function() : SimEntityAtTerminal`: Creates a new, empty `SimEntityAtTerminal`.
- `line : Engine.Entity`: The line the entity is waiting for
- `lineStop0 : integer`: The stop the entity is waiting at
- `lineStop1 : integer`: The stop the entity will get off the vehicle
- `arrivalTime : number`: the timestamp when the item arrived at the station terminal
- `vehicle : Engine.Entity`: the vehicle the entity is currently waiting for

#### Engine.Component.SimEntityAtVehicle (record)

```teal
record SimEntityAtVehicle
```

Additional data for entities travelling on a vehicle.

- `new : function() : SimEntityAtVehicle`: Creates a new, empty `SimEntityAtVehicle`.
- `line : Engine.Entity`: The line the entity is currently using
- `lineStop0 : integer`: The stop the entity waited at
- `lineStop1 : integer`: The stop the entity will get off the vehicle
- `vehicle : Engine.Entity`: the vehicle the entity is currently in
- `roundTripCounter : integer`: how often the entity reached the stock again where it was by the vehicle loaded

#### Engine.Component.SimEntityIdle (record)

```teal
record SimEntityIdle
```

Additional data for entities that are idle.

- `new : function() : SimEntityIdle`: Creates a new, empty `SimEntityIdle`.

#### Engine.Component.SimEntityNeedsPath (record)

```teal
record SimEntityNeedsPath
```

Additional data for entities that need path calculation.

- `new : function() : SimEntityNeedsPath`: Creates a new, empty `SimEntityNeedsPath`.
- `input : PathFactoryInput`: The pathfinding request for the entity.
- `output : PathFactoryOutput`: The pathfinding result for the entity.

#### Engine.Component.SimEntityMoving (record)

```teal
record SimEntityMoving
```

Additional data for entities that are moving.

- `new : function() : SimEntityMoving`: Creates a new, empty `SimEntityMoving`.
- `line : Engine.Entity`: The line the entity is currently using
- `lineStop0 : integer`: The stop the entity waited at
- `lineStop1 : integer`: The stop where the entity will get off the vehicle.
- `partialPath : boolean`: true if the path complete until the destination
- `expectedArrivalTime : number`: Game time (milliseconds) at which the entity is expected to arrive; negative if unknown. For a person in a car, the base game shows happiness as decreasing once the current game time is later.

#### Engine.Component.SimPersonAtTerminal (record)

```teal
record SimPersonAtTerminal
```

Additional data for a person waiting at the terminal

- `new : function() : SimPersonAtTerminal`: Creates a new, empty `SimPersonAtTerminal`.
- `edgeId : EdgeId`: the edge of the transport network the entity is waiting at
- `place : integer`: the place on the edge

#### Engine.Component.SimPersonAtVehicle (record)

```teal
record SimPersonAtVehicle
```

Additional data for a person travelling on a vehicle

- `new : function() : SimPersonAtVehicle`: Creates a new, empty `SimPersonAtVehicle`.
- `vehicle : Engine.Entity`: the vehicle the entity is using
- `part : integer`: the vehicle part (carriage) the entity is using
- `place : integer`: the place (seat) in the vehicle
- `expectedArrivalTime : number`: Game time (milliseconds) at which the person is expected to arrive; when the current game time is later, the base game shows the person's happiness as decreasing.

#### Engine.Component.SimPerson (record)

```teal
record SimPerson
```

Additional data for persons.

- `new : function() : SimPerson`: Creates a new, empty `SimPerson`.
- `cargoType : CargoTypeId`: the cargo type of the person (always 0)
- `destinations : {Engine.Entity}`: the destinations for residence, shopping and work
- `moveModes : {MoveMode}`: Move mode per destination, indexed by land-use type + 1 (residential, commercial, industrial), like `destinations`.
- `travelTimes : {number}`
- `targetOrAtEntity : Engine.Entity`: the entity where the person is or where the person heads to
- `lastNonResType : LandUseType`: Land-use type of the last non-residential destination the person visited. The base game uses `destinations[lastNonResType + 1]` as the origin of a person who is on the way home.
- `lastMoveMode : MoveMode`: the last used move mode
- `walkSpeed : number`
- `modelIdPerson : integer`: the model that is used for the person
- `colorConfigPerson : integer`: the color config for the clothes of the person
- `modelIdCar : integer`: the model that is used as a car
- `colorConfigCar : integer`: the color config for the car
- `lastDestinationUpdate : integer`: the timestamp of the latest destination recalculation
- `landUse2ReachableWalkDrive : {integer, integer}`
- `landUse2ReachableLines : {integer, integer}`
- `pickupTime : integer`
- `happiness : number`: Happiness of the person from 0 to 1; the base game shows values of 0.5 and above as happy.

#### Engine.Component.StationGroup (record)

```teal
record StationGroup
```

Defines a group of stations, can group together stations with very different transport modes

- `new : function() : StationGroup`: Creates a new, empty `StationGroup`.
- `stations : {Engine.Entity}`: A list of stations that are grouped together

#### Engine.Component.StockList (record)

```teal
record StockList
```
Access: `api.type.StockList`

Stock list data.

Example:
```lua
-- total output capacity of an industry (from the base game's emission script)
local stockList = api.engine.getComponent(industry.stockList, api.type.ComponentType.STOCK_LIST)
local capacity = 0
for i = 1, #stockList.stocks do
  if stockList.stocks[i].type == api.type.enum.StockListType.OutputStock then
    capacity = capacity + stockList.stocks[i].capacity
  end
end
```

- `new : function() : StockList`  (call as `api.type.StockList.new(…)`): Creates a new, empty `StockList`.
- `stocks : {Stock}`: The stocks of the list; rule inputs refer to them by index.
- `rules : {Rule}`: The production rules of the stock list (the industry recipe).
- `stockDatas : {StockData}`: Additional runtime data per stock, parallel to `stocks`.
- `ruleDatas : {RuleData}`: Runtime data per rule (application history), parallel to `rules`.
- `cargoData : {CargoTypeId : CargoData}`: Additional runtime data per cargo type.
- `thrownAwayCargo : integer`
- `modifiers : Modifiers`: Modifiers of the stock list, such as `productivity`.

##### Engine.Component.StockList.Stock (record)

```teal
record Stock
```
Access: `api.type.StockList.Stock`

Stock data for a single stock in a StockList.

- `new : function() : Stock`  (call as `api.type.StockList.Stock.new(…)`): Creates a new, empty `Stock`.
- `type : StockListType`: Kind of stock (`api.type.enum.StockListType`): `InputStock`, `OutputStock` or `StorageStock`.
- `cargoType : CargoTypeId`: Cargo type id the stock holds; negative when the stock accepts a set of cargo types (see `cargoTypes`).
- `cargoTypes : CargoTypeSet`: Set of cargo classes and types the stock accepts (`cargoClassesIncluded`, `cargoTypesIncluded`, ...).
- `mixedTypes : boolean`
- `capacity : integer`: Capacity of the stock (number of cargo items).
- `loadSpeedModifier : number`
- `loadIndicatorModelInstanceIndices : {integer}`
- `tag : integer`

##### Engine.Component.StockList.StockData (record)

```teal
record StockData
```
Access: `api.type.StockList.StockData`

StockData for a StockList.

- `new : function() : StockData`  (call as `api.type.StockList.StockData.new(…)`): Creates a new, empty `StockData`.
- `consumeFrequency : number`

##### Engine.Component.StockList.Rule (record)

```teal
record Rule
```
Access: `api.type.StockList.Rule`

Rule data for a StockList.

- `new : function() : Rule`  (call as `api.type.StockList.Rule.new(…)`): Creates a new, empty `Rule`.
- `input : {{integer}}`: Input requirements as a list of alternatives; each alternative maps the stock index (1-based, as in `stocks`) to the amount consumed per application. The base game only reads the first alternative (`input[1]`).
- `output : {CargoTypeId : integer}`: Amount of cargo produced per application, keyed by cargo type id.
- `capacity : integer`: Number of applications per year; one application takes `api.util.getDefaultYearDuration() / capacity`.
- `consumptionFactor : number`: Multiplier on consumption; the base game computes the yearly consumption of a rule as `capacity * consumptionFactor * input[j]`.
- `booster : boolean`: True if the rule is a booster rule; a booster that is running (or has its inputs in stock) multiplies production by `1 + boostFactor`.
- `boostFactor : number`: Production boost of a booster rule; the boost is `1 + boostFactor`, and boosts of several booster rules multiply.
- `priority : integer`
- `slot : integer`

##### Engine.Component.StockList.RuleData (record)

```teal
record RuleData
```
Access: `api.type.StockList.RuleData`

RuleData data for a StockList.

- `new : function() : RuleData`  (call as `api.type.StockList.RuleData.new(…)`): Creates a new, empty `RuleData`.
- `applyHistory : {ApplyInfo}`: Past applications of the rule, latest last. An empty history means the rule has never been applied.
- `lastApplyDecayFactor : number`: Duration factor of the last application caused by delivery time; the base game shows `duration * (lastApplyDecayFactor - 1)` as extra time "due to delivery time".

###### Engine.Component.StockList.RuleData.ApplyInfo (record)

```teal
record ApplyInfo
```
Access: `api.type.StockList.RuleData.ApplyInfo`

ApplyInfo data for a StockList.

- `new : function() : ApplyInfo`  (call as `api.type.StockList.RuleData.ApplyInfo.new(…)`): Creates a new, empty `ApplyInfo`.
- `time : integer`: Game time (milliseconds) when the rule application started.
- `interval : integer`: Duration of the application in milliseconds; it is running while `time + interval` is not yet reached.
- `consumedCargo : {StockId}`: The cargo items (stock ids) consumed by the application.
- `producedCargo : {{CargoTypeId, integer}}`: Cargo produced by the application, as pairs of cargo type id and amount.

##### Engine.Component.StockList.CargoData (record)

```teal
record CargoData
```
Access: `api.type.StockList.CargoData`

CargoData for a StockList.

- `new : function() : CargoData`  (call as `api.type.StockList.CargoData.new(…)`): Creates a new, empty `CargoData`.
- `produceFrequency : integer`

##### Engine.Component.StockList.Modifiers (record)

```teal
record Modifiers
```
Access: `api.type.StockList.Modifiers`

Modifiers for the stocklist

- `new : function() : Modifiers`  (call as `api.type.StockList.Modifiers.new(…)`): Creates a new, empty `Modifiers`.
    - used 1x in the base game, e.g. `base/content/industries/industry_workers.script.tl:11`: `local newModifiers = api.type.StockList.Modifiers.new()`
- `new : function(Modifiers) : Modifiers`  (call as `api.type.StockList.Modifiers.new(…)`): Creates a new, empty `Modifiers`.
    - used 1x in the base game, e.g. `base/content/industries/industry_workers.script.tl:11`: `local newModifiers = api.type.StockList.Modifiers.new()`
- `productivity : number`: the productivity of the stocklist

#### Engine.Component.Subconstruction (record)

```teal
record Subconstruction
```

Subconstruction data.

- `new : function() : Subconstruction`: Creates a new, empty `Subconstruction`.
- `tag : integer`
- `slotIds : {integer}`: Slots that are assigned to this subconstruction, for selection/bulldoze purposes
- `metadata : SubconstructionInstanceMetadata`: Metadata of the subconstruction as returned by the construction's `updateFn` (for example `emissions`, read by the base game's emission script).

#### Engine.Component.Terrain (record)

```teal
record Terrain
```

Contains information about the map.

- `new : function() : Terrain`: Creates a new, empty `Terrain`.
- `size : Vec2i`: the number of tiles of the terrain
- `baseLevels : integer`: number of base levels
- `baseResolution : Vec3f`: resolution for all three dimensions
- `highLevels : integer`
- `offsetZ : number`
- `waterLevel : number`: level for the surface of the navigatable water
- `dataMaps : {string, HeightmapNew}`

#### Engine.Component.TerrainAlignmentList (record)

```teal
record TerrainAlignmentList
```

Contains information about how terrain needs to be aligned.

- `new : function() : TerrainAlignmentList`: Creates a new, empty `TerrainAlignmentList`.
- `terrainAlignments : {TerrainAlignment}`: The terrain alignments of the entity.

#### Engine.Component.TerrainTile (record)

```teal
record TerrainTile
```

Contains information about a single tile of the map.
A tile is a 256m x 256m chunk of the map.

- `new : function() : TerrainTile`: Creates a new, empty `TerrainTile`.
- `terrainEntity : Engine.Entity`: a reference to the entity with terrain information
- `position : Vec2i`: the position of the tile in the tile grid

#### Engine.Component.TerrainTileBrush (record)

```teal
record TerrainTileBrush
```

Contains information about the brush layer of a tile of the map.

- `new : function() : TerrainTileBrush`: Creates a new, empty `TerrainTileBrush`.
- `materialIndexBrush : {integer}`: contains the material id of the brush layer
- `forceAboveBit : {boolean}`

#### Engine.Component.TerrainTileHeightmap (record)

```teal
record TerrainTileHeightmap
```

Contains information about the geometry of the tile.

- `new : function() : TerrainTileHeightmap`: Creates a new, empty `TerrainTileHeightmap`.
- `vertices : {integer}`: contains the heightmap information for the tile. Each value is for a position in the heightmap grid of the tile with a 4m resolution

#### Engine.Component.TickEpoch (record)

```teal
record TickEpoch
```

Tracks information about the last time an entity was modified or added.

- `new : function() : TickEpoch`: Creates a new, empty `TickEpoch`.
- `time : integer`: a unique index incrementing each simulation update step
- `commandIndex : integer`: a unique index counting commands within a frame
- `subIndex : integer`: a unique increasing index within a command

#### Engine.Component.Town (record)

```teal
record Town
```

Contains information about a single town.

- `new : function() : Town`: Creates a new, empty `Town`.
- `initialLandUseCapacities : {integer}`: initial capacities for the residential, commercial and industrial district of the town
- `capacityScalingFactor : number`: Scaling factor for the town's capacities; the town builder initialises it from `api.res.getBaseConfig().townCapacityScalingFactor`.
- `levelToCapacityDistributionWeights : {{number}}`: Custom weights that distribute the town's capacity over building levels 1 to 4, one list of four weights per land-use type (residential, commercial, industrial). `nil` means the default weights are used. Set it with `api.cmd.makeTownCustomDistributionWeightsCmd`.
- `sizeFactors : {number}`: a base size of the town
- `cargoNeeds : {{{CargoTypeId, number}}}`: list of cargo type ids for the needs of each of the three town districts **residential**, **commercial** and **industrial**
- `developmentActive : boolean`: turns off growth of the town
- `noise : number`: the accumulated noise in decibels, it's prefered to use `api.engine.util.getTownEmissionDB`
- `pollution : number`: the accumulated noise in decibels, it's prefered to use `api.engine.util.getTownEmissionDB`

#### Engine.Component.TownBuilding (record)

```teal
record TownBuilding
```

Contains information about a town building.

- `new : function() : TownBuilding`: Creates a new, empty `TownBuilding`.
- `personCapacity : Engine.Entity`: Entity that holds the building's `PERSON_CAPACITY` component.
- `stockList : Engine.Entity`: Entity that holds the building's `STOCK_LIST` component (its cargo demand).
- `construction : Engine.Entity`: The construction entity of the building.
- `level : integer`: level of the building (1-4)
- `parcels : {Engine.Entity}`: parcels occupied by the building
- `depth : integer`: the depth in number of parcels (1-4)
- `height : number`: the height in meters of the building
- `town : Engine.Entity`: the town entity
- `timeBuilt : integer`: the timestamp when the building was built
- `blockedDevelopment : boolean`: block town developer from changing this building

#### Engine.Component.TownConnection (record)

```teal
record TownConnection
```

Defines a connection between towns.

- `new : function() : TownConnection`: Creates a new, empty `TownConnection`.
- `entity0 : Engine.Entity`: first town
- `entity1 : Engine.Entity`: second town
- `position0 : Vec2f`: position of the first town
- `position1 : Vec2f`: position of the second town
- `time : number`: approximate time taken to travel the connection
- `currentTime : number`
- `lastUpgradeYear : integer`
- `valid : boolean`

#### Engine.Component.TpNetLink (record)

```teal
record TpNetLink
```

Represents a link between transport networks.
A link is an automatic pedestrian connection.

- `new : function() : TpNetLink`: Creates a new, empty `TpNetLink`.
- `from : EdgePos`: link starting edge position
- `to : EdgePos`: link end edge position

#### Engine.Component.Train (record)

```teal
record Train
```

Represents an entire train

- `new : function() : Train`: Creates a new, empty `Train`.

#### Engine.Component.TransportNetwork (record)

```teal
record TransportNetwork
```

Contains the graph of edges and nodes onto which vehicles and person move.

- `new : function() : TransportNetwork`: Creates a new, empty `TransportNetwork`.
- `nodes : {Node}`: The list of nodes of the transport network
- `edges : {Edge}`: The list of edges of the transport network
- `turnaroundEdges : {integer}`: for each edge, the index of the first edge on the opposite lane

#### Engine.Component.TransportNetworkNodes (record)

```teal
record TransportNetworkNodes
```

Data for TransportNetworkNodes marker component.

- `new : function() : TransportNetworkNodes`: Creates a new, empty `TransportNetworkNodes`.

#### Engine.Component.TransportVehicle (record)

```teal
record TransportVehicle
```
Access: `api.type.TransportVehicle`

Represents a single transport vehicle

- `new : function() : TransportVehicle`  (call as `api.type.TransportVehicle.new(…)`): Creates a new, empty `TransportVehicle`.
- `carrier : Carrier`: the type of transport vehicle
- `transportVehicleConfig : TransportVehicleConfig`: the configuration of vehicle models in this vehicle
- `config : Config`: various configuration properties for the vehicle
- `state : TransportVehicleState`: current state of the vehicle
- `userStopped : boolean`: whether or not the user stopped the vehicle
- `depot : Engine.Entity`: if in depot, the Entity Id of the depot
- `sellOnArrival : boolean`: whether or not the vehicle will be sold on arrival
- `line : Engine.Entity`: the entity of the line (if assigned to one)
- `stopIndex : integer`: the stop index of the line (if assigned to one)
- `arrivalStationTerminal : StationTerminal`: The terminal to which the vehicle currently tries to go
- `arrivalStationTerminalLocked : boolean`: If the arrivalStationTerminal is the final decision
- `lineStopDepartures : {integer}`: timestamps of the last departures at each line stop
- `lastLineStopDeparture : integer`: timestamp of the last departure at any line
- `sectionTimes : {number}`: time taken for each section between line stops
- `lineStop2cargo2available : {{CargoTypeId}}`: available capacity per cargo type for each line stop
- `loadState : LoadState`: current loading state of the vehicle
- `timeUntilNextScheduledDeparture : number`: how much time is left until the vehicle will start loading cargo
- `loadSpeedModifiers : {CargoTypeId : LoadSpeedModifiers}`: current effective load speed modifiers per cargo type
- `timeUntilCloseDoors : number`: how much time is left until the vehicle starts closing the doors
- `timeUntilDeparture : number`: how much time is left until the vehicle leaves the terminal
- `timeUntilNextLoad : {CargoTypeId : number}`: time until the next load per cargo type
- `timeUntilNextUnload : {CargoTypeId : number}`: time until the next unload per cargo type
- `lastStationStockIndex : integer`: index of the last station/stock where cargo was loaded/unloaded if multiple compatible stocks are in reach of the current
terminal of the currently loading/unloading cargo type.
- `doorsTime : integer`: time at which the door have been closed/open
- `daysInDepot : integer`: amount of days waiting in depot
- `daysAtTerminal : integer`: amount of days waiting at terminal
- `doorsOpen : boolean`: whether doors should be open or closed
- `noPath : boolean`: whether or not the vehicle has a path
- `autoDeparture : boolean`: whether the vehicle departure is managed by the game or not
- `maintenanceStation : Engine.Entity`: the maintenance station maintaining the vehicle
- `modifiers : Modifiers`: modifiers applied to the transport vehicle
- `unloadPendingIncome : PendingIncomeParams`: list of pending income
- `closestTown : Engine.Entity`: the closest town to the vehicle
- `visitedStops : {integer}`: The last stops on the line the transport vehicle stopped at
The length is limited to the number of stops on the line

##### Engine.Component.TransportVehicle.Modifiers (record)

```teal
record Modifiers
```
Access: `api.type.TransportVehicle.Modifiers`

Contains modifiers for the entire transport vehicle

- `new : function() : Modifiers`  (call as `api.type.TransportVehicle.Modifiers.new(…)`): Creates a new, empty `Modifiers`.
    - used 3x in the base game, e.g. `base/content/gui/gui/entity_window/vehicle/vehicle.tl:31`: `return api.type.TransportVehicle.Modifiers.new(tv.modifiers)`
- `new : function(Modifiers) : Modifiers`  (call as `api.type.TransportVehicle.Modifiers.new(…)`): Creates a new, empty `Modifiers`.
    - used 3x in the base game, e.g. `base/content/gui/gui/entity_window/vehicle/vehicle.tl:31`: `return api.type.TransportVehicle.Modifiers.new(tv.modifiers)`
- `topSpeedScale : number`: allows scaling the transport vehicle top speed
- `noiseScale : number`: allows scaling the transport vehicle noise
- `pollutionScale : number`: allows scaling the transport vehicle pollution
- `comfortScale : number`: allows scaling the transport vehicle comfort

##### Engine.Component.TransportVehicle.LoadSpeedModifiers (record)

```teal
record LoadSpeedModifiers
```
Access: `api.type.TransportVehicle.LoadSpeedModifiers`

Contains current effective modifiers for the transport vehicle load speed.

- `vehicleOverlength : number`: the modifier poriton because of overlength with the terminal
- `stockCompatibility : number`: the compatibility with the stock
- `vehicleCompatibility : number`: the compatibility with the terminal
- `vehicleIncompatibility : number`: the incompatibility with the terminal
- `getTotalPenalty : function(LoadSpeedModifiers) : number`  (call as `loadSpeedModifiers:getTotalPenalty(…) (method)`): computed total penalty from all modifiers
    - used 2x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_util.tl:2222`: `local totalPenalty = loadSpeedModifiers ~= nil and loadSpeedModifiers:getTotalPenalty() or 0.0`

##### Engine.Component.TransportVehicle.Config (record)

```teal
record Config
```
Access: `api.type.TransportVehicle.Config`

Contains additional data for the transport vehicle

- `new : function() : Config`  (call as `api.type.TransportVehicle.Config.new(…)`): Creates a new, empty `Config`.
- `transportModes : {TransportMode : boolean}`
- `allCaps : {CargoTypeId}`: Capacity per cargo type, indexed by cargo type id + 1 (Lua arrays are 1-based). The base game checks `config.allCaps[cargoTypeId + 1] > 0` to see whether a vehicle can carry a cargo type.
- `capacities : {CargoTypeId}`: all cargo capacities of the vehicle
- `loadSpeeds : {number}`
- `reversible : boolean`

##### Engine.Component.TransportVehicle.LoadState (record)

```teal
record LoadState
```
Access: `api.type.TransportVehicle.LoadState`

Contains additional data for the transport vehicle

- `new : function() : LoadState`  (call as `api.type.TransportVehicle.LoadState.new(…)`): Creates a new, empty `LoadState`.
- `None : LoadState`: Not in a loading cycle at a terminal.
- `Arrived : LoadState`: Arrived at the terminal.
- `Unloading : LoadState`: Unloading cargo.
    - used 3x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_util.tl:2251`: `elseif info and info.loadState == api.type.TransportVehicle.LoadState.Unloading then`
- `Loading : LoadState`: Loading cargo.
    - used 4x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_util.tl:2249`: `if info and info.loadState == api.type.TransportVehicle.LoadState.Loading then`
- `WaitForScheduledDeparture : LoadState`: Waiting for the scheduled departure; the base game shows "Ensuring safe distance".
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_util.tl:2283`: `elseif transportVehicle.loadState == api.type.TransportVehicle.LoadState.WaitForScheduledDeparture then`
- `Departing : LoadState`: Departing from the terminal.

##### Engine.Component.TransportVehicle.PendingIncomeParams (record)

```teal
record PendingIncomeParams
```
Access: `api.type.TransportVehicle.PendingIncomeParams`

Contains information about pending split income for the transport vehicle

- `new : function() : PendingIncomeParams`  (call as `api.type.TransportVehicle.PendingIncomeParams.new(…)`): Creates a new, empty `PendingIncomeParams`.
- `amount : integer`: total income pending to be unloaded
- `carrier : Carrier`: the carrier for which the income is pending
- `vehicleEntity : Engine.Entity`: accounted vehicle entity id
- `lineEntity : Engine.Entity`: accounted line entity id

###### Engine.Component.TransportVehicle.PendingIncomeParams.EntityRev0 (record)

```teal
record EntityRev0
```
Access: `api.type.TransportVehicle.PendingIncomeParams.EntityRev0`

Contains information about entity and revision for the pending income

- `entity : Engine.Entity`: the entity id
- `rev0 : integer`: the revision 0 of the entity

#### Engine.Component.VehicleDepot (record)

```teal
record VehicleDepot
```

Defines a vehicle depot

- `new : function() : VehicleDepot`: Creates a new, empty `VehicleDepot`.
- `carrier : Carrier`: The transport method for the depot, affects available vehicles
- `transportModes : {TransportMode : boolean}`
- `inNodes : {NodeId}`: the nodes that are entry points for the depot
- `outNodes : {NodeId}`: the nodes that are exit points for the depot
- `state : integer`
- `doors : number`
- `stateTime : integer`
- `catchmentArea : CatchmentArea`: the area around the station where vehicles are maintained
- `maintenancePool : integer`: number of vehicles that can be maintained
- `maxPoolUsage : number`: maximum number of vehicles that were recently maintained at the same time
- `averagePoolUsage : number`: average accumulated number of vehicles that were recently maintained at the same time
- `poolUsageTicks : integer`: number of ticks for recently maintained vehicles statistics
- `filterTags : {string}`: vehicle filter tags

#### Engine.Component.VehicleOrder (record)

```teal
record VehicleOrder
```

Contains information about precedence and breaking.

- `new : function() : VehicleOrder`: Creates a new, empty `VehicleOrder`.
- `equals : function(VehicleOrder, VehicleOrder) : boolean`: Returns `true` when both `VehicleOrder` values are equal.
- `entity : Engine.Entity`: the vehicle entity of the vehicle before this vehicle
- `strongBrakeEntity : boolean`
- `waitingForSpace : boolean`
- `waitingForTrafficLight : boolean`

#### Engine.Component.Warehouse (record)

```teal
record Warehouse
```

Contains information about warehouses.

- `new : function() : Warehouse`: Creates a new, empty `Warehouse`.
- `stockList : Engine.Entity`: contains coresponding stock list
- `construction : Engine.Entity`: containing the warehouse

#### Engine.Component.WaterMesh (record)

```teal
record WaterMesh
```

Contains the geometry information for a single tile of water.

- `new : function() : WaterMesh`: Creates a new, empty `WaterMesh`.
- `waterLevel : number`: level for the surface of the navigatable water
- `vertices : {Vec2f}`: the list of vertices of the mesh
- `depths : {number}`: the water depth at the vertex position
- `indices : {integer}`
- `contours : {Contour}`: a list with boundaries of the water mesh
- `pos : Vec2i`: coordinate of the tile

##### Engine.Component.WaterMesh.Contour (record)

```teal
record Contour
```

Represents the boundary of a water mesh.

- `new : function() : Contour`: Creates a new, empty `Contour`.
- `vertices : {Vec2f}`: The list of vertices of the contour
- `normals : {Vec2f}`: The list of normals at the vertex

#### Engine.Component.World (record)

```teal
record World
```

The World Component

- `new : function() : World`: Creates a new, empty `World`.
- `cloudCoverage : number`: Current cloud coverage of the world. The base game's weather script changes it with `api.cmd.makeGameSetCloudCoverageCmd`.

### Engine.ComponentType (record)

```teal
record ComponentType
```
Access: `api.type.ComponentType`

Represents a single component that can be attached to an entity.

- `AIRCRAFT : ComponentType`: Component type of `Engine.Component.Aircraft`.
- `ANIMAL : ComponentType`: Component type of `Engine.Component.Animal`.
    - used 6x in the base game, e.g. `base/content/game_mechanics/game_mechanics/achievements/achievements.script.tl:810`: `local animalComponent = api.engine.getComponent(selectedEntity, api.type.ComponentType.ANIMAL) as api.engine.Component.Animal`
- `ASSET_GROUP : ComponentType`: Component type of `Engine.Component.AssetGroup`.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util.tl:1132`: `local assetGroup : Engine.Component.AssetGroup = api.engine.getComponent(entity, api.type.ComponentType.ASSET_GROUP)`
- `BASE_EDGE : ComponentType`: Component type of `Engine.Component.BaseEdge` (street and track segments).
    - used 16x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util.tl:1285`: `return api.engine.getComponent(edge, api.type.ComponentType.BASE_EDGE)`
- `BASE_EDGE_STREET : ComponentType`: Component type of `Engine.Component.BaseEdgeStreet`.
- `BASE_NODE : ComponentType`: Component type of `Engine.Component.BaseNode` (street and track nodes).
    - used 14x in the base game, e.g. `base/content/gui/gui/construction/construction_desc_react_util.tl:978`: `api.type.ComponentType.BASE_NODE,`
- `BASE_PARALLEL_STRIP : ComponentType`: Component type of `Engine.Component.BaseParallelStrip`.
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util.tl:1406`: `local baseParallelStrip : Engine.Component.BaseParallelStrip = api.engine.getComponent(proposalData.parallelProposal.toRemove_native:at(i), api.type.ComponentTy`
- `CUSTOM_STATE : ComponentType`: Component type of `Engine.Component.CustomState`.
    - used 6x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/explorations/exploration_plane.script.tl:34`: `local customState : Engine.Component.CustomState = api.engine.getComponent(plane.entity, api.type.ComponentType.CUSTOM_STATE)`
- `EMISSION_EMITTER : ComponentType`: Component type of `Engine.Component.EmissionEmitter`.
    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/emission/emissions.script.tl:50`: `local previousEmitter = api.engine.getComponent(subEntity, api.type.ComponentType.EMISSION_EMITTER) as Engine.Component.EmissionEmitter`
- `FIELD : ComponentType`: Component type of `Engine.Component.Field`.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util.tl:1785`: `if api.engine.getComponent(emitter, api.type.ComponentType.INDUSTRY) ~= nil or api.engine.getComponent(emitter, api.type.ComponentType.FIELD) ~= nil then`
- `GAME_TIME : ComponentType`: Component type of `Engine.Component.GameTime`; read it from the world entity (`api.engine.util.getWorld()`).
    - used 124x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company.script.tl:22`: `local gameTimeComponent : Engine.Component.GameTime = api.engine.getComponent(api.engine.util.getWorld(), api.type.ComponentType.GAME_TIME)`
- `GAME_SPEED : ComponentType`: Component type of `Engine.Component.GameSpeed`; read it from the world entity (`api.engine.util.getWorld()`).
    - used 34x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company.tl:459`: `local currentGameSpeed : Engine.Component.GameSpeed = api.engine.getComponent(api.engine.util.getWorld(), api.type.ComponentType.GAME_SPEED)`
- `MODEL_INSTANCE_LIST : ComponentType`: Component type of `Engine.Component.ModelInstanceList`.
    - used 16x in the base game, e.g. `base/content/game_mechanics/game_mechanics/achievements/achievements.script.tl:470`: `local modelInstanceListComponent = api.engine.getComponent(carriageEntity, api.type.ComponentType.MODEL_INSTANCE_LIST) as api.engine.Component.ModelInstanceList`
- `NAME : ComponentType`: Component type of `Engine.Component.Name`.
    - used 5x in the base game, e.g. `base/content/gui/gui/entity_window/view_manager.tl:218`: `local nameComp = api.engine.getComponent(actualEntity, api.type.ComponentType.NAME) as Engine.Component.Name`
- `LINE : ComponentType`: Component type of `Engine.Component.Line`.
    - used 62x in the base game, e.g. `base/content/game_mechanics/game_mechanics/achievements/achievements.script.tl:426`: `local lineComponent = api.engine.getComponent(lineEntity, api.type.ComponentType.LINE) as api.engine.Component.Line`
- `LOG_BOOK : ComponentType`: Component type of `Engine.Component.LogBook`.
    - used 4x in the base game, e.g. `base/content/game_mechanics/game_mechanics/industries/industries.script.tl:105`: `local logBook : Engine.Component.LogBook = api.engine.getComponent(industry.stockList, api.type.ComponentType.LOG_BOOK)`
- `MODEL_PERSON : ComponentType`: Component type of `Engine.Component.ModelPerson`.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/sim_person/sim_person_eow.script.tl:352`: `local modelPerson = api.engine.getComponent(params.personEntity, api.type.ComponentType.MODEL_PERSON) as Engine.Component.ModelPerson`
- `MOVE_PATH : ComponentType`: Component type of `Engine.Component.MovePath`.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/sim_person/sim_person_eow.script.tl:362`: `local movePath = api.engine.getComponent(params.personEntity, api.type.ComponentType.MOVE_PATH) as Engine.Component.MovePath`
- `MOVE_PATH_AIRCRAFT : ComponentType`: Component type of `Engine.Component.MovePathAircraft`.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/explorations/exploration_plane.script.tl:196`: `local mpa : Engine.Component.MovePathAircraft = api.engine.getComponent(plane.entity, api.type.ComponentType.MOVE_PATH_AIRCRAFT)`
- `STATION : ComponentType`: Component type of `Engine.Component.Station`.
    - used 30x in the base game, e.g. `base/content/game_mechanics/game_mechanics/emission/emissions.script.tl:211`: `updateEmissions(UpdateIntervalOffsetStations, api.type.ComponentType.STATION, false, function(entity : Engine.Entity) : EntityEmissionConfig`
- `STATION_GROUP : ComponentType`: Component type of `Engine.Component.StationGroup`.
    - used 84x in the base game, e.g. `base/content/game_mechanics/game_mechanics/achievements/achievements.script.tl:430`: `local stationGroupComponent = api.engine.getComponent(stopStationGroupEntity, api.type.ComponentType.STATION_GROUP) as api.engine.Component.StationGroup`
- `SIM_PERSON : ComponentType`: Component type of `Engine.Component.SimPerson`.
    - used 18x in the base game, e.g. `base/content/game_mechanics/game_mechanics/arrival_tracker/arrivaltracker.script.tl:38`: `local simPerson : Engine.Component.SimPerson = api.engine.getComponent(simPersonEntity, api.type.ComponentType.SIM_PERSON)`
- `SIM_PERSON_AT_TERMINAL : ComponentType`: Component type of `Engine.Component.SimPersonAtTerminal`.
- `SIM_PERSON_AT_VEHICLE : ComponentType`: Component type of `Engine.Component.SimPersonAtVehicle`.
    - used 2x in the base game, e.g. `base/content/gui/gui/entity_window/sim_person/sim_person.tl:21`: `local spav = api.engine.getComponent(params.keyEntity, api.type.ComponentType.SIM_PERSON_AT_VEHICLE) as Engine.Component.SimPersonAtVehicle`
- `SIM_CARGO : ComponentType`: Component type of `Engine.Component.SimCargo`.
    - used 8x in the base game, e.g. `base/content/game_mechanics/game_mechanics/achievements/achievements.script.tl:227`: `local cargoComponent = api.engine.getComponent(cargoEntity, api.type.ComponentType.SIM_CARGO) as api.engine.Component.SimCargo`
- `SIM_ENTITY_AT_BUILDING : ComponentType`: Component type of `Engine.Component.SimEntityAtBuilding`.
    - used 3x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_util.tl:1379`: `local simEntityAtBuilding = api.engine.getComponent(personEntityId, api.type.ComponentType.SIM_ENTITY_AT_BUILDING) as Engine.Component.SimEntityAtBuilding`
- `SIM_ENTITY_AT_VEHICLE : ComponentType`: Component type of `Engine.Component.SimEntityAtVehicle`.
    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/deliver_passengers/deliver_passengers.script.tl:61`: `local simEntityAtVehicle = api.engine.getComponent(simEntity, api.type.ComponentType.SIM_ENTITY_AT_VEHICLE) as Engine.Component.SimEntityAtVehicle`
- `SIM_ENTITY_AT_TERMINAL : ComponentType`: Component type of `Engine.Component.SimEntityAtTerminal`.
    - used 7x in the base game, e.g. `base/content/game_mechanics/game_mechanics/celebrations/celebrations.script.tl:234`: `local simEntityAtTerminalComponent = api.engine.getComponent(simPersonEntity, api.type.ComponentType.SIM_ENTITY_AT_TERMINAL) as Engine.Component.SimEntityAtTerm`
- `SIM_ENTITY_IDLE : ComponentType`: Component type of `Engine.Component.SimEntityIdle`.
- `SIM_ENTITY_MOVING : ComponentType`: Component type of `Engine.Component.SimEntityMoving`.
    - used 2x in the base game, e.g. `base/content/gui/gui/entity_window/sim_person/sim_person.tl:42`: `local simEntityMoving = api.engine.getComponent(params.keyEntity, api.type.ComponentType.SIM_ENTITY_MOVING) as Engine.Component.SimEntityMoving`
- `SIGNAL_LIST : ComponentType`: Component type of `Engine.Component.SignalList`.
    - used 13x in the base game, e.g. `base/content/gui/gui/construction/construction_desc_react_util.tl:996`: `api.type.ComponentType.SIGNAL_LIST,`
- `TOWN : ComponentType`: Component type of `Engine.Component.Town`.
    - used 129x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notification_legacy_util.tl:184`: `if api.engine.entityExists(legacyTownEntity) and api.engine.getComponent(legacyTownEntity, api.type.ComponentType.TOWN) then`
- `INDUSTRY : ComponentType`: Component type of `Engine.Component.Industry`.
    - used 76x in the base game, e.g. `base/content/game_mechanics/game_mechanics/celebrations/celebrations.script.tl:202`: `local industryComponent = api.engine.getComponent(stockEntity, api.type.ComponentType.INDUSTRY) as Engine.Component.Industry`
- `STOCK_LIST : ComponentType`: Component type of `Engine.Component.StockList`.
    - used 66x in the base game, e.g. `base/content/game_mechanics/game_mechanics/emission/emissions.script.tl:168`: `local stockList = api.engine.getComponent(entity, api.type.ComponentType.STOCK_LIST) as Engine.Component.StockList`
- `TOWN_BUILDING : ComponentType`: Component type of `Engine.Component.TownBuilding`.
    - used 30x in the base game, e.g. `base/content/game_mechanics/game_mechanics/arrival_tracker/arrivaltracker.script.tl:12`: `local townBuilding : Engine.Component.TownBuilding = api.engine.getComponent(townBuildingEntity, api.type.ComponentType.TOWN_BUILDING)`
- `TRANSPORT_VEHICLE : ComponentType`: Component type of `Engine.Component.TransportVehicle`.
    - used 151x in the base game, e.g. `base/content/game_mechanics/game_mechanics/achievements/achievements.script.tl:457`: `local transportVehicleComponent = api.engine.getComponent(vehicleEntity, api.type.ComponentType.TRANSPORT_VEHICLE) as Engine.Component.TransportVehicle`
- `TRAIN : ComponentType`: Component type of `Engine.Component.Train`.
- `CARRIAGE : ComponentType`: Component type of `Engine.Component.Carriage`.
    - used 10x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util.tl:1769`: `if api.engine.getComponent(emitter, api.type.ComponentType.CARRIAGE) ~= nil then`
- `CARRIAGE_LIST : ComponentType`: Component type of `Engine.Component.CarriageList`.
    - used 9x in the base game, e.g. `base/content/game_mechanics/game_mechanics/achievements/achievements.script.tl:405`: `local carriageList = api.engine.getComponent(vehicleEntity, api.type.ComponentType.CARRIAGE_LIST) as api.engine.Component.CarriageList`
- `VEHICLE_DEPOT : ComponentType`: Component type of `Engine.Component.VehicleDepot`.
    - used 61x in the base game, e.g. `base/content/game_mechanics/game_mechanics/emission/emissions.script.tl:223`: `updateEmissions(UpdateIntervalOffsetDepots, api.type.ComponentType.VEHICLE_DEPOT, false, function(entity : Engine.Entity) : EntityEmissionConfig`
- `COLOR : ComponentType`: Component type of `Engine.Component.Color`.
    - used 16x in the base game, e.g. `base/content/gui/gui/entity_window/line/line.tl:80`: `local color = api.engine.getComponent(entityId, api.type.ComponentType.COLOR) as Engine.Component.Color`
- `BOUNDING_VOLUME : ComponentType`: Component type of `Engine.Component.BoundingVolume`.
    - used 9x in the base game, e.g. `base/content/game_mechanics/game_mechanics/fun_elements/ufo_util.tl:114`: `local bb = api.engine.getComponent(selectedField, api.type.ComponentType.BOUNDING_VOLUME)`
- `CONSTRUCTION : ComponentType`: Component type of `Engine.Component.Construction`.
    - used 123x in the base game, e.g. `base/content/game_mechanics/game_mechanics/achievements/achievements.script.tl:308`: `local constructionComponent = api.engine.getComponent(proposal.toRemove[1], api.type.ComponentType.CONSTRUCTION) as api.engine.Component.Construction`
- `SUBCONSTRUCTION : ComponentType`: Component type of `Engine.Component.Subconstruction`.
    - used 19x in the base game, e.g. `base/content/game_mechanics/game_mechanics/emission/emissions.script.tl:56`: `= api.engine.getComponent(subEntity, api.type.ComponentType.SUBCONSTRUCTION) as Engine.Component.Subconstruction`
- `PERSON_CAPACITY : ComponentType`: Component type of `Engine.Component.PersonCapacity`.
    - used 23x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/deliver_workers/deliver_workers.script.tl:124`: `if api.engine.getComponent(entity, api.type.ComponentType.PERSON_CAPACITY) then`
- `PLAYER_OWNED : ComponentType`: Component type of `Engine.Component.PlayerOwned`.
    - used 6x in the base game, e.g. `base/content/game_mechanics/game_mechanics/industries/industries.script.tl:97`: `if api.engine.getComponent(entityId, api.type.ComponentType.PLAYER_OWNED) then`
- `ACCOUNT : ComponentType`: Component type of `Engine.Component.Account`.
- `GAME_SCRIPT : ComponentType`: Component type of `Engine.Component.GameScript`; find the entity of a script with `api.engine.system.gameScriptSystem.getEntityForGameScript(path)`.
    - used 38x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company.script.tl:486`: `local emissionsGameScript : Engine.Component.GameScript<table> = emissionsScriptEntity and api.engine.getComponent(emissionsScriptEntity, api.type.ComponentType`
- `WAREHOUSE : ComponentType`: Component type of `Engine.Component.Warehouse`.
    - used 31x in the base game, e.g. `base/content/game_mechanics/game_mechanics/emission/emissions.script.tl:237`: `updateEmissions(UpdateIntervalOffsetWarehouses, api.type.ComponentType.WAREHOUSE, false, function(entity : Engine.Entity) : EntityEmissionConfig`
- `TRANSPORT_NETWORK : ComponentType`: Component type of `Engine.Component.TransportNetwork`.
    - used 10x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/line_manager_panel.tl:1115`: `local transportNetwork : Engine.Component.TransportNetwork = api.engine.getComponent(via.waypoint.edgePos.edgeId.entity, api.type.ComponentType.TRANSPORT_NETWOR`
- `MAINTENANCE_COST : ComponentType`: Component type of `Engine.Component.MaintenanceCost`.
    - used 2x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_util.tl:182`: `local maintenance = api.engine.getComponent(constructionId, api.type.ComponentType.MAINTENANCE_COST) as Engine.Component.MaintenanceCost`
- `RAILROAD_CROSSING : ComponentType`: Component type of `Engine.Component.RailroadCrossing`.
    - used 4x in the base game, e.g. `base/content/gui/gui/entity_window/make_entity_window.tl:38`: `[api.type.ComponentType.RAILROAD_CROSSING]		= 14000,`
- `PLAYER : ComponentType`: Component type of `Engine.Component.Player`.
    - used 4x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company_util.tl:259`: `local player : Engine.Component.Player = api.engine.getComponent(companyEntity, api.type.ComponentType.PLAYER)`
- `WORLD : ComponentType`: Component type of `Engine.Component.World`; read it from `api.engine.util.getWorld()`.
    - used 7x in the base game, e.g. `base/content/game_mechanics/game_mechanics/game_time/game_time.script.tl:249`: `= api.engine.getComponent(api.engine.util.getWorld(), api.type.ComponentType.WORLD)`
- `BRIDGE : ComponentType`: Component type of `Engine.Component.Bridge`.
    - used 3x in the base game, e.g. `base/content/gui/gui/entity_window/make_entity_window.tl:41`: `[api.type.ComponentType.BRIDGE]					= 17000,`
- `BASE_NODE_CONFIG : ComponentType`: Component type of `Engine.Component.BaseNodeConfig`.
    - used 3x in the base game, e.g. `base/content/gui/gui/entity_window/double_slip_switch.tl:27`: `local baseNodeConfig : Engine.Component.BaseNodeConfig = api.engine.getComponent(param.keyEntity, api.type.ComponentType.BASE_NODE_CONFIG)`
- `LAND_VEHICLE : ComponentType`: Component type of `Engine.Component.LandVehicle`.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/sim_person/sim_person.tl:39`: `local isInCar : boolean = api.engine.getComponent(params.keyEntity, api.type.ComponentType.LAND_VEHICLE) ~= nil`
- `BASE_NODE_TRAFFIC_LIGHT : ComponentType`: Component type of `Engine.Component.BaseNodeTrafficLight`.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/double_slip_switch.tl:22`: `local baseNodeTrafficLight : Engine.Component.BaseNodeTrafficLight = api.engine.getComponent(param.keyEntity, api.type.ComponentType.BASE_NODE_TRAFFIC_LIGHT)`
- `EDGE_OBJECT : ComponentType`: Component type of `Engine.Component.EdgeObject`.
    - used 4x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_util.tl:176`: `local edgeObject = api.engine.getComponent(constructionId, api.type.ComponentType.EDGE_OBJECT) as Engine.Component.EdgeObject`
- `EMISSION_GRID : ComponentType`: Component type of `Engine.Component.EmissionGrid`.
    - used 1x in the base game, e.g. `dlcs/urbangames_deluxe_upgrade_pack/content/fun_elements/fun_elements/balloon.script.tl:69`: `local emissionGrid = api.engine.getComponent(windEntity, api.type.ComponentType.EMISSION_GRID) as Engine.Component.EmissionGrid`
- `TERRAIN : ComponentType`: Component type of `Engine.Component.Terrain`.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/explorations/exploration_plane.script.tl:60`: `local waterLevel = (api.engine.getComponent(api.engine.util.getWorld(), api.type.ComponentType.TERRAIN) as Engine.Component.Terrain).waterLevel`
- `SHIP : ComponentType`: Component type of `Engine.Component.Ship`.

### Engine.EntityFilters (record)

```teal
record EntityFilters
```
Access: `api.engine.EntityFilters`

Optional filter configuration for `getEntitiesWithComponent`.

- `requireOwnedByPlayer : Engine.Entity`: Only include entities owned by the given player entity.
- `filterFn : function(Engine.Entity) : boolean`  (call as `api.engine.EntityFilters.filterFn(…)`): Custom filter: Most flexible - but very slow.

### Engine.UtilTerrain (record)

```teal
record UtilTerrain
```
Access: `api.engine.terrain`

Terrain query functions, available as `api.engine.terrain`.

Example:
```lua
local pos = api.type.Vec2f.new(x, y)
if api.engine.terrain.isValidCoordinate(pos) then
  local z = api.engine.terrain.getHeightAt(pos)
  print(z, api.engine.terrain.isOnWater(pos))
end
```

- `isValidCoordinate : function(Vec2f) : boolean`  (call as `api.engine.terrain.isValidCoordinate(…)`): Check if coordinate is within bounds of world.
    - param `position`: world position with x and y coordinate from center of the map
    - returns: true if valid coordinate, false if non valid coordinate
    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/explorations/exploration_plane.script.tl:189`: `if plane.exiting and not api.engine.terrain.isValidCoordinate(make2D(pathPosData.position)) then`
- `getHeightAt : function(Vec2f) : number`  (call as `api.engine.terrain.getHeightAt(…)`): Fetches height at the world position. When the coordinates are outside the map, the value of the nearest position on the map is returned.
    - param `position`: world position with x and y coordinate from center of the map
    - returns: the height at the position
    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/fun_elements/custom_entity_util.tl:74`: `local height = api.engine.terrain.getHeightAt(pos2d)`
- `getBaseHeightAt : function(Vec2f) : number`  (call as `api.engine.terrain.getBaseHeightAt(…)`): Fetches base height at the world position. Differently than `getHeightAt`, this does not include terrain alignments (non-permanent modifications)
and has a lower resolution of 4 meters. When the coordinates are outside the map, the value of the nearest position on the map is returned.
    - param `position`: world position with x and y coordinate from center of the map
    - returns: the height at the position
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/explorations/exploration_plane.script.tl:62`: `return api.type.Vec3f.new(v.x, v.y, math.max(api.engine.terrain.getBaseHeightAt(v), waterLevel) + altitude)`
- `isOnWater : function(Vec2f) : boolean`  (call as `api.engine.terrain.isOnWater(…)`): Returns whether the given world position is on water, i.e. below the water level.
    - param `position`: world position with x and y coordinate from center of the map
    - returns: true if the position is on water, false otherwise
    - used 1x in the base game, e.g. `base/content/mission/mission/tutorial/tutorial_story_util.tl:422`: `elseif not api.engine.terrain.isOnWater(testPos) then`
- `getHeightmapEntity : function(Vec2i) : Engine.Entity`  (call as `api.engine.terrain.getHeightmapEntity(…)`): Fetches the entity at the given tile position.
    - param `position`: tile position
    - returns: the heightmap entity at the position
- `getBoundingBox : function() : Box2f`  (call as `api.engine.terrain.getBoundingBox(…)`): Returns the bounding box of the terrain.
    - returns: bounding box of the terrain
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/fun_elements/custom_entity_util.tl:79`: `local box = api.engine.terrain.getBoundingBox()`
- `makeMapFromGame : function(withHeightmap : boolean, withTowns : boolean, withIndustries : boolean, withAssets : boolean) : GameMap`  (call as `api.engine.terrain.makeMapFromGame(…)`): Creates a map data structure from the current game state.
    - param `withHeightmap`: include heightmap data
    - param `withTowns`: include town data
    - param `withIndustries`: include industry data
    - param `withAssets`: include asset data
    - returns: the generated map
    - used 2x in the base game, e.g. `base/content/gui/gui/map_editor/map_editor.tl:326`: `local engineMap = api.engine.terrain.makeMapFromGame(mapParams.withHeightmap, mapParams.withTowns, mapParams.withIndustries, mapParams.withAssets)`
- `getTerrainEntityRevisions : function() : {Engine.Entity : Engine.Revision}`  (call as `api.engine.terrain.getTerrainEntityRevisions(…)`): Gets the entity revision numbers for all terrain entities.
    - returns: map of entity to revision number
    - used 1x in the base game, e.g. `base/content/gui/gui/map_editor/map_editor.tl:319`: `terrain = api.engine.terrain.getTerrainEntityRevisions(),`
- `getBaseResolution : function() : Vec3f`  (call as `api.engine.terrain.getBaseResolution(…)`): Gets the base resolution of the terrain.
    - returns: the base resolution
- `getHighResolution : function() : Vec3f`  (call as `api.engine.terrain.getHighResolution(…)`): Gets the high resolution of the terrain.
    - returns: the high resolution
- `ceilTerrainFragmentInTiles : function(percentage : number) : integer`  (call as `api.engine.terrain.ceilTerrainFragmentInTiles(…)`): Calculates the ceiling of terrain fragment size in tiles based on percentage.
    - param `percentage`: the percentage value
    - returns: the calculated terrain fragment size in tiles
    - used 2x in the base game, e.g. `base/content/terrain/reforestation.script.tl:24`: `local tilesPerMonth = api.engine.terrain.ceilTerrainFragmentInTiles(MonthlyTerrainFactor)`

### Engine.MapGen (record)

```teal
record MapGen
```
Access: `api.engine.mapgen`

Map generation helpers, available as `api.engine.mapgen`. The base game uses them in the map editor.

- `createTowns : function(name : string, {GameMap.Town}) : TownResult`  (call as `api.engine.mapgen.createTowns(…)`): Create a town proposal from a town list.
    - used 1x in the base game, e.g. `base/content/gui/gui/map_editor/map_editor.tl:1622`: `local townResult = api.engine.mapgen.createTowns(seed or "", towns)`
- `createIndustries : function(name : string, {GameMap.Industry}) : IndustryResult`  (call as `api.engine.mapgen.createIndustries(…)`): Create an industry proposal from an industry list.
    - used 1x in the base game, e.g. `base/content/gui/gui/map_editor/map_editor.tl:1565`: `local industryResult = api.engine.mapgen.createIndustries(seed or "", industries)`
- `getDefaultTownConnections : function({GameMap.Town}) : {{integer, integer}}`  (call as `api.engine.mapgen.getDefaultTownConnections(…)`): Create default town connections.
    - used 1x in the base game, e.g. `base/content/gui/gui/map_editor/map_editor.tl:2544`: `for __, conn in ipairs(api.engine.mapgen.getDefaultTownConnections(map.towns)) do`
- `getMinMaxValidTerrainHeight : function() : {number, number}`  (call as `api.engine.mapgen.getMinMaxValidTerrainHeight(…)`): Returns the absolute limits of terrain height ranges.
    - used 2x in the base game, e.g. `base/content/gui/gui/map_editor/map_editor.tl:870`: `local minMaxHeight = api.engine.mapgen.getMinMaxValidTerrainHeight()`
- `getMinMaxValidNumTowns : function() : {integer, integer}`  (call as `api.engine.mapgen.getMinMaxValidNumTowns(…)`): Returns the absolute limits of number of towns.
    - used 1x in the base game, e.g. `base/content/gui/gui/map_editor/map_editor.tl:2074`: `local minMaxNumTowns = api.engine.mapgen.getMinMaxValidNumTowns()`
- `getMinMaxValidNumIndustries : function() : {integer, integer}`  (call as `api.engine.mapgen.getMinMaxValidNumIndustries(…)`): Returns the absolute limits of number of industries.
    - used 1x in the base game, e.g. `base/content/gui/gui/map_editor/map_editor.tl:2075`: `local minMaxNumIndustries = api.engine.mapgen.getMinMaxValidNumIndustries()`

### Engine.Config (record)

```teal
record Config
```
Access: `api.engine.config`

Access to the game configuration, available as `api.engine.config`.

Example:
```lua
-- the base game checks whether it runs in the map editor like this
local isMapEditor = api.engine.config.getModParams()[""]["isMapEditor"] == true
```

- `getConfigDict : function() : {string : string}`  (call as `api.engine.config.getConfigDict(…)`): Returns the config dict for the game.
- `getModParams : function() : {string : table}`  (call as `api.engine.config.getModParams(…)`): Returns the mod params for the game.
    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company.script.tl:232`: `if api.engine.config.getModParams()[""]["isMapEditor"] then`
