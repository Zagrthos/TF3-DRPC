using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;

using Tf3Drpc.Core.Constants;
using Tf3Drpc.Core.Serialization;
using Tf3Drpc.Core.Status;
using Tf3Drpc.Core.Structs;
using Tf3Drpc.Core.Utilities;

namespace Tf3Drpc.Core.GameLog;

/// <summary>
/// Maps a parsed game log entry to the event the companion reacts to.
/// </summary>
internal static class GameLogEventClassifier
{
    /// <summary>
    /// Classifies an entry; unknown lines give <see langword="false"/>.
    /// </summary>
    public static bool TryClassify(in GameLogEntryStruct entry, [NotNullWhen(true)] out GameLogEvent? gameEvent)
    {
        string message = entry.Message;
        gameEvent = null;

        switch (message)
        {
            case GameLogConstants.StartupMessage:
                gameEvent = new StartupEvent();
                return true;

            case GameLogConstants.MainMenuMessage:
                gameEvent = new MainMenuEvent();
                return true;

            case GameLogConstants.GameReadyMessage:
                gameEvent = new GameReadyEvent();
                return true;

            case GameLogConstants.GameStoppedMessage:
                gameEvent = new GameStoppedEvent();
                return true;

            // "Quit requested" isn't one: the game can stay open after it (seen 2026-10-17), and a real quit logs the shutdown a second later.
            case GameLogConstants.NormalShutdownMessage:
                gameEvent = new ClosingEvent(IsCrash: false);
                return true;

            case GameLogConstants.UngracefulExitMessage:
                gameEvent = new ClosingEvent(IsCrash: true);
                return true;

            default:
                return TryClassifyPrefixed(message, out gameEvent);
        }
    }

    private static bool TryClassifyPrefixed(string message, [NotNullWhen(true)] out GameLogEvent? gameEvent)
    {
        gameEvent = null;

        if (message.StartsWith(GameLogConstants.LoadingGamePrefix, StringComparison.Ordinal))
        {
            string saveName = message[GameLogConstants.LoadingGamePrefix.Length..];
            gameEvent = new LoadingGameEvent(saveName);

            return true;
        }

        if (message.StartsWith(GameLogConstants.GameBuildPrefix, StringComparison.Ordinal))
        {
            int? build = ParseGameBuild(message);

            if (build is null)
                return false;

            gameEvent = new GameBuildEvent(build.Value);

            return true;
        }

        if (message.StartsWith(GameLogConstants.MissionPrefix, StringComparison.Ordinal))
        {
            int? missionNumber = ParseMissionNumber(message);
            gameEvent = new MissionLoadingEvent(missionNumber);

            return true;
        }

        if (!message.StartsWith(GameLogConstants.StatusPrefix, StringComparison.Ordinal))
            return false;

        ModStatus? status = ParseStatus(message[GameLogConstants.StatusPrefix.Length..]);

        if (status is null)
            return false;

        gameEvent = new StatusEvent(status);

        return true;
    }

    private static int? ParseMissionNumber(string message)
    {
        Match? match = RegexUtils.MatchMissionNumber(message);

        if (match is null)
            return null;

        bool parsed = int.TryParse(match.Groups["number"].ValueSpan, NumberStyles.None, CultureInfo.InvariantCulture, out int number);

        return (parsed) ? number : null;
    }

    private static int? ParseGameBuild(string message)
    {
        Match? match = RegexUtils.MatchGameBuild(message);

        if (match is null)
            return null;

        bool parsed = int.TryParse(match.Groups["build"].ValueSpan, NumberStyles.None, CultureInfo.InvariantCulture, out int build);

        return (parsed) ? build : null;
    }

    private static ModStatus? ParseStatus(string json)
    {
        try
        {
            ModStatus? status = JsonSerializer.Deserialize(json, JsonSourceGenContext.Default.ModStatus);

            return (status is { Version: GameLogConstants.SupportedStatusVersion }) ? status : null;
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
