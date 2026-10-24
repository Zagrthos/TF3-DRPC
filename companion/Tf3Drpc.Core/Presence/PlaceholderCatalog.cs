using System;
using System.Collections.Generic;

using Tf3Drpc.Core.Constants;
using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Extensions;
using Tf3Drpc.Core.Structs;

namespace Tf3Drpc.Core.Presence;

/// <summary>
/// The placeholders of the templates in display order.
/// </summary>
public static class PlaceholderCatalog
{
    private static readonly PlaceholderInfoStruct[] Placeholders = Build();

    /// <summary>
    /// All placeholders with their braces, whether they are private and their description.
    /// </summary>
    public static IReadOnlyList<PlaceholderInfoStruct> All
        => Placeholders;

    private static PlaceholderInfoStruct[] Build()
    {
        PresencePlaceholder[] values = Enum.GetValues<PresencePlaceholder>();
        PlaceholderInfoStruct[] result = new PlaceholderInfoStruct[values.Length];

        for (int i = 0; i < values.Length; i++)
        {
            result[i] = new PlaceholderInfoStruct()
            {
                Name = PresenceConstants.PlaceholderStart + values[i].Name + PresenceConstants.PlaceholderEnd,
                IsPrivate = values[i].IsPrivate,
                Description = values[i].Description
            };
        }

        return result;
    }
}
