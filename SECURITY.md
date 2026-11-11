# Security

## Reporting a vulnerability

Report a vulnerability privately through the repository's [Security tab](https://github.com/Zagrthos/TF3-DRPC/security/advisories/new) ("Report a vulnerability"), not in a public issue.
Describe what you found, how to reproduce it, and which app version or commit it affects.

## How the project handles input

- The app treats paths, log content and file input as untrusted: it reads the game log, the game's language setting and its own `settings.json`, and talks only to the Discord app on the same computer.
- The repository holds no tokens or credentials; the Discord application ID is public and not a secret.
- Build output, `DATA/` folders, logs and machine-specific settings stay out of commits.

## Keeping personal data out

Everything in this repository is public.
Before you commit logs, screenshots or other evidence:

- Replace local paths with placeholders such as `<repo>`, `<Steam>`, `<userdata>` and `<steam-account>`.
- Leave out user and computer names, account IDs, e-mail addresses, hardware, display size and scaling, time zones and installed software.
- Record UI coordinates in device-independent pixels, and strip the metadata from images.
- Check the presence text in the app's settings window; screenshots of the Discord window stay out, because it shows content unrelated to the test.

`tools/docs/Test-Docs.ps1` checks Markdown for user folders, e-mail addresses and Steam account IDs.
