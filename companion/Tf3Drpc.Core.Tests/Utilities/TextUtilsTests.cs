using System;
using System.Text;

using Tf3Drpc.Core.Utilities;

using Xunit;

namespace Tf3Drpc.Core.Tests.Utilities;

public sealed class TextUtilsTests
{
    [Fact]
    public void CutToUtf8Bytes_KeepsAShortText()
        => Assert.Equal("Get Transport Fever 3", TextUtils.CutToUtf8Bytes("Get Transport Fever 3", 31));

    [Fact]
    public void CutToUtf8Bytes_KeepsATextOfExactlyTheLimit()
        => Assert.Equal(new string('a', 31), TextUtils.CutToUtf8Bytes(new string('a', 31), 31));

    [Fact]
    public void CutToUtf8Bytes_CutsAsciiAtTheLimit()
        => Assert.Equal(new string('a', 31), TextUtils.CutToUtf8Bytes(new string('a', 40), 31));

    [Theory]
    [InlineData("ü", 2)]
    [InlineData("日", 3)]
    [InlineData("😀", 4)]
    public void CutToUtf8Bytes_NeverSplitsACharacter(string character, int bytes)
    {
        ArgumentNullException.ThrowIfNull(character);

        string text = new StringBuilder().Insert(0, character, 20).ToString();

        string cut = TextUtils.CutToUtf8Bytes(text, 31);

        Assert.Equal(31 / bytes, cut.Length / character.Length);
        Assert.True(Encoding.UTF8.GetByteCount(cut) <= 31);
        Assert.Equal(new StringBuilder().Insert(0, character, 31 / bytes).ToString(), cut);
    }

    [Fact]
    public void CutToUtf8Bytes_NeverSplitsATextElement()
    {
        // "e" plus a combining acute accent is one element of 3 bytes.
        string text = new StringBuilder().Insert(0, "e\u0301", 20).ToString();

        string cut = TextUtils.CutToUtf8Bytes(text, 31);

        Assert.Equal(new StringBuilder().Insert(0, "e\u0301", 10).ToString(), cut);
    }

    [Fact]
    public void CutToUtf8Bytes_OfAnEmptyTextIsEmpty()
        => Assert.Equal(string.Empty, TextUtils.CutToUtf8Bytes(string.Empty, 31));
}
