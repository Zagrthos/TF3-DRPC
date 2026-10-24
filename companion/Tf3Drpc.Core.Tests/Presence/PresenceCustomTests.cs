using System;
using System.Globalization;

using Tf3Drpc.Core.Constants;
using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Presence;
using Tf3Drpc.Core.Settings;
using Tf3Drpc.Core.Status;
using Tf3Drpc.Core.Structs;
using Tf3Drpc.Core.Translation;

using Xunit;

namespace Tf3Drpc.Core.Tests.Presence;

public sealed class PresenceCustomTests
{
    private static readonly CultureInfo English = CultureInfo.GetCultureInfo("en-US");
    private static readonly DateTimeOffset Timer = new(2026, 10, 13, 21, 15, 24, TimeSpan.Zero);

    private static ModStatus Status(long? speedup = 1)
        => new()
        {
            Version = 1,
            SaveName = "Archipelago2",
            Rank = 2,
            IsTycoon = true,
            Year = 1905,
            Month = 9,
            Speedup = speedup,
            Balance = 14422756,
            CompanyTotalAssets = 55956242,
            CompanyDebt = 0,
            CompanyNumberOfLines = 44
        };

    private static PresenceContentStruct Build(PresenceTemplates? templates, PresencePreset preset = PresencePreset.Custom, long? speedup = 1, GameState state = GameState.Playing, bool withStatus = true)
    {
        ModStatus? status = (withStatus) ? Status(speedup) : null;
        GameStateSnapshotStruct snapshot = new() { State = state, TimerStart = Timer, Status = status, IsPaused = speedup is 0 };
        PresenceOptionsStruct options = new() { Preset = preset, Templates = templates };

        Assert.True(PresenceBuilder.TryBuild(snapshot, English, GameTexts.English, options, out PresenceContentStruct content));

        return content;
    }

    [Fact]
    public void Custom_RendersTheDetailsAndStateTemplates()
    {
        PresenceTemplates templates = new() { Details = "{rank} · Lines {lines}", State = "{date}" };

        PresenceContentStruct content = Build(templates);

        Assert.Equal("Mechanic · Lines 44", content.Details);
        Assert.Equal("September 1905", content.State);
        Assert.Equal(Timer, content.StartTimestamp);
    }

    [Fact]
    public void Custom_WithoutTemplatesUsesTheDefaults()
    {
        PresenceContentStruct content = Build(null);

        Assert.Equal("Mechanic · Tycoon", content.Details);
        Assert.Equal("September 1905", content.State);
        Assert.Equal("Transport Fever 3", content.LargeImageText);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("{unknown}")]
    public void Custom_EmptyDetailsFallBackToPlaying(string details)
        => Assert.Equal("Playing", Build(new PresenceTemplates() { Details = details }).Details);

    [Fact]
    public void Custom_LongStateKeepsThePausedSuffix()
    {
        PresenceTemplates templates = new() { State = new string('S', 150) };

        PresenceContentStruct playing = Build(templates, speedup: 1);
        PresenceContentStruct paused = Build(templates, speedup: 0);

        Assert.NotEqual(playing, paused);
        Assert.NotNull(paused.State);
        Assert.EndsWith("… · Paused", paused.State, StringComparison.Ordinal);
        Assert.True(paused.State.Length is <= 128);
        Assert.Equal(128, paused.State.Length);
        Assert.EndsWith("…", playing.State, StringComparison.Ordinal);
    }

    [Fact]
    public void Custom_ShortStateWithPausedIsUnchanged()
        => Assert.Equal("September 1905 · Paused", Build(new PresenceTemplates() { State = "{date}" }, speedup: 0).State);

    [Fact]
    public void Custom_EmptyStateRendersNothing()
        => Assert.Null(Build(new PresenceTemplates() { State = string.Empty }).State);

    [Fact]
    public void Custom_PauseSuffixIsAppendedToTheStateTemplate()
    {
        PresenceTemplates templates = new() { State = "Year {year}" };

        Assert.Equal("Year 1905 · Paused", Build(templates, speedup: 0).State);
        Assert.Equal("Paused", Build(new PresenceTemplates() { State = string.Empty }, speedup: 0).State);
    }

    [Fact]
    public void Custom_UnknownPlaceholderDropsItsPart()
    {
        PresenceTemplates templates = new() { Details = "{nothing} · {rank}", State = "{Rank}" };

        PresenceContentStruct content = Build(templates);

        Assert.Equal("Mechanic", content.Details);
        Assert.Null(content.State);
    }

    [Fact]
    public void Custom_PrivatePlaceholdersRender()
    {
        PresenceTemplates templates = new() { Details = "{saveName}", State = "{balance} · {companyValue}" };

        PresenceContentStruct content = Build(templates);

        Assert.Equal("Archipelago2", content.Details);
        Assert.Equal("$14.4M · $70.4M", content.State);
    }

    [Fact]
    public void Hover_IsRenderedOnlyForCustomWhilePlayingWithAStatus()
    {
        PresenceTemplates templates = new() { Hover = "{saveName} in {year}" };

        Assert.Equal("Archipelago2 in 1905", Build(templates).LargeImageText);
        Assert.Null(Build(templates, withStatus: false).LargeImageText);
        Assert.Null(Build(templates, state: GameState.MainMenu).LargeImageText);
        Assert.Null(Build(templates, state: GameState.Loading).LargeImageText);
    }

    [Theory]
    [InlineData(PresencePreset.Career)]
    [InlineData(PresencePreset.Network)]
    [InlineData(PresencePreset.Era)]
    [InlineData(PresencePreset.Alternating)]
    [InlineData(PresencePreset.Minimal)]
    public void Hover_IsNullForOtherPresets(PresencePreset preset)
        => Assert.Null(Build(new PresenceTemplates() { Hover = "Hover text" }, preset).LargeImageText);

    [Fact]
    public void Hover_IsCutAtTheLimitWithAnEllipsis()
    {
        string hover = new('x', 200);

        string? text = Build(new PresenceTemplates() { Hover = hover }).LargeImageText;

        Assert.NotNull(text);
        Assert.Equal(DiscordConstants.MaxTextLength, text.Length);
        Assert.EndsWith("…", text, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("a")]
    [InlineData("{unknown}")]
    public void Hover_BlankOrShortGivesTheDefault(string hover)
        => Assert.Null(Build(new PresenceTemplates() { Hover = hover }).LargeImageText);

    [Fact]
    public void Hover_GetsNoPauseSuffix()
        => Assert.Equal("Hover text", Build(new PresenceTemplates() { Hover = "Hover text" }, speedup: 0).LargeImageText);

    [Fact]
    public void ContentEquality_IncludesTheHoverText()
    {
        PresenceContentStruct first = new() { Details = "A", LargeImageText = "One" };
        PresenceContentStruct same = new() { Details = "A", LargeImageText = "One" };
        PresenceContentStruct other = new() { Details = "A", LargeImageText = "Two" };
        PresenceContentStruct none = new() { Details = "A" };

        Assert.Equal(first, same);
        Assert.Equal(first.GetHashCode(), same.GetHashCode());
        Assert.NotEqual(first, other);
        Assert.NotEqual(first, none);
        Assert.True(first != other);
    }

    [Fact]
    public void ExistingOverloads_DelegateToTheOptions()
    {
        GameStateSnapshotStruct snapshot = new() { State = GameState.Playing, TimerStart = Timer, Status = Status() };

        PresenceBuilder.TryBuild(snapshot, English, GameTexts.English, PresencePreset.Network, 0, out PresenceContentStruct overload);
        PresenceBuilder.TryBuild(snapshot, English, GameTexts.English, new PresenceOptionsStruct() { Preset = PresencePreset.Network }, out PresenceContentStruct options);

        Assert.Equal(options, overload);
    }

    [Fact]
    public void Custom_IsTheLastPresetAndParses()
    {
        Assert.Equal(PresencePreset.Custom, PresetCatalog.All[PresetCatalog.All.Count - 1]);
        Assert.Equal(6, PresetCatalog.All.Count);
        Assert.Equal("custom", PresetCatalog.ToSetting(PresencePreset.Custom));
        Assert.True(PresetCatalog.TryParse("custom", out PresencePreset parsed));
        Assert.Equal(PresencePreset.Custom, parsed);
        Assert.Equal(OwnText.PresetCustom, PresetCatalog.GetDisplayText(PresencePreset.Custom));
    }

    [Fact]
    public void PresenceOptions_CompareByValue()
    {
        PresenceTemplates templates = new() { Details = "x" };
        PresenceOptionsStruct first = new() { Preset = PresencePreset.Custom, RotationStep = 2, Templates = templates };
        PresenceOptionsStruct same = new() { Preset = PresencePreset.Custom, RotationStep = 2, Templates = new PresenceTemplates() { Details = "x" } };

        Assert.Equal(first, same);
        Assert.Equal(first.GetHashCode(), same.GetHashCode());
        Assert.True(first != new PresenceOptionsStruct() { Preset = PresencePreset.Custom, RotationStep = 3, Templates = templates });
    }
}
