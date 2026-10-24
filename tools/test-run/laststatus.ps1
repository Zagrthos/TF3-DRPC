<#
Prints the last [drpc-status] line of the game log: its time and the values the presets use.
  laststatus.ps1 [-UserDataFolder <Steam>\userdata\<your Steam id>\3493540\local]
Without -UserDataFolder the folder is found like the app does (see common.ps1). Reads the log only.
Example: ./laststatus.ps1
#>
param([string]$UserDataFolder)
$ErrorActionPreference = 'Stop'
. "$PSScriptRoot\common.ps1"

$last = Get-LastStatus (Get-GameLogPath $UserDataFolder)
if (-not $last) { 'no status line'; return }
$status = $last.Status
$vehicles = $status.company_roadVehicles + $status.company_railVehicles + $status.company_trams + $status.company_aircrafts + $status.company_ships
[pscustomobject]@{
    time = $last.Time.ToString('yyyy-MM-dd HH:mm:ss\Z'); rank = $status.rank; year = $status.year; month = $status.month; speedup = $status.speedup
    lines = $status.company_numberOfLines; stations = $status.company_totalStations; trains = $status.company_railVehicles; vehicles = $vehicles
    passengers = $status.passengersTransported; cargo = $status.cargoTransported; balance = $status.balance
    companyValue = $status.company_totalAssets + $status.balance - $status.company_debt; saveName = $status.saveName; climate = $status.climate
} | Format-List
