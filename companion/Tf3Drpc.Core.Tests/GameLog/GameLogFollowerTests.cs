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

public sealed class GameLogFollowerTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "tf3drpc-tests-" + Guid.NewGuid().ToString("N"));
    private readonly GameLogFollower _follower = new();

    public GameLogFollowerTests()
        => Directory.CreateDirectory(_directory);

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

    [Fact]
    public async Task SessionLogCopy_YieldsAllLinesUnchangedAsync()
    {
        string path = PathOf("stdout.txt");
        File.Copy(SampleFiles.GetPath("session.log"), path);

        GameLogReadResultStruct result = await PollAsync(path);

        Assert.Equal(23, result.Lines.Count);
        Assert.Equal(SampleFiles.ReadLines("session.log"), result.Lines);
        Assert.False(result.Restarted);
    }

    [Fact]
    public async Task IncrementalAppends_YieldOnlyNewLinesAsync()
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

    [Fact]
    public async Task PartialLine_IsKeptUntilItsLineBreakArrivesAsync()
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

    [Fact]
    public async Task Utf8SequenceSplitAcrossReads_IsDecodedCorrectlyAsync()
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

    [Fact]
    public async Task CrlfAndLf_AreBothAcceptedAsync()
    {
        string path = PathOf("log.txt");
        Append(path, "a\r\nb\nc\r\n\nd\n");

        GameLogReadResultStruct result = await PollAsync(path);

        Assert.Equal(["a", "b", "c", string.Empty, "d"], result.Lines);
    }

    [Fact]
    public async Task Utf8Bom_IsIgnoredAsync()
    {
        string path = PathOf("log.txt");
        Append(path, [0xEF, 0xBB, 0xBF]);
        Append(path, "first\nsecond\n");

        GameLogReadResultStruct result = await PollAsync(path);

        Assert.Equal(["first", "second"], result.Lines);
    }

    [Fact]
    public async Task Truncation_RestartsAndReadsFromTheBeginningAsync()
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

    [Fact]
    public async Task RewrittenLogLongerThanTheOldOffset_RestartsAndDropsThePartialLineAsync()
    {
        string path = PathOf("log.txt");
        Append(path, "old\npartial");
        _ = await PollAsync(path);

        await File.WriteAllTextAsync(path, "new complete line\n", TestContext.Current.CancellationToken);
        GameLogReadResultStruct result = await PollAsync(path);

        Assert.True(result.Restarted);
        Assert.Equal(["new complete line"], result.Lines);
    }

    [Fact]
    public async Task RewrittenLogOfEqualLength_RestartsAsync()
    {
        string path = PathOf("log.txt");
        Append(path, "old\n");
        _ = await PollAsync(path);

        await File.WriteAllTextAsync(path, "new\n", TestContext.Current.CancellationToken);
        GameLogReadResultStruct result = await PollAsync(path);

        Assert.True(result.Restarted);
        Assert.Equal(["new"], result.Lines);
    }

    [Fact]
    public async Task OrdinaryAppendsBeyondTheFingerprint_ReportNoRestartAsync()
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

    [Fact]
    public async Task UnterminatedLine_IsDroppedOnceTooLongAndTheNextLineIsReadAsync()
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

    [Fact]
    public async Task ManyShortLines_AreReadInBatchesOfAtMostTheCapAsync()
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

    [Fact]
    public async Task LineJustBelowTheLimit_SpanningTwoPolls_IsDeliveredAsync()
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

    [Fact]
    public async Task OversizedLine_SpanningTwoPolls_IsStillSkippedAsync()
    {
        string path = PathOf("log.txt");
        AppendFillerUpToTheCapMinus(path, 64);
        Append(path, new string('z', GameLogConstants.MaxLineBytes + 10) + "\nafter\n");

        _ = await PollAsync(path);
        GameLogReadResultStruct second = await PollAsync(path);

        Assert.Equal(["after"], second.Lines);
        Assert.Equal(0, _follower.PendingBytes);
    }

    [Fact]
    public async Task OversizedLine_StartingNearTheCapAndEndingLater_IsSkippedAcrossSeveralPollsAsync()
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

    [Fact]
    public async Task OversizedLineCompletedWithinOnePoll_IsSkippedAsync()
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

    [Fact]
    public async Task PartialLineBelowTheLimit_StillCompletesAfterLargeChunksAsync()
    {
        string path = PathOf("log.txt");
        byte[] chunk = new byte[GameLogConstants.MaxLineBytes / 2];
        Array.Fill(chunk, (byte)'y');
        Append(path, chunk);
        _ = await PollAsync(path);
        Append(path, "end\n");
        GameLogReadResultStruct result = await PollAsync(path);

        Assert.Single(result.Lines);
        Assert.EndsWith("yend", result.Lines[0], StringComparison.Ordinal);
    }

    [Fact]
    public async Task MissingFile_GivesNoLinesAndKeepsPositionAsync()
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

    [Fact]
    public async Task PathChange_RestartsFromTheBeginningAsync()
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

    [Fact]
    public async Task FileHeldOpenForWriting_CanBeFollowedAsync()
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

    [Fact]
    public async Task LargeInitialRead_ReturnsEveryLineAsync()
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

    [Fact]
    public async Task CancelledToken_ThrowsAsync()
    {
        string path = PathOf("log.txt");
        Append(path, "a\n");
        using CancellationTokenSource source = new();
        await source.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => _follower.PollAsync(path, source.Token));
    }
}
