---
title: 0011 Tray icon through WinUIEx TrayIcon
type: decision
audience: [contributor, maintainer, agent]
area: [app, windows]
status: accepted
decided: 2026-10-24
last-reviewed: 2026-11-10
---

# 0011 Tray icon through WinUIEx TrayIcon

## Context

The tray icon was first made with `H.NotifyIcon.WinUI`.
WinUIEx's `TrayIcon` has the same behaviour and no AOT warnings, is actively maintained and is used by Microsoft apps.

## Decision

The tray icon uses WinUIEx's `TrayIcon`, which replaced `H.NotifyIcon.WinUI`.

## Alternatives considered

- `H.NotifyIcon.WinUI`: used first and then replaced by WinUIEx's `TrayIcon`.
