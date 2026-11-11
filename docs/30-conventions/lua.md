---
title: Lua style
type: rules
audience: [contributor, agent]
area: [mod]
covers:
  - .luarc.json
last-reviewed: 2026-11-11
---

# Lua style

How the mod in `mod/zagrthos_drpc/` is written.
How it works is in [the mod](../40-reference/mod/mod.md), its output format in [the status line](../40-reference/mod/status-line.md).

## Code

- Four spaces (from [.editorconfig](../../.editorconfig)), in place of the base game's tabs.
- Every variable is `local`; `function data()` is the only global, because the game logs every new global as an error.
- The Lua language server ([.luarc.json](../../.luarc.json)) reports no problems.

## The game API

- Call only functions and fields that exist in the TF3 definition files; look them up with the `tf3-modding` skill, and take no Transport Fever 2 API for granted.
- Read every game value inside its own `pcall`, and a shared source once, so a changed API after a game update drops only the values that depend on it.
- Time intervals use `api.util.getLocalTime()`, because `os.clock()` is CPU time on macOS.

## Output

- The JSON encoder leaves out NaN and infinity: they have no JSON form and would make the companion reject the whole line.
- Numbers use `%.0f` and `%.6g` in place of `%d`, because the game's Windows Lua takes only 32-bit values for `%d`.
- Patterns use explicit byte ranges in place of `%c`, which is locale-dependent and can match UTF-8 bytes; the encoder's output never depends on the C locale.
- Log with `log.message` / `log.warning`, prefixed `[drpc]`.
- The status line uses `[drpc-status]`, and its format is fixed in [the status line](../40-reference/mod/status-line.md); raise `v` when a key changes meaning.
