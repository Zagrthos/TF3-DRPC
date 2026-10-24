using System;
using System.Collections.Generic;

using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.GameLog;
using Tf3Drpc.Core.Status;
using Tf3Drpc.Core.Structs;

using Xunit;

namespace Tf3Drpc.Core.Tests.GameLog;

public sealed class GameLogEventClassifierTests
{
    private static readonly DateTimeOffset Time = new(2026, 10, 13, 21, 0, 0, TimeSpan.Zero);

    private static GameLogEntryStruct Entry(string message)
        => new() { Timestamp = Time, Level = GameLogLevel.Message, Message = message };

    [Fact]
    public void SessionLog_ProducesTheExpectedEventSequence()
    {
        List<TimedEvent> events = SampleFiles.ReadEvents("session.log");

        Assert.Equal
        (
            [
                GameLogEventKind.Startup,
                GameLogEventKind.MainMenu,
                GameLogEventKind.LoadingGame,
                GameLogEventKind.GameReady,
                GameLogEventKind.Status,
                GameLogEventKind.Status,
                GameLogEventKind.Status,
                GameLogEventKind.GameStopped,
                GameLogEventKind.MainMenu,
                GameLogEventKind.Closing
            ],
            events.ConvertAll(static timed => timed.Event.Kind)
        );
        Assert.Equal("Archipelago2", events[2].Event.SaveName);
        Assert.False(events[9].Event.IsCrash);
    }

    [Fact]
    public void SessionLog_StatusLineIsDeserialised()
    {
        List<TimedEvent> events = SampleFiles.ReadEvents("session.log");

        ModStatus? status = events[4].Event.Status;

        Assert.NotNull(status);
        Assert.Equal(1, status.Version);
        Assert.Equal(1905, status.Year);
        Assert.Equal(9, status.Month);
        Assert.Equal(30, status.Day);
        Assert.Equal(4, status.Speedup);
        Assert.Equal(2, status.Rank);
        Assert.Equal("Mechaniker", status.RankName);
        Assert.Equal(14422756, status.Balance);
        Assert.Equal(1791926125, status.Timestamp);
        Assert.Equal(44, status.CompanyNumberOfLines);
        Assert.Equal(55956242d, status.CompanyTotalAssets);
        Assert.Equal(8.3333, status.CompanyTopSpeed);
        Assert.Equal(0.522476, status.CloudCoverage);
        Assert.Equal(18620d, status.TimeOfDaySec);
        Assert.Equal("::/climates/tropical/tropical.clima", status.Climate);
        Assert.Equal("::/economy/tropic.eco", status.Economy);
        Assert.Equal("Archipelago2", status.SaveName);
        Assert.False(status.InfiniteMoney);
        Assert.False(status.IsTycoon);
    }

    [Fact]
    public void MissionLog_ProducesMissionEventWithNumber()
    {
        List<TimedEvent> events = SampleFiles.ReadEvents("mission.log");

        Assert.Equal
        (
            [
                GameLogEventKind.MainMenu,
                GameLogEventKind.MissionLoading,
                GameLogEventKind.LoadingGame,
                GameLogEventKind.GameReady
            ],
            events.ConvertAll(static timed => timed.Event.Kind)
        );
        Assert.Equal(1, events[1].Event.MissionNumber);
        Assert.Equal("savegame", events[2].Event.SaveName);
    }

    [Fact]
    public void CrashLog_ProducesCrashClosing()
    {
        List<TimedEvent> events = SampleFiles.ReadEvents("crash.log");

        TimedEvent timed = Assert.Single(events);
        Assert.Equal(GameLogEventKind.Closing, timed.Event.Kind);
        Assert.True(timed.Event.IsCrash);
    }

    [Theory]
    [InlineData("Campaign/mission: some_campaign::/info.campaign/some_mission::/info.mission")]
    [InlineData("Campaign/mission: ")]
    [InlineData("Campaign/mission: x_mission_99999999999::/info.mission")]
    public void MissionWithoutUsableNumberHasNullNumber(string message)
    {
        GameLogEntryStruct entry = Entry(message);

        bool classified = GameLogEventClassifier.TryClassify(entry, out GameLogEventStruct gameEvent);

        Assert.True(classified);
        Assert.Equal(GameLogEventKind.MissionLoading, gameEvent.Kind);
        Assert.Null(gameEvent.MissionNumber);
    }

    [Theory]
    [InlineData("")]
    [InlineData("game is ready")]
    [InlineData("Game is ready.")]
    [InlineData("Loading config from file savegame")]
    [InlineData("Shutting down PDX SDK")]
    [InlineData("Quit requested")]
    [InlineData("[drpc] reading year failed")]
    [InlineData("[drpc-status] {\"v\":1,\"year\":1900")]
    [InlineData("[drpc-status] {\"v\":1,\"balance\":inf}")]
    [InlineData("[drpc-status] {\"v\":1,\"timeOfDaySec\":nan}")]
    [InlineData("[drpc-status] {\"v\":2,\"year\":1900}")]
    [InlineData("[drpc-status] {\"year\":1900}")]
    [InlineData("[drpc-status] {\"v\":1,\"year\":\"text\"}")]
    [InlineData("[drpc-status] null")]
    [InlineData("[drpc-status] ")]
    public void UnknownOrBrokenMessagesGiveNoEvent(string message)
    {
        GameLogEntryStruct entry = Entry(message);

        Assert.False(GameLogEventClassifier.TryClassify(entry, out GameLogEventStruct _));
    }

    [Fact]
    public void StatusWithUnknownKeysAndFractionalNumbersIsAccepted()
    {
        GameLogEntryStruct entry = Entry("[drpc-status] {\"v\":1,\"future\":{\"a\":[1,2]},\"company_numberOfLines\":3,\"company_debt\":1.5e+06}");

        bool classified = GameLogEventClassifier.TryClassify(entry, out GameLogEventStruct gameEvent);

        Assert.True(classified);
        Assert.NotNull(gameEvent.Status);
        Assert.Equal(3, gameEvent.Status.CompanyNumberOfLines);
        Assert.Equal(1_500_000d, gameEvent.Status.CompanyDebt);
        Assert.Null(gameEvent.Status.Year);
    }

    [Fact]
    public void StatusWithFractionalIntegerKey_IsRoundedTowardZero()
    {
        GameLogEntryStruct entry = Entry("[drpc-status] {\"v\":1,\"balance\":1.5,\"loan\":-2.9,\"year\":1900}");

        bool classified = GameLogEventClassifier.TryClassify(entry, out GameLogEventStruct gameEvent);

        Assert.True(classified);
        Assert.NotNull(gameEvent.Status);
        Assert.Equal(1, gameEvent.Status.Balance);
        Assert.Equal(-2, gameEvent.Status.Loan);
        Assert.Equal(1900, gameEvent.Status.Year);
    }

    [Fact]
    public void StatusWithExponentIntegerKey_IsExpanded()
    {
        GameLogEntryStruct entry = Entry("[drpc-status] {\"v\":1,\"balance\":1.44228e+07}");

        bool classified = GameLogEventClassifier.TryClassify(entry, out GameLogEventStruct gameEvent);

        Assert.True(classified);
        Assert.NotNull(gameEvent.Status);
        Assert.Equal(14_422_800, gameEvent.Status.Balance);
    }

    [Theory]
    [InlineData("1e30")]
    [InlineData("-1e30")]
    [InlineData("1e999")]
    [InlineData("9223372036854775808")]
    public void StatusWithOutOfRangeIntegerKey_GivesNullForThatKeyOnly(string number)
    {
        GameLogEntryStruct entry = Entry("[drpc-status] {\"v\":1,\"balance\":" + number + ",\"year\":1900}");

        bool classified = GameLogEventClassifier.TryClassify(entry, out GameLogEventStruct gameEvent);

        Assert.True(classified);
        Assert.NotNull(gameEvent.Status);
        Assert.Null(gameEvent.Status.Balance);
        Assert.Equal(1900, gameEvent.Status.Year);
    }

    [Fact]
    public void StartupLog_ProducesStartupThenTheGameBuild()
    {
        List<TimedEvent> events = SampleFiles.ReadEvents("startup.log");

        Assert.Equal([GameLogEventKind.Startup, GameLogEventKind.GameBuild], events.ConvertAll(static timed => timed.Event.Kind));
        Assert.Equal(40420, events[1].Event.GameBuild);
    }

    [Theory]
    [InlineData("Starting up build version: , build name \"Build\"")]
    [InlineData("Starting up build version: abc, build name \"Build\"")]
    [InlineData("Starting up build version: 99999999999, build name \"Build\"")]
    [InlineData("Starting up build version: -5, build name \"Build\"")]
    [InlineData("Starting up build version: 40420")]
    [InlineData("Build: Build 40420 Windows 64-bit")]
    public void GameBuildLineWithoutAUsableNumber_IsNotClassified(string message)
    {
        bool classified = GameLogEventClassifier.TryClassify(Entry(message), out GameLogEventStruct gameEvent);

        Assert.False(classified);
        Assert.Null(gameEvent.GameBuild);
    }
}
