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

// Review check (refuted suspicion): a log path that changes while the game runs must not carry the old file's state or offset over.
public sealed class GameMonitorLogSwitchReviewTests : IDisposable
{
    private static readonly DateTimeOffset Start = new(2026, 10, 22, 10, 0, 0, TimeSpan.Zero);
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "tf3drpc-review-" + Guid.NewGuid().ToString("N"));

    public void Dispose()
        => Directory.Delete(_directory, recursive: true);

    private static string Line(DateTimeOffset timestamp, string message)
        => string.Create(CultureInfo.InvariantCulture, $"[{timestamp.UtcDateTime:yyyy-MM-dd HH:mm:ss}Z - MESSAGE  - Main Thread      - Main           ]  {message}\r\n");

    [Fact]
    public async Task LogPathSwitchWhileRunning_ResetsTheStateAndReadsTheNewFileFromTheStartAsync()
    {
        Directory.CreateDirectory(_directory);
        string first = Path.Combine(_directory, "a.txt");
        string second = Path.Combine(_directory, "b.txt");
        File.WriteAllText(first, Line(Start.AddSeconds(5), "Game is ready") + Line(Start.AddSeconds(6), "Saved settings to settings.lua"));
        File.SetLastWriteTimeUtc(first, Start.AddSeconds(6).UtcDateTime);
        File.WriteAllText(second, Line(Start.AddSeconds(3), "Main menu is ready"));
        File.SetLastWriteTimeUtc(second, Start.AddSeconds(6).UtcDateTime);
        SettableTimeProvider clock = new() { Now = Start.AddSeconds(10) };
        FakeMonitorServices services = new()
        {
            Process = new GameProcessInfoStruct() { IsRunning = true, ExecutablePath = "/games/TransportFever3", StartTime = Start },
            Paths = new GamePathsStruct() { LogPath = first, GameDirectory = "/games/Transport Fever 3" }
        };
        GameMonitor monitor = new(services, services, services, services, services, services, clock, new RecordingMonitorLogger());

        AppStatusStruct? playing = await monitor.TickAsync(TestContext.Current.CancellationToken);
        services.Paths = new GamePathsStruct() { LogPath = second, ProfilePath = "/games/userdata/profile.lua", GameDirectory = "/games/Transport Fever 3" };
        clock.Now += TimeSpan.FromSeconds(15);
        AppStatusStruct? switched = await monitor.TickAsync(TestContext.Current.CancellationToken);

        Assert.Equal(GameState.Playing, playing?.Game.State);
        Assert.Equal(GameState.MainMenu, switched?.Game.State);
    }
}
