# mission.tasks.build_construction.mission_task_build_construction_util

Definition file: `base/tealdef/mission/tasks/build_construction/mission_task_build_construction_util.d.tl` (in the game folder).

Proposal checks used by the build-construction task and by other tasks that filter what the player may build.

## Globals

- `MissionInterface` (type): Namespace of the core mission-framework types. A mission is a list of tasks; each task is run by a `TaskScript` and described by static data.
    - notes: How the pieces fit together (from `mission/mission_sim.script.tl` and the campaign mods): 1. A mod registers a generic resource of type `missiontask_boot` (mission), `tutorial_boot` (tutorial) or `guide_system_boot` (guide system). Its `data` is a `BootstrapDesc` whose `bootstrapScriptFn` points to the `spawnBootTasks` function of a module built with `MissionBootUtil.makeMission` / `makeGuide`. 2. The game script `::mission/mission.gs` (or the guide-system script) calls each bootstrap function once per savegame (remembered in `MissionState.bootTypes`); it adds the boot tasks. 3. Each task is identified by a `TaskConfig`. The framework resolves its `TaskVTable` through `getVTableScriptFn`, which returns the task's `TaskScript` and its static params from the story's task-data factory (`MissionTaskUtil.StaticTaskInfo`). 4. Every update the framework calls `onUpdate` and `isComplete` of each open task. When a task completes, `onComplete` runs and its `followup` tasks are added. State is kept in the game script state (`MissionState`); everything in `StaticTaskInfo` (functions included) is re-created from the story code and never saved.

    ```lua
    -- mission.res.lua of urbangames_campaign_mission_01
    function data()
    return {
        type = "missiontask_boot",
        data = {
            bootstrapScriptFn = "urbangames_campaign_mission_01::/mission/mission.script@mission.spawnBootTasks",
            bootstrapScriptFnParams = nil,
        }
    }
    end
    
    -- mission.script.lua
    local mission = require "mission.tl"
    function data()
        return { mission = mission }
    end
    ```

## BuildConstructionUtil (record)

```teal
global record BuildConstructionUtil
```
Load: `local build_construction_util = ug_require "::/mission/tasks/build_construction/mission_task_build_construction_util.tl" as BuildConstructionUtil`

Proposal checks for mission tasks. Each `checkX` tests one property of a build proposal; each `makeCheckX` returns that check as a function for a `HandleProposalEventConfig` field.

Notes:

A check returns `true` when the proposal passes. Otherwise it returns the `failureResult` given to
it (shown to the player) or `false`/`nil`, which makes the handler ignore the proposal. The handler
runs the checks in this order: `checkBuilderId`, `checkSignal`, `checkSignalLocation`,
`checkSignalDirection`, `checkStop`, `checkStopLocation`, `checkConstruction`, `checkLocation`,
`checkRestrictedArea`, `checkSnapping`. The first check that does not pass decides.

- `makeProposalHandlerFilterApply : function(filterApply : BuildConstructionUtil.HandleProposalEventConfig<any>, whitelist : boolean) : ProposalHandlerConfig`  (call as `build_construction_util.makeProposalHandlerFilterApply(…)`): Uses the same checks for previews and for applied proposals.
    - param `filterApply`: Checks to use.
    - param `whitelist`: Marks passing results as whitelist results.
    - returns: Proposal handler config.
    - used 13x in the base game, e.g. `base/content/mission/mission/tutorial/tutorial_story.tl:442`: `proposalHandler = build_construction_util.makeProposalHandlerFilterApply({`
- `checkStreetTerminalBuilderId : function<TaskContext>(event : BuildConstructionUtil.HandleProposalEventParams<TaskContext>, failureResult : MissionInterface.ProposalHandlerResult) : boolean`  (call as `build_construction_util.checkStreetTerminalBuilderId(…)`): Passes proposals of the street-stop tool (`streetTerminalBuilder`).
    - param `event`: Proposal event.
    - param `failureResult`: Result for other tools.
    - returns: `true`, or `failureResult` (`false` if not set).
- `makeCheckStreetTerminalBuilderId : function(failureResult : MissionInterface.ProposalHandlerResult) : function<TaskContext>(event : BuildConstructionUtil.HandleProposalEventParams<TaskContext>) : boolean`  (call as `build_construction_util.makeCheckStreetTerminalBuilderId(…)`): Returns `checkStreetTerminalBuilderId` bound to a failure result.
    - param `failureResult`: Result for other tools.
    - returns: Check function.
    - used 3x in the base game, e.g. `base/content/mission/mission/tutorial/tutorial_story_util.tl:162`: `checkBuilderId = build_construction_util.makeCheckStreetTerminalBuilderId({ error = _("Not a Stop!") }),`
- `checkConstructionBuilderId : function<TaskContext>(event : BuildConstructionUtil.HandleProposalEventParams<TaskContext>, failureResult : MissionInterface.ProposalHandlerResult) : boolean`  (call as `build_construction_util.checkConstructionBuilderId(…)`): Passes proposals of the construction tool (`constructionBuilder`).
    - param `event`: Proposal event.
    - param `failureResult`: Result for other tools.
    - returns: `true`, or `failureResult` (`false` if not set).
- `makeCheckConstructionBuilderId : function(failureResult : MissionInterface.ProposalHandlerResult) : function<TaskContext>(event : BuildConstructionUtil.HandleProposalEventParams<TaskContext>) : boolean`  (call as `build_construction_util.makeCheckConstructionBuilderId(…)`): Returns `checkConstructionBuilderId` bound to a failure result.
    - param `failureResult`: Result for other tools.
    - returns: Check function.
    - used 7x in the base game, e.g. `base/content/mission/mission/tutorial/tutorial_story.tl:443`: `checkBuilderId = build_construction_util.makeCheckConstructionBuilderId({ error = _("Pick the Headquarters!") }),`
- `checkSignal : function<TaskContext>(event : BuildConstructionUtil.HandleProposalEventParams, failureResult : MissionInterface.ProposalHandlerResult) : MissionInterface.ProposalHandlerResult | boolean`  (call as `build_construction_util.checkSignal(…)`): Passes a proposal that replaces one segment and adds exactly one edge object, a signal.
    - param `event`: Proposal event.
    - param `failureResult`: Result otherwise.
    - returns: `true` or `failureResult`.
- `makeCheckSignal : function(failureResult : MissionInterface.ProposalHandlerResult) : function<TaskContext>(event : BuildConstructionUtil.HandleProposalEventParams<TaskContext>) : MissionInterface.ProposalHandlerResult | boolean`  (call as `build_construction_util.makeCheckSignal(…)`): Returns `checkSignal` bound to a failure result.
    - param `failureResult`: Result otherwise.
    - returns: Check function.
    - used 1x in the base game, e.g. `mods/release/urbangames_campaign_mission_02/content/mission/mission/mission_story.tl:216`: `checkSignal = build_construction_util.makeCheckSignal({ error = _("Not a signal!") }),`
- `checkSignalLocation : function<TaskContext>(event : BuildConstructionUtil.HandleProposalEventParams, baseEdgeEntities : {Engine.Entity}, failureResult : MissionInterface.ProposalHandlerResult) : MissionInterface.ProposalHandlerResult | boolean`  (call as `build_construction_util.checkSignalLocation(…)`): Passes a proposal that adds one signal on one of the given edges.
    - param `event`: Proposal event.
    - param `baseEdgeEntities`: Allowed edges.
    - param `failureResult`: Result otherwise.
    - returns: `true` or `failureResult`.
- `makeCheckSignalLocation : function(baseEdgeEntities : {Engine.Entity}, failureResult : MissionInterface.ProposalHandlerResult) : function<TaskContext>(event : BuildConstructionUtil.HandleProposalEventParams<TaskContext>) : MissionInterface.ProposalHandlerResult | boolean`  (call as `build_construction_util.makeCheckSignalLocation(…)`): Returns `checkSignalLocation` bound to its arguments.
    - param `baseEdgeEntities`: Allowed edges.
    - param `failureResult`: Result otherwise.
    - returns: Check function.
    - used 1x in the base game, e.g. `mods/release/urbangames_campaign_mission_02/content/mission/mission/mission_story.tl:217`: `checkSignalLocation = build_construction_util.makeCheckSignalLocation(edges, { error = _("MISSION_PROPOSAL_FEEDBACK_NOT_IN_OUTLINED_AREA") }),`
- `checkSignalDirection : function<TaskContext>(event : BuildConstructionUtil.HandleProposalEventParams, baseEdgeEntitiesAndLeft : {Engine.Entity : boolean}, failureResult : MissionInterface.ProposalHandlerResult) : MissionInterface.ProposalHandlerResult | boolean`  (call as `build_construction_util.checkSignalDirection(…)`): Passes a proposal that adds one signal on one of the given edges, on the required side.
    - param `event`: Proposal event.
    - param `baseEdgeEntitiesAndLeft`: Allowed edge -> required `left` flag of the signal.
    - param `failureResult`: Result otherwise.
    - returns: `true` or `failureResult`.
- `makeCheckSignalDirection : function(baseEdgeEntitiesAndLeft : {Engine.Entity : boolean}, failureResult : MissionInterface.ProposalHandlerResult) : function<TaskContext>(event : BuildConstructionUtil.HandleProposalEventParams<TaskContext>) : MissionInterface.ProposalHandlerResult | boolean`  (call as `build_construction_util.makeCheckSignalDirection(…)`): Returns `checkSignalDirection` bound to its arguments.
    - param `baseEdgeEntitiesAndLeft`: Allowed edge -> required `left` flag.
    - param `failureResult`: Result otherwise.
    - returns: Check function.
    - used 1x in the base game, e.g. `mods/release/urbangames_campaign_mission_02/content/mission/mission/mission_story.tl:218`: `checkSignalDirection = build_construction_util.makeCheckSignalDirection(edgesAndLeft, { error = _("Signal direction is wrong!") }),`
- `checkStop : function<TaskContext>(event : BuildConstructionUtil.HandleProposalEventParams, failureResult : MissionInterface.ProposalHandlerResult) : MissionInterface.ProposalHandlerResult | boolean`  (call as `build_construction_util.checkStop(…)`): Passes a proposal that replaces one segment and adds one or two street stops.
    - param `event`: Proposal event.
    - param `failureResult`: Result otherwise.
    - returns: `true` or `failureResult`.
- `makeCheckStop : function(failureResult : MissionInterface.ProposalHandlerResult) : function<TaskContext>(event : BuildConstructionUtil.HandleProposalEventParams<TaskContext>) : MissionInterface.ProposalHandlerResult | boolean`  (call as `build_construction_util.makeCheckStop(…)`): Returns `checkStop` bound to a failure result.
    - param `failureResult`: Result otherwise.
    - returns: Check function.
    - used 2x in the base game, e.g. `base/content/mission/mission/tutorial/tutorial_story_util.tl:163`: `checkStop = build_construction_util.makeCheckStop({ error = _("Not a Station!") }),`
- `checkStopLocation : function<TaskContext>(event : BuildConstructionUtil.HandleProposalEventParams, baseEdgeEntities : {Engine.Entity}, failureResult : MissionInterface.ProposalHandlerResult) : MissionInterface.ProposalHandlerResult | boolean`  (call as `build_construction_util.checkStopLocation(…)`): Passes a proposal that adds one or two street stops on one of the given edges.
    - param `event`: Proposal event.
    - param `baseEdgeEntities`: Allowed edges.
    - param `failureResult`: Result otherwise.
    - returns: `true` or `failureResult`.
- `makeCheckStopLocation : function(baseEdgeEntities : {Engine.Entity}, failureResult : MissionInterface.ProposalHandlerResult) : function<TaskContext>(event : BuildConstructionUtil.HandleProposalEventParams<TaskContext>) : MissionInterface.ProposalHandlerResult | boolean`  (call as `build_construction_util.makeCheckStopLocation(…)`): Returns `checkStopLocation` bound to its arguments.
    - param `baseEdgeEntities`: Allowed edges.
    - param `failureResult`: Result otherwise.
    - returns: Check function.
    - used 2x in the base game, e.g. `base/content/mission/mission/tutorial/tutorial_story_util.tl:164`: `checkStopLocation = build_construction_util.makeCheckStopLocation(edges, { error = _("MISSION_PROPOSAL_FEEDBACK_NOT_IN_OUTLINED_AREA") }),`

{ error = _("MISSION_PROPOSAL_FEEDBACK_NOT_A_CARGO_ROAD_STATION") }
- `checkConstruction : function<TaskContext>(event : BuildConstructionUtil.HandleProposalEventParams, conFileName : string, hasCargoPlatform : boolean, failureResult : MissionInterface.ProposalHandlerResult, customCheck? : function(Proposal.ConstructionEntity) : boolean) : MissionInterface.ProposalHandlerResult | boolean`  (call as `build_construction_util.checkConstruction(…)`): Passes a proposal whose first new construction has the given file, passes `customCheck` and, if `hasCargoPlatform` is not `nil`, has that cargo-platform flag.
    - param `event`: Proposal event.
    - param `conFileName`: Required construction file.
    - param `hasCargoPlatform`: Required cargo-platform flag; `nil` accepts both.
    - param `failureResult`: Result otherwise.
    - param `customCheck`: Extra test of the proposed construction.
    - returns: `true`, or `failureResult` (`false` if not set).
- `makeCheckConstruction : function(conFileName : string, hasCargoPlatform : boolean, failureResult : MissionInterface.ProposalHandlerResult, customCheck? : function(Proposal.ConstructionEntity) : boolean) : function<TaskContext>(event : BuildConstructionUtil.HandleProposalEventParams<TaskContext>) : MissionInterface.ProposalHandlerResult | boolean`  (call as `build_construction_util.makeCheckConstruction(…)`): Returns `checkConstruction` bound to its arguments.
    - param `conFileName`: Required construction file.
    - param `hasCargoPlatform`: Required cargo-platform flag; `nil` accepts both.
    - param `failureResult`: Result otherwise.
    - param `customCheck`: Extra test of the proposed construction.
    - returns: Check function.
    - used 8x in the base game, e.g. `base/content/mission/mission/tutorial/tutorial_story.tl:444`: `checkConstruction = build_construction_util.makeCheckConstruction("::/landmarks/hq/headquarter.con", nil, { error = _("Pick the Headquarters!") }),`

{ error = _("MISSION_PROPOSAL_FEEDBACK_NOT_CONNECTED_TO_CROSSING") }
- `checkSnapping : function<TaskContext>(event : BuildConstructionUtil.HandleProposalEventParams, snapAnyOf : {Engine.Entity}, failureResult : MissionInterface.ProposalHandlerResult) : MissionInterface.ProposalHandlerResult | boolean`  (call as `build_construction_util.checkSnapping(…)`): Passes a proposal whose new segments use one of the given existing nodes, or any existing node when the list is empty.
    - param `event`: Proposal event.
    - param `snapAnyOf`: Nodes of which one must be used.
    - param `failureResult`: Result otherwise.
    - returns: `true`, or `failureResult` (`false` if not set).
- `makeCheckSnapping : function(snapAnyOf : {Engine.Entity}, failureResult : MissionInterface.ProposalHandlerResult) : function<TaskContext>(event : BuildConstructionUtil.HandleProposalEventParams<TaskContext>) : MissionInterface.ProposalHandlerResult | boolean`  (call as `build_construction_util.makeCheckSnapping(…)`): Returns `checkSnapping` bound to its arguments.
    - param `snapAnyOf`: Nodes of which one must be used; empty or `nil` accepts any existing node.
    - param `failureResult`: Result otherwise.
    - returns: Check function.
    - used 4x in the base game, e.g. `base/content/mission/mission/tutorial/tutorial_story.tl:852`: `checkSnapping = build_construction_util.makeCheckSnapping({}, { error = _("MISSION_PROPOSAL_FEEDBACK_NOT_CONNECTED_TO_STREET") }),`

{ error = _("MISSION_PROPOSAL_FEEDBACK_NOT_IN_OUTLINED_AREA") }
- `checkLocation : function<TaskContext>(event : BuildConstructionUtil.HandleProposalEventParams, targetPosX : number, targetPosY : number, targetMaxDist : number, failureResult : MissionInterface.ProposalHandlerResult) : MissionInterface.ProposalHandlerResult | boolean`  (call as `build_construction_util.checkLocation(…)`): Passes a proposal whose first new construction is within `targetMaxDist` metres of the target position.
    - param `event`: Proposal event.
    - param `targetPosX`: Target x position.
    - param `targetPosY`: Target y position.
    - param `targetMaxDist`: Maximum distance in metres.
    - param `failureResult`: Result otherwise.
    - returns: `true`, or `failureResult` (`false` if not set).
- `makeCheckLocation : function(targetPosX : number, targetPosY : number, targetMaxDist : number, failureResult : MissionInterface.ProposalHandlerResult) : function<TaskContext>(event : BuildConstructionUtil.HandleProposalEventParams<TaskContext>) : MissionInterface.ProposalHandlerResult | boolean`  (call as `build_construction_util.makeCheckLocation(…)`): Returns `checkLocation` bound to its arguments.
    - param `targetPosX`: Target x position.
    - param `targetPosY`: Target y position.
    - param `targetMaxDist`: Maximum distance in metres.
    - param `failureResult`: Result otherwise.
    - returns: Check function.
    - used 6x in the base game, e.g. `mods/release/urbangames_campaign_mission_01/content/mission/mission/mission_story.tl:778`: `checkLocation = build_construction_util.makeCheckLocation(params.streetDepotZone.pos[1], params.streetDepotZone.pos[2], 20, { error = _("MISSION_PROPOSAL_FEEDBA`
- `makeCheckLocations : function(tagetPositions : {{number, number}}, targetMaxDist : number, failureResult : MissionInterface.ProposalHandlerResult) : function<TaskContext>(event : BuildConstructionUtil.HandleProposalEventParams<TaskContext>) : MissionInterface.ProposalHandlerResult | boolean`  (call as `build_construction_util.makeCheckLocations(…)`): Returns a check that passes when the construction is near any of several positions.
    - param `tagetPositions`: Target positions `{x, y}`.
    - param `targetMaxDist`: Maximum distance in metres.
    - param `failureResult`: Result otherwise.
    - returns: Check function.
- `checkRestrictedArea : function<TaskContext>(event : BuildConstructionUtil.HandleProposalEventParams, areas : {PolygonUtil.Polygon}, failureResult : MissionInterface.ProposalHandlerResult, allowStreetTerminalBuilder? : boolean) : MissionInterface.ProposalHandlerResult | boolean`  (call as `build_construction_util.checkRestrictedArea(…)`): Blocks proposals that build, bulldoze or add nodes inside the areas, and every tool other than construction, bulldozer, track and street builder.
    - param `event`: Proposal event.
    - param `areas`: Restricted polygons.
    - param `failureResult`: Result for changes inside the areas.
    - param `allowStreetTerminalBuilder`: Also allows the street-stop tool.
    - returns: `true`, `failureResult`, or an error result for other tools.
- `makeRestrictedArea : function(areas : {PolygonUtil.Polygon}, failureResult : MissionInterface.ProposalHandlerResult, allowStreetTerminalBuilder? : boolean) : function<TaskContext>(event : BuildConstructionUtil.HandleProposalEventParams<TaskContext>) : MissionInterface.ProposalHandlerResult | boolean`  (call as `build_construction_util.makeRestrictedArea(…)`): Returns `checkRestrictedArea` bound to its arguments.
    - param `areas`: Restricted polygons.
    - param `failureResult`: Result for changes inside the areas.
    - param `allowStreetTerminalBuilder`: Also allows the street-stop tool.
    - returns: Check function.
    - used 2x in the base game, e.g. `mods/release/urbangames_campaign_mission_02/content/mission/mission/mission_story.tl:371`: `checkRestrictedArea = build_construction_util.makeRestrictedArea(params.restrictedAreaPolygons, { error = _("MISSION_PROPOSAL_FEEDBACK_RESTRICTED_AREA") }, true`
- `handleConstructionProposal : function<TaskContext>(event : BuildConstructionUtil.HandleProposalEventParams<TaskContext>, config : HandleProposalEventConfig<TaskContext>, taskApi : MissionInterface.DeferredTaskApi, isWhitelist : boolean) : MissionInterface.ProposalHandlerResult`  (call as `build_construction_util.handleConstructionProposal(…)`): this is the 'master' function for 'builder.proposalCreate'
    - used 1x in the base game, e.g. `base/content/mission/mission/tasks/build_construction/build_construction.tl:42`: `return build_construction_util.handleConstructionProposal(event, config as BuildConstructionUtil.HandleProposalEventConfig<TaskContext>, taskApi, proposalHandle`

### BuildConstructionUtil.ProposalHandlerConfig (record)

```teal
record ProposalHandlerConfig
```

Checks used for proposal previews (filter) and for applied proposals (success).

- `proposalApply : BuildConstructionUtil.HandleProposalEventConfig<any>`: to detect success
- `proposalFilter : BuildConstructionUtil.HandleProposalEventConfig<any>`: to disallow stuff
- `proposalFilterWhitelist : boolean`: controls how multiple proposal handlers are combined

### BuildConstructionUtil.HandleProposalEventParams (record)

```teal
record HandleProposalEventParams<TaskContext>
```

A proposal event passed to the checks.

- `id : string`: Id of the tool that made the proposal (e.g. `constructionBuilder`, `streetTerminalBuilder`, `bulldozer`, `trackBuilder`, `streetBuilder`).
- `name : string`: Event name (`builder.proposalCreate` or `builder.proposalApply`).
- `param : ProposalEventData`: Proposal, its data and, on apply, the result entities.
- `isApply : boolean`: The proposal is being applied.
- `ctx : TaskContext`: Context of the task.

### BuildConstructionUtil.HandleProposalEventConfig (record)

```teal
record HandleProposalEventConfig<TaskContext>
```

Set of checks; unset checks are skipped. The `checkStreetTerminalBuilderId` field is not read by the handler; use `checkBuilderId` with `makeCheckStreetTerminalBuilderId`.

- `checkStreetTerminalBuilderId : function(HandleProposalEventParams<TaskContext>) : MissionInterface.ProposalHandlerResult | boolean`: return true if builder is ok, false/nil to skip, or if you like a specific message
- `checkBuilderId : function(HandleProposalEventParams<TaskContext>) : MissionInterface.ProposalHandlerResult | boolean`: return true if builder is ok, false/nil to skip, or if you like a specific message
- `checkConstruction : function(HandleProposalEventParams<TaskContext>) : MissionInterface.ProposalHandlerResult | boolean`: Checks the construction being built.
    - param `#1`: Proposal event.
    - returns: `true`, a failure result, or `false`/`nil`.
- `checkSignal : function(HandleProposalEventParams<TaskContext>) : MissionInterface.ProposalHandlerResult | boolean`: Checks that the proposal adds a signal.
    - param `#1`: Proposal event.
    - returns: `true`, a failure result, or `false`/`nil`.
- `checkSignalLocation : function(HandleProposalEventParams<TaskContext>) : MissionInterface.ProposalHandlerResult | boolean`: Checks where a signal is placed.
    - param `#1`: Proposal event.
    - returns: `true`, a failure result, or `false`/`nil`.
- `checkSignalDirection : function(HandleProposalEventParams<TaskContext>) : MissionInterface.ProposalHandlerResult | boolean`: Checks the side of a signal.
    - param `#1`: Proposal event.
    - returns: `true`, a failure result, or `false`/`nil`.
- `checkStop : function(HandleProposalEventParams<TaskContext>) : MissionInterface.ProposalHandlerResult | boolean`: Checks that the proposal adds a street stop.
    - param `#1`: Proposal event.
    - returns: `true`, a failure result, or `false`/`nil`.
- `checkStopLocation : function(HandleProposalEventParams<TaskContext>) : MissionInterface.ProposalHandlerResult | boolean`: Checks where a street stop is placed.
    - param `#1`: Proposal event.
    - returns: `true`, a failure result, or `false`/`nil`.
- `checkSnapping : function(HandleProposalEventParams<TaskContext>) : MissionInterface.ProposalHandlerResult | boolean`: Checks that new segments connect to existing nodes.
    - param `#1`: Proposal event.
    - returns: `true`, a failure result, or `false`/`nil`.
- `checkLocation : function(HandleProposalEventParams<TaskContext>) : MissionInterface.ProposalHandlerResult | boolean`: Checks the position of the construction.
    - param `#1`: Proposal event.
    - returns: `true`, a failure result, or `false`/`nil`.
- `checkRestrictedArea : function(HandleProposalEventParams<TaskContext>) : MissionInterface.ProposalHandlerResult | boolean`: Blocks changes inside restricted areas.
    - param `#1`: Proposal event.
    - returns: `true`, a failure result, or `false`/`nil`.
