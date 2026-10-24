# api.type.mod

Definition file: `api/tealdef/api/type/mod.d.tl` (in the game folder).

Contains all type definitions for the mods.

## Globals

- `ModId : integer`: Global alias of `integer`. It is the type of `SaveGameInfo.modId`; mod identifiers used by the mod repository are `Mod.ModId` records instead.

## Mod (record)

```teal
global record Mod
```
Access: `api.type.mod`

Refer to https://wiki.transportfever3.com/doku.php?id=modding:moddefinition for more details about mod and mod interface in general

Forward declaration of the `Mod` record, which is defined in `api/type/mod.d.tl`.

### Mod.ModId (record)

```teal
record ModId
```
Access: `api.type.ModId`

Mod identifier

- `new : function() : ModId`  (call as `api.type.ModId.new(…)`): Creates a new, empty `ModId`.
    - used 12x in the base game, e.g. `base/content/gui/gui/main/script_param_util.tl:666`: `local modId = api.type.mod.ModId.new()`
- `equals : function(ModId, ModId) : boolean`  (call as `api.type.ModId.equals(…)`): Returns `true` when both `ModId` values are equal.
- `name : string`: A string that identify the mod

### Mod.ModRef (record)

```teal
record ModRef
```
Access: `api.type.mod.ModRef`

A reference to a mod

- `modId : ModId`: Mod id
- `revisionMin : integer`: The minimum compatible revision
- `revisionMax : integer`: The maximum compatible revision

### Mod.ModRefInfo (record)

```teal
record ModRefInfo
```
Access: `api.type.mod.ModRefInfo`

User facing mod informations

- `displayName : string`: Display name of the mod
- `url : string`: URL to the mod page

### Mod.ModDependency (record)

```teal
record ModDependency
```
Access: `api.type.mod.ModDependency`

Describes a dependency to another mod

- `mod : ModRef`: Mod Ref of the depedency
- `optional : boolean`: If the depedency is optional
- `loadBefore : boolean`: If this depedency needs to be loaded before the dependant
- `modInfo : ModRefInfo`: Mod info for the depedency

### Mod.ModIncompatibility (record)

```teal
record ModIncompatibility
```
Access: `api.type.mod.ModIncompatibility`

Describes an incompatibility with another mod

- `mod : ModRef`: Reference (id and revision range) to the mod this mod is incompatible with.

### Mod.GameModDesc (record)

```teal
record GameModDesc
```
Access: `api.type.mod.GameModDesc`

Contains general information about the mod

- `modId : ModId`: Mod id
- `modSource : string`: The origin of the mod (eg. User Mod, Staging Area, DLC, mod.io, ...)
- `modhubModId : string`: Mod id in the modhub backend
- `name : string`: Display name of the mod (User facing)
- `severityRemove : integer`: If removing this mod could cause instability to a savegame.
Any value above 0 will trigger a message when trying to remove the mod from a savegame
0: None, 1: Warning, 2:Critical

### Mod.ModDesc (record)

```teal
record ModDesc
```
Access: `api.type.ModDesc`

Settings of a mod as declared in its `mod.json` (visibility, cosmetic flag, dependencies, script parameters, revision). Read it with `modRep:getModDesc(modId)`.

Example:
```lua
local modRep = app.getUserProfile():getModRep()
for _, modId in ipairs(modRep:getInstalledMods()) do
  local modDesc = modRep:getModDesc(modId)
  if modDesc.visible then print(modId.name) end
end
```

- `severityAdd : integer`: If adding this mod could cause instability to a savegame.
Any value above 0 will trigger a message when trying to add the mod to a savegame
0: None, 1: Warning, 2:Critical
- `severityRemove : integer`: If removing this mod could cause instability to a savegame.
Any value above 0 will trigger a message when trying to remove the mod from a savegame
0: None, 1: Warning, 2:Critical
- `params : {ScriptParam}`: Script parameters to dynamically configure the mod
- `visible : boolean`: If if the mod is visible, hidden mod cannot auto-activate
- `cosmetic : boolean`: If the mod is purely cosmetic, cosmetic mods still allow achievement from being earned while playing
- `autoActivate : boolean`: If the mod is automatically activate by the game
- `url : string`: URL to the mod page
- `dependencies : {ModDependency}`: Mods this mod depends on
- `incompatibilities : {ModIncompatibility}`: Mods this mod is incompatible with
- `revision : integer`: version of the mod

### Mod.ModSize (record)

```teal
record ModSize
```
Access: `api.type.mod.ModSize`

Information about the size of the mod

- `numModels : integer`: The number of models contained in the mod
- `numTextures : integer`: The number of texture contained in the mod
- `numMeshes : integer`: The number of meshes contained in the mod
- `totalEstimatedSizeMB : integer`: The approximate size of the mod
