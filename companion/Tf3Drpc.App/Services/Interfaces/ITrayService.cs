using System;

namespace Tf3Drpc.App.Services.Interfaces;

/// <summary>
/// The tray icon with its menu.
/// </summary>
internal interface ITrayService : IDisposable
{
    /// <summary>
    /// Creates the icon; call it on the UI thread. Returns <see langword="false"/> when the icon could not be created.
    /// </summary>
    bool Show();
}
