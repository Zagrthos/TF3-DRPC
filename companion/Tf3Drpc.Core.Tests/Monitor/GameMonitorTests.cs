using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Monitor;
using Tf3Drpc.Core.Settings;
using Tf3Drpc.Core.Structs;

using Xunit;

namespace Tf3Drpc.Core.Tests.Monitor;

public sealed class GameMonitorTests : IDisposable
{
    private const string ExecutablePath = "/games/TransportFever3";
    private const string ProfilePath = "/games/userdata/profile.lua";
    private const string GameDirectory = "/games/Transport Fever 3";
    private const string StartupMessage = "Application startup. Hello!";
    private const string MainMenuMessage = "Main menu is ready";
    private const string ReadyMessage = "Game is ready";
    private const string LoadingMessage = "Loading game from file Archipelago2";
    private const string BuildMessage = "Starting up build version: 40420, build name \"Build\"";
    private const string UnrelatedMessage = "Saved settings to settings.lua";
    private static readonly DateTimeOffset Start = new(2026, 10, 22, 10, 0, 0, TimeSpan.Zero);
    private static readonly TimeSpan PathRetry = TimeSpan.FromSeconds(15);
    private static readonly TimeSpan ProfileRead = TimeSpan.FromSeconds(30);
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "tf3drpc-tests-" + Guid.NewGuid().ToString("N"));
    private readonly string _logPath;
    private readonly SettableTimeProvider _clock = new() { Now = Start.AddSeconds(10) };
    private readonly FakeMonitorServices _services = new();
    private readonly RecordingMonitorLogger _logger = new();
    private readonly GameMonitor _monitor;

    public GameMonitorTests()
    {
        Directory.CreateDirectory(_directory);
        _logPath = Path.Combine(_directory, "stdout.txt");
        _services.Process = Running(Start);
        _services.Paths = CompletePaths(_logPath);
        _monitor = new GameMonitor(_services, _services, _services, _services, _services, _services, _clock, _logger);
    }

    public void Dispose()
        => Directory.Delete(_directory, recursive: true);

    private static GameProcessInfoStruct Running(in DateTimeOffset startedAt)
        => new() { IsRunning = true, ExecutablePath = ExecutablePath, StartTime = startedAt };

    private static GamePathsStruct CompletePaths(string logPath)
        => new() { LogPath = logPath, ProfilePath = ProfilePath, GameDirectory = GameDirectory };

    private static string Line(in DateTimeOffset timestamp, string message)
        => string.Create(CultureInfo.InvariantCulture, $"[{timestamp.UtcDateTime:yyyy-MM-dd HH:mm:ss}Z - MESSAGE  - Main Thread      - Main           ]  {message}\r\n");

    private static void SetWritten(string path, in DateTimeOffset written)
        => File.SetLastWriteTimeUtc(path, written.UtcDateTime);

    private void WriteLog(string content, in DateTimeOffset written)
        => WriteLog(_logPath, content, written);

    private static void WriteLog(string path, string content, in DateTimeOffset written)
    {
        File.WriteAllText(path, content);
        SetWritten(path, written);
    }

    private void AppendLog(string content, in DateTimeOffset written)
    {
        File.AppendAllText(_logPath, content);
        SetWritten(_logPath, written);
    }

    private Task<AppStatusStruct?> TickAsync()
        => _monitor.TickAsync(TestContext.Current.CancellationToken);

    private async Task<AppStatusStruct?> TickAfterAsync(TimeSpan elapsed)
    {
        _clock.Now += elapsed;

        return await TickAsync();
    }

    private PresenceContentStruct? LastUpdate()
        => _services.Updates[^1];

    [Fact]
    public async Task NoGame_ClearsThePresenceAndTheSecondIdenticalTickReportsNoChangeAsync()
    {
        _services.Process = default;

        AppStatusStruct? first = await TickAsync();
        AppStatusStruct? second = await TickAsync();

        Assert.Null(first);
        Assert.Null(second);
        Assert.Equal(GameState.NotRunning, _monitor.Status.Game.State);
        Assert.Equal([null, null], _services.Updates);
        Assert.Equal([null], _services.ResolvedExecutables);
        Assert.Equal(0, _logger.Count("The game started"));
    }

    [Fact]
    public async Task DiscordConnectionChange_ShowsInTheReturnedStatusAndIsLoggedAsync()
    {
        _services.Process = default;
        _services.IsConnected = true;

        AppStatusStruct? status = await TickAsync();

        Assert.NotNull(status);
        Assert.True(status.Value.DiscordConnected);
        Assert.Equal(status.Value, _monitor.Status);
        Assert.Equal(1, _logger.Count("Discord connection: True."));
        Assert.Null(await TickAsync());
    }

    [Fact]
    public async Task GameStarts_ResolvesPathsReadsTheLogFromTheStartAndPublishesTheStateAsync()
    {
        WriteLog(Line(Start.AddSeconds(1), StartupMessage) + Line(Start.AddSeconds(2), BuildMessage) + Line(Start.AddSeconds(3), MainMenuMessage) + Line(Start.AddSeconds(5), ReadyMessage), Start.AddSeconds(5));

        AppStatusStruct? status = await TickAsync();

        Assert.NotNull(status);
        Assert.Equal(GameState.Playing, status.Value.Game.State);
        Assert.Equal(40420, status.Value.Game.GameBuild);
        Assert.Equal("Playing", status.Value.PresenceDetails);
        Assert.Equal("en", status.Value.PresenceState);
        Assert.Equal("Playing", LastUpdate()?.Details);
        Assert.Equal([ExecutablePath], _services.ResolvedExecutables);
        Assert.Equal(1, _logger.Count($"The game started. Executable: {ExecutablePath}."));
        Assert.Equal(1, _logger.Count("Game paths: log"));
        Assert.Equal(1, _logger.Count("Game log: 4 new event(s), restarted: False."));
        Assert.Equal(1, _logger.Count("Game state: Playing"));
        Assert.Equal(1, _logger.Count("Game build: 40420."));
    }

    [Fact]
    public async Task NewLogLines_AreReadOnTheNextTickAndAnUnchangedTickReportsNoChangeAsync()
    {
        WriteLog(Line(Start.AddSeconds(5), ReadyMessage), Start.AddSeconds(5));
        _ = await TickAsync();

        AppStatusStruct? unchanged = await TickAsync();
        AppendLog(Line(Start.AddSeconds(8), LoadingMessage), Start.AddSeconds(8));
        AppStatusStruct? loading = await TickAsync();

        Assert.Null(unchanged);
        Assert.NotNull(loading);
        Assert.Equal(GameState.Loading, loading.Value.Game.State);
        Assert.Equal("Loading", LastUpdate()?.Details);
    }

    [Fact]
    public async Task LogLinesFromBeforeTheProcessStart_AreIgnoredAsync()
    {
        WriteLog(Line(Start.AddHours(-1), ReadyMessage), Start.AddSeconds(5));

        AppStatusStruct? old = await TickAsync();
        AppendLog(Line(Start.AddSeconds(3), MainMenuMessage), Start.AddSeconds(6));
        AppStatusStruct? current = await TickAsync();

        Assert.Equal(GameState.Starting, old?.Game.State);
        Assert.NotNull(current);
        Assert.Equal(GameState.MainMenu, current.Value.Game.State);
    }

    [Fact]
    public async Task LogWrittenBeforeTheProcessStart_IsNotReadAsync()
    {
        WriteLog(Line(Start.AddHours(-1), ReadyMessage), Start.AddMinutes(-30));

        AppStatusStruct? status = await TickAsync();

        Assert.Equal(GameState.Starting, status?.Game.State);
        Assert.Equal(0, _logger.Count("Game log:"));
    }

    [Fact]
    public async Task UnchangedGameBuild_IsLoggedOnlyOnceAcrossStateChangesAsync()
    {
        WriteLog(Line(Start.AddSeconds(1), StartupMessage) + Line(Start.AddSeconds(2), BuildMessage) + Line(Start.AddSeconds(3), MainMenuMessage), Start.AddSeconds(3));

        AppStatusStruct? menu = await TickAsync();
        AppendLog(Line(Start.AddSeconds(5), ReadyMessage), Start.AddSeconds(6));
        AppStatusStruct? playing = await TickAsync();

        Assert.Equal(GameState.MainMenu, menu?.Game.State);
        Assert.Equal(GameState.Playing, playing?.Game.State);
        Assert.Equal(40420, playing?.Game.GameBuild);
        Assert.Equal(1, _logger.Count("Game build: 40420."));
    }

    [Fact]
    public async Task PresenceBuilderSaysNothing_ClearsThePresenceWhileTheGameRunsAsync()
    {
        _services.BuildsPresence = false;
        WriteLog(Line(Start.AddSeconds(5), ReadyMessage), Start.AddSeconds(5));

        AppStatusStruct? status = await TickAsync();

        Assert.NotNull(status);
        Assert.Equal(GameState.Playing, status.Value.Game.State);
        Assert.Null(status.Value.PresenceDetails);
        Assert.Null(LastUpdate());
    }

    [Fact]
    public async Task IncompletePaths_AreResolvedAgainOnlyAtTheRetryIntervalAsync()
    {
        _services.Paths = new GamePathsStruct() { GameDirectory = GameDirectory };
        WriteLog(Line(Start.AddSeconds(5), ReadyMessage), Start.AddSeconds(5));
        _ = await TickAsync();
        _services.Paths = CompletePaths(_logPath);

        _ = await TickAfterAsync(PathRetry - TimeSpan.FromSeconds(1));
        int before = _services.ResolvedExecutables.Count;
        AppStatusStruct? status = await TickAfterAsync(TimeSpan.FromSeconds(1));

        Assert.Equal(1, before);
        Assert.Equal(2, _services.ResolvedExecutables.Count);
        Assert.Equal(1, _logger.Count("The game's files were not found yet"));
        Assert.Equal(1, _logger.Count("Game paths: log"));
        Assert.Equal(GameState.Playing, status?.Game.State);
    }

    [Fact]
    public async Task IncompletePaths_AreResolvedOverAndOverUntilTheyAreCompleteAsync()
    {
        WriteLog(Line(Start.AddSeconds(5), ReadyMessage), Start.AddSeconds(5));
        _services.Paths = new GamePathsStruct() { GameDirectory = GameDirectory };
        _ = await TickAsync();

        _ = await TickAfterAsync(PathRetry);
        _ = await TickAfterAsync(PathRetry);
        _services.Paths = CompletePaths(_logPath);
        _ = await TickAfterAsync(PathRetry);
        _ = await TickAfterAsync(PathRetry);
        _ = await TickAfterAsync(PathRetry);

        Assert.Equal(4, _services.ResolvedExecutables.Count);
    }

    [Fact]
    public async Task IncompletePaths_WaitTheRetryIntervalAgainAfterARetryAsync()
    {
        _services.Paths = new GamePathsStruct() { GameDirectory = GameDirectory };
        _ = await TickAsync();
        _ = await TickAfterAsync(PathRetry);
        _ = await TickAfterAsync(TimeSpan.FromSeconds(1));
        _ = await TickAfterAsync(TimeSpan.FromSeconds(1));

        Assert.Equal(2, _services.ResolvedExecutables.Count);
    }

    [Fact]
    public async Task UnchangedIncompletePaths_AreLoggedOnlyOnceAsync()
    {
        _services.Paths = new GamePathsStruct() { LogPath = _logPath };

        for (int i = 0; i < 4; i++)
            _ = await TickAfterAsync(PathRetry);

        Assert.Equal(4, _services.ResolvedExecutables.Count);
        Assert.Equal(1, _logger.Count("Game paths: log"));
        Assert.Equal(0, _logger.Count("The game's files were not found yet"));
    }

    [Fact]
    public async Task PathsThatDisappear_LogThatTheGameFilesWereNotFoundAsync()
    {
        _services.Paths = new GamePathsStruct() { LogPath = _logPath };
        _ = await TickAsync();
        _services.Paths = default;

        _ = await TickAfterAsync(PathRetry);
        _ = await TickAfterAsync(PathRetry);

        Assert.Equal(1, _logger.Count("Game paths: log"));
        Assert.Equal(1, _logger.Count("The game's files were not found yet"));
        Assert.Contains(_logger.Entries, static (entry) => entry.Level is LogLevel.Warning);
    }

    [Fact]
    public async Task PathsMissingFromTheStart_WhileTheGameRuns_LogTheNotFoundWarningOnceAsync()
    {
        _services.Paths = default;

        for (int i = 0; i < 10; i++)
            _ = await TickAfterAsync(PathRetry);

        Assert.Equal(10, _services.ResolvedExecutables.Count);
        Assert.Equal(1, _logger.Count("The game's files were not found yet"));
        Assert.Equal(0, _logger.Count("Game paths: log"));
    }

    [Fact]
    public async Task PathsMissingFromTheStart_WithoutTheGame_LogTheNotFoundWarningOnceAsync()
    {
        _services.Process = default;
        _services.Paths = default;

        for (int i = 0; i < 4; i++)
            _ = await TickAfterAsync(PathRetry);

        Assert.Equal(4, _services.ResolvedExecutables.Count);
        Assert.Equal(1, _logger.Count("The game's files were not found yet"));
        Assert.Equal(0, _logger.Count("Game paths: log"));
    }

    [Fact]
    public async Task PathsFoundAfterBeingMissingAndLostAgain_LogTheNotFoundWarningAgainAsync()
    {
        _services.Paths = default;
        _ = await TickAsync();
        _ = await TickAfterAsync(PathRetry);
        _services.Paths = new GamePathsStruct() { LogPath = _logPath };
        _ = await TickAfterAsync(PathRetry);
        _services.Paths = default;
        _ = await TickAfterAsync(PathRetry);
        _ = await TickAfterAsync(PathRetry);

        Assert.Equal(2, _logger.Count("The game's files were not found yet"));
        Assert.Equal(1, _logger.Count("Game paths: log"));
    }

    [Fact]
    public async Task CompletePaths_AreNotResolvedAgainAsync()
    {
        WriteLog(Line(Start.AddSeconds(5), ReadyMessage), Start.AddSeconds(5));
        _ = await TickAsync();

        _ = await TickAfterAsync(PathRetry);
        _ = await TickAfterAsync(PathRetry);
        _ = await TickAfterAsync(TimeSpan.FromMinutes(1));

        Assert.Single(_services.ResolvedExecutables);
    }

    [Fact]
    public async Task LogOfThePreviousSession_IsResolvedAgainAfterTheRetryIntervalAndTheCurrentLogIsReadAsync()
    {
        string oldLog = Path.Combine(_directory, "old.txt");
        WriteLog(oldLog, Line(Start.AddHours(-3), ReadyMessage), Start.AddHours(-3));
        _services.Paths = CompletePaths(oldLog);

        AppStatusStruct? first = await TickAsync();
        _ = await TickAfterAsync(TimeSpan.FromSeconds(10));
        int before = _services.ResolvedExecutables.Count;
        _services.Paths = CompletePaths(_logPath);
        WriteLog(Line(Start.AddSeconds(4), ReadyMessage), Start.AddSeconds(5));
        AppStatusStruct? switched = await TickAfterAsync(TimeSpan.FromSeconds(5));

        Assert.Equal(GameState.Starting, first?.Game.State);
        Assert.Equal(1, before);
        Assert.Equal(2, _services.ResolvedExecutables.Count);
        Assert.NotNull(switched);
        Assert.Equal(GameState.Playing, switched.Value.Game.State);
    }

    [Fact]
    public async Task Profile_IsReadOnTheFirstRunningTickAndAgainOnlyAfterTheIntervalAsync()
    {
        _ = await TickAsync();
        _ = await TickAfterAsync(ProfileRead - TimeSpan.FromSeconds(1));
        int before = _services.ProfileReads.Count;
        _ = await TickAfterAsync(TimeSpan.FromSeconds(1));
        _ = await TickAfterAsync(TimeSpan.FromSeconds(5));

        Assert.Equal(1, before);
        Assert.Equal([ProfilePath, ProfilePath], _services.ProfileReads);
    }

    [Fact]
    public async Task Profile_IsReadWhileTheGameStopsOnlyUntilTheGameLanguageIsKnownAsync()
    {
        _ = await TickAsync();
        _services.Process = default;

        _ = await TickAfterAsync(ProfileRead);
        int stillUnknown = _services.ProfileReads.Count;
        _services.GameLanguageCode = "de";
        AppStatusStruct? learned = await TickAfterAsync(ProfileRead);
        _ = await TickAfterAsync(ProfileRead);
        _ = await TickAfterAsync(ProfileRead);

        Assert.Equal(2, stillUnknown);
        Assert.Equal(3, _services.ProfileReads.Count);
        Assert.NotNull(learned);
        Assert.Equal("de", learned.Value.Language.Code);
    }

    [Fact]
    public async Task NewGameStart_ReadsTheProfileAtOnceAsync()
    {
        _services.GameLanguageCode = "fr";
        _ = await TickAsync();
        _services.Process = default;
        _ = await TickAfterAsync(TimeSpan.FromSeconds(5));
        _services.GameLanguageCode = "de";
        _services.Process = Running(Start.AddSeconds(18));
        AppStatusStruct? restarted = await TickAfterAsync(TimeSpan.FromSeconds(10));

        Assert.Equal(2, _services.ProfileReads.Count);
        Assert.Equal("de", restarted?.Language.Code);
    }

    [Fact]
    public async Task UnreadableProfile_KeepsTheKnownGameLanguageAsync()
    {
        _services.GameLanguageCode = "fr";
        _ = await TickAsync();
        _services.GameLanguageCode = null;
        _ = await TickAfterAsync(ProfileRead);

        Assert.Equal(2, _services.ProfileReads.Count);
        Assert.Equal("fr", _monitor.Status.Language.Code);
    }

    [Fact]
    public async Task SettingsLanguageChange_IsPickedUpOnTheNextTickAsync()
    {
        _services.Settings = new AppSettings() { Language = "en" };
        _services.GameLanguageCode = "fr";
        _ = await TickAsync();

        _services.Settings = new AppSettings() { Language = "de" };
        AppStatusStruct? status = await TickAsync();

        Assert.NotNull(status);
        Assert.Equal("de", status.Value.Language.Code);
        Assert.Equal("de", status.Value.PresenceState);
        Assert.Equal(1, _logger.Count("Language: de."));
        Assert.Equal(new LoadedTexts(GameDirectory, "de", "fr"), _services.LoadedGameTexts[^1]);
        Assert.Equal(new LoadedTexts(GameDirectory, "en", "fr"), _services.LoadedGameTexts[0]);
    }

    [Fact]
    public async Task GameLanguageSetting_FollowsTheLanguageOfTheProfileAsync()
    {
        _services.GameLanguageCode = "fr";

        AppStatusStruct? status = await TickAsync();

        Assert.NotNull(status);
        Assert.Equal("fr", status.Value.Language.Code);
        Assert.Equal(1, _logger.Count("Language: fr."));
        Assert.Equal(new LoadedTexts(GameDirectory, "fr", "fr"), _services.LoadedGameTexts[^1]);
    }

    [Fact]
    public async Task ReplacedGameProcess_ResolvesPathsAgainAndDoesNotCarryTheOldStateOverAsync()
    {
        WriteLog(Line(Start.AddSeconds(5), ReadyMessage), Start.AddSeconds(5));
        AppStatusStruct? playing = await TickAsync();

        DateTimeOffset replacedAt = Start.AddHours(1);
        _services.Process = Running(replacedAt);
        _clock.Now = replacedAt.AddSeconds(10);
        SetWritten(_logPath, replacedAt.AddSeconds(5));
        AppStatusStruct? restarted = await TickAsync();

        Assert.Equal(GameState.Playing, playing?.Game.State);
        Assert.NotNull(restarted);
        Assert.Equal(GameState.Starting, restarted.Value.Game.State);
        Assert.Equal(2, _services.ResolvedExecutables.Count);
        Assert.Equal(2, _logger.Count("The game started"));
        Assert.Equal("Starting", LastUpdate()?.Details);
    }

    [Fact]
    public async Task ReplacedGameProcess_ReadsTheLogFromTheStartAgainAsync()
    {
        WriteLog(Line(Start.AddSeconds(5), ReadyMessage), Start.AddSeconds(5));
        _ = await TickAsync();

        _services.Process = Running(Start.AddSeconds(4));
        AppStatusStruct? status = await TickAfterAsync(TimeSpan.FromSeconds(1));

        Assert.Equal(2, _logger.Count("Game log: 1 new event(s), restarted: False."));
        Assert.Equal(2, _services.ResolvedExecutables.Count);
        Assert.Equal(GameState.Playing, _monitor.Status.Game.State);
        Assert.Null(status);
    }

    [Fact]
    public async Task RewrittenLog_ResetsTheStateAsync()
    {
        WriteLog(Line(Start.AddSeconds(1), StartupMessage) + Line(Start.AddSeconds(2), BuildMessage) + Line(Start.AddSeconds(5), ReadyMessage), Start.AddSeconds(5));
        AppStatusStruct? playing = await TickAsync();

        WriteLog(Line(Start.AddSeconds(8), UnrelatedMessage), Start.AddSeconds(9));
        AppStatusStruct? reset = await TickAsync();

        Assert.Equal(GameState.Playing, playing?.Game.State);
        Assert.Equal(40420, playing?.Game.GameBuild);
        Assert.NotNull(reset);
        Assert.Equal(GameState.Starting, reset.Value.Game.State);
        Assert.Null(reset.Value.Game.GameBuild);
        Assert.Equal(1, _logger.Count("Game log: 0 new event(s), restarted: True."));
    }

    [Fact]
    public async Task MissionChangeWithinOnePoll_IsLoggedAsync()
    {
        const string Mission1 = "Campaign/mission: urbangames_campaign::/info.campaign/urbangames_campaign_mission_01::/info.mission";
        const string Mission2 = "Campaign/mission: urbangames_campaign::/info.campaign/urbangames_campaign_mission_02::/info.mission";
        WriteLog(Line(Start.AddSeconds(3), Mission1) + Line(Start.AddSeconds(5), ReadyMessage), Start.AddSeconds(5));
        AppStatusStruct? first = await TickAsync();
        AppendLog(Line(Start.AddSeconds(7), Mission2) + Line(Start.AddSeconds(9), ReadyMessage), Start.AddSeconds(9));
        AppStatusStruct? second = await TickAsync();

        Assert.Equal(1, first?.Game.MissionNumber);
        Assert.Equal(2, second?.Game.MissionNumber);
        Assert.Equal(1, _logger.Count("Game state: Mission, mission 2."));
    }

    [Fact]
    public async Task NoUsableLogLineForTwoMinutes_FallsBackToPlayingAndLogsOncePerGameSessionAsync()
    {
        WriteLog(Line(Start.AddSeconds(5), UnrelatedMessage), Start.AddSeconds(5));
        AppStatusStruct? starting = await TickAsync();

        _ = await TickAfterAsync(TimeSpan.FromSeconds(119));
        int beforeFallback = _logger.Count("The game log");
        AppStatusStruct? fallback = await TickAfterAsync(TimeSpan.FromSeconds(1));
        _ = await TickAfterAsync(TimeSpan.FromSeconds(10));
        _ = await TickAfterAsync(TimeSpan.FromSeconds(10));
        int afterFallback = _logger.Count("The game log");

        DateTimeOffset replacedAt = Start.AddHours(1);
        _services.Process = Running(replacedAt);
        _clock.Now = replacedAt.AddSeconds(10);
        SetWritten(_logPath, replacedAt.AddSeconds(5));
        _ = await TickAsync();
        _ = await TickAfterAsync(TimeSpan.FromMinutes(2));

        Assert.Equal(GameState.Starting, starting?.Game.State);
        Assert.Equal(0, beforeFallback);
        Assert.NotNull(fallback);
        Assert.True(fallback.Value.Game.UsesLogFallback);
        Assert.Equal(GameState.Playing, fallback.Value.Game.State);
        Assert.Equal(1, afterFallback);
        Assert.Equal(2, _logger.Count("The game log"));
        Assert.Contains(_logger.Entries, static (entry) => (entry.Level is LogLevel.Warning) && entry.Message.Contains("gave no usable lines", StringComparison.Ordinal));
    }

    [Fact]
    public async Task GameStops_LogsItAndClearsThePresenceAsync()
    {
        WriteLog(Line(Start.AddSeconds(5), ReadyMessage), Start.AddSeconds(5));
        _ = await TickAsync();

        _services.Process = default;
        AppStatusStruct? stopped = await TickAsync();
        AppStatusStruct? again = await TickAsync();

        Assert.NotNull(stopped);
        Assert.Equal(GameState.NotRunning, stopped.Value.Game.State);
        Assert.Null(stopped.Value.PresenceDetails);
        Assert.Null(LastUpdate());
        Assert.Null(again);
        Assert.Equal(1, _logger.Count("The game stopped."));
    }

    [Fact]
    public async Task DependencyThrows_ReturnsNullLogsTheFailureAndTheNextTickWorksAsync()
    {
        InvalidOperationException failure = new("boom");
        _services.CheckException = failure;

        AppStatusStruct? failed = await TickAsync();
        _services.CheckException = null;
        _services.IsConnected = true;
        AppStatusStruct? recovered = await TickAsync();

        Assert.Null(failed);
        LoggedEntry entry = Assert.Single(_logger.Entries, static (logged) => logged.Level is LogLevel.Error);
        Assert.Equal("A monitor tick failed.", entry.Message);
        Assert.Same(failure, entry.Exception);
        Assert.NotNull(recovered);
        Assert.True(recovered.Value.DiscordConnected);
        Assert.Equal(1, _logger.Count("A monitor tick failed."));
    }

    [Fact]
    public async Task CancelledToken_PropagatesTheCancellationAsync()
    {
        _services.Process = default;
        using CancellationTokenSource cancellation = new();
        await cancellation.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => _monitor.TickAsync(cancellation.Token));

        Assert.Equal(0, _logger.Count("A monitor tick failed."));
    }

    [Fact]
    public async Task CancellationWithoutACancelledToken_IsLoggedLikeAnyOtherFailureAsync()
    {
        _services.UpdateException = new OperationCanceledException();

        AppStatusStruct? status = await TickAsync();

        Assert.Null(status);
        Assert.Equal(1, _logger.Count("A monitor tick failed."));
    }
}
