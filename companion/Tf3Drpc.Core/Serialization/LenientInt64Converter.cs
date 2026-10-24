using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tf3Drpc.Core.Serialization;

/// <summary>
/// Reads an integer that the mod may have written as a fraction or with an exponent (<c>1.5</c>, <c>1.44228e+07</c>).
/// </summary>
/// <remarks>
/// The value is rounded toward zero; a number outside the range of <see cref="long"/> reads as <see langword="null"/>.
/// </remarks>
internal sealed class LenientInt64Converter : JsonConverter<long?>
{
    // 2^63 is exactly representable as a double; long.MaxValue is not.
    private const double LongRangeLimit = 9223372036854775808.0;

    public override long? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is not JsonTokenType.Number)
            throw new JsonException();

        if (reader.TryGetInt64(out long integer))
            return integer;

        if (!reader.TryGetDouble(out double number) || double.IsNaN(number))
            return null;

        number = Math.Truncate(number);

        return (number is >= -LongRangeLimit and < LongRangeLimit) ? (long)number : null;
    }

    public override void Write(Utf8JsonWriter writer, long? value, JsonSerializerOptions options)
    {
        if (value is long number)
        {
            writer.WriteNumberValue(number);
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}
