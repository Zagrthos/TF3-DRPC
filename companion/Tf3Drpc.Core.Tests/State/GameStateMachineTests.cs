using System;
using System.Collections.Generic;

using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.GameLog;
using Tf3Drpc.Core.State;
using Tf3Drpc.Core.Status;
using Tf3Drpc.Core.Structs;

using Xunit;

namespace Tf3Drpc.Core.Tests.State;

/// <summary>
/// Tests of <see cref="GameStateMachine"/>, covering log events from real samples, process tracking, pause detection, the log fallback and the game build.
/// </summary>
public sealed class GameStateMachineTests
{
    private readonly SettableTimeProvider _clock = new();
    private readonly GameStateMachine _machine;

    /// <summary>
    /// Creates a state machine on a settable clock and tells it that the game process is running.
    /// </summary>
    public GameStateMachineTests()
    {
        _machine = new GameStateMachine(_clock);
        _machine.SetProcessRunning(running: true);
    }

    private static DateTimeOffset At(int hour, int minute, int second)
        => new(2026, 10, 13, hour, minute, second, TimeSpan.Zero);

    private GameStateSnapshotStruct Feed<TEvent>(List<TimedEvent> events, int index)
        where TEvent : GameLogEvent
    {
        Assert.IsType<TEvent>(events[index].Event);
        _machine.Apply(events[index].Event, events[index].Timestamp);

        return _machine.GetSnapshot();
    }

    /// <summary>
    /// Feeding the events of the sample session log walks through starting, main menu, loading and playing, takes the timer start from the log and the status from the mod, reports a pause once the status is older than 20 seconds, and returns to the main menu with no status when the game stops.
    /// </summary>
    [Fact]
    public void SessionLogWalksThroughTheStates()
    {
        List<TimedEvent> events = SampleFiles.ReadEvents("session.log");
        _clock.Now = (At(21, 20, 44));

        GameStateSnapshotStruct snapshot = Feed<StartupEvent>(events, 0);
        Assert.Equal(GameState.Starting, snapshot.State);
        Assert.Equal(At(21, 20, 43), snapshot.TimerStart);

        snapshot = Feed<MainMenuEvent>(events, 1);
        Assert.Equal(GameState.MainMenu, snapshot.State);
        Assert.Equal(At(21, 20, 43), snapshot.TimerStart);

        snapshot = Feed<LoadingGameEvent>(events, 2);
        Assert.Equal(GameState.Loading, snapshot.State);

        snapshot = Feed<GameReadyEvent>(events, 3);
        Assert.Equal(GameState.Playing, snapshot.State);
        Assert.Equal(At(21, 15, 24), snapshot.TimerStart);
        Assert.Null(snapshot.MissionNumber);
        Assert.Null(snapshot.Status);

        _clock.Now = (At(21, 15, 26));
        snapshot = Feed<StatusEvent>(events, 4);
        Assert.Equal(GameState.Playing, snapshot.State);
        Assert.NotNull(snapshot.Status);
        Assert.Equal(1905, snapshot.Status.Year);

        _clock.Now = (At(21, 15, 44));
        Assert.NotNull(_machine.GetSnapshot().Status);
        Assert.False(_machine.GetSnapshot().IsPaused);

        _clock.Now = (At(21, 15, 46));
        Assert.NotNull(_machine.GetSnapshot().Status);
        Assert.True(_machine.GetSnapshot().IsPaused);

        _clock.Now = (At(21, 15, 36));
        snapshot = Feed<StatusEvent>(events, 5);
        Assert.NotNull(snapshot.Status);
        Assert.Equal(At(21, 15, 24), snapshot.TimerStart);

        snapshot = Feed<StatusEvent>(events, 6);
        Assert.NotNull(snapshot.Status);

        snapshot = Feed<GameStoppedEvent>(events, 7);
        Assert.Equal(GameState.MainMenu, snapshot.State);
        Assert.Null(snapshot.Status);
        Assert.Equal(At(21, 20, 43), snapshot.TimerStart);

        snapshot = Feed<MainMenuEvent>(events, 8);
        Assert.Equal(GameState.MainMenu, snapshot.State);

        snapshot = Feed<ClosingEvent>(events, 9);
        Assert.Equal(GameState.Closing, snapshot.State);
    }

    /// <summary>
    /// Feeding the events of the sample mission log ends in the mission state with mission number 1 and the timer start of the game-ready event.
    /// </summary>
    [Fact]
    public void MissionLogEndsInMissionWithNumber()
    {
        List<TimedEvent> events = SampleFiles.ReadEvents("mission.log");

        GameStateSnapshotStruct snapshot = Feed<MainMenuEvent>(events, 0);
        Assert.Equal(GameState.MainMenu, snapshot.State);
        Assert.Equal(At(21, 3, 53), snapshot.TimerStart);

        snapshot = Feed<MissionLoadingEvent>(events, 1);
        Assert.Equal(GameState.Loading, snapshot.State);

        snapshot = Feed<LoadingGameEvent>(events, 2);
        Assert.Equal(GameState.Loading, snapshot.State);

        snapshot = Feed<GameReadyEvent>(events, 3);
        Assert.Equal(GameState.Mission, snapshot.State);
        Assert.Equal(1, snapshot.MissionNumber);
        Assert.Equal(At(21, 4, 52), snapshot.TimerStart);
    }

    /// <summary>
    /// Feeding the single closing event of the sample crash log gives the closing state.
    /// </summary>
    [Fact]
    public void CrashLogEndsInClosing()
    {
        List<TimedEvent> events = SampleFiles.ReadEvents("crash.log");

        GameStateSnapshotStruct snapshot = Feed<ClosingEvent>(events, 0);

        Assert.Equal(GameState.Closing, snapshot.State);
    }

    /// <summary>
    /// A mission announced before loading is forgotten when the game stops, so loading a normal game afterwards gives the playing state without a mission number.
    /// </summary>
    [Fact]
    public void PendingMissionIsClearedByLeavingTheGame()
    {
        _machine.Apply(new MainMenuEvent(), At(10, 0, 0));
        _machine.Apply(new MissionLoadingEvent(2), At(10, 1, 0));
        _machine.Apply(new GameReadyEvent(), At(10, 2, 0));
        Assert.Equal(GameState.Mission, _machine.GetSnapshot().State);

        _machine.Apply(new GameStoppedEvent(), At(10, 3, 0));
        _machine.Apply(new MainMenuEvent(), At(10, 3, 1));
        _machine.Apply(new LoadingGameEvent("x"), At(10, 4, 0));
        _machine.Apply(new GameReadyEvent(), At(10, 5, 0));

        GameStateSnapshotStruct snapshot = _machine.GetSnapshot();
        Assert.Equal(GameState.Playing, snapshot.State);
        Assert.Null(snapshot.MissionNumber);
        Assert.Equal(At(10, 5, 0), snapshot.TimerStart);
    }

    /// <summary>
    /// When the process is reported as not running the state is not running with no timer start whatever the log says, and starting it again returns to the starting state with no timer start.
    /// </summary>
    [Fact]
    public void ProcessNotRunningOverridesTheLog()
    {
        List<TimedEvent> events = SampleFiles.ReadEvents("session.log");
        _ = Feed<StartupEvent>(events, 0);
        _ = Feed<MainMenuEvent>(events, 1);

        _machine.SetProcessRunning(running: false);
        GameStateSnapshotStruct stopped = _machine.GetSnapshot();
        _machine.Apply(events[2].Event, events[2].Timestamp);

        Assert.Equal(GameState.NotRunning, stopped.State);
        Assert.Null(stopped.TimerStart);
        Assert.Equal(GameState.NotRunning, _machine.GetSnapshot().State);

        _machine.SetProcessRunning(running: true);
        GameStateSnapshotStruct restarted = _machine.GetSnapshot();

        Assert.Equal(GameState.Starting, restarted.State);
        Assert.Null(restarted.TimerStart);
    }

    /// <summary>
    /// Resetting after startup, game-ready and status events returns to the starting state and clears the timer start, status and mission number.
    /// </summary>
    [Fact]
    public void ResetClearsEverything()
    {
        List<TimedEvent> events = SampleFiles.ReadEvents("session.log");
        _clock.Now = (At(21, 15, 26));
        _ = Feed<StartupEvent>(events, 0);
        _ = Feed<GameReadyEvent>(events, 3);
        _ = Feed<StatusEvent>(events, 4);

        _machine.Reset();
        GameStateSnapshotStruct snapshot = _machine.GetSnapshot();

        Assert.Equal(GameState.Starting, snapshot.State);
        Assert.Null(snapshot.TimerStart);
        Assert.Null(snapshot.Status);
        Assert.Null(snapshot.MissionNumber);
    }

    /// <summary>
    /// Resetting a state machine whose process was never reported as running keeps the not-running state.
    /// </summary>
    [Fact]
    public void ResetWithoutProcessStaysNotRunning()
    {
        GameStateMachine machine = new(_clock);

        machine.Reset();

        Assert.Equal(GameState.NotRunning, machine.GetSnapshot().State);
    }

    /// <summary>
    /// Two snapshots taken without any change are equal, while one taken after the status has aged into a pause is not.
    /// </summary>
    [Fact]
    public void SnapshotsAreEqualUntilSomethingChanges()
    {
        List<TimedEvent> events = SampleFiles.ReadEvents("session.log");
        _clock.Now = (At(21, 15, 26));
        _ = Feed<GameReadyEvent>(events, 3);
        GameStateSnapshotStruct withStatus = Feed<StatusEvent>(events, 4);

        GameStateSnapshotStruct same = _machine.GetSnapshot();
        _clock.Now = (At(21, 16, 30));
        GameStateSnapshotStruct stale = _machine.GetSnapshot();

        Assert.Equal(withStatus, same);
        Assert.True(withStatus == same);
        Assert.NotEqual(withStatus, stale);
        Assert.True(withStatus != stale);
        Assert.False(withStatus.IsPaused);
        Assert.True(stale.IsPaused);
    }

    private static StatusEvent CreateStatusEvent(long speedup)
        => new(new ModStatus() { Version = 1, Speedup = speedup });

    /// <summary>
    /// A status counts as fresh until 20 seconds after it arrived, including at exactly 20 seconds, and as paused one second later, while the status itself is kept.
    /// </summary>
    [Fact]
    public void StatusAtTIsNotPausedBefore20SecondsAndPausedAfterwardsWithTheStatusKept()
    {
        _machine.Apply(new GameReadyEvent(), At(10, 0, 0));
        _clock.Now = At(10, 0, 10);
        _machine.Apply(CreateStatusEvent(4), At(10, 0, 10));

        _clock.Now = At(10, 0, 29);
        GameStateSnapshotStruct early = _machine.GetSnapshot();
        _clock.Now = At(10, 0, 30);
        GameStateSnapshotStruct boundary = _machine.GetSnapshot();
        _clock.Now = At(10, 0, 31);
        GameStateSnapshotStruct late = _machine.GetSnapshot();

        Assert.NotNull(early.Status);
        Assert.False(early.IsPaused);
        Assert.False(boundary.IsPaused);
        Assert.NotNull(late.Status);
        Assert.True(late.IsPaused);
    }

    /// <summary>
    /// The pause is measured on the monotonic clock, so setting the wall clock back by an hour neither pauses early nor prevents a pause after 30 real seconds.
    /// </summary>
    [Fact]
    public void PauseIsMeasuredInRealTimeDespiteAWallClockRollback()
    {
        _clock.Now = At(10, 0, 10);
        _machine.Apply(new GameReadyEvent(), At(10, 0, 0));
        _machine.Apply(CreateStatusEvent(4), At(10, 0, 10));

        _clock.JumpWallClock(At(9, 0, 10));
        _clock.Now = _clock.Now.AddSeconds(15);
        Assert.False(_machine.GetSnapshot().IsPaused);

        _clock.Now = _clock.Now.AddSeconds(15);
        Assert.True(_machine.GetSnapshot().IsPaused);
    }

    /// <summary>
    /// A status whose timestamp is already more than 20 seconds old when it is applied counts as paused immediately.
    /// </summary>
    [Fact]
    public void StatusThatWasAlreadyOldWhenAppliedCountsAsPausedAtOnce()
    {
        _clock.Now = At(10, 0, 35);
        _machine.Apply(new GameReadyEvent(), At(10, 0, 0));
        _machine.Apply(CreateStatusEvent(4), At(10, 0, 10));

        Assert.True(_machine.GetSnapshot().IsPaused);
    }

    /// <summary>
    /// A status stamped five minutes in the future counts as fresh and is paused only after 20 real seconds, rather than being treated as negatively aged.
    /// </summary>
    [Fact]
    public void StatusStampedInTheFutureIsTreatedAsFreshNotNegativelyAged()
    {
        _clock.Now = At(10, 0, 0);
        _machine.Apply(new GameReadyEvent(), At(10, 0, 0));
        _machine.Apply(CreateStatusEvent(4), At(10, 5, 0));

        _clock.Now = At(10, 0, 19);
        Assert.False(_machine.GetSnapshot().IsPaused);

        _clock.Now = At(10, 0, 21);
        Assert.True(_machine.GetSnapshot().IsPaused);
    }

    /// <summary>
    /// When no log event arrives for two minutes after the process start, the state falls back to playing with the process start as the timer start, no status and no pause, and just before two minutes it is still the starting state.
    /// </summary>
    [Fact]
    public void NoEventForTwoMinutesFallsBackToPlayingWithTheProcessStart()
    {
        _clock.Now = At(12, 0, 0);
        _machine.SetProcessRunning(running: false);
        _machine.SetProcessRunning(running: true, At(12, 0, 0));

        _clock.Now = At(12, 1, 59);
        Assert.Equal(GameState.Starting, _machine.GetSnapshot().State);
        Assert.False(_machine.GetSnapshot().UsesLogFallback);

        _clock.Now = At(12, 2, 0);
        GameStateSnapshotStruct snapshot = _machine.GetSnapshot();
        Assert.Equal(GameState.Playing, snapshot.State);
        Assert.True(snapshot.UsesLogFallback);
        Assert.Equal(At(12, 0, 0), snapshot.TimerStart);
        Assert.Null(snapshot.Status);
        Assert.False(snapshot.IsPaused);
    }

    /// <summary>
    /// A log event received within two minutes of the process start prevents the playing fallback even ten minutes later.
    /// </summary>
    [Fact]
    public void AnEventWithinTwoMinutesPreventsTheFallback()
    {
        _machine.SetProcessRunning(running: true, At(12, 0, 0));
        _machine.Apply(new StartupEvent(), At(12, 0, 5));

        _clock.Now = _clock.Now.AddMinutes(10);

        Assert.Equal(GameState.Starting, _machine.GetSnapshot().State);
        Assert.False(_machine.GetSnapshot().UsesLogFallback);
    }

    /// <summary>
    /// After the playing fallback has been used, a recognised event such as the main menu returns to the normal state and ends the fallback.
    /// </summary>
    [Fact]
    public void FallbackThenARecognisedEventReturnsToTheNormalStates()
    {
        _machine.SetProcessRunning(running: true, At(12, 0, 0));
        _clock.Now = _clock.Now.AddMinutes(3);
        Assert.True(_machine.GetSnapshot().UsesLogFallback);

        _machine.Apply(new MainMenuEvent(), At(12, 3, 0));

        GameStateSnapshotStruct snapshot = _machine.GetSnapshot();
        Assert.Equal(GameState.MainMenu, snapshot.State);
        Assert.False(snapshot.UsesLogFallback);
    }

    /// <summary>
    /// Resetting after a log event, which happens when the log file restarts, does not rearm the fallback, so the starting state stays after ten minutes.
    /// </summary>
    [Fact]
    public void LogRestartDoesNotRearmTheFallback()
    {
        _machine.SetProcessRunning(running: true, At(12, 0, 0));
        _machine.Apply(new StartupEvent(), At(12, 0, 5));
        _machine.Reset();

        _clock.Now = _clock.Now.AddMinutes(10);

        Assert.Equal(GameState.Starting, _machine.GetSnapshot().State);
        Assert.False(_machine.GetSnapshot().UsesLogFallback);
    }

    /// <summary>
    /// Reporting a new process start time or a stopped process rearms the playing fallback even after an earlier event had disarmed it.
    /// </summary>
    [Fact]
    public void ProcessStopOrNewProcessRearmsTheFallback()
    {
        _machine.SetProcessRunning(running: true, At(12, 0, 0));
        _machine.Apply(new StartupEvent(), At(12, 0, 5));

        _machine.SetProcessRunning(running: true, At(13, 0, 0));
        _clock.Now = _clock.Now.AddMinutes(3);
        Assert.True(_machine.GetSnapshot().UsesLogFallback);

        _machine.Apply(new StartupEvent(), At(13, 0, 5));
        _machine.SetProcessRunning(running: false);
        _machine.SetProcessRunning(running: true, At(14, 0, 0));
        Assert.False(_machine.GetSnapshot().UsesLogFallback);
        _clock.Now = _clock.Now.AddMinutes(3);
        Assert.True(_machine.GetSnapshot().UsesLogFallback);
    }

    /// <summary>
    /// Log lines stamped before the process start do not count as seen, so the fallback still applies three minutes after the start.
    /// </summary>
    [Fact]
    public void PreviousSessionLinesDoNotCountAsSeenForTheFallback()
    {
        _machine.SetProcessRunning(running: true, At(12, 0, 0));
        _machine.Apply(new MainMenuEvent(), At(11, 0, 0));
        _clock.Now = _clock.Now.AddMinutes(3);

        Assert.True(_machine.GetSnapshot().UsesLogFallback);
    }

    /// <summary>
    /// A new status clears the pause caused by an earlier status that had grown too old.
    /// </summary>
    [Fact]
    public void ANewStatusAfterAPauseClearsIt()
    {
        _machine.Apply(new GameReadyEvent(), At(10, 0, 0));
        _machine.Apply(CreateStatusEvent(4), At(10, 0, 10));
        _clock.Now = At(10, 0, 50);
        Assert.True(_machine.GetSnapshot().IsPaused);

        _machine.Apply(CreateStatusEvent(4), At(10, 0, 50));

        Assert.False(_machine.GetSnapshot().IsPaused);
    }

    /// <summary>
    /// A status reporting a game speed-up of zero counts as paused at once, without waiting for 20 seconds.
    /// </summary>
    [Fact]
    public void StatusWithSpeedupZeroIsPausedAtOnce()
    {
        _machine.Apply(new GameReadyEvent(), At(10, 0, 0));
        _clock.Now = At(10, 0, 11);

        _machine.Apply(CreateStatusEvent(0), At(10, 0, 10));

        Assert.True(_machine.GetSnapshot().IsPaused);
    }

    /// <summary>
    /// A status from the map editor is never paused, even with a speed-up of zero and a minute later.
    /// </summary>
    [Fact]
    public void MapEditorIsNeverPaused()
    {
        _machine.Apply(new GameReadyEvent(), At(10, 0, 0));
        _machine.Apply(new StatusEvent(new ModStatus() { Version = 1, Speedup = 0, IsMapEditor = true }), At(10, 0, 10));
        _clock.Now = At(10, 0, 11);

        Assert.False(_machine.GetSnapshot().IsPaused);

        _clock.Now = At(10, 1, 0);

        Assert.False(_machine.GetSnapshot().IsPaused);
    }

    /// <summary>
    /// Without a status, no pause is reported even five minutes after the game became ready, and the state is playing.
    /// </summary>
    [Fact]
    public void WithoutAStatusNothingIsPaused()
    {
        _machine.Apply(new GameReadyEvent(), At(10, 0, 0));
        _clock.Now = At(10, 5, 0);

        GameStateSnapshotStruct snapshot = _machine.GetSnapshot();

        Assert.Equal(GameState.Playing, snapshot.State);
        Assert.Null(snapshot.Status);
        Assert.False(snapshot.IsPaused);
    }

    /// <summary>
    /// Stopping the game, returning to the main menu or applying another game-ready event each clear the status and the pause; the rows vary the event.
    /// </summary>
    [Theory]
    [InlineData(nameof(GameStoppedEvent))]
    [InlineData(nameof(MainMenuEvent))]
    [InlineData(nameof(GameReadyEvent))]
    public void LeavingOrRestartingTheGameClearsTheStatus(string eventName)
    {
        GameLogEvent gameEvent = eventName switch
        {
            nameof(GameStoppedEvent) => new GameStoppedEvent(),
            nameof(MainMenuEvent) => new MainMenuEvent(),
            _ => new GameReadyEvent()
        };

        _machine.Apply(new GameReadyEvent(), At(10, 0, 0));
        _machine.Apply(CreateStatusEvent(4), At(10, 0, 10));
        _clock.Now = At(10, 5, 0);

        _machine.Apply(gameEvent, At(10, 4, 59));
        if (gameEvent is GameStoppedEvent or MainMenuEvent)
            _machine.Apply(new GameReadyEvent(), At(10, 4, 59));

        GameStateSnapshotStruct snapshot = _machine.GetSnapshot();

        Assert.Null(snapshot.Status);
        Assert.False(snapshot.IsPaused);
    }

    /// <summary>
    /// Events stamped more than two seconds before the process start are ignored, while events two seconds before the start or later are accepted.
    /// </summary>
    [Fact]
    public void EventsBeforeTheProcessStartAreIgnoredWithATwoSecondTolerance()
    {
        DateTimeOffset started = At(12, 0, 10);
        _machine.SetProcessRunning(running: true, started);

        _machine.Apply(new MainMenuEvent(), At(12, 0, 7));
        Assert.Equal(GameState.Starting, _machine.GetSnapshot().State);
        Assert.Null(_machine.GetSnapshot().TimerStart);

        _machine.Apply(new MainMenuEvent(), At(12, 0, 8));
        Assert.Equal(GameState.MainMenu, _machine.GetSnapshot().State);

        _machine.Apply(new ClosingEvent(IsCrash: false), At(12, 0, 9));
        Assert.Equal(GameState.Closing, _machine.GetSnapshot().State);
    }

    /// <summary>
    /// When the process start time is unknown, events are not filtered by time.
    /// </summary>
    [Fact]
    public void WithoutAStartTimeNothingIsFiltered()
    {
        _machine.SetProcessRunning(running: true);

        _machine.Apply(new MainMenuEvent(), At(1, 0, 0));

        Assert.Equal(GameState.MainMenu, _machine.GetSnapshot().State);
    }

    /// <summary>
    /// A running process reported with a different start time is a new process and returns to the starting state, while repeating the same start time keeps the state.
    /// </summary>
    [Fact]
    public void ADifferentStartTimeOfARunningProcessCountsAsANewProcess()
    {
        _machine.SetProcessRunning(running: true, At(12, 0, 0));
        _machine.Apply(new MainMenuEvent(), At(12, 0, 5));
        Assert.Equal(GameState.MainMenu, _machine.GetSnapshot().State);

        _machine.SetProcessRunning(running: true, At(12, 0, 5));
        Assert.Equal(GameState.Starting, _machine.GetSnapshot().State);

        _machine.SetProcessRunning(running: true, At(12, 0, 5));
        _machine.Apply(new MainMenuEvent(), At(12, 0, 6));
        _machine.SetProcessRunning(running: true, At(12, 0, 5));
        Assert.Equal(GameState.MainMenu, _machine.GetSnapshot().State);
    }

    /// <summary>
    /// A start time that becomes known later, or goes unknown again, keeps the state, and an event stamped before the known start time is still ignored.
    /// </summary>
    [Fact]
    public void AStartTimeReadLaterOrNotAtAllKeepsTheState()
    {
        _machine.SetProcessRunning(running: true);
        _machine.Apply(new MainMenuEvent(), At(12, 0, 5));

        _machine.SetProcessRunning(running: true, At(12, 0, 0));
        Assert.Equal(GameState.MainMenu, _machine.GetSnapshot().State);

        _machine.SetProcessRunning(running: true);
        Assert.Equal(GameState.MainMenu, _machine.GetSnapshot().State);

        _machine.Apply(new ClosingEvent(IsCrash: false), At(11, 59, 0));
        Assert.Equal(GameState.MainMenu, _machine.GetSnapshot().State);
    }

    /// <summary>
    /// Resetting keeps the start time filter, so an event stamped before the process start is still ignored afterwards.
    /// </summary>
    [Fact]
    public void ResetKeepsTheStartTimeFilter()
    {
        _machine.SetProcessRunning(running: true, At(12, 0, 10));
        _machine.Reset();

        _machine.Apply(new MainMenuEvent(), At(12, 0, 0));

        Assert.Equal(GameState.Starting, _machine.GetSnapshot().State);
    }

    /// <summary>
    /// Once an event of the current session has been accepted, later events with earlier timestamps, such as after a clock set back, are still applied.
    /// </summary>
    [Fact]
    public void ConfirmedSessionKeepsAcceptingEventsWhenTheClockIsSetBack()
    {
        _machine.SetProcessRunning(running: true, At(12, 0, 0));
        _machine.Apply(new GameReadyEvent(), At(12, 1, 0));
        _machine.Apply(new StatusEvent(new ModStatus() { Version = 1, Speedup = 1, Rank = 2 }), At(12, 1, 10));
        Assert.Equal(2, _machine.GetSnapshot().Status?.Rank);

        _machine.Apply(new StatusEvent(new ModStatus() { Version = 1, Speedup = 1, Rank = 3 }), At(11, 0, 0));
        Assert.Equal(3, _machine.GetSnapshot().Status?.Rank);

        _machine.Apply(new MainMenuEvent(), At(11, 0, 20));
        Assert.Equal(GameState.MainMenu, _machine.GetSnapshot().State);
    }

    /// <summary>
    /// A confirmed session survives a reset, which happens when the log restarts, but a new start time or a stop and restart makes older timestamps be ignored again.
    /// </summary>
    [Fact]
    public void ConfirmedSessionSurvivesAResetButNotANewProcess()
    {
        _machine.SetProcessRunning(running: true, At(12, 0, 0));
        _machine.Apply(new MainMenuEvent(), At(12, 1, 0));
        _machine.Reset();

        _machine.Apply(new MainMenuEvent(), At(11, 0, 0));
        Assert.Equal(GameState.MainMenu, _machine.GetSnapshot().State);

        _machine.SetProcessRunning(running: true, At(13, 0, 0));
        _machine.Apply(new MainMenuEvent(), At(11, 0, 0));
        Assert.Equal(GameState.Starting, _machine.GetSnapshot().State);

        _machine.SetProcessRunning(running: false);
        _machine.SetProcessRunning(running: true, At(14, 0, 0));
        _machine.Apply(new MainMenuEvent(), At(11, 0, 0));
        Assert.Equal(GameState.Starting, _machine.GetSnapshot().State);
    }

    /// <summary>
    /// Log lines stamped before the process start are ignored until an event of the current session arrives, after which later events are accepted.
    /// </summary>
    [Fact]
    public void PreviousSessionLinesBeforeTheFirstConfirmationAreStillIgnored()
    {
        _machine.SetProcessRunning(running: true, At(12, 0, 0));
        _machine.Apply(new GameReadyEvent(), At(11, 0, 0));
        _machine.Apply(new MainMenuEvent(), At(11, 30, 0));
        Assert.Equal(GameState.Starting, _machine.GetSnapshot().State);

        _machine.Apply(new MainMenuEvent(), At(12, 0, 1));
        Assert.Equal(GameState.MainMenu, _machine.GetSnapshot().State);
    }

    /// <summary>
    /// Stopping the process forgets its start time, so after a restart without a known start time events are no longer filtered.
    /// </summary>
    [Fact]
    public void ProcessStopForgetsTheStartTime()
    {
        _machine.SetProcessRunning(running: true, At(12, 0, 10));
        _machine.SetProcessRunning(running: false);
        _machine.SetProcessRunning(running: true);

        _machine.Apply(new MainMenuEvent(), At(1, 0, 0));

        Assert.Equal(GameState.MainMenu, _machine.GetSnapshot().State);
    }

    /// <summary>
    /// Replaying the sample session log from an earlier session leaves the state at starting, and the events of the current session that follow then drive the state and the timer start.
    /// </summary>
    [Fact]
    public void ReplayOfThePreviousSessionDoesNotDriveTheState()
    {
        List<TimedEvent> old = SampleFiles.ReadEvents("session.log");
        _machine.SetProcessRunning(running: true, At(23, 0, 0));

        foreach (TimedEvent timed in old)
        {
            _machine.Apply(timed.Event, timed.Timestamp);
            Assert.Equal(GameState.Starting, _machine.GetSnapshot().State);
        }

        _machine.Apply(new StartupEvent(), At(23, 0, 3));
        _machine.Apply(new MainMenuEvent(), At(23, 0, 9));

        GameStateSnapshotStruct snapshot = _machine.GetSnapshot();
        Assert.Equal(GameState.MainMenu, snapshot.State);
        Assert.Equal(At(23, 0, 3), snapshot.TimerStart);
    }

    private static GameBuildEvent BuildEvent(int build)
        => new(build);

    /// <summary>
    /// The game build from a build event is kept through the main menu, loading, ready and stopped events.
    /// </summary>
    [Fact]
    public void GameBuildIsSetAfterStartupAndKeptThroughTheGameStates()
    {
        _machine.Apply(new StartupEvent(), At(10, 0, 0));
        _machine.Apply(BuildEvent(40420), At(10, 0, 0));

        Assert.Equal(40420, _machine.GetSnapshot().GameBuild);

        foreach (GameLogEvent gameEvent in (GameLogEvent[])[new MainMenuEvent(), new LoadingGameEvent("x"), new GameReadyEvent(), new GameStoppedEvent()])
        {
            _machine.Apply(gameEvent, At(10, 1, 0));
            Assert.Equal(40420, _machine.GetSnapshot().GameBuild);
        }
    }

    /// <summary>
    /// Resetting clears the game build until another build event arrives, and stopping the process clears it again so that a restart starts without one.
    /// </summary>
    [Fact]
    public void GameBuildIsClearedByResetAndGoneAfterTheProcessStops()
    {
        _machine.SetProcessRunning(running: true, At(10, 0, 0));
        _machine.Apply(BuildEvent(40420), At(10, 0, 1));

        Assert.Equal(40420, _machine.GetSnapshot().GameBuild);

        _machine.Reset();

        Assert.Null(_machine.GetSnapshot().GameBuild);

        _machine.Apply(BuildEvent(40420), At(10, 0, 2));

        Assert.Equal(40420, _machine.GetSnapshot().GameBuild);

        _machine.SetProcessRunning(running: false);

        Assert.Null(_machine.GetSnapshot().GameBuild);

        _machine.SetProcessRunning(running: true, At(11, 0, 0));

        Assert.Null(_machine.GetSnapshot().GameBuild);
    }

    /// <summary>
    /// Reporting a new process start time clears the game build.
    /// </summary>
    [Fact]
    public void GameBuildIsClearedByANewProcess()
    {
        _machine.SetProcessRunning(running: true, At(12, 0, 0));
        _machine.Apply(BuildEvent(40420), At(12, 0, 1));
        _machine.SetProcessRunning(running: true, At(13, 0, 0));

        Assert.Null(_machine.GetSnapshot().GameBuild);
    }

    /// <summary>
    /// A build event stamped before the process start is ignored, while one of the current session is applied.
    /// </summary>
    [Fact]
    public void GameBuildOfThePreviousSessionIsIgnored()
    {
        _machine.SetProcessRunning(running: true, At(23, 0, 0));
        _machine.Apply(BuildEvent(40420), At(22, 0, 0));

        Assert.Null(_machine.GetSnapshot().GameBuild);

        _machine.Apply(BuildEvent(40421), At(23, 0, 3));

        Assert.Equal(40421, _machine.GetSnapshot().GameBuild);
    }

    /// <summary>
    /// Two snapshots that differ only in the game build are not equal, with the same state and timer start.
    /// </summary>
    [Fact]
    public void SnapshotsDifferWhenOnlyTheGameBuildDiffers()
    {
        _machine.Apply(new MainMenuEvent(), At(10, 0, 0));
        GameStateSnapshotStruct before = _machine.GetSnapshot();
        _machine.Apply(BuildEvent(40420), At(10, 0, 1));
        GameStateSnapshotStruct after = _machine.GetSnapshot();

        Assert.NotEqual(before, after);
        Assert.True(before != after);
        Assert.Equal(before.State, after.State);
        Assert.Equal(before.TimerStart, after.TimerStart);
    }
}
