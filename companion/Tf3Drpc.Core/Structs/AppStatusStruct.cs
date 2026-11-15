using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using Tf3Drpc.Core.Languages;

namespace Tf3Drpc.Core.Structs;

/// <summary>
/// What the app currently knows: the game state, the Discord connection, the language and the presence texts.
/// </summary>
[StructLayout(LayoutKind.Auto)]
internal readonly struct AppStatusStruct : IEquatable<AppStatusStruct>
{
    /// <summary>
    /// The game state.
    /// </summary>
    public GameStateSnapshotStruct Game { get; init; }

    /// <summary>
    /// Whether a connection to Discord exists.
    /// </summary>
    public bool DiscordConnected { get; init; }

    /// <summary>
    /// The language the app uses.
    /// </summary>
    public LanguageInfo Language { get; init; }

    /// <summary>
    /// The first line of the shown presence, or null when none is shown.
    /// </summary>
    public string? PresenceDetails { get; init; }

    /// <summary>
    /// The second line of the shown presence, or null when it has none.
    /// </summary>
    public string? PresenceState { get; init; }

    /// <inheritdoc/>
    public bool Equals(AppStatusStruct other)
        => Game.Equals(other.Game) &&
            (DiscordConnected == other.DiscordConnected) &&
            Equals(Language, other.Language) &&
            string.Equals(PresenceDetails, other.PresenceDetails, StringComparison.Ordinal) &&
            string.Equals(PresenceState, other.PresenceState, StringComparison.Ordinal);

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? obj)
        => (obj is AppStatusStruct other) && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
        => HashCode.Combine(Game, DiscordConnected, Language, PresenceDetails, PresenceState);

    /// <summary>
    /// Tells whether two values are equal.
    /// </summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    public static bool operator ==(in AppStatusStruct left, in AppStatusStruct right)
        => left.Equals(right);

    /// <summary>
    /// Tells whether two values differ.
    /// </summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    public static bool operator !=(in AppStatusStruct left, in AppStatusStruct right)
        => !left.Equals(right);
}
