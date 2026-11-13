using System.Collections.Generic;
using System.Globalization;

using Tf3Drpc.Core.Languages;

using Xunit;

namespace Tf3Drpc.Core.Tests.Languages;

/// <summary>
/// Tests of <see cref="LanguageCatalog"/>, which lists the supported languages, resolves the language setting and reads the game language from the game profile.
/// </summary>
public sealed class LanguageCatalogTests
{
    private const string RealProfile = "profile = {\n\tgraphics = {\n\t\tfov = 60,\n\t},\n\tlanguage = { \n\t\tcode = \"de\",\n\t\tmod = { \"\", },\n\t},\n}\n";

    /// <summary>
    /// The catalogue lists the 13 supported languages in order, each with its game code, native name and culture, from "de" and "Deutsch" to "zh_TW" and "繁體中文".
    /// </summary>
    [Fact]
    public void CatalogHasThe13Languages()
    {
        string[] codes = ["de", "en", "es", "fr", "it", "ja", "ko", "nl", "pl", "pt_BR", "ru", "zh_CN", "zh_TW"];
        string[] cultures = ["de-DE", "en-US", "es-ES", "fr-FR", "it-IT", "ja-JP", "ko-KR", "nl-NL", "pl-PL", "pt-BR", "ru-RU", "zh-CN", "zh-TW"];
        string[] names =
        [
            "Deutsch", "English", "Español", "Français", "Italiano", "日本語", "한국어", "Nederlands",
            "Polski", "Português (Brasil)", "Русский", "简体中文", "繁體中文"
        ];

        IReadOnlyList<LanguageInfo> all = LanguageCatalog.All;

        Assert.Equal(13, all.Count);

        for (int i = 0; i < all.Count; i++)
        {
            Assert.Equal(codes[i], all[i].Code);
            Assert.Equal(names[i], all[i].NativeName);
            Assert.Equal(cultures[i], all[i].Culture.Name);
        }
    }

    /// <summary>
    /// The setting "game" resolves to the game's own language code when it is supported, a fixed language code resolves to itself regardless of the game, and an unknown, empty, missing or wrongly capitalised value falls back to English.
    /// </summary>
    [Theory]
    [InlineData("game", "de", "de")]
    [InlineData("game", "pt_BR", "pt_BR")]
    [InlineData("game", "xx", "en")]
    [InlineData("game", null, "en")]
    [InlineData("game", "", "en")]
    [InlineData("ja", "de", "ja")]
    [InlineData("zh_TW", null, "zh_TW")]
    [InlineData("klingon", "de", "en")]
    [InlineData(null, "de", "en")]
    [InlineData("DE", "fr", "en")]
    public void Resolve(string? setting, string? gameCode, string expectedCode)
    {
        LanguageInfo language = LanguageCatalog.Resolve(setting, gameCode);

        Assert.Equal(expectedCode, language.Code);
    }

    /// <summary>
    /// The culture of a resolved language is the culture of that language, here pt-BR for "pt_BR".
    /// </summary>
    [Fact]
    public void ResolvedCultureIsTheLanguagesCulture()
        => Assert.Equal(CultureInfo.GetCultureInfo("pt-BR"), LanguageCatalog.Resolve("pt_BR", gameLanguageCode: null).Culture);

    /// <summary>
    /// Only "game" and the codes of the supported languages are valid settings, while a hyphenated code, an empty string and <see langword="null"/> are not.
    /// </summary>
    [Theory]
    [InlineData("game", true)]
    [InlineData("de", true)]
    [InlineData("zh_CN", true)]
    [InlineData("zh-CN", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsValidSetting(string? setting, bool expected)
        => Assert.Equal(expected, LanguageCatalog.IsValidSetting(setting));

    /// <summary>
    /// A profile text with a language block, written in any layout, spacing or line ending, yields its own language code, ignoring other blocks, a nested code and a missing closing brace.
    /// </summary>
    [Theory]
    [InlineData(RealProfile, "de")]
    [InlineData("language={code=\"fr\"}", "fr")]
    [InlineData("language = {\n    mod = { \"abc\", },\n    code = \"pt_BR\",\n}", "pt_BR")]
    [InlineData("language\t=\t{\r\n\t\tcode\t=\t\"zh_CN\"\r\n}", "zh_CN")]
    [InlineData("other = { code = \"xx\" }, language = { code = \"ja\" }", "ja")]
    [InlineData("language = { mod = { code = \"nested\" }, code = \"ko\" }", "ko")]
    [InlineData("language = { code = \"de\"", "de")]
    public void ProfileTextWithLanguageCode(string text, string expected)
    {
        bool read = LanguageCatalog.TryReadLanguageCode(text, out string code);

        Assert.True(read);
        Assert.Equal(expected, code);
    }

    /// <summary>
    /// Text with no language block, a language that is not a block, a block without a string code, or a code outside the language block gives no code and an empty result.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("garbage \0 {{{{ ]]]")]
    [InlineData("language = \"de\"")]
    [InlineData("languages = { code = \"de\" }")]
    [InlineData("language = { mod = { \"\" } }")]
    [InlineData("language = { code = 5 }")]
    [InlineData("other = { code = \"de\" }")]
    public void ProfileTextWithoutLanguageCode(string text)
    {
        Assert.False(LanguageCatalog.TryReadLanguageCode(text, out string code));
        Assert.Empty(code);
    }

    /// <summary>
    /// Lua comments, quoted strings and long strings are skipped correctly, so a commented-out language never counts and a double hyphen inside a string does not start a comment.
    /// </summary>
    [Theory]
    [InlineData("profile = {\n    -- language = { code = \"fr\" },\n    language = { code = \"de\" },\n}")]
    [InlineData("--[[ language = { code = \"fr\" } ]]\nlanguage = { code = \"de\" }")]
    [InlineData("--[==[\nlanguage = { code = \"fr\" }\n]] still comment\n]==] language = { code = \"de\" }")]
    [InlineData("language = { --[[ x ]] code = \"de\" -- \"fr\"\n}")]
    [InlineData("name = \"a--b\" language = { code = \"de\" }")]
    [InlineData("name = 'it\\'s -- x' language = { code = \"de\" }")]
    [InlineData("--[ not long\nlanguage = { code = \"de\" }")]
    [InlineData("x = [[ -- kept ]] language = { code = \"de\" }")]
    public void ProfileTextIgnoresLuaComments(string text)
    {
        Assert.True(LanguageCatalog.TryReadLanguageCode(text, out string code));
        Assert.Equal("de", code);
    }

    /// <summary>
    /// A language block that is only inside a line comment or a block comment, closed or unterminated, gives no code.
    /// </summary>
    [Theory]
    [InlineData("-- language = { code = \"fr\" }")]
    [InlineData("--[[ language = { code = \"fr\" } ]]")]
    [InlineData("--[[ language = { code = \"fr\" }")]
    public void ProfileTextWithOnlyCommentedLanguageHasNoCode(string text)
        => Assert.False(LanguageCatalog.TryReadLanguageCode(text, out string _));

    /// <summary>
    /// A very long run of braces and letters with no valid language block fails to read instead of throwing.
    /// </summary>
    [Fact]
    public void HugeGarbageDoesNotThrow()
    {
        string text = "language = {" + new string('{', 100_000) + new string('x', 100_000);

        Assert.False(LanguageCatalog.TryReadLanguageCode(text, out string _));
    }
}
