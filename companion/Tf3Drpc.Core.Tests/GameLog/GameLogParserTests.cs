using System;
using System.Collections.Generic;

using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.GameLog;
using Tf3Drpc.Core.Structs;

using Xunit;

namespace Tf3Drpc.Core.Tests.GameLog;

/// <summary>
/// Tests of <see cref="Tf3Drpc.Core.GameLog.GameLogParser"/> on the sample game logs and on hand-written lines, valid and invalid.
/// </summary>
public sealed class GameLogParserTests
{
    private const string GameReadyLine = "[2026-10-13 21:15:24Z - MESSAGE  - Main Thread      - Main           ]  Game is ready";

    /// <summary>
    /// In the session sample log, the 18 header lines of the 23 lines parse, and the continuation lines (indexes 2 and 6) do not.
    /// </summary>
    [Fact]
    public void SessionLogParsesHeaderLinesAndSkipsContinuationLines()
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

    /// <summary>
    /// The first line of the session sample log parses as a single-line message with a UTC timestamp, the message level and the text of the startup message.
    /// </summary>
    [Fact]
    public void SessionLogFirstLineIsSingleLineMessage()
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

    /// <summary>
    /// A session log header line that is followed by continuation lines parses with a message of "Memory types:" and is flagged as starting a multi-line entry.
    /// </summary>
    [Fact]
    public void SessionLogMultiLineEntryStartsMultiLine()
    {
        string[] lines = SampleFiles.ReadLines("session.log");

        bool parsed = GameLogParser.TryParse(lines[1], out GameLogEntryStruct entry);

        Assert.True(parsed);
        Assert.Equal("Memory types:", entry.Message);
        Assert.True(entry.StartsMultiLine);
    }

    /// <summary>
    /// A session log line whose category is written in full-width characters parses, and its message is kept.
    /// </summary>
    [Fact]
    public void SessionLogFullWidthCategoryParses()
    {
        string[] lines = SampleFiles.ReadLines("session.log");

        bool parsed = GameLogParser.TryParse(lines[7], out GameLogEntryStruct entry);

        Assert.True(parsed);
        Assert.StartsWith("new ModHubMod was installed", entry.Message, StringComparison.Ordinal);
        Assert.False(entry.StartsMultiLine);
    }

    /// <summary>
    /// A session log message with non-ASCII characters (an umlaut in a place name) parses with those characters intact.
    /// </summary>
    [Fact]
    public void SessionLogNonAsciiMessageIsKept()
    {
        string[] lines = SampleFiles.ReadLines("session.log");

        bool parsed = GameLogParser.TryParse(lines[14], out GameLogEntryStruct entry);

        Assert.True(parsed);
        Assert.Contains("Holzfällerlager", entry.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// The error entry of the crash sample log parses as a multi-line entry with the message "Ungraceful exit", and the continuation line after it does not parse.
    /// </summary>
    [Fact]
    public void CrashLogErrorEntryStartsMultiLine()
    {
        string[] lines = SampleFiles.ReadLines("crash.log");

        bool parsed = GameLogParser.TryParse(lines[5], out GameLogEntryStruct entry);

        Assert.True(parsed);
        Assert.Equal(GameLogLevel.Error, entry.Level);
        Assert.Equal("Ungraceful exit", entry.Message);
        Assert.True(entry.StartsMultiLine);
        Assert.False(GameLogParser.TryParse(lines[6], out GameLogEntryStruct _));
    }

    /// <summary>
    /// The verbose entry on the first line of the crash sample log parses with the verbose level and the message "Exception type: Lua error".
    /// </summary>
    [Fact]
    public void CrashLogVerboseEntryParses()
    {
        string[] lines = SampleFiles.ReadLines("crash.log");

        bool parsed = GameLogParser.TryParse(lines[0], out GameLogEntryStruct entry);

        Assert.True(parsed);
        Assert.Equal(GameLogLevel.Verbose, entry.Level);
        Assert.Equal("Exception type: Lua error", entry.Message);
    }

    /// <summary>
    /// All 8 lines of the mission sample log are header lines and parse.
    /// </summary>
    [Fact]
    public void MissionLogAllHeaderLinesParse()
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

    /// <summary>
    /// A header line without a trailing carriage return parses and yields its message.
    /// </summary>
    [Fact]
    public void LineWithoutCarriageReturnParses()
    {
        bool parsed = GameLogParser.TryParse(GameReadyLine, out GameLogEntryStruct entry);

        Assert.True(parsed);
        Assert.Equal("Game is ready", entry.Message);
    }

    /// <summary>
    /// A header line with a trailing carriage return parses and the carriage return is not part of the message.
    /// </summary>
    [Fact]
    public void LineWithTrailingCarriageReturnDropsIt()
    {
        bool parsed = GameLogParser.TryParse(GameReadyLine + "\r", out GameLogEntryStruct entry);

        Assert.True(parsed);
        Assert.Equal("Game is ready", entry.Message);
    }

    /// <summary>
    /// A line that is not a header (empty, garbage, continuation markers, wrong spacing after the bracket, an impossible date, an unknown level or a truncated header) is rejected and leaves a default entry.
    /// </summary>
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

    /// <summary>
    /// A 200,000-character garbage line that starts like a header is rejected without throwing.
    /// </summary>
    [Fact]
    public void HugeGarbageLineDoesNotThrow()
    {
        string line = "[2026-10-13 21:15:24Z - MESSAGE" + new string(' ', 200_000) + "- x";

        bool parsed = GameLogParser.TryParse(line, out GameLogEntryStruct _);

        Assert.False(parsed);
    }
}
