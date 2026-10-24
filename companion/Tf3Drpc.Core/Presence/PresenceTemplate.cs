using System;
using System.Globalization;
using System.Text;

using Tf3Drpc.Core.Constants;
using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Extensions;
using Tf3Drpc.Core.Status;
using Tf3Drpc.Core.Translation;
using Tf3Drpc.Core.Utilities;

namespace Tf3Drpc.Core.Presence;

/// <summary>
/// Renders a template: parts separated by a middle dot, with <c>{name}</c> placeholders for status values.
/// </summary>
internal static class PresenceTemplate
{
    private static readonly PresencePlaceholder[] Placeholders = Enum.GetValues<PresencePlaceholder>();

    /// <summary>
    /// Renders the template; a part with a placeholder that has no value or is unknown, a part that renders empty, and in a Tycoon game a part with the mode but not the rank when another part shows the rank, is dropped. Returns <see langword="null"/> when no part is left.
    /// </summary>
    public static string? Render(string template, ModStatus status, CultureInfo culture, GameTexts gameTexts)
    {
        ArgumentNullException.ThrowIfNull(template);
        ArgumentNullException.ThrowIfNull(status);
        ArgumentNullException.ThrowIfNull(culture);
        ArgumentNullException.ThrowIfNull(gameTexts);

        string[] parts = template.Split(PresenceConstants.PartSeparator);
        string?[] rendered = new string?[parts.Length];
        bool[] modeOnly = new bool[parts.Length];
        bool rankShown = false;

        for (int i = 0; i < parts.Length; i++)
        {
            rendered[i] = RenderPart(parts[i].Trim(), status, culture, gameTexts, out bool usesRank, out bool usesMode);
            modeOnly[i] = usesMode && !usesRank;
            rankShown |= usesRank && !string.IsNullOrWhiteSpace(rendered[i]);
        }

        // The Tycoon mod fixes the rank at Tycoon, so a part with only the mode would say it again.
        bool skipMode = rankShown && PresenceValueUtils.ModeRepeatsRank(status);
        StringBuilder result = new(template.Length);

        for (int i = 0; i < parts.Length; i++)
        {
            string? part = rendered[i];

            if (string.IsNullOrWhiteSpace(part) || (skipMode && modeOnly[i]))
                continue;

            if (result.Length is > 0)
                result.Append(DiscordConstants.Separator);

            result.Append(part.Trim());
        }

        return (result.Length is 0) ? null : result.ToString();
    }

    private static string? RenderPart(string part, ModStatus status, CultureInfo culture, GameTexts gameTexts, out bool usesRank, out bool usesMode)
    {
        usesRank = false;
        usesMode = false;
        StringBuilder builder = new(part.Length);
        int position = 0;

        while (position < part.Length)
        {
            int open = part.IndexOf(PresenceConstants.PlaceholderStart, position, StringComparison.Ordinal);
            int close = (open is >= 0) ? part.IndexOf(PresenceConstants.PlaceholderEnd, open + 1, StringComparison.Ordinal) : -1;

            if (close is < 0)
            {
                builder.Append(part, position, part.Length - position);

                break;
            }

            int nextOpen = part.IndexOf(PresenceConstants.PlaceholderStart, open + 1, StringComparison.Ordinal);

            if ((nextOpen is >= 0) && (nextOpen < close))
            {
                // A brace that is not closed before the next one is plain text.
                builder.Append(part, position, nextOpen - position);
                position = nextOpen;

                continue;
            }

            builder.Append(part, position, open - position);

            if (!TryFind(part.AsSpan(open + 1, close - open - 1), out PresencePlaceholder placeholder))
                return null;

            usesRank |= placeholder is PresencePlaceholder.Rank;
            usesMode |= placeholder is PresencePlaceholder.Mode;

            string? value = PresenceValueUtils.GetValue(placeholder, status, culture, gameTexts);

            if (value is null)
                return null;

            builder.Append(value);
            position = close + 1;
        }

        return builder.ToString();
    }

    private static bool TryFind(ReadOnlySpan<char> name, out PresencePlaceholder placeholder)
    {
        for (int i = 0; i < Placeholders.Length; i++)
        {
            if (name.SequenceEqual(Placeholders[i].Name))
            {
                placeholder = Placeholders[i];

                return true;
            }
        }

        placeholder = default;

        return false;
    }
}
