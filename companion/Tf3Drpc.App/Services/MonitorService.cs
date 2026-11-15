using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;

using Microsoft.Extensions.Logging;

using Tf3Drpc.App.Constants;
using Tf3Drpc.App.Logging;
using Tf3Drpc.App.Messages;
using Tf3Drpc.App.Services.Interfaces;
using Tf3Drpc.Core.Monitor;
using Tf3Drpc.Core.Structs;

namespace Tf3Drpc.App.Services;

/// <summary>
/// Runs the one background loop that drives the <see cref="GameMonitor"/> and sends its status changes.
/// </summary>
#pragma warning disable CA1812 // Avoid uninstantiated internal classes
internal sealed class MonitorService(
    IGameProcessService processService,
    IPathService pathService,
    ISettingsService settingsService,
    ITranslationService translationService,
    IPresenceService presenceService,
    IDiscordService discordService,
    IMessenger messenger,
    TimeProvider timeProvider,
    ILoggerFactory loggerFactory
)
    : IMonitorService
#pragma warning restore CA1812
{
    private readonly GameMonitor _monitor = new(processService, pathService, settingsService, translationService, presenceService, discordService, timeProvider, loggerFactory.CreateLogger<GameMonitor>());
    private readonly IMessenger _messenger = messenger;
    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly ILogger<MonitorService> _logger = loggerFactory.CreateLogger<MonitorService>();
    private CancellationTokenSource? _cancellation;
    private Task _loop = Task.CompletedTask;

    public AppStatusStruct Status
        => _monitor.Status;

    public void Start()
    {
        if (_cancellation is not null)
            return;

        _cancellation = new CancellationTokenSource();
        CancellationToken token = _cancellation.Token;
        _loop = Task.Run(() => RunAsync(token), CancellationToken.None);
    }

    public async Task StopAsync()
    {
        CancellationTokenSource? cancellation = _cancellation;

        if (cancellation is null)
            return;

        await cancellation.CancelAsync();
        await _loop;
        cancellation.Dispose();
        _cancellation = null;
    }

    public async ValueTask DisposeAsync()
        => await StopAsync();

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        using PeriodicTimer timer = new(AppConstants.TickInterval, _timeProvider);

        try
        {
            do
            {
                if (await _monitor.TickAsync(cancellationToken) is AppStatusStruct status)
                    SendStatus(status);
            }
            while (await timer.WaitForNextTickAsync(cancellationToken));
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // The app is exiting.
        }
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "A failing receiver must never end the loop.")]
    private void SendStatus(in AppStatusStruct status)
    {
        try
        {
            _messenger.Send(new AppStatusChangedMessage(status));
        }
        catch (Exception exception)
        {
            _logger.StatusSendFailed(exception);
        }
    }
}
