using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Tf3Drpc.Core.Monitor;
using Tf3Drpc.Core.Structs;
using Tf3Drpc.Core.Tests.Monitor;

using Xunit;

namespace Tf3Drpc.Core.Tests.Review;

// Review test: the "game's files were not found yet" warning is never logged when the paths are missing from the start.
public sealed class GameMonitorPathsNotFoundReviewTests
{
    private static readonly DateTimeOffset Start = new(2026, 10, 22, 10, 0, 0, TimeSpan.Zero);
    private readonly SettableTimeProvider _clock = new() { Now = Start.AddSeconds(10) };
    private readonly FakeMonitorServices _services = new();
    private readonly RecordingMonitorLogger _logger = new();

    private GameMonitor CreateMonitor()
        => new(_services, _services, _services, _services, _services, _services, _clock, _logger);

    [Fact]
    public async Task NoGamePathsAtAll_WhileTheGameRuns_LogsTheNotFoundWarningAsync()
    {
        _services.Paths = default;
        _services.Process = new GameProcessInfoStruct() { IsRunning = true, ExecutablePath = "/games/TransportFever3", StartTime = Start };
        GameMonitor monitor = CreateMonitor();

        for (int i = 0; i < 10; i++)
        {
            _ = await monitor.TickAsync(TestContext.Current.CancellationToken);
            _clock.Now += TimeSpan.FromSeconds(15);
        }

        foreach (LoggedEntry entry in _logger.Entries)
            TestContext.Current.TestOutputHelper?.WriteLine($"{entry.Level}: {entry.Message}");

        Assert.Equal(10, _services.ResolvedExecutables.Count);
        Assert.Equal(1, _logger.Count("The game's files were not found yet"));
    }

    [Fact]
    public async Task NoGamePathsAtAll_WithoutTheGame_LogsTheNotFoundWarningAsync()
    {
        _services.Paths = default;
        _services.Process = default;
        GameMonitor monitor = CreateMonitor();

        for (int i = 0; i < 4; i++)
        {
            _ = await monitor.TickAsync(TestContext.Current.CancellationToken);
            _clock.Now += TimeSpan.FromSeconds(15);
        }

        Assert.Equal(4, _services.ResolvedExecutables.Count);
        Assert.Equal(1, _logger.Count("The game's files were not found yet"));
    }
}
