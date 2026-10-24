using System.Collections.Generic;
using System.Globalization;

using Tf3Drpc.Core.Languages;

using Xunit;

namespace Tf3Drpc.Core.Tests.Languages;

public sealed class LanguageCatalogTests
{
    private const string RealProfile = "profile = {\n\tgraphics = {\n\t\tfov = 60,\n\t},\n\tlanguage = { \n\t\tcode = \"de\",\n\t\tmod = { \"\", },\n\t},\n}\n";

    [Fact]
    public void Catalog_HasThe13Languages()
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

    [Fact]
    public void ResolvedCulture_IsTheLanguagesCulture()
        => Assert.Equal(CultureInfo.GetCultureInfo("pt-BR"), LanguageCatalog.Resolve("pt_BR", null).Culture);

    [Theory]
    [InlineData("game", true)]
    [InlineData("de", true)]
    [InlineData("zh_CN", true)]
    [InlineData("zh-CN", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsValidSetting(string? setting, bool expected)
        => Assert.Equal(expected, LanguageCatalog.IsValidSetting(setting));

    [Theory]
    [InlineData(RealProfile, "de")]
    [InlineData("language={code=\"fr\"}", "fr")]
    [InlineData("language = {\n    mod = { \"abc\", },\n    code = \"pt_BR\",\n}", "pt_BR")]
    [InlineData("language\t=\t{\r\n\t\tcode\t=\t\"zh_CN\"\r\n}", "zh_CN")]
    [InlineData("other = { code = \"xx\" }, language = { code = \"ja\" }", "ja")]
    [InlineData("language = { mod = { code = \"nested\" }, code = \"ko\" }", "ko")]
    [InlineData("language = { code = \"de\"", "de")]
    public void ProfileText_WithLanguageCode(string text, string expected)
    {
        bool read = LanguageCatalog.TryReadLanguageCode(text, out string code);

        Assert.True(read);
        Assert.Equal(expected, code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("garbage \0 {{{{ ]]]")]
    [InlineData("language = \"de\"")]
    [InlineData("languages = { code = \"de\" }")]
    [InlineData("language = { mod = { \"\" } }")]
    [InlineData("language = { code = 5 }")]
    [InlineData("other = { code = \"de\" }")]
    public void ProfileText_WithoutLanguageCode(string text)
    {
        Assert.False(LanguageCatalog.TryReadLanguageCode(text, out string code));
        Assert.Empty(code);
    }

    [Theory]
    [InlineData("profile = {\n    -- language = { code = \"fr\" },\n    language = { code = \"de\" },\n}")]
    [InlineData("--[[ language = { code = \"fr\" } ]]\nlanguage = { code = \"de\" }")]
    [InlineData("--[==[\nlanguage = { code = \"fr\" }\n]] still comment\n]==] language = { code = \"de\" }")]
    [InlineData("language = { --[[ x ]] code = \"de\" -- \"fr\"\n}")]
    [InlineData("name = \"a--b\" language = { code = \"de\" }")]
    [InlineData("name = 'it\\'s -- x' language = { code = \"de\" }")]
    [InlineData("--[ not long\nlanguage = { code = \"de\" }")]
    [InlineData("x = [[ -- kept ]] language = { code = \"de\" }")]
    public void ProfileText_IgnoresLuaComments(string text)
    {
        Assert.True(LanguageCatalog.TryReadLanguageCode(text, out string code));
        Assert.Equal("de", code);
    }

    [Theory]
    [InlineData("-- language = { code = \"fr\" }")]
    [InlineData("--[[ language = { code = \"fr\" } ]]")]
    [InlineData("--[[ language = { code = \"fr\" }")]
    public void ProfileText_WithOnlyCommentedLanguage_HasNoCode(string text)
        => Assert.False(LanguageCatalog.TryReadLanguageCode(text, out string _));

    [Fact]
    public void HugeGarbage_DoesNotThrow()
    {
        string text = "language = {" + new string('{', 100_000) + new string('x', 100_000);

        Assert.False(LanguageCatalog.TryReadLanguageCode(text, out string _));
    }
}
