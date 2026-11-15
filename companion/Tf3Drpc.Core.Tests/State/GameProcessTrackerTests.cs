using System;

using Tf3Drpc.Core.State;

using Xunit;

namespace Tf3Drpc.Core.Tests.State;

/// <summary>
/// Tests of <see cref="GameProcessTracker"/>, covering when an update counts as a new game process.
/// </summary>
public sealed class GameProcessTrackerTests
{
    private static readonly DateTimeOffset First = new(2026, 10, 14, 12, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset Second = First.AddMinutes(1);

    private readonly GameProcessTracker _tracker = new();

    /// <summary>
    /// Updating repeatedly while no game is running never reports a new process and leaves the tracker not running.
    /// </summary>
    [Fact]
    public void NothingRunningIsNeverANewProcess()
    {
        Assert.False(_tracker.Update(running: false, startedAt: null));
        Assert.False(_tracker.Update(running: false, startedAt: null));
        Assert.False(_tracker.IsRunning);
    }

    /// <summary>
    /// The first update that reports a running game is a new process, and repeating the same start time does not report another.
    /// </summary>
    [Fact]
    public void RunningAfterNoneIsANewProcessOnce()
    {
        Assert.True(_tracker.Update(running: true, First));
        Assert.False(_tracker.Update(running: true, First));
        Assert.True(_tracker.IsRunning);
        Assert.Equal(First, _tracker.StartedAt);
    }

    /// <summary>
    /// A running game whose start time cannot be read is a new process.
    /// </summary>
    [Fact]
    public void RunningWithoutAStartTimeIsANewProcessToo()
        => Assert.True(_tracker.Update(running: true, startedAt: null));

    /// <summary>
    /// A running game with a different known start time than before is a new process even without an intervening stopped update, and its start time is remembered.
    /// </summary>
    [Fact]
    public void DifferentKnownStartTimeIsANewProcessWithoutAStoppedCheck()
    {
        _ = _tracker.Update(running: true, First);

        Assert.True(_tracker.Update(running: true, Second));
        Assert.False(_tracker.Update(running: true, Second));
        Assert.Equal(Second, _tracker.StartedAt);
    }

    /// <summary>
    /// A start time that becomes readable only after a running update without one belongs to the same process and is stored.
    /// </summary>
    [Fact]
    public void StartTimeReadableOnlyLaterIsTheSameProcess()
    {
        _ = _tracker.Update(running: true, startedAt: null);

        Assert.False(_tracker.Update(running: true, First));
        Assert.Equal(First, _tracker.StartedAt);
    }

    /// <summary>
    /// A start time that cannot be read in one update belongs to the same process, keeps the known start time and does not make the next update with that time a new process.
    /// </summary>
    [Fact]
    public void StartTimeUnreadableOnceIsTheSameProcessAndKeepsTheKnownTime()
    {
        _ = _tracker.Update(running: true, First);

        Assert.False(_tracker.Update(running: true, startedAt: null));
        Assert.Equal(First, _tracker.StartedAt);
        Assert.False(_tracker.Update(running: true, First));
    }

    /// <summary>
    /// Starting again after a stopped update is a new process even when the start time equals the earlier one, and the start time is cleared while stopped.
    /// </summary>
    [Fact]
    public void StopAndStartAgainIsANewProcessEvenWithTheSameStartTime()
    {
        _ = _tracker.Update(running: true, First);

        Assert.False(_tracker.Update(running: false, startedAt: null));
        Assert.Null(_tracker.StartedAt);
        Assert.True(_tracker.Update(running: true, First));
    }
}
