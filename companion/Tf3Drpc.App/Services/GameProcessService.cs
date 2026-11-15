using System;
using System.ComponentModel;
using System.Diagnostics;

using Tf3Drpc.App.Constants;
using Tf3Drpc.App.Services.Interfaces;
using Tf3Drpc.Core.Structs;

namespace Tf3Drpc.App.Services;

/// <summary>
/// Looks for the game process; call it from one loop only.
/// </summary>
/// <remarks>
/// Once a search by name has found the game with its path and start time, later checks look up only its process ID,
/// and the start time tells the same process from a new one that got the ID again.
/// </remarks>
#pragma warning disable CA1812 // Avoid uninstantiated internal classes
internal sealed class GameProcessService : IGameProcessService
#pragma warning restore CA1812
{
    private int? _processId;
    private GameProcessInfoStruct _game;

    public GameProcessInfoStruct Check()
    {
        if ((_processId is int processId) && IsStillRunning(processId))
            return _game;

        _processId = null;

        return Search();
    }

    private GameProcessInfoStruct Search()
    {
        Process[] processes = Process.GetProcessesByName(AppConstants.GameProcessName);

        try
        {
            if (processes.Length is 0)
                return new GameProcessInfoStruct();

            Process process = processes[0];
            GameProcessInfoStruct game = new() { IsRunning = true, ExecutablePath = TryGetExecutablePath(process), StartTime = TryGetStartTime(process) };

            // Without the path or the start time the next check searches again, as the values may be readable then.
            if ((game.ExecutablePath is not null) && (game.StartTime is not null))
            {
                _processId = process.Id;
                _game = game;
            }

            return game;
        }
        finally
        {
            foreach (Process process in processes)
            {
                process.Dispose();
            }
        }
    }

    private bool IsStillRunning(int processId)
    {
        if (!Process.TryGetProcessById(processId, out Process? process))
            return false;

        using (process)
        {
            return !HasExited(process) && (TryGetStartTime(process) == _game.StartTime);
        }
    }

    private static bool HasExited(Process process)
    {
        try
        {
            return process.HasExited;
        }
        catch (Exception exception) when (exception is Win32Exception or InvalidOperationException or NotSupportedException)
        {
            return true;
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
