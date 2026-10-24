<#
.SYNOPSIS
Links the mod from this repository into the TF3 staging area ("My Mods").

.DESCRIPTION
Creates a link <Steam>/userdata/<steamid>/3493540/local/staging_area/zagrthos_drpc that points to
mod/zagrthos_drpc in this repository, so edits in the repository are live in the game: a directory
junction on Windows, a symbolic link on macOS; neither needs admin rights. Run it once per Steam
account; -Remove deletes the link again (the repository files stay untouched).

.PARAMETER SteamId
Steam account folder under <Steam>\userdata. Defaults to every account that has TF3 user data.

.PARAMETER Remove
Removes the junction instead of creating it.
#>
param(
    [string]$SteamId,
    [switch]$Remove
)

$ErrorActionPreference = 'Stop'

$modId = 'zagrthos_drpc'
$tf3AppId = '3493540'
$source = Join-Path $PSScriptRoot (Join-Path '..' (Join-Path 'mod' $modId)) | Resolve-Path
$linkType = if ($IsMacOS) { 'SymbolicLink' } else { 'Junction' }

$steamPath = if ($IsMacOS) {
    Join-Path $HOME 'Library' 'Application Support' 'Steam'
} else {
    (Get-ItemProperty -Path 'HKCU:\Software\Valve\Steam' -Name SteamPath).SteamPath
}
$userdata = Join-Path $steamPath 'userdata'

$accounts = if ($SteamId) {
    @(Get-Item (Join-Path $userdata $SteamId))
} else {
    @(Get-ChildItem $userdata -Directory | Where-Object { Test-Path (Join-Path $_.FullName (Join-Path $tf3AppId 'local')) })
}
if ($accounts.Count -eq 0) {
    throw "No Steam account with Transport Fever 3 user data found in $userdata. Start the game once first."
}

foreach ($account in $accounts) {
    $stagingArea = Join-Path $account.FullName (Join-Path $tf3AppId (Join-Path 'local' 'staging_area'))
    $target = Join-Path $stagingArea $modId

    if ($Remove) {
        if (Test-Path $target) {
            # Deleting a junction or symbolic link removes only the link, not the files it points to.
            (Get-Item $target).Delete()
            Write-Host "Removed $target"
        }
        continue
    }

    if (Test-Path $target) {
        $item = Get-Item $target
        if ($item.LinkType -eq $linkType -and $item.Target -contains $source.Path) {
            Write-Host "Already linked: $target"
            continue
        }
        throw "$target already exists and is not a link to $source. Remove it first."
    }

    New-Item -ItemType Directory -Force -Path $stagingArea | Out-Null
    New-Item -ItemType $linkType -Path $target -Target $source.Path | Out-Null
    Write-Host "Linked $target -> $source"
}
