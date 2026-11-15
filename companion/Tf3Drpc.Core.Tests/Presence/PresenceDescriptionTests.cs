using System.Globalization;

using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Presence;
using Tf3Drpc.Core.Settings;
using Tf3Drpc.Core.Structs;
using Tf3Drpc.Core.Translation;

using Xunit;

namespace Tf3Drpc.Core.Tests.Presence;

/// <summary>
/// Tests of <see cref="PresenceDescription"/>, which describes a preset as plain text made of the details and state lines it would show.
/// </summary>
public sealed class PresenceDescriptionTests
{
    private static PresenceContentStruct Render(PresencePreset preset)
        => preset switch
        {
            PresencePreset.Career => new PresenceContentStruct() { Details = "Career details", State = "Career state" },
            PresencePreset.Network => new PresenceContentStruct() { Details = "Network details", State = "Network state" },
            PresencePreset.Era => new PresenceContentStruct() { Details = "Era details", State = "Era state" },
            PresencePreset.Minimal => new PresenceContentStruct() { Details = "Minimal details", State = null },
            PresencePreset.Alternating or PresencePreset.Custom => new PresenceContentStruct() { Details = "Custom details", State = string.Empty },
            _ => new PresenceContentStruct() { Details = "Custom details", State = string.Empty }
        };

    /// <summary>
    /// A preset whose content has a state is described as two lines, details then state, separated by a newline.
    /// </summary>
    [Fact]
    public void APresetWithAStateHasTwoLines()
        => Assert.Equal("Career details\nCareer state", PresenceDescription.Describe(PresencePreset.Career, Render));

    /// <summary>
    /// A preset whose state is null (minimal) or empty (custom) is described by its details line alone.
    /// </summary>
    [Theory]
    [InlineData(PresencePreset.Minimal, "Minimal details")]
    [InlineData(PresencePreset.Custom, "Custom details")]
    internal void APresetWithoutAStateHasOneLine(PresencePreset preset, string expected)
        => Assert.Equal(expected, PresenceDescription.Describe(preset, Render));

    /// <summary>
    /// The alternating preset is described as the career, network and era layouts in rotation order, each as two lines and separated by a blank line.
    /// </summary>
    [Fact]
    public void AlternatingListsTheThreeLayoutsInRotationOrder()
        => Assert.Equal(
            "Career details\nCareer state\n\nNetwork details\nNetwork state\n\nEra details\nEra state",
            PresenceDescription.Describe(PresencePreset.Alternating, Render));

    /// <summary>
    /// A custom preset is described by rendering the given templates through the preview, giving "1905" over "Mechanic".
    /// </summary>
    [Fact]
    public void ACustomPresetUsesTheGivenTemplates()
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
