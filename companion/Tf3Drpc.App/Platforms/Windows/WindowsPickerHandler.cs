using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

using Tf3Drpc.App.ViewModels;

using MauiHandlers = Microsoft.Maui.Handlers;

namespace Tf3Drpc.App.Platforms.Windows;

/// <summary>
/// Creates pickers whose closed drop-down ignores the mouse wheel, so the wheel scrolls the page instead of changing the setting, and whose entries show a tooltip when their item describes itself.
/// </summary>
#pragma warning disable CA1812 // Avoid uninstantiated internal classes
internal sealed partial class WindowsPickerHandler : MauiHandlers.PickerHandler
#pragma warning restore CA1812
{
    protected override ComboBox CreatePlatformView()
        => new WheelPassingComboBox();

    protected override void ConnectHandler(ComboBox platformView)
    {
        base.ConnectHandler(platformView);

        if (platformView is WheelPassingComboBox comboBox)
            comboBox.Picker = VirtualView as Microsoft.Maui.Controls.Picker;
    }

    protected override void DisconnectHandler(ComboBox platformView)
    {
        if (platformView is WheelPassingComboBox comboBox)
            comboBox.Picker = null;

        base.DisconnectHandler(platformView);
    }

    /// <summary>
    /// A combo box that changes its selection with the wheel only while it is open, and gives its entries tooltips.
    /// </summary>
    /// <remarks>
    /// A focused WinUI combo box steps through its items on the wheel and marks the event handled, so the page around it stops scrolling.
    /// WinUI creates the entries lazily, so the tooltips are attached when an entry is prepared. MAUI fills the combo box with the display texts, so the described item comes from the picker's item source.
    /// </remarks>
    private sealed partial class WheelPassingComboBox : ComboBox
    {
        public Microsoft.Maui.Controls.Picker? Picker { get; set; }

        protected override void OnPointerWheelChanged(PointerRoutedEventArgs e)
        {
            if (IsDropDownOpen)
                base.OnPointerWheelChanged(e);
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            if (GetDescribedOption(element) is null)
            {
                ToolTipService.SetToolTip(element, null);

                return;
            }

            // WinUI reuses entries, so the tooltip and its handlers are created once per entry.
            if (ToolTipService.GetToolTip(element) is ToolTip)
                return;

            ToolTip toolTip = new();

            // The pointer enters before the tooltip's delay ends, and Opened covers a tooltip that opens first; the entry may show another item by then.
            toolTip.Opened += (_, _) => UpdateToolTip(element, toolTip);

            if (element is UIElement entry)
                entry.PointerEntered += (_, _) => UpdateToolTip(element, toolTip);

            ToolTipService.SetToolTip(element, toolTip);
        }

        private void UpdateToolTip(DependencyObject element, ToolTip toolTip)
        {
            string? description = GetDescribedOption(element)?.Description;

            if (string.IsNullOrEmpty(description))
            {
                toolTip.IsOpen = false;
                toolTip.Content = null;

                return;
            }

            toolTip.Content = description;
        }

        private IDescribedOption? GetDescribedOption(DependencyObject element)
        {
            int index = IndexFromContainer(element);

            if (Picker?.ItemsSource is not { } items || index < 0 || index >= items.Count)
                return null;

            return items[index] as IDescribedOption;
        }
    }
}
