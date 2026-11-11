---
title: Commits and pull requests
type: guide
audience: [contributor, maintainer, agent]
area: [docs, tools]
covers:
  - .github/PULL_REQUEST_TEMPLATE.md
  - CHANGELOG.md
last-reviewed: 2026-11-11
---

# Commits and pull requests

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

## Stacked pull requests

The maintainers stack work with [`gh stack`](https://github.com/github/gh-stack): one branch and one pull request per topic, each based on the one below, with `main` as the trunk.

- Start a new stack from `main` with `gh stack init <branch>`.
- Commit a change on the layer that owns it (`gh stack checkout <branch>`), then `gh stack rebase --upstack` and `gh stack push --remote origin`.
- A new topic is a new layer on top: `gh stack add <branch>`, later `gh stack submit --auto --open --remote origin` for its pull request.
- Show the stack with `gh stack view --json`; the bare `gh stack view` opens an interactive screen.
- Merge with `gh stack merge <top PR> --yes`; it merges that pull request and everything below it.
- A pull request stacked on a branch that GitHub doesn't know as part of the stack merges into that branch, not into `main`: add it with `gh stack add`, or retarget it to `main` before merging.
