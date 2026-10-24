using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using Tf3Drpc.Core.Languages;

namespace Tf3Drpc.Core.Structs;

/// <summary>
/// What the app currently knows: the game state, the Discord connection, the language and the presence texts.
/// </summary>
[StructLayout(LayoutKind.Auto)]
public readonly struct AppStatusStruct : IEquatable<AppStatusStruct>
{
    public GameStateSnapshotStruct Game { get; init; }

    public bool DiscordConnected { get; init; }

    public LanguageInfo Language { get; init; }

    public string? PresenceDetails { get; init; }

    public string? PresenceState { get; init; }

    public bool Equals(AppStatusStruct other)
        => Game.Equals(other.Game) &&
            (DiscordConnected == other.DiscordConnected) &&
            Equals(Language, other.Language) &&
            string.Equals(PresenceDetails, other.PresenceDetails, StringComparison.Ordinal) &&
            string.Equals(PresenceState, other.PresenceState, StringComparison.Ordinal);

    public override bool Equals([NotNullWhen(true)] object? obj)
        => (obj is AppStatusStruct other) && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(Game, DiscordConnected, Language, PresenceDetails, PresenceState);

    public static bool operator ==(in AppStatusStruct left, in AppStatusStruct right)
        => left.Equals(right);

    public static bool operator !=(in AppStatusStruct left, in AppStatusStruct right)
        => !left.Equals(right);
}
