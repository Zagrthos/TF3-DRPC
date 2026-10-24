namespace Tf3Drpc.Core.Enums;

/// <summary>
/// The opcode of a Discord IPC frame.
/// </summary>
internal enum DiscordOpcode
{
    Handshake = 0,
    Frame = 1,
    Close = 2,
    Ping = 3,
    Pong = 4
}
