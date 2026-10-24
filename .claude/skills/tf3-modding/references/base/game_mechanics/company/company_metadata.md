# game_mechanics.company.company_metadata

Definition file: `base/tealdef/game_mechanics/company/company_metadata.d.tl` (in the game folder).

Company metadata of constructions (`metadata.company`), which ties perks, landmarks and the headquarters to company ranks and permits.

## Globals

- `ConstructionDescReactUtil` (type): Helper module for the info panel of construction menu entries. Load it with `ug_require "::/gui/construction/construction_desc_react_util.tl"`.

## CompanyMetadata (record)

```teal
global record CompanyMetadata
```
Load: `local company_metadata = ug_require "::/game_mechanics/company/company_metadata.tl" as CompanyMetadata`

Accessors for the `company` metadata of construction resources, construction instances and savegames.

Example:
```lua
-- base game: landmarks/statue_of_liberty/statue_of_liberty/statue_of_liberty.con.lua
constr.metadata = {
    company = {
        perkCategory = "LANDMARK",
        rankAndPermits = {{11, 1}},
    },
}
```

- `constructionDesc : MetadataAccessor<ConstructionDescMetadata, CompanyMetadata.ConstructionDesc>`: Accessor for `ConstructionDesc`.
    - used 26x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company_react_util.tl:11`: `local companyMeta = company_metadata.constructionDesc.get(perk.metadata)`
- `constructionInstance : MetadataAccessor<ConstructionInstanceMetadata, CompanyMetadata.ConstructionInstance>`: Accessor for `ConstructionInstance`.
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company_react_util.tl:197`: `local companyMeta = company_metadata.constructionInstance.get(construction.persistentMetadata)`
- `savegameScript : MetadataAccessor<SavegameScriptMetadata, CompanyMetadata.Savegame>`: Accessor for `Savegame`.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company_growth.script.tl:139`: `return metadata_util.savegameScript.make({company_metadata.savegameScript.make(metadata)})`
- `getKey : function() : string`  (call as `company_metadata.getKey(…)`): Returns the metadata key.
    - returns: `"company"`.
    - used 4x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company_react_util.tl:204`: `api.engine.system.streetConnectorSystem.forEachConstructionWithMetadata(company_metadata.getKey(), false, true, checkFn)`

### CompanyMetadata.ConstructionDesc (record)

```teal
record ConstructionDesc
```

The `metadata.company` table of a construction resource.

- `companyRank : integer`: Rank at which the construction unlocks; takes precedence over `rankAndPermits`.
- `headquarters : boolean`: Marks the company headquarters.
- `maxCompanyRank : integer`: Rank above which the construction is obsolete.
- `permitKey : ResName`: Shared permit key (a `company_permit_key` resource); without it the construction file is the key.
- `rankAndPermits : {{integer, integer}}`: {{rank, times}}
- `perkCategory : PerkCategory`: Kind of perk.
- `cargoTypes : {ResName}`: Cargo types of the perk.
- `numbers : ConstructionDescReactUtil.Numbers`: Figures shown in the construction description.
- `customLandmarkTab : CustomLandmarkTab`: Custom landmark tab.

#### CompanyMetadata.ConstructionDesc.PerkCategory (enum)

```teal
enum PerkCategory
```

Kind of company perk.

- `"HQ"`: Headquarters.
  In game: "Headquarters Upgrades" (German: „Hauptquartier-Erweiterungen“)
- `"LANDMARK"`: Landmark.
  In game: "Landmarks", "Unique Use" (German: „Wahrzeichen“, „Einzigartige Nutzung“)
- `"MECHANICS"`: Game-mechanics perk (e.g. pollution cleanup facility, marketing campaign, industry greenify).
  In game: "Mechanics" (German: „Mechaniken“)
- `"PROSPECTION"`: Prospection.
  In game: "Prospecting", "Cooldown: {cooldown}" (German: „Prospektion“, „Abklingzeit: {cooldown}“)

#### CompanyMetadata.ConstructionDesc.CustomLandmarkTab (record)

```teal
record CustomLandmarkTab
```

Custom tab in which a landmark is listed.

- `name : string`: Tab name.
- `description : string`: Tab description.
- `sortKey : integer`: Sort order of the tab.

### CompanyMetadata.ConstructionInstance (record)

```teal
record ConstructionInstance
```

Company metadata stored on a built construction.

- `permitKey : ResName`: Permit key the construction was built with.

### CompanyMetadata.Savegame (record)

```teal
record Savegame
```

Company metadata stored in the savegame metadata.

- `level : integer`: Company rank of the player.
