using System;
using System.Collections.Generic;

using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.GameLog;
using Tf3Drpc.Core.Structs;

using Xunit;

namespace Tf3Drpc.Core.Tests.GameLog;

public sealed class GameLogParserTests
{
    private const string GameReadyLine = "[2026-10-13 21:15:24Z - MESSAGE  - Main Thread      - Main           ]  Game is ready";

    [Fact]
    public void SessionLog_ParsesHeaderLinesAndSkipsContinuationLines()
    {
        string[] lines = SampleFiles.ReadLines("session.log");
        List<int> parsedIndexes = [];

        for (int i = 0; i < lines.Length; i++)
        {
            if (GameLogParser.TryParse(lines[i], out GameLogEntryStruct _))
                parsedIndexes.Add(i);
        }

        Assert.Equal(23, lines.Length);
        Assert.Equal(18, parsedIndexes.Count);
        Assert.DoesNotContain(2, parsedIndexes);
        Assert.DoesNotContain(6, parsedIndexes);
        Assert.Contains(1, parsedIndexes);
        Assert.Contains(7, parsedIndexes);
    }

    [Fact]
    public void SessionLog_FirstLineIsSingleLineMessage()
    {
        string[] lines = SampleFiles.ReadLines("session.log");

        bool parsed = GameLogParser.TryParse(lines[0], out GameLogEntryStruct entry);

        Assert.True(parsed);
        Assert.Equal(new DateTimeOffset(2026, 10, 13, 21, 20, 43, TimeSpan.Zero), entry.Timestamp);
        Assert.Equal(TimeSpan.Zero, entry.Timestamp.Offset);
        Assert.Equal(GameLogLevel.Message, entry.Level);
        Assert.Equal("Application startup. Hello!", entry.Message);
        Assert.False(entry.StartsMultiLine);
    }

    [Fact]
    public void SessionLog_MultiLineEntryStartsMultiLine()
    {
        string[] lines = SampleFiles.ReadLines("session.log");

        bool parsed = GameLogParser.TryParse(lines[1], out GameLogEntryStruct entry);

        Assert.True(parsed);
        Assert.Equal("Memory types:", entry.Message);
        Assert.True(entry.StartsMultiLine);
    }

    [Fact]
    public void SessionLog_FullWidthCategoryParses()
    {
        string[] lines = SampleFiles.ReadLines("session.log");

        bool parsed = GameLogParser.TryParse(lines[7], out GameLogEntryStruct entry);

        Assert.True(parsed);
        Assert.StartsWith("new ModHubMod was installed", entry.Message, StringComparison.Ordinal);
        Assert.False(entry.StartsMultiLine);
    }

    [Fact]
    public void SessionLog_NonAsciiMessageIsKept()
    {
        string[] lines = SampleFiles.ReadLines("session.log");

        bool parsed = GameLogParser.TryParse(lines[14], out GameLogEntryStruct entry);

        Assert.True(parsed);
        Assert.Contains("Holzfällerlager", entry.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void CrashLog_ErrorEntryStartsMultiLine()
    {
        string[] lines = SampleFiles.ReadLines("crash.log");

        bool parsed = GameLogParser.TryParse(lines[5], out GameLogEntryStruct entry);

        Assert.True(parsed);
        Assert.Equal(GameLogLevel.Error, entry.Level);
        Assert.Equal("Ungraceful exit", entry.Message);
        Assert.True(entry.StartsMultiLine);
        Assert.False(GameLogParser.TryParse(lines[6], out GameLogEntryStruct _));
    }

    [Fact]
    public void CrashLog_VerboseEntryParses()
    {
        string[] lines = SampleFiles.ReadLines("crash.log");

        bool parsed = GameLogParser.TryParse(lines[0], out GameLogEntryStruct entry);

        Assert.True(parsed);
        Assert.Equal(GameLogLevel.Verbose, entry.Level);
        Assert.Equal("Exception type: Lua error", entry.Message);
    }

    [Fact]
    public void MissionLog_AllHeaderLinesParse()
    {
        string[] lines = SampleFiles.ReadLines("mission.log");
        int parsedCount = 0;

        foreach (string line in lines)
        {
            if (GameLogParser.TryParse(line, out GameLogEntryStruct _))
                parsedCount++;
        }

        Assert.Equal(8, parsedCount);
    }

    [Fact]
    public void LineWithoutCarriageReturnParses()
    {
        bool parsed = GameLogParser.TryParse(GameReadyLine, out GameLogEntryStruct entry);

        Assert.True(parsed);
        Assert.Equal("Game is ready", entry.Message);
    }

    [Fact]
    public void LineWithTrailingCarriageReturnDropsIt()
    {
        bool parsed = GameLogParser.TryParse(GameReadyLine + "\r", out GameLogEntryStruct entry);

        Assert.True(parsed);
        Assert.Equal("Game is ready", entry.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("garbage")]
    [InlineData("    | continuation")]
    [InlineData("    + last continuation")]
    [InlineData("[2026-10-13 21:15:24Z - MESSAGE  - Main Thread      - Main           ] Game is ready")]
    [InlineData("[2026-10-13 21:15:24Z - MESSAGE  - Main Thread      - Main           ]Game is ready")]
    [InlineData("[2026-13-45 99:99:99Z - MESSAGE  - Main Thread      - Main           ]  Game is ready")]
    [InlineData("[2026-10-13 21:15:24Z - LOUD     - Main Thread      - Main           ]  Game is ready")]
    [InlineData("[2026-10-13 21:15:24Z - MESSAGE  - Main")]
    public void NonEntriesAreRejected(string line)
    {
        bool parsed = GameLogParser.TryParse(line, out GameLogEntryStruct entry);

        Assert.False(parsed);
        Assert.Equal(default, entry);
    }

    [Fact]
    public void HugeGarbageLineDoesNotThrow()
    {
        string line = "[2026-10-13 21:15:24Z - MESSAGE" + new string(' ', 200_000) + "- x";

        bool parsed = GameLogParser.TryParse(line, out GameLogEntryStruct _);

        Assert.False(parsed);
    }
}
