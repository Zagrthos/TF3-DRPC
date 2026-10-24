using System.Text.Json.Serialization;

using Tf3Drpc.Core.Constants;

namespace Tf3Drpc.Core.Settings;

/// <summary>
/// The templates of the custom preset: the first line, the second line and the hover text of the image.
/// </summary>
public sealed record PresenceTemplates
{
    [JsonPropertyName("details")]
    public string Details { get; init; } = PresenceConstants.DefaultDetailsTemplate;

    [JsonPropertyName("state")]
    public string State { get; init; } = PresenceConstants.DefaultStateTemplate;

    [JsonPropertyName("hover")]
    public string Hover { get; init; } = DiscordConstants.LargeImageText;
}
