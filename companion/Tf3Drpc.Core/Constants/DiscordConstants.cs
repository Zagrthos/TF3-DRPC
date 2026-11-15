namespace Tf3Drpc.Core.Constants;

/// <summary>
/// Fixed values and limits of the Discord presence.
/// </summary>
internal static class DiscordConstants
{
    /// <summary>
    /// The key of the large image in the Discord application.
    /// </summary>
    public const string LargeImageKey = "cover";

    /// <summary>
    /// The hover text of the large image.
    /// </summary>
    public const string LargeImageText = "Transport Fever 3";

    /// <summary>
    /// The most characters Discord accepts in a presence line.
    /// </summary>
    public const int MaxTextLength = 128;

    /// <summary>
    /// The fewest characters Discord accepts in a presence line.
    /// </summary>
    public const int MinTextLength = 2;

    /// <summary>
    /// The middle dot that separates the parts of a presence line.
    /// </summary>
    public const string Separator = " · ";

    /// <summary>
    /// The character that marks a shortened text.
    /// </summary>
    public const string Ellipsis = "…";

    /// <summary>
    /// The character that pads a text that is too short.
    /// </summary>
    public const char PaddingCharacter = ' ';

    /// <summary>
    /// The most UTF-8 bytes Discord accepts in a button label.
    /// </summary>
    public const int MaxButtonLabelBytes = 31;

#pragma warning disable S1075 // The store page of the game is a fixed address
    /// <summary>
    /// The store page of the game, which the button opens.
    /// </summary>
    public const string StoreUrl = "https://store.steampowered.com/app/3493540";
#pragma warning restore S1075
}
