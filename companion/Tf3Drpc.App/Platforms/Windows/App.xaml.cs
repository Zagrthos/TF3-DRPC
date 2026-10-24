using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace Tf3Drpc.App.Platforms.Windows;

/// <summary>
/// The WinUI application that hosts the MAUI app.
/// </summary>
#pragma warning disable CA1812 // Avoid uninstantiated internal classes
public sealed partial class WinUIApplication : MauiWinUIApplication
#pragma warning restore CA1812
{
    public WinUIApplication()
        => InitializeComponent();

    protected override MauiApp CreateMauiApp()
        => MauiProgram.CreateMauiApp();
}
