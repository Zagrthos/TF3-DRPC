<#
Screenshot of another app's main window without focusing it or sending input.
  winshot.ps1 <processName> <png> [width]
It asks the window for its content (PrintWindow), so a window behind others still comes out right.
Do not use it on the Discord window: it shows content unrelated to the test.
Example: ./winshot.ps1 TF3-DRPC settings.png
#>
param([string]$Name, [string]$Out, [int]$Width = 1280)
$ErrorActionPreference = 'Stop'

if ($IsMacOS) {
    # screencapture -l asks the window server for that window's picture, also when it is covered.
    . "$PSScriptRoot/common.ps1"
    $proc = Get-Process -Name $Name -ErrorAction SilentlyContinue | Select-Object -First 1
    if (-not $proc) { throw "$Name window not found" }
    $id, $null, $null, $w, $h = (& (Get-MacHelper) window $proc.Id) -split ' '
    if ($LASTEXITCODE -ne 0) { throw "$Name window not found" }
    $path = Resolve-OutputPath $Out
    & screencapture -x -o -l $id $path
    # macOS doesn't draw a window on another Space, such as the borderless game behind another app;
    # use tf3ctl.ps1 shot for the game.
    if ($LASTEXITCODE -ne 0) { throw 'screencapture failed: is Screen Recording allowed for this terminal, and is the window on the current Space?' }
    & sips --resampleWidth $Width $path | Out-Null
    "saved $path (window ${w}x$h points)"
    return
}
Add-Type -AssemblyName System.Drawing
if (-not ('WinShot' -as [type])) { Add-Type -TypeDefinition @'
using System;
using System.Runtime.InteropServices;
public static class WinShot {
    [DllImport("user32.dll")] public static extern bool SetProcessDPIAware();
    [DllImport("user32.dll")] public static extern bool PrintWindow(IntPtr h, IntPtr hdc, uint flags);
    [DllImport("user32.dll")] public static extern bool GetClientRect(IntPtr h, out RECT r);
    [StructLayout(LayoutKind.Sequential)] public struct RECT { public int L, T, R, B; }
}
'@ }
[WinShot]::SetProcessDPIAware() | Out-Null
$proc = Get-Process $Name | Where-Object { $_.MainWindowHandle -ne 0 } | Select-Object -First 1
if (-not $proc) { throw "$Name window not found" }
$r = New-Object WinShot+RECT; [WinShot]::GetClientRect($proc.MainWindowHandle, [ref]$r) | Out-Null
$w = $r.R - $r.L; $h = $r.B - $r.T
$bmp = New-Object System.Drawing.Bitmap $w, $h
$g = [System.Drawing.Graphics]::FromImage($bmp)
$hdc = $g.GetHdc(); [WinShot]::PrintWindow($proc.MainWindowHandle, $hdc, 3) | Out-Null; $g.ReleaseHdc($hdc)
$small = New-Object System.Drawing.Bitmap $bmp, $Width, ([int]($h * $Width / $w))
$path = [IO.Path]::GetFullPath($Out, (Get-Location).ProviderPath)
New-Item -ItemType Directory -Force -Path ([IO.Path]::GetDirectoryName($path)) | Out-Null
$small.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
"saved $path (client ${w}x$h)"
