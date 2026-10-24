using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

using Foundation;

using Tf3Drpc.App.Services;

namespace Tf3Drpc.App.Platforms.MacCatalyst;

/// <summary>
/// Finds the app's data folder and the game's files on macOS.
/// </summary>
internal sealed partial class MacPathService : PathServiceBase
{
    private const string BundleExtension = ".app";

    public MacPathService() : base(FindPortableDirectory())
        => IsTranslocated = DetectTranslocation();

    public override bool IsTranslocated { get; }

    [LibraryImport("/System/Library/Frameworks/Security.framework/Security")]
    [return: MarshalAs(UnmanagedType.U1)]
    private static partial bool SecTranslocateIsTranslocatedURL(nint path, [MarshalAs(UnmanagedType.U1)] out bool isTranslocated, nint error);

    // A quarantined app that was not moved by the user runs from a hidden read-only copy; its URL tells.
    private static bool DetectTranslocation()
    {
        nint bundleUrl = NSBundle.MainBundle.BundleUrl.Handle;

        return SecTranslocateIsTranslocatedURL(bundleUrl, out bool isTranslocated, 0) && isTranslocated;
    }

    // The folder that holds the .app bundle, never the bundle itself; the app is portable and a bundle may be replaced on update.
    private static string FindPortableDirectory()
    {
        string? directory = Path.TrimEndingDirectorySeparator(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (directory.EndsWith(BundleExtension, StringComparison.OrdinalIgnoreCase))
                return Path.GetDirectoryName(directory) ?? AppContext.BaseDirectory;

            directory = Path.GetDirectoryName(directory);
        }

        return AppContext.BaseDirectory;
    }

    protected override string? FindSteamDirectory()
    {
        try
        {
            string profile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            if (string.IsNullOrWhiteSpace(profile))
                return null;

            string steam = Path.Combine(profile, "Library", "Application Support", "Steam");

            return (Directory.Exists(steam)) ? steam : null;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or SecurityException or ArgumentException or NotSupportedException)
        {
            return null;
        }
    }
}
