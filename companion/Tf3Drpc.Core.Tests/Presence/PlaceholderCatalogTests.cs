using System.Collections.Generic;

using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Extensions;
using Tf3Drpc.Core.Presence;
using Tf3Drpc.Core.Structs;

using Xunit;

namespace Tf3Drpc.Core.Tests.Presence;

public sealed class PlaceholderCatalogTests
{
    [Fact]
    public void All_HasFifteenEntriesWithBraces()
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
