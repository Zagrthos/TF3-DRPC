using System.Globalization;

namespace Tf3Drpc.Core.Languages;

/// <summary>
/// One of the game's languages with its native name and culture.
/// </summary>
public sealed record LanguageInfo(string Code, string NativeName, CultureInfo Culture);
