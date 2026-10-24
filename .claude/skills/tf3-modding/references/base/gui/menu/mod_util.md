# gui.menu.mod_util

Definition file: `base/tealdef/gui/menu/mod_util.d.tl` (in the game folder).

Checks the dependencies, load order and incompatibilities of a list of active mods and turns the findings into a readable message.

## ModUtil (record)

```teal
global record ModUtil
```

Util table of `mod_util.tl`.

- `validateActiveMods : function(activeMods : {string}, onlyMods? : {string}) : {ModUtil.ToAddModResult}, {ModUtil.MissingModResult}, {ModUtil.ModOrderResult}, {ModUtil.ModIncompatibilityResult}`: Validates every installed mod of an activation list with `validateMod` and collects the problems.
    - param `activeMods`: Mod names (folder ids) in activation order.
    - param `onlyMods`: When given, only these mods of `activeMods` are checked (the order still comes from `activeMods`). Optional.
    - returns: Four lists: inactive dependencies (`ToAddModResult`), missing dependencies (`MissingModResult`), dependencies loaded too late (`ModOrderResult`) and incompatibilities (`ModIncompatibilityResult`).
- `validateMod : function(modDesc : Mod.ModDesc, order : integer, modName : string, modOrder : {string : integer}, toAdd : {ModUtil.ToAddModResult}, missing : {ModUtil.MissingModResult}, incorrectOrder : {ModUtil.ModOrderResult}, incompatible : {ModUtil.ModIncompatibilityResult})`: Checks one mod's dependencies and incompatibilities against the active mods and appends the problems to the four result lists.
    - param `modDesc`: Description of the mod to check, from the mod repository.
    - param `order`: Position of the mod in the activation order.
    - param `modName`: Name (folder id) of the mod.
    - param `modOrder`: Position of each active mod, keyed by mod name.
    - param `toAdd`: List that receives dependencies that are installed but not active.
    - param `missing`: List that receives dependencies that are not installed or have a wrong revision.
    - param `incorrectOrder`: List that receives `loadBefore` dependencies that are active but come later.
    - param `incompatible`: List that receives active mods (in a matching revision) that this mod declares incompatible.
    - notes: A dependency on a built-in mod with a wrong version is reported as `IncorrectVersion`. In the base game the built-in check runs twice, so such a problem appears twice in `missing`.
- `getProblemDescription : function(toAdd : {ModUtil.ToAddModResult}, missing : {ModUtil.MissingModResult}, incorrectOrder : {ModUtil.ModOrderResult}, incompatible : {ModUtil.ModIncompatibilityResult}) : string, boolean`: Formats the results of `validateActiveMods` or `validateMod` as a translated, multi-line message.
  In game: "The following mods are missing:", "The following dependent mods are not active:", "The following mods are incompatbile:" (German: „Die folgenden Mods fehlen:“, „Die folgenden abhängigen Mods sind nicht aktiv:“, „Die folgenden Mods sind inkompatibel:“)
    - returns: The message (empty string when there are no problems) and whether a problem is critical. The second value is true
when a required (non-optional) dependency is inactive or missing, or when there is any incompatibility. Wrong load order alone is not critical. When there
are no problems the second value is nil.

### ModUtil.ToAddModResult (record)

```teal
record ToAddModResult
```

A dependency that is installed in a matching version but not active.

- `dependent : string`: Name (folder id) of the mod that declares the dependency.
- `name : string`: Name (folder id) of the required mod.
- `loadBefore : boolean`: True when the dependency must be loaded before the dependent mod.
- `optional : boolean`: True when the dependency is optional.

### ModUtil.MissingModResult (record)

```teal
record MissingModResult
```

A dependency that is not installed or not installed in a matching version.

- `dependent : string`: Name (folder id) of the mod that declares the dependency.
- `reason : Reason`: Why the dependency is missing.
- `name : string`: Name (folder id) of the required mod.
- `loadBefore : boolean`: True when the dependency must be loaded before the dependent mod.
- `optional : boolean`: True when the dependency is optional.
- `displayName : string`: Display name of the required mod as given in the dependency's mod info; may be empty.
- `url : string`: Download URL of the required mod as given in the dependency's mod info; may be empty.

#### ModUtil.MissingModResult.Reason (enum)

```teal
enum Reason
```

Why the dependency counts as missing.

- `"NotFound"`: The required mod is not installed.
- `"IncorrectVersion"`: The required mod is installed, but its revision is outside the range the dependent mod accepts.

### ModUtil.ModOrderResult (record)

```teal
record ModOrderResult
```

An active dependency that must load before the dependent mod but comes after it in the activation order.

- `dependent : string`: Name (folder id) of the mod that declares the dependency.
- `name : string`: Name (folder id) of the dependency that is loaded too late.

### ModUtil.ModIncompatibilityResult (record)

```teal
record ModIncompatibilityResult
```

A pair of active mods where one declares the other incompatible.

- `incompatible : string`: Name (folder id) of the mod that declares the incompatibility.
- `name : string`: Name (folder id) of the active mod it is incompatible with.

### ModUtil.ModNameAndVersion (record)

```teal
record ModNameAndVersion
```

Display name and version of a built-in "mod" (`""` = base content, `"ug_api_scripting"` = scripting API) that mods can reference in dependencies and incompatibilities.

- `name : string`: Translated display name.
- `version : integer`: Version compared against the revision range of a dependency.
