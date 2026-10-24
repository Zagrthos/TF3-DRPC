<#
.SYNOPSIS
Writes THIRD-PARTY-NOTICES.txt for the companion app from its restore output.

.DESCRIPTION
Reads companion/artifacts/obj/Tf3Drpc.App/project.assets.json (written by the restore of the
publish) and collects the licence texts of every NuGet package in the target of the given runtime
identifier, plus the runtime, ILCompiler and framework packs the publish downloaded
(downloadDependencies); the packs hold the .NET runtime that Native AOT compiles into the app.
Test projects are not part of that file. Build-only packages (analysers, MSBuild tasks) are included
too: an extra notice is harmless, a missing one is not.

Per package the texts are, in this order of preference:
- the files in the package folder: LICENSE*, COPYING*, NOTICE*, THIRD-PARTY-NOTICES*, ThirdPartyNotices*
  and the file the nuspec names in <license type="file">;
- when there is none, the standard text of the SPDX expression in <license type="expression">
  (tools/licences/spdx/<expression>.txt) with the nuspec's <copyright>;
- when there is none, the text kept in tools/licences/packages/<id>.txt, for packages that only
  link to their licence (the file names its source);
- when there is none, the nuspec's <licenseUrl>.
A package without any of these fails the script. Packages that share a text are listed under one
copy of it. The output is sorted and has no timestamps, so equal inputs give an equal file.

Package folders are the ones restore used (packageFolders in the assets file), then NUGET_PACKAGES,
then ~/.nuget/packages; packs missing there are looked up in the dotnet root's packs folder.

.PARAMETER Rid
Runtime identifier of the publish: win-x64 or maccatalyst-arm64.

.PARAMETER Output
Path of the file to write (UTF-8 without byte order mark, LF).
#>
param(
    [Parameter(Mandatory)][string]$Rid,
    [Parameter(Mandatory)][string]$Output
)

$ErrorActionPreference = 'Stop'

$root = Join-Path $PSScriptRoot '..' | Resolve-Path
$assetsPath = Join-Path $root 'companion' 'artifacts' 'obj' 'Tf3Drpc.App' 'project.assets.json'
if (-not (Test-Path $assetsPath)) { throw "No restore output at $assetsPath; publish the app first." }
$assets = Get-Content $assetsPath -Raw | ConvertFrom-Json -AsHashtable

# The folders of the restore, then the usual ones.
$nugetFolders = [System.Collections.Generic.List[string]]::new()
foreach ($folder in @($assets.packageFolders.Keys) + @($env:NUGET_PACKAGES, (Join-Path $HOME '.nuget' 'packages'))) {
    if ($folder -and (Test-Path $folder) -and -not $nugetFolders.Contains($folder)) { $nugetFolders.Add($folder) }
}

$dotnetRoots = [System.Collections.Generic.List[string]]::new()
if ($env:DOTNET_ROOT) { $dotnetRoots.Add($env:DOTNET_ROOT) }
$dotnetCommand = Get-Command dotnet -ErrorAction SilentlyContinue
if ($dotnetCommand) {
    $item = Get-Item $dotnetCommand.Source
    $dotnetRoots.Add((Split-Path ($item.ResolvedTarget ?? $item.FullName)))
}

# id + version -> @{ Id; Version; Folders (candidate package folders); Optional }
$packages = @{}
function Add-Package([string]$id, [string]$version, [string]$relative, [bool]$optional = $false) {
    $key = "$id $version"
    if ($packages.ContainsKey($key)) { return }
    $candidates = [System.Collections.Generic.List[string]]::new()
    if ($relative) {
        foreach ($folder in $nugetFolders) { $candidates.Add((Join-Path $folder $relative)) }
    }
    foreach ($dotnetRoot in $dotnetRoots) { $candidates.Add((Join-Path $dotnetRoot 'packs' $id $version)) }
    $packages[$key] = @{ Id = $id; Version = $version; Candidates = $candidates; Optional = $optional }
}

$targetKey = @($assets.targets.Keys | Where-Object { $_.EndsWith("/$Rid", [StringComparison]::Ordinal) })[0]
if (-not $targetKey) { throw "No target for $Rid in $assetsPath" }
foreach ($name in $assets.targets[$targetKey].Keys) {
    $library = $assets.libraries[$name]
    if ($library.type -ne 'package') { continue }
    $id, $version = $name.Split('/')
    Add-Package $id $version $library.path
}
foreach ($framework in $assets.project.frameworks.Values) {
    foreach ($dependency in $framework.downloadDependencies) {
        $version = ($dependency.version -replace '[\[\]()\s]', '').Split(',')[0]
        Add-Package $dependency.name $version (Join-Path $dependency.name.ToLowerInvariant() $version)
    }
}

# Workload packs are not in the assets file; the Mac Catalyst ones sit in the dotnet root.
if ($Rid.StartsWith('maccatalyst', [StringComparison]::Ordinal)) {
    foreach ($dotnetRoot in $dotnetRoots) {
        $packs = Join-Path $dotnetRoot 'packs'
        if (-not (Test-Path $packs)) { continue }
        foreach ($pack in Get-ChildItem $packs -Directory -Filter 'Microsoft.MacCatalyst.*') {
            foreach ($version in Get-ChildItem $pack.FullName -Directory) {
                Add-Package $pack.Name $version.Name '' $true
            }
        }
    }
}

function Get-NuspecValue([xml]$nuspec, [string]$name) {
    $node = $nuspec.package.metadata.ChildNodes | Where-Object { $_.LocalName -eq $name } | Select-Object -First 1
    if ($node) { return $node.InnerText.Trim() }
    return $null
}

function ConvertTo-Text([string]$text) {
    # Same text, same key: line endings and trailing blanks do not count.
    $lines = $text.TrimStart([char]0xFEFF) -split "`r?`n" | ForEach-Object { $_.TrimEnd() }
    return (($lines -join "`n").Trim("`n")) + "`n"
}

$sections = @{}      # normalised text -> list of "id version"
$skipped = [System.Collections.Generic.List[string]]::new()
function Add-Text([string]$text, [string]$package) {
    $text = ConvertTo-Text $text
    if (-not $sections.ContainsKey($text)) { $sections[$text] = [System.Collections.Generic.List[string]]::new() }
    if (-not $sections[$text].Contains($package)) { $sections[$text].Add($package) }
}

$filePattern = '^(licen[cs]e|copying|notice|third[-_ ]?party[-_ ]?notices?)'
foreach ($key in $packages.Keys) {
    $package = $packages[$key]
    $folder = $package.Candidates | Where-Object { Test-Path $_ } | Select-Object -First 1
    if (-not $folder) {
        if ($package.Optional) { $skipped.Add($key); continue }
        throw "Package $key is not in any of: $($package.Candidates -join ', ')"
    }

    $nuspecFile = Get-ChildItem $folder -File -Filter '*.nuspec' | Select-Object -First 1
    $nuspec = if ($nuspecFile) { [xml](Get-Content $nuspecFile.FullName -Raw) } else { $null }

    $files = [System.Collections.Generic.List[string]]::new()
    foreach ($file in Get-ChildItem $folder -File | Where-Object { $_.Name -match $filePattern }) { $files.Add($file.FullName) }
    $expression = $null
    $url = $null
    if ($nuspec) {
        $licenceNode = $nuspec.package.metadata.ChildNodes | Where-Object { $_.LocalName -eq 'license' } | Select-Object -First 1
        if ($licenceNode -and $licenceNode.type -eq 'file') {
            $named = Join-Path $folder ($licenceNode.InnerText.Trim() -replace '\\', '/')
            if ((Test-Path $named -PathType Leaf) -and -not $files.Contains($named)) { $files.Add($named) }
        } elseif ($licenceNode -and $licenceNode.type -eq 'expression') {
            $expression = $licenceNode.InnerText.Trim()
        }
        $url = Get-NuspecValue $nuspec 'licenseUrl'
    }

    if ($files.Count -gt 0) {
        foreach ($file in $files) { Add-Text (Get-Content $file -Raw -Encoding utf8) $key }
    } elseif ($expression) {
        $spdx = Join-Path $PSScriptRoot 'licences' 'spdx' "$expression.txt"
        if (-not (Test-Path $spdx)) { throw "Package $key is licensed under '$expression'; add tools/licences/spdx/$expression.txt" }
        $copyright = Get-NuspecValue $nuspec 'copyright'
        if (-not $copyright) { $copyright = 'Copyright (c) ' + (Get-NuspecValue $nuspec 'authors') }
        $body = (Get-Content $spdx -Raw -Encoding utf8)
        # The MIT text starts with its title, then the copyright line; other texts get it in front.
        if ($body.StartsWith('MIT License', [StringComparison]::Ordinal)) {
            $body = $body -replace '^MIT License\r?\n', "MIT License`n`n$copyright`n"
        } else {
            $body = "$copyright`n`n$body"
        }
        Add-Text $body $key
    } elseif (Test-Path ($kept = Join-Path $PSScriptRoot 'licences' 'packages' "$($package.Id.ToLowerInvariant()).txt")) {
        Add-Text (Get-Content $kept -Raw -Encoding utf8) $key
    } elseif ($url -and $url -ne 'https://aka.ms/deprecateLicenseUrl') {
        Add-Text "No licence file ships in the package. Its licence is published at:`n$url" $key
    } elseif ($package.Optional) {
        $skipped.Add($key)
    } else {
        throw "Package $key has no licence information (no licence file, SPDX expression or licenseUrl) in $folder"
    }
}

$lines = [System.Collections.Generic.List[string]]::new()
$lines.Add('TF3-DRPC THIRD-PARTY NOTICES')
$lines.Add('')
$lines.Add('TF3-DRPC itself is under the MIT Licence (see LICENSE.txt). This release also contains')
$lines.Add('or is built from the packages below, which are under their own licences. Packages that')
$lines.Add('share a licence text are listed together above one copy of it.')
$lines.Add('')

# Sorted by the packages of each text, then by the text, for a stable order.
$entries = [System.Collections.Generic.SortedDictionary[string, object]]::new([StringComparer]::Ordinal)
foreach ($text in $sections.Keys) {
    $names = [string[]]$sections[$text]
    [Array]::Sort($names, [StringComparer]::Ordinal)
    $entries[($names -join '|') + "`0" + $text] = [pscustomobject]@{ Names = $names; Text = $text }
}

$rule = '=' * 78
foreach ($entry in $entries.Values) {
    $lines.Add($rule)
    $lines.Add('Packages:')
    foreach ($name in $entry.Names) { $lines.Add("  $name") }
    $lines.Add($rule)
    $lines.Add('')
    $lines.Add($entry.Text.TrimEnd("`n"))
    $lines.Add('')
}

$outDirectory = Split-Path ([IO.Path]::GetFullPath($Output))
New-Item -ItemType Directory -Force $outDirectory | Out-Null
[IO.File]::WriteAllText([IO.Path]::GetFullPath($Output), ($lines -join "`n") + "`n", [Text.UTF8Encoding]::new($false))

foreach ($name in $skipped | Sort-Object) { Write-Warning "No licence information for optional pack $name; left out." }
"{0} packages, {1} licence texts -> {2}" -f $packages.Count, $sections.Count, $Output
