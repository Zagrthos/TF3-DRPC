namespace Tf3Drpc.Core.Enums;

/// <summary>
/// A ready-made layout of the presence while playing.
/// </summary>
internal enum PresencePreset
{
    /// <summary>
    /// Rank and date of the company.
    /// </summary>
    Career,

    /// <summary>
    /// Lines and vehicles, stations and year.
    /// </summary>
    Network,

    /// <summary>
    /// Year and climate, with the rank.
    /// </summary>
    Era,

    /// <summary>
    /// Changes between the other presets at an interval.
    /// </summary>
    Alternating,

    /// <summary>
    /// Only that the game runs, with the year.
    /// </summary>
    Minimal,

    /// <summary>
    /// The templates the user wrote.
    /// </summary>
    Custom
}
