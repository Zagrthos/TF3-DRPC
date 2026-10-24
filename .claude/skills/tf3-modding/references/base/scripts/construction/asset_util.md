# scripts.construction.asset_util

Definition file: `base/tealdef/scripts/construction/asset_util.d.tl` (in the game folder).

Types of `/scripts/construction/asset_util.tl` (`AssetUtil`), the table of named asset groups that
`addModelsAndGroups` picks random models from.

## AssetUtil (record)

```teal
global record AssetUtil
```
Load: `local asset_util = require "::/scripts/construction/asset_util.tl" as AssetUtil`

The asset group table returned by `ug_require "/scripts/construction/asset_util.tl"`. Generated data
that names a group gets a random model from `assets[name]`.

- `assets : {string : ModelList}`: Map of asset group names to list of model file paths
    - used 1x in the base game, e.g. `base/content/scripts/scripts/construction/constructionutil.lua:143`: `group = asset_util.assets[actualGroupName]`

### AssetUtil.ModelList (record)

```teal
record ModelList is {string}
```

paths pointing to .mdl files
