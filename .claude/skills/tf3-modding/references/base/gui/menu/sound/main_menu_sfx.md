# gui.menu.sound.main_menu_sfx

Definition file: `base/tealdef/gui/menu/sound/main_menu_sfx.d.tl` (in the game folder).

Sound resource of the main menu.

## MainMenuSFX (record)

```teal
global record MainMenuSFX
```

Sounds of the main menu.

- `StartGame : {FilePath}`: Sound (`start_game.wav`) of the "load savegame" buttons in the main and pause menu, played when such a button leaves its pressed state (`soundOnExit` of the `load-savegame-sound` style class).
