using System;

using Foundation;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;

using ObjCRuntime;

using Tf3Drpc.App.Services.Interfaces;

using UIKit;

namespace Tf3Drpc.App.Platforms.MacCatalyst;

/// <summary>
/// Starts the MAUI app on Mac Catalyst.
/// </summary>
[Register("AppDelegate")]
#pragma warning disable CA1812 // Avoid uninstantiated internal classes
internal sealed class AppDelegate : MauiUIApplicationDelegate
#pragma warning restore CA1812
{
    private const string SystemQuitAction = "terminate:";
    private const string QuitAction = "quitThroughLifecycle";
    private const string QuitKey = "q";
    private IAppLifecycleService? _lifecycle;

    public override void BuildMenu(IUIMenuBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        base.BuildMenu(builder);

        // The system's Quit (Cmd+Q) ends the process at once; this one leaves through the app's exit steps, like Exit.
        string? quitMenu = UIMenuIdentifier.Quit.GetConstant();

        if (quitMenu is not null)
            builder.ReplaceChildrenOfMenu(quitMenu, ReplaceQuit);
    }

    // Answers the replaced Quit command, which UIKit sends up the responder chain to the app delegate.
    [Export(QuitAction)]
    public void QuitThroughLifecycle()
    {
        _lifecycle ??= IPlatformApplication.Current?.Services.GetService<IAppLifecycleService>();

        // An Objective-C action returns nothing; ExitAsync ends the app when it is done.
        if (_lifecycle is not null)
            _ = _lifecycle.ExitAsync();
    }

    protected override MauiApp CreateMauiApp()
        => MauiProgram.CreateMauiApp();

    // Keeps the system's localised title and Cmd+Q.
    private static UIMenuElement[] ReplaceQuit(UIMenuElement[] children)
    {
        UIMenuElement[] replaced = new UIMenuElement[children.Length];

        for (int i = 0; i < children.Length; i++)
        {
            replaced[i] = (children[i] is UICommand { Action.Name: SystemQuitAction } quit) ? CreateQuit(quit.Title) : children[i];
        }

        return replaced;
    }

    private static UIKeyCommand CreateQuit(string title)
        => UIKeyCommand.Create(title, null, new Selector(QuitAction), QuitKey, UIKeyModifierFlags.Command, null);
}
