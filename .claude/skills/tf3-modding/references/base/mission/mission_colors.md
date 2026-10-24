# mission.mission_colors

Definition file: `base/tealdef/mission/mission_colors.d.tl` (in the game folder).

Colour palette (`gui_res` `::/mission/mission_colors.gres`) used by mission markers, zones and highlights.

## MissionColors (record)

```teal
global record MissionColors
```

Colour palette of mission markers, zones and highlights (`::/mission/mission_colors.gres`).

- `Medal : GResColor`: Medal colour (gold).
- `Good : GResColor`: Positive colour (green).
- `Target : GResColor`: Colour of target zones (yellow).
- `SecondaryTarget : GResColor`: Colour of secondary targets (dark orange).
- `Bad : GResColor`: Negative colour (red).
- `Highlight : GResColor`: Colour of highlighted GUI elements (daffodil).
- `Guide : GResColor`: Guide colour (gold).
- `GuideBlink : GResColor`: Blink colour of guides (goldenrod).
