---
title: Set up a Mac
type: guide
audience: [contributor, agent]
area: [macos, app, tools]
covers:
  - companion/global.json
  - companion/Tf3Drpc.App/Tf3Drpc.App.csproj
  - .gitattributes
  - tools/test-run/.swift-format
  - tools/test-run/tf3mac.swift
last-reviewed: 2026-11-13
---

# Set up a Mac

The steps a Mac needs on top of [setting up a development machine](setup.md).

## Steps

1. Install the .NET SDK with the Homebrew formula, not the `dotnet-sdk` cask:

   ```sh
   brew install dotnet
   ```

   The formula's SDK lives in `/opt/homebrew` and keeps its workloads in `~/.dotnet`, so no command needs `sudo`.
   Switching from the cask leaves the new SDK without any workloads, and every restore fails until step 2 has run.
   A `brew upgrade` that brings a new SDK feature band needs step 2 again.
2. Install the pinned workload set and the Mac Catalyst workload from a folder outside the repository, because `dotnet workload` refuses `--version` inside a folder whose `global.json` already pins one:

   ```sh
   cd ~
   dotnet workload update --version <workloadVersion from global.json>
   dotnet workload install maui-maccatalyst --version <workloadVersion from global.json>
   ```

   `dotnet workload list` then shows that workload version and `maui-maccatalyst`.
3. Install the Xcode version the Mac Catalyst SDK was made for (Xcode 27.2 for the .NET 11 GA workload set) and select it:

   ```sh
   sudo xcode-select -s /Applications/Xcode_27.2.app
   ```

   The SDK refuses any other major or minor version with error `E0191`.
   When `global.json` moves, check `RecommendedXcodeVersion` of the new SDK.
4. Make git write CRLF, once per clone, with a clean working tree:

   ```sh
   git config core.autocrlf false
   git config core.eol crlf
   rm .git/index && git checkout HEAD -- .
   ```

   macOS's `/etc/gitconfig` sets `core.autocrlf=input`, which checks files out with LF, and every C# file then gets `RCS0044`.
   Swift sources stay LF (`.gitattributes`, `tools/test-run/.editorconfig`), because `swift-format` misreads CRLF.
5. For the test-run scripts in [tools/test-run/](../../tools/test-run/README.md): install `pwsh` and the Xcode command line tools, and give the app that runs them (Terminal, VS Code) Accessibility and Screen Recording in System Settings → Privacy & Security.

## Check

`dotnet build TF3-DRPC.slnx` in `companion/` builds the Mac Catalyst app.
The app project picks its target by the host (`net11.0-maccatalyst` on macOS, `net11.0-windows10.0.19041.0` on Windows), so a Mac never builds the Windows target, and a pull request from a Mac says that Windows wasn't checked.

## Formatting the Swift helper

`tools/test-run/tf3mac.swift` is formatted with `swift-format` by `tools/test-run/.swift-format` (4 spaces, 140 characters).
Without the standalone tool, the toolchain does the same:

```sh
swift format --configuration tools/test-run/.swift-format -i tools/test-run/tf3mac.swift
```
