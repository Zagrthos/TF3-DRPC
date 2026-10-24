using System;
using System.Collections.Generic;
using System.Globalization;

using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Presence;
using Tf3Drpc.Core.Status;
using Tf3Drpc.Core.Structs;
using Tf3Drpc.Core.Translation;

using Xunit;

namespace Tf3Drpc.Core.Tests.Presence;

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

    [Theory]
    [InlineData(GameState.NotRunning)]
    [InlineData(GameState.Closing)]
    public void NotRunningAndClosing_ClearThePresence(GameState state)
    {
        bool built = PresenceBuilder.TryBuild(Snapshot(state), English, GameTexts.English, out PresenceContentStruct _);

        Assert.False(built);
    }

    [Theory]
    [InlineData("en-US", "Starting the game", "In the main menu")]
    [InlineData("de-DE", "Spiel wird gestartet", "Im Hauptmenü")]
    [InlineData("ja-JP", "ゲームを起動中", "メインメニュー")]
    public void StartingAndMainMenu_UseOwnTexts(string culture, string starting, string mainMenu)
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

    [Fact]
    public void Loading_UsesTheGameLabel()
    {
        GameTexts german = new(new Dictionary<string, string>(StringComparer.Ordinal) { ["Loading..."] = "Laden..." });

        Assert.Equal("Loading...", Build(Snapshot(GameState.Loading), English).Details);
        Assert.Equal("Laden...", Build(Snapshot(GameState.Loading), German, german).Details);
    }

    [Fact]
    public void Mission_ShowsCampaignAndName()
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

    [Theory]
    [InlineData(null)]
    [InlineData(0)]
    [InlineData(9)]
    public void Mission_WithoutKnownNumberShowsOnlyCampaign(int? mission)
    {
        PresenceContentStruct content = Build(Snapshot(GameState.Mission, mission), English);

        Assert.Equal("Campaign", content.Details);
        Assert.Null(content.State);
    }

    [Theory]
    [InlineData("en-US", "Playing")]
    [InlineData("de-DE", "Spielt")]
    [InlineData("ja-JP", "プレイ中")]
    public void PlayingWithoutStatus_UsesOwnText(string culture, string expected)
    {
        PresenceContentStruct content = Build(Snapshot(GameState.Playing), CultureInfo.GetCultureInfo(culture));

        Assert.Equal(expected, content.Details);
        Assert.Null(content.State);
        Assert.Equal(Timer, content.StartTimestamp);
    }

    [Theory]
    [InlineData("en-US", "Mechanic", "September 1905")]
    [InlineData("de-DE", "Mechaniker", "September 1905")]
    [InlineData("ja-JP", "Mechaniker", "1905年9月")]
    public void DesignA_ShowsRankAndDate(string culture, string rankName, string date)
    {
        GameTexts texts = new(new Dictionary<string, string>(StringComparer.Ordinal) { ["Mechanic"] = rankName });

        PresenceContentStruct content = Build(Snapshot(GameState.Playing, status: Career()), CultureInfo.GetCultureInfo(culture), texts);

        Assert.Equal(rankName, content.Details);
        Assert.Equal(date, content.State);
        Assert.Equal(Timer, content.StartTimestamp);
    }

    [Fact]
    public void DesignA_IgnoresTheRankNameOfTheStatus()
    {
        ModStatus status = Career() with { RankName = "Something else" };

        Assert.Equal("Mechanic", Build(Snapshot(GameState.Playing, status: status), English).Details);
    }

    [Theory]
    [InlineData(false, false, true, "Mechanic · Tycoon")]
    [InlineData(false, true, false, "Mechanic · Sandbox Mode")]
    [InlineData(true, false, false, "Mechanic · Map Editor")]
    [InlineData(true, true, true, "Mechanic · Map Editor")]
    [InlineData(false, true, true, "Mechanic · Sandbox Mode")]
    [InlineData(false, false, false, "Mechanic")]
    public void DesignA_AppendsTheModeByPriority(bool mapEditor, bool sandbox, bool tycoon, string expected)
    {
        ModStatus status = Career() with { IsMapEditor = mapEditor, IsSandbox = sandbox, IsTycoon = tycoon };

        Assert.Equal(expected, Build(Snapshot(GameState.Playing, status: status), English).Details);
    }

    [Fact]
    public void DesignA_WithoutRankShowsTheModeOrPlaying()
    {
        ModStatus sandbox = Career(rank: null) with { IsSandbox = true };

        Assert.Equal("Sandbox Mode", Build(Snapshot(GameState.Playing, status: sandbox), English).Details);
        Assert.Equal("Playing", Build(Snapshot(GameState.Playing, status: Career(rank: null)), English).Details);
        Assert.Equal("Playing", Build(Snapshot(GameState.Playing, status: Career(rank: 0)), English).Details);
        Assert.Equal("Playing", Build(Snapshot(GameState.Playing, status: Career(rank: 16)), English).Details);
        Assert.Equal("Spielt", Build(Snapshot(GameState.Playing, status: Career(rank: null)), German).Details);
    }

    [Fact]
    public void DesignA_RanksOneAndFifteenWork()
    {
        Assert.Equal("Junior", Build(Snapshot(GameState.Playing, status: Career(rank: 1)), English).Details);
        Assert.Equal("Tycoon", Build(Snapshot(GameState.Playing, status: Career(rank: 15)), English).Details);
    }

    [Fact]
    public void DesignA_TycoonGameNamesTycoonOnce()
    {
        ModStatus status = Career(rank: 15) with { IsTycoon = true };

        Assert.Equal("Tycoon", Build(Snapshot(GameState.Playing, status: status), English).Details);
    }

    [Theory]
    [InlineData(null, 9)]
    [InlineData(1905, null)]
    [InlineData(0, 9)]
    [InlineData(10000, 9)]
    [InlineData(1905, 0)]
    [InlineData(1905, 13)]
    public void DesignA_WithMissingOrInvalidDateHasNoState(int? year, int? month)
    {
        PresenceContentStruct content = Build(Snapshot(GameState.Playing, status: Career(year: year, month: month)), English);

        Assert.Equal("Mechanic", content.Details);
        Assert.Null(content.State);
    }

    [Fact]
    public void DesignA_StaleStatusIsShownAsPaused()
    {
        PresenceContentStruct content = Build(Snapshot(GameState.Playing, status: Career(speedup: 4), paused: true), English);

        Assert.Equal("Mechanic", content.Details);
        Assert.Equal("September 1905 \u00B7 Paused", content.State);
    }

    [Fact]
    public void DesignA_PausedAppendsTheLabel()
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

    [Fact]
    public void LongDetails_AreShortenedWithAnEllipsis()
    {
        GameTexts texts = new(new Dictionary<string, string>(StringComparer.Ordinal) { ["MISSION_01_NAME"] = new string('x', 300) });

        PresenceContentStruct content = Build(Snapshot(GameState.Mission, 1), English, texts);

        Assert.Equal(128, content.Details.Length);
        Assert.EndsWith("…", content.Details, StringComparison.Ordinal);
    }

    [Fact]
    public void TextExactlyAtTheLimit_IsKept()
    {
        string name = new('x', 128 - "Campaign · ".Length);
        GameTexts texts = new(new Dictionary<string, string>(StringComparer.Ordinal) { ["MISSION_01_NAME"] = name });

        PresenceContentStruct content = Build(Snapshot(GameState.Mission, 1), English, texts);

        Assert.Equal(128, content.Details.Length);
        Assert.DoesNotContain("…", content.Details, StringComparison.Ordinal);
    }

    [Fact]
    public void Shortening_DoesNotCutSurrogatePairs()
    {
        GameTexts texts = new(new Dictionary<string, string>(StringComparer.Ordinal) { ["Mechanic"] = new string('x', 126) + "\U0001F600\U0001F600" });

        PresenceContentStruct content = Build(Snapshot(GameState.Playing, status: Career()), English, texts);

        Assert.StartsWith(new string('x', 126), content.Details, StringComparison.Ordinal);
        Assert.EndsWith("…", content.Details, StringComparison.Ordinal);
        Assert.True(content.Details.Length is <= 128);
        AssertNoLoneSurrogates(content.Details);
    }

    [Fact]
    public void Shortening_KeepsTextElementsWhole()
    {
        string accented = string.Concat(System.Linq.Enumerable.Repeat("é", 100));
        GameTexts texts = new(new Dictionary<string, string>(StringComparer.Ordinal) { ["Mechanic"] = accented });

        PresenceContentStruct content = Build(Snapshot(GameState.Playing, status: Career()), English, texts);

        string withoutEllipsis = content.Details[..^1];
        Assert.True(content.Details.Length is <= 128);
        Assert.Equal(0, withoutEllipsis.Length % 2);
        Assert.EndsWith("́", withoutEllipsis, StringComparison.Ordinal);
    }

    [Fact]
    public void LongState_IsShortenedToo()
    {
        GameTexts texts = new(new Dictionary<string, string>(StringComparer.Ordinal) { ["Paused"] = new string('p', 200) });

        PresenceContentStruct content = Build(Snapshot(GameState.Playing, status: Career(speedup: 0)), English, texts);

        Assert.NotNull(content.State);
        Assert.Equal(128, content.State.Length);
        Assert.EndsWith("…", content.State, StringComparison.Ordinal);
    }

    [Fact]
    public void ShortDetails_ArePaddedToTwoCharacters()
    {
        GameTexts texts = new(new Dictionary<string, string>(StringComparer.Ordinal) { ["Mechanic"] = "X" });

        PresenceContentStruct content = Build(Snapshot(GameState.Playing, status: Career()), English, texts);

        Assert.Equal("X ", content.Details);
    }

    [Fact]
    public void ShortState_IsDropped()
    {
        GameTexts texts = new(new Dictionary<string, string>(StringComparer.Ordinal) { ["Paused"] = "p" });

        PresenceContentStruct content = Build(Snapshot(GameState.Playing, status: Career(year: null, speedup: 0)), English, texts);

        Assert.Equal("Mechanic", content.Details);
        Assert.Null(content.State);
    }

    [Fact]
    public void Contents_AreEqualByValue()
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
