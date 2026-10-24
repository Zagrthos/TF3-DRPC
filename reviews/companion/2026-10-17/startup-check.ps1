$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot
$repo = (Resolve-Path (Join-Path $root '../../..')).Path
if (@(Get-Process -Name 'TF3-DRPC', 'TransportFever3' -ErrorAction SilentlyContinue).Count -gt 0) { throw 'An existing companion or game must not be disturbed.' }
$publish = Join-Path $repo 'companion/artifacts/publish/Tf3Drpc.App/release_win-x64'
$results = @()
foreach ($scenario in @('Intact', 'MissingIcon', 'CorruptIcon', 'LockedIcon')) {
    $copy = Join-Path $root ('artifacts/startup-' + [Guid]::NewGuid().ToString('N'))
    New-Item $copy -ItemType Directory | Out-Null
    Get-ChildItem $publish | Where-Object Name -ne DATA | Copy-Item -Destination $copy -Recurse
    $icon = Join-Path $copy 'tray.ico'
    $lock = $null
    $reviewProcess = $null
    try {
        if ($scenario -eq 'MissingIcon') { Remove-Item $icon }
        if ($scenario -eq 'CorruptIcon') { [System.IO.File]::WriteAllText($icon, 'The private review icon fixture is deliberately corrupt.') }
        if ($scenario -eq 'LockedIcon') { $lock = [System.IO.FileStream]::new($icon, [System.IO.FileMode]::Open, [System.IO.FileAccess]::Read, [System.IO.FileShare]::None) }
        $started = [DateTime]::UtcNow
        $reviewProcess = Start-Process (Join-Path $copy 'TF3-DRPC.exe') -PassThru
        Start-Sleep -Seconds 4
        $reviewProcess.Refresh()
        $logText = ''
        $logs = Join-Path $copy 'DATA/logs'
        if (Test-Path $logs) {
            foreach ($file in (Get-ChildItem $logs -File)) {
                $stream = [System.IO.FileStream]::new($file.FullName, [System.IO.FileMode]::Open, [System.IO.FileAccess]::Read, ([System.IO.FileShare]::ReadWrite -bor [System.IO.FileShare]::Delete))
                $reader = [System.IO.StreamReader]::new($stream)
                try { $logText += $reader.ReadToEnd() } finally { $reader.Dispose() }
            }
        }
        $result = [ordered]@{
            scenario = $scenario; processAlive = (-not $reviewProcess.HasExited)
            visibleWindow = ($reviewProcess.MainWindowHandle -ne [IntPtr]::Zero)
            trayCreated = $logText.Contains('Tray icon created: True')
            trayFailureLogged = $logText.Contains('tray icon could not be created')
        }
        if ($reviewProcess.HasExited) {
            $result.exitCode = $reviewProcess.ExitCode
            $events = @(Get-WinEvent -FilterHashtable @{ LogName = 'Application'; StartTime = $started.ToLocalTime() } -ErrorAction SilentlyContinue | Where-Object { $_.Message.Contains($copy) })
            $result.crashMessages = @($events | ForEach-Object { $_.Message.Replace($copy, '<private review copy>') })
        }
        $results += $result
        Write-Output ($scenario + ': alive=' + $result.processAlive + ', visibleWindow=' + $result.visibleWindow + ', tray=' + $result.trayCreated)
    }
    finally {
        if ($null -ne $reviewProcess) {
            $reviewProcess.Refresh()
            if (-not $reviewProcess.HasExited) { $reviewProcess.Kill(); $reviewProcess.WaitForExit() }
        }
        if ($null -ne $lock) { $lock.Dispose() }
        Remove-Item $copy -Recurse -Force
    }
}
$results | ConvertTo-Json -Depth 6 | Set-Content (Join-Path $root 'artifacts/latest-startup-results-windows.json') -Encoding UTF8
