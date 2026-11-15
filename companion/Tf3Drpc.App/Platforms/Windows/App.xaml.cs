using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace Tf3Drpc.App.Platforms.Windows;

/// <summary>
/// The WinUI application that hosts the MAUI app.
/// </summary>
// CA1812: Avoid uninstantiated internal classes
// CA1515: Consider making public types internal; WinUI's generated XamlTypeInfo.g.cs declares the application class public, and WinUI XAML has no x:ClassModifier (CS0262, WMC0011)
#pragma warning disable CA1812, CA1515
public sealed partial class WinUIApplication : MauiWinUIApplication
#pragma warning restore CA1812, CA1515
{
    /// <summary>
    /// Creates the application and loads its XAML.
    /// </summary>
    public WinUIApplication()
        => InitializeComponent();

    /// <inheritdoc/>
    protected override MauiApp CreateMauiApp()
        => MauiProgram.CreateMauiApp();
}
