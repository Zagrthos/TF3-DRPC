using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tf3Drpc.Core.Discord;
using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Utilities;

namespace Tf3Drpc.Core.Tests.Discord;

internal sealed class ByteQueue
{
    private readonly Lock _gate = new();
    private readonly Queue<byte> _buffer = new();
    private TaskCompletionSource? _waiter;
    private bool _closed;

    public int Count
    {
        get
        {
            lock (_gate)
            {
                return _buffer.Count;
            }
        }
    }

    public void Write(ReadOnlySpan<byte> data)
    {
        TaskCompletionSource? waiter;

        lock (_gate)
        {
            if (_closed)
                throw new IOException("The pipe is closed.");

            foreach (byte value in data)
            {
                _buffer.Enqueue(value);
            }

            waiter = _waiter;
            _waiter = null;
        }

        waiter?.TrySetResult();
    }

    public void Close()
    {
        TaskCompletionSource? waiter;

        lock (_gate)
        {
            _closed = true;
            waiter = _waiter;
            _waiter = null;
        }

        waiter?.TrySetResult();
    }

    public async ValueTask<int> ReadAsync(Memory<byte> destination, CancellationToken cancellationToken)
    {
        while (true)
        {
            Task wait;

            lock (_gate)
            {
                if (_buffer.Count is > 0)
                {
                    int count = Math.Min(destination.Length, _buffer.Count);

                    for (int i = 0; i < count; i++)
                    {
                        destination.Span[i] = _buffer.Dequeue();
                    }

                    return count;
                }

                if (_closed)
                    return 0;

                _waiter ??= new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
                wait = _waiter.Task;
            }

            await wait.WaitAsync(cancellationToken);
        }
    }
}

internal sealed class PipeStream(ByteQueue incoming, ByteQueue outgoing) : Stream
{
    public override bool CanRead
        => true;

    public override bool CanSeek
        => false;

    public override bool CanWrite
        => true;

    public override long Length
        => throw new NotSupportedException();

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    public override void Flush()
    { }

    public override Task FlushAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;

    public override int Read(byte[] buffer, int offset, int count)
        => throw new NotSupportedException();

    public override long Seek(long offset, SeekOrigin origin)
        => throw new NotSupportedException();

    public override void SetLength(long value)
        => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count)
        => outgoing.Write(buffer.AsSpan(offset, count));

    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        => incoming.ReadAsync(buffer, cancellationToken);

    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        outgoing.Write(buffer.Span);

        return ValueTask.CompletedTask;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            outgoing.Close();
            incoming.Close();
        }

        base.Dispose(disposing);
    }
}

internal sealed class StallableStream(Stream inner) : Stream
{
    private readonly Stream _inner = inner;

    public bool StallWrites { get; set; }

    public override bool CanRead
        => _inner.CanRead;

    public override bool CanSeek
        => false;

    public override bool CanWrite
        => _inner.CanWrite;

    public override long Length
        => throw new NotSupportedException();

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    public override void Flush()
        => _inner.Flush();

    public override Task FlushAsync(CancellationToken cancellationToken)
        => (StallWrites) ? Task.Delay(Timeout.Infinite, cancellationToken) : _inner.FlushAsync(cancellationToken);

    public override int Read(byte[] buffer, int offset, int count)
        => throw new NotSupportedException();

    public override long Seek(long offset, SeekOrigin origin)
        => throw new NotSupportedException();

    public override void SetLength(long value)
        => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count)
        => throw new NotSupportedException();

    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        => _inner.ReadAsync(buffer, cancellationToken);

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        if (StallWrites)
        {
            await Task.Delay(Timeout.Infinite, cancellationToken);

            return;
        }

        await _inner.WriteAsync(buffer, cancellationToken);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _inner.Dispose();

        base.Dispose(disposing);
    }
}

internal sealed record ReceivedFrame(DiscordOpcode Opcode, byte[] Payload)
{
    public string Json
        => Encoding.UTF8.GetString(Payload);
}

internal sealed class FakeServer(ByteQueue incoming, PipeStream stream)
{
    private readonly ByteQueue _incoming = incoming;
    private readonly PipeStream _stream = stream;

    public byte[]? HandshakeBytes { get; set; }

    public bool HasPendingInput
        => _incoming.Count is > 0;

    public async Task<ReceivedFrame> ReadFrameAsync(CancellationToken cancellationToken)
    {
        byte[] header = new byte[8];
        await _stream.ReadExactlyAsync(header, cancellationToken);
        uint opcode = BitConverter.ToUInt32(header, 0);
        uint length = BitConverter.ToUInt32(header, 4);
        byte[] payload = new byte[length];
        await _stream.ReadExactlyAsync(payload, cancellationToken);

        return new ReceivedFrame((DiscordOpcode)opcode, payload);
    }

    public Task SendAsync(DiscordOpcode opcode, string json)
        => SendRawAsync(DiscordFrameUtils.Encode(opcode, Encoding.UTF8.GetBytes(json)));

    public async Task SendRawAsync(byte[] bytes)
        => await _stream.WriteAsync(bytes);

    public async Task RejectActivitiesAsync(Action<string> onActivity, CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                ReceivedFrame frame = await ReadFrameAsync(cancellationToken);

                if (frame.Opcode is not DiscordOpcode.Frame)
                    continue;

                using JsonDocument document = JsonDocument.Parse(frame.Payload);
                string? nonce = document.RootElement.GetProperty("nonce").GetString();

                if (nonce is null)
                    continue;

                onActivity(nonce);
                await SendAsync
                (
                    DiscordOpcode.Frame,
                    "{\"cmd\":\"SET_ACTIVITY\",\"evt\":\"ERROR\",\"data\":{\"code\":4000,\"message\":\"Rejected\"},\"nonce\":\"" + nonce + "\"}"
                );
            }
        }
        catch (Exception exception) when (exception is EndOfStreamException or IOException or OperationCanceledException or ObjectDisposedException)
        {
            // The test ended or the connection was closed.
        }
    }

    public Task SendReadyAsync()
        => SendAsync(DiscordOpcode.Frame, "{\"cmd\":\"DISPATCH\",\"evt\":\"READY\",\"data\":{\"v\":1,\"user\":{\"id\":\"1\"}},\"nonce\":null}");

    public void Close()
        => _stream.Dispose();

    // Breaks only the client's writes; its read loop keeps waiting, so the next send is what fails.
    public void CloseInput()
        => _incoming.Close();
}

internal enum HandshakeReply
{
    Ready,
    Error,
    Silent
}

internal sealed class FakeConnector(CancellationToken cancellationToken) : IDiscordConnector
{
    private readonly CancellationToken _cancellationToken = cancellationToken;

    public bool Available { get; set; } = true;

    public HandshakeReply Reply { get; set; } = HandshakeReply.Ready;

    public int Calls { get; private set; }

    public List<StallableStream> Streams { get; } = [];

    public bool UseStallableStreams { get; set; }

    public bool RejectActivities { get; set; }

    public List<string> ActivityNonces { get; } = [];

    public List<FakeServer> Servers { get; } = [];

    public FakeServer Current
        => Servers[^1];

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "The client owns its stream and the fake server ends live as long as the test.")]
    public ValueTask<Stream?> ConnectAsync(CancellationToken cancellationToken)
    {
        Calls++;

        if (!Available)
            return ValueTask.FromResult<Stream?>(null);

        ByteQueue toServer = new();
        ByteQueue toClient = new();
        PipeStream client = new(toClient, toServer);
        PipeStream serverStream = new(toServer, toClient);
        FakeServer server = new(toServer, serverStream);
        Servers.Add(server);
        _ = Task.Run(() => RespondToHandshakeAsync(server), CancellationToken.None);

        if (!UseStallableStreams)
            return ValueTask.FromResult<Stream?>(client);

        StallableStream stallable = new(client);
        Streams.Add(stallable);

        return ValueTask.FromResult<Stream?>(stallable);
    }

    private void RecordNonce(string nonce)
    {
        lock (ActivityNonces)
        {
            ActivityNonces.Add(nonce);
        }
    }

    public int ActivityCount
    {
        get
        {
            lock (ActivityNonces)
            {
                return ActivityNonces.Count;
            }
        }
    }

    private async Task RespondToHandshakeAsync(FakeServer server)
    {
        ReceivedFrame frame = await server.ReadFrameAsync(_cancellationToken);
        server.HandshakeBytes = DiscordFrameUtils.Encode(frame.Opcode, frame.Payload);

        switch (Reply)
        {
            case HandshakeReply.Ready:
                await server.SendReadyAsync();

                if (RejectActivities)
                    await server.RejectActivitiesAsync(RecordNonce, _cancellationToken);

                break;

            case HandshakeReply.Error:
                await server.SendAsync(DiscordOpcode.Frame, "{\"cmd\":\"DISPATCH\",\"evt\":\"ERROR\",\"data\":{\"code\":4000,\"message\":\"Invalid client ID\"}}");
                break;

            case HandshakeReply.Silent:
                break;
        }
    }
}

internal sealed class ListLogger : ILogger<DiscordPresenceClient>
{
    public List<string> Messages { get; } = [];

    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull
        => null;

    public bool IsEnabled(LogLevel logLevel)
        => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        lock (Messages)
        {
            Messages.Add(formatter(state, exception));
        }
    }
}
