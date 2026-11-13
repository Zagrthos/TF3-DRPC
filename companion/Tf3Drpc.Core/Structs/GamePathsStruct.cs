using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Tf3Drpc.Core.Structs;

/// <summary>
/// The folders and files of the game that the app reads; a missing part is null.
/// </summary>
[StructLayout(LayoutKind.Auto)]
internal readonly struct GamePathsStruct : IEquatable<GamePathsStruct>
{
    /// <summary>
    /// The path of the game log, or null when unknown.
    /// </summary>
    public string? LogPath { get; init; }

    /// <summary>
    /// The path of the profile file, or null when unknown.
    /// </summary>
    public string? ProfilePath { get; init; }

    /// <summary>
    /// The directory of the game, or null when unknown.
    /// </summary>
    public string? GameDirectory { get; init; }

    /// <summary>
    /// Whether the log path, the profile path and the game directory are all known.
    /// </summary>
    public bool IsComplete
        => (LogPath is not null) && (ProfilePath is not null) && (GameDirectory is not null);

    /// <inheritdoc/>
    public bool Equals(GamePathsStruct other)
        => string.Equals(LogPath, other.LogPath, StringComparison.Ordinal) &&
            string.Equals(ProfilePath, other.ProfilePath, StringComparison.Ordinal) &&
            string.Equals(GameDirectory, other.GameDirectory, StringComparison.Ordinal);

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? obj)
        => (obj is GamePathsStruct other) && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
        => HashCode.Combine(LogPath, ProfilePath, GameDirectory);

    /// <summary>
    /// Tells whether two values are equal.
    /// </summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    public static bool operator ==(in GamePathsStruct left, in GamePathsStruct right)
        => left.Equals(right);

    /// <summary>
    /// Tells whether two values differ.
    /// </summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    public static bool operator !=(in GamePathsStruct left, in GamePathsStruct right)
        => !left.Equals(right);
}
