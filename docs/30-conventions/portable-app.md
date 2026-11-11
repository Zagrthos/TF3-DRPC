---
title: Portable app
type: rules
audience: [contributor, agent]
area: [app, windows, macos]
covers:
  - companion/Tf3Drpc.App/Services/PathServiceBase.cs
  - companion/Tf3Drpc.App/Platforms/*/*PathService.cs
last-reviewed: 2026-11-11
---

# Portable app

The companion app writes only into `DATA/` next to its own files.
How that works, and what the operating system writes by itself, is in [portable operation](../40-reference/companion/portable.md).

- Settings and logs go to `DATA/` through `IPathService`.
- The app keeps out of the registry and the user profile folders, and uses no MAUI `Preferences`, `SecureStorage` or `FileSystem.AppDataDirectory`.
- The app doesn't start with the system: no `Run` key, no login item.
- `DATA/` stays outside the macOS bundle, because changing files inside it breaks the code signature.
