using Tf3Drpc.Core.Structs;

namespace Tf3Drpc.Core.Monitor;

/// <summary>
/// Knows where the game's files are.
/// </summary>
public interface IGamePathResolver
{
    /// <summary>
    /// Looks for the game's log, profile and folder; <paramref name="runningExecutablePath"/> is the path of the running game, if known.
    /// </summary>
    GamePathsStruct ResolveGamePaths(string? runningExecutablePath);
}
