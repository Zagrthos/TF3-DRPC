---
title: 0025 The game build shows in the settings window only, untranslated
type: decision
audience: [contributor, maintainer, agent]
area: [app, discord]
status: accepted
decided: 2026-10-24
last-reviewed: 2026-11-10
---

# 0025 The game build shows in the settings window only, untranslated

## Context

The build number helps with bug reports and costs no new read: the app already follows the log, which names the build at every start.
The game itself shows `"Build <number>"` untranslated in every language.
Discord doesn't get it, because it means nothing to others.

## Decision

The game build shows in the settings window only, untranslated, as `Transport Fever 3 · Build <number>`.
