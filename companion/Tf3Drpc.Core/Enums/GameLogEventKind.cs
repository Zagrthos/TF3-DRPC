namespace Tf3Drpc.Core.Enums;

/// <summary>
/// The kinds of game log lines the companion reacts to.
/// </summary>
public enum GameLogEventKind
{
    Startup,
    MainMenu,
    LoadingGame,
    MissionLoading,
    GameReady,
    GameStopped,
    Closing,
    Status,
    GameBuild
}
