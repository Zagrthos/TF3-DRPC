using System;

using Microsoft.Extensions.Logging;

using Tf3Drpc.Core.Discord;
using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Monitor;

namespace Tf3Drpc.Core.Logging;

internal static partial class LogMessages
{
    #region Debug

    [LoggerMessage(LogLevel.Debug, "Discord is not running.")]
    public static partial void DiscordNotAvailable(this ILogger<DiscordPresenceClient> logger);

    [LoggerMessage(LogLevel.Debug, "Presence update deferred by the rate limit for {Seconds:F1} s.")]
    public static partial void DiscordSendDeferred(this ILogger<DiscordPresenceClient> logger, double seconds);

    [LoggerMessage(LogLevel.Debug, "Game log: {Count} new event(s), restarted: {Restarted}.")]
    public static partial void GameLogPolled(this ILogger<GameMonitor> logger, int count, bool restarted);

    #endregion Debug

    #region Information

    [LoggerMessage(LogLevel.Information, "Connected to Discord.")]
    public static partial void DiscordConnected(this ILogger<DiscordPresenceClient> logger);

    [LoggerMessage(LogLevel.Information, "Disconnected from Discord.")]
    public static partial void DiscordDisconnected(this ILogger<DiscordPresenceClient> logger);

    [LoggerMessage(LogLevel.Information, "The game started. Executable: {ExecutablePath}.")]
    public static partial void GameStarted(this ILogger<GameMonitor> logger, string? executablePath);

    [LoggerMessage(LogLevel.Information, "The game stopped.")]
    public static partial void GameStopped(this ILogger<GameMonitor> logger);

    [LoggerMessage(LogLevel.Information, "Game state: {State}, mission {Mission}.")]
    public static partial void GameStateChanged(this ILogger<GameMonitor> logger, GameState state, int? mission);

    [LoggerMessage(LogLevel.Information, "Game build: {Build}.")]
    public static partial void GameBuildDetected(this ILogger<GameMonitor> logger, int build);

    [LoggerMessage(LogLevel.Information, "Language: {Language}.")]
    public static partial void LanguageChanged(this ILogger<GameMonitor> logger, string language);

    [LoggerMessage(LogLevel.Information, "Game paths: log {LogPath}, profile {ProfilePath}, game folder {GameDirectory}.")]
    public static partial void GamePathsResolved(this ILogger<GameMonitor> logger, string? logPath, string? profilePath, string? gameDirectory);

    [LoggerMessage(LogLevel.Information, "Discord connection: {Connected}.")]
    public static partial void DiscordConnectionChanged(this ILogger<GameMonitor> logger, bool connected);

    #endregion Information

    #region Warning

    [LoggerMessage(LogLevel.Warning, "The Discord handshake failed.")]
    public static partial void DiscordHandshakeFailed(this ILogger<DiscordPresenceClient> logger);

    [LoggerMessage(LogLevel.Warning, "The Discord connection was lost.")]
    public static partial void DiscordConnectionLost(this ILogger<DiscordPresenceClient> logger);

    [LoggerMessage(LogLevel.Warning, "Discord reported an error: code {Code}, message {ErrorMessage}.")]
    public static partial void DiscordError(this ILogger<DiscordPresenceClient> logger, int? code, string? errorMessage);

    [LoggerMessage(LogLevel.Warning, "Discord rejected the presence; trying again in {Seconds:F0} s.")]
    public static partial void DiscordActivityRejected(this ILogger<DiscordPresenceClient> logger, double seconds);

    [LoggerMessage(LogLevel.Warning, "The game's files were not found yet (Steam folder, user data or game folder).")]
    public static partial void GamePathsNotFound(this ILogger<GameMonitor> logger);

    [LoggerMessage(LogLevel.Warning, "The game log {LogPath} gave no usable lines for 2 minutes; showing Playing.")]
    public static partial void GameLogUnusable(this ILogger<GameMonitor> logger, string? logPath);

    #endregion Warning

    #region Error

    [LoggerMessage(LogLevel.Error, "A monitor tick failed.")]
    public static partial void MonitorTickFailed(this ILogger<GameMonitor> logger, Exception exception);

    #endregion Error
}
