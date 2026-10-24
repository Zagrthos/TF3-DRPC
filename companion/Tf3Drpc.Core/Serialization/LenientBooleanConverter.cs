using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tf3Drpc.Core.Serialization;

/// <summary>
/// Reads a flag of the settings so that a value of the wrong type costs only that flag, never the other settings.
/// </summary>
/// <remarks>
/// Anything but <c>true</c> and <c>false</c> (a string, a number, an object, <c>null</c>) reads as <see langword="false"/>.
/// </remarks>
internal sealed class LenientBooleanConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        bool value = reader.TokenType is JsonTokenType.True;

        reader.Skip();

        return value;
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        => writer.WriteBooleanValue(value);
}
