# tf3-modding skill

A skill for coding agents that write Transport Fever 3 mods. It contains the complete
scripting reference from https://tf3-docs.transportfever.net as plain Markdown, a searchable
symbol index and the modding guides, so the agent can look up the real API instead of
guessing it.

## Install

The folder follows the Agent Skills layout (`SKILL.md` with a name and description).

| Agent | Where to put the `tf3-modding` folder |
|---|---|
| Claude Code | `~/.claude/skills/` (all projects) or `.claude/skills/` in your mod's folder |
| Codex CLI | `~/.codex/skills/` |
| Gemini CLI | `~/.gemini/skills/`, or the skills folder your version documents |
| Other agents | Copy the folder into your mod project and add a line to your `AGENTS.md`, `GEMINI.md` or rules file: "For Transport Fever 3 modding, read `tf3-modding/SKILL.md` first." |

Restart the agent afterwards so it picks the skill up.

## Updates

`VERSION` holds the skill's version. The current one is at
https://tf3-docs.transportfever.net/agents/skill-version.txt. The skill tells the agent to compare
the two once per session, to use the online reference when they differ, and to offer you
the update. It only replaces the folder after you agree.

## Contents

- `SKILL.md`: when to use the skill and how to look things up
- `references/index.tsv`: one row per symbol (name, kind, signature, how to call it, base-game uses, description)
- `references/api/` and `references/base/`: one Markdown file per definition module
- `references/guides/`: the guide index and every guide on the site

The reference is generated from the game's own definition files and does not include them.
Your game installation has them in `api/tealdef` and `base/tealdef`.
