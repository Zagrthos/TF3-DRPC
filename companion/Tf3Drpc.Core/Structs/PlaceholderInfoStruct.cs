using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using Tf3Drpc.Core.Enums;

namespace Tf3Drpc.Core.Structs;

/// <summary>
/// A placeholder a template can use.
/// </summary>
[StructLayout(LayoutKind.Auto)]
public readonly struct PlaceholderInfoStruct : IEquatable<PlaceholderInfoStruct>
{
    /// <summary>
    /// The placeholder as written in a template, with its braces.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Tells whether the value is personal, such as money or the name of the save game.
    /// </summary>
    public bool IsPrivate { get; init; }

    /// <summary>
    /// The own text that explains the placeholder.
    /// </summary>
    public OwnText Description { get; init; }

    public bool Equals(PlaceholderInfoStruct other)
        => string.Equals(Name, other.Name, StringComparison.Ordinal) &&
            (IsPrivate == other.IsPrivate) &&
            (Description == other.Description);

    public override bool Equals([NotNullWhen(true)] object? obj)
        => (obj is PlaceholderInfoStruct other) && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(Name, IsPrivate, Description);

    public static bool operator ==(in PlaceholderInfoStruct left, in PlaceholderInfoStruct right)
        => left.Equals(right);

    public static bool operator !=(in PlaceholderInfoStruct left, in PlaceholderInfoStruct right)
        => !left.Equals(right);
}
