using Microsoft.Maui.Controls;

namespace Tf3Drpc.App.Views;

/// <summary>
/// The settings window: its size, title bar and its only page.
/// </summary>
#pragma warning disable CA1812 // Avoid uninstantiated internal classes
internal sealed partial class SettingsWindow : Window
#pragma warning restore CA1812
{
    public SettingsWindow(SettingsPage page)
    {
        InitializeComponent();
        Page = page;
    }
}
