# mission.modifier

Definition file: `base/tealdef/mission/modifier.d.tl` (in the game folder).

Resource-modifier helper that missions use to patch loaded game resources by file path.

## Modifier (record)

```teal
global record Modifier
```
Load: `local modifier = require "::/mission/modifier.lua" as Modifier`

Resource-modifier helper for missions.

- `treevisitor : function(tree : any)`  (call as `modifier.treevisitor(…)`): Registers modifiers for all resource types that walk a table tree by the parts of the loaded file path and call the function found there on the resource data.
    - param `tree`: Nested tables keyed by path segments (with `default` as fallback), whose leaves are functions `function(data)`.
