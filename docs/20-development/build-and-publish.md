---
title: Build and publish
type: guide
audience: [contributor, agent]
area: [app, tools]
covers:
  - companion/TF3-DRPC.slnx
  - companion/Directory.Build.props
  - companion/Tf3Drpc.App/Tf3Drpc.App.csproj
  - tools/build-release.ps1
last-reviewed: 2026-11-13
---

# Build and publish

How to build, test and publish the companion app, and build a release zip.
Everything for the app runs from `companion/`, where `global.json` and the solution are.

## Before you start

The machine is set up as in [set up a development machine](setup.md).

## Steps

1. Build and test:

   ```sh
   dotnet build TF3-DRPC.slnx
   dotnet build TF3-DRPC.slnx -c Release
   dotnet test TF3-DRPC.slnx
   ```

   `dotnet build` validates; the build reports no new warning.
2. Publish the app with Native AOT for the platform you are on:

   ```sh
   dotnet publish Tf3Drpc.App/Tf3Drpc.App.csproj -c Release -r win-x64            # Windows
   dotnet publish Tf3Drpc.App/Tf3Drpc.App.csproj -c Release -r maccatalyst-arm64  # macOS
   ```

   Only the publish shows every trim and AOT warning, each on its own; it adds none ([Native AOT](../30-conventions/native-aot.md#warnings)).
   The Windows output is in `artifacts/publish/Tf3Drpc.App/release_win-x64/`; the macOS `.app` is in `artifacts/bin/Tf3Drpc.App/release_maccatalyst-arm64/`, and a `.pkg` in `artifacts/publish/`.
3. To start the published app, copy it outside the repository first, because the app writes its `DATA` folder next to itself.
4. To build the release zip of the platform you are on (tests, Native AOT publish, zip into `artifacts/`), run from the repository root:

   ```sh
   pwsh ./tools/build-release.ps1              # -SkipTests right after a full test run
   ```

   CI runs the same script ([CI and releases](../40-reference/ci-and-release.md)).

## Check

A change to the app builds in Debug and Release, passes the tests and the analysers, and publishes without a new warning; what else it needs is in the [testing strategy](../50-testing/strategy.md).

## If it fails

The known traps of the build are in [build pitfalls](build-pitfalls.md).
