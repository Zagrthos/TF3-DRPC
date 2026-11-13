using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Presence;
using Tf3Drpc.Core.Status;
using Tf3Drpc.Core.Structs;
using Tf3Drpc.Core.Translation;

using Xunit;

namespace Tf3Drpc.Core.Tests.Presence;

/// <summary>
/// Tests of <see cref="PresenceBuilder"/>, which turns a game state snapshot into the Discord presence text, covering each state, the rank and date line, the paused label and the length limits.
/// </summary>
public sealed class PresenceBuilderTests
{
    private static readonly CultureInfo English = CultureInfo.GetCultureInfo("en-US");
    private static readonly CultureInfo German = CultureInfo.GetCultureInfo("de-DE");
    private static readonly CultureInfo Japanese = CultureInfo.GetCultureInfo("ja-JP");
    private static readonly DateTimeOffset Timer = new(2026, 10, 13, 21, 15, 24, TimeSpan.Zero);

    private static GameStateSnapshotStruct Snapshot(GameState state, int? mission = null, ModStatus? status = null, bool? paused = null)
        => new() { State = state, MissionNumber = mission, TimerStart = Timer, Status = status, IsPaused = paused ?? (status?.Speedup is 0) };

    private static ModStatus Career(long? rank = 2, long? year = 1905, long? month = 9, long? speedup = 1)
        => new() { Version = 1, Rank = rank, Year = year, Month = month, Speedup = speedup };

    private static PresenceContentStruct Build(in GameStateSnapshotStruct snapshot, CultureInfo culture, GameTexts? texts = null)
    {
        bool built = PresenceBuilder.TryBuild(snapshot, culture, texts ?? GameTexts.English, out PresenceContentStruct content);

        Assert.True(built);

        return content;
    }

    /// <summary>
    /// A game that is not running or is closing builds no presence, so any existing presence is cleared.
    /// </summary>
    [Theory]
    [InlineData(GameState.NotRunning)]
    [InlineData(GameState.Closing)]
    internal void NotRunningAndClosingClearThePresence(GameState state)
    {
        bool built = PresenceBuilder.TryBuild(Snapshot(state), English, GameTexts.English, out PresenceContentStruct _);

        Assert.False(built);
    }

    /// <summary>
    /// A game state outside the enum values throws <see cref="UnreachableException"/> instead of building a presence.
    /// </summary>
    [Fact]
    public void AStateOutsideTheEnumIsUnreachable()
    {
        GameStateSnapshotStruct snapshot = Snapshot((GameState)99);

        Assert.Throws<UnreachableException>(() => PresenceBuilder.TryBuild(snapshot, English, GameTexts.English, out PresenceContentStruct _));
    }

    /// <summary>
    /// The starting and main menu states show their own localised text as details, with no state line and the timer start as the timestamp, in English, German and Japanese.
    /// </summary>
    [Theory]
    [InlineData("en-US", "Starting the game", "In the main menu")]
    [InlineData("de-DE", "Spiel wird gestartet", "Im Hauptmenü")]
    [InlineData("ja-JP", "ゲームを起動中", "メインメニュー")]
    public void StartingAndMainMenuUseOwnTexts(string culture, string starting, string mainMenu)
    {
        CultureInfo info = CultureInfo.GetCultureInfo(culture);

        PresenceContentStruct first = Build(Snapshot(GameState.Starting), info);
        PresenceContentStruct second = Build(Snapshot(GameState.MainMenu), info);

        Assert.Equal(starting, first.Details);
        Assert.Null(first.State);
        Assert.Equal(Timer, first.StartTimestamp);
        Assert.Equal(mainMenu, second.Details);
        Assert.Null(second.State);
        Assert.Equal(Timer, second.StartTimestamp);
    }

    /// <summary>
    /// The loading state shows the game's own "Loading..." label, translated when the game texts provide a translation.
    /// </summary>
    [Fact]
    public void LoadingUsesTheGameLabel()
    {
        GameTexts german = new(new Dictionary<string, string>(StringComparer.Ordinal) { ["Loading..."] = "Laden..." });

        Assert.Equal("Loading...", Build(Snapshot(GameState.Loading), English).Details);
        Assert.Equal("Laden...", Build(Snapshot(GameState.Loading), German, german).Details);
    }

    /// <summary>
    /// A mission shows the campaign label and the mission name from the game texts, with the timer start as the timestamp.
    /// </summary>
    [Fact]
    public void MissionShowsCampaignAndName()
    {
        GameTexts german = new(new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["Campaign"] = "Kampagne",
            ["MISSION_01_NAME"] = "Mardi Gras retten"
        });

        Assert.Equal("Campaign · Saving Mardi Gras", Build(Snapshot(GameState.Mission, 1), English).Details);
        Assert.Equal("Kampagne · Mardi Gras retten", Build(Snapshot(GameState.Mission, 1), German, german).Details);
        Assert.Equal(Timer, Build(Snapshot(GameState.Mission, 1), English).StartTimestamp);
    }

    /// <summary>
    /// A mission with no number, or with a number that has no mission name (0 and 9), shows only the campaign label and no state line.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData(0)]
    [InlineData(9)]
    public void MissionWithoutKnownNumberShowsOnlyCampaign(int? mission)
    {
        PresenceContentStruct content = Build(Snapshot(GameState.Mission, mission), English);

        Assert.Equal("Campaign", content.Details);
        Assert.Null(content.State);
    }

    /// <summary>
    /// Playing without a mod status shows a localised "Playing" text with no state line and the timer start as the timestamp.
    /// </summary>
    [Theory]
    [InlineData("en-US", "Playing")]
    [InlineData("de-DE", "Spielt")]
    [InlineData("ja-JP", "プレイ中")]
    public void PlayingWithoutStatusUsesOwnText(string culture, string expected)
    {
        PresenceContentStruct content = Build(Snapshot(GameState.Playing), CultureInfo.GetCultureInfo(culture));

        Assert.Equal(expected, content.Details);
        Assert.Null(content.State);
        Assert.Equal(Timer, content.StartTimestamp);
    }

    /// <summary>
    /// In design A the career shows the translated rank name as details and the culture-formatted month and year as the state line, for English, German and Japanese.
    /// </summary>
    [Theory]
    [InlineData("en-US", "Mechanic", "September 1905")]
    [InlineData("de-DE", "Mechaniker", "September 1905")]
    [InlineData("ja-JP", "Mechaniker", "1905年9月")]
    public void DesignAShowsRankAndDate(string culture, string rankName, string date)
    {
        GameTexts texts = new(new Dictionary<string, string>(StringComparer.Ordinal) { ["Mechanic"] = rankName });

        PresenceContentStruct content = Build(Snapshot(GameState.Playing, status: Career()), CultureInfo.GetCultureInfo(culture), texts);

        Assert.Equal(rankName, content.Details);
        Assert.Equal(date, content.State);
        Assert.Equal(Timer, content.StartTimestamp);
    }

    /// <summary>
    /// In design A the rank name comes from the game texts for the rank number, so a different rank name sent in the status is ignored.
    /// </summary>
    [Fact]
    public void DesignAIgnoresTheRankNameOfTheStatus()
    {
        ModStatus status = Career() with { RankName = "Something else" };

        Assert.Equal("Mechanic", Build(Snapshot(GameState.Playing, status: status), English).Details);
    }

    /// <summary>
    /// In design A the game mode is appended to the rank name with priority map editor, then sandbox, then tycoon, and nothing is appended when no mode flag is set.
    /// </summary>
    [Theory]
    [InlineData(false, false, true, "Mechanic · Tycoon")]
    [InlineData(false, true, false, "Mechanic · Sandbox Mode")]
    [InlineData(true, false, false, "Mechanic · Map Editor")]
    [InlineData(true, true, true, "Mechanic · Map Editor")]
    [InlineData(false, true, true, "Mechanic · Sandbox Mode")]
    [InlineData(false, false, false, "Mechanic")]
    public void DesignAAppendsTheModeByPriority(bool mapEditor, bool sandbox, bool tycoon, string expected)
    {
        ModStatus status = Career() with { IsMapEditor = mapEditor, IsSandbox = sandbox, IsTycoon = tycoon };

        Assert.Equal(expected, Build(Snapshot(GameState.Playing, status: status), English).Details);
    }

    /// <summary>
    /// In design A without a usable rank (missing, 0 or 16) the details show the mode name or "Playing", localised for German.
    /// </summary>
    [Fact]
    public void DesignAWithoutRankShowsTheModeOrPlaying()
    {
        ModStatus sandbox = Career(rank: null) with { IsSandbox = true };

        Assert.Equal("Sandbox Mode", Build(Snapshot(GameState.Playing, status: sandbox), English).Details);
        Assert.Equal("Playing", Build(Snapshot(GameState.Playing, status: Career(rank: null)), English).Details);
        Assert.Equal("Playing", Build(Snapshot(GameState.Playing, status: Career(rank: 0)), English).Details);
        Assert.Equal("Playing", Build(Snapshot(GameState.Playing, status: Career(rank: 16)), English).Details);
        Assert.Equal("Spielt", Build(Snapshot(GameState.Playing, status: Career(rank: null)), German).Details);
    }

    /// <summary>
    /// In design A rank 1 is shown as "Junior" and rank 15 as "Tycoon", the two ends of the valid range.
    /// </summary>
    [Fact]
    public void DesignARanksOneAndFifteenWork()
    {
        Assert.Equal("Junior", Build(Snapshot(GameState.Playing, status: Career(rank: 1)), English).Details);
        Assert.Equal("Tycoon", Build(Snapshot(GameState.Playing, status: Career(rank: 15)), English).Details);
    }

    /// <summary>
    /// In design A a tycoon game at rank 15 shows "Tycoon" once, not repeated as the mode suffix.
    /// </summary>
    [Fact]
    public void DesignATycoonGameNamesTycoonOnce()
    {
        ModStatus status = Career(rank: 15) with { IsTycoon = true };

        Assert.Equal("Tycoon", Build(Snapshot(GameState.Playing, status: status), English).Details);
    }

    /// <summary>
    /// In design A a missing or out-of-range year (0, 10000) or month (0, 13) leaves the state line empty while the rank name is still shown.
    /// </summary>
    [Theory]
    [InlineData(null, 9)]
    [InlineData(1905, null)]
    [InlineData(0, 9)]
    [InlineData(10000, 9)]
    [InlineData(1905, 0)]
    [InlineData(1905, 13)]
    public void DesignAWithMissingOrInvalidDateHasNoState(int? year, int? month)
    {
        PresenceContentStruct content = Build(Snapshot(GameState.Playing, status: Career(year: year, month: month)), English);

        Assert.Equal("Mechanic", content.Details);
        Assert.Null(content.State);
    }

    /// <summary>
    /// In design A a status flagged as stale is shown as paused even though its speed-up is still 4.
    /// </summary>
    [Fact]
    public void DesignAStaleStatusIsShownAsPaused()
    {
        PresenceContentStruct content = Build(Snapshot(GameState.Playing, status: Career(speedup: 4), paused: true), English);

        Assert.Equal("Mechanic", content.Details);
        Assert.Equal("September 1905 \u00B7 Paused", content.State);
    }

    /// <summary>
    /// In design A a speed-up of 0 appends the localised "Paused" label to the date, or shows it alone without a date, while a running or unknown speed-up leaves the date unchanged.
    /// </summary>
    [Fact]
    public void DesignAPausedAppendsTheLabel()
    {
        GameTexts german = new(new Dictionary<string, string>(StringComparer.Ordinal) { ["Paused"] = "Pausiert" });

        PresenceContentStruct paused = Build(Snapshot(GameState.Playing, status: Career(speedup: 0)), English);
        PresenceContentStruct pausedGerman = Build(Snapshot(GameState.Playing, status: Career(speedup: 0)), German, german);
        PresenceContentStruct pausedJapanese = Build(Snapshot(GameState.Playing, status: Career(speedup: 0)), Japanese);
        PresenceContentStruct pausedWithoutDate = Build(Snapshot(GameState.Playing, status: Career(year: null, speedup: 0)), English);
        PresenceContentStruct running = Build(Snapshot(GameState.Playing, status: Career(speedup: 4)), English);
        PresenceContentStruct unknownSpeed = Build(Snapshot(GameState.Playing, status: Career(speedup: null)), English);

        Assert.Equal("September 1905 · Paused", paused.State);
        Assert.Equal("September 1905 · Pausiert", pausedGerman.State);
        Assert.Equal("1905年9月 · Paused", pausedJapanese.State);
        Assert.Equal("Paused", pausedWithoutDate.State);
        Assert.Equal("September 1905", running.State);
        Assert.Equal("September 1905", unknownSpeed.State);
    }

    /// <summary>
    /// Details longer than the 128-character limit are cut to exactly 128 characters and end with an ellipsis.
    /// </summary>
    [Fact]
    public void LongDetailsAreShortenedWithAnEllipsis()
    {
        GameTexts texts = new(new Dictionary<string, string>(StringComparer.Ordinal) { ["MISSION_01_NAME"] = new string('x', 300) });

        PresenceContentStruct content = Build(Snapshot(GameState.Mission, 1), English, texts);

        Assert.Equal(128, content.Details.Length);
        Assert.EndsWith("…", content.Details, StringComparison.Ordinal);
    }

    /// <summary>
    /// Text of exactly 128 characters is kept whole, with no ellipsis added.
    /// </summary>
    [Fact]
    public void TextExactlyAtTheLimitIsKept()
    {
        string name = new('x', 128 - "Campaign · ".Length);
        GameTexts texts = new(new Dictionary<string, string>(StringComparer.Ordinal) { ["MISSION_01_NAME"] = name });

        PresenceContentStruct content = Build(Snapshot(GameState.Mission, 1), English, texts);

        Assert.Equal(128, content.Details.Length);
        Assert.DoesNotContain("…", content.Details, StringComparison.Ordinal);
    }

    /// <summary>
    /// Shortening an over-long text never splits a surrogate pair, so the result has no lone surrogates and stays within 128 characters.
    /// </summary>
    [Fact]
    public void ShorteningDoesNotCutSurrogatePairs()
    {
        GameTexts texts = new(new Dictionary<string, string>(StringComparer.Ordinal) { ["Mechanic"] = new string('x', 126) + "\U0001F600\U0001F600" });

        PresenceContentStruct content = Build(Snapshot(GameState.Playing, status: Career()), English, texts);

        Assert.StartsWith(new string('x', 126), content.Details, StringComparison.Ordinal);
        Assert.EndsWith("…", content.Details, StringComparison.Ordinal);
        Assert.True(content.Details.Length is <= 128);
        AssertNoLoneSurrogates(content.Details);
    }

    /// <summary>
    /// Shortening an over-long text never splits a text element, so a base letter and its combining accent are cut together.
    /// </summary>
    [Fact]
    public void ShorteningKeepsTextElementsWhole()
    {
        string accented = string.Concat(System.Linq.Enumerable.Repeat("é", 100));
        GameTexts texts = new(new Dictionary<string, string>(StringComparer.Ordinal) { ["Mechanic"] = accented });

        PresenceContentStruct content = Build(Snapshot(GameState.Playing, status: Career()), English, texts);

        string withoutEllipsis = content.Details[..^1];
        Assert.True(content.Details.Length is <= 128);
        Assert.Equal(0, withoutEllipsis.Length % 2);
        Assert.EndsWith("́", withoutEllipsis, StringComparison.Ordinal);
    }

    /// <summary>
    /// A state line longer than the 128-character limit is also cut to exactly 128 characters and ends with an ellipsis.
    /// </summary>
    [Fact]
    public void LongStateIsShortenedToo()
    {
        GameTexts texts = new(new Dictionary<string, string>(StringComparer.Ordinal) { ["Paused"] = new string('p', 200) });

        PresenceContentStruct content = Build(Snapshot(GameState.Playing, status: Career(speedup: 0)), English, texts);

        Assert.NotNull(content.State);
        Assert.Equal(128, content.State.Length);
        Assert.EndsWith("…", content.State, StringComparison.Ordinal);
    }

    /// <summary>
    /// Details of a single character are padded with a space to the two-character minimum Discord requires.
    /// </summary>
    [Fact]
    public void ShortDetailsArePaddedToTwoCharacters()
    {
        GameTexts texts = new(new Dictionary<string, string>(StringComparer.Ordinal) { ["Mechanic"] = "X" });

        PresenceContentStruct content = Build(Snapshot(GameState.Playing, status: Career()), English, texts);

        Assert.Equal("X ", content.Details);
    }

    /// <summary>
    /// A state line shorter than two characters is dropped, leaving no state.
    /// </summary>
    [Fact]
    public void ShortStateIsDropped()
    {
        GameTexts texts = new(new Dictionary<string, string>(StringComparer.Ordinal) { ["Paused"] = "p" });

        PresenceContentStruct content = Build(Snapshot(GameState.Playing, status: Career(year: null, speedup: 0)), English, texts);

        Assert.Equal("Mechanic", content.Details);
        Assert.Null(content.State);
    }

    /// <summary>
    /// Presence contents with the same values are equal, and contents built from different ranks are not, under both value equality and the equality operators.
    /// </summary>
    [Fact]
    public void ContentsAreEqualByValue()
    {
        PresenceContentStruct first = Build(Snapshot(GameState.Playing, status: Career()), English);
        PresenceContentStruct second = Build(Snapshot(GameState.Playing, status: Career()), English);
        PresenceContentStruct other = Build(Snapshot(GameState.Playing, status: Career(rank: 3)), English);

        Assert.Equal(first, second);
        Assert.True(first == second);
        Assert.NotEqual(first, other);
        Assert.True(first != other);
    }

    private static void AssertNoLoneSurrogates(string text)
    {
        int highSurrogates = 0;
        int lowSurrogates = 0;

        for (int i = 0; i < text.Length; i++)
        {
            if (char.IsHighSurrogate(text[i]))
            {
                highSurrogates++;
                Assert.True((i + 1 < text.Length) && char.IsLowSurrogate(text[i + 1]));
            }
            else if (char.IsLowSurrogate(text[i]))
            {
                lowSurrogates++;
                Assert.True((i is > 0) && char.IsHighSurrogate(text[i - 1]));
            }
        }

        Assert.Equal(highSurrogates, lowSurrogates);
    }
}
