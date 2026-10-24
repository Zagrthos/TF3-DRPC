using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Microsoft.Extensions.Logging;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Dispatching;

using Tf3Drpc.App.Constants;
using Tf3Drpc.App.Logging;
using Tf3Drpc.App.Messages;
using Tf3Drpc.App.Services.Interfaces;
using Tf3Drpc.Core.Constants;
using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Languages;
using Tf3Drpc.Core.Presence;
using Tf3Drpc.Core.Settings;
using Tf3Drpc.Core.Structs;

namespace Tf3Drpc.App.ViewModels;

/// <summary>
/// The state of the settings window: language, theme and preset choice, the custom templates, the store button, a preview and a read-only status.
/// </summary>
#pragma warning disable CA1812 // Avoid uninstantiated internal classes
internal sealed partial class SettingsViewModel : ObservableObject, IRecipient<AppStatusChangedMessage>, IRecipient<SettingsChangedMessage>, IRecipient<LogFileUnavailableMessage>
#pragma warning restore CA1812
{
    private readonly ISettingsService _settings;
    private readonly ITranslationService _translation;
    private readonly IPathService _paths;
    private readonly IAppLifecycleService _lifecycle;
    private readonly IPresenceService _presence;
    private readonly ILogger<SettingsViewModel> _logger;
    private AppStatusStruct _status;
    private IDispatcherTimer? _saveTimer;
    private string? _builtForLanguage;
    private bool _updating;
    private bool _hasPendingEdit;

    public SettingsViewModel
    (
        ISettingsService settings,
        ITranslationService translation,
        IMonitorService monitor,
        IPathService paths,
        IAppLifecycleService lifecycle,
        IPresenceService presence,
        ILogger<SettingsViewModel> logger,
        IMessenger messenger
    )
    {
        ArgumentNullException.ThrowIfNull(paths);
        _settings = settings;
        _translation = translation;
        _paths = paths;
        _lifecycle = lifecycle;
        _presence = presence;
        _logger = logger;
        _status = monitor.Status;
        messenger.Register<AppStatusChangedMessage>(this);
        messenger.Register<SettingsChangedMessage>(this);
        messenger.Register<LogFileUnavailableMessage>(this);

        // Both Exit routes end here, so a template edit still waiting for its save timer is kept.
        lifecycle.Exiting += OnExiting;
        Refresh(_status);
    }

    #region Properties

    public ObservableCollection<LanguageOption> Languages { get; } = [];

    [ObservableProperty]
    public partial LanguageOption? SelectedLanguage { get; set; }

    public ObservableCollection<ThemeOption> Themes { get; } = [];

    [ObservableProperty]
    public partial ThemeOption? SelectedTheme { get; set; }

    public ObservableCollection<PresetOption> Presets { get; } = [];

    [ObservableProperty]
    public partial PresetOption? SelectedPreset { get; set; }

    [ObservableProperty]
    public partial string DetailsTemplate { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string StateTemplate { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string HoverTemplate { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string TemplateFirstLineLabel { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string TemplateSecondLineLabel { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string TemplateHoverLabel { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string PlaceholdersLabel { get; set; } = string.Empty;

    public ObservableCollection<PlaceholderOption> Placeholders { get; } = [];

    [ObservableProperty]
    public partial string PrivateHint { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string PreviewLabel { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string PreviewDetails { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string PreviewState { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string PreviewHover { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string PreviewExampleText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsPreviewExampleVisible { get; set; }

    [ObservableProperty]
    public partial string PreviewButton { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool ShowStoreButton { get; set; }

    [ObservableProperty]
    public partial string ShowStoreButtonLabel { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string StoreButtonHint { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string NotWritableText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsNotWritableVisible { get; set; }

    [ObservableProperty]
    public partial string LogNotWritableText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsLogNotWritableVisible { get; set; }

    [ObservableProperty]
    public partial string LanguageLabel { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string ThemeLabel { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string PresetLabel { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string ExitText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string GameStatus { get; set; } = string.Empty;

    [ObservableProperty]
    public partial int? GameBuild { get; set; }

    [ObservableProperty]
    public partial string DiscordStatus { get; set; } = string.Empty;

    #endregion Properties

    #region Message handling

    public void Receive(AppStatusChangedMessage message)
        => MainThread.BeginInvokeOnMainThread(() => Refresh(message.Status));

    public void Receive(SettingsChangedMessage message)
        => MainThread.BeginInvokeOnMainThread(() => Refresh(_status));

    public void Receive(LogFileUnavailableMessage message)
        => MainThread.BeginInvokeOnMainThread(() => Refresh(_status));

    #endregion Message handling

    #region Commands

    [RelayCommand]
    private Task ExitAsync()
        => _lifecycle.ExitAsync();

    // Concurrent runs stay allowed: a disabled button would make WinUI move the keyboard focus away, to Exit.
    [RelayCommand(AllowConcurrentExecutions = true)]
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "A failing link must never throw out of the command.")]
    private async Task OpenUrlAsync(string url)
    {
        try
        {
            Uri uri = new(url);

            if (!await Launcher.Default.OpenAsync(uri))
                _logger.UrlNotOpened(url);
        }
        catch (Exception exception)
        {
            _logger.UrlOpenFailed(url, exception);
        }
    }

    #endregion Commands

    #region Language selection

    partial void OnSelectedLanguageChanged(LanguageOption? value)
    {
        if (!_updating && (value is not null))
            _ = _settings.SetLanguage(value.Code);
    }

    #endregion Language selection

    #region Theme selection

    partial void OnSelectedThemeChanged(ThemeOption? value)
    {
        if (!_updating && (value is not null))
            _ = _settings.SetTheme(value.Theme);
    }

    #endregion Theme selection

    #region Preset selection

    partial void OnSelectedPresetChanged(PresetOption? value)
    {
        if (_updating || (value is null))
            return;

        FlushPendingEdit();
        _ = _settings.SetPreset(value.Preset);
        RefreshPreview();
    }

    #endregion Preset selection

    #region Store button

    partial void OnShowStoreButtonChanged(bool value)
    {
        if (_updating)
            return;

        _ = _settings.SetStoreButton(value);
        RefreshPreview();
    }

    #endregion Store button

    #region Template editing

    partial void OnDetailsTemplateChanged(string value)
        => OnTemplateEdited();

    partial void OnStateTemplateChanged(string value)
        => OnTemplateEdited();

    partial void OnHoverTemplateChanged(string value)
        => OnTemplateEdited();

    private void OnTemplateEdited()
    {
        if (_updating)
            return;

        _hasPendingEdit = true;
        RefreshPreview();

        _saveTimer ??= CreateSaveTimer();

        if (_saveTimer is null)
        {
            FlushPendingEdit();

            return;
        }

        _saveTimer.Stop();
        _saveTimer.Start();
    }

    private IDispatcherTimer? CreateSaveTimer()
    {
        IDispatcher? dispatcher = Dispatcher.GetForCurrentThread();

        if (dispatcher is null)
            return null;

        IDispatcherTimer timer = dispatcher.CreateTimer();
        timer.Interval = AppConstants.TemplateSaveDelay;
        timer.IsRepeating = false;
        timer.Tick += OnSaveTimerTick;

        return timer;
    }

    private void OnSaveTimerTick(object? sender, EventArgs e)
        => FlushPendingEdit();

    private void OnExiting(object? sender, EventArgs e)
        => FlushPendingEdit();

    private void FlushPendingEdit()
    {
        if (!_hasPendingEdit)
            return;

        _hasPendingEdit = false;
        _saveTimer?.Stop();
        _ = _settings.SetTemplates(GetEditedTemplates());
    }

    private PresenceTemplates GetEditedTemplates()
        => new() { Details = DetailsTemplate, State = StateTemplate, Hover = HoverTemplate };

    #endregion Template editing

    #region Preview

    private void RefreshPreview()
    {
        PresencePreset preset = SelectedPreset?.Preset ?? PresencePreset.Career;
        PresenceContentStruct content = _presence.BuildPreview(_status.Language, preset, GetEditedTemplates(), out bool usesExample);

        PreviewDetails = content.Details;
        PreviewState = content.State ?? string.Empty;
        PreviewHover = content.LargeImageText ?? DiscordConstants.LargeImageText;
        PreviewButton = content.ButtonLabel ?? string.Empty;
        IsPreviewExampleVisible = usesExample;
    }

    private string DescribePreset(PresencePreset preset)
    {
        PresenceTemplates templates = GetEditedTemplates();

        return PresenceDescription.Describe(preset, shown => _presence.BuildPreview(_status.Language, shown, templates, out _));
    }

    #endregion Preview

    #region Status refresh

    private void Refresh(in AppStatusStruct status)
    {
        _status = status;
        LanguageInfo language = status.Language;
        _updating = true;

        try
        {
            if (!string.Equals(_builtForLanguage, language.Code, StringComparison.Ordinal))
            {
                BuildLanguages(language);
                BuildThemes(language);
                BuildPresets(language);
                BuildPlaceholders(language);
                _builtForLanguage = language.Code;
            }

            SelectedLanguage = FindOption(_settings.Current.Language);
            SelectedTheme = FindThemeOption(_settings.Current.Theme);
            SelectedPreset = FindPresetOption(_settings.Current.Preset);
            ShowStoreButton = _settings.Current.StoreButton;

            // Never overwrite what the user typed and has not saved yet.
            if (!_hasPendingEdit)
            {
                PresenceTemplates templates = _settings.Current.Templates;
                DetailsTemplate = templates.Details;
                StateTemplate = templates.State;
                HoverTemplate = templates.Hover;
            }
        }
        finally
        {
            _updating = false;
        }

        IsNotWritableVisible = _paths.IsTranslocated || !_paths.IsDataWritable || _settings.LastSaveFailed;
        NotWritableText = _translation.GetOwnText((_paths.IsTranslocated) ? OwnText.Translocated : OwnText.NotWritable, language);
        IsLogNotWritableVisible = !_paths.IsLogFileAvailable;
        LogNotWritableText = _translation.GetOwnText(OwnText.LogNotWritable, language);
        LanguageLabel = _translation.GetOwnText(OwnText.Language, language);
        ThemeLabel = _translation.GetOwnText(OwnText.Theme, language);
        PresetLabel = _translation.GetOwnText(OwnText.StatusAppearance, language);
        TemplateFirstLineLabel = _translation.GetOwnText(OwnText.TemplateFirstLine, language);
        TemplateSecondLineLabel = _translation.GetOwnText(OwnText.TemplateSecondLine, language);
        TemplateHoverLabel = _translation.GetOwnText(OwnText.TemplateHoverText, language);
        PlaceholdersLabel = _translation.GetOwnText(OwnText.Placeholders, language);
        PrivateHint = _translation.GetOwnText(OwnText.PrivateHint, language);
        ShowStoreButtonLabel = _translation.GetOwnText(OwnText.ShowStoreButton, language);
        StoreButtonHint = _translation.GetOwnText(OwnText.StoreButtonHint, language);
        PreviewLabel = _translation.GetOwnText(OwnText.Preview, language);
        PreviewExampleText = _translation.GetOwnText(OwnText.PreviewExample, language);
        ExitText = _translation.GetOwnText(OwnText.Exit, language);
        GameStatus = BuildGameStatus(status, language);
        GameBuild = status.Game.GameBuild;
        DiscordStatus = _translation.GetOwnText((status.DiscordConnected) ? OwnText.DiscordConnected : OwnText.DiscordNotConnected, language);
        RefreshPreview();
    }

    private string BuildGameStatus(in AppStatusStruct status, LanguageInfo language)
    {
        if (status.PresenceDetails is null)
            return _translation.GetOwnText(OwnText.GameNotRunning, language);

        return (status.PresenceState is null)
            ? status.PresenceDetails
            : (status.PresenceDetails + DiscordConstants.Separator + status.PresenceState);
    }

    private void BuildLanguages(LanguageInfo language)
    {
        Languages.Clear();
        Languages.Add(new LanguageOption(LanguageConstants.GameSetting, _translation.GetOwnText(OwnText.SameAsTheGame, language)));

        for (int i = 0; i < _translation.Languages.Count; i++)
        {
            LanguageInfo entry = _translation.Languages[i];
            Languages.Add(new LanguageOption(entry.Code, entry.NativeName));
        }
    }

    private void BuildThemes(LanguageInfo language)
    {
        Themes.Clear();

        for (int i = 0; i < ThemeCatalog.All.Count; i++)
        {
            ThemeMode theme = ThemeCatalog.All[i];
            Themes.Add(new ThemeOption(theme, _translation.GetOwnText(ThemeCatalog.GetDisplayText(theme), language)));
        }
    }

    private void BuildPresets(LanguageInfo language)
    {
        Presets.Clear();

        for (int i = 0; i < PresetCatalog.All.Count; i++)
        {
            PresencePreset preset = PresetCatalog.All[i];
            Presets.Add(new PresetOption(preset, _translation.GetOwnText(PresetCatalog.GetDisplayText(preset), language), () => DescribePreset(preset)));
        }
    }

    private void BuildPlaceholders(LanguageInfo language)
    {
        Placeholders.Clear();

        for (int i = 0; i < PlaceholderCatalog.All.Count; i++)
        {
            PlaceholderInfoStruct placeholder = PlaceholderCatalog.All[i];
            string description = _translation.GetOwnText(placeholder.Description, language);

            Placeholders.Add(new PlaceholderOption(placeholder.Name, description, placeholder.IsPrivate));
        }
    }

    private ThemeOption? FindThemeOption(string setting)
    {
        ThemeMode wanted = (ThemeCatalog.TryParse(setting, out ThemeMode parsed)) ? parsed : ThemeMode.System;

        for (int i = 0; i < Themes.Count; i++)
        {
            if (Themes[i].Theme == wanted)
                return Themes[i];
        }

        return null;
    }

    private PresetOption? FindPresetOption(string setting)
    {
        PresencePreset wanted = (PresetCatalog.TryParse(setting, out PresencePreset parsed)) ? parsed : PresencePreset.Career;

        for (int i = 0; i < Presets.Count; i++)
        {
            if (Presets[i].Preset == wanted)
                return Presets[i];
        }

        return null;
    }

    private LanguageOption? FindOption(string code)
    {
        for (int i = 0; i < Languages.Count; i++)
        {
            if (string.Equals(Languages[i].Code, code, StringComparison.Ordinal))
                return Languages[i];
        }

        return null;
    }

    #endregion Status refresh
}
