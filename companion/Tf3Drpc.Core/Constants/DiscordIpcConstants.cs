using System;

namespace Tf3Drpc.Core.Constants;

/// <summary>
/// Limits and fixed values of the Discord IPC connection.
/// </summary>
internal static class DiscordIpcConstants
{
    public const string ApplicationId = "1559680023458484244";

    public const int ProtocolVersion = 1;

    public const string EndpointPrefix = "discord-ipc-";

    public const int EndpointCount = 10;

    public const int ConnectTimeoutMilliseconds = 300;

    public const int HeaderBytes = 8;

    public const int MaxFrameBytes = 64 * 1024;

    public const int CloseTimeoutMilliseconds = 1000;

#pragma warning disable S5443 // The fallback is the documented Discord socket location when no runtime directory variable is set.
    public const string FallbackDirectory = "/tmp";
#pragma warning restore S5443

    public const string SetActivityCommand = "SET_ACTIVITY";

    public const string DispatchCommand = "DISPATCH";

    public const string ReadyEvent = "READY";

    public const string ErrorEvent = "ERROR";

    public static readonly TimeSpan ReadyTimeout = TimeSpan.FromSeconds(5);

    public static readonly TimeSpan WriteTimeout = TimeSpan.FromSeconds(5);

    public static readonly TimeSpan UpdateInterval = TimeSpan.FromSeconds(15);

    public static readonly TimeSpan MaxRejectedRetryInterval = TimeSpan.FromMinutes(5);

    public static readonly TimeSpan ReconnectInterval = TimeSpan.FromSeconds(15);
}
