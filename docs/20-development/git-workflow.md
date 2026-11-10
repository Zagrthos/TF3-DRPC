---
title: Commits, pull requests and labels
type: guide
audience: [contributor, maintainer, agent]
area: [docs, tools]
covers:
  - .github/PULL_REQUEST_TEMPLATE.md
  - .github/ISSUE_TEMPLATE/**
  - CHANGELOG.md
last-reviewed: 2026-11-10
---

# Commits, pull requests and labels

How a change gets from a branch into `main`.

## Steps

1. Commit with a short, imperative subject, for example `Fix the log truncation check`, one topic per commit.
   Dependency commits use bracketed scopes: `[NuGet]`, `[.NET SDK]`.
   Commits and pull requests carry no AI attribution: no `Co-Authored-By` trailer for an AI model and no "Generated with" line.
2. Pass the [docs gate](../00-meta/documentation-guide.md#docs-gate) before each commit.
3. A change players notice gets a line in [CHANGELOG.md](../../CHANGELOG.md), in the section of the next release.
   Each paragraph and list item of a section stays on one line, because the GitHub release and the Mod Hub show every line break.
   A section that isn't released says "not released yet" in place of the date; `release.yml` refuses a tag until the heading ends with the date.
4. Open the pull request from the [template](../../.github/PULL_REQUEST_TEMPLATE.md): what changes and why, the validation performed, and what wasn't verified (for example the other platform).
5. Label it as described in [labels](#labels).

## Labels

Every issue and pull request gets one kind label and the `area-` labels of the parts it touches.

- **Kind:** `bug` (something doesn't work as documented) or `enhancement` (a new feature or an improvement).
- **Area:** `area-mod`, `area-app`, `area-windows`, `area-macos`, `area-discord`, `area-translations`, `area-accessibility`, `area-docs`, `area-tools`.
- **For contributors**, on issues only: `good first issue` and `help wanted`.
- Duplicates and issues that won't be worked on are closed with GitHub's close reasons (`Close as duplicate`, `Close as not planned`), not with labels.

| Change | Labels |
|---|---|
| The companion app on both platforms: settings, presets, templates, log reading, the monitor | kind, `area-app` |
| A refactor or new tests of the app without a behaviour change | `enhancement`, `area-app` |
| Only on Windows (tray, settings window, publish) or only on macOS (menu bar, Gatekeeper) | kind, `area-windows` or `area-macos`, plus `area-app` when shared code changes too |
| The Discord connection or what the status shows | kind, `area-discord` |
| The mod or the status line it writes | kind, `area-mod`, plus `area-app` when the app's parser changes with it |
| The app's own texts in the 13 languages | kind, `area-translations` |
| Screen readers, keyboard use, contrast | kind, `area-accessibility` |
| A dependency or SDK update (`[NuGet]`, `[.NET SDK]`) | `enhancement`, `area-app` |
| Documents only | `bug` for a wrong or outdated statement, otherwise `enhancement`; `area-docs` |
| Test-run scripts in `tools/` and recorded test runs | kind, `area-tools`, plus `area-docs` when the documents record the run |

Combine the rows a change spans.

## Stacked pull requests

The maintainers stack work with [`gh stack`](https://github.com/github/gh-stack): one branch and one pull request per topic, each based on the one below, with `main` as the trunk.

- Start a new stack from `main` with `gh stack init <branch>`.
- Commit a change on the layer that owns it (`gh stack checkout <branch>`), then `gh stack rebase --upstack` and `gh stack push --remote origin`.
- A new topic is a new layer on top: `gh stack add <branch>`, later `gh stack submit --auto --open --remote origin` for its pull request.
- Show the stack with `gh stack view --json`; the bare `gh stack view` opens an interactive screen.
- Merge with `gh stack merge <top PR> --yes`; it merges that pull request and everything below it.
- A pull request stacked on a branch that GitHub doesn't know as part of the stack merges into that branch, not into `main`: add it with `gh stack add`, or retarget it to `main` before merging.
