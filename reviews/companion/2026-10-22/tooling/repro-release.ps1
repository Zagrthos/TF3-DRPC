<#
.SYNOPSIS
Reproduces R02, R03, R04, R05 and R06: the release workflow's check and notes steps run on
crafted changelogs, tags and branches.

.DESCRIPTION
Each case gets its own work folder under -OutFolder: a throwaway git repository with a
`main` branch and an `origin/main` ref, holding a copy of companion/Directory.Build.props
and CHANGELOG.md (edited for the case) and a dummy dist/a.zip. The steps run through
Invoke-ReleaseStep.ps1 with GITHUB_SHA set to the case's commit, so the code under test is the
release.yml of -Repo: an export of the reviewed code shows the findings, the current tree the fixes.
Nothing in the repository is changed.

.PARAMETER Repo
The repository root, or an export of a commit.

.PARAMETER OutFolder
An empty scratch folder outside the repository.
#>
param(
    [Parameter(Mandatory)][string]$Repo,
    [Parameter(Mandatory)][string]$OutFolder
)

$ErrorActionPreference = 'Stop'
$Repo = (Resolve-Path $Repo).Path
New-Item -ItemType Directory -Force $OutFolder | Out-Null
$OutFolder = (Resolve-Path $OutFolder).Path
$workflow = Join-Path $Repo '.github/workflows/release.yml'
$runStep = Join-Path $PSScriptRoot 'Invoke-ReleaseStep.ps1'
$check = 'Match the tag with the app version and the changelog'
$notes = 'Write the release notes'
$git = @('-c', 'user.name=review', '-c', 'user.email=review@example.invalid', '-c', 'commit.gpgsign=false', '-c', 'tag.gpgsign=false', '-c', 'core.autocrlf=false')

# A throwaway repository whose main branch holds the case's files; origin/main points to it.
function New-Case([string]$Name, [scriptblock]$EditChangelog) {
    $dir = Join-Path $OutFolder $Name
    New-Item -ItemType Directory -Force (Join-Path $dir 'companion'), (Join-Path $dir 'dist') | Out-Null
    Copy-Item (Join-Path $Repo 'companion/Directory.Build.props') (Join-Path $dir 'companion') -Force
    & $EditChangelog (Get-Content (Join-Path $Repo 'CHANGELOG.md')) | Set-Content (Join-Path $dir 'CHANGELOG.md')
    Set-Content (Join-Path $dir 'dist/a.zip') 'dummy'
    git -C $dir init -q -b main
    git -C $dir add -A
    git -C $dir @git commit -q -m case
    git -C $dir update-ref refs/remotes/origin/main main
    $dir
}

function Step([string]$Name, [hashtable]$Expressions, [string]$Dir) {
    $sha = git -C $Dir rev-parse HEAD
    & $runStep -Workflow $workflow -Step $Name -Expressions $Expressions -WorkDir $Dir -Environment @{ GITHUB_SHA = $sha } 2>&1 | Out-String
}

$dated = { param($c) $c -replace 'not released yet', '2026-10-30' }

'=== R02: the README link in the release notes'
$dir = New-Case 'r02' $dated
Step $check @{ 'github.ref_name' = 'v0.1.0' } $dir
Step $notes @{ 'needs.check.outputs.version' = '0.1.0' } $dir
Select-String -Path (Join-Path $dir 'release-notes.md') -Pattern '\]\(' | ForEach-Object Line
'--- the same section with a relative link put back'
$dir = New-Case 'r02-relative' { param($c) (& $dated $c) -replace '\]\(https://github\.com/[^/]+/[^/)#]+#', '](README.md#' }
Select-String -Path (Join-Path $dir 'CHANGELOG.md') -Pattern '\]\(' | ForEach-Object Line
Step $check @{ 'github.ref_name' = 'v0.1.0' } $dir

'=== R03: a tag on a commit that is not on main'
$dir = New-Case 'r03' $dated
git -C $dir checkout -q -b unreviewed
Set-Content (Join-Path $dir 'unreviewed.txt') 'never reviewed'
git -C $dir add -A
git -C $dir @git commit -q -m unreviewed
git -C $dir @git tag v0.1.0
git -C $dir merge-base --is-ancestor v0.1.0 main
'tag v0.1.0 is on main: ' + $(if ($LASTEXITCODE -eq 0) { 'yes' } else { 'no' })
Step $check @{ 'github.ref_name' = 'v0.1.0' } $dir

'=== R04: a tag name with a quote'
$tag = "v0.1.0';Write-Host('INJECTED-'+'PWSH');'"
git check-ref-format "refs/tags/$tag"
"git accepts the tag name (exit $LASTEXITCODE): $tag"
$dir = New-Case 'r04' $dated
Step $check @{ 'github.ref_name' = $tag } $dir

'=== R05: an empty changelog section'
$dir = New-Case 'r05' {
    param($c)
    $i = [Array]::FindIndex([string[]]$c, [Predicate[string]] { param($l) $l -like '## 0.1.0 *' })
    $c[0..($i - 1)] + '## 0.2.0 — 2026-12-01' + ($c[$i] -replace 'not released yet', '2026-10-30') + $c[($i + 1)..($c.Count - 1)]
}
$props = Join-Path $dir 'companion/Directory.Build.props'
(Get-Content $props) -replace '<VersionPrefix>0\.1\.0<', '<VersionPrefix>0.2.0<' | Set-Content $props
git -C $dir add -A
git -C $dir @git commit -q -m version
git -C $dir update-ref refs/remotes/origin/main main
Step $check @{ 'github.ref_name' = 'v0.2.0' } $dir
Step $notes @{ 'needs.check.outputs.version' = '0.2.0' } $dir
'release-notes.md, first lines:'
Get-Content (Join-Path $dir 'release-notes.md') -ErrorAction SilentlyContinue | Select-Object -First 4

'=== R06: a heading without a date'
$dir = New-Case 'r06-undated' { param($c) $c -replace ' — not released yet', '' }
Get-Content (Join-Path $dir 'CHANGELOG.md') | Where-Object { $_ -like '## 0.1.0*' }
Step $check @{ 'github.ref_name' = 'v0.1.0' } $dir
$dir = New-Case 'r06-tbd' { param($c) $c -replace 'not released yet', 'TBD' }
Get-Content (Join-Path $dir 'CHANGELOG.md') | Where-Object { $_ -like '## 0.1.0*' }
Step $check @{ 'github.ref_name' = 'v0.1.0' } $dir

'=== control: a correct release on main'
$dir = New-Case 'control' $dated
Step $check @{ 'github.ref_name' = 'v0.1.0' } $dir
Get-Content (Join-Path $dir '_github_output')
