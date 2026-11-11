---
title: 2026-10-24 The first release
type: record
audience: [contributor, maintainer, agent]
area: [mod, app, tools, docs]
last-reviewed: 2026-11-10
---

# 2026-10-24 The first release

## Next steps

1. **The mod:** done.
   Players activate it per savegame and save once; `autoActivate` doesn't help.
2. **Companion MVP:** done on Windows and macOS, both tested end to end with the game and Discord.
3. **Polish**, in this order:
   1. Presets B–E, custom templates for both lines and the hover text, and the live preview: done ([presence.md](../presence.md#presets), [custom templates](../presence.md#custom-templates-and-preview)) and tested with the game and Discord on 2026-10-17.
   2. The optional store button, and the "New Game" save name in the chosen language: done ([store button](../presence.md#store-button), [texts](../presence.md#texts-and-languages)) and tested with the game and Discord on 2026-10-17; the button was also checked from a second Discord account.
   3. **The first release, `0.1.0`:** unsigned, on .NET 11 RC2 (see [Decisions](../70-decisions/README.md)).
      - [x] The release script `tools/build-release.ps1`, the README's SmartScreen section, the mod's `url` in `modinfo.json` and [CHANGELOG.md](../../CHANGELOG.md).
      - [x] The Windows zip built and checked: 94 files in a `TF3-DRPC` folder, with `LICENSE.txt` and `THIRD-PARTY-NOTICES.txt`, no `DATA` folder, 33 MB zipped, and the exe's copyright "© 2026 Zagrthos.
        MIT Licence." instead of "All rights reserved", which contradicted the licence (2026-10-22).
      - [x] CI (`.github/workflows/`): every pull request and every push to `main` is built and tested on Windows and macOS; a push to `main` also leaves both zips as workflow artifacts.
      - [x] The macOS zip from CI unzipped on a Mac and started (2026-10-23): Gatekeeper blocks the quarantined copy with "Done" and "Move to Trash" and offers "Open Anyway" in Privacy & Security, as the README says; the started app shows the menu bar icon and its menu, the settings window in front, and GitHub, mod.io and Exit work.
        The check found the bundle at 99 MB, 80 MB of it CoreCLR files the Native AOT app never loads, and the version shown as `0.1.0.0`; both fixed, the zip is now 7 MB.
      - [x] The Mod Hub's logo and gallery images, from a game run on 2026-10-24 in English: an in-game photo mode shot with a drawn status card, the settings window with a preset tooltip and the plain settings window.
      - [x] The mod prepared for the upload (2026-10-24), against the wiki's [guidelines](https://wiki.transportfever3.com/doku.php?id=modding:general:guidelines): the images are in the mod's `_metadata/` as the game's own mods have them, all 1920 × 1080: `0.png` the cover with the name and the status card (see [Decisions](../70-decisions/0004-mod-hub-cover-with-name-and-status-card.md)), then the settings window on the darkened photo.
        The description in `modinfo.json` says what the mod shows, that it needs the app, that it is for PC only, and how to turn it on.
        The game's validator found no problems for PC and console.
      - [x] **A session on a Mac** (2026-10-24, macOS 27, game build 40425):
        - The app icon was missing from every macOS build: `Info.plist` named the icon set `appicon`, while MAUI names it after `cover.png`, so actool left it out without a warning.
          Fixed; Finder now shows the cover, and the zip grows to 10 MB.
          The menu bar icon is sharp at Retina size; normal size wasn't checked (no such display at hand), the code sets it to 18 points.
          A menu bar app has no Dock icon.
        - `tools/build-release.ps1` now also deletes the platform's `obj` folder: after the `Info.plist` change an incremental build still skipped actool.
        - `LICENSE.txt` and `THIRD-PARTY-NOTICES.txt` are next to `TF3-DRPC.app` in the zip.
          A quarantined copy is stopped by Gatekeeper with "Done" and "Move to Trash", as the README says; "Open Anyway" was last checked on 2026-10-23.
        - The settings window shows "Transport Fever 3 · Build 40425" while the game runs.
        - The test-run scripts: `tf3mac` compiles, its `pixels` command read the Windows reference pictures correctly, and the macOS set (`macos-804`, the screenshot height here) is measured, all but the download bar; `start`, `load`, `quit`, `state` and `photo` ran on the live game ([testing.md](../testing.md#on-macos)).
          The reference pictures and their step are deleted.
      - [x] The mod uploaded from the Mod Hub (2026-10-24), not public yet: mod.io id `6477175` in `_metadata/mod.io_fileid.txt`, page <https://mod.io/g/transportfever3/m/discord-rich-presence>, the same as `AppConstants.ModIoUrl` (the app's "mod.io" button).
        The upload made `1.png` a copy of the cover and moved the settings window to `2.png` and `3.png`, as the gallery on mod.io has them.
        Delete any `.sonar` folder in `mod/zagrthos_drpc/` (an editor extension creates them) before each upload.
      - [x] The changelog's heading dated (`## 0.1.0 (app) and revision 1 (mod) — 2026-10-24`; the Release workflow wants it to end in `YYYY-MM-DD` and the section's links to be absolute).
      - In this order, because the mod's description and the app's buttons link to this repository and its Releases page:
        - [x] Make the repository public (2026-10-24).
        - [x] Release the app (2026-10-24): `v0.1.0`, immutable, with both zips and `SHA256SUMS.txt`.
          The Release workflow checks that the tag matches the app version and sits on `main`, that the changelog's section is dated, not empty and free of relative links, and that the version isn't released yet; then it builds and tests both zips, builds the release as a draft, compares the draft's files with the built ones, publishes it and verifies its attestation.
          Its run for `v0.1.0` published the release but failed that last check, which ran one second after publishing, before GitHub had made the attestation; minutes later `gh release verify` and `verify-asset` passed for the release and all three files.
          The check now waits for it up to 2 minutes.
        - [x] Make the mod public on mod.io (2026-10-24).
