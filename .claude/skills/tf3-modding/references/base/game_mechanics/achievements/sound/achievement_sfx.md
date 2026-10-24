# game_mechanics.achievements.sound.achievement_sfx

Definition file: `base/tealdef/game_mechanics/achievements/sound/achievement_sfx.d.tl` (in the game folder).

Sound played when an achievement is earned (`achievement_sfx.gres`).

## AchievementSFX (record)

```teal
global record AchievementSFX
```

Sound of earned achievements, defined in `::/game_mechanics/achievements/sound/achievement_sfx.gres`.

- `Earned : {FilePath}`: Sound files passed to `api.gui.achievements.setEarnedSFX` by the achievements game script.
