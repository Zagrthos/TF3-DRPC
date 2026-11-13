---
title: Single instance
type: reference
audience: [contributor, agent]
area: [app, windows, macos]
covers:
  - companion/Tf3Drpc.App/Platforms/Windows/WindowsSingleInstanceService.cs
  - companion/Tf3Drpc.App/Platforms/MacCatalyst/MacSingleInstanceService.cs
  - companion/Tf3Drpc.App/Services/Interfaces/ISingleInstanceService.cs
last-reviewed: 2026-11-13
verified:
  - macos 2026-10-20
---

# Single instance

Only one copy of the [companion app](README.md) runs per user session; a second start hands over to the running app, which shows its [settings window](settings-window.md).

## Windows

MAUI's `OnAppInstanceActivated` hook with `AppInstance.FindOrRegisterForKey`: a second start hands over to the running app, which shows the settings window.

## macOS

- One instance per user session as well, without a file: a new process that finds an older one (a lower process id, not terminated) with the same bundle identifier (`NSRunningApplication`) posts a distributed notification and exits, and the running app shows its settings window.
- Only the younger process yields, so of two starts at the same moment exactly one keeps running ([review R02](../../../reviews/companion/2026-10-20/findings.md)).
- A start that LaunchServices hands to the running app as a reopen event does the same.
- A lock file in `DATA/` would only cover one folder, so two copies in different folders could run at once.
- The reopen handler is registered when the window scene first activates; one set earlier is replaced while the app finishes launching.
- Limit: when a second copy hands over, the running one shows its window but may stay behind the active app, because macOS grants the activation to the process the user started, which exits.
  Over the full-screen game the window does come to the front, since it joins the game's Space.
