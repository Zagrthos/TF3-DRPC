# api

Definition file: `api/tealdef/api.d.tl` (in the game folder).

Declares the global `api` table, the entry point of the engine scripting API, and the
globals `app` and `loaderHelper`. `api.engine` reads the entity component system,
`api.cmd` changes it through commands, `api.res` holds the resource repositories,
`api.type` the constructors and enums, `api.gui` the GUI, `api.util` general helpers
and `api.modhub` the mod hub backends.

Requires: `api.util`, `api.modhub`

## Globals

- `app : App`: Global application object (`App`), e.g. `app.getProgressMonitor()` or `app.getUserProfile()`; used by the main menu and the in-game menus.

    ```lua
    local monitor = app.getProgressMonitor()
    local progress, task = monitor:getProgress(), monitor:getTask()
    ```

- `loaderHelper : LoaderHelper`: Global `LoaderHelper` object; the base script `scripts/util.tl` calls `loaderHelper:useFn(ref)` before it resolves a script function reference of the form `"<file>@<path>"`.

## api (record)

```teal
global record api
```

Global table with the engine scripting API; each field is one sub-API.

Example:
```lua
local playerAccount = api.engine.getComponent(api.engine.util.getPlayer(), api.type.ComponentType.ACCOUNT)
local passengers = api.res.cargoTypeRep.getPassengerCargoTypeId()
local text = api.util.lang.formatInt(12345)
```

- `res : Res`: Resource repositories (models, constructions, streets, cargo types, generic resources, ...); see `Res`.
- `gui : Gui`: GUI functionality apart from react; see `Gui`.
    - used 47x in the base game, e.g. `base/content/gui/gui/main/builtin.lua:9`: `builtin.type.Orientation = api.gui.react.params.builtin.Orientation`
- `engine : Engine`: Read-only access to the engine state (the entity component system), e.g. `api.engine.getComponent`; see `Engine`.
- `type : Type`: Constructors, component type ids and enums used by the other APIs (e.g. `api.type.ComponentType`, `api.type.SimpleProposal.new()`).
    - used 912x in the base game, e.g. `base/content/base/base/mod.lua:187`: `if api.util.isPlatform(api.type["enum"].Platform.PS) then`
- `util : ApiUtil`: Generic helper functions that do not depend on a game instance (formatting, platform checks, settings); see `ApiUtil`.
    - used 2x in the base game, e.g. `base/content/scripts/scripts/app_util.tl:4`: `return api.util ~= nil and api.util.getInputMode ~= nil and api.util.getInputMode() == api.type["enum"].InputMode.Gamepad`
- `cmd : Cmd`: Commands that change the game state, created with the `make...Cmd` functions and sent with `api.cmd.sendCommand`; see `Cmd`.
- `modhub : ModhubApi`: Interface to the mod hub backends (mod browsing, subscriptions, installation, login); see `ModhubApi`.
