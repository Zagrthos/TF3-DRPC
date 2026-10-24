# Lists the buttons and texts of the TF3-DRPC window with UIA name, help text, enabled state and bounds (physical px)

param([string]$Filter = '')
Add-Type -AssemblyName UIAutomationClient, UIAutomationTypes
$proc = Get-Process TF3-DRPC | Where-Object { $_.MainWindowHandle -ne 0 } | Select-Object -First 1
if (-not $proc) { throw 'TF3-DRPC window not found; nothing was sent' }
$root = [System.Windows.Automation.AutomationElement]::FromHandle($proc.MainWindowHandle)
$all = $root.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
foreach ($e in $all) {
  $c = $e.Current
  $type = $c.ControlType.ProgrammaticName -replace 'ControlType.',''
  if ($type -notin 'Button','Text') { continue }
  if ($Filter -and ($c.Name -notmatch $Filter)) { continue }
  $r = $c.BoundingRectangle
  '{0,-7} name="{1}" help="{2}" enabled={3} focusable={4} rect=({5},{6},{7}x{8})' -f $type, $c.Name, $c.HelpText, $c.IsEnabled, $c.IsKeyboardFocusable, $r.X, $r.Y, $r.Width, $r.Height
}
