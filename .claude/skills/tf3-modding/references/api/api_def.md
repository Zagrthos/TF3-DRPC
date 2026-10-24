# api_def

Definition file: `api/tealdef/api_def.d.tl` (in the game folder).

Aggregate definition file: requires every engine API definition module in order
(`api.type`, `api.type.mod`, `api.type.modhub`, `api.type.transformator`, `api.res`,
`api.engine`, `api.engine.system`, `api.engine.util`, `api.cmd`, `app`, `api.gui`,
`api`, `main`), so one require makes the whole engine API known to the type checker.

Requires: `api.type`, `api.type.mod`, `api.type.modhub`, `api.type.transformator`, `api.res`, `api.engine`, `api.engine.system`, `api.engine.util`, `api.cmd`, `app`, `api.gui`, `api`, `main`
