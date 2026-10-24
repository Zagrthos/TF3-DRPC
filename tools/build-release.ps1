<#
.SYNOPSIS
Builds the release zip of the companion app for the platform it runs on.

.DESCRIPTION
Runs the tests, publishes the app with Native AOT and zips the result into artifacts/
(ignored by git), named after the version in companion/Directory.Build.props:

- on Windows TF3-DRPC-<version>-windows-x64.zip, a TF3-DRPC folder with TF3-DRPC.exe and its files;
- on macOS TF3-DRPC-<version>-macos-arm64.zip with TF3-DRPC.app, copied and zipped with ditto so the
  bundle's links and signature survive.

Both zips also hold LICENSE.txt (the repository's LICENSE) and THIRD-PARTY-NOTICES.txt, which
tools/new-third-party-notices.ps1 generates from the restore output of the publish, next to the exe
or the .app.

The platform's bin, obj and publish folders are deleted first, so no file from an earlier build ships.
Each platform builds only its own zip, so a release needs one run on Windows and one on a Mac.
The zips are unsigned (see docs/README.md, decisions). The script prints each zip's size and SHA-256
for the release notes.

.PARAMETER SkipTests
Publishes without running the tests first, for example right after a full test run.
#>
param(
    [switch]$SkipTests
)

$ErrorActionPreference = 'Stop'

$root = Join-Path $PSScriptRoot '..' | Resolve-Path
$companion = Join-Path $root 'companion'
$outFolder = Join-Path $root 'artifacts'

$props = [xml](Get-Content (Join-Path $companion 'Directory.Build.props') -Raw)
$version = @($props.Project.PropertyGroup.VersionPrefix | Where-Object { $_ })[0]
if (-not $version) { throw 'No VersionPrefix in companion/Directory.Build.props' }

if ($IsWindows) {
    $rid = 'win-x64'
    $zipName = "TF3-DRPC-$version-windows-x64.zip"
} elseif ($IsMacOS) {
    $rid = 'maccatalyst-arm64'
    $zipName = "TF3-DRPC-$version-macos-arm64.zip"
} else {
    throw 'Release builds run on Windows or macOS only.'
}

Push-Location $companion
try {
    if (-not $SkipTests) {
        dotnet test TF3-DRPC.slnx
        if ($LASTEXITCODE) { throw "Tests failed ($LASTEXITCODE)" }
    }

    # dotnet publish leaves files it no longer produces, and they would end up in the zip; the incremental
    # build can also skip a step whose inputs it doesn't track, such as actool after an Info.plist change.
    foreach ($kind in 'bin', 'obj', 'publish') {
        $output = Join-Path $companion 'artifacts' $kind 'Tf3Drpc.App' "release_$rid"
        if (Test-Path $output) { Remove-Item $output -Recurse -Force }
    }

    dotnet publish Tf3Drpc.App/Tf3Drpc.App.csproj -c Release -r $rid
    if ($LASTEXITCODE) { throw "Publish failed ($LASTEXITCODE)" }
} finally {
    Pop-Location
}

New-Item -ItemType Directory -Force $outFolder | Out-Null
$zip = Join-Path $outFolder $zipName
if (Test-Path $zip) { Remove-Item $zip }

if ($IsWindows) {
    $published = Join-Path $companion 'artifacts' 'publish' 'Tf3Drpc.App' "release_$rid"
    # Zip a clean copy in a TF3-DRPC folder; a DATA folder from a test start must not ship.
    $staging = Join-Path $outFolder 'staging'
    $appFolder = Join-Path $staging 'TF3-DRPC'
    if (Test-Path $staging) { Remove-Item $staging -Recurse -Force }
    New-Item -ItemType Directory -Force $appFolder | Out-Null
    Get-ChildItem $published -Force | Where-Object Name -ne 'DATA' | Copy-Item -Destination $appFolder -Recurse
    Copy-Item (Join-Path $root 'LICENSE') (Join-Path $appFolder 'LICENSE.txt')
    & (Join-Path $PSScriptRoot 'new-third-party-notices.ps1') -Rid $rid -Output (Join-Path $appFolder 'THIRD-PARTY-NOTICES.txt')
    Compress-Archive -Path $appFolder -DestinationPath $zip
    Remove-Item $staging -Recurse -Force
} else {
    $bin = Join-Path $companion 'artifacts' 'bin' 'Tf3Drpc.App' "release_$rid"
    $app = @(Get-ChildItem $bin -Directory -Recurse -Filter '*.app' | Select-Object -First 1)[0]
    if (-not $app) { throw "No .app below $bin" }
    # The licence files go next to the .app at the top level of the zip; the signed bundle stays untouched.
    $staging = Join-Path $outFolder 'staging'
    if (Test-Path $staging) { Remove-Item $staging -Recurse -Force }
    New-Item -ItemType Directory -Force $staging | Out-Null
    ditto $app.FullName (Join-Path $staging $app.Name)
    if ($LASTEXITCODE) { throw "ditto failed ($LASTEXITCODE)" }
    Copy-Item (Join-Path $root 'LICENSE') (Join-Path $staging 'LICENSE.txt')
    & (Join-Path $PSScriptRoot 'new-third-party-notices.ps1') -Rid $rid -Output (Join-Path $staging 'THIRD-PARTY-NOTICES.txt')
    ditto -c -k $staging $zip
    if ($LASTEXITCODE) { throw "ditto failed ($LASTEXITCODE)" }
    Remove-Item $staging -Recurse -Force
}

$item = Get-Item $zip
$hash = (Get-FileHash $zip -Algorithm SHA256).Hash.ToLowerInvariant()
"{0}  {1:N1} MB  SHA-256 {2}" -f $item.Name, ($item.Length / 1MB), $hash
