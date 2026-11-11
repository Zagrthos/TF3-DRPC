@{
    # Settings of Test-Docs.ps1. Paths are relative to the repository root, with forward slashes.
    # Patterns are globs: ** crosses folders, * and ? stay inside one folder level.

    # Markdown that no check looks at (a vendored skill).
    Excluded = @(
        '.claude/**'
    )

    # Templates for new documents: only the privacy check applies, because they hold placeholders.
    Templates = @(
        'docs/00-meta/templates/**'
    )

    # Allowed values of the front matter keys type, audience and area.
    Types = @('guide', 'rules', 'reference', 'explanation', 'decision', 'runbook', 'record', 'index')
    Audiences = @('player', 'contributor', 'maintainer', 'agent')
    Areas = @('mod', 'app', 'windows', 'macos', 'discord', 'translations', 'accessibility', 'docs', 'tools')

    # Types that need a covers key, and the paths where covers stays optional.
    TypesRequiringCovers = @('reference', 'rules', 'guide', 'runbook')
    CoversOptional = @('docs/00-meta/**')

    # The numbered folders under docs/, and folders that hold no documents to check (sample data).
    Folders = @('00-meta', '10-overview', '20-development', '30-conventions', '40-reference', '50-testing', '60-runbooks', '70-decisions', '90-records')
    LegacyFolders = @('log-samples')

    # The types each folder accepts. A README.md is an index in every folder.
    FolderTypes = @{
        '00-meta'        = @('rules', 'reference', 'guide')
        '10-overview'    = @('explanation', 'reference')
        '20-development' = @('guide')
        '30-conventions' = @('rules')
        '40-reference'   = @('reference')
        '50-testing'     = @('guide', 'reference')
        '60-runbooks'    = @('runbook')
        '70-decisions'   = @('decision')
        '90-records'     = @('record')
    }

    # Files not yet moved into the new layout: no front matter, naming, folder, heading or line-style
    # checks; links and privacy are still checked. This list only shrinks.
    Legacy = @(
        'docs/companion.md'
        'docs/game-log.md'
        'docs/mod.md'
        'docs/platform-parity.md'
        'docs/presence.md'
        'docs/status-line.md'
        'docs/testing.md'
        'README.md'
        '.github/PULL_REQUEST_TEMPLATE.md'
        'tools/test-run/README.md'
    )

    # Files exempt from single checks. Records are never rewritten (reviews/<area>/<date>/ only;
    # reviews/README.md is checked). The changelog keeps one line per paragraph for the release text.
    LineStyleExempt = @('CHANGELOG.md', 'reviews/*/**')
    LinkCheckExempt = @('reviews/*/**')
    HeadingCheckExempt = @('reviews/*/**')

    # Words that announce plans; they belong in docs/10-overview/status.md only.
    PlanMarkers = @('planned', 'not yet', 'TODO', 'will be', 'for now')
    PlanMarkerTypes = @('reference', 'rules', 'guide', 'explanation')
    PlanMarkerExempt = @('docs/10-overview/status.md')

    # Personal data that must not appear in any Markdown file (the whole text, code blocks included).
    PrivacyPatterns = @(
        @{
            Pattern = '(?i)\b[a-z]:\\Users\\(?!<|Public\\)'
            Message = 'Windows user folder; use a placeholder such as <userdata> or <repo>'
        }
        @{
            Pattern = '/Users/(?!<|Shared/)\w'
            Message = 'macOS user folder; use a placeholder such as <repo>'
        }
        @{
            Pattern = '/home/(?!<)\w'
            Message = 'Linux home folder; use a placeholder such as <repo>'
        }
        @{
            Pattern = '(?i)(?<![\w.%+-])(?!noreply@)[a-z0-9._%+-]+@(?!(?:users\.noreply\.github\.com|example\.(?:com|org))\b)[a-z0-9-]+(?:\.[a-z0-9-]+)*\.[a-z]{2,}'
            Message = 'e-mail address'
        }
        @{
            Pattern = '\b7656119\d{10}\b'
            Message = 'Steam account ID (SteamID64)'
        }
    )
}
