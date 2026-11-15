using System;
using System.IO;
using System.Threading.Tasks;

using Tf3Drpc.Core.Settings;

using Xunit;

namespace Tf3Drpc.Core.Tests.Settings;

/// <summary>
/// Tests of <see cref="SettingsStore"/> and <see cref="AppSettings"/>, covering saving, loading, defaults and the repair of invalid settings files in a temporary folder.
/// </summary>
public sealed class SettingsStoreTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "tf3drpc-settings-" + Guid.NewGuid().ToString("N"));

    /// <summary>
    /// Creates a unique temporary folder named with the prefix "tf3drpc-settings-" under the system temporary path for the settings files of the test.
    /// </summary>
    public SettingsStoreTests()
        => Directory.CreateDirectory(_directory);

    /// <summary>
    /// Deletes the temporary folder and every file in it.
    /// </summary>
    public void Dispose()
        => Directory.Delete(_directory, recursive: true);

    /// <summary>
    /// A new <see cref="AppSettings"/> uses the game language ("game") and the career preset ("career").
    /// </summary>
    [Fact]
    public void DefaultsUseTheGameLanguageAndTheCareerPreset()
    {
        Assert.Equal("game", new AppSettings().Language);
        Assert.Equal("career", new AppSettings().Preset);
    }

    /// <summary>
    /// Saving a settings object with the language "pt_BR" and loading the file gives back the same language.
    /// </summary>
    [Fact]
    public void RoundTripKeepsTheLanguage()
    {
        string path = Path.Combine(_directory, "settings.json");

        bool saved = SettingsStore.TrySave(path, new AppSettings() { Language = "pt_BR" });
        AppSettings loaded = SettingsStore.Load(path);

        Assert.True(saved);
        Assert.Equal("pt_BR", loaded.Language);
    }

    /// <summary>
    /// Saving and loading keeps each preset (career, network, era, alternating, minimal and custom) together with the chosen language.
    /// </summary>
    [Theory]
    [InlineData("career")]
    [InlineData("network")]
    [InlineData("era")]
    [InlineData("alternating")]
    [InlineData("minimal")]
    [InlineData("custom")]
    public void RoundTripKeepsThePreset(string preset)
    {
        string path = Path.Combine(_directory, "settings.json");

        bool saved = SettingsStore.TrySave(path, new AppSettings() { Language = "de", Preset = preset });
        AppSettings loaded = SettingsStore.Load(path);

        Assert.True(saved);
        Assert.Equal(preset, loaded.Preset);
        Assert.Equal("de", loaded.Language);
    }

    /// <summary>
    /// Saving writes indented multi-line JSON with the lower-case keys "language" and "preset".
    /// </summary>
    [Fact]
    public void SaveWritesIndentedJsonWithTheLowerCaseKey()
    {
        string path = Path.Combine(_directory, "settings.json");

        SettingsStore.TrySave(path, new AppSettings() { Language = "de" });
        string text = File.ReadAllText(path);

        Assert.Contains("\"language\": \"de\"", text, StringComparison.Ordinal);
        Assert.Contains("\"preset\": \"career\"", text, StringComparison.Ordinal);
        Assert.Contains('\n', text);
    }

    /// <summary>
    /// Saving twice to the same path leaves no ".tmp" file, replaces the earlier content with the later one and leaves only the settings file in the folder.
    /// </summary>
    [Fact]
    public void SaveLeavesNoTemporaryFileAndReplacesAnExistingFile()
    {
        string path = Path.Combine(_directory, "settings.json");

        SettingsStore.TrySave(path, new AppSettings() { Language = "de" });
        SettingsStore.TrySave(path, new AppSettings() { Language = "fr" });

        Assert.False(File.Exists(path + ".tmp"));
        Assert.Equal("fr", SettingsStore.Load(path).Language);
        Assert.Single(Directory.GetFiles(_directory));
    }

    /// <summary>
    /// Saving to a path whose folder does not exist yet creates the folder and the file.
    /// </summary>
    [Fact]
    public void SaveCreatesAMissingFolder()
    {
        string path = Path.Combine(_directory, "DATA", "settings.json");

        Assert.True(SettingsStore.TrySave(path, new AppSettings()));
        Assert.True(File.Exists(path));
    }

    /// <summary>
    /// Saving to a path below a regular file returns false instead of throwing.
    /// </summary>
    [Fact]
    public void SaveInAnUnwritableLocationFailsWithoutThrowing()
    {
        string blocker = Path.Combine(_directory, "blocker");
        File.WriteAllText(blocker, "a file, not a folder");

        bool saved = SettingsStore.TrySave(Path.Combine(blocker, "settings.json"), new AppSettings());

        Assert.False(saved);
    }

    /// <summary>
    /// Saving over a file that another stream holds open returns false without throwing and leaves no ".tmp" file; the test runs only on Windows, because only Windows enforces the file share.
    /// </summary>
    [Fact]
    public void SaveOverALockedFileFailsWithoutThrowing()
    {
        Assert.SkipUnless(OperatingSystem.IsWindows(), "Only Windows enforces FileShare; elsewhere the save replaces the open file.");
        string path = Path.Combine(_directory, "settings.json");
        SettingsStore.TrySave(path, new AppSettings() { Language = "de" });
        using FileStream held = new(path, FileMode.Open, FileAccess.Read, FileShare.Read);

        bool saved = SettingsStore.TrySave(path, new AppSettings() { Language = "fr" });

        Assert.False(saved);
        Assert.False(File.Exists(path + ".tmp"));
    }

    /// <summary>
    /// Loading from a path where no file exists gives the default settings.
    /// </summary>
    [Fact]
    public void MissingFileGivesDefaults()
        => Assert.Equal("game", SettingsStore.Load(Path.Combine(_directory, "nothing.json")).Language);

    /// <summary>
    /// Loading content that is empty, blank, truncated, not JSON, JSON null or an array, or that has a non-string, null, unknown or wrong-case language gives the default settings.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("{")]
    [InlineData("not json at all")]
    [InlineData("null")]
    [InlineData("[]")]
    [InlineData("{\"language\": 5}")]
    [InlineData("{\"language\": null}")]
    [InlineData("{\"language\": \"klingon\"}")]
    [InlineData("{\"language\": \"DE\"}")]
    [InlineData("{}")]
    public async Task BrokenOrUnknownContentGivesDefaultsAsync(string content)
    {
        string path = Path.Combine(_directory, "settings.json");
        await File.WriteAllTextAsync(path, content, TestContext.Current.CancellationToken);

        AppSettings loaded = SettingsStore.Load(path);

        Assert.Equal("game", loaded.Language);
    }

    /// <summary>
    /// Keys that the store does not know, such as "theme" in an old file, are ignored while the known keys are still read.
    /// </summary>
    [Fact]
    public async Task UnknownKeysAreIgnoredAsync()
    {
        string path = Path.Combine(_directory, "settings.json");
        await File.WriteAllTextAsync(path, "{\"language\": \"ko\", \"theme\": \"dark\"}", TestContext.Current.CancellationToken);

        Assert.Equal("ko", SettingsStore.Load(path).Language);
    }

    /// <summary>
    /// A preset that is unknown, wrong-case, empty, null or absent becomes career, while the language of the file is kept.
    /// </summary>
    [Theory]
    [InlineData("{\"language\": \"ko\", \"preset\": \"B\"}")]
    [InlineData("{\"language\": \"ko\", \"preset\": \"Network\"}")]
    [InlineData("{\"language\": \"ko\", \"preset\": \"\"}")]
    [InlineData("{\"language\": \"ko\", \"preset\": null}")]
    [InlineData("{\"language\": \"ko\"}")]
    public async Task UnknownOrMissingPresetBecomesCareerAndKeepsTheLanguageAsync(string content)
    {
        string path = Path.Combine(_directory, "settings.json");
        await File.WriteAllTextAsync(path, content, TestContext.Current.CancellationToken);

        AppSettings loaded = SettingsStore.Load(path);

        Assert.Equal("career", loaded.Preset);
        Assert.Equal("ko", loaded.Language);
    }

    /// <summary>
    /// An unknown language becomes the default "game" while a valid preset in the same file is kept.
    /// </summary>
    [Fact]
    public async Task InvalidLanguageBecomesTheDefaultAndKeepsTheValidPresetAsync()
    {
        string path = Path.Combine(_directory, "settings.json");
        await File.WriteAllTextAsync(path, "{\"language\": \"klingon\", \"preset\": \"era\"}", TestContext.Current.CancellationToken);

        AppSettings loaded = SettingsStore.Load(path);

        Assert.Equal("game", loaded.Language);
        Assert.Equal("era", loaded.Preset);
    }

    /// <summary>
    /// Saving an unknown preset stores career instead, and the language is kept.
    /// </summary>
    [Fact]
    public void SaveNormalisesAnUnknownPresetAndKeepsTheLanguage()
    {
        string path = Path.Combine(_directory, "settings.json");

        SettingsStore.TrySave(path, new AppSettings() { Language = "fr", Preset = "xx" });
        AppSettings loaded = SettingsStore.Load(path);

        Assert.Equal("fr", loaded.Language);
        Assert.Equal("career", loaded.Preset);
    }

    /// <summary>
    /// Saving an unknown language stores the default "game" instead.
    /// </summary>
    [Fact]
    public void SaveNormalisesAnUnknownLanguage()
    {
        string path = Path.Combine(_directory, "settings.json");

        SettingsStore.TrySave(path, new AppSettings() { Language = "xx" });

        Assert.Equal("game", SettingsStore.Load(path).Language);
    }

    /// <summary>
    /// A new <see cref="AppSettings"/> carries the default presence templates for details, state and hover text.
    /// </summary>
    [Fact]
    public void DefaultsHaveTheDefaultTemplates()
    {
        PresenceTemplates templates = new AppSettings().Templates;

        Assert.Equal("{rank} · {mode}", templates.Details);
        Assert.Equal("{date}", templates.State);
        Assert.Equal("Transport Fever 3", templates.Hover);
    }

    /// <summary>
    /// Saving and loading keeps custom templates, including quotes and an emoji, together with the preset, and writes them as a "templates" object.
    /// </summary>
    [Fact]
    public void RoundTripKeepsTheTemplates()
    {
        string path = Path.Combine(_directory, "settings.json");
        PresenceTemplates templates = new() { Details = "{saveName} \"quoted\" 🚂", State = "{balance}", Hover = "Hover" };

        SettingsStore.TrySave(path, new AppSettings() { Preset = "custom", Templates = templates });
        AppSettings loaded = SettingsStore.Load(path);

        Assert.Equal(templates, loaded.Templates);
        Assert.Equal("custom", loaded.Preset);
        Assert.Contains("\"templates\": {", File.ReadAllText(path), StringComparison.Ordinal);
    }

    /// <summary>
    /// Templates that are absent, null or an empty object give the default templates, while the language is kept.
    /// </summary>
    [Theory]
    [InlineData("{\"language\": \"ko\"}")]
    [InlineData("{\"language\": \"ko\", \"templates\": null}")]
    [InlineData("{\"language\": \"ko\", \"templates\": {}}")]
    public async Task MissingTemplatesGiveTheDefaultsAsync(string content)
    {
        string path = Path.Combine(_directory, "settings.json");
        await File.WriteAllTextAsync(path, content, TestContext.Current.CancellationToken);

        AppSettings loaded = SettingsStore.Load(path);

        Assert.Equal(new PresenceTemplates(), loaded.Templates);
        Assert.Equal("ko", loaded.Language);
    }

    /// <summary>
    /// A template field that is absent or null keeps its default, while an empty string is kept as it is.
    /// </summary>
    [Fact]
    public async Task MissingOrNullFieldsKeepTheirDefaultAndEmptyStringsAreKeptAsync()
    {
        string path = Path.Combine(_directory, "settings.json");
        await File.WriteAllTextAsync(path, "{\"templates\": {\"details\": \"\", \"state\": null}}", TestContext.Current.CancellationToken);

        PresenceTemplates loaded = SettingsStore.Load(path).Templates;

        Assert.Equal(string.Empty, loaded.Details);
        Assert.Equal("{date}", loaded.State);
        Assert.Equal("Transport Fever 3", loaded.Hover);
    }

    /// <summary>
    /// Saving cuts every template field longer than 512 characters to 512 characters, and shorter or exactly 512-character fields stay unchanged.
    /// </summary>
    [Fact]
    public void LongTemplatesAreCutAt512()
    {
        string path = Path.Combine(_directory, "settings.json");
        string longText = new('a', 600);

        SettingsStore.TrySave(path, new AppSettings() { Templates = new PresenceTemplates() { Details = longText, State = new string('b', 512), Hover = "short" } });
        PresenceTemplates loaded = SettingsStore.Load(path).Templates;

        Assert.Equal(new string('a', 512), loaded.Details);
        Assert.Equal(new string('b', 512), loaded.State);
        Assert.Equal("short", loaded.Hover);
    }

    /// <summary>
    /// Loading a file cuts templates at 512 characters without splitting a surrogate pair, so an emoji straddling the limit is dropped, while one that fits is kept.
    /// </summary>
    [Fact]
    public async Task LongTemplatesInTheFileAreCutWithoutSplittingASurrogatePairAsync()
    {
        string path = Path.Combine(_directory, "settings.json");
        string split = new string('a', 511) + "😀tail";
        string whole = new string('a', 510) + "😀tail";
        await File.WriteAllTextAsync(path, $"{{\"templates\": {{\"details\": \"{split}\", \"state\": \"{whole}\"}}}}", TestContext.Current.CancellationToken);

        PresenceTemplates loaded = SettingsStore.Load(path).Templates;

        Assert.Equal(new string('a', 511), loaded.Details);
        Assert.Equal(new string('a', 510) + "😀", loaded.State);
    }

    /// <summary>
    /// A templates value that is a number, a string, an array or a boolean gives the default templates, while the language and preset are kept.
    /// </summary>
    [Theory]
    [InlineData("5")]
    [InlineData("\"text\"")]
    [InlineData("[1, {\"details\": \"x\"}]")]
    [InlineData("true")]
    public async Task InvalidTemplatesKeepTheLanguageAndPresetAsync(string templates)
    {
        string path = Path.Combine(_directory, "settings.json");
        await File.WriteAllTextAsync(path, $"{{\"language\": \"ko\", \"preset\": \"era\", \"templates\": {templates}}}", TestContext.Current.CancellationToken);

        AppSettings loaded = SettingsStore.Load(path);

        Assert.Equal("ko", loaded.Language);
        Assert.Equal("era", loaded.Preset);
        Assert.Equal(new PresenceTemplates(), loaded.Templates);
    }

    /// <summary>
    /// A template field with the wrong type falls back to its default while the other template fields and the language are kept.
    /// </summary>
    [Fact]
    public async Task InvalidTemplateFieldKeepsTheOtherFieldsAsync()
    {
        string path = Path.Combine(_directory, "settings.json");
        await File.WriteAllTextAsync(path, "{\"language\": \"ko\", \"templates\": {\"details\": 5, \"state\": \"S1\", \"hover\": {\"a\": [1]}}}", TestContext.Current.CancellationToken);

        AppSettings loaded = SettingsStore.Load(path);

        Assert.Equal("ko", loaded.Language);
        Assert.Equal("{rank} · {mode}", loaded.Templates.Details);
        Assert.Equal("S1", loaded.Templates.State);
        Assert.Equal("Transport Fever 3", loaded.Templates.Hover);
    }

    /// <summary>
    /// A new <see cref="AppSettings"/> has the store button switched off.
    /// </summary>
    [Fact]
    public void StoreButtonIsOffByDefault()
        => Assert.False(new AppSettings().StoreButton);

    /// <summary>
    /// A file without the store button key loads with the store button switched off.
    /// </summary>
    [Fact]
    public async Task MissingStoreButtonIsOffAsync()
    {
        string path = Path.Combine(_directory, "settings.json");
        await File.WriteAllTextAsync(path, "{\"language\": \"ko\"}", TestContext.Current.CancellationToken);

        Assert.False(SettingsStore.Load(path).StoreButton);
    }

    /// <summary>
    /// Saving and loading keeps the store button value, true or false, together with the language and writes the "storeButton" key.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void StoreButtonRoundTrips(bool enabled)
    {
        string path = Path.Combine(_directory, "settings.json");

        SettingsStore.TrySave(path, new AppSettings() { Language = "de", StoreButton = enabled });
        AppSettings loaded = SettingsStore.Load(path);

        Assert.Equal(enabled, loaded.StoreButton);
        Assert.Equal("de", loaded.Language);
        Assert.Contains("\"storeButton\": " + ((enabled) ? "true" : "false"), File.ReadAllText(path), StringComparison.Ordinal);
    }

    /// <summary>
    /// A store button value that is a string, a number, null, an object or an array loads as off, while the language, preset and templates are kept.
    /// </summary>
    [Theory]
    [InlineData("\"yes\"")]
    [InlineData("\"true\"")]
    [InlineData("1")]
    [InlineData("0")]
    [InlineData("null")]
    [InlineData("{\"on\": true}")]
    [InlineData("[true]")]
    public async Task WrongStoreButtonTypeIsOffAndKeepsTheOtherSettingsAsync(string value)
    {
        string path = Path.Combine(_directory, "settings.json");
        await File.WriteAllTextAsync(path, $"{{\"language\": \"ko\", \"storeButton\": {value}, \"preset\": \"custom\", \"templates\": {{\"details\": \"x\"}}}}", TestContext.Current.CancellationToken);

        AppSettings loaded = SettingsStore.Load(path);

        Assert.False(loaded.StoreButton);
        Assert.Equal("ko", loaded.Language);
        Assert.Equal("custom", loaded.Preset);
        Assert.Equal("x", loaded.Templates.Details);
    }

    /// <summary>
    /// A preset of the wrong type becomes career and costs nothing else, so the language, templates and store button are kept.
    /// </summary>
    [Fact]
    public async Task WrongPresetTypeCostsOnlyThePresetAsync()
    {
        string path = Path.Combine(_directory, "settings.json");
        await File.WriteAllTextAsync(path, "{\"language\":\"fr\",\"preset\":0,\"templates\":{\"details\":\"KEEP ME\"},\"storeButton\":true}", TestContext.Current.CancellationToken);

        AppSettings loaded = SettingsStore.Load(path);

        Assert.Equal("fr", loaded.Language);
        Assert.Equal("career", loaded.Preset);
        Assert.Equal("KEEP ME", loaded.Templates.Details);
        Assert.True(loaded.StoreButton);
    }

    /// <summary>
    /// A language of the wrong type becomes the default "game" and costs nothing else, so the preset, templates and store button are kept.
    /// </summary>
    [Fact]
    public async Task WrongLanguageTypeCostsOnlyTheLanguageAsync()
    {
        string path = Path.Combine(_directory, "settings.json");
        await File.WriteAllTextAsync(path, "{\"language\":{\"code\":\"fr\"},\"preset\":\"era\",\"templates\":{\"details\":\"KEEP ME\"},\"storeButton\":true}", TestContext.Current.CancellationToken);

        AppSettings loaded = SettingsStore.Load(path);

        Assert.Equal("game", loaded.Language);
        Assert.Equal("era", loaded.Preset);
        Assert.Equal("KEEP ME", loaded.Templates.Details);
        Assert.True(loaded.StoreButton);
    }

    /// <summary>
    /// A language and a preset that both have a non-string type (number, boolean, null, array or object) become the defaults, while the templates and the store button are kept.
    /// </summary>
    [Theory]
    [InlineData("0")]
    [InlineData("1.5")]
    [InlineData("true")]
    [InlineData("false")]
    [InlineData("null")]
    [InlineData("[\"fr\"]")]
    [InlineData("{\"a\":[1,{\"b\":2}]}")]
    public async Task WrongLanguageAndPresetTypesGiveTheDefaultsAndKeepTheOtherSettingsAsync(string value)
    {
        string path = Path.Combine(_directory, "settings.json");
        await File.WriteAllTextAsync(path, $"{{\"language\":{value},\"preset\":{value},\"templates\":{{\"state\":\"KEEP ME\"}},\"storeButton\":true}}", TestContext.Current.CancellationToken);

        AppSettings loaded = SettingsStore.Load(path);

        Assert.Equal("game", loaded.Language);
        Assert.Equal("career", loaded.Preset);
        Assert.Equal("KEEP ME", loaded.Templates.State);
        Assert.True(loaded.StoreButton);
    }

    /// <summary>
    /// A store button that is true in the file loads as true, together with the preset.
    /// </summary>
    [Fact]
    public async Task StoreButtonTrueInTheFileIsReadAsync()
    {
        string path = Path.Combine(_directory, "settings.json");
        await File.WriteAllTextAsync(path, "{\"storeButton\": true, \"preset\": \"era\"}", TestContext.Current.CancellationToken);

        AppSettings loaded = SettingsStore.Load(path);

        Assert.True(loaded.StoreButton);
        Assert.Equal("era", loaded.Preset);
    }

    /// <summary>
    /// A new <see cref="AppSettings"/> follows the system theme ("system").
    /// </summary>
    [Fact]
    public void DefaultsFollowTheSystemTheme()
        => Assert.Equal("system", new AppSettings().Theme);

    /// <summary>
    /// Saving and loading keeps each theme value (system, light and dark).
    /// </summary>
    [Theory]
    [InlineData("system")]
    [InlineData("light")]
    [InlineData("dark")]
    public void RoundTripKeepsTheTheme(string theme)
    {
        string path = Path.Combine(_directory, "settings.json");

        bool saved = SettingsStore.TrySave(path, new AppSettings() { Theme = theme });
        AppSettings loaded = SettingsStore.Load(path);

        Assert.True(saved);
        Assert.Equal(theme, loaded.Theme);
    }

    /// <summary>
    /// Saving writes the "theme" key with its value.
    /// </summary>
    [Fact]
    public void SaveWritesTheThemeKey()
    {
        string path = Path.Combine(_directory, "settings.json");

        SettingsStore.TrySave(path, new AppSettings() { Theme = "dark" });

        Assert.Contains("\"theme\": \"dark\"", File.ReadAllText(path), StringComparison.Ordinal);
    }

    /// <summary>
    /// A theme that is unknown, wrong-case, empty, a number or null becomes "system", while the language, preset and store button are kept.
    /// </summary>
    [Theory]
    [InlineData("\"auto\"")]
    [InlineData("\"Dark\"")]
    [InlineData("\"\"")]
    [InlineData("0")]
    [InlineData("null")]
    public async Task UnknownOrWrongTypeThemeBecomesSystemAndKeepsTheOtherSettingsAsync(string value)
    {
        string path = Path.Combine(_directory, "settings.json");
        await File.WriteAllTextAsync(path, $"{{\"language\":\"fr\",\"preset\":\"era\",\"theme\":{value},\"storeButton\":true}}", TestContext.Current.CancellationToken);

        AppSettings loaded = SettingsStore.Load(path);

        Assert.Equal("system", loaded.Theme);
        Assert.Equal("fr", loaded.Language);
        Assert.Equal("era", loaded.Preset);
        Assert.True(loaded.StoreButton);
    }

    /// <summary>
    /// A file without a theme key loads with the "system" theme, while the language is kept.
    /// </summary>
    [Fact]
    public async Task MissingThemeBecomesSystemAsync()
    {
        string path = Path.Combine(_directory, "settings.json");
        await File.WriteAllTextAsync(path, "{\"language\": \"ko\", \"preset\": \"era\"}", TestContext.Current.CancellationToken);

        AppSettings loaded = SettingsStore.Load(path);

        Assert.Equal("system", loaded.Theme);
        Assert.Equal("ko", loaded.Language);
    }

    /// <summary>
    /// A valid theme is kept next to an invalid language, which itself becomes the default "game".
    /// </summary>
    [Fact]
    public async Task ValidThemeNextToAnInvalidLanguageIsKeptAsync()
    {
        string path = Path.Combine(_directory, "settings.json");
        await File.WriteAllTextAsync(path, "{\"language\": \"klingon\", \"theme\": \"light\"}", TestContext.Current.CancellationToken);

        AppSettings loaded = SettingsStore.Load(path);

        Assert.Equal("game", loaded.Language);
        Assert.Equal("light", loaded.Theme);
    }

    /// <summary>
    /// Normalising templates that are already valid returns the same instance without copying it.
    /// </summary>
    [Fact]
    public void NormalizeTemplatesReturnsValidTemplatesUnchanged()
    {
        PresenceTemplates templates = new() { Details = "x" };

        Assert.Same(templates, SettingsStore.NormalizeTemplates(templates));
    }

    /// <summary>
    /// Loading from the path of an existing folder instead of a file gives the default settings.
    /// </summary>
    [Fact]
    public void LoadOfAFolderPathGivesDefaults()
        => Assert.Equal("game", SettingsStore.Load(_directory).Language);
}
