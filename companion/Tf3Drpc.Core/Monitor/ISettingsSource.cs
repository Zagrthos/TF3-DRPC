using Tf3Drpc.Core.Settings;

namespace Tf3Drpc.Core.Monitor;

/// <summary>
/// Gives the current settings.
/// </summary>
public interface ISettingsSource
{
    /// <summary>
    /// The current settings.
    /// </summary>
    AppSettings Current { get; }
}
