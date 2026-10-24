# mission.tasks.build_construction.mission_task_build_construction_variant

Definition file: `base/tealdef/mission/tasks/build_construction/mission_task_build_construction_variant.d.tl` (in the game folder).

Task that waits until constructions of given files exist in given areas.

## MissionTaskBuildConstructionVariant (record)

```teal
global record MissionTaskBuildConstructionVariant is MissionInterface.TaskScript<MissionTaskBuildConstructionVariant.State, MissionTaskBuildConstructionVariant.Params>
```
Load: `local build_construction_variant = ug_require "::/mission/tasks/build_construction/build_construction_variant.tl" as MissionTaskBuildConstructionVariant`

Task (`build_construction_variant`) that completes when at least `minCount` constructions of the listed files exist in the areas (circles or polygons), or when `customCheckFn` returns enough entities. The areas are drawn as zones.

### MissionTaskBuildConstructionVariant.Params (record)

```teal
record Params
```

Params of the task. Give `areas`, `getAreas`, `getPolygonAreas` or `customCheckFn`.

- `areas : {MissionInterface.PosRadius}`: Circular areas to search.
- `getAreas : function() : {MissionInterface.PosRadius}`: Returns the circular areas; takes precedence over `areas`.
    - returns: Areas.
- `getPolygonAreas : function() : {PolygonUtil.Polygon}`: Returns polygon areas, used when no circular areas are set.
    - returns: Polygons.
- `conFiles : {ResName}`: Accepted construction files.
- `checkFn : function(entity : Engine.Entity) : boolean`: Extra test of a found construction.
    - param `entity`: Construction entity.
    - returns: `true` to count it.
- `customCheckFn : function() : {Engine.Entity}`: Replaces the area search.
    - returns: Constructions that count.
- `minCount : integer`: Number needed (default 1). A counter is shown above 1.
- `dontDraw : boolean`: Does not draw the areas.

### MissionTaskBuildConstructionVariant.State (record)

```teal
record State
```

State of the task.

- `resultEntities : {Engine.Entity}`: Constructions found at the last update.
