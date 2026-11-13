using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tf3Drpc.Core.Discord;
using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Structs;
using Tf3Drpc.Core.Utilities;
using Xunit;

namespace Tf3Drpc.Core.Tests.Discord;

/// <summary>
/// Tests of <see cref="Tf3Drpc.Core.Discord.DiscordPresenceClient"/> against an in-memory fake Discord, with a settable clock, covering the handshake, rate limiting, reconnects and error handling.
/// </summary>
public sealed class DiscordPresenceClientTests : IAsyncDisposable
{
    private static readonly DateTimeOffset Start = new(2026, 10, 13, 21, 15, 24, TimeSpan.Zero);
    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(15);
    private readonly CancellationTokenSource _cancellation;

    private readonly SettableTimeProvider _clock = new() { Now = new DateTimeOffset(2026, 10, 14, 12, 0, 0, TimeSpan.Zero) };

    private readonly FakeConnector _connector;
    private readonly ListLogger _logger = new();
    private readonly DiscordPresenceClient _client;

    /// <summary>
    /// Creates the fake connector, the logger, the settable clock and the client under test, with a cancellation token that fires after 20 seconds.
    /// </summary>
    public DiscordPresenceClientTests()
    {
        _cancellation = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        _cancellation.CancelAfter(TimeSpan.FromSeconds(20));
        _connector = new FakeConnector(_cancellation.Token);
        _client = new DiscordPresenceClient(_connector, _clock, _logger, TimeSpan.FromMilliseconds(300));
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        await _client.DisposeAsync();
        _cancellation.Dispose();
    }

    private CancellationToken Token
        => _cancellation.Token;

    private static PresenceContentStruct Content(string details, string? state = "State", DateTimeOffset? start = null)
        => new() { Details = details, State = state, StartTimestamp = start ?? Start };

    private Task UpdateAsync(PresenceContentStruct? desired)
        => _client.UpdateAsync(desired, Token);

    private async Task WaitUntilAsync(Func<bool> condition)
    {
        while (!condition())
        {
            await Task.Delay(5, Token);
        }
    }

    private void Advance(in TimeSpan amount)
        => _clock.Now += amount;

    /// <summary>
    /// Connecting sends the handshake frame with the handshake opcode and the JSON of the protocol version and client id.
    /// </summary>
    [Fact]
    public async Task ConnectSendsTheExactHandshakeBytesAsync()
    {
        await UpdateAsync(Content("A"));

        byte[] expectedJson = Encoding.UTF8.GetBytes("{\"v\":1,\"client_id\":\"1559680023458484244\"}");
        byte[] expected = DiscordFrameUtils.Encode(DiscordOpcode.Handshake, expectedJson);
        Assert.True(_client.IsConnected);
        Assert.Equal(expected, _connector.Current.HandshakeBytes);
        Assert.Equal(new byte[] { 0, 0, 0, 0, (byte)expectedJson.Length, 0, 0, 0 }, _connector.Current.HandshakeBytes![..8]);
    }

    /// <summary>
    /// The first presence update after connecting sends a SET_ACTIVITY frame carrying the nonce, the process id, the details, the state, the start timestamp, the cover image with its text, and a false instance flag.
    /// </summary>
    [Fact]
    public async Task SetActivityCarriesAllFieldsAsync()
    {
        await UpdateAsync(Content("Mechanic", "September 1905"));

        ReceivedFrame frame = await _connector.Current.ReadFrameAsync(Token);
        using JsonDocument document = JsonDocument.Parse(frame.Payload);
        JsonElement root = document.RootElement;
        JsonElement activity = root.GetProperty("args").GetProperty("activity");

        Assert.Equal(DiscordOpcode.Frame, frame.Opcode);
        Assert.Equal("SET_ACTIVITY", root.GetProperty("cmd").GetString());
        Assert.True(Guid.TryParse(root.GetProperty("nonce").GetString(), out Guid _));
        Assert.Equal(Environment.ProcessId, root.GetProperty("args").GetProperty("pid").GetInt32());
        Assert.Equal("Mechanic", activity.GetProperty("details").GetString());
        Assert.Equal("September 1905", activity.GetProperty("state").GetString());
        Assert.Equal(Start.ToUnixTimeMilliseconds(), activity.GetProperty("timestamps").GetProperty("start").GetInt64());
        Assert.Equal("cover", activity.GetProperty("assets").GetProperty("large_image").GetString());
        Assert.Equal("Transport Fever 3", activity.GetProperty("assets").GetProperty("large_text").GetString());
        Assert.False(activity.GetProperty("instance").GetBoolean());
    }

    /// <summary>
    /// A presence without a state and a start timestamp sends an activity with neither a state nor a timestamps entry, but with its details.
    /// </summary>
    [Fact]
    public async Task SetActivityOmitsMissingStateAndTimerAsync()
    {
        PresenceContentStruct content = new() { Details = "Only details" };

        await UpdateAsync(content);
        ReceivedFrame frame = await _connector.Current.ReadFrameAsync(Token);
        using JsonDocument document = JsonDocument.Parse(frame.Payload);
        JsonElement activity = document.RootElement.GetProperty("args").GetProperty("activity");

        Assert.False(activity.TryGetProperty("state", out JsonElement _));
        Assert.False(activity.TryGetProperty("timestamps", out JsonElement _));
        Assert.Equal("Only details", activity.GetProperty("details").GetString());
    }

    /// <summary>
    /// A handshake that is answered with an error instead of ready leaves the client disconnected after a single connection attempt.
    /// </summary>
    [Fact]
    public async Task ReadyIsRequiredAsync()
    {
        _connector.Reply = HandshakeReply.Error;

        await UpdateAsync(Content("A"));

        Assert.False(_client.IsConnected);
        Assert.Equal(1, _connector.Calls);
    }

    /// <summary>
    /// A handshake that is never answered times out, leaves the client disconnected and logs that the handshake failed.
    /// </summary>
    [Fact]
    public async Task MissingReadyTimesOutAsync()
    {
        _connector.Reply = HandshakeReply.Silent;

        await UpdateAsync(Content("A"));

        Assert.False(_client.IsConnected);
        Assert.Contains(_logger.Messages, static message => message.Contains("handshake failed", StringComparison.Ordinal));
    }

    /// <summary>
    /// A ping from Discord is answered with a pong that carries the same payload, and the client stays connected.
    /// </summary>
    [Fact]
    public async Task PingIsAnsweredWithPongCarryingTheSamePayloadAsync()
    {
        await UpdateAsync(Content("A"));
        _ = await _connector.Current.ReadFrameAsync(Token);

        await _connector.Current.SendAsync(DiscordOpcode.Ping, "{\"n\":42}");
        ReceivedFrame pong = await _connector.Current.ReadFrameAsync(Token);

        Assert.Equal(DiscordOpcode.Pong, pong.Opcode);
        Assert.Equal("{\"n\":42}", pong.Json);
        Assert.True(_client.IsConnected);
    }

    /// <summary>
    /// After Discord closes the connection, the client reconnects only once the 15-second interval has passed and then sends the presence again.
    /// </summary>
    [Fact]
    public async Task ServerCloseIsFollowedByAReconnectAfterTheIntervalAsync()
    {
        await UpdateAsync(Content("A"));
        await _connector.Current.SendAsync(DiscordOpcode.Close, "{\"code\":1000}");
        await WaitUntilAsync(() => !_client.IsConnected);

        Advance(TimeSpan.FromSeconds(14));
        await UpdateAsync(Content("A"));
        Assert.Equal(1, _connector.Calls);

        Advance(TimeSpan.FromSeconds(1));
        await UpdateAsync(Content("A"));

        Assert.Equal(2, _connector.Calls);
        Assert.True(_client.IsConnected);
        ReceivedFrame frame = await _connector.Current.ReadFrameAsync(Token);
        Assert.Contains("\"details\":\"A\"", frame.Json, StringComparison.Ordinal);
    }

    /// <summary>
    /// While the rate limit holds, intermediate changes are deferred and only the newest content is sent once the interval after the previous send has passed.
    /// </summary>
    [Fact]
    public async Task RateLimitDefersChangesAndKeepsTheNewestAsync()
    {
        await UpdateAsync(Content("A"));
        Assert.Equal("A", ParseDetails(await _connector.Current.ReadFrameAsync(Token)));

        Advance(TimeSpan.FromSeconds(1));
        await UpdateAsync(Content("B"));
        Advance(TimeSpan.FromSeconds(1));
        await UpdateAsync(Content("C"));
        Advance(TimeSpan.FromSeconds(12));
        await UpdateAsync(Content("C"));
        Assert.False(_connector.Current.HasPendingInput);

        Advance(TimeSpan.FromSeconds(1));
        await UpdateAsync(Content("C"));

        Assert.Equal("C", ParseDetails(await _connector.Current.ReadFrameAsync(Token)));
        Assert.False(_connector.Current.HasPendingInput);
    }

    /// <summary>
    /// The rate limit interval is measured from the previous send, so a change made exactly one interval later is sent at once and the next one has to wait a full interval.
    /// </summary>
    [Fact]
    public async Task RateLimitAppliesFromThePreviousSendAsync()
    {
        await UpdateAsync(Content("A"));
        _ = await _connector.Current.ReadFrameAsync(Token);

        Advance(Interval);
        await UpdateAsync(Content("B"));
        Assert.Equal("B", ParseDetails(await _connector.Current.ReadFrameAsync(Token)));

        Advance(TimeSpan.FromSeconds(14));
        await UpdateAsync(Content("C"));
        Assert.False(_connector.Current.HasPendingInput);

        Advance(TimeSpan.FromSeconds(1));
        await UpdateAsync(Content("C"));
        Assert.Equal("C", ParseDetails(await _connector.Current.ReadFrameAsync(Token)));
    }

    /// <summary>
    /// A presence that has not changed is not sent again, however much time passes, and the client stays connected.
    /// </summary>
    [Fact]
    public async Task UnchangedValueIsNotResentAsync()
    {
        await UpdateAsync(Content("A"));
        _ = await _connector.Current.ReadFrameAsync(Token);

        Advance(TimeSpan.FromMinutes(5));
        await UpdateAsync(Content("A"));
        Advance(TimeSpan.FromMinutes(5));
        await UpdateAsync(Content("A"));

        Assert.False(_connector.Current.HasPendingInput);
        Assert.True(_client.IsConnected);
    }

    /// <summary>
    /// With nothing to show, the client never calls the connector and stays disconnected.
    /// </summary>
    [Fact]
    public async Task NothingToShowNeverConnectsAsync()
    {
        await UpdateAsync(desired: null);
        Advance(TimeSpan.FromMinutes(1));
        await UpdateAsync(desired: null);

        Assert.Equal(0, _connector.Calls);
        Assert.False(_client.IsConnected);
    }

    /// <summary>
    /// When no Discord is available, the connection is retried only once per interval, and it succeeds once Discord becomes available.
    /// </summary>
    [Fact]
    public async Task ConnectorWithoutDiscordIsRetriedOnlyAfterTheIntervalAsync()
    {
        _connector.Available = false;

        await UpdateAsync(Content("A"));
        Advance(TimeSpan.FromSeconds(14));
        await UpdateAsync(Content("A"));
        Assert.Equal(1, _connector.Calls);

        Advance(TimeSpan.FromSeconds(1));
        await UpdateAsync(Content("A"));
        Assert.Equal(2, _connector.Calls);

        _connector.Available = true;
        Advance(Interval);
        await UpdateAsync(Content("A"));

        Assert.Equal(3, _connector.Calls);
        Assert.True(_client.IsConnected);
    }

    /// <summary>
    /// Clearing the presence sends no new activity but closes the connection at once, after the earlier activity.
    /// </summary>
    [Fact]
    public async Task ClearClosesTheConnectionAtOnceAsync()
    {
        await UpdateAsync(Content("A"));
        await UpdateAsync(desired: null);

        Assert.False(_client.IsConnected);
        Assert.Equal("A", ParseDetails(await _connector.Current.ReadFrameAsync(Token)));
        Assert.Equal(DiscordOpcode.Close, (await _connector.Current.ReadFrameAsync(Token)).Opcode);
    }

    /// <summary>
    /// An inbound frame that is oversized or has an unknown opcode makes the client drop the connection and not reconnect before the interval has passed; once it has passed, the next update reconnects and sends the presence again.
    /// </summary>
    [Theory]
    [InlineData(1_000_000u, 1u)]
    [InlineData(10u, 99u)]
    public async Task OversizedOrUnknownInboundFrameDropsTheConnectionAsync(uint length, uint opcode)
    {
        await UpdateAsync(Content("A"));
        byte[] header = new byte[8];
        BitConverter.TryWriteBytes(header.AsSpan(0, 4), opcode);
        BitConverter.TryWriteBytes(header.AsSpan(4, 4), length);

        await _connector.Current.SendRawAsync(header);
        await WaitUntilAsync(() => !_client.IsConnected);
        await UpdateAsync(Content("A"));

        Assert.False(_client.IsConnected);
        Assert.Equal(1, _connector.Calls);

        Advance(TimeSpan.FromSeconds(15));
        await UpdateAsync(Content("A"));

        Assert.Equal(2, _connector.Calls);
        Assert.True(_client.IsConnected);
        ReceivedFrame frame = await _connector.Current.ReadFrameAsync(Token);
        Assert.Contains("\"details\":\"A\"", frame.Json, StringComparison.Ordinal);
    }

    /// <summary>
    /// Inbound frames that contain invalid JSON or JSON that is not an object are ignored, and the client stays connected and still answers a ping.
    /// </summary>
    [Fact]
    public async Task GarbageJsonIsIgnoredAsync()
    {
        await UpdateAsync(Content("A"));

        await _connector.Current.SendAsync(DiscordOpcode.Frame, "this is not json {{{");
        await _connector.Current.SendAsync(DiscordOpcode.Frame, "[1,2,3]");
        await _connector.Current.SendAsync(DiscordOpcode.Ping, "x");
        ReceivedFrame frame = await _connector.Current.ReadFrameAsync(Token);
        ReceivedFrame pong = await _connector.Current.ReadFrameAsync(Token);

        Assert.Equal(DiscordOpcode.Frame, frame.Opcode);
        Assert.Equal(DiscordOpcode.Pong, pong.Opcode);
        Assert.True(_client.IsConnected);
    }

    /// <summary>
    /// An error response from Discord is logged with its code and message, and the client stays connected.
    /// </summary>
    [Fact]
    public async Task ErrorResponseIsLoggedWithCodeAndMessageAsync()
    {
        await UpdateAsync(Content("A"));

        await _connector.Current.SendAsync
        (
            DiscordOpcode.Frame,
            "{\"cmd\":\"SET_ACTIVITY\",\"evt\":\"ERROR\",\"data\":{\"code\":4000,\"message\":\"Bad activity\"}}"
        );
        await WaitUntilAsync(() => _logger.Messages.Exists(static message => message.Contains("4000", StringComparison.Ordinal)));

        Assert.Contains(_logger.Messages, static message => message.Contains("Bad activity", StringComparison.Ordinal));
        Assert.True(_client.IsConnected);
    }

    /// <summary>
    /// A write that stalls past the write timeout is abandoned, so the update returns and the client disconnects, and the client connects again after the next interval.
    /// </summary>
    [Fact]
    public async Task StalledWriteTimesOutDisconnectsAndRecoversAsync()
    {
        _connector.UseStallableStreams = true;
        await using DiscordPresenceClient client = new(_connector, _clock, _logger, TimeSpan.FromMilliseconds(300), TimeSpan.FromMilliseconds(200));

        await client.UpdateAsync(Content("A"), Token);
        Assert.True(client.IsConnected);
        _connector.Streams[0].StallWrites = true;
        Advance(Interval);

        Task update = client.UpdateAsync(Content("B"), Token);
        Task finished = await Task.WhenAny(update, Task.Delay(TimeSpan.FromSeconds(10), TimeProvider.System, Token));
        await update;

        Assert.Same(update, finished);
        Assert.False(client.IsConnected);

        Advance(Interval);
        await client.UpdateAsync(Content("B"), Token);

        Assert.True(client.IsConnected);
        Assert.Equal(2, _connector.Calls);
    }

    /// <summary>
    /// When the caller cancels the token during a stalled write, the cancellation is thrown to the caller instead of being treated as a write timeout.
    /// </summary>
    [Fact]
    public async Task CallerCancellationIsStillPropagatedFromAStalledWriteAsync()
    {
        _connector.UseStallableStreams = true;
        await using DiscordPresenceClient client = new(_connector, _clock, _logger, TimeSpan.FromMilliseconds(300), TimeSpan.FromSeconds(30));

        await client.UpdateAsync(Content("A"), Token);
        _connector.Streams[0].StallWrites = true;
        Advance(Interval);
        using CancellationTokenSource caller = CancellationTokenSource.CreateLinkedTokenSource(Token);
        Task update = client.UpdateAsync(Content("B"), caller.Token);
        await caller.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => update);
    }

    /// <summary>
    /// Moving the wall clock back by an hour does not block the next send, which is still sent after the interval has elapsed.
    /// </summary>
    [Fact]
    public async Task WallClockMovedBackDoesNotFreezeSendsAsync()
    {
        await UpdateAsync(Content("A"));
        _ = await _connector.Current.ReadFrameAsync(Token);

        _clock.JumpWallClock(_clock.Now - TimeSpan.FromHours(1));
        Advance(Interval);
        await UpdateAsync(Content("B"));

        Assert.Equal("B", ParseDetails(await _connector.Current.ReadFrameAsync(Token)));
    }

    /// <summary>
    /// Moving the wall clock back by an hour does not delay a reconnect, which still happens after the interval has elapsed.
    /// </summary>
    [Fact]
    public async Task WallClockMovedBackDoesNotFreezeReconnectsAsync()
    {
        await UpdateAsync(Content("A"));
        await _connector.Current.SendAsync(DiscordOpcode.Close, "{\"code\":1000}");
        await WaitUntilAsync(() => !_client.IsConnected);

        _clock.JumpWallClock(_clock.Now - TimeSpan.FromHours(1));
        Advance(TimeSpan.FromSeconds(14));
        await UpdateAsync(Content("A"));
        Assert.Equal(1, _connector.Calls);

        Advance(TimeSpan.FromSeconds(1));
        await UpdateAsync(Content("A"));

        Assert.Equal(2, _connector.Calls);
        Assert.True(_client.IsConnected);
    }

    private int ErrorCount()
    {
        lock (_logger.Messages)
        {
            return _logger.Messages.FindAll(static message => message.Contains("Rejected", StringComparison.Ordinal)).Count;
        }
    }

    private bool HasMessage(string text)
    {
        lock (_logger.Messages)
        {
            return _logger.Messages.Exists(message => message.Contains(text, StringComparison.Ordinal));
        }
    }

    private int DeferredCount()
    {
        lock (_logger.Messages)
        {
            return _logger.Messages.FindAll(static message => message.Contains("deferred", StringComparison.Ordinal)).Count;
        }
    }

    /// <summary>
    /// A presence that Discord rejected is sent again after the interval even though its content is unchanged, and the retry is logged.
    /// </summary>
    [Fact]
    public async Task RejectedStablePresenceIsSentAgainAfterTheIntervalAsync()
    {
        _connector.RejectActivities = true;

        await UpdateAsync(Content("A"));
        await WaitUntilAsync(() => ErrorCount() is 1);
        await UpdateAsync(Content("A"));
        Assert.Equal(1, _connector.ActivityCount);

        Advance(Interval);
        await UpdateAsync(Content("A"));
        await WaitUntilAsync(() => _connector.ActivityCount is 2);

        Assert.True(HasMessage("trying again in 15 s"));
    }

    /// <summary>
    /// Repeated rejections of an unchanged presence make the client wait longer between attempts, so that over two minutes it sends only between 2 and 5 times while staying connected.
    /// </summary>
    [Fact]
    public async Task RepeatedRejectionsBackOffAsync()
    {
        _connector.RejectActivities = true;
        int sends = 0;

        for (int second = 0; second <= 120; second += 15)
        {
            int deferred = DeferredCount();
            await UpdateAsync(Content("A"));

            if (DeferredCount() == deferred)
            {
                sends++;
                await WaitUntilAsync(() => ErrorCount() >= sends);
            }

            Advance(Interval);
        }

        Assert.InRange(sends, 2, 5);
        await WaitUntilAsync(() => _connector.ActivityCount == sends);
        Assert.True(_client.IsConnected);
    }

    /// <summary>
    /// A rejected presence whose content changes is sent after the normal interval, without the back-off applied to unchanged content.
    /// </summary>
    [Fact]
    public async Task RejectedPresenceChangedContentIsSentAtTheNormalIntervalAsync()
    {
        _connector.RejectActivities = true;

        await UpdateAsync(Content("A"));
        await WaitUntilAsync(() => ErrorCount() is 1);
        Advance(Interval);
        await UpdateAsync(Content("A"));
        await WaitUntilAsync(() => ErrorCount() is 2);
        Advance(Interval);
        await UpdateAsync(Content("B"));
        await WaitUntilAsync(() => _connector.ActivityCount is 3);

        Assert.Equal(3, _connector.ActivityCount);
    }

    /// <summary>
    /// A presence that Discord accepted is not sent again over eight further intervals.
    /// </summary>
    [Fact]
    public async Task AcceptedPresenceIsNotSentAgainAsync()
    {
        await UpdateAsync(Content("A"));
        _ = await _connector.Current.ReadFrameAsync(Token);

        for (int i = 0; i < 8; i++)
        {
            Advance(Interval);
            await UpdateAsync(Content("A"));
        }

        Assert.False(_connector.Current.HasPendingInput);
    }

    /// <summary>
    /// A failed write does not throw to the caller, the client reports itself as disconnected, and it makes no new connection attempt before the interval has passed.
    /// </summary>
    [Fact]
    public async Task WriteFailureDoesNotThrowAndDisconnectsAsync()
    {
        await UpdateAsync(Content("A"));
        _connector.Current.CloseInput();
        Advance(Interval);

        await UpdateAsync(Content("B"));

        Assert.False(_client.IsConnected);
        Assert.Equal(1, _connector.Calls);
    }

    /// <summary>
    /// Disposing a connected client sends a close frame and leaves it disconnected.
    /// </summary>
    [Fact]
    public async Task DisposeSendsACloseFrameAsync()
    {
        await UpdateAsync(Content("A"));
        _ = await _connector.Current.ReadFrameAsync(Token);

        await _client.DisposeAsync();

        ReceivedFrame frame = await _connector.Current.ReadFrameAsync(Token);
        Assert.Equal(DiscordOpcode.Close, frame.Opcode);
        Assert.False(_client.IsConnected);
    }

    /// <summary>
    /// Updating with an already cancelled token throws an <see cref="System.OperationCanceledException"/>.
    /// </summary>
    [Fact]
    public async Task CancellationIsTheOnlyThingThrownAsync()
    {
        using CancellationTokenSource cancelled = new();
        await cancelled.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => _client.UpdateAsync(Content("A"), cancelled.Token));
    }

    /// <summary>
    /// With the real Discord desktop app, and only when TF3DRPC_DISCORD_LIVE is 1, a presence with a button is shown without errors and then cleared, which closes the connection.
    /// </summary>
    [Fact]
    public async Task LiveDiscordShowsAndClearsAPresenceAsync()
    {
        Assert.SkipUnless
        (
            string.Equals(Environment.GetEnvironmentVariable("TF3DRPC_DISCORD_LIVE"), "1", StringComparison.Ordinal),
            "Set TF3DRPC_DISCORD_LIVE=1 to talk to the real Discord client."
        );

        await using DiscordPresenceClient live = new(new DiscordConnector(), TimeProvider.System, _logger);

        PresenceContentStruct content = new() { Details = "Test", State = "Test", StartTimestamp = DateTimeOffset.UtcNow, ButtonLabel = "Get Transport Fever 3" };

        await live.UpdateAsync(content, TestContext.Current.CancellationToken);
        Assert.True(live.IsConnected);
        await Task.Delay(TimeSpan.FromSeconds(5), TimeProvider.System, TestContext.Current.CancellationToken);

        // The client notices an ERROR reply of Discord (such as a refused button) only on its next call.
        await live.UpdateAsync(content, TestContext.Current.CancellationToken);
        Assert.DoesNotContain(_logger.Messages, static message => message.Contains("reported an error", StringComparison.Ordinal) || message.Contains("rejected the presence", StringComparison.Ordinal));

        await live.UpdateAsync(desired: null, TestContext.Current.CancellationToken);
        Assert.False(live.IsConnected);
    }

    private static string ParseDetails(ReceivedFrame frame)
    {
        using JsonDocument document = JsonDocument.Parse(frame.Payload);

        return document.RootElement.GetProperty("args").GetProperty("activity").GetProperty("details").GetString()!;
    }
}
