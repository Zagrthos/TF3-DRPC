using System.Globalization;

using Tf3Drpc.Core.Utilities;

using Xunit;

namespace Tf3Drpc.Core.Tests.Utilities;

/// <summary>
/// Tests of <see cref="MoneyUtils"/>, which formats amounts of money compactly with a currency symbol and a K, M, B or T unit.
/// </summary>
public sealed class MoneyUtilsTests
{
    private static readonly CultureInfo English = CultureInfo.GetCultureInfo("en-US");
    private static readonly CultureInfo German = CultureInfo.GetCultureInfo("de-DE");

    /// <summary>
    /// A positive amount, from $0 to beyond $1000T, gets its unit after rounding, so 999,949 is $999.9K but 999,950 is $1M, not $1000K.
    /// </summary>
    [Theory]
    [InlineData(0, "$0")]
    [InlineData(1, "$1")]
    [InlineData(999, "$999")]
    [InlineData(999.4, "$999")]
    [InlineData(1000, "$1K")]
    [InlineData(1500, "$1.5K")]
    [InlineData(1549, "$1.5K")]
    [InlineData(15_600_000, "$15.6M")]
    [InlineData(2_000_000, "$2M")]
    [InlineData(2_040_000, "$2M")]
    [InlineData(3_400_000_000, "$3.4B")]
    [InlineData(5_000_000_000_000, "$5T")]
    [InlineData(1_200_000_000_000_000, "$1200T")]
    [InlineData(999_949, "$999.9K")]
    [InlineData(999_950, "$1M")]
    [InlineData(999_999, "$1M")]
    [InlineData(999_950_000, "$1B")]
    [InlineData(999_950_000_000, "$1T")]
    public void PositiveUsesTheUnitAfterRounding(double amount, string expected)
        => Assert.Equal(expected, MoneyUtils.FormatCompact(amount, English));

    /// <summary>
    /// A negative amount puts the minus sign before the currency symbol, so -1,200,000 is -$1.2M, and -0.2 rounds to $0 without a sign.
    /// </summary>
    [Theory]
    [InlineData(-1_200_000, "-$1.2M")]
    [InlineData(-999, "-$999")]
    [InlineData(-999_950, "-$1M")]
    [InlineData(-1500, "-$1.5K")]
    [InlineData(-0.2, "$0")]
    public void NegativePutsTheSignBeforeTheCurrency(double amount, string expected)
        => Assert.Equal(expected, MoneyUtils.FormatCompact(amount, English));

    /// <summary>
    /// German formatting uses the decimal comma, so 15,600,000 is $15,6M and -1,200,000 is -$1,2M, while whole values such as $2M are unchanged.
    /// </summary>
    [Theory]
    [InlineData(15_600_000, "$15,6M")]
    [InlineData(2_000_000, "$2M")]
    [InlineData(1500, "$1,5K")]
    [InlineData(-1_200_000, "-$1,2M")]
    public void GermanUsesTheDecimalComma(double amount, string expected)
        => Assert.Equal(expected, MoneyUtils.FormatCompact(amount, German));

    /// <summary>
    /// An amount below one thousand is shown as a whole number with no unit and no group separator, in English and in German, with 999.4 rounding to 999.
    /// </summary>
    [Theory]
    [InlineData("en-US", 0, "$0")]
    [InlineData("en-US", 1, "$1")]
    [InlineData("en-US", 999, "$999")]
    [InlineData("en-US", 999.4, "$999")]
    [InlineData("de-DE", 0, "$0")]
    [InlineData("de-DE", 1, "$1")]
    [InlineData("de-DE", 999, "$999")]
    [InlineData("de-DE", 999.4, "$999")]
    public void BelowOneThousandIsAWholeNumberWithoutAUnit(string culture, double amount, string expected)
        => Assert.Equal(expected, MoneyUtils.FormatCompact(amount, (culture is "de-DE") ? German : English));

    /// <summary>
    /// NaN and both infinities have no representation, so the result is <see langword="null"/>.
    /// </summary>
    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void NotFiniteGivesNull(double amount)
        => Assert.Null(MoneyUtils.FormatCompact(amount, English));

    /// <summary>
    /// The largest double is formatted instead of throwing, giving a non-null result.
    /// </summary>
    [Fact]
    public void HugeAmountsDoNotThrow()
        => Assert.NotNull(MoneyUtils.FormatCompact(double.MaxValue, English));
}
