---
title: Discord Rich Presence for Transport Fever 3
type: index
audience: [player, contributor, maintainer, agent]
area: [docs]
last-reviewed: 2026-11-11
---

# Discord Rich Presence for Transport Fever 3

These documents describe the mod and the companion app that show a Transport Fever 3 game in Discord, and how to work on them.
How they are organised and kept current is in the [documentation guide](00-meta/documentation-guide.md).
Where the project stands, its known limits and its to-dos are in [status.md](10-overview/status.md).
What each folder and file of the repository is for is in [10-overview/repository-layout.md](10-overview/repository-layout.md).
What the project does for players is in the [README](../README.md); how to contribute is in [CONTRIBUTING.md](../CONTRIBUTING.md).
Coding agents start at [AGENTS.md](../AGENTS.md).

## Documents

### 00-meta

| Document | Content |
|---|---|
| [00-meta/documentation-guide.md](00-meta/documentation-guide.md) | how to document: types, places, front matter, writing rules, the docs gate |
| [00-meta/glossary.md](00-meta/glossary.md) | the terms used across the documents |

### 10-overview

| Document | Content |
|---|---|
| [10-overview/architecture.md](10-overview/architecture.md) | how the mod and the companion app work together, and the sources |
| [10-overview/status.md](10-overview/status.md) | the project's state, known limits and to-dos |
| [10-overview/repository-layout.md](10-overview/repository-layout.md) | what each folder and file of the repository is for |

### 20-development

| Document | Content |
|---|---|
| [20-development/setup.md](20-development/setup.md) | setting up a development machine |
| [20-development/setup-macos.md](20-development/setup-macos.md) | the extra setup steps on a Mac |
| [20-development/build-and-publish.md](20-development/build-and-publish.md) | building, testing and publishing the app, the release zip |
| [20-development/build-pitfalls.md](20-development/build-pitfalls.md) | known build traps and their fixes |
| [20-development/mod-development.md](20-development/mod-development.md) | running the mod from the checkout |
| [20-development/git-workflow.md](20-development/git-workflow.md) | commits, pull requests, stacked pull requests |

### 30-conventions

| Document | Content |
|---|---|
| [30-conventions/csharp.md](30-conventions/csharp.md) | C# style |
| [30-conventions/architecture-patterns.md](30-conventions/architecture-patterns.md) | the patterns of the companion app |
| [30-conventions/native-aot.md](30-conventions/native-aot.md) | the Native AOT rules |
| [30-conventions/portable-app.md](30-conventions/portable-app.md) | writing only into `DATA/` |
| [30-conventions/lua.md](30-conventions/lua.md) | Lua style for the mod |
| [30-conventions/writing.md](30-conventions/writing.md) | the language of documents, commits and the app's texts |

### 40-reference, 50-testing, 70-decisions and 90-records

| Document | Content |
|---|---|
| [40-reference/ci-and-release.md](40-reference/ci-and-release.md) | the workflows and the release |
| [50-testing/strategy.md](50-testing/strategy.md) | what is tested how, and the validation per change |
| [70-decisions/](70-decisions/README.md) | decision records, one file per decision |
| [90-records/](90-records/README.md) | dated records: test runs, releases |
| [90-records/2026-10-24-first-release.md](90-records/2026-10-24-first-release.md) | the first release: the steps and checks |

### Documents in the root of docs/

| Document | Content |
|---|---|
| [mod.md](mod.md) | the mod |
| [status-line.md](status-line.md) | the `[drpc-status]` line, the interface between mod and companion |
| [game-log.md](game-log.md) | finding and reading the game log, the lines that matter, samples |
| [companion.md](companion.md) | the companion app: platform, portability, projects, architecture, Discord connection, states, settings |
| [presence.md](presence.md) | what Discord shows, texts and languages |
| [testing.md](testing.md) | how to test with the game and Discord, where to look, known traps |
| [platform-parity.md](platform-parity.md) | what differs between Windows and macOS, and what is decided for each difference |
| [log-samples/](log-samples/) | real game log lines for the parser tests |
