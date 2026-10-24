using System;

using Microsoft.Maui.ApplicationModel;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

using Tf3Drpc.App.Services.Interfaces;

using Windows.Graphics;

namespace Tf3Drpc.App.Platforms.Windows;

/// <summary>
/// Owns the native settings window: it starts hidden and closing only hides it.
/// </summary>
internal sealed class WindowsWindowService : IWindowService
{
    private const int OffScreen = -32000;
    private Window? _window;
    private bool _initialised;
    private bool _showWhenReady;

    /// <summary>
    /// Takes over the native window right after it was created, before it is first activated.
    /// </summary>
    public void Attach(Window window)
    {
        ArgumentNullException.ThrowIfNull(window);
        _window = window;
        window.SystemBackdrop = CreateBackdrop();
        window.AppWindow.Closing += OnClosing;
        window.Activated += OnFirstActivated;
        window.AppWindow.Move(new PointInt32(OffScreen, OffScreen));
    }

    public void ShowSettings()
        => MainThread.BeginInvokeOnMainThread(ShowOnUiThread);

    // Mica needs Windows 11; Desktop Acrylic also works on Windows 10.
    private static SystemBackdrop CreateBackdrop()
        => (MicaController.IsSupported()) ? new MicaBackdrop() : new DesktopAcrylicBackdrop();

    private static void OnClosing(AppWindow sender, AppWindowClosingEventArgs args)
    {
        args.Cancel = true;
        sender.IsShownInSwitchers = false;
        sender.Hide();
    }

    // The primary display, not the nearest one: the window waits off-screen, so "nearest" depends on the display layout.
    private static void CenterOnPrimaryDisplay(AppWindow appWindow)
    {
        RectInt32 area = DisplayArea.Primary.WorkArea;
        SizeInt32 size = appWindow.Size;
        appWindow.Move(new PointInt32(area.X + ((area.Width - size.Width) / 2), area.Y + ((area.Height - size.Height) / 2)));
    }

    private void OnFirstActivated(object sender, WindowActivatedEventArgs args)
    {
        if (_window is null)
            return;

        _window.Activated -= OnFirstActivated;
        _initialised = true;
        CenterOnPrimaryDisplay(_window.AppWindow);

        if (_showWhenReady)
        {
            _window.AppWindow.Show();
        }
        else
        {
            _window.AppWindow.IsShownInSwitchers = false;
            _window.AppWindow.Hide();
        }
    }

    private void ShowOnUiThread()
    {
        if (_window is null || !_initialised)
        {
            _showWhenReady = true;

            return;
        }

        if (!_window.AppWindow.IsVisible)
            CenterOnPrimaryDisplay(_window.AppWindow);

        _window.AppWindow.IsShownInSwitchers = true;
        _window.AppWindow.Show();
        _window.Activate();
    }
}
