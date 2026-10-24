using System.Globalization;

using Tf3Drpc.Core.Utilities;

using Xunit;

namespace Tf3Drpc.Core.Tests.Utilities;

public sealed class MoneyUtilsTests
{
    private static readonly CultureInfo English = CultureInfo.GetCultureInfo("en-US");
    private static readonly CultureInfo German = CultureInfo.GetCultureInfo("de-DE");

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
    public void Positive_UsesTheUnitAfterRounding(double amount, string expected)
        => Assert.Equal(expected, MoneyUtils.FormatCompact(amount, English));

    [Theory]
    [InlineData(-1_200_000, "-$1.2M")]
    [InlineData(-999, "-$999")]
    [InlineData(-999_950, "-$1M")]
    [InlineData(-1500, "-$1.5K")]
    [InlineData(-0.2, "$0")]
    public void Negative_PutsTheSignBeforeTheCurrency(double amount, string expected)
        => Assert.Equal(expected, MoneyUtils.FormatCompact(amount, English));

    [Theory]
    [InlineData(15_600_000, "$15,6M")]
    [InlineData(2_000_000, "$2M")]
    [InlineData(1500, "$1,5K")]
    [InlineData(-1_200_000, "-$1,2M")]
    public void German_UsesTheDecimalComma(double amount, string expected)
        => Assert.Equal(expected, MoneyUtils.FormatCompact(amount, German));

    [Fact]
    public void BelowOneThousand_UsesTheGroupFormat()
        => Assert.Equal("$999", MoneyUtils.FormatCompact(999, German));

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void NotFinite_GivesNull(double amount)
        => Assert.Null(MoneyUtils.FormatCompact(amount, English));

    [Fact]
    public void HugeAmounts_DoNotThrow()
        => Assert.NotNull(MoneyUtils.FormatCompact(double.MaxValue, English));
}
