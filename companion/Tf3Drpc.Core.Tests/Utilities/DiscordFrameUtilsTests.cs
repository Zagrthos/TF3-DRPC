using System.IO;
using System.Threading.Tasks;

using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Structs;
using Tf3Drpc.Core.Utilities;

using Xunit;

namespace Tf3Drpc.Core.Tests.Utilities;

/// <summary>
/// Tests of the Discord IPC frame encoding and reading in <see cref="Tf3Drpc.Core.Utilities.DiscordFrameUtils"/>, using in-memory streams.
/// </summary>
public sealed class DiscordFrameUtilsTests
{
    /// <summary>
    /// Encoding a frame writes the opcode and then the payload length as little-endian 32-bit integers, followed by the payload bytes.
    /// </summary>
    [Fact]
    public void EncodeWritesLittleEndianHeader()
    {
        byte[] frame = DiscordFrameUtils.Encode(DiscordOpcode.Pong, "{}"u8);

        Assert.Equal(new byte[] { 4, 0, 0, 0, 2, 0, 0, 0, (byte)'{', (byte)'}' }, frame);
    }

    /// <summary>
    /// A frame that is encoded and then read from a stream comes back with the same opcode and payload.
    /// </summary>
    [Fact]
    public async Task RoundTripReturnsTheSameFrameAsync()
    {
        byte[] bytes = DiscordFrameUtils.Encode(DiscordOpcode.Frame, "{\"a\":1}"u8);
        await using MemoryStream stream = new(bytes);

        DiscordFrameStruct? frame = await DiscordFrameUtils.ReadAsync(stream, TestContext.Current.CancellationToken);

        Assert.Equal(new DiscordFrameStruct() { Opcode = DiscordOpcode.Frame, Payload = "{\"a\":1}"u8.ToArray() }, frame);
    }

    /// <summary>
    /// A frame with an empty payload is read successfully and yields a zero-length payload.
    /// </summary>
    [Fact]
    public async Task ZeroLengthPayloadIsAcceptedAsync()
    {
        await using MemoryStream stream = new(DiscordFrameUtils.Encode(DiscordOpcode.Ping, []));

        DiscordFrameStruct? frame = await DiscordFrameUtils.ReadAsync(stream, TestContext.Current.CancellationToken);

        Assert.NotNull(frame);
        Assert.Equal(0, frame.Value.Payload.Length);
    }

    /// <summary>
    /// A header that announces a payload of about 4 GiB is rejected by returning <see langword="null"/> without reading a payload.
    /// </summary>
    [Fact]
    public async Task OversizedLengthIsRejectedWithoutAllocationAsync()
    {
        byte[] header = [1, 0, 0, 0, 0xFF, 0xFF, 0xFF, 0xFF];
        await using MemoryStream stream = new(header);

        DiscordFrameStruct? frame = await DiscordFrameUtils.ReadAsync(stream, TestContext.Current.CancellationToken);

        Assert.Null(frame);
    }

    /// <summary>
    /// A payload of exactly 64 KiB is read, while a payload one byte larger is rejected by returning <see langword="null"/>.
    /// </summary>
    [Fact]
    public async Task LengthAtTheLimitIsAcceptedAndOneOverIsNotAsync()
    {
        byte[] atLimit = DiscordFrameUtils.Encode(DiscordOpcode.Frame, new byte[64 * 1024]);
        byte[] overLimit = DiscordFrameUtils.Encode(DiscordOpcode.Frame, new byte[(64 * 1024) + 1]);

        await using MemoryStream first = new(atLimit);
        await using MemoryStream second = new(overLimit);

        Assert.NotNull(await DiscordFrameUtils.ReadAsync(first, TestContext.Current.CancellationToken));
        Assert.Null(await DiscordFrameUtils.ReadAsync(second, TestContext.Current.CancellationToken));
    }

    /// <summary>
    /// A frame with an opcode outside the known range (5) is rejected by returning <see langword="null"/>.
    /// </summary>
    [Fact]
    public async Task UnknownOpcodeIsRejectedAsync()
    {
        await using MemoryStream stream = new([5, 0, 0, 0, 0, 0, 0, 0]);

        Assert.Null(await DiscordFrameUtils.ReadAsync(stream, TestContext.Current.CancellationToken));
    }

    /// <summary>
    /// A stream that ends before the frame is complete, whether empty, within the header or within the payload (0, 5 or 10 bytes of a 17-byte frame), yields <see langword="null"/>.
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(10)]
    public async Task TruncatedFrameGivesNullAsync(int length)
    {
        byte[] bytes = DiscordFrameUtils.Encode(DiscordOpcode.Frame, "{\"abc\":1}"u8);
        await using MemoryStream stream = new(bytes[..length]);

        Assert.Null(await DiscordFrameUtils.ReadAsync(stream, TestContext.Current.CancellationToken));
    }
}
