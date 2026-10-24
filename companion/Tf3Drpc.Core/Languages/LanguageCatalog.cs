using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

using Tf3Drpc.Core.Constants;
using Tf3Drpc.Core.Utilities;

namespace Tf3Drpc.Core.Languages;

/// <summary>
/// The 13 languages of the game and the resolution of the language setting.
/// </summary>
public static class LanguageCatalog
{
    private static readonly LanguageInfo[] Languages =
    [
        new("de", "Deutsch", CultureInfo.GetCultureInfo("de-DE")),
        new("en", "English", CultureInfo.GetCultureInfo("en-US")),
        new("es", "Español", CultureInfo.GetCultureInfo("es-ES")),
        new("fr", "Français", CultureInfo.GetCultureInfo("fr-FR")),
        new("it", "Italiano", CultureInfo.GetCultureInfo("it-IT")),
        new("ja", "日本語", CultureInfo.GetCultureInfo("ja-JP")),
        new("ko", "한국어", CultureInfo.GetCultureInfo("ko-KR")),
        new("nl", "Nederlands", CultureInfo.GetCultureInfo("nl-NL")),
        new("pl", "Polski", CultureInfo.GetCultureInfo("pl-PL")),
        new("pt_BR", "Português (Brasil)", CultureInfo.GetCultureInfo("pt-BR")),
        new("ru", "Русский", CultureInfo.GetCultureInfo("ru-RU")),
        new("zh_CN", "简体中文", CultureInfo.GetCultureInfo("zh-CN")),
        new("zh_TW", "繁體中文", CultureInfo.GetCultureInfo("zh-TW"))
    ];

    /// <summary>
    /// All languages in display order.
    /// </summary>
    public static IReadOnlyList<LanguageInfo> All
        => Languages;

    /// <summary>
    /// Finds a language by its code (ordinal, case-sensitive).
    /// </summary>
    public static bool TryGet(string? code, out LanguageInfo language)
    {
        for (int i = 0; i < Languages.Length; i++)
        {
            if (string.Equals(Languages[i].Code, code, StringComparison.Ordinal))
            {
                language = Languages[i];

                return true;
            }
        }

        language = Languages[1];

        return false;
    }

    /// <summary>
    /// Tells whether a settings value is <c>game</c> or a known language code.
    /// </summary>
    public static bool IsValidSetting(string? setting)
        => string.Equals(setting, LanguageConstants.GameSetting, StringComparison.Ordinal) || TryGet(setting, out LanguageInfo _);

    /// <summary>
    /// Resolves the language setting; <c>game</c> uses the game language, anything unknown gives English.
    /// </summary>
    public static LanguageInfo Resolve(string? setting, string? gameLanguageCode)
    {
        string? code = (string.Equals(setting, LanguageConstants.GameSetting, StringComparison.Ordinal)) ? gameLanguageCode : setting;
        _ = TryGet(code, out LanguageInfo language);

        return language;
    }

    /// <summary>
    /// Reads <c>language.code</c> from the text of the game's <c>profile.lua</c>.
    /// </summary>
    public static bool TryReadLanguageCode(string profileText, out string code)
    {
        ArgumentNullException.ThrowIfNull(profileText);

        Match? match = RegexUtils.MatchLanguageCode(LuaUtils.StripComments(profileText));
        code = match?.Groups["code"].Value ?? string.Empty;

        return match is not null;
    }
}
