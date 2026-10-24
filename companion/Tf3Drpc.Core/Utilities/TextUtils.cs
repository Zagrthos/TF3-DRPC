using System;
using System.Globalization;
using System.Text;

namespace Tf3Drpc.Core.Utilities;

/// <summary>
/// Helpers for plain text.
/// </summary>
internal static class TextUtils
{
    /// <summary>
    /// Cuts a text to at most the given length without splitting a surrogate pair.
    /// </summary>
    public static string CutToLength(string text, int maxLength)
    {
        ArgumentNullException.ThrowIfNull(text);

        if (text.Length <= maxLength)
            return text;

        int length = (char.IsHighSurrogate(text[maxLength - 1])) ? (maxLength - 1) : maxLength;

        return text[..length];
    }

    /// <summary>
    /// Cuts a text to at most the given number of UTF-8 bytes at a text-element boundary, so that no character is split.
    /// </summary>
    public static string CutToUtf8Bytes(string text, int maxBytes)
    {
        ArgumentNullException.ThrowIfNull(text);

        if (Encoding.UTF8.GetByteCount(text) <= maxBytes)
            return text;

        ReadOnlySpan<char> rest = text;
        int length = 0;
        int bytes = 0;

        while (!rest.IsEmpty)
        {
            int element = StringInfo.GetNextTextElementLength(rest);
            int elementBytes = Encoding.UTF8.GetByteCount(rest[..element]);

            if ((bytes + elementBytes) > maxBytes)
                break;

            bytes += elementBytes;
            length += element;
            rest = rest[element..];
        }

        return text[..length];
    }
}
