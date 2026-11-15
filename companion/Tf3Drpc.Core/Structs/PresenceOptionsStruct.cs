using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using Tf3Drpc.Core.Constants;
using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Settings;
using Tf3Drpc.Core.Translation;

namespace Tf3Drpc.Core.Structs;

/// <summary>
/// What shapes the presence besides the game state: the layout while playing, the custom templates, the store button and the game language's texts.
/// </summary>
[StructLayout(LayoutKind.Auto)]
internal readonly struct PresenceOptionsStruct : IEquatable<PresenceOptionsStruct>
{
    /// <summary>
    /// The layout while playing.
    /// </summary>
    public PresencePreset Preset { get; init; }

    /// <summary>
    /// The step of the alternating preset, counted in <see cref="PresenceConstants.AlternatingInterval"/>.
    /// </summary>
    public long RotationStep { get; init; }

    /// <summary>
    /// The templates of the custom preset; <see langword="null"/> means the defaults.
    /// </summary>
    public PresenceTemplates? Templates { get; init; }

    /// <summary>
    /// Tells whether every presence carries the store button.
    /// </summary>
    public bool ShowStoreButton { get; init; }

    /// <summary>
    /// The game's texts in the game's own language; <see langword="null"/> means unknown.
    /// </summary>
    public GameTexts? GameLanguageTexts { get; init; }

    /// <inheritdoc/>
    public bool Equals(PresenceOptionsStruct other)
        => (Preset == other.Preset) &&
            (RotationStep == other.RotationStep) &&
            EqualityComparer<PresenceTemplates?>.Default.Equals(Templates, other.Templates) &&
            (ShowStoreButton == other.ShowStoreButton) &&
            ReferenceEquals(GameLanguageTexts, other.GameLanguageTexts);

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? obj)
        => (obj is PresenceOptionsStruct other) && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
        => HashCode.Combine(Preset, RotationStep, Templates, ShowStoreButton, GameLanguageTexts);

    /// <summary>
    /// Tells whether two values are equal.
    /// </summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    public static bool operator ==(in PresenceOptionsStruct left, in PresenceOptionsStruct right)
        => left.Equals(right);

    /// <summary>
    /// Tells whether two values differ.
    /// </summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    public static bool operator !=(in PresenceOptionsStruct left, in PresenceOptionsStruct right)
        => !left.Equals(right);
}
