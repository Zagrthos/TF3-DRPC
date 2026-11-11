---
title: 0012 A theme setting for the settings window only
type: decision
audience: [player, contributor, maintainer, agent]
area: [app, windows, macos]
status: accepted
decided: 2026-10-24
last-reviewed: 2026-11-10
---

# 0012 A theme setting for the settings window only

## Context

The settings window is the app's only own surface.
The tray and menu bar menus are drawn by the system and follow its setting.

## Decision

The app has a theme setting for the settings window only, and "Same as the system" is the default.

## Consequences

- The default changes nothing for players who never open the picker.
