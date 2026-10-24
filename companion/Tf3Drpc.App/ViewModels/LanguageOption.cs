namespace Tf3Drpc.App.ViewModels;

/// <summary>
/// One entry of the language picker: <c>game</c> or a language code with its display name.
/// </summary>
/// <param name="Code">The language setting this entry selects.</param>
/// <param name="DisplayName">The text shown in the picker.</param>
internal sealed record class LanguageOption(string Code, string DisplayName);
