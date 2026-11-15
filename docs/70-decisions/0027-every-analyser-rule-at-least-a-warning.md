---
title: 0027 Every analyser rule that is on reports at least a warning
type: decision
audience: [contributor, maintainer, agent]
area: [app]
status: accepted
decided: 2026-11-13
last-reviewed: 2026-11-13
---

# 0027 Every analyser rule that is on reports at least a warning

## Context

Rules that report as info or hidden show only in the editor, as suggestions or quick fixes, so a command-line build or CI never sees them.
The shared `.editorconfig` can't change in this repository, and `AnalysisLevel` `latest-all` ignores bulk severity entries such as `dotnet_analyzer_diagnostic.severity`.

## Decision

`companion/Analyzers.globalconfig` raises every analyser rule that is on by default and reports below a warning to a warning, one `dotnet_diagnostic` entry per rule.
Rules that the shared `.editorconfig` configures keep that setting, unless `companion/.editorconfig` sets them again ([decision 0028](0028-dotnet-analysers-are-the-baseline.md)).
MA0127 stays silent, with its reason in the file: a string pattern compiles to the same ordinal comparison as `string.Equals`, and the C# style rule prefers patterns.

## Consequences

- The build shows what the editor showed, and unused usings (IDE0005) are warnings in every build.
- CI builds with `-warnaserror`, so a warning fails the pull request instead of showing only as an annotation.
  The advisory warnings stay warnings there, NuGet audit (NU1901–NU1904) and the SDK check (NETSDK1238, NETSDK1239): a new advisory for a package, transitive ones included, or for the pinned SDK would otherwise fail every pull request without a code change.
- A new analyser version can bring new rules; they report below a warning until they are added to the file.

## Alternatives considered

- Change the shared `.editorconfig`: it is maintained upstream for several repositories.
- A bulk entry for all rules: `AnalysisLevel` `latest-all` ignores it.
