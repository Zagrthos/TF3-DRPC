using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Tf3Drpc.Core.Structs;

/// <summary>
/// A Steam account's data folder of the game and the time its game log was last written, if it has one.
/// </summary>
[StructLayout(LayoutKind.Auto)]
internal readonly struct UserDataCandidateStruct : IEquatable<UserDataCandidateStruct>
{
    /// <summary>
    /// The folder of the user data.
    /// </summary>
    public string Folder { get; init; }

    /// <summary>
    /// The time the log in the folder was last written, or null when it has none.
    /// </summary>
    public DateTime? LogWrittenUtc { get; init; }

    /// <inheritdoc/>
    public bool Equals(UserDataCandidateStruct other)
        => string.Equals(Folder, other.Folder, StringComparison.Ordinal) &&
            (LogWrittenUtc == other.LogWrittenUtc);

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? obj)
        => (obj is UserDataCandidateStruct other) && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
        => HashCode.Combine(Folder, LogWrittenUtc);

    /// <summary>
    /// Tells whether two values are equal.
    /// </summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    public static bool operator ==(in UserDataCandidateStruct left, in UserDataCandidateStruct right)
        => left.Equals(right);

    /// <summary>
    /// Tells whether two values differ.
    /// </summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    public static bool operator !=(in UserDataCandidateStruct left, in UserDataCandidateStruct right)
        => !left.Equals(right);
}
