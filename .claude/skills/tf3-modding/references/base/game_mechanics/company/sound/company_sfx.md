# game_mechanics.company.sound.company_sfx

Definition file: `base/tealdef/game_mechanics/company/sound/company_sfx.d.tl` (in the game folder).

Sound effects of the company window (`company_sfx.gres`).

## CompanySFX (record)

```teal
global record CompanySFX
```

Sound effects of the company window.

- `Unlock : {FilePath}`: Sounds played on an unlock.
- `LevelUp : {FilePath}`: Sounds played on a rank-up.
- `Animation : AnimationSFX`: Sounds of the rank animation.

### CompanySFX.AnimationSFX (record)

```teal
record AnimationSFX
```

Sounds of the rank animation by intensity.

- `VeryLow : {FilePath}`: Lowest intensity.
- `Low : {FilePath}`: Low intensity.
- `Medium : {FilePath}`: Medium intensity.
- `High : {FilePath}`: High intensity.
- `VeryHigh : {FilePath}`: Highest intensity.
