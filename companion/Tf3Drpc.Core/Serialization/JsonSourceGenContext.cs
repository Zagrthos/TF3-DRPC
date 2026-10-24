using System.Text.Json.Serialization;

using Tf3Drpc.Core.Discord;
using Tf3Drpc.Core.Settings;
using Tf3Drpc.Core.Status;

namespace Tf3Drpc.Core.Serialization;

[JsonSourceGenerationOptions
(
    GenerationMode = JsonSourceGenerationMode.Default,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    WriteIndented = false,
    Converters = [typeof(LenientInt64Converter)]
)]
[JsonSerializable(typeof(AppSettings))]
[JsonSerializable(typeof(DiscordCommand))]
[JsonSerializable(typeof(DiscordHandshake))]
[JsonSerializable(typeof(DiscordMessage))]
[JsonSerializable(typeof(ModStatus))]
[JsonSerializable(typeof(PresenceTemplates))]
internal sealed partial class JsonSourceGenContext : JsonSerializerContext;
