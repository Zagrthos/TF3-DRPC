using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Tf3Drpc.Core.Structs;

/// <summary>
/// The new complete lines of one poll of the game log and whether the log started over.
/// </summary>
[StructLayout(LayoutKind.Auto)]
public readonly struct GameLogReadResultStruct : IEquatable<GameLogReadResultStruct>
{
    public IReadOnlyList<string> Lines { get; init; }

    public bool Restarted { get; init; }

    public bool Equals(GameLogReadResultStruct other)
        => ReferenceEquals(Lines, other.Lines) &&
            (Restarted == other.Restarted);

    public override bool Equals([NotNullWhen(true)] object? obj)
        => (obj is GameLogReadResultStruct other) && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(Lines, Restarted);

    public static bool operator ==(in GameLogReadResultStruct left, in GameLogReadResultStruct right)
        => left.Equals(right);

    public static bool operator !=(in GameLogReadResultStruct left, in GameLogReadResultStruct right)
        => !left.Equals(right);
}
