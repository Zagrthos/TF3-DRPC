<#
Switches the presets of the running TF3-DRPC through UI Automation (WinApp CLI 0.7), without the
game, and checks the Custom section, the preview's second line, settings.json and screenshots.
  ui-tests.ps1 -AppFolder <folder of the published app> [-Language <game code>] [-OutFolder <folder>]
-AppFolder is the published copy the running app was started from; its DATA\settings.json is read and
must exist (a fresh app writes it only after a setting changes, e.g. the theme switched and back).
-Language is the language the app shows, as a game code (de, en, es, fr, it, ja, ko, nl, pl, pt_BR, ru,
zh_CN, zh_TW). Without it the language setting of settings.json counts, and "game" (follow the game) means
the language in the game's profile.lua; with no game installed that is English.
-OutFolder gets the screenshots (default: artifacts\ui-tests next to the scripts).
No text of one language is in the script: the names of the picker, its entries and the labels come from the
app's resx files by key (common.ps1: Get-AppText), and the sample date from the culture's format (Format-AppDate).
The app must show the Career preset first, with the sample values (September 1905) that it shows when no
game runs; the game must not have written a status to the app. Open the settings window first (start the exe
a second time).
Example: ./ui-tests.ps1 -AppFolder <folder of the published app> -Language ja
#>
param(
    [Parameter(Mandatory)][string]$AppFolder,
    [string]$Language,
    [string]$OutFolder = (Join-Path $PSScriptRoot 'artifacts\ui-tests')
)

$ErrorActionPreference = 'Stop'
. "$PSScriptRoot\common.ps1"
Set-TestLanguage -Language $Language -AppFolder $AppFolder
# The example date the preview shows without a game (1905-09), in the culture's own format.
$sampleDate = Format-AppDate 1905 9
# The words of the private-data hint before its colon (ASCII or full-width), which the Custom section shows.
$privateHintStart = (Get-AppText PrivateHint) -split '\s*[:：]', 2 | Select-Object -First 1
$SettingsFile = Join-Path $AppFolder 'DATA\settings.json'
$ArtifactDirectory = [IO.Path]::GetFullPath($OutFolder, (Get-Location).ProviderPath)
$PSNativeCommandUseErrorActionPreference = $false
$env:WINAPP_UI_WORKFLOW_ID = [guid]::NewGuid().ToString('N')
$results = [Collections.Generic.List[object]]::new()
$screenshots = [Collections.Generic.List[string]]::new()

function Invoke-WinAppChecked {
    param([string[]]$Arguments)
    $global:LASTEXITCODE = $null
    $output = @(& winapp @Arguments 2>$null)
    $code = $global:LASTEXITCODE
    if ($null -eq $code -or $code -ne 0) {
        throw "winapp $($Arguments -join ' ') failed (exit $code). $($output -join "`n")"
    }
    $output
}

function Invoke-Ui {
    param([string[]]$Arguments)
    Invoke-WinAppChecked (@('ui') + $Arguments + @('-w', $hwnd))
}

function Test-UI {
    param([string]$Name, [scriptblock]$Script)
    try {
        & $Script | Out-Null
        $results.Add(@{ name = $Name; status = 'PASS' })
    } catch {
        $results.Add(@{ name = $Name; status = 'FAIL'; detail = "$_" })
    }
}

function Save-Screenshot {
    param([string]$Name)
    $path = Join-Path $ArtifactDirectory $Name
    Invoke-Ui @('screenshot', '--output', $path) | Out-Null
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) { throw "No screenshot: $path" }
    $screenshots.Add($path)
}

function Get-Picker {
    $json = (Invoke-Ui @('inspect', '--interactive', '--json', '--depth', '12')) -join "`n" | ConvertFrom-Json
    function Get-Elements($nodes) { foreach ($node in $nodes) { if ($null -ne $node) { $node; Get-Elements $node.children } } }
    $picker = Get-Elements @($json.windows | ForEach-Object { $_.elements }) |
        Where-Object { $_.type -eq 'ComboBox' -and $_.name -eq (Get-AppText StatusAppearance) } | Select-Object -First 1
    if (-not $picker) { throw "No combo box '$(Get-AppText StatusAppearance)'" }
    $picker
}

function Get-PickerValue {
    $picker = Get-Picker
    (Invoke-Ui @('get-value', $picker.selector, '--json') -join "`n" | ConvertFrom-Json).text
}

function Select-Preset {
    param([string]$Preset)
    $Item = Get-PresetText $Preset
    Invoke-Ui @('invoke', (Get-Picker).selector, '--action', 'expand') | Out-Null
    $itemSlug = $null
    for ($try = 0; ($try -lt 15) -and (-not $itemSlug); $try++) {
        if ($try) { Start-Sleep -Milliseconds 200 }
        $global:LASTEXITCODE = $null
        $found = @(& winapp ui search $Item --json -w $hwnd 2>$null)
        if ($found.Count) {
            $itemSlug = ($found -join "`n" | ConvertFrom-Json).matches |
                Where-Object { $_.type -eq 'ListItem' } | Select-Object -First 1 -ExpandProperty selector
        }
    }
    if (-not $itemSlug) { throw "No list item '$Item'" }
    Invoke-Ui @('invoke', $itemSlug, '--action', 'select') | Out-Null
    $picker = Get-Picker
    if ($picker.expandState -eq 'expanded') { Invoke-Ui @('invoke', $picker.selector, '--action', 'collapse') | Out-Null }
    $value = Get-PickerValue
    if ($value -ne $Item) { throw "Picker shows '$value', expected '$Item'" }
}

function Assert-Saved {
    param([string]$Preset)
    $deadline = (Get-Date).AddSeconds(3)
    do {
        $saved = (Get-Content -LiteralPath $SettingsFile -Raw | ConvertFrom-Json).preset
        if ($saved -eq $Preset) { return }
        Start-Sleep -Milliseconds 200
    } while ((Get-Date) -lt $deadline)
    throw "settings.json preset is '$saved', expected '$Preset'"
}

# The number of elements named like the text; wait-for can't be used for the first line's label, because the
# label and the template field (its accessible name) both match and wait-for fails on two matches.
function Get-MatchCount {
    param([string]$Text)
    $found = @(& winapp ui search $Text --json -w $hwnd 2>$null)
    if (-not $found.Count) { return 0 }
    @(($found -join "`n" | ConvertFrom-Json).matches).Count
}

function Assert-Custom {
    param([bool]$Visible)
    $deadline = (Get-Date).AddSeconds(3)
    do {
        $count = Get-MatchCount (Get-AppText TemplateFirstLine)
        if ($Visible -eq ($count -gt 0)) { break }
        Start-Sleep -Milliseconds 200
    } while ((Get-Date) -lt $deadline)
    if ($Visible -ne ($count -gt 0)) { throw "'$(Get-AppText TemplateFirstLine)' found $count times, expected the Custom section visible = $Visible" }
    if ($Visible) { Invoke-Ui @('wait-for', $privateHintStart, '--contains', '-t', '3000') | Out-Null }
}

function Assert-StateLine {
    param([bool]$Visible)
    if ($Visible) {
        Invoke-Ui @('wait-for', $sampleDate, '-t', '3000') | Out-Null
    } else {
        Invoke-Ui @('wait-for', $sampleDate, '--gone', '-t', '3000') | Out-Null
    }
}

try {
    Assert-WinApp
    if (-not (Test-Path -LiteralPath $SettingsFile -PathType Leaf)) { throw "No settings file '$SettingsFile'; is -AppFolder the folder the app runs from?" }
    New-Item -ItemType Directory -Force -Path $ArtifactDirectory | Out-Null
    $windows = @(Invoke-WinAppChecked @('ui', 'list-windows', '-a', "$((Get-AppProcess).Id)", '--json') | ConvertFrom-Json)
    $hwnd = [string]($windows | Where-Object { $_.title -eq 'TF3-DRPC' } | Select-Object -First 1).hwnd
    if (-not $hwnd) { throw 'No TF3-DRPC window' }

    $steps = @(
        @{ Item = 'career'; Key = 'career'; Custom = $false; State = $true; Select = $true },
        @{ Item = 'custom'; Key = 'custom'; Custom = $true; State = $true; Select = $true },
        @{ Item = 'network'; Key = 'network'; Custom = $false; State = $false; Select = $true },
        @{ Item = 'custom'; Key = 'custom'; Custom = $true; State = $true; Select = $true },
        @{ Item = 'minimal'; Key = 'minimal'; Custom = $false; State = $false; Select = $true },
        @{ Item = 'career'; Key = 'career'; Custom = $false; State = $true; Select = $true }
    )

    for ($i = 0; $i -lt $steps.Count; $i++) {
        $step = $steps[$i]
        $n = $i + 1
        if ($step.Select) {
            Test-UI "$n. select $($step.Item)" { Select-Preset $step.Item }
        } else {
            Test-UI "$n. starts on $($step.Item)" {
                $picker = Get-Picker
                if ($picker.expandState -eq 'expanded') { Invoke-Ui @('invoke', $picker.selector, '--action', 'collapse') | Out-Null }
                $value = Get-PickerValue
                if ($value -ne (Get-PresetText $step.Item)) { throw "Picker shows '$value'" }
            }
        }
        Test-UI "$n. $($step.Item): Custom section visible = $($step.Custom)" { Assert-Custom $step.Custom }
        Test-UI "$n. $($step.Item): preview second line (the sample date) visible = $($step.State)" { Assert-StateLine $step.State }
        Test-UI "$n. $($step.Item): settings.json preset = $($step.Key)" { Assert-Saved $step.Key }
        Test-UI "$n. $($step.Item): screenshot" { Save-Screenshot ('{0:D2}-{1}.png' -f $n, $step.Key) }
    }
} catch {
    $results.Add(@{ name = 'setup'; status = 'FAIL'; detail = "$_" })
} finally {
    try { winapp ui yield | Out-Null } catch { }
}

foreach ($r in $results) { '{0}  {1}{2}' -f $r.status, $r.name, $(if ($r.detail) { " -- $($r.detail)" } else { '' }) }
$failed = @($results | Where-Object { $_.status -eq 'FAIL' }).Count
"Passed: $($results.Count - $failed) | Failed: $failed"
