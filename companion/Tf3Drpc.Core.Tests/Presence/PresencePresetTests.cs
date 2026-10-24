using System;
using System.Collections.Generic;
using System.Globalization;

using Tf3Drpc.Core.Constants;
using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Presence;
using Tf3Drpc.Core.Status;
using Tf3Drpc.Core.Structs;
using Tf3Drpc.Core.Translation;

using Xunit;

namespace Tf3Drpc.Core.Tests.Presence;

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

    [Fact]
    public void Career_IsTheDefaultOfTheShortOverload()
    {
        GameStateSnapshotStruct snapshot = new() { State = GameState.Playing, TimerStart = Timer, Status = Status() };

        PresenceBuilder.TryBuild(snapshot, English, GameTexts.English, out PresenceContentStruct shortOverload);

        Assert.Equal(Build(Status(), PresencePreset.Career), shortOverload);
        Assert.Equal("Mechanic · Tycoon", shortOverload.Details);
        Assert.Equal("September 1905", shortOverload.State);
    }

    [Theory]
    [InlineData("en-US", "Lines 44 · Vehicles 118", "Stations 74 · 1905")]
    [InlineData("de-DE", "Linien 44 · Fahrzeuge 118", "Stationen 74 · 1905")]
    public void Network_ShowsLinesVehiclesStationsAndYear(string culture, string details, string state)
    {
        GameTexts texts = (culture is "de-DE") ? GermanTexts : GameTexts.English;

        PresenceContentStruct content = Build(Status(), PresencePreset.Network, CultureInfo.GetCultureInfo(culture), texts);

        Assert.Equal(details, content.Details);
        Assert.Equal(state, content.State);
        Assert.Equal(Timer, content.StartTimestamp);
    }

    [Theory]
    [InlineData("en-US", "Year 1905 · Tropical", "Mechanic")]
    [InlineData("de-DE", "Jahr 1905 · Tropisch", "Mechaniker")]
    public void Era_ShowsYearClimateAndRank(string culture, string details, string state)
    {
        GameTexts texts = (culture is "de-DE") ? GermanTexts : GameTexts.English;

        PresenceContentStruct content = Build(Status(), PresencePreset.Era, CultureInfo.GetCultureInfo(culture), texts);

        Assert.Equal(details, content.Details);
        Assert.Equal(state, content.State);
    }

    [Theory]
    [InlineData("en-US", "In game · 1905")]
    [InlineData("de-DE", "Im Spiel · 1905")]
    public void Minimal_ShowsInGameAndTheYearWithoutState(string culture, string details)
    {
        PresenceContentStruct content = Build(Status(), PresencePreset.Minimal, CultureInfo.GetCultureInfo(culture));

        Assert.Equal(details, content.Details);
        Assert.Null(content.State);
    }

    [Fact]
    public void Minimal_WithoutYearShowsOnlyInGame()
        => Assert.Equal("In game", Build(Status() with { Year = null }, PresencePreset.Minimal).Details);

    [Theory]
    [InlineData(0, "Mechanic · Tycoon", "September 1905")]
    [InlineData(1, "Lines 44 · Vehicles 118", "Stations 74 · 1905")]
    [InlineData(2, "Year 1905 · Tropical", "Mechanic")]
    [InlineData(3, "Mechanic · Tycoon", "September 1905")]
    [InlineData(4, "Lines 44 · Vehicles 118", "Stations 74 · 1905")]
    [InlineData(5, "Year 1905 · Tropical", "Mechanic")]
    [InlineData(-1, "Year 1905 · Tropical", "Mechanic")]
    public void Alternating_CyclesByStep(long step, string details, string state)
    {
        PresenceContentStruct content = Build(Status(), PresencePreset.Alternating, step: step);

        Assert.Equal(details, content.Details);
        Assert.Equal(state, content.State);
    }

    [Theory]
    [InlineData(PresencePreset.Career, 0)]
    [InlineData(PresencePreset.Network, 1)]
    [InlineData(PresencePreset.Era, 2)]
    [InlineData(PresencePreset.Minimal, 2)]
    public void FixedPresets_IgnoreTheStep(PresencePreset preset, long step)
        => Assert.Equal(Build(Status(), preset), Build(Status(), preset, step: step));

    [Fact]
    public void AlternatingInterval_IsFortyFiveSeconds()
        => Assert.Equal(TimeSpan.FromSeconds(45), PresenceConstants.AlternatingInterval);

    [Theory]
    [InlineData(PresencePreset.Career, "September 1905 · Paused")]
    [InlineData(PresencePreset.Network, "Stations 74 · 1905 · Paused")]
    [InlineData(PresencePreset.Era, "Mechanic · Paused")]
    [InlineData(PresencePreset.Minimal, "Paused")]
    public void Paused_AppendsTheLabelToTheState(PresencePreset preset, string state)
        => Assert.Equal(state, Build(Status(speedup: 0), preset).State);

    [Fact]
    public void Paused_UsesTheGermanLabel()
        => Assert.Equal("Stationen 74 · 1905 · Pausiert", Build(Status(speedup: 0), PresencePreset.Network, German, GermanTexts).State);

    [Fact]
    public void Paused_OnTheAlternatingPresetFollowsTheShownLayout()
    {
        Assert.Equal("September 1905 · Paused", Build(Status(speedup: 0), PresencePreset.Alternating, step: 0).State);
        Assert.Equal("Stations 74 · 1905 · Paused", Build(Status(speedup: 0), PresencePreset.Alternating, step: 1).State);
        Assert.Equal("Mechanic · Paused", Build(Status(speedup: 0), PresencePreset.Alternating, step: 2).State);
    }

    [Theory]
    [InlineData(PresencePreset.Career)]
    [InlineData(PresencePreset.Network)]
    [InlineData(PresencePreset.Era)]
    public void EmptyDetails_FallBackToPlaying(PresencePreset preset)
    {
        ModStatus empty = new() { Version = 1 };

        Assert.Equal("Playing", Build(empty, preset).Details);
        Assert.Equal("Spielt", Build(empty, preset, German).Details);
    }

    [Fact]
    public void Minimal_WithoutAnyDataStillShowsInGame()
        => Assert.Equal("In game", Build(new ModStatus() { Version = 1 }, PresencePreset.Minimal).Details);

    [Fact]
    public void Network_WithPartialDataDropsOnlyThePartsWithoutValues()
    {
        ModStatus status = new() { Version = 1, CompanyNumberOfLines = 3, Year = 1900 };

        PresenceContentStruct content = Build(status, PresencePreset.Network);

        Assert.Equal("Lines 3", content.Details);
        Assert.Equal("1900", content.State);
    }

    [Fact]
    public void Network_WithAnUnrepresentableVehicleTotal_DropsOnlyTheVehicles()
    {
        ModStatus status = new() { Version = 1, CompanyNumberOfLines = 3, CompanyRoadVehicles = long.MaxValue, CompanyRailVehicles = 1, Year = 1900 };

        PresenceContentStruct content = Build(status, PresencePreset.Network);

        Assert.Equal("Lines 3", content.Details);
        Assert.Equal("1900", content.State);
    }

    [Fact]
    public void Network_WithNegativeVehicleCountsBeyondTheRange_DropsOnlyTheVehicles()
    {
        ModStatus status = new() { Version = 1, CompanyNumberOfLines = 3, CompanyRoadVehicles = long.MinValue, CompanyRailVehicles = -1 };

        Assert.Equal("Lines 3", Build(status, PresencePreset.Network).Details);
    }

    [Fact]
    public void Network_SumsTheLargestRepresentableTotal()
    {
        ModStatus status = new() { Version = 1, CompanyRoadVehicles = long.MaxValue - 1, CompanyRailVehicles = 1 };

        Assert.Equal("Vehicles 9,223,372,036,854,775,807", Build(status, PresencePreset.Network).Details);
    }

    [Fact]
    public void Presets_HaveSettingValuesThatParseBack()
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

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Career")]
    [InlineData("B")]
    [InlineData("nothing")]
    public void UnknownSetting_DoesNotParse(string? setting)
    {
        Assert.False(PresetCatalog.TryParse(setting, out PresencePreset preset));
        Assert.Equal(PresencePreset.Career, preset);
    }

    // The game's load dialogue showed 44 lines, 74 stations and 118 vehicles for this save.
    [Theory]
    [InlineData(PresencePreset.Career, "Mechanic", "September 1905")]
    [InlineData(PresencePreset.Network, "Lines 44 · Vehicles 118", "Stations 74 · 1905")]
    [InlineData(PresencePreset.Era, "Year 1905 · Tropical", "Mechanic")]
    [InlineData(PresencePreset.Minimal, "In game · 1905", null)]
    public void RealStatusLine_GivesTheDocumentedExamples(PresencePreset preset, string details, string? state)
    {
        List<TimedEvent> events = SampleFiles.ReadEvents("session.log");
        ModStatus? status = events.Find(static timed => timed.Event.Kind is GameLogEventKind.Status)?.Event.Status;
        Assert.NotNull(status);

        PresenceContentStruct content = Build(status, preset);

        Assert.Equal(details, content.Details);
        Assert.Equal(state, content.State);
    }
}
