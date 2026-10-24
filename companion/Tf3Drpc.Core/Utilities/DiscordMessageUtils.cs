using System;
using System.Text.Json;

using Tf3Drpc.Core.Constants;
using Tf3Drpc.Core.Discord;
using Tf3Drpc.Core.Serialization;
using Tf3Drpc.Core.Structs;

namespace Tf3Drpc.Core.Utilities;

/// <summary>
/// Builds and reads the JSON payloads of the Discord IPC protocol.
/// </summary>
internal static class DiscordMessageUtils
{
    public static byte[] BuildHandshake()
    {
        DiscordHandshake handshake = new(DiscordIpcConstants.ProtocolVersion, DiscordIpcConstants.ApplicationId);

        return JsonSerializer.SerializeToUtf8Bytes(handshake, JsonSourceGenContext.Default.DiscordHandshake);
    }

    /// <summary>
    /// Builds a SET_ACTIVITY command; <see langword="null"/> content clears the activity.
    /// </summary>
    public static byte[] BuildSetActivity(PresenceContentStruct? content, string nonce)
    {
        DiscordActivity? activity = null;

        if (content is PresenceContentStruct presence)
        {
            DiscordTimestamps? timestamps = (presence.StartTimestamp is DateTimeOffset start)
                ? new DiscordTimestamps(start.ToUnixTimeMilliseconds())
                : null;
            DiscordAssets assets = new(DiscordConstants.LargeImageKey, presence.LargeImageText ?? DiscordConstants.LargeImageText);
            DiscordButton[]? buttons = (presence.ButtonLabel is string label) ? [new DiscordButton(label, DiscordConstants.StoreUrl)] : null;
            activity = new DiscordActivity(presence.Details, presence.State, timestamps, assets, Instance: false, buttons);
        }

        DiscordCommandArguments arguments = new(Environment.ProcessId, activity);
        DiscordCommand command = new(DiscordIpcConstants.SetActivityCommand, nonce, arguments);

        return JsonSerializer.SerializeToUtf8Bytes(command, JsonSourceGenContext.Default.DiscordCommand);
    }

    /// <summary>
    /// Reads an incoming message; anything that is not a JSON object of the expected shape gives <see langword="null"/>.
    /// </summary>
    public static DiscordMessage? ParseMessage(ReadOnlySpan<byte> payload)
    {
        try
        {
            return JsonSerializer.Deserialize(payload, JsonSourceGenContext.Default.DiscordMessage);
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
