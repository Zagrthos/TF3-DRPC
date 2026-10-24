param([switch]$IncludePublish)
$ErrorActionPreference = 'Stop'
$reviewRoot = $PSScriptRoot
$repoRoot = (Resolve-Path (Join-Path $reviewRoot '../../..')).Path
$companion = Join-Path $repoRoot 'companion'
$artifacts = Join-Path $reviewRoot 'artifacts'
$temporaryConfig = Join-Path $companion '.globalconfig'
if (Test-Path $temporaryConfig) { throw 'An existing companion/.globalconfig must not be overwritten.' }
New-Item $artifacts -ItemType Directory -Force | Out-Null
$commands = [System.Collections.Generic.List[object]]::new()
$createdConfig = $false
$dotnetExe = Join-Path $env:ProgramFiles 'dotnet\dotnet.exe'

function Invoke-RecordedDotnet([string]$name, [string[]]$arguments) {
    $log = Join-Path $artifacts ($name + '.log')
    $timer = [System.Diagnostics.Stopwatch]::StartNew()
    $standardOutput = $log + '.stdout'
    $standardError = $log + '.stderr'
    $process = Start-Process -FilePath $dotnetExe -ArgumentList $arguments -WorkingDirectory $companion -RedirectStandardOutput $standardOutput -RedirectStandardError $standardError -Wait -PassThru
    $code = $process.ExitCode
    [System.IO.File]::WriteAllText($log, [System.IO.File]::ReadAllText($standardOutput) + [System.IO.File]::ReadAllText($standardError))
    Remove-Item $standardOutput, $standardError
    $commands.Add([ordered]@{ name = $name; arguments = $arguments; exitCode = $code; milliseconds = $timer.Elapsed.TotalMilliseconds; log = ('artifacts/' + $name + '.log') })
    Write-Output "$name exit=$code"
    if ($code -ne 0) { throw "$name failed; see $log" }
}

Push-Location $companion
try {
    Invoke-RecordedDotnet 'sdk-version' @('--version')
    $sdk = (Get-Content (Join-Path $artifacts 'sdk-version.log') -Raw).Trim()
    $pinned = (Get-Content 'global.json' -Raw | ConvertFrom-Json).sdk.version
    if ($sdk -ne $pinned) { throw "Expected pinned SDK $pinned, found $sdk" }
    Invoke-RecordedDotnet 'core-tests-final' @('test', '--project', 'Tf3Drpc.Core.Tests/Tf3Drpc.Core.Tests.csproj', ('-bl:' + (Join-Path $artifacts 'tests-{}.binlog')))
    [System.IO.File]::WriteAllText($temporaryConfig, "is_global = true`r`ndotnet_diagnostic.IDE0005.severity = warning`r`n", [System.Text.UTF8Encoding]::new($false))
    $createdConfig = $true
    Invoke-RecordedDotnet 'debug-unused-usings' @('build', 'TF3-DRPC.slnx', '-c', 'Debug', ('-bl:' + (Join-Path $artifacts 'debug-unused-{}.binlog')))
    Invoke-RecordedDotnet 'release-unused-usings' @('build', 'TF3-DRPC.slnx', '-c', 'Release', ('-bl:' + (Join-Path $artifacts 'release-unused-{}.binlog')))
    Remove-Item $temporaryConfig
    $createdConfig = $false
    if ($IncludePublish) {
        Invoke-RecordedDotnet 'publish-final' @('publish', 'Tf3Drpc.App/Tf3Drpc.App.csproj', '-c', 'Release', '-r', 'win-x64', ('-bl:' + (Join-Path $artifacts 'publish-final-{}.binlog')))
    }
    [ordered]@{ sdk = $sdk; workload = (Get-Content global.json -Raw | ConvertFrom-Json).sdk.workloadVersion; commands = $commands } |
        ConvertTo-Json -Depth 6 | Set-Content (Join-Path $artifacts 'latest-validation-windows.json') -Encoding UTF8
}
finally {
    if ($createdConfig -and (Test-Path $temporaryConfig)) { Remove-Item $temporaryConfig }
    Pop-Location
}
