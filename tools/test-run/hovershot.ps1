<#
Opens the preset picker of the running TF3-DRPC window, hovers one entry and captures the screen around the window while the tooltip is open.
  hovershot.ps1 <entry> <png> [-Language <game code>] [-AppFolder <folder of the published app>]
<entry> is a preset key (career, network, era, alternating, minimal, custom) or the literal text of a picker entry;
a key is turned into the entry's text in the app's language (common.ps1: Get-AppText), as is the picker's name.
The language is -Language, else the app's settings.json (-AppFolder, default the folder of the running app),
else the game's profile.lua; see ui-tests.ps1.
The picture is a copy of the screen region (window plus 400 px to the right), so keep other windows,
Discord above all, away from it. Needs the settings window open and WinApp CLI 0.7.
Example: ./hovershot.ps1 network network.png
#>
param([Parameter(Mandatory)][string]$Entry, [Parameter(Mandatory)][string]$Out, [string]$Language, [string]$AppFolder)
$ErrorActionPreference = 'Stop'
. "$PSScriptRoot\common.ps1"
Assert-WinApp
Set-TestLanguage -Language $Language -AppFolder $AppFolder
$Entry = Get-PresetText $Entry
$pickerName = Get-AppText StatusAppearance
Add-Type -AssemblyName System.Drawing
if (-not ('HoverShot' -as [type])) { Add-Type -TypeDefinition 'using System; using System.Runtime.InteropServices; public static class HoverShot { [DllImport("user32.dll")] public static extern bool SetProcessDPIAware(); [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr h, out RECT r); [StructLayout(LayoutKind.Sequential)] public struct RECT { public int L, T, R, B; } }' }
[HoverShot]::SetProcessDPIAware() | Out-Null
$app = Get-AppProcess
$h = [string](@(winapp ui list-windows -a $app.Id --json 2>$null | ConvertFrom-Json) | Where-Object { $_.title -eq 'TF3-DRPC' } | Select-Object -First 1).hwnd
if (-not $h) { throw 'settings window not visible; start the app a second time to show it' }
$open = @(winapp ui inspect -w $h --interactive 2>$null | Select-String 'ListItem').Count -gt 0
if (-not $open) {
    $preset = @(winapp ui inspect -w $h --interactive 2>$null | Select-String -SimpleMatch ('ComboBox "' + $pickerName + '"'))[0].ToString().Trim().Split(' ')[0]
    winapp ui invoke $preset --action expand -w $h | Out-Null
    Start-Sleep -Milliseconds 800
}
$m = ((winapp ui search $Entry --json -w $h 2>$null) -join "`n" | ConvertFrom-Json).matches | Where-Object { $_.type -eq 'ListItem' } | Select-Object -First 1
if (-not $m) { throw "No list item '$Entry'" }
$job = Start-Job -ScriptBlock { param($sel, $hw) winapp ui hover $sel --dwell-time 4000 -w $hw 2>&1 } -ArgumentList $m.selector, $h
Start-Sleep -Seconds 3
$r = New-Object HoverShot+RECT; [HoverShot]::GetWindowRect([IntPtr][long]$h, [ref]$r) | Out-Null
$b = New-Object System.Drawing.Bitmap ($r.R - $r.L + 400), ($r.B - $r.T)
$g = [System.Drawing.Graphics]::FromImage($b); $g.CopyFromScreen($r.L, $r.T, 0, 0, $b.Size); $path = [IO.Path]::GetFullPath($Out, (Get-Location).ProviderPath)
New-Item -ItemType Directory -Force -Path ([IO.Path]::GetDirectoryName($path)) | Out-Null
$b.Save($path)
Wait-Job $job | Out-Null; Remove-Job $job
"saved $path"
