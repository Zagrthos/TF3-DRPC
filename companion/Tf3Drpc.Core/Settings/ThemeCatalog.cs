using System.Collections.Generic;

using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Extensions;

namespace Tf3Drpc.Core.Settings;

/// <summary>
/// The themes in display order and their settings values.
/// </summary>
public static class ThemeCatalog
{
    private static readonly ThemeMode[] Themes =
    [
        ThemeMode.System,
        ThemeMode.Light,
        ThemeMode.Dark
    ];

    /// <summary>
    /// All themes in display order.
    /// </summary>
    public static IReadOnlyList<ThemeMode> All
        => Themes;

    /// <summary>
    /// Finds a theme by its settings value (ordinal, case-sensitive).
    /// </summary>
    public static bool TryParse(string? setting, out ThemeMode theme)
        => ThemeMode.TryParseSetting(setting, out theme);

    /// <summary>
    /// Returns the settings value of a theme.
    /// </summary>
    public static string ToSetting(ThemeMode theme)
        => theme.SettingValue;

    /// <summary>
    /// Returns the own text that names a theme.
    /// </summary>
    public static OwnText GetDisplayText(ThemeMode theme)
        => theme.DisplayText;
}
