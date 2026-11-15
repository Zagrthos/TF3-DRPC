using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Tf3Drpc.Core.Structs;

/// <summary>
/// Whether the game process runs and, if known, where its executable is and when it started (UTC).
/// </summary>
[StructLayout(LayoutKind.Auto)]
internal readonly struct GameProcessInfoStruct : IEquatable<GameProcessInfoStruct>
{
    /// <summary>
    /// Whether the game process runs.
    /// </summary>
    public bool IsRunning { get; init; }

    /// <summary>
    /// The path of the running executable, or null when unknown.
    /// </summary>
    public string? ExecutablePath { get; init; }

    /// <summary>
    /// The start time of the process, or null when unknown.
    /// </summary>
    public DateTimeOffset? StartTime { get; init; }

    /// <inheritdoc/>
    public bool Equals(GameProcessInfoStruct other)
        => (IsRunning == other.IsRunning) &&
            string.Equals(ExecutablePath, other.ExecutablePath, StringComparison.Ordinal) &&
            (StartTime == other.StartTime);

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? obj)
        => (obj is GameProcessInfoStruct other) && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
        => HashCode.Combine(IsRunning, ExecutablePath, StartTime);

    /// <summary>
    /// Tells whether two values are equal.
    /// </summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    public static bool operator ==(in GameProcessInfoStruct left, in GameProcessInfoStruct right)
        => left.Equals(right);

    /// <summary>
    /// Tells whether two values differ.
    /// </summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    public static bool operator !=(in GameProcessInfoStruct left, in GameProcessInfoStruct right)
        => !left.Equals(right);
}
