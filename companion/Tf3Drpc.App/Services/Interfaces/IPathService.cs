using Tf3Drpc.Core.Monitor;

namespace Tf3Drpc.App.Services.Interfaces;

/// <summary>
/// Knows where the app's own files and the game's files are.
/// </summary>
internal interface IPathService : IGamePathResolver
{
    /// <summary>
    /// The folder next to the executable that holds everything the app writes.
    /// </summary>
    string DataDirectory { get; }

    /// <summary>
    /// The folder of the daily log files.
    /// </summary>
    string LogDirectory { get; }

    /// <summary>
    /// The path of the settings file.
    /// </summary>
    string SettingsFilePath { get; }

    /// <summary>
    /// Tells whether the OS started the app from a hidden read-only copy (macOS App Translocation), so the data folder is not next to the real app.
    /// </summary>
    bool IsTranslocated { get; }

    /// <summary>
    /// Tells whether the data folder exists and can be written to.
    /// </summary>
    bool IsDataWritable { get; }

    /// <summary>
    /// Tells whether the log folder exists (or can be created) and can be written to.
    /// </summary>
    bool IsLogDirectoryWritable { get; }

    /// <summary>
    /// Tells whether the daily log file is being written; it turns false for good at the first failure.
    /// </summary>
    bool IsLogFileAvailable { get; }

    /// <summary>
    /// Probes the data folder again and returns whether it can be written to now.
    /// </summary>
    /// <remarks>The file logger is not created later, so <see cref="IsLogFileAvailable"/> does not recover.</remarks>
    bool RefreshDataWritable();

    /// <summary>
    /// Records that the daily log file cannot be written; returns true only for the first report.
    /// </summary>
    bool ReportLogFileFailure();
}
