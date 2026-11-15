using System;
using System.Collections.Generic;

using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.GameLog;
using Tf3Drpc.Core.Status;
using Tf3Drpc.Core.Structs;

using Xunit;

namespace Tf3Drpc.Core.Tests.GameLog;

/// <summary>
/// Tests of <see cref="GameLogEventClassifier"/>, which turns game log entries into events such as startup, loading, status and closing, using real sample logs and hand-written messages.
/// </summary>
public sealed class GameLogEventClassifierTests
{
    private static readonly DateTimeOffset Time = new(2026, 10, 13, 21, 0, 0, TimeSpan.Zero);

    private static GameLogEntryStruct Entry(string message)
        => new() { Timestamp = Time, Level = GameLogLevel.Message, Message = message };

    /// <summary>
    /// The sample session log produces the events startup, main menu, loading game, game ready, three status events, game stopped, main menu and a closing event that is not a crash, with the save name "Archipelago2".
    /// </summary>
    [Fact]
    public void SessionLogProducesTheExpectedEventSequence()
    {
        List<TimedEvent> events = SampleFiles.ReadEvents("session.log");

        Assert.Equal
        (
            [
                typeof(StartupEvent),
                typeof(MainMenuEvent),
                typeof(LoadingGameEvent),
                typeof(GameReadyEvent),
                typeof(StatusEvent),
                typeof(StatusEvent),
                typeof(StatusEvent),
                typeof(GameStoppedEvent),
                typeof(MainMenuEvent),
                typeof(ClosingEvent)
            ],
            events.ConvertAll(static timed => timed.Event.GetType())
        );
        Assert.Equal("Archipelago2", Assert.IsType<LoadingGameEvent>(events[2].Event).SaveName);
        Assert.False(Assert.IsType<ClosingEvent>(events[9].Event).IsCrash);
    }

    /// <summary>
    /// The status line of the sample session log is deserialised into a <see cref="ModStatus"/> with its year, month, day, speed-up, rank, balance, company figures, climate and save name.
    /// </summary>
    [Fact]
    public void SessionLogStatusLineIsDeserialised()
    {
        List<TimedEvent> events = SampleFiles.ReadEvents("session.log");

        ModStatus status = Assert.IsType<StatusEvent>(events[4].Event).Status;

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

    /// <summary>
    /// The sample mission log produces a main menu, a mission loading event for mission 1, a loading game event for "savegame" and a game ready event.
    /// </summary>
    [Fact]
    public void MissionLogProducesMissionEventWithNumber()
    {
        List<TimedEvent> events = SampleFiles.ReadEvents("mission.log");

        Assert.Equal
        (
            [
                typeof(MainMenuEvent),
                typeof(MissionLoadingEvent),
                typeof(LoadingGameEvent),
                typeof(GameReadyEvent)
            ],
            events.ConvertAll(static timed => timed.Event.GetType())
        );
        Assert.Equal(1, Assert.IsType<MissionLoadingEvent>(events[1].Event).MissionNumber);
        Assert.Equal("savegame", Assert.IsType<LoadingGameEvent>(events[2].Event).SaveName);
    }

    /// <summary>
    /// The sample crash log produces a single closing event that is flagged as a crash.
    /// </summary>
    [Fact]
    public void CrashLogProducesCrashClosing()
    {
        List<TimedEvent> events = SampleFiles.ReadEvents("crash.log");

        TimedEvent timed = Assert.Single(events);
        ClosingEvent closing = Assert.IsType<ClosingEvent>(timed.Event);
        Assert.True(closing.IsCrash);
    }

    /// <summary>
    /// A campaign or mission message with no usable mission number (a missing number, an empty name, or a number too large to parse) is still a mission loading event, with a null mission number.
    /// </summary>
    [Theory]
    [InlineData("Campaign/mission: some_campaign::/info.campaign/some_mission::/info.mission")]
    [InlineData("Campaign/mission: ")]
    [InlineData("Campaign/mission: x_mission_99999999999::/info.mission")]
    public void MissionWithoutUsableNumberHasNullNumber(string message)
    {
        GameLogEntryStruct entry = Entry(message);

        bool classified = GameLogEventClassifier.TryClassify(entry, out GameLogEvent? gameEvent);

        Assert.True(classified);
        MissionLoadingEvent mission = Assert.IsType<MissionLoadingEvent>(gameEvent);
        Assert.Null(mission.MissionNumber);
    }

    /// <summary>
    /// Empty, wrongly worded, truncated or malformed messages, including status lines with a bad or missing version, non-finite numbers, a wrong value type or no JSON, are not classified and give no event.
    /// </summary>
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

        Assert.False(GameLogEventClassifier.TryClassify(entry, out GameLogEvent? _));
    }

    /// <summary>
    /// A status line with unknown keys, nested values and a fractional exponent number is accepted, with the known keys read and the missing year left null.
    /// </summary>
    [Fact]
    public void StatusWithUnknownKeysAndFractionalNumbersIsAccepted()
    {
        GameLogEntryStruct entry = Entry("[drpc-status] {\"v\":1,\"future\":{\"a\":[1,2]},\"company_numberOfLines\":3,\"company_debt\":1.5e+06}");

        bool classified = GameLogEventClassifier.TryClassify(entry, out GameLogEvent? gameEvent);

        Assert.True(classified);
        ModStatus status = Assert.IsType<StatusEvent>(gameEvent).Status;
        Assert.Equal(3, status.CompanyNumberOfLines);
        Assert.Equal(1_500_000d, status.CompanyDebt);
        Assert.Null(status.Year);
    }

    /// <summary>
    /// A fractional value for an integer key is rounded toward zero, so 1.5 becomes 1 and -2.9 becomes -2.
    /// </summary>
    [Fact]
    public void StatusWithFractionalIntegerKeyIsRoundedTowardZero()
    {
        GameLogEntryStruct entry = Entry("[drpc-status] {\"v\":1,\"balance\":1.5,\"loan\":-2.9,\"year\":1900}");

        bool classified = GameLogEventClassifier.TryClassify(entry, out GameLogEvent? gameEvent);

        Assert.True(classified);
        ModStatus status = Assert.IsType<StatusEvent>(gameEvent).Status;
        Assert.Equal(1, status.Balance);
        Assert.Equal(-2, status.Loan);
        Assert.Equal(1900, status.Year);
    }

    /// <summary>
    /// A value written with an exponent for an integer key is expanded to the whole number, so 1.44228e+07 is read as 14,422,800.
    /// </summary>
    [Fact]
    public void StatusWithExponentIntegerKeyIsExpanded()
    {
        GameLogEntryStruct entry = Entry("[drpc-status] {\"v\":1,\"balance\":1.44228e+07}");

        bool classified = GameLogEventClassifier.TryClassify(entry, out GameLogEvent? gameEvent);

        Assert.True(classified);
        ModStatus status = Assert.IsType<StatusEvent>(gameEvent).Status;
        Assert.Equal(14_422_800, status.Balance);
    }

    /// <summary>
    /// An integer key whose value is out of range (1e30, -1e30, 1e999 or 9223372036854775808) is read as null while the other keys of the same status line are kept.
    /// </summary>
    [Theory]
    [InlineData("1e30")]
    [InlineData("-1e30")]
    [InlineData("1e999")]
    [InlineData("9223372036854775808")]
    public void StatusWithOutOfRangeIntegerKeyGivesNullForThatKeyOnly(string number)
    {
        GameLogEntryStruct entry = Entry("[drpc-status] {\"v\":1,\"balance\":" + number + ",\"year\":1900}");

        bool classified = GameLogEventClassifier.TryClassify(entry, out GameLogEvent? gameEvent);

        Assert.True(classified);
        ModStatus status = Assert.IsType<StatusEvent>(gameEvent).Status;
        Assert.Null(status.Balance);
        Assert.Equal(1900, status.Year);
    }

    /// <summary>
    /// The sample startup log produces a startup event followed by a game build event with build 40420.
    /// </summary>
    [Fact]
    public void StartupLogProducesStartupThenTheGameBuild()
    {
        List<TimedEvent> events = SampleFiles.ReadEvents("startup.log");

        Assert.Equal([typeof(StartupEvent), typeof(GameBuildEvent)], events.ConvertAll(static timed => timed.Event.GetType()));
        Assert.Equal(40420, Assert.IsType<GameBuildEvent>(events[1].Event).GameBuild);
    }

    /// <summary>
    /// A game build line whose version is empty, not a number, too large, negative or missing its build name suffix is not classified, and neither is a different build line format.
    /// </summary>
    [Theory]
    [InlineData("Starting up build version: , build name \"Build\"")]
    [InlineData("Starting up build version: abc, build name \"Build\"")]
    [InlineData("Starting up build version: 99999999999, build name \"Build\"")]
    [InlineData("Starting up build version: -5, build name \"Build\"")]
    [InlineData("Starting up build version: 40420")]
    [InlineData("Build: Build 40420 Windows 64-bit")]
    public void GameBuildLineWithoutAUsableNumberIsNotClassified(string message)
    {
        bool classified = GameLogEventClassifier.TryClassify(Entry(message), out GameLogEvent? gameEvent);

        Assert.False(classified);
        Assert.Null(gameEvent);
    }
}
