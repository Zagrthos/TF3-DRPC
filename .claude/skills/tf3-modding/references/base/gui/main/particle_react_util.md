# gui.main.particle_react_util

Definition file: `base/tealdef/gui/main/particle_react_util.d.tl` (in the game folder).

Helpers for 2D GUI particle effects (`builtin.ParticleSystem2d`): presets for the company
level-up and unlock animations and a builder that turns a plain table into a
`ParticleSystem2dEmitter`.

## ParticleReactUtil (record)

```teal
global record ParticleReactUtil
```
Load: `local particle_react_util = ug_require "::/gui/main/particle_react_util.tl" as ParticleReactUtil`

GUI particle effect helpers.

- `makeCompanyLevelUpParticles : function(overrides? : Emitter) : {Builtin.Type.ParticleSystem2dEmitter}`  (call as `particle_react_util.makeCompanyLevelUpParticles(…)`): Creates the rising gold stars of the company level-up effect.
    - param `overrides`: Optional emitter fields that replace the preset values (e.g. `emissionEnabled`).
    - returns: The emitter list for `builtin.ParticleSystem2d{ emitters = ... }`.

    ```lua
    -- from game_bar.tl
    builtin.ParticleSystem2d{
        emitters = particle_react_util.makeCompanyLevelUpParticles({
            emissionEnabled = state:old().applyDiff > 0
        })
    }
    ```

    - used 1x in the base game, e.g. `base/content/gui/gui/game_bar/game_bar.tl:425`: `emitters = particle_react_util.makeCompanyLevelUpParticles({`
- `makeUnlockAnimationParticles : function(overrides1? : Emitter, overrides2? : Emitter) : {Builtin.Type.ParticleSystem2dEmitter}`  (call as `particle_react_util.makeUnlockAnimationParticles(…)`): Creates the star burst of the unlock animation (a short inward phase, then an explosion of gold and white stars).
    - param `overrides1`: Optional emitter fields that replace the values of the first phase.
    - param `overrides2`: Optional emitter fields that replace the values of the second phase.
    - returns: The emitter list for `builtin.ParticleSystem2d`.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company.tl:85`: `emitters = particle_react_util.makeUnlockAnimationParticles(`
- `makeParticleSystem2dEmitter : function(... : Emitter) : Builtin.Type.ParticleSystem2dEmitter`  (call as `particle_react_util.makeParticleSystem2dEmitter(…)`): Creates a `ParticleSystem2dEmitter` from one or more `Emitter` tables; later tables override earlier ones.
    - notes: Plain numbers, `Vec2f` values and lists of curve points are wrapped into the matching `ParticleParam*` types, so callers can write `lifeTime = 1.5` instead of `api.type.ParticleParamFloat.new(1.5)`.

### ParticleReactUtil.Emitter (record)

```teal
record Emitter
```

Simplified proxy for creating particle emitters
See Builtin.Type.ParticleSystem2dEmitter for details on the parameters

- `emissionEnabled : boolean`: Whether the emitter currently emits.
- `speed : number`: Factor that slows down or speeds up the emission rate.
- `texture : string`: Texture of a particle.
- `duration : number | any`: number|ParticleParamFloat.RandomMinMax|ParticleParamFloat.ComponentWiseMinMax
- `emissionInterval : number | any`: number|ParticleParamFloat.RandomMinMax|ParticleParamFloat.ComponentWiseMinMax
- `burstCount : number | any`: number|ParticleParamFloat.RandomMinMax|ParticleParamFloat.ComponentWiseMinMax
- `burstInterval : number | any`: number|ParticleParamFloat.RandomMinMax|ParticleParamFloat.ComponentWiseMinMax
- `lifeTime : number | any`: number|ParticleParamFloat.RandomMinMax|ParticleParamFloat.ComponentWiseMinMax
- `subEmitters : {Builtin.Type.ParticleSystem2dSubEmitter}`: Sub-emitters started from this emitter's particles; plain tables with an `emitterIndex` are converted.
- `shape : Builtin.Type.ParticleSystem2dEmitterShape`: Emitter shape (content rectangle or circle).
- `velocity : Vec2f | any`: Vec2f|ParticleParamVec2f.RandomMinMax|ParticleParamVec2f.ComponentWiseMinMax
- `velocityByDirection : number | any`: number|ParticleParamFloat.RandomMinMax|ParticleParamFloat.ComponentWiseMinMax
- `velocityOverLifeTime : {ParticleParamCurveVec2f.CurvePoint}`: Additional velocity curve over the particle lifetime, as list of curve points.
- `gravity : Vec2f | any`: Vec2f|ParticleParamVec2f.RandomMinMax|ParticleParamVec2f.ComponentWiseMinMax
- `orientation : Vec2f`: Reference orientation vector for rotation calculations (default `{0, -1}`).
- `rotationByDirection : number | any`: number|ParticleParamFloat.RandomMinMax|ParticleParamFloat.ComponentWiseMinMax
- `rotation : number | any`: number|ParticleParamFloat.RandomMinMax|ParticleParamFloat.ComponentWiseMinMax
- `rotationSpeed : number | any`: number|ParticleParamFloat.RandomMinMax|ParticleParamFloat.ComponentWiseMinMax
- `rotationSpeedByDirection : number | any`: number|ParticleParamFloat.RandomMinMax|ParticleParamFloat.ComponentWiseMinMax
- `colorOverLifeTime : {ParticleParamCurveVec3f.CurvePoint}`: Colour (RGB) curve over the particle lifetime, as list of curve points.
- `alphaOverLifeTime : {ParticleParamCurveFloat.CurvePoint}`: Alpha curve over the particle lifetime, as list of curve points.
- `size : Vec2f | any`: Vec2f|ParticleParamVec2f.RandomMinMax|ParticleParamVec2f.ComponentWiseMinMax
- `scaleOverLifeTime : {ParticleParamCurveVec2f.CurvePoint}`: Size curve over the particle lifetime, as list of curve points.
