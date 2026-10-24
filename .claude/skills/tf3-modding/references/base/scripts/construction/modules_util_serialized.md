# scripts.construction.modules_util_serialized

Definition file: `base/tealdef/scripts/construction/modules_util_serialized.d.tl` (in the game folder).

Types of the module helpers in `/scripts/construction/modulesutil.lua` (`ModulesUtilSerialized`) used
by modular constructions and their module scripts.

## ModulesUtilSerialized (record)

```teal
global record ModulesUtilSerialized
```

Module helpers from `/scripts/construction/modulesutil.lua`.

- `addCosts : function(result : ConstructionUtilSerialized.ConstructionResult, mod : ConstructionUtilSerialized.ModuleUpdateFnParams.Module)`: Adds a module's `metadata.price`, `metadata.maintenanceCost` and `metadata.bulldozePrice` to the result's `cost`, `maintenanceCost` and `bulldozeCost`.
    - notes: The construction script calls this automatically after running each module's update script.
- `TransformAlignmentFaces : function(transf : Mat4Serialized.Mat4, terrainAlignmentLists : {ConstructionUtilSerialized.TerrainAlignmentList})`: Transforms the points of all `faces` (and `triangles`) of the terrain alignment lists in place with `transf`.

    ```lua
    -- from base/content/landmarks/hq/hq/headquarter.script.tl
    modulesutil.TransformAlignmentFaces(transf2, terrainAlignmentLists)
    table.insert(result.subconstructions[1].terrainAlignmentLists, terrainAlignmentLists[1])
    ```

- `addGroundFaces : function(groundTexture : string, data : any, result : ConstructionUtilSerialized.SubConstruction, transform : Mat4Serialized.Mat4)`: Adds a ground face filled with `groundTexture` from the `static.curves.ground_face` curve of the
generated data, with texture coordinates stretched over its bounding box.
    - param `groundTexture`: Ground texture key, e.g. a `.gtex` file.
    - param `data`: Generated data with a `static` group.
    - param `result`: Subconstruction (or result) whose `groundFaces` receive the face.
    - param `transform`: Optional transform applied to the points first.
