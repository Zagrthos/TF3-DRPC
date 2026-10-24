# landmarks.landmark

Definition file: `base/tealdef/landmarks/landmark.d.tl` (in the game folder).

State and notification types of the landmarks game script (`/landmarks/landmarks.script.tl`).

## LandmarksNotificationParam (record)

```teal
global record LandmarksNotificationParam
```

Parameters of the notification shown when a landmark is finished.

In game: "Landmark Progress", "Supply resources to the construction site.", "Landmark Finished" (German: „Wahrzeichen Baufortschritt“, „Liefere Ressourcen zur Baustelle.“, „Wahrzeichen fertiggestellt“)

- `sound : FilePath`: Sound played with the notification (the landmark's `constructionFinishedSound`).
- `entity : EntityUtil.EntityAndRevision`: The finished landmark construction.

## LandmarksState (record)

```teal
global record LandmarksState
```

State of the landmarks game script.

- `entity2cargoType2count : {Engine.Entity : {CargoTypeId : integer}}`: Delivered cargo per landmark stock list and cargo type.
- `town2carrier2modifiers : {Engine.Entity : {Carrier : {string : number}}}`: Vehicle modifiers currently applied per town and carrier.
- `transportVehicle2town : {Engine.Entity : Engine.Entity}`: Legacy field; the script clears it.
