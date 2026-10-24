using System.Globalization;

using Tf3Drpc.Core.Presence;
using Tf3Drpc.Core.Status;
using Tf3Drpc.Core.Translation;

using Xunit;

namespace Tf3Drpc.Core.Tests.Presence;

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

    [Fact]
    public void Placeholders_AreReplaced()
        => Assert.Equal("Mechanic · Tycoon", Render("{rank} · {mode}"));

    [Fact]
    public void PartWithMissingValue_IsDroppedWithItsSeparator()
    {
        ModStatus status = Full() with { IsTycoon = null };

        Assert.Equal("Mechanic", Render("{rank} · {mode}", status));
        Assert.Equal("Mechanic", Render("{mode} · {rank}", status));
        Assert.Equal("Mechanic", Render("{rank} · {mode} · {mode}", status));
    }

    [Fact]
    public void PartWithUnknownPlaceholder_IsDropped()
    {
        Assert.Equal("Mechanic", Render("{rank} · {nothing}"));
        Assert.Equal("Mechanic", Render("{rank} · {Rank}"));
        Assert.Equal("Mechanic", Render("{rank} · {}"));
    }

    [Fact]
    public void PartWithOneMissingPlaceholder_IsDroppedAsAWhole()
    {
        ModStatus status = Full() with { Month = null };

        Assert.Equal("Mechanic", Render("{rank} · Year {year} {month}", status));
    }

    [Fact]
    public void LiteralOnlyPart_IsKept()
        => Assert.Equal("Hello · Mechanic", Render("Hello · {rank}"));

    [Fact]
    public void LiteralStaysAsTyped()
        => Assert.Equal("Rank: Mechanic!", Render("Rank: {rank}!"));

    [Fact]
    public void TycoonRank_DropsTheModeNextToIt()
    {
        ModStatus status = Full() with { Rank = 15 };

        Assert.Equal("Tycoon", Render("{rank} · {mode}", status));
        Assert.Equal("Tycoon · September 1905", Render("{mode} · {rank} · {date}", status));
    }

    [Fact]
    public void TycoonMode_IsKeptWhenTheRankPartIsDropped()
    {
        ModStatus status = Full() with { Rank = 15, CompanyNumberOfLines = null };

        Assert.Equal("Tycoon", Render("{rank} {lines} · {mode}", status));
        Assert.Equal("Tycoon", Render("{rank} {typo} · {mode}", status));
    }

    [Fact]
    public void TycoonPartWithRankAndMode_IsKeptAsWritten()
        => Assert.Equal("Tycoon (Tycoon)", Render("{rank} ({mode})", Full() with { Rank = 15 }));

    [Fact]
    public void TycoonMode_IsKeptWithoutTheRank()
        => Assert.Equal("Tycoon · 1905", Render("{mode} · {year}", Full() with { Rank = 15 }));

    [Fact]
    public void OtherMode_IsKeptNextToTheTycoonRank()
        => Assert.Equal("Tycoon · Sandbox Mode", Render("{rank} · {mode}", Full() with { Rank = 15, IsSandbox = true }));

    [Fact]
    public void EmptyTemplate_GivesNull()
    {
        Assert.Null(Render(string.Empty));
        Assert.Null(Render("   "));
    }

    [Fact]
    public void AllPartsDropped_GivesNull()
    {
        ModStatus status = new();

        Assert.Null(Render("{rank} · {mode}", status));
        Assert.Null(Render("{nothing}", status));
    }

    [Fact]
    public void PartsAreTrimmed()
        => Assert.Equal("Hello · Mechanic", Render("   Hello   ·\t{rank}  "));

    [Fact]
    public void SeparatorWithoutSpacesWorks()
        => Assert.Equal("Hello · Mechanic", Render("Hello·{rank}"));

    [Fact]
    public void AdjacentLeadingAndTrailingSeparators_AreIgnored()
    {
        Assert.Equal("Mechanic", Render("·{rank}"));
        Assert.Equal("Mechanic", Render("{rank}·"));
        Assert.Equal("Mechanic · Tycoon", Render("{rank} · · {mode}"));
        Assert.Equal("Mechanic", Render("···{rank}···"));
    }

    [Fact]
    public void PlaceholderAtThePartEdges_Works()
    {
        Assert.Equal("Mechanic", Render("{rank}"));
        Assert.Equal("MechanicTycoon", Render("{rank}{mode}"));
        Assert.Equal("<Mechanic>", Render("<{rank}>"));
    }

    [Theory]
    [InlineData("{rank", "{rank")]
    [InlineData("rank}", "rank}")]
    [InlineData("{ {rank}", "{ Mechanic")]
    [InlineData("a { b", "a { b")]
    [InlineData("{rank} {", "Mechanic {")]
    public void UnclosedBrace_IsLiteralText(string template, string expected)
        => Assert.Equal(expected, Render(template));

    [Fact]
    public void PartThatRendersEmpty_IsDropped()
    {
        ModStatus status = Full() with { SaveName = "  " };

        Assert.Equal("Mechanic", Render("{rank} · {saveName}", status));
    }

    #endregion Engine

    #region Values

    [Fact]
    public void Rank_UsesTheGameTexts()
    {
        GameTexts german = new(new System.Collections.Generic.Dictionary<string, string>(System.StringComparer.Ordinal) { ["Mechanic"] = "Mechaniker" });

        Assert.Equal("Mechaniker", Render("{rank}", texts: german));
        Assert.Null(Render("{rank}", Full() with { Rank = 0 }));
        Assert.Null(Render("{rank}", Full() with { Rank = 16 }));
        Assert.Null(Render("{rank}", Full() with { Rank = null }));
    }

    [Theory]
    [InlineData(false, false, false, null)]
    [InlineData(false, false, true, "Tycoon")]
    [InlineData(false, true, true, "Sandbox Mode")]
    [InlineData(true, true, true, "Map Editor")]
    public void Mode_FollowsThePriority(bool mapEditor, bool sandbox, bool tycoon, string? expected)
    {
        ModStatus status = Full() with { IsMapEditor = mapEditor, IsSandbox = sandbox, IsTycoon = tycoon };

        Assert.Equal(expected, Render("{mode}", status));
    }

    [Theory]
    [InlineData("en-US", "September 1905")]
    [InlineData("de-DE", "September 1905")]
    [InlineData("ja-JP", "1905年9月")]
    public void Date_UsesTheYearMonthPattern(string culture, string expected)
        => Assert.Equal(expected, Render("{date}", culture: CultureInfo.GetCultureInfo(culture)));

    [Fact]
    public void Month_UsesTheMonthNameOfTheCulture()
    {
        Assert.Equal("September", Render("{month}"));
        Assert.Equal("März", Render("{month}", Full() with { Month = 3 }, German));
        Assert.Null(Render("{month}", Full() with { Month = 13 }));
        Assert.Null(Render("{month}", Full() with { Month = 0 }));
        Assert.Null(Render("{month}", Full() with { Month = null }));
    }

    [Fact]
    public void Year_HasNoGroupSeparator()
    {
        Assert.Equal("1905", Render("{year}"));
        Assert.Equal("1905", Render("{year}", culture: German));
        Assert.Null(Render("{year}", Full() with { Year = null }));
        Assert.Null(Render("{year}", Full() with { Year = 0 }));
        Assert.Null(Render("{year}", Full() with { Year = 10000 }));
    }

    [Fact]
    public void Climate_UsesTheGameTexts()
    {
        GameTexts german = new(new System.Collections.Generic.Dictionary<string, string>(System.StringComparer.Ordinal) { ["Tropical"] = "Tropisch" });

        Assert.Equal("Tropical", Render("{climate}"));
        Assert.Equal("Tropisch", Render("{climate}", texts: german));
        Assert.Null(Render("{climate}", Full() with { Climate = "::/climates/other/other.clima" }));
        Assert.Null(Render("{climate}", Full() with { Climate = null }));
    }

    [Fact]
    public void Counts_UseTheGroupSeparatorOfTheCulture()
    {
        Assert.Equal("1,234", Render("{lines}"));
        Assert.Equal("1.234", Render("{lines}", culture: German));
        Assert.Equal("74", Render("{stations}"));
        Assert.Equal("10", Render("{trains}"));
        Assert.Equal("1,234,567", Render("{passengers}"));
        Assert.Equal("1.234.567", Render("{passengers}", culture: German));
        Assert.Equal("89", Render("{cargo}"));
    }

    [Fact]
    public void MissingCounts_DropThePart()
    {
        ModStatus status = new();

        Assert.Null(Render("{lines}", status));
        Assert.Null(Render("{stations}", status));
        Assert.Null(Render("{trains}", status));
        Assert.Null(Render("{vehicles}", status));
        Assert.Null(Render("{passengers}", status));
        Assert.Null(Render("{cargo}", status));
    }

    [Fact]
    public void Vehicles_SumsAllFiveKinds()
    {
        Assert.Equal("42", Render("{vehicles}"));
        Assert.Equal("7", Render("{vehicles}", new ModStatus() { CompanyRailVehicles = 7 }));
        Assert.Equal("0", Render("{vehicles}", new ModStatus() { CompanyShips = 0 }));
        Assert.Equal("1,500", Render("{vehicles}", new ModStatus() { CompanyRoadVehicles = 1000, CompanyTrams = 500 }));
    }

    [Fact]
    public void CompanyValue_IsAssetsPlusBalanceMinusDebt()
    {
        Assert.Equal("$15.6M", Render("{companyValue}"));
        Assert.Equal("$10M", Render("{companyValue}", new ModStatus() { CompanyTotalAssets = 10_000_000 }));
        Assert.Equal("$12M", Render("{companyValue}", new ModStatus() { CompanyTotalAssets = 10_000_000, Balance = 2_000_000 }));
        Assert.Equal("$8M", Render("{companyValue}", new ModStatus() { CompanyTotalAssets = 10_000_000, CompanyDebt = 2_000_000 }));
        Assert.Equal("-$1.2M", Render("{companyValue}", new ModStatus() { CompanyTotalAssets = 1_000_000, CompanyDebt = 2_200_000 }));
        Assert.Null(Render("{companyValue}", new ModStatus() { Balance = 5, CompanyDebt = 1 }));
    }

    [Fact]
    public void Balance_IsHiddenWithInfiniteMoney()
    {
        Assert.Equal("$7.6M", Render("{balance}"));
        Assert.Equal("$7,6M", Render("{balance}", culture: German));
        Assert.Null(Render("{balance}", Full() with { InfiniteMoney = true }));
        Assert.Equal("$7.6M", Render("{balance}", Full() with { InfiniteMoney = false }));
        Assert.Null(Render("{balance}", Full() with { Balance = null }));
    }

    [Fact]
    public void SaveName_IsShownAsIs()
    {
        Assert.Equal("My Save", Render("{saveName}"));
        Assert.Null(Render("{saveName}", Full() with { SaveName = null }));
    }

    #endregion Values
}
