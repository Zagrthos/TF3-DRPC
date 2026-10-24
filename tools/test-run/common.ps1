<#
.SYNOPSIS
Helpers shared by the scripts of tools/test-run. Dot-source it: . "$PSScriptRoot\common.ps1"

.DESCRIPTION
Finds the Transport Fever 3 user data folder the way the companion app does, reads the game
log and resolves output paths. Defines functions only, plus the two variables below.
#>

$Tf3AppId = '3493540'
$Tf3ProcessName = 'TransportFever3'
$AppProcessName = 'TF3-DRPC'

# Where screenshots, results and the settings.lua backup go by default; ignored by git (artifacts/).
function Get-DefaultOutFolder {
    Join-Path $PSScriptRoot 'artifacts'
}

# The Steam folder: on Windows from the registry, with the default install folder as a fallback for a
# machine without that value; on macOS always ~/Library/Application Support/Steam. If Steam lives
# elsewhere, pass -UserDataFolder to the script instead.
function Get-SteamFolder {
    $steam = $null
    if ($IsMacOS) {
        $steam = Join-Path $HOME 'Library' 'Application Support' 'Steam'
    } else {
        try { $steam = (Get-ItemProperty -Path 'HKCU:\Software\Valve\Steam' -Name SteamPath -ErrorAction Stop).SteamPath } catch { $steam = $null }
        if ([string]::IsNullOrWhiteSpace($steam)) { $steam = 'C:\Program Files (x86)\Steam' }
    }
    if (-not (Test-Path -LiteralPath $steam -PathType Container)) {
        throw "Steam folder '$steam' not found. Pass -UserDataFolder <Steam>\userdata\<your Steam id>\$Tf3AppId\local instead."
    }
    [IO.Path]::GetFullPath($steam)
}

# <Steam>/userdata/<id>/3493540/local of the account whose crash_dump/stdout.txt was written last
# (the app's rule); -Override (the script's -UserDataFolder) is used as it is.
function Get-UserDataFolder {
    param([string]$Override)
    if ($Override) {
        if (-not (Test-Path -LiteralPath $Override -PathType Container)) { throw "-UserDataFolder '$Override' doesn't exist." }
        return (Resolve-Path -LiteralPath $Override).ProviderPath
    }
    $root = Join-Path (Get-SteamFolder) 'userdata'
    if (-not (Test-Path -LiteralPath $root -PathType Container)) { throw "No userdata folder at '$root'. Pass -UserDataFolder." }
    $best = $null
    $bestWritten = [datetime]::MinValue
    foreach ($account in Get-ChildItem -LiteralPath $root -Directory) {
        $local = Join-Path $account.FullName $Tf3AppId 'local'
        if (-not (Test-Path -LiteralPath $local -PathType Container)) { continue }
        $log = Join-Path $local 'crash_dump' 'stdout.txt'
        $written = if (Test-Path -LiteralPath $log -PathType Leaf) { (Get-Item -LiteralPath $log).LastWriteTimeUtc } else { [datetime]::MinValue }
        if (($null -eq $best) -or ($written -gt $bestWritten)) { $best = $local; $bestWritten = $written }
    }
    if (-not $best) {
        throw "No Steam account with Transport Fever 3 user data under '$root'. Start the game once, or pass -UserDataFolder."
    }
    $best
}

function Get-GameLogPath {
    param([string]$UserDataFolder)
    Join-Path (Get-UserDataFolder $UserDataFolder) 'crash_dump' 'stdout.txt'
}

# The UTC time at the start of a game log line ("[2026-10-13 21:15:24Z - MESSAGE ..."), or $null.
function ConvertTo-LogTime {
    param([string]$Line)
    if ($Line -notmatch '^\[(\d{4}-\d\d-\d\d \d\d:\d\d:\d\d)Z') { return $null }
    [datetime]::ParseExact($Matches[1], 'yyyy-MM-dd HH:mm:ss', [Globalization.CultureInfo]::InvariantCulture,
        [Globalization.DateTimeStyles]::AssumeUniversal -bor [Globalization.DateTimeStyles]::AdjustToUniversal)
}

# The last log line matching the regex whose time is at or after -Since (UTC); $null if none, also without a log.
function Find-LogLine {
    param([string]$LogPath, [string]$Pattern, [datetime]$Since = [datetime]::MinValue)
    if (-not (Test-Path -LiteralPath $LogPath -PathType Leaf)) { return $null }
    $found = $null
    foreach ($match in Select-String -LiteralPath $LogPath -Pattern $Pattern -Encoding utf8) {
        $time = ConvertTo-LogTime $match.Line
        if ($null -eq $time) {
            if ($Since -gt [datetime]::MinValue) { continue }
        } elseif ($time -lt $Since) {
            continue
        }
        $found = [pscustomobject]@{ Time = $time; Line = $match.Line }
    }
    $found
}

# The last [drpc-status] line: its time and the parsed JSON; $null without one.
function Get-LastStatus {
    param([string]$LogPath)
    $hit = Find-LogLine -LogPath $LogPath -Pattern '\[drpc-status\] '
    if (-not $hit) { return $null }
    $json = $hit.Line.Substring($hit.Line.IndexOf('[drpc-status] ') + 14)
    [pscustomobject]@{ Time = $hit.Time; Status = ($json | ConvertFrom-Json) }
}

# A full path for a file to write (relative paths count from the current PowerShell location); creates its folder.
function Resolve-OutputPath {
    param([string]$Path)
    $full = [IO.Path]::GetFullPath($Path, (Get-Location).ProviderPath)
    New-Item -ItemType Directory -Force -Path ([IO.Path]::GetDirectoryName($full)) | Out-Null
    $full
}

# The native helper for macOS (tf3mac.swift), compiled into artifacts/bin on first use and after a change.
function Get-MacHelper {
    $source = Join-Path $PSScriptRoot 'tf3mac.swift'
    $binary = Join-Path (Get-DefaultOutFolder) 'bin' 'tf3mac'
    if (-not (Test-Path -LiteralPath $binary) -or (Get-Item -LiteralPath $source).LastWriteTimeUtc -gt (Get-Item -LiteralPath $binary).LastWriteTimeUtc) {
        if (-not (Get-Command swiftc -ErrorAction SilentlyContinue)) { throw 'swiftc is missing; install the Xcode command line tools.' }
        New-Item -ItemType Directory -Force -Path (Split-Path -Parent $binary) | Out-Null
        & swiftc -O -o $binary $source
        if ($LASTEXITCODE -ne 0) { throw 'Compiling tf3mac.swift failed.' }
    }
    $binary
}

function Assert-WinApp {
    if (-not (Get-Command winapp -ErrorAction SilentlyContinue)) {
        throw 'winapp (WinApp CLI 0.7) is not on the PATH; it is needed to drive the settings window.'
    }
}

# The running TF3-DRPC process; the app must run from a published copy, see the README.
function Get-AppProcess {
    $app = Get-Process -Name $AppProcessName -ErrorAction SilentlyContinue | Select-Object -First 1
    if (-not $app) { throw "$AppProcessName is not running. Start the published app first." }
    $app
}

# ---- Languages -----------------------------------------------------------------------------------------------
# The game and the app both come in 13 languages. The test scripts never hold a UI name or a pattern in one
# language: they ask for the app's own texts by key (Get-AppText) and build the patterns of the status line from
# the structure (Get-PresencePatterns).

# Code is the game's language code (<userdata>/profile.lua), Resx the culture of the app's texts file (Texts.<Resx>.resx;
# '' is Texts.resx, English) and Culture the one the app formats numbers and dates with. It is the table of
# companion/Tf3Drpc.Core/Languages/LanguageCatalog.cs.
$Tf3Languages = @(
    [pscustomobject]@{ Code = 'de'; Resx = 'de'; Culture = 'de-DE' }
    [pscustomobject]@{ Code = 'en'; Resx = ''; Culture = 'en-US' }
    [pscustomobject]@{ Code = 'es'; Resx = 'es'; Culture = 'es-ES' }
    [pscustomobject]@{ Code = 'fr'; Resx = 'fr'; Culture = 'fr-FR' }
    [pscustomobject]@{ Code = 'it'; Resx = 'it'; Culture = 'it-IT' }
    [pscustomobject]@{ Code = 'ja'; Resx = 'ja'; Culture = 'ja-JP' }
    [pscustomobject]@{ Code = 'ko'; Resx = 'ko'; Culture = 'ko-KR' }
    [pscustomobject]@{ Code = 'nl'; Resx = 'nl'; Culture = 'nl-NL' }
    [pscustomobject]@{ Code = 'pl'; Resx = 'pl'; Culture = 'pl-PL' }
    [pscustomobject]@{ Code = 'pt_BR'; Resx = 'pt-BR'; Culture = 'pt-BR' }
    [pscustomobject]@{ Code = 'ru'; Resx = 'ru'; Culture = 'ru-RU' }
    [pscustomobject]@{ Code = 'zh_CN'; Resx = 'zh-CN'; Culture = 'zh-CN' }
    [pscustomobject]@{ Code = 'zh_TW'; Resx = 'zh-TW'; Culture = 'zh-TW' }
)
$Tf3ResourceFolder = Join-Path $PSScriptRoot '..' '..' 'companion' 'Tf3Drpc.Core' 'Resources'
# The same pattern as RegexUtils.LanguageCode of the app.
$Tf3LanguageCodePattern = '\blanguage\s*=\s*\{(?:[^{}]|\{[^{}]*\})*?\bcode\s*=\s*"(?<code>[A-Za-z_]+)"'
$script:ResxCache = @{}
$script:TestLanguage = $null

# The entry of $Tf3Languages for a game language code (case-sensitive like the app); throws for another code.
function Get-LanguageInfo {
    param([Parameter(Mandatory)][string]$Code)
    foreach ($language in $Tf3Languages) { if ($language.Code -ceq $Code) { return $language } }
    throw "Unknown language code '$Code'; the game's codes are $(($Tf3Languages | ForEach-Object { $_.Code }) -join ', ')."
}

# language.code of <userdata>/profile.lua, found like the app does (LanguageCatalog.TryReadLanguageCode); $null without
# the file or the entry. The game keeps its language only there, not in settings.lua.
function Get-GameLanguageCode {
    param([string]$UserDataFolder)
    try { $path = Join-Path (Get-UserDataFolder $UserDataFolder) 'profile.lua' } catch { return $null }
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) { return $null }
    $match = [regex]::Match([IO.File]::ReadAllText($path, [Text.Encoding]::Latin1), $Tf3LanguageCodePattern)
    if ($match.Success) { $match.Groups['code'].Value } else { $null }
}

# The folder of the running TF3-DRPC (its DATA\settings.json is the app's settings); $null if the app doesn't run.
function Get-RunningAppFolder {
    $app = Get-Process -Name $AppProcessName -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($app -and $app.Path) { Split-Path -Parent $app.Path }
}

# The language the app and the game are tested in. In this order: -Language (a game code); the language setting in
# <AppFolder>\DATA\settings.json (a code, or "game": the language of profile.lua); no settings file: the app's default,
# which is "game". -AppFolder defaults to the folder of the running app. Without a profile.lua entry: English, as the
# app does. Returns the language entry plus Source, a text that says where it came from.
function Resolve-TestLanguage {
    param([string]$Language, [string]$AppFolder, [string]$UserDataFolder)
    if ($Language) { return (Get-LanguageInfo $Language | Select-Object *, @{ n = 'Source'; e = { '-Language' } }) }
    if (-not $AppFolder) { $AppFolder = Get-RunningAppFolder }
    $setting = 'game'
    $source = 'the app default'
    if ($AppFolder) {
        $file = Join-Path $AppFolder 'DATA' 'settings.json'
        if (Test-Path -LiteralPath $file -PathType Leaf) {
            $value = (Get-Content -LiteralPath $file -Raw | ConvertFrom-Json).language
            if ($value) { $setting = [string]$value; $source = $file }
        }
    }
    $code = $setting
    if ($setting -ceq 'game') {
        $code = Get-GameLanguageCode $UserDataFolder
        $source = "$source, follows the game (profile.lua)"
        if (-not $code) { $code = 'en'; $source = "$source, no entry found: English" }
    }
    $info = $Tf3Languages | Where-Object { $_.Code -ceq $code } | Select-Object -First 1
    if (-not $info) {
        $info = $Tf3Languages | Where-Object { $_.Code -eq 'en' }
        $source = "$source, unknown code '$code': English"
    }
    $info | Select-Object *, @{ n = 'Source'; e = { $source } }
}

# Resolves the language (see Resolve-TestLanguage) and keeps it for Get-AppText and Get-PresencePatterns.
function Set-TestLanguage {
    param([string]$Language, [string]$AppFolder, [string]$UserDataFolder)
    $script:TestLanguage = Resolve-TestLanguage -Language $Language -AppFolder $AppFolder -UserDataFolder $UserDataFolder
    Write-Host "Language under test: $($script:TestLanguage.Code) ($($script:TestLanguage.Source))"
}

function Read-ResxTexts {
    param([string]$Resx)
    if ($script:ResxCache.ContainsKey($Resx)) { return $script:ResxCache[$Resx] }
    $name = if ($Resx) { "Texts.$Resx.resx" } else { 'Texts.resx' }
    $path = Join-Path $Tf3ResourceFolder $name
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) { throw "No texts file '$path'." }
    $texts = @{}
    foreach ($entry in ([xml](Get-Content -LiteralPath $path -Raw -Encoding utf8)).root.data) { $texts[[string]$entry.name] = [string]$entry.value }
    $script:ResxCache[$Resx] = $texts
    $texts
}

# The app's own text for a key of Texts.resx (StatusAppearance, PresetCareer, InGame, ...) in the language under test,
# or in -Code (a game code) for a check of all languages. A key missing from a translation falls back to English like the app.
function Get-AppText {
    param([Parameter(Mandatory, Position = 0)][string]$Key, [string]$Code)
    $language = if ($Code) { Get-LanguageInfo $Code } else { $script:TestLanguage }
    if (-not $language) { throw 'No language under test; call Set-TestLanguage first.' }
    $texts = Read-ResxTexts $language.Resx
    if ($texts.ContainsKey($Key) -and $texts[$Key]) { return $texts[$Key] }
    $english = Read-ResxTexts ''
    if ($english.ContainsKey($Key) -and $english[$Key]) { return $english[$Key] }
    throw "No app text '$Key'."
}

$Tf3PresetKeys = [ordered]@{
    career = 'PresetCareer'; network = 'PresetNetwork'; era = 'PresetEra'
    alternating = 'PresetAlternating'; minimal = 'PresetMinimal'; custom = 'PresetCustom'
}

# The picker entry for a preset key (career, network, era, alternating, minimal, custom) in the language under test;
# any other text is returned as it is.
function Get-PresetText {
    param([Parameter(Mandatory, Position = 0)][string]$KeyOrText, [string]$Code)
    if ($Tf3PresetKeys.Contains($KeyOrText)) { return (Get-AppText $Tf3PresetKeys[$KeyOrText] -Code $Code) }
    $KeyOrText
}

# The month and year of the game date the way the app writes them: the culture's year-and-month pattern
# (PresenceValueUtils.FormatDate), for example "September 1905" or "septiembre de 1905"; Japanese and Chinese write the number of the month and a month character after the year.
function Format-AppDate {
    param([int]$Year, [int]$Month, [string]$Code)
    $language = if ($Code) { Get-LanguageInfo $Code } else { $script:TestLanguage }
    $culture = [cultureinfo]::GetCultureInfo($language.Culture)
    [datetime]::new($Year, $Month, 1).ToString($culture.DateTimeFormat.YearMonthPattern, $culture)
}

# ---- Patterns of the status line -----------------------------------------------------------------------------
# The tokens, in regex form. The app owns InGame, the game owns every other word, so a word is a label:
#   Label   one or more words without digits, separators (middle dot, bar) or line breaks: a label of the game
#           (Lines, Year, Paused), a rank, a mode, a climate. It may not start or end with a space.
#   Count   a number the app formats with the thousands separator of the culture (comma, dot, no-break space,
#           narrow no-break space), no sign.
#   Money   a compact amount: the dollar sign, a number, optionally K, M, B or T ($456, $17.3M).
#   Year    four digits, the game year without formatting.
#   Date    the month and year in the culture's year-and-month pattern (see ConvertTo-DateRegex).
$Tf3LabelRegex = '[^\d\u00B7|\s](?:[^\d\u00B7|]*[^\d\u00B7|\s])?'
$Tf3CountRegex = '\d[\d.,\u00A0\u202F]*'
$Tf3MoneyRegex = '[-\u2212]?\$\d[\d.,\u00A0\u202F]*[KMBT]?'

# A .NET year-and-month pattern as a regex: yyyy is four digits, M and MM one or two, MMM and MMMM a month name (any
# characters but spaces and separators: Korean writes the month as a number with a month character), quoted text and other characters are literal, a
# space is any space (ICU uses a no-break space in some cultures).
function ConvertTo-DateRegex {
    param([Parameter(Mandatory)][string]$Pattern)
    $regex = [Text.StringBuilder]::new()
    $i = 0
    while ($i -lt $Pattern.Length) {
        $c = $Pattern[$i]
        if ($c -eq "'" -or $c -eq '"') {
            $end = $Pattern.IndexOf($c, $i + 1)
            if ($end -lt 0) { $end = $Pattern.Length }
            [void]$regex.Append([regex]::Escape($Pattern.Substring($i + 1, $end - $i - 1)))
            $i = $end + 1
        } elseif ($c -eq 'y' -or $c -eq 'M') {
            $run = 1
            while (($i + $run) -lt $Pattern.Length -and $Pattern[$i + $run] -eq $c) { $run++ }
            if ($c -eq 'y') { [void]$regex.Append($(if ($run -ge 3) { '\d{4}' } else { '\d{2,4}' })) }
            elseif ($run -ge 3) { [void]$regex.Append('[^\s\u00B7|]+') }
            else { [void]$regex.Append('\d{1,2}') }
            $i += $run
        } elseif ([char]::IsWhiteSpace($c)) {
            [void]$regex.Append('\s')
            $i++
        } else {
            [void]$regex.Append([regex]::Escape([string]$c))
            $i++
        }
    }
    $regex.ToString()
}

# The patterns of the status line of the language under test, the way PresenceBuilder composes it: details, then
# the middle dot with spaces and the state. Preview is what the settings window shows (the lines side by side with " | ", the label of the
# store button taken out), Sent the one line the app sends to Discord. Custom is for the three templates of the custom
# phase of game-tests.ps1 (rank and balance; company value and save name; climate and passengers, each pair joined by the middle dot).
# Checked in every language: the app's own word (InGame) exactly; the number and order of the parts and their
# separators; digits where a count or the year stands; the date in the culture's format; every game word as a label,
# that is words without digits. Not checked: the game's own words themselves (labels, ranks, modes, climates), which
# the app takes from the game's text files.
function Get-PresencePatterns {
    param([string]$SaveName)
    $label = $Tf3LabelRegex; $count = $Tf3CountRegex; $money = $Tf3MoneyRegex
    $sep = ' \u00B7 '
    $culture = [cultureinfo]::GetCultureInfo($script:TestLanguage.Culture)
    $date = ConvertTo-DateRegex $culture.DateTimeFormat.YearMonthPattern
    $inGame = [regex]::Escape((Get-AppText InGame))
    $save = if ($SaveName) { [regex]::Escape($SaveName) } else { '.+?' }
    [pscustomobject]@{
        Label = $label; Count = $count; Money = $money; Date = $date
        Minimal = [pscustomobject]@{
            Preview = "^$inGame$sep\d{4} \| Transport Fever 3"
            Sent = "^$inGame$sep\d{4}$"
        }
        Network = [pscustomobject]@{
            Preview = "^$label\s$count$sep$label\s$count \| $label\s$count$sep\d{4} \| Transport Fever 3"
            Sent = "^$label\s$count$sep$label\s$count$sep$label\s$count$sep\d{4}$"
        }
        Era = [pscustomobject]@{
            Preview = "^$label\s\d{4}$sep$label \| $label \| Transport Fever 3"
            Sent = "^$label\s\d{4}$sep$label$sep$label$"
        }
        # Rank, the mode when there is one (not in the normal game), the date.
        Career = [pscustomobject]@{ Sent = "^$label(?:$sep$label)?$sep$date$" }
        Custom = [pscustomobject]@{
            Preview = "^$label$sep$money \| $money$sep$save \| $label$sep$count"
            Sent = "^$label$sep$money$sep$money$sep$save$"
            Paused = "$sep$save$sep$label$"
            Running = "$sep$save$"
        }
    }
}

# The click points and screen check boxes (calibration.psd1), a hashtable keyed '<platform>-<height>'.
function Get-CalibrationTable {
    if (-not $script:CalibrationTable) {
        $script:CalibrationTable = Import-PowerShellDataFile -LiteralPath (Join-Path $PSScriptRoot 'calibration.psd1')
    }
    $script:CalibrationTable
}

# The set for this platform and a screenshot height in pixels (the 1280 px wide picture: 720 is 16:9, 804 a macOS game window); a
# gap of up to two pixels from rounding is accepted. Its Key entry names the set.
function Get-Calibration {
    param([Parameter(Mandatory)][int]$Height)
    # $CalibrationPlatform (set by tf3ctl.ps1 -Platform) reads a picture of the other platform, e.g. a Windows
    # screenshot on a Mac.
    $platform = if ($CalibrationPlatform) { $CalibrationPlatform } elseif ($IsMacOS) { 'macos' } elseif ($IsWindows) { 'windows' } else { throw 'The test-run scripts run on Windows and macOS only.' }
    $table = Get-CalibrationTable
    $best = $null
    $bestGap = 3
    foreach ($key in $table.Keys) {
        if (-not $key.StartsWith("$platform-", [StringComparison]::Ordinal)) { continue }
        $gap = [Math]::Abs([int]$key.Substring($platform.Length + 1) - $Height)
        if ($gap -lt $bestGap) { $best = $key; $bestGap = $gap }
    }
    if (-not $best) {
        throw "No calibration for $platform at a screenshot height of $Height px (1280x$Height). calibration.psd1 has: $(($table.Keys | Sort-Object) -join ', '). Another aspect ratio or UI scale needs a set of its own; see 'Calibrating' in tools/test-run/README.md."
    }
    $set = $table[$best]
    $set.Key = $best
    $set
}

# A value of a set by its dotted name ('Points.Quit', 'Screen.MainMenu'). Not calibrated ($null or missing) is an error
# naming the value, or $null with -Optional.
function Get-CalibrationValue {
    param([Parameter(Mandatory)]$Set, [Parameter(Mandatory)][string]$Name, [switch]$Optional)
    $value = $Set
    foreach ($part in $Name.Split('.')) {
        $value = if ($value -is [hashtable] -and $value.ContainsKey($part)) { $value[$part] } else { $null }
        if ($null -eq $value) { break }
    }
    if (($null -eq $value) -and -not $Optional) {
        throw "'$Name' is not calibrated for the set '$($Set.Key)' (tools/test-run/calibration.psd1). Measure it on a screenshot and enter it there: see 'Calibrating' in tools/test-run/README.md."
    }
    , $value
}

# The names (dotted, as above) of the given values that a set doesn't have.
function Get-MissingCalibration {
    param([Parameter(Mandatory)]$Set, [Parameter(Mandatory)][string[]]$Name)
    $missing = [Collections.Generic.List[string]]::new()
    foreach ($item in $Name) {
        if ($null -eq (Get-CalibrationValue $Set $item -Optional)) { $missing.Add($item) }
    }
    , $missing.ToArray()
}
