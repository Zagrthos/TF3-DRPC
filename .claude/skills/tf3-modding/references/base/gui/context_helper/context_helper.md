# gui.context_helper.context_helper

Definition file: `base/tealdef/gui/context_helper/context_helper.d.tl` (in the game folder).

State of the context helper game script (`context_helper.script.tl`).

## ContextHelperScriptState (record)

```teal
global record ContextHelperScriptState
```

Game script state of the context helper (`gui/context_helper/context_helper.script.tl`).

- `showWelcome : boolean`: Set to `true` when the script receives the `initNewGameFromMap` event and reset to `false` on the
script's next update.
