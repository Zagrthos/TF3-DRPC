---
title: Texts and languages
type: reference
audience: [contributor, agent]
area: [app, translations, discord]
covers:
  - companion/Tf3Drpc.Core/Resources/**
  - companion/Tf3Drpc.Core/Languages/**
  - companion/Tf3Drpc.Core/Translation/**
  - companion/Tf3Drpc.Core/Enums/OwnText.cs
last-reviewed: 2026-11-11
verified:
  - windows 2026-10-21
---

# Texts and languages

Where the texts of the Discord presence and of the [companion app](../companion/README.md) come from, and the 13 languages they come in.
The layouts that use them are in [presets and templates](presets-and-templates.md).

## Languages

The 13 game languages: `de`, `en`, `es`, `fr`, `it`, `ja`, `ko`, `nl`, `pl`, `pt_BR`, `ru`, `zh_CN`, `zh_TW`.

The player picks the language in the menu → "Language" or in the settings window: "Same as the game" (default; `profile.lua` → `language.code`, Lua comments ignored, English for unknown codes) or one of the 13, each in its own name (Deutsch, English, Español, Français, Italiano, 日本語, 한국어, Nederlands, Polski, Português (Brasil), Русский, 简体中文, 繁體中文).
It applies to Discord and to the app's own texts from the next update and is saved in the [settings file](../companion/settings-file.md).

## Status values in the game's language

Two status values are in the game's language:

- `rankName` isn't used; the app looks the rank name up by `rank`.
- Before a game is first saved, the mod's `saveName` is the game language's `New Game` text (German "Neues Spiel"; English has no entry and uses `New Game`).
  The app compares `saveName` with that text and, if equal, shows the `New Game` text of the chosen language instead, wherever `{saveName}` is used.
  Any other name is shown as it is.
  If the texts of the game language can't be read, nothing is replaced.
- A new game on a ready-made map ("Play custom map") has the map's name as `saveName` before its first save (for example "Emerald Shores"), so it is shown as it is, not as `New Game`.
  This is kept on purpose.

## The game's own texts

Read at runtime from `<game>/base/strings/<lang>/LC_MESSAGES/base.mo` (gettext; the msgid is the English text) in the folder of the running game, so no game texts are copied into this repository.
All 13 files ship with every game install.
The `.mo` reader is our own, small and AOT-safe.
Keys:

| Use | Keys |
|---|---|
| Ranks 1–15 | `Junior`, `Mechanic`, `Engineer`, `Coordinator`, `Expert`, `Team Leader`, `Supervisor`, `Manager`, `Director`, `Senior Director`, `CEO`, `Chairperson`, `Vice President`, `President`, `Tycoon` |
| Modes | `MOD_TYCOON_NAME` (Tycoon), `MOD_SANDBOX_NAME` (Sandbox Mode), `Map Editor` |
| Missions | `MISSION_01_NAME` … `MISSION_08_NAME`, number from the mod id `urbangames_campaign_mission_0X` (Saving Mardi Gras, Alpine Crossing, Desert Adventure, Biggest Festival Ever, Oil for the People, Island Expansion, Big City Problems, Final Countdown) |
| Climates | `Temperate`, `Subarctic`, `Tropical`, `Dry` for `::/climates/<name>/<name>.clima`; other climates get no name |
| Labels | `Campaign`, `Paused`, `Loading...`; for the presets `Lines`, `Vehicles`, `Stations`, `Trains`, `Passengers`, `Cargo`, `Year`, `Climate`, `Company Value`; `New Game` for the save name of an unsaved game |

## Formatting

Month names and numbers come from `CultureInfo` for the chosen language (`de` → `de-DE`, `pt_BR` → `pt-BR`, `zh_CN` → `zh-CN`, `zh_TW` → `zh-TW`, …), so `InvariantGlobalization` stays off.

## Our own texts

Shipped as `.resx` in all 13 languages (`Tf3Drpc.Core/Resources/Texts*.resx`, keys in the `OwnText` enum), ideally checked by native speakers:

- The presence and the tray / menu bar: "Starting the game", "In the main menu", "Playing", "In game", "Language", "Same as the game", "Settings…", "Exit".
- The settings window: "Transport Fever 3 isn't running", "Connected to Discord", "Not connected to Discord", the "can't write to its folder" warning, the "can't write its log file" warning, the macOS warning that the app runs from a hidden temporary copy (App Translocation), "Theme" with "Same as the system", "Light mode" and "Dark mode", "Status appearance" and the preset names ("Career", "Network", "Era", "Alternating", "Minimal", "Custom").
- The custom templates and the preview: "First line", "Second line", "Image tooltip", "Placeholders", an explanation for each placeholder, the note on personal data, "Preview", "Example values".
- The store button: its label "Get Transport Fever 3", the check box "Show a button to the Steam store", and the hint that others see it below the status but the player doesn't.

They were translated with DeepL.
Some store button labels were shortened or corrected by hand to fit Discord's 31 bytes and to say "get" rather than "download"; a test checks the limit in every language.
Some texts are spelled as in English ("Minimal" in German and French, "Era" in Italian, Polish and Portuguese), which the resource test allows.

## References

- Game: `base/strings/<lang>/LC_MESSAGES/base.mo` (translations), `base/content/game_mechanics.zip` → `company/company_static_util.tl` (rank order), `base/content/gui.zip` → `gui/game_bar/game_bar_widgets.tl` (speed values)
