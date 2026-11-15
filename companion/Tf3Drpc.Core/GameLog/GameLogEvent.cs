using Tf3Drpc.Core.Status;

namespace Tf3Drpc.Core.GameLog;

/// <summary>
/// A game log line the companion reacts to, with the values taken from it.
/// </summary>
internal closed record GameLogEvent;

/// <summary>
/// The game started.
/// </summary>
internal sealed record StartupEvent : GameLogEvent;

/// <summary>
/// The game reached the main menu.
/// </summary>
internal sealed record MainMenuEvent : GameLogEvent;

/// <summary>
/// The game started loading a save game.
/// </summary>
/// <param name="SaveName">The name of the save game.</param>
internal sealed record LoadingGameEvent(string SaveName) : GameLogEvent;

/// <summary>
/// The game started loading a mission.
/// </summary>
/// <param name="MissionNumber">The number of the mission, or <see langword="null"/> when the line has none.</param>
internal sealed record MissionLoadingEvent(int? MissionNumber) : GameLogEvent;

/// <summary>
/// The game finished loading and is ready to play.
/// </summary>
internal sealed record GameReadyEvent : GameLogEvent;

/// <summary>
/// The running game was stopped and went back to the menu.
/// </summary>
internal sealed record GameStoppedEvent : GameLogEvent;

/// <summary>
/// The game is shutting down.
/// </summary>
/// <param name="IsCrash">Whether the game exited ungracefully.</param>
internal sealed record ClosingEvent(bool IsCrash) : GameLogEvent;

/// <summary>
/// The mod wrote a status line.
/// </summary>
/// <param name="Status">The status taken from the line.</param>
internal sealed record StatusEvent(ModStatus Status) : GameLogEvent;

/// <summary>
/// The log named the game build.
/// </summary>
/// <param name="GameBuild">The build number.</param>
internal sealed record GameBuildEvent(int GameBuild) : GameLogEvent;
