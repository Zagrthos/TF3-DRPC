# Tests only private copies of the published app. Never stops an existing user process.
$ErrorActionPreference = 'Stop'
$repoRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..\..'))
$sourceFolder = Join-Path $repoRoot 'companion\artifacts\publish\Tf3Drpc.App\release_win-x64'
$existingProcesses = @(Get-Process -Name 'TF3-DRPC','TransportFever3' -ErrorAction SilentlyContinue)
if ($existingProcesses.Count -gt 0) {
    throw 'An app or game process is already running; no desktop test was started.'
}
$caseRoot = Join-Path $PSScriptRoot ('artifacts\smoke-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $caseRoot | Out-Null
$observations = @()
try {
    foreach ($caseName in @('baseline', 'missing-tray', 'blocked-log-folder', 'blocked-data-folder')) {
        $caseFolder = Join-Path $caseRoot $caseName
        New-Item -ItemType Directory -Path $caseFolder | Out-Null
        Get-ChildItem -LiteralPath $sourceFolder | Copy-Item -Destination $caseFolder -Recurse
        if ($caseName -eq 'missing-tray') { Remove-Item -LiteralPath (Join-Path $caseFolder 'tray.ico') }
        if ($caseName -eq 'blocked-data-folder') { Set-Content -LiteralPath (Join-Path $caseFolder 'DATA') -Value 'A file occupies the data directory.' }
        if ($caseName -eq 'blocked-log-folder') {
            New-Item -ItemType Directory -Path (Join-Path $caseFolder 'DATA') | Out-Null
            Set-Content -LiteralPath (Join-Path $caseFolder 'DATA\logs') -Value 'A file occupies the log directory.'
        }
        $testProcess = Start-Process -FilePath (Join-Path $caseFolder 'TF3-DRPC.exe') -WorkingDirectory $caseFolder -PassThru -RedirectStandardOutput (Join-Path $caseFolder 'stdout.txt') -RedirectStandardError (Join-Path $caseFolder 'stderr.txt')
        $retainedHandle = $testProcess.Handle
        try {
            Start-Sleep -Seconds 5
            $testProcess.Refresh()
            $logText = ''
            $logFolder = Join-Path $caseFolder 'DATA\logs'
            if (Test-Path -LiteralPath $logFolder -PathType Container) {
                $logText = (@(Get-ChildItem -LiteralPath $logFolder -Filter '*.log') | ForEach-Object { Get-Content -LiteralPath $_.FullName -Raw }) -join "`n"
            }
            $singleInstanceControl = $null
            if ($caseName -eq 'baseline') {
                $secondProcess = Start-Process -FilePath (Join-Path $caseFolder 'TF3-DRPC.exe') -WorkingDirectory $caseFolder -PassThru
                try {
                    Start-Sleep -Seconds 2
                    $testProcess.Refresh(); $secondProcess.Refresh()
                    $singleInstanceControl = [pscustomobject]@{ SecondExited = $secondProcess.HasExited; FirstWindowVisible = ($testProcess.MainWindowHandle.ToInt64() -ne 0) }
                    if (!$singleInstanceControl.SecondExited -or !$singleInstanceControl.FirstWindowVisible) { throw 'Single-instance settings activation control failed.' }
                }
                finally {
                    if (!$secondProcess.HasExited) { Stop-Process -Id $secondProcess.Id -Force; $secondProcess.WaitForExit() }
                    $secondProcess.Dispose()
                }
            }
            $hasExited = $testProcess.HasExited
            if ($hasExited) { $testProcess.WaitForExit() }
            $windowHandle = 0
            if (!$hasExited) { $windowHandle = $testProcess.MainWindowHandle.ToInt64() }
            $observations += [pscustomobject]@{
                Case = $caseName
                SingleInstanceControl = $singleInstanceControl
                HasExited = $hasExited
                ExitCode = if ($testProcess.HasExited) { $testProcess.ExitCode } else { $null }
                MainWindowHandle = $windowHandle
                Log = $logText
                StandardError = [string](Get-Content -LiteralPath (Join-Path $caseFolder 'stderr.txt') -Raw)
            }
            if ($caseName -eq 'blocked-data-folder' -and ($hasExited -or $windowHandle -eq 0)) { throw 'Unwritable top-level DATA fallback control failed.' }
            Write-Output ($observations[-1] | ConvertTo-Json -Compress)
        }
        finally {
            if (!$testProcess.HasExited) { Stop-Process -Id $testProcess.Id -Force; $testProcess.WaitForExit() }
            $testProcess.Dispose()
        }
    }
    $observations | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath (Join-Path $PSScriptRoot 'artifacts\latest-smoke-results-windows.json') -Encoding UTF8
}
finally { Remove-Item -LiteralPath $caseRoot -Recurse -Force }
