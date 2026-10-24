<#
Drives the main window of another process (the TF3-DRPC settings window, say) by its process name.
  appctl.ps1 <process> shot <png>        screenshot of the window, scaled to 700 px wide
  appctl.ps1 <process> click <x> <y>     left click at coordinates of the scaled screenshot
  appctl.ps1 <process> rclick <x> <y>    right click
  appctl.ps1 <process> wheel <x> <y>     scroll up by 5 notches at the point; no focus change, so a popup stays open
  appctl.ps1 <process> wheeldown <x> <y> scroll down by 3 notches, the same way
  appctl.ps1 <process> key <VK> [count]  press a virtual key (e.g. ESCAPE, RETURN, TAB)
  appctl.ps1 <process> type <text>       type text (Unicode)
shot, click, rclick, key and type first bring the window to the front and refuse to act if another
window is in front. Do not use it on the Discord window: it shows content unrelated to the test.
Example: ./appctl.ps1 TF3-DRPC shot settings.png
#>
param([string]$ProcName, [string]$Cmd, [string]$A1, [string]$A2)
$ErrorActionPreference = 'Stop'

Add-Type -AssemblyName System.Drawing, System.Windows.Forms
if (-not ('AppInput' -as [type])) { Add-Type -TypeDefinition @'
using System;
using System.Runtime.InteropServices;
public static class AppInput {
    [DllImport("user32.dll")] public static extern bool SetProcessDPIAware();
    [DllImport("user32.dll")] public static extern bool PrintWindow(IntPtr h, IntPtr hdc, uint flags);
    [DllImport("user32.dll")] public static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr h);
    [DllImport("user32.dll")] public static extern bool ShowWindow(IntPtr h, int cmd);
    [DllImport("user32.dll")] public static extern bool GetClientRect(IntPtr h, out RECT r);
    [DllImport("user32.dll")] public static extern bool ClientToScreen(IntPtr h, ref POINT p);
    [DllImport("user32.dll")] public static extern bool SetCursorPos(int x, int y);
    [DllImport("user32.dll")] public static extern uint MapVirtualKey(uint code, uint type);
    [DllImport("user32.dll", SetLastError = true)] public static extern uint SendInput(uint n, INPUT[] inputs, int size);
    [StructLayout(LayoutKind.Sequential)] public struct RECT { public int L, T, R, B; }
    [StructLayout(LayoutKind.Sequential)] public struct POINT { public int X, Y; }
    [StructLayout(LayoutKind.Sequential)] public struct MOUSEINPUT { public int dx, dy; public uint data, flags, time; public IntPtr extra; }
    [StructLayout(LayoutKind.Sequential)] public struct KEYBDINPUT { public ushort vk, scan; public uint flags, time; public IntPtr extra; }
    [StructLayout(LayoutKind.Explicit)] public struct UNION { [FieldOffset(0)] public MOUSEINPUT mi; [FieldOffset(0)] public KEYBDINPUT ki; }
    [StructLayout(LayoutKind.Sequential)] public struct INPUT { public uint type; public UNION u; }
    public static void Mouse(uint flags) {
        var i = new INPUT[1]; i[0].type = 0; i[0].u.mi.flags = flags;
        SendInput(1, i, Marshal.SizeOf(typeof(INPUT)));
    }
    public static void Wheel(int delta) {
        var i = new INPUT[1]; i[0].type = 0; i[0].u.mi.flags = 0x0800; i[0].u.mi.data = (uint)delta;
        SendInput(1, i, Marshal.SizeOf(typeof(INPUT)));
    }
    public static void Key(ushort vk, bool up) {
        var i = new INPUT[1]; i[0].type = 1;
        i[0].u.ki.scan = (ushort)MapVirtualKey(vk, 0);
        i[0].u.ki.flags = 0x0008 | (up ? 0x0002u : 0u); // SCANCODE (+ KEYUP)
        SendInput(1, i, Marshal.SizeOf(typeof(INPUT)));
    }
    public static void Char(char c, bool up) {
        var i = new INPUT[1]; i[0].type = 1;
        i[0].u.ki.scan = c; i[0].u.ki.flags = 0x0004 | (up ? 0x0002u : 0u); // UNICODE (+ KEYUP)
        SendInput(1, i, Marshal.SizeOf(typeof(INPUT)));
    }
}
'@ }
[AppInput]::SetProcessDPIAware() | Out-Null

$proc = Get-Process $ProcName -ErrorAction SilentlyContinue | Where-Object { $_.MainWindowHandle -ne 0 } | Select-Object -First 1
if (-not $proc) { throw "$ProcName window not found" }
$hwnd = $proc.MainWindowHandle

function Get-ClientArea {
    $r = New-Object AppInput+RECT; [AppInput]::GetClientRect($hwnd, [ref]$r) | Out-Null
    $p = New-Object AppInput+POINT; [AppInput]::ClientToScreen($hwnd, [ref]$p) | Out-Null
    [pscustomobject]@{ X = $p.X; Y = $p.Y; W = $r.R - $r.L; H = $r.B - $r.T }
}

function Set-Foreground {
    if ([AppInput]::GetForegroundWindow() -ne $hwnd) {
        [AppInput]::ShowWindow($hwnd, 9) | Out-Null
        [AppInput]::SetForegroundWindow($hwnd) | Out-Null
        Start-Sleep -Milliseconds 400
    }
    if ([AppInput]::GetForegroundWindow() -ne $hwnd) { throw "$ProcName is not the foreground window; input refused" }
}

$scaleWidth = 700
$area = Get-ClientArea
$scale = $area.W / $scaleWidth

switch ($Cmd) {
    'shot' {
        Set-Foreground
        $bmp = New-Object System.Drawing.Bitmap $area.W, $area.H
        $g = [System.Drawing.Graphics]::FromImage($bmp)
        if ($A2 -eq 'gdi') {
            $g.CopyFromScreen($area.X, $area.Y, 0, 0, $bmp.Size)
        } else {
            # PW_CLIENTONLY | PW_RENDERFULLCONTENT: asks DWM for the window's current content.
            $hdc = $g.GetHdc(); [AppInput]::PrintWindow($hwnd, $hdc, 3) | Out-Null; $g.ReleaseHdc($hdc)
        }
        $h = [int]($area.H / $scale)
        $small = New-Object System.Drawing.Bitmap $bmp, $scaleWidth, $h
        $path = [IO.Path]::GetFullPath($A1, (Get-Location).ProviderPath)
        New-Item -ItemType Directory -Force -Path ([IO.Path]::GetDirectoryName($path)) | Out-Null
        $small.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
        "saved $path (client $($area.W)x$($area.H) at $($area.X),$($area.Y); image ${scaleWidth}x$h)"
    }
    { $_ -in 'click', 'rclick' } {
        Set-Foreground
        $x = $area.X + [int]([double]$A1 * $scale); $y = $area.Y + [int]([double]$A2 * $scale)
        [AppInput]::SetCursorPos($x, $y) | Out-Null; Start-Sleep -Milliseconds 120
        $down, $up = if ($Cmd -eq 'click') { 0x0002, 0x0004 } else { 0x0008, 0x0010 }
        [AppInput]::Mouse($down); Start-Sleep -Milliseconds 60; [AppInput]::Mouse($up)
        "$Cmd at screen $x,$y"
    }
    'wheel' {
        # no Set-Foreground: an open popup must stay open
        $x = $area.X + [int]([double]$A1 * $scale); $y = $area.Y + [int]([double]$A2 * $scale)
        [AppInput]::SetCursorPos($x, $y) | Out-Null; Start-Sleep -Milliseconds 120
        for ($i = 0; $i -lt 5; $i++) { [AppInput]::Wheel(120); Start-Sleep -Milliseconds 80 }
        "wheel up at screen $x,$y"
    }
    'wheeldown' {
        $x = $area.X + [int]([double]$A1 * $scale); $y = $area.Y + [int]([double]$A2 * $scale)
        [AppInput]::SetCursorPos($x, $y) | Out-Null; Start-Sleep -Milliseconds 120
        for ($i = 0; $i -lt 3; $i++) { [AppInput]::Wheel(-120); Start-Sleep -Milliseconds 150 }
        "wheel down at screen $x,$y"
    }
    'key' {
        Set-Foreground
        $vk = [uint16][System.Windows.Forms.Keys]::$A1
        $count = if ($A2) { [int]$A2 } else { 1 }
        for ($i = 0; $i -lt $count; $i++) { [AppInput]::Key($vk, $false); Start-Sleep -Milliseconds 60; [AppInput]::Key($vk, $true); Start-Sleep -Milliseconds 120 }
        "key $A1 x$count"
    }
    'type' {
        Set-Foreground
        foreach ($c in $A1.ToCharArray()) { [AppInput]::Char($c, $false); [AppInput]::Char($c, $true); Start-Sleep -Milliseconds 40 }
        "typed $A1"
    }
    default { throw "unknown command $Cmd" }
}
