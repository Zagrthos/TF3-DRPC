using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using Tf3Drpc.Core.Enums;

namespace Tf3Drpc.Core.Structs;

/// <summary>
/// The parsed first line of a game log entry.
/// </summary>
[StructLayout(LayoutKind.Auto)]
internal readonly struct GameLogEntryStruct : IEquatable<GameLogEntryStruct>
{
    /// <summary>
    /// The time of the log line.
    /// </summary>
    public DateTimeOffset Timestamp { get; init; }

    internal GameLogLevel Level { get; init; }

    /// <summary>
    /// The text of the log line after its header.
    /// </summary>
    public string Message { get; init; }

    /// <summary>
    /// Whether the message continues on the following lines.
    /// </summary>
    public bool StartsMultiLine { get; init; }

    /// <inheritdoc/>
    public bool Equals(GameLogEntryStruct other)
        => (Timestamp == other.Timestamp) &&
            (Level == other.Level) &&
            string.Equals(Message, other.Message, StringComparison.Ordinal) &&
            (StartsMultiLine == other.StartsMultiLine);

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? obj)
        => (obj is GameLogEntryStruct other) && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
        => HashCode.Combine(Timestamp, Level, Message, StartsMultiLine);

    /// <summary>
    /// Tells whether two values are equal.
    /// </summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    public static bool operator ==(in GameLogEntryStruct left, in GameLogEntryStruct right)
        => left.Equals(right);

    /// <summary>
    /// Tells whether two values differ.
    /// </summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    public static bool operator !=(in GameLogEntryStruct left, in GameLogEntryStruct right)
        => !left.Equals(right);
}
