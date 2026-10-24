using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Threading;

using Tf3Drpc.App.Constants;
using Tf3Drpc.App.Services.Interfaces;
using Tf3Drpc.Core.Structs;
using Tf3Drpc.Core.Utilities;

namespace Tf3Drpc.App.Services;

/// <summary>
/// What the path services of both platforms share: the data folder, its write probes and the game's files under Steam.
/// </summary>
internal abstract class PathServiceBase : IPathService
{
    private volatile bool _isDataWritable;
    private int _logFileFailed;

    protected PathServiceBase(string portableDirectory)
    {
        DataDirectory = Path.Combine(portableDirectory, AppConstants.DataFolderName);
        LogDirectory = Path.Combine(DataDirectory, AppConstants.LogFolderName);
        SettingsFilePath = Path.Combine(DataDirectory, AppConstants.SettingsFileName);
        _isDataWritable = ProbeDirectory(DataDirectory);
        IsLogDirectoryWritable = _isDataWritable && ProbeDirectory(LogDirectory);
        _logFileFailed = (IsLogDirectoryWritable) ? 0 : 1;
    }

    public string DataDirectory { get; }

    public string LogDirectory { get; }

    public string SettingsFilePath { get; }

    public virtual bool IsTranslocated
        => false;

    public bool IsDataWritable
        => _isDataWritable;

    public bool IsLogDirectoryWritable { get; }

    public bool IsLogFileAvailable
        => Volatile.Read(ref _logFileFailed) is 0;

    public bool RefreshDataWritable()
    {
        _isDataWritable = ProbeDirectory(DataDirectory);

        return _isDataWritable;
    }

    public bool ReportLogFileFailure()
        => Interlocked.Exchange(ref _logFileFailed, 1) is 0;

    public GamePathsStruct ResolveGamePaths(string? runningExecutablePath)
    {
        string? steam = FindSteamDirectory();
        string? userData = (steam is null) ? null : FindUserDataDirectory(steam, TimeProvider.System.GetUtcNow().UtcDateTime);
        string? logPath = (userData is null) ? null : Path.Combine(userData, "crash_dump", "stdout.txt");
        string? profilePath = (userData is null) ? null : Path.Combine(userData, "profile.lua");

        return new GamePathsStruct() { LogPath = logPath, ProfilePath = profilePath, GameDirectory = FindGameDirectory(steam, runningExecutablePath) };
    }

    /// <summary>
    /// The Steam folder of the user, or <see langword="null"/> when it cannot be found.
    /// </summary>
    protected abstract string? FindSteamDirectory();

    private static string? FindGameDirectory(string? steam, string? runningExecutablePath)
    {
        string? fromProcess = (runningExecutablePath is null) ? null : Path.GetDirectoryName(runningExecutablePath);

        if (fromProcess is not null)
            return fromProcess;

        if (steam is null)
            return null;

        string fallback = Path.Combine(steam, "steamapps", "common", AppConstants.GameFolderName);

        return (Directory.Exists(fallback)) ? fallback : null;
    }

    private static string? FindUserDataDirectory(string steam, in DateTime utcNow)
    {
        string root = Path.Combine(steam, "userdata");
        List<UserDataCandidateStruct> candidates = [];

        try
        {
            foreach (string account in Directory.EnumerateDirectories(root))
            {
                string local = Path.Combine(account, AppConstants.GameSteamAppId, "local");

                if (!Directory.Exists(local))
                    continue;

                string log = Path.Combine(local, "crash_dump", "stdout.txt");
                DateTime? written = (File.Exists(log)) ? File.GetLastWriteTimeUtc(log) : null;
                candidates.Add(new UserDataCandidateStruct() { Folder = local, LogWrittenUtc = written });
            }
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or SecurityException)
        {
            return null;
        }

        return UserDataUtils.ChooseFolder(candidates, utcNow);
    }

    private static bool ProbeDirectory(string directory)
    {
        string probe = Path.Combine(directory, AppConstants.WriteProbePrefix + Guid.NewGuid().ToString("N"));

        try
        {
            Directory.CreateDirectory(directory);
            File.WriteAllBytes(probe, []);
            File.Delete(probe);

            return true;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or SecurityException or ArgumentException or NotSupportedException)
        {
            return false;
        }
    }
}
