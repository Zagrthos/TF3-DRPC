<!--
Thanks for contributing. Please read CONTRIBUTING.md first.
Everything here is public: keep personal details out of descriptions, logs and screenshots.
-->

## What changes

<!-- The change in behaviour, and why. Link the issue it fixes, for example "Fixes #12". -->

## Validation

<!-- Tick what you did and add details where useful. -->

- [ ] `dotnet build` in Debug and Release without new warnings
- [ ] `dotnet test`
- [ ] `dotnet publish` (Native AOT) without new warnings
- [ ] Tested with the game and Discord ([docs/50-testing/manual-test.md](https://github.com/Zagrthos/TF3-DRPC/blob/main/docs/50-testing/manual-test.md)): which steps
- [ ] Windows
- [ ] macOS

## Not verified

<!-- What you couldn't check, for example the other platform or a game mode. Write "Nothing" if you checked everything. -->

## Checklist

- [ ] The [docs gate](https://github.com/Zagrthos/TF3-DRPC/blob/main/docs/00-meta/documentation-guide.md#docs-gate) is done: every document it lists is updated (added, changed or removed content) and has a new `last-reviewed` date, and `pwsh ./tools/docs/Test-Docs.ps1 -Base origin/main` reports no errors.
- [ ] `docs/10-overview/status.md` is up to date, and a new or reversed decision has its record in `docs/70-decisions/`.
- [ ] A change players notice has a line in `CHANGELOG.md`, and `README.md` follows what players see or do.
- [ ] Logs, screenshots and evidence contain no personal or machine details.
- [ ] Documentation and commit messages are in British English.
