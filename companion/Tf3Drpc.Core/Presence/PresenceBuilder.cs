using System;
using System.Globalization;

using Tf3Drpc.Core.Constants;
using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Resources;
using Tf3Drpc.Core.Settings;
using Tf3Drpc.Core.Status;
using Tf3Drpc.Core.Structs;
using Tf3Drpc.Core.Translation;
using Tf3Drpc.Core.Utilities;

namespace Tf3Drpc.Core.Presence;

/// <summary>
/// Builds the Discord presence from a game state.
/// </summary>
public static class PresenceBuilder
{
    private static readonly PresenceTemplates DefaultTemplates = new();

    #region Building

    /// <summary>
    /// Builds the presence; <see langword="false"/> means the presence is cleared.
    /// </summary>
    /// <remarks>
    /// A state shorter than two characters is dropped, a shorter details text is padded with a no-break space, because Discord rejects both.
    /// </remarks>
    public static bool TryBuild
    (
        in GameStateSnapshotStruct snapshot,
        CultureInfo culture,
        GameTexts gameTexts,
        out PresenceContentStruct content
    )
        => TryBuild(snapshot, culture, gameTexts, new PresenceOptionsStruct() { Preset = PresencePreset.Career }, out content);

    /// <summary>
    /// Builds the presence with a preset; <see langword="false"/> means the presence is cleared.
    /// </summary>
    /// <param name="snapshot">The game state.</param>
    /// <param name="culture">The culture of the texts and numbers.</param>
    /// <param name="gameTexts">The game's own texts.</param>
    /// <param name="preset">The layout while playing.</param>
    /// <param name="rotationStep">The step of the alternating preset, counted in <see cref="PresenceConstants.AlternatingInterval"/>.</param>
    /// <param name="content">The presence.</param>
    public static bool TryBuild
    (
        in GameStateSnapshotStruct snapshot,
        CultureInfo culture,
        GameTexts gameTexts,
        PresencePreset preset,
        long rotationStep,
        out PresenceContentStruct content
    )
        => TryBuild(snapshot, culture, gameTexts, new PresenceOptionsStruct() { Preset = preset, RotationStep = rotationStep }, out content);

    /// <summary>
    /// Builds the presence with options; <see langword="false"/> means the presence is cleared.
    /// </summary>
    /// <param name="snapshot">The game state.</param>
    /// <param name="culture">The culture of the texts and numbers.</param>
    /// <param name="gameTexts">The game's own texts.</param>
    /// <param name="options">The layout while playing, the step of the alternating preset and the templates of the custom preset.</param>
    /// <param name="content">The presence.</param>
    public static bool TryBuild
    (
        in GameStateSnapshotStruct snapshot,
        CultureInfo culture,
        GameTexts gameTexts,
        in PresenceOptionsStruct options,
        out PresenceContentStruct content
    )
    {
        ArgumentNullException.ThrowIfNull(culture);
        ArgumentNullException.ThrowIfNull(gameTexts);

        content = default;
        string? state = null;
        string? hover = null;
        string details;

        switch (snapshot.State)
        {
            case GameState.Starting:
                details = OwnTexts.Get(OwnText.StartingTheGame, culture);
                break;

            case GameState.MainMenu:
                details = OwnTexts.Get(OwnText.InTheMainMenu, culture);
                break;

            case GameState.Loading:
                details = gameTexts.GetLabel(GameLabel.Loading);
                break;

            case GameState.Mission:
                details = BuildMissionDetails(snapshot.MissionNumber, gameTexts);
                break;

            case GameState.Playing:
                details = OwnTexts.Get(OwnText.Playing, culture);

                if (snapshot.Status is ModStatus original)
                {
                    ModStatus status = TranslateNewGame(original, options.GameLanguageTexts, gameTexts);
                    PresencePreset shown = PresencePresets.Resolve(options.Preset, options.RotationStep);
                    PresenceTemplates templates = options.Templates ?? DefaultTemplates;
                    bool isCustom = shown is PresencePreset.Custom;
                    string detailsTemplate = (isCustom) ? templates.Details : PresencePresets.GetDetails(shown, culture, gameTexts);
                    string? stateTemplate = (isCustom) ? templates.State : PresencePresets.GetState(shown, gameTexts);

                    details = PresenceTemplate.Render(detailsTemplate, status, culture, gameTexts) ?? details;
                    state = BuildPlayingState(stateTemplate, status, snapshot.IsPaused, culture, gameTexts);

                    if (isCustom)
                        hover = FixHover(PresenceTemplate.Render(templates.Hover, status, culture, gameTexts));
                }

                break;

            default:
                return false;
        }

        content = new PresenceContentStruct()
        {
            Details = FixDetails(details),
            State = FixState(state),
            StartTimestamp = snapshot.TimerStart,
            LargeImageText = hover,
            ButtonLabel = (options.ShowStoreButton) ? FixButtonLabel(OwnTexts.Get(OwnText.StoreButton, culture)) : null
        };

        return true;
    }

    #endregion Building

    #region Playing

    private static string BuildMissionDetails(int? missionNumber, GameTexts gameTexts)
    {
        string campaign = gameTexts.GetLabel(GameLabel.Campaign);
        string? name = (missionNumber is int number) ? gameTexts.GetMissionName(number) : null;

        return (name is null) ? campaign : (campaign + DiscordConstants.Separator + name);
    }

    /// <summary>
    /// Shows the name the game gives an unsaved game in the game language in the chosen language.
    /// </summary>
    private static ModStatus TranslateNewGame(ModStatus status, GameTexts? gameLanguageTexts, GameTexts gameTexts)
    {
        if ((gameLanguageTexts is null) || (status.SaveName is null))
            return status;

        if (!string.Equals(status.SaveName, gameLanguageTexts.GetLabel(GameLabel.NewGame), StringComparison.Ordinal))
            return status;

        return status with { SaveName = gameTexts.GetLabel(GameLabel.NewGame) };
    }

    private static string? BuildPlayingState(string? template, ModStatus status, bool isPaused, CultureInfo culture, GameTexts gameTexts)
    {
        string? state = (template is null) ? null : PresenceTemplate.Render(template, status, culture, gameTexts);

        if (!isPaused)
            return state;

        string paused = gameTexts.GetLabel(GameLabel.Paused);

        if (state is null)
            return paused;

        // Cut the rendered text first, so that a long second line cannot push the suffix out of the limit.
        string suffix = DiscordConstants.Separator + paused;

        // A label too long for the limit leaves no room for the text; the final truncation then shortens the whole.
        return (suffix.Length >= DiscordConstants.MaxTextLength) ? (state + suffix) : (Truncate(state, DiscordConstants.MaxTextLength - suffix.Length) + suffix);
    }

    #endregion Playing

    #region Text limits

    private static string FixDetails(string details)
    {
        string text = Truncate(details);

        return (text.Length is < DiscordConstants.MinTextLength) ? text.PadRight(DiscordConstants.MinTextLength, DiscordConstants.PaddingCharacter) : text;
    }

    private static string? FixState(string? state)
    {
        if (state is null)
            return null;

        string text = Truncate(state);

        return (text.Length is < DiscordConstants.MinTextLength) ? null : text;
    }

    private static string? FixHover(string? hover)
    {
        if (hover is null)
            return null;

        string text = Truncate(hover);

        return (string.IsNullOrWhiteSpace(text) || (text.Length is < DiscordConstants.MinTextLength)) ? null : text;
    }

    private static string FixButtonLabel(string label)
        => TextUtils.CutToUtf8Bytes(label, DiscordConstants.MaxButtonLabelBytes);

    private static string Truncate(string text)
        => Truncate(text, DiscordConstants.MaxTextLength);

    private static string Truncate(string text, int maxLength)
    {
        if (text.Length <= maxLength)
            return text;

        int budget = maxLength - DiscordConstants.Ellipsis.Length;
        ReadOnlySpan<char> rest = text;
        int length = 0;

        while (!rest.IsEmpty)
        {
            int element = System.Globalization.StringInfo.GetNextTextElementLength(rest);

            if ((length + element) > budget)
                break;

            length += element;
            rest = rest[element..];
        }

        return string.Concat(text.AsSpan(0, length), DiscordConstants.Ellipsis);
    }

    #endregion Text limits
}
