using System;
using System.Globalization;

using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Extensions;

namespace Tf3Drpc.Core.Resources;

/// <summary>
/// The companion app's own texts in a given culture.
/// </summary>
public static class OwnTexts
{
    /// <summary>
    /// Returns a text for an explicit culture; a missing translation falls back to English.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The text is not defined.</exception>
    public static string Get(OwnText text, CultureInfo culture)
    {
        string key = text.ResourceKey;

        if (key.Length is 0)
            throw new ArgumentOutOfRangeException(nameof(text), text, null);

        return Texts.ResourceManager.GetString(key, culture) ?? key;
    }
}
