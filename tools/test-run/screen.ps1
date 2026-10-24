<#
.SYNOPSIS
Pixel checks on a tf3ctl.ps1 screenshot (1280 px wide). Dot-source it after common.ps1: . "$PSScriptRoot\screen.ps1"

.DESCRIPTION
Tells which screen of Transport Fever 3 a screenshot shows, so that a script need not click blind.
The checks look at the layout (dark grey panels, white icons, a grey and blue bar), never at text,
so they work in every language. Pixels are read with System.Drawing on Windows and with `tf3mac pixels` (ImageIO)
on macOS. Every box and threshold comes from calibration.psd1, the set for the platform and the height of the
screenshot (Windows: 16:9, UI scale 100%, builds 40423 and 40425, the 1280x720 picture). A check whose values are not
calibrated stops with a message naming them; another aspect ratio or UI scale needs a set of its own.
-Verbose prints the numbers behind every answer.

  Read-Screenshot <png>           the pixels of a PNG
  Get-BoxStats <shot> x0 x1 y0 y1 the statistics of a box (what `tf3ctl.ps1 measure` prints)
  Test-DownloadBar <shot>         the Mod Hub download bar under the account box of the main menu
  Test-MainMenu <shot>            the main menu (its four icons at the top right)
  Test-GameMenuOpen <shot>        the game menu in a running game (five dark buttons at the left)
  Test-QuitFlyoutOpen <shot>      the two buttons beside "Quit" (Return to Main Menu, Return to Desktop)
  Test-PhotoMode <shot>           photo mode (the hint box at the bottom right)
  Get-ScreenState <shot>          quit-flyout, game-menu, main-menu or other

The map editor's game menu has one more entry, so Test-GameMenuOpen is not calibrated for it.
#>

# The pixels of a PNG as B, G, R, A bytes, row after row (Width, Height, Stride, Bytes); the file isn't kept open.
function Read-Screenshot {
    param([Parameter(Mandatory)][string]$Path)
    if ($IsMacOS) {
        # tf3mac draws the picture into an sRGB BGRA buffer and writes it as raw bytes; stdout has "<width> <height>".
        $helper = Get-MacHelper
        $temp = [IO.Path]::GetTempFileName()
        try {
            $size = (& $helper pixels $Path $temp) -split ' '
            if ($LASTEXITCODE -ne 0) { throw "tf3mac could not read '$Path'." }
            $width = [int]$size[0]; $height = [int]$size[1]
            $buffer = [IO.File]::ReadAllBytes($temp)
            if ($buffer.Length -ne $width * $height * 4) { throw "tf3mac wrote $($buffer.Length) bytes for a ${width}x$height picture." }
            return [pscustomobject]@{ Width = $width; Height = $height; Stride = $width * 4; Bytes = $buffer }
        } finally {
            Remove-Item -LiteralPath $temp -ErrorAction SilentlyContinue
        }
    }
    Add-Type -AssemblyName System.Drawing
    $stream = [IO.MemoryStream]::new([IO.File]::ReadAllBytes($Path))
    $bitmap = [Drawing.Bitmap]::new($stream)
    try {
        $rect = [Drawing.Rectangle]::new(0, 0, $bitmap.Width, $bitmap.Height)
        $data = $bitmap.LockBits($rect, [Drawing.Imaging.ImageLockMode]::ReadOnly, [Drawing.Imaging.PixelFormat]::Format32bppArgb)
        $buffer = [byte[]]::new($data.Stride * $bitmap.Height)
        [Runtime.InteropServices.Marshal]::Copy($data.Scan0, $buffer, 0, $buffer.Length)
        $bitmap.UnlockBits($data)
        [pscustomobject]@{ Width = $bitmap.Width; Height = $bitmap.Height; Stride = $data.Stride; Bytes = $buffer }
    } finally {
        $bitmap.Dispose()
        $stream.Dispose()
    }
}

# The mean colour, the largest standard deviation of a channel, the number of bright pixels (every channel at
# least 200) and the number of samples in a box (inclusive, every second pixel; the box is clipped to the picture).
# $null for an empty box.
function Get-BoxStats {
    param($Shot, [int]$X0, [int]$X1, [int]$Y0, [int]$Y1)
    $X0 = [Math]::Max($X0, 0); $Y0 = [Math]::Max($Y0, 0)
    $X1 = [Math]::Min($X1, $Shot.Width - 1); $Y1 = [Math]::Min($Y1, $Shot.Height - 1)
    if ($X1 -lt $X0 -or $Y1 -lt $Y0) { return $null }
    $bytes = $Shot.Bytes
    $n = 0; $bright = 0
    $sum = [double[]]::new(3); $square = [double[]]::new(3)
    for ($y = $Y0; $y -le $Y1; $y += 2) {
        for ($x = $X0; $x -le $X1; $x += 2) {
            $i = $y * $Shot.Stride + $x * 4
            $r = $bytes[$i + 2]; $g = $bytes[$i + 1]; $b = $bytes[$i]
            $sum[0] += $r; $sum[1] += $g; $sum[2] += $b
            $square[0] += $r * $r; $square[1] += $g * $g; $square[2] += $b * $b
            if ($r -ge 200 -and $g -ge 200 -and $b -ge 200) { $bright++ }
            $n++
        }
    }
    $deviation = 0.0
    for ($c = 0; $c -lt 3; $c++) {
        $mean = $sum[$c] / $n
        $deviation = [Math]::Max($deviation, [Math]::Sqrt([Math]::Max(0.0, $square[$c] / $n - $mean * $mean)))
    }
    [pscustomobject]@{ R = $sum[0] / $n; G = $sum[1] / $n; B = $sum[2] / $n; Deviation = $deviation; Bright = $bright; Samples = $n }
}

# The statistics as one line of text (the numbers the checks compare), or "empty box".
function Format-BoxStats {
    param($Stats)
    if ($null -eq $Stats) { return 'empty box' }
    $culture = [Globalization.CultureInfo]::InvariantCulture
    $high = [Math]::Max($Stats.R, [Math]::Max($Stats.G, $Stats.B))
    $low = [Math]::Min($Stats.R, [Math]::Min($Stats.G, $Stats.B))
    'mean {0},{1},{2} max-channel {3} spread {4} deviation {5} bright {6} of {7} samples' -f `
        $Stats.R.ToString('0.0', $culture), $Stats.G.ToString('0.0', $culture), $Stats.B.ToString('0.0', $culture),
        $high.ToString('0.0', $culture), ($high - $low).ToString('0.0', $culture), $Stats.Deviation.ToString('0.0', $culture),
        $Stats.Bright, $Stats.Samples
}

# The values of one screen check for the screenshot's height; stops if they are not calibrated.
function Get-ScreenCalibration {
    param($Shot, [Parameter(Mandatory)][string]$Section)
    Get-CalibrationValue (Get-Calibration $Shot.Height) "Screen.$Section"
}

# True for a flat dark grey box: the panels of the game menu (calibrated 37..46 per channel, deviation up to 4).
function Test-DarkPanel {
    param($Stats, $Limits)
    if ($null -eq $Stats) { return $false }
    $high = [Math]::Max($Stats.R, [Math]::Max($Stats.G, $Stats.B))
    $low = [Math]::Min($Stats.R, [Math]::Min($Stats.G, $Stats.B))
    ($high -le $Limits.MaxHigh) -and (($high - $low) -le $Limits.MaxSpread) -and ($Stats.Deviation -le $Limits.MaxDeviation)
}

# The download bar of the Mod Hub under the account box at the top left: a grey track (46,54,61) with a blue
# fill (77,147,218 to 146,200,253) in its left part, about x 80..310 and 9 px high. The rows 100..130 are
# scanned, because the account box can wrap onto a second line in a language with longer texts. A row counts when
# 60% of its samples are track or fill (the text on the bar spoils the rows through it); the bar is 11 rows high, so six such rows
# are needed, which a card of the Load Game screen (two rows) does not give.
function Test-DownloadBar {
    param($Shot)
    $c = Get-ScreenCalibration $Shot 'DownloadBar'
    $rows = 0
    for ($y = $c.Y0; $y -le [Math]::Min($c.Y1, $Shot.Height - 1); $y++) {
        $hit = 0; $samples = 0
        for ($x = $c.X0; $x -le $c.X1; $x += $c.XStep) {
            $i = $y * $Shot.Stride + $x * 4
            $r = [int]$Shot.Bytes[$i + 2]; $g = [int]$Shot.Bytes[$i + 1]; $b = [int]$Shot.Bytes[$i]
            $track = ([Math]::Abs($r - $c.Track[0]) -le $c.TrackTolerance) -and ([Math]::Abs($g - $c.Track[1]) -le $c.TrackTolerance) -and ([Math]::Abs($b - $c.Track[2]) -le $c.TrackTolerance)
            $fill = ($b -ge $c.FillMinBlue) -and (($b - $r) -ge $c.FillBlueOverRed)
            if ($track -or $fill) { $hit++ }
            $samples++
        }
        if ($hit -ge [Math]::Ceiling($c.RowFraction * $samples)) { $rows++ }
    }
    $found = $rows -ge $c.MinRows
    Write-Verbose "DownloadBar: $rows rows of track or fill between y $($c.Y0) and $($c.Y1) (needs $($c.MinRows)): $found"
    $found
}

# The game menu: the first four of the five buttons are flat dark grey panels (x 75..410, centres at y 235, 297,
# 360 and 422) with a white icon at x 160..190 (about 65 sampled bright pixels; the settings screens have up to 6). The fifth, "Quit", turns blue while its flyout is open, so it isn't used.
function Test-GameMenuOpen {
    param($Shot)
    $c = Get-ScreenCalibration $Shot 'GameMenu'
    $dark = Get-ScreenCalibration $Shot 'DarkPanel'
    $open = $true
    foreach ($y in $c.RowYs) {
        $panel = Get-BoxStats $Shot $c.PanelX0 $c.PanelX1 ($y - $c.HalfHeight) ($y + $c.HalfHeight)
        $panelOk = Test-DarkPanel $panel $dark
        $icon = Get-BoxStats $Shot $c.IconX0 $c.IconX1 ($y - $c.HalfHeight) ($y + $c.HalfHeight)
        $iconOk = ($null -ne $icon) -and ($icon.Bright -ge $c.MinIconBright)
        Write-Verbose "GameMenu y $y panel $(Format-BoxStats $panel): dark $panelOk"
        Write-Verbose "GameMenu y $y icon $(Format-BoxStats $icon): bright >= $($c.MinIconBright) $iconOk"
        if (-not ($panelOk -and $iconOk)) { $open = $false }
    }
    $open
}

# The flyout of "Quit": two dark buttons in the row of "Quit" (Points.Quit, or the row given).
function Test-QuitFlyoutOpen {
    param($Shot, [int]$Y = 0)
    $c = Get-ScreenCalibration $Shot 'QuitFlyout'
    $dark = Get-ScreenCalibration $Shot 'DarkPanel'
    if ($Y -le 0) { $Y = (Get-CalibrationValue (Get-Calibration $Shot.Height) 'Points.Quit')[1] }
    $first = Get-BoxStats $Shot $c.Button1X0 $c.Button1X1 ($Y - $c.HalfHeight) ($Y + $c.HalfHeight)
    $second = Get-BoxStats $Shot $c.Button2X0 $c.Button2X1 ($Y - $c.HalfHeight) ($Y + $c.HalfHeight)
    $firstOk = Test-DarkPanel $first $dark
    $secondOk = Test-DarkPanel $second $dark
    Write-Verbose "QuitFlyout y $Y first $(Format-BoxStats $first): dark $firstOk"
    Write-Verbose "QuitFlyout y $Y second $(Format-BoxStats $second): dark $secondOk"
    $firstOk -and $secondOk
}

# The main menu: settings, achievements, news and exit icons at the top right (x 1064..1203, y 36..64), each with
# a good deal of white. The game menu has white text in that corner, but at most 25 sampled pixels per box (the icons have 46 or more).
function Test-MainMenu {
    param($Shot)
    $c = Get-ScreenCalibration $Shot 'MainMenu'
    $found = $true
    foreach ($range in $c.IconXs) {
        $icon = Get-BoxStats $Shot $range[0] $range[1] $c.Y0 $c.Y1
        $ok = ($null -ne $icon) -and ($icon.Bright -ge $c.MinBright)
        Write-Verbose "MainMenu icon x $($range[0])..$($range[1]) $(Format-BoxStats $icon): bright >= $($c.MinBright) $ok"
        if (-not $ok) { $found = $false }
    }
    $found
}

# quit-flyout and game-menu are in a running game, main-menu is the main menu, other is anything else (the game
# without a menu, a window, a loading screen, the settings screen). -QuitY is the row of "Quit" if it isn't the calibrated one.
function Get-ScreenState {
    param($Shot, [int]$QuitY = 0)
    if (Test-GameMenuOpen $Shot) {
        if (Test-QuitFlyoutOpen $Shot -Y $QuitY) { return 'quit-flyout' }
        return 'game-menu'
    }
    if (Test-MainMenu $Shot) { return 'main-menu' }
    'other'
}

# Photo mode (the eye icon of a running game): the HUD is gone and a hint box with two dark buttons ("Esc Back",
# "F9 Screenshot") sits at the bottom right. The pills are translucent, so the check is only for three dark
# patches in them (calibrated: 41..54 per channel); a night scene without HUD could look alike, so a script
# confirms photo mode by the file that F9 writes. The optional MinHigh keeps out the black band of the loading screen.
function Test-PhotoMode {
    param($Shot)
    $c = Get-ScreenCalibration $Shot 'PhotoMode'
    $minHigh = if ($c.ContainsKey('MinHigh')) { $c.MinHigh } else { 0 }
    $found = $true
    foreach ($box in $c.Boxes) {
        $stats = Get-BoxStats $Shot $box[0] $box[1] $box[2] $box[3]
        $ok = $false
        if ($null -ne $stats) {
            $high = [Math]::Max($stats.R, [Math]::Max($stats.G, $stats.B))
            $low = [Math]::Min($stats.R, [Math]::Min($stats.G, $stats.B))
            $ok = ($high -ge $minHigh) -and ($high -le $c.MaxHigh) -and (($high - $low) -le $c.MaxSpread)
        }
        Write-Verbose "PhotoMode patch x $($box[0])..$($box[1]) y $($box[2])..$($box[3]) $(Format-BoxStats $stats): dark $ok"
        if (-not $ok) { $found = $false }
    }
    $found
}
