using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using Tf3Drpc.Core.Enums;

namespace Tf3Drpc.Core.Structs;

/// <summary>
/// The parsed first line of a game log entry.
/// </summary>
[StructLayout(LayoutKind.Auto)]
public readonly struct GameLogEntryStruct : IEquatable<GameLogEntryStruct>
{
    public DateTimeOffset Timestamp { get; init; }

    internal GameLogLevel Level { get; init; }

    public string Message { get; init; }

    public bool StartsMultiLine { get; init; }

    public bool Equals(GameLogEntryStruct other)
        => (Timestamp == other.Timestamp) &&
            (Level == other.Level) &&
            string.Equals(Message, other.Message, StringComparison.Ordinal) &&
            (StartsMultiLine == other.StartsMultiLine);

    public override bool Equals([NotNullWhen(true)] object? obj)
        => (obj is GameLogEntryStruct other) && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(Timestamp, Level, Message, StartsMultiLine);

    public static bool operator ==(in GameLogEntryStruct left, in GameLogEntryStruct right)
        => left.Equals(right);

    public static bool operator !=(in GameLogEntryStruct left, in GameLogEntryStruct right)
        => !left.Equals(right);
}
