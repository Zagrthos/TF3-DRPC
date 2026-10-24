using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Tf3Drpc.Core.Structs;

/// <summary>
/// The folders and files of the game that the app reads; a missing part is null.
/// </summary>
[StructLayout(LayoutKind.Auto)]
public readonly struct GamePathsStruct : IEquatable<GamePathsStruct>
{
    public string? LogPath { get; init; }

    public string? ProfilePath { get; init; }

    public string? GameDirectory { get; init; }

    public bool IsComplete
        => (LogPath is not null) && (ProfilePath is not null) && (GameDirectory is not null);

    public bool Equals(GamePathsStruct other)
        => string.Equals(LogPath, other.LogPath, StringComparison.Ordinal) &&
            string.Equals(ProfilePath, other.ProfilePath, StringComparison.Ordinal) &&
            string.Equals(GameDirectory, other.GameDirectory, StringComparison.Ordinal);

    public override bool Equals([NotNullWhen(true)] object? obj)
        => (obj is GamePathsStruct other) && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(LogPath, ProfilePath, GameDirectory);

    public static bool operator ==(in GamePathsStruct left, in GamePathsStruct right)
        => left.Equals(right);

    public static bool operator !=(in GamePathsStruct left, in GamePathsStruct right)
        => !left.Equals(right);
}
