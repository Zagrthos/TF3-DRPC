<#
Asks a process's main window to take a size (physical pixels) and reports the size it really got.
  resize.ps1 <processName> <width> <height>
Example: ./resize.ps1 TF3-DRPC 400 500
#>
param([string]$Name, [int]$Width, [int]$Height)
$ErrorActionPreference = 'Stop'
if (-not ('Resizer' -as [type])) { Add-Type -TypeDefinition @'
using System;
using System.Runtime.InteropServices;
public static class Resizer {
    [DllImport("user32.dll")] public static extern bool SetProcessDPIAware();
    [DllImport("user32.dll")] public static extern bool SetWindowPos(IntPtr h, IntPtr after, int x, int y, int cx, int cy, uint flags);
    [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr h, out RECT r);
    [DllImport("user32.dll")] public static extern uint GetDpiForWindow(IntPtr h);
    [StructLayout(LayoutKind.Sequential)] public struct RECT { public int L, T, R, B; }
}
'@ }
[Resizer]::SetProcessDPIAware() | Out-Null
$proc = Get-Process $Name | Where-Object { $_.MainWindowHandle -ne 0 } | Select-Object -First 1
if (-not $proc) { throw "$Name window not found" }
$hwnd = $proc.MainWindowHandle
# SWP_NOMOVE | SWP_NOZORDER | SWP_NOACTIVATE
[Resizer]::SetWindowPos($hwnd, [IntPtr]::Zero, 0, 0, $Width, $Height, 0x0002 -bor 0x0004 -bor 0x0010) | Out-Null
Start-Sleep -Milliseconds 500
$r = New-Object Resizer+RECT; [Resizer]::GetWindowRect($hwnd, [ref]$r) | Out-Null
$scale = [Resizer]::GetDpiForWindow($hwnd) / 96.0
"asked ${Width}x$Height px, got $($r.R - $r.L)x$($r.B - $r.T) px = $([int](($r.R - $r.L) / $scale))x$([int](($r.B - $r.T) / $scale)) DIP (scale $scale)"
