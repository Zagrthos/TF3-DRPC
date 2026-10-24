using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Status;

namespace Tf3Drpc.Core.Structs;

/// <summary>
/// A game log line the companion reacts to, with the values taken from it.
/// </summary>
[StructLayout(LayoutKind.Auto)]
public readonly struct GameLogEventStruct : IEquatable<GameLogEventStruct>
{
    public GameLogEventKind Kind { get; init; }

    public string? SaveName { get; init; }

    public int? MissionNumber { get; init; }

    public int? GameBuild { get; init; }

    internal ModStatus? Status { get; init; }

    public bool IsCrash { get; init; }

    public bool Equals(GameLogEventStruct other)
        => (Kind == other.Kind) &&
            string.Equals(SaveName, other.SaveName, StringComparison.Ordinal) &&
            (MissionNumber == other.MissionNumber) &&
            (GameBuild == other.GameBuild) &&
            EqualityComparer<ModStatus?>.Default.Equals(Status, other.Status) &&
            (IsCrash == other.IsCrash);

    public override bool Equals([NotNullWhen(true)] object? obj)
        => (obj is GameLogEventStruct other) && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(Kind, SaveName, MissionNumber, GameBuild, Status, IsCrash);

    public static bool operator ==(in GameLogEventStruct left, in GameLogEventStruct right)
        => left.Equals(right);

    public static bool operator !=(in GameLogEventStruct left, in GameLogEventStruct right)
        => !left.Equals(right);
}
