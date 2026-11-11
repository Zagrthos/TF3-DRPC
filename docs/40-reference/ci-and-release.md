---
title: CI and releases
type: reference
audience: [contributor, maintainer, agent]
area: [tools]
covers:
  - .github/workflows/**
  - .github/codeql/**
  - tools/build-release.ps1
  - tools/new-third-party-notices.ps1
last-reviewed: 2026-11-11
---

# CI and releases

The GitHub Actions workflows in `.github/workflows/` and the release they publish.
Every action is pinned to a commit hash.

## Workflows

| Workflow | Runs on | Does |
|---|---|---|
| `ci.yml` (CI) | every pull request; a push to `main` that changes what the build reads; `workflow_dispatch` | builds Debug and Release and runs the tests on Windows and macOS; a push to `main` also keeps both release zips as workflow artifacts for 14 days |
| `release.yml` (Release) | a pushed `v*` tag | checks the tag, builds and tests both zips, publishes an immutable GitHub release |
| `docs.yml` (Docs) | every pull request and push to `main`; `workflow_dispatch` | runs `tools/docs/Test-Docs.ps1`, with the docs gate against the base branch on a pull request ([documentation guide](../00-meta/documentation-guide.md#the-validator)) |
| `links.yml` (Links) | every Monday; `workflow_dispatch` | checks the external links of the Markdown files with lychee, leaving out the review records in `reviews/` |
| `codeql-<language>.yml` | per language: Actions, C# (on Windows and macOS), Swift | CodeQL analysis, configured by `.github/codeql/codeql-config.yml`, which leaves out `reviews/`; Lua and PowerShell have no CodeQL support |

## CI

- The paths the build reads are listed in `ci.yml` twice: under `push.paths`, and in its first step, which skips the rest of a pull request's job when none of them changed.
  The two lists stay in step.
- The `CI on main` ruleset requires the `Windows`, `macOS` and `Docs` checks before a pull request merges into `main`.
- Pull requests always start the CI jobs, because a workflow that a path filter never starts leaves its required checks pending.
- Both jobs call `tools/build-release.ps1`, the same script a developer runs by hand ([build and publish](../20-development/build-and-publish.md)).
- macOS runs on the `xcode-27` preview image, the only one with Xcode 27.2, which the .NET 11 Mac Catalyst SDK requires; a step selects it with `xcode-select`.
- Jobs that build nothing use the `ubuntu-slim` runner, which has PowerShell and the GitHub CLI.
- A full run takes about 6 minutes on Windows and 3 on macOS.

## Release

`release.yml` publishes the release for a `v<VersionPrefix>` tag, after these checks:

- the tag matches `VersionPrefix` in `companion/Directory.Build.props`, and the tagged commit is on `main`;
- [CHANGELOG.md](../../CHANGELOG.md) has a section for that version whose heading ends with the date (`YYYY-MM-DD`), which isn't empty and has no relative links, because the release page and mod.io can't resolve them;
- the version isn't released yet.

Then it builds and tests both zips, creates the release as a draft, compares the draft's files with the built ones, publishes it and verifies GitHub's attestation (`gh release verify`).
Everything goes through the `gh` CLI: GitHub's own release actions are archived, and third-party ones would get write access to the releases.

- Releases are immutable (Settings → General → Releases): once published, a release's tag and files can't change and its tag name can't be used again, so a fix needs a new version.
- A tag whose run failed before publishing can be moved, or its run re-run.
- A download is checked against the attestation with `gh release verify-asset <tag> <zip>`.
- The release has `TF3-DRPC-<version>-windows-x64.zip`, `TF3-DRPC-<version>-macos-arm64.zip` and `SHA256SUMS.txt`; each zip holds `LICENSE.txt` and `THIRD-PARTY-NOTICES.txt`, which `tools/new-third-party-notices.ps1` writes from the publish's restore output.
- The mod has no workflow: it is uploaded by hand from the game's Mod Hub, the only place that validates it and prepares it for consoles.
