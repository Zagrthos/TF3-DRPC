using System;
using System.Text.Json;
using System.Text.Json.Serialization;

using Tf3Drpc.Core.Settings;

namespace Tf3Drpc.Core.Serialization;

/// <summary>
/// Reads the templates of the settings so that a value of the wrong type costs only that value, never the other settings.
/// </summary>
/// <remarks>
/// Anything that is not an object gives the default templates; a field that is not a string keeps its default.
/// </remarks>
internal sealed class LenientTemplatesConverter : JsonConverter<PresenceTemplates>
{
    public override PresenceTemplates Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is not JsonTokenType.StartObject)
        {
            reader.Skip();

            return new PresenceTemplates();
        }

        PresenceTemplates templates = new();

        while (reader.Read() && (reader.TokenType is JsonTokenType.PropertyName))
        {
            string? name = reader.GetString();
            reader.Read();
            string? value = (reader.TokenType is JsonTokenType.String) ? reader.GetString() : null;
            reader.Skip();

            if (value is null)
                continue;

            templates = name switch
            {
                "details" => templates with { Details = value },
                "state" => templates with { State = value },
                "hover" => templates with { Hover = value },
                _ => templates
            };
        }

        return templates;
    }

    public override void Write(Utf8JsonWriter writer, PresenceTemplates value, JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, value, JsonSourceGenContext.Default.PresenceTemplates);
}
