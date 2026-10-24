$ErrorActionPreference = 'Stop'
$entries = @(Get-WinEvent -FilterHashtable @{ LogName = 'Application'; StartTime = (Get-Date).AddMinutes(-15) } -ErrorAction SilentlyContinue | Where-Object { $_.Message -like '*TF3-DRPC*' -and $_.Message -like '*blocked-log-folder*' })
$entries | ForEach-Object { $_.Message } | Set-Content -LiteralPath (Join-Path $PSScriptRoot 'startup-crash-events.txt') -Encoding UTF8
Write-Output ('Matched app crash events: ' + $entries.Count)
