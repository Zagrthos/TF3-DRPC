# gui.main.credits_react_util

Definition file: `base/tealdef/gui/main/credits_react_util.d.tl` (in the game folder).

The credits slide show of the main menu. The slides come from
`gui/menu/configs/credits_screen_data.lua`, which returns a table of the form `{ slides = {...} }`.

## CreditsReactUtil (record)

```teal
global record CreditsReactUtil
```
Load: `local credits_react_util = ug_require "::/gui/main/credits_react_util.tl" as CreditsReactUtil`

Credits recipe and the data types of the credits slides.

- `slides : {Slide}`: The list of slides (filled from `credits_screen_data.lua`).
- `CreditsContent : Recipe<CreditsContentParam>`: The credits slide show; the user can go back and forward (left/right or click), pause (up) and continue (down).
  In game: "Pause", "Continue", "Next" (German: „Pause“, „Fortsetzen“, „Weiter“)
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/credits_page.tl:17`: `credits_react_util.CreditsContent{`

### CreditsReactUtil.CreditsContentParam (record)

```teal
record CreditsContentParam
```

Parameters of `CreditsContent`.

- `setPause : function(value : boolean)`: Called with `true` or `false` when the user pauses or continues the slide show.
- `getPause : function() : boolean`: Returns whether the slide show is paused.
- `setFinished : function()`: Called when a `Breakpoint` slide is reached while `stopOnBreakpoint` is `true`.
- `stopOnBreakpoint : boolean`: When `true`, the show stops at `Breakpoint` slides (and calls `setFinished`); otherwise they are skipped.

### CreditsReactUtil.Credit (record)

```teal
record Credit
```

One entry of a credits slide (a text line, an image, or two sub-columns).

- `filterOut : any`
- `type : string`: Kind of text entry, which sets its style (`"HEADER0"`, `"HEADER1"`, `"HEADER2"`, `"NAME"`, `"LICENSE"`, `"LINK"`, or `"TWO_COLUMNS"` for an entry made of two columns in `subItems`).
- `text : string`: Text of the entry.
- `rich : boolean`
- `image : string`: Path of an image; when set, the entry shows only this image.
- `subItems : {{Credit}}`: For `type = "TWO_COLUMNS"`, the entries of the left (`[1]`) and right (`[2]`) column.

### CreditsReactUtil.Slide (record)

```teal
record Slide
```

One slide of the credits.

- `slideType : SlideType`: Layout of the slide.
- `items : {Credit}`: Entries of a `SingleColumn` or `Scroller` slide.
- `rows : {{{Credit}}}`: Rows of a `TwoColumns` slide; each row is `{leftEntries, rightEntries}`.
- `duration : number`: Display time of the slide, in seconds (default 6); not used for `Scroller` slides.

#### CreditsReactUtil.Slide.SlideType (enum)

```teal
enum SlideType
```

Layout of a slide.

- `"SingleColumn"`: The `items` in one column.
- `"TwoColumns"`: The `rows`, each with a left and a right column.
- `"Scroller"`: The `items` in an automatically scrolling area; the next slide starts when the end is reached.
- `"Breakpoint"`: No content; ends the show when `stopOnBreakpoint` is set, otherwise skipped.
