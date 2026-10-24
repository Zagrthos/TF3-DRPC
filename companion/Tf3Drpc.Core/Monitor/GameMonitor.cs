using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Tf3Drpc.Core.Constants;
using Tf3Drpc.Core.GameLog;
using Tf3Drpc.Core.Languages;
using Tf3Drpc.Core.Logging;
using Tf3Drpc.Core.State;
using Tf3Drpc.Core.Structs;

namespace Tf3Drpc.Core.Monitor;

/// <summary>
/// Runs one tick of the monitor: process check, log, language, presence, Discord; call it from one loop only.
/// </summary>
public sealed class GameMonitor
{
    private readonly IGameProcessSource _processSource;
    private readonly IGamePathResolver _pathResolver;
    private readonly ISettingsSource _settingsSource;
    private readonly ILanguageResolver _languageResolver;
    private readonly IPresenceBuilder _presenceBuilder;
    private readonly IPresenceSink _presenceSink;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<GameMonitor> _logger;
    private readonly GameProcessTracker _processTracker = new();
    private readonly GameLogReader _logReader;
    private readonly GameStateMachine _stateMachine;
    private GamePathsStruct _paths;
    private long? _pathAttemptTimestamp;
    private long? _profileReadTimestamp;
    private bool _fallbackLogged;
    private bool _pathsNotFoundLogged;
    private string? _gameLanguageCode;
    private LanguageInfo _language = LanguageCatalog.Resolve(LanguageConstants.DefaultCode, null);
    private StatusHolder _statusHolder = new(new AppStatusStruct() { Language = LanguageCatalog.Resolve(LanguageConstants.DefaultCode, null) });

#pragma warning disable IDE0290 // Use primary constructor: it cannot take its parameter list on separate lines
    /// <summary>
    /// Creates a monitor that reads from the given sources and writes to the presence sink.
    /// </summary>
    public GameMonitor
    (
        IGameProcessSource processSource,
        IGamePathResolver pathResolver,
        ISettingsSource settingsSource,
        ILanguageResolver languageResolver,
        IPresenceBuilder presenceBuilder,
        IPresenceSink presenceSink,
        TimeProvider timeProvider,
        ILogger<GameMonitor> logger
    )
    {
        _processSource = processSource;
        _pathResolver = pathResolver;
        _settingsSource = settingsSource;
        _languageResolver = languageResolver;
        _presenceBuilder = presenceBuilder;
        _presenceSink = presenceSink;
        _timeProvider = timeProvider;
        _logger = logger;
        _logReader = new GameLogReader(timeProvider);
        _stateMachine = new GameStateMachine(timeProvider);
    }
#pragma warning restore IDE0290

    #region Public API

    /// <summary>
    /// The status as of the last tick; safe to read from another thread.
    /// </summary>
    public AppStatusStruct Status
        => Volatile.Read(ref _statusHolder).Value;

    /// <summary>
    /// Runs one tick; returns the new status if it changed, otherwise <see langword="null"/>. A failing tick is logged and returns <see langword="null"/>; only cancellation is thrown.
    /// </summary>
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "A failing tick must never end the loop.")]
    public async Task<AppStatusStruct?> TickAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await RunTickAsync(cancellationToken);
        }
        catch (Exception exception) when (!((exception is OperationCanceledException) && cancellationToken.IsCancellationRequested))
        {
            _logger.MonitorTickFailed(exception);

            return null;
        }
    }

    #endregion Public API

    #region Tick

    private async Task<AppStatusStruct?> RunTickAsync(CancellationToken cancellationToken)
    {
        GameProcessInfoStruct process = _processSource.Check();
        _stateMachine.SetProcessRunning(process.IsRunning, process.StartTime);
        HandleProcessChange(process);
        EnsurePaths(process);

        if (process.IsRunning)
            await PollLogAsync(process.StartTime, cancellationToken);

        UpdateLanguage(process.IsRunning);

        GameStateSnapshotStruct snapshot = _stateMachine.GetSnapshot();

        if (snapshot.UsesLogFallback && !_fallbackLogged)
        {
            _fallbackLogged = true;
            _logger.GameLogUnusable(_paths.LogPath);
        }

        PresenceContentStruct? desired = (_presenceBuilder.TryBuild(snapshot, _language, out PresenceContentStruct content)) ? content : null;
        await _presenceSink.UpdateAsync(desired, cancellationToken);

        return Publish(snapshot, desired);
    }

    private void HandleProcessChange(in GameProcessInfoStruct process)
    {
        bool wasRunning = _processTracker.IsRunning;

        if (_processTracker.Update(process.IsRunning, process.StartTime))
        {
            _logger.GameStarted(process.ExecutablePath);
            _paths = _pathResolver.ResolveGamePaths(process.ExecutablePath);
            _pathAttemptTimestamp = _timeProvider.GetTimestamp();
            _profileReadTimestamp = null;
            _fallbackLogged = false;
            _logReader.Restart();
            LogPaths(isChange: true);
        }
        else if (!process.IsRunning && wasRunning)
        {
            _logger.GameStopped();
        }
    }

    private bool IsDue(long? timestamp, in TimeSpan interval)
        => (timestamp is not long start) || (_timeProvider.GetElapsedTime(start) >= interval);

    private void EnsurePaths(in GameProcessInfoStruct process)
    {
        if (!IsDue(_pathAttemptTimestamp, MonitorConstants.PathRetryInterval))
            return;

        // A log older than the process is another account's; look again until the right one shows up.
        bool wrongLog = process.IsRunning && (_paths.LogPath is not null) && _logReader.IsPreviousSession(_paths.LogPath, process.StartTime);

        if (_paths.IsComplete && !wrongLog)
            return;

        GamePathsStruct resolved = _pathResolver.ResolveGamePaths(process.ExecutablePath);
        _pathAttemptTimestamp = _timeProvider.GetTimestamp();

        bool changed = resolved != _paths;
        _paths = resolved;
        LogPaths(changed);
    }

    // The not-found warning comes once per lost log path, not on every retry; a found log path re-arms it.
    private void LogPaths(bool isChange)
    {
        if (_paths.LogPath is null)
        {
            if (_pathsNotFoundLogged)
                return;

            _pathsNotFoundLogged = true;
            _logger.GamePathsNotFound();

            return;
        }

        _pathsNotFoundLogged = false;

        if (isChange)
            _logger.GamePathsResolved(_paths.LogPath, _paths.ProfilePath, _paths.GameDirectory);
    }

    private async Task PollLogAsync(DateTimeOffset? startedAt, CancellationToken cancellationToken)
    {
        if (_paths.LogPath is null)
            return;

        GameLogPollResultStruct result = await _logReader.PollAsync(_paths.LogPath, startedAt, cancellationToken);

        if (result.Restarted)
            _stateMachine.Reset();

        for (int i = 0; i < result.Events.Count; i++)
        {
            TimedGameLogEventStruct timed = result.Events[i];
            _stateMachine.Apply(timed.GameEvent, timed.Timestamp);
        }

        if (!result.Restarted && (result.Events.Count is 0))
            return;

        _logger.GameLogPolled(result.Events.Count, result.Restarted);
    }

    #endregion Tick

    #region Language and status

    private void UpdateLanguage(bool running)
    {
        bool profileDue = IsDue(_profileReadTimestamp, MonitorConstants.ProfileReadInterval) && (running || (_gameLanguageCode is null));

        if (profileDue && (_paths.ProfilePath is not null))
        {
            _profileReadTimestamp = _timeProvider.GetTimestamp();
            _gameLanguageCode = _languageResolver.ReadGameLanguageCode(_paths.ProfilePath) ?? _gameLanguageCode;
        }

        LanguageInfo resolved = _languageResolver.Resolve(_settingsSource.Current.Language, _gameLanguageCode);
        LanguageInfo gameLanguage = _languageResolver.Resolve(LanguageConstants.GameSetting, _gameLanguageCode);

        if (!Equals(resolved, _language))
        {
            _language = resolved;
            _logger.LanguageChanged(resolved.Code);
        }

        _languageResolver.LoadGameTexts(_paths.GameDirectory, _language, gameLanguage);
    }

    private AppStatusStruct? Publish(in GameStateSnapshotStruct snapshot, PresenceContentStruct? desired)
    {
        AppStatusStruct previous = Status;
        AppStatusStruct status = new()
        {
            Game = snapshot,
            DiscordConnected = _presenceSink.IsConnected,
            Language = _language,
            PresenceDetails = desired?.Details,
            PresenceState = desired?.State
        };

        if (status == previous)
            return null;

        if ((status.Game.State != previous.Game.State) || (status.Game.MissionNumber != previous.Game.MissionNumber))
            _logger.GameStateChanged(status.Game.State, status.Game.MissionNumber);

        if ((status.Game.GameBuild is int build) && (status.Game.GameBuild != previous.Game.GameBuild))
            _logger.GameBuildDetected(build);

        if (status.DiscordConnected != previous.DiscordConnected)
            _logger.DiscordConnectionChanged(status.DiscordConnected);

        Volatile.Write(ref _statusHolder, new StatusHolder(status));

        return status;
    }

    #endregion Language and status

    /// <summary>
    /// Publishes the status struct as one reference, so a reader on another thread never sees a torn value.
    /// </summary>
    private sealed class StatusHolder(AppStatusStruct value)
    {
        public AppStatusStruct Value { get; } = value;
    }
}
