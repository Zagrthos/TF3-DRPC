using System;

using Tf3Drpc.Core.Enums;

namespace Tf3Drpc.App.ViewModels;

/// <summary>
/// One entry of the preset picker: a preset with its display name and the description for its tooltip.
/// </summary>
/// <param name="Preset">The preset this entry selects.</param>
/// <param name="DisplayName">The text shown in the picker.</param>
/// <param name="Describe">Builds what the preset would show on Discord with the current values.</param>
internal sealed record class PresetOption(PresencePreset Preset, string DisplayName, Func<string?> Describe) : IDescribedOption
{
    public string? Description
        => Describe();
}
