namespace Tf3Drpc.Core.Constants;

/// <summary>
/// Limits, tuning values and fixed texts of the game log pipeline.
/// </summary>
internal static class GameLogConstants
{
    public const int MaxReadBytesPerPoll = 4 * 1024 * 1024;

    public const int HeadFingerprintBytes = 256;

    public const int MaxLineBytes = 1024 * 1024;

    public const long SupportedStatusVersion = 1;

    public const string StatusPrefix = "[drpc-status] ";

    public const string StartupMessage = "Application startup. Hello!";

    public const string MainMenuMessage = "Main menu is ready";

    public const string LoadingGamePrefix = "Loading game from file ";

    public const string MissionPrefix = "Campaign/mission: ";

    public const string GameBuildPrefix = "Starting up build version: ";

    public const string GameReadyMessage = "Game is ready";

    public const string GameStoppedMessage = "Game is stopped";

    public const string NormalShutdownMessage = "Normal shutdown. Bye!";

    public const string UngracefulExitMessage = "Ungraceful exit";
}
