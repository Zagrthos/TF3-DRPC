using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Languages;
using Tf3Drpc.Core.Monitor;
using Tf3Drpc.Core.Settings;
using Tf3Drpc.Core.Structs;

namespace Tf3Drpc.Core.Tests.Monitor;

internal sealed record LoggedEntry(LogLevel Level, string Message, Exception? Exception);

internal sealed record LoadedTexts(string? GameDirectory, string LanguageCode, string GameLanguageCode);

/// <summary>
/// Stands in for the six sources and sinks of the monitor; records every call and is configured through its properties.
/// </summary>
internal sealed class FakeMonitorServices : IGameProcessSource, IGamePathResolver, ISettingsSource, ILanguageResolver, IPresenceBuilder, IPresenceSink
{
    public GameProcessInfoStruct Process { get; set; }

    public Exception? CheckException { get; set; }

    public int CheckCount { get; private set; }

    public GamePathsStruct Paths { get; set; }

    public List<string?> ResolvedExecutables { get; } = [];

    public AppSettings Settings { get; set; } = new();

    public AppSettings Current
        => Settings;

    public string? GameLanguageCode { get; set; }

    public List<string?> ProfileReads { get; } = [];

    public List<LoadedTexts> LoadedGameTexts { get; } = [];

    public bool BuildsPresence { get; set; } = true;

    public List<GameStateSnapshotStruct> BuiltSnapshots { get; } = [];

    public bool IsConnected { get; set; }

    public Exception? UpdateException { get; set; }

    public List<PresenceContentStruct?> Updates { get; } = [];

    public GameProcessInfoStruct Check()
    {
        CheckCount++;

        if (CheckException is not null)
            throw CheckException;

        return Process;
    }

    public GamePathsStruct ResolveGamePaths(string? runningExecutablePath)
    {
        ResolvedExecutables.Add(runningExecutablePath);

        return Paths;
    }

    public LanguageInfo Resolve(string setting, string? gameLanguageCode)
        => LanguageCatalog.Resolve(setting, gameLanguageCode);

    public string? ReadGameLanguageCode(string? profilePath)
    {
        ProfileReads.Add(profilePath);

        return GameLanguageCode;
    }

    public void LoadGameTexts(string? gameDirectory, LanguageInfo language, LanguageInfo gameLanguage)
        => LoadedGameTexts.Add(new LoadedTexts(gameDirectory, language.Code, gameLanguage.Code));

    public bool TryBuild(in GameStateSnapshotStruct snapshot, LanguageInfo language, out PresenceContentStruct content)
    {
        BuiltSnapshots.Add(snapshot);
        content = new PresenceContentStruct() { Details = snapshot.State.ToString(), State = language.Code };

        return BuildsPresence && (snapshot.State is not GameState.NotRunning);
    }

    public Task UpdateAsync(PresenceContentStruct? desired, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Updates.Add(desired);

        return (UpdateException is null) ? Task.CompletedTask : Task.FromException(UpdateException);
    }
}

internal sealed class RecordingMonitorLogger : ILogger<GameMonitor>
{
    public List<LoggedEntry> Entries { get; } = [];

    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull
        => null;

    public bool IsEnabled(LogLevel logLevel)
        => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        => Entries.Add(new LoggedEntry(logLevel, formatter(state, exception), exception));

    public int Count(string messageStart)
    {
        int count = 0;

        foreach (LoggedEntry entry in Entries)
        {
            if (entry.Message.StartsWith(messageStart, StringComparison.Ordinal))
                count++;
        }

        return count;
    }
}
