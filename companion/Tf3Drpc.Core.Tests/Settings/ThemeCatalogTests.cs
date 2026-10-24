using System;
using System.Collections.Generic;

using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Settings;

using Xunit;

namespace Tf3Drpc.Core.Tests.Settings;

public sealed class ThemeCatalogTests
{
    [Fact]
    public void All_ListsSystemLightDarkInOrder()
        => Assert.Equal([ThemeMode.System, ThemeMode.Light, ThemeMode.Dark], ThemeCatalog.All);

    [Fact]
    public void All_CoversEveryThemeMode()
        => Assert.Equal(Enum.GetValues<ThemeMode>().Length, ThemeCatalog.All.Count);

    [Theory]
    [InlineData(ThemeMode.System, "system")]
    [InlineData(ThemeMode.Light, "light")]
    [InlineData(ThemeMode.Dark, "dark")]
    public void ToSetting_GivesTheSettingsValue(ThemeMode theme, string expected)
        => Assert.Equal(expected, ThemeCatalog.ToSetting(theme));

    [Fact]
    public void EveryTheme_RoundTripsThroughItsSetting()
    {
        foreach (ThemeMode theme in ThemeCatalog.All)
        {
            Assert.True(ThemeCatalog.TryParse(ThemeCatalog.ToSetting(theme), out ThemeMode parsed));
            Assert.Equal(theme, parsed);
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Dark")]
    [InlineData("LIGHT")]
    [InlineData("auto")]
    public void TryParse_RejectsUnknownAndWrongCaseValues(string? setting)
    {
        Assert.False(ThemeCatalog.TryParse(setting, out ThemeMode parsed));
        Assert.Equal(ThemeMode.System, parsed);
    }

    [Fact]
    public void GetDisplayText_IsDistinctForEveryTheme()
    {
        HashSet<OwnText> texts = [];

        foreach (ThemeMode theme in ThemeCatalog.All)
            Assert.True(texts.Add(ThemeCatalog.GetDisplayText(theme)));

        Assert.Equal(ThemeCatalog.All.Count, texts.Count);
    }
}
