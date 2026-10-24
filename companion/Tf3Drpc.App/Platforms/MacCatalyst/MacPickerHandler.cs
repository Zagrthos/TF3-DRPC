using System;

using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;

using Tf3Drpc.App.ViewModels;

using UIKit;

namespace Tf3Drpc.App.Platforms.MacCatalyst;

/// <summary>
/// Shows a picker as the native macOS pop-up button instead of MAUI's iPad text field with a wheel, with a tooltip on each entry whose item describes itself.
/// </summary>
#pragma warning disable CA1812 // Avoid uninstantiated internal classes
internal sealed class MacPickerHandler : ViewHandler<IPicker, UIButton>
#pragma warning restore CA1812
{
    private const string ReloadCommand = "Reload";

    private static readonly PropertyMapper<IPicker, MacPickerHandler> Mapper = new(ViewMapper)
    {
        [nameof(IPicker.Items)] = MapMenu,
        [nameof(IPicker.SelectedIndex)] = MapMenu,
        [nameof(IPicker.Title)] = MapMenu
    };

    private static readonly CommandMapper<IPicker, MacPickerHandler> CommandMapper = new(ViewCommandMapper)
    {
        [ReloadCommand] = MapReload
    };

    public MacPickerHandler()
        : base(Mapper, CommandMapper)
    { }

    protected override UIButton CreatePlatformView()
    {
        UIButton button = UIButton.FromType(UIButtonType.System);

        // In the Mac idiom a button whose menu changes the selection is AppKit's pop-up button.
        button.ShowsMenuAsPrimaryAction = true;
        button.ChangesSelectionAsPrimaryAction = true;

        return button;
    }

    protected override void ConnectHandler(UIButton platformView)
    {
        base.ConnectHandler(platformView);

        // The descriptions show the current values, so the menu is built again each time it opens, as the Windows tooltips are.
        platformView.AddTarget(OnMenuOpening, UIControlEvent.MenuActionTriggered);
    }

    protected override void DisconnectHandler(UIButton platformView)
    {
        platformView.RemoveTarget(OnMenuOpening, UIControlEvent.MenuActionTriggered);
        base.DisconnectHandler(platformView);
    }

    private static void MapMenu(MacPickerHandler handler, IPicker picker)
        => handler.UpdateMenu();

    private static void MapReload(MacPickerHandler handler, IPicker picker, object? argument)
        => handler.UpdateMenu();

    private void UpdateMenu()
    {
        int count = VirtualView.GetCount();
        UIMenuElement[] actions = new UIMenuElement[count];

        for (int i = 0; i < count; i++)
        {
            int index = i;
            // While the items source is rebuilt, an item's display text can still be null.
            UIAction action = UIAction.Create(VirtualView.GetItem(i) ?? string.Empty, null, null, _ => OnSelected(index));
            action.State = (i == VirtualView.SelectedIndex) ? UIMenuElementState.On : UIMenuElementState.Off;

            // Mac Catalyst shows the discoverability title as the entry's tooltip.
            action.DiscoverabilityTitle = GetDescription(i);
            actions[i] = action;
        }

        UIMenu menu = UIMenu.Create(VirtualView.Title ?? string.Empty, actions);
        bool hasSelection = VirtualView.SelectedIndex >= 0 && VirtualView.SelectedIndex < count;

        // UIKit throws when a selecting pop-up gets a menu without a selected element, which happens while the
        // items source is rebuilt; until there is one, the button only shows the menu.
        if (!hasSelection)
            PlatformView.ChangesSelectionAsPrimaryAction = false;

        PlatformView.Menu = menu;

        if (hasSelection)
            PlatformView.ChangesSelectionAsPrimaryAction = true;
    }

    private string? GetDescription(int index)
    {
        if (VirtualView is not Picker { ItemsSource: { } items } || index >= items.Count)
            return null;

        return (items[index] as IDescribedOption)?.Description;
    }

    private void OnMenuOpening(object? sender, EventArgs e)
        => UpdateMenu();

    private void OnSelected(int index)
    {
        if (VirtualView is not null && VirtualView.SelectedIndex != index)
            VirtualView.SelectedIndex = index;
    }
}
