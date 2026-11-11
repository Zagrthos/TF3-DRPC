---
title: Testing strategy
type: reference
audience: [contributor, agent]
area: [app, mod]
covers:
  - companion/Tf3Drpc.Core.Tests/Tf3Drpc.Core.Tests.csproj
  - docs/40-reference/log-samples/**
last-reviewed: 2026-11-11
---

# Testing strategy

What is tested automatically, what needs a person, and what each kind of change is validated with.

## Automated tests

- `Tf3Drpc.Core.Tests` uses xunit v3 on Microsoft.Testing.Platform and covers `Tf3Drpc.Core`, including the monitor's tick (`GameMonitor`).
- It sees the internal types of `Tf3Drpc.Core` through `InternalsVisibleTo`; the test classes stay public, as xunit needs.
- The tests can't reference the MAUI app, so logic that can be tested lives in `Tf3Drpc.Core`; only the timer loop and the messenger stay in the app.
- Parser tests read the real log lines in [docs/40-reference/log-samples/](../40-reference/log-samples/); `.gitattributes` keeps those files byte for byte, and they change only by copying real log output.
- A test that relies on Windows behaviour (`FileShare` locks, named pipes) starts with `Assert.SkipUnless(OperatingSystem.IsWindows(), "…")`.
- The live Discord test sets a test presence for 5 seconds; it runs only on request, from `companion/`:

  ```sh
  TF3DRPC_DISCORD_LIVE=1 dotnet test TF3-DRPC.slnx -- --filter-method '*LiveDiscord*'
  ```

  In PowerShell, set `$env:TF3DRPC_DISCORD_LIVE = '1'` first.

## What needs a person

- The mod has no automated tests: it is tested in the game.
- The game, Discord and the MAUI app's timer loop and UI need a run with the game and the Discord desktop app: [test by hand on Windows](manual-test.md) and [on macOS](manual-test-macos.md), or driven by script ([scripted test runs](scripted-runs.md)).

## Validation per change

| Change | Validation |
|---|---|
| The companion app | a build in Debug and Release, the tests, the analyser output and a publish, all without a new warning ([build and publish](../20-development/build-and-publish.md)); for behaviour, a run with the game and Discord |
| The mod | a run in the game, with the Lua language server reporting no problems ([work on the mod](../20-development/mod-development.md)) |
| Documents | the [docs gate](../00-meta/documentation-guide.md#docs-gate) |

The commit or pull request names what wasn't verified, for example the other platform or a game mode.
