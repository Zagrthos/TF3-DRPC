using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Tf3Drpc.App.Services.Interfaces;
using Tf3Drpc.Core.Discord;
using Tf3Drpc.Core.Structs;

namespace Tf3Drpc.App.Services;

/// <summary>
/// Wraps the Discord presence client of the core library.
/// </summary>
#pragma warning disable CA1812 // Avoid uninstantiated internal classes
internal sealed class DiscordService(TimeProvider timeProvider, ILoggerFactory loggerFactory) : IDiscordService
#pragma warning restore CA1812
{
    private readonly DiscordPresenceClient _client = new(timeProvider, loggerFactory.CreateLogger<DiscordPresenceClient>());

    public bool IsConnected
        => _client.IsConnected;

    public Task UpdateAsync(PresenceContentStruct? desired, CancellationToken cancellationToken)
        => _client.UpdateAsync(desired, cancellationToken);

    public ValueTask DisposeAsync()
        => _client.DisposeAsync();
}
