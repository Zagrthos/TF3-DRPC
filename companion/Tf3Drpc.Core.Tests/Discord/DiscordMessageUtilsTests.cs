using System;
using System.Text;
using System.Text.Json;

using Tf3Drpc.Core.Structs;
using Tf3Drpc.Core.Utilities;

using Xunit;

namespace Tf3Drpc.Core.Tests.Discord;

public sealed class DiscordMessageUtilsTests
{
    private static JsonElement Activity(in PresenceContentStruct content, out string json)
    {
        json = Encoding.UTF8.GetString(DiscordMessageUtils.BuildSetActivity(content, "nonce"));
        using JsonDocument document = JsonDocument.Parse(json);

        return document.RootElement.GetProperty("args").GetProperty("activity").Clone();
    }

    [Fact]
    public void WithALabel_TheFrameCarriesTheStoreButton()
    {
        JsonElement activity = Activity(new PresenceContentStruct() { Details = "Test", ButtonLabel = "Hol dir Transport Fever 3" }, out string json);

        Assert.Contains("\"buttons\":[{\"label\":\"Hol dir Transport Fever 3\",\"url\":\"https://store.steampowered.com/app/3493540\"}]", json, StringComparison.Ordinal);
        Assert.Equal(1, activity.GetProperty("buttons").GetArrayLength());
    }

    [Fact]
    public void WithoutALabel_TheFrameHasNoButtonsKey()
    {
        JsonElement activity = Activity(new PresenceContentStruct() { Details = "Test" }, out string json);

        Assert.DoesNotContain("buttons", json, StringComparison.Ordinal);
        Assert.False(activity.TryGetProperty("buttons", out _));
    }

    [Fact]
    public void TheLabelIsWrittenAsUtf8()
    {
        JsonElement activity = Activity(new PresenceContentStruct() { Details = "Test", ButtonLabel = "Transport Fever 3を入手" }, out _);

        Assert.Equal("Transport Fever 3を入手", activity.GetProperty("buttons")[0].GetProperty("label").GetString());
    }
}
