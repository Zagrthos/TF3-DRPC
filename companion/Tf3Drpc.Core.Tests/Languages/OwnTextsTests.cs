using System;
using System.Globalization;
using System.Text;

using Tf3Drpc.Core.Constants;
using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Languages;
using Tf3Drpc.Core.Resources;

using Xunit;

namespace Tf3Drpc.Core.Tests.Languages;

/// <summary>
/// Tests of <see cref="OwnTexts"/>, which holds the application's own translated texts for the 13 supported languages.
/// </summary>
public sealed class OwnTextsTests
{
    // Words that are written the same as in English in these languages.
    private static readonly (OwnText Text, string Code)[] IdenticalToEnglish =
    [
        (OwnText.PresetMinimal, "de"),
        (OwnText.PresetMinimal, "fr"),
        (OwnText.PresetEra, "it"),
        (OwnText.PresetEra, "pl"),
        (OwnText.PresetEra, "pt_BR")
    ];

    /// <summary>
    /// Every own text exists, and is not blank, in every supported language, and differs from the English text except for the few words listed as written the same as in English.
    /// </summary>
    [Fact]
    public void EveryKeyExistsInAllCulturesAndDiffersFromEnglish()
    {
        foreach (OwnText text in Enum.GetValues<OwnText>())
        {
            string english = OwnTexts.Get(text, CultureInfo.GetCultureInfo("en-US"));

            Assert.False(string.IsNullOrWhiteSpace(english));

            foreach (LanguageInfo language in LanguageCatalog.All)
            {
                string translated = OwnTexts.Get(text, language.Culture);

                Assert.False(string.IsNullOrWhiteSpace(translated), $"{text} in {language.Code}");

                if (!string.Equals(language.Code, "en", StringComparison.Ordinal) && !IsIdenticalToEnglish(text, language.Code))
                    Assert.NotEqual(english, translated, StringComparer.Ordinal);
            }
        }
    }

    /// <summary>
    /// The store button label fits into Discord's limit of button label bytes, measured as UTF-8, in every supported language.
    /// </summary>
    [Fact]
    public void StoreButtonFitsIntoDiscordsLimitInEveryLanguage()
    {
        foreach (LanguageInfo language in LanguageCatalog.All)
        {
            string label = OwnTexts.Get(OwnText.StoreButton, language.Culture);

            Assert.True(Encoding.UTF8.GetByteCount(label) <= DiscordConstants.MaxButtonLabelBytes, $"{language.Code}: {label}");
        }
    }

    /// <summary>
    /// The words listed as identical to English, such as the minimal preset in German and French, really are equal to the English text.
    /// </summary>
    [Fact]
    public void IdenticalTranslationsAreReallyIdentical()
    {
        for (int i = 0; i < IdenticalToEnglish.Length; i++)
        {
            LanguageCatalog.TryGet(IdenticalToEnglish[i].Code, out LanguageInfo language);

            Assert.Equal(OwnTexts.Get(IdenticalToEnglish[i].Text, CultureInfo.GetCultureInfo("en-US")), OwnTexts.Get(IdenticalToEnglish[i].Text, language.Culture));
        }
    }

    /// <summary>
    /// A culture passed explicitly is used for the lookup instead of the current UI culture, which the test sets to Japanese while asking for German and English.
    /// </summary>
    [Fact]
    public void ExplicitCultureIsUsedInsteadOfTheCurrentUiCulture()
    {
        CultureInfo previous = CultureInfo.CurrentUICulture;

        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("ja-JP");

            Assert.Equal("Im Hauptmenü", OwnTexts.Get(OwnText.InTheMainMenu, CultureInfo.GetCultureInfo("de-DE")));
            Assert.Equal("In the main menu", OwnTexts.Get(OwnText.InTheMainMenu, CultureInfo.GetCultureInfo("en-US")));
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }
    }

    /// <summary>
    /// Sample texts have the expected translation in their culture, covering the settings, preset names, hints and store button texts across German, Spanish, Japanese, Chinese, Portuguese, Russian, French, Korean and English.
    /// </summary>
    [Theory]
    [InlineData("de-DE", OwnText.StartingTheGame, "Spiel wird gestartet")]
    [InlineData("es-ES", OwnText.Language, "Idioma")]
    [InlineData("ja-JP", OwnText.Settings, "設定…")]
    [InlineData("zh-TW", OwnText.Exit, "結束")]
    [InlineData("pt-BR", OwnText.SameAsTheGame, "Igual ao jogo")]
    [InlineData("ru-RU", OwnText.Playing, "В игре")]
    [InlineData("de-DE", OwnText.PresetAlternating, "Abwechselnd")]
    [InlineData("de-DE", OwnText.StatusAppearance, "Statusanzeige")]
    [InlineData("ja-JP", OwnText.InGame, "ゲーム内")]
    [InlineData("zh-TW", OwnText.PresetMinimal, "極簡")]
    [InlineData("en-US", OwnText.PresetCareer, "Career")]
    [InlineData("de-DE", OwnText.PresetCustom, "Benutzerdefiniert")]
    [InlineData("fr-FR", OwnText.TemplateHoverText, "Info-bulle de l'image")]
    [InlineData("ja-JP", OwnText.PreviewExample, "値の例")]
    [InlineData("ko-KR", OwnText.PrivateHint, "개인 정보: 디스코드 상태를 확인하는 모든 사용자에게 표시됩니다.")]
    [InlineData("zh-TW", OwnText.Placeholders, "佔位符")]
    [InlineData("en-US", OwnText.StoreButton, "Get Transport Fever 3")]
    [InlineData("de-DE", OwnText.StoreButton, "Hol dir Transport Fever 3")]
    [InlineData("ja-JP", OwnText.ShowStoreButton, "Steamストアへのボタンを表示する")]
    [InlineData("ru-RU", OwnText.StoreButtonHint, "Другие видят эту кнопку под твоим статусом; Discord тебе её не показывает.")]
    internal void SpotChecks(string culture, OwnText text, string expected)
        => Assert.Equal(expected, OwnTexts.Get(text, CultureInfo.GetCultureInfo(culture)));

    private static bool IsIdenticalToEnglish(OwnText text, string code)
    {
        for (int i = 0; i < IdenticalToEnglish.Length; i++)
        {
            if ((IdenticalToEnglish[i].Text == text) && string.Equals(IdenticalToEnglish[i].Code, code, StringComparison.Ordinal))
                return true;
        }

        return false;
    }

    /// <summary>
    /// A culture with no translation, here the invariant culture, falls back to the English text.
    /// </summary>
    [Fact]
    public void UnknownCultureFallsBackToEnglish()
        => Assert.Equal("Exit", OwnTexts.Get(OwnText.Exit, CultureInfo.InvariantCulture));
}
