# game_mechanics.subventions.subvention_legacy_util

Definition file: `base/tealdef/game_mechanics/subventions/subvention_legacy_util.d.tl` (in the game folder).

Upgrade of subsidy state from older savegames.

## SubventionLegacyUtil (record)

```teal
global record SubventionLegacyUtil
```
Load: `local subvention_legacy_util = ug_require "::/game_mechanics/subventions/subvention_legacy_util.tl" as SubventionLegacyUtil`

Upgrade of `SubventionState` from older savegames, run by the subsidy game script on `handleLegacy`.

- `CurrentVersion : integer`: Current state version (2).
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/subvention_util.tl:49`: `version = subvention_legacy_util.CurrentVersion,`
- `updateLegacyState : function(subventionState : SubventionState)`  (call as `subvention_legacy_util.updateLegacyState(…)`): Upgrades a subsidy state in place to `CurrentVersion`. Version 2 moved the subsidy res files from `::/subventions/...` to `::/game_mechanics/subventions/...`.
    - param `subventionState`: State to upgrade.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/subventions.script.tl:385`: `subvention_legacy_util.updateLegacyState(subventionState)`
