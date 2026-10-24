using System.Text.Json.Serialization;

using Tf3Drpc.Core.Constants;
using Tf3Drpc.Core.Serialization;

namespace Tf3Drpc.Core.Settings;

/// <summary>
/// The companion app's settings: the language (<c>game</c> or a language code), the preset, the theme (<c>system</c>, <c>light</c> or <c>dark</c>), the templates of the custom preset and whether the presence carries a store button.
/// </summary>
public sealed record AppSettings
{
    [JsonPropertyName("language")]
    [JsonConverter(typeof(LenientStringConverter))]
    public string Language { get; init; } = LanguageConstants.GameSetting;

    [JsonPropertyName("preset")]
    [JsonConverter(typeof(LenientStringConverter))]
    public string Preset { get; init; } = SettingsConstants.DefaultPreset;

    [JsonPropertyName("theme")]
    [JsonConverter(typeof(LenientStringConverter))]
    public string Theme { get; init; } = SettingsConstants.DefaultTheme;

    [JsonPropertyName("templates")]
    [JsonConverter(typeof(LenientTemplatesConverter))]
    public PresenceTemplates Templates { get; init; } = new PresenceTemplates();

    [JsonPropertyName("storeButton")]
    [JsonConverter(typeof(LenientBooleanConverter))]
    public bool StoreButton { get; init; }
}
