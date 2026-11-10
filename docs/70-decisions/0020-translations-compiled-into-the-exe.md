---
title: 0020 Translations compiled into the exe by an extra build target
type: decision
audience: [contributor, maintainer, agent]
area: [app, translations]
status: accepted
decided: 2026-10-24
last-reviewed: 2026-11-10
---

# 0020 Translations compiled into the exe by an extra build target

## Context

Native AOT can't load satellite assemblies, and ILC only takes those of NuGet packages.

## Decision

The translations are compiled into the exe by an extra build target.
