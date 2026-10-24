using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Tf3Drpc.Core.Constants;
using Tf3Drpc.Core.Logging;
using Tf3Drpc.Core.Structs;

namespace Tf3Drpc.Core.Discord;

/// <summary>
/// Keeps the Discord presence in line with the desired content; call it from one loop only.
/// </summary>
/// <remarks>
/// Connects only while there is something to show and sends at most once per update interval. Nothing to show
/// closes the connection at once instead of sending a rate-limited clear, because Discord clears the presence
/// of a closed connection.
/// </remarks>
public sealed class DiscordPresenceClient : IAsyncDisposable
{
    private readonly IDiscordConnector _connector;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<DiscordPresenceClient> _logger;
    private readonly TimeSpan _readyTimeout;
    private readonly TimeSpan _writeTimeout;
    private DiscordIpcConnection? _connection;
    private PresenceContentStruct? _lastSent;
    private PresenceContentStruct? _rejectedContent;
    private string? _lastNonce;
    private TimeSpan _rejectedInterval = DiscordIpcConstants.UpdateInterval;
    private int _rejections;
    private long? _lastSendTimestamp;
    private long? _lastConnectTimestamp;

    /// <summary>
    /// Creates a client that talks to the running Discord client.
    /// </summary>
    public DiscordPresenceClient(TimeProvider timeProvider, ILogger<DiscordPresenceClient> logger)
        : this(new DiscordConnector(), timeProvider, logger)
    { }

#pragma warning disable IDE0290 // Use primary constructor: it cannot take its parameter list on separate lines
    internal DiscordPresenceClient
    (
        IDiscordConnector connector,
        TimeProvider timeProvider,
        ILogger<DiscordPresenceClient> logger,
        TimeSpan? readyTimeout = null,
        TimeSpan? writeTimeout = null
    )
    {
        _connector = connector;
        _timeProvider = timeProvider;
        _logger = logger;
        _readyTimeout = readyTimeout ?? DiscordIpcConstants.ReadyTimeout;
        _writeTimeout = writeTimeout ?? DiscordIpcConstants.WriteTimeout;
    }
#pragma warning restore IDE0290

    #region Public API

    /// <summary>
    /// Tells whether a handshaken connection to Discord exists.
    /// </summary>
    public bool IsConnected
        => _connection is { IsAlive: true };

    /// <summary>
    /// Applies the desired presence; <see langword="null"/> means clear. Only cancellation is thrown.
    /// </summary>
    public async Task UpdateAsync(PresenceContentStruct? desired, CancellationToken cancellationToken)
    {
        try
        {
            await DropDeadConnectionAsync();

            if (_connection is null)
                await TryConnectAsync(desired, cancellationToken);

            if (_connection is not null)
                await SynchroniseAsync(_connection, desired, cancellationToken);
        }
        catch (Exception exception) when (IsExpectedFailure(exception) && !cancellationToken.IsCancellationRequested)
        {
            _logger.DiscordConnectionLost();
            await DropConnectionAsync();
        }
    }

    public async ValueTask DisposeAsync()
        => await DropConnectionAsync();

    #endregion Public API

    #region Connection handling

    private static bool IsExpectedFailure(Exception exception)
        => exception is IOException or ObjectDisposedException or SocketException or InvalidOperationException
            or UnauthorizedAccessException or NotSupportedException or ArgumentException or TimeoutException;

    private async Task DropDeadConnectionAsync()
    {
        if (_connection is not { IsAlive: false })
            return;

        _logger.DiscordConnectionLost();
        await DropConnectionAsync();
    }

    private async Task DropConnectionAsync()
    {
        DiscordIpcConnection? connection = _connection;
        _connection = null;
        _lastSent = null;
        _lastNonce = null;

        if (connection is null)
            return;

        await connection.DisposeAsync();
        _logger.DiscordDisconnected();
    }

    private async Task TryConnectAsync(PresenceContentStruct? desired, CancellationToken cancellationToken)
    {
        if (desired is null)
            return;

        if ((_lastConnectTimestamp is long attempt) && (_timeProvider.GetElapsedTime(attempt) < DiscordIpcConstants.ReconnectInterval))
            return;

        _lastConnectTimestamp = _timeProvider.GetTimestamp();
        Stream? stream = await _connector.ConnectAsync(cancellationToken);

        if (stream is null)
        {
            _logger.DiscordNotAvailable();

            return;
        }

        DiscordIpcConnection? connection = await DiscordIpcConnection.OpenAsync(stream, _logger.DiscordError, _readyTimeout, _writeTimeout, cancellationToken);

        if (connection is null)
        {
            _logger.DiscordHandshakeFailed();

            return;
        }

        _connection = connection;
        _lastSent = null;
        _lastNonce = null;
        ResetRejections();
        _lastSendTimestamp = null;
        _logger.DiscordConnected();
    }

    #endregion Connection handling

    #region Synchronisation

    private async Task SynchroniseAsync(DiscordIpcConnection connection, PresenceContentStruct? desired, CancellationToken cancellationToken)
    {
        if (desired is null)
        {
            await DropConnectionAsync();

            return;
        }

        long now = _timeProvider.GetTimestamp();

        ObserveRejection(connection, now);

        if (desired == _lastSent)
            return;

        bool retry = desired == _rejectedContent;
        TimeSpan interval = (retry) ? _rejectedInterval : DiscordIpcConstants.UpdateInterval;

        if (_lastSendTimestamp is long sent)
        {
            TimeSpan elapsed = _timeProvider.GetElapsedTime(sent, now);

            if (elapsed < interval)
            {
                _logger.DiscordSendDeferred((interval - elapsed).TotalSeconds);

                return;
            }
        }

        if (!retry)
            ResetRejections();

        _lastNonce = await connection.SendActivityAsync(desired, cancellationToken);
        _lastSent = desired;
        _lastSendTimestamp = now;
    }

    private void ObserveRejection(DiscordIpcConnection connection, long now)
    {
        if (_lastNonce is not string nonce)
            return;

        if (connection.WasRejected(nonce))
        {
            _lastNonce = null;
            _rejectedContent = _lastSent;
            _lastSent = null;
            _rejections++;
            _rejectedInterval = (_rejections is 1)
                ? DiscordIpcConstants.UpdateInterval
                : Min(_rejectedInterval * 2, DiscordIpcConstants.MaxRejectedRetryInterval);
            _logger.DiscordActivityRejected(_rejectedInterval.TotalSeconds);

            return;
        }

        bool settled = _lastSendTimestamp is long sent && (_timeProvider.GetElapsedTime(sent, now) >= DiscordIpcConstants.UpdateInterval);

        if (settled)
            ResetRejections();
    }

    private static TimeSpan Min(in TimeSpan first, in TimeSpan second)
        => (first < second) ? first : second;

    private void ResetRejections()
    {
        _rejections = 0;
        _rejectedContent = null;
        _rejectedInterval = DiscordIpcConstants.UpdateInterval;
    }

    #endregion Synchronisation
}
