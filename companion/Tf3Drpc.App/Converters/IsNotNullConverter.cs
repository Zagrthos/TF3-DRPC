using System;
using System.Globalization;

using Microsoft.Maui.Controls;

namespace Tf3Drpc.App.Converters;

/// <summary>
/// Converts a value to whether it is set, for <c language="csharp">IsVisible</c> bindings.
/// </summary>
/// <remarks>
/// A <c language="xaml">DataTrigger</c> with <c language="xaml">Value="{x:Null}"</c> can't do this: it only runs when the bound value changes away from its own default, which is null too.
/// </remarks>
#pragma warning disable CA1812 // Avoid uninstantiated internal classes
internal sealed class IsNotNullConverter : IValueConverter
#pragma warning restore CA1812
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is not null;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
