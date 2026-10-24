<#
Prints the texts of the TF3-DRPC settings window in tree order, which follows the XAML: game
status, Discord connection, the preview heading, the preview lines, the language heading, ... in the language the
app shows (it needs no language setting of its own: it prints what the window has).
  apptext.ps1 [-Limit <count>]   the first <count> texts (default 12)
The tree order is used, not the screen position: after the settings scrolled, positions misread.
Needs the app running with its settings window open (start the exe a second time) and WinApp CLI 0.7.
Example: ./apptext.ps1 -Limit 20
#>
param([int]$Limit = 12)
$ErrorActionPreference = 'Stop'
. "$PSScriptRoot\common.ps1"

Assert-WinApp
$app = Get-AppProcess
$win = @(winapp ui list-windows -a $app.Id --json 2>$null | ConvertFrom-Json) | Where-Object { $_.title -eq 'TF3-DRPC' } | Select-Object -First 1
if (-not $win) { throw 'settings window not visible; start the app a second time to show it' }
$json = (winapp ui inspect -w $win.hwnd --json --depth 14 2>$null) -join "`n" | ConvertFrom-Json
function Get-Elements($nodes) { foreach ($node in $nodes) { if ($null -ne $node) { $node; Get-Elements $node.children } } }
$texts = @(Get-Elements @($json.windows | ForEach-Object { $_.elements }) | Where-Object { $_.type -eq 'Text' -and $_.name } |
    Select-Object -ExpandProperty name)
$texts | Select-Object -First $Limit
