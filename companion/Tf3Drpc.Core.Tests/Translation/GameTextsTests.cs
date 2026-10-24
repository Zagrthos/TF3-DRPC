using System.Collections.Generic;

using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Translation;

using Xunit;

namespace Tf3Drpc.Core.Tests.Translation;

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

    [Fact]
    public void WithoutFile_EveryRankIsEnglish()
    {
        for (int rank = 1; rank is <= 15; rank++)
        {
            Assert.Equal(EnglishRanks[rank - 1], GameTexts.English.GetRankName(rank));
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(16)]
    [InlineData(-1)]
    public void RankOutsideTheRange_IsNull(int rank)
    {
        Assert.Null(GameTexts.English.GetRankName(rank));
        Assert.Null(German().GetRankName(rank));
    }

    [Fact]
    public void WithoutFile_EveryMissionIsEnglish()
    {
        for (int mission = 1; mission is <= 8; mission++)
        {
            Assert.Equal(EnglishMissions[mission - 1], GameTexts.English.GetMissionName(mission));
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(9)]
    public void MissionOutsideTheRange_IsNull(int mission)
        => Assert.Null(GameTexts.English.GetMissionName(mission));

    [Fact]
    public void WithoutFile_ModesAndLabelsAreEnglish()
    {
        Assert.Equal("Tycoon", GameTexts.English.GetModeName(GameMode.Tycoon));
        Assert.Equal("Sandbox Mode", GameTexts.English.GetModeName(GameMode.Sandbox));
        Assert.Equal("Map Editor", GameTexts.English.GetModeName(GameMode.MapEditor));
        Assert.Equal("Campaign", GameTexts.English.GetLabel(GameLabel.Campaign));
        Assert.Equal("Paused", GameTexts.English.GetLabel(GameLabel.Paused));
        Assert.Equal("Loading...", GameTexts.English.GetLabel(GameLabel.Loading));
    }

    [Fact]
    public void TranslatedKeys_AreUsedAndMissingKeysFallBack()
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

    [Fact]
    public void EmptyTranslation_FallsBack()
    {
        GameTexts texts = new(new Dictionary<string, string>(System.StringComparer.Ordinal) { ["Mechanic"] = string.Empty });

        Assert.Equal("Mechanic", texts.GetRankName(2));
    }

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

    [Fact]
    public void ClimateName_IsTranslated()
        => Assert.Equal("Tropisch", German().GetClimateName("::/climates/tropical/tropical.clima"));
}
