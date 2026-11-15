using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Tf3Drpc.Core.Structs;

/// <summary>
/// The new complete lines of one poll of the game log and whether the log started over.
/// </summary>
[StructLayout(LayoutKind.Auto)]
internal readonly struct GameLogReadResultStruct : IEquatable<GameLogReadResultStruct>
{
    /// <summary>
    /// The lines added since the last read.
    /// </summary>
    public IReadOnlyList<string> Lines { get; init; }

    /// <summary>
    /// Whether the log started over.
    /// </summary>
    public bool Restarted { get; init; }

    /// <inheritdoc/>
    public bool Equals(GameLogReadResultStruct other)
        => ReferenceEquals(Lines, other.Lines) &&
            (Restarted == other.Restarted);

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? obj)
        => (obj is GameLogReadResultStruct other) && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
        => HashCode.Combine(Lines, Restarted);

    /// <summary>
    /// Tells whether two values are equal.
    /// </summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    public static bool operator ==(in GameLogReadResultStruct left, in GameLogReadResultStruct right)
        => left.Equals(right);

    /// <summary>
    /// Tells whether two values differ.
    /// </summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    public static bool operator !=(in GameLogReadResultStruct left, in GameLogReadResultStruct right)
        => !left.Equals(right);
}
