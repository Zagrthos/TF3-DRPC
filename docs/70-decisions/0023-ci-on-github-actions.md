---
title: 0023 CI on GitHub Actions with an immutable release built through the gh CLI
type: decision
audience: [contributor, maintainer, agent]
area: [tools]
status: accepted
decided: 2026-10-24
last-reviewed: 2026-11-10
---

# 0023 CI on GitHub Actions with an immutable release built through the gh CLI

## Context

Both workflows call the same `tools/build-release.ps1` that a developer runs by hand.
Releases are immutable: a published release's tag and files can't change, and the tag name can't be reused.
The workflow therefore builds a draft, compares its files, publishes it and verifies GitHub's attestation, all with the `gh` CLI.
GitHub's own release actions are archived, and third-party ones would get write access to the releases.
The `CI on main` ruleset requires the `Windows` and `macOS` checks, so their jobs must report on every pull request.
While the repository was private, each full run cost about 42 Actions minutes.
Only the game's Mod Hub validates the mod and prepares it for consoles.

## Decision

CI runs on GitHub Actions.
`ci.yml` builds and tests every pull request and every push to `main` that changes what the build reads (`companion/`, the log samples, the app icon, the release scripts, `LICENSE`) on Windows and macOS.
`release.yml` publishes an immutable release for a `v<VersionPrefix>` tag on `main`, with no pre-release tags.
The mod stays a manual upload.

## Consequences

- A pull request that changes only the docs, the mod or the test-run scripts skips the build after the first step, and such a push to `main` doesn't start one.
- The workflows, their settings and the release steps are in [CI and releases](../40-reference/ci-and-release.md).

## Alternatives considered

- GitHub's own release actions: they are archived.
- Third-party release actions: they would get write access to the releases.
- A workflow for the mod: only the game's Mod Hub validates it and prepares it for consoles.
