using System;
using System.Globalization;
using System.Text.RegularExpressions;

using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Structs;
using Tf3Drpc.Core.Utilities;

namespace Tf3Drpc.Core.GameLog;

/// <summary>
/// Parses the header of a game log line.
/// </summary>
public static class GameLogParser
{
    private const string TimeFormat = "yyyy-MM-dd HH:mm:ss";

    /// <summary>
    /// Parses a line; continuation lines and garbage give <see langword="false"/>.
    /// </summary>
    public static bool TryParse(string line, out GameLogEntryStruct entry)
    {
        ArgumentNullException.ThrowIfNull(line);
        entry = default;

        if (line.Length is 0 || line[0] is not '[')
            return false;

        Match? match = RegexUtils.MatchGameLogHeader(line);

        if (match is null)
            return false;

        bool timeParsed = DateTimeOffset.TryParseExact
        (
            match.Groups["time"].ValueSpan,
            TimeFormat,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
            out DateTimeOffset timestamp
        );

        GameLogLevel? level = ParseLevel(match.Groups["level"].Value);

        if (!timeParsed || level is null)
            return false;

        string message = match.Groups["message"].Value;
        bool startsMultiLine = match.Groups["multi"].Success;
        entry = new GameLogEntryStruct() { Timestamp = timestamp, Level = level.Value, Message = message, StartsMultiLine = startsMultiLine };

        return true;
    }

    private static GameLogLevel? ParseLevel(string text)
        => text switch
        {
            "VERBOSE" => GameLogLevel.Verbose,
            "MESSAGE" => GameLogLevel.Message,
            "WARNING" => GameLogLevel.Warning,
            "ERROR" => GameLogLevel.Error,
            _ => null
        };
}
