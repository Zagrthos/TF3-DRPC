<#
Screenshot of the TF3-DRPC window by its DWM bounds (the visible frame), optionally resized first.
  shot.ps1 <png> [width height]   the window is moved to 100,100 and resized (physical pixels); without them it stays as it is
It brings the window to the front and copies that screen region, so anything above it, such as a
tooltip popup or another window, is part of the picture. Do not let the Discord window be in front.
Example: ./shot.ps1 settings.png 400 500
#>
param([Parameter(Mandatory)][string]$Out, [int]$Width = 0, [int]$Height = 0)
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
if (-not ('ShotNative' -as [type])) { Add-Type -TypeDefinition @'
using System;
using System.Runtime.InteropServices;
public static class ShotNative {
    [StructLayout(LayoutKind.Sequential)] public struct RECT { public int L, T, R, B; }
    [DllImport("user32.dll")] public static extern bool SetProcessDPIAware();
    [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr h, out RECT r);
    [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr h);
    [DllImport("user32.dll")] public static extern bool SetWindowPos(IntPtr h, IntPtr a, int x, int y, int cx, int cy, uint f);
    [DllImport("dwmapi.dll")] public static extern int DwmGetWindowAttribute(IntPtr h, int a, out RECT r, int s);
}
'@ }
[ShotNative]::SetProcessDPIAware() | Out-Null
$proc = Get-Process -Name 'TF3-DRPC' -ErrorAction SilentlyContinue | Where-Object { $_.MainWindowHandle -ne 0 } | Select-Object -First 1
if (-not $proc) { throw 'TF3-DRPC window not found; start the app a second time to show it' }
$hwnd = $proc.MainWindowHandle
# SWP_NOZORDER | SWP_SHOWWINDOW, at 100,100
if ($Width -gt 0) { [ShotNative]::SetWindowPos($hwnd, [IntPtr]::Zero, 100, 100, $Width, $Height, 0x0044) | Out-Null; Start-Sleep -Milliseconds 800 }
[ShotNative]::SetForegroundWindow($hwnd) | Out-Null
Start-Sleep -Milliseconds 800
$rect = New-Object ShotNative+RECT
# 9 = DWMWA_EXTENDED_FRAME_BOUNDS
[ShotNative]::DwmGetWindowAttribute($hwnd, 9, [ref]$rect, 16) | Out-Null
$w = $rect.R - $rect.L; $h = $rect.B - $rect.T
$bmp = New-Object System.Drawing.Bitmap $w, $h
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.CopyFromScreen($rect.L, $rect.T, 0, 0, $bmp.Size)
$path = [IO.Path]::GetFullPath($Out, (Get-Location).ProviderPath)
New-Item -ItemType Directory -Force -Path ([IO.Path]::GetDirectoryName($path)) | Out-Null
$bmp.Save($path)
"saved $path (size ${w}x${h})"
