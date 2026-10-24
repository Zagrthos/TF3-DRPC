using System.Globalization;

using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Presence;
using Tf3Drpc.Core.Settings;
using Tf3Drpc.Core.Structs;
using Tf3Drpc.Core.Translation;

using Xunit;

namespace Tf3Drpc.Core.Tests.Presence;

public sealed class PresenceDescriptionTests
{
    private static PresenceContentStruct Render(PresencePreset preset)
        => preset switch
        {
            PresencePreset.Career => new PresenceContentStruct() { Details = "Career details", State = "Career state" },
            PresencePreset.Network => new PresenceContentStruct() { Details = "Network details", State = "Network state" },
            PresencePreset.Era => new PresenceContentStruct() { Details = "Era details", State = "Era state" },
            PresencePreset.Minimal => new PresenceContentStruct() { Details = "Minimal details", State = null },
            _ => new PresenceContentStruct() { Details = "Custom details", State = string.Empty }
        };

    [Fact]
    public void APresetWithAState_HasTwoLines()
        => Assert.Equal("Career details\nCareer state", PresenceDescription.Describe(PresencePreset.Career, Render));

    [Theory]
    [InlineData(PresencePreset.Minimal, "Minimal details")]
    [InlineData(PresencePreset.Custom, "Custom details")]
    public void APresetWithoutAState_HasOneLine(PresencePreset preset, string expected)
        => Assert.Equal(expected, PresenceDescription.Describe(preset, Render));

    [Fact]
    public void Alternating_ListsTheThreeLayoutsInRotationOrder()
        => Assert.Equal(
            "Career details\nCareer state\n\nNetwork details\nNetwork state\n\nEra details\nEra state",
            PresenceDescription.Describe(PresencePreset.Alternating, Render));

    [Fact]
    public void ACustomPreset_UsesTheGivenTemplates()
    {
        PresenceTemplates templates = new() { Details = "{year}", State = "{rank}", Hover = "x" };
        PresencePreview preview = new();

        string text = PresenceDescription.Describe
        (
            PresencePreset.Custom,
            preset => preview.Build(CultureInfo.GetCultureInfo("en-US"), GameTexts.English, new PresenceOptionsStruct() { Preset = preset, Templates = templates }, out _)
        );

        Assert.Equal("1905\nMechanic", text);
    }
}
