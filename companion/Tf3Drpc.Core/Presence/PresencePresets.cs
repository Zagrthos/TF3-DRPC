using System.Globalization;

using Tf3Drpc.Core.Constants;
using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Extensions;
using Tf3Drpc.Core.Resources;
using Tf3Drpc.Core.Translation;

namespace Tf3Drpc.Core.Presence;

/// <summary>
/// The templates of the presets, with the labels taken from the game's texts.
/// </summary>
internal static class PresencePresets
{
    /// <summary>
    /// Returns the preset that is shown at a rotation step: the alternating preset cycles through career, network and era.
    /// </summary>
    public static PresencePreset Resolve(PresencePreset preset, long rotationStep)
    {
        if (preset is not PresencePreset.Alternating)
            return preset;

        long layout = ((rotationStep % PresenceConstants.AlternatingLayoutCount) + PresenceConstants.AlternatingLayoutCount) % PresenceConstants.AlternatingLayoutCount;

        return layout switch
        {
            0 => PresencePreset.Career,
            1 => PresencePreset.Network,
            _ => PresencePreset.Era
        };
    }

    /// <summary>
    /// Returns the details template of a preset; the alternating preset must be resolved first.
    /// </summary>
    public static string GetDetails(PresencePreset preset, CultureInfo culture, GameTexts gameTexts)
        => preset switch
        {
            PresencePreset.Network => Join(Label(GameLabel.Lines, PresencePlaceholder.Lines, gameTexts), Label(GameLabel.Vehicles, PresencePlaceholder.Vehicles, gameTexts)),
            PresencePreset.Era => Join(Label(GameLabel.Year, PresencePlaceholder.Year, gameTexts), Placeholder(PresencePlaceholder.Climate)),
            PresencePreset.Minimal => Join(OwnTexts.Get(OwnText.InGame, culture), Placeholder(PresencePlaceholder.Year)),
            _ => Join(Placeholder(PresencePlaceholder.Rank), Placeholder(PresencePlaceholder.Mode))
        };

    /// <summary>
    /// Returns the state template of a preset, or <see langword="null"/> when the preset has none.
    /// </summary>
    public static string? GetState(PresencePreset preset, GameTexts gameTexts)
        => preset switch
        {
            PresencePreset.Network => Join(Label(GameLabel.Stations, PresencePlaceholder.Stations, gameTexts), Placeholder(PresencePlaceholder.Year)),
            PresencePreset.Era => Placeholder(PresencePlaceholder.Rank),
            PresencePreset.Minimal => null,
            _ => Placeholder(PresencePlaceholder.Date)
        };

    private static string Placeholder(PresencePlaceholder placeholder)
        => PresenceConstants.PlaceholderStart + placeholder.Name + PresenceConstants.PlaceholderEnd;

    private static string Label(GameLabel label, PresencePlaceholder placeholder, GameTexts gameTexts)
        => gameTexts.GetLabel(label) + " " + Placeholder(placeholder);

    private static string Join(string first, string second)
        => first + DiscordConstants.Separator + second;
}
