using System;
using System.Buffers.Binary;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Tf3Drpc.Core.Constants;
using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Structs;

namespace Tf3Drpc.Core.Utilities;

/// <summary>
/// Encodes and decodes the frames of the Discord IPC protocol.
/// </summary>
internal static class DiscordFrameUtils
{
    public static byte[] Encode(DiscordOpcode opcode, ReadOnlySpan<byte> payload)
    {
        byte[] buffer = new byte[DiscordIpcConstants.HeaderBytes + payload.Length];
        BinaryPrimitives.WriteUInt32LittleEndian(buffer, (uint)opcode);
        BinaryPrimitives.WriteUInt32LittleEndian(buffer.AsSpan(4), (uint)payload.Length);
        payload.CopyTo(buffer.AsSpan(DiscordIpcConstants.HeaderBytes));

        return buffer;
    }

    /// <summary>
    /// Reads one frame; end of stream, an unknown opcode or an oversized length give <see langword="null"/>.
    /// </summary>
    public static async ValueTask<DiscordFrameStruct?> ReadAsync(Stream stream, CancellationToken cancellationToken)
    {
        byte[] header = new byte[DiscordIpcConstants.HeaderBytes];

        try
        {
            await stream.ReadExactlyAsync(header, cancellationToken);

            uint opcode = BinaryPrimitives.ReadUInt32LittleEndian(header);
            uint length = BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(4));

            if ((opcode is > (uint)DiscordOpcode.Pong) || (length is > DiscordIpcConstants.MaxFrameBytes))
                return null;

            byte[] payload = new byte[length];
            await stream.ReadExactlyAsync(payload, cancellationToken);

            return new DiscordFrameStruct() { Opcode = (DiscordOpcode)opcode, Payload = payload };
        }
        catch (EndOfStreamException)
        {
            return null;
        }
    }
}
