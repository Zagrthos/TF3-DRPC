# mission.tasks.connect_nodes.mission_task_connect_nodes

Definition file: `base/tealdef/mission/tasks/connect_nodes/mission_task_connect_nodes.d.tl` (in the game folder).

Task that only allows street or track proposals that connect given nodes.

## MissionTaskConnectNodes (record)

```teal
global record MissionTaskConnectNodes is MissionInterface.TaskScript<MissionTaskConnectNodes.State, MissionTaskConnectNodes.Params>
```
Load: `local connect_nodes = ug_require "::/mission/tasks/connect_nodes/connect_nodes.tl" as MissionTaskConnectNodes`

Task (`connect_nodes`) that blocks every street or track proposal that does not connect one of the configured node pairs (the bulldozer stays allowed). It completes when all pairs were connected.

### MissionTaskConnectNodes.Mode (enum)

```teal
enum Mode
```

Tool the player must use.

- `"Tracks"`: Only the track builder.
- `"Street"`: Only the street builder.

### MissionTaskConnectNodes.Config (record)

```teal
record Config
```

One connection to build (checked with `api.engine.util.proposal.checkProposalConnectsNodes`).

- `startNode : Engine.Entity`: Start node.
- `startEdge : Engine.Entity`: Edge at the start node.
- `targetNode : Engine.Entity`: Target node.

### MissionTaskConnectNodes.Params (record)

```teal
record Params
```

Params of the task.

- `mode : Mode`: Tool the player must use.
- `configs : {Config}`: Connections to build.
- `renderZonesForEntities : {{MissionInterface.PosRadius, MissionInterface.PosRadius}}`: Area pair drawn per connection (required); `pos[3]` is the zone height.

### MissionTaskConnectNodes.State (record)

```teal
record State
```

State of the task.

- `complete : {boolean}`: Per connection, whether it was built.
