using System;

using CommunityToolkit.Mvvm.Messaging;

using Foundation;

using Microsoft.Extensions.Logging;
using Microsoft.Maui.ApplicationModel;

using Tf3Drpc.App.Constants;
using Tf3Drpc.App.Logging;
using Tf3Drpc.App.Messages;
using Tf3Drpc.App.Services.Interfaces;
using Tf3Drpc.Core.Constants;
using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Languages;
using Tf3Drpc.Core.Structs;

using static Tf3Drpc.App.Utilities.ObjCRuntimeUtils;

namespace Tf3Drpc.App.Platforms.MacCatalyst;

/// <summary>
/// The menu bar icon on macOS with its Language, Settings and Exit menu: AppKit's <c>NSStatusItem</c> through the Objective-C runtime.
/// </summary>
#pragma warning disable CA1812 // Avoid uninstantiated internal classes
internal sealed class MacTrayService : ITrayService, IRecipient<AppStatusChangedMessage>, IRecipient<SettingsChangedMessage>
#pragma warning restore CA1812
{
    private const string IconName = "tray";
    private const string IconType = "png";
    private const double IconPoints = 18;
    private const double VariableLength = -1;
    private const nint StateOff = 0;
    private const nint StateOn = 1;
    private const nint SameAsGameTag = -1;
    private readonly ITranslationService _translation;
    private readonly ISettingsService _settings;
    private readonly IWindowService _windows;
    private readonly IAppLifecycleService _lifecycle;
    private readonly ILogger<MacTrayService> _logger;
    private readonly MenuTarget _target;
    private readonly nint[] _languageItems;
    private AppStatusStruct _status;
    private nint _statusItem;
    private nint _languageMenuItem;
    private nint _sameAsGameItem;
    private nint _settingsItem;
    private nint _exitItem;

    public MacTrayService
    (
        ITranslationService translation,
        ISettingsService settings,
        IMonitorService monitor,
        IWindowService windows,
        IAppLifecycleService lifecycle,
        IMessenger messenger,
        ILogger<MacTrayService> logger
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
        _target = new MenuTarget(this);
        _languageItems = new nint[translation.Languages.Count];
        messenger.Register<AppStatusChangedMessage>(this);
        messenger.Register<SettingsChangedMessage>(this);
        lifecycle.Exiting += OnExiting;
    }

    // The system-wide status bar; AppKit returns the same object every time.
    private static nint SystemStatusBar
        => Send(GetClass("NSStatusBar"), "systemStatusBar");

    #region Showing and disposing

    public bool Show()
    {
        try
        {
            // The status bar returns an autoreleased item; it is kept until Dispose.
            nint statusItem = Send(SystemStatusBar, "statusItemWithLength:", VariableLength);
            _statusItem = Send(statusItem, "retain");

            nint button = Send(_statusItem, "button");
            nint image = CreateIcon();

            if (button == 0 || image == 0)
            {
                _logger.TrayCreated(false);
                RemoveStatusItem();

                return false;
            }

            Send(button, "setImage:", image);
            Send(image, "release");
            using NSString toolTip = new(AppConstants.ApplicationName);
            Send(button, "setToolTip:", toolTip.Handle);
            Send(_statusItem, "setMenu:", BuildMenu());
            RefreshTexts(_status);
            _logger.TrayCreated(true);

            return true;
        }
        catch (Exception exception) when (exception is InvalidOperationException or ArgumentException or ObjCRuntime.ObjCException)
        {
            _logger.TrayFailed(exception);
            RemoveStatusItem();

            return false;
        }
    }

    public void Receive(AppStatusChangedMessage message)
        => MainThread.BeginInvokeOnMainThread(() => RefreshTexts(message.Status));

    public void Receive(SettingsChangedMessage message)
        => MainThread.BeginInvokeOnMainThread(() => RefreshTexts(_status));

    public void Dispose()
    {
        RemoveStatusItem();
        _target.Dispose();
    }

    private void RemoveStatusItem()
    {
        if (_statusItem == 0)
            return;

        Send(SystemStatusBar, "removeStatusItem:", _statusItem);
        Send(_statusItem, "release");
        _statusItem = 0;
    }

    #endregion Showing and disposing

    #region Menu

    // The bundle's icon at 18 points, the menu bar's icon size; 36 pixels for Retina displays.
    private static nint CreateIcon()
    {
        string? path = NSBundle.MainBundle.PathForResource(IconName, IconType);

        if (path is null)
            return 0;

        using NSString nativePath = new(path);
        nint allocated = Send(GetClass("NSImage"), "alloc");
        nint image = Send(allocated, "initWithContentsOfFile:", nativePath.Handle);

        if (image != 0)
            SendSize(image, "setSize:", new NativeSize(IconPoints, IconPoints));

        return image;
    }

    private static nint CreateMenu()
    {
        using NSString title = new(string.Empty);

        nint allocated = Send(GetClass("NSMenu"), "alloc");

        return Send(allocated, "initWithTitle:", title.Handle);
    }

    // Adds the item to the menu, which then owns it.
    private static void Add(in nint menu, in nint item)
    {
        Send(menu, "addItem:", item);
        Send(item, "release");
    }

    private static void SetTitle(in nint item, string text)
    {
        using NSString title = new(text);
        Send(item, "setTitle:", title.Handle);
    }

    private static void SetChecked(in nint item, bool isChecked)
        => SendInteger(item, "setState:", (isChecked) ? StateOn : StateOff);

    // An item without an action is for a submenu, which AppKit keeps enabled on its own.
    private nint CreateItem(string? action, in nint tag)
    {
        using NSString title = new(string.Empty);
        using NSString keyEquivalent = new(string.Empty);
        nint selector = (action is null) ? 0 : ObjCRuntime.Selector.GetHandle(action);
        nint allocated = Send(GetClass("NSMenuItem"), "alloc");
        nint item = Send(allocated, "initWithTitle:action:keyEquivalent:", title.Handle, selector, keyEquivalent.Handle);

        if (action is not null)
            Send(item, "setTarget:", _target.Handle);

        SendInteger(item, "setTag:", tag);

        return item;
    }

    private nint BuildMenu()
    {
        nint languageMenu = CreateMenu();
        _sameAsGameItem = CreateItem(MenuTarget.LanguageAction, SameAsGameTag);
        Add(languageMenu, _sameAsGameItem);

        for (int i = 0; i < _languageItems.Length; i++)
        {
            _languageItems[i] = CreateItem(MenuTarget.LanguageAction, i);
            Add(languageMenu, _languageItems[i]);
        }

        nint menu = CreateMenu();
        _languageMenuItem = CreateItem(null, 0);
        Send(_languageMenuItem, "setSubmenu:", languageMenu);
        Send(languageMenu, "release");
        Add(menu, _languageMenuItem);
        _settingsItem = CreateItem(MenuTarget.SettingsAction, 0);
        Add(menu, _settingsItem);
        nint separator = Send(GetClass("NSMenuItem"), "separatorItem");
        Send(menu, "addItem:", separator);
        _exitItem = CreateItem(MenuTarget.ExitAction, 0);
        Add(menu, _exitItem);

        // The status item keeps the menu.
        Send(menu, "autorelease");

        return menu;
    }

    private void RefreshTexts(in AppStatusStruct status)
    {
        _status = status;

        if (_statusItem == 0)
            return;

        LanguageInfo language = status.Language;
        string setting = _settings.Current.Language;
        SetTitle(_languageMenuItem, _translation.GetOwnText(OwnText.Language, language));
        SetTitle(_settingsItem, _translation.GetOwnText(OwnText.Settings, language));
        SetTitle(_exitItem, _translation.GetOwnText(OwnText.Exit, language));
        SetTitle(_sameAsGameItem, _translation.GetOwnText(OwnText.SameAsTheGame, language));
        SetChecked(_sameAsGameItem, string.Equals(setting, LanguageConstants.GameSetting, StringComparison.Ordinal));

        for (int i = 0; i < _languageItems.Length; i++)
        {
            SetTitle(_languageItems[i], _translation.Languages[i].NativeName);
            SetChecked(_languageItems[i], string.Equals(setting, _translation.Languages[i].Code, StringComparison.Ordinal));
        }
    }

    #endregion Menu

    #region Menu events

    private void OnLanguageClicked(in nint tag)
    {
        string code = (tag == SameAsGameTag) ? LanguageConstants.GameSetting : _translation.Languages[(int)tag].Code;
        _ = _settings.SetLanguage(code);
        RefreshTexts(_status);
    }

    private void OnSettingsClicked()
        => _windows.ShowSettings();

    private async void OnExitClickedAsync()
        => await _lifecycle.ExitAsync();

    private void OnExiting(object? sender, EventArgs e)
        => Dispose();

    #endregion Menu events

    /// <summary>
    /// The Objective-C target of the menu items; AppKit calls its exported methods.
    /// </summary>
    [Register("Tf3DrpcMenuTarget")]
    private sealed class MenuTarget(MacTrayService owner) : NSObject
    {
        public const string LanguageAction = "languageClicked:";

        // AppKit also calls an action method that takes no sender.
        public const string SettingsAction = "settingsClicked";
        public const string ExitAction = "exitClicked";

        [Export(LanguageAction)]
        public void LanguageClicked(NSObject sender)
            => owner.OnLanguageClicked(GetInteger(sender.Handle, "tag"));

        [Export(SettingsAction)]
        public void SettingsClicked()
            => owner.OnSettingsClicked();

        [Export(ExitAction)]
        public void ExitClicked()
            => owner.OnExitClickedAsync();
    }
}
