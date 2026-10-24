using System;
using System.Threading.Tasks;

using Microsoft.Maui.ApplicationModel;
using Microsoft.Windows.AppLifecycle;

using Tf3Drpc.App.Services.Interfaces;

namespace Tf3Drpc.App.Platforms.Windows;

/// <summary>
/// Single instance through the Windows App SDK: a second start redirects to the first and exits.
/// </summary>
internal sealed class WindowsSingleInstanceService : ISingleInstanceService
{
    private const string InstanceKey = "TF3-DRPC";
    private bool _initialActivationSeen;

    public event EventHandler? ActivationRequested;

    /// <summary>
    /// The handler for MAUI's <c>OnAppInstanceActivated</c> lifecycle hook; <see langword="true"/> means the activation is handled.
    /// </summary>
    public bool HandleActivation(Microsoft.UI.Xaml.Application application, AppActivationArguments arguments)
    {
        ArgumentNullException.ThrowIfNull(application);
        AppInstance keyInstance = AppInstance.FindOrRegisterForKey(InstanceKey);

        if (!keyInstance.IsCurrent)
        {
            _ = RedirectAndExitAsync(application, keyInstance, arguments);

            return true;
        }

        if (_initialActivationSeen)
            MainThread.BeginInvokeOnMainThread(RaiseActivationRequested);

        _initialActivationSeen = true;

        return false;
    }

    private static async Task RedirectAndExitAsync(Microsoft.UI.Xaml.Application application, AppInstance keyInstance, AppActivationArguments arguments)
    {
        try
        {
            await keyInstance.RedirectActivationToAsync(arguments).AsTask();
        }
        finally
        {
            application.Exit();
        }
    }

    private void RaiseActivationRequested()
        => ActivationRequested?.Invoke(this, EventArgs.Empty);
}
