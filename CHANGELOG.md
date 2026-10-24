# Changelog

What each release of TF3-DRPC brings. The companion app and the mod are released together; the
app's version is on GitHub, the mod's revision on mod.io.

## 0.1.0 (app) and revision 1 (mod) — 2026-10-24

The first release.

**The mod** writes your game's state (date, rank, mode, network and money) to the game's log
every 10 seconds. It changes nothing in your game, keeps no data in your savegame, and
achievements stay available. Turn it on once per savegame. It is for PC only (Windows and
macOS) and needs the companion app: on its own, or on PS5 and Xbox, it has no visible effect.

**The companion app** shows that state in your Discord status:

- your company's rank and the in-game date, the game mode (Tycoon, Sandbox, map editor), a
  campaign mission's name, "Paused", and the time played;
- six looks for your status: Career, Network, Era, Alternating, Minimal, or your own text with
  placeholders, with a live preview and a tooltip on each look;
- 13 languages, "Same as the game" by default;
- an optional "Get Transport Fever 3" button below your status;
- a light, dark or system theme for the settings window, and buttons to this project on GitHub
  and to the mod on mod.io.

It runs from the extracted zip, needs no installation, sits in the Windows tray or the macOS
menu bar, and writes only to the `DATA` folder next to itself.

Good to know:

- The app isn't code-signed yet. Windows shows a SmartScreen warning and macOS blocks the first
  start; the [README](https://github.com/Zagrthos/TF3-DRPC#3-get-the-companion-app) explains what to do.
- The macOS app is experimental and needs a Mac with Apple silicon and macOS 26 or later.
- The app runs on .NET 11 RC2, a release candidate with a go-live licence; an update to the
  final .NET 11 follows.
