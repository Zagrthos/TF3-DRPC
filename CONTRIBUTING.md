# Contributing to TF3-DRPC

Thanks for helping.
What the project does for players is in the [README](README.md); everything about working on it is in [docs/](docs/README.md).

## Ways to help

- **Report a bug:** open an issue with your operating system, what you did, what you expected and what happened.
  Attach the app's log from `DATA/logs/` next to the app, and the lines from the game log that matter ([where the game log is](docs/40-reference/game-log.md)).
  Remove anything personal from them first.
- **Check a translation:** the app's own texts in the 12 languages other than English were machine-translated and need a check by native speakers.
  They are in `companion/Tf3Drpc.Core/Resources/Texts.<language>.resx`, and the translation issue form says what to send.
- **Suggest a feature:** open an issue and describe what you'd like to see in your status.
- **Change code or docs:** for anything larger than a small fix, open an issue first, so we can agree on the approach.
- **Report a vulnerability** privately, as [SECURITY.md](SECURITY.md) describes.

Issues use forms for bug reports, feature requests and translations; pull requests start from a template with the validation checklist.

## Getting started

1. [Set up your machine](docs/20-development/setup.md), and on a Mac also [the Mac steps](docs/20-development/setup-macos.md).
2. [Build and publish](docs/20-development/build-and-publish.md) the companion app, or [work on the mod](docs/20-development/mod-development.md).
3. Follow the rules for the part you change: [C#](docs/30-conventions/csharp.md), [Native AOT](docs/30-conventions/native-aot.md), [architecture patterns](docs/30-conventions/architecture-patterns.md), [Lua](docs/30-conventions/lua.md), [written content](docs/30-conventions/writing.md).
4. Test as the [testing strategy](docs/50-testing/strategy.md) says, and with the game and Discord as in [test by hand](docs/50-testing/manual-test.md).
5. Update the documentation with the [docs gate](docs/00-meta/documentation-guide.md#docs-gate).
6. Commit and open a pull request as in [commits and pull requests](docs/20-development/git-workflow.md).

A change to the app builds in Debug and Release, passes the tests and the analysers, and publishes without a new warning.
Say in your pull request what you tested with the game, and what you couldn't, for example the other platform.

## Keep personal data out

Everything in this repository is public: logs, screenshots and evidence follow [SECURITY.md](SECURITY.md#keeping-personal-data-out).

## Licence

By contributing, you agree that your contributions are released under the project's [MIT Licence](LICENSE).
