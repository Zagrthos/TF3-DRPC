using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using Tf3Drpc.Core.Enums;

namespace Tf3Drpc.Core.Structs;

/// <summary>
/// One frame of the Discord IPC protocol.
/// </summary>
[StructLayout(LayoutKind.Auto)]
internal readonly struct DiscordFrameStruct : IEquatable<DiscordFrameStruct>
{
    public DiscordOpcode Opcode { get; init; }

    public ReadOnlyMemory<byte> Payload { get; init; }

    public bool Equals(DiscordFrameStruct other)
        => (Opcode == other.Opcode) &&
            Payload.Span.SequenceEqual(other.Payload.Span);

    public override bool Equals([NotNullWhen(true)] object? obj)
        => (obj is DiscordFrameStruct other) && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(Opcode, Payload.Length);

    public static bool operator ==(in DiscordFrameStruct left, in DiscordFrameStruct right)
        => left.Equals(right);

    public static bool operator !=(in DiscordFrameStruct left, in DiscordFrameStruct right)
        => !left.Equals(right);
}
