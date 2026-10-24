using Microsoft.Maui.Controls;

using Tf3Drpc.App.ViewModels;

namespace Tf3Drpc.App.Views;

/// <summary>
/// The settings window's only page.
/// </summary>
#pragma warning disable CA1812 // Avoid uninstantiated internal classes
internal sealed partial class SettingsPage : ContentPage
#pragma warning restore CA1812
{
    public SettingsPage(SettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
