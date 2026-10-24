using System;
using System.Collections.Generic;
using System.IO;

using Tf3Drpc.Core.GameLog;
using Tf3Drpc.Core.Structs;

namespace Tf3Drpc.Core.Tests;

internal sealed record TimedEvent(DateTimeOffset Timestamp, GameLogEventStruct Event);

internal static class SampleFiles
{
    public static string GetPath(string name)
        => Path.Combine(AppContext.BaseDirectory, "log-samples", name);

    public static string[] ReadLines(string name)
        => File.ReadAllLines(GetPath(name));

    public static List<TimedEvent> ReadEvents(string name)
    {
        string[] lines = ReadLines(name);
        List<TimedEvent> events = [with(lines.Length)];

        foreach (string line in lines)
        {
            if (GameLogParser.TryParse(line, out GameLogEntryStruct entry) &&
                GameLogEventClassifier.TryClassify(entry, out GameLogEventStruct gameEvent))
            {
                events.Add(new TimedEvent(entry.Timestamp, gameEvent));
            }
        }

        return events;
    }
}
