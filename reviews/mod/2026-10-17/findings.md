# Full mod review — 2026-10-17

Reviewed: the mod as of 2026-10-17.
Scope: every file in `mod/`, its status-line contract, the installed game's definitions
and relevant base-game implementations, and the companion's consumption of emitted lines.
Production files were not changed.

**One P3 resilience finding. No ordinary-gameplay defect was reproduced with inputs that
follow the current game's API contracts.** Three additional deliberately invalid-input
behaviours were tested and recorded below, with reasons for not counting them as current
defects. These conclusions do not establish behaviour in a live game or on macOS.

## R1 — P3: One throwing calendar getter prevents later, readable date fields from being collected

**Location:** `mod/zagrthos_drpc/content/drpc/drpc.script.lua:140–145`, especially
`:142–144`. The helper at `:89–94` catches the exception around all three assignments.

**Trigger:** `getCalendarDate` successfully returns a record, but reading one property
raises an error. The reproduction models a failing native property with a Lua `__index`
getter. The other properties remain readable. It does not make `getCalendarDate` itself
fail and does not replace or instrument the production chunk.

**Expected:** read the calendar record once, then isolate its fields. A failing `year`
getter should omit `year`, while the available `month = 9` and `day = 30` survive. This
follows the repository's explicit rule that each game value has its own `pcall`, so a
changed API only drops dependent values.

**Observed:** all three fields share one `read(status, "date", ...)` call. The first
exception exits that whole function; assignments later in it never run:

| Injected exception | Omitted fields | Still available at the source |
|---|---|---|
| `date.year` | `year`, `month`, `day` | `month = 9`, `day = 30` |
| `date.month` | `month`, `day` | `year = 1905`, `day = 30` |
| `date.day` | `day` | `year = 1905`, `month = 9` |

Other status fields survive, one warning is logged, subsequent warnings are deduplicated,
and the date recovers when the injected fault is removed. Simply returning `nil` for the
year does **not** reproduce the issue: the month and day then survive. An exception is
required.

**Meaningful effect:** the existing custom-template placeholder `{month}` only needs the
month (`companion/Tf3Drpc.Core/Utilities/PresenceValueUtils.cs:64–65`). With a throwing year
getter, the mod unnecessarily removes that otherwise usable value. The unchanged
companion accepts the resulting status, but its actual template renderer returns no text
for `{month}`. As a control, restoring just the still-readable month/day in the emitted
JSON, without adding a year, makes the same renderer return `September`.

**Reproduction:**

```powershell
py -3 reviews/mod/2026-10-17/review.py --case R1
```

The full suite exports `calendar_year`, `calendar_month`, `calendar_day`,
`calendar_year_retained_month_control` and `calendar_year_nil_control` messages. The
[receiver harness](receiver/Program.cs) passes those actual outputs to the production
classifier and template renderer. See the [reproduction instructions](README.md).

**Evidence:** `R1` in [results-windows.json](results-windows.json) and
[results-linux.json](results-linux.json); the calendar records in
[receiver-results.json](receiver-results.json); exact emitted messages in
[messages.json](messages.json).

**Correction direction:** obtain the date through one `readValue`, then obtain its
`year`, `month` and `day` through separate protected reads, as the mod already does for
transported statistics and company values. Keep failure of the calendar conversion
itself as a shared failure affecting all three fields.

**Severity and reachability:** low priority, conditional on a field getter raising after
an API change or a failing native binding. The current installed definitions expose all
three date fields; this review did not observe one failing in the real game. This is an
incomplete implementation of the promised fault isolation, not a claim that normal dates
currently fail.

## Tested boundary behaviours not counted as current defects

These are executable observations, not untested speculation. Each requires an API result
that violates the installed game's declared type or structure. No such result was found
in the installed implementations or the real log samples. They are recorded for future
hardening, without inflating the finding count.

### O1: A value with an incompatible scalar type can invalidate the whole status for the companion

**Locations:** `drpc.script.lua:64–75` serialises any supported Lua scalar according to its
runtime type; `:163` supplies `rankName`, `:187` supplies company fields, and `:194–197`
supplies configuration values. The receiver deserialises the fixed schema at
`companion/Tf3Drpc.Core/GameLog/GameLogEventClassifier.cs:109–115` and drops a status on a
`JsonException`.

**Test `O1`:** return `rankName = 3`, `company_roadVehicles = "20"`, `climate = true`,
`speedup = false`, or `cloudCoverage = "0.5"`, one at a time. Each case produces
syntactically valid JSON, emits it repeatedly without a mod warning, and retains the
otherwise healthy rank/date in the JSON. The production companion rejects all five
messages. Removing only the incompatible key makes every control message acceptable.

This demonstrates that `pcall` handles thrown getters, but does not validate return
types. Per-key type validation would harden the producer against this. It is not counted
as a current bug because all five injected values violate their declared API contracts;
the actual game and sample inputs inspected here provide the expected types. The
companion's rejection of a malformed schema is also intentional defensive behaviour.

**Reproduce:** `py -3 reviews/mod/2026-10-17/review.py --case O1`, then run the receiver
against the generated messages. Both Lua platforms reproduce the emission; all ten
rejection/omitted-key controls were checked with the production receiver.

### O2: A NaN clock value can permanently stop subsequent status writes

**Location:** `drpc.script.lua:221–228` checks the clock against `nil`, then stores it in
`lastWriteSec` and uses numeric comparisons to decide whether to write again.

**Test `O2`:** return NaN on the initial tick, then return ordinary finite POSIX times up
to a day later. One initial line is written, without its non-finite timestamp; no later
line is written and no warning is logged. Once `lastWriteSec` is NaN, the comparisons
against it are all false. An exception from the clock, by contrast, is contained and
recovers correctly (`C04`).

A finite-number check before storing the scheduling baseline would prevent this.
However, `api/tealdef/api/util.d.tl:151` declares `getLocalTime()` as returning an integer.
NaN cannot satisfy that contract; there is no evidence that the real binding can return
it. This is therefore recorded as an invalid-input boundary, not a current clock defect.

**Reproduce:** `py -3 reviews/mod/2026-10-17/review.py --case O2`. Reproduced on both Lua
platforms.

### O3: A malformed, unrelated configuration pair discards valid climate/economy values

**Location:** `drpc.script.lua:111–116`, called under one protected read at `:194`.

**Test `O3`:** append `{nil, "unused"}` after valid climate and economy pairs. The table
assignment at `:114` raises `table index is nil`, so the conversion returns no table and
both valid values are omitted. The rest of the status remains available. Iterating only
the required pairs, or protecting pair conversion individually, would retain them.

The installed base-game hooks type configuration entries as `{string, string}` pairs.
The injected nil key violates that structure. No malformed entry was found in real input,
so this is not counted as a current defect. The engine definition's map-shaped
`getConfigDict` annotation is inconsistent with its actual pair-list usage; the mod's
pair iteration agrees with the game's own hooks and the real log samples.

**Reproduce:** `py -3 reviews/mod/2026-10-17/review.py --case O3`. Reproduced on both Lua
platforms.

## Coverage and conclusions by area

| Area | Checks performed | Result |
|---|---|---|
| All four mod files | Full source inspection, syntax, encoding/BOM, naming, registration path, metadata limits | No additional finding |
| TF3 APIs and fields | 52 static checks against the installed game, including all 24 copied company fields, expected resource paths, signatures and mode implementations | Matches the installed game |
| Baseline status | Compare every one of the 46 emitted fields to the first real log sample; check key ordering | Exact match |
| JSON strings | 1,005 cases per platform, including every ASCII byte, NUL/control escapes, newlines, quotes, backslashes, non-ASCII text, emoji and combining characters | Round-trip; one physical log line |
| JSON numbers | 3,018 cases per platform, including 32-bit boundaries, exact integers below 2^53, subnormals and very large finite numbers | Valid JSON; exact integers or documented six-significant-digit tolerance |
| Non-finite status values | NaN and both infinities in five separate fields | Omitted without losing the rest of the status |
| Windows C locale | German numeric, ctype and all-category locale checks with non-ASCII save names and a three-billion balance | Passed on Windows; non-C locales unavailable on Linux |
| Scheduling and pausing | First frame, repeated frames, 10 s boundaries, forward/backward clock jumps, 10,000 fixed-time frames, 1x/2x/4x/0 speed and resume | Cadence maintained for valid clock values |
| Throwing clock | 1,000 calls, warning deduplication and recovery | No repeated frame error; resumes after recovery |
| Getter failure isolation | 56 API/property fault points, repeated failures and recovery; exact surviving-output assertions for individual company/transport fields and the rank-name helper | Remaining calendar issue isolated as R1 |
| Initial/missing data | Missing progression, account, time, transported statistics and company statistics; infinite money | Other independent data survives |
| Ranks and modes | All 15 earned ranks, level fallback, normal/sandbox/tycoon/editor flags; compare rank and mode logic with installed base scripts | No additional finding; mode tests are fixtures, not game sessions |
| Game mutations and globals | Read-only proxy records, traps on both game-script state parameters, unexpected-global check, source inspection for mutating calls | No mutation or extra global |
| Logger failure | Repeated failing status writes, one warning and later successful write | Contained and retried |
| Receiver impact | 19 exported messages through unchanged `Tf3Drpc.Core`, including controls | All assertions passed |
| Lua language server | LuaLS 3.19.2, repository configuration, Hint threshold, all production Lua files | Zero diagnostics |

The previous Windows integer-format, locale-sensitive string/number encoding, grouped
company-field reads and applied-versus-earned rank defects were not reproduced on this
checkout. Their current fixes were exercised by the controls above. The earlier review
reports are historical evidence; their finding assertions are not a current regression
suite and are not presented as new findings here.

## Limits

- The Lua tests execute the unchanged mod in Lupa's Lua 5.2 runtimes on Windows and Linux.
  They substitute the game API and logger. The installed game identifies its Lua as
  5.2.2; these tests do not execute that embedded runtime or its native bindings.
- Installed-game inspection is read-only static evidence, not an in-game test. The
  executable SHA-256 matches the build inspected by the earlier reviews:
  `74861ac43b041aebc5179154345b3cf1ec83154c8e6cc58e0d9e02ff5fa602e4`.
- No live TF3 or Discord session, save/load/removal run, minimised-window test, macOS run,
  console run or mod.io validator/publish was performed. The previously recorded live
  runs were read for context, not counted as tests performed by this review.
- Fault injection proves the stated effects under the stated conditions. It does not
  prove that a current native API produces those failures or invalid values.
- The receiver harness uses the real Core project and its source-generated JSON context;
  it does not exercise the MAUI app, its monitor loop, Discord IPC or a Native AOT publish.

Commands, runtime versions, hashes, recorded results and how to repeat the tests are in
[README.md](README.md), [validation.json](validation.json) and
[source-manifest.json](source-manifest.json).
