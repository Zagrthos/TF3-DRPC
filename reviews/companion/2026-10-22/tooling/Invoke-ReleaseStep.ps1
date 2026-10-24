<#
.SYNOPSIS
Runs one inline pwsh step of .github/workflows/release.yml locally, as the Actions runner would.

.DESCRIPTION
Finds the step by its name, takes its `run: |` block, replaces each `${{ expression }}` as plain
text (the runner does the same before the shell sees the script), and runs the result like
`shell: pwsh` does: with `$ErrorActionPreference = 'stop'`, dot-sourced, in the work folder.
The step's `env:` entries are set as environment variables, with their expressions replaced
the same way. GITHUB_OUTPUT points to `_github_output` in the work folder. Prints the step's
output and `exit=<code>`.

.PARAMETER Workflow
Path to release.yml.

.PARAMETER Step
The step's `name:`, for example 'Write the release notes'.

.PARAMETER Expressions
The `${{ }}` expressions and their values, for example @{ 'github.ref_name' = 'v0.1.0' }.

.PARAMETER WorkDir
The folder the step runs in; it needs what the step reads (companion/Directory.Build.props,
CHANGELOG.md, dist/, and a git repository with an origin/main ref for the ancestry check).

.PARAMETER Environment
Variables the runner provides itself, for example @{ GITHUB_SHA = '<commit>' }.
#>
param(
    [Parameter(Mandatory)][string]$Workflow,
    [Parameter(Mandatory)][string]$Step,
    [hashtable]$Expressions = @{},
    [Parameter(Mandatory)][string]$WorkDir,
    [hashtable]$Environment = @{}
)

function Expand-Expressions([string]$Text) {
    foreach ($key in $Expressions.Keys) { $Text = $Text.Replace('${{ ' + $key + ' }}', [string]$Expressions[$key]) }
    $Text
}

$ErrorActionPreference = 'Stop'

$lines = Get-Content $Workflow
$nameIndex = [Array]::FindIndex([string[]]$lines, [Predicate[string]] { param($l) $l -match "^\s*(- )?name: $([regex]::Escape($Step))\s*$" })
if ($nameIndex -lt 0) { throw "No step named '$Step' in $Workflow" }
$runIndex = [Array]::FindIndex([string[]]$lines, $nameIndex, [Predicate[string]] { param($l) $l -match '^\s*run: \|\s*$' })

# The step's env: block, between its name and its run: line.
$stepEnvironment = @{}
$inEnv = $false
for ($i = $nameIndex + 1; $i -lt $runIndex; $i++) {
    if ($lines[$i] -match '^\s*env:\s*$') { $inEnv = $true; continue }
    if ($inEnv -and ($lines[$i] -match '^\s+([A-Z_][A-Z0-9_]*):\s*(.*)$')) { $stepEnvironment[$Matches[1]] = Expand-Expressions $Matches[2] }
    elseif ($inEnv) { $inEnv = $false }
}
foreach ($key in $Environment.Keys) { $stepEnvironment[$key] = [string]$Environment[$key] }
$indent = ($lines[$runIndex + 1] -replace '^(\s*).*$', '$1').Length
$body = [Collections.Generic.List[string]]::new()
for ($i = $runIndex + 1; $i -lt $lines.Count; $i++) {
    $line = $lines[$i]
    if (($line.Trim().Length -gt 0) -and ((($line -replace '^(\s*).*$', '$1').Length) -lt $indent)) { break }
    $body.Add($(if ($line.Length -ge $indent) { $line.Substring($indent) } else { '' }))
}

$text = Expand-Expressions ($body -join "`n")

$WorkDir = (Resolve-Path $WorkDir).Path
$script = Join-Path $WorkDir '_step.ps1'
"`$ErrorActionPreference = 'stop'`n$text" | Set-Content $script
$stepEnvironment['GITHUB_OUTPUT'] = Join-Path $WorkDir '_github_output'
$saved = @{}
foreach ($key in $stepEnvironment.Keys) {
    $saved[$key] = [Environment]::GetEnvironmentVariable($key)
    [Environment]::SetEnvironmentVariable($key, $stepEnvironment[$key])
}
Push-Location $WorkDir
try {
    & pwsh -NoProfile -Command ". '$script'"
    "exit=$LASTEXITCODE"
} finally {
    Pop-Location
    foreach ($key in $saved.Keys) { [Environment]::SetEnvironmentVariable($key, $saved[$key]) }
}
