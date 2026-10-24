# gui.main.particle_colors

Definition file: `base/tealdef/gui/main/particle_colors.d.tl` (in the game folder).

Colours of GUI particle effects, as a GUI resource (`::/gui/main/particle_colors.gres`), used by `ParticleReactUtil`.

## ParticleColors (record)

```teal
global record ParticleColors
```

Particle effect colours; read with `api.gui.genericRep.get(api.gui.genericRep.find("::/gui/main/particle_colors.gres")).data`.

- `LevelUpStars : GResColor`: Colour of the stars of the level-up particle effect (gold).
- `UnlockStarPhase1 : GResColor`: Colour of the first phase of the unlock star effect (white).
- `UnlockStarPhase2main : GResColor`: Main colour of the second phase of the unlock star effect (gold).
- `UnlockStarPhase2secondary : GResColor`: Secondary colour of the second phase of the unlock star effect (white).
