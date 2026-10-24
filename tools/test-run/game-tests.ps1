<#
The in-game checks: the running TF3-DRPC settings window against a running game with the mod.
  game-tests.ps1 -Phase presets|alternating|rerun|custom -AppFolder <folder of the published app>
                 [-Language <game code>] [-SaveName <name>] [-UserDataFolder <...\3493540\local>]
                 [-StoreButtonText <text>]

Phases
  presets      Network, Era and Minimal: picker, preview, what is sent, settings.json; then alternating
  alternating  Alternating rotates through Career, Network and Era (150 s); then Career again
  rerun        Minimal, then alternating (after a failed presets run)
  custom       the three custom templates, the preview, what is sent, settings.json and both pause
               paths (pause button and game menu); needs -SaveName

-AppFolder   the published copy the running app was started from; its DATA\settings.json is read.
-Language    the language the app shows, as a game code (de, en, es, fr, it, ja, ko, nl, pl, pt_BR, ru, zh_CN,
             zh_TW). Without it: the language setting in <AppFolder>\DATA\settings.json, and when that is "game"
             (the app follows the game) the language in the game's profile.lua. Set the game's language with
             run-game.ps1 prepare -Language.
-SaveName    the name of the savegame that is loaded, as the status line's saveName; without it the
             last status line of the game log is used. Put your own save's name here if that fails.
-StoreButtonText  the label of the store button in the app's language; without it the app's own text
             (StoreButton in Texts.<language>.resx). The preview shows it as a last line when the store
             button is ticked in settings.json.

No text of one language is in this script. The names of the picker and its entries come from the app's resx
files by key (common.ps1: Get-AppText), the status patterns from Get-PresencePatterns: the app's own word
("In game") exactly, the date in the app's culture format, the game's words (labels, ranks, climates, "Paused")
as words without digits, counts and the year as digits, and the separators " · " and " | ".
Start the game with run-game.ps1, load a savegame with the mod, show the settings window (start the app a
second time), and leave the game in the foreground afterwards: the custom phase presses Space and Escape in it.
The game keeps running, so the checks use patterns, not fixed values.
Example: ./game-tests.ps1 -Phase presets -AppFolder <folder of the published app>
#>
param(
    [Parameter(Mandatory)][ValidateSet('presets', 'alternating', 'rerun', 'custom')][string]$Phase,
    [Parameter(Mandatory)][string]$AppFolder,
    [string]$Language,
    [string]$SaveName,
    [string]$UserDataFolder,
    [string]$StoreButtonText
)

$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $false
. "$PSScriptRoot\common.ps1"

Assert-WinApp
if (-not (Test-Path -LiteralPath $AppFolder -PathType Container)) { throw "-AppFolder '$AppFolder' doesn't exist." }
Set-TestLanguage -Language $Language -AppFolder $AppFolder -UserDataFolder $UserDataFolder
if (-not $StoreButtonText) { $StoreButtonText = Get-AppText StoreButton }
$SettingsFile = Join-Path $AppFolder 'DATA\settings.json'
$results = [Collections.Generic.List[object]]::new()

$app = Get-AppProcess
$hwnd = [string](@(winapp ui list-windows -a $app.Id --json 2>$null | ConvertFrom-Json) | Where-Object { $_.title -eq 'TF3-DRPC' } | Select-Object -First 1).hwnd
if (-not $hwnd) { throw 'settings window not visible; start the app a second time to show it' }

if ($Phase -eq 'custom' -and -not $SaveName) {
    $lastStatus = Get-LastStatus (Get-GameLogPath $UserDataFolder)
    $SaveName = if ($lastStatus) { [string]$lastStatus.Status.saveName } else { '' }
    if (-not $SaveName) { throw 'The custom phase needs -SaveName (the name of the loaded savegame); none in the last status line.' }
}
$patterns = Get-PresencePatterns $SaveName

function Invoke-Ui {
    param([string[]]$Arguments)
    $global:LASTEXITCODE = $null
    $output = @(& winapp ui @Arguments -w $hwnd 2>$null)
    if ($global:LASTEXITCODE -ne 0) { throw "winapp ui $($Arguments -join ' ') failed (exit $global:LASTEXITCODE)" }
    $output
}

function Get-AllElements {
    $json = (Invoke-Ui @('inspect', '--json', '--depth', '14')) -join "`n" | ConvertFrom-Json
    function Get-Elements($nodes) { foreach ($node in $nodes) { if ($null -ne $node) { $node; Get-Elements $node.children } } }
    @(Get-Elements @($json.windows | ForEach-Object { $_.elements }))
}

# The texts in tree order, which follows the XAML: game status, Discord, Preview, preview lines, Theme, Language, ...
# Not sorted by position (it misreads after the settings scrolled) and not de-duplicated (two lines can be equal).
function Get-Texts {
    @(Get-AllElements | Where-Object { $_.type -eq 'Text' -and $_.name } | Select-Object -ExpandProperty name)
}

# The game status is what the app sends to Discord; it is the line above the Discord connection.
function Get-GameStatus {
    $texts = Get-Texts
    $connected = Get-AppText DiscordConnected
    $notConnected = Get-AppText DiscordNotConnected
    for ($i = 1; $i -lt $texts.Count; $i++) {
        if (($texts[$i] -ne $connected) -and ($texts[$i] -ne $notConnected)) { continue }
        # The game build line sits between the status and the Discord line while the game runs.
        $j = $i - 1
        if (($j -gt 0) -and ($texts[$j] -match '^Transport Fever 3 · Build \d+$')) { $j-- }
        return $texts[$j]
    }
    throw 'no Discord line'
}

function Get-Preview {
    $texts = Get-Texts
    $first = [Array]::IndexOf($texts, (Get-AppText Preview))
    $end = [Array]::IndexOf($texts, (Get-AppText Theme))
    if ($first -lt 0 -or $end -le $first) { throw 'no preview block in the settings window' }
    $texts[($first + 1)..($end - 1)] -join ' | '
}

function Get-Picker {
    $name = Get-AppText StatusAppearance
    $picker = Get-AllElements | Where-Object { $_.type -eq 'ComboBox' -and $_.name -eq $name } | Select-Object -First 1
    if (-not $picker) { throw "No combo box '$name'" }
    $picker
}

# -Preset is a key (career, network, era, alternating, minimal, custom); the picker entry is the app's text for it.
function Select-Preset {
    param([string]$Preset)
    $Item = Get-PresetText $Preset
    Invoke-Ui @('invoke', (Get-Picker).selector, '--action', 'expand') | Out-Null
    $itemSlug = $null
    for ($try = 0; ($try -lt 15) -and (-not $itemSlug); $try++) {
        if ($try) { Start-Sleep -Milliseconds 200 }
        $found = @(& winapp ui search $Item --json -w $hwnd 2>$null)
        if ($found.Count) {
            $itemSlug = ($found -join "`n" | ConvertFrom-Json).matches |
                Where-Object { $_.type -eq 'ListItem' } | Select-Object -First 1 -ExpandProperty selector
        }
    }
    if (-not $itemSlug) { throw "No list item '$Item'" }
    Invoke-Ui @('invoke', $itemSlug, '--action', 'select') | Out-Null
    $value = (Invoke-Ui @('get-value', (Get-Picker).selector, '--json') -join "`n" | ConvertFrom-Json).text
    if ($value -ne $Item) { throw "Picker shows '$value', expected '$Item'" }
}

# Waits until the status line (what the app sends to Discord) matches; Discord takes an update every 15 s at most.
function Wait-GameStatus {
    param([string]$Pattern, [int]$Seconds = 40)
    $deadline = (Get-Date).AddSeconds($Seconds)
    $start = Get-Date
    do {
        $status = Get-GameStatus
        if ($status -match $Pattern) { return "after $([int]((Get-Date) - $start).TotalSeconds) s: '$status'" }
        Start-Sleep -Seconds 1
    } while ((Get-Date) -lt $deadline)
    throw "status '$status' doesn't match '$Pattern' after $Seconds s"
}

function Test-Step {
    param([string]$Name, [scriptblock]$Script)
    try {
        $out = & $Script
        $results.Add("PASS  $Name$(if ($out) { " -- $($out -join '; ')" })")
    } catch {
        $results.Add("FAIL  $Name -- $_")
    }
    Write-Host $results[$results.Count - 1]
}

function Get-Saved {
    if (-not (Test-Path -LiteralPath $SettingsFile -PathType Leaf)) { throw "No settings file '$SettingsFile'; is -AppFolder the folder the app runs from?" }
    Get-Content -LiteralPath $SettingsFile -Raw | ConvertFrom-Json
}

# The preview must match the pattern (its lines, then the tooltip line). With the store button on,
# the preview shows the button label between the lines and the tooltip; it must be there exactly
# once and is taken out before matching. With the button off it must be absent.
function Assert-Preview {
    param([string]$Pattern)
    $preview = Get-Preview
    $button = ' | ' + $StoreButtonText + ' | '
    $count = ($preview.Length - $preview.Replace($button, '').Length) / $button.Length
    if ((Get-Saved).storeButton -eq $true) {
        if ($count -ne 1) { throw "store button line expected once: $preview" }
    } elseif ($count -ne 0) {
        throw "store button line without the setting: $preview"
    }
    if ($preview.Replace($button, ' | ') -notmatch ($Pattern + '$')) { throw $preview }
    $preview
}

if ($Phase -eq 'rerun') {
    Test-Step 'Minimal: picker' { Select-Preset 'minimal' }
    Test-Step 'Minimal: preview' { Assert-Preview $patterns.Minimal.Preview }
    Test-Step 'Minimal: sent' { Wait-GameStatus $patterns.Minimal.Sent }
    $Phase = 'alternating'
}

if ($Phase -eq 'presets') {
    Test-Step 'Network: picker' { Select-Preset 'network' }
    Test-Step 'Network: preview' { Assert-Preview $patterns.Network.Preview }
    Test-Step 'Network: sent' { Wait-GameStatus $patterns.Network.Sent }
    Test-Step 'Network: saved' { if ((Get-Saved).preset -ne 'network') { throw (Get-Saved).preset } }

    # The climate and the rank come from the save, so the patterns take any words.
    Test-Step 'Era: picker' { Select-Preset 'era' }
    Test-Step 'Era: preview' { Assert-Preview $patterns.Era.Preview }
    Test-Step 'Era: sent' { Wait-GameStatus $patterns.Era.Sent }

    Test-Step 'Minimal: picker' { Select-Preset 'minimal' }
    Test-Step 'Minimal: preview' { Assert-Preview $patterns.Minimal.Preview }
    Test-Step 'Minimal: sent' { Wait-GameStatus $patterns.Minimal.Sent }
    $Phase = 'alternating'
}

if ($Phase -eq 'alternating') {
    Test-Step 'Alternating: picker' { Select-Preset 'alternating' }
    Test-Step 'Alternating: rotates through Career, Network and Era (150 s)' {
        $seen = [Collections.Generic.List[string]]::new()
        $changes = [Collections.Generic.List[string]]::new()
        $start = Get-Date
        $last = $null
        while (((Get-Date) - $start).TotalSeconds -lt 150) {
            $current = Get-GameStatus
            if ($current -ne $last) { $changes.Add("$([int]((Get-Date) - $start).TotalSeconds)s '$current'"); $last = $current }
            $kind = if ($current -match $patterns.Network.Sent) { 'Network' } elseif ($current -match $patterns.Era.Sent) { 'Era' } elseif ($current -match $patterns.Career.Sent) { 'Career' } else { "other: $current" }
            # The first 20 s are a grace period: the layout that was sent before the switch is still shown.
            if ((((Get-Date) - $start).TotalSeconds -ge 20) -and (-not $seen.Contains($kind))) { $seen.Add($kind) }
            Start-Sleep -Seconds 2
        }
        foreach ($k in 'Career', 'Network', 'Era') { if (-not $seen.Contains($k)) { throw "never saw $k; changes: $($changes -join ', ')" } }
        $other = @($seen | Where-Object { $_ -like 'other*' })
        if ($other.Count) { throw "unexpected: $($other -join ', '); changes: $($changes -join ', ')" }
        $changes -join ', '
    }

    Test-Step 'Career: picker' { Select-Preset 'career' }
    Test-Step 'Career: sent' { Wait-GameStatus $patterns.Career.Sent }
}

if ($Phase -eq 'custom') {
    $ctl = Join-Path $PSScriptRoot 'tf3ctl.ps1'
    Test-Step 'Custom: picker' { Select-Preset 'custom' }
    if ((Get-Saved).templates.details -ne '{rank} · {balance}') {
        Test-Step 'Custom: defaults look like Career' { Wait-GameStatus $patterns.Career.Sent }
    }
    Test-Step 'Custom: type the three templates' {
        $edits = @(Get-AllElements | Where-Object { $_.type -eq 'Edit' } | Sort-Object { [int]$_.y })
        if ($edits.Count -ne 3) { throw "expected 3 template fields, found $($edits.Count)" }
        Invoke-Ui @('set-value', $edits[0].selector, '{rank} · {balance}') | Out-Null
        Invoke-Ui @('set-value', $edits[1].selector, '{companyValue} · {saveName}') | Out-Null
        Invoke-Ui @('set-value', $edits[2].selector, '{climate} · {passengers}') | Out-Null
    }
    Test-Step 'Custom: preview' { Assert-Preview $patterns.Custom.Preview }
    Test-Step 'Custom: sent' { Wait-GameStatus $patterns.Custom.Sent }
    Test-Step 'Custom: templates saved after 1 s' {
        Start-Sleep -Seconds 2
        $templates = (Get-Saved).templates
        if ($templates.details -ne '{rank} · {balance}' -or $templates.state -ne '{companyValue} · {saveName}' -or $templates.hover -ne '{climate} · {passengers}') { throw ($templates | ConvertTo-Json -Compress) }
        $templates | ConvertTo-Json -Compress
    }
    Test-Step 'Pause button (Space): the pause label is appended to the custom second line' {
        & $ctl key SPACE | Out-Null
        Wait-GameStatus $patterns.Custom.Paused 45
    }
    Test-Step 'Resume (Space): pause label gone' {
        & $ctl key SPACE | Out-Null
        Wait-GameStatus $patterns.Custom.Running 45
    }
    Test-Step 'Game menu (Escape) for 40 s: pause label' {
        & $ctl key ESCAPE | Out-Null
        Wait-GameStatus $patterns.Custom.Paused 60
    }
    Test-Step 'Close the menu (Escape): pause label gone' {
        & $ctl key ESCAPE | Out-Null
        Wait-GameStatus $patterns.Custom.Running 45
    }
}

"`n$(@($results | Where-Object { $_ -like 'PASS*' }).Count) passed, $(@($results | Where-Object { $_ -like 'FAIL*' }).Count) failed"
