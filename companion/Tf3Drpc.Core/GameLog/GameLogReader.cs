using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Tf3Drpc.Core.Constants;
using Tf3Drpc.Core.Structs;

namespace Tf3Drpc.Core.GameLog;

/// <summary>
/// Reads the new lines of the game log and turns them into timed events; call it from one loop only.
/// </summary>
internal sealed class GameLogReader(TimeProvider timeProvider)
{
    private readonly TimeProvider _timeProvider = timeProvider;
    private GameLogFollower _follower = new();
    private string? _confirmedPath;
    private DateTimeOffset? _confirmedNotWrittenBefore;

    /// <summary>
    /// Tells whether the file was last written before <paramref name="notWrittenBefore"/> (less the start tolerance) or after <paramref name="now"/> (plus a tolerance), which makes it not the running session's.
    /// </summary>
    /// <remarks>A file dated in the future comes from a clock that was set back; it proves nothing about the running process.</remarks>
    public static bool IsStale(string path, DateTimeOffset? notWrittenBefore, in DateTimeOffset now)
    {
        if (notWrittenBefore is not DateTimeOffset limit)
            return false;

        DateTimeOffset written = new(File.GetLastWriteTimeUtc(path));

        return (written < (limit - GameStateConstants.ProcessStartTolerance)) || (written > (now + GameStateConstants.FutureLogTolerance));
    }

    /// <summary>
    /// Tells whether the file belongs to the previous session; a file once found current for the same limit stays current.
    /// </summary>
    /// <remarks>Without that memory a clock set back would make the running session's own file look old.</remarks>
    public bool IsPreviousSession(string path, DateTimeOffset? notWrittenBefore)
    {
        if (string.Equals(_confirmedPath, path, StringComparison.Ordinal) && (_confirmedNotWrittenBefore == notWrittenBefore))
            return false;

        if (IsStale(path, notWrittenBefore, _timeProvider.GetUtcNow()))
            return true;

        _confirmedPath = path;
        _confirmedNotWrittenBefore = notWrittenBefore;

        return false;
    }

    /// <summary>
    /// Forgets the read position and the confirmed file, so the next poll reads the file from the start.
    /// </summary>
    public void Restart()
    {
        _follower = new GameLogFollower();
        _confirmedPath = null;
        _confirmedNotWrittenBefore = null;
    }

    /// <summary>
    /// Reads the lines added since the last poll; a stale file is not read.
    /// </summary>
    public async Task<GameLogPollResultStruct> PollAsync(string path, DateTimeOffset? notWrittenBefore, CancellationToken cancellationToken)
    {
        if (IsPreviousSession(path, notWrittenBefore))
            return new GameLogPollResultStruct() { Events = [] };

        GameLogReadResultStruct read = await _follower.PollAsync(path, cancellationToken);
        List<TimedGameLogEventStruct> events = [];

        for (int i = 0; i < read.Lines.Count; i++)
        {
            if (GameLogParser.TryParse(read.Lines[i], out GameLogEntryStruct entry) &&
                GameLogEventClassifier.TryClassify(entry, out GameLogEventStruct gameEvent))
            {
                events.Add(new TimedGameLogEventStruct() { Timestamp = entry.Timestamp, GameEvent = gameEvent });
            }
        }

        return new GameLogPollResultStruct() { Events = events, Restarted = read.Restarted };
    }
}
