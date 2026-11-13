---
title: 0028 The .NET analysers are the baseline and win over extension analysers
type: decision
audience: [contributor, maintainer, agent]
area: [app]
status: accepted
decided: 2026-11-13
last-reviewed: 2026-11-13
---

# 0028 The .NET analysers are the baseline and win over extension analysers

## Context

The shared `.editorconfig` turned off some .NET rules (CA1515, CA1707, CA2025, CS1591) and kept a .NET rule (IDE0290) suppressed where a Roslynator formatting rule objected.
The .NET analysers ship with the framework and follow its guidance, while Meziantou, Roslynator, Sonar and xUnit add their own opinions on top.

## Decision

A .NET analyser rule (`CA`, `IDE`, `CS`) is respected: `companion/.editorconfig` turns on again a .NET rule that the shared file turns off, and when a .NET rule collides with an extension rule, the .NET rule stays and the code follows it.

## Consequences

- `companion/.editorconfig` turns on CA1515, CA1707, CA2025 and CS1591; its entries win over the shared file for the files under `companion/`.
- CA1515 also checks `Tf3Drpc.Core`, which is a library and outside the rule by default, so Core has no public types and the app reaches it through `InternalsVisibleTo`.
- Test methods are named without underscores, every public type and member has an XML doc comment, and switch statements end in a throwing `default:` (IDE0010) instead of following Roslynator RCS1069, RCS1070 and RCS1136.
- Classes with a multi-line parameter list use primary constructors (IDE0290) in the layout RCS0039 accepts.
- A .NET rule stays off only on request: CA2007, IDE0046, IDE0047 and IDE0058 remain off as the shared file sets them, and IDE0046 would nest conditional expressions.
- A suppression of a .NET rule needs a reason the compiler forces, such as CS0051 for the explicit constructor of `DiscordPresenceClient`.

## Alternatives considered

- Follow the shared `.editorconfig` alone: it turns off .NET rules for every repository it serves, including ones this project wants.
- Prefer the extension rule in a collision: it would put a third-party opinion above the framework's own guidance.
