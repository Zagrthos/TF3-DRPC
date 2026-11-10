---
title: Repository layout
type: reference
audience: [contributor, maintainer, agent]
area: [docs, tools]
covers:
  - .gitignore
  - companion/*.slnx
  - tools/*.ps1
last-reviewed: 2026-11-11
---

# Repository layout

What each top-level folder and file is for.

| Path | Content |
|---|---|
| [companion/](../../companion/) | the companion app: `Tf3Drpc.Core` (logic, no UI), `Tf3Drpc.App` (the MAUI app), `Tf3Drpc.Core.Tests`; the solution `TF3-DRPC.slnx`, `global.json` and the shared build files |
| [mod/zagrthos_drpc/](../../mod/zagrthos_drpc/) | the game mod (Lua); `_metadata/0.png` and the following images are its Mod Hub cover and gallery, 1920 × 1080 |
| [docs/](../README.md) | the documentation, in numbered folders ([documentation guide](../00-meta/documentation-guide.md#files-and-folders)), and the game log samples in `docs/log-samples/` |
| [tools/](../../tools/) | `install-mod.ps1` (links the mod into the game), `build-release.ps1` (the release zip), `new-third-party-notices.ps1` with `licences/` (the zips' notices), `docs/` (the documentation validator) and `test-run/` (scripts that drive the game and the app in a test run) |
| [reviews/](../../reviews/README.md) | dated code reviews with their reproduction scripts and recorded evidence |
| [assets/discord/](../../assets/discord/) | the images of the Discord application; `cover.png` is also the source of the app icon |
| [.github/](../../.github/) | the issue forms and chooser, the pull request template, and the workflows ([CI and releases](../40-reference/ci-and-release.md)) |
| [.claude/skills/tf3-modding/](../../.claude/skills/tf3-modding/) | an offline copy of the TF3 modding reference for coding agents |
| [README.md](../../README.md) | the overview for players |
| [CONTRIBUTING.md](../../CONTRIBUTING.md) | the entry point for contributors |
| [AGENTS.md](../../AGENTS.md) | the entry point for coding agents; [CLAUDE.md](../../CLAUDE.md) points to it |
| [SECURITY.md](../../SECURITY.md) | reporting a vulnerability, and keeping personal data out of the repository |
| [CHANGELOG.md](../../CHANGELOG.md) | what each release brings; also the text of the GitHub release and the Mod Hub upload |
| [LICENSE](../../LICENSE) | the MIT licence |

Key configuration: [.editorconfig](../../.editorconfig) (shared between repositories; it changes only upstream), [.gitattributes](../../.gitattributes) and [.luarc.json](../../.luarc.json) (the Lua language server).
`.gitignore` is the Visual Studio template plus `DATA/`, `*.zip` and local editor and system files (`.vscode/`, `.DS_Store`); the log samples are excepted from its `*.log` rule.
