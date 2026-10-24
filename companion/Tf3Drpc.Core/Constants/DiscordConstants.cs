namespace Tf3Drpc.Core.Constants;

/// <summary>
/// Fixed values and limits of the Discord presence.
/// </summary>
public static class DiscordConstants
{
    public const string LargeImageKey = "cover";

    public const string LargeImageText = "Transport Fever 3";

    public const int MaxTextLength = 128;

    public const int MinTextLength = 2;

    public const string Separator = " · ";

    public const string Ellipsis = "…";

    public const char PaddingCharacter = ' ';

    public const int MaxButtonLabelBytes = 31;

#pragma warning disable S1075 // The store page of the game is a fixed address
    public const string StoreUrl = "https://store.steampowered.com/app/3493540";
#pragma warning restore S1075
}
