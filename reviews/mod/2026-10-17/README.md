# Mod review and reproducible checks — 2026-10-17

Start with [findings.md](findings.md). The review covers all four mod files as of 2026-10-17. It
reports one P3 fault-isolation issue and records three tested invalid-input boundaries
separately. No production file was changed.

## What is recorded

| File | Content |
|---|---|
| `findings.md` | Exact locations, triggers, expected/observed behaviour, impact, reproduction and correction direction |
| `review.py`, `fixture.lua` | Run the production Lua chunk unchanged with controlled API responses and throwing property getters |
| `results-windows.json`, `results-linux.json` | Recorded Lua results, runtime versions and production hashes |
| `messages.json` | Exact messages exported by the Windows Lua run; the Linux messages were compared and are identical |
| `receiver/` | Small .NET console harness referencing the unchanged Core project, using its existing internal-test access |
| `receiver-results.json`, `receiver-output.txt` | Real classifier acceptance/rejection and `{month}` rendering results |
| `inspect_game.py`, `game-inspection.json` | Repeatable read-only installed-game checks, hashes and exact source-line evidence |
| `luals-check.json` | Raw LuaLS diagnostic array (`[]`) |
| `validation.json`, `source-manifest.json` | Commands, exit codes, toolchain evidence, scope and integrity checks |

Fresh executions write to ignored `artifacts/`. They do not overwrite these recorded files.
Downloaded test tools and build output also stay outside the tracked review evidence.

## Run the Lua checks

Python 3.12 or newer and `lupa==2.8` are required. The runner imports `lupa.lua52`
explicitly; it does not use Lupa's default Lua version. Every case creates a fresh runtime.

Windows PowerShell, from the repository root:

```powershell
py -3 -m pip install --target reviews/mod/2026-10-17/artifacts/deps lupa==2.8
py -3 reviews/mod/2026-10-17/review.py
```

The Windows runner also looks in that review-local dependency directory. Alternatively,
install Lupa in an existing Python virtual environment and use its Python executable.

Linux, from the repository root, in an environment with pip:

```sh
python3 -m pip install --target /tmp/tf3-mod-review-deps lupa==2.8
PYTHONPATH=/tmp/tf3-mod-review-deps python3 reviews/mod/2026-10-17/review.py
```

To run just the finding, add `--case R1`. `--case O1`, `O2` and `O3` select the explicitly
invalid-input boundary observations. `--output <path>` chooses another fresh results
file; its companion `*-messages.json` contains that run's emitted messages.

- `PASS` means the control assertions succeeded.
- `REPRODUCED` means the finding/boundary's stated behaviour was asserted. Read `kind`:
  `boundary` is not a confirmed current-game defect.
- `SKIP` means the necessary non-C locale was unavailable. All three German locale
  categories were exercised on Windows; the Linux environment had none of those locales.
- `FAILED` means an assertion or the harness failed. It is not evidence of a reproduced
  production defect. The runner returns a nonzero exit code if any case fails.

## Check the actual companion receiver

Requires the .NET SDK pinned in `receiver/global.json`, copied from `companion/global.json`.
No MAUI workload, game or Discord client is needed. The recorded execution used the
installed Windows SDK `11.0.100-rc.2.26504.105`.

After the full Windows Lua run, from the repository root in PowerShell:

```powershell
Push-Location reviews/mod/2026-10-17/receiver
dotnet run --project Receiver.csproj -- ../artifacts/latest-results-win32-messages.json ../artifacts/latest-receiver-results.json
Pop-Location
```

For a Linux Lua dataset, substitute `latest-results-linux-messages.json`. The receiver
compares the production classifier's result with each message's expected acceptance and
asserts the finding's month-rendering controls. It writes JSON through `Utf8JsonWriter`;
reflection-based JSON remains disabled. A mismatched result throws and exits nonzero.

The harness references the real `Tf3Drpc.Core.csproj`. It neither rewrites the classifier
nor replaces its source-generated serializer or template engine. The harness assembly
name is `Tf3Drpc.Core.Tests`, using the Core project's existing `InternalsVisibleTo`
declaration. It does not run the xunit test suite.

## Repeat the installed-game checks

This uses Python's standard library only and reads the game files without modifying them:

```powershell
py -3 reviews/mod/2026-10-17/inspect_game.py 'C:\Program Files (x86)\Steam\steamapps\common\Transport Fever 3'
```

The recorded script completed 52 checks, including all 24 copied `CompanyValue` fields.
It checks the installed definitions, relevant files inside the game's archives and the
shipped sandbox/tycoon scripts. A missing symbol or mismatch exits nonzero. The report
contains small matched snippets and hashes, not copies of the game binaries or archives.

The repository's `tf3-modding` skill was used. Its local version and the fetched remote
version were both `2026.10.12.1`; installed-game files were the source of truth.

## Lua language server

Using LuaLS 3.19.2, substitute local paths as appropriate:

```sh
lua-language-server --check="$PWD/mod" --configpath="$PWD/.luarc.json" --checklevel=Hint --check_format=json --check_out_path=/tmp/tf3-mod-check.json --logpath=/tmp/tf3-mod-luals-log --metapath=/tmp/tf3-mod-luals-meta
```

This checks every production Lua file with the repository's Lua 5.2 configuration.
The recorded run exited 0 and returned an empty diagnostic array.

## Scope limits

No live game, Discord, macOS, console, mod.io upload/validation or AOT publish was run.
R1 is a reproduced exception-isolation defect under an injected property-getter failure;
the installed game's ordinary date getters were not observed to fail. The three `O`
cases deliberately violate current API return contracts and are not counted as current
defects. See the report for the exact conditions and the distinction between static
installed-game checks, executed Lua tests and real companion receiver checks.
