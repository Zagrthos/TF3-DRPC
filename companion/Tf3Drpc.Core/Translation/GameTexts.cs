using System;
using System.Collections.Generic;
using System.Globalization;

using Tf3Drpc.Core.Constants;
using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Extensions;

namespace Tf3Drpc.Core.Translation;

/// <summary>
/// The game's own texts for the presence, with English fallbacks.
/// </summary>
public sealed class GameTexts(IReadOnlyDictionary<string, string>? translations)
{
    private static readonly string[] RankKeys =
    [
        "Junior",
        "Mechanic",
        "Engineer",
        "Coordinator",
        "Expert",
        "Team Leader",
        "Supervisor",
        "Manager",
        "Director",
        "Senior Director",
        "CEO",
        "Chairperson",
        "Vice President",
        "President",
        "Tycoon"
    ];

    private static readonly string[] MissionNames =
    [
        "Saving Mardi Gras",
        "Alpine Crossing",
        "Desert Adventure",
        "Biggest Festival Ever",
        "Oil for the People",
        "Island Expansion",
        "Big City Problems",
        "Final Countdown"
    ];

    /// <summary>
    /// The texts without a translation file; everything is English.
    /// </summary>
    public static GameTexts English { get; } = new(null);

    /// <summary>
    /// Returns the name of a rank from 1 to 15, otherwise <see langword="null"/>.
    /// </summary>
    internal string? GetRankName(int rank)
        => (rank is >= 1 and <= TranslationConstants.RankCount) ? Lookup(RankKeys[rank - 1], RankKeys[rank - 1]) : null;

    /// <summary>
    /// Returns the name of a game mode.
    /// </summary>
    internal string GetModeName(GameMode mode)
        => Lookup(mode.TranslationKey, mode.EnglishText);

    /// <summary>
    /// Returns the name of a campaign mission from 1 to 8, otherwise <see langword="null"/>.
    /// </summary>
    internal string? GetMissionName(int mission)
    {
        if (mission is < 1 or > TranslationConstants.MissionCount)
            return null;

        string key = string.Create(CultureInfo.InvariantCulture, $"MISSION_{mission:00}_NAME");

        return Lookup(key, MissionNames[mission - 1]);
    }

    /// <summary>
    /// Returns a label of the game.
    /// </summary>
    internal string GetLabel(GameLabel label)
        => Lookup(label.TranslationKey, label.TranslationKey);

    /// <summary>
    /// Returns the name of a climate resource such as <c>::/climates/tropical/tropical.clima</c>, otherwise <see langword="null"/>.
    /// </summary>
    internal string? GetClimateName(string? climateResource)
    {
        if (climateResource?.StartsWith(TranslationConstants.ClimatePrefix, StringComparison.Ordinal) is not true)
            return null;

        string rest = climateResource[TranslationConstants.ClimatePrefix.Length..];
        int slash = rest.IndexOf('/', StringComparison.Ordinal);

        if (slash is <= 0)
            return null;

        string folder = rest[..slash];
        string file = rest[(slash + 1)..];

        if (!string.Equals(file, folder + TranslationConstants.ClimateSuffix, StringComparison.Ordinal))
            return null;

        return folder switch
        {
            "temperate" => Lookup("Temperate", "Temperate"),
            "subarctic" => Lookup("Subarctic", "Subarctic"),
            "tropical" => Lookup("Tropical", "Tropical"),
            "dry" => Lookup("Dry", "Dry"),
            _ => null
        };
    }

    private string Lookup(string key, string fallback)
        => ((translations is not null) && translations.TryGetValue(key, out string? text) && (text.Length is > 0)) ? text : fallback;
}
