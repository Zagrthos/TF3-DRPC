using System;

namespace Tf3Drpc.Core.Tests;

internal sealed class SettableTimeProvider : TimeProvider
{
    private DateTimeOffset _now;
    private long _ticks;

    public override long TimestampFrequency
        => TimeSpan.TicksPerSecond;

    /// <summary>
    /// Gets or sets the wall clock; moving it forwards advances the monotonic timestamp by the same amount.
    /// </summary>
    public DateTimeOffset Now
    {
        get => _now;

        set
        {
            _ticks += (value - _now).Ticks;
            _now = value;
        }
    }

    /// <summary>
    /// Sets the wall clock without touching the monotonic timestamp, like a clock correction.
    /// </summary>
    public void JumpWallClock(in DateTimeOffset value)
        => _now = value;

    public override DateTimeOffset GetUtcNow()
        => _now;

    public override long GetTimestamp()
        => _ticks;
}
