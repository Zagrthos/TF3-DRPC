<#
Drives the Transport Fever 3 window in a test run (Windows: SendInput, screenshots with PrintWindow).
  tf3ctl.ps1 shot <png>          screenshot of the TF3 window, scaled to 1280 px wide
  tf3ctl.ps1 size                size of that screenshot, for example 1280x720 (it picks the set in calibration.psd1)
  tf3ctl.ps1 click <x> <y>       left click at coordinates of the scaled screenshot
  tf3ctl.ps1 rclick <x> <y>      right click
  tf3ctl.ps1 key <VK> [count]    press a virtual key (e.g. ESCAPE, RETURN, F5, SPACE)
  tf3ctl.ps1 type <text>         type text (Unicode)
  tf3ctl.ps1 photo <png>         photo mode: eye icon, F9, copy the full-resolution picture from the game's
                                 screenshots folder to <png>, Escape (game running, game menu closed)
  tf3ctl.ps1 state               what the screen shows: state=<quit-flyout|game-menu|main-menu|other>
                                 downloads=<True|False> (the Mod Hub download bar of the main menu)
  tf3ctl.ps1 classify <png>      the same for a saved screenshot, and photo=<True|False>; needs no game and sends
                                 no input. With -Verbose it prints the numbers behind every check.
  tf3ctl.ps1 measure <png> <x> <y> <w> <h>   the statistics that the checks use (mean colour, spread, largest
                                 channel, bright pixels) for a box of a saved screenshot; to read off values for
                                 calibration.psd1 (see "Calibrating" in the README). No game, no input.
Every input first brings TF3 to the front and refuses to act if another window is in front.
The click coordinates are those of the 1280 px wide screenshot (change $scaleWidth to use another scale).
The game must run in BORDERLESS mode or the screenshots show only the splash (see run-game.ps1 prepare).
On macOS the same commands go through tf3mac.swift (CoreGraphics input), screencapture and sips; key names
are those of tf3mac.swift. They need the Accessibility and Screen Recording permissions.
photo, state, classify and measure read pixels (System.Drawing on Windows, `tf3mac pixels` on macOS); all but
measure use the boxes and points of calibration.psd1 for the platform and the screenshot's height, and a value
that is not calibrated there stops the command with a message naming it.
-UserDataFolder   <Steam>\userdata\<your Steam id>\3493540\local, for photo (its screenshots folder); without
                  it the folder is found like in common.ps1.
-EyePoint         x,y of the photo mode icon in the 1280 px picture; default: Points.Eye of calibration.psd1.
Example: ./tf3ctl.ps1 key SPACE
         ./tf3ctl.ps1 photo game.png
         ./tf3ctl.ps1 classify game.png -Verbose
         ./tf3ctl.ps1 classify win.png -Platform windows   (a Windows picture, on a Mac too)
         ./tf3ctl.ps1 measure game.png 300 223 100 24
#>
[CmdletBinding()]
param(
    [string]$Cmd, [string]$A1, [string]$A2, [string]$A3, [string]$A4, [string]$A5,
    [string]$UserDataFolder, [int[]]$EyePoint = @(),
    [ValidateSet('', 'windows', 'macos')][string]$Platform = ''
)
# classify and measure: the calibration set of this platform instead of the current one.
$CalibrationPlatform = $Platform
$ErrorActionPreference = 'Stop'

. "$PSScriptRoot/common.ps1"
. "$PSScriptRoot/screen.ps1"

# The width and height of a PNG file, from its IHDR chunk.
function Get-PngSize {
    param([string]$Path)
    $head = [byte[]]::new(24)
    $stream = [IO.File]::OpenRead($Path)
    try { $null = $stream.Read($head, 0, 24) } finally { $stream.Dispose() }
    [pscustomobject]@{
        Width = ([int]$head[16] -shl 24) -bor ([int]$head[17] -shl 16) -bor ([int]$head[18] -shl 8) -bor [int]$head[19]
        Height = ([int]$head[20] -shl 24) -bor ([int]$head[21] -shl 16) -bor ([int]$head[22] -shl 8) -bor [int]$head[23]
    }
}

# A shot to a temporary file, read as pixels for screen.ps1.
function Get-GameShot {
    $temp = Join-Path ([IO.Path]::GetTempPath()) "tf3-shot-$([guid]::NewGuid().ToString('N')).png"
    try {
        Save-Screenshot $temp | Out-Null
        Read-Screenshot $temp
    } finally {
        Remove-Item -LiteralPath $temp -ErrorAction SilentlyContinue
    }
}

# Photo mode: the eye icon hides the HUD, F9 writes a full-resolution PNG without HUD to <userdata>/screenshots, and
# Escape leaves the mode. F9 outside photo mode does nothing, so photo mode is confirmed first (without it Escape
# would open the game menu instead). Returns the text for the output.
function Save-PhotoModeShot {
    param([string]$Target)
    if (-not $Target) { throw 'photo needs a path for the picture: tf3ctl.ps1 photo <png>' }
    if ($EyePoint.Count -notin 0, 2) { throw '-EyePoint takes two numbers: x,y' }
    $target = [IO.Path]::GetFullPath($Target, (Get-Location).ProviderPath)
    $folder = Join-Path (Get-UserDataFolder $UserDataFolder) 'screenshots'
    $known = @{}
    if (Test-Path -LiteralPath $folder -PathType Container) {
        foreach ($file in Get-ChildItem -LiteralPath $folder -Filter '*.png') { $known[$file.Name] = $true }
    }
    $before = Get-GameShot
    # Everything the checks need is looked up before the first click, so that a missing value stops the command early.
    $eye = if ($EyePoint.Count -eq 2) { $EyePoint } else { Get-CalibrationValue (Get-Calibration $before.Height) 'Points.Eye' }
    $null = Get-ScreenCalibration $before 'PhotoMode'
    $state = Get-ScreenState $before
    if ($state -ne 'other') { throw "Photo mode starts in the running game with the menu closed, but the screen shows '$state'." }
    Send-Click $eye[0] $eye[1]
    Start-Sleep -Seconds 2
    if (-not (Test-PhotoMode (Get-GameShot))) {
        throw "No photo mode after the click at $($eye -join ','): the hint box (Esc Back, F9 Screenshot) is missing. A window may be open, or the point is wrong."
    }
    try {
        $since = [datetime]::UtcNow.AddSeconds(-1)
        Send-Key 'F9'
        $new = $null
        $deadline = [datetime]::UtcNow.AddSeconds(30)
        # Right after another window was in front the game can miss the first F9 (seen 2026-10-24), so it is
        # pressed again after 8 and 16 s without a picture; the game takes about 2 s to write one.
        $repeats = @([datetime]::UtcNow.AddSeconds(8), [datetime]::UtcNow.AddSeconds(16))
        while ((-not $new) -and ([datetime]::UtcNow -lt $deadline)) {
            if ($repeats.Count -and ([datetime]::UtcNow -ge $repeats[0])) {
                Send-Key 'F9'
                $repeats = @($repeats | Select-Object -Skip 1)
            }
            Start-Sleep -Milliseconds 500
            if (Test-Path -LiteralPath $folder -PathType Container) {
                $new = Get-ChildItem -LiteralPath $folder -Filter '*.png' |
                    Where-Object { (-not $known.ContainsKey($_.Name)) -and ($_.LastWriteTimeUtc -ge $since) } |
                    Sort-Object LastWriteTimeUtc | Select-Object -Last 1
            }
        }
        if (-not $new) { throw "No new picture in '$folder' within 30 s of F9." }
        # The game may still be writing it: wait until the size stays the same and the file opens.
        $size = -1
        for ($try = 0; $try -lt 40; $try++) {
            $new.Refresh()
            if ($new.Length -eq $size) {
                try { [IO.File]::Open($new.FullName, 'Open', 'Read', 'Read').Dispose(); break } catch { }
            }
            $size = $new.Length
            Start-Sleep -Milliseconds 500
        }
        New-Item -ItemType Directory -Force -Path ([IO.Path]::GetDirectoryName($target)) | Out-Null
        Copy-Item -LiteralPath $new.FullName -Destination $target -Force
        $picture = Get-PngSize $target
        "saved $target ($($picture.Width)x$($picture.Height); from $($new.Name))"
    } finally {
        Send-Key 'ESCAPE'
        Start-Sleep -Seconds 1
    }
}

# The download bar check, or 'not-calibrated' when its set has no DownloadBar, so the other checks still answer.
function Get-DownloadText {
    param($Shot)
    if ($null -eq (Get-CalibrationValue (Get-Calibration $Shot.Height) 'Screen.DownloadBar' -Optional)) { return 'not-calibrated' }
    Test-DownloadBar $Shot
}

function Get-StateText {
    $current = Get-GameShot
    "state=$(Get-ScreenState $current) downloads=$(Get-DownloadText $current)"
}

# Commands that read a saved screenshot: no game and no input, on both platforms.
if ($Cmd -eq 'classify') {
    if (-not $A1) { throw 'classify needs a PNG: tf3ctl.ps1 classify <png>' }
    $saved = Read-Screenshot ([IO.Path]::GetFullPath($A1, (Get-Location).ProviderPath))
    "state=$(Get-ScreenState $saved) downloads=$(Get-DownloadText $saved) photo=$(Test-PhotoMode $saved)"
    return
}
if ($Cmd -eq 'measure') {
    if (-not $A5) { throw 'measure needs a PNG and a box: tf3ctl.ps1 measure <png> <x> <y> <w> <h>' }
    $saved = Read-Screenshot ([IO.Path]::GetFullPath($A1, (Get-Location).ProviderPath))
    $x = [int]$A2; $y = [int]$A3; $width = [int]$A4; $height = [int]$A5
    if (($width -lt 1) -or ($height -lt 1)) { throw '<w> and <h> must be at least 1.' }
    $stats = Get-BoxStats $saved $x ($x + $width - 1) $y ($y + $height - 1)
    "picture $($saved.Width)x$($saved.Height); box x $x..$($x + $width - 1) y $y..$($y + $height - 1) (every second pixel, clipped to the picture)"
    Format-BoxStats $stats
    return
}

if ($IsMacOS) {
    # The same commands through tf3mac.swift (CoreGraphics input, screencapture). Window coordinates
    # are in points; the screenshot is taken at the display's pixel size and scaled to 1280 px.
    $helper = Get-MacHelper
    $proc = Get-Process -Name $Tf3ProcessName -ErrorAction SilentlyContinue | Select-Object -First 1
    if (-not $proc) { throw 'TF3 window not found' }
    $id, $x0, $y0, $w, $h = (& $helper window $proc.Id) -split ' '
    if ($LASTEXITCODE -ne 0) { throw 'TF3 window not found' }
    $scale = [double]$w / 1280
    function Set-Foreground {
        & $helper activate $proc.Id
        if ($LASTEXITCODE -ne 0) { throw 'TF3 is not the foreground window; input refused' }
    }
    function Save-Screenshot {
        param([string]$Target)
        Set-Foreground
        $path = Resolve-OutputPath $Target
        & screencapture -x -o -l $id $path
        if ($LASTEXITCODE -ne 0) { throw 'screencapture failed; is Screen Recording allowed for this terminal?' }
        & sips --resampleWidth 1280 $path | Out-Null
        "saved $path (window ${w}x$h points at $x0,$y0; image 1280x$([int]([double]$h / $scale)))"
    }
    function Send-Click {
        param([int]$X, [int]$Y)
        Set-Foreground
        & $helper click ([int]$x0 + $X * $scale) ([int]$y0 + $Y * $scale)
    }
    function Send-Key {
        param([string]$Name)
        Set-Foreground
        & $helper key $Name 1
    }
    switch ($Cmd) {
        'shot' { Save-Screenshot $A1 }
        'size' { "1280x$([int]([double]$h / $scale))" }
        'state' { Get-StateText }
        'photo' { Save-PhotoModeShot $A1 }
        { $_ -in 'click', 'rclick' } {
            Set-Foreground
            $x = [int]$x0 + [double]$A1 * $scale; $y = [int]$y0 + [double]$A2 * $scale
            & $helper click $x $y $(if ($Cmd -eq 'rclick') { 'right' })
            "$Cmd at screen $([int]$x),$([int]$y)"
        }
        'key' {
            Set-Foreground
            & $helper key $A1 $(if ($A2) { $A2 } else { 1 })
            if ($LASTEXITCODE -ne 0) { throw "unknown key $A1" }
            "key $A1 x$(if ($A2) { $A2 } else { 1 })"
        }
        'type' {
            Set-Foreground
            & $helper type $A1
            "typed $A1"
        }
        default { throw "unknown command $Cmd" }
    }
    return
}

Add-Type -AssemblyName System.Drawing, System.Windows.Forms
if (-not ('Tf3Input' -as [type])) { Add-Type -TypeDefinition @'
using System;
using System.Runtime.InteropServices;
public static class Tf3Input {
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
[Tf3Input]::SetProcessDPIAware() | Out-Null

$proc = Get-Process TransportFever3 -ErrorAction SilentlyContinue | Where-Object { $_.MainWindowHandle -ne 0 } | Select-Object -First 1
if (-not $proc) { throw 'TF3 window not found' }
$hwnd = $proc.MainWindowHandle

function Get-ClientArea {
    $r = New-Object Tf3Input+RECT; [Tf3Input]::GetClientRect($hwnd, [ref]$r) | Out-Null
    $p = New-Object Tf3Input+POINT; [Tf3Input]::ClientToScreen($hwnd, [ref]$p) | Out-Null
    [pscustomobject]@{ X = $p.X; Y = $p.Y; W = $r.R - $r.L; H = $r.B - $r.T }
}

function Set-Foreground {
    if ([Tf3Input]::GetForegroundWindow() -ne $hwnd) {
        [Tf3Input]::ShowWindow($hwnd, 9) | Out-Null
        [Tf3Input]::SetForegroundWindow($hwnd) | Out-Null
        Start-Sleep -Milliseconds 400
    }
    if ([Tf3Input]::GetForegroundWindow() -ne $hwnd) {
        # Windows' foreground lock refuses SetForegroundWindow unless this process sent the last input; a lone Alt tap counts.
        [Tf3Input]::Key(0x12, $false); [Tf3Input]::Key(0x12, $true)
        [Tf3Input]::SetForegroundWindow($hwnd) | Out-Null
        Start-Sleep -Milliseconds 400
    }
    if ([Tf3Input]::GetForegroundWindow() -ne $hwnd) { throw 'TF3 is not the foreground window; input refused' }
}

$scaleWidth = 1280
$area = Get-ClientArea
$scale = $area.W / $scaleWidth

# Saves the 1280 px wide screenshot of the game window; returns the text that the shot command prints.
function Save-Screenshot {
    param([string]$Target, [string]$Mode)
    Set-Foreground
    $bmp = New-Object System.Drawing.Bitmap $area.W, $area.H
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    if ($Mode -eq 'gdi') {
        $g.CopyFromScreen($area.X, $area.Y, 0, 0, $bmp.Size)
    } else {
        # PW_CLIENTONLY | PW_RENDERFULLCONTENT: asks DWM for the window's current content.
        $hdc = $g.GetHdc(); [Tf3Input]::PrintWindow($hwnd, $hdc, 3) | Out-Null; $g.ReleaseHdc($hdc)
    }
    $h = [int]($area.H / $scale)
    $small = New-Object System.Drawing.Bitmap $bmp, $scaleWidth, $h
    $path = [IO.Path]::GetFullPath($Target, (Get-Location).ProviderPath)
    New-Item -ItemType Directory -Force -Path ([IO.Path]::GetDirectoryName($path)) | Out-Null
    $small.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
    $small.Dispose(); $g.Dispose(); $bmp.Dispose()
    "saved $path (client $($area.W)x$($area.H) at $($area.X),$($area.Y); image ${scaleWidth}x$h)"
}

function Send-Click {
    param([int]$X, [int]$Y)
    Set-Foreground
    [Tf3Input]::SetCursorPos($area.X + [int]($X * $scale), $area.Y + [int]($Y * $scale)) | Out-Null; Start-Sleep -Milliseconds 120
    [Tf3Input]::Mouse(0x0002); Start-Sleep -Milliseconds 60; [Tf3Input]::Mouse(0x0004)
}

function Send-Key {
    param([string]$Name)
    Set-Foreground
    $vk = [uint16][System.Windows.Forms.Keys]::$Name
    [Tf3Input]::Key($vk, $false); Start-Sleep -Milliseconds 60; [Tf3Input]::Key($vk, $true); Start-Sleep -Milliseconds 120
}

switch ($Cmd) {
    'shot' { Save-Screenshot $A1 $A2 }
    'size' { "${scaleWidth}x$([int]($area.H / $scale))" }
    'state' { Get-StateText }
    'photo' { Save-PhotoModeShot $A1 }
    { $_ -in 'click', 'rclick' } {
        Set-Foreground
        $x = $area.X + [int]([double]$A1 * $scale); $y = $area.Y + [int]([double]$A2 * $scale)
        [Tf3Input]::SetCursorPos($x, $y) | Out-Null; Start-Sleep -Milliseconds 120
        $down, $up = if ($Cmd -eq 'click') { 0x0002, 0x0004 } else { 0x0008, 0x0010 }
        [Tf3Input]::Mouse($down); Start-Sleep -Milliseconds 60; [Tf3Input]::Mouse($up)
        "$Cmd at screen $x,$y"
    }
    'key' {
        Set-Foreground
        $vk = [uint16][System.Windows.Forms.Keys]::$A1
        $count = if ($A2) { [int]$A2 } else { 1 }
        for ($i = 0; $i -lt $count; $i++) { [Tf3Input]::Key($vk, $false); Start-Sleep -Milliseconds 60; [Tf3Input]::Key($vk, $true); Start-Sleep -Milliseconds 120 }
        "key $A1 x$count"
    }
    'type' {
        Set-Foreground
        foreach ($c in $A1.ToCharArray()) { [Tf3Input]::Char($c, $false); [Tf3Input]::Char($c, $true); Start-Sleep -Milliseconds 40 }
        "typed $A1"
    }
    default { throw "unknown command $Cmd" }
}
