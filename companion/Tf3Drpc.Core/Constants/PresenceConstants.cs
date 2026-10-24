using System;

namespace Tf3Drpc.Core.Constants;

/// <summary>
/// Fixed values of the presence templates and presets.
/// </summary>
public static class PresenceConstants
{
    /// <summary>
    /// How long the alternating preset shows one layout.
    /// </summary>
    public static readonly TimeSpan AlternatingInterval = TimeSpan.FromSeconds(45);

    /// <summary>
    /// The longest template that is kept.
    /// </summary>
    public const int MaxTemplateLength = 512;

    internal const string DefaultDetailsTemplate = "{rank} · {mode}";

    internal const string DefaultStateTemplate = "{date}";

    internal const char PartSeparator = '·';

    internal const char PlaceholderStart = '{';

    internal const char PlaceholderEnd = '}';

    internal const int AlternatingLayoutCount = 3;

    internal const int MinYear = 1;

    internal const int MaxYear = 9999;

    internal const string NumberFormat = "N0";

    internal const string CurrencySymbol = "$";

    internal const decimal MoneyScaleStep = 1000m;

    internal const string MoneyDecimalFormat = "0.#";
}
