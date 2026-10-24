# Companion review of 2026-10-15

Reviewed: the companion app as of 2026-10-15.

Thirteen findings were reproduced: eleven P2 findings and two P3 findings. Production
source, configuration and resources were left unchanged. P2 means a material correctness
or recovery defect under the stated trigger; P3 means a lower-priority robustness defect.
A condition is part of the finding, not a claim that it happens in an ordinary game run.

The review covered all three companion projects, the Windows platform code, service
coordination, ViewModels and XAML, log input, state transitions, localisation, settings,
Discord IPC, constants and result types, build/publish targets and the existing tests.
The reproductions run unchanged Core code and linked, unchanged app code; the published
app was also exercised on Windows. See [README.md](README.md) for commands, the harness
boundary, controls and validation. Recorded results are in
[results-windows.json](results-windows.json) and
[smoke-results-windows.json](smoke-results-windows.json).

## F12 — P2: A blocked log directory crashes the app during startup

**Locations:** [MauiProgram.cs](../../../companion/Tf3Drpc.App/MauiProgram.cs#L62),
lines 62–71; [WindowsPathService.cs](../../../companion/Tf3Drpc.App/Platforms/Windows/WindowsPathService.cs#L25),
lines 25–28 and 109–115.

`IsDataWritable` probes only the top-level `DATA` directory. It can be true while
`DATA/logs` cannot be created or written. File logging is then registered without a
fallback. The pinned NReco provider throws while initialising its directory; this escapes
application startup. The user gets neither the settings window nor a working tray app.
Logging failure should not prevent a companion from running with defaults or displaying
its storage problem.

**Reproduction:** harness case `F12` (`BlockedLogProviderAsync`), plus the
`blocked-log-folder` case in `smoke.ps1`. Create a writable `DATA` directory, put a regular
file named `logs` inside it, then start a private copy of the published app. The harness
also invokes the actual Windows path probe and the real NReco 1.4.0 provider with the
app's filename/options.

**Expected:** a usable app with file logging disabled and a visible storage error.
**Observed:** the path probe reports true; provider initialisation throws `IOException`
("Cannot create … DATA\\logs because a file or directory with the same name already
exists"). The published app exits with `-1073741189` (`0xc000027b`). The normal private
copy stays running. Windows Application crash events corroborate the failing copy in
[startup-crash-events.txt](startup-crash-events.txt).

**Correction:** validate log storage separately, isolate file-provider initialisation
from app startup, and fall back to a usable UI when it fails. Checking once should also
not make later logging I/O failures fatal.

## F06 — P2: Discord backpressure can stop the whole monitor loop

**Locations:** [DiscordIpcConnection.cs](../../../companion/Tf3Drpc.Core/Discord/DiscordIpcConnection.cs#L117),
lines 117–126; [MonitorService.cs](../../../companion/Tf3Drpc.App/Services/MonitorService.cs#L159),
lines 159–162.

Activity writes and flushes use the caller's cancellation token without an operation
deadline. The caller is the one monitor loop, whose token is cancelled on app shutdown.
After a successful handshake, a Discord endpoint that remains open but stops reading
can fill the pipe buffer and leave `UpdateAsync` pending. The loop then cannot perform
another process check, read game events, publish its UI status or notice that the game
has exited. Handshake and close deadlines do not bound this activity-write path.

**Reproduction:** `F06` (`PipeBackpressureAsync`). A real Windows named-pipe server,
with a requested 256-byte buffer, reads the handshake and sends READY, then stops
reading. The client sends different, ordinary-sized presences; a controlled clock
advances past the rate limit between attempts.

**Expected:** a bounded failed send that disconnects and lets the monitor continue.
**Observed:** after one completed update, the next remains blocked for more than
6.1 seconds, with `IsConnected=true`. Only the test's explicit cancellation releases
it. Source inspection confirms there is no activity-write deadline.

**Correction:** apply a linked per-operation deadline to write-lock acquisition,
activity writes and flushes; close the connection on timeout. Retain the existing
shutdown cancellation behaviour.

**Limit:** this tests genuine OS pipe backpressure with a controlled peer; it does not
claim that an ordinary Discord client stopped reading in a live game session.

## F01 — P2: Replacing the game process between ticks does not restart log following

**Location:** [MonitorService.cs](../../../companion/Tf3Drpc.App/Services/MonitorService.cs#L165),
lines 165–181, especially the boolean-only condition at line 167.

The state machine recognises a different process start time and resets its state, but
`HandleProcessChange` recognises only a sampled `false → true` running transition.
If the old process exits and a replacement starts between ticks, log following retains
its old offset, game paths are not resolved again, and profile refresh is not restarted.
This can happen with a quick replacement or delayed monitor ticks.

**Reproduction:** `F01` (`FastRestartAsync`). Feed a process and a 2048-byte log through
the actual monitor tick until Playing. Replace the log with an equally sized new
session and provide a new start time while `IsRunning` remains true. Then append a
valid new status and tick again, including a tick beyond the path retry interval.

**Expected:** recognise the new process, restart the log reader, replay its session
header and reach Playing with the new status.
**Observed:** the new process remains Starting and Discord receives "Starting the
game", even after fresh telemetry. `ResolveGamePaths` was called only once. The control
with a sampled stopped tick correctly reaches Playing after the replacement.

**Correction:** make process identity changes trigger the monitor's restart work as
well as the state machine's reset. Account for a start time that becomes readable later
without treating that as another process.

## F02 — P2: A log truncated and regrown between polls loses lines and mixes sessions

**Location:** [GameLogFollower.cs](../../../companion/Tf3Drpc.Core/GameLog/GameLogFollower.cs#L54),
lines 54–75, especially the length-only check at line 56.

A rewrite is detected only when the length observed at a poll is smaller than the old
read offset. If the file has already regrown to that offset or beyond, the reader seeks
into the replacement file and retains the previous file's partial line. New events can
be skipped, or a new line can be prefixed with stale bytes. The game's truncation at
startup is one relevant producer of this rewrite pattern.

**Reproduction:** `F02` (`RegrownLogAsync`). Read `old\npartial` (11 bytes), rewrite the
same path to `new complete line\n` (18 bytes), then poll. Separately replace `old\n`
with the equal-length `new\n` after the first poll.

**Expected:** restart at byte zero, discard the old partial line and emit the new line.
**Observed:** `Restarted=false`; the larger replacement emits `partiale line`. The
equal-length replacement emits no lines at all. Observed smaller-file truncation passes
its control test.

**Correction:** recognise replacement/rewrite beyond the observed-length check, such
as by tracking file identity and a verified content/session marker, and clear pending
bytes when it changes. F01 is the separate process-coordination defect; this case
executes the follower directly and demonstrates its own data corruption.

## F03 — P2: An unfinished log line has no memory bound

**Location:** [GameLogFollower.cs](../../../companion/Tf3Drpc.Core/GameLog/GameLogFollower.cs#L68),
lines 68–71 and 93–103.

The per-poll 64 MiB limit applies only to new bytes. `_pending` can grow without limit,
and every subsequent poll allocates and copies the entire unfinished line again.
Malformed or unusually large log input can therefore consume increasing memory and
quadratic cumulative allocation, eventually disrupting or exhausting the companion.
This is distinct from accepting a normal short partial line.

**Reproduction:** `F03` (`UnterminatedLogAsync`). Append and poll eighteen 4 MiB blocks
without a line feed. Every individual poll is below the configured limit.

**Expected:** cap retained line data, discard an oversized entry safely and recover at
the next line boundary.
**Observed:** 75,497,472 bytes (72 MiB) remain pending, with more than 700 million bytes
allocated over the sequence. No complete lines are returned. The normal partial-line
and truncation controls pass.

**Correction:** impose a maximum line/pending length and skip to the next newline after
exceeding it. Avoid repeatedly reallocating an ever-growing pending array.

**Limit:** the test measures retention and allocation; it deliberately does not exhaust
the machine or claim that the normal captured TF3 logs contain such a line.

## F07 — P2: A rejected activity is remembered as successfully synchronised

**Locations:** [DiscordPresenceClient.cs](../../../companion/Tf3Drpc.Core/Discord/DiscordPresenceClient.cs#L168),
lines 168–182; [DiscordIpcConnection.cs](../../../companion/Tf3Drpc.Core/Discord/DiscordIpcConnection.cs#L233),
lines 233–240.

`_lastSent` is assigned after writing a command, before the peer accepts it. An ERROR
reply is only logged and cannot invalidate that cached value. Unchanged desired content
then produces no later command, so a rejected main-menu, paused or other stable presence
can remain absent/stale until its content changes or the connection ends.

**Reproduction:** `F07` (`RejectedActivityAsync`). The existing fake Discord peer sends
READY, receives the activity, and returns a SET_ACTIVITY ERROR with the exact command
nonce. After observing the error log, call UpdateAsync with the same desired content
eight times, advancing the clock by 15 seconds each time.

**Expected:** represent the failed synchronisation and permit bounded recovery for a
retryable failure rather than permanently treating the rejected command as applied.
**Observed:** the error is logged, but there are no retries or reconnects over two
simulated minutes. `IsConnected` remains true.

The [official RPC reference](https://docs.discord.com/developers/topics/rpc#error) defines
command-reply nonces and includes the nonce in ERROR responses.

**Correction:** correlate replies with command nonces, distinguish written and accepted
content, and propagate rejection to the owner. Retry recoverable errors with backoff;
handle permanent validation errors explicitly rather than looping blindly.

**Limit:** a controlled protocol ERROR tests the client's recovery. It does not assert
that this specific ERROR was emitted by live Discord for the current Career payload.

## F05 — P2: Retrying a failed settings save returns success without saving

**Locations:** [SettingsService.cs](../../../companion/Tf3Drpc.App/Services/SettingsService.cs#L36),
lines 36–41; [SettingsViewModel.cs](../../../companion/Tf3Drpc.App/ViewModels/SettingsViewModel.cs#L91),
lines 91–94, with initial warning state at line 45.

The in-memory language is changed before persistence succeeds. After a failed save,
choosing the same language returns true at the equality guard and never retries the
write. The picker ignores the failed result, and its storage warning reflects only the
initial directory probe. A user can see the chosen language and an apparently successful
retry, then lose it on restart.

**Reproduction:** `F05` (`FailedSettingsRetryAsync`). Save German settings, keep the
file open with `FileShare.Read` to block replacement, and choose French through the
actual linked settings ViewModel. Release the lock and call SetLanguage("fr") again.
The real settings service/store and generated ViewModel properties execute.

**Expected:** retain a dirty state and retry persistence after the lock is released;
communicate that the first selection could not be saved.
**Observed:** the picker shows French without the warning, while disk still holds German.
The retry returns true, with memory `fr`, disk `de`, and `CanSave=true`. Ordinary writable
changes persist in the control.

**Correction:** track unsaved settings separately from the current in-memory value,
retry dirty values even when unchanged, and expose persistence failure to the UI.

## F04 — P2: A temporary translation-read failure is cached for the whole session

**Location:** [TranslationService.cs](../../../companion/Tf3Drpc.App/Services/TranslationService.cs#L61),
lines 61–76, especially assigning `_loadedKey` at line 66 before a successful read.

The attempted language/folder is cached even if its `.mo` file cannot be read. Subsequent
monitor ticks skip that same key forever. A transient file lock, temporarily absent
file or incomplete game update therefore leaves game terminology in English after the
file recovers. Own app texts still use the chosen language, producing a mixed result.

**Reproduction:** `F04` (`TranslationFailureAsync`). Create a valid German base.mo,
hold it with `FileShare.None`, call LoadGameTexts, release the lock and call it again
with the same folder/language. Load the same file with a fresh service as a control.

**Expected:** retry after a bounded delay and use the now-readable German terms.
**Observed:** the original service remains at "Mechanic"; the fresh service reads
"Mechaniker" from exactly the same file.

**Correction:** cache successful loads separately from failed attempts and retry failed
reads with backoff, without logging the same warning every second.

## F08 — P2: Moving the clock backwards freezes Discord sends and reconnects

**Location:** [DiscordPresenceClient.cs](../../../companion/Tf3Drpc.Core/Discord/DiscordPresenceClient.cs#L125),
lines 125–130 and 171–182.

Send and reconnect intervals use UTC wall-clock subtraction. After a backwards clock
change, the elapsed value becomes negative and remains below the interval until the
clock catches up. A one-hour correction can suspend presence updates or reconnects for
about an hour, despite many monitor ticks and changed desired content.

**Reproduction:** `F08` (`DiscordClockRollbackAsync`). Send A at noon, move the controlled
wall clock back one hour, advance ten minutes and request B. Have the peer close the
connection, advance another ten minutes and request B again. Finally advance to the
original time plus 15 seconds.

**Expected:** sends and reconnects remain governed by elapsed monotonic time.
**Observed:** B is not sent after ten minutes, and there is no reconnect after twenty
minutes. Catching up to the old clock value restores the reconnect.

**Correction:** use TimeProvider's monotonic timestamps/GetElapsedTime for intervals;
retain wall-clock time only for externally meaningful dates and Discord timestamps.

## F09 — P2: Moving the clock backwards makes the current game log look stale

**Locations:** [GameLogReader.cs](../../../companion/Tf3Drpc.Core/GameLog/GameLogReader.cs#L23),
lines 23–24 and 35–38; [GameStateMachine.cs](../../../companion/Tf3Drpc.Core/State/GameStateMachine.cs#L76),
lines 76–79.

The process-start wall-clock cutoff is applied to both file write times and every log
event for the entire session. After the clock moves below that start time, current
writes and events are treated as belonging to a previous session. This independently
blocks current telemetry and menu transitions even if F08's interval handling is fixed.

**Reproduction:** `F09` (`StateClockRollbackAsync`). Establish Playing with rank 2 for a
process started at noon. Keep that same process, move the controlled clock below noon,
and apply fresh rank-3 and MainMenu events. Also write a current-session log with the
rolled-back timestamp and poll it through the actual GameLogReader.

**Expected:** continue accepting events of the already-established current session,
while still protecting against a previous session's log at startup.
**Observed:** the state stays Playing with rank 2; the MainMenu event is discarded.
The current file is classified stale and yields no events.

**Correction:** use process/session identity and confirmed session boundaries for replay
protection, and handle clock discontinuities explicitly. Do not repeatedly classify a
confirmed current session as old solely from its wall-clock timestamps.

**Limit:** no system clock was changed; the clock and file timestamps were controlled
inside the reproduction.

## F11 — P2: Tray creation failure leaves an invisible running app

**Locations:** [WindowsTrayService.cs](../../../companion/Tf3Drpc.App/Platforms/Windows/WindowsTrayService.cs#L95),
lines 95–98; [App.xaml.cs](../../../companion/Tf3Drpc.App/App.xaml.cs#L74),
lines 74–85.

The tray service catches icon-creation errors and only logs them. Startup continues,
while the settings window stays hidden. The default controls for settings and Exit
therefore disappear, leaving a background process with no visible explanation or normal
exit control. This is a failed recovery path, not a claim that the published icon is
normally missing.

**Reproduction:** `missing-tray` in `smoke.ps1`. Copy the published folder, remove only
tray.ico from the private copy, and start it with no game or other companion running.

**Expected:** show the settings/error window or terminate cleanly with a useful error.
**Observed:** after five seconds the published process remains running, its main-window
handle is zero, and its log contains TrayFailed plus the continuing monitor startup.
The exception is `ArgumentException: Failed to load icon …`. The intact-copy control
creates its tray icon; launching it a second time exits the second instance and shows
the first instance's settings window.

**Correction:** report tray-start failure to application startup and provide a visible
fallback with an Exit action, or stop gracefully. Relaunch activation can provide a
settings-window workaround, but that window currently has no Exit control of its own.

## F10 — P3: Small malformed .mo input amplifies decoding allocation

**Location:** [MoReader.cs](../../../companion/Tf3Drpc.Core/Translation/MoReader.cs#L48),
lines 48–63.

The 32 MiB file limit bounds input bytes, but not the total amount of text decoded.
Repeated table entries can reference the same large translation. The reader decodes
that text again for every entry, even when it immediately overwrites the same dictionary
key. The app performs this work synchronously in its monitor's translation-loading path,
so pathological input can consume substantial allocation and processing time.

**Reproduction:** `F10` (`AliasedMoAsync`). Build a 131,110-byte .mo with 4096 table
entries that all reference one 64 KiB translation and the same msgid; call the real reader.

**Expected:** reject invalid duplicate entries or apply a bounded decoding budget.
**Observed:** parsing succeeds with one final dictionary entry but allocates more than
537 million bytes (about 512 MiB). The exact elapsed time and allocation count are in
results-windows.json. Ordinary translation files and 20,000 deterministic malformed-file
mutations complete their controls without uncaught exceptions.

**Correction:** reject duplicate original entries and bound aggregate decoded bytes;
consider reusing shared string spans where valid catalogue entries alias data.

**Limit:** this is a malformed-input robustness/performance finding. No official game
translation file was shown to have this structure, and it is not presented as a remote
attack on the companion.

## F13 — P3: One locked old log prevents cleanup of every later old log

**Location:** [LogFileUtils.cs](../../../companion/Tf3Drpc.App/Utilities/LogFileUtils.cs#L27),
lines 27–36 and 40–45.

The exception handler surrounds the entire deletion loop. If one old file cannot be
deleted, cleanup returns before considering any later files. A persistent lock or
permission problem on that first file consequently defeats retention for unrelated,
unlocked daily logs too.

**Reproduction:** `F13` (`LockedOldLogAsync`). Create three matching logs last written
30 days ago. Obtain the first enumerated path and hold it with FileShare.Read, which
prevents Windows deletion. Invoke the unchanged cleanup; release the lock and repeat.

**Expected:** keep the locked file and delete the other two old files.
**Observed:** all three remain after the first call. The second call, after unlocking,
deletes all three.

**Correction:** catch expected I/O failures per file and continue the loop. Keep the
outer handler for enumeration failures.
