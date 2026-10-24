<#
Starts, drives and stops Transport Fever 3 for a scripted test run, in any of the game's 13 languages.
  run-game.ps1 prepare [-Language <code>]   back up <userdata>/settings.lua and switch screenMode to BORDERLESS; with
                          -Language also back up profile.lua and set the game language (game closed only)
  run-game.ps1 start      start the game through Steam; wait for the process and for "Main menu is ready", then until
                          the Mod Hub downloads on the main menu are finished (the bar under the account box is gone)
  run-game.ps1 load [-SaveName <name>]   Load Game tile; with a name type it into the filter; then the first card's
                          Load Game button; wait for the savegame to load, clicking "Start Game" if needed
  run-game.ps1 quit       find out whether the game menu is open (open it if not), Quit, Return to Desktop (no save);
                          from the main menu the exit icon; wait for the process to end
  run-game.ps1 restore    copy the backups of settings.lua and profile.lua back (game closed only)
  run-game.ps1 continue   old name of load: the build 40423 main menu has no "Continue" tile any more

-Language         a game language code: de, en, es, fr, it, ja, ko, nl, pl, pt_BR, ru, zh_CN or zh_TW. The game keeps
                  its language in <userdata>/profile.lua (language = { code = "de", ... }), not in settings.lua; it is
                  locked in a running game, so it is set here while the game is closed. The game rewrites the file
                  when it quits, so restore only afterwards.
-SaveName         the savegame to load; typed into the filter box of the Load Game screen. Without it the first card
                  is loaded: the save written last (the grid is sorted by date). The line "Loading game from file"
                  that load prints shows which file it was.
-OutFolder        where the backups (settings.lua.orig, profile.lua.orig) live (default: artifacts next to the
                  scripts, ignored by git).
-UserDataFolder   <Steam>\userdata\<your Steam id>\3493540\local; without it the folder of the account whose
                  game log was written last is used (see common.ps1).
-StartTimeoutSeconds, -LoadTimeoutSeconds, -DownloadTimeoutSeconds   how long start waits for the main menu, load for
                  the savegame and start for the Mod Hub downloads (a load while they run must be avoided).
-NoMenuCheck      quit: don't look at the screen, press Escape once and click the points (the old behaviour), for a
                  menu that isn't calibrated or shows dialogues (a campaign mission).
-Editor           quit in the map editor: its menu has one more entry, so Quit and Return to Desktop are lower
                  (EditorQuit and EditorToDesktop in calibration.psd1); implies -NoMenuCheck.

The click points and the boxes of the screen checks are in the 1280 px wide screenshot of tf3ctl.ps1 and come from
calibration.psd1, the set for the platform and the height of the screenshot (tf3ctl.ps1 size): Windows 16:9 (720),
macOS (804). They are the same in every language: the layout doesn't depend on the language, only the texts do.
A point that is not calibrated there ($null) stops the command before the first click with a message naming it; pass
-LoadGameTilePoint, -FilterPoint, -SaveCardPoint, -StartGamePoint, -MenuButtonPoint, -QuitPoint, -ToDesktopPoint or
-ExitPoint (x,y) to override or fill in a point, or calibrate (README, "Calibrating"). While the screen checks of a
set are not calibrated, start doesn't wait for the downloads, load doesn't check the main menu and quit presses Escape
and clicks blind, each with a warning.
Examples: ./run-game.ps1 prepare -Language de
          ./run-game.ps1 load -SaveName Archipelago -LoadTimeoutSeconds 400
#>
param(
    [Parameter(Position = 0)][string]$Command,
    [string]$OutFolder = (Join-Path $PSScriptRoot 'artifacts'),
    [string]$UserDataFolder,
    [string]$Language,
    [string]$SaveName,
    [int]$StartTimeoutSeconds = 240,
    [int]$LoadTimeoutSeconds = 300,
    [int]$DownloadTimeoutSeconds = 900,
    [switch]$NoMenuCheck,
    [switch]$Editor,
    # Coordinates of the 1280 px wide screenshot; without them the points of calibration.psd1 are used.
    [int[]]$LoadGameTilePoint = @(),
    [int[]]$FilterPoint = @(),
    [int[]]$SaveCardPoint = @(),
    [int[]]$StartGamePoint = @(),
    [int[]]$MenuButtonPoint = @(),
    [int[]]$QuitPoint = @(),
    [int[]]$ToDesktopPoint = @(),
    [int[]]$ExitPoint = @()
)

$ErrorActionPreference = 'Stop'
. "$PSScriptRoot\common.ps1"
. "$PSScriptRoot\screen.ps1"

$commands = @('prepare', 'start', 'load', 'continue', 'quit', 'restore')
$usage = @"
Usage: run-game.ps1 <command> [options]
Commands:
  prepare    back up settings.lua, set screenMode to BORDERLESS; with -Language <code> also set the game
             language in profile.lua (game must be closed)
  start      start the game through Steam, wait for the main menu and for the Mod Hub downloads
  load       load a savegame from the Load Game screen (-SaveName <name>, else the newest) and wait until it is ready
  quit       leave the game through its menu and wait for the process to end
  restore    put the backed-up settings.lua and profile.lua back (game must be closed)
  continue   old name of load
Options: -OutFolder, -UserDataFolder, -Language, -SaveName, -StartTimeoutSeconds, -LoadTimeoutSeconds,
         -DownloadTimeoutSeconds, -NoMenuCheck, -Editor, -LoadGameTilePoint, -FilterPoint, -SaveCardPoint, -StartGamePoint,
         -MenuButtonPoint, -QuitPoint, -ToDesktopPoint, -ExitPoint
See the header of this script and tools/test-run/README.md.
"@

if (-not $Command) { $usage; return }
if ($Command -notin $commands) { throw "Unknown command '$Command'.`n$usage" }
foreach ($point in $LoadGameTilePoint, $FilterPoint, $SaveCardPoint, $StartGamePoint, $MenuButtonPoint, $QuitPoint, $ToDesktopPoint, $ExitPoint) {
    if ($point.Count -notin 0, 2) { throw 'Every -...Point takes two numbers: x,y' }
}
if ($Command -eq 'continue') {
    'The main menu of build 40423 has no "Continue" tile any more: continue runs load now (the newest savegame, or -SaveName).'
    $Command = 'load'
}

$ctl = Join-Path $PSScriptRoot 'tf3ctl.ps1'

function Get-GameProcess {
    Get-Process -Name $Tf3ProcessName -ErrorAction SilentlyContinue | Select-Object -First 1
}

function Assert-GameClosed {
    if (Get-GameProcess) { throw 'Transport Fever 3 is running; close it first (settings.lua and profile.lua are rewritten when it quits).' }
}

function Get-SettingsLuaPath {
    $path = Join-Path (Get-UserDataFolder $UserDataFolder) 'settings.lua'
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) { throw "No settings.lua at '$path'. Start the game once first." }
    $path
}

function Get-ProfileLuaPath {
    $path = Join-Path (Get-UserDataFolder $UserDataFolder) 'profile.lua'
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) { throw "No profile.lua at '$path'. Start the game once first." }
    $path
}

function Get-BackupFolder {
    [IO.Path]::GetFullPath($OutFolder, (Get-Location).ProviderPath)
}

function Get-BackupPath {
    Join-Path (Get-BackupFolder) 'settings.lua.orig'
}

function Get-ProfileBackupPath {
    Join-Path (Get-BackupFolder) 'profile.lua.orig'
}

function Get-NowUtc {
    # The game log has whole-second stamps, so look two seconds back.
    [datetime]::UtcNow.AddSeconds(-2)
}

# Waits for the first log line matching the pattern at or after -Since; $null at the timeout.
function Wait-LogLine {
    param([string]$Pattern, [datetime]$Since, [int]$Seconds)
    $log = Get-GameLogPath $UserDataFolder
    $deadline = [datetime]::UtcNow.AddSeconds($Seconds)
    do {
        $hit = Find-LogLine -LogPath $log -Pattern $Pattern -Since $Since
        if ($hit) { return $hit }
        Start-Sleep -Seconds 2
    } while ([datetime]::UtcNow -lt $deadline)
    $null
}

# The calibration set (calibration.psd1) for the game window now: the platform and the height of tf3ctl.ps1's screenshot.
function Get-RunCalibration {
    if (-not $script:RunCalibration) {
        $size = & $ctl size
        if ("$size" -notmatch '^(\d+)x(\d+)$') { throw "tf3ctl.ps1 size answered '$size', not <width>x<height>." }
        $script:RunCalibration = Get-Calibration ([int]$Matches[2])
    }
    $script:RunCalibration
}

# A click point: the one given on the command line, else Points.<Name> of the calibration set; stops with a message
# naming it when it isn't calibrated. -Name is the point's name without "Point", as in the parameter -<Name>Point.
function Get-Point {
    param([Parameter(Mandatory)][string]$Name, [int[]]$Given)
    if ($Given.Count -eq 2) { return $Given }
    try {
        $value = Get-CalibrationValue (Get-RunCalibration) "Points.$Name"
    } catch {
        throw "$($_.Exception.Message) Or pass the point as -${Name}Point x,y."
    }
    $value
}

# True if the calibration set has all of the dotted names ('Screen.MainMenu', 'Points.Exit').
function Test-RunCalibrated {
    param([string[]]$Name)
    (Get-MissingCalibration (Get-RunCalibration) $Name).Count -eq 0
}

# What Get-ScreenState needs, and what the download wait needs.
$stateValues = 'Screen.DarkPanel', 'Screen.GameMenu', 'Screen.QuitFlyout', 'Screen.MainMenu', 'Points.Quit'
$downloadValues = 'Screen.MainMenu', 'Screen.DownloadBar'

function Invoke-Click {
    param([int[]]$Point)
    & $ctl click $Point[0] $Point[1] | Out-Null
}

# A screenshot of the game window as pixels (see screen.ps1).
function Get-GameShot {
    $temp = Join-Path ([IO.Path]::GetTempPath()) "tf3-shot-$([guid]::NewGuid().ToString('N')).png"
    try {
        & $ctl shot $temp | Out-Null
        Read-Screenshot $temp
    } finally {
        Remove-Item -LiteralPath $temp -ErrorAction SilentlyContinue
    }
}

# The screen state (screen.ps1) now, or $null when no screenshot was possible (the game not in front yet).
function Get-GameScreenState {
    try {
        $row = @{}
        if ($QuitPoint.Count -eq 2) { $row.QuitY = $QuitPoint[1] }
        Get-ScreenState (Get-GameShot) @row
    } catch { $null }
}

# Waits until the main menu shows no download bar. While Mod Hub downloads run the menu shows a progress bar under the
# account box, and loading a game then must be avoided. The game log has no line for the end of the downloads, so the
# bar on the screen is the only sign. Needs two checks without the bar in a row, in case it blinks between two mods.
function Wait-DownloadsFinished {
    try {
        $calibrated = Test-RunCalibrated $downloadValues
    } catch {
        Write-Warning "$($_.Exception.Message) Not waiting for the Mod Hub downloads; wait for them by hand."
        return
    }
    if (-not $calibrated) {
        Write-Warning "The download bar check is not calibrated for '$((Get-RunCalibration).Key)' (calibration.psd1); not waiting for the Mod Hub downloads. Wait for them by hand."
        return
    }
    Start-Sleep -Seconds 5 # the bar needs a moment to appear after the log line
    $started = [datetime]::UtcNow
    $deadline = $started.AddSeconds($DownloadTimeoutSeconds)
    $announced = $false
    $clear = 0
    while ($true) {
        $shot = $null
        try { $shot = Get-GameShot } catch { $shot = $null }
        if ($null -eq $shot) {
            $clear = 0
            $detail = 'no screenshot yet'
        } elseif (-not (Test-MainMenu $shot)) {
            $clear = 0
            $detail = 'the main menu is not on the screen'
        } elseif (Test-DownloadBar $shot) {
            $clear = 0
            $detail = 'Mod Hub downloads running'
        } else {
            $clear++
            if ($clear -ge 2) {
                if ($announced) { "Mod Hub downloads finished after $([int]([datetime]::UtcNow - $started).TotalSeconds) s." } else { 'No Mod Hub downloads running.' }
                return
            }
            Start-Sleep -Seconds 3
            continue
        }
        $waited = [int]([datetime]::UtcNow - $started).TotalSeconds
        if ([datetime]::UtcNow -ge $deadline) { throw "Still not clear after $DownloadTimeoutSeconds s ($detail). Check the main menu with tf3ctl.ps1 shot x.png." }
        "Waiting for the main menu to be clear of downloads, $waited s ($detail)."
        $announced = $true
        Start-Sleep -Seconds 10
    }
}

# Gets from any screen of a running game to Return to Desktop, or from the main menu to the exit icon. The game menu has
# state: Escape opens and closes it (and first closes an open window), so the screen is looked at before every step.
# Escape and the menu button alternate, because the game sometimes ignored Escape right after another window was in front.
function Invoke-QuitSteps {
    $escapes = 0
    $quitClicks = 0
    for ($step = 1; $step -le 8; $step++) {
        $state = Get-GameScreenState
        switch ($state) {
            'quit-flyout' { Invoke-Click $ToDesktopPoint; return 'Return to Desktop' }
            'main-menu' { Invoke-Click $ExitPoint; return 'the exit icon of the main menu' }
            'game-menu' {
                if ($quitClicks -ge 3) { throw "The Quit flyout didn't open after $quitClicks clicks at $($QuitPoint -join ','); check the point with tf3ctl.ps1 shot x.png." }
                Invoke-Click $QuitPoint
                $quitClicks++
            }
            default {
                # Closed menu, an open window, a loading screen, or no screenshot ($null).
                if (($escapes % 2) -eq 0) { & $ctl key ESCAPE | Out-Null } else { Invoke-Click $MenuButtonPoint }
                $escapes++
            }
        }
        Start-Sleep -Milliseconds 1500
    }
    throw "No way to the game's quit entry found: the last screen was '$state'. Look at it with tf3ctl.ps1 shot x.png and open the menu by hand, or use -NoMenuCheck."
}

switch ($Command) {
    'prepare' {
        Assert-GameClosed
        $settings = Get-SettingsLuaPath
        $backup = Get-BackupPath
        # Latin-1 maps every byte to one character, so the file comes back byte for byte except for the edit.
        $text = [IO.File]::ReadAllText($settings, [Text.Encoding]::Latin1)
        # Case-sensitive and at the line start: "oldScreenMode" is another key.
        $pattern = '(?m)^(\s*screenMode\s*=\s*)"FULLSCREEN"'
        $changeScreen = $text -cmatch $pattern
        $messages = [Collections.Generic.List[string]]::new()
        if (-not $changeScreen) {
            $current = [regex]::Match($text, '(?m)^\s*screenMode\s*=\s*"(\w+)"')
            $messages.Add("settings.lua has screenMode $(if ($current.Success) { $current.Groups[1].Value } else { '(not found)' }), not FULLSCREEN; nothing changed, no backup made.")
        }
        $profile = $null
        $profileText = $null
        $codeGroup = $null
        $changeLanguage = $false
        if ($Language) {
            $wanted = Get-LanguageInfo $Language
            $profile = Get-ProfileLuaPath
            $profileText = [IO.File]::ReadAllText($profile, [Text.Encoding]::Latin1)
            $found = [regex]::Match($profileText, $Tf3LanguageCodePattern)
            if (-not $found.Success) { throw "No language.code entry in '$profile'; start the game once and set a language in its settings." }
            $codeGroup = $found.Groups['code']
            $changeLanguage = $codeGroup.Value -cne $wanted.Code
            if (-not $changeLanguage) { $messages.Add("profile.lua has the language $($wanted.Code) already; nothing changed, no backup made.") }
        }
        # Check everything before the first write, so that a refusal leaves both files alone.
        if ($changeScreen -and (Test-Path -LiteralPath $backup)) {
            throw "A backup exists already: '$backup'. Run 'restore' first, or delete it if it is outdated."
        }
        if ($changeLanguage -and (Test-Path -LiteralPath (Get-ProfileBackupPath))) {
            throw "A backup exists already: '$(Get-ProfileBackupPath)'. Run 'restore' first, or delete it if it is outdated."
        }
        if ($changeScreen) {
            New-Item -ItemType Directory -Force -Path (Split-Path -Parent $backup) | Out-Null
            Copy-Item -LiteralPath $settings -Destination $backup
            [IO.File]::WriteAllText($settings, [regex]::Replace($text, $pattern, '$1"BORDERLESS"'), [Text.Encoding]::Latin1)
            $messages.Add("screenMode is BORDERLESS now. Backup: $backup")
        }
        if ($changeLanguage) {
            $profileBackup = Get-ProfileBackupPath
            New-Item -ItemType Directory -Force -Path (Split-Path -Parent $profileBackup) | Out-Null
            Copy-Item -LiteralPath $profile -Destination $profileBackup
            $edited = $profileText.Substring(0, $codeGroup.Index) + $wanted.Code + $profileText.Substring($codeGroup.Index + $codeGroup.Length)
            [IO.File]::WriteAllText($profile, $edited, [Text.Encoding]::Latin1)
            $messages.Add("The game language is $($wanted.Code) now (was $($codeGroup.Value)). Backup: $profileBackup")
        }
        if ($changeScreen -or $changeLanguage) { $messages.Add('After the run: run-game.ps1 restore') }
        $messages
    }
    'restore' {
        Assert-GameClosed
        $backup = Get-BackupPath
        $profileBackup = Get-ProfileBackupPath
        $restored = [Collections.Generic.List[string]]::new()
        if (Test-Path -LiteralPath $backup -PathType Leaf) {
            Copy-Item -LiteralPath $backup -Destination (Get-SettingsLuaPath) -Force
            Remove-Item -LiteralPath $backup
            $restored.Add("settings.lua restored from $backup (backup deleted).")
        }
        if (Test-Path -LiteralPath $profileBackup -PathType Leaf) {
            Copy-Item -LiteralPath $profileBackup -Destination (Get-ProfileLuaPath) -Force
            Remove-Item -LiteralPath $profileBackup
            $restored.Add("profile.lua restored from $profileBackup (backup deleted).")
        }
        if ($restored.Count -eq 0) { throw "No backup at '$backup' or '$profileBackup'; nothing to restore." }
        $restored
    }
    'start' {
        if (Get-GameProcess) { throw 'Transport Fever 3 is running already.' }
        if ($IsMacOS) { & open "steam://rungameid/$Tf3AppId" } else { Start-Process "steam://rungameid/$Tf3AppId" }
        $deadline = [datetime]::UtcNow.AddSeconds($StartTimeoutSeconds)
        $game = $null
        while (-not $game -and [datetime]::UtcNow -lt $deadline) {
            Start-Sleep -Seconds 2
            $game = Get-GameProcess
        }
        if (-not $game) { throw "The game process didn't appear within $StartTimeoutSeconds s. Is Steam logged in?" }
        $startedUtc = $game.StartTime.ToUniversalTime()
        "Game process started at $($startedUtc.ToString('HH:mm:ss'))Z."
        # The log still holds the previous session until the game truncates it, so only a line written after the process start counts.
        $remaining = [int]($deadline - [datetime]::UtcNow).TotalSeconds
        $menu = Wait-LogLine -Pattern 'Main menu is ready' -Since $startedUtc -Seconds ([Math]::Max($remaining, 30))
        if (-not $menu) { throw 'No "Main menu is ready" in the game log in time.' }
        "Main menu is ready ($($menu.Time.ToString('HH:mm:ss'))Z)."
        Wait-DownloadsFinished
    }
    'load' {
        if (-not (Get-GameProcess)) { throw 'Transport Fever 3 is not running; run start first.' }
        # Every point first, so that a missing one stops the command before the first click.
        $LoadGameTilePoint = Get-Point 'LoadGameTile' $LoadGameTilePoint
        $SaveCardPoint = Get-Point 'SaveCard' $SaveCardPoint
        $StartGamePoint = Get-Point 'StartGame' $StartGamePoint
        if ($SaveName) { $FilterPoint = Get-Point 'Filter' $FilterPoint }
        if (Test-RunCalibrated $stateValues) {
            # Not blind: the points are those of the main menu, and a download must not run while a game loads.
            $state = Get-GameScreenState
            if ($state -ne 'main-menu') { throw "The main menu is not on the screen (it shows '$state'); load starts there. Check with tf3ctl.ps1 shot x.png." }
            Wait-DownloadsFinished
        } else {
            Write-Warning "The screen checks are not calibrated for '$((Get-RunCalibration).Key)' (calibration.psd1); not checking for the main menu or the downloads. Make sure the main menu shows and no download runs."
        }
        $since = Get-NowUtc
        Invoke-Click $LoadGameTilePoint
        Start-Sleep -Seconds 3
        if ($SaveName) {
            Invoke-Click $FilterPoint
            Start-Sleep -Milliseconds 500
            & $ctl type $SaveName | Out-Null
            Start-Sleep -Seconds 2
        }
        Invoke-Click $SaveCardPoint
        $loading = Wait-LogLine -Pattern 'Loading game from file' -Since $since -Seconds 60
        if (-not $loading) { throw 'No "Loading game from file" after the click; check the points with tf3ctl.ps1 shot x.png.' }
        "Loading: $($loading.Line.Trim())"
        if ($SaveName -and ($loading.Line.IndexOf($SaveName, [StringComparison]::OrdinalIgnoreCase) -lt 0)) {
            Write-Warning "The line doesn't mention '$SaveName'; the file name can differ from the name shown, but check that the right save loads."
        }
        $deadline = [datetime]::UtcNow.AddSeconds($LoadTimeoutSeconds)
        $nextClick = [datetime]::UtcNow.AddSeconds(15)
        while ([datetime]::UtcNow -lt $deadline) {
            $ready = Find-LogLine -LogPath (Get-GameLogPath $UserDataFolder) -Pattern 'Game is ready' -Since $since
            if ($ready) { return "Game is ready ($($ready.Time.ToString('HH:mm:ss'))Z)." }
            # The loading screen can wait for a click on "Start Game"; clicking while it loads does no harm.
            if ([datetime]::UtcNow -ge $nextClick) {
                Invoke-Click $StartGamePoint
                $nextClick = [datetime]::UtcNow.AddSeconds(20)
            }
            Start-Sleep -Seconds 2
        }
        throw "No 'Game is ready' within $LoadTimeoutSeconds s."
    }
    'quit' {
        $game = Get-GameProcess
        if (-not $game) { throw 'Transport Fever 3 is not running.' }
        $since = Get-NowUtc
        $blind = $NoMenuCheck -or $Editor
        $prefix = if ($Editor) { 'Editor' } else { '' }
        $QuitPoint = Get-Point "${prefix}Quit" $QuitPoint
        $ToDesktopPoint = Get-Point "${prefix}ToDesktop" $ToDesktopPoint
        if (-not $blind) {
            $quitValues = $stateValues + 'Points.MenuButton', 'Points.Exit'
            if (Test-RunCalibrated $quitValues) {
                $MenuButtonPoint = Get-Point 'MenuButton' $MenuButtonPoint
                $ExitPoint = Get-Point 'Exit' $ExitPoint
            } else {
                Write-Warning "The screen checks are not calibrated for '$((Get-RunCalibration).Key)' (calibration.psd1); leaving blind: Escape, then the Quit and Return to Desktop points. Make sure the game menu was closed and the game is not in the main menu."
                $blind = $true
            }
        }
        if ($blind) {
            # Blind, as before: Escape toggles the menu, so start with it closed.
            & $ctl key ESCAPE | Out-Null
            Start-Sleep -Seconds 2
            Invoke-Click $QuitPoint
            Start-Sleep -Seconds 2
            Invoke-Click $ToDesktopPoint
        } else {
            "Left through $(Invoke-QuitSteps)."
        }
        if (-not $game.WaitForExit(120000)) { throw "The game didn't end within 120 s; check the screen with tf3ctl.ps1 shot x.png." }
        $end = Find-LogLine -LogPath (Get-GameLogPath $UserDataFolder) -Pattern 'Normal shutdown|Ungraceful exit' -Since $since
        if ($end) { "Game ended: $($end.Line.Trim())" } else { 'Game ended; no shutdown line in the log.' }
    }
}
