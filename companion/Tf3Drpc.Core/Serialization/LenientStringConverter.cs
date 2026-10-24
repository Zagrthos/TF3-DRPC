using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tf3Drpc.Core.Serialization;

/// <summary>
/// Reads a text of the settings so that a value of the wrong type costs only that text, never the other settings.
/// </summary>
/// <remarks>
/// Anything but a string (a number, a flag, an array, an object, <c>null</c>) reads as an empty text, which the settings normalisation replaces with the default.
/// </remarks>
internal sealed class LenientStringConverter : JsonConverter<string>
{
    public override bool HandleNull
        => true;

    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string value = (reader.TokenType is JsonTokenType.String) ? (reader.GetString() ?? string.Empty) : string.Empty;

        reader.Skip();

        return value;
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        => writer.WriteStringValue(value);
}
