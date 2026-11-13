using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using Tf3Drpc.Core.GameLog;

namespace Tf3Drpc.Core.Structs;

/// <summary>
/// A classified game log event with the time of its log line.
/// </summary>
[StructLayout(LayoutKind.Auto)]
internal readonly struct TimedGameLogEventStruct : IEquatable<TimedGameLogEventStruct>
{
    public DateTimeOffset Timestamp { get; init; }

    public required GameLogEvent GameEvent { get; init; }

    public bool Equals(TimedGameLogEventStruct other)
        => (Timestamp == other.Timestamp) &&
            EqualityComparer<GameLogEvent>.Default.Equals(GameEvent, other.GameEvent);

    public override bool Equals([NotNullWhen(true)] object? obj)
        => (obj is TimedGameLogEventStruct other) && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(Timestamp, GameEvent);

    public static bool operator ==(in TimedGameLogEventStruct left, in TimedGameLogEventStruct right)
        => left.Equals(right);

    public static bool operator !=(in TimedGameLogEventStruct left, in TimedGameLogEventStruct right)
        => !left.Equals(right);
}
