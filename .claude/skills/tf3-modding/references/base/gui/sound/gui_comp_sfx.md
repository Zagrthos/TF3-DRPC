# gui.sound.gui_comp_sfx

Definition file: `base/tealdef/gui/sound/gui_comp_sfx.d.tl` (in the game folder).

specific on exits:

Sound effects of the standard GUI components, defined in the GUI resource `gui/sound/gui_comp_sfx.gres`.

## GuiCompSFX (record)

```teal
global record GuiCompSFX
```

Sound files of the standard GUI components (data of `gui/sound/gui_comp_sfx.gres`).

- `GeneralOnEnter : {FilePath}`: Sound played when a component is pressed (`soundOnEnter` of `:active` states), `on_enter.wav`.
- `Hover : {FilePath}`: Sound played when the mouse moves onto some components, `hover.wav`.
- `ButtonClick : {FilePath}`: Sound of a button click, `button_click.wav`.
- `SliderDrag : {FilePath}`: Sound when a slider handle is released, `slider.wav`.
- `ScrollBar : {FilePath}`: Sound when a scroll bar is released, `scrollbar.wav`.
- `SpinBox : {FilePath}`: Sound of spin boxes, `spinbox.wav`.
- `TabClick : {FilePath}`: Sound when a tab is selected, `tab.wav`.
- `ToggleOn : {FilePath}`: Sound when a toggle button is switched on, `toggle_on.wav`.
- `ToggleOff : {FilePath}`: Sound when a toggle button is switched off, `toggle_off.wav`.
- `CheckboxCheck : {FilePath}`: Sound when a check box is checked, `checkbox_check.wav`.
- `CheckboxUncheck : {FilePath}`: Sound when a check box is unchecked, `checkbox_uncheck.wav`.
- `Dropdown : {FilePath}`: Sound of drop-down buttons, `dropdown.wav`.
- `WindowClose : {FilePath}`: Sound of a window's close button, `window_close.wav`.
