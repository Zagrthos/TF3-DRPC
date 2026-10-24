using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Tf3Drpc.Core.Structs;

/// <summary>
/// A Steam account's data folder of the game and the time its game log was last written, if it has one.
/// </summary>
[StructLayout(LayoutKind.Auto)]
public readonly struct UserDataCandidateStruct : IEquatable<UserDataCandidateStruct>
{
    public string Folder { get; init; }

    public DateTime? LogWrittenUtc { get; init; }

    public bool Equals(UserDataCandidateStruct other)
        => string.Equals(Folder, other.Folder, StringComparison.Ordinal) &&
            (LogWrittenUtc == other.LogWrittenUtc);

    public override bool Equals([NotNullWhen(true)] object? obj)
        => (obj is UserDataCandidateStruct other) && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(Folder, LogWrittenUtc);

    public static bool operator ==(in UserDataCandidateStruct left, in UserDataCandidateStruct right)
        => left.Equals(right);

    public static bool operator !=(in UserDataCandidateStruct left, in UserDataCandidateStruct right)
        => !left.Equals(right);
}
