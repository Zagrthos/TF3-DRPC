# industries.industry_workers

Definition file: `base/tealdef/industries/industry_workers.d.tl` (in the game folder).

State of the `::/industries/industry_workers.gs` game script, which records the workers that reach each industry by line.

## IndustriesWorkersState (record)

```teal
global record IndustriesWorkersState
```

State of the `industry_workers` game script.

- `industriesState : {Engine.Entity : IndustryState}`: Worker state per industry entity.

### IndustriesWorkersState.IndustryState (record)

```teal
record IndustryState
```

Worker visits of one industry.

- `lastVisitors : {Visitor}`: Recent visitors, newest first; at most 20, none older than `workersTimeIntervalMax`.
- `pendingVisitors : {Visitor}`: Arrivals not yet processed by the next update.

#### IndustriesWorkersState.IndustryState.Visitor (record)

```teal
record Visitor
```

One worker who arrived at the industry.

- `time : integer`: Game time of the arrival.
- `entity : EntityUtil.EntityAndRevision`: The sim person, with revision.
