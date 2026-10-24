using System;
using System.IO;
using System.Security;

using Microsoft.Win32;

using Tf3Drpc.App.Services;

namespace Tf3Drpc.App.Platforms.Windows;

/// <summary>
/// Finds the app's data folder and the game's files on Windows; the registry is only read.
/// </summary>
internal sealed class WindowsPathService : PathServiceBase
{
    private const string SteamRegistryKey = @"HKEY_CURRENT_USER\Software\Valve\Steam";

    public WindowsPathService() : base(AppContext.BaseDirectory)
    { }

    protected override string? FindSteamDirectory()
    {
        try
        {
            string? value = Registry.GetValue(SteamRegistryKey, "SteamPath", null) as string;

            return (string.IsNullOrWhiteSpace(value)) ? null : Path.GetFullPath(value);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or SecurityException or ArgumentException or NotSupportedException)
        {
            return null;
        }
    }
}
