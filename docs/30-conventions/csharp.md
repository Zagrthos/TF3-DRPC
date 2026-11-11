---
title: C# style
type: rules
audience: [contributor, agent]
area: [app]
covers:
  - .editorconfig
  - companion/Directory.Build.props
last-reviewed: 2026-11-10
---

# C# style

How C# in `companion/` is written.
The structure of the app is in [architecture patterns](architecture-patterns.md), the Native AOT limits in [Native AOT](native-aot.md).

## Tooling

- Follow [.editorconfig](../../.editorconfig) and the analysers (Meziantou, Roslynator, Sonar); they run in every build with code-style enforcement, strict compiler features and NuGet audit.
- Fix a finding, or suppress it narrowly with a real justification.
- A change adds no warning: `dotnet build` and `dotnet publish` stay as clean as before.
- [.editorconfig](../../.editorconfig) is shared between repositories, so it changes only upstream, never locally.

## Layout

- Four spaces, CRLF, UTF-8, a final newline, no trailing whitespace.
- File-scoped namespaces; using directives outside the namespace, `System` first.
- A single-line `if` without `else` and with one single-line statement of any kind (`return;`, a call, an assignment) has no braces; `if`/`else`, multi-line statements and loops keep them.
- In an expression-bodied member the `=>` starts the line after the member and is followed directly by the first part of the expression.
  Continuation lines are indented one level deeper than the arrow.
- The parentheses of a multi-line parameter or argument list stand on their own lines, aligned with the member.
- A primary constructor can't be formatted like that (Roslynator RCS0039), so a class with a multi-line constructor parameter list gets an explicit constructor, with a narrow `#pragma warning disable IDE0290` around it.
- Private fields form one block without blank lines.
- ViewModels and classes of more than about 150 lines group their members in `#region` / `#endregion` blocks, with a blank line after and before each.

## Language

- Modern C#: primary constructors, pattern matching, collection expressions, `extension` blocks; `LangVersion` is `latest`, not `preview`.
- Patterns for every comparison with a constant (literal, `const`, enum member, `null`), also combined and as property patterns: `is 0`, `is not null`, `is >= 1 and <= 9`, `is A or B`, `status is { Version: Const }`.
  Comparisons between two variables and against `static readonly` values (such as a `TimeSpan`) stay operators, because they can't be patterns.
- Explicit types everywhere, in place of `var`.
- Static lambdas wherever they capture nothing.
- A repeated expression or a constructed object is bound to a named local, not nested in a call.
- Nullable annotations stay correct; nullable warnings are errors.

## Types and names

- Types are `internal sealed` unless they must be public: in `Tf3Drpc.Core` only what the app uses, in the app only what generated code needs (the WinUI application class).
- Interfaces are internal too, except the six in `Tf3Drpc.Core/Monitor/`, which the app's services inherit (`GameMonitor` reaches the app through them).
- A class that only DI or a framework creates has `#pragma warning disable CA1812 // Avoid uninstantiated internal classes` and the matching `restore` around its declaration.
- A service assigns its primary-constructor parameters to `private readonly` fields and uses the fields.
- `_camelCase` for private and internal fields, `PascalCase` for types, members, static fields and constants.
- Shared limits and tuning values go into an `<Area>Constants` class in `Constants/`.
- Helper statics go into `Utilities/<Name>Utils.cs`.

## Comments

- Comments explain intent, constraints or workarounds; the code shows what it does.
- XML doc comments are a one-line `<summary>`, with `<remarks>` only for a real gotcha, and `<see cref="..."/>` for symbol references.
- The reason for a change goes into the commit message; source keeps no plan or task rationale.
