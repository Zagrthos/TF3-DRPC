using System;
using System.Collections.Generic;

using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Settings;

using Xunit;

namespace Tf3Drpc.Core.Tests.Settings;

/// <summary>
/// Tests of <see cref="ThemeCatalog"/>, covering the theme list, the setting strings and the display texts.
/// </summary>
public sealed class ThemeCatalogTests
{
    /// <summary>
    /// The catalogue lists the themes in the order system, light, dark.
    /// </summary>
    [Fact]
    public void AllListsSystemLightDarkInOrder()
        => Assert.Equal([ThemeMode.System, ThemeMode.Light, ThemeMode.Dark], ThemeCatalog.All);

    /// <summary>
    /// The catalogue contains exactly one entry for every value of <see cref="ThemeMode"/>.
    /// </summary>
    [Fact]
    public void AllCoversEveryThemeMode()
        => Assert.Equal(Enum.GetValues<ThemeMode>().Length, ThemeCatalog.All.Count);

    /// <summary>
    /// Each theme maps to its lower-case setting string, namely "system", "light" and "dark".
    /// </summary>
    [Theory]
    [InlineData(ThemeMode.System, "system")]
    [InlineData(ThemeMode.Light, "light")]
    [InlineData(ThemeMode.Dark, "dark")]
    internal void ToSettingGivesTheSettingsValue(ThemeMode theme, string expected)
        => Assert.Equal(expected, ThemeCatalog.ToSetting(theme));

    /// <summary>
    /// Converting every listed theme to its setting string and parsing it back returns the same theme.
    /// </summary>
    [Fact]
    public void EveryThemeRoundTripsThroughItsSetting()
    {
        foreach (ThemeMode theme in ThemeCatalog.All)
        {
            Assert.True(ThemeCatalog.TryParse(ThemeCatalog.ToSetting(theme), out ThemeMode parsed));
            Assert.Equal(theme, parsed);
        }
    }

    /// <summary>
    /// Parsing rejects null, an empty string, wrong-case values such as "Dark" and "LIGHT" and the unknown value "auto", and yields the system theme as the output.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Dark")]
    [InlineData("LIGHT")]
    [InlineData("auto")]
    public void TryParseRejectsUnknownAndWrongCaseValues(string? setting)
    {
        Assert.False(ThemeCatalog.TryParse(setting, out ThemeMode parsed));
        Assert.Equal(ThemeMode.System, parsed);
    }

    /// <summary>
    /// The display text of every listed theme is different from that of every other theme.
    /// </summary>
    [Fact]
    public void GetDisplayTextIsDistinctForEveryTheme()
    {
        HashSet<OwnText> texts = [];

        foreach (ThemeMode theme in ThemeCatalog.All)
            Assert.True(texts.Add(ThemeCatalog.GetDisplayText(theme)));

        Assert.Equal(ThemeCatalog.All.Count, texts.Count);
    }
}
