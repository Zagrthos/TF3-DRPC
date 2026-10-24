using System.Text.RegularExpressions;

using Tf3Drpc.Core.Constants;

namespace Tf3Drpc.Core.Utilities;

internal static partial class RegexUtils
{
    /// <summary>
    /// Matches the header of a game log line: time, level, thread, category and the separator before the message.
    /// </summary>
    [GeneratedRegex
    (
        @"^\[(?<time>[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2})Z - (?<level>[A-Z]+) +- .{16} - .{15}\](?:(?<multi>\+ )|  )(?<message>[^\r\n]*)",
        RegexOptions.CultureInvariant,
        RegexConstants.TimeoutMilliseconds
    )]
    private static partial Regex GameLogHeader { get; }

    /// <summary>
    /// Matches the number of a campaign mission at the end of its mod id.
    /// </summary>
    [GeneratedRegex(@"_mission_(?<number>[0-9]+)::/info\.mission$", RegexOptions.CultureInvariant, RegexConstants.TimeoutMilliseconds)]
    private static partial Regex MissionNumber { get; }

    /// <summary>
    /// Matches the build number of the game's <c>Starting up build version:</c> message.
    /// </summary>
    [GeneratedRegex(@"^Starting up build version: (?<build>[0-9]+), build name ", RegexOptions.CultureInvariant, RegexConstants.TimeoutMilliseconds)]
    private static partial Regex GameBuild { get; }

    /// <summary>
    /// Matches <c>language = { ... code = "xx" ... }</c> in the game's <c>profile.lua</c>, one level of nested tables allowed.
    /// </summary>
    [GeneratedRegex("\\blanguage\\s*=\\s*\\{(?:[^{}]|\\{[^{}]*\\})*?\\bcode\\s*=\\s*\"(?<code>[A-Za-z_]+)\"", RegexOptions.CultureInvariant, RegexConstants.TimeoutMilliseconds)]
    private static partial Regex LanguageCode { get; }

    /// <summary>
    /// Matches a game log line header; no match and a regex timeout give <see langword="null"/>.
    /// </summary>
    public static Match? MatchGameLogHeader(string line)
        => SafeMatch(GameLogHeader, line);

    /// <summary>
    /// Matches the mission number of a <c>Campaign/mission:</c> message; no match and a regex timeout give <see langword="null"/>.
    /// </summary>
    public static Match? MatchMissionNumber(string message)
        => SafeMatch(MissionNumber, message);

    /// <summary>
    /// Matches the build number of a <c>Starting up build version:</c> message; no match and a regex timeout give <see langword="null"/>.
    /// </summary>
    public static Match? MatchGameBuild(string message)
        => SafeMatch(GameBuild, message);

    /// <summary>
    /// Matches the language code of a <c>profile.lua</c>; no match and a regex timeout give <see langword="null"/>.
    /// </summary>
    public static Match? MatchLanguageCode(string profileText)
        => SafeMatch(LanguageCode, profileText);

    private static Match? SafeMatch(Regex regex, string input)
    {
        try
        {
            Match match = regex.Match(input);

            return (match.Success) ? match : null;
        }
        catch (RegexMatchTimeoutException)
        {
            return null;
        }
    }
}
