using System;
using System.Threading.Tasks;

namespace Tf3Drpc.App.Services.Interfaces;

/// <summary>
/// Ends the app in an orderly way.
/// </summary>
internal interface IAppLifecycleService
{
    /// <summary>
    /// Raised on the UI thread after the presence was cleared, just before the app quits.
    /// </summary>
    event EventHandler? Exiting;

    /// <summary>
    /// Stops the loop, clears the presence, removes the tray icon and quits.
    /// </summary>
    Task ExitAsync();
}
