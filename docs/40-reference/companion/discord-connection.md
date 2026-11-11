---
title: Discord connection
type: reference
audience: [contributor, agent]
area: [app, discord]
covers:
  - companion/Tf3Drpc.Core/Discord/**
  - companion/Tf3Drpc.Core/Constants/Discord*.cs
  - companion/Tf3Drpc.Core/Utilities/Discord*.cs
  - companion/Tf3Drpc.App/Services/DiscordService.cs
last-reviewed: 2026-11-11
---

# Discord connection

How the [companion app](README.md) talks to the Discord desktop client; what it sends is in [presets and templates](../presence/presets-and-templates.md).
The decision for the own client is [0010](../../70-decisions/0010-own-source-generated-discord-ipc-client.md).

## Application

- **Application ID:** `1559680023458484244` (not secret).
- **Large image:** Art Asset `cover` (`assets/discord/cover.png`, a square crop of a harbour in our savegame from the game's photo mode, 1024 × 1024), hover text "Transport Fever 3".
  No small image.

## Transport

- **Endpoint:** Windows named pipe `\\.\pipe\discord-ipc-0` … `-9`; macOS Unix socket `$TMPDIR/discord-ipc-0` … `-9`.
- **Protocol:** frames of `opcode:uint32 LE, length:uint32 LE, JSON`.
  Handshake (opcode 0, `{"v":1,"client_id":"…"}`), then `SET_ACTIVITY` (opcode 1); answer `PING` with `PONG`; handle `CLOSE`.
- **Client:** our own, in `Tf3Drpc.Core`: `NamedPipeClientStream` on Windows, a `Socket` with `UnixDomainSocketEndPoint` on macOS, frames serialised through the source-generated JSON context.
  Third-party libraries such as Lachee's `DiscordRichPresence` depend on Newtonsoft.Json and aren't Native AOT compatible.

## Updates

- **Rate:** Discord allows 5 updates per 20 s; the app sends only on change, at most every ~15 s.
- **Text limits** (as Lachee's library enforces them): `details`, `state` and image texts up to 128 characters, button labels up to 31 UTF-8 bytes, URLs up to 512.
  The app shortens texts before sending.
- **Store button:** `SET_ACTIVITY` carries `"buttons":[{"label":"…","url":"https://store.steampowered.com/app/3493540"}]` only while the [setting](settings-file.md) is on; without it the `buttons` key is absent.
  The label is cut to 31 UTF-8 bytes at a character boundary.
  Discord doesn't show the player their own button.
  A refused button comes back as an `ERROR` reply ([rejected presence](#reconnect-and-rejected-presence)); the live test checks that.
- When the game exits, the app clears the presence.

## Reconnect and rejected presence

- **Reconnect:** if Discord isn't running or restarts, the app retries every ~15 s.
- **Write deadline:** a write to Discord that doesn't finish within 5 s drops the connection, so a Discord that stopped reading can't stall the monitor loop; the next update reconnects.
- **Rejected presence:** an `ERROR` reply carrying the nonce of the last `SET_ACTIVITY` means the presence isn't shown.
  The app sends the same content again after 15 s and doubles the wait per further rejection, up to 5 min; changed content is sent at the normal rate.
