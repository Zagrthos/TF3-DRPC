using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Tf3Drpc.Core.Structs;

/// <summary>
/// The texts, timer and store button of a Discord presence.
/// </summary>
[StructLayout(LayoutKind.Auto)]
internal readonly struct PresenceContentStruct : IEquatable<PresenceContentStruct>
{
    /// <summary>
    /// The first line of the presence.
    /// </summary>
    public string Details { get; init; }

    /// <summary>
    /// The second line of the presence, or null for none.
    /// </summary>
    public string? State { get; init; }

    /// <summary>
    /// The time the elapsed timer counts from, or null for none.
    /// </summary>
    public DateTimeOffset? StartTimestamp { get; init; }

    /// <summary>
    /// The hover text of the image; <see langword="null"/> means the default.
    /// </summary>
    public string? LargeImageText { get; init; }

    /// <summary>
    /// The label of the store button; <see langword="null"/> means no button.
    /// </summary>
    public string? ButtonLabel { get; init; }

    /// <inheritdoc/>
    public bool Equals(PresenceContentStruct other)
        => string.Equals(Details, other.Details, StringComparison.Ordinal) &&
            string.Equals(State, other.State, StringComparison.Ordinal) &&
            (StartTimestamp == other.StartTimestamp) &&
            string.Equals(LargeImageText, other.LargeImageText, StringComparison.Ordinal) &&
            string.Equals(ButtonLabel, other.ButtonLabel, StringComparison.Ordinal);

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? obj)
        => (obj is PresenceContentStruct other) && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
        => HashCode.Combine(Details, State, StartTimestamp, LargeImageText, ButtonLabel);

    /// <summary>
    /// Tells whether two values are equal.
    /// </summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    public static bool operator ==(in PresenceContentStruct left, in PresenceContentStruct right)
        => left.Equals(right);

    /// <summary>
    /// Tells whether two values differ.
    /// </summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    public static bool operator !=(in PresenceContentStruct left, in PresenceContentStruct right)
        => !left.Equals(right);
}
