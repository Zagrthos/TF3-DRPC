using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Tf3Drpc.Core.Structs;

/// <summary>
/// The texts, timer and store button of a Discord presence.
/// </summary>
[StructLayout(LayoutKind.Auto)]
public readonly struct PresenceContentStruct : IEquatable<PresenceContentStruct>
{
    public string Details { get; init; }

    public string? State { get; init; }

    public DateTimeOffset? StartTimestamp { get; init; }

    /// <summary>
    /// The hover text of the image; <see langword="null"/> means the default.
    /// </summary>
    public string? LargeImageText { get; init; }

    /// <summary>
    /// The label of the store button; <see langword="null"/> means no button.
    /// </summary>
    public string? ButtonLabel { get; init; }

    public bool Equals(PresenceContentStruct other)
        => string.Equals(Details, other.Details, StringComparison.Ordinal) &&
            string.Equals(State, other.State, StringComparison.Ordinal) &&
            (StartTimestamp == other.StartTimestamp) &&
            string.Equals(LargeImageText, other.LargeImageText, StringComparison.Ordinal) &&
            string.Equals(ButtonLabel, other.ButtonLabel, StringComparison.Ordinal);

    public override bool Equals([NotNullWhen(true)] object? obj)
        => (obj is PresenceContentStruct other) && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(Details, State, StartTimestamp, LargeImageText, ButtonLabel);

    public static bool operator ==(in PresenceContentStruct left, in PresenceContentStruct right)
        => left.Equals(right);

    public static bool operator !=(in PresenceContentStruct left, in PresenceContentStruct right)
        => !left.Equals(right);
}
