using System;

using Tf3Drpc.Core.Structs;
using Tf3Drpc.Core.Utilities;

using Xunit;

namespace Tf3Drpc.Core.Tests.Utilities;

public sealed class UserDataUtilsTests
{
    private static readonly DateTime Noon = new(2026, 10, 13, 12, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime Now = Noon.AddDays(1);

    private static UserDataCandidateStruct Candidate(string folder, DateTime? written)
        => new() { Folder = folder, LogWrittenUtc = written };

    [Fact]
    public void NoAccounts_GiveNull()
        => Assert.Null(UserDataUtils.ChooseFolder([], Now));

    [Fact]
    public void SingleAccountWithoutLog_IsChosen()
        => Assert.Equal("a", UserDataUtils.ChooseFolder([Candidate("a", null)], Now));

    [Fact]
    public void SeveralAccounts_NewestLogWins()
    {
        string? chosen = UserDataUtils.ChooseFolder([Candidate("a", Noon), Candidate("b", Noon.AddHours(2)), Candidate("c", Noon.AddHours(1))], Now);

        Assert.Equal("b", chosen);
    }

    [Fact]
    public void AccountWithLog_BeatsAccountsWithout()
    {
        string? chosen = UserDataUtils.ChooseFolder([Candidate("a", null), Candidate("b", Noon), Candidate("c", null)], Now);

        Assert.Equal("b", chosen);
    }

    [Fact]
    public void NoAccountHasALog_FirstAccountIsChosen()
    {
        string? chosen = UserDataUtils.ChooseFolder([Candidate("a", null), Candidate("b", null)], Now);

        Assert.Equal("a", chosen);
    }

    [Fact]
    public void EqualLogTimes_FirstAccountIsChosen()
    {
        string? chosen = UserDataUtils.ChooseFolder([Candidate("a", Noon), Candidate("b", Noon)], Now);

        Assert.Equal("a", chosen);
    }

    [Fact]
    public void FutureDatedLog_LosesAgainstACurrentOne()
    {
        string? chosen = UserDataUtils.ChooseFolder([Candidate("old", Now.AddHours(1)), Candidate("active", Now.AddMinutes(-5))], Now);

        Assert.Equal("active", chosen);
    }

    [Fact]
    public void FutureDatedLog_LosesAgainstAnAccountWithoutLog()
    {
        string? chosen = UserDataUtils.ChooseFolder([Candidate("old", Now.AddHours(1)), Candidate("empty", null)], Now);

        Assert.Equal("empty", chosen);
    }

    [Fact]
    public void LogJustAheadOfTheClock_IsStillCurrent()
    {
        string? chosen = UserDataUtils.ChooseFolder([Candidate("a", Now.AddMinutes(-5)), Candidate("b", Now.AddSeconds(30))], Now);

        Assert.Equal("b", chosen);
    }

    [Fact]
    public void AllLogsInTheFuture_NewestWins()
    {
        string? chosen = UserDataUtils.ChooseFolder([Candidate("a", Now.AddHours(1)), Candidate("b", Now.AddHours(3)), Candidate("c", Now.AddHours(2))], Now);

        Assert.Equal("b", chosen);
    }
}
