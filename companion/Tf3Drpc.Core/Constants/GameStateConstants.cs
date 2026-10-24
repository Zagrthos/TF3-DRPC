using System;

namespace Tf3Drpc.Core.Constants;

/// <summary>
/// Tuning values of the game state machine.
/// </summary>
public static class GameStateConstants
{
    public static readonly TimeSpan PauseAfter = TimeSpan.FromSeconds(20);

    public static readonly TimeSpan LogFallbackAfter = TimeSpan.FromMinutes(2);

    public static readonly TimeSpan ProcessStartTolerance = TimeSpan.FromSeconds(2);

    public static readonly TimeSpan FutureLogTolerance = TimeSpan.FromMinutes(1);
}
