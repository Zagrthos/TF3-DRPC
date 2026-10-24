using System.Globalization;

using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Presence;
using Tf3Drpc.Core.Settings;
using Tf3Drpc.Core.Status;
using Tf3Drpc.Core.Structs;
using Tf3Drpc.Core.Translation;

using Xunit;

namespace Tf3Drpc.Core.Tests.Presence;

public sealed class PresencePreviewTests
{
    private static readonly CultureInfo English = CultureInfo.GetCultureInfo("en-US");

    private static PresenceContentStruct Build(PresencePreview preview, PresencePreset preset, out bool usesExample, PresenceTemplates? templates = null, long step = 0)
    {
        PresenceOptionsStruct options = new() { Preset = preset, RotationStep = step, Templates = templates };

        return preview.Build(English, GameTexts.English, options, out usesExample);
    }

    [Fact]
    public void WithoutAStatus_TheExampleIsUsed()
    {
        PresencePreview preview = new();

        PresenceContentStruct career = Build(preview, PresencePreset.Career, out bool careerExample);
        PresenceContentStruct network = Build(preview, PresencePreset.Network, out bool networkExample);

        Assert.True(careerExample);
        Assert.True(networkExample);
        Assert.Equal("Mechanic", career.Details);
        Assert.Equal("September 1905", career.State);
        Assert.Equal("Lines 44 · Vehicles 118", network.Details);
        Assert.Equal("Stations 74 · 1905", network.State);
        Assert.Null(career.StartTimestamp);
    }

    [Fact]
    public void ExampleValues_MatchTheRealStatusLine()
    {
        PresencePreview preview = new();
        PresenceTemplates templates = new()
        {
            Details = "{saveName} · {passengers} · {cargo}",
            State = "{balance} · {companyValue}"
        };

        PresenceContentStruct content = Build(preview, PresencePreset.Custom, out _, templates);

        Assert.Equal("Archipelago2 · 3,466 · 9,424", content.Details);
        Assert.Equal("$14.4M · $70.4M", content.State);
    }

    [Fact]
    public void AfterRemember_TheLastStatusIsUsedAndNotPaused()
    {
        PresencePreview preview = new();
        ModStatus status = new() { Version = 1, Rank = 5, Year = 1920, Month = 3, Speedup = 0 };
        GameStateSnapshotStruct snapshot = new() { State = GameState.Playing, Status = status, IsPaused = true };

        preview.Remember(snapshot);
        PresenceContentStruct content = Build(preview, PresencePreset.Career, out bool usesExample);

        Assert.False(usesExample);
        Assert.Equal("March 1920", content.State);
        Assert.Equal(GameTexts.English.GetRankName(5), content.Details);
    }

    [Fact]
    public void TheLastStatusStaysAfterLeavingTheGame()
    {
        PresencePreview preview = new();
        preview.Remember(new GameStateSnapshotStruct() { State = GameState.Playing, Status = new ModStatus() { Version = 1, Year = 1920, Month = 3 } });

        preview.Remember(new GameStateSnapshotStruct() { State = GameState.MainMenu });
        PresenceContentStruct content = Build(preview, PresencePreset.Career, out bool usesExample);

        Assert.False(usesExample);
        Assert.Equal("March 1920", content.State);
    }

    [Fact]
    public void CustomTemplates_AreApplied()
    {
        PresencePreview preview = new();
        PresenceTemplates templates = new() { Details = "Year {year}", State = "{climate}", Hover = "Hover {lines}" };

        PresenceContentStruct content = Build(preview, PresencePreset.Custom, out _, templates);

        Assert.Equal("Year 1905", content.Details);
        Assert.Equal("Tropical", content.State);
        Assert.Equal("Hover 44", content.LargeImageText);
    }

    [Fact]
    public void Alternating_FollowsTheStep()
    {
        PresencePreview preview = new();

        Assert.Equal("Mechanic", Build(preview, PresencePreset.Alternating, out _, step: 0).Details);
        Assert.Equal("Lines 44 · Vehicles 118", Build(preview, PresencePreset.Alternating, out _, step: 1).Details);
        Assert.Equal("Year 1905 · Tropical", Build(preview, PresencePreset.Alternating, out _, step: 2).Details);
    }
}
