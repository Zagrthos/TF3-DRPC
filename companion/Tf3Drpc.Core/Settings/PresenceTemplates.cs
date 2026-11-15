using System.Text.Json.Serialization;

using Tf3Drpc.Core.Constants;

namespace Tf3Drpc.Core.Settings;

/// <summary>
/// The templates of the custom preset: the first line, the second line and the hover text of the image.
/// </summary>
internal sealed record PresenceTemplates
{
    /// <summary>
    /// The template of the first line.
    /// </summary>
    [JsonPropertyName("details")]
    public string Details { get; init; } = PresenceConstants.DefaultDetailsTemplate;

    /// <summary>
    /// The template of the second line.
    /// </summary>
    [JsonPropertyName("state")]
    public string State { get; init; } = PresenceConstants.DefaultStateTemplate;

    /// <summary>
    /// The template of the hover text of the large image.
    /// </summary>
    [JsonPropertyName("hover")]
    public string Hover { get; init; } = DiscordConstants.LargeImageText;
}
