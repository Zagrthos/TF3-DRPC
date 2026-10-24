using System.Text.Json.Serialization;

namespace Tf3Drpc.Core.Discord;

internal sealed record DiscordHandshake
(
    [property: JsonPropertyName("v")] int Version,
    [property: JsonPropertyName("client_id")] string ClientId
);

internal sealed record DiscordTimestamps
(
    [property: JsonPropertyName("start")] long Start
);

internal sealed record DiscordAssets
(
    [property: JsonPropertyName("large_image")] string LargeImage,
    [property: JsonPropertyName("large_text")] string LargeText
);

internal sealed record DiscordButton
(
    [property: JsonPropertyName("label")] string Label,
    [property: JsonPropertyName("url")] string Url
);

internal sealed record DiscordActivity
(
    [property: JsonPropertyName("details")] string Details,
    [property: JsonPropertyName("state")] string? State,
    [property: JsonPropertyName("timestamps")] DiscordTimestamps? Timestamps,
    [property: JsonPropertyName("assets")] DiscordAssets Assets,
    [property: JsonPropertyName("instance")] bool Instance,
    [property: JsonPropertyName("buttons")] DiscordButton[]? Buttons = null
);

internal sealed record DiscordCommandArguments
(
    [property: JsonPropertyName("pid")] int ProcessId,
    [property: JsonPropertyName("activity")] DiscordActivity? Activity
);

internal sealed record DiscordCommand
(
    [property: JsonPropertyName("cmd")] string Command,
    [property: JsonPropertyName("nonce")] string Nonce,
    [property: JsonPropertyName("args")] DiscordCommandArguments Arguments
);

internal sealed record DiscordMessageData
(
    [property: JsonPropertyName("code")] int? Code,
    [property: JsonPropertyName("message")] string? Message
);

internal sealed record DiscordMessage
(
    [property: JsonPropertyName("cmd")] string? Command,
    [property: JsonPropertyName("evt")] string? Event,
    [property: JsonPropertyName("nonce")] string? Nonce,
    [property: JsonPropertyName("data")] DiscordMessageData? Data
);
