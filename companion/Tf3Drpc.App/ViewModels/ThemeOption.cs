using Tf3Drpc.Core.Enums;

namespace Tf3Drpc.App.ViewModels;

/// <summary>
/// One entry of the theme picker: a theme with its display name.
/// </summary>
/// <param name="Theme">The theme this entry selects.</param>
/// <param name="DisplayName">The text shown in the picker.</param>
internal sealed record class ThemeOption(ThemeMode Theme, string DisplayName);
