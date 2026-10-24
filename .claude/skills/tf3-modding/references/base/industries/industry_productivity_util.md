# industries.industry_productivity_util

Definition file: `base/tealdef/industries/industry_productivity_util.d.tl` (in the game folder).

Types of `/industries/industry_productivity_util.tl` (`IndustryProductivityUtil`): helpers for the
worker boost of industries, which raises an industry's productivity while enough workers arrive by line.

## IndustryProductivityUtil (record)

```teal
global record IndustryProductivityUtil
```

Worker boost helpers returned by `ug_require "/industries/industry_productivity_util.tl"`.

- `workersTimeIntervalMax : integer`: How long a worker visit counts, two game years (`2 * api.util.getDefaultYearDuration()`).
- `getIndustryWorkersState : function(Engine.Entity) : IndustriesWorkersState.IndustryState`: Reads the worker state of one industry from the `industry_workers` game script.
    - returns: The state, or nil if the script or the industry has none.
- `isIndustryWorkersBoosted : function(Engine.Entity, numWorkers? : integer) : boolean`: True if at least half of the industry's person capacity (`RequiredWorkersFractionForBoost`) arrived as workers.
    - param `numWorkers`: Number of workers to test; defaults to the number of recent visitors.
- `getVisitors : function(Engine.Entity) : {IndustriesWorkersState.IndustryState.Visitor}`: The workers that recently reached the industry by line (empty if none).
- `getIndustryProductivity : function(Engine.Entity) : number`: The productivity modifier of the industry's stock list; above 1.0 while boosted, 0 without a stock list.
- `getPersonCapacityPotentialBoost : function(Engine.Entity) : number`: The productivity factor the industry gets when boosted.
    - returns: `workersBoostFactor` from the construction's metadata if set, otherwise a value between 1.5 and 2.5 derived from the construction seed (2 if there is no seed).
- `RequiredWorkersFractionForBoost : number`: Share of the person capacity that must be filled with workers for the boost (0.5).
