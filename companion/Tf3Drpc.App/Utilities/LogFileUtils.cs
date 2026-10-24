using System;
using System.IO;
using System.Security;

using Tf3Drpc.App.Constants;

namespace Tf3Drpc.App.Utilities;

/// <summary>
/// Deletes old daily log files; other files in the folder are left alone.
/// </summary>
internal static class LogFileUtils
{
    /// <summary>
    /// Deletes <c>TF3-DRPC_*.log</c> files older than the retention time in <paramref name="logDirectory"/>.
    /// </summary>
    public static void DeleteOldFiles(string logDirectory, in DateTime nowUtc)
    {
        try
        {
            if (!Directory.Exists(logDirectory))
                return;

            const string pattern = AppConstants.LogFilePrefix + "*" + AppConstants.LogFileExtension;
            DateTime limit = nowUtc.AddDays(-AppConstants.LogRetentionDays);

            string[] files = Directory.GetFiles(logDirectory, pattern);

            for (int i = 0; i < files.Length; i++)
            {
                DeleteIfOlder(files[i], limit);
            }
        }
        catch (Exception exception) when (IsExpectedFailure(exception))
        {
            // Cleaning up is best effort; the next start tries again.
        }
    }

    private static void DeleteIfOlder(string file, in DateTime limit)
    {
        try
        {
            if (File.GetLastWriteTimeUtc(file) >= limit)
                return;

            File.Delete(file);
        }
        catch (Exception exception) when (IsExpectedFailure(exception))
        {
            // A locked file stays; the other old files are still cleaned up.
        }
    }

    private static bool IsExpectedFailure(Exception exception)
        => exception is IOException or UnauthorizedAccessException or SecurityException;
}
