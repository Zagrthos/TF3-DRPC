using System;
using System.Text;

using Tf3Drpc.Core.Constants;
using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Structs;

namespace Tf3Drpc.Core.Presence;

/// <summary>
/// Turns a preset into the text that describes what it would show on Discord.
/// </summary>
public static class PresenceDescription
{
    /// <summary>
    /// Describes a preset: its two lines, or for the alternating preset the lines of every layout it rotates through, separated by a blank line.
    /// </summary>
    /// <param name="preset">The preset to describe.</param>
    /// <param name="render">Renders a preset that is not alternating.</param>
    public static string Describe(PresencePreset preset, Func<PresencePreset, PresenceContentStruct> render)
    {
        ArgumentNullException.ThrowIfNull(render);

        if (preset is not PresencePreset.Alternating)
            return Format(render(preset));

        StringBuilder builder = new();

        for (int i = 0; i < PresenceConstants.AlternatingLayoutCount; i++)
        {
            if (i > 0)
                builder.Append("\n\n");

            builder.Append(Format(render(PresencePresets.Resolve(preset, i))));
        }

        return builder.ToString();
    }

    private static string Format(in PresenceContentStruct content)
        => (string.IsNullOrEmpty(content.State)) ? content.Details : content.Details + "\n" + content.State;
}
