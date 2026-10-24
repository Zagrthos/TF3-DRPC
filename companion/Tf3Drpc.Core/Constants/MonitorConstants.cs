using System;

namespace Tf3Drpc.Core.Constants;

/// <summary>
/// Tuning values of the monitor.
/// </summary>
internal static class MonitorConstants
{
    public static readonly TimeSpan PathRetryInterval = TimeSpan.FromSeconds(15);

    public static readonly TimeSpan ProfileReadInterval = TimeSpan.FromSeconds(30);
}
