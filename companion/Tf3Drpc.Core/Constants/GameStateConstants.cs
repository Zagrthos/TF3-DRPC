using System;

namespace Tf3Drpc.Core.Constants;

/// <summary>
/// Tuning values of the game state machine.
/// </summary>
internal static class GameStateConstants
{
    /// <summary>
    /// The age of the latest mod status after which the game counts as paused.
    /// </summary>
    public static readonly TimeSpan PauseAfter = TimeSpan.FromSeconds(20);

    /// <summary>
    /// The time without a usable log event after which a running process counts as playing.
    /// </summary>
    public static readonly TimeSpan LogFallbackAfter = TimeSpan.FromMinutes(2);

    /// <summary>
    /// How much older than the process start a log line may be and still count as the current session's.
    /// </summary>
    public static readonly TimeSpan ProcessStartTolerance = TimeSpan.FromSeconds(2);

    /// <summary>
    /// How far ahead of the clock a log file may be dated before it counts as written by a clock that was set back.
    /// </summary>
    public static readonly TimeSpan FutureLogTolerance = TimeSpan.FromMinutes(1);
}
