namespace Tf3Drpc.Core.Constants;

/// <summary>
/// Limits of the regular expressions.
/// </summary>
internal static class RegexConstants
{
    // Wall-clock time: 100 ms ran out for a 30-character profile on a busy CI runner. The patterns
    // take about 55 ms for 1 MB, so 1 s still stops only a runaway match.
    public const int TimeoutMilliseconds = 1000;
}
