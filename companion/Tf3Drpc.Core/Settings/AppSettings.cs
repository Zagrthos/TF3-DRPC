using System.Text.Json.Serialization;

using Tf3Drpc.Core.Constants;
using Tf3Drpc.Core.Serialization;

namespace Tf3Drpc.Core.Settings;

/// <summary>
/// The companion app's settings: the language (<c language="text">game</c> or a language code), the preset, the theme (<c language="text">system</c>, <c language="text">light</c> or <c language="text">dark</c>), the templates of the custom preset and whether the presence carries a store button.
/// </summary>
internal sealed record AppSettings
{
    /// <summary>
    /// The language setting: the game setting or a language code.
    /// </summary>
    [JsonPropertyName("language")]
    [JsonConverter(typeof(LenientStringConverter))]
    public string Language { get; init; } = LanguageConstants.GameSetting;

    /// <summary>
    /// The name of the presence preset.
    /// </summary>
    [JsonPropertyName("preset")]
    [JsonConverter(typeof(LenientStringConverter))]
    public string Preset { get; init; } = SettingsConstants.DefaultPreset;

    /// <summary>
    /// The name of the theme.
    /// </summary>
    [JsonPropertyName("theme")]
    [JsonConverter(typeof(LenientStringConverter))]
    public string Theme { get; init; } = SettingsConstants.DefaultTheme;

    /// <summary>
    /// The templates of the custom preset.
    /// </summary>
    [JsonPropertyName("templates")]
    [JsonConverter(typeof(LenientTemplatesConverter))]
    public PresenceTemplates Templates { get; init; } = new PresenceTemplates();

    /// <summary>
    /// Whether the presence carries a store button.
    /// </summary>
    [JsonPropertyName("storeButton")]
    [JsonConverter(typeof(LenientBooleanConverter))]
    public bool StoreButton { get; init; }
}
