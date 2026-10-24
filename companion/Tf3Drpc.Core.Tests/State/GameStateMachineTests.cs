using System;
using System.Collections.Generic;

using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.State;
using Tf3Drpc.Core.Status;
using Tf3Drpc.Core.Structs;

using Xunit;

namespace Tf3Drpc.Core.Tests.State;

public sealed class GameStateMachineTests
{
    private readonly SettableTimeProvider _clock = new();
    private readonly GameStateMachine _machine;

    public GameStateMachineTests()
    {
        _machine = new GameStateMachine(_clock);
        _machine.SetProcessRunning(true);
    }

    private static DateTimeOffset At(int hour, int minute, int second)
        => new(2026, 10, 13, hour, minute, second, TimeSpan.Zero);

    private GameStateSnapshotStruct Feed(List<TimedEvent> events, int index, GameLogEventKind expected)
    {
        Assert.Equal(expected, events[index].Event.Kind);
        _machine.Apply(events[index].Event, events[index].Timestamp);

        return _machine.GetSnapshot();
    }

    [Fact]
    public void SessionLog_WalksThroughTheStates()
    {
        List<TimedEvent> events = SampleFiles.ReadEvents("session.log");
        _clock.Now = (At(21, 20, 44));

        GameStateSnapshotStruct snapshot = Feed(events, 0, GameLogEventKind.Startup);
        Assert.Equal(GameState.Starting, snapshot.State);
        Assert.Equal(At(21, 20, 43), snapshot.TimerStart);

        snapshot = Feed(events, 1, GameLogEventKind.MainMenu);
        Assert.Equal(GameState.MainMenu, snapshot.State);
        Assert.Equal(At(21, 20, 43), snapshot.TimerStart);

        snapshot = Feed(events, 2, GameLogEventKind.LoadingGame);
        Assert.Equal(GameState.Loading, snapshot.State);

        snapshot = Feed(events, 3, GameLogEventKind.GameReady);
        Assert.Equal(GameState.Playing, snapshot.State);
        Assert.Equal(At(21, 15, 24), snapshot.TimerStart);
        Assert.Null(snapshot.MissionNumber);
        Assert.Null(snapshot.Status);

        _clock.Now = (At(21, 15, 26));
        snapshot = Feed(events, 4, GameLogEventKind.Status);
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
        snapshot = Feed(events, 5, GameLogEventKind.Status);
        Assert.NotNull(snapshot.Status);
        Assert.Equal(At(21, 15, 24), snapshot.TimerStart);

        snapshot = Feed(events, 6, GameLogEventKind.Status);
        Assert.NotNull(snapshot.Status);

        snapshot = Feed(events, 7, GameLogEventKind.GameStopped);
        Assert.Equal(GameState.MainMenu, snapshot.State);
        Assert.Null(snapshot.Status);
        Assert.Equal(At(21, 20, 43), snapshot.TimerStart);

        snapshot = Feed(events, 8, GameLogEventKind.MainMenu);
        Assert.Equal(GameState.MainMenu, snapshot.State);

        snapshot = Feed(events, 9, GameLogEventKind.Closing);
        Assert.Equal(GameState.Closing, snapshot.State);
    }

    [Fact]
    public void MissionLog_EndsInMissionWithNumber()
    {
        List<TimedEvent> events = SampleFiles.ReadEvents("mission.log");

        GameStateSnapshotStruct snapshot = Feed(events, 0, GameLogEventKind.MainMenu);
        Assert.Equal(GameState.MainMenu, snapshot.State);
        Assert.Equal(At(21, 3, 53), snapshot.TimerStart);

        snapshot = Feed(events, 1, GameLogEventKind.MissionLoading);
        Assert.Equal(GameState.Loading, snapshot.State);

        snapshot = Feed(events, 2, GameLogEventKind.LoadingGame);
        Assert.Equal(GameState.Loading, snapshot.State);

        snapshot = Feed(events, 3, GameLogEventKind.GameReady);
        Assert.Equal(GameState.Mission, snapshot.State);
        Assert.Equal(1, snapshot.MissionNumber);
        Assert.Equal(At(21, 4, 52), snapshot.TimerStart);
    }

    [Fact]
    public void CrashLog_EndsInClosing()
    {
        List<TimedEvent> events = SampleFiles.ReadEvents("crash.log");

        GameStateSnapshotStruct snapshot = Feed(events, 0, GameLogEventKind.Closing);

        Assert.Equal(GameState.Closing, snapshot.State);
    }

    [Fact]
    public void PendingMission_IsClearedByLeavingTheGame()
    {
        _machine.Apply(new GameLogEventStruct() { Kind = GameLogEventKind.MainMenu }, At(10, 0, 0));
        _machine.Apply(new GameLogEventStruct() { Kind = GameLogEventKind.MissionLoading, MissionNumber = 2 }, At(10, 1, 0));
        _machine.Apply(new GameLogEventStruct() { Kind = GameLogEventKind.GameReady }, At(10, 2, 0));
        Assert.Equal(GameState.Mission, _machine.GetSnapshot().State);

        _machine.Apply(new GameLogEventStruct() { Kind = GameLogEventKind.GameStopped }, At(10, 3, 0));
        _machine.Apply(new GameLogEventStruct() { Kind = GameLogEventKind.MainMenu }, At(10, 3, 1));
        _machine.Apply(new GameLogEventStruct() { Kind = GameLogEventKind.LoadingGame, SaveName = "x" }, At(10, 4, 0));
        _machine.Apply(new GameLogEventStruct() { Kind = GameLogEventKind.GameReady }, At(10, 5, 0));

        GameStateSnapshotStruct snapshot = _machine.GetSnapshot();
        Assert.Equal(GameState.Playing, snapshot.State);
        Assert.Null(snapshot.MissionNumber);
        Assert.Equal(At(10, 5, 0), snapshot.TimerStart);
    }

    [Fact]
    public void ProcessNotRunning_OverridesTheLog()
    {
        List<TimedEvent> events = SampleFiles.ReadEvents("session.log");
        _ = Feed(events, 0, GameLogEventKind.Startup);
        _ = Feed(events, 1, GameLogEventKind.MainMenu);

        _machine.SetProcessRunning(false);
        GameStateSnapshotStruct stopped = _machine.GetSnapshot();
        _machine.Apply(events[2].Event, events[2].Timestamp);

        Assert.Equal(GameState.NotRunning, stopped.State);
        Assert.Null(stopped.TimerStart);
        Assert.Equal(GameState.NotRunning, _machine.GetSnapshot().State);

        _machine.SetProcessRunning(true);
        GameStateSnapshotStruct restarted = _machine.GetSnapshot();

        Assert.Equal(GameState.Starting, restarted.State);
        Assert.Null(restarted.TimerStart);
    }

    [Fact]
    public void Reset_ClearsEverything()
    {
        List<TimedEvent> events = SampleFiles.ReadEvents("session.log");
        _clock.Now = (At(21, 15, 26));
        _ = Feed(events, 0, GameLogEventKind.Startup);
        _ = Feed(events, 3, GameLogEventKind.GameReady);
        _ = Feed(events, 4, GameLogEventKind.Status);

        _machine.Reset();
        GameStateSnapshotStruct snapshot = _machine.GetSnapshot();

        Assert.Equal(GameState.Starting, snapshot.State);
        Assert.Null(snapshot.TimerStart);
        Assert.Null(snapshot.Status);
        Assert.Null(snapshot.MissionNumber);
    }

    [Fact]
    public void Reset_WithoutProcessStaysNotRunning()
    {
        GameStateMachine machine = new(_clock);

        machine.Reset();

        Assert.Equal(GameState.NotRunning, machine.GetSnapshot().State);
    }

    [Fact]
    public void Snapshots_AreEqualUntilSomethingChanges()
    {
        List<TimedEvent> events = SampleFiles.ReadEvents("session.log");
        _clock.Now = (At(21, 15, 26));
        _ = Feed(events, 3, GameLogEventKind.GameReady);
        GameStateSnapshotStruct withStatus = Feed(events, 4, GameLogEventKind.Status);

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

    private static GameLogEventStruct StatusEvent(long speedup)
        => new() { Kind = GameLogEventKind.Status, Status = new ModStatus() { Version = 1, Speedup = speedup } };

    [Fact]
    public void StatusAtT_IsNotPausedBefore20SecondsAndPausedAfterwardsWithTheStatusKept()
    {
        _machine.Apply(Event(GameLogEventKind.GameReady), At(10, 0, 0));
        _clock.Now = At(10, 0, 10);
        _machine.Apply(StatusEvent(4), At(10, 0, 10));

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

    [Fact]
    public void Pause_IsMeasuredInRealTimeDespiteAWallClockRollback()
    {
        _clock.Now = At(10, 0, 10);
        _machine.Apply(Event(GameLogEventKind.GameReady), At(10, 0, 0));
        _machine.Apply(StatusEvent(4), At(10, 0, 10));

        _clock.JumpWallClock(At(9, 0, 10));
        _clock.Now = _clock.Now.AddSeconds(15);
        Assert.False(_machine.GetSnapshot().IsPaused);

        _clock.Now = _clock.Now.AddSeconds(15);
        Assert.True(_machine.GetSnapshot().IsPaused);
    }

    [Fact]
    public void StatusThatWasAlreadyOldWhenApplied_CountsAsPausedAtOnce()
    {
        _clock.Now = At(10, 0, 35);
        _machine.Apply(Event(GameLogEventKind.GameReady), At(10, 0, 0));
        _machine.Apply(StatusEvent(4), At(10, 0, 10));

        Assert.True(_machine.GetSnapshot().IsPaused);
    }

    [Fact]
    public void StatusStampedInTheFuture_IsTreatedAsFreshNotNegativelyAged()
    {
        _clock.Now = At(10, 0, 0);
        _machine.Apply(Event(GameLogEventKind.GameReady), At(10, 0, 0));
        _machine.Apply(StatusEvent(4), At(10, 5, 0));

        _clock.Now = At(10, 0, 19);
        Assert.False(_machine.GetSnapshot().IsPaused);

        _clock.Now = At(10, 0, 21);
        Assert.True(_machine.GetSnapshot().IsPaused);
    }

    [Fact]
    public void NoEventForTwoMinutes_FallsBackToPlayingWithTheProcessStart()
    {
        _clock.Now = At(12, 0, 0);
        _machine.SetProcessRunning(false);
        _machine.SetProcessRunning(true, At(12, 0, 0));

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

    [Fact]
    public void AnEventWithinTwoMinutes_PreventsTheFallback()
    {
        _machine.SetProcessRunning(true, At(12, 0, 0));
        _machine.Apply(Event(GameLogEventKind.Startup), At(12, 0, 5));

        _clock.Now = _clock.Now.AddMinutes(10);

        Assert.Equal(GameState.Starting, _machine.GetSnapshot().State);
        Assert.False(_machine.GetSnapshot().UsesLogFallback);
    }

    [Fact]
    public void FallbackThenARecognisedEvent_ReturnsToTheNormalStates()
    {
        _machine.SetProcessRunning(true, At(12, 0, 0));
        _clock.Now = _clock.Now.AddMinutes(3);
        Assert.True(_machine.GetSnapshot().UsesLogFallback);

        _machine.Apply(Event(GameLogEventKind.MainMenu), At(12, 3, 0));

        GameStateSnapshotStruct snapshot = _machine.GetSnapshot();
        Assert.Equal(GameState.MainMenu, snapshot.State);
        Assert.False(snapshot.UsesLogFallback);
    }

    [Fact]
    public void LogRestart_DoesNotRearmTheFallback()
    {
        _machine.SetProcessRunning(true, At(12, 0, 0));
        _machine.Apply(Event(GameLogEventKind.Startup), At(12, 0, 5));
        _machine.Reset();

        _clock.Now = _clock.Now.AddMinutes(10);

        Assert.Equal(GameState.Starting, _machine.GetSnapshot().State);
        Assert.False(_machine.GetSnapshot().UsesLogFallback);
    }

    [Fact]
    public void ProcessStopOrNewProcess_RearmsTheFallback()
    {
        _machine.SetProcessRunning(true, At(12, 0, 0));
        _machine.Apply(Event(GameLogEventKind.Startup), At(12, 0, 5));

        _machine.SetProcessRunning(true, At(13, 0, 0));
        _clock.Now = _clock.Now.AddMinutes(3);
        Assert.True(_machine.GetSnapshot().UsesLogFallback);

        _machine.Apply(Event(GameLogEventKind.Startup), At(13, 0, 5));
        _machine.SetProcessRunning(false);
        _machine.SetProcessRunning(true, At(14, 0, 0));
        Assert.False(_machine.GetSnapshot().UsesLogFallback);
        _clock.Now = _clock.Now.AddMinutes(3);
        Assert.True(_machine.GetSnapshot().UsesLogFallback);
    }

    [Fact]
    public void PreviousSessionLines_DoNotCountAsSeenForTheFallback()
    {
        _machine.SetProcessRunning(true, At(12, 0, 0));
        _machine.Apply(Event(GameLogEventKind.MainMenu), At(11, 0, 0));
        _clock.Now = _clock.Now.AddMinutes(3);

        Assert.True(_machine.GetSnapshot().UsesLogFallback);
    }

    [Fact]
    public void ANewStatusAfterAPause_ClearsIt()
    {
        _machine.Apply(Event(GameLogEventKind.GameReady), At(10, 0, 0));
        _machine.Apply(StatusEvent(4), At(10, 0, 10));
        _clock.Now = At(10, 0, 50);
        Assert.True(_machine.GetSnapshot().IsPaused);

        _machine.Apply(StatusEvent(4), At(10, 0, 50));

        Assert.False(_machine.GetSnapshot().IsPaused);
    }

    [Fact]
    public void StatusWithSpeedupZero_IsPausedAtOnce()
    {
        _machine.Apply(Event(GameLogEventKind.GameReady), At(10, 0, 0));
        _clock.Now = At(10, 0, 11);

        _machine.Apply(StatusEvent(0), At(10, 0, 10));

        Assert.True(_machine.GetSnapshot().IsPaused);
    }

    [Fact]
    public void MapEditor_IsNeverPaused()
    {
        _machine.Apply(Event(GameLogEventKind.GameReady), At(10, 0, 0));
        _machine.Apply(new GameLogEventStruct() { Kind = GameLogEventKind.Status, Status = new ModStatus() { Version = 1, Speedup = 0, IsMapEditor = true } }, At(10, 0, 10));
        _clock.Now = At(10, 0, 11);

        Assert.False(_machine.GetSnapshot().IsPaused);

        _clock.Now = At(10, 1, 0);

        Assert.False(_machine.GetSnapshot().IsPaused);
    }

    [Fact]
    public void WithoutAStatus_NothingIsPaused()
    {
        _machine.Apply(Event(GameLogEventKind.GameReady), At(10, 0, 0));
        _clock.Now = At(10, 5, 0);

        GameStateSnapshotStruct snapshot = _machine.GetSnapshot();

        Assert.Equal(GameState.Playing, snapshot.State);
        Assert.Null(snapshot.Status);
        Assert.False(snapshot.IsPaused);
    }

    [Theory]
    [InlineData(GameLogEventKind.GameStopped)]
    [InlineData(GameLogEventKind.MainMenu)]
    [InlineData(GameLogEventKind.GameReady)]
    public void LeavingOrRestartingTheGame_ClearsTheStatus(GameLogEventKind kind)
    {
        _machine.Apply(Event(GameLogEventKind.GameReady), At(10, 0, 0));
        _machine.Apply(StatusEvent(4), At(10, 0, 10));
        _clock.Now = At(10, 5, 0);

        _machine.Apply(Event(kind), At(10, 4, 59));
        if (kind is GameLogEventKind.GameStopped or GameLogEventKind.MainMenu)
            _machine.Apply(Event(GameLogEventKind.GameReady), At(10, 4, 59));

        GameStateSnapshotStruct snapshot = _machine.GetSnapshot();

        Assert.Null(snapshot.Status);
        Assert.False(snapshot.IsPaused);
    }

    private static GameLogEventStruct Event(GameLogEventKind kind)
        => new() { Kind = kind };

    [Fact]
    public void EventsBeforeTheProcessStart_AreIgnoredWithATwoSecondTolerance()
    {
        DateTimeOffset started = At(12, 0, 10);
        _machine.SetProcessRunning(true, started);

        _machine.Apply(Event(GameLogEventKind.MainMenu), At(12, 0, 7));
        Assert.Equal(GameState.Starting, _machine.GetSnapshot().State);
        Assert.Null(_machine.GetSnapshot().TimerStart);

        _machine.Apply(Event(GameLogEventKind.MainMenu), At(12, 0, 8));
        Assert.Equal(GameState.MainMenu, _machine.GetSnapshot().State);

        _machine.Apply(Event(GameLogEventKind.Closing), At(12, 0, 9));
        Assert.Equal(GameState.Closing, _machine.GetSnapshot().State);
    }

    [Fact]
    public void WithoutAStartTime_NothingIsFiltered()
    {
        _machine.SetProcessRunning(true);

        _machine.Apply(Event(GameLogEventKind.MainMenu), At(1, 0, 0));

        Assert.Equal(GameState.MainMenu, _machine.GetSnapshot().State);
    }

    [Fact]
    public void ADifferentStartTimeOfARunningProcess_CountsAsANewProcess()
    {
        _machine.SetProcessRunning(true, At(12, 0, 0));
        _machine.Apply(Event(GameLogEventKind.MainMenu), At(12, 0, 5));
        Assert.Equal(GameState.MainMenu, _machine.GetSnapshot().State);

        _machine.SetProcessRunning(true, At(12, 0, 5));
        Assert.Equal(GameState.Starting, _machine.GetSnapshot().State);

        _machine.SetProcessRunning(true, At(12, 0, 5));
        _machine.Apply(Event(GameLogEventKind.MainMenu), At(12, 0, 6));
        _machine.SetProcessRunning(true, At(12, 0, 5));
        Assert.Equal(GameState.MainMenu, _machine.GetSnapshot().State);
    }

    [Fact]
    public void AStartTimeReadLaterOrNotAtAll_KeepsTheState()
    {
        _machine.SetProcessRunning(true);
        _machine.Apply(Event(GameLogEventKind.MainMenu), At(12, 0, 5));

        _machine.SetProcessRunning(true, At(12, 0, 0));
        Assert.Equal(GameState.MainMenu, _machine.GetSnapshot().State);

        _machine.SetProcessRunning(true);
        Assert.Equal(GameState.MainMenu, _machine.GetSnapshot().State);

        _machine.Apply(Event(GameLogEventKind.Closing), At(11, 59, 0));
        Assert.Equal(GameState.MainMenu, _machine.GetSnapshot().State);
    }

    [Fact]
    public void Reset_KeepsTheStartTimeFilter()
    {
        _machine.SetProcessRunning(true, At(12, 0, 10));
        _machine.Reset();

        _machine.Apply(Event(GameLogEventKind.MainMenu), At(12, 0, 0));

        Assert.Equal(GameState.Starting, _machine.GetSnapshot().State);
    }

    [Fact]
    public void ConfirmedSession_KeepsAcceptingEventsWhenTheClockIsSetBack()
    {
        _machine.SetProcessRunning(true, At(12, 0, 0));
        _machine.Apply(Event(GameLogEventKind.GameReady), At(12, 1, 0));
        _machine.Apply(new GameLogEventStruct() { Kind = GameLogEventKind.Status, Status = new ModStatus() { Version = 1, Speedup = 1, Rank = 2 } }, At(12, 1, 10));
        Assert.Equal(2, _machine.GetSnapshot().Status?.Rank);

        _machine.Apply(new GameLogEventStruct() { Kind = GameLogEventKind.Status, Status = new ModStatus() { Version = 1, Speedup = 1, Rank = 3 } }, At(11, 0, 0));
        Assert.Equal(3, _machine.GetSnapshot().Status?.Rank);

        _machine.Apply(Event(GameLogEventKind.MainMenu), At(11, 0, 20));
        Assert.Equal(GameState.MainMenu, _machine.GetSnapshot().State);
    }

    [Fact]
    public void ConfirmedSession_SurvivesAResetButNotANewProcess()
    {
        _machine.SetProcessRunning(true, At(12, 0, 0));
        _machine.Apply(Event(GameLogEventKind.MainMenu), At(12, 1, 0));
        _machine.Reset();

        _machine.Apply(Event(GameLogEventKind.MainMenu), At(11, 0, 0));
        Assert.Equal(GameState.MainMenu, _machine.GetSnapshot().State);

        _machine.SetProcessRunning(true, At(13, 0, 0));
        _machine.Apply(Event(GameLogEventKind.MainMenu), At(11, 0, 0));
        Assert.Equal(GameState.Starting, _machine.GetSnapshot().State);

        _machine.SetProcessRunning(false);
        _machine.SetProcessRunning(true, At(14, 0, 0));
        _machine.Apply(Event(GameLogEventKind.MainMenu), At(11, 0, 0));
        Assert.Equal(GameState.Starting, _machine.GetSnapshot().State);
    }

    [Fact]
    public void PreviousSessionLines_BeforeTheFirstConfirmationAreStillIgnored()
    {
        _machine.SetProcessRunning(true, At(12, 0, 0));
        _machine.Apply(Event(GameLogEventKind.GameReady), At(11, 0, 0));
        _machine.Apply(Event(GameLogEventKind.MainMenu), At(11, 30, 0));
        Assert.Equal(GameState.Starting, _machine.GetSnapshot().State);

        _machine.Apply(Event(GameLogEventKind.MainMenu), At(12, 0, 1));
        Assert.Equal(GameState.MainMenu, _machine.GetSnapshot().State);
    }

    [Fact]
    public void ProcessStop_ForgetsTheStartTime()
    {
        _machine.SetProcessRunning(true, At(12, 0, 10));
        _machine.SetProcessRunning(false);
        _machine.SetProcessRunning(true);

        _machine.Apply(Event(GameLogEventKind.MainMenu), At(1, 0, 0));

        Assert.Equal(GameState.MainMenu, _machine.GetSnapshot().State);
    }

    [Fact]
    public void ReplayOfThePreviousSession_DoesNotDriveTheState()
    {
        List<TimedEvent> old = SampleFiles.ReadEvents("session.log");
        _machine.SetProcessRunning(true, At(23, 0, 0));

        foreach (TimedEvent timed in old)
        {
            _machine.Apply(timed.Event, timed.Timestamp);
            Assert.Equal(GameState.Starting, _machine.GetSnapshot().State);
        }

        _machine.Apply(Event(GameLogEventKind.Startup), At(23, 0, 3));
        _machine.Apply(Event(GameLogEventKind.MainMenu), At(23, 0, 9));

        GameStateSnapshotStruct snapshot = _machine.GetSnapshot();
        Assert.Equal(GameState.MainMenu, snapshot.State);
        Assert.Equal(At(23, 0, 3), snapshot.TimerStart);
    }

    private static GameLogEventStruct BuildEvent(int build)
        => new() { Kind = GameLogEventKind.GameBuild, GameBuild = build };

    [Fact]
    public void GameBuild_IsSetAfterStartupAndKeptThroughTheGameStates()
    {
        _machine.Apply(Event(GameLogEventKind.Startup), At(10, 0, 0));
        _machine.Apply(BuildEvent(40420), At(10, 0, 0));

        Assert.Equal(40420, _machine.GetSnapshot().GameBuild);

        foreach (GameLogEventKind kind in (GameLogEventKind[])[GameLogEventKind.MainMenu, GameLogEventKind.LoadingGame, GameLogEventKind.GameReady, GameLogEventKind.GameStopped])
        {
            _machine.Apply(Event(kind), At(10, 1, 0));
            Assert.Equal(40420, _machine.GetSnapshot().GameBuild);
        }
    }

    [Fact]
    public void GameBuild_IsClearedByResetAndGoneAfterTheProcessStops()
    {
        _machine.SetProcessRunning(true, At(10, 0, 0));
        _machine.Apply(BuildEvent(40420), At(10, 0, 1));

        Assert.Equal(40420, _machine.GetSnapshot().GameBuild);

        _machine.Reset();

        Assert.Null(_machine.GetSnapshot().GameBuild);

        _machine.Apply(BuildEvent(40420), At(10, 0, 2));

        Assert.Equal(40420, _machine.GetSnapshot().GameBuild);

        _machine.SetProcessRunning(false);

        Assert.Null(_machine.GetSnapshot().GameBuild);

        _machine.SetProcessRunning(true, At(11, 0, 0));

        Assert.Null(_machine.GetSnapshot().GameBuild);
    }

    [Fact]
    public void GameBuild_IsClearedByANewProcess()
    {
        _machine.SetProcessRunning(true, At(12, 0, 0));
        _machine.Apply(BuildEvent(40420), At(12, 0, 1));
        _machine.SetProcessRunning(true, At(13, 0, 0));

        Assert.Null(_machine.GetSnapshot().GameBuild);
    }

    [Fact]
    public void GameBuild_OfThePreviousSessionIsIgnored()
    {
        _machine.SetProcessRunning(true, At(23, 0, 0));
        _machine.Apply(BuildEvent(40420), At(22, 0, 0));

        Assert.Null(_machine.GetSnapshot().GameBuild);

        _machine.Apply(BuildEvent(40421), At(23, 0, 3));

        Assert.Equal(40421, _machine.GetSnapshot().GameBuild);
    }

    [Fact]
    public void Snapshots_DifferWhenOnlyTheGameBuildDiffers()
    {
        _machine.Apply(Event(GameLogEventKind.MainMenu), At(10, 0, 0));
        GameStateSnapshotStruct before = _machine.GetSnapshot();
        _machine.Apply(BuildEvent(40420), At(10, 0, 1));
        GameStateSnapshotStruct after = _machine.GetSnapshot();

        Assert.NotEqual(before, after);
        Assert.True(before != after);
        Assert.Equal(before.State, after.State);
        Assert.Equal(before.TimerStart, after.TimerStart);
    }
}
