---
title: 0000 One documentation system for people and agents
type: decision
audience: [contributor, maintainer, agent]
area: [docs]
status: accepted
decided: 2026-11-10
last-reviewed: 2026-11-10
---

# 0000 One documentation system for people and agents

## Context

The documentation grew in a few large files that each did several jobs: `AGENTS.md` held commands, pitfalls, every coding rule and the contribution workflow, and `docs/README.md` held the overview, all decisions, the to-dos and a release diary.
The same facts were written in up to three places and drifted apart, reference documents carried dated test notes, and nothing checked the documents.
Coding agents loaded the whole of `AGENTS.md` for every task.

## Decision

The repository follows the [documentation guide](../00-meta/documentation-guide.md):

- every document has one type (guide, rules, reference, explanation, decision, runbook, record, index) and lives in the numbered folder for that type under `docs/`;
- every document under `docs/` has front matter, and its `covers` patterns tie it to the code it describes;
- one sentence per line;
- one file per decision in `docs/70-decisions/`;
- plans and open work live in `docs/10-overview/status.md`, as a list for now, each item becoming a GitHub issue later;
- every change passes the docs gate, and `tools/docs/Test-Docs.ps1` checks the rules in CI.

## Consequences

- Agents read `AGENTS.md`, a short routing table, and open only the documents its rows point to.
- A changed code path lists the documents to check, and the check fails until each one has a new `last-reviewed` date.
- Line-based diffs show exactly which sentence changed.
- Moving the existing documents takes several pull requests; until then they are listed as legacy in `tools/docs/docs-config.psd1`.
- Folder numbers and headings are link targets and stay stable.

## Alternatives considered

- Plain folder names: no visible reading order on GitHub or in a directory listing.
- Wrapped lines at about 95 columns: a one-word edit rewraps a paragraph, and diffs hide the change.
- Decisions grouped by area in a few files: an agent can't link or supersede one decision on its own.
- To-dos as GitHub issues right away: kept as a list first, so the restructure doesn't depend on issue triage.
