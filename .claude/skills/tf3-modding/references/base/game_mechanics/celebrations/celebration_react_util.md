# game_mechanics.celebrations.celebration_react_util

Definition file: `base/tealdef/game_mechanics/celebrations/celebration_react_util.d.tl` (in the game folder).

Celebration types, the GUI event that shows a celebration and the recipe that displays them.

## CelebrationType (enum)

```teal
global enum CelebrationType
```

Kind of celebration callout. The celebrations game script tracks these firsts and level-ups.

- `"ProductDeliveredToTown"`: First delivery of products to a town.
  In game: "Congratulations, you have just started the delivery of products requested by {townName}!" (German: „Glückwunsch, du hast soeben mit der Lieferung der von {townName} angeforderten Produkte begonnen!“)
- `"CargoDeliveredToIndustry"`: First delivery of cargo to an industry.
  In game: "Congratulations, you have just started delivering cargo requested by {industryName}!" (German: „Glückwunsch, du hast soeben begonnen, die von {industryName} angeforderten Güter zu liefern!“)
- `"PassengerTransportedInTown"`: First passenger boarding a line in a town.
  In game: "Congratulations, the first passenger just boarded a transit vehicle in {townName}!" (German: „Glückwunsch, soeben hat der erste Passagier ein öffentliches Verkehrsmittel in {townName} bestiegen!“)
- `"TownLevelUp"`: A town reached a new level.
  In game: "Congratulations, thanks to the excellent care, {townName} was able to grow steadily and reached a new level: {townType}." (German: „Glückwunsch, dank der ausgezeichneten Versorgung konnte {townName} stetig wachsen und erreichte einen neue Stadtstufe: {townType}.“)
- `"CompanyLevelUp"`: The player's company reached a new rank.
  In game: "Congratulations, your hard work and endurance are paying off. You got promoted to {role} and great rewards have been unlocked." (German: „Glückwunsch, deine harte Arbeit und Ausdauer zahlen sich aus, mit einer Beförderung zu {role} und tollen Belohnungen.“)

## CelebrationAddedEventParams (record)

```teal
global record CelebrationAddedEventParams
```

Param of the GUI React event `celebration-add`, fired by the celebrations game script (`guiUpdate`). `CelebrationsContainer` queues and shows it.

Example:
```lua
-- base game, celebrations/celebrations.script.tl
api.gui.fireReactEvent("celebration-add", { type = "TownLevelUp", entity = entityAndRev, value = v.counter, reputationEffect = v.reputationEffect })
```

- `type : CelebrationType`: Kind of celebration.
- `entity : EntityUtil.EntityAndRevision`: The town or industry. Not set for `CompanyLevelUp`.
- `value : integer`: For `TownLevelUp` the new town level, for `CompanyLevelUp` the new company rank, otherwise the count of deliveries or boardings so far.
- `reputationEffect : boolean`: The event also raised a town's reputation; the text then says so.
- `reputationEffectEntity : EntityUtil.EntityAndRevision`: Town whose reputation was raised, if it is not `entity` (for `CargoDeliveredToIndustry`, the town closest to the industry).

## CelebrationsContainerParams (record)

```teal
global record CelebrationsContainerParams
```

Params of `CelebrationsContainer`.

- `gameCtx : GameContext`: Game GUI context; its callout container shows the celebrations.

## CelebrationReactUtil (record)

```teal
global record CelebrationReactUtil
```
Load: `local celebration_react_util = ug_require "::/game_mechanics/celebrations/celebration_react_util.tl" as CelebrationReactUtil`

GUI recipe for celebration callouts.

- `CelebrationsContainer : Recipe<CelebrationsContainerParams>`: Collects `celebration-add` events and shows them one at a time as callouts at the top of the screen, with the `CelebrationSFX.Earned` sound.
  In game: "Time to Celebrate!", "Reputation improved in {townName}." (German: „Zeit zum Feiern!“, „Ruf in {townName} verbessert.“)
    - notes: Does nothing unless the app config has `enableCelebrations`. Celebrations whose entities have changed are skipped. The base game places it once in `gui/main/game.tl`.

    ```lua
    -- base game, gui/main/game.tl
    item = celebration_react_util.CelebrationsContainer{ gameCtx = gameCtx },
    ```

    - used 1x in the base game, e.g. `base/content/gui/gui/main/game.tl:595`: `item = celebration_react_util.CelebrationsContainer{ gameCtx = gameCtx },`
