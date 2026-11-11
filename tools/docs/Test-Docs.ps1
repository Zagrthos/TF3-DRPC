<#
.SYNOPSIS
Checks the Markdown documentation of the repository against the rules of docs/00-meta/documentation-guide.md.

.DESCRIPTION
Checks every Markdown file that git lists (tracked, or untracked and not ignored): front matter, folders
and names, headings, links and anchors, index files, plan words, personal data and the one-sentence-per-line
style. The settings (allowed values, folder rules, files not yet moved into the new layout) are in
docs-config.psd1 next to this script. Each finding has a code (DOC001 to DOC012) that the guide's rules
marked "checked" map to.

With -Base the docs gate checks run too (DOC011, DOC012): the working tree is compared with the merge base
of <Base> and HEAD, so a changed file lists the documents that cover it, and those documents need a new
last-reviewed date.

Text output is one line per finding, "ERROR DOC007 docs/x.md:12 message" or "INFO ...", then a summary line.
The exit code is 1 when there is an error, 0 otherwise, 2 when the script itself can't run.

.PARAMETER Base
A git ref to compare with, for example origin/main. Turns on the docs gate checks.

.PARAMETER Json
Writes the findings as one JSON array to the output and nothing else: code, severity, file, line, message.

.PARAMETER AllowRecordEdit
Skips the records check (DOC012), for the one allowed correction: removing personal data from a record.
#>
param(
    [string]$Base,
    [switch]$Json,
    [switch]$AllowRecordEdit
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3.0

# A failure of the script itself (no git, a bad ref) is exit code 2; 1 is for findings.
trap {
    [Console]::Error.WriteLine($_.Exception.Message)
    exit 2
}

$root = (Join-Path $PSScriptRoot '..' '..' | Resolve-Path).Path
$config = Import-PowerShellDataFile (Join-Path $PSScriptRoot 'docs-config.psd1')

# git prints UTF-8; without this, Windows decodes non-ASCII file names with the OEM code page.
try { [Console]::OutputEncoding = [Text.UTF8Encoding]::new($false) } catch { }

$utf8 = [Text.UTF8Encoding]::new($false)
$today = (Get-Date).Date
$findings = [Collections.Generic.List[object]]::new()

#region Helpers

function Add-Finding {
    param([string]$Code, [string]$Severity, [string]$File, $Line, [string]$Message)
    $findings.Add([pscustomobject]@{
            code     = $Code
            severity = $Severity
            file     = $File
            line     = $Line
            message  = $Message
        })
}

function Add-Error {
    param([string]$Code, [string]$File, $Line, [string]$Message)
    Add-Finding $Code 'error' $File $Line $Message
}

function Invoke-Git {
    param([string[]]$GitArgs)
    $output = & git -c core.quotepath=false -C $root @GitArgs 2>$null
    if ($LASTEXITCODE -ne 0) { throw "git $($GitArgs -join ' ') failed ($LASTEXITCODE)" }
    $output
}

$globCache = @{}

# ** and **/ cross folders, * and ? stay inside one folder level, and the pattern covers the whole path.
function ConvertTo-GlobRegex {
    param([string]$Glob)
    if ($globCache.ContainsKey($Glob)) { return $globCache[$Glob] }
    $builder = [Text.StringBuilder]::new('^')
    $i = 0
    while ($i -lt $Glob.Length) {
        $c = $Glob[$i]
        if ($c -eq '*') {
            if ($i + 1 -lt $Glob.Length -and $Glob[$i + 1] -eq '*') {
                if ($i + 2 -lt $Glob.Length -and $Glob[$i + 2] -eq '/') {
                    [void]$builder.Append('(?:.*/)?')
                    $i += 3
                } else {
                    [void]$builder.Append('.*')
                    $i += 2
                }
            } else {
                [void]$builder.Append('[^/]*')
                $i++
            }
        } elseif ($c -eq '?') {
            [void]$builder.Append('[^/]')
            $i++
        } else {
            [void]$builder.Append([regex]::Escape([string]$c))
            $i++
        }
    }
    [void]$builder.Append('$')
    $regex = [regex]::new($builder.ToString(), [Text.RegularExpressions.RegexOptions]::CultureInvariant)
    $globCache[$Glob] = $regex
    $regex
}

function Test-Glob {
    param([string]$Path, [string[]]$Patterns)
    foreach ($pattern in $Patterns) {
        if ((ConvertTo-GlobRegex $pattern).IsMatch($Path)) { return $true }
    }
    $false
}

function Read-Lines {
    param([string]$Path)
    [regex]::Split([IO.File]::ReadAllText($Path, $utf8), '\r?\n')
}

function ConvertTo-Date {
    param([string]$Text)
    if ($Text -notmatch '^\d{4}-\d{2}-\d{2}$') { return $null }
    $date = [datetime]::MinValue
    $ok = [datetime]::TryParseExact($Text, 'yyyy-MM-dd', [Globalization.CultureInfo]::InvariantCulture, [Globalization.DateTimeStyles]::None, [ref]$date)
    if ($ok) { $date } else { $null }
}

# Returns a problem description for a date text, or nothing when it is a valid date that is not in the future.
function Get-DateProblem {
    param([string]$Text)
    $date = ConvertTo-Date $Text
    if ($null -eq $date) { return "'$Text' is not a valid YYYY-MM-DD date" }
    if ($date -gt $today) { return "'$Text' is in the future" }
}

#endregion

#region Front matter

# A plain YAML scalar can't start with, or contain, these.
function Get-ScalarProblem {
    param([string]$Text)
    if ($Text -match '^[\[\]{}&*!|>''"%@`#,?]') { return "value '$Text' starts with a character that is not allowed in a plain value" }
    if ($Text -match ':(\s|$)') { return "value '$Text' contains ': ', which only a quoted value may; reword it" }
    if ($Text -match '\s#') { return "value '$Text' contains ' #', which starts a comment" }
}

# Reads the small YAML subset the guide allows: key: scalar, key: [a, b] and key: with "  - item" lines.
function Read-FrontMatter {
    param([string[]]$Lines)
    $result = [pscustomobject]@{
        Found  = $false
        End    = -1
        Errors = [Collections.Generic.List[object]]::new()
        Data   = [ordered]@{}
    }
    if ($Lines.Count -eq 0 -or $Lines[0].TrimEnd() -ne '---') { return $result }
    $result.Found = $true
    for ($i = 1; $i -lt $Lines.Count; $i++) {
        if ($Lines[$i].TrimEnd() -eq '---') {
            $result.End = $i
            break
        }
    }
    if ($result.End -lt 0) { return $result }

    $i = 1
    while ($i -lt $result.End) {
        $line = $Lines[$i]
        if ($line.Trim() -eq '') {
            $i++
            continue
        }
        if (-not ($line -match '^([A-Za-z][A-Za-z0-9-]*):(?:[ \t]+(.*?))?[ \t]*$')) {
            $result.Errors.Add([pscustomobject]@{ Line = $i + 1; Message = "unsupported front matter line '$($line.Trim())'; only 'key: value', 'key: [a, b]' and 'key:' with '  - item' lines are allowed" })
            $i++
            continue
        }
        $key = $Matches[1]
        $value = if ($Matches.ContainsKey(2)) { $Matches[2] } else { '' }
        $entry = [pscustomobject]@{ Value = $null; IsList = $false; Line = $i + 1 }
        $problems = [Collections.Generic.List[string]]::new()

        if ($value -eq '') {
            $items = [Collections.Generic.List[string]]::new()
            while ($i + 1 -lt $result.End -and $Lines[$i + 1] -match '^[ \t]+-[ \t]+(.+?)[ \t]*$') {
                $i++
                $item = $Matches[1]
                $problem = Get-ScalarProblem $item
                if ($problem) { $problems.Add($problem) }
                $items.Add($item)
            }
            $entry.IsList = $true
            $entry.Value = $items.ToArray()
        } elseif ($value.StartsWith('[')) {
            if (-not $value.EndsWith(']')) {
                $problems.Add("flow list of '$key' is not closed with ']' on the same line")
            } else {
                $inner = $value.Substring(1, $value.Length - 2).Trim()
                $items = [Collections.Generic.List[string]]::new()
                if ($inner -ne '') {
                    foreach ($part in $inner.Split(',')) {
                        $item = $part.Trim()
                        if ($item -eq '') {
                            $problems.Add("empty item in the list of '$key'")
                            continue
                        }
                        $problem = Get-ScalarProblem $item
                        if ($problem) { $problems.Add($problem) }
                        $items.Add($item)
                    }
                }
                $entry.IsList = $true
                $entry.Value = $items.ToArray()
            }
        } else {
            $problem = Get-ScalarProblem $value
            if ($problem) { $problems.Add($problem) }
            $entry.Value = $value
        }

        foreach ($problem in $problems) {
            $result.Errors.Add([pscustomobject]@{ Line = $i + 1; Message = "'$key': $problem" })
        }
        if ($result.Data.Contains($key)) {
            $result.Errors.Add([pscustomobject]@{ Line = $i + 1; Message = "duplicate key '$key'" })
        } else {
            $result.Data[$key] = $entry
        }
        $i++
    }
    $result
}

function Get-FmScalar {
    param($Fm, [string]$Key)
    if (-not $Fm.Data.Contains($Key)) { return $null }
    $entry = $Fm.Data[$Key]
    if ($entry.IsList) { return $null }
    $entry.Value
}

function Get-FmList {
    param($Fm, [string]$Key)
    if (-not $Fm.Data.Contains($Key)) { return @() }
    $entry = $Fm.Data[$Key]
    if ($entry.IsList) { return @($entry.Value) }
    if ($entry.Value -eq '') { return @() }
    @($entry.Value)
}

#endregion

#region Markdown structure

# One character per line: F front matter, C fenced code, H HTML comment, N everything else.
function Get-Kinds {
    param([string[]]$Lines, [int]$FrontMatterEnd)
    $kinds = [char[]]::new($Lines.Count)
    $fenceChar = $null
    $fenceLength = 0
    $inComment = $false
    for ($i = 0; $i -lt $Lines.Count; $i++) {
        $line = $Lines[$i]
        if ($i -le $FrontMatterEnd) {
            $kinds[$i] = 'F'
        } elseif ($null -ne $fenceChar) {
            $kinds[$i] = 'C'
            if ($line -match '^\s*(`{3,}|~{3,})\s*$' -and $Matches[1][0] -eq $fenceChar -and $Matches[1].Length -ge $fenceLength) {
                $fenceChar = $null
            }
        } elseif ($inComment) {
            $kinds[$i] = 'H'
            if ($line.Contains('-->')) { $inComment = $false }
        } elseif ($line -match '^\s*(`{3,}|~{3,})') {
            $kinds[$i] = 'C'
            $fenceChar = $Matches[1][0]
            $fenceLength = $Matches[1].Length
        } elseif ($line.Contains('<!--') -and -not $line.Substring($line.IndexOf('<!--')).Contains('-->')) {
            $kinds[$i] = 'H'
            $inComment = $true
        } else {
            $kinds[$i] = 'N'
        }
    }
    , $kinds
}

$codeSpanRegex = [regex]::new('(?<!`)(`+)(?!`)(.+?)(?<!`)\1(?!`)')

# A code span becomes `x`, so its content counts for nothing and a span after a full stop is still seen.
function Remove-CodeSpans {
    param([string]$Text)
    $codeSpanRegex.Replace($Text, '`x`')
}

function Get-Headings {
    param([string[]]$Lines, [char[]]$Kinds)
    $headings = [Collections.Generic.List[object]]::new()
    for ($i = 0; $i -lt $Lines.Count; $i++) {
        if ($Kinds[$i] -ne 'N') { continue }
        if ($Lines[$i] -match '^ {0,3}(#{1,6})[ \t]+(.+?)[ \t]*$') {
            $text = ($Matches[2] -replace '[ \t]+#+$', '').Trim()
            if ($text -eq '') { continue }
            $headings.Add([pscustomobject]@{ Line = $i + 1; Level = $Matches[1].Length; Text = $text })
        }
    }
    , $headings
}

# GitHub's heading anchor: the text of the heading, lower case, without punctuation, spaces as hyphens.
function Get-Slug {
    param([string]$Heading)
    $text = [regex]::Replace($Heading, '!?\[([^\]]*)\]\([^)]*\)', '$1')
    $text = [regex]::Replace($text, '\[([^\]]*)\]\[[^\]]*\]', '$1')
    $text = [regex]::Replace($text, '<[^>]+>', '')
    $text = $text.Trim().ToLowerInvariant()
    $text = [regex]::Replace($text, '[^\p{L}\p{N}\p{M} _-]', '')
    $text.Replace(' ', '-')
}

function Get-Anchors {
    param([string[]]$Lines, [char[]]$Kinds)
    $anchors = [Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
    $seen = @{}
    foreach ($heading in (Get-Headings $Lines $Kinds)) {
        $slug = Get-Slug $heading.Text
        if ($seen.ContainsKey($slug)) {
            $seen[$slug]++
            [void]$anchors.Add("$slug-$($seen[$slug])")
        } else {
            $seen[$slug] = 0
            [void]$anchors.Add($slug)
        }
    }
    foreach ($m in [regex]::Matches(($Lines -join "`n"), '(?:\bname|\bid)\s*=\s*["'']([^"'']+)["'']')) {
        [void]$anchors.Add($m.Groups[1].Value)
    }
    , $anchors
}

#endregion

#region File set

$allFiles = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
foreach ($file in @(Invoke-Git @('ls-files', '-c', '-o', '--exclude-standard'))) {
    if ([IO.File]::Exists((Join-Path $root $file))) { [void]$allFiles.Add($file) }
}
$fileLower = @{}
$dirSet = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
$dirLower = @{}
foreach ($file in $allFiles) {
    $fileLower[$file.ToLowerInvariant()] = $file
    $index = $file.LastIndexOf('/')
    while ($index -gt 0) {
        $dir = $file.Substring(0, $index)
        if (-not $dirSet.Add($dir)) { break }
        $dirLower[$dir.ToLowerInvariant()] = $dir
        $index = $dir.LastIndexOf('/')
    }
}

$legacySet = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
foreach ($entry in $config.Legacy) { [void]$legacySet.Add($entry) }

# template and legacy get fewer checks; frontmatter is every other file under docs/; other is the rest.
function Get-Category {
    param([string]$Rel)
    if (Test-Glob $Rel $config.Templates) { return 'template' }
    if ($legacySet.Contains($Rel)) { return 'legacy' }
    if ($Rel.StartsWith('docs/')) { return 'frontmatter' }
    'other'
}

$models = [ordered]@{}
foreach ($rel in ($allFiles | Where-Object { $_.EndsWith('.md') } | Sort-Object { $_ } )) {
    if (Test-Glob $rel $config.Excluded) { continue }
    $lines = Read-Lines (Join-Path $root $rel)
    $category = Get-Category $rel
    $fm = $null
    $fmEnd = -1
    if ($category -eq 'frontmatter') {
        $fm = Read-FrontMatter $lines
        $fmEnd = $fm.End
    }
    $models[$rel] = [pscustomobject]@{
        Rel      = $rel
        Category = $category
        Lines    = $lines
        Kinds    = Get-Kinds $lines $fmEnd
        Fm       = $fm
        Links    = $null
    }
}

#endregion

#region Links

$rootPrefix = $root.TrimEnd('\', '/') + [IO.Path]::DirectorySeparatorChar
$inlineLinkRegex = [regex]::new('\]\((?<t><[^>]*>|(?:[^\s()]|\([^\s()]*\))*)(?:\s+(?:"[^"]*"|''[^'']*''))?\s*\)')
$refLinkRegex = [regex]::new('^\s{0,3}\[(?!\^)[^\]]+\]:\s*(?<t><[^>]*>|\S+)')

function Get-Links {
    param($Model)
    if ($null -ne $Model.Links) { return $Model.Links }
    $links = [Collections.Generic.List[object]]::new()
    for ($i = 0; $i -lt $Model.Lines.Count; $i++) {
        if ($Model.Kinds[$i] -ne 'N') { continue }
        $text = Remove-CodeSpans $Model.Lines[$i]
        $targets = [Collections.Generic.List[string]]::new()
        $ref = $refLinkRegex.Match($text)
        if ($ref.Success) { $targets.Add($ref.Groups['t'].Value) }
        foreach ($m in $inlineLinkRegex.Matches($text)) { $targets.Add($m.Groups['t'].Value) }
        foreach ($target in $targets) {
            $links.Add([pscustomobject]@{ Line = $i + 1; Target = $target.Trim('<', '>') })
        }
    }
    $Model.Links = $links
    $links
}

function ConvertFrom-UrlEncoding {
    param([string]$Text)
    try { [uri]::UnescapeDataString($Text) } catch { $Text }
}

# Finds the file a link points to. Skip is true for links that are not checked: other schemes, targets
# outside the repository and empty targets.
function Resolve-Link {
    param([string]$FromRel, [string]$Target)
    $result = [pscustomobject]@{ Skip = $true; Rel = $null; Fragment = ''; Exists = $false; CaseNote = $null }
    $text = $Target.Trim()
    if ($text -eq '' -or $text -match '^[A-Za-z][A-Za-z0-9+.-]*:' -or $text.StartsWith('//')) { return $result }

    $fragment = ''
    $hash = $text.IndexOf('#')
    if ($hash -ge 0) {
        $fragment = $text.Substring($hash + 1)
        $text = $text.Substring(0, $hash)
    }
    $query = $text.IndexOf('?')
    if ($query -ge 0) { $text = $text.Substring(0, $query) }
    $text = ConvertFrom-UrlEncoding $text
    $result.Fragment = ConvertFrom-UrlEncoding $fragment

    if ($text -eq '') {
        $result.Skip = $false
        $result.Rel = $FromRel
        $result.Exists = $true
        return $result
    }
    try {
        $baseDir = if ($text.StartsWith('/')) { $root } else { [IO.Path]::GetDirectoryName([IO.Path]::Combine($root, $FromRel)) }
        $full = [IO.Path]::GetFullPath([IO.Path]::Combine($baseDir, $text.TrimStart('/')))
    } catch {
        $result.Skip = $false
        $result.Rel = $text
        return $result
    }
    if ($full.TrimEnd('\', '/') -eq $root.TrimEnd('\', '/')) {
        $result.Skip = $false
        $result.Rel = ''
        $result.Exists = $true
        return $result
    }
    if (-not $full.StartsWith($rootPrefix, [StringComparison]::OrdinalIgnoreCase)) { return $result }

    $result.Skip = $false
    $rel = $full.Substring($rootPrefix.Length).Replace('\', '/').TrimEnd('/')
    $result.Rel = $rel
    $result.Exists = [IO.File]::Exists($full) -or [IO.Directory]::Exists($full)
    if ($result.Exists -and -not $allFiles.Contains($rel) -and -not $dirSet.Contains($rel)) {
        $lower = $rel.ToLowerInvariant()
        if ($fileLower.ContainsKey($lower)) { $result.CaseNote = $fileLower[$lower] }
        elseif ($dirLower.ContainsKey($lower)) { $result.CaseNote = $dirLower[$lower] }
    }
    $result
}

$anchorCache = @{}
function Get-FileAnchors {
    param([string]$Rel)
    if ($anchorCache.ContainsKey($Rel)) { return $anchorCache[$Rel] }
    if ($models.Contains($Rel)) {
        $model = $models[$Rel]
        $anchors = Get-Anchors $model.Lines $model.Kinds
    } else {
        $lines = Read-Lines (Join-Path $root $Rel)
        $fmEnd = -1
        if ((Get-Category $Rel) -eq 'frontmatter') { $fmEnd = (Read-FrontMatter $lines).End }
        $anchors = Get-Anchors $lines (Get-Kinds $lines $fmEnd)
    }
    $anchorCache[$Rel] = $anchors
    $anchors
}

#endregion

#region Per-file checks

$decisionNumbers = @{}
$planRegexes = foreach ($marker in $config.PlanMarkers) {
    [pscustomobject]@{ Marker = $marker; Regex = [regex]::new('(?i)\b' + [regex]::Escape($marker).Replace('\ ', '\s+') + '\b') }
}
$privacyRegexes = foreach ($pattern in $config.PrivacyPatterns) {
    [pscustomobject]@{ Regex = [regex]::new($pattern.Pattern); Message = $pattern.Message }
}
$allowedKeys = 'title', 'type', 'audience', 'area', 'covers', 'last-reviewed', 'verified', 'status', 'decided', 'superseded-by'
$scalarKeys = 'title', 'type', 'last-reviewed', 'status', 'decided', 'superseded-by'
$kebab = '[a-z0-9]+(?:-[a-z0-9]+)*'

# DOC001 and DOC002
function Test-FrontMatter {
    param($Model)
    $rel = $Model.Rel
    $fm = $Model.Fm
    if (-not $fm.Found) {
        Add-Error 'DOC001' $rel 1 'front matter is missing; the file must start with a --- line (see docs/00-meta/templates/)'
        return
    }
    if ($fm.End -lt 0) {
        Add-Error 'DOC001' $rel 1 'front matter is not closed with a --- line'
        return
    }
    if ($fm.Errors.Count -gt 0) {
        foreach ($problem in $fm.Errors) { Add-Error 'DOC001' $rel $problem.Line $problem.Message }
        return
    }

    $data = $fm.Data
    foreach ($key in $data.Keys) {
        if ($key -cnotin $allowedKeys) { Add-Error 'DOC002' $rel $data[$key].Line "unknown front matter key '$key'" }
    }
    foreach ($key in $scalarKeys) {
        if ($data.Contains($key) -and $data[$key].IsList) { Add-Error 'DOC002' $rel $data[$key].Line "'$key' takes a single value, not a list" }
    }
    foreach ($key in 'title', 'type', 'last-reviewed') {
        if (-not $data.Contains($key) -or ((Get-FmScalar $fm $key) -eq '' -and -not $data[$key].IsList)) {
            Add-Error 'DOC002' $rel 1 "required key '$key' is missing or empty"
        }
    }

    $type = Get-FmScalar $fm 'type'
    if ($data.Contains('type') -and $type -and $type -cnotin $config.Types) {
        Add-Error 'DOC002' $rel $data['type'].Line "type '$type' is not one of: $($config.Types -join ', ')"
        $type = $null
    }

    foreach ($spec in @(
            @{ Key = 'audience'; Allowed = $config.Audiences },
            @{ Key = 'area'; Allowed = $config.Areas })) {
        $key = $spec.Key
        $items = @(Get-FmList $fm $key)
        if ($items.Count -eq 0) {
            Add-Error 'DOC002' $rel 1 "required key '$key' is missing or has no value"
            continue
        }
        foreach ($item in $items) {
            if ($item -cnotin $spec.Allowed) { Add-Error 'DOC002' $rel $data[$key].Line "$key '$item' is not one of: $($spec.Allowed -join ', ')" }
        }
    }

    foreach ($key in 'last-reviewed', 'decided') {
        $value = Get-FmScalar $fm $key
        if ($value) {
            $problem = Get-DateProblem $value
            if ($problem) { Add-Error 'DOC002' $rel $data[$key].Line "$key $problem" }
        }
    }

    $covers = @(Get-FmList $fm 'covers')
    if ($type -and $type -cin $config.TypesRequiringCovers -and $covers.Count -eq 0 -and -not (Test-Glob $rel $config.CoversOptional)) {
        Add-Error 'DOC002' $rel 1 "type '$type' needs a 'covers' list of the files it describes"
    }
    foreach ($glob in $covers) {
        $regex = ConvertTo-GlobRegex $glob
        $matched = $false
        foreach ($file in $allFiles) {
            if ($regex.IsMatch($file)) {
                $matched = $true
                break
            }
        }
        if (-not $matched) { Add-Error 'DOC002' $rel $data['covers'].Line "covers pattern '$glob' matches no file" }
    }

    foreach ($item in (Get-FmList $fm 'verified')) {
        if ($item -match '^(windows|macos) (\d{4}-\d{2}-\d{2})( build \d+)?$') {
            $problem = Get-DateProblem $Matches[2]
            if ($problem) { Add-Error 'DOC002' $rel $data['verified'].Line "verified item '$item': date $problem" }
        } else {
            Add-Error 'DOC002' $rel $data['verified'].Line "verified item '$item' does not match '<windows|macos> YYYY-MM-DD [build N]'"
        }
    }

    $status = Get-FmScalar $fm 'status'
    if ($type -ceq 'decision') {
        if (-not $status) {
            Add-Error 'DOC002' $rel 1 "a decision needs a 'status' (proposed, accepted or superseded)"
        } elseif ($status -cnotin 'proposed', 'accepted', 'superseded') {
            Add-Error 'DOC002' $rel $data['status'].Line "decision status '$status' is not one of: proposed, accepted, superseded"
        }
        if (-not $data.Contains('decided')) { Add-Error 'DOC002' $rel 1 "a decision needs a 'decided' date" }
        $supersededBy = Get-FmScalar $fm 'superseded-by'
        if ($status -ceq 'superseded') {
            if (-not $supersededBy) {
                Add-Error 'DOC002' $rel 1 "a superseded decision needs 'superseded-by'"
            } elseif ($supersededBy -notmatch '^\d{4}$') {
                Add-Error 'DOC002' $rel $data['superseded-by'].Line "superseded-by '$supersededBy' must be a four-digit decision number"
            } elseif (-not ($allFiles | Where-Object { $_ -match "^docs/70-decisions/$supersededBy-[^/]+\.md$" })) {
                Add-Error 'DOC002' $rel $data['superseded-by'].Line "superseded-by $supersededBy names no file docs/70-decisions/$supersededBy-*.md"
            }
        } elseif ($data.Contains('superseded-by')) {
            Add-Error 'DOC002' $rel $data['superseded-by'].Line "'superseded-by' is only for a decision with status superseded"
        }
    } elseif ($type) {
        if ($status -and $status -cnotin 'current', 'deprecated') {
            Add-Error 'DOC002' $rel $data['status'].Line "status '$status' is not allowed for type '$type' (only current or deprecated)"
        }
        foreach ($key in 'decided', 'superseded-by') {
            if ($data.Contains($key)) { Add-Error 'DOC002' $rel $data[$key].Line "'$key' is only for decisions" }
        }
    }
}

# DOC003
function Test-Headings {
    param($Model)
    $headings = Get-Headings $Model.Lines $Model.Kinds
    $h1 = @($headings | Where-Object { $_.Level -eq 1 })
    if ($h1.Count -eq 0) {
        Add-Error 'DOC003' $Model.Rel 1 'the file has no H1 heading'
    } elseif ($h1.Count -gt 1) {
        Add-Error 'DOC003' $Model.Rel $h1[1].Line "the file has $($h1.Count) H1 headings; it needs exactly one"
    }
    if ($h1.Count -ge 1 -and $null -ne $Model.Fm -and $Model.Fm.Errors.Count -eq 0) {
        $title = Get-FmScalar $Model.Fm 'title'
        if ($title -and $h1[0].Text -cne $title) {
            Add-Error 'DOC003' $Model.Rel $h1[0].Line "the H1 '$($h1[0].Text)' differs from the title '$title'"
        }
    }
    $previous = 0
    foreach ($heading in $headings) {
        if ($heading.Level -gt $previous + 1) {
            Add-Error 'DOC003' $Model.Rel $heading.Line "heading level jumps from H$previous to H$($heading.Level)"
        }
        $previous = $heading.Level
    }
}

# DOC004 and DOC005
function Test-Naming {
    param($Model)
    $rel = $Model.Rel
    $parts = $rel.Split('/')
    $name = $parts[-1]
    if ($parts.Count -eq 2) {
        if ($name -cne 'README.md') { Add-Error 'DOC004' $rel $null 'a Markdown file directly in docs/ must be README.md or listed as Legacy in the settings; move it into a numbered folder' }
        return
    }
    $folder = $parts[1]
    if ($folder -cnotin $config.Folders -and $folder -cnotin $config.LegacyFolders) {
        Add-Error 'DOC004' $rel $null "folder '$folder' is not one of the numbered folders: $($config.Folders -join ', ')"
    }
    if ($name -cne 'README.md' -and $name -cnotmatch "^$kebab\.md$") {
        Add-Error 'DOC004' $rel $null "file name '$name' is not lower-case kebab case"
    }

    $title = if ($Model.Fm.Errors.Count -eq 0) { Get-FmScalar $Model.Fm 'title' } else { $null }
    if ($folder -ceq '70-decisions' -and $parts.Count -eq 3 -and $name -cne 'README.md') {
        if ($name -cmatch "^(\d{4})-$kebab\.md$") {
            $number = $Matches[1]
            if ($decisionNumbers.ContainsKey($number)) {
                Add-Error 'DOC004' $rel $null "decision number $number is also used by $($decisionNumbers[$number])"
            } else {
                $decisionNumbers[$number] = $rel
            }
            if ($title -and -not $title.StartsWith("$number ", [StringComparison]::Ordinal)) {
                Add-Error 'DOC004' $rel $Model.Fm.Data['title'].Line "the title must start with the decision number '$number' and a space"
            }
        } else {
            Add-Error 'DOC004' $rel $null "decision file name '$name' must be NNNN-<slug>.md"
        }
    }
    if ($folder -ceq '90-records' -and $parts.Count -eq 3 -and $name -cne 'README.md' -and $name -cnotmatch "^\d{4}-\d{2}-\d{2}-$kebab\.md$") {
        Add-Error 'DOC004' $rel $null "record file name '$name' must be YYYY-MM-DD-<slug>.md"
    }

    if ($Model.Fm.Errors.Count -gt 0) { return }
    $type = Get-FmScalar $Model.Fm 'type'
    if (-not $type) { return }
    if ($name -ceq 'README.md') {
        if ($type -cne 'index') { Add-Error 'DOC005' $rel $Model.Fm.Data['type'].Line "a README.md is the folder's index and has type 'index', not '$type'" }
    } elseif ($type -ceq 'index') {
        Add-Error 'DOC005' $rel $Model.Fm.Data['type'].Line "type 'index' is only for README.md"
    } elseif ($config.FolderTypes.ContainsKey($folder) -and $type -cnotin $config.FolderTypes[$folder]) {
        Add-Error 'DOC005' $rel $Model.Fm.Data['type'].Line "type '$type' does not belong in docs/$folder/; it takes: $($config.FolderTypes[$folder] -join ', ')"
    }
}

# DOC007
function Test-Links {
    param($Model)
    foreach ($link in (Get-Links $Model)) {
        $target = Resolve-Link $Model.Rel $link.Target
        if ($target.Skip) { continue }
        if (-not $target.Exists) {
            Add-Error 'DOC007' $Model.Rel $link.Line "link target '$($link.Target)' does not exist"
            continue
        }
        if ($target.CaseNote) {
            Add-Error 'DOC007' $Model.Rel $link.Line "link '$($link.Target)' differs in letter case from '$($target.CaseNote)', which breaks on GitHub"
            continue
        }
        if ($target.Fragment -ne '' -and $target.Rel.EndsWith('.md') -and $allFiles.Contains($target.Rel)) {
            if (-not (Get-FileAnchors $target.Rel).Contains($target.Fragment)) {
                Add-Error 'DOC007' $Model.Rel $link.Line "link '$($link.Target)' points to heading '#$($target.Fragment)', which '$($target.Rel)' does not have"
            }
        }
    }
}

# DOC008
function Test-PlanMarkers {
    param($Model)
    for ($i = 0; $i -lt $Model.Lines.Count; $i++) {
        if ($Model.Kinds[$i] -ne 'N') { continue }
        $text = Remove-CodeSpans $Model.Lines[$i]
        foreach ($planRegex in $planRegexes) {
            if ($planRegex.Regex.IsMatch($text)) {
                Add-Error 'DOC008' $Model.Rel ($i + 1) "plan word '$($planRegex.Marker)'; plans and open work belong in docs/10-overview/status.md, here state what is true today"
            }
        }
    }
}

# DOC009
function Test-Privacy {
    param($Model)
    for ($i = 0; $i -lt $Model.Lines.Count; $i++) {
        foreach ($privacy in $privacyRegexes) {
            if ($privacy.Regex.IsMatch($Model.Lines[$i])) { Add-Error 'DOC009' $Model.Rel ($i + 1) $privacy.Message }
        }
    }
}

function Get-LineClass {
    param([string]$Line, [char]$Kind)
    if ($Kind -ne 'N') { return 'skip' }
    if ($Line.Trim() -eq '') { return 'blank' }
    if ($Line -match '^ {0,3}#{1,6}(\s|$)') { return 'heading' }
    if ($Line -match '^\s*\|') { return 'table' }
    if ($Line -match '^\s*<') { return 'html' }
    if ($Line -match '^\s*([-*_])(\s*\1){2,}\s*$') { return 'rule' }
    if ($Line -match '^\s{0,3}\[(?!\^)[^\]]+\]:\s') { return 'refdef' }
    if ($Line -match '^\s*(?:[-*+]|\d+[.)])\s+') { return 'list' }
    'prose'
}

$sentenceRegex = [regex]::new('[.!?]["'')\]*_]*\s+[\p{Lu}`\[*_]')
$sentenceEndRegex = [regex]::new('(?:[.!?:;]["'')\]*_]*|\))$')

# DOC010
function Test-LineStyle {
    param($Model)
    $lines = $Model.Lines
    for ($i = 0; $i -lt $lines.Count; $i++) {
        $class = Get-LineClass $lines[$i] $Model.Kinds[$i]
        if ($class -ne 'prose' -and $class -ne 'list') { continue }
        $text = Remove-CodeSpans $lines[$i]
        $text = [regex]::Replace($text, '^\s*>\s*', '')
        $text = [regex]::Replace($text, '^\s*(?:[-*+]|\d+[.)])\s+', '')
        if ($sentenceRegex.IsMatch($text)) {
            Add-Error 'DOC010' $Model.Rel ($i + 1) 'more than one sentence on a line; put each sentence on its own line'
        }
        if (-not $sentenceEndRegex.IsMatch($text.TrimEnd()) -and $i + 1 -lt $lines.Count) {
            $next = Get-LineClass $lines[$i + 1] $Model.Kinds[$i + 1]
            if ($next -eq 'prose') {
                Add-Error 'DOC010' $Model.Rel ($i + 1) 'the sentence is wrapped over several lines; keep it on one line'
            }
        }
    }
}

foreach ($model in $models.Values) {
    $rel = $model.Rel
    $category = $model.Category
    if ($category -ne 'template') {
        if ($category -eq 'frontmatter') {
            Test-FrontMatter $model
            Test-Naming $model
        }
        if (-not (Test-Glob $rel $config.LinkCheckExempt)) { Test-Links $model }
        if ($category -in 'frontmatter', 'other') {
            if (-not (Test-Glob $rel $config.HeadingCheckExempt)) { Test-Headings $model }
            if (-not (Test-Glob $rel $config.LineStyleExempt)) { Test-LineStyle $model }
        }
        if ($category -eq 'frontmatter' -and $model.Fm.Errors.Count -eq 0 -and $model.Fm.End -ge 0) {
            $type = Get-FmScalar $model.Fm 'type'
            if ($type -and $type -cin $config.PlanMarkerTypes -and -not (Test-Glob $rel $config.PlanMarkerExempt)) { Test-PlanMarkers $model }
        }
    }
    Test-Privacy $model
}

#endregion

#region Index files

# DOC006
function Test-IndexLinks {
    $links = @{}
    foreach ($model in $models.Values) { $links[$model.Rel] = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal) }
    foreach ($model in $models.Values) {
        if ($model.Rel -ceq 'docs/README.md' -or $model.Rel -cmatch '^docs/.+/README\.md$' -or $model.Rel -ceq 'reviews/README.md') {
            foreach ($link in (Get-Links $model)) {
                $target = Resolve-Link $model.Rel $link.Target
                if (-not $target.Skip -and $target.Exists) { [void]$links[$model.Rel].Add($target.Rel) }
            }
        }
    }

    $docsIndex = 'docs/README.md'
    if ($models.Contains($docsIndex)) {
        foreach ($rel in $models.Keys) {
            if (-not $rel.StartsWith('docs/') -or $rel -ceq $docsIndex -or $models[$rel].Category -eq 'template') { continue }
            if (-not $links[$docsIndex].Contains($rel)) { Add-Error 'DOC006' $docsIndex $null "docs/README.md does not link $rel" }
        }
    }

    foreach ($index in $models.Keys) {
        if ($index -cnotmatch '^docs/.+/README\.md$' -or $models[$index].Category -eq 'template') { continue }
        $folder = $index.Substring(0, $index.Length - 'README.md'.Length)
        foreach ($rel in $models.Keys) {
            if ($rel -ceq $index -or -not $rel.StartsWith($folder) -or $rel.IndexOf('/', $folder.Length) -ge 0) { continue }
            if (-not $links[$index].Contains($rel)) { Add-Error 'DOC006' $index $null "$index does not link $rel" }
        }
    }

    if ($models.Contains('reviews/README.md')) {
        $folders = [Collections.Generic.SortedSet[string]]::new([StringComparer]::Ordinal)
        foreach ($file in $allFiles) {
            if ($file -cmatch '^(reviews/[^/]+/\d{4}-\d{2}-\d{2})/') { [void]$folders.Add($Matches[1]) }
        }
        foreach ($folder in $folders) {
            $linked = $false
            foreach ($target in $links['reviews/README.md']) {
                if ($target -ceq $folder -or $target.StartsWith("$folder/")) {
                    $linked = $true
                    break
                }
            }
            if (-not $linked) { Add-Error 'DOC006' 'reviews/README.md' $null "reviews/README.md does not link the review folder $folder/" }
        }
    }
}

Test-IndexLinks

#endregion

#region Docs gate

if ($Base) {
    $mergeBase = @(Invoke-Git @('merge-base', $Base, 'HEAD'))[0]
    if (-not $mergeBase) { throw "No merge base of '$Base' and HEAD" }
    $baseFiles = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
    foreach ($file in @(Invoke-Git @('ls-tree', '-r', '--name-only', $mergeBase))) { [void]$baseFiles.Add($file) }

    $changed = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
    foreach ($row in @(Invoke-Git @('diff', '--name-status', '-M', $mergeBase))) {
        $columns = $row -split "`t"
        for ($i = 1; $i -lt $columns.Count; $i++) { [void]$changed.Add($columns[$i]) }
    }
    foreach ($file in @(Invoke-Git @('ls-files', '-o', '--exclude-standard'))) { [void]$changed.Add($file) }

    function Get-BaseLines {
        param([string]$Rel)
        @(Invoke-Git @('show', "${mergeBase}:$Rel")) | ForEach-Object { $_ -replace '\r$', '' }
    }

    # The date of the change: today while the working tree has edits, otherwise the newest commit since the
    # merge base. Merge commits don't count, because CI checks out a merge commit made at run time.
    $changeDate = $today.ToString('yyyy-MM-dd', [Globalization.CultureInfo]::InvariantCulture)
    if (@(Invoke-Git @('status', '--porcelain')).Count -eq 0) {
        $newest = @(Invoke-Git @('log', '--no-merges', '-1', '--format=%cs', "$mergeBase..HEAD"))[0]
        if ($newest) { $changeDate = $newest }
    }

    $triggers = [Collections.Generic.List[string]]::new()
    foreach ($file in ($changed | Sort-Object { $_ })) {
        if ((Get-Category $file) -eq 'frontmatter' -and $file.EndsWith('.md')) { continue }
        $triggers.Add($file)
    }

    # DOC011
    foreach ($model in $models.Values) {
        if ($model.Category -ne 'frontmatter' -or $model.Fm.Errors.Count -gt 0 -or $model.Fm.End -lt 0) { continue }
        $rel = $model.Rel
        $isNew = -not $baseFiles.Contains($rel)
        $current = Get-FmScalar $model.Fm 'last-reviewed'
        $previous = $null
        if (-not $isNew) {
            $baseFm = Read-FrontMatter @(Get-BaseLines $rel)
            $previous = if ($baseFm.End -ge 0) { Get-FmScalar $baseFm 'last-reviewed' } else { $null }
        }
        # A date that is not older than the change counts as reviewed: a second change on the same day can't move it.
        $bumped = $isNew -or $current -cne $previous -or [string]::CompareOrdinal($current, $changeDate) -ge 0

        $hits = [Collections.Generic.List[string]]::new()
        $regexes = @(foreach ($glob in (Get-FmList $model.Fm 'covers')) { ConvertTo-GlobRegex $glob })
        if ($regexes.Count -gt 0) {
            foreach ($trigger in $triggers) {
                foreach ($regex in $regexes) {
                    if ($regex.IsMatch($trigger)) {
                        $hits.Add($trigger)
                        break
                    }
                }
            }
        }
        $reported = $false
        if ($hits.Count -gt 0) {
            Add-Finding 'DOC011' 'info' $rel $null "check this document, it covers changed files: $($hits -join ', ')"
            if (-not $bumped) {
                Add-Error 'DOC011' $rel $model.Fm.Data['last-reviewed'].Line "covers changed files ($($hits -join ', ')) but last-reviewed was not updated"
                $reported = $true
            }
        }
        if (-not $reported -and $changed.Contains($rel) -and -not $isNew -and -not $bumped) {
            Add-Error 'DOC011' $rel $model.Fm.Data['last-reviewed'].Line "the document changed since $Base but last-reviewed was not updated"
        }
    }

    # DOC012
    if (-not $AllowRecordEdit) {
        $reviewFolder = '^reviews/[^/]+/\d{4}-\d{2}-\d{2}/'
        foreach ($row in @(Invoke-Git @('diff', '--name-status', '--no-renames', $mergeBase, '--', 'reviews'))) {
            $columns = $row -split "`t"
            if ($columns[0] -cin 'M', 'D', 'T' -and $columns[1] -cmatch $reviewFolder) {
                $what = if ($columns[0] -ceq 'D') { 'deleted' } else { 'changed' }
                Add-Error 'DOC012' $columns[1] $null "review file was $what; reviews are records and stay as written (-AllowRecordEdit for removing personal data only)"
            }
        }
        foreach ($rel in $baseFiles) {
            if ($rel -cnotmatch '^docs/90-records/[^/]+\.md$' -or $rel -ceq 'docs/90-records/README.md') { continue }
            if (-not $allFiles.Contains($rel)) {
                Add-Error 'DOC012' $rel $null 'record was deleted; records stay as written (-AllowRecordEdit for removing personal data only)'
                continue
            }
            $getBody = {
                param([string[]]$Lines)
                $fm = Read-FrontMatter $Lines
                $start = if ($fm.End -ge 0) { $fm.End + 1 } else { 0 }
                ($Lines[$start..($Lines.Count - 1)] -join "`n").TrimEnd()
            }
            $before = & $getBody @(Get-BaseLines $rel)
            $after = & $getBody $models[$rel].Lines
            if ($before -cne $after) {
                Add-Error 'DOC012' $rel $null 'the body of a record changed; records stay as written (-AllowRecordEdit for removing personal data only)'
            }
        }
    }
}

#endregion

#region Output

$sorted = @($findings | Sort-Object -Stable -Property file, @{ Expression = { if ($null -eq $_.line) { 0 } else { $_.line } } }, code)
$errorCount = @($sorted | Where-Object { $_.severity -eq 'error' }).Count
$infoCount = $sorted.Count - $errorCount

if ($Json) {
    # An empty pipeline gives no output at all, so an empty result is written by hand.
    $text = if ($sorted.Count -eq 0) { '[]' } else { $sorted | ConvertTo-Json -AsArray -Depth 3 }
    [Console]::Out.WriteLine($text)
} else {
    foreach ($finding in $sorted) {
        $where = if ($null -eq $finding.line) { $finding.file } else { "$($finding.file):$($finding.line)" }
        [Console]::Out.WriteLine("$($finding.severity.ToUpperInvariant()) $($finding.code) $where $($finding.message)")
    }
    [Console]::Out.WriteLine("Checked $($models.Count) Markdown files: $errorCount error(s), $infoCount info.")
}

exit ($errorCount -gt 0 ? 1 : 0)

#endregion
