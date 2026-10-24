using System;
using System.Text;

namespace Tf3Drpc.Core.Utilities;

/// <summary>
/// Reads Lua source text without running it.
/// </summary>
internal static class LuaUtils
{
    /// <summary>
    /// Removes the comments of a Lua source text; quoted strings and long strings keep their content, line breaks stay.
    /// </summary>
    /// <remarks>
    /// A line comment ends before its line break; an unterminated long comment runs to the end of the text.
    /// </remarks>
    public static string StripComments(string source)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (!source.Contains("--", StringComparison.Ordinal))
            return source;

        StringBuilder result = new(source.Length);
        int i = 0;

        while (i < source.Length)
        {
            char c = source[i];

            if (c is '"' or '\'')
            {
                int end = FindStringEnd(source, i);
                result.Append(source, i, end - i);
                i = end;
            }
            else if ((c is '[') && (GetLongBracketLevel(source, i) is int stringLevel and >= 0))
            {
                int end = FindLongBracketEnd(source, i, stringLevel);
                result.Append(source, i, end - i);
                i = end;
            }
            else if ((c is '-') && ((i + 1) < source.Length) && (source[i + 1] is '-'))
            {
                int start = i + 2;
                int commentLevel = ((start < source.Length) && (source[start] is '[')) ? GetLongBracketLevel(source, start) : -1;

                i = (commentLevel >= 0) ? FindLongBracketEnd(source, start, commentLevel) : FindLineEnd(source, start);
                result.Append(' ');
            }
            else
            {
                result.Append(c);
                i++;
            }
        }

        return result.ToString();
    }

    private static int FindStringEnd(string source, int start)
    {
        char quote = source[start];
        int i = start + 1;

        while (i < source.Length)
        {
            char c = source[i];

            if (c is '\\')
            {
                i += 2;
            }
            else if ((c == quote) || (c is '\n'))
            {
                return (c == quote) ? (i + 1) : i;
            }
            else
            {
                i++;
            }
        }

        return source.Length;
    }

    private static int FindLineEnd(string source, int start)
    {
        int end = source.IndexOf('\n', start, StringComparison.Ordinal);

        return (end < 0) ? source.Length : end;
    }

    /// <summary>
    /// Returns the number of equals signs of a long bracket opening at <paramref name="start"/> (<c>[[</c> is 0), or -1.
    /// </summary>
    private static int GetLongBracketLevel(string source, int start)
    {
        int i = start + 1;

        while ((i < source.Length) && (source[i] is '='))
            i++;

        return ((i < source.Length) && (source[i] is '[')) ? (i - start - 1) : -1;
    }

    private static int FindLongBracketEnd(string source, int start, int level)
    {
        string closing = "]" + new string('=', level) + "]";
        int end = source.IndexOf(closing, start + level + 2, StringComparison.Ordinal);

        return (end < 0) ? source.Length : (end + closing.Length);
    }
}
