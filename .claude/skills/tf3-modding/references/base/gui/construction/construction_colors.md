# gui.construction.construction_colors

Definition file: `base/tealdef/gui/construction/construction_colors.d.tl` (in the game folder).

Colors of the construction tools, loaded from the GUI resource `::/gui/construction/construction_colors.gres`.

## ConstructionColors (record)

```teal
global record ConstructionColors
```

Colors of the construction tools (GUI resource `::/gui/construction/construction_colors.gres`).

Example:
```lua
local constructionColors = api.gui.genericRep.get(api.gui.genericRep.find("::/gui/construction/construction_colors.gres")).data
```

- `Modifier : ModifierColor`: The modifier highlight colors.
- `Bulldozer : GResColor`: Color of the bulldozer button in the game bar (gold in the base game).

### ConstructionColors.ModifierColor (record)

```teal
record ModifierColor
```

Colors used to highlight edges while a lane or edge modifier tool is active.

- `Valid : GResColor`: Highlight of edges the modifier can be applied to (used with medium and low transparency).
- `Invalid : GResColor`: Highlight of edges the modifier cannot be applied to.
- `Upgrade : GResColor`: Highlight of edges that would be upgraded.
- `Downgrade : GResColor`: Highlight of edges that would be downgraded.
- `InProgress : GResColor`: Highlight of edges while the modification is being applied (white in the base game).
