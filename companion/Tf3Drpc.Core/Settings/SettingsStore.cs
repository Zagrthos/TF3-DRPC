using System;
using System.IO;
using System.Security;
using System.Text.Json;
using System.Text.Json.Serialization;

using Tf3Drpc.Core.Constants;
using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Extensions;
using Tf3Drpc.Core.Languages;
using Tf3Drpc.Core.Serialization;
using Tf3Drpc.Core.Utilities;

namespace Tf3Drpc.Core.Settings;

/// <summary>
/// Loads and saves the settings file.
/// </summary>
public static class SettingsStore
{
    private static readonly JsonSourceGenContext IndentedContext = new(new JsonSerializerOptions() { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });

    /// <summary>
    /// Loads the settings; a missing, empty, broken or unreadable file gives the defaults.
    /// </summary>
    public static AppSettings Load(string path)
    {
        try
        {
            FileInfo info = new(path);

            if (!info.Exists || (info.Length is 0) || (info.Length is > SettingsConstants.MaxFileBytes))
                return new AppSettings();

            string json = File.ReadAllText(path);
            AppSettings? settings = JsonSerializer.Deserialize(json, JsonSourceGenContext.Default.AppSettings);

            return Normalize(settings);
        }
        catch (Exception exception) when (IsExpected(exception))
        {
            return new AppSettings();
        }
    }

    /// <summary>
    /// Saves the settings through a temporary file; <see langword="false"/> means the file could not be written.
    /// </summary>
    public static bool TrySave(string path, AppSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        string temporaryPath = path + SettingsConstants.TemporaryExtension;

        try
        {
            string? directory = Path.GetDirectoryName(Path.GetFullPath(path));

            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            string json = JsonSerializer.Serialize(Normalize(settings), IndentedContext.AppSettings);
            File.WriteAllText(temporaryPath, json);
            File.Move(temporaryPath, path, overwrite: true);

            return true;
        }
        catch (Exception exception) when (IsExpected(exception))
        {
            TryDelete(temporaryPath);

            return false;
        }
    }

    private static AppSettings Normalize(AppSettings? settings)
    {
        if (settings is null)
            return new AppSettings();

        bool languageValid = LanguageCatalog.IsValidSetting(settings.Language);
        bool presetValid = PresencePreset.TryParseSetting(settings.Preset, out _);
        bool themeValid = ThemeMode.TryParseSetting(settings.Theme, out _);
        PresenceTemplates templates = NormalizeTemplates(settings.Templates);
        bool templatesValid = ReferenceEquals(templates, settings.Templates);

        if (languageValid && presetValid && themeValid && templatesValid)
            return settings;

        return settings with
        {
            Language = (languageValid) ? settings.Language : LanguageConstants.GameSetting,
            Preset = (presetValid) ? settings.Preset : SettingsConstants.DefaultPreset,
            Theme = (themeValid) ? settings.Theme : SettingsConstants.DefaultTheme,
            Templates = templates
        };
    }

    /// <summary>
    /// Returns the templates themselves when they are valid; otherwise a copy with defaults for missing fields and cut texts.
    /// </summary>
    public static PresenceTemplates NormalizeTemplates(PresenceTemplates? templates)
    {
        if (templates is null)
            return new PresenceTemplates();

        // A null field comes from JSON null; the converter keeps the defaults for other wrong types.
        string details = CutTemplate(templates.Details) ?? PresenceConstants.DefaultDetailsTemplate;
        string state = CutTemplate(templates.State) ?? PresenceConstants.DefaultStateTemplate;
        string hover = CutTemplate(templates.Hover) ?? DiscordConstants.LargeImageText;

        if (string.Equals(details, templates.Details, StringComparison.Ordinal) &&
            string.Equals(state, templates.State, StringComparison.Ordinal) &&
            string.Equals(hover, templates.Hover, StringComparison.Ordinal))
        {
            return templates;
        }

        return new PresenceTemplates() { Details = details, State = state, Hover = hover };
    }

    private static string? CutTemplate(string? template)
        => (template is null) ? null : TextUtils.CutToLength(template, PresenceConstants.MaxTemplateLength);

    private static void TryDelete(string path)
    {
        try
        {
            File.Delete(path);
        }
        catch (Exception exception) when (IsExpected(exception))
        {
            // The temporary file stays behind; the next save overwrites it.
        }
    }

    private static bool IsExpected(Exception exception)
        => exception is IOException or UnauthorizedAccessException or SecurityException or JsonException or ArgumentException or NotSupportedException;
}
