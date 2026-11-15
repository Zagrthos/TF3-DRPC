using System.Globalization;

using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Presence;
using Tf3Drpc.Core.Settings;
using Tf3Drpc.Core.Status;
using Tf3Drpc.Core.Structs;
using Tf3Drpc.Core.Translation;

using Xunit;

namespace Tf3Drpc.Core.Tests.Presence;

/// <summary>
/// Tests of <see cref="PresencePreview"/>, which builds the settings preview of the presence from the last seen game status, or from an example status when there is none.
/// </summary>
public sealed class PresencePreviewTests
{
    private static readonly CultureInfo English = CultureInfo.GetCultureInfo("en-US");

    private static PresenceContentStruct Build(PresencePreview preview, PresencePreset preset, out bool usesExample, PresenceTemplates? templates = null, long step = 0)
    {
        PresenceOptionsStruct options = new() { Preset = preset, RotationStep = step, Templates = templates };

        return preview.Build(English, GameTexts.English, options, out usesExample);
    }

    /// <summary>
    /// With no remembered status the preview builds from the example status and reports that it did, so the career preset shows "Mechanic" over "September 1905", the network preset shows "Lines 44 · Vehicles 118" over "Stations 74 · 1905", and there is no start timestamp.
    /// </summary>
    [Fact]
    public void WithoutAStatusTheExampleIsUsed()
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

    /// <summary>
    /// The example status carries the values of the real status line, so the custom placeholders render "Archipelago2 · 3,466 · 9,424" and "$14.4M · $70.4M".
    /// </summary>
    [Fact]
    public void ExampleValuesMatchTheRealStatusLine()
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

    /// <summary>
    /// After a playing snapshot is remembered, the preview uses that status instead of the example and shows it as not paused, even though the snapshot was flagged as paused.
    /// </summary>
    [Fact]
    public void AfterRememberTheLastStatusIsUsedAndNotPaused()
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

    /// <summary>
    /// The remembered status survives a later snapshot of the main menu, so the preview still shows "March 1920".
    /// </summary>
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

    /// <summary>
    /// Custom templates are rendered against the example status, giving "Year 1905", "Tropical" and the hover "Hover 44".
    /// </summary>
    [Fact]
    public void CustomTemplatesAreApplied()
    {
        PresencePreview preview = new();
        PresenceTemplates templates = new() { Details = "Year {year}", State = "{climate}", Hover = "Hover {lines}" };

        PresenceContentStruct content = Build(preview, PresencePreset.Custom, out _, templates);

        Assert.Equal("Year 1905", content.Details);
        Assert.Equal("Tropical", content.State);
        Assert.Equal("Hover 44", content.LargeImageText);
    }

    /// <summary>
    /// The alternating preset in the preview follows the rotation step, showing the career, network and era layouts for steps 0, 1 and 2.
    /// </summary>
    [Fact]
    public void AlternatingFollowsTheStep()
    {
        PresencePreview preview = new();

        Assert.Equal("Mechanic", Build(preview, PresencePreset.Alternating, out _, step: 0).Details);
        Assert.Equal("Lines 44 · Vehicles 118", Build(preview, PresencePreset.Alternating, out _, step: 1).Details);
        Assert.Equal("Year 1905 · Tropical", Build(preview, PresencePreset.Alternating, out _, step: 2).Details);
    }
}
