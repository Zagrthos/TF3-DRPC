using System;
using System.Collections.Generic;
using System.IO;
using System.Security;

using Microsoft.Extensions.Logging;

using Tf3Drpc.App.Constants;
using Tf3Drpc.App.Logging;
using Tf3Drpc.App.Services.Interfaces;
using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Languages;
using Tf3Drpc.Core.Resources;
using Tf3Drpc.Core.Translation;

namespace Tf3Drpc.App.Services;

/// <summary>
/// Loads the game's texts from its <c>.mo</c> files, for the chosen language and for the game's own language, and serves the app's own texts.
/// </summary>
#pragma warning disable CA1812 // Avoid uninstantiated internal classes
internal sealed class TranslationService(TimeProvider timeProvider, ILogger<TranslationService> logger) : ITranslationService
#pragma warning restore CA1812
{
    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly ILogger<TranslationService> _logger = logger;
    private readonly TextsCache _chosenCache = new();
    private readonly TextsCache _gameCache = new();

    public IReadOnlyList<LanguageInfo> Languages
        => LanguageCatalog.All;

    public GameTexts GameTexts { get; private set; } = GameTexts.English;

    public GameTexts GameLanguageTexts { get; private set; } = GameTexts.English;

    public LanguageInfo Resolve(string setting, string? gameLanguageCode)
        => LanguageCatalog.Resolve(setting, gameLanguageCode);

    public string? ReadGameLanguageCode(string? profilePath)
    {
        if (profilePath is null)
            return null;

        try
        {
            FileInfo info = new(profilePath);

            if (!info.Exists || (info.Length is > AppConstants.MaxProfileBytes))
                return null;

            string text = File.ReadAllText(profilePath);

            return (LanguageCatalog.TryReadLanguageCode(text, out string code)) ? code : null;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or SecurityException or ArgumentException)
        {
            return null;
        }
    }

    public void LoadGameTexts(string? gameDirectory, LanguageInfo language, LanguageInfo gameLanguage)
    {
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(gameLanguage);

        GameTexts = Load(_chosenCache, gameDirectory, language);
        GameLanguageTexts = (Equals(gameLanguage, language)) ? GameTexts : Load(_gameCache, gameDirectory, gameLanguage);
    }

    private GameTexts Load(TextsCache cache, string? gameDirectory, LanguageInfo language)
    {
        string key = language.Code + "|" + gameDirectory;

        if (string.Equals(key, cache.LoadedKey, StringComparison.Ordinal))
            return cache.Texts;

        bool retry = string.Equals(key, cache.FailedKey, StringComparison.Ordinal);

        if (retry && (_timeProvider.GetElapsedTime(cache.FailedTimestamp) < AppConstants.TranslationRetryInterval))
            return cache.Texts;

        cache.LoadedKey = null;
        cache.Texts = GameTexts.English;

        if (gameDirectory is null)
        {
            cache.LoadedKey = key;
            cache.FailedKey = null;

            return cache.Texts;
        }

        string path = Path.Combine(gameDirectory, "base", "strings", language.Code, "LC_MESSAGES", "base.mo");

        if (MoReader.TryReadFile(path, out IReadOnlyDictionary<string, string>? entries))
        {
            cache.Texts = new GameTexts(entries);
            cache.LoadedKey = key;
            cache.FailedKey = null;
            _logger.GameTextsLoaded(language.Code, path);

            return cache.Texts;
        }

        cache.FailedKey = key;
        cache.FailedTimestamp = _timeProvider.GetTimestamp();

        if (!retry)
            _logger.GameTextsNotFound(language.Code, gameDirectory);

        return cache.Texts;
    }

    public string GetOwnText(OwnText text, LanguageInfo language)
    {
        ArgumentNullException.ThrowIfNull(language);

        return OwnTexts.Get(text, language.Culture);
    }

    private sealed class TextsCache
    {
        public GameTexts Texts { get; set; } = GameTexts.English;

        public string? LoadedKey { get; set; }

        public string? FailedKey { get; set; }

        public long FailedTimestamp { get; set; }
    }
}
