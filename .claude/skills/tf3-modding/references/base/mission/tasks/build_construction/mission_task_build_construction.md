# mission.tasks.build_construction.mission_task_build_construction

Definition file: `base/tealdef/mission/tasks/build_construction/mission_task_build_construction.d.tl` (in the game folder).

Task that waits until the player builds something that passes a set of proposal checks.

## MissionTaskBuildConstruction (record)

```teal
global record MissionTaskBuildConstruction is MissionInterface.TaskScript<MissionTaskBuildConstruction.State, MissionTaskBuildConstruction.Params>
```
Load: `local build_construction = ug_require "::/mission/tasks/build_construction/build_construction.tl" as MissionTaskBuildConstruction`

Task (`build_construction`) that checks every build proposal with `proposalHandler`. A proposal that fails a check is blocked with the check's message; each applied proposal that passes counts, and the task completes at `minCount` (default 1).

Example:
```lua
-- urbangames_campaign_mission_01: build a road depot in a marked area
taskScript = build_construction as MissionInterface.TaskScript<any, any>,
taskParams : MissionTaskBuildConstruction.Params = {
    zones = { { key = "buildStreetDepot", polygon = depotPolygon, draw = true, getDrawColor = targetColor, playAnimation = true } },
    initialAngle = params.streetDepotAngle,
    proposalHandler = build_construction_util.makeProposalHandlerFilterApply({
        checkBuilderId = build_construction_util.makeCheckConstructionBuilderId({ error = _("MISSION_PROPOSAL_FEEDBACK_NOT_A_ROAD_DEPOT") }),
        checkConstruction = build_construction_util.makeCheckConstruction("::/depots/road/road_depot/road_depot.con", nil, { error = _("MISSION_PROPOSAL_FEEDBACK_NOT_A_ROAD_DEPOT") }),
        checkLocation = build_construction_util.makeCheckLocation(params.streetDepotZone.pos[1], params.streetDepotZone.pos[2], 20, { error = _("MISSION_PROPOSAL_FEEDBACK_NOT_IN_OUTLINED_AREA") }),
        checkSnapping = build_construction_util.makeCheckSnapping({}, { error = _("MISSION_PROPOSAL_FEEDBACK_NOT_CONNECTED_TO_STREET") }),
    }, true),
},
```

### MissionTaskBuildConstruction.Params (record)

```teal
record Params
```

Params of the task.

- `proposalHandler : BuildConstructionUtil.ProposalHandlerConfig`: Checks applied to proposals (required unless `makeProposalHandler` is given).
- `getProposalHandlerData : function() : any`: Returns the data from which `makeProposalHandler` builds the proposal handler. Called when the task spawns; the result is kept in `State.proposalHandlerData` and saved with the task.
    - returns: Data for `makeProposalHandler`. `urbangames_campaign_mission_02` returns a table of edge entities for its signal tasks.
    - notes: For a task from an older save (task version below 5) that is not complete, it is called again on load if the state has no data. If it then returns nil, the task completes.
- `makeProposalHandler : function(data : any) : BuildConstructionUtil.ProposalHandlerConfig`: receives result of getProposalHandlerData

Builds the proposal handler from `State.proposalHandlerData` for each proposal; takes precedence over `proposalHandler`.
    - param `data`: The stored result of `getProposalHandlerData`.
    - returns: Proposal handler, e.g. from `build_construction_util.makeProposalHandlerFilterApply`.
    - notes: If the state holds no data, the task logs an error and leaves the proposal unchecked.
- `zones : {MissionInterface.Zone}`: Zones to show.
- `getZones : function() : {MissionInterface.Zone}`: Returns the zones to show; takes precedence over `zones`.
    - returns: Zones.
- `getEntitiesToDraw : function() : {Engine.Entity}`: Returns entities to mark with a 5 m circle each; takes precedence over `getZones` and `zones`.
    - returns: Entities.
- `initialAngle : number`: Rotation set in the construction tool when the task opens, in radians.
- `minCount : integer`: Number of passing proposals needed (default 1; 0 completes at once). A counter is shown above 1.
- `getMinCount : function() : integer`: Returns the number needed when `minCount` is not set; called at spawn.
    - returns: Count.

### MissionTaskBuildConstruction.State (record)

```teal
record State
```

State of the task.

- `minCount : integer`: Number needed.
- `resultEntities : {Engine.Entity}`: First result entity of each passing proposal.
- `signalsFound : integer`: Passing proposals without result entity (signals, for example).
- `proposalHandlerData : any`: Result of `getProposalHandlerData`, stored at spawn and passed to `makeProposalHandler`. Older saves (task version below 5) get it on load while the task is not complete.
