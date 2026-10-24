using System;
using System.ComponentModel;
using System.Diagnostics;

using Tf3Drpc.App.Constants;
using Tf3Drpc.App.Services.Interfaces;
using Tf3Drpc.Core.Structs;

namespace Tf3Drpc.App.Services;

/// <summary>
/// Looks for the game process.
/// </summary>
#pragma warning disable CA1812 // Avoid uninstantiated internal classes
internal sealed class GameProcessService : IGameProcessService
#pragma warning restore CA1812
{
    public GameProcessInfoStruct Check()
    {
        Process[] processes = Process.GetProcessesByName(AppConstants.GameProcessName);

        try
        {
            if (processes.Length is 0)
                return new GameProcessInfoStruct();

            return new GameProcessInfoStruct() { IsRunning = true, ExecutablePath = TryGetExecutablePath(processes[0]), StartTime = TryGetStartTime(processes[0]) };
        }
        finally
        {
            foreach (Process process in processes)
            {
                process.Dispose();
            }
        }
    }

    private static DateTimeOffset? TryGetStartTime(Process process)
    {
        try
        {
            return new DateTimeOffset(process.StartTime).ToUniversalTime();
        }
        catch (Exception exception) when (exception is Win32Exception or InvalidOperationException or NotSupportedException)
        {
            return null;
        }
    }

    private static string? TryGetExecutablePath(Process process)
    {
        try
        {
            return process.MainModule?.FileName;
        }
        catch (Exception exception) when (exception is Win32Exception or InvalidOperationException or NotSupportedException)
        {
            return null;
        }
    }
}
