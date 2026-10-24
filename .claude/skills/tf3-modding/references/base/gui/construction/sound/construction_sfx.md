# gui.construction.sound.construction_sfx

Definition file: `base/tealdef/gui/construction/sound/construction_sfx.d.tl` (in the game folder).

Sound effects of the construction tools, loaded from the GUI resource `::/gui/construction/sound/construction_sfx.gres`.

## ConstructionSFX (record)

```teal
global record ConstructionSFX
```

Sound effects of the construction tools (GUI resource `::/gui/construction/sound/construction_sfx.gres`). Each entry is a list of sound files.

- `ConfirmBuild : {FilePath}`: Sound played when a build is confirmed (`tool/build.wav`).
- `CancelBuild : {FilePath}`: Sound played when a build is cancelled (`tool/cancel.wav`).
- `RaiseSlope : {FilePath}`: Sound played when the slope is raised (`slope/raise.wav`).
- `LowerSlope : {FilePath}`: Sound played when the slope is lowered (`slope/lower.wav`).
- `ToggleOnFixedSlope : {FilePath}`: Sound played when the fixed slope is switched on (`slope/fixed_on.wav`).
- `ToggleOffFixedSlope : {FilePath}`: Sound played when the fixed slope is switched off (`slope/fixed_off.wav`).
