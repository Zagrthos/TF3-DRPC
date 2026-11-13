using System;
using System.Collections.Generic;
using System.Globalization;

using Tf3Drpc.Core.Constants;
using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.GameLog;
using Tf3Drpc.Core.Presence;
using Tf3Drpc.Core.Status;
using Tf3Drpc.Core.Structs;
using Tf3Drpc.Core.Translation;

using Xunit;

namespace Tf3Drpc.Core.Tests.Presence;

/// <summary>
/// Tests of the presence presets, which choose the details and state layout that <see cref="PresenceBuilder"/> builds for a playing game, together with <see cref="PresetCatalog"/>.
/// </summary>
public sealed class PresencePresetTests
{
    private static readonly CultureInfo English = CultureInfo.GetCultureInfo("en-US");
    private static readonly CultureInfo German = CultureInfo.GetCultureInfo("de-DE");
    private static readonly DateTimeOffset Timer = new(2026, 10, 13, 21, 15, 24, TimeSpan.Zero);

    private static readonly GameTexts GermanTexts = new(new Dictionary<string, string>(StringComparer.Ordinal)
    {
        ["Lines"] = "Linien",
        ["Vehicles"] = "Fahrzeuge",
        ["Stations"] = "Stationen",
        ["Year"] = "Jahr",
        ["Tropical"] = "Tropisch",
        ["Mechanic"] = "Mechaniker",
        ["Paused"] = "Pausiert"
    });

    private static ModStatus Status(long? speedup = 1)
        => new()
        {
            Version = 1,
            Rank = 2,
            IsTycoon = true,
            Year = 1905,
            Month = 9,
            Speedup = speedup,
            Climate = "::/climates/tropical/tropical.clima",
            CompanyNumberOfLines = 44,
            CompanyRoadVehicles = 100,
            CompanyRailVehicles = 18,
            CompanyTotalStations = 74
        };

    private static PresenceContentStruct Build(ModStatus status, PresencePreset preset, CultureInfo? culture = null, GameTexts? texts = null, long step = 0)
    {
        GameStateSnapshotStruct snapshot = new() { State = GameState.Playing, TimerStart = Timer, Status = status, IsPaused = status.Speedup is 0 };
        bool built = PresenceBuilder.TryBuild(snapshot, culture ?? English, texts ?? GameTexts.English, preset, step, out PresenceContentStruct content);

        Assert.True(built);

        return content;
    }

    /// <summary>
    /// The short <see cref="PresenceBuilder"/> overload without a preset builds the career layout, here "Mechanic · Tycoon" above "September 1905".
    /// </summary>
    [Fact]
    public void CareerIsTheDefaultOfTheShortOverload()
    {
        GameStateSnapshotStruct snapshot = new() { State = GameState.Playing, TimerStart = Timer, Status = Status() };

        PresenceBuilder.TryBuild(snapshot, English, GameTexts.English, out PresenceContentStruct shortOverload);

        Assert.Equal(Build(Status(), PresencePreset.Career), shortOverload);
        Assert.Equal("Mechanic · Tycoon", shortOverload.Details);
        Assert.Equal("September 1905", shortOverload.State);
    }

    /// <summary>
    /// The network preset shows lines and vehicles as details and stations and the year as the state line, localised for English and German.
    /// </summary>
    [Theory]
    [InlineData("en-US", "Lines 44 · Vehicles 118", "Stations 74 · 1905")]
    [InlineData("de-DE", "Linien 44 · Fahrzeuge 118", "Stationen 74 · 1905")]
    public void NetworkShowsLinesVehiclesStationsAndYear(string culture, string details, string state)
    {
        GameTexts texts = (culture is "de-DE") ? GermanTexts : GameTexts.English;

        PresenceContentStruct content = Build(Status(), PresencePreset.Network, CultureInfo.GetCultureInfo(culture), texts);

        Assert.Equal(details, content.Details);
        Assert.Equal(state, content.State);
        Assert.Equal(Timer, content.StartTimestamp);
    }

    /// <summary>
    /// The era preset shows the year and climate as details and the rank name as the state line, localised for English and German.
    /// </summary>
    [Theory]
    [InlineData("en-US", "Year 1905 · Tropical", "Mechanic")]
    [InlineData("de-DE", "Jahr 1905 · Tropisch", "Mechaniker")]
    public void EraShowsYearClimateAndRank(string culture, string details, string state)
    {
        GameTexts texts = (culture is "de-DE") ? GermanTexts : GameTexts.English;

        PresenceContentStruct content = Build(Status(), PresencePreset.Era, CultureInfo.GetCultureInfo(culture), texts);

        Assert.Equal(details, content.Details);
        Assert.Equal(state, content.State);
    }

    /// <summary>
    /// The minimal preset shows "In game" followed by the year as details, with no state line, in English and German.
    /// </summary>
    [Theory]
    [InlineData("en-US", "In game · 1905")]
    [InlineData("de-DE", "Im Spiel · 1905")]
    public void MinimalShowsInGameAndTheYearWithoutState(string culture, string details)
    {
        PresenceContentStruct content = Build(Status(), PresencePreset.Minimal, CultureInfo.GetCultureInfo(culture));

        Assert.Equal(details, content.Details);
        Assert.Null(content.State);
    }

    /// <summary>
    /// The minimal preset without a year shows only "In game".
    /// </summary>
    [Fact]
    public void MinimalWithoutYearShowsOnlyInGame()
        => Assert.Equal("In game", Build(Status() with { Year = null }, PresencePreset.Minimal).Details);

    /// <summary>
    /// The alternating preset cycles through the career, network and era layouts by step modulo three, and a negative step (-1) wraps to the era layout.
    /// </summary>
    [Theory]
    [InlineData(0, "Mechanic · Tycoon", "September 1905")]
    [InlineData(1, "Lines 44 · Vehicles 118", "Stations 74 · 1905")]
    [InlineData(2, "Year 1905 · Tropical", "Mechanic")]
    [InlineData(3, "Mechanic · Tycoon", "September 1905")]
    [InlineData(4, "Lines 44 · Vehicles 118", "Stations 74 · 1905")]
    [InlineData(5, "Year 1905 · Tropical", "Mechanic")]
    [InlineData(-1, "Year 1905 · Tropical", "Mechanic")]
    public void AlternatingCyclesByStep(long step, string details, string state)
    {
        PresenceContentStruct content = Build(Status(), PresencePreset.Alternating, step: step);

        Assert.Equal(details, content.Details);
        Assert.Equal(state, content.State);
    }

    /// <summary>
    /// The career, network, era and minimal presets build the same content whatever the step, which only matters for the alternating preset.
    /// </summary>
    [Theory]
    [InlineData(PresencePreset.Career, 0)]
    [InlineData(PresencePreset.Network, 1)]
    [InlineData(PresencePreset.Era, 2)]
    [InlineData(PresencePreset.Minimal, 2)]
    internal void FixedPresetsIgnoreTheStep(PresencePreset preset, long step)
        => Assert.Equal(Build(Status(), preset), Build(Status(), preset, step: step));

    /// <summary>
    /// The alternating interval is 45 seconds.
    /// </summary>
    [Fact]
    public void AlternatingIntervalIsFortyFiveSeconds()
        => Assert.Equal(TimeSpan.FromSeconds(45), PresenceConstants.AlternatingInterval);

    /// <summary>
    /// A paused game appends the "Paused" label to the state line of every preset, and the minimal preset shows it alone.
    /// </summary>
    [Theory]
    [InlineData(PresencePreset.Career, "September 1905 · Paused")]
    [InlineData(PresencePreset.Network, "Stations 74 · 1905 · Paused")]
    [InlineData(PresencePreset.Era, "Mechanic · Paused")]
    [InlineData(PresencePreset.Minimal, "Paused")]
    internal void PausedAppendsTheLabelToTheState(PresencePreset preset, string state)
        => Assert.Equal(state, Build(Status(speedup: 0), preset).State);

    /// <summary>
    /// A paused network preset appends the German "Pausiert" label to the translated state line.
    /// </summary>
    [Fact]
    public void PausedUsesTheGermanLabel()
        => Assert.Equal("Stationen 74 · 1905 · Pausiert", Build(Status(speedup: 0), PresencePreset.Network, German, GermanTexts).State);

    /// <summary>
    /// A paused alternating preset appends the label to whichever layout the step selects, here career, network and era for steps 0 to 2.
    /// </summary>
    [Fact]
    public void PausedOnTheAlternatingPresetFollowsTheShownLayout()
    {
        Assert.Equal("September 1905 · Paused", Build(Status(speedup: 0), PresencePreset.Alternating, step: 0).State);
        Assert.Equal("Stations 74 · 1905 · Paused", Build(Status(speedup: 0), PresencePreset.Alternating, step: 1).State);
        Assert.Equal("Mechanic · Paused", Build(Status(speedup: 0), PresencePreset.Alternating, step: 2).State);
    }

    /// <summary>
    /// A status with no data falls back to "Playing" as details in the career, network and era presets, translated to "Spielt" for German.
    /// </summary>
    [Theory]
    [InlineData(PresencePreset.Career)]
    [InlineData(PresencePreset.Network)]
    [InlineData(PresencePreset.Era)]
    internal void EmptyDetailsFallBackToPlaying(PresencePreset preset)
    {
        ModStatus empty = new() { Version = 1 };

        Assert.Equal("Playing", Build(empty, preset).Details);
        Assert.Equal("Spielt", Build(empty, preset, German).Details);
    }

    /// <summary>
    /// The minimal preset shows "In game" even for a status with no data.
    /// </summary>
    [Fact]
    public void MinimalWithoutAnyDataStillShowsInGame()
        => Assert.Equal("In game", Build(new ModStatus() { Version = 1 }, PresencePreset.Minimal).Details);

    /// <summary>
    /// The network preset with only some company figures shows just those parts, here "Lines 3" and the year "1900", and omits the rest.
    /// </summary>
    [Fact]
    public void NetworkWithPartialDataDropsOnlyThePartsWithoutValues()
    {
        ModStatus status = new() { Version = 1, CompanyNumberOfLines = 3, Year = 1900 };

        PresenceContentStruct content = Build(status, PresencePreset.Network);

        Assert.Equal("Lines 3", content.Details);
        Assert.Equal("1900", content.State);
    }

    /// <summary>
    /// The network preset drops the vehicle count when the road and rail totals overflow a long, but keeps the lines and year.
    /// </summary>
    [Fact]
    public void NetworkWithAnUnrepresentableVehicleTotalDropsOnlyTheVehicles()
    {
        ModStatus status = new() { Version = 1, CompanyNumberOfLines = 3, CompanyRoadVehicles = long.MaxValue, CompanyRailVehicles = 1, Year = 1900 };

        PresenceContentStruct content = Build(status, PresencePreset.Network);

        Assert.Equal("Lines 3", content.Details);
        Assert.Equal("1900", content.State);
    }

    /// <summary>
    /// The network preset drops the vehicle count when the road and rail counts (the smallest long and -1) add up to less than a long can hold, but keeps the lines.
    /// </summary>
    [Fact]
    public void NetworkWithNegativeVehicleCountsBeyondTheRangeDropsOnlyTheVehicles()
    {
        ModStatus status = new() { Version = 1, CompanyNumberOfLines = 3, CompanyRoadVehicles = long.MinValue, CompanyRailVehicles = -1 };

        Assert.Equal("Lines 3", Build(status, PresencePreset.Network).Details);
    }

    /// <summary>
    /// The network preset sums road and rail vehicles to the largest representable total, 9,223,372,036,854,775,807.
    /// </summary>
    [Fact]
    public void NetworkSumsTheLargestRepresentableTotal()
    {
        ModStatus status = new() { Version = 1, CompanyRoadVehicles = long.MaxValue - 1, CompanyRailVehicles = 1 };

        Assert.Equal("Vehicles 9,223,372,036,854,775,807", Build(status, PresencePreset.Network).Details);
    }

    /// <summary>
    /// Every preset in the catalogue has a setting value that parses back to the same preset, there are six presets, and career and alternating use their lower-case names.
    /// </summary>
    [Fact]
    public void PresetsHaveSettingValuesThatParseBack()
    {
        for (int i = 0; i < PresetCatalog.All.Count; i++)
        {
            PresencePreset preset = PresetCatalog.All[i];

            Assert.True(PresetCatalog.TryParse(PresetCatalog.ToSetting(preset), out PresencePreset parsed));
            Assert.Equal(preset, parsed);
        }

        Assert.Equal(6, PresetCatalog.All.Count);
        Assert.Equal("career", PresetCatalog.ToSetting(PresencePreset.Career));
        Assert.Equal("alternating", PresetCatalog.ToSetting(PresencePreset.Alternating));
    }

    /// <summary>
    /// A missing, empty, wrongly capitalised or unknown setting value does not parse and leaves the career preset as the result.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Career")]
    [InlineData("B")]
    [InlineData("nothing")]
    public void UnknownSettingDoesNotParse(string? setting)
    {
        Assert.False(PresetCatalog.TryParse(setting, out PresencePreset preset));
        Assert.Equal(PresencePreset.Career, preset);
    }

    // The game's load dialogue showed 44 lines, 74 stations and 118 vehicles for this save.
    /// <summary>
    /// The status line of a real session log produces the documented example text for the career, network, era and minimal presets.
    /// </summary>
    [Theory]
    [InlineData(PresencePreset.Career, "Mechanic", "September 1905")]
    [InlineData(PresencePreset.Network, "Lines 44 · Vehicles 118", "Stations 74 · 1905")]
    [InlineData(PresencePreset.Era, "Year 1905 · Tropical", "Mechanic")]
    [InlineData(PresencePreset.Minimal, "In game · 1905", null)]
    internal void RealStatusLineGivesTheDocumentedExamples(PresencePreset preset, string details, string? state)
    {
        List<TimedEvent> events = SampleFiles.ReadEvents("session.log");
        ModStatus? status = (events.Find(static timed => timed.Event is StatusEvent)?.Event as StatusEvent)?.Status;
        Assert.NotNull(status);

        PresenceContentStruct content = Build(status, preset);

        Assert.Equal(details, content.Details);
        Assert.Equal(state, content.State);
    }
}
