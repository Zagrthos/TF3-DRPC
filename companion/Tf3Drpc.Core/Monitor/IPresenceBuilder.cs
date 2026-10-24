using Tf3Drpc.Core.Languages;
using Tf3Drpc.Core.Structs;

namespace Tf3Drpc.Core.Monitor;

/// <summary>
/// Builds the Discord presence from the game state.
/// </summary>
public interface IPresenceBuilder
{
    /// <summary>
    /// Builds the presence; <see langword="false"/> means it is cleared.
    /// </summary>
    bool TryBuild(in GameStateSnapshotStruct snapshot, LanguageInfo language, out PresenceContentStruct content);
}
