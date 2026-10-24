# scripts.random_util

Definition file: `base/tealdef/scripts/random_util.d.tl` (in the game folder).

Types of `/scripts/random_util.tl` (`RandomUtil`) for weighted random selection.

## RandomUtil (record)

```teal
global record RandomUtil
```
Load: `local random_util = ug_require "::/scripts/random_util.tl" as RandomUtil`

Random helpers returned by `ug_require "/scripts/random_util.tl"`.

- `pickRandomWeightedItem : function<T>({{T, number}}) : T`  (call as `random_util.pickRandomWeightedItem(…)`): Picks one item at random, with probability proportional to its weight.
    - param `#1`: List of `{item, weight}` pairs; weights are non-negative numbers.
    - returns: The chosen item.
    - used 4x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/deliver_cargo_town/deliver_cargo_town.script.tl:288`: `local townEntityAndCargoTypeId = random_util.pickRandomWeightedItem(allTowns)`
