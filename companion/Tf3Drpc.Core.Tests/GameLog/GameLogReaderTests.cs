using System;
using System.IO;
using System.Threading.Tasks;

using Tf3Drpc.Core.Constants;
using Tf3Drpc.Core.GameLog;
using Tf3Drpc.Core.Structs;

using Xunit;

namespace Tf3Drpc.Core.Tests.GameLog;

/// <summary>
/// Tests of <see cref="Tf3Drpc.Core.GameLog.GameLogReader"/>, which turns new log lines into timed events, using temporary log files and a settable clock.
/// </summary>
public sealed class GameLogReaderTests : IDisposable
{
    private const string ReadyLine = "[2026-10-13 21:15:24Z - MESSAGE  - Main Thread      - Main           ]  Game is ready\r\n";
    private static readonly DateTimeOffset Start = new(2026, 10, 13, 21, 0, 0, TimeSpan.Zero);
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "tf3drpc-tests-" + Guid.NewGuid().ToString("N"));
    private readonly SettableTimeProvider _clock = new() { Now = Start.AddHours(1) };
    private readonly GameLogReader _reader;

    /// <summary>
    /// Sets up a temporary directory for the log file, a reader and a clock set one hour after the process start.
    /// </summary>
    public GameLogReaderTests()
    {
        _reader = new GameLogReader(_clock);
        Directory.CreateDirectory(_directory);
    }

    /// <summary>
    /// Deletes the temporary directory and the log file in it.
    /// </summary>
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

    /// <summary>
    /// A log file last written long before the process start is skipped without events, and it is read normally once its write time moves past the start.
    /// </summary>
    [Fact]
    public async Task FileWrittenBeforeTheProcessStartIsSkippedAndTheFollowerIsNotAdvancedAsync()
    {
        string path = WriteLog(ReadyLine, Start.AddHours(-3));

        GameLogPollResultStruct skipped = await PollAsync(path, Start);
        File.SetLastWriteTimeUtc(path, Start.AddMinutes(5).UtcDateTime);
        GameLogPollResultStruct read = await PollAsync(path, Start);

        Assert.Empty(skipped.Events);
        Assert.False(skipped.Restarted);
        Assert.Single(read.Events);
    }

    /// <summary>
    /// A log file written after the process start is read into an event that carries the timestamp of its log line.
    /// </summary>
    [Fact]
    public async Task FileWrittenAfterTheProcessStartIsReadIntoTimedEventsAsync()
    {
        string path = WriteLog(ReadyLine, Start.AddMinutes(5));

        GameLogPollResultStruct result = await PollAsync(path, Start);

        TimedGameLogEventStruct item = Assert.Single(result.Events);
        Assert.IsType<GameReadyEvent>(item.GameEvent);
        Assert.Equal(new DateTimeOffset(2026, 10, 13, 21, 15, 24, TimeSpan.Zero), item.Timestamp);
    }

    /// <summary>
    /// Without a process start time, a log file is read even if it was last written a year before the reference start.
    /// </summary>
    [Fact]
    public async Task WithoutStartTimeTheFileIsAlwaysReadAsync()
    {
        string path = WriteLog(ReadyLine, Start.AddYears(-1));

        GameLogPollResultStruct result = await PollAsync(path, notWrittenBefore: null);

        Assert.Single(result.Events);
    }

    /// <summary>
    /// A file that has been read once stays accepted when a later poll finds its write time rolled back to before the process start.
    /// </summary>
    [Fact]
    public async Task ConfirmedFileKeepsYieldingEventsAfterItsWriteTimeIsRolledBackAsync()
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

    /// <summary>
    /// The confirmation of a file ends when the process start time changes or the reader is restarted, after which the file with the old write time counts as the previous session again.
    /// </summary>
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

    /// <summary>
    /// A run of blank lines longer than one poll's read limit delays the first event by one poll, so the first poll yields nothing and the second poll yields the event.
    /// </summary>
    [Fact]
    public async Task ManyBlankLinesBeforeAnEventStillYieldTheEventAcrossPollsAsync()
    {
        string path = WriteLog(new string('\n', GameLogConstants.MaxReadBytesPerPoll * 3 / 2) + ReadyLine, Start.AddMinutes(5));

        GameLogPollResultStruct first = await PollAsync(path, Start);
        GameLogPollResultStruct second = await PollAsync(path, Start);

        Assert.Empty(first.Events);
        TimedGameLogEventStruct item = Assert.Single(second.Events);
        Assert.IsType<GameReadyEvent>(item.GameEvent);
    }

    /// <summary>
    /// Well-formed log lines that are not recognised game events, and lines that are not log lines at all, yield no events.
    /// </summary>
    [Fact]
    public async Task LinesThatAreNoEventsAreDroppedAsync()
    {
        string path = WriteLog("[2026-10-13 21:15:24Z - MESSAGE  - Main Thread      - Main           ]  Something else\r\nnot a log line\r\n", Start.AddMinutes(5));

        GameLogPollResultStruct result = await PollAsync(path, Start);

        Assert.Empty(result.Events);
    }

    /// <summary>
    /// A file that does not exist yields an empty result that is not marked as restarted.
    /// </summary>
    [Fact]
    public async Task MissingFileGivesAnEmptyResultAsync()
    {
        GameLogPollResultStruct result = await PollAsync(Path.Combine(_directory, "missing.txt"), notWrittenBefore: null);

        Assert.Empty(result.Events);
        Assert.False(result.Restarted);
    }

    /// <summary>
    /// When the file shrinks to less than what was read, the poll reports a restart and yields the events of the new content.
    /// </summary>
    [Fact]
    public async Task ShrinkingFileIsReportedAsRestartedAsync()
    {
        string path = WriteLog(ReadyLine + ReadyLine, Start.AddMinutes(5));
        _ = await PollAsync(path, Start);
        WriteLog(ReadyLine, Start.AddMinutes(6));

        GameLogPollResultStruct result = await PollAsync(path, Start);

        Assert.True(result.Restarted);
        Assert.Single(result.Events);
    }

    /// <summary>
    /// After a restart, the same file is read again from its beginning.
    /// </summary>
    [Fact]
    public async Task RestartReadsTheFileFromTheStartAgainAsync()
    {
        string path = WriteLog(ReadyLine, Start.AddMinutes(5));
        _ = await PollAsync(path, Start);
        _reader.Restart();

        GameLogPollResultStruct result = await PollAsync(path, Start);

        Assert.Single(result.Events);
    }

    /// <summary>
    /// A file counts as stale when it was written before the start time less its tolerance, but never when there is no start time or when it was written exactly at the start time.
    /// </summary>
    [Fact]
    public void IsStaleUsesTheStartToleranceAndIgnoresMissingStartTimes()
    {
        string path = WriteLog(ReadyLine, Start.AddHours(-3));

        Assert.True(GameLogReader.IsStale(path, Start, Start.AddHours(1)));
        Assert.False(GameLogReader.IsStale(path, notWrittenBefore: null, Start.AddHours(1)));
        Assert.False(GameLogReader.IsStale(path, Start.AddHours(-3), Start.AddHours(1)));
    }

    /// <summary>
    /// An unconfirmed file dated five hours in the future counts as the previous session and as stale.
    /// </summary>
    [Fact]
    public void FutureDatedUnconfirmedFileIsThePreviousSession()
    {
        string path = WriteLog(ReadyLine, Start.AddHours(5));

        Assert.True(_reader.IsPreviousSession(path, Start));
        Assert.True(GameLogReader.IsStale(path, Start, _clock.Now));
    }

    /// <summary>
    /// An unconfirmed file dated five hours in the future is skipped, and it is read once the clock has caught up with its write time.
    /// </summary>
    [Fact]
    public async Task FutureDatedUnconfirmedFileIsNotReadAndIsAcceptedOnceTheClockCatchesUpAsync()
    {
        string path = WriteLog(ReadyLine, Start.AddHours(5));

        GameLogPollResultStruct skipped = await PollAsync(path, Start);
        _clock.Now = Start.AddHours(5);
        GameLogPollResultStruct read = await PollAsync(path, Start);

        Assert.Empty(skipped.Events);
        Assert.Single(read.Events);
    }

    /// <summary>
    /// A confirmed file stays the current session's when the clock is set back and its write time then lies in the future.
    /// </summary>
    [Fact]
    public async Task ConfirmedFileStaysCurrentWhenTheClockIsSetBackAsync()
    {
        string path = WriteLog(ReadyLine, Start.AddMinutes(30));
        _ = await PollAsync(path, Start);

        _clock.JumpWallClock(Start.AddMinutes(-30));
        File.SetLastWriteTimeUtc(path, Start.AddHours(2).UtcDateTime);

        Assert.False(_reader.IsPreviousSession(path, Start));
    }

    /// <summary>
    /// An unconfirmed file dated only 30 seconds ahead of the clock lies within the future tolerance and does not count as the previous session.
    /// </summary>
    [Fact]
    public void FileWithinTheFutureToleranceIsNotStale()
    {
        string path = WriteLog(ReadyLine, _clock.Now.AddSeconds(30));

        Assert.False(_reader.IsPreviousSession(path, Start));
    }
}
