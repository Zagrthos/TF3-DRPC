# Mod review reproductions

The [review report](findings.md) describes all four findings, their source lines, impact,
reproduction conditions and suggested corrections.

Reviewed: the mod as of 2026-10-15.
Production script SHA-256: `f5becc0c841c312153e8a8184cd2f4491b245977b542a5ae560634ea688c747f`.

The fixture supplies controlled game API responses. The production Lua file is read from
the checkout and executed unchanged. These are runtime reproductions with fault injection,
not a live Transport Fever 3 session.

## Python harness

On Windows, using Python 3.12 or newer:

```powershell
py -m pip install lupa==2.8
py .\review.py ..\..\..
```

Run it from `reviews/mod/2026-10-15/`, or pass the repository root explicitly.
Without an argument, the Python harness finds the root relative to this folder. The harness deliberately imports
`lupa.lua52`, not Lupa's default Lua version. `REPRODUCED` means that the test asserted the
defective behaviour. `PASS` refers to a control test; `SKIP` means the platform lacks the
relevant integer range or locale. New Python results are written to the system temporary
folder as `tf3-mod-review-results-<platform>.json`; the evidence below stays as recorded.

Linux can run the same harness with `python3`; the grouped-read defect reproduces there,
but the Windows integer and locale cases are platform-specific:

```sh
python3 -m pip install lupa==2.8
python3 review.py ../../..
```

## Standalone Lua harness

With a Windows Lua 5.2 interpreter, no Python dependency is needed:

```powershell
lua52.exe .\repro.lua ..\..\..
```

`repro.lua` includes one minimal reproduction for each finding. `review.py` adds more
failure scenarios, numeric boundaries, normal-operation controls and strict JSON checks.

## Recorded evidence

- `results-windows.json`: Windows Lua 5.2.4 / Lupa 2.8, all four findings reproduced.
- `results-linux.json`: Linux Lua 5.2.4 / Lupa 2.8, platform-independent controls and F3.
- `luals-check.json`: LuaLS 3.19.2, repository `.luarc.json`, Hint threshold, no diagnostics.
- `luals-output.txt`: LuaLS completion and exit code recorded by the runner.
- `game-integer-format-disassembly.txt`: read-only inspection of installed TF3's formatter.
  At `0x142fd9255`, `vcvttsd2si %xmm0,%ebx` converts the `%d` value to a signed 32-bit
  integer; the following instructions check the conversion and branch to the range error.
  The `%d` and `%i` switch entries both resolve to `0x142fd924a`.
- `game-integer-format-verification.txt`: recorded switch-target checks and executable hash.
- `verify_game_formatter.py`: repeat the binary inspection with Python and GNU objdump:
  `python3 verify_game_formatter.py "<game folder>/TransportFever3.exe"`. The switch offsets
  apply to the recorded executable build.

The installed game identifies its Lua as 5.2.2. Its executable SHA-256 was
`74861ac43b041aebc5179154345b3cf1ec83154c8e6cc58e0d9e02ff5fa602e4`.
The executable's integer conversion matches the Windows runtime reproduction. The locale
findings require the indicated numeric or character locale; the player's language choice
by itself does not establish that either locale is active in the game.
