# mission.tasks.task_decorator

Definition file: `base/tealdef/mission/tasks/task_decorator.d.tl` (in the game folder).

Wraps a task script with a second script (decorator) that adds behaviour.

## TaskDecorator (record)

```teal
global record TaskDecorator
```
Load: `local task_decorator = ug_require "::/mission/tasks/task_decorator.tl" as TaskDecorator`

Combines a task script with a decorator script into one task script.

Notes:

The decorated task is named `<taskScriptName>_decorated`. Its state is a `DecoratorState`
and its params a `DecoratorParam`. The wrapped task's callbacks get only their own part
(`taskParam`, `taskState`); the decorator's callbacks get the whole context. The task is
complete when the decorator's `isComplete` (if any) and the task's `isComplete` both
return true. The decorator's `getInfo` is ignored.

Example:
```lua
-- urbangames_campaign_mission_01
taskScript = task_decorator.decorate(collect_entity, guide_collect_entity) as MissionInterface.TaskScript<any, any>,
taskParams : DecoratorParam<MissionTaskCollectEntity.Params, MissionTaskGuideCollectEntity.Param> = {
    taskParam = { soundEffectFile = params.crocodileSound, getEntities = getEntities },
    decoratorParam = guideParam,
},
```

- `decorate : function<S, P, GS, GP>(task : MissionInterface.TaskScript<S, P>, decorator : MissionInterface.TaskScript<DecoratorState<S, GS>, DecoratorParam<P, GP>>) : MissionInterface.TaskScript<DecoratorState<S, GS>, DecoratorParam<P, GP>>`  (call as `task_decorator.decorate(…)`): Builds a task script that runs `task` and `decorator` side by side.
    - param `task`: Task script to wrap.
    - param `decorator`: Decorator script working on the combined state and params.
    - returns: Combined task script.
    - used 5x in the base game, e.g. `mods/release/urbangames_campaign_mission_01/content/mission/mission/mission_story.tl:464`: `taskScript = task_decorator.decorate(collect_entity, guide_collect_entity) as MissionInterface.TaskScript<any, any>,`

## DecoratorState (record)

```teal
global record DecoratorState<SpecificState, DecoratorSpecificState>
```

State of a decorated task.

- `taskState : SpecificState`: State of the wrapped task.
- `decoratorState : DecoratorSpecificState`: State of the decorator.

## DecoratorParam (record)

```teal
global record DecoratorParam<TaskSpecificParam, DecoratorSpecificParam>
```

Params of a decorated task.

- `taskParam : TaskSpecificParam`: Params of the wrapped task.
- `decoratorParam : DecoratorSpecificParam`: Params of the decorator.
