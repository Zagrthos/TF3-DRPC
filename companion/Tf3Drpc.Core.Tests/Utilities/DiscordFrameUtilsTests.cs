using System.IO;
using System.Threading.Tasks;

using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Structs;
using Tf3Drpc.Core.Utilities;

using Xunit;

namespace Tf3Drpc.Core.Tests.Utilities;

public sealed class DiscordFrameUtilsTests
{
    [Fact]
    public void Encode_WritesLittleEndianHeader()
    {
        byte[] frame = DiscordFrameUtils.Encode(DiscordOpcode.Pong, "{}"u8);

        Assert.Equal(new byte[] { 4, 0, 0, 0, 2, 0, 0, 0, (byte)'{', (byte)'}' }, frame);
    }

    [Fact]
    public async Task RoundTrip_ReturnsTheSameFrameAsync()
    {
        byte[] bytes = DiscordFrameUtils.Encode(DiscordOpcode.Frame, "{\"a\":1}"u8);
        await using MemoryStream stream = new(bytes);

        DiscordFrameStruct? frame = await DiscordFrameUtils.ReadAsync(stream, TestContext.Current.CancellationToken);

        Assert.Equal(new DiscordFrameStruct() { Opcode = DiscordOpcode.Frame, Payload = "{\"a\":1}"u8.ToArray() }, frame);
    }

    [Fact]
    public async Task ZeroLengthPayload_IsAcceptedAsync()
    {
        await using MemoryStream stream = new(DiscordFrameUtils.Encode(DiscordOpcode.Ping, []));

        DiscordFrameStruct? frame = await DiscordFrameUtils.ReadAsync(stream, TestContext.Current.CancellationToken);

        Assert.NotNull(frame);
        Assert.Equal(0, frame.Value.Payload.Length);
    }

    [Fact]
    public async Task OversizedLength_IsRejectedWithoutAllocationAsync()
    {
        byte[] header = [1, 0, 0, 0, 0xFF, 0xFF, 0xFF, 0xFF];
        await using MemoryStream stream = new(header);

        DiscordFrameStruct? frame = await DiscordFrameUtils.ReadAsync(stream, TestContext.Current.CancellationToken);

        Assert.Null(frame);
    }

    [Fact]
    public async Task LengthAtTheLimit_IsAcceptedAndOneOverIsNotAsync()
    {
        byte[] atLimit = DiscordFrameUtils.Encode(DiscordOpcode.Frame, new byte[64 * 1024]);
        byte[] overLimit = DiscordFrameUtils.Encode(DiscordOpcode.Frame, new byte[(64 * 1024) + 1]);

        await using MemoryStream first = new(atLimit);
        await using MemoryStream second = new(overLimit);

        Assert.NotNull(await DiscordFrameUtils.ReadAsync(first, TestContext.Current.CancellationToken));
        Assert.Null(await DiscordFrameUtils.ReadAsync(second, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task UnknownOpcode_IsRejectedAsync()
    {
        await using MemoryStream stream = new([5, 0, 0, 0, 0, 0, 0, 0]);

        Assert.Null(await DiscordFrameUtils.ReadAsync(stream, TestContext.Current.CancellationToken));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(10)]
    public async Task TruncatedFrame_GivesNullAsync(int length)
    {
        byte[] bytes = DiscordFrameUtils.Encode(DiscordOpcode.Frame, "{\"abc\":1}"u8);
        await using MemoryStream stream = new(bytes[..length]);

        Assert.Null(await DiscordFrameUtils.ReadAsync(stream, TestContext.Current.CancellationToken));
    }
}
