namespace Tf3Drpc.Core.Enums;

/// <summary>
/// What the game is doing, as far as the process check and the game log show.
/// </summary>
public enum GameState
{
    NotRunning,
    Starting,
    MainMenu,
    Loading,
    Mission,
    Playing,
    Closing
}
