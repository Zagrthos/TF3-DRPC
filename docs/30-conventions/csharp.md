---
title: C# style
type: rules
audience: [contributor, agent]
area: [app]
covers:
  - .editorconfig
  - companion/.editorconfig
  - companion/Analyzers.globalconfig
  - companion/Directory.Build.props
last-reviewed: 2026-11-13
---

# C# style

How C# in `companion/` is written.
The structure of the app is in [architecture patterns](architecture-patterns.md), the Native AOT limits in [Native AOT](native-aot.md).

## Tooling

- Follow [.editorconfig](../../.editorconfig) and the analysers (Meziantou, Roslynator, Sonar); they run in every build with code-style enforcement, strict compiler features and NuGet audit.
- Every analyser rule that is on reports at least a warning (`companion/Analyzers.globalconfig`, [decision 0027](../70-decisions/0027-every-analyser-rule-at-least-a-warning.md)), so what the editor suggests, the build reports too.
- The .NET analysers (`CA`, `IDE`, `CS`) are the baseline ([decision 0028](../70-decisions/0028-dotnet-analysers-are-the-baseline.md)): a .NET rule that the shared file turns off is turned on again in `companion/.editorconfig`, and when it collides with a rule of Meziantou, Roslynator, Sonar or xUnit, the .NET rule wins and the other one is turned off there.
- Fix a finding, or suppress it narrowly with a real justification.
- A change adds no warning: `dotnet build` and `dotnet publish` stay as clean as before.
- [.editorconfig](../../.editorconfig) is shared between repositories, so it changes only upstream, never locally; `companion/.editorconfig` holds the local entries, which win over it for the files under `companion/`.

## Layout

- Four spaces, CRLF, UTF-8, a final newline, no trailing whitespace.
- File-scoped namespaces; using directives outside the namespace, `System` first.
- A single-line `if` without `else` and with one single-line statement of any kind (`return;`, a call, an assignment) has no braces; `if`/`else`, multi-line statements and loops keep them.
- In an expression-bodied member the `=>` starts the line after the member and is followed directly by the first part of the expression.
  Continuation lines are indented one level deeper than the arrow.
- The parentheses of a multi-line parameter or argument list stand on their own lines, aligned with the member.
- A primary constructor with a multi-line parameter list is the exception, because Roslynator RCS0039 rejects a line break before the base list: the `(` stays on the line of the type name, `)` stands on its own line, and the base list follows on the next line.
- A public class keeps an explicit constructor (with a narrow `#pragma warning disable IDE0290` and the reason) when its constructor takes an internal type, because a primary constructor is public and gives CS0051.
- Private fields form one block without blank lines.
- ViewModels and classes of more than about 150 lines group their members in `#region` / `#endregion` blocks, with a blank line after and before each.

## Language

- Modern C#: primary constructors, pattern matching, collection expressions with `with(...)` arguments for a capacity or a comparer, `extension` blocks, `closed` hierarchies; `LangVersion` is `latest` (C# 15), not `preview`.
- Patterns for every comparison with a constant (literal, `const`, enum member, `null`), also combined and as property patterns: `is 0`, `is not null`, `is >= 1 and <= 9`, `is A or B`, `status is { Version: Const }`.
  A string pattern compiles to the same ordinal, null-safe comparison as `string.Equals(s, "x", StringComparison.Ordinal)`, so Meziantou MA0127 stays silent.
- A `switch` statement ends in a `default:` that throws `UnreachableException` (IDE0010); the monitor tick catches and logs it.
  Comparisons between two variables and against `static readonly` values (such as a `TimeSpan`) stay operators, because they can't be patterns.
- Explicit types everywhere, in place of `var`.
- Static lambdas wherever they capture nothing.
- A repeated expression or a constructed object is bound to a named local, not nested in a call.
- Nullable annotations stay correct; nullable warnings are errors.

## Types and names

- Types are `internal sealed` unless they must be public, which in the app is only what generated code needs (the WinUI application class); CA1515 checks this in the app and, through `dotnet_code_quality.CA1515.output_kind` in `companion/.editorconfig`, in `Tf3Drpc.Core` too.
- `Tf3Drpc.Core` has no public types: the app and the tests see its internal types through `InternalsVisibleTo`, including the six interfaces in `Tf3Drpc.Core/Monitor/` that the app's services inherit.
- A class that only DI or a framework creates has `#pragma warning disable CA1812 // Avoid uninstantiated internal classes` and the matching `restore` around its declaration.
- A service assigns its primary-constructor parameters to `private readonly` fields and uses the fields.
- `_camelCase` for private and internal fields, `PascalCase` for types, members, static fields and constants, without underscores (CA1707), test methods too: `WithALabelTheFrameCarriesTheStoreButton`.
- Shared limits and tuning values go into an `<Area>Constants` class in `Constants/`.
- Helper statics go into `Utilities/<Name>Utils.cs`.

## Comments

- Comments explain intent, constraints or workarounds; the code shows what it does.
- Every public type and member has an XML doc comment (CS1591), the test classes and methods too.
- XML doc comments are a one-sentence `<summary>` in the multi-line form (Roslynator RCS1253), with `<remarks>` only for a real gotcha, and `<see cref="..."/>` for symbol references.
- The reason for a change goes into the commit message; source keeps no plan or task rationale.
