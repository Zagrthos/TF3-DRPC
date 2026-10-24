# landmarks.landmark_util_serialized

Definition file: `base/tealdef/landmarks/landmark_util_serialized.d.tl` (in the game folder).

Types of `/landmarks/landmark_util_serialized.tl` (`LandmarkUtilSerialized`), the shared update function of the landmark constructions.

## LandmarkUtilSerialized (record)

```teal
global record LandmarkUtilSerialized
```

Landmark construction helper returned by `ug_require "::/landmarks/landmark_util_serialized.tl"`.

- `makeLandmarksUpdateFn : function(generatedData : ConstructionUtilSerialized.GeneratedDataGroup) : function(captureParams : table, params : table) : ConstructionUtilSerialized.ConstructionResult`: Creates the `updateFn` of a landmark construction from its generated data.
    - param `generatedData`: Generated data with one group per construction level (`level0`, `level1`, ...).
    - returns: The update function; it builds the subconstruction from the group of the current level and adds stocks, rules, lanes and terminals from `captureParams.constructionConfig` and `miscConfig`.

    ```lua
    -- from base/content/landmarks/neuschwanstein/neuschwanstein/neuschwanstein.script.lua
    updateFn = landmark_util_serialized.makeLandmarksUpdateFn(generatedData),
    ```

