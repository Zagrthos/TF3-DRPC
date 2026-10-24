# game_mechanics.company.company

Definition file: `base/tealdef/game_mechanics/company/company.d.tl` (in the game folder).

State of the company game script (`::/game_mechanics/company/company.gs`) and the params of its notifications.

## CompanyState (record)

```teal
global record CompanyState
```

Saved state of one company (player) in the company game script.

- `companyEntity : EntityUtil.EntityAndRevision`: Player entity of the company.
- `consumedPermits : {ResName : {integer}}`: for each permit, list of timestamps when it was consumed
- `pendingProspections : {SpawnIndustry}`: Running prospections.

### CompanyState.SpawnIndustry (record)

```teal
record SpawnIndustry
```

A running prospection, which tries to spawn an industry near a town.

- `townEntity : EntityUtil.EntityAndRevision`: Town the prospection targets.
- `types : {string}`: Industry types that may be spawned; the order is shuffled per attempt.
- `permitKey : ResName`: Permit used; it is returned if the prospection fails.
- `cargoType : ResName`: Cargo type of the prospection.
- `initiatedTimestamp : integer`: Game time (ms) at which the prospection started.
- `attempts : integer`: Spawn attempts made so far (one per game month).

## CompaniesState (record)

```teal
global record CompaniesState
```

State of the company game script.

- `companyStates : {CompanyState}`: State per company.
- `basePopulation : integer`: World population when the game was started; the experience needed per rank depends on it. It does not change during play.
- `version : integer`: Layout version of this state (see `CompanyLegacyUtil`).

## CompanyRankUpNotificationParams (record)

```teal
global record CompanyRankUpNotificationParams
```

Params of the rank-up notification (`company_notification_rank_up.script`).

In game: "Time To Celebrate", "Hard work and endurance are paying off. You got promoted to {role} and great rewards have been unlocked." (German: „Zeit zum Feiern!“, „Harte Arbeit und Ausdauer zahlen sich aus mit einer Beförderung zu {role} und tollen Belohnungen.“)

- `companyEntity : Engine.Entity`: Company that reached the rank.
- `rank : integer`: New rank.

## CompanyProspectionNotificationParams (record)

```teal
global record CompanyProspectionNotificationParams
```

Params of the prospection notification (`company_notification_prospection.script`).

In game: "Prospecting Started", "Prospecting Successful", "Prospecting Failed" (German: „Prospektion gestartet“, „Prospektion erfolgreich!“, „Prospektion fehlgeschlagen“)

- `companyEntity : Engine.Entity`: Company.
- `townEntity : EntityUtil.EntityAndRevision`: Target town.
- `cargoType : ResName`: Cargo type of the prospection.
- `start : boolean`: The notification reports the start of the prospection.
- `resultEntity : EntityUtil.EntityAndRevision`: nil if failed
- `initiatedTimestamp : integer`: Game time (ms) at which the prospection started.

## CompanyProspectionNotificationSimParams (record)

```teal
global record CompanyProspectionNotificationSimParams
```

Data the notification resolves on the simulation side.

- `townEntityName : string`: Name of the town.

## CompanyMarketingNotificationParams (record)

```teal
global record CompanyMarketingNotificationParams
```

Params of the marketing-campaign notification (`company_notification_marketing.script`).

In game: "Marketing Campaign Started", "Marketing Campaign Ended", "A sophisticated marketing campaign has been launched in {townName}. While the campaign is running, residents are subtly enticed to use public transportation more often." (German: „Marketingkampagne gestartet“, „Marketingkampagne beendet“, „In {townName} wurde eine ausgeklügelte Marketingkampagne gestartet. Während die Kampagne läuft, werden die Einwohner auf subtile Weise dazu gebracht, häufiger öffentliche Verkehrsmittel zu benutzen.“)

- `companyEntity : Engine.Entity`: Company.
- `townEntity : EntityUtil.EntityAndRevision`: Town of the campaign.
- `initiatedTimestamp : integer`: Game time (ms) at which the campaign started.
- `totalDurationMs : integer`: Length of the campaign in game time (ms).
- `start : boolean`: The notification reports the start of the campaign.

## CompanyMarketingNotificationSimParams (record)

```teal
global record CompanyMarketingNotificationSimParams
```

Data the notification resolves on the simulation side.

- `townEntityName : string`: Name of the town.

## CompanyGreenifyNotificationParams (record)

```teal
global record CompanyGreenifyNotificationParams
```

Params of the industry-greenify notification (`company_notification_greenify.script`).

In game: "Improved Ecological Footprint", "Using the latest scientific methods, the experts have improved the ecological footprint of {constructionName}, resulting in fewer emissions." (German: „Verbesserter Ökologischer Fussabdruck“, „Mit Hilfe modernster wissenschaftlicher Methoden haben die Experten den ökologischen Fussabdruck von {constructionName} und damit die Emissionen reduziert“)

- `companyEntity : Engine.Entity`: Company.
- `constructionEntity : EntityUtil.EntityAndRevision`: Industry construction that was changed.
- `ecoLevel : integer`: New eco level of the industry.

## CompanyGreenifyNotificationSimParams (record)

```teal
global record CompanyGreenifyNotificationSimParams
```

Data the notification resolves on the simulation side.

- `constructionEntityName : string`: Name of the construction.

## CompanyLegacyUtil (record)

```teal
global record CompanyLegacyUtil
```

Upgrades a company state saved by an older game version.

- `currentVersion : integer`: Current layout version of the company state (4 in the base game).
- `updateLegacyState : function(companiesState : CompaniesState)`: Upgrades the state in place to `currentVersion`.
    - param `companiesState`: State to upgrade.

## CompanyToolParam (record)

```teal
global record CompanyToolParam is ToolReactUtil.IToolParam
```

Params of the company window.

- `initialTabKey : TabKey`: Tab shown when the window opens.
- `unlockRank : integer`: Rank to show; defaults to the rank the company has reached.
- `allCompanyDefinitionsCacheRef : ReactRefT<{ConstructionDefinition}>`: Cached list of all construction definitions with company metadata.

### CompanyToolParam.TabKey (enum)

```teal
enum TabKey
```

Tab of the company window.

- `"Company"`: Ranks and perks.
- `"Achievements"`: Achievements.

## Exploration (record)

```teal
global record Exploration
```

Definition of a prospection (generic resource of type `company_exploration`, e.g. `explorations/exploration_clay.res.lua`).

Example:
```lua
function data()
return {
    type = "company_exploration",
    data = {
        cargoType = "::/cargos/clay/clay.cargo",
        icon = "game_mechanics/company/explorations/exploration_clay.tga",
        lockedIcon = "game_mechanics/company/explorations/exploration_clay_locked.tga",
        rankAndPermits = { {3, 1} },
        requiredTags = { "OUTPUT_CLAY" },
        menuCategory = { categories = { { category = "prospections", order = 2000 } } },
    }
}
end
```

- `permitKey : ResName`: Permit key shared with other constructions or prospections.
- `rankAndPermits : {{integer, integer}}`: {{rank, times}}
- `requiredTags : {string}`: Industry tags (e.g. `OUTPUT_CLAY`) that select the industry types to spawn.
- `cargoType : ResName`: Cargo type the prospection looks for.
- `icon : ResName`: Icon of the prospection.
- `lockedIcon : ResName`: Icon shown while the prospection is locked.
- `previewIcon : ResName`: Preview icon.
- `menuCategory : ModelMetadata.MenuCategory`: Menu category of the prospection in the construction menu.
