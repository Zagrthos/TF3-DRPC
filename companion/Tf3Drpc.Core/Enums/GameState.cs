namespace Tf3Drpc.Core.Enums;

/// <summary>
/// What the game is doing, as far as the process check and the game log show.
/// </summary>
internal enum GameState
{
    /// <summary>
    /// The game process does not run.
    /// </summary>
    NotRunning,

    /// <summary>
    /// The game is starting.
    /// </summary>
    Starting,

    /// <summary>
    /// The game shows the main menu.
    /// </summary>
    MainMenu,

    /// <summary>
    /// The game loads a save game or a mission.
    /// </summary>
    Loading,

    /// <summary>
    /// A mission is running.
    /// </summary>
    Mission,

    /// <summary>
    /// A save game is running.
    /// </summary>
    Playing,

    /// <summary>
    /// The game is shutting down.
    /// </summary>
    Closing
}
