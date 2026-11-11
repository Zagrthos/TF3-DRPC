---
title: Portable operation
type: reference
audience: [contributor, agent]
area: [app, windows, macos]
covers:
  - companion/Tf3Drpc.App/Services/PathServiceBase.cs
  - companion/Tf3Drpc.App/Platforms/*/*PathService.cs
  - companion/Tf3Drpc.App/Utilities/LogFileUtils.cs
  - companion/Tf3Drpc.App/Services/AppLifecycleService.cs
last-reviewed: 2026-11-11
verified:
  - macos 2026-10-20
---

# Portable operation

The [companion app](README.md) has no installer: it runs from the extracted zip and writes only into a `DATA` folder next to its own files.
The rule for contributors is in [portable app](../../30-conventions/portable-app.md); the decision is [0007](../../70-decisions/0007-portable-app-writing-only-to-data.md).

```
TF3-DRPC/                    ← extracted zip, anywhere the user likes
├── TF3-DRPC.exe + DLLs      (Windows)  /  TF3-DRPC.app  (macOS)
├── LICENSE.txt, THIRD-PARTY-NOTICES.txt
└── DATA/                    ← created on first start
    ├── settings.json
    └── logs/TF3-DRPC_<yyyy-MM-dd>.log
```

## Where DATA is

Next to the executable on Windows (`AppContext.BaseDirectory`), next to the `.app` bundle on macOS.
Never inside the bundle: changing files there breaks the code signature.

## Nothing outside

No registry, no `%APPDATA%`/`%LOCALAPPDATA%`, no `~/Library/Preferences` or `~/Library/Application Support`.
So no MAUI `Preferences`, `SecureStorage` or `FileSystem.AppDataDirectory`, and no start with the system (registry `Run` key or login item).

## Not writable

- When `DATA` can't be written (for example extracted into `Program Files`), the app shows a message and runs with defaults, without saving.
- The same message appears when saving a setting failed; changing any setting again retries the save.
- A save probes the data folder again when it wasn't writable, so a folder repaired while the app runs is noticed and the message goes away; a save that wasn't written counts as failed.
- A template edit still waiting for its save is kept on both Exit routes: the lifecycle service raises `Exiting` on the UI thread before it quits, and the settings view model saves the edit then.

## macOS App Translocation

- macOS starts a quarantined app downloaded in a zip from a hidden read-only copy, where `DATA` can't be written.
- The app detects this (`SecTranslocateIsTranslocatedURL` on the bundle URL), opens the settings window at start and shows "Move TF3-DRPC.app once with Finder…" in the place of the not-writable warning; moving the app ends the translocation.
- A check with an ad-hoc-signed copy carrying a user-approved quarantine flag showed the process running from `…/AppTranslocation/…`.
- An unsigned or ad-hoc-signed download is first stopped by Gatekeeper ("Open Anyway" in Privacy & Security); a signed and notarised release avoids that.

## Operating-system data

Operating-system data the app doesn't write itself (for example macOS crash reports or the Windows tray icon cache) is outside its control.
On macOS that includes two small files the system frameworks write before the app can prevent it:

- `~/Library/Preferences/com.zagrthos.tf3drpc.plist` with the window's first frame (AppKit's frame autosave; it can also hold the frames of AppKit's spelling panels).
- `~/Library/Saved Application State/com.zagrthos.tf3drpc~iosmac.savedState` with UIKit's list of scene sessions.

The app turns off window state restoration and further frame saves; stopping the first writes would need private APIs.

## Check

- A bounded check on Windows found nothing outside `DATA/` (see [other checks](../../50-testing/manual-test.md#other-checks)).
- On macOS the only files seen outside `DATA/` are the two above, and the bounded check over a 45-minute game run found nothing written outside `DATA/` (both files were older than its marker).
- A full trace with Process Monitor or `fs_usage` is not part of the check ([decision 0007](../../70-decisions/0007-portable-app-writing-only-to-data.md)).

## Logs

- NReco.Logging.File writes one file per day, `DATA/logs/TF3-DRPC_<yyyy-MM-dd>.log`, and files older than 7 days are deleted at start (a locked file is skipped).
- The log folder is probed on its own: if it can't be written, or the file logger fails to start, the app runs without a log file.
- A log file that can't be written (for example because another program holds today's file open) is reported once by NReco's `HandleFileError`; the settings window then shows its own warning, separate from the one about settings, through `IPathService.IsLogFileAvailable` and a message.
- NReco can't retry with another name and still report a second failure (it doesn't call the handler recursively), so there is no retry.
- A log folder that was fixed later isn't picked up: the file logger is created only at start.
