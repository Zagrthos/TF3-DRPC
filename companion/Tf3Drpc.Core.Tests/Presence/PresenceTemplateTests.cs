using System.Globalization;

using Tf3Drpc.Core.Presence;
using Tf3Drpc.Core.Status;
using Tf3Drpc.Core.Translation;

using Xunit;

namespace Tf3Drpc.Core.Tests.Presence;

/// <summary>
/// Tests of how <see cref="Tf3Drpc.Core.Presence.PresenceTemplate"/> renders a template into presence text, covering the template engine and the individual placeholder values.
/// </summary>
public sealed class PresenceTemplateTests
{
    private static readonly CultureInfo English = CultureInfo.GetCultureInfo("en-US");
    private static readonly CultureInfo German = CultureInfo.GetCultureInfo("de-DE");

    private static ModStatus Full()
        => new()
        {
            Rank = 2,
            IsTycoon = true,
            Year = 1905,
            Month = 9,
            Climate = "::/climates/tropical/tropical.clima",
            CompanyNumberOfLines = 1234,
            CompanyTotalStations = 74,
            CompanyRailVehicles = 10,
            CompanyRoadVehicles = 20,
            CompanyAircrafts = 3,
            CompanyShips = 4,
            CompanyTrams = 5,
            PassengersTransported = 1234567,
            CargoTransported = 89,
            CompanyTotalAssets = 10_000_000,
            CompanyDebt = 2_000_000,
            Balance = 7_600_000,
            SaveName = "My Save"
        };

    private static string? Render(string template, ModStatus? status = null, CultureInfo? culture = null, GameTexts? texts = null)
        => PresenceTemplate.Render(template, status ?? Full(), culture ?? English, texts ?? GameTexts.English);

    #region Engine

    /// <summary>
    /// Placeholders in a template are replaced by the values of the status.
    /// </summary>
    [Fact]
    public void PlaceholdersAreReplaced()
        => Assert.Equal("Mechanic · Tycoon", Render("{rank} · {mode}"));

    /// <summary>
    /// A part whose placeholder has no value is dropped together with its separator, wherever it stands in the template.
    /// </summary>
    [Fact]
    public void PartWithMissingValueIsDroppedWithItsSeparator()
    {
        ModStatus status = Full() with { IsTycoon = null };

        Assert.Equal("Mechanic", Render("{rank} · {mode}", status));
        Assert.Equal("Mechanic", Render("{mode} · {rank}", status));
        Assert.Equal("Mechanic", Render("{rank} · {mode} · {mode}", status));
    }

    /// <summary>
    /// A part that contains an unknown placeholder (an unrecognised name, a name with different capitalisation or an empty one) is dropped.
    /// </summary>
    [Fact]
    public void PartWithUnknownPlaceholderIsDropped()
    {
        Assert.Equal("Mechanic", Render("{rank} · {nothing}"));
        Assert.Equal("Mechanic", Render("{rank} · {Rank}"));
        Assert.Equal("Mechanic", Render("{rank} · {}"));
    }

    /// <summary>
    /// A part with several placeholders is dropped as a whole when one of them has no value.
    /// </summary>
    [Fact]
    public void PartWithOneMissingPlaceholderIsDroppedAsAWhole()
    {
        ModStatus status = Full() with { Month = null };

        Assert.Equal("Mechanic", Render("{rank} · Year {year} {month}", status));
    }

    /// <summary>
    /// A part that consists only of literal text is kept.
    /// </summary>
    [Fact]
    public void LiteralOnlyPartIsKept()
        => Assert.Equal("Hello · Mechanic", Render("Hello · {rank}"));

    /// <summary>
    /// Literal text around a placeholder is kept exactly as typed.
    /// </summary>
    [Fact]
    public void LiteralStaysAsTyped()
        => Assert.Equal("Rank: Mechanic!", Render("Rank: {rank}!"));

    /// <summary>
    /// With the tycoon rank, a mode part that would only repeat the rank text is dropped, so the output shows "Tycoon" once.
    /// </summary>
    [Fact]
    public void TycoonRankDropsTheModeNextToIt()
    {
        ModStatus status = Full() with { Rank = 15 };

        Assert.Equal("Tycoon", Render("{rank} · {mode}", status));
        Assert.Equal("Tycoon · September 1905", Render("{mode} · {rank} · {date}", status));
    }

    /// <summary>
    /// The tycoon mode is still shown when the part with the tycoon rank is dropped, either for a missing value or for an unknown placeholder.
    /// </summary>
    [Fact]
    public void TycoonModeIsKeptWhenTheRankPartIsDropped()
    {
        ModStatus status = Full() with { Rank = 15, CompanyNumberOfLines = null };

        Assert.Equal("Tycoon", Render("{rank} {lines} · {mode}", status));
        Assert.Equal("Tycoon", Render("{rank} {typo} · {mode}", status));
    }

    /// <summary>
    /// A single part that holds both the tycoon rank and the mode is kept as written, so "Tycoon" appears twice.
    /// </summary>
    [Fact]
    public void TycoonPartWithRankAndModeIsKeptAsWritten()
        => Assert.Equal("Tycoon (Tycoon)", Render("{rank} ({mode})", Full() with { Rank = 15 }));

    /// <summary>
    /// The tycoon mode is shown when the template has no rank placeholder at all.
    /// </summary>
    [Fact]
    public void TycoonModeIsKeptWithoutTheRank()
        => Assert.Equal("Tycoon · 1905", Render("{mode} · {year}", Full() with { Rank = 15 }));

    /// <summary>
    /// A mode other than tycoon, such as sandbox, is shown next to the tycoon rank.
    /// </summary>
    [Fact]
    public void OtherModeIsKeptNextToTheTycoonRank()
        => Assert.Equal("Tycoon · Sandbox Mode", Render("{rank} · {mode}", Full() with { Rank = 15, IsSandbox = true }));

    /// <summary>
    /// An empty or whitespace-only template renders to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void EmptyTemplateGivesNull()
    {
        Assert.Null(Render(string.Empty));
        Assert.Null(Render("   "));
    }

    /// <summary>
    /// A template whose parts are all dropped renders to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void AllPartsDroppedGivesNull()
    {
        ModStatus status = new();

        Assert.Null(Render("{rank} · {mode}", status));
        Assert.Null(Render("{nothing}", status));
    }

    /// <summary>
    /// Whitespace around each part is trimmed.
    /// </summary>
    [Fact]
    public void PartsAreTrimmed()
        => Assert.Equal("Hello · Mechanic", Render("   Hello   ·\t{rank}  "));

    /// <summary>
    /// The part separator works without surrounding spaces.
    /// </summary>
    [Fact]
    public void SeparatorWithoutSpacesWorks()
        => Assert.Equal("Hello · Mechanic", Render("Hello·{rank}"));

    /// <summary>
    /// Empty parts caused by leading, trailing or repeated separators are ignored.
    /// </summary>
    [Fact]
    public void AdjacentLeadingAndTrailingSeparatorsAreIgnored()
    {
        Assert.Equal("Mechanic", Render("·{rank}"));
        Assert.Equal("Mechanic", Render("{rank}·"));
        Assert.Equal("Mechanic · Tycoon", Render("{rank} · · {mode}"));
        Assert.Equal("Mechanic", Render("···{rank}···"));
    }

    /// <summary>
    /// A placeholder at the start or end of a part, or directly next to another placeholder or to literal characters, is replaced correctly.
    /// </summary>
    [Fact]
    public void PlaceholderAtThePartEdgesWorks()
    {
        Assert.Equal("Mechanic", Render("{rank}"));
        Assert.Equal("MechanicTycoon", Render("{rank}{mode}"));
        Assert.Equal("<Mechanic>", Render("<{rank}>"));
    }

    /// <summary>
    /// A brace that is not part of a complete placeholder is kept as literal text, across opening and closing braces in several positions.
    /// </summary>
    [Theory]
    [InlineData("{rank", "{rank")]
    [InlineData("rank}", "rank}")]
    [InlineData("{ {rank}", "{ Mechanic")]
    [InlineData("a { b", "a { b")]
    [InlineData("{rank} {", "Mechanic {")]
    public void UnclosedBraceIsLiteralText(string template, string expected)
        => Assert.Equal(expected, Render(template));

    /// <summary>
    /// A part whose rendered text is blank (a save name of spaces) is dropped.
    /// </summary>
    [Fact]
    public void PartThatRendersEmptyIsDropped()
    {
        ModStatus status = Full() with { SaveName = "  " };

        Assert.Equal("Mechanic", Render("{rank} · {saveName}", status));
    }

    #endregion Engine

    #region Values

    /// <summary>
    /// The rank placeholder renders the rank name through the game texts, and renders nothing for a rank outside 1 to 15 or without a rank.
    /// </summary>
    [Fact]
    public void RankUsesTheGameTexts()
    {
        GameTexts german = new(new System.Collections.Generic.Dictionary<string, string>(System.StringComparer.Ordinal) { ["Mechanic"] = "Mechaniker" });

        Assert.Equal("Mechaniker", Render("{rank}", texts: german));
        Assert.Null(Render("{rank}", Full() with { Rank = 0 }));
        Assert.Null(Render("{rank}", Full() with { Rank = 16 }));
        Assert.Null(Render("{rank}", Full() with { Rank = null }));
    }

    /// <summary>
    /// The mode placeholder renders the map editor over the sandbox mode over the tycoon mode, and nothing when none of them applies.
    /// </summary>
    [Theory]
    [InlineData(false, false, false, null)]
    [InlineData(false, false, true, "Tycoon")]
    [InlineData(false, true, true, "Sandbox Mode")]
    [InlineData(true, true, true, "Map Editor")]
    public void ModeFollowsThePriority(bool mapEditor, bool sandbox, bool tycoon, string? expected)
    {
        ModStatus status = Full() with { IsMapEditor = mapEditor, IsSandbox = sandbox, IsTycoon = tycoon };

        Assert.Equal(expected, Render("{mode}", status));
    }

    /// <summary>
    /// The date placeholder renders the month and year in the year-month pattern of the culture, for English, German and Japanese.
    /// </summary>
    [Theory]
    [InlineData("en-US", "September 1905")]
    [InlineData("de-DE", "September 1905")]
    [InlineData("ja-JP", "1905年9月")]
    public void DateUsesTheYearMonthPattern(string culture, string expected)
        => Assert.Equal(expected, Render("{date}", culture: CultureInfo.GetCultureInfo(culture)));

    /// <summary>
    /// The month placeholder renders the month name in the culture, and nothing for a month outside 1 to 12 or without a month.
    /// </summary>
    [Fact]
    public void MonthUsesTheMonthNameOfTheCulture()
    {
        Assert.Equal("September", Render("{month}"));
        Assert.Equal("März", Render("{month}", Full() with { Month = 3 }, German));
        Assert.Null(Render("{month}", Full() with { Month = 13 }));
        Assert.Null(Render("{month}", Full() with { Month = 0 }));
        Assert.Null(Render("{month}", Full() with { Month = null }));
    }

    /// <summary>
    /// The year placeholder renders the year without a group separator in any culture, and nothing for a year of 0, above 9999 or missing.
    /// </summary>
    [Fact]
    public void YearHasNoGroupSeparator()
    {
        Assert.Equal("1905", Render("{year}"));
        Assert.Equal("1905", Render("{year}", culture: German));
        Assert.Null(Render("{year}", Full() with { Year = null }));
        Assert.Null(Render("{year}", Full() with { Year = 0 }));
        Assert.Null(Render("{year}", Full() with { Year = 10000 }));
    }

    /// <summary>
    /// The climate placeholder renders the climate name through the game texts, and nothing for an unknown climate or without a climate.
    /// </summary>
    [Fact]
    public void ClimateUsesTheGameTexts()
    {
        GameTexts german = new(new System.Collections.Generic.Dictionary<string, string>(System.StringComparer.Ordinal) { ["Tropical"] = "Tropisch" });

        Assert.Equal("Tropical", Render("{climate}"));
        Assert.Equal("Tropisch", Render("{climate}", texts: german));
        Assert.Null(Render("{climate}", Full() with { Climate = "::/climates/other/other.clima" }));
        Assert.Null(Render("{climate}", Full() with { Climate = null }));
    }

    /// <summary>
    /// Counts such as lines and passengers are formatted with the group separator of the culture.
    /// </summary>
    [Fact]
    public void CountsUseTheGroupSeparatorOfTheCulture()
    {
        Assert.Equal("1,234", Render("{lines}"));
        Assert.Equal("1.234", Render("{lines}", culture: German));
        Assert.Equal("74", Render("{stations}"));
        Assert.Equal("10", Render("{trains}"));
        Assert.Equal("1,234,567", Render("{passengers}"));
        Assert.Equal("1.234.567", Render("{passengers}", culture: German));
        Assert.Equal("89", Render("{cargo}"));
    }

    /// <summary>
    /// A placeholder for a count renders nothing when the status has no value for it.
    /// </summary>
    [Fact]
    public void MissingCountsDropThePart()
    {
        ModStatus status = new();

        Assert.Null(Render("{lines}", status));
        Assert.Null(Render("{stations}", status));
        Assert.Null(Render("{trains}", status));
        Assert.Null(Render("{vehicles}", status));
        Assert.Null(Render("{passengers}", status));
        Assert.Null(Render("{cargo}", status));
    }

    /// <summary>
    /// The vehicles placeholder renders the sum of the rail, road, air, ship and tram vehicles, treating missing kinds as zero.
    /// </summary>
    [Fact]
    public void VehiclesSumsAllFiveKinds()
    {
        Assert.Equal("42", Render("{vehicles}"));
        Assert.Equal("7", Render("{vehicles}", new ModStatus() { CompanyRailVehicles = 7 }));
        Assert.Equal("0", Render("{vehicles}", new ModStatus() { CompanyShips = 0 }));
        Assert.Equal("1,500", Render("{vehicles}", new ModStatus() { CompanyRoadVehicles = 1000, CompanyTrams = 500 }));
    }

    /// <summary>
    /// The company value placeholder renders the total assets plus the balance minus the debt in a short money format, and nothing when there are no assets.
    /// </summary>
    [Fact]
    public void CompanyValueIsAssetsPlusBalanceMinusDebt()
    {
        Assert.Equal("$15.6M", Render("{companyValue}"));
        Assert.Equal("$10M", Render("{companyValue}", new ModStatus() { CompanyTotalAssets = 10_000_000 }));
        Assert.Equal("$12M", Render("{companyValue}", new ModStatus() { CompanyTotalAssets = 10_000_000, Balance = 2_000_000 }));
        Assert.Equal("$8M", Render("{companyValue}", new ModStatus() { CompanyTotalAssets = 10_000_000, CompanyDebt = 2_000_000 }));
        Assert.Equal("-$1.2M", Render("{companyValue}", new ModStatus() { CompanyTotalAssets = 1_000_000, CompanyDebt = 2_200_000 }));
        Assert.Null(Render("{companyValue}", new ModStatus() { Balance = 5, CompanyDebt = 1 }));
    }

    /// <summary>
    /// The balance placeholder renders the balance in a short money format using the culture's decimal separator, and nothing with infinite money or without a balance.
    /// </summary>
    [Fact]
    public void BalanceIsHiddenWithInfiniteMoney()
    {
        Assert.Equal("$7.6M", Render("{balance}"));
        Assert.Equal("$7,6M", Render("{balance}", culture: German));
        Assert.Null(Render("{balance}", Full() with { InfiniteMoney = true }));
        Assert.Equal("$7.6M", Render("{balance}", Full() with { InfiniteMoney = false }));
        Assert.Null(Render("{balance}", Full() with { Balance = null }));
    }

    /// <summary>
    /// The save name placeholder renders the save name unchanged, and nothing when there is none.
    /// </summary>
    [Fact]
    public void SaveNameIsShownAsIs()
    {
        Assert.Equal("My Save", Render("{saveName}"));
        Assert.Null(Render("{saveName}", Full() with { SaveName = null }));
    }

    #endregion Values
}
