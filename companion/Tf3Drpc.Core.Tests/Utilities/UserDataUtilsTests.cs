using System;

using Tf3Drpc.Core.Structs;
using Tf3Drpc.Core.Utilities;

using Xunit;

namespace Tf3Drpc.Core.Tests.Utilities;

/// <summary>
/// Tests of <see cref="UserDataUtils"/>, which chooses the Steam account folder whose game log was written last.
/// </summary>
public sealed class UserDataUtilsTests
{
    private static readonly DateTime Noon = new(2026, 10, 13, 12, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime Now = Noon.AddDays(1);

    private static UserDataCandidateStruct Candidate(string folder, DateTime? written)
        => new() { Folder = folder, LogWrittenUtc = written };

    /// <summary>
    /// With no accounts there is no folder to choose, so the result is <see langword="null"/>.
    /// </summary>
    [Fact]
    public void NoAccountsGiveNull()
        => Assert.Null(UserDataUtils.ChooseFolder([], Now));

    /// <summary>
    /// A single account is chosen even when it has no log.
    /// </summary>
    [Fact]
    public void SingleAccountWithoutLogIsChosen()
        => Assert.Equal("a", UserDataUtils.ChooseFolder([Candidate("a", written: null)], Now));

    /// <summary>
    /// With several accounts the one with the newest log is chosen.
    /// </summary>
    [Fact]
    public void SeveralAccountsNewestLogWins()
    {
        string? chosen = UserDataUtils.ChooseFolder([Candidate("a", Noon), Candidate("b", Noon.AddHours(2)), Candidate("c", Noon.AddHours(1))], Now);

        Assert.Equal("b", chosen);
    }

    /// <summary>
    /// An account that has a log is chosen over accounts that have none.
    /// </summary>
    [Fact]
    public void AccountWithLogBeatsAccountsWithout()
    {
        string? chosen = UserDataUtils.ChooseFolder([Candidate("a", written: null), Candidate("b", Noon), Candidate("c", written: null)], Now);

        Assert.Equal("b", chosen);
    }

    /// <summary>
    /// When no account has a log, the first account is chosen.
    /// </summary>
    [Fact]
    public void NoAccountHasALogFirstAccountIsChosen()
    {
        string? chosen = UserDataUtils.ChooseFolder([Candidate("a", written: null), Candidate("b", written: null)], Now);

        Assert.Equal("a", chosen);
    }

    /// <summary>
    /// When two accounts have equal log times, the first of them is chosen.
    /// </summary>
    [Fact]
    public void EqualLogTimesFirstAccountIsChosen()
    {
        string? chosen = UserDataUtils.ChooseFolder([Candidate("a", Noon), Candidate("b", Noon)], Now);

        Assert.Equal("a", chosen);
    }

    /// <summary>
    /// A log dated an hour in the future, which suggests a clock that was set back, loses against a log written five minutes ago.
    /// </summary>
    [Fact]
    public void FutureDatedLogLosesAgainstACurrentOne()
    {
        string? chosen = UserDataUtils.ChooseFolder([Candidate("old", Now.AddHours(1)), Candidate("active", Now.AddMinutes(-5))], Now);

        Assert.Equal("active", chosen);
    }

    /// <summary>
    /// A log dated an hour in the future also loses against an account that has no log.
    /// </summary>
    [Fact]
    public void FutureDatedLogLosesAgainstAnAccountWithoutLog()
    {
        string? chosen = UserDataUtils.ChooseFolder([Candidate("old", Now.AddHours(1)), Candidate("empty", written: null)], Now);

        Assert.Equal("empty", chosen);
    }

    /// <summary>
    /// A log dated 30 seconds ahead of the clock is within the tolerance, so it still counts as current and beats an older log.
    /// </summary>
    [Fact]
    public void LogJustAheadOfTheClockIsStillCurrent()
    {
        string? chosen = UserDataUtils.ChooseFolder([Candidate("a", Now.AddMinutes(-5)), Candidate("b", Now.AddSeconds(30))], Now);

        Assert.Equal("b", chosen);
    }

    /// <summary>
    /// When every log is dated in the future, the newest of them is chosen.
    /// </summary>
    [Fact]
    public void AllLogsInTheFutureNewestWins()
    {
        string? chosen = UserDataUtils.ChooseFolder([Candidate("a", Now.AddHours(1)), Candidate("b", Now.AddHours(3)), Candidate("c", Now.AddHours(2))], Now);

        Assert.Equal("b", chosen);
    }
}
