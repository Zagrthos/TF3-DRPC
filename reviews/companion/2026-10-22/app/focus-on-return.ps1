# Brings the TF3-DRPC window back to the front (as Alt+Tab would: an Alt press lets SetForegroundWindow through) and reports which control has the keyboard focus
Add-Type -AssemblyName UIAutomationClient, UIAutomationTypes
$ErrorActionPreference = 'Stop'
if (-not ('Fg' -as [type])) { Add-Type -TypeDefinition @'
using System; using System.Runtime.InteropServices;
public static class Fg {
  [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr h);
  [DllImport("user32.dll")] public static extern void keybd_event(byte vk, byte scan, uint flags, IntPtr extra);
}
'@ }
$proc = Get-Process TF3-DRPC | Where-Object { $_.MainWindowHandle -ne 0 } | Select-Object -First 1
if (-not $proc) { throw 'TF3-DRPC window not found' }
"before: focus in app = " + ([System.Windows.Automation.AutomationElement]::FocusedElement.Current.ProcessId -eq $proc.Id)
[Fg]::keybd_event(0x12, 0, 0, [IntPtr]::Zero); [Fg]::keybd_event(0x12, 0, 2, [IntPtr]::Zero)
"SetForegroundWindow: " + [Fg]::SetForegroundWindow($proc.MainWindowHandle)
Start-Sleep -Milliseconds 600
$f = [System.Windows.Automation.AutomationElement]::FocusedElement.Current
"after return: focus in app = $($f.ProcessId -eq $proc.Id), focused = '$($f.Name)' ($($f.ControlType.ProgrammaticName))"
