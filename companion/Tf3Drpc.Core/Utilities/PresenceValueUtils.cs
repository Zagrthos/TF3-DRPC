using System;
using System.Globalization;

using Tf3Drpc.Core.Constants;
using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Status;
using Tf3Drpc.Core.Translation;

namespace Tf3Drpc.Core.Utilities;

/// <summary>
/// Turns the mod's status into the texts of the template placeholders.
/// </summary>
internal static class PresenceValueUtils
{
    /// <summary>
    /// Returns the text of a placeholder, or <see langword="null"/> when the status has no value for it.
    /// </summary>
    public static string? GetValue(PresencePlaceholder placeholder, ModStatus status, CultureInfo culture, GameTexts gameTexts)
        => placeholder switch
        {
            PresencePlaceholder.Rank => GetRank(status, gameTexts),
            PresencePlaceholder.Mode => (GetMode(status) is GameMode mode) ? gameTexts.GetModeName(mode) : null,
            PresencePlaceholder.Date => FormatDate(status, culture),
            PresencePlaceholder.Month => GetMonth(status, culture),
            PresencePlaceholder.Year => GetYear(status),
            PresencePlaceholder.Climate => gameTexts.GetClimateName(status.Climate),
            PresencePlaceholder.Lines => FormatCount(status.CompanyNumberOfLines, culture),
            PresencePlaceholder.Stations => FormatCount(status.CompanyTotalStations, culture),
            PresencePlaceholder.Trains => FormatCount(status.CompanyRailVehicles, culture),
            PresencePlaceholder.Vehicles => FormatCount(SumVehicles(status), culture),
            PresencePlaceholder.Passengers => FormatCount(status.PassengersTransported, culture),
            PresencePlaceholder.Cargo => FormatCount(status.CargoTransported, culture),
            PresencePlaceholder.CompanyValue => GetCompanyValue(status, culture),
            PresencePlaceholder.Balance => GetBalance(status, culture),
            PresencePlaceholder.SaveName => status.SaveName,
            _ => null
        };

    /// <summary>
    /// Whether the mode only repeats the rank: the Tycoon mod fixes the rank at Tycoon, which some languages word differently from the mode.
    /// </summary>
    public static bool ModeRepeatsRank(ModStatus status)
        => (GetMode(status) is GameMode.Tycoon) && (status.Rank is TranslationConstants.RankCount);

    private static string? GetRank(ModStatus status, GameTexts gameTexts)
        => (status.Rank is long rank and >= 1 and <= TranslationConstants.RankCount) ? gameTexts.GetRankName((int)rank) : null;

    private static GameMode? GetMode(ModStatus status)
    {
        if (status.IsMapEditor is true)
            return GameMode.MapEditor;

        if (status.IsSandbox is true)
            return GameMode.Sandbox;

        return (status.IsTycoon is true) ? GameMode.Tycoon : null;
    }

    private static string? FormatDate(ModStatus status, CultureInfo culture)
    {
        if ((status.Year is not (>= PresenceConstants.MinYear and <= PresenceConstants.MaxYear)) || (status.Month is not (>= 1 and <= 12)))
            return null;

        DateTime date = new((int)status.Year.Value, (int)status.Month.Value, 1, 0, 0, 0, DateTimeKind.Unspecified);

        return date.ToString(culture.DateTimeFormat.YearMonthPattern, culture);
    }

    private static string? GetMonth(ModStatus status, CultureInfo culture)
        => (status.Month is long month and >= 1 and <= 12) ? culture.DateTimeFormat.GetMonthName((int)month) : null;

    private static string? GetYear(ModStatus status)
        => (status.Year is long year and >= PresenceConstants.MinYear and <= PresenceConstants.MaxYear) ? year.ToString(CultureInfo.InvariantCulture) : null;

    private static string? FormatCount(long? count, CultureInfo culture)
        => count?.ToString(PresenceConstants.NumberFormat, culture);

    private static long? SumVehicles(ModStatus status)
    {
        long? road = status.CompanyRoadVehicles;
        long? rail = status.CompanyRailVehicles;
        long? aircraft = status.CompanyAircrafts;
        long? ships = status.CompanyShips;
        long? trams = status.CompanyTrams;

        if ((road is null) && (rail is null) && (aircraft is null) && (ships is null) && (trams is null))
            return null;

        // Five counts can exceed the range of a long; a total that does not fit is no value, not an exception.
        Int128 total = (Int128)(road ?? 0) + (rail ?? 0) + (aircraft ?? 0) + (ships ?? 0) + (trams ?? 0);
        Int128 min = long.MinValue;
        Int128 max = long.MaxValue;

        return ((total < min) || (total > max)) ? null : (long)total;
    }

    private static string? GetCompanyValue(ModStatus status, CultureInfo culture)
    {
        if (status.CompanyTotalAssets is not double assets)
            return null;

        double value = assets + (status.Balance ?? 0) - (status.CompanyDebt ?? 0);

        return MoneyUtils.FormatCompact(value, culture);
    }

    private static string? GetBalance(ModStatus status, CultureInfo culture)
        => ((status.InfiniteMoney is true) || (status.Balance is not long balance)) ? null : MoneyUtils.FormatCompact(balance, culture);
}
