# game_mechanics.towns.sound.town_sfx

Definition file: `base/tealdef/game_mechanics/towns/sound/town_sfx.d.tl` (in the game folder).

Sound effect lists for town events, loaded from `sound/town_sfx.gres`.

## TownSFX (record)

```teal
global record TownSFX
```

Sound files for town events. Each field is a list; `api.gui.sound.playRandomSoundEffect` picks one entry.

Example:
```lua
local townSFX = api.gui.genericRep.get(api.gui.genericRep.find("::/game_mechanics/towns/sound/town_sfx.gres")).data as TownSFX
api.gui.sound.playRandomSoundEffect(townSFX.Marketing)
```

- `LevelUp : {FilePath}`: Sounds of the town level-up notification (three variants).
- `Select : {FilePath}`: Selection sound of a town entity (`view_manager_util.tl`).
- `Prospection : {FilePath}`: Sound played when a new exploration plane appears.
- `Marketing : {FilePath}`: Sound played by the marketing campaign HUD display.
- `Greenification : {FilePath}`: Sound played by the greenification HUD display.
