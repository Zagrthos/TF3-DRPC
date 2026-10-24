# Review of mod/ of 2026-10-15

Four defects were reproduced. The production files were not modified. Runtime tests use
Lua 5.2.4 via Lupa 2.8 on Windows and Linux, with a controlled TF3 API fixture. The game
installation was inspected directly; it embeds Lua 5.2.2, and its Windows formatter was
verified to use the same signed 32-bit conversion as the Windows reproduction.

## F1 - P1: Large integer values stop all telemetry on Windows

Location: `mod/zagrthos_drpc/content/drpc/drpc.script.lua:60-63`; outer containment at 180-183.

The encoder treats every whole number below 2^53 as safe for `%d`. TF3's Windows Lua
formatter actually accepts a signed 32-bit value for that conversion. A balance, asset
value, debt or transport count outside -2147483648 through 2147483647 therefore throws
instead of being encoded. Because serialisation has one outer pcall, the entire status
line is lost. The warning is emitted once, and future lines remain absent while any
integer field exceeds the range. The companion consequently loses even unrelated
rank/date updates and may infer a pause from an old status.

Reproduction: `windows_integer_overflow()` in `review.py`, and F1 in `repro.lua`.
Use the normal fixture, set `BALANCE = 2147483648`, and invoke the unchanged `guiUpdate`.
Expected: a valid line containing balance 2147483648 and all other working values.
Observed: zero status lines and one `writing the status failed` warning containing
`not a number in proper range`. Repeat after 10 seconds: still no status line or new
warning. Also reproduced with assets 3000000000 and balance -2147483649; 2147483647
is a passing boundary control.

Independent game corroboration: `verify_game_formatter.py` maps the actual `%d` and `%i`
switch entries to `0x142fd924a`. At `0x142fd9255`, `vcvttsd2si %xmm0,%ebx` performs
the 32-bit conversion; the following range check points at the exact Lua error string.
The verification script and disassembly are included in the bundle.

Suggested correction: use a floating-point decimal format such as `%.0f` for exact
whole numbers below 2^53, with invariant decimal formatting for non-integers.

## F2 - P2: Numeric locale can make every status line invalid JSON

Location: `mod/zagrthos_drpc/content/drpc/drpc.script.lua:63`.

`string.format("%.6g", value)` uses the active C numeric locale. With a decimal-comma
locale, ordinary weather and vehicle-speed values are encoded with commas. JSON requires
a dot, so the entire line is invalid. This path logs no warning because formatting and
logging both succeed. `GameLogEventClassifier.ParseStatus` catches the JSON exception and
discards the status.

Reproduction: `numeric_locale()` in `review.py`, and F2 in `repro.lua`.
Call `os.setlocale("German_Germany.1252", "numeric")`, keep the normal fixture and invoke
`guiUpdate`. Expected: `"cloudCoverage":0.522476`. Observed:
`"cloudCoverage":0,522476`, a strict JSON parse failure, and zero warnings.
The C locale is a passing control.

This finding is conditional on the numeric locale being changed by the process or a
script. The tests do not establish that selecting German in TF3 changes it. The remedy
should normalise numeric output locally rather than change the game's process-wide locale.

## F3 - P2: Grouped reads discard independently available values after one failure

Locations: `mod/zagrthos_drpc/content/drpc/drpc.script.lua:118-174`.

Several independent reads share one pcall and run sequentially. An exception in the first
read prevents later, working reads from executing. This defeats the stated resilience
rule that an API change should lose only the affected value. It can remove the current
presence's map-editor, tycoon or rank information, and unnecessarily drops future
template statistics.

Reproduction: `grouped_reads()` in `review.py`; F3 in `repro.lua` is the minimal example.
Make only `getDefaultSavegameId()` throw, keep `isMapEditor()` working and returning true,
then invoke `guiUpdate`. Expected: saveName absent and isMapEditor true. Observed:
both keys absent, with the rest of the JSON valid.

All additional scenarios below were also executed and asserted on Windows and Linux:

| Failing operation | Independently working data lost | Lines |
|---|---|---|
| Save-name getter | isMapEditor | 119-120 |
| Base config getter | isTycoon from growth config | 123-125 |
| Rank-name helper | experience from already-read company data | 145-147 |
| Loading the rank-name module | rank and experience from progression module | 140-147 |
| Missing WORLD component | climate and economy from config dict | 169-173 |
| Calendar conversion | timeOfDaySec from already-read GAME_TIME | 128-133 |
| totalAssets native field getter | vehicle, line and station counts | 163-166 |

Suggested correction: protect independent getters and native field accesses separately;
read shared dependencies once, and keep failures of optional helpers from blocking
unrelated fields. These fault-injection tests establish the failure behaviour, not that
all seven failures currently occur in the installed game.

## F4 - P2: Locale-dependent control-character escaping corrupts UTF-8

Location: `mod/zagrthos_drpc/content/drpc/drpc.script.lua:37-38`.

Lua's `%c` uses the active C character classification. Under Windows single-byte locales,
some bytes above 127 are classified as controls. Such bytes also occur inside valid UTF-8
characters. Escaping one as `\\u00xx` splits its multibyte sequence and corrupts the text.
This affects saveName and potentially rankName; there is no warning. The companion's
UTF8Encoding(false) uses replacement fallback, so invalid sequences become replacement
characters before JSON parsing.

Reproduction: `ctype_locale()` in `review.py`, and F4 in `repro.lua`.
Keep the numeric locale C, call `os.setlocale("German_Germany.1252", "ctype")`, set
`SAVE_NAME = "東京"`, and invoke `guiUpdate`. Expected: unchanged UTF-8 save name.
Observed: its bytes E6 9D B1 E4 BA AC become E6 [ASCII \\u009d] B1 E4 BA AC;
strict UTF-8 decoding fails, replacement decoding produces a different save name, and
there are zero warnings. The original text round-trips under the C character locale.

The active character locale is the condition; the player's game-language setting alone
does not prove it is active. Suggested correction: match JSON's explicit ASCII escape
set (bytes 0-31, quote and backslash), leaving UTF-8 bytes untouched.

## Checks and limits

- All four files under mod/ were inspected, including metadata and the script registration.
- Game API names, native fields, company helpers and sandbox/tycoon detection were checked
  against the installed definitions and base-game scripts, with the current local skill.
- Baseline output exactly matches all 46 fields of the first real session-log status.
- Normal cadence, backward clock changes, global isolation, warning deduplication, clock
  failure/recovery, non-finite omission, Unicode and control-character escaping under C,
  optional values, mode flags, fractional numbers and metadata/hook checks passed.
- LuaLS 3.19.2 reported no diagnostics, including hints, with repository configuration.
- No live game/Discord session or macOS run was performed. Auto-activation and removal
  behaviour were not treated as confirmed defects without a live game reproduction.
