# game_mechanics.subventions.subvention

Definition file: `base/tealdef/game_mechanics/subventions/subvention.d.tl` (in the game folder).

Types of the subsidy system, its saved state and the interface each subsidy type script implements.

## SubventionBonusMalus (record)

```teal
global record SubventionBonusMalus
```

One reward or penalty of a subsidy, applied with `SubventionUtil.applyBonusMalus` on accept, completion or failure.

Notes:

Create it with `SubventionUtil.makeBonusMalusMoney`, `makeBonusMalusReputation` or `makeBonusMalusTownExp`.
`params` has the fields of the record named by `type`.

- `type : Type`: Kind of this entry.
- `params : Param`: Values of this entry; cast to the record named by `type`.

### SubventionBonusMalus.Type (enum)

```teal
enum Type
```

Kind of reward or penalty.

- `"Money"`: Payment or fine in game currency.
  In game: "{amount} Payment", "{amount} Fine" (German: „{amount} Zahlung“, „{amount} Strafe“)
- `"Reputation"`: Change of the player's reputation in a town.
- `"TownExperience"`: Level progress of a town.
  In game: "{amount} Level Progress in {townName}" (German: „{amount} Level-Fortschritt in {townName}“)

### SubventionBonusMalus.Money (record)

```teal
record Money
```

Params of a `Money` entry.

- `amount : integer`: Amount in game currency. Paid when the entry is applied as positive, charged otherwise.

### SubventionBonusMalus.Reputation (record)

```teal
record Reputation
```

Params of a `Reputation` entry.

- `townEntity : EntityUtil.EntityAndRevision`: Town whose reputation changes.
- `amount : number`: Reputation change as a fraction (shown as a percentage).

### SubventionBonusMalus.TownExperience (record)

```teal
record TownExperience
```

Params of a `TownExperience` entry.

- `townEntity : EntityUtil.EntityAndRevision`: Town that gains level progress.
- `amount : number`: Level progress as a fraction (shown as a percentage).

### SubventionBonusMalus.Param (record)

```teal
record Param
```

Union of the params records, the declared type of `params`.

- `amount : integer`: Amount; its meaning depends on `type`.
- `townEntity : EntityUtil.EntityAndRevision`: Town for `Reputation` and `TownExperience` entries.

### SubventionBonusMalus.Data (record)

```teal
record Data
```

Display line for a reward or penalty, as made by `SubventionUtil.getData`.

- `text : string`: Translated text, e.g. "$1,000,000 Payment" or "10% Reputation in Davao".

## ISubvention (interface)

```teal
global interface ISubvention
```

One subsidy instance as stored in `SubventionState`.

Notes:

The subsidy game script moves an instance from `proposedSubventions` to `activeSubventions` on the
`Subvention` event `onAccept`, then to `completedSubventions` when the type's `isComplete` returns
true, or to `failedSubventions` when `shouldDespawn` returns a reason while it is active.

- `data : ISubventionData`: Data of the subsidy; cast it to the type's own record.
- `id : string`: Res name of the subsidy type, e.g. `::/game_mechanics/subventions/deliver_cargo/deliver_cargo.res`.
- `acceptedTime : number`: Game time (ms) when the player accepted the offer; `nil` while proposed.
- `spawnTime : number`: Game time (ms) when the offer was created.
- `completedTime : number`: Game time (ms) when the subsidy was fulfilled; `nil` until then.
- `uid : number`: Unique id of the subsidy within its type. Accept and decline events and notifications refer to it.

### ISubvention.ISubventionData (interface)

```teal
interface ISubventionData
```

Data part of a subsidy. Each subsidy type extends it with its own fields, e.g. the target entity.

Notes:

Start from `SubventionUtil.makeDefaultSubventionTemplate`, which sets `name` and the three durations.

- `upfront : {SubventionBonusMalus}`: Rewards applied when the player accepts the offer.
- `complete : {SubventionBonusMalus}`: Rewards applied when the subsidy is fulfilled.
- `failure : {SubventionBonusMalus}`: Penalties applied when the subsidy fails. The base game types repeat the upfront payment here, so it is paid back.
- `name : string`: Translated title of the subsidy, e.g. "Supply Industry".
- `expireDurationProposed : number`: Time (ms) the offer stays open before it is withdrawn; `-1` means it never expires.
- `expireDuration : number`: Time (ms) after acceptance to fulfil the subsidy.
- `effectDuration : integer`: Time (ms) the reward effect lasts after completion; then the subsidy is removed. The cargo and passenger types show the effect as "2x Income".
- `toDeliver : integer`: Amount the player has to deliver, for the cargo subsidies one unit per delivered cargo item.
- `delivered : integer`: Amount delivered so far.

### ISubvention.ISpecificSubventionParam (interface)

```teal
interface ISpecificSubventionParam
```

Parameters for spawning a given subsidy type through the `Subvention` script event `subventionSpawn`. The type's `spawnSpecific` receives them.

Example:
```lua
-- urbangames_campaign_mission_06
local subvention : DeliverCargoSubventionParam = {
    target = entity,
    expireDurationProposed = -1,
}
api.cmd.sendCommand(api.cmd.makeScriptingSendEventCmd("", "Subvention", "subventionSpawn", {
    name = "::/game_mechanics/subventions/deliver_cargo/deliver_cargo.res",
    specific = subvention,
}))
```

- `expireDurationProposed : number`: -1 means, does not expire
- `uid : number`: Unique id of the new subsidy. When `nil`, the type derives one from its target. `MissionTaskSubsidies` sets it from a counter.

### ISubvention.Status (enum)

```teal
enum Status
```

Stage of a subsidy.

- `"Proposed"`: Offered, not yet accepted.
  In game: "New Subsidy Available" (German: „Neue Subvention verfügbar“)
- `"Active"`: Accepted and running.
  In game: "Subsidy in Progress" (German: „Subvention läuft“)
- `"Complete"`: Fulfilled; the reward effect is running.
  In game: "Subsidy Fulfilled" (German: „Subvention erfüllt“)
- `"Failed"`: Not fulfilled in time.
  In game: "Subsidy Failed" (German: „Subvention gescheitert“)

### ISubvention.FailReason (enum)

```teal
enum FailReason
```

Why a subsidy ended without being fulfilled.

- `"None"`: No failure.
- `"Timeout"`: A deadline passed (see `SubventionUtil.defaultTimeout`).
- `"InvalidEntity"`: A target entity was removed or replaced.
  In game: "Subsidy Removed", "The subsidy offer has been rescinded, as its conditions are no longer met." (German: „Subvention entfernt“, „Das Subventionsangebot wurde zurückgezogen, da seine Bedingungen nicht mehr erfüllt sind.“)

## Subvention (record)

```teal
global record Subvention<T is ISubvention.ISubventionData> is ISubvention
```

Subsidy instance with its data typed as `T`.

- `data : T`: Data of the subsidy.

## SubventionState (record)

```teal
global record SubventionState
```

Saved state of the subsidy game script `::/game_mechanics/subventions/subventions.gs`.

Notes:

Read it with `api.engine.system.gameScriptSystem.getEntityForGameScript("::/game_mechanics/subventions/subventions.gs")`
and the `GAME_SCRIPT` component, as `SubventionUtil.getSubventionAndStatusFromGameScript` does.
A random offer spawns when the spawn interval has passed since `lastSpawnTime` and fewer than 5 subsidies are active.

- `lastSpawnTime : number`: Game time (ms) of the last spawned offer; `-1` before the first one.
- `proposedSubventions : {ISubvention}`: Open offers.
- `activeSubventions : {ISubvention}`: Accepted subsidies that are running.
- `completedSubventions : {ISubvention}`: Fulfilled subsidies whose reward effect is still running.
- `failedSubventions : {ISubvention}`: Subsidies that failed while active.
- `usedUids : {string : {string : boolean}}`: Per subsidy type res name, the uids already spawned (key is the uid as string). `true` while the subsidy exists, `false` after it ended.
- `version : integer`: State version; `SubventionLegacyUtil.updateLegacyState` upgrades states older than `SubventionLegacyUtil.CurrentVersion`.
- `spawnIntervalModifier : number`: 1.0 is "neutral" (doing nothing)
- `pause : boolean`: No subsidies spawn or update while true. Set with the `Subvention` event `setPause` (param is a boolean).

## SubventionIcons (record)

```teal
global record SubventionIcons
```

Icon paths of a subsidy type for each status, read by `SubventionUtil.getIcon`.

- `fallback : string`: Icon used when the status has no icon of its own.
- `proposed : string`: Icon of an open offer.
- `active : string`: Icon of a running subsidy.
- `complete : string`: Icon of a fulfilled subsidy.
- `failed : string`: Icon of a failed or missed subsidy.

## SubventionDesc (record)

```teal
global record SubventionDesc
```

Data of a generic res of type `subvention`, one per subsidy type.

Example:
```lua
-- ::/game_mechanics/subventions/deliver_cargo/deliver_cargo.res.lua
function data()
return {
    type = "subvention",
    data = {
        scriptFile = "::/game_mechanics/subventions/deliver_cargo/deliver_cargo.script@deliver_cargo",
        icons = {
            fallback = "::/game_mechanics/subventions/deliver_cargo/deliver_cargo.tga",
        }
    }
}
end
```

- `scriptFile : string`: Script table of the type (`<script file>@<table name>`) that implements `SubventionScript`. The game script looks up each function as `scriptFile .. ".canSpawn"` and so on.
- `icons : SubventionIcons`: Icons per status.

## SubventionCardData (record)

```teal
global record SubventionCardData
```

Content of a subsidy card and window, returned by the type's `getCardData`.

Notes:

Start from `SubventionUtil.makeDefaultCardData` and add `location`, `progress`, `description` and the cargo fields.

- `title : Entry`: Icon and title of the subsidy.
- `deadline : Entry`: Calendar icon and the formatted time to fulfil the subsidy. Hidden in the window once the deadline has passed.
- `description : string`: Translated description for the current status.
- `expireDuration : Expiration`: Remaining time; set only while the subsidy is active.
- `progress : Progress`: Delivery progress; set by the cargo subsidies while active.
- `location : Location`: Source and target of the subsidy.
- `cargoIcons : {string}`: Icon paths of the cargo types to deliver.
- `cargoToDeliver : integer`: Amount to deliver, shown after the cargo icons.
- `upfront : {SubventionBonusMalus.Data}`: Display lines of the upfront rewards.
- `failure : {SubventionBonusMalus.Data}`: Display lines of the penalties.
- `complete : {SubventionBonusMalus.Data}`: Display lines of the completion rewards.
- `status : ISubvention.Status`: Status the card was made for.
- `layerConfig : LayerConfig`: Map layer shown with the window, e.g. cargo flow arrows from suppliers to the target.

### SubventionCardData.Entry (record)

```teal
record Entry
```

Icon with a text.

- `icon : string`: Icon path.
- `name : string`: Text next to the icon.

### SubventionCardData.Expiration (record)

```teal
record Expiration
```

Remaining time of a running subsidy.

- `value : number`: Remaining share of `expireDuration`, from 1 (just accepted) to 0.
- `name : string`: Remaining time, formatted for the current calendar speed.

### SubventionCardData.Progress (record)

```teal
record Progress
```

Delivery progress.

- `value : number`: Progress from 0 to 1.
- `text : string`: Progress as text, e.g. "12 / 50".

### SubventionCardData.Location (record)

```teal
record Location
```

Source and target shown in the window, each with a locate and a select button.

- `from : {Engine.Entity, string}`: Source entity and its display name.
- `to : {Engine.Entity, string}`: Target entity and its display name.

## SubventionScript (record)

```teal
global record SubventionScript<T>
```

Functions a subsidy type script implements. The subsidy game script calls them through the `scriptFile` of the type's `SubventionDesc`.

Notes:

Most base game types use `SubventionUtil.defaultOnAccept`, `defaultOnFailure` and `defaultOnComplete` directly,
and `SubventionUtil.defaultTimeout` inside `shouldDespawn`.

- `handleLegacy : function(state : Subvention<T>) : Subvention<T>`: Fills fields missing in data from older savegames. The base game types implement it, but the subsidy game script does not call it.
    - param `state`: Subsidy to update.
    - returns: The updated subsidy.
- `canSpawn : function() : boolean`: Whether an offer of this type can be created now (e.g. a suitable industry exists). The game script picks a random type among those that return true.
    - returns: True if `onSpawn` can create an offer.
- `onSpawn : function() : T, number`: Creates the data of a random new offer of this type.
    - returns: The subsidy data (or `nil` if no offer could be made) and its uid.
- `spawnSpecific : function(param : ISubvention.ISpecificSubventionParam) : T, number`: Creates the data of an offer for given parameters. Called for the `Subvention` event `subventionSpawn`.
    - param `param`: Type-specific parameters, e.g. `DeliverCargoSubventionParam`.
    - returns: The subsidy data (or `nil` if the target does not qualify) and its uid.
- `isComplete : function(state : Subvention<T>) : boolean`: Checks whether an active subsidy is fulfilled. Called on every update.
    - param `state`: Active subsidy.
    - returns: True if the subsidy is fulfilled.
- `onComplete : function(state : Subvention<T>)`: Called once when the subsidy is fulfilled, before it moves to the completed list.
    - param `state`: Fulfilled subsidy.
- `onFailure : function(state : Subvention<T>)`: Called once when an active subsidy fails.
    - param `state`: Failed subsidy.
- `onAccept : function(state : Subvention<T>)`: Called once when the player accepts the offer.
    - param `state`: Accepted subsidy.
- `onUpdate : function(state : Subvention<T>)`: Called on every update of the game script for each active subsidy, before the despawn and completion checks.
    - param `state`: Active subsidy.
- `shouldDespawn : function(state : Subvention<T>, currentGameTime : number) : string`: Checks whether a proposed, active or completed subsidy ends. Active ones then fail; proposed ones count as missed; completed ones end their reward effect.
    - param `state`: Subsidy to check.
    - param `currentGameTime`: Current game time (ms).
    - returns: A `ISubvention.FailReason` value, or `nil` to keep the subsidy.
- `onDespawn : function(state : Subvention<T>)`: Called when a proposed or completed subsidy is removed.
    - param `state`: Removed subsidy.
- `eventsToHandle : function() : {string}`: Names of extra script events the subsidy game script subscribes to for this type.
    - returns: Event names, e.g. `{ "OnCalcTicketPrice" }`.
- `handleEvent : function(src : string, id : string, name : string, param : any, state : Subvention<T>) : any`: Handles a script event for one active or completed subsidy.
    - param `src`: Event source.
    - param `id`: Event id.
    - param `name`: Event name.
    - param `param`: Event parameter.
    - param `state`: Active or completed subsidy.
    - returns: Event result. The game script returns the last non-`nil` result, e.g. ticket price factors for `OnCalcTicketPrice`.
- `getNotificationParams : function(state : Subvention<T>) : any`: Data to store in a notification about the subsidy, so the notification can be built later without the subsidy.
    - param `state`: Subsidy.
    - returns: Type-specific table, passed back to `getNotificationData` and `getNotificationMissedData`.
- `getNotificationData : function(subventionId : string, params : any, status : ISubvention.Status, reason : ISubvention.FailReason, entityNames : {Engine.Entity : string}) : NotificationGuiData`: Builds the notification for a status change.
    - param `subventionId`: Res name of the subsidy type.
    - param `params`: Table from `getNotificationParams`.
    - param `status`: Status to report.
    - param `reason`: Fail reason, `"None"` if there is none.
    - param `entityNames`: Display names of the relevant entities.
    - returns: Title, description and icon of the notification.
- `getNotificationMissedData : function(subventionId : string, params : any, reason : ISubvention.FailReason, entityNames : {Engine.Entity : string}) : NotificationGuiData`: Builds the notification for an offer that was withdrawn before the player accepted it.
    - param `subventionId`: Res name of the subsidy type.
    - param `params`: Table from `getNotificationParams`.
    - param `reason`: Why the offer was withdrawn.
    - param `entityNames`: Display names of the relevant entities.
    - returns: Title, description and icon of the notification.
- `getCardData : function(state : Subvention<T>, lightweight? : boolean) : SubventionCardData`: Returns information about the subvention to display icons and cards, lgihtweird only returns icons, descriptions and progress

Builds the card and window content of the subsidy.
    - param `state`: Subsidy.
    - param `lightweight`: When true, returns the default icon, description and progress without resolving detailed entity locations. Omitted or false builds the full card.
    - returns: Card content.
- `getRelevantEntities : function(state : Subvention<T>) : {Engine.Entity}, {CargoTypeId : CargoUtil.StockProperties}`: Entities and cargo types of the subsidy, used for notifications and for the subsidy icons on the map.
    - param `state`: Subsidy.
    - returns: Entities (e.g. the target, and for `deliver_cargo` also the delivering lines and their vehicles) and the cargo types to highlight with their stock properties.
- `getRelevantEntityNames : function(state : Subvention<T>) : {Engine.Entity : string}`: Display names of the relevant entities. Only `deliver_cargo_town` implements it, and the base game does not call it.
    - param `state`: Subsidy.
    - returns: Name per entity.
