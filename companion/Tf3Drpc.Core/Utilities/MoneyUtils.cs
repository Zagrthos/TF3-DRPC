using System;
using System.Globalization;

using Tf3Drpc.Core.Constants;

namespace Tf3Drpc.Core.Utilities;

/// <summary>
/// Formats amounts of money.
/// </summary>
internal static class MoneyUtils
{
    private const string Units = "KMBT";

    // Beyond this a decimal overflows; the display is clamped, nobody plays with more.
    private const double MaxAmount = 1e27;

    /// <summary>
    /// Formats an amount compactly, such as <c>$15.6M</c>; <see langword="null"/> for NaN or infinity.
    /// </summary>
    /// <remarks>
    /// The unit is picked after rounding, so 999,950 gives <c>$1M</c> and not <c>$1000K</c>.
    /// </remarks>
    public static string? FormatCompact(double amount, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(culture);

        if (!double.IsFinite(amount))
            return null;

        decimal magnitude = (decimal)Math.Min(Math.Abs(amount), MaxAmount);
        decimal whole = Math.Round(magnitude, MidpointRounding.AwayFromZero);
        string sign = ((amount < 0) && (whole is not 0m)) ? culture.NumberFormat.NegativeSign : string.Empty;

        if (whole < PresenceConstants.MoneyScaleStep)
            return sign + PresenceConstants.CurrencySymbol + whole.ToString(PresenceConstants.NumberFormat, culture);

        int unit = 0;
        decimal divisor = PresenceConstants.MoneyScaleStep;

        while (((unit + 1) < Units.Length) && (magnitude >= (divisor * PresenceConstants.MoneyScaleStep)))
        {
            divisor *= PresenceConstants.MoneyScaleStep;
            unit++;
        }

        decimal scaled = Math.Round(magnitude / divisor, 1, MidpointRounding.AwayFromZero);

        if (((unit + 1) < Units.Length) && (scaled >= PresenceConstants.MoneyScaleStep))
        {
            divisor *= PresenceConstants.MoneyScaleStep;
            unit++;
            scaled = Math.Round(magnitude / divisor, 1, MidpointRounding.AwayFromZero);
        }

        return sign + PresenceConstants.CurrencySymbol + scaled.ToString(PresenceConstants.MoneyDecimalFormat, culture) + Units[unit];
    }
}
