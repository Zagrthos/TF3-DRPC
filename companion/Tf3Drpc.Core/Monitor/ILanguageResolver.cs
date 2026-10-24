using Tf3Drpc.Core.Languages;

namespace Tf3Drpc.Core.Monitor;

/// <summary>
/// Resolves the language and loads the game's texts.
/// </summary>
public interface ILanguageResolver
{
    /// <summary>
    /// Resolves the language setting against the game language.
    /// </summary>
    LanguageInfo Resolve(string setting, string? gameLanguageCode);

    /// <summary>
    /// Reads the game's language code from its <c>profile.lua</c>.
    /// </summary>
    string? ReadGameLanguageCode(string? profilePath);

    /// <summary>
    /// Loads the game's texts for the chosen language and for the game language; without a file they are English. Does nothing if already loaded.
    /// </summary>
    void LoadGameTexts(string? gameDirectory, LanguageInfo language, LanguageInfo gameLanguage);
}
