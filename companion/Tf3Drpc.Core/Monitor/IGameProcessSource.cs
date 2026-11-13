using Tf3Drpc.Core.Structs;

namespace Tf3Drpc.Core.Monitor;

/// <summary>
/// Checks whether the game runs.
/// </summary>
internal interface IGameProcessSource
{
    /// <summary>
    /// Looks for the game process.
    /// </summary>
    GameProcessInfoStruct Check();
}
