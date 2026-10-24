# game_mechanics.notifications.types.notification_types

Definition file: `base/tealdef/game_mechanics/notifications/types/notification_types.d.tl` (in the game folder).

Params and sim params of the base game's notification types in `game_mechanics/notifications/types/`.

## Globals

- `ISubvention` (type): One subsidy instance as stored in `SubventionState`.
    - notes: The subsidy game script moves an instance from `proposedSubventions` to `activeSubventions` on the `Subvention` event `onAccept`, then to `completedSubventions` when the type's `isComplete` returns true, or to `failedSubventions` when `shouldDespawn` returns a reason while it is active.

## AnimalDespawnNotificationParams (record)

```teal
global record AnimalDespawnNotificationParams
```

Params of the `animaldespawn` notification ("A Grieving Day", category `Info`).

In game: "A Grieving Day", "{animalName} became a victim of technological advance. It is now on its way to heaven." (German: „Ein trauriger Tag“, „{animalName} wurde ein Opfer des technologischen Fortschritts. Es ist nun auf dem Weg in den Himmel.“)

- `entity : EntityUtil.EntityAndRevision`: The animal; clicking the notification focuses it if it still exists.
- `animalName : string`: Name of the animal, used in the text.

## AvailabilityNotificationParams (record)

```teal
global record AvailabilityNotificationParams
```

Params of the `availability` notification about newly available vehicles (category `Info`), sent by `availability_notifications.script.tl`.

In game: "New Vehicle Available", "{name} has hit the market and is now available for purchase." (German: „Neues Fahrzeug verfügbar“, „{name} ist auf dem Markt und jetzt zum Kauf verfügbar.“)

- `models : {string}`: Resource names of the new vehicle models.
- `multipleUnits : {string}`: Resource names of the new multiple units.

## ConAvailabilityNotificationParams (record)

```teal
global record ConAvailabilityNotificationParams
```

Params of the `con_availability` notification about newly available constructions, streets and tracks (category `Info`), sent by `availability_notifications.script.tl`.

In game: "New Construction Available", "{name} is now available." (German: „Neue Konstruktion verfügbar“, „{name} ist jetzt verfügbar.“)

- `resName : ResName`: Resource name; clicking the notification opens the construction menu at it.
- `conRepType : ConRepType`: Repository of `resName`.
- `image : string`: Preview image, the resource's description icon.
- `name : string`: Display name.
- `availabilityGroup : string`: The resource's `availability.notificationGroup`. Resources of the same group become one notification; `""` means no group.
- `availabilitySortKey : integer`: The resource's `availability.notificationSortKey`; orders the variations of a group.
- `categories : {string}`: Menu categories of the resource, e.g. `tracks` or `street_constructions`; they pick the icon and text.
- `variations : {string}`: Names of all resources in the group; the text lists up to 6.
- `special : SpecialNotification`: Set for a new feature (tram catenary, traffic lights) instead of a resource.

### ConAvailabilityNotificationParams.ConRepType (enum)

```teal
enum ConRepType
```

Repository that `resName` belongs to.

- `"ConstructionRep"`: A construction (`api.res.constructionRep`).
  In game: "Helicopters Available", "Road Constructions Available" (German: „Helikopter verfügbar“, „Strassenkonstruktionen verfügbar“)
- `"MetaConstructionRep"`: A meta construction (`api.res.metaConstructionRep`).
  In game: "First Airplanes Available", "Large Airplanes and Airport Available" (German: „Erste Flugzeuge verfügbar“, „Grosse Flugzeuge und Flughafen verfügbar“)
- `"StreetTemplateRep"`: A street or track template (`api.res.streetTemplateRep`).
  In game: "Electric Trains Available", "Highways Available", "New Track Type" (German: „Elektrische Züge verfügbar“, „Autobahnen verfügbar“, „Neuer Gleistyp“)

### ConAvailabilityNotificationParams.SpecialNotification (enum)

```teal
enum SpecialNotification
```

Newly available features that are not a resource.

- `"TramCatenary"`: Tram track catenary became available.
  In game: "Electric Trams Available", "The era of electric trams has arrived and the first vehicles are now available for purchase. New tram tracks will automatically include catenaries, while existing infrastructure can be easily modernized using the tram track tool." (German: „Elektrische Trams verfügbar“, „Das Zeitalter der elektrischen Tram ist angebrochen. Die ersten dieser Fahrzeuge sind nun zum Kauf erhältlich. Neu gelegte Tramgleise sind ab jetzt automatisch mit Oberleitungen ausgestattet, während bestehende Infrastruktur umstandslos mit dem Tramgleis-Werkzeug modernisiert werden kann.“)
- `"TrafficLight"`: Traffic lights became available.
  In game: "Traffic Lights Available", "Crossings can now be equipped with traffic lights. These improve traffic flow, as their phases can be configured according to specific needs." (German: „Verkehrsampeln verfügbar“, „Kreuzungen können jetzt mit Verkehrsampeln ausgestattet werden. Diese lassen sich entsprechend der jeweiligen Anforderungen konfigurieren und verbessern so den Verkehrsfluss.“)

## CargoRtcWarningNotificationParams (record)

```teal
global record CargoRtcWarningNotificationParams
```

Params of the persistent `cargo_rtc_warning` notification ("Cargo Unload Problem", category `Caution`) for vehicles that carry cargo around their line without unloading it.

In game: "Cargo Unload Problem", "{cargoCount} cargo of {vehicle} was not unloaded on line." (German: „Güter-Entladeproblem“, „{cargoCount} Güter von {vehicle} wurden auf der Linie nicht entladen.“)

- `entities : {EntityUtil.EntityAndRevision}`: vehicle enitities (all the ones with a roundtrip)
- `param : integer`: always 1 to not produce multiple warnings

## CargoRtcWarningNotificationSimParams (record)

```teal
global record CargoRtcWarningNotificationSimParams
```

Sim params of `cargo_rtc_warning`.

- `mapping : {Engine.Entity : {string, integer}}`: Per vehicle, `{ name, cargo count }`. The name is refreshed on each update; the count is taken from `getVehiclesWithRoundTripCount()` once.

## CargoThrownAwayNotificationParams (record)

```teal
global record CargoThrownAwayNotificationParams
```

Params for a cargo-thrown-away notification. No notification type in the base game uses it.

- `entities : {EntityUtil.EntityAndRevision}`
- `param : integer`: always 1 to not produce multiple warnings

## CargoThrownAwayNotificationSimParams (record)

```teal
global record CargoThrownAwayNotificationSimParams
```

Sim params for a cargo-thrown-away notification. No notification type in the base game uses it.

- `cargo : integer`

## IndustryNotificationParams (record)

```teal
global record IndustryNotificationParams
```

Params of the `industry_spawn` ("New Industry Founded", `Info`) and `industry_close` (`Caution`, persistent) notifications.

In game: "Industry Closing", "New Industry Founded", "{industry} is complaining about insufficient workload and is urgently looking for a logistics partner who can satisfy its demands. Otherwise the industry will soon close forever." (German: „Industrieschliessung“, „Neue Industrie gegründet“, „{industry} beklagt eine unzureichende Auslastung und sucht dringend einen Logistikpartner, der ihre Bedürfnisse erfüllen kann. Andernfalls wird die Industrie bald für immer schliessen.“)

Notes:

The definition file declares this record twice. The second declaration has
`entity : Engine.Entity` and `revision0`; the type scripts use the first one
(`entity : EntityUtil.EntityAndRevision`).

- `entity : EntityUtil.EntityAndRevision`: The industry.
- `resName : string`: For `industry_close`, the resource name that `api.engine.util.industry.getClosingIndustries()` returns with the industry.

## IndustryNotificationSimParams (record)

```teal
global record IndustryNotificationSimParams
```

Sim params of the industry notifications.

- `mapping : {Engine.Entity : string}`: Name of the industry entity, kept so the text works after the industry is gone.

## IndustryNotificationParams (record)

```teal
global record IndustryNotificationParams
```

Params of the `industry_spawn` ("New Industry Founded", `Info`) and `industry_close` (`Caution`, persistent) notifications.

In game: "Industry Closing", "New Industry Founded", "{industry} is complaining about insufficient workload and is urgently looking for a logistics partner who can satisfy its demands. Otherwise the industry will soon close forever." (German: „Industrieschliessung“, „Neue Industrie gegründet“, „{industry} beklagt eine unzureichende Auslastung und sucht dringend einen Logistikpartner, der ihre Bedürfnisse erfüllen kann. Andernfalls wird die Industrie bald für immer schliessen.“)

Notes:

The definition file declares this record twice. The second declaration has
`entity : Engine.Entity` and `revision0`; the type scripts use the first one
(`entity : EntityUtil.EntityAndRevision`).

- `entity : Engine.Entity`: The industry.
- `revision0 : integer`: First revision number of the industry entity.
- `resName : string`: For `industry_close`, the resource name that `api.engine.util.industry.getClosingIndustries()` returns with the industry.

## IndustryNotificationSimParams (record)

```teal
global record IndustryNotificationSimParams
```

Sim params of the industry notifications.

- `mapping : {Engine.Entity : string}`: Name of the industry entity, kept so the text works after the industry is gone.

## LineStationProblemNotificationParams (record)

```teal
global record LineStationProblemNotificationParams
```

Params of the persistent `line_station_warning` notification ("Station Issue", category `Problem`), created from `api.engine.util.line.getLineStationProblems()`.

In game: "Station Problem", "Stop Not Compatible With Current Vehicles", "Passenger Stop Incompatible With Cargo Vehicles" (German: „Stationsproblem“, „Haltestelle nicht mit den aktuellen Fahrzeugen kompatibel“, „Passagierhaltestelle nicht mit Güterfahrzeugen kompatibel“)

- `entities : {EntityUtil.EntityAndRevision}`: 2: line entity, station group entity
- `param : integer`: LineStationProblem

## LineStationProblemNotificationSimParams (record)

```teal
global record LineStationProblemNotificationSimParams
```

Sim params of `line_station_warning`.

- `lineName : string`: Name of the line.
- `stationName : string`: Name of the station group.

## LineProblemNotificationParams (record)

```teal
global record LineProblemNotificationParams
```

Params of the persistent `line_warning` notification ("Line Problem", category `Problem`), created from `api.engine.util.line.getLineProblems()` and `getLinesIssues()`.

In game: "Line Problem", "Line Contains Too Few Stations", "Could Not Connect Stations" (German: „Linienproblem“, „Linie enthält zu wenig Stationen“, „Stationen konnten nicht verbunden werden“)

- `entities : {EntityUtil.EntityAndRevision}`: should be exactly one line entity
- `param : LineProblemParams`: What is wrong with the line.

### LineProblemNotificationParams.LineProblemParams (record)

```teal
record LineProblemParams
```

What is wrong with the line.

- `lineProblem : integer`: Value of `api.type.enum.LineProblem` (e.g. `NO_PATH`), or -1 for a cargo misconfiguration described by `lineIssues`.
- `okModes : {TransportMode : boolean}`: Transport modes from the problem data; with `relaxedModes` and `allowedModes` they give the reason text for `NO_PATH` and `BAD_ALTERNATIVE_TERMINAL`.
- `relaxedModes : {TransportMode : boolean}`: Relaxed transport modes from the problem data, see `okModes`.
- `allowedModes : {TransportMode : boolean}`: Allowed transport modes from the problem data, see `okModes`.
- `locationOverride : {integer, integer, integer}`: Position `{x, y, z}` of the problem node. Clicking the notification focuses the camera there instead of on the line.
- `lineIssues : {{LineIssue.Type, integer, CargoTypeId}}`: can't pass an array of `LineIssue` from sim to UI (it causes a crash when copying the type), so passing a tuple of its content instead

## LineProblemNotificationSimParams (record)

```teal
global record LineProblemNotificationSimParams
```

Sim params of `line_warning`.

- `lineName : string`: Name of the line.

## NewCargoDemandNotificationParams (record)

```teal
global record NewCargoDemandNotificationParams
```

Params of the `newcargodemand` notification ("New Cargo Demand", category `Info`).

In game: "New Cargo Demand", "{town} has grown and its industrial and commercial buildings offer new products. Therefore, they now also require a new cargo type: {cargoName}." (German: „Neuer Güterbedarf“, „{town} ist gewachsen und seine Industrie- und Gewerbegebäude bieten neue Produkte an. Daher benötigen sie nun auch einen neuen Gütertyp: {cargoName}.“)

- `entity : EntityUtil.EntityAndRevision`: The town.
- `cargoTypeId : CargoTypeId`: The cargo type the town now requires.

## NewCargoDemandNotificationSimParams (record)

```teal
global record NewCargoDemandNotificationSimParams
```

Sim params of `newcargodemand`.

- `name : string`: Name of the town.

## NoRoadConnection (record)

```teal
global record NoRoadConnection
```

Params of the persistent `noroadconnection` notification ("Station Not Connected", category `Caution`), created from `api.engine.util.line.getNoRoadConnectionProblems()`.

In game: "No Road Connection", "{stationName} is not connected to a road." (German: „Keine Strassenanbindung“, „{stationName} ist nicht an eine Strasse angeschlossen.“)

- `entities : {EntityUtil.EntityAndRevision}`: The entity without road connection.

## NoRoadConnectionSimParams (record)

```teal
global record NoRoadConnectionSimParams
```

Sim params of `noroadconnection`.

- `name : string`: Name of the entity, set when `entities` has exactly one entry.

## SubventionMissedNotificationParams (record)

```teal
global record SubventionMissedNotificationParams
```

Params of the `subvention_missed` notification ("Subsidy Missed", category `Info`), sent by `subventions.script.tl` when an offered subsidy despawns.

In game: "Subsidy Missed" (German: „Subvention verpasst“)

- `entitiesAndRevisions0 : {{Engine.Entity, integer}}`: Relevant entities of the subsidy as `{entity, first revision number}` pairs.
- `subventionId : string`: Resource name of the subsidy.
- `reason : ISubvention.FailReason`: Why the subsidy was missed.
- `params : any`: Data from the subsidy script's `getNotificationParams`, passed back to its `getNotificationMissedData`.

## SubventionMissedNotificationSimParams (record)

```teal
global record SubventionMissedNotificationSimParams
```

Sim params of `subvention_missed`.

- `mapping : {Engine.Entity : string}`: Names of the relevant entities that still exist.

## SubventionNotificationParams (record)

```teal
global record SubventionNotificationParams
```

Params of the persistent `subvention_notification` notification ("Subsidy Status", category `Opportunity`) for proposed, active and completed subsidies.

In game: "Subsidy Available", "Subsidy in Progress", "Subsidy Effect Active" (German: „Neue Subvention verfügbar“, „Subvention läuft“, „Subventionseffekt aktiv“)

- `entitiesAndRevisions0 : {{Engine.Entity, integer}}`: Relevant entities of the subsidy as `{entity, first revision number}` pairs.
- `uid : number`: Unique id of the subsidy instance.
- `id : string`: Resource name of the subsidy.
- `status : integer`: 1 = proposed, 2 = in progress, 3 = effect active.
- `params : any`: Data from the subsidy script's `getNotificationParams`.

## SubventionNotificationSimParams (record)

```teal
global record SubventionNotificationSimParams
```

Sim params of `subvention_notification`.

- `mapping : {Engine.Entity : string}`: Names of the relevant entities.

## SubventionFailedNotificationParams (record)

```teal
global record SubventionFailedNotificationParams
```

Params of the `subvention` notification ("Subsidy Failed", category `Opportunity`), sent by `subventions.script.tl`.

In game: "Subsidy Failed" (German: „Subvention gescheitert“)

- `entitiesAndRevisions0 : {{Engine.Entity, integer}}`: Relevant entities of the subsidy as `{entity, first revision number}` pairs.
- `status : ISubvention.Status`: Always `"Failed"` in the base game.
- `taskState : ISubvention.FailReason`: Why the subsidy failed.
- `uid : number`: Unique id of the subsidy instance.
- `id : string`: Resource name of the subsidy.
- `params : any`: Data from the subsidy script's `getNotificationParams`.

## SubventionFailedNotificationSimParams (record)

```teal
global record SubventionFailedNotificationSimParams
```

Sim params of `subvention`.

- `mapping : {Engine.Entity : string}`: Names of the relevant entities.

## TownRatingNotificationParams (record)

```teal
global record TownRatingNotificationParams
```

Params of the persistent `town_rating_warning` ("Town Rating Very Poor", `Problem`) and `town_rating_warning_nonperistent` ("Town Rating Decreasing", `Caution`) notifications.

In game: "Town Rating Very Poor", "Town Rating Decreasing", "The rating "{what}" of {town} is very poor and the residents are furious. Action should be taken immediately." (German: „Stadtbewertung sehr schlecht“, „Stadtbewertung sinkt“, „Die "{what}"-Bewertung von {town} ist sehr schlecht und die Einwohner sind wütend. Es sollte umgehend gehandelt werden.“)

- `entities : {EntityUtil.EntityAndRevision}`: The town.
- `param : TownRatingParams`: Which rating is low.

### TownRatingNotificationParams.TownRatingParams (record)

```teal
record TownRatingParams
```

Which rating is low.

- `key : string`: Key of the rating, as in `town_util.GetRatings()`.

## TownProblemNotificationParams (record)

```teal
global record TownProblemNotificationParams
```

Params of the persistent `town_warning` notification ("Town Not Connected", category `Problem`), created from `api.engine.util.town.getTownProblems()`.

In game: "Town Not Connected", "The road connection between {fromTownName} and {toTownName} has been interrupted.", "Travel Time Between Towns Is Too Long" (German: „Stadt nicht angebunden“, „Die Strassenverbindung zwischen {fromTownName} und {toTownName} wurde unterbrochen.“, „Fahrzeit zwischen Städten ist zu lang“)

- `entities : {EntityUtil.EntityAndRevision}`: The two towns.
- `param : TownProblem`: Value of `api.type.enum.TownProblem`. `Disconnected` means the road connection is interrupted; any other value means the travel time is too long.

## TownNotificationSimParams (record)

```teal
global record TownNotificationSimParams
```

Sim params of the town notifications.

- `mapping : {Engine.Entity : string}`: Names of the towns.

## VehicleProblemNotificationParams (record)

```teal
global record VehicleProblemNotificationParams
```

Params of the persistent `vehicle_warning` notification ("Vehicle Problem", category `Problem`), created from `api.engine.util.vehicle.getVehicleProblems()`.

In game: "Vehicle Problem", "No Electrified Path", "Trains Are Blocking Each Other" (German: „Fahrzeugproblem“, „Keine elektrifizierte Strecke“, „Züge blockieren sich gegenseitig“)

- `entities : {EntityUtil.EntityAndRevision}`: The affected vehicles.
- `param : integer`: VehicleProblem

## VehicleProblemNotificatioSimParams (record)

```teal
global record VehicleProblemNotificatioSimParams
```

Sim params of `vehicle_warning` (the record name is misspelled in the game files).

- `mapping : {Engine.Entity : string}`: Names of the vehicles.
- `typeMapping : {Engine.Entity : VehicleUtil.VehicleType}`: Vehicle type of each vehicle, taken once; `"Train"` for vehicles that no longer exist.

## VehicleConditionNotificationParams (record)

```teal
global record VehicleConditionNotificationParams
```

Params of the persistent `vehiclecondition` notification ("Vehicle Condition", category `Caution`) for vehicles in bad condition (see `NotificationsState.wastedVehicles`).

In game: "Vehicle Condition", "The condition of {vehicle} is very bad." (German: „Fahrzeugzustand“, „Der Zustand von {vehicle} ist sehr schlecht.“)

- `entities : {EntityUtil.EntityAndRevision}`: The vehicle.

## VehicleConditionNotificationSimParams (record)

```teal
global record VehicleConditionNotificationSimParams
```

Sim params of `vehiclecondition`.

- `mapping : {Engine.Entity : string}`: Names of the vehicles.
- `typeMapping : {Engine.Entity : VehicleUtil.VehicleType}`: Vehicle type of each vehicle, taken once; `"Train"` for vehicles that no longer exist.

## OvercrowdingNotificationParams (record)

```teal
global record OvercrowdingNotificationParams
```

Params of the persistent `overcrowding` notification ("Station Overcrowded", category `Caution`) for player-served station groups whose cargo overflows.

In game: "Station Overcrowded", "{stationName} is overcrowded, passengers will be leaving." (German: „Station überfüllt“, „{stationName} ist überfüllt, Passagiere werden heimgehen.“)

- `entity : EntityUtil.EntityAndRevision`: The station group.

## StationBusyNotificationSimParams (record)

```teal
global record StationBusyNotificationSimParams
```

Sim params of `overcrowding`.

- `mapping : {Engine.Entity : string}`: Name of the station group.
