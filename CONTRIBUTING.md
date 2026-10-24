# Contributing to TF3-DRPC

Thanks for helping. This guide covers how to report problems, set up the project, build and
test it, and send changes. What the project does for players is in the [README](README.md);
how it works inside is in [docs/](docs/README.md).

## Ways to help

- **Report a bug:** open an issue with your operating system, what you did, what you expected
  and what happened. Attach the app's log from `DATA/logs/` next to the app, and the lines
  from the game log that matter (see [docs/game-log.md](docs/game-log.md) for where it is).
  Remove anything personal from them first.
- **Check a translation:** the app's own texts in the 12 languages other than English were
  machine-translated and need a check by native speakers. They are in
  `companion/Tf3Drpc.Core/Resources/Texts.<language>.resx`.
- **Suggest a feature:** open an issue and describe what you'd like to see in your status.
- **Change code or docs:** for anything larger than a small fix, open an issue first, so we
  can agree on the approach.

Issues use forms for bug reports, feature requests and translations; pull requests start from
a template with the validation checklist.

### Labels

- **Kind:** `bug` (something doesn't work as documented) and `enhancement` (a new feature or
  improvement).
- **Area**, one or more `area-` labels for the part that is affected: `area-mod`, `area-app`,
  `area-windows`, `area-macos`, `area-discord`, `area-translations`, `area-accessibility`,
  `area-docs` and `area-tools`.
- **For contributors:** `good first issue` (a good start in the project) and `help wanted`
  (the maintainers would welcome help).

Duplicates and issues that won't be worked on are closed with GitHub's "Close as duplicate"
and "Close as not planned".

## What you need

| For | You need |
|---|---|
| The companion app | the .NET SDK and MAUI workload set pinned in [companion/global.json](companion/global.json) (.NET 11) |
| Building on Windows | Windows 10 version 2004 or later |
| Building on macOS | macOS 26 or later, Xcode 27, and the `maui-maccatalyst` workload |
| The mod | Transport Fever 3; optionally the [Lua language server](https://luals.github.io/) |
| The scripts in `tools/` | PowerShell 7 (`pwsh`) |
| Manual tests | Transport Fever 3 on Steam and the Discord desktop app |
| Scripted test runs | additionally WinApp CLI 0.7 on Windows |

A Mac builds only the macOS app and Windows only the Windows app. Say in your pull request
which platform you couldn't check. The CI builds and tests both platforms on every pull
request that changes the app or what its build reads; a game run with Discord still needs a
person.

## Repository layout

| Path | Content |
|---|---|
| [mod/zagrthos_drpc/](mod/zagrthos_drpc/) | the game mod (Lua) |
| [companion/](companion/) | the companion app: `Tf3Drpc.Core` (logic, no UI), `Tf3Drpc.App` (the MAUI app) and `Tf3Drpc.Core.Tests` |
| [docs/](docs/README.md) | design documents, decisions, status and real game log samples |
| [tools/](tools/) | `install-mod.ps1`, `build-release.ps1` (the release zip) and the scripts for test runs in [tools/test-run/](tools/test-run/README.md) |
| [reviews/](reviews/) | dated code reviews with their reproduction scripts and recorded evidence |
| [assets/discord/](assets/discord/) | the images of the Discord application |
| [CHANGELOG.md](CHANGELOG.md) | what each release brings; also the text of the GitHub release and the Mod Hub upload |
| [AGENTS.md](AGENTS.md) | the detailed rules for code, tests and commits |

## Build and run

Everything for the companion app runs from `companion/`, where `global.json` and the solution
are:

```sh
dotnet build TF3-DRPC.slnx
dotnet test TF3-DRPC.slnx
dotnet publish Tf3Drpc.App/Tf3Drpc.App.csproj -c Release -r win-x64            # Windows
dotnet publish Tf3Drpc.App/Tf3Drpc.App.csproj -c Release -r maccatalyst-arm64  # macOS
```

The publish is a Native AOT build. On Windows the output is in
`artifacts/publish/Tf3Drpc.App/release_win-x64/`; on macOS the `.app` is in
`artifacts/bin/Tf3Drpc.App/release_maccatalyst-arm64/`. Copy it outside the repository before
you start it, because the app writes its `DATA` folder next to itself.

To work on the mod, link it into the game once:

```sh
pwsh ./tools/install-mod.ps1           # -Remove unlinks it again
```

The game then loads the mod straight from your checkout. After a change, loading a savegame is
enough; you don't have to restart the game. Turn the mod on in the savegame as described in
the [README](README.md#2-turn-the-mod-on-for-your-game).

More commands and known pitfalls (line endings, locked files during a build, macOS specifics)
are in [AGENTS.md](AGENTS.md#quick-reference).

## Testing

- **Unit tests:** `dotnet test TF3-DRPC.slnx` covers `Tf3Drpc.Core`. Logic that can be tested
  belongs there, because the tests can't reference the MAUI app.
- **Live Discord test:** set `TF3DRPC_DISCORD_LIVE=1`, then run
  `dotnet test TF3-DRPC.slnx -- --filter-method '*LiveDiscord*'`. It shows a test status on
  your Discord profile for 5 seconds.
- **With the game:** the mod has no automated tests, and the app's behaviour needs the game
  and Discord. Follow [docs/testing.md](docs/testing.md); it lists every step and what
  Discord should show. [tools/test-run/](tools/test-run/README.md) can drive a run by script.

A change to the app must build in Debug and Release, pass the tests and the analysers, and
publish without a new warning. Say in your pull request what you tested with the game and
what you couldn't.

## Code style

The full rules are in [AGENTS.md](AGENTS.md#coding-style--naming-conventions). The most
important ones:

- [.editorconfig](.editorconfig) and the analysers run in every build; a change must not add
  a warning.
- The app is published with Native AOT: no LINQ, no reflection, source generators for JSON,
  logging and regular expressions. See
  [AGENTS.md](AGENTS.md#native-aot-rules).
- The app is portable and writes only to its `DATA` folder.
- The mod reads every game value inside its own `pcall` and uses only the official scripting
  API.
- Documentation and commit messages are in British English.

## Commits and pull requests

- Short, imperative commit subjects, for example `Fix the log truncation check`, and one topic
  per commit.
- A pull request explains the change in behaviour, lists what you tested and names what you
  couldn't (for example the other platform).
- Update the documentation in [docs/](docs/README.md) and [AGENTS.md](AGENTS.md) in the same
  pull request when your change makes them out of date.
- A change players notice gets a line in [CHANGELOG.md](CHANGELOG.md), in the section of the
  next release.
- The maintainers work with stacked pull requests on the `main` branch; see
  [AGENTS.md](AGENTS.md#commit--pull-request-guidelines).

## Keep personal data out

Everything in this repository is public. Before you commit logs, screenshots or other
evidence:

- Replace local paths with placeholders such as `<repo>`, `<Steam>` and `<steam-account>`.
- Leave out user and computer names, account IDs, hardware, display size and scaling, and
  time zones.
- Never include a screenshot of the Discord window; it shows content unrelated to the test.
- Never commit tokens or credentials. The Discord application ID isn't secret.

## Licence

By contributing, you agree that your contributions are released under the project's
[MIT Licence](LICENSE).
