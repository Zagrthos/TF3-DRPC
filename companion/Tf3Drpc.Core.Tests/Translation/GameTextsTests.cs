using System.Collections.Generic;

using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Translation;

using Xunit;

namespace Tf3Drpc.Core.Tests.Translation;

/// <summary>
/// Tests of <see cref="GameTexts"/>, which looks up the game's own rank, mission, mode, label and climate names with an English fallback.
/// </summary>
public sealed class GameTextsTests
{
    private static readonly string[] EnglishRanks =
    [
        "Junior", "Mechanic", "Engineer", "Coordinator", "Expert", "Team Leader", "Supervisor", "Manager",
        "Director", "Senior Director", "CEO", "Chairperson", "Vice President", "President", "Tycoon"
    ];

    private static readonly string[] EnglishMissions =
    [
        "Saving Mardi Gras", "Alpine Crossing", "Desert Adventure", "Biggest Festival Ever",
        "Oil for the People", "Island Expansion", "Big City Problems", "Final Countdown"
    ];

    private static GameTexts German()
        => new(new Dictionary<string, string>(System.StringComparer.Ordinal)
        {
            ["Mechanic"] = "Mechaniker",
            ["MOD_SANDBOX_NAME"] = "Sandkastenmodus",
            ["MISSION_01_NAME"] = "Mardi Gras retten",
            ["Loading..."] = "Laden...",
            ["Paused"] = "Pausiert",
            ["Campaign"] = "Kampagne",
            ["Tropical"] = "Tropisch"
        });

    /// <summary>
    /// Without a translation file, ranks 1 to 15 have their English names, from "Junior" to "Tycoon".
    /// </summary>
    [Fact]
    public void WithoutFileEveryRankIsEnglish()
    {
        for (int rank = 1; rank is <= 15; rank++)
        {
            Assert.Equal(EnglishRanks[rank - 1], GameTexts.English.GetRankName(rank));
        }
    }

    /// <summary>
    /// A rank outside 1 to 15 (0, 16 or -1) has no name, in both the English and a translated set of texts.
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(16)]
    [InlineData(-1)]
    public void RankOutsideTheRangeIsNull(int rank)
    {
        Assert.Null(GameTexts.English.GetRankName(rank));
        Assert.Null(German().GetRankName(rank));
    }

    /// <summary>
    /// Without a translation file, missions 1 to 8 have their English names, from "Saving Mardi Gras" to "Final Countdown".
    /// </summary>
    [Fact]
    public void WithoutFileEveryMissionIsEnglish()
    {
        for (int mission = 1; mission is <= 8; mission++)
        {
            Assert.Equal(EnglishMissions[mission - 1], GameTexts.English.GetMissionName(mission));
        }
    }

    /// <summary>
    /// A mission number outside 1 to 8 (0 or 9) has no name.
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(9)]
    public void MissionOutsideTheRangeIsNull(int mission)
        => Assert.Null(GameTexts.English.GetMissionName(mission));

    /// <summary>
    /// Without a translation file, the three game modes and the campaign, paused and loading labels have their English texts.
    /// </summary>
    [Fact]
    public void WithoutFileModesAndLabelsAreEnglish()
    {
        Assert.Equal("Tycoon", GameTexts.English.GetModeName(GameMode.Tycoon));
        Assert.Equal("Sandbox Mode", GameTexts.English.GetModeName(GameMode.Sandbox));
        Assert.Equal("Map Editor", GameTexts.English.GetModeName(GameMode.MapEditor));
        Assert.Equal("Campaign", GameTexts.English.GetLabel(GameLabel.Campaign));
        Assert.Equal("Paused", GameTexts.English.GetLabel(GameLabel.Paused));
        Assert.Equal("Loading...", GameTexts.English.GetLabel(GameLabel.Loading));
    }

    /// <summary>
    /// Keys present in the translation are used and keys it lacks fall back to English, so rank 2 is "Mechaniker" while rank 3 stays "Engineer".
    /// </summary>
    [Fact]
    public void TranslatedKeysAreUsedAndMissingKeysFallBack()
    {
        GameTexts texts = German();

        Assert.Equal("Mechaniker", texts.GetRankName(2));
        Assert.Equal("Engineer", texts.GetRankName(3));
        Assert.Equal("Sandkastenmodus", texts.GetModeName(GameMode.Sandbox));
        Assert.Equal("Tycoon", texts.GetModeName(GameMode.Tycoon));
        Assert.Equal("Mardi Gras retten", texts.GetMissionName(1));
        Assert.Equal("Alpine Crossing", texts.GetMissionName(2));
        Assert.Equal("Laden...", texts.GetLabel(GameLabel.Loading));
        Assert.Equal("Pausiert", texts.GetLabel(GameLabel.Paused));
        Assert.Equal("Kampagne", texts.GetLabel(GameLabel.Campaign));
    }

    /// <summary>
    /// A translation whose value is empty is ignored and the English rank name is used.
    /// </summary>
    [Fact]
    public void EmptyTranslationFallsBack()
    {
        GameTexts texts = new(new Dictionary<string, string>(System.StringComparer.Ordinal) { ["Mechanic"] = string.Empty });

        Assert.Equal("Mechanic", texts.GetRankName(2));
    }

    /// <summary>
    /// A climate resource name gives its display name for the four known climates, and gives null for an unknown climate, a wrong file name, a mission climate, an empty string or <see langword="null"/>.
    /// </summary>
    [Theory]
    [InlineData("::/climates/tropical/tropical.clima", "Tropical")]
    [InlineData("::/climates/temperate/temperate.clima", "Temperate")]
    [InlineData("::/climates/subarctic/subarctic.clima", "Subarctic")]
    [InlineData("::/climates/dry/dry.clima", "Dry")]
    [InlineData("::/climates/moon/moon.clima", null)]
    [InlineData("::/climates/tropical/other.clima", null)]
    [InlineData("urbangames::/clima/mission1/mission1_tropical.clima", null)]
    [InlineData("", null)]
    [InlineData(null, null)]
    public void ClimateNames(string? resource, string? expected)
        => Assert.Equal(expected, GameTexts.English.GetClimateName(resource));

    /// <summary>
    /// A climate name is translated when the texts contain a translation, so the tropical climate is "Tropisch" in German.
    /// </summary>
    [Fact]
    public void ClimateNameIsTranslated()
        => Assert.Equal("Tropisch", German().GetClimateName("::/climates/tropical/tropical.clima"));
}
