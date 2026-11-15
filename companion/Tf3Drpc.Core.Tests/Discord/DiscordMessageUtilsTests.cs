using System;
using System.Text;
using System.Text.Json;

using Tf3Drpc.Core.Structs;
using Tf3Drpc.Core.Utilities;

using Xunit;

namespace Tf3Drpc.Core.Tests.Discord;

/// <summary>
/// Tests of the SET_ACTIVITY frame built by <see cref="Tf3Drpc.Core.Utilities.DiscordMessageUtils"/>, covering the store button, the hover text and the clear payload.
/// </summary>
public sealed class DiscordMessageUtilsTests
{
    private static JsonElement Activity(in PresenceContentStruct content, out string json)
    {
        json = Encoding.UTF8.GetString(DiscordMessageUtils.BuildSetActivity(content, "nonce"));
        using JsonDocument document = JsonDocument.Parse(json);

        return document.RootElement.GetProperty("args").GetProperty("activity").Clone();
    }

    /// <summary>
    /// A presence with a button label produces a frame whose single button carries that label and the Transport Fever 3 Steam store URL.
    /// </summary>
    [Fact]
    public void WithALabelTheFrameCarriesTheStoreButton()
    {
        JsonElement activity = Activity(new PresenceContentStruct() { Details = "Test", ButtonLabel = "Hol dir Transport Fever 3" }, out string json);

        Assert.Contains("\"buttons\":[{\"label\":\"Hol dir Transport Fever 3\",\"url\":\"https://store.steampowered.com/app/3493540\"}]", json, StringComparison.Ordinal);
        Assert.Equal(1, activity.GetProperty("buttons").GetArrayLength());
    }

    /// <summary>
    /// A presence without a button label produces a frame that contains no buttons key at all.
    /// </summary>
    [Fact]
    public void WithoutALabelTheFrameHasNoButtonsKey()
    {
        JsonElement activity = Activity(new PresenceContentStruct() { Details = "Test" }, out string json);

        Assert.DoesNotContain("buttons", json, StringComparison.Ordinal);
        Assert.False(activity.TryGetProperty("buttons", out _));
    }

    /// <summary>
    /// A button label with non-ASCII characters (Japanese text) is written to the frame and read back unchanged.
    /// </summary>
    [Fact]
    public void TheLabelIsWrittenAsUtf8()
    {
        JsonElement activity = Activity(new PresenceContentStruct() { Details = "Test", ButtonLabel = "Transport Fever 3を入手" }, out _);

        Assert.Equal("Transport Fever 3を入手", activity.GetProperty("buttons")[0].GetProperty("label").GetString());
    }

    /// <summary>
    /// The hover text becomes the large image text of the activity, and without a hover text the text defaults to "Transport Fever 3"; the image is always "cover".
    /// </summary>
    [Theory]
    [InlineData("Archipelago2 in 1905", "Archipelago2 in 1905")]
    [InlineData(null, "Transport Fever 3")]
    public void SetActivityPutsTheHoverTextIntoLargeText(string? hover, string expected)
    {
        PresenceContentStruct content = new() { Details = "Mechanic", LargeImageText = hover };

        byte[] payload = DiscordMessageUtils.BuildSetActivity(content, "nonce");
        using JsonDocument document = JsonDocument.Parse(payload);
        JsonElement assets = document.RootElement.GetProperty("args").GetProperty("activity").GetProperty("assets");

        Assert.Equal(expected, assets.GetProperty("large_text").GetString());
        Assert.Equal("cover", assets.GetProperty("large_image").GetString());
    }

    /// <summary>
    /// Building a clear payload produces a SET_ACTIVITY command with the process id but without an activity.
    /// </summary>
    [Fact]
    public void ClearPayloadOmitsTheActivity()
    {
        byte[] payload = DiscordMessageUtils.BuildSetActivity(content: null, "nonce");
        using JsonDocument document = JsonDocument.Parse(payload);
        JsonElement arguments = document.RootElement.GetProperty("args");

        Assert.Equal("SET_ACTIVITY", document.RootElement.GetProperty("cmd").GetString());
        Assert.Equal(Environment.ProcessId, arguments.GetProperty("pid").GetInt32());
        Assert.False(arguments.TryGetProperty("activity", out JsonElement _));
    }
}
