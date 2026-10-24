using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Tf3Drpc.Core.Structs;

/// <summary>
/// The relevant events of one poll of the game log and whether the log started over.
/// </summary>
[StructLayout(LayoutKind.Auto)]
public readonly struct GameLogPollResultStruct : IEquatable<GameLogPollResultStruct>
{
    public IReadOnlyList<TimedGameLogEventStruct> Events { get; init; }

    public bool Restarted { get; init; }

    public bool Equals(GameLogPollResultStruct other)
        => ReferenceEquals(Events, other.Events) &&
            (Restarted == other.Restarted);

    public override bool Equals([NotNullWhen(true)] object? obj)
        => (obj is GameLogPollResultStruct other) && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(Events, Restarted);

    public static bool operator ==(in GameLogPollResultStruct left, in GameLogPollResultStruct right)
        => left.Equals(right);

    public static bool operator !=(in GameLogPollResultStruct left, in GameLogPollResultStruct right)
        => !left.Equals(right);
}
