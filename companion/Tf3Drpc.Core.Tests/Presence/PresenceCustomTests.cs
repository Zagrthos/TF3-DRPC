using System;
using System.Globalization;
using System.Text.Json;

using Tf3Drpc.Core.Constants;
using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Presence;
using Tf3Drpc.Core.Settings;
using Tf3Drpc.Core.Status;
using Tf3Drpc.Core.Structs;
using Tf3Drpc.Core.Translation;
using Tf3Drpc.Core.Utilities;

using Xunit;

namespace Tf3Drpc.Core.Tests.Presence;

/// <summary>
/// Tests of the custom preset, in which <see cref="PresenceBuilder"/> renders user-written details, state and hover templates, together with the preset's catalogue entry and the option types.
/// </summary>
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

    /// <summary>
    /// The custom preset renders the details and state templates against the status, here "Mechanic · Lines 44" over "September 1905", with the timer start as the timestamp.
    /// </summary>
    [Fact]
    public void CustomRendersTheDetailsAndStateTemplates()
    {
        PresenceTemplates templates = new() { Details = "{rank} · Lines {lines}", State = "{date}" };

        PresenceContentStruct content = Build(templates);

        Assert.Equal("Mechanic · Lines 44", content.Details);
        Assert.Equal("September 1905", content.State);
        Assert.Equal(Timer, content.StartTimestamp);
    }

    /// <summary>
    /// The custom preset without templates uses the default templates, showing "Mechanic · Tycoon" over "September 1905" and the default hover "Transport Fever 3".
    /// </summary>
    [Fact]
    public void CustomWithoutTemplatesUsesTheDefaults()
    {
        PresenceContentStruct content = Build(templates: null);

        Assert.Equal("Mechanic · Tycoon", content.Details);
        Assert.Equal("September 1905", content.State);
        Assert.Equal("Transport Fever 3", content.LargeImageText);
    }

    /// <summary>
    /// Details templates that are empty, blank or only an unknown placeholder fall back to "Playing".
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("{unknown}")]
    public void CustomEmptyDetailsFallBackToPlaying(string details)
        => Assert.Equal("Playing", Build(new PresenceTemplates() { Details = details }).Details);

    /// <summary>
    /// A custom state template longer than the limit is cut before the paused suffix is added, so the paused state still ends with "… · Paused" and is exactly 128 characters long.
    /// </summary>
    [Fact]
    public void CustomLongStateKeepsThePausedSuffix()
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

    /// <summary>
    /// A short custom state template keeps its full text and gains the " · Paused" suffix when the game is paused.
    /// </summary>
    [Fact]
    public void CustomShortStateWithPausedIsUnchanged()
        => Assert.Equal("September 1905 · Paused", Build(new PresenceTemplates() { State = "{date}" }, speedup: 0).State);

    /// <summary>
    /// An empty custom state template renders no state line.
    /// </summary>
    [Fact]
    public void CustomEmptyStateRendersNothing()
        => Assert.Null(Build(new PresenceTemplates() { State = string.Empty }).State);

    /// <summary>
    /// The paused suffix is appended to a custom state template, and stands alone as "Paused" when the template is empty.
    /// </summary>
    [Fact]
    public void CustomPauseSuffixIsAppendedToTheStateTemplate()
    {
        PresenceTemplates templates = new() { State = "Year {year}" };

        Assert.Equal("Year 1905 · Paused", Build(templates, speedup: 0).State);
        Assert.Equal("Paused", Build(new PresenceTemplates() { State = string.Empty }, speedup: 0).State);
    }

    /// <summary>
    /// An unknown or wrongly capitalised placeholder renders as nothing, so its part of the template is dropped and a template with no remaining text gives no state.
    /// </summary>
    [Fact]
    public void CustomUnknownPlaceholderDropsItsPart()
    {
        PresenceTemplates templates = new() { Details = "{nothing} · {rank}", State = "{Rank}" };

        PresenceContentStruct content = Build(templates);

        Assert.Equal("Mechanic", content.Details);
        Assert.Null(content.State);
    }

    /// <summary>
    /// The custom preset renders the private placeholders for the save name, balance and company value, here "Archipelago2" and "$14.4M · $70.4M".
    /// </summary>
    [Fact]
    public void CustomPrivatePlaceholdersRender()
    {
        PresenceTemplates templates = new() { Details = "{saveName}", State = "{balance} · {companyValue}" };

        PresenceContentStruct content = Build(templates);

        Assert.Equal("Archipelago2", content.Details);
        Assert.Equal("$14.4M · $70.4M", content.State);
    }

    /// <summary>
    /// The hover template is rendered only for the custom preset while playing with a status, and gives no hover with no status or in the main menu and loading states.
    /// </summary>
    [Fact]
    public void HoverIsRenderedOnlyForCustomWhilePlayingWithAStatus()
    {
        PresenceTemplates templates = new() { Hover = "{saveName} in {year}" };

        Assert.Equal("Archipelago2 in 1905", Build(templates).LargeImageText);
        Assert.Null(Build(templates, withStatus: false).LargeImageText);
        Assert.Null(Build(templates, state: GameState.MainMenu).LargeImageText);
        Assert.Null(Build(templates, state: GameState.Loading).LargeImageText);
    }

    /// <summary>
    /// Presets other than custom give no hover text even when templates with a hover are supplied.
    /// </summary>
    [Theory]
    [InlineData(PresencePreset.Career)]
    [InlineData(PresencePreset.Network)]
    [InlineData(PresencePreset.Era)]
    [InlineData(PresencePreset.Alternating)]
    [InlineData(PresencePreset.Minimal)]
    internal void HoverIsNullForOtherPresets(PresencePreset preset)
        => Assert.Null(Build(new PresenceTemplates() { Hover = "Hover text" }, preset).LargeImageText);

    /// <summary>
    /// A hover template longer than the Discord text limit is cut to exactly that length and ends with an ellipsis.
    /// </summary>
    [Fact]
    public void HoverIsCutAtTheLimitWithAnEllipsis()
    {
        string hover = new('x', 200);

        string? text = Build(new PresenceTemplates() { Hover = hover }).LargeImageText;

        Assert.NotNull(text);
        Assert.Equal(DiscordConstants.MaxTextLength, text.Length);
        Assert.EndsWith("…", text, StringComparison.Ordinal);
    }

    /// <summary>
    /// A hover template that is empty, blank, a single character or only an unknown placeholder sets no hover text, so the SET_ACTIVITY payload falls back to the default "Transport Fever 3".
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("a")]
    [InlineData("{unknown}")]
    public void HoverBlankOrShortSetsNoHoverText(string hover)
    {
        PresenceContentStruct content = Build(new PresenceTemplates() { Hover = hover });

        byte[] payload = DiscordMessageUtils.BuildSetActivity(content, "nonce");
        using JsonDocument document = JsonDocument.Parse(payload);
        JsonElement assets = document.RootElement.GetProperty("args").GetProperty("activity").GetProperty("assets");

        Assert.Null(content.LargeImageText);
        Assert.Equal("Transport Fever 3", assets.GetProperty("large_text").GetString());
    }

    /// <summary>
    /// The hover text does not receive the paused suffix when the game is paused.
    /// </summary>
    [Fact]
    public void HoverGetsNoPauseSuffix()
        => Assert.Equal("Hover text", Build(new PresenceTemplates() { Hover = "Hover text" }, speedup: 0).LargeImageText);

    /// <summary>
    /// Presence contents are compared including their hover text, so contents that differ in or lack a hover are unequal, and equal contents share a hash code.
    /// </summary>
    [Fact]
    public void ContentEqualityIncludesTheHoverText()
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

    /// <summary>
    /// The older <see cref="PresenceBuilder"/> overload taking a preset and step builds the same content as the overload taking an options struct with that preset.
    /// </summary>
    [Fact]
    public void ExistingOverloadsDelegateToTheOptions()
    {
        GameStateSnapshotStruct snapshot = new() { State = GameState.Playing, TimerStart = Timer, Status = Status() };

        PresenceBuilder.TryBuild(snapshot, English, GameTexts.English, PresencePreset.Network, 0, out PresenceContentStruct overload);
        PresenceBuilder.TryBuild(snapshot, English, GameTexts.English, new PresenceOptionsStruct() { Preset = PresencePreset.Network }, out PresenceContentStruct options);

        Assert.Equal(options, overload);
    }

    /// <summary>
    /// The custom preset is the last entry of the preset catalogue, which has six presets, and its setting value "custom" parses back to it and has its own display text.
    /// </summary>
    [Fact]
    public void CustomIsTheLastPresetAndParses()
    {
        Assert.Equal(PresencePreset.Custom, PresetCatalog.All[PresetCatalog.All.Count - 1]);
        Assert.Equal(6, PresetCatalog.All.Count);
        Assert.Equal("custom", PresetCatalog.ToSetting(PresencePreset.Custom));
        Assert.True(PresetCatalog.TryParse("custom", out PresencePreset parsed));
        Assert.Equal(PresencePreset.Custom, parsed);
        Assert.Equal(OwnText.PresetCustom, PresetCatalog.GetDisplayText(PresencePreset.Custom));
    }

    /// <summary>
    /// Presence options compare by value across the preset, rotation step and templates, so options with an equal templates object are equal and a different rotation step makes them unequal.
    /// </summary>
    [Fact]
    public void PresenceOptionsCompareByValue()
    {
        PresenceTemplates templates = new() { Details = "x" };
        PresenceOptionsStruct first = new() { Preset = PresencePreset.Custom, RotationStep = 2, Templates = templates };
        PresenceOptionsStruct same = new() { Preset = PresencePreset.Custom, RotationStep = 2, Templates = new PresenceTemplates() { Details = "x" } };

        Assert.Equal(first, same);
        Assert.Equal(first.GetHashCode(), same.GetHashCode());
        Assert.True(first != new PresenceOptionsStruct() { Preset = PresencePreset.Custom, RotationStep = 3, Templates = templates });
    }
}
