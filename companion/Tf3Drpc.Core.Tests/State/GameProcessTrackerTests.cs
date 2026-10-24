using System;

using Tf3Drpc.Core.State;

using Xunit;

namespace Tf3Drpc.Core.Tests.State;

public sealed class GameProcessTrackerTests
{
    private static readonly DateTimeOffset First = new(2026, 10, 14, 12, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset Second = First.AddMinutes(1);

    private readonly GameProcessTracker _tracker = new();

    [Fact]
    public void NothingRunning_IsNeverANewProcess()
    {
        Assert.False(_tracker.Update(running: false, startedAt: null));
        Assert.False(_tracker.Update(running: false, startedAt: null));
        Assert.False(_tracker.IsRunning);
    }

    [Fact]
    public void RunningAfterNone_IsANewProcessOnce()
    {
        Assert.True(_tracker.Update(running: true, First));
        Assert.False(_tracker.Update(running: true, First));
        Assert.True(_tracker.IsRunning);
        Assert.Equal(First, _tracker.StartedAt);
    }

    [Fact]
    public void RunningWithoutAStartTime_IsANewProcessToo()
        => Assert.True(_tracker.Update(running: true, startedAt: null));

    [Fact]
    public void DifferentKnownStartTime_IsANewProcessWithoutAStoppedCheck()
    {
        _ = _tracker.Update(running: true, First);

        Assert.True(_tracker.Update(running: true, Second));
        Assert.False(_tracker.Update(running: true, Second));
        Assert.Equal(Second, _tracker.StartedAt);
    }

    [Fact]
    public void StartTimeReadableOnlyLater_IsTheSameProcess()
    {
        _ = _tracker.Update(running: true, startedAt: null);

        Assert.False(_tracker.Update(running: true, First));
        Assert.Equal(First, _tracker.StartedAt);
    }

    [Fact]
    public void StartTimeUnreadableOnce_IsTheSameProcessAndKeepsTheKnownTime()
    {
        _ = _tracker.Update(running: true, First);

        Assert.False(_tracker.Update(running: true, startedAt: null));
        Assert.Equal(First, _tracker.StartedAt);
        Assert.False(_tracker.Update(running: true, First));
    }

    [Fact]
    public void StopAndStartAgain_IsANewProcessEvenWithTheSameStartTime()
    {
        _ = _tracker.Update(running: true, First);

        Assert.False(_tracker.Update(running: false, startedAt: null));
        Assert.Null(_tracker.StartedAt);
        Assert.True(_tracker.Update(running: true, First));
    }
}
