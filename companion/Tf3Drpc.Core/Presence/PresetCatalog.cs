using System.Collections.Generic;

using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Extensions;

namespace Tf3Drpc.Core.Presence;

/// <summary>
/// The presets in display order and their settings values.
/// </summary>
public static class PresetCatalog
{
    private static readonly PresencePreset[] Presets =
    [
        PresencePreset.Career,
        PresencePreset.Network,
        PresencePreset.Era,
        PresencePreset.Alternating,
        PresencePreset.Minimal,
        PresencePreset.Custom
    ];

    /// <summary>
    /// All presets in display order.
    /// </summary>
    public static IReadOnlyList<PresencePreset> All
        => Presets;

    /// <summary>
    /// Finds a preset by its settings value (ordinal, case-sensitive).
    /// </summary>
    public static bool TryParse(string? setting, out PresencePreset preset)
        => PresencePreset.TryParseSetting(setting, out preset);

    /// <summary>
    /// Returns the settings value of a preset.
    /// </summary>
    public static string ToSetting(PresencePreset preset)
        => preset.SettingValue;

    /// <summary>
    /// Returns the own text that names a preset.
    /// </summary>
    public static OwnText GetDisplayText(PresencePreset preset)
        => preset.DisplayText;
}
