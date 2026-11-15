using System;
using System.Text;

using Tf3Drpc.Core.Utilities;

using Xunit;

namespace Tf3Drpc.Core.Tests.Utilities;

/// <summary>
/// Tests of <see cref="TextUtils"/>, which cuts text to a number of UTF-8 bytes without splitting characters.
/// </summary>
public sealed class TextUtilsTests
{
    /// <summary>
    /// A text shorter than the limit of 31 bytes is returned unchanged.
    /// </summary>
    [Fact]
    public void CutToUtf8BytesKeepsAShortText()
        => Assert.Equal("Get Transport Fever 3", TextUtils.CutToUtf8Bytes("Get Transport Fever 3", 31));

    /// <summary>
    /// A text of exactly 31 bytes is returned unchanged.
    /// </summary>
    [Fact]
    public void CutToUtf8BytesKeepsATextOfExactlyTheLimit()
        => Assert.Equal(new string('a', 31), TextUtils.CutToUtf8Bytes(new string('a', 31), 31));

    /// <summary>
    /// An ASCII text of 40 characters is cut to its first 31 characters.
    /// </summary>
    [Fact]
    public void CutToUtf8BytesCutsAsciiAtTheLimit()
        => Assert.Equal(new string('a', 31), TextUtils.CutToUtf8Bytes(new string('a', 40), 31));

    /// <summary>
    /// A text of two-, three- or four-byte characters is cut after the last whole character that fits in 31 bytes, so the result never exceeds 31 bytes and never ends in half a character.
    /// </summary>
    [Theory]
    [InlineData("ü", 2)]
    [InlineData("日", 3)]
    [InlineData("😀", 4)]
    public void CutToUtf8BytesNeverSplitsACharacter(string character, int bytes)
    {
        ArgumentNullException.ThrowIfNull(character);

        string text = new StringBuilder().Insert(0, character, 20).ToString();

        string cut = TextUtils.CutToUtf8Bytes(text, 31);

        Assert.Equal(31 / bytes, cut.Length / character.Length);
        Assert.True(Encoding.UTF8.GetByteCount(cut) <= 31);
        Assert.Equal(new StringBuilder().Insert(0, character, 31 / bytes).ToString(), cut);
    }

    /// <summary>
    /// A letter with a combining accent, which takes 3 bytes, is never split from its accent, so 31 bytes of such elements give exactly 10 of them.
    /// </summary>
    [Fact]
    public void CutToUtf8BytesNeverSplitsATextElement()
    {
        // "e" plus a combining acute accent is one element of 3 bytes.
        string text = new StringBuilder().Insert(0, "e\u0301", 20).ToString();

        string cut = TextUtils.CutToUtf8Bytes(text, 31);

        Assert.Equal(new StringBuilder().Insert(0, "e\u0301", 10).ToString(), cut);
    }

    /// <summary>
    /// An empty text is cut to an empty text.
    /// </summary>
    [Fact]
    public void CutToUtf8BytesOfAnEmptyTextIsEmpty()
        => Assert.Equal(string.Empty, TextUtils.CutToUtf8Bytes(string.Empty, 31));
}
