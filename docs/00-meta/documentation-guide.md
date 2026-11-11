---
title: Documentation guide
type: rules
audience: [contributor, maintainer, agent]
area: [docs]
covers:
  - tools/docs/**
  - docs/00-meta/templates/**
  - .github/workflows/docs.yml
  - .github/PULL_REQUEST_TEMPLATE.md
last-reviewed: 2026-11-11
---

# Documentation guide

How this repository is documented, for people and coding agents alike.
Every change follows the [docs gate](#docs-gate) before it is committed.
`tools/docs/Test-Docs.ps1` checks the rules marked _(checked)_; the others need a reviewer's eye.

## Docs gate

Run these steps for every change, before every commit.
The gate is done when the last step reports no errors.

1. Run `pwsh ./tools/docs/Test-Docs.ps1 -Base origin/main` from the repository root.
   It lists every document whose `covers` patterns match a changed file; changed documents under `docs/` with front matter list no others.
2. Open each listed document and bring it in line with the change:
   - **add** what the change introduces (a command, a setting, a state, a limit);
   - **update** every statement the change makes wrong;
   - **delete** what can never apply again (a removed feature, a finished to-do, a workaround that is gone).
3. Search all Markdown for every name the change adds, renames or removes (types, settings keys, commands, file paths, labels, log lines) with `git grep -n '<name>' -- '*.md'`, and fix each hit.
4. Update [status.md](../10-overview/status.md): delete finished items, add new limits and to-dos.
   A new decision gets a new [decision record](#decisions); a reversed one gets a new record that supersedes the old one.
5. A change players notice gets a line in [CHANGELOG.md](../../CHANGELOG.md), and [README.md](../../README.md) follows when what players see or do changes.
6. Set `last-reviewed` to today in every document you checked, also in those that needed no change: the date is the record that it was checked against this change _(checked; a date not older than the change counts: today while there are uncommitted edits, otherwise the date of its newest commit)_.
7. Run the script from step 1 again.
   The gate is done when it reports no errors.

## Where things go

| You want to write down… | Type | Place |
|---|---|---|
| How to do a task (set up, build, test, release) | `guide` | `docs/20-development/`, `docs/50-testing/` |
| A rule contributors must follow | `rules` | `docs/30-conventions/` |
| How a part works now | `reference` | `docs/40-reference/` |
| Why the project is shaped as it is, the big picture | `explanation` | `docs/10-overview/` |
| What was decided and why | `decision` | `docs/70-decisions/` |
| A maintainer procedure (release, dependency update) | `runbook` | `docs/60-runbooks/` |
| What happened or was found on a date (a test run, a release, a review) | `record` | `docs/90-records/`, `reviews/` |
| The project's state, limits, to-dos and plans | `explanation` | [status.md](../10-overview/status.md) |
| A term used across documents | glossary entry | [glossary.md](glossary.md) |
| What players need | — | [README.md](../../README.md) |
| What a release brings | — | [CHANGELOG.md](../../CHANGELOG.md) |

Each document has exactly one type, and its folder accepts only the types in the table _(checked)_.
`docs/00-meta/` holds this guide and the glossary, and `docs/50-testing/` takes guides and references.

### What each type contains

| Type | Contains | Leaves to other types |
|---|---|---|
| `guide` | ordered steps, copy-ready commands, the expected result of each step | reasons (explanation), history (record) |
| `rules` | short, testable rules, each with its reason | how-tos, history |
| `reference` | the current behaviour, in the present tense: tables, limits, formats | plans, test dates, history |
| `explanation` | the overview, diagrams, trade-offs, the project's status | step-by-step instructions |
| `decision` | context, the decision, its consequences | later changes, except its `status` |
| `runbook` | a maintainer's checklist, in order, with a check per step | design |
| `record` | evidence and results from one date | — |
| `index` | links with one line each | content |

Plans, open work and the words that announce them (`planned`, `not yet`, `TODO`, `will be`, `for now`) belong in [status.md](../10-overview/status.md); `reference`, `rules`, `guide` and `explanation` documents other than `status.md` state what is true today _(checked)_.

The date a behaviour was last confirmed in the game goes into the `verified` field, not into the text.

## Files and folders

- Folders under `docs/` carry a two-digit number that sets the reading order: `00-meta`, `10-overview`, `20-development`, `30-conventions`, `40-reference`, `50-testing`, `60-runbooks`, `70-decisions`, `90-records`.
  A number, once given, keeps its meaning; a new section takes a free number between two others.
- File names are lower-case kebab case with `.md` _(checked)_.
  Decisions are `NNNN-<slug>.md` with a unique four-digit number _(checked)_; records are `YYYY-MM-DD-<slug>.md` _(checked)_.
- `README.md` in a folder is that folder's index _(checked: it links every document in the folder)_.
- [docs/README.md](../README.md) links every document under `docs/`, directly or through the README of its folder _(checked)_.
- A file of more than about 300 lines, or one that covers two topics, is split.
- A moved or renamed file has every link to it fixed in the same change _(checked)_.

## Front matter

Every Markdown file under `docs/` starts with front matter _(checked)_.
The root documents (README, CONTRIBUTING, AGENTS, CLAUDE, CHANGELOG, SECURITY), the pull request template and the README files under `tools/` and `reviews/` have none, because GitHub would show it as a table on their page.

```yaml
---
title: Discord connection
type: reference
audience: [contributor, agent]
area: [app, discord]
covers:
  - companion/Tf3Drpc.Core/Discord/**
last-reviewed: 2026-11-10
verified:
  - windows 2026-10-21 build 40425
  - macos 2026-10-20 build 40423
---
```

| Key | Required | Value |
|---|---|---|
| `title` | yes | the document's H1, word for word |
| `type` | yes | `guide`, `rules`, `reference`, `explanation`, `decision`, `runbook`, `record` or `index` |
| `audience` | yes | one or more of `player`, `contributor`, `maintainer`, `agent` |
| `area` | yes | one or more of `mod`, `app`, `windows`, `macos`, `discord`, `translations`, `accessibility`, `docs`, `tools` (the `area-` labels) |
| `covers` | for `reference`, `rules`, `guide`, `runbook` outside `docs/00-meta/` | globs (`*`, `**`, `?`) of the files the document describes, relative to the repository root; each one matches at least one file _(checked)_, and the docs gate lists the document when a matching file changes ([choosing covers](#choosing-covers)) |
| `last-reviewed` | yes | `YYYY-MM-DD`, not in the future; changes in every commit that changes the file |
| `verified` | no | one item per platform: `<windows or macos> <YYYY-MM-DD>`, optionally ` build <game build>` |
| `status` | for `decision` | `proposed`, `accepted` or `superseded`; other types may say `deprecated` |
| `decided` | for `decision` | `YYYY-MM-DD` |
| `superseded-by` | when `status` is `superseded` | the number of the newer decision, such as `0007` |

The front matter uses a small part of YAML only: `key: value`, flow lists `[a, b]` and block lists of plain strings.
Values are plain text without quotes, and without `: ` or ` #` inside them, so a title uses a dash or a comma where a colon would go.
Unknown keys are errors _(checked)_.

### Choosing covers

`covers` names the files whose change can make the document wrong, and no more, so the docs gate lists a document only when it really needs a look.

- A `reference` document covers the code it describes: the folder of that part, or its main files.
- A `rules` document covers the files that set the rules up (`.editorconfig`, the build props, the registration file), not every file the rules apply to; a rule changes when its setup changes, not when one more file follows it.
- A `guide` or `runbook` covers the scripts, project files and workflows its steps use.
- A pattern like `companion/**/*.cs` lists the document on nearly every change, which turns the gate into busywork; name the files instead.

## Writing

- One H1 per file, the same text as `title`, and headings that go down one level at a time _(checked)_.
- A heading is a link target: a renamed heading has every link to it fixed in the same change _(checked)_.
- One sentence per line, and a line holds a whole sentence _(checked)_.
  A list item or table cell is one line.
  [CHANGELOG.md](../../CHANGELOG.md) keeps one line per paragraph and list item, because the GitHub release and the Mod Hub show every line break.
- British English in prose (colour, behaviour, licence), and code identifiers, keys and package names as they are spelled in code.
- "For example" in place of "e.g.", "that is" in place of "i.e."
- Code identifiers, paths, commands, keys and log lines in backticks.
  Every command can be pasted as it stands; the text says which folder it runs from.
- Relative links inside the repository; absolute links in [CHANGELOG.md](../../CHANGELOG.md) and in text that leaves GitHub (the Mod Hub description).
  A link points to the heading it means, not to "above" or "below".
- Each fact has one home.
  Elsewhere it gets one summary sentence at most and a link to its home.
- A document states what the repository can't show by itself: the reason for a choice, a convention, a trap.
  What a config file, a script's `-?` output or the folder layout already shows is linked, not copied.
- Rules say what to do; a prohibition comes with the thing to do instead.

## Privacy

Everything in the repository is public.

- Local paths use placeholders: `<repo>`, `<Steam>`, `<userdata>`, `<steam-account>` _(checked for user folders)_.
- Leave out user and computer names, account IDs, e-mail addresses, hardware, display size and scaling, time zones and installed software _(checked for e-mail addresses and Steam IDs)_.
- Screenshots have their metadata stripped and never show the Discord window.
- UI coordinates are in device-independent pixels.

## Decisions

A decision record is one file per decision in `docs/70-decisions/`, from the [template](templates/decision.md), listed in [its index](../70-decisions/README.md) _(checked)_.

- Number it with the next free number.
- After it is merged, only `status`, `superseded-by` and link targets change.
- A reversed decision keeps its file: it gets `status: superseded` and `superseded-by`, and the new record says what it replaces.

## Records

Records are history: test runs, releases and reviews.

- New records go into `docs/90-records/` from the [template](templates/record.md); reviews go into `reviews/<area>/<YYYY-MM-DD>/` from the [review template](templates/review/README.md).
- Once merged, a record's text stays as it was; only its link targets follow moved files _(checked)_.
  The one other correction allowed is removing personal data, with `-AllowRecordEdit` and a note in the pull request.
- A record is listed in its index: [docs/90-records/README.md](../90-records/README.md), or [reviews/README.md](../../reviews/README.md) for reviews _(checked)_.

## The validator

`pwsh ./tools/docs/Test-Docs.ps1` checks the rules marked _(checked)_ for the whole repository; `-Base <ref>` adds the [docs gate](#docs-gate) checks against that commit; `-Json` prints the findings as JSON for tools and agents.
Its settings (the allowed values, the folder rules, the exemptions) are in `tools/docs/docs-config.psd1`.
The `Docs` workflow runs it on every pull request and push to `main`, and the `CI on main` ruleset requires it to pass.
External links are checked once a week by the `Links` workflow, because a site that is down for a while must not fail a pull request.
