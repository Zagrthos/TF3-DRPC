namespace Tf3Drpc.App.ViewModels;

/// <summary>
/// A picker entry that can describe itself in a tooltip.
/// </summary>
internal interface IDescribedOption
{
    /// <summary>
    /// The text of the tooltip, built when it is read; <see langword="null"/> or empty means no tooltip.
    /// </summary>
    string? Description { get; }
}
