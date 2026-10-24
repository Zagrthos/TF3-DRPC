<#
.SYNOPSIS
Reproduces R07 (a file left in the publish folder ships in the release zip) and R09 (the zip's
file count), with a real Native AOT publish on Windows.

.DESCRIPTION
Exports the given commit as a zip into -OutFolder, runs tools/build-release.ps1
-SkipTests there, counts the zip's files and folders, then puts OldDependency.dll into the
publish folder (standing in for a file an earlier publish produced and the current one
doesn't) and runs the script again. Takes a few minutes. Nothing in the repository is changed.

.PARAMETER Repo
The repository root.

.PARAMETER Commit
The commit to export, for example HEAD.

.PARAMETER OutFolder
An empty scratch folder outside the repository.
#>
param(
    [Parameter(Mandatory)][string]$Repo,
    [Parameter(Mandatory)][string]$Commit,
    [Parameter(Mandatory)][string]$OutFolder
)

$ErrorActionPreference = 'Stop'
New-Item -ItemType Directory -Force $OutFolder | Out-Null
$OutFolder = (Resolve-Path $OutFolder).Path
$archive = Join-Path $OutFolder 'source.zip'
git -C $Repo archive --format=zip -o $archive $Commit
Expand-Archive $archive -DestinationPath $OutFolder

Add-Type -AssemblyName System.IO.Compression.FileSystem
function Show-Zip {
    $zip = [IO.Compression.ZipFile]::OpenRead((Join-Path $OutFolder 'artifacts/TF3-DRPC-0.1.0-windows-x64.zip'))
    try {
        $folders = @($zip.Entries | Where-Object { $_.FullName.EndsWith('/') })
        "zip entries: $($zip.Entries.Count), files: $($zip.Entries.Count - $folders.Count), folders: $($folders.Count) ($(($folders | ForEach-Object FullName) -join ', '))"
        'OldDependency.dll in the zip: ' + [bool]($zip.Entries | Where-Object FullName -EQ 'TF3-DRPC/OldDependency.dll')
    } finally {
        $zip.Dispose()
    }
}

'=== first run (clean publish folder)'
& (Join-Path $OutFolder 'tools/build-release.ps1') -SkipTests | Select-Object -Last 1
Show-Zip

'=== second run, after a file was left in the publish folder'
Set-Content (Join-Path $OutFolder 'companion/artifacts/publish/Tf3Drpc.App/release_win-x64/OldDependency.dll') 'from an earlier publish'
& (Join-Path $OutFolder 'tools/build-release.ps1') -SkipTests | Select-Object -Last 1
Show-Zip
