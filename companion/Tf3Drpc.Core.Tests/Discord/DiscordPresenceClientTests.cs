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

public sealed class DiscordPresenceClientTests : IAsyncDisposable
{
    private static readonly DateTimeOffset Start = new(2026, 10, 13, 21, 15, 24, TimeSpan.Zero);
    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(15);
    private readonly CancellationTokenSource _cancellation;

    private readonly SettableTimeProvider _clock = new() { Now = new DateTimeOffset(2026, 10, 14, 12, 0, 0, TimeSpan.Zero) };

    private readonly FakeConnector _connector;
    private readonly ListLogger _logger = new();
    private readonly DiscordPresenceClient _client;

    public DiscordPresenceClientTests()
    {
        _cancellation = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        _cancellation.CancelAfter(TimeSpan.FromSeconds(20));
        _connector = new FakeConnector(_cancellation.Token);
        _client = new DiscordPresenceClient(_connector, _clock, _logger, TimeSpan.FromMilliseconds(300));
    }

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

    [Fact]
    public async Task Connect_SendsTheExactHandshakeBytesAsync()
    {
        await UpdateAsync(Content("A"));

        byte[] expectedJson = Encoding.UTF8.GetBytes("{\"v\":1,\"client_id\":\"1559680023458484244\"}");
        byte[] expected = DiscordFrameUtils.Encode(DiscordOpcode.Handshake, expectedJson);
        Assert.True(_client.IsConnected);
        Assert.Equal(expected, _connector.Current.HandshakeBytes);
        Assert.Equal(new byte[] { 0, 0, 0, 0, (byte)expectedJson.Length, 0, 0, 0 }, _connector.Current.HandshakeBytes![..8]);
    }

    [Fact]
    public async Task SetActivity_CarriesAllFieldsAsync()
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

    [Fact]
    public async Task SetActivity_OmitsMissingStateAndTimerAsync()
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

    [Theory]
    [InlineData("Archipelago2 in 1905", "Archipelago2 in 1905")]
    [InlineData(null, "Transport Fever 3")]
    public void SetActivity_PutsTheHoverTextIntoLargeText(string? hover, string expected)
    {
        PresenceContentStruct content = new() { Details = "Mechanic", LargeImageText = hover };

        byte[] payload = DiscordMessageUtils.BuildSetActivity(content, "nonce");
        using JsonDocument document = JsonDocument.Parse(payload);
        JsonElement assets = document.RootElement.GetProperty("args").GetProperty("activity").GetProperty("assets");

        Assert.Equal(expected, assets.GetProperty("large_text").GetString());
        Assert.Equal("cover", assets.GetProperty("large_image").GetString());
    }

    [Fact]
    public void ClearPayload_OmitsTheActivity()
    {
        byte[] payload = DiscordMessageUtils.BuildSetActivity(null, "nonce");
        using JsonDocument document = JsonDocument.Parse(payload);
        JsonElement arguments = document.RootElement.GetProperty("args");

        Assert.Equal("SET_ACTIVITY", document.RootElement.GetProperty("cmd").GetString());
        Assert.Equal(Environment.ProcessId, arguments.GetProperty("pid").GetInt32());
        Assert.False(arguments.TryGetProperty("activity", out JsonElement _));
    }

    [Fact]
    public async Task ReadyIsRequiredAsync()
    {
        _connector.Reply = HandshakeReply.Error;

        await UpdateAsync(Content("A"));

        Assert.False(_client.IsConnected);
        Assert.Equal(1, _connector.Calls);
    }

    [Fact]
    public async Task MissingReady_TimesOutAsync()
    {
        _connector.Reply = HandshakeReply.Silent;

        await UpdateAsync(Content("A"));

        Assert.False(_client.IsConnected);
        Assert.Contains(_logger.Messages, static message => message.Contains("handshake failed", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Ping_IsAnsweredWithPongCarryingTheSamePayloadAsync()
    {
        await UpdateAsync(Content("A"));
        _ = await _connector.Current.ReadFrameAsync(Token);

        await _connector.Current.SendAsync(DiscordOpcode.Ping, "{\"n\":42}");
        ReceivedFrame pong = await _connector.Current.ReadFrameAsync(Token);

        Assert.Equal(DiscordOpcode.Pong, pong.Opcode);
        Assert.Equal("{\"n\":42}", pong.Json);
        Assert.True(_client.IsConnected);
    }

    [Fact]
    public async Task ServerClose_IsFollowedByAReconnectAfterTheIntervalAsync()
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

    [Fact]
    public async Task RateLimit_DefersChangesAndKeepsTheNewestAsync()
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

    [Fact]
    public async Task RateLimit_AppliesFromThePreviousSendAsync()
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

    [Fact]
    public async Task UnchangedValue_IsNotResentAsync()
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

    [Fact]
    public async Task NothingToShow_NeverConnectsAsync()
    {
        await UpdateAsync(null);
        Advance(TimeSpan.FromMinutes(1));
        await UpdateAsync(null);

        Assert.Equal(0, _connector.Calls);
        Assert.False(_client.IsConnected);
    }

    [Fact]
    public async Task ConnectorWithoutDiscord_IsRetriedOnlyAfterTheIntervalAsync()
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

    [Fact]
    public async Task Clear_ClosesTheConnectionAtOnceAsync()
    {
        await UpdateAsync(Content("A"));
        await UpdateAsync(null);

        Assert.False(_client.IsConnected);
        Assert.Equal("A", ParseDetails(await _connector.Current.ReadFrameAsync(Token)));
        Assert.Equal(DiscordOpcode.Close, (await _connector.Current.ReadFrameAsync(Token)).Opcode);
    }

    [Theory]
    [InlineData(1_000_000u, 1u)]
    [InlineData(10u, 99u)]
    public async Task OversizedOrUnknownInboundFrame_DropsTheConnectionAsync(uint length, uint opcode)
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
    }

    [Fact]
    public async Task GarbageJson_IsIgnoredAsync()
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

    [Fact]
    public async Task ErrorResponse_IsLoggedWithCodeAndMessageAsync()
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

    [Fact]
    public async Task StalledWrite_TimesOutDisconnectsAndRecoversAsync()
    {
        _connector.UseStallableStreams = true;
        await using DiscordPresenceClient client = new(_connector, _clock, _logger, TimeSpan.FromMilliseconds(300), TimeSpan.FromMilliseconds(200));

        await client.UpdateAsync(Content("A"), Token);
        Assert.True(client.IsConnected);
        _connector.Streams[0].StallWrites = true;
        Advance(Interval);

        Task update = client.UpdateAsync(Content("B"), Token);
        Task finished = await Task.WhenAny(update, Task.Delay(TimeSpan.FromSeconds(10), Token));
        await update;

        Assert.Same(update, finished);
        Assert.False(client.IsConnected);

        Advance(Interval);
        await client.UpdateAsync(Content("B"), Token);

        Assert.True(client.IsConnected);
        Assert.Equal(2, _connector.Calls);
    }

    [Fact]
    public async Task CallerCancellation_IsStillPropagatedFromAStalledWriteAsync()
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

    [Fact]
    public async Task WallClockMovedBack_DoesNotFreezeSendsAsync()
    {
        await UpdateAsync(Content("A"));
        _ = await _connector.Current.ReadFrameAsync(Token);

        _clock.JumpWallClock(_clock.Now - TimeSpan.FromHours(1));
        Advance(Interval);
        await UpdateAsync(Content("B"));

        Assert.Equal("B", ParseDetails(await _connector.Current.ReadFrameAsync(Token)));
    }

    [Fact]
    public async Task WallClockMovedBack_DoesNotFreezeReconnectsAsync()
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

    [Fact]
    public async Task RejectedStablePresence_IsSentAgainAfterTheIntervalAsync()
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

    [Fact]
    public async Task RepeatedRejections_BackOffAsync()
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

    [Fact]
    public async Task RejectedPresence_ChangedContentIsSentAtTheNormalIntervalAsync()
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

    [Fact]
    public async Task AcceptedPresence_IsNotSentAgainAsync()
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

    [Fact]
    public async Task WriteFailure_DoesNotThrowAndDisconnectsAsync()
    {
        await UpdateAsync(Content("A"));
        _connector.Current.CloseInput();
        Advance(Interval);

        await UpdateAsync(Content("B"));

        Assert.False(_client.IsConnected);
        Assert.Equal(1, _connector.Calls);
    }

    [Fact]
    public async Task Dispose_SendsACloseFrameAsync()
    {
        await UpdateAsync(Content("A"));
        _ = await _connector.Current.ReadFrameAsync(Token);

        await _client.DisposeAsync();

        ReceivedFrame frame = await _connector.Current.ReadFrameAsync(Token);
        Assert.Equal(DiscordOpcode.Close, frame.Opcode);
        Assert.False(_client.IsConnected);
    }

    [Fact]
    public async Task Cancellation_IsTheOnlyThingThrownAsync()
    {
        using CancellationTokenSource cancelled = new();
        await cancelled.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => _client.UpdateAsync(Content("A"), cancelled.Token));
    }

    [Fact]
    public async Task LiveDiscord_ShowsAndClearsAPresenceAsync()
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
        await Task.Delay(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);

        // The client notices an ERROR reply of Discord (such as a refused button) only on its next call.
        await live.UpdateAsync(content, TestContext.Current.CancellationToken);
        Assert.DoesNotContain(_logger.Messages, static message => message.Contains("reported an error", StringComparison.Ordinal) || message.Contains("rejected the presence", StringComparison.Ordinal));

        await live.UpdateAsync(null, TestContext.Current.CancellationToken);
        Assert.False(live.IsConnected);
    }

    private static string ParseDetails(ReceivedFrame frame)
    {
        using JsonDocument document = JsonDocument.Parse(frame.Payload);

        return document.RootElement.GetProperty("args").GetProperty("activity").GetProperty("details").GetString()!;
    }
}
