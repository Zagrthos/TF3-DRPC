# Gives a TF3-DRPC button (by UIA name) keyboard focus and presses Space with SendInput, like a keyboard user

param([string]$Name)
Add-Type -AssemblyName UIAutomationClient, UIAutomationTypes, System.Windows.Forms
if (-not ('KeyPress' -as [type])) { Add-Type -TypeDefinition @'
using System; using System.Runtime.InteropServices;
public static class KeyPress {
  [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr h);
  [DllImport("user32.dll")] public static extern IntPtr GetForegroundWindow();
  [DllImport("user32.dll")] public static extern void keybd_event(byte vk, byte scan, uint flags, IntPtr extra);
}
'@ }
$proc = Get-Process TF3-DRPC | Where-Object { $_.MainWindowHandle -ne 0 } | Select-Object -First 1
if (-not $proc) { throw 'TF3-DRPC window not found; nothing was sent' }
[KeyPress]::SetForegroundWindow($proc.MainWindowHandle) | Out-Null
Start-Sleep -Milliseconds 400
"foreground is app window: " + ([KeyPress]::GetForegroundWindow() -eq $proc.MainWindowHandle)
$root = [System.Windows.Automation.AutomationElement]::FromHandle($proc.MainWindowHandle)
$cond = New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::NameProperty, $Name)
$el = $root.FindFirst([System.Windows.Automation.TreeScope]::Descendants, $cond)
$el.SetFocus()
Start-Sleep -Milliseconds 300
"focused element: " + [System.Windows.Automation.AutomationElement]::FocusedElement.Current.Name
$focused = [System.Windows.Automation.AutomationElement]::FocusedElement.Current
if (($focused.ProcessId -ne $proc.Id) -or ($focused.Name -ne $Name)) { throw "the button has no keyboard focus; no key sent" }
[KeyPress]::keybd_event(0x20, 0, 0, [IntPtr]::Zero); Start-Sleep -Milliseconds 50; [KeyPress]::keybd_event(0x20, 0, 2, [IntPtr]::Zero)
"pressed Space at $(Get-Date -Format HH:mm:ss) in process $($proc.Id)"
