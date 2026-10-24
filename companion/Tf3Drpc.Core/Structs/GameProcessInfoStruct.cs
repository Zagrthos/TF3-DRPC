using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Tf3Drpc.Core.Structs;

/// <summary>
/// Whether the game process runs and, if known, where its executable is and when it started (UTC).
/// </summary>
[StructLayout(LayoutKind.Auto)]
public readonly struct GameProcessInfoStruct : IEquatable<GameProcessInfoStruct>
{
    public bool IsRunning { get; init; }

    public string? ExecutablePath { get; init; }

    public DateTimeOffset? StartTime { get; init; }

    public bool Equals(GameProcessInfoStruct other)
        => (IsRunning == other.IsRunning) &&
            string.Equals(ExecutablePath, other.ExecutablePath, StringComparison.Ordinal) &&
            (StartTime == other.StartTime);

    public override bool Equals([NotNullWhen(true)] object? obj)
        => (obj is GameProcessInfoStruct other) && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(IsRunning, ExecutablePath, StartTime);

    public static bool operator ==(in GameProcessInfoStruct left, in GameProcessInfoStruct right)
        => left.Equals(right);

    public static bool operator !=(in GameProcessInfoStruct left, in GameProcessInfoStruct right)
        => !left.Equals(right);
}
