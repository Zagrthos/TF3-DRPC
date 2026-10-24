using System;

using Tf3Drpc.Core.Constants;
using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Status;
using Tf3Drpc.Core.Structs;

namespace Tf3Drpc.Core.State;

/// <summary>
/// Derives the game state from the process check and the classified game log events.
/// </summary>
/// <remarks>Call <see cref="SetProcessRunning"/> before applying the events of a poll: a change of the process state clears the log state.</remarks>
internal sealed class GameStateMachine(TimeProvider timeProvider)
{
    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly GameProcessTracker _process = new();
    private bool _sessionConfirmed;
    private bool _eventSeen;
    private long _processSeenAt;
    private GameState _state = GameState.Starting;
    private int? _missionNumber;
    private bool _missionPending;
    private int? _pendingMissionNumber;
    private DateTimeOffset? _gameStart;
    private DateTimeOffset? _timerStart;
    private int? _gameBuild;
    private ModStatus? _status;
    private TimeSpan _statusInitialAge;
    private long _statusAppliedAt;

    #region Process state

    /// <summary>
    /// Tells the machine whether the game process runs and when it started; log lines older than the start are ignored.
    /// </summary>
    /// <remarks>
    /// A different start time of a running process counts as a new process. Without a start time nothing is filtered. The start
    /// time only guards against the previous session: once an event of the current one passed it, later events are accepted
    /// whatever the clock says.
    /// </remarks>
    public void SetProcessRunning(bool running, DateTimeOffset? startedAt = null)
    {
        bool stopped = _process.IsRunning && !running;

        if (!_process.Update(running, startedAt) && !stopped)
            return;

        _sessionConfirmed = false;
        _eventSeen = false;
        _processSeenAt = _timeProvider.GetTimestamp();
        Reset();
    }

    /// <summary>
    /// Forgets everything read from the log, for example after the log started over.
    /// </summary>
    public void Reset()
    {
        _state = GameState.Starting;
        _gameStart = null;
        _timerStart = null;
        _gameBuild = null;
        ClearGame();
    }

    #endregion Process state

    #region Log events

    /// <summary>
    /// Applies an event of the log; <paramref name="entryTimestamp"/> is the log time of its line.
    /// </summary>
    public void Apply(in GameLogEventStruct gameEvent, in DateTimeOffset entryTimestamp)
    {
        if (_process.StartedAt is DateTimeOffset started && !_sessionConfirmed)
        {
            if (entryTimestamp < (started - GameStateConstants.ProcessStartTolerance))
                return;

            _sessionConfirmed = true;
        }

        _eventSeen = true;

        switch (gameEvent.Kind)
        {
            case GameLogEventKind.Startup:
                _state = GameState.Starting;
                _gameStart = entryTimestamp;
                _timerStart = entryTimestamp;
                ClearGame();
                break;

            case GameLogEventKind.MainMenu:
            case GameLogEventKind.GameStopped:
                _gameStart ??= entryTimestamp;
                _state = GameState.MainMenu;
                _timerStart = _gameStart;
                ClearGame();
                break;

            case GameLogEventKind.LoadingGame:
                _state = GameState.Loading;
                _missionNumber = null;
                _status = null;
                break;

            case GameLogEventKind.MissionLoading:
                _state = GameState.Loading;
                _missionNumber = null;
                _status = null;
                _missionPending = true;
                _pendingMissionNumber = gameEvent.MissionNumber;
                break;

            case GameLogEventKind.GameReady:
                _state = (_missionPending) ? GameState.Mission : GameState.Playing;
                _missionNumber = _pendingMissionNumber;
                _missionPending = false;
                _pendingMissionNumber = null;
                _timerStart = entryTimestamp;
                _status = null;
                break;

            case GameLogEventKind.Status:
                _status = gameEvent.Status;
                _statusInitialAge = Max(_timeProvider.GetUtcNow() - entryTimestamp, TimeSpan.Zero);
                _statusAppliedAt = _timeProvider.GetTimestamp();
                break;

            case GameLogEventKind.Closing:
                _state = GameState.Closing;
                break;

            case GameLogEventKind.GameBuild:
                _gameBuild = gameEvent.GameBuild;
                break;
        }
    }

    #endregion Log events

    #region Snapshot

    /// <summary>
    /// Returns the current state. In Playing the latest mod status is included once one arrived; the game counts as paused
    /// when that status says so or is older than <see cref="GameStateConstants.PauseAfter"/>, because the mod writes nothing while the game menu is open; the map editor never counts as paused.
    /// A process that gave no usable log event for <see cref="GameStateConstants.LogFallbackAfter"/> counts as Playing.
    /// </summary>
    public GameStateSnapshotStruct GetSnapshot()
    {
        if (!_process.IsRunning)
            return new GameStateSnapshotStruct() { State = GameState.NotRunning };

        if (!_eventSeen && (_timeProvider.GetElapsedTime(_processSeenAt) >= GameStateConstants.LogFallbackAfter))
        {
            return new GameStateSnapshotStruct() { State = GameState.Playing, TimerStart = _process.StartedAt, UsesLogFallback = true };
        }

        ModStatus? status = (_state is GameState.Playing) ? _status : null;

        // Time doesn't run in the map editor, which always reports speedup 0.
        bool paused = (status is { IsMapEditor: not true }) &&
            ((status.Speedup is 0) || ((_statusInitialAge + _timeProvider.GetElapsedTime(_statusAppliedAt)) > GameStateConstants.PauseAfter));

        return new GameStateSnapshotStruct() { State = _state, MissionNumber = _missionNumber, TimerStart = _timerStart, GameBuild = _gameBuild, Status = status, IsPaused = paused };
    }

    private static TimeSpan Max(in TimeSpan first, in TimeSpan second)
        => (first > second) ? first : second;

    private void ClearGame()
    {
        _missionNumber = null;
        _missionPending = false;
        _pendingMissionNumber = null;
        _status = null;
    }

    #endregion Snapshot
}
