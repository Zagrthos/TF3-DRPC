using System;
using System.IO;
using System.Threading.Tasks;

using Tf3Drpc.Core.Settings;

using Xunit;

namespace Tf3Drpc.Core.Tests.Settings;

public sealed class SettingsStoreTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "tf3drpc-settings-" + Guid.NewGuid().ToString("N"));

    public SettingsStoreTests()
        => Directory.CreateDirectory(_directory);

    public void Dispose()
        => Directory.Delete(_directory, recursive: true);

    [Fact]
    public void Defaults_UseTheGameLanguageAndTheCareerPreset()
    {
        Assert.Equal("game", new AppSettings().Language);
        Assert.Equal("career", new AppSettings().Preset);
    }

    [Fact]
    public void RoundTrip_KeepsTheLanguage()
    {
        string path = Path.Combine(_directory, "settings.json");

        bool saved = SettingsStore.TrySave(path, new AppSettings() { Language = "pt_BR" });
        AppSettings loaded = SettingsStore.Load(path);

        Assert.True(saved);
        Assert.Equal("pt_BR", loaded.Language);
    }

    [Theory]
    [InlineData("career")]
    [InlineData("network")]
    [InlineData("era")]
    [InlineData("alternating")]
    [InlineData("minimal")]
    [InlineData("custom")]
    public void RoundTrip_KeepsThePreset(string preset)
    {
        string path = Path.Combine(_directory, "settings.json");

        bool saved = SettingsStore.TrySave(path, new AppSettings() { Language = "de", Preset = preset });
        AppSettings loaded = SettingsStore.Load(path);

        Assert.True(saved);
        Assert.Equal(preset, loaded.Preset);
        Assert.Equal("de", loaded.Language);
    }

    [Fact]
    public void Save_WritesIndentedJsonWithTheLowerCaseKey()
    {
        string path = Path.Combine(_directory, "settings.json");

        SettingsStore.TrySave(path, new AppSettings() { Language = "de" });
        string text = File.ReadAllText(path);

        Assert.Contains("\"language\": \"de\"", text, StringComparison.Ordinal);
        Assert.Contains("\"preset\": \"career\"", text, StringComparison.Ordinal);
        Assert.Contains('\n', text);
    }

    [Fact]
    public void Save_LeavesNoTemporaryFileAndReplacesAnExistingFile()
    {
        string path = Path.Combine(_directory, "settings.json");

        SettingsStore.TrySave(path, new AppSettings() { Language = "de" });
        SettingsStore.TrySave(path, new AppSettings() { Language = "fr" });

        Assert.False(File.Exists(path + ".tmp"));
        Assert.Equal("fr", SettingsStore.Load(path).Language);
        Assert.Single(Directory.GetFiles(_directory));
    }

    [Fact]
    public void Save_CreatesAMissingFolder()
    {
        string path = Path.Combine(_directory, "DATA", "settings.json");

        Assert.True(SettingsStore.TrySave(path, new AppSettings()));
        Assert.True(File.Exists(path));
    }

    [Fact]
    public void Save_InAnUnwritableLocationFailsWithoutThrowing()
    {
        string blocker = Path.Combine(_directory, "blocker");
        File.WriteAllText(blocker, "a file, not a folder");

        bool saved = SettingsStore.TrySave(Path.Combine(blocker, "settings.json"), new AppSettings());

        Assert.False(saved);
    }

    [Fact]
    public void Save_OverALockedFileFailsWithoutThrowing()
    {
        Assert.SkipUnless(OperatingSystem.IsWindows(), "Only Windows enforces FileShare; elsewhere the save replaces the open file.");
        string path = Path.Combine(_directory, "settings.json");
        SettingsStore.TrySave(path, new AppSettings() { Language = "de" });
        using FileStream held = new(path, FileMode.Open, FileAccess.Read, FileShare.Read);

        bool saved = SettingsStore.TrySave(path, new AppSettings() { Language = "fr" });

        Assert.False(saved);
        Assert.False(File.Exists(path + ".tmp"));
    }

    [Fact]
    public void MissingFile_GivesDefaults()
        => Assert.Equal("game", SettingsStore.Load(Path.Combine(_directory, "nothing.json")).Language);

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
    public async Task BrokenOrUnknownContent_GivesDefaultsAsync(string content)
    {
        string path = Path.Combine(_directory, "settings.json");
        await File.WriteAllTextAsync(path, content, TestContext.Current.CancellationToken);

        AppSettings loaded = SettingsStore.Load(path);

        Assert.Equal("game", loaded.Language);
    }

    [Fact]
    public async Task UnknownKeys_AreIgnoredAsync()
    {
        string path = Path.Combine(_directory, "settings.json");
        await File.WriteAllTextAsync(path, "{\"language\": \"ko\", \"theme\": \"dark\"}", TestContext.Current.CancellationToken);

        Assert.Equal("ko", SettingsStore.Load(path).Language);
    }

    [Theory]
    [InlineData("{\"language\": \"ko\", \"preset\": \"B\"}")]
    [InlineData("{\"language\": \"ko\", \"preset\": \"Network\"}")]
    [InlineData("{\"language\": \"ko\", \"preset\": \"\"}")]
    [InlineData("{\"language\": \"ko\", \"preset\": null}")]
    [InlineData("{\"language\": \"ko\"}")]
    public async Task UnknownOrMissingPreset_BecomesCareerAndKeepsTheLanguageAsync(string content)
    {
        string path = Path.Combine(_directory, "settings.json");
        await File.WriteAllTextAsync(path, content, TestContext.Current.CancellationToken);

        AppSettings loaded = SettingsStore.Load(path);

        Assert.Equal("career", loaded.Preset);
        Assert.Equal("ko", loaded.Language);
    }

    [Fact]
    public async Task InvalidLanguage_BecomesTheDefaultAndKeepsTheValidPresetAsync()
    {
        string path = Path.Combine(_directory, "settings.json");
        await File.WriteAllTextAsync(path, "{\"language\": \"klingon\", \"preset\": \"era\"}", TestContext.Current.CancellationToken);

        AppSettings loaded = SettingsStore.Load(path);

        Assert.Equal("game", loaded.Language);
        Assert.Equal("era", loaded.Preset);
    }

    [Fact]
    public void Save_NormalisesAnUnknownPresetAndKeepsTheLanguage()
    {
        string path = Path.Combine(_directory, "settings.json");

        SettingsStore.TrySave(path, new AppSettings() { Language = "fr", Preset = "xx" });
        AppSettings loaded = SettingsStore.Load(path);

        Assert.Equal("fr", loaded.Language);
        Assert.Equal("career", loaded.Preset);
    }

    [Fact]
    public void Save_NormalisesAnUnknownLanguage()
    {
        string path = Path.Combine(_directory, "settings.json");

        SettingsStore.TrySave(path, new AppSettings() { Language = "xx" });

        Assert.Equal("game", SettingsStore.Load(path).Language);
    }

    [Fact]
    public void Defaults_HaveTheDefaultTemplates()
    {
        PresenceTemplates templates = new AppSettings().Templates;

        Assert.Equal("{rank} · {mode}", templates.Details);
        Assert.Equal("{date}", templates.State);
        Assert.Equal("Transport Fever 3", templates.Hover);
    }

    [Fact]
    public void RoundTrip_KeepsTheTemplates()
    {
        string path = Path.Combine(_directory, "settings.json");
        PresenceTemplates templates = new() { Details = "{saveName} \"quoted\" 🚂", State = "{balance}", Hover = "Hover" };

        SettingsStore.TrySave(path, new AppSettings() { Preset = "custom", Templates = templates });
        AppSettings loaded = SettingsStore.Load(path);

        Assert.Equal(templates, loaded.Templates);
        Assert.Equal("custom", loaded.Preset);
        Assert.Contains("\"templates\": {", File.ReadAllText(path), StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("{\"language\": \"ko\"}")]
    [InlineData("{\"language\": \"ko\", \"templates\": null}")]
    [InlineData("{\"language\": \"ko\", \"templates\": {}}")]
    public async Task MissingTemplates_GiveTheDefaultsAsync(string content)
    {
        string path = Path.Combine(_directory, "settings.json");
        await File.WriteAllTextAsync(path, content, TestContext.Current.CancellationToken);

        AppSettings loaded = SettingsStore.Load(path);

        Assert.Equal(new PresenceTemplates(), loaded.Templates);
        Assert.Equal("ko", loaded.Language);
    }

    [Fact]
    public async Task MissingOrNullFields_KeepTheirDefaultAndEmptyStringsAreKeptAsync()
    {
        string path = Path.Combine(_directory, "settings.json");
        await File.WriteAllTextAsync(path, "{\"templates\": {\"details\": \"\", \"state\": null}}", TestContext.Current.CancellationToken);

        PresenceTemplates loaded = SettingsStore.Load(path).Templates;

        Assert.Equal(string.Empty, loaded.Details);
        Assert.Equal("{date}", loaded.State);
        Assert.Equal("Transport Fever 3", loaded.Hover);
    }

    [Fact]
    public void LongTemplates_AreCutAt512()
    {
        string path = Path.Combine(_directory, "settings.json");
        string longText = new('a', 600);

        SettingsStore.TrySave(path, new AppSettings() { Templates = new PresenceTemplates() { Details = longText, State = new string('b', 512), Hover = "short" } });
        PresenceTemplates loaded = SettingsStore.Load(path).Templates;

        Assert.Equal(new string('a', 512), loaded.Details);
        Assert.Equal(new string('b', 512), loaded.State);
        Assert.Equal("short", loaded.Hover);
    }

    [Fact]
    public async Task LongTemplatesInTheFile_AreCutWithoutSplittingASurrogatePairAsync()
    {
        string path = Path.Combine(_directory, "settings.json");
        string split = new string('a', 511) + "😀tail";
        string whole = new string('a', 510) + "😀tail";
        await File.WriteAllTextAsync(path, $"{{\"templates\": {{\"details\": \"{split}\", \"state\": \"{whole}\"}}}}", TestContext.Current.CancellationToken);

        PresenceTemplates loaded = SettingsStore.Load(path).Templates;

        Assert.Equal(new string('a', 511), loaded.Details);
        Assert.Equal(new string('a', 510) + "😀", loaded.State);
    }

    [Theory]
    [InlineData("5")]
    [InlineData("\"text\"")]
    [InlineData("[1, {\"details\": \"x\"}]")]
    [InlineData("true")]
    public async Task InvalidTemplates_KeepTheLanguageAndPresetAsync(string templates)
    {
        string path = Path.Combine(_directory, "settings.json");
        await File.WriteAllTextAsync(path, $"{{\"language\": \"ko\", \"preset\": \"era\", \"templates\": {templates}}}", TestContext.Current.CancellationToken);

        AppSettings loaded = SettingsStore.Load(path);

        Assert.Equal("ko", loaded.Language);
        Assert.Equal("era", loaded.Preset);
        Assert.Equal(new PresenceTemplates(), loaded.Templates);
    }

    [Fact]
    public async Task InvalidTemplateField_KeepsTheOtherFieldsAsync()
    {
        string path = Path.Combine(_directory, "settings.json");
        await File.WriteAllTextAsync(path, "{\"language\": \"ko\", \"templates\": {\"details\": 5, \"state\": \"S1\", \"hover\": {\"a\": [1]}}}", TestContext.Current.CancellationToken);

        AppSettings loaded = SettingsStore.Load(path);

        Assert.Equal("ko", loaded.Language);
        Assert.Equal("{rank} · {mode}", loaded.Templates.Details);
        Assert.Equal("S1", loaded.Templates.State);
        Assert.Equal("Transport Fever 3", loaded.Templates.Hover);
    }

    [Fact]
    public void StoreButton_IsOffByDefault()
        => Assert.False(new AppSettings().StoreButton);

    [Fact]
    public async Task MissingStoreButton_IsOffAsync()
    {
        string path = Path.Combine(_directory, "settings.json");
        await File.WriteAllTextAsync(path, "{\"language\": \"ko\"}", TestContext.Current.CancellationToken);

        Assert.False(SettingsStore.Load(path).StoreButton);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void StoreButton_RoundTrips(bool enabled)
    {
        string path = Path.Combine(_directory, "settings.json");

        SettingsStore.TrySave(path, new AppSettings() { Language = "de", StoreButton = enabled });
        AppSettings loaded = SettingsStore.Load(path);

        Assert.Equal(enabled, loaded.StoreButton);
        Assert.Equal("de", loaded.Language);
        Assert.Contains("\"storeButton\": " + ((enabled) ? "true" : "false"), File.ReadAllText(path), StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("\"yes\"")]
    [InlineData("\"true\"")]
    [InlineData("1")]
    [InlineData("0")]
    [InlineData("null")]
    [InlineData("{\"on\": true}")]
    [InlineData("[true]")]
    public async Task WrongStoreButtonType_IsOffAndKeepsTheOtherSettingsAsync(string value)
    {
        string path = Path.Combine(_directory, "settings.json");
        await File.WriteAllTextAsync(path, $"{{\"language\": \"ko\", \"storeButton\": {value}, \"preset\": \"custom\", \"templates\": {{\"details\": \"x\"}}}}", TestContext.Current.CancellationToken);

        AppSettings loaded = SettingsStore.Load(path);

        Assert.False(loaded.StoreButton);
        Assert.Equal("ko", loaded.Language);
        Assert.Equal("custom", loaded.Preset);
        Assert.Equal("x", loaded.Templates.Details);
    }

    [Fact]
    public async Task WrongPresetType_CostsOnlyThePresetAsync()
    {
        string path = Path.Combine(_directory, "settings.json");
        await File.WriteAllTextAsync(path, "{\"language\":\"fr\",\"preset\":0,\"templates\":{\"details\":\"KEEP ME\"},\"storeButton\":true}", TestContext.Current.CancellationToken);

        AppSettings loaded = SettingsStore.Load(path);

        Assert.Equal("fr", loaded.Language);
        Assert.Equal("career", loaded.Preset);
        Assert.Equal("KEEP ME", loaded.Templates.Details);
        Assert.True(loaded.StoreButton);
    }

    [Fact]
    public async Task WrongLanguageType_CostsOnlyTheLanguageAsync()
    {
        string path = Path.Combine(_directory, "settings.json");
        await File.WriteAllTextAsync(path, "{\"language\":{\"code\":\"fr\"},\"preset\":\"era\",\"templates\":{\"details\":\"KEEP ME\"},\"storeButton\":true}", TestContext.Current.CancellationToken);

        AppSettings loaded = SettingsStore.Load(path);

        Assert.Equal("game", loaded.Language);
        Assert.Equal("era", loaded.Preset);
        Assert.Equal("KEEP ME", loaded.Templates.Details);
        Assert.True(loaded.StoreButton);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("1.5")]
    [InlineData("true")]
    [InlineData("false")]
    [InlineData("null")]
    [InlineData("[\"fr\"]")]
    [InlineData("{\"a\":[1,{\"b\":2}]}")]
    public async Task WrongLanguageAndPresetTypes_GiveTheDefaultsAndKeepTheOtherSettingsAsync(string value)
    {
        string path = Path.Combine(_directory, "settings.json");
        await File.WriteAllTextAsync(path, $"{{\"language\":{value},\"preset\":{value},\"templates\":{{\"state\":\"KEEP ME\"}},\"storeButton\":true}}", TestContext.Current.CancellationToken);

        AppSettings loaded = SettingsStore.Load(path);

        Assert.Equal("game", loaded.Language);
        Assert.Equal("career", loaded.Preset);
        Assert.Equal("KEEP ME", loaded.Templates.State);
        Assert.True(loaded.StoreButton);
    }

    [Fact]
    public async Task StoreButtonTrueInTheFile_IsReadAsync()
    {
        string path = Path.Combine(_directory, "settings.json");
        await File.WriteAllTextAsync(path, "{\"storeButton\": true, \"preset\": \"era\"}", TestContext.Current.CancellationToken);

        AppSettings loaded = SettingsStore.Load(path);

        Assert.True(loaded.StoreButton);
        Assert.Equal("era", loaded.Preset);
    }

    [Fact]
    public void Defaults_FollowTheSystemTheme()
        => Assert.Equal("system", new AppSettings().Theme);

    [Theory]
    [InlineData("system")]
    [InlineData("light")]
    [InlineData("dark")]
    public void RoundTrip_KeepsTheTheme(string theme)
    {
        string path = Path.Combine(_directory, "settings.json");

        bool saved = SettingsStore.TrySave(path, new AppSettings() { Theme = theme });
        AppSettings loaded = SettingsStore.Load(path);

        Assert.True(saved);
        Assert.Equal(theme, loaded.Theme);
    }

    [Fact]
    public void Save_WritesTheThemeKey()
    {
        string path = Path.Combine(_directory, "settings.json");

        SettingsStore.TrySave(path, new AppSettings() { Theme = "dark" });

        Assert.Contains("\"theme\": \"dark\"", File.ReadAllText(path), StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("\"auto\"")]
    [InlineData("\"Dark\"")]
    [InlineData("\"\"")]
    [InlineData("0")]
    [InlineData("null")]
    public async Task UnknownOrWrongTypeTheme_BecomesSystemAndKeepsTheOtherSettingsAsync(string value)
    {
        string path = Path.Combine(_directory, "settings.json");
        await File.WriteAllTextAsync(path, $"{{\"language\":\"fr\",\"preset\":\"era\",\"theme\":{value},\"storeButton\":true}}", TestContext.Current.CancellationToken);

        AppSettings loaded = SettingsStore.Load(path);

        Assert.Equal("system", loaded.Theme);
        Assert.Equal("fr", loaded.Language);
        Assert.Equal("era", loaded.Preset);
        Assert.True(loaded.StoreButton);
    }

    [Fact]
    public async Task MissingTheme_BecomesSystemAsync()
    {
        string path = Path.Combine(_directory, "settings.json");
        await File.WriteAllTextAsync(path, "{\"language\": \"ko\", \"preset\": \"era\"}", TestContext.Current.CancellationToken);

        AppSettings loaded = SettingsStore.Load(path);

        Assert.Equal("system", loaded.Theme);
        Assert.Equal("ko", loaded.Language);
    }

    [Fact]
    public async Task ValidThemeNextToAnInvalidLanguage_IsKeptAsync()
    {
        string path = Path.Combine(_directory, "settings.json");
        await File.WriteAllTextAsync(path, "{\"language\": \"klingon\", \"theme\": \"light\"}", TestContext.Current.CancellationToken);

        AppSettings loaded = SettingsStore.Load(path);

        Assert.Equal("game", loaded.Language);
        Assert.Equal("light", loaded.Theme);
    }

    [Fact]
    public void NormalizeTemplates_ReturnsValidTemplatesUnchanged()
    {
        PresenceTemplates templates = new() { Details = "x" };

        Assert.Same(templates, SettingsStore.NormalizeTemplates(templates));
    }

    [Fact]
    public void Load_OfAFolderPathGivesDefaults()
        => Assert.Equal("game", SettingsStore.Load(_directory).Language);
}
