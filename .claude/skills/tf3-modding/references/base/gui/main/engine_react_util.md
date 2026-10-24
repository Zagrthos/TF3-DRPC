# gui.main.engine_react_util

Definition file: `base/tealdef/gui/main/engine_react_util.d.tl` (in the game folder).

React hooks that mirror engine data into React state. The step and timer hooks re-read the
engine through a `getFromEngine` callback; the parallel hooks run a registered function
(`useFnName`) in a worker thread and pass its result through `finalize`. The hooks update the
state only when the value changed, and most of them also return a `commit` function that sets the
state and sends engine commands.

## EngineReactUtil (record)

```teal
global record EngineReactUtil
```
Load: `local engine_react_util = ug_require "::/gui/main/engine_react_util.tl" as EngineReactUtil`

React hooks that keep a React state in sync with engine data and send commands to change it.

Notes:

Hooks must be called from a recipe function, like all React hooks, and always in the same order.

The commit function returned by most hooks has the form `commit(newState, cmd?, callback?)`:
it calls `onChange(newState, old, true)`, sets the state, then sends `cmd` (or the commands
built by `makeCommand`/`makeCommands` when `cmd` is `nil`) with `api.cmd.sendCommand`.
While commands are pending, the engine is not re-read, so the state does not jump back
before the command has been applied. The default comparison is `table_util.deepEquals`.

- `useStepStateMulti : function<StateTypeT, CommandDataT is ICommandData>(getFromEngine : (function(old? : StateTypeT) : StateTypeT), makeCommands? : (function(StateTypeT) : {{Command<CommandDataT>, function(cmdData : CommandDataT, success : boolean, affectedEntities : {{Engine.Entity, Engine.Revision}})}}), stateEqualsFn? : (function(StateTypeT, StateTypeT) : boolean), onChange? : function(now : StateTypeT, old : StateTypeT, commit : boolean), once? : boolean) : (ReactStateT<StateTypeT>, function(type : StateTypeT, cmd? : Command<CommandDataT>, callback? : function(cmdData : CommandDataT, success : boolean, affectedEntites : {{Engine.Entity, Engine.Revision}})))`  (call as `engine_react_util.useStepStateMulti(…)`): react state, commit fn

Like `useStepState`, but `makeCommands` returns a list of `{command, callback}` pairs that are all sent on commit.
    - used 3x in the base game, e.g. `base/content/gui/gui/debug_panel/make_entity_debug_panel.tl:621`: `local state, commit = engine_react_util.useStepStateMulti(makeState, makeCommands)`
- `useStepState : function<StateTypeT, CommandDataT is ICommandData>(getFromEngine : (function(old? : StateTypeT) : StateTypeT), makeCommand? : (function(StateTypeT) : Command<CommandDataT>, (function(CommandDataT, boolean, {{Engine.Entity, Engine.Revision}}))), stateEqualsFn? : (function(typeA : StateTypeT, typeB : StateTypeT) : boolean), onChange? : function(now : StateTypeT, old : StateTypeT, commit : boolean), once? : boolean) : (ReactStateT<StateTypeT>, function(type : StateTypeT, cmd? : Command<CommandDataT>, callback? : function(cmdData : CommandDataT, success : boolean, affectedEntites : {{Engine.Entity, Engine.Revision}})))`  (call as `engine_react_util.useStepState(…)`): react state, commit fn

Creates a React state that is re-read from the engine every step and can be committed back with a command.
    - param `getFromEngine`: Reads the current value from the engine; receives the previous value (or `nil` the first time).
    - param `makeCommand`: Optional; builds the command (and optionally a result callback as second return value) used by `commit` when no command is passed to it.
    - param `stateEqualsFn`: Optional comparison of old and new value; defaults to `table_util.deepEquals`.
    - param `onChange`: Optional; called before the state changes, with `commit = true` when the change comes from the commit function.
    - param `once`: Optional; when `true`, the commit function only works while no command has been sent yet (avoids double clicks).
    - returns: The React state and the commit function `commit(newState, cmd?, callback?)`.
    - used 313x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company.tl:679`: `local companyState = engine_react_util.useStepState(makeCompanyState)`
- `useStepStateTimer : function<StateTypeT>(getFromEngine : (function(old? : StateTypeT) : StateTypeT), interval? : number, stateEqualsFn? : (function(typeA : StateTypeT, typeB : StateTypeT) : boolean)) : ReactStateT<StateTypeT>`  (call as `engine_react_util.useStepStateTimer(…)`): react state

Creates a read-only React state that is re-read from the engine at a fixed interval instead of every step.
    - param `getFromEngine`: Reads the current value from the engine; receives the previous value (or `nil` the first time).
    - param `interval`: Optional interval between reads, in seconds; defaults to 0.5.
    - param `stateEqualsFn`: Optional comparison of old and new value; defaults to `table_util.deepEquals`.
    - returns: The React state.
    - used 124x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company_notification_greenify.script.tl:15`: `local state = engine_react_util.useStepStateTimer(makeState)`
- `useStepStateTimerWithCommit : function<StateTypeT, CommandDataT is ICommandData>(getFromEngine : (function(old? : StateTypeT) : StateTypeT), interval? : number, makeCommand? : (function(StateTypeT) : Command<CommandDataT>, (function(CommandDataT, boolean, {{Engine.Entity, Engine.Revision}}))), stateEqualsFn? : (function(typeA : StateTypeT, typeB : StateTypeT) : boolean), onChange? : function(now : StateTypeT, old : StateTypeT, commit : boolean), once? : boolean) : (ReactStateT<StateTypeT>, function(type : StateTypeT, cmd? : Command<CommandDataT>, callback? : function(cmdData : CommandDataT, success : boolean, affectedEntites : {{Engine.Entity, Engine.Revision}})))`  (call as `engine_react_util.useStepStateTimerWithCommit(…)`): react state, commit fn

Like `useStepStateTimer` (re-read at an interval, default 0.5 seconds), but also returns a commit function as in `useStepState`.
    - used 7x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company.tl:943`: `local playerNameState, commitName = engine_react_util.useStepStateTimerWithCommit(makePlayerNameState, 0, makeNameCommand)`
- `useStepStateParallel : function<StateTypeT, ParamT, WorkItemResultT, CommandDataT is ICommandData>(useFnName : string, useFnParams : ParamT, finalize : (function(result : WorkItemResultT, useFnParams : ParamT, old? : StateTypeT) : StateTypeT), makeCommand? : (function(StateTypeT) : Command<CommandDataT>, (function(CommandDataT, boolean, {{Engine.Entity, Engine.Revision}}))), stateEqualsFn? : (function(StateTypeT, StateTypeT) : boolean), onChange? : (function(now : StateTypeT, old : StateTypeT, commit : boolean)), once? : boolean) : (ReactStateT<StateTypeT>, function(type : StateTypeT, cmd? : Command<CommandDataT>, callback? : function(cmdData : CommandDataT, success : boolean, affectedEntites : {{Engine.Entity, Engine.Revision}})))`  (call as `engine_react_util.useStepStateParallel(…)`): make state that will update each step but in another thread. The result is finalize()d in the main thread and the resulting dirty state is applied next frame.
react state, commit fn

Creates a React state computed each step in a worker thread by a named function, finalized on the main thread.
    - param `useFnName`: Name of the function to run, as accepted by `util.useFn` (a script reference such as `"::/path/file.script@fnName"`).
    - param `useFnParams`: Parameter passed to that function (and to `finalize`).
    - param `finalize`: Turns the worker result into the state value on the main thread; receives the result, `useFnParams` and the previous state.
    - param `makeCommand`: Optional; builds the command used by the commit function.
    - param `stateEqualsFn`: Optional comparison; defaults to `table_util.deepEquals`.
    - param `onChange`: Optional; called before a committed change.
    - param `once`: Optional; allow only one commit.
    - returns: The React state and the commit function.
    - notes: The initial value is computed synchronously on the main thread. Afterwards the function is enqueued with `react.enqueueParallel` every step while no command is pending.
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_react_util.tl:170`: `local reactState = engine_react_util.useStepStateParallel(`

make state that will update each step but in another thread. The resulting dirty state is applied next frame.
- `useStepStateParallelSimple : function<StateTypeT, ParamT>(useFnName : string, useFnParams : ParamT, stateEqualsFn? : (function(StateTypeT, StateTypeT))) : (ReactStateT<StateTypeT>, function(type : StateTypeT, cmd? : Command<ICommandData>, callback? : function(cmdData : ICommandData, success : boolean, affectedEntites : {{Engine.Entity, Engine.Revision}})))`  (call as `engine_react_util.useStepStateParallelSimple(…)`): react state, commit fn

`useStepStateParallel` without a `finalize` step and without a command: the worker result is the state.
    - used 11x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_react_util.tl:264`: `local state = engine_react_util.useStepStateParallelSimple("::/game_mechanics/towns/town_util_parallel.script@town_util_parallel.getDeliveriesState", params.ent`
- `useDummyStepState : function()`  (call as `engine_react_util.useDummyStepState(…)`): Calls the same hooks as `useStepStateMulti` (a ref and a state) without doing anything.
    - notes: Use it in a branch where the real step-state hook is skipped, so that the order of hook calls in the recipe stays the same between renders.
