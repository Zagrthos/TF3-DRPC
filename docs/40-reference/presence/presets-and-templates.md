---
title: Presets and templates
type: reference
audience: [contributor, agent]
area: [app, discord]
covers:
  - companion/Tf3Drpc.Core/Presence/**
  - companion/Tf3Drpc.Core/Constants/PresenceConstants.cs
  - companion/Tf3Drpc.Core/Settings/PresenceTemplates.cs
last-reviewed: 2026-11-13
---

# Presets and templates

How the [companion app](../companion/README.md) turns its state and the mod's [status](../mod/status-line.md) into the Discord presence.
The texts and languages are in [texts and languages](texts-and-languages.md).

## Design A "Career" (default)

```
Transport Fever 3                  ← the Discord application's name
Mechanic · Tycoon                  ← details: rank; the mode only if it isn't a normal game
September 1905                     ← state: in-game month and year
01:23:45 elapsed
```

- Large image: the cover, hover text "Transport Fever 3".
  No small image.
- Mode: "Tycoon", "Sandbox Mode" or "Map Editor".
  The Tycoon mod fixes the rank at Tycoon, so a Tycoon game shows "Tycoon", not "Tycoon · Tycoon": a part with `{mode}` but not `{rank}` is left out when another part of the same template shows the rank.
  A part with both stays as written.
  The rule doesn't compare the texts, because some languages word the rank and the mode differently (Korean 거물 and 타이쿤).
- Save name and money aren't shown by default; players can add them with the [custom templates](#custom-templates-and-preview) (`{saveName}`, `{balance}`).

Other situations:

| Situation | Details | State |
|---|---|---|
| Starting | "Starting the game" | |
| Main menu | "In the main menu" | |
| Loading | the game's `Loading...` | |
| Mission | "Campaign · <mission name>" | |
| Playing without the mod | "Playing" | |
| Paused (`speedup` 0 from the pause button, or no new status line for more than 20 s while the game menu is open; never in the map editor, where time doesn't run and `speedup` is always 0) | as in design A, from the last status | "· Paused" appended |

## Presets

The player picks the preset in the settings window ("Status appearance"); it is saved in the [settings file](../companion/settings-file.md).
Each preset is a pair of [templates](#templates).

| Preset | Details | State |
|---|---|---|
| A "Career" (default) | `{rank} · {mode}` → `Mechanic` | `{date}` → `September 1905` |
| B "Network" | `Lines {lines} · Vehicles {vehicles}` → `Lines 44 · Vehicles 118` | `Stations {stations} · {year}` → `Stations 74 · 1905` |
| C "Era" | `Year {year} · {climate}` → `Year 1905 · Tropical` | `{rank}` → `Mechanic` |
| D "Alternating" | A, B and C in turn, each for 45 s | |
| E "Minimal" | `In game · {year}` → `In game · 1905` | |
| F "Custom" | the player's [templates](#custom-templates-and-preview) | |

- Labels come before numbers ("Lines 44"), so the game's labels work without plural rules.
  They are the game's own texts, except "In game", which is ours.
- Hovering an entry of the "Status appearance" picker shows what that preset would show with the values the app already has (the last status, or the example values): the two lines, built like the preview when the tooltip opens (on macOS when the menu opens); nothing new is read.
  "Alternating" lists its three layouts in rotation order, separated by a blank line.
- Details that end up empty show "Playing"; a pause appends " · Paused" to the state, as in design A.
  A second line too long for Discord is shortened first, so the suffix always stays.
- Discord accepts an update at most every 15 s, so the 45 s of "Alternating" stay well inside its limit.

## Templates

A template is text with placeholders, split into parts by `·`:

- A part with a placeholder that has no value, or with an unknown placeholder, is dropped together with its `·` separator; the remaining parts are joined with ` · `.
- Numbers follow the chosen language (`1,234` or `1.234`); the year has no group separator.
  Money is compact: `$950`, `$15.6M` (`$15,6M` in German), `-$1.2M`.

| Placeholder | Value |
|---|---|
| `{rank}` | rank name, as the game bar shows it |
| `{mode}` | "Tycoon", "Sandbox Mode" or "Map Editor"; none in a normal game, and in a Tycoon game none in its own part while another part shows `{rank}` |
| `{date}` | in-game month and year in the language's own order (`September 1905`, `1905年9月`) |
| `{month}`, `{year}` | month name, year |
| `{climate}` | "Temperate", "Subarctic", "Tropical" or "Dry" |
| `{lines}`, `{stations}` | lines, stations |
| `{trains}` | rail vehicles |
| `{vehicles}` | road vehicles, rail vehicles, trams, aircraft and ships, summed as in the game's load dialogue; no value if the sum doesn't fit a 64-bit integer |
| `{passengers}`, `{cargo}` | passengers and cargo transported so far |
| `{companyValue}` (private) | assets + balance − debt, as in the game's finance chart |
| `{balance}` (private) | money; none with infinite money |
| `{saveName}` (private) | savegame name; before the first save "New Game" in the chosen language (see [status values in the game's language](texts-and-languages.md#status-values-in-the-games-language)) |

## Custom templates and preview

The preset "Custom" uses three templates the player writes in the settings window:

| Template | Shown as | Default |
|---|---|---|
| First line | details | `{rank} · {mode}` |
| Second line | state, with " · Paused" appended while paused (a long text is shortened to make room) | `{date}` |
| Image tooltip | hover text of the cover image | `Transport Fever 3` |

- The defaults are design A, so switching to Custom changes nothing until a template is edited.
  An empty first line shows "Playing", an empty tooltip "Transport Fever 3".
- Only Custom changes the tooltip; every other preset and state keeps "Transport Fever 3".
  It has the same limits as the two lines (128 characters, then "…").
- The window lists the placeholders, each with a short explanation in the chosen language (as in the [table](#templates)), and marks the private ones with 🔒, with the note that they are shown to everyone who sees the Discord status.
- A template is saved one second after the last change, and at most 512 characters long.
- The preview shows what Discord would show for the chosen preset and templates, from the last status the mod sent, also after leaving the game.
  Before any status arrived it uses example values from [log-samples/session.log](../log-samples/session.log) and says so.

## Store button

An optional button below the status that leads to the game's Steam page, `https://store.steampowered.com/app/3493540`.

- **Setting:** a check box in the settings window, "Show a button to the Steam store", saved as `"storeButton"` in the [settings file](../companion/settings-file.md).
  It is off by default, because a button in the status is advertising that the player has to choose.
- **Label:** "Get Transport Fever 3", in the chosen language (the own text `StoreButton`, in all 13 languages).
  Every presence carries it while the setting is on: starting, main menu, loading, mission and playing.
  The preview shows it under the two lines as a button-like frame; it isn't clickable there.
- **Where it shows:** Discord shows the button to everyone who looks at the profile or the status, but not to the player themselves, so check it from a second account.
- **Limits:** Discord takes a button label of at most 31 UTF-8 bytes.
  Our own labels all fit; a longer one would be cut at a character boundary, never in the middle of a character.
  Switching the button on or off changes the content, so the app sends it at the next update (within about 15 s).
