$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$root = $PSScriptRoot
$desktop = Get-Content (Join-Path $root 'artifacts/latest-desktop-results-windows.json') -Raw | ConvertFrom-Json
$repoRoot = (Resolve-Path (Join-Path $root '../../..')).Path
[xml]$xaml = Get-Content (Join-Path $repoRoot 'companion/Tf3Drpc.App/App.xaml') -Raw
$style = $xaml.SelectNodes("//*[local-name()='Style']") | Where-Object { $_.GetAttribute('Key', 'http://schemas.microsoft.com/winfx/2009/xaml') -eq 'SecondaryLabel' }
$setter = @($style.ChildNodes | Where-Object { $_.Property -eq 'TextColor' })[0]
if ($setter.Value -ne 'Gray') { throw 'The reviewed foreground style changed; re-evaluate this case.' }
$foreground = [System.Drawing.Color]::Gray

function Relative-Luminance($colour) {
    $values = @()
    foreach ($channel in @($colour.R, $colour.G, $colour.B)) {
        $normal = $channel / 255.0
        $values += $(if ($normal -le 0.04045) { $normal / 12.92 } else { [Math]::Pow(($normal + 0.055) / 1.055, 2.4) })
    }
    0.2126 * $values[0] + 0.7152 * $values[1] + 0.0722 * $values[2]
}
function Contrast($first, $second) {
    $a = Relative-Luminance $first
    $b = Relative-Luminance $second
    ([Math]::Max($a, $b) + 0.05) / ([Math]::Min($a, $b) + 0.05)
}

$bitmap = [System.Drawing.Bitmap]::FromFile((Join-Path $root 'artifacts/native-settings-normal.png'))
try {
    # The results are in device-independent pixels and the screenshot in physical ones.
    $scale = $bitmap.Width / $desktop.normal.window.width
    $rect = $desktop.secondaryLabel.rect
    $left = [int](($rect.x - $desktop.normal.window.x) * $scale)
    $top = [int](($rect.y - $desktop.normal.window.y) * $scale)
    $right = [int]($left + ($rect.width - 5) * $scale)
    $height = [int]($rect.height * $scale)
    $background = $bitmap.GetPixel($right, [int]($top + $height / 2))
    $solidForegroundPixels = 0
    for ($y = $top; $y -lt ($top + $height); $y++) {
        for ($x = $left; $x -lt $right; $x++) {
            $pixel = $bitmap.GetPixel($x, $y)
            if ($pixel.R -eq $foreground.R -and $pixel.G -eq $foreground.G -and $pixel.B -eq $foreground.B) { $solidForegroundPixels++ }
        }
    }
    if ($solidForegroundPixels -lt 20) { throw 'The screenshot does not confirm the reviewed foreground colour.' }
    $ratio = Contrast $foreground $background
    if ([Math]::Abs((Contrast ([System.Drawing.Color]::Black) ([System.Drawing.Color]::White)) - 21.0) -gt 0.000001) { throw 'Contrast formula control failed.' }
    $result = [ordered]@{
        id = 'R12'; foreground = ('#{0:X2}{1:X2}{2:X2}' -f $foreground.R, $foreground.G, $foreground.B)
        background = ('#{0:X2}{1:X2}{2:X2}' -f $background.R, $background.G, $background.B)
        ratio = $ratio; minimumForNormalText = 4.5; solidForegroundPixels = $solidForegroundPixels
        label = $desktop.secondaryLabel; reproduced = ($ratio -lt 4.5)
        blackWhiteControl = 21.0
    }
    $result | ConvertTo-Json -Depth 6 | Set-Content (Join-Path $root 'artifacts/latest-contrast-results-windows.json') -Encoding UTF8
    $result | ConvertTo-Json -Depth 6
    if ($ratio -ge 4.5) { throw 'The reviewed low-contrast condition was not reproduced.' }
}
finally { $bitmap.Dispose() }
