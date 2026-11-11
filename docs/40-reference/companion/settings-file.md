---
title: Settings file
type: reference
audience: [contributor, agent]
area: [app]
covers:
  - companion/Tf3Drpc.Core/Settings/**
  - companion/Tf3Drpc.Core/Serialization/**
  - companion/Tf3Drpc.App/Services/SettingsService.cs
last-reviewed: 2026-11-11
---

# Settings file

`DATA/settings.json` (see [portable operation](portable.md)) is read and written with a source-generated `System.Text.Json` context.

It stores:

- The language (`"language"`: `"game"` or a language code).
- The [theme](settings-window.md#theme) of the settings window (`"theme"`: `"system"`, `"light"` or `"dark"`, case-sensitive, `"system"` by default).
- The [preset](../presence/presets-and-templates.md#presets) (`"preset"`: `"career"`, `"network"`, `"era"`, `"alternating"`, `"minimal"` or `"custom"`).
- The [custom templates](../presence/presets-and-templates.md#custom-templates-and-preview) (`"templates"` with `"details"`, `"state"` and `"hover"`).
- Whether the presence carries the [store button](../presence/presets-and-templates.md#store-button) (`"storeButton"`, a boolean, `false` by default).

An invalid value falls back to its default without resetting the others, whatever its JSON type (a `language`, `theme` or `preset` that isn't a string, a `storeButton` that isn't `true` or `false`, `templates` that isn't an object); a template over 512 characters is cut.
A missing or broken file means defaults; the app writes to a temporary file and renames it, so a crash can't leave a half-written file.
