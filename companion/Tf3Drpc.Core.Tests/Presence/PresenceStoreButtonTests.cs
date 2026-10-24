using System;
using System.Collections.Generic;
using System.Globalization;

using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Presence;
using Tf3Drpc.Core.Settings;
using Tf3Drpc.Core.Status;
using Tf3Drpc.Core.Structs;
using Tf3Drpc.Core.Translation;

using Xunit;

namespace Tf3Drpc.Core.Tests.Presence;

public sealed class PresenceStoreButtonTests
{
    private static readonly CultureInfo English = CultureInfo.GetCultureInfo("en-US");
    private static readonly CultureInfo German = CultureInfo.GetCultureInfo("de-DE");
    private static readonly DateTimeOffset Timer = new(2026, 10, 13, 21, 15, 24, TimeSpan.Zero);

    private static GameTexts Texts(string newGame)
        => new(new Dictionary<string, string>(StringComparer.Ordinal) { ["New Game"] = newGame });

    private static GameStateSnapshotStruct Snapshot(GameState state, string? saveName = "Archipelago2")
    {
        ModStatus? status = (state is GameState.Playing) ? new ModStatus() { Version = 1, SaveName = saveName, Rank = 2, Speedup = 1 } : null;

        return new GameStateSnapshotStruct() { State = state, TimerStart = Timer, Status = status };
    }

    private static PresenceContentStruct Build(GameState state, CultureInfo culture, bool showButton)
    {
        PresenceOptionsStruct options = new() { Preset = PresencePreset.Career, ShowStoreButton = showButton };

        Assert.True(PresenceBuilder.TryBuild(Snapshot(state), culture, GameTexts.English, options, out PresenceContentStruct content));

        return content;
    }

    private static string RenderSaveName(string? saveName, GameTexts chosen, GameTexts? gameLanguage, CultureInfo culture, PresencePreset preset = PresencePreset.Custom)
    {
        PresenceOptionsStruct options = new() { Preset = preset, Templates = new PresenceTemplates() { Details = "{saveName}" }, GameLanguageTexts = gameLanguage };

        Assert.True(PresenceBuilder.TryBuild(Snapshot(GameState.Playing, saveName), culture, chosen, options, out PresenceContentStruct content));

        return content.Details;
    }

    #region Store button

    [Theory]
    [InlineData(GameState.Starting)]
    [InlineData(GameState.MainMenu)]
    [InlineData(GameState.Loading)]
    [InlineData(GameState.Mission)]
    [InlineData(GameState.Playing)]
    public void ButtonOn_EveryPresenceCarriesTheLabelInTheChosenLanguage(GameState state)
    {
        Assert.Equal("Get Transport Fever 3", Build(state, English, showButton: true).ButtonLabel);
        Assert.Equal("Hol dir Transport Fever 3", Build(state, German, showButton: true).ButtonLabel);
    }

    [Theory]
    [InlineData(GameState.Starting)]
    [InlineData(GameState.MainMenu)]
    [InlineData(GameState.Loading)]
    [InlineData(GameState.Mission)]
    [InlineData(GameState.Playing)]
    public void ButtonOff_NoPresenceCarriesALabel(GameState state)
        => Assert.Null(Build(state, English, showButton: false).ButtonLabel);

    [Fact]
    public void Default_HasNoButton()
    {
        Assert.True(PresenceBuilder.TryBuild(Snapshot(GameState.Playing), English, GameTexts.English, out PresenceContentStruct content));
        Assert.Null(content.ButtonLabel);
    }

    [Fact]
    public void Preview_FollowsTheOption()
    {
        PresencePreview preview = new();

        PresenceContentStruct on = preview.Build(English, GameTexts.English, new PresenceOptionsStruct() { Preset = PresencePreset.Career, ShowStoreButton = true }, out _);
        PresenceContentStruct off = preview.Build(English, GameTexts.English, new PresenceOptionsStruct() { Preset = PresencePreset.Career }, out _);

        Assert.Equal("Get Transport Fever 3", on.ButtonLabel);
        Assert.Null(off.ButtonLabel);
    }

    [Fact]
    public void ContentsThatDifferOnlyInTheLabel_AreNotEqual()
    {
        PresenceContentStruct first = new() { Details = "A", ButtonLabel = "Get Transport Fever 3" };
        PresenceContentStruct second = new() { Details = "A" };
        PresenceContentStruct same = new() { Details = "A", ButtonLabel = "Get Transport Fever 3" };

        Assert.True(first != second);
        Assert.NotEqual(first.GetHashCode(), second.GetHashCode());
        Assert.True(first == same);
        Assert.Equal(first.GetHashCode(), same.GetHashCode());
    }

    [Fact]
    public void OptionsThatDifferOnlyInTheButtonOrTheGameTexts_AreNotEqual()
    {
        GameTexts texts = Texts("Neues Spiel");
        PresenceOptionsStruct plain = new() { Preset = PresencePreset.Career };

        Assert.True(plain != new PresenceOptionsStruct() { Preset = PresencePreset.Career, ShowStoreButton = true });
        Assert.True(plain != new PresenceOptionsStruct() { Preset = PresencePreset.Career, GameLanguageTexts = texts });
        Assert.True(new PresenceOptionsStruct() { GameLanguageTexts = texts } == new PresenceOptionsStruct() { GameLanguageTexts = texts });
    }

    #endregion Store button

    #region New Game

    [Fact]
    public void NewGame_OfTheGameLanguageIsShownInTheChosenLanguage()
    {
        Assert.Equal("New Game", RenderSaveName("Neues Spiel", GameTexts.English, Texts("Neues Spiel"), English));
        Assert.Equal("Nouvelle partie", RenderSaveName("Neues Spiel", Texts("Nouvelle partie"), Texts("Neues Spiel"), CultureInfo.GetCultureInfo("fr-FR")));
    }

    [Fact]
    public void NewGame_OtherNamesStayAsTheyAre()
        => Assert.Equal("Archipelago2", RenderSaveName("Archipelago2", GameTexts.English, Texts("Neues Spiel"), English));

    [Fact]
    public void NewGame_WithoutGameLanguageTextsNothingIsMapped()
        => Assert.Equal("Neues Spiel", RenderSaveName("Neues Spiel", GameTexts.English, null, English));

    [Fact]
    public void NewGame_WithAnEnglishGameLanguageIsShownInTheChosenLanguage()
        => Assert.Equal("Neues Spiel", RenderSaveName("New Game", Texts("Neues Spiel"), GameTexts.English, German));

    [Fact]
    public void NewGame_ComparesOrdinallyAndKeepsAMissingName()
    {
        Assert.Equal("neues spiel", RenderSaveName("neues spiel", GameTexts.English, Texts("Neues Spiel"), English));
        Assert.Equal("Playing", RenderSaveName(null, GameTexts.English, Texts("Neues Spiel"), English));
    }

    #endregion New Game
}
