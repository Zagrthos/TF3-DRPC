using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Tf3Drpc.Core.Constants;
using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Structs;
using Tf3Drpc.Core.Utilities;

namespace Tf3Drpc.Core.Discord;

/// <summary>
/// One handshaken connection to Discord with a background read loop.
/// </summary>
internal sealed class DiscordIpcConnection : IAsyncDisposable
{
    private static readonly byte[] EmptyObject = "{}"u8.ToArray();
    private readonly Stream _stream;
    private readonly Action<int?, string?> _onError;
    private readonly TimeSpan _writeTimeout;
    private readonly SemaphoreSlim _writeLock = new(1, 1);
    private readonly CancellationTokenSource _readCancellation = new();
    private Task _readLoop = Task.CompletedTask;
    private volatile bool _dead;
    private string? _activityNonce;
    private string? _rejectedNonce;
    private int _disposed;

    private DiscordIpcConnection(Stream stream, Action<int?, string?> onError, in TimeSpan writeTimeout)
    {
        _stream = stream;
        _onError = onError;
        _writeTimeout = writeTimeout;
    }

    #region Opening and disposing

    /// <summary>
    /// Tells whether the connection is still usable.
    /// </summary>
    public bool IsAlive
        => !_dead;

    /// <summary>
    /// Handshakes on the stream and waits for READY; returns <see langword="null"/> and closes the stream on failure.
    /// </summary>
    public static async Task<DiscordIpcConnection?> OpenAsync
    (
        Stream stream,
        Action<int?, string?> onError,
        TimeSpan readyTimeout,
        TimeSpan writeTimeout,
        CancellationToken cancellationToken
    )
    {
        DiscordIpcConnection connection = new(stream, onError, writeTimeout);
        bool ready;

        try
        {
            ready = await connection.HandshakeAsync(readyTimeout, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            await connection.DisposeAsync();

            throw;
        }

        if (!ready)
        {
            await connection.DisposeAsync();

            return null;
        }

        connection._readLoop = Task.Run(() => connection.ReadLoopAsync(connection._readCancellation.Token), CancellationToken.None);

        return connection;
    }

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, 1) is not 0)
            return;

        if (!_dead)
            await TrySendCloseAsync();

        _dead = true;
        await _readCancellation.CancelAsync();
        await _stream.DisposeAsync();

        try
        {
            await _readLoop;
        }
        catch (Exception exception) when (IsConnectionFailure(exception))
        {
            // The loop ends because the stream was closed.
        }

        _readCancellation.Dispose();
        _writeLock.Dispose();
    }

    #endregion Opening and disposing

    #region Sending

    /// <summary>
    /// Sends SET_ACTIVITY and returns its nonce; <see langword="null"/> content clears the activity.
    /// </summary>
    public async Task<string> SendActivityAsync(PresenceContentStruct? content, CancellationToken cancellationToken)
    {
        string nonce = Guid.NewGuid().ToString();
        byte[] payload = DiscordMessageUtils.BuildSetActivity(content, nonce);

        Volatile.Write(ref _activityNonce, nonce);
        await WriteFrameAsync(DiscordOpcode.Frame, payload, cancellationToken);

        return nonce;
    }

    /// <summary>
    /// Tells whether Discord answered the activity command with this nonce by an error.
    /// </summary>
    public bool WasRejected(string nonce)
        => string.Equals(Volatile.Read(ref _rejectedNonce), nonce, StringComparison.Ordinal);

    private async Task WriteFrameAsync(DiscordOpcode opcode, ReadOnlyMemory<byte> payload, CancellationToken cancellationToken)
    {
        byte[] frame = DiscordFrameUtils.Encode(opcode, payload.Span);

        using CancellationTokenSource deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        deadline.CancelAfter(_writeTimeout);

        try
        {
            await _writeLock.WaitAsync(deadline.Token);
        }
        catch (OperationCanceledException exception) when (!cancellationToken.IsCancellationRequested)
        {
            _dead = true;

            throw new TimeoutException("Waiting for the Discord write lock timed out.", exception);
        }

        try
        {
            await _stream.WriteAsync(frame, deadline.Token);
            await _stream.FlushAsync(deadline.Token);
        }
        catch (OperationCanceledException exception) when (!cancellationToken.IsCancellationRequested)
        {
            _dead = true;

            throw new TimeoutException("Writing to Discord timed out.", exception);
        }
        catch (Exception exception) when (IsConnectionFailure(exception))
        {
            _dead = true;

            throw;
        }
        finally
        {
            _writeLock.Release();
        }
    }

    private async Task TrySendCloseAsync()
    {
        try
        {
            using CancellationTokenSource timeout = new(DiscordIpcConstants.CloseTimeoutMilliseconds);
            await WriteFrameAsync(DiscordOpcode.Close, EmptyObject, timeout.Token);
        }
        catch (Exception exception) when (IsConnectionFailure(exception))
        {
            // Best effort only; the stream is closed next anyway.
        }
    }

    #endregion Sending

    #region Handshake and reading

    private static bool IsConnectionFailure(Exception exception)
        => exception is IOException or ObjectDisposedException or OperationCanceledException or InvalidOperationException
            or TimeoutException;

    private async Task<bool> HandshakeAsync(TimeSpan readyTimeout, CancellationToken cancellationToken)
    {
        using CancellationTokenSource timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(readyTimeout);

        try
        {
            await WriteFrameAsync(DiscordOpcode.Handshake, DiscordMessageUtils.BuildHandshake(), timeout.Token);

            while (true)
            {
                DiscordFrameStruct? frame = await DiscordFrameUtils.ReadAsync(_stream, timeout.Token);

                if (frame is null)
                    return false;

                if (frame.Value.Opcode is DiscordOpcode.Ping)
                {
                    await WriteFrameAsync(DiscordOpcode.Pong, frame.Value.Payload, timeout.Token);

                    continue;
                }

                if (frame.Value.Opcode is not DiscordOpcode.Frame)
                    return false;

                DiscordMessage? message = DiscordMessageUtils.ParseMessage(frame.Value.Payload.Span);

                return message is { Command: DiscordIpcConstants.DispatchCommand, Event: DiscordIpcConstants.ReadyEvent };
            }
        }
        catch (Exception exception) when (IsConnectionFailure(exception) && !cancellationToken.IsCancellationRequested)
        {
            return false;
        }
    }

    private async Task ReadLoopAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                DiscordFrameStruct? frame = await DiscordFrameUtils.ReadAsync(_stream, cancellationToken);

                if (frame is null)
                    break;

                DiscordFrameStruct current = frame.Value;

                if (current.Opcode is DiscordOpcode.Close)
                    break;

                if (current.Opcode is DiscordOpcode.Ping)
                {
                    await WriteFrameAsync(DiscordOpcode.Pong, current.Payload, cancellationToken);
                }
                else if (current.Opcode is DiscordOpcode.Frame)
                {
                    LogIfError(current.Payload);
                }
            }
        }
        catch (Exception exception) when (IsConnectionFailure(exception))
        {
            // The connection ended; the owner notices through IsAlive.
        }
        finally
        {
            _dead = true;
        }
    }

    private void LogIfError(in ReadOnlyMemory<byte> payload)
    {
        DiscordMessage? message = DiscordMessageUtils.ParseMessage(payload.Span);

        if (message is not { Event: DiscordIpcConstants.ErrorEvent })
            return;

        if ((message.Nonce is string nonce) && string.Equals(nonce, Volatile.Read(ref _activityNonce), StringComparison.Ordinal))
            Volatile.Write(ref _rejectedNonce, nonce);

        _onError(message.Data?.Code, message.Data?.Message);
    }

    #endregion Handshake and reading
}
