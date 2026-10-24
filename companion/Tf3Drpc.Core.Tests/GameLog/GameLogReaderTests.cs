using System;
using System.IO;
using System.Threading.Tasks;

using Tf3Drpc.Core.Constants;
using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.GameLog;
using Tf3Drpc.Core.Structs;

using Xunit;

namespace Tf3Drpc.Core.Tests.GameLog;

public sealed class GameLogReaderTests : IDisposable
{
    private const string ReadyLine = "[2026-10-13 21:15:24Z - MESSAGE  - Main Thread      - Main           ]  Game is ready\r\n";
    private static readonly DateTimeOffset Start = new(2026, 10, 13, 21, 0, 0, TimeSpan.Zero);
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "tf3drpc-tests-" + Guid.NewGuid().ToString("N"));
    private readonly SettableTimeProvider _clock = new() { Now = Start.AddHours(1) };
    private readonly GameLogReader _reader;

    public GameLogReaderTests()
    {
        _reader = new GameLogReader(_clock);
        Directory.CreateDirectory(_directory);
    }

    public void Dispose()
        => Directory.Delete(_directory, recursive: true);

    private string WriteLog(string content, in DateTimeOffset written)
    {
        string path = Path.Combine(_directory, "stdout.txt");
        File.WriteAllText(path, content);
        File.SetLastWriteTimeUtc(path, written.UtcDateTime);

        return path;
    }

    private Task<GameLogPollResultStruct> PollAsync(string path, DateTimeOffset? notWrittenBefore)
        => _reader.PollAsync(path, notWrittenBefore, TestContext.Current.CancellationToken);

    [Fact]
    public async Task FileWrittenBeforeTheProcessStart_IsSkippedAndTheFollowerIsNotAdvancedAsync()
    {
        string path = WriteLog(ReadyLine, Start.AddHours(-3));

        GameLogPollResultStruct skipped = await PollAsync(path, Start);
        File.SetLastWriteTimeUtc(path, Start.AddMinutes(5).UtcDateTime);
        GameLogPollResultStruct read = await PollAsync(path, Start);

        Assert.Empty(skipped.Events);
        Assert.False(skipped.Restarted);
        Assert.Single(read.Events);
    }

    [Fact]
    public async Task FileWrittenAfterTheProcessStart_IsReadIntoTimedEventsAsync()
    {
        string path = WriteLog(ReadyLine, Start.AddMinutes(5));

        GameLogPollResultStruct result = await PollAsync(path, Start);

        Assert.Single(result.Events);
        Assert.Equal(GameLogEventKind.GameReady, result.Events[0].GameEvent.Kind);
        Assert.Equal(new DateTimeOffset(2026, 10, 13, 21, 15, 24, TimeSpan.Zero), result.Events[0].Timestamp);
    }

    [Fact]
    public async Task WithoutStartTime_TheFileIsAlwaysReadAsync()
    {
        string path = WriteLog(ReadyLine, Start.AddYears(-1));

        GameLogPollResultStruct result = await PollAsync(path, null);

        Assert.Single(result.Events);
    }

    [Fact]
    public async Task ConfirmedFile_KeepsYieldingEventsAfterItsWriteTimeIsRolledBackAsync()
    {
        string path = WriteLog(ReadyLine, Start.AddMinutes(5));
        GameLogPollResultStruct first = await PollAsync(path, Start);

        await File.AppendAllTextAsync(path, ReadyLine, TestContext.Current.CancellationToken);
        File.SetLastWriteTimeUtc(path, Start.AddHours(-1).UtcDateTime);
        GameLogPollResultStruct second = await PollAsync(path, Start);

        Assert.Single(first.Events);
        Assert.Single(second.Events);
        Assert.False(_reader.IsPreviousSession(path, Start));
    }

    [Fact]
    public async Task ConfirmationEndsWithARestartOrAnotherStartTimeAsync()
    {
        string path = WriteLog(ReadyLine, Start.AddMinutes(5));
        _ = await PollAsync(path, Start);
        File.SetLastWriteTimeUtc(path, Start.AddHours(-1).UtcDateTime);

        Assert.True(_reader.IsPreviousSession(path, Start.AddMinutes(10)));

        _reader.Restart();

        Assert.True(_reader.IsPreviousSession(path, Start));
    }

    [Fact]
    public async Task ManyBlankLinesBeforeAnEvent_StillYieldTheEventAcrossPollsAsync()
    {
        string path = WriteLog(new string('\n', GameLogConstants.MaxReadBytesPerPoll * 3 / 2) + ReadyLine, Start.AddMinutes(5));

        GameLogPollResultStruct first = await PollAsync(path, Start);
        GameLogPollResultStruct second = await PollAsync(path, Start);

        Assert.Empty(first.Events);
        Assert.Single(second.Events);
        Assert.Equal(GameLogEventKind.GameReady, second.Events[0].GameEvent.Kind);
    }

    [Fact]
    public async Task LinesThatAreNoEvents_AreDroppedAsync()
    {
        string path = WriteLog("[2026-10-13 21:15:24Z - MESSAGE  - Main Thread      - Main           ]  Something else\r\nnot a log line\r\n", Start.AddMinutes(5));

        GameLogPollResultStruct result = await PollAsync(path, Start);

        Assert.Empty(result.Events);
    }

    [Fact]
    public async Task MissingFile_GivesAnEmptyResultAsync()
    {
        GameLogPollResultStruct result = await PollAsync(Path.Combine(_directory, "missing.txt"), null);

        Assert.Empty(result.Events);
        Assert.False(result.Restarted);
    }

    [Fact]
    public async Task ShrinkingFile_IsReportedAsRestartedAsync()
    {
        string path = WriteLog(ReadyLine + ReadyLine, Start.AddMinutes(5));
        _ = await PollAsync(path, Start);
        WriteLog(ReadyLine, Start.AddMinutes(6));

        GameLogPollResultStruct result = await PollAsync(path, Start);

        Assert.True(result.Restarted);
        Assert.Single(result.Events);
    }

    [Fact]
    public async Task Restart_ReadsTheFileFromTheStartAgainAsync()
    {
        string path = WriteLog(ReadyLine, Start.AddMinutes(5));
        _ = await PollAsync(path, Start);
        _reader.Restart();

        GameLogPollResultStruct result = await PollAsync(path, Start);

        Assert.Single(result.Events);
    }

    [Fact]
    public void IsStale_UsesTheStartToleranceAndIgnoresMissingStartTimes()
    {
        string path = WriteLog(ReadyLine, Start.AddHours(-3));

        Assert.True(GameLogReader.IsStale(path, Start, Start.AddHours(1)));
        Assert.False(GameLogReader.IsStale(path, null, Start.AddHours(1)));
        Assert.False(GameLogReader.IsStale(path, Start.AddHours(-3), Start.AddHours(1)));
    }

    [Fact]
    public void FutureDatedUnconfirmedFile_IsThePreviousSession()
    {
        string path = WriteLog(ReadyLine, Start.AddHours(5));

        Assert.True(_reader.IsPreviousSession(path, Start));
        Assert.True(GameLogReader.IsStale(path, Start, _clock.Now));
    }

    [Fact]
    public async Task FutureDatedUnconfirmedFile_IsNotReadAndIsAcceptedOnceTheClockCatchesUpAsync()
    {
        string path = WriteLog(ReadyLine, Start.AddHours(5));

        GameLogPollResultStruct skipped = await PollAsync(path, Start);
        _clock.Now = Start.AddHours(5);
        GameLogPollResultStruct read = await PollAsync(path, Start);

        Assert.Empty(skipped.Events);
        Assert.Single(read.Events);
    }

    [Fact]
    public async Task ConfirmedFile_StaysCurrentWhenTheClockIsSetBackAsync()
    {
        string path = WriteLog(ReadyLine, Start.AddMinutes(30));
        _ = await PollAsync(path, Start);

        _clock.JumpWallClock(Start.AddMinutes(-30));
        File.SetLastWriteTimeUtc(path, Start.AddHours(2).UtcDateTime);

        Assert.False(_reader.IsPreviousSession(path, Start));
    }

    [Fact]
    public void FileWithinTheFutureTolerance_IsNotStale()
    {
        string path = WriteLog(ReadyLine, _clock.Now.AddSeconds(30));

        Assert.False(_reader.IsPreviousSession(path, Start));
    }
}
