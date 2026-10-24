# game_mechanics.subventions.subvention_util

Definition file: `base/tealdef/game_mechanics/subventions/subvention_util.d.tl` (in the game folder).

Helpers shared by the subsidy type scripts (default timeouts, rewards and penalties, card and notification data).

## SubventionUtil (record)

```teal
global record SubventionUtil
```
Load: `local subvention_util = ug_require "::/game_mechanics/subventions/subvention_util.tl" as SubventionUtil`

Helpers for subsidy type scripts and the subsidy game script.

- `getSpawnIntervalDuration : function(modifier : number, companyRank : integer) : integer`  (call as `subvention_util.getSpawnIntervalDuration(…)`): nil, if there is nothing to spawn at all!
- `initStateIfEmpty : function(SubventionState) : SubventionState`  (call as `subvention_util.initStateIfEmpty(…)`): Returns a fresh subsidy state if the given one is empty, otherwise fills missing `spawnIntervalModifier` and `failedSubventions`.
    - param `#1`: State from the game script.
    - returns: The usable state.
- `getDefaultSubventionStatus : function<T>(state : Subvention<T>) : Subvention.Status`  (call as `subvention_util.getDefaultSubventionStatus(…)`): Status from the timestamps. `Active` if accepted and not completed, `Complete` if completed, otherwise `Proposed`; never `Failed`.
    - param `state`: Subsidy.
    - returns: Status of the subsidy.
    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/deliver_cargo/deliver_cargo.script.tl:385`: `local subventionStatus = subvention_util.getDefaultSubventionStatus(state)`
- `getAllCargoRequired : function(Engine.Component.StockList) : {CargoTypeId}`  (call as `subvention_util.getAllCargoRequired(…)`): Cargo types used as input by any rule of a stock list. A type can appear more than once.
    - param `#1`: Stock list component of an industry.
    - returns: Required cargo type ids.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/deliver_cargo/deliver_cargo.script.tl:65`: `local allCargoToDeliver = subvention_util.getAllCargoRequired(stockList)`
- `didStockListProduce : function(Engine.Entity) : boolean`  (call as `subvention_util.didStockListProduce(…)`): Whether any production rule of a stock list has been applied.
    - param `#1`: Stock list entity.
    - returns: True if the stock list produced at least once.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/deliver_cargo/deliver_cargo.script.tl:57`: `local didProduce : boolean = subvention_util.didStockListProduce(industry.stockList)`
- `getAllProducedCargo : function() : {CargoTypeId : integer}`  (call as `subvention_util.getAllProducedCargo(…)`): At any time in the past
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/deliver_cargo/deliver_cargo.script.tl:142`: `local allCargoProduced = subvention_util.getAllProducedCargo()`
- `getSpawnYearScaleFactor : function() : number`  (call as `subvention_util.getSpawnYearScaleFactor(…)`): Factor from 1 to 4 that gives early years longer deadlines. Computed as `ceil((2000 - year) / 100 * 4)` with the year clamped to 1900..1999.
    - returns: Scale factor; `makeDefaultSubventionTemplate` adds 12 months per step to `expireDuration`.
- `defaultTimeout : function<T is ISubvention.ISubventionData>(state : Subvention<T>, currentGameTime : number) : ISubvention.FailReason`  (call as `subvention_util.defaultTimeout(…)`): Returns `"Timeout"` when the deadline for the current stage has passed: `expireDuration` after acceptance,
`effectDuration` after completion, or `expireDurationProposed` after spawning while proposed (unless it is -1).
    - param `state`: Subsidy.
    - param `currentGameTime`: Current game time (ms).
    - returns: `"Timeout"` or `nil`.
    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/deliver_cargo/deliver_cargo.script.tl:213`: `return subvention_util.defaultTimeout(state, currentGameTime)`
- `makeDefaultSubventionTemplate : function(string) : ISubvention.ISubventionData`  (call as `subvention_util.makeDefaultSubventionTemplate(…)`): Creates subsidy data with a name and default durations.
    - param `#1`: Translated title.
    - returns: Data with `expireDurationProposed` of about 6 months (plus or minus 30 days), `expireDuration` of
12 months plus 12 per `getSpawnYearScaleFactor` step, and `effectDuration` of 5 years.

    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/deliver_cargo/deliver_cargo.script.tl:72`: `local state = subvention_util.makeDefaultSubventionTemplate(_("Supply Industry")) as DeliverCargoState`
- `makeDefaultCardData : function<T is ISubvention.ISubventionData>(state : Subvention<T>, icon : string) : SubventionCardData`  (call as `subvention_util.makeDefaultCardData(…)`): Card content with title, deadline, remaining time and the reward and penalty lines for the current stage.
    - param `state`: Subsidy.
    - param `icon`: Title icon.
    - returns: Card content; add `location`, `description` and the type-specific fields.
    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/deliver_cargo/deliver_cargo.script.tl:301`: `local cardData = subvention_util.makeDefaultCardData(state, "::/game_mechanics/subventions/icons/industry.tga")`
- `makeDefaultNotificationData : function(status : ISubvention.Status, failReason : ISubvention.FailReason, icon : string, getDesc : function(ISubvention.Status) : string) : NotificationGuiData`  (call as `subvention_util.makeDefaultNotificationData(…)`): Notification with the standard title for a status ("New Subsidy Available", "Subsidy in Progress", "Subsidy Fulfilled", "Subsidy Failed", or "Subsidy Removed" for `InvalidEntity`).
  In game: "New Subsidy Available", "Subsidy in Progress", "Subsidy Fulfilled" (German: „Neue Subvention verfügbar“, „Subvention läuft“, „Subvention erfüllt“)
    - param `status`: Status to report.
    - param `failReason`: Fail reason.
    - param `icon`: Notification icon.
    - param `getDesc`: Returns the description for a status.
    - returns: Notification content.
    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/deliver_cargo/deliver_cargo.script.tl:245`: `return subvention_util.makeDefaultNotificationData(status, reason, subvention_util.getIcon(subventionId, status),`
- `makeDefaultMissedNotificationData : function(failReason : ISubvention.FailReason, icon : string, getDesc : function(ISubvention.Status) : string) : NotificationGuiData`  (call as `subvention_util.makeDefaultMissedNotificationData(…)`): Notification titled "Subsidy Missed" for a withdrawn offer. For `InvalidEntity` it uses a fixed description instead of `getDesc`.
  In game: "Subsidy Missed", "The subsidy offer has been rescinded, as its conditions are no longer met." (German: „Subvention verpasst“, „Das Subventionsangebot wurde zurückgezogen, da seine Bedingungen nicht mehr erfüllt sind.“)
    - param `failReason`: Why the offer was withdrawn.
    - param `icon`: Notification icon.
    - param `getDesc`: Returns the description.
    - returns: Notification content.
    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/deliver_cargo/deliver_cargo.script.tl:266`: `return subvention_util.makeDefaultMissedNotificationData(reason, subvention_util.getIcon(subventionId, "Failed"),`
- `applyBonusMalus : function(bonusMalus : SubventionBonusMalus, positive : boolean)`  (call as `subvention_util.applyBonusMalus(…)`): Applies one reward or penalty. `Money` books a subsidy journal entry for the player, `Reputation` sends the
`Towns` event `decreaseReputation`, `TownExperience` sends `addExperiencePercentage`.
    - param `bonusMalus`: Entry to apply.
    - param `positive`: True for a reward; false negates the money and reputation amounts.
- `makeBonusMalusMoney : function(amount : integer) : SubventionBonusMalus`  (call as `subvention_util.makeBonusMalusMoney(…)`): Creates a `Money` entry.
    - param `amount`: Amount in game currency, rounded down.
    - returns: The entry.
    - used 9x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/deliver_cargo/deliver_cargo.script.tl:123`: `subvention_util.makeBonusMalusMoney(upfrontPayment)`
- `makeBonusMalusReputation : function(townEntity : EntityUtil.EntityAndRevision, value : number) : SubventionBonusMalus`  (call as `subvention_util.makeBonusMalusReputation(…)`): Creates a `Reputation` entry.
    - param `townEntity`: Town.
    - param `value`: Reputation change as a fraction.
    - returns: The entry.
    - used 7x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/deliver_cargo_town/deliver_cargo_town.script.tl:199`: `subvention_util.makeBonusMalusReputation(state.townEntity, upfrontReputation),`
- `makeBonusMalusTownExp : function(townEntity : EntityUtil.EntityAndRevision, value : number) : SubventionBonusMalus`  (call as `subvention_util.makeBonusMalusTownExp(…)`): Creates a `TownExperience` entry.
    - param `townEntity`: Town.
    - param `value`: Level progress as a fraction.
    - returns: The entry.
    - used 4x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/deliver_cargo_town/deliver_cargo_town.script.tl:216`: `subvention_util.makeBonusMalusTownExp(state.townEntity, math.random(4, 20) * 0.05),`
- `makeBonusMalus : function(t : SubventionBonusMalus.Type, params : SubventionBonusMalus.Param) : SubventionBonusMalus`  (call as `subvention_util.makeBonusMalus(…)`): Creates an entry of any type.
    - param `t`: Type of the entry.
    - param `params`: Values of the entry.
    - returns: The entry.
- `getData : function(upfront : {SubventionBonusMalus}, positive : boolean) : {SubventionBonusMalus.Data}`  (call as `subvention_util.getData(…)`): Display lines for a list of rewards or penalties. Money entries are summed into one line, reputation entries into one line per town.
  In game: "{amount} Payment", "{amount} Reputation in {townName}", "{amount} Level Progress in {townName}" (German: „{amount} Zahlung“, „{amount} Ruf in {townName}“, „{amount} Level-Fortschritt in {townName}“)
    - param `upfront`: Entries to describe.
    - param `positive`: True for rewards ("Payment"), false for penalties ("Fine", reputation shown negated).
    - returns: One display line per merged entry; entries with a zero amount or a changed town are left out.
- `defaultOnAccept : function<T is ISubvention.ISubventionData>(state : Subvention<T>)`  (call as `subvention_util.defaultOnAccept(…)`): Applies all `upfront` entries as rewards.
    - param `state`: Accepted subsidy.
    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/deliver_cargo/deliver_cargo.script.tl:219`: `onAccept = subvention_util.defaultOnAccept,`
- `defaultOnFailure : function<T is ISubvention.ISubventionData>(state : Subvention<T>)`  (call as `subvention_util.defaultOnFailure(…)`): Applies all `failure` entries as penalties.
    - param `state`: Failed subsidy.
    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/deliver_cargo/deliver_cargo.script.tl:221`: `onFailure = subvention_util.defaultOnFailure,`
- `defaultOnComplete : function<T is ISubvention.ISubventionData>(state : Subvention<T>)`  (call as `subvention_util.defaultOnComplete(…)`): Applies all `complete` entries as rewards.
    - param `state`: Fulfilled subsidy.
    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/deliver_cargo/deliver_cargo.script.tl:223`: `onComplete = subvention_util.defaultOnComplete,`
- `defaultUpdateIndustry : function<T is ISubvention.ISubventionData>(state : Subvention<T>, industryEntity : EntityUtil.EntityAndRevision)`  (call as `subvention_util.defaultUpdateIndustry(…)`): While the subsidy is active, cancels a scheduled closure of the industry.
    - param `state`: Subsidy.
    - param `industryEntity`: Target industry; nothing happens if it changed.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/deliver_cargo/deliver_cargo.script.tl:227`: `subvention_util.defaultUpdateIndustry(state, data.stockListEntity)`
- `getDefaultExpectedTimeInMonths : function(distance : number) : integer`  (call as `subvention_util.getDefaultExpectedTimeInMonths(…)`): Game months a vehicle at the current top rail speed needs for a distance, rounded up. The base game types scale it into extra deadline time.
    - param `distance`: Distance in metres.
    - returns: Number of game months.
    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/deliver_cargo/deliver_cargo.script.tl:116`: `local extraTime = subvention_util.getDefaultExpectedTimeInMonths(minDist) * 32`
- `getSubventionAndStatusFromGameScript : function(scriptEntity : Engine.Entity, subventionUID : number) : ISubvention, integer`  (call as `subvention_util.getSubventionAndStatusFromGameScript(…)`): Finds a subsidy by uid in the state of the subsidy game script.
    - param `scriptEntity`: Entity of the game script `::/game_mechanics/subventions/subventions.gs`.
    - param `subventionUID`: Uid to look for.
    - returns: The subsidy and its list index (1 proposed, 2 active, 3 completed, 4 failed), or `nil, nil`.
    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notification_legacy_util.tl:113`: `local subvention, status = subvention_util.getSubventionAndStatusFromGameScript(scriptEntity, params.uid)`
- `getStatusEnumFromStatusIndex : function(status : integer) : ISubvention.Status`  (call as `subvention_util.getStatusEnumFromStatusIndex(…)`): Converts a list index from `getSubventionAndStatusFromGameScript` to a status.
    - param `status`: Index 1 to 4.
    - returns: The status; `Proposed` (with an error logged) for other values.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/types/subvention_notification.script.tl:36`: `local notificationData = scriptGetNotificationData.getNotificationData(notificationParams.id, notificationParams.params, subvention_util.getStatusEnumFromStatus`
- `getIcon : function(id : string, status : ISubvention.Status) : string`  (call as `subvention_util.getIcon(…)`): Icon of a subsidy type for a status, from the type's res file. Falls back to `icons.fallback`.
    - param `id`: Res name of the subsidy type.
    - param `status`: Status.
    - returns: Icon path, or `nil` if the res does not exist.
    - used 10x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/deliver_cargo/deliver_cargo.script.tl:245`: `return subvention_util.makeDefaultNotificationData(status, reason, subvention_util.getIcon(subventionId, status),`
- `isLineADirectConnectionLine : function(lineEntity : Engine.Entity, fromTown : Engine.Entity, toTown : Engine.Entity, passengerOnly? : boolean) : boolean`  (call as `subvention_util.isLineADirectConnectionLine(…)`): Whether a line stops in both towns.
    - param `lineEntity`: Line.
    - param `fromTown`: First town.
    - param `toTown`: Second town.
    - param `passengerOnly`: If true, only stops that load passengers count.
    - returns: True if the line connects both towns.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/deliver_passengers/deliver_passengers.script.tl:275`: `if subvention_util.isLineADirectConnectionLine(lineEntity, fromTown, toTown) then`
- `areTownsConnected : function(fromTown : Engine.Entity, toTown : Engine.Entity, passengerOnly? : boolean) : Engine.Entity`  (call as `subvention_util.areTownsConnected(…)`): Finds a line that stops in both towns.
    - param `fromTown`: First town.
    - param `toTown`: Second town.
    - param `passengerOnly`: If true, only stops that load passengers count.
    - returns: The first connecting line, or `nil`.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/deliver_passengers/deliver_passengers.script.tl:72`: `local lineEntity = subvention_util.areTownsConnected(fromTownEntity, toTownEntity, true)`
