using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Monitor;
using Tf3Drpc.Core.Structs;
using Tf3Drpc.Core.Tests.Monitor;

using Xunit;

namespace Tf3Drpc.Core.Tests.Review;

// Review tests: each passes on the reviewed GameMonitor and fails on one mutant of GameMonitor that the shipped GameMonitorTests do not catch.
public sealed class GameMonitorReviewTests : IDisposable
{
    private const string ExecutablePath = "/games/TransportFever3";
    private const string ProfilePath = "/games/userdata/profile.lua";
    private const string GameDirectory = "/games/Transport Fever 3";
    private static readonly DateTimeOffset Start = new(2026, 10, 22, 10, 0, 0, TimeSpan.Zero);
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "tf3drpc-review-" + Guid.NewGuid().ToString("N"));
    private readonly string _logPath;
    private readonly SettableTimeProvider _clock = new() { Now = Start.AddSeconds(10) };
    private readonly FakeMonitorServices _services = new();
    private readonly RecordingMonitorLogger _logger = new();
    private readonly GameMonitor _monitor;

    public GameMonitorReviewTests()
    {
        Directory.CreateDirectory(_directory);
        _logPath = Path.Combine(_directory, "stdout.txt");
        _services.Process = Running(Start);
        _services.Paths = new GamePathsStruct() { LogPath = _logPath, ProfilePath = ProfilePath, GameDirectory = GameDirectory };
        _monitor = new GameMonitor(_services, _services, _services, _services, _services, _services, _clock, _logger);
    }

    public void Dispose()
        => Directory.Delete(_directory, recursive: true);

    private static GameProcessInfoStruct Running(DateTimeOffset startedAt)
        => new() { IsRunning = true, ExecutablePath = ExecutablePath, StartTime = startedAt };

    private static string Line(DateTimeOffset timestamp, string message)
        => string.Create(CultureInfo.InvariantCulture, $"[{timestamp.UtcDateTime:yyyy-MM-dd HH:mm:ss}Z - MESSAGE  - Main Thread      - Main           ]  {message}\r\n");

    private void WriteLog(string content, DateTimeOffset written)
    {
        File.WriteAllText(_logPath, content);
        File.SetLastWriteTimeUtc(_logPath, written.UtcDateTime);
    }

    private void AppendLog(string content, DateTimeOffset written)
    {
        File.AppendAllText(_logPath, content);
        File.SetLastWriteTimeUtc(_logPath, written.UtcDateTime);
    }

    private Task<AppStatusStruct?> TickAsync()
        => _monitor.TickAsync(TestContext.Current.CancellationToken);

    private Task<AppStatusStruct?> TickAfterAsync(TimeSpan elapsed)
    {
        _clock.Now += elapsed;

        return TickAsync();
    }

    // Mutant M5 (drop "if (result.Restarted) _stateMachine.Reset();"): the shipped RewrittenLog_ResetsTheStateAsync still passes,
    // because its rewritten log starts with "Application startup. Hello!", which resets the state by itself.
    [Fact]
    public async Task RewrittenLogWithoutAStartupLine_ResetsStateAndBuildAsync()
    {
        WriteLog(Line(Start.AddSeconds(1), "Application startup. Hello!") + Line(Start.AddSeconds(2), "Starting up build version: 40420, build name \"Build\"") + Line(Start.AddSeconds(5), "Game is ready"), Start.AddSeconds(5));
        AppStatusStruct? playing = await TickAsync();

        WriteLog(Line(Start.AddSeconds(8), "Saved settings to settings.lua"), Start.AddSeconds(9));
        AppStatusStruct? reset = await TickAsync();

        Assert.Equal(GameState.Playing, playing?.Game.State);
        Assert.Equal(GameState.Starting, reset?.Game.State);
        Assert.Null(reset?.Game.GameBuild);
    }

    // Mutant M13 (no "_pathAttemptTimestamp = ..." after a retry): the shipped retry tests only measure the first interval, which
    // HandleProcessChange starts; without the timestamp every tick after the first retry resolves the paths again.
    [Fact]
    public async Task IncompletePaths_WaitTheRetryIntervalAgainAfterARetryAsync()
    {
        _services.Paths = new GamePathsStruct() { GameDirectory = GameDirectory };
        _ = await TickAsync();
        _ = await TickAfterAsync(TimeSpan.FromSeconds(15));
        _ = await TickAfterAsync(TimeSpan.FromSeconds(1));
        _ = await TickAfterAsync(TimeSpan.FromSeconds(1));

        Assert.Equal(2, _services.ResolvedExecutables.Count);
    }

    // Mutant M3 (no "_profileReadTimestamp = null" on a game start): a new game started within 30 s of the last read keeps the old language.
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

    // Mutant M7 (drop "?? _gameLanguageCode"): an unreadable profile would forget the known game language.
    [Fact]
    public async Task UnreadableProfile_KeepsTheKnownGameLanguageAsync()
    {
        _services.GameLanguageCode = "fr";
        _ = await TickAsync();
        _services.GameLanguageCode = null;
        _ = await TickAfterAsync(TimeSpan.FromSeconds(30));

        Assert.Equal(2, _services.ProfileReads.Count);
        Assert.Equal("fr", _monitor.Status.Language.Code);
    }

    // Mutant M8 (state log ignores the mission number): going from mission 1 to mission 2 within one poll is not logged.
    [Fact]
    public async Task MissionChangeWithinOnePoll_IsLoggedAsync()
    {
        const string Mission1 = "Campaign/mission: urbangames_campaign::/info.campaign/urbangames_campaign_mission_01::/info.mission";
        const string Mission2 = "Campaign/mission: urbangames_campaign::/info.campaign/urbangames_campaign_mission_02::/info.mission";
        WriteLog(Line(Start.AddSeconds(3), Mission1) + Line(Start.AddSeconds(5), "Game is ready"), Start.AddSeconds(5));
        AppStatusStruct? first = await TickAsync();
        AppendLog(Line(Start.AddSeconds(7), Mission2) + Line(Start.AddSeconds(9), "Game is ready"), Start.AddSeconds(9));
        AppStatusStruct? second = await TickAsync();

        Assert.Equal(1, first?.Game.MissionNumber);
        Assert.Equal(2, second?.Game.MissionNumber);
        Assert.Equal(1, _logger.Count("Game state: Mission, mission 2."));
    }
}
