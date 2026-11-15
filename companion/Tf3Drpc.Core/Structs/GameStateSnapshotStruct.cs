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
internal readonly struct GameStateSnapshotStruct : IEquatable<GameStateSnapshotStruct>
{
    /// <summary>
    /// The state of the game.
    /// </summary>
    public GameState State { get; init; }

    /// <summary>
    /// The number of the running mission, or null when there is none.
    /// </summary>
    public int? MissionNumber { get; init; }

    /// <summary>
    /// The time the shown timer counts from, or null for none.
    /// </summary>
    public DateTimeOffset? TimerStart { get; init; }

    /// <summary>
    /// The build number of the running game, once the log named it.
    /// </summary>
    public int? GameBuild { get; init; }

    internal ModStatus? Status { get; init; }

    /// <summary>
    /// Whether the game counts as paused.
    /// </summary>
    public bool IsPaused { get; init; }

    /// <summary>
    /// Tells whether the state is assumed because the game log gave no usable line.
    /// </summary>
    public bool UsesLogFallback { get; init; }

    /// <inheritdoc/>
    public bool Equals(GameStateSnapshotStruct other)
        => (State == other.State) &&
            (MissionNumber == other.MissionNumber) &&
            (TimerStart == other.TimerStart) &&
            (GameBuild == other.GameBuild) &&
            EqualityComparer<ModStatus?>.Default.Equals(Status, other.Status) &&
            (IsPaused == other.IsPaused) &&
            (UsesLogFallback == other.UsesLogFallback);

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? obj)
        => (obj is GameStateSnapshotStruct other) && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
        => HashCode.Combine(State, MissionNumber, TimerStart, GameBuild, Status, IsPaused, UsesLogFallback);

    /// <summary>
    /// Tells whether two values are equal.
    /// </summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    public static bool operator ==(in GameStateSnapshotStruct left, in GameStateSnapshotStruct right)
        => left.Equals(right);

    /// <summary>
    /// Tells whether two values differ.
    /// </summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    public static bool operator !=(in GameStateSnapshotStruct left, in GameStateSnapshotStruct right)
        => !left.Equals(right);
}
