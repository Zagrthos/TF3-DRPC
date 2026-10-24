using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Status;

namespace Tf3Drpc.Core.Structs;

/// <summary>
/// The game state at one moment; equal snapshots mean nothing changed.
/// </summary>
[StructLayout(LayoutKind.Auto)]
public readonly struct GameStateSnapshotStruct : IEquatable<GameStateSnapshotStruct>
{
    public GameState State { get; init; }

    public int? MissionNumber { get; init; }

    public DateTimeOffset? TimerStart { get; init; }

    /// <summary>
    /// The build number of the running game, once the log named it.
    /// </summary>
    public int? GameBuild { get; init; }

    internal ModStatus? Status { get; init; }

    public bool IsPaused { get; init; }

    /// <summary>
    /// Tells whether the state is assumed because the game log gave no usable line.
    /// </summary>
    public bool UsesLogFallback { get; init; }

    public bool Equals(GameStateSnapshotStruct other)
        => (State == other.State) &&
            (MissionNumber == other.MissionNumber) &&
            (TimerStart == other.TimerStart) &&
            (GameBuild == other.GameBuild) &&
            EqualityComparer<ModStatus?>.Default.Equals(Status, other.Status) &&
            (IsPaused == other.IsPaused) &&
            (UsesLogFallback == other.UsesLogFallback);

    public override bool Equals([NotNullWhen(true)] object? obj)
        => (obj is GameStateSnapshotStruct other) && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(State, MissionNumber, TimerStart, GameBuild, Status, IsPaused, UsesLogFallback);

    public static bool operator ==(in GameStateSnapshotStruct left, in GameStateSnapshotStruct right)
        => left.Equals(right);

    public static bool operator !=(in GameStateSnapshotStruct left, in GameStateSnapshotStruct right)
        => !left.Equals(right);
}
