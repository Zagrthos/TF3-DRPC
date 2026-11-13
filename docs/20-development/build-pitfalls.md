---
title: Build pitfalls
type: guide
audience: [contributor, agent]
area: [app, tools]
covers:
  - companion/Directory.Build.props
  - companion/Tf3Drpc.App/Tf3Drpc.App.csproj
  - companion/Tf3Drpc.App/Platforms/MacCatalyst/Info.plist
last-reviewed: 2026-11-13
---

# Build pitfalls

Known traps when building and editing the app, each with its fix.
Setup problems are in [set up a development machine](setup.md#if-it-fails).

## If it fails

| Symptom | Cause | Fix |
|---|---|---|
| `MAUIR0001`, a locked icon file | the app or another build runs | Stop the app and build again. |
| The macOS `.app` of a publish is about 100 MB, with `libcoreclr.dylib` and a 66 MB `TF3-DRPC.r2r.dylib` in `Contents/MonoBundle` | a `dotnet build -c Release` put the CoreCLR app into `artifacts/bin/Tf3Drpc.App/release_maccatalyst-arm64/`, and the publish keeps those files | Delete `release_maccatalyst-arm64` under `artifacts/bin`, `artifacts/obj` and `artifacts/publish` for `Tf3Drpc.App` before publishing, as `tools/build-release.ps1` does. |
| An old `.pdb` in the publish output | `DebugType` changed, and `artifacts/` keeps the old symbols | Delete `artifacts/bin/Tf3Drpc.App` and `artifacts/obj/Tf3Drpc.App` once. |
| The built `Contents/Info.plist` keeps old values (seen with `UIDeviceFamily`) | an incremental build keeps the cached manifest | After changing `Platforms/MacCatalyst/Info.plist`, delete `artifacts/obj/Tf3Drpc.App/<config>_maccatalyst-arm64/*AppManifest*`, build, and check the built `Contents/Info.plist`. |
| A translation works in `dotnet test` but not in the published app | ILC compiles only the satellite assemblies of NuGet packages | The app project's `AddReferenceSatellitesToIlc` target adds those of project references; check a new translation in the published exe ([Native AOT](../30-conventions/native-aot.md#translations)). |
| A Windows project outside `companion/` doesn't restore on macOS | `EnableWindowsTargeting` in `companion/Directory.Build.props` reaches only projects under `companion/` | Set `EnableWindowsTargeting` in that project itself. |
| Touched files have LF line endings | `dotnet format` and `sed -i` in Git Bash write LF | Convert the touched files back to CRLF. |
| An invisible byte order mark in C# source, or a real line break where `\n` was meant | escapes written through a shell heredoc or some edit tools turn into the real character | After such edits, check the touched files for control characters, U+FEFF after the first byte and zero-width characters; PowerShell single-quoted strings keep escapes literal. |

## Check for unused usings

Unused usings (IDE0005) don't fail the build, and `dotnet format --verify-no-changes` misses them.

1. Add a temporary `companion/.globalconfig`:

   ```ini
   is_global = true
   dotnet_diagnostic.IDE0005.severity = warning
   ```

2. Build Debug and Release; each IDE0005 warning is an unused using.
3. Delete `companion/.globalconfig` again.
