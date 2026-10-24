using Tf3Drpc.Core.Structs;

namespace Tf3Drpc.App.Messages;

/// <summary>
/// Message sent when the game state, the Discord connection, the resolved language or the presence changed.
/// </summary>
/// <param name="Status">The new status.</param>
internal sealed record class AppStatusChangedMessage(AppStatusStruct Status);
