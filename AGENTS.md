# Repository Guidelines

TF3-DRPC shows a Transport Fever 3 game in the player's Discord status.
It has two parts: a game mod that writes the game state to the game log, and a portable companion app for Windows and macOS that reads the log and talks to Discord.

This file is the entry point for coding agents: the rules that hold for every change, and where to read before you touch a part.
[CLAUDE.md](CLAUDE.md) only points here.
When the code and a document disagree, inspect the code and correct the document.

## Every change

- Read the rows of [Before you change](#before-you-change) that match the files you touch, before you edit them.
- Pass the [docs gate](docs/00-meta/documentation-guide.md#docs-gate) before every commit: it ends when `pwsh ./tools/docs/Test-Docs.ps1 -Base origin/main` reports no errors.
- A change adds no build, analyser, trim or AOT warning.
- The app and `Tf3Drpc.Core` follow the [Native AOT rules](docs/30-conventions/native-aot.md): loops in place of LINQ, compile-time metadata in place of reflection, source generators for JSON, logging and regular expressions.
- The app writes only into its `DATA/` folder ([portable app](docs/30-conventions/portable-app.md)).
- The mod reads every game value inside its own `pcall` and calls only the documented TF3 API ([Lua style](docs/30-conventions/lua.md)).
- Everything in the repository is public: placeholders for local paths, no personal or machine details ([SECURITY.md](SECURITY.md#keeping-personal-data-out)).
- Prose, documents and commit messages are in British English ([written content](docs/30-conventions/writing.md)).
- Commits and pull requests carry no AI attribution: no `Co-Authored-By` trailer for an AI model and no "Generated with" line ([commits and pull requests](docs/20-development/git-workflow.md)).
- The commit or pull request names what wasn't verified, for example the other platform.

## Before you change

| Files | Read first |
|---|---|
| `companion/**/*.cs` | [C# style](docs/30-conventions/csharp.md), [Native AOT](docs/30-conventions/native-aot.md), [architecture patterns](docs/30-conventions/architecture-patterns.md), [companion architecture](docs/40-reference/companion/architecture.md) |
| `companion/**/*.xaml` | [architecture patterns](docs/30-conventions/architecture-patterns.md#xaml-first), [settings window](docs/40-reference/companion/settings-window.md) |
| `companion/**/*.resx` | [written content](docs/30-conventions/writing.md), [texts and languages](docs/40-reference/presence/texts-and-languages.md) |
| `companion/Tf3Drpc.Core/Presence/**`, `companion/Tf3Drpc.Core/Settings/**` | [presets and templates](docs/40-reference/presence/presets-and-templates.md), [settings file](docs/40-reference/companion/settings-file.md) |
| `companion/Tf3Drpc.Core/GameLog/**`, `companion/Tf3Drpc.Core/State/**` | [the game log](docs/40-reference/game-log.md), [game states](docs/40-reference/companion/states.md) |
| `companion/Tf3Drpc.Core/Discord/**` | [Discord connection](docs/40-reference/companion/discord-connection.md) |
| `companion/Tf3Drpc.App/Platforms/**` | [companion app platform](docs/40-reference/companion/platform.md), [Windows and macOS parity](docs/40-reference/platform-parity.md), [single instance](docs/40-reference/companion/single-instance.md) |
| project and build files in `companion/` | [build and publish](docs/20-development/build-and-publish.md), [build pitfalls](docs/20-development/build-pitfalls.md), [projects and build setup](docs/40-reference/companion/projects-and-build.md) |
| `companion/Tf3Drpc.Core.Tests/**` | [testing strategy](docs/50-testing/strategy.md) |
| `mod/**` | [Lua style](docs/30-conventions/lua.md), [the mod](docs/40-reference/mod/mod.md), the `tf3-modding` skill |
| the status line, on either side | [the status line](docs/40-reference/mod/status-line.md) (raise `v` when a key changes meaning) |
| `.github/workflows/**`, `tools/build-release.ps1` | [CI and releases](docs/40-reference/ci-and-release.md) |
| `tools/test-run/**` | [scripted test runs](docs/50-testing/scripted-runs.md), [calibrate the test-run scripts](docs/50-testing/calibration.md) |
| any Markdown file | [documentation guide](docs/00-meta/documentation-guide.md) |

## Commands

From `companion/`, where `global.json` and the solution are:

```sh
dotnet build TF3-DRPC.slnx
dotnet test TF3-DRPC.slnx
dotnet publish Tf3Drpc.App/Tf3Drpc.App.csproj -c Release -r win-x64   # -r maccatalyst-arm64 on macOS
```

From the repository root: `pwsh ./tools/install-mod.ps1` links the mod into the game, `pwsh ./tools/build-release.ps1` builds the release zip, and `pwsh ./tools/docs/Test-Docs.ps1` checks the documents.
Details and traps: [build and publish](docs/20-development/build-and-publish.md), [build pitfalls](docs/20-development/build-pitfalls.md), [set up a Mac](docs/20-development/setup-macos.md).

## Where things are

- [docs/README.md](docs/README.md): every document.
- [docs/10-overview/repository-layout.md](docs/10-overview/repository-layout.md): what each folder is for.
- [docs/10-overview/status.md](docs/10-overview/status.md): the project's state, known limits and to-dos.
- [docs/20-development/git-workflow.md](docs/20-development/git-workflow.md): commits, pull requests and `gh stack`.
