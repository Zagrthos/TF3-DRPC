using System;

using Microsoft.Maui;
using Microsoft.Maui.Handlers;

using UIKit;

namespace Tf3Drpc.App.Platforms.MacCatalyst;

/// <summary>
/// Shows a check box as the native macOS check box instead of MAUI's drawn iPad circle.
/// </summary>
#pragma warning disable CA1812 // Avoid uninstantiated internal classes
internal sealed class MacCheckBoxHandler : ViewHandler<ICheckBox, UISwitch>
#pragma warning restore CA1812
{
    private static readonly PropertyMapper<ICheckBox, MacCheckBoxHandler> Mapper = new(ViewMapper)
    {
        [nameof(ICheckBox.IsChecked)] = MapIsChecked
    };

    public MacCheckBoxHandler()
        : base(Mapper)
    { }

    // The check box style needs the Mac idiom (UIDeviceFamily 6 in Info.plist).
    protected override UISwitch CreatePlatformView()
        => new() { PreferredStyle = UISwitchStyle.Checkbox };

    protected override void ConnectHandler(UISwitch platformView)
    {
        base.ConnectHandler(platformView);
        platformView.ValueChanged += OnValueChanged;
    }

    protected override void DisconnectHandler(UISwitch platformView)
    {
        platformView.ValueChanged -= OnValueChanged;
        base.DisconnectHandler(platformView);
    }

    private static void MapIsChecked(MacCheckBoxHandler handler, ICheckBox checkBox)
        => handler.PlatformView.SetState(checkBox.IsChecked, false);

    private void OnValueChanged(object? sender, EventArgs e)
    {
        if (VirtualView is not null && VirtualView.IsChecked != PlatformView.On)
            VirtualView.IsChecked = PlatformView.On;
    }
}
