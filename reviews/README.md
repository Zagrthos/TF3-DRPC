# Reviews

Dated code reviews with their reproduction scripts and recorded evidence.
Each review is a record: its files stay as they were written ([documentation guide](../docs/00-meta/documentation-guide.md#records)).
A new review goes into `reviews/<area>/<YYYY-MM-DD>/` from the [review template](../docs/00-meta/templates/review/README.md) and gets a row here.
The `review.py` harnesses of the mod reviews assert the behaviour as it was before their fixes, so their defect checks (F1–F4, R1, O1–O3) fail by design after the fixes, while their controls must pass.

| Date | Area | Findings | Folder |
|---|---|---|---|
| 2026-10-15 | companion | F01–F13 | [companion/2026-10-15/](companion/2026-10-15/README.md) |
| 2026-10-16 | companion | N1–N2 | [companion/2026-10-16/](companion/2026-10-16/findings.md) |
| 2026-10-17 | companion | R01–R12 | [companion/2026-10-17/](companion/2026-10-17/README.md) |
| 2026-10-20 | companion | R01–R07 | [companion/2026-10-20/](companion/2026-10-20/findings.md) |
| 2026-10-22 | companion | R01–R15 | [companion/2026-10-22/](companion/2026-10-22/findings.md) |
| 2026-10-15 | mod | F1–F4 | [mod/2026-10-15/](mod/2026-10-15/README.md) |
| 2026-10-16 | mod | R1–R2 | [mod/2026-10-16/](mod/2026-10-16/findings.md) |
| 2026-10-17 | mod | R1, O1–O3 | [mod/2026-10-17/](mod/2026-10-17/README.md) |
