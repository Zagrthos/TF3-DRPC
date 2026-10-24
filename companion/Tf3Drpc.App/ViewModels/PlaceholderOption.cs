namespace Tf3Drpc.App.ViewModels;

/// <summary>
/// One entry of the placeholder list: a placeholder with its explanation.
/// </summary>
/// <param name="Name">The placeholder as written in a template, without any marker.</param>
/// <param name="Description">The explanation in the current language.</param>
/// <param name="IsPrivate">Whether the placeholder is private; the page marks it then.</param>
internal sealed record class PlaceholderOption(string Name, string Description, bool IsPrivate);
