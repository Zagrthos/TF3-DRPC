using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Tf3Drpc.Core.Constants;
using Tf3Drpc.Core.GameLog;
using Tf3Drpc.Core.Structs;

using Xunit;

namespace Tf3Drpc.Core.Tests.GameLog;

/// <summary>
/// Tests of <see cref="Tf3Drpc.Core.GameLog.GameLogFollower"/>, which returns the lines appended to a growing log file, using temporary files.
/// </summary>
public sealed class GameLogFollowerTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "tf3drpc-tests-" + Guid.NewGuid().ToString("N"));
    private readonly GameLogFollower _follower = new();

    /// <summary>
    /// Creates a temporary directory for the log files of the test.
    /// </summary>
    public GameLogFollowerTests()
        => Directory.CreateDirectory(_directory);

    /// <summary>
    /// Deletes the temporary directory and the log files in it.
    /// </summary>
    public void Dispose()
        => Directory.Delete(_directory, recursive: true);

    private string PathOf(string name)
        => Path.Combine(_directory, name);

    private static void Append(string path, byte[] bytes)
    {
        using FileStream stream = new(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
        stream.Write(bytes);
    }

    private static void Append(string path, string text)
        => Append(path, Encoding.UTF8.GetBytes(text));

    private Task<GameLogReadResultStruct> PollAsync(string path)
        => _follower.PollAsync(path, TestContext.Current.CancellationToken);

    /// <summary>
    /// A copy of the session sample log is read in one poll as its 23 lines, identical to the file lines and without a restart.
    /// </summary>
    [Fact]
    public async Task SessionLogCopyYieldsAllLinesUnchangedAsync()
    {
        string path = PathOf("stdout.txt");
        File.Copy(SampleFiles.GetPath("session.log"), path);

        GameLogReadResultStruct result = await PollAsync(path);

        Assert.Equal(23, result.Lines.Count);
        Assert.Equal(SampleFiles.ReadLines("session.log"), result.Lines);
        Assert.False(result.Restarted);
    }

    /// <summary>
    /// Each poll returns only the lines appended since the previous poll, and a poll without new data returns none.
    /// </summary>
    [Fact]
    public async Task IncrementalAppendsYieldOnlyNewLinesAsync()
    {
        string path = PathOf("log.txt");
        Append(path, "one\r\ntwo\r\n");

        GameLogReadResultStruct first = await PollAsync(path);
        GameLogReadResultStruct idle = await PollAsync(path);
        Append(path, "three\r\n");
        GameLogReadResultStruct second = await PollAsync(path);

        Assert.Equal(["one", "two"], first.Lines);
        Assert.Empty(idle.Lines);
        Assert.Equal(["three"], second.Lines);
    }

    /// <summary>
    /// A line without a line break waits for the next poll and is returned complete once its line break has arrived.
    /// </summary>
    [Fact]
    public async Task PartialLineIsKeptUntilItsLineBreakArrivesAsync()
    {
        string path = PathOf("log.txt");
        Append(path, "complete\nunfin");

        GameLogReadResultStruct first = await PollAsync(path);
        Append(path, "ished");
        GameLogReadResultStruct second = await PollAsync(path);
        Append(path, "\r\n");
        GameLogReadResultStruct third = await PollAsync(path);

        Assert.Equal(["complete"], first.Lines);
        Assert.Empty(second.Lines);
        Assert.Equal(["unfinished"], third.Lines);
    }

    /// <summary>
    /// A multi-byte UTF-8 character that is split between two polls is decoded correctly once its remaining bytes arrive.
    /// </summary>
    [Fact]
    public async Task Utf8SequenceSplitAcrossReadsIsDecodedCorrectlyAsync()
    {
        string path = PathOf("log.txt");
        byte[] bytes = Encoding.UTF8.GetBytes("Holzfällerlager\r\n");
        int split = Array.IndexOf(bytes, (byte)0xC3) + 1;
        Append(path, bytes[..split]);

        GameLogReadResultStruct first = await PollAsync(path);
        Append(path, bytes[split..]);
        GameLogReadResultStruct second = await PollAsync(path);

        Assert.Empty(first.Lines);
        Assert.Equal(["Holzfällerlager"], second.Lines);
    }

    /// <summary>
    /// Lines ending in CRLF and in LF are both split correctly, and an empty line is returned as an empty string.
    /// </summary>
    [Fact]
    public async Task CrlfAndLfAreBothAcceptedAsync()
    {
        string path = PathOf("log.txt");
        Append(path, "a\r\nb\nc\r\n\nd\n");

        GameLogReadResultStruct result = await PollAsync(path);

        Assert.Equal(["a", "b", "c", string.Empty, "d"], result.Lines);
    }

    /// <summary>
    /// A UTF-8 byte order mark at the start of the file is not part of the first line.
    /// </summary>
    [Fact]
    public async Task Utf8BomIsIgnoredAsync()
    {
        string path = PathOf("log.txt");
        Append(path, [0xEF, 0xBB, 0xBF]);
        Append(path, "first\nsecond\n");

        GameLogReadResultStruct result = await PollAsync(path);

        Assert.Equal(["first", "second"], result.Lines);
    }

    /// <summary>
    /// A file that is overwritten with shorter content is reported as restarted, is read from its beginning, and the old partial line is discarded.
    /// </summary>
    [Fact]
    public async Task TruncationRestartsAndReadsFromTheBeginningAsync()
    {
        string path = PathOf("log.txt");
        Append(path, "old one\nold two\npartial");
        _ = await PollAsync(path);

        await File.WriteAllTextAsync(path, "new\n", TestContext.Current.CancellationToken);
        GameLogReadResultStruct result = await PollAsync(path);
        Append(path, "more\n");
        GameLogReadResultStruct next = await PollAsync(path);

        Assert.True(result.Restarted);
        Assert.Equal(["new"], result.Lines);
        Assert.False(next.Restarted);
        Assert.Equal(["more"], next.Lines);
    }

    /// <summary>
    /// A file that is overwritten with content longer than the previous read offset is detected as rewritten, reported as restarted, and read from its beginning without the old partial line.
    /// </summary>
    [Fact]
    public async Task RewrittenLogLongerThanTheOldOffsetRestartsAndDropsThePartialLineAsync()
    {
        string path = PathOf("log.txt");
        Append(path, "old\npartial");
        _ = await PollAsync(path);

        await File.WriteAllTextAsync(path, "new complete line\n", TestContext.Current.CancellationToken);
        GameLogReadResultStruct result = await PollAsync(path);

        Assert.True(result.Restarted);
        Assert.Equal(["new complete line"], result.Lines);
    }

    /// <summary>
    /// A file that is overwritten with content of exactly the same length is detected as rewritten and reported as restarted.
    /// </summary>
    [Fact]
    public async Task RewrittenLogOfEqualLengthRestartsAsync()
    {
        string path = PathOf("log.txt");
        Append(path, "old\n");
        _ = await PollAsync(path);

        await File.WriteAllTextAsync(path, "new\n", TestContext.Current.CancellationToken);
        GameLogReadResultStruct result = await PollAsync(path);

        Assert.True(result.Restarted);
        Assert.Equal(["new"], result.Lines);
    }

    /// <summary>
    /// Appending lines to a file whose size is past the head fingerprint is never mistaken for a rewrite and yields exactly the new line each time.
    /// </summary>
    [Fact]
    public async Task OrdinaryAppendsBeyondTheFingerprintReportNoRestartAsync()
    {
        string path = PathOf("log.txt");
        string line = new string('x', 100) + "\n";
        Append(path, line);
        _ = await PollAsync(path);

        for (int i = 0; i < 6; i++)
        {
            Append(path, line);
            GameLogReadResultStruct result = await PollAsync(path);

            Assert.False(result.Restarted);
            Assert.Single(result.Lines);
        }
    }

    /// <summary>
    /// A line that grows beyond the maximum line length without a line break is dropped, the pending bytes stay within that limit, and reading resumes with the line that follows.
    /// </summary>
    [Fact]
    public async Task UnterminatedLineIsDroppedOnceTooLongAndTheNextLineIsReadAsync()
    {
        string path = PathOf("log.txt");
        byte[] chunk = new byte[4 * 1024 * 1024];
        Array.Fill(chunk, (byte)'x');
        Append(path, "before\n");
        GameLogReadResultStruct first = await PollAsync(path);

        for (int i = 0; i < 6; i++)
        {
            Append(path, chunk);
            GameLogReadResultStruct result = await PollAsync(path);

            Assert.Empty(result.Lines);
            Assert.True(_follower.PendingBytes <= GameLogConstants.MaxLineBytes);
        }

        Append(path, "tail of the long line\nafter\n");
        GameLogReadResultStruct recovered = await PollAsync(path);

        Assert.Equal(["before"], first.Lines);
        Assert.Equal(["after"], recovered.Lines);
        Assert.Equal(0, _follower.PendingBytes);
    }

    /// <summary>
    /// A file of many two-byte lines is returned in several polls, each of at most the per-poll read limit, which together yield every line.
    /// </summary>
    [Fact]
    public async Task ManyShortLinesAreReadInBatchesOfAtMostTheCapAsync()
    {
        string path = PathOf("log.txt");
        const int lineCount = ((2 * GameLogConstants.MaxReadBytesPerPoll) + 1000) / 2;
        Append(path, string.Concat(Enumerable.Repeat("x\n", lineCount)));
        int total = 0;

        for (int i = 0; i < 3; i++)
        {
            GameLogReadResultStruct result = await PollAsync(path);

            Assert.True((result.Lines.Count * 2) <= GameLogConstants.MaxReadBytesPerPoll);
            Assert.NotEmpty(result.Lines);
            total += result.Lines.Count;
        }

        Assert.Equal(lineCount, total);
        Assert.Empty((await PollAsync(path)).Lines);
    }

    /// <summary>
    /// A line just under the maximum length that is split across two polls is delivered whole, together with the line after it.
    /// </summary>
    [Fact]
    public async Task LineJustBelowTheLimitSpanningTwoPollsIsDeliveredAsync()
    {
        string path = PathOf("log.txt");
        string text = new('z', GameLogConstants.MaxLineBytes - 10);
        AppendFillerUpToTheCapMinus(path, 64);
        Append(path, text + "\nafter\n");

        GameLogReadResultStruct first = await PollAsync(path);
        int pending = _follower.PendingBytes;
        GameLogReadResultStruct second = await PollAsync(path);

        Assert.Equal(64, pending);
        Assert.All(first.Lines, static line => Assert.Equal(63, line.Length));
        Assert.Equal([text, "after"], second.Lines);
    }

    /// <summary>
    /// A line over the maximum length that is split across two polls is skipped, and the line after it is delivered with no bytes left pending.
    /// </summary>
    [Fact]
    public async Task OversizedLineSpanningTwoPollsIsStillSkippedAsync()
    {
        string path = PathOf("log.txt");
        AppendFillerUpToTheCapMinus(path, 64);
        Append(path, new string('z', GameLogConstants.MaxLineBytes + 10) + "\nafter\n");

        _ = await PollAsync(path);
        GameLogReadResultStruct second = await PollAsync(path);

        Assert.Equal(["after"], second.Lines);
        Assert.Equal(0, _follower.PendingBytes);
    }

    /// <summary>
    /// An over-long line that begins just before the per-poll read limit and ends several polls later is skipped, so that only the preceding filler lines and the final line are returned.
    /// </summary>
    [Fact]
    public async Task OversizedLineStartingNearTheCapAndEndingLaterIsSkippedAcrossSeveralPollsAsync()
    {
        string path = PathOf("log.txt");
        AppendFillerUpToTheCapMinus(path, 64);
        Append(path, new string('z', GameLogConstants.MaxReadBytesPerPoll + 10) + "\nafter\n");
        List<string> lines = [];

        for (int i = 0; i < 4; i++)
            lines.AddRange((await PollAsync(path)).Lines);

        Assert.Equal(65535, lines.Count - 1);
        Assert.Equal("after", lines[^1]);
    }

    private static void AppendFillerUpToTheCapMinus(string path, int missing)
    {
        string line = new string('f', 63) + "\n";

        Append(path, string.Concat(Enumerable.Repeat(line, (GameLogConstants.MaxReadBytesPerPoll - missing) / 64)));
    }

    /// <summary>
    /// An over-long line that begins and ends within one poll is skipped, and the lines before and after it are delivered.
    /// </summary>
    [Fact]
    public async Task OversizedLineCompletedWithinOnePollIsSkippedAsync()
    {
        string path = PathOf("log.txt");
        byte[] chunk = new byte[GameLogConstants.MaxLineBytes + 10];
        Array.Fill(chunk, (byte)'x');
        Append(path, "one\n");
        Append(path, chunk);
        Append(path, "\ntwo\n");

        GameLogReadResultStruct result = await PollAsync(path);

        Assert.Equal(["one", "two"], result.Lines);
    }

    /// <summary>
    /// A partial line of half the maximum length that is followed by a short ending in a later poll is returned as one complete line.
    /// </summary>
    [Fact]
    public async Task PartialLineBelowTheLimitStillCompletesAfterLargeChunksAsync()
    {
        string path = PathOf("log.txt");
        byte[] chunk = new byte[GameLogConstants.MaxLineBytes / 2];
        Array.Fill(chunk, (byte)'y');
        Append(path, chunk);
        _ = await PollAsync(path);
        Append(path, "end\n");
        GameLogReadResultStruct result = await PollAsync(path);

        string item = Assert.Single(result.Lines);
        Assert.EndsWith("yend", item, StringComparison.Ordinal);
    }

    /// <summary>
    /// A file that is missing yields no lines and no restart, and after the file has been deleted again the follower keeps its position and yields no lines.
    /// </summary>
    [Fact]
    public async Task MissingFileGivesNoLinesAndKeepsPositionAsync()
    {
        string path = PathOf("log.txt");

        GameLogReadResultStruct missing = await PollAsync(path);
        Append(path, "a\n");
        GameLogReadResultStruct first = await PollAsync(path);
        File.Delete(path);
        GameLogReadResultStruct deleted = await PollAsync(path);

        Assert.Empty(missing.Lines);
        Assert.False(missing.Restarted);
        Assert.Equal(["a"], first.Lines);
        Assert.Empty(deleted.Lines);
        Assert.False(deleted.Restarted);
    }

    /// <summary>
    /// Polling a different path than before reads that file from its beginning and reports a restart.
    /// </summary>
    [Fact]
    public async Task PathChangeRestartsFromTheBeginningAsync()
    {
        string first = PathOf("a.txt");
        string second = PathOf("b.txt");
        Append(first, "a1\na2\n");
        Append(second, "b1\n");

        GameLogReadResultStruct firstResult = await PollAsync(first);
        GameLogReadResultStruct secondResult = await PollAsync(second);

        Assert.False(firstResult.Restarted);
        Assert.True(secondResult.Restarted);
        Assert.Equal(["b1"], secondResult.Lines);
    }

    /// <summary>
    /// A file that another stream still holds open for writing can be read, and later writes to it are picked up by the next poll.
    /// </summary>
    [Fact]
    public async Task FileHeldOpenForWritingCanBeFollowedAsync()
    {
        string path = PathOf("log.txt");
        await using FileStream writer = new(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);

        await writer.WriteAsync("one\r\n"u8.ToArray(), TestContext.Current.CancellationToken);
        await writer.FlushAsync(TestContext.Current.CancellationToken);
        GameLogReadResultStruct first = await PollAsync(path);
        await writer.WriteAsync("two\r\n"u8.ToArray(), TestContext.Current.CancellationToken);
        await writer.FlushAsync(TestContext.Current.CancellationToken);
        GameLogReadResultStruct second = await PollAsync(path);

        Assert.Equal(["one"], first.Lines);
        Assert.Equal(["two"], second.Lines);
    }

    /// <summary>
    /// A file larger than the per-poll read limit is read over several polls that together return all 100,000 lines in order.
    /// </summary>
    [Fact]
    public async Task LargeInitialReadReturnsEveryLineAsync()
    {
        string path = PathOf("big.txt");
        const int LineCount = 100_000;
        StringBuilder builder = new();

        for (int i = 0; i < LineCount; i++)
        {
            builder.Append("[line ").Append(i).Append("] some padding text to grow the file\r\n");
        }

        Append(path, builder.ToString());

        List<string> lines = [];
        int polls = 0;

        while (lines.Count < LineCount && polls < 10)
        {
            lines.AddRange((await PollAsync(path)).Lines);
            polls++;
        }

        Assert.True(new FileInfo(path).Length is > GameLogConstants.MaxReadBytesPerPoll);
        Assert.True(polls is > 1);
        Assert.Equal(LineCount, lines.Count);
        Assert.Equal("[line 99999] some padding text to grow the file", lines[LineCount - 1]);
    }

    /// <summary>
    /// Polling with an already cancelled token throws an <see cref="System.OperationCanceledException"/>.
    /// </summary>
    [Fact]
    public async Task CancelledTokenThrowsAsync()
    {
        string path = PathOf("log.txt");
        Append(path, "a\n");
        using CancellationTokenSource source = new();
        await source.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => _follower.PollAsync(path, source.Token));
    }
}
