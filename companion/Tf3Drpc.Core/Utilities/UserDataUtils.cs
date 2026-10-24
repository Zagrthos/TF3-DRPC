using System;
using System.Collections.Generic;

using Tf3Drpc.Core.Constants;
using Tf3Drpc.Core.Structs;

namespace Tf3Drpc.Core.Utilities;

/// <summary>
/// Chooses between the Steam accounts of the machine.
/// </summary>
public static class UserDataUtils
{
    /// <summary>
    /// Returns the folder of the account whose game log was written last; without any log the first account, without accounts <see langword="null"/>.
    /// </summary>
    /// <remarks>
    /// A log dated later than <paramref name="utcNow"/> (plus a tolerance) comes from a clock that was set back; such a log only wins when every log is dated like that.
    /// </remarks>
    public static string? ChooseFolder(IReadOnlyList<UserDataCandidateStruct> candidates, in DateTime utcNow)
    {
        ArgumentNullException.ThrowIfNull(candidates);

        DateTime latestPlausible = utcNow + GameStateConstants.FutureLogTolerance;
        string? best = null;
        DateTime bestTime = DateTime.MinValue;
        string? bestFuture = null;
        DateTime bestFutureTime = DateTime.MinValue;

        for (int i = 0; i < candidates.Count; i++)
        {
            UserDataCandidateStruct candidate = candidates[i];
            DateTime written = candidate.LogWrittenUtc ?? DateTime.MinValue;

            if (written > latestPlausible)
            {
                if ((bestFuture is null) || (written > bestFutureTime))
                {
                    bestFuture = candidate.Folder;
                    bestFutureTime = written;
                }
            }
            else if ((best is null) || (written > bestTime))
            {
                best = candidate.Folder;
                bestTime = written;
            }
        }

        return best ?? bestFuture;
    }
}
