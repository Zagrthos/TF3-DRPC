using System;
using System.IO;

using CommunityToolkit.Mvvm.Messaging;

using Microsoft.Extensions.Logging;
using Microsoft.Maui.ApplicationModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using Tf3Drpc.App.Constants;
using Tf3Drpc.App.Logging;
using Tf3Drpc.App.Messages;
using Tf3Drpc.App.Services.Interfaces;
using Tf3Drpc.Core.Constants;
using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Languages;
using Tf3Drpc.Core.Structs;

using WinUIEx;

namespace Tf3Drpc.App.Platforms.Windows;

/// <summary>
/// The tray icon on Windows with its Language, Settings and Exit menu.
/// </summary>
#pragma warning disable CA1812 // Avoid uninstantiated internal classes
internal sealed class WindowsTrayService : ITrayService, IRecipient<AppStatusChangedMessage>, IRecipient<SettingsChangedMessage>
#pragma warning restore CA1812
{
    private const uint TrayIconId = 1;
    private readonly ITranslationService _translation;
    private readonly ISettingsService _settings;
    private readonly IWindowService _windows;
    private readonly IAppLifecycleService _lifecycle;
    private readonly ILogger<WindowsTrayService> _logger;
    private readonly MenuFlyoutSubItem _languageMenu = new();
    private readonly MenuFlyoutItem _settingsItem = new();
    private readonly MenuFlyoutItem _exitItem = new();
    private readonly ToggleMenuFlyoutItem _sameAsGameItem = new() { Tag = LanguageConstants.GameSetting };
    private readonly ToggleMenuFlyoutItem[] _languageItems;
    private AppStatusStruct _status;
    private TrayIcon? _icon;
    private MenuFlyout? _flyout;

    public WindowsTrayService
    (
        ITranslationService translation,
        ISettingsService settings,
        IMonitorService monitor,
        IWindowService windows,
        IAppLifecycleService lifecycle,
        IMessenger messenger,
        ILogger<WindowsTrayService> logger
    )
    {
        ArgumentNullException.ThrowIfNull(translation);
        ArgumentNullException.ThrowIfNull(lifecycle);
        _translation = translation;
        _settings = settings;
        _status = monitor.Status;
        _windows = windows;
        _lifecycle = lifecycle;
        _logger = logger;
        _languageItems = new ToggleMenuFlyoutItem[translation.Languages.Count];
        messenger.Register<AppStatusChangedMessage>(this);
        messenger.Register<SettingsChangedMessage>(this);
        lifecycle.Exiting += OnExiting;
    }

    #region Showing and disposing

    public bool Show()
    {
        try
        {
            BuildMenu();

            string iconPath = Path.Combine(AppContext.BaseDirectory, AppConstants.TrayIconFileName);
            _flyout = new MenuFlyout();
            _flyout.Items.Add(_languageMenu);
            _flyout.Items.Add(_settingsItem);
            _flyout.Items.Add(new MenuFlyoutSeparator());
            _flyout.Items.Add(_exitItem);

            _icon = new TrayIcon(TrayIconId, iconPath, AppConstants.ApplicationName)
            {
                IsVisible = true
            };
            _icon.LeftDoubleClick += OnTrayDoubleClick;
            _icon.ContextMenu += OnTrayContextMenu;
            RefreshTexts(_status);
            _logger.TrayCreated(_icon.IsVisible);

            return _icon.IsVisible;
        }
        catch (Exception exception) when (exception is InvalidOperationException or ArgumentException or IOException or System.Runtime.InteropServices.COMException)
        {
            _logger.TrayFailed(exception);

            return false;
        }
    }

    public void Receive(AppStatusChangedMessage message)
        => MainThread.BeginInvokeOnMainThread(() => RefreshTexts(message.Status));

    public void Receive(SettingsChangedMessage message)
        => MainThread.BeginInvokeOnMainThread(() => RefreshTexts(_status));

    public void Dispose()
    {
        _icon?.Dispose();
        _icon = null;
    }

    #endregion Showing and disposing

    #region Tray events

    private void OnTrayDoubleClick(TrayIcon sender, TrayIconEventArgs e)
        => _windows.ShowSettings();

    private void OnTrayContextMenu(TrayIcon sender, TrayIconEventArgs e)
        => e.Flyout = _flyout;

    private void OnExiting(object? sender, EventArgs e)
        => Dispose();

    #endregion Tray events

    #region Menu

    private void BuildMenu()
    {
        _sameAsGameItem.Click += OnLanguageClicked;
        _languageMenu.Items.Add(_sameAsGameItem);

        for (int i = 0; i < _languageItems.Length; i++)
        {
            LanguageInfo language = _translation.Languages[i];
            ToggleMenuFlyoutItem item = new() { Text = language.NativeName, Tag = language.Code };
            item.Click += OnLanguageClicked;
            _languageItems[i] = item;
            _languageMenu.Items.Add(item);
        }

        _settingsItem.Click += OnSettingsClicked;
        _exitItem.Click += OnExitClickedAsync;
    }

    private void RefreshTexts(in AppStatusStruct status)
    {
        _status = status;
        LanguageInfo language = status.Language;
        string setting = _settings.Current.Language;
        _languageMenu.Text = _translation.GetOwnText(OwnText.Language, language);
        _settingsItem.Text = _translation.GetOwnText(OwnText.Settings, language);
        _exitItem.Text = _translation.GetOwnText(OwnText.Exit, language);
        _sameAsGameItem.Text = _translation.GetOwnText(OwnText.SameAsTheGame, language);
        _sameAsGameItem.IsChecked = string.Equals(setting, LanguageConstants.GameSetting, StringComparison.Ordinal);

        for (int i = 0; i < _languageItems.Length; i++)
        {
            _languageItems[i].IsChecked = string.Equals(setting, _translation.Languages[i].Code, StringComparison.Ordinal);
        }
    }

    #endregion Menu

    #region Menu events

    private void OnLanguageClicked(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { Tag: string code })
            _ = _settings.SetLanguage(code);

        RefreshTexts(_status);
    }

    private void OnSettingsClicked(object sender, RoutedEventArgs e)
        => _windows.ShowSettings();

    private async void OnExitClickedAsync(object sender, RoutedEventArgs e)
        => await _lifecycle.ExitAsync();

    #endregion Menu events
}
