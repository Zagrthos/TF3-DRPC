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

/// <summary>
/// Tests of the store button label in the presence and of how the "New Game" save name is shown in the chosen language.
/// </summary>
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

    /// <summary>
    /// With the store button on, the presence of every game state from starting to playing carries the label "Get Transport Fever 3", or "Hol dir Transport Fever 3" in German.
    /// </summary>
    [Theory]
    [InlineData(GameState.Starting)]
    [InlineData(GameState.MainMenu)]
    [InlineData(GameState.Loading)]
    [InlineData(GameState.Mission)]
    [InlineData(GameState.Playing)]
    internal void ButtonOnEveryPresenceCarriesTheLabelInTheChosenLanguage(GameState state)
    {
        Assert.Equal("Get Transport Fever 3", Build(state, English, showButton: true).ButtonLabel);
        Assert.Equal("Hol dir Transport Fever 3", Build(state, German, showButton: true).ButtonLabel);
    }

    /// <summary>
    /// With the store button off, the presence of every game state from starting to playing carries no label.
    /// </summary>
    [Theory]
    [InlineData(GameState.Starting)]
    [InlineData(GameState.MainMenu)]
    [InlineData(GameState.Loading)]
    [InlineData(GameState.Mission)]
    [InlineData(GameState.Playing)]
    internal void ButtonOffNoPresenceCarriesALabel(GameState state)
        => Assert.Null(Build(state, English, showButton: false).ButtonLabel);

    /// <summary>
    /// The store button is off by default, so the presence built without options has no label.
    /// </summary>
    [Fact]
    public void DefaultHasNoButton()
    {
        Assert.True(PresenceBuilder.TryBuild(Snapshot(GameState.Playing), English, GameTexts.English, out PresenceContentStruct content));
        Assert.Null(content.ButtonLabel);
    }

    /// <summary>
    /// The presence preview carries the store button label when the option is on and none when it is off.
    /// </summary>
    [Fact]
    public void PreviewFollowsTheOption()
    {
        PresencePreview preview = new();

        PresenceContentStruct on = preview.Build(English, GameTexts.English, new PresenceOptionsStruct() { Preset = PresencePreset.Career, ShowStoreButton = true }, out _);
        PresenceContentStruct off = preview.Build(English, GameTexts.English, new PresenceOptionsStruct() { Preset = PresencePreset.Career }, out _);

        Assert.Equal("Get Transport Fever 3", on.ButtonLabel);
        Assert.Null(off.ButtonLabel);
    }

    /// <summary>
    /// Presence contents that differ only in the button label are unequal and have different hash codes, while contents with the same label are equal.
    /// </summary>
    [Fact]
    public void ContentsThatDifferOnlyInTheLabelAreNotEqual()
    {
        PresenceContentStruct first = new() { Details = "A", ButtonLabel = "Get Transport Fever 3" };
        PresenceContentStruct second = new() { Details = "A" };
        PresenceContentStruct same = new() { Details = "A", ButtonLabel = "Get Transport Fever 3" };

        Assert.True(first != second);
        Assert.NotEqual(first.GetHashCode(), second.GetHashCode());
        Assert.True(first == same);
        Assert.Equal(first.GetHashCode(), same.GetHashCode());
    }

    /// <summary>
    /// Presence options that differ only in the store button or the game language texts are unequal, while options with the same game language texts are equal.
    /// </summary>
    [Fact]
    public void OptionsThatDifferOnlyInTheButtonOrTheGameTextsAreNotEqual()
    {
        GameTexts texts = Texts("Neues Spiel");
        PresenceOptionsStruct plain = new() { Preset = PresencePreset.Career };

        Assert.True(plain != new PresenceOptionsStruct() { Preset = PresencePreset.Career, ShowStoreButton = true });
        Assert.True(plain != new PresenceOptionsStruct() { Preset = PresencePreset.Career, GameLanguageTexts = texts });
        Assert.True(new PresenceOptionsStruct() { GameLanguageTexts = texts } == new PresenceOptionsStruct() { GameLanguageTexts = texts });
    }

    #endregion Store button

    #region New Game

    /// <summary>
    /// A save name equal to the game's "New Game" text in the game language is shown with the "New Game" text of the chosen language, in English and French.
    /// </summary>
    [Fact]
    public void NewGameOfTheGameLanguageIsShownInTheChosenLanguage()
    {
        Assert.Equal("New Game", RenderSaveName("Neues Spiel", GameTexts.English, Texts("Neues Spiel"), English));
        Assert.Equal("Nouvelle partie", RenderSaveName("Neues Spiel", Texts("Nouvelle partie"), Texts("Neues Spiel"), CultureInfo.GetCultureInfo("fr-FR")));
    }

    /// <summary>
    /// A save name other than the game's "New Game" text is shown unchanged.
    /// </summary>
    [Fact]
    public void NewGameOtherNamesStayAsTheyAre()
        => Assert.Equal("Archipelago2", RenderSaveName("Archipelago2", GameTexts.English, Texts("Neues Spiel"), English));

    /// <summary>
    /// Without game language texts, a save name that looks like a "New Game" text in another language is shown unchanged.
    /// </summary>
    [Fact]
    public void NewGameWithoutGameLanguageTextsNothingIsMapped()
        => Assert.Equal("Neues Spiel", RenderSaveName("Neues Spiel", GameTexts.English, gameLanguage: null, English));

    /// <summary>
    /// When the game language is English, the save name "New Game" is shown with the German "Neues Spiel" of the chosen language.
    /// </summary>
    [Fact]
    public void NewGameWithAnEnglishGameLanguageIsShownInTheChosenLanguage()
        => Assert.Equal("Neues Spiel", RenderSaveName("New Game", Texts("Neues Spiel"), GameTexts.English, German));

    /// <summary>
    /// The "New Game" comparison is case-sensitive, so "neues spiel" stays as it is, and a missing save name gives the "Playing" fallback as details.
    /// </summary>
    [Fact]
    public void NewGameComparesOrdinallyAndKeepsAMissingName()
    {
        Assert.Equal("neues spiel", RenderSaveName("neues spiel", GameTexts.English, Texts("Neues Spiel"), English));
        Assert.Equal("Playing", RenderSaveName(saveName: null, GameTexts.English, Texts("Neues Spiel"), English));
    }

    #endregion New Game
}
