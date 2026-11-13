using System.Collections.Generic;

using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Extensions;
using Tf3Drpc.Core.Presence;
using Tf3Drpc.Core.Structs;

using Xunit;

namespace Tf3Drpc.Core.Tests.Presence;

/// <summary>
/// Tests of <see cref="PlaceholderCatalog"/>, the list of placeholders that custom presence templates can use.
/// </summary>
public sealed class PlaceholderCatalogTests
{
    /// <summary>
    /// The catalogue has 15 placeholders, from "{rank}" first to "{saveName}" last, each name written in braces.
    /// </summary>
    [Fact]
    public void AllHasFifteenEntriesWithBraces()
    {
        Assert.Equal(15, PlaceholderCatalog.All.Count);
        Assert.Equal("{rank}", PlaceholderCatalog.All[0].Name);
        Assert.Equal("{saveName}", PlaceholderCatalog.All[14].Name);

        for (int i = 0; i < PlaceholderCatalog.All.Count; i++)
        {
            Assert.StartsWith("{", PlaceholderCatalog.All[i].Name, System.StringComparison.Ordinal);
            Assert.EndsWith("}", PlaceholderCatalog.All[i].Name, System.StringComparison.Ordinal);
        }
    }

    /// <summary>
    /// Every placeholder has a non-blank description resource key that no other placeholder shares.
    /// </summary>
    [Fact]
    public void EveryPlaceholderHasItsOwnDescription()
    {
        HashSet<OwnText> descriptions = [];

        for (int i = 0; i < PlaceholderCatalog.All.Count; i++)
        {
            OwnText description = PlaceholderCatalog.All[i].Description;

            Assert.False(string.IsNullOrWhiteSpace(description.ResourceKey));
            Assert.True(descriptions.Add(description), PlaceholderCatalog.All[i].Name);
        }
    }

    /// <summary>
    /// Only the money placeholders "{companyValue}" and "{balance}" and the save name "{saveName}" are marked as private.
    /// </summary>
    [Fact]
    public void OnlyMoneyAndTheSaveNameArePrivate()
    {
        List<string> privateNames = [];

        for (int i = 0; i < PlaceholderCatalog.All.Count; i++)
        {
            PlaceholderInfoStruct placeholder = PlaceholderCatalog.All[i];

            if (placeholder.IsPrivate)
                privateNames.Add(placeholder.Name);
        }

        Assert.Equal(["{companyValue}", "{balance}", "{saveName}"], privateNames);
    }
}
