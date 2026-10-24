using System;

namespace Tf3Drpc.Core.State;

/// <summary>
/// Decides from the process checks whether a new game process has started.
/// </summary>
internal sealed class GameProcessTracker
{
    /// <summary>
    /// Gets whether the last check found the game process.
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    /// Gets the start time of the running process, if it is known.
    /// </summary>
    public DateTimeOffset? StartedAt { get; private set; }

    /// <summary>
    /// Takes the result of a process check; returns whether a new process has started since the last one.
    /// </summary>
    /// <remarks>
    /// A new process is a process after none, or a different known start time. A start time read only now, or not readable
    /// this time, is the same process.
    /// </remarks>
    public bool Update(bool running, DateTimeOffset? startedAt)
    {
        if (running != IsRunning)
        {
            IsRunning = running;
            StartedAt = (running) ? startedAt : null;

            return running;
        }

        if (!running || (startedAt is null))
            return false;

        bool replaced = (StartedAt is DateTimeOffset known) && (known != startedAt);
        StartedAt = startedAt;

        return replaced;
    }
}
