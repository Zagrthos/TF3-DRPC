# Keyboard user activates a footer button with Space; records every change of the keyboard focus and of the footer buttons' enabled state for 2 s
param([string]$Name = 'GitHub')
Add-Type -AssemblyName UIAutomationClient, UIAutomationTypes
$ErrorActionPreference = 'Stop'
$proc = Get-Process TF3-DRPC | Where-Object { $_.MainWindowHandle -ne 0 } | Select-Object -First 1
if (-not $proc) { throw 'TF3-DRPC window not found; nothing was sent' }
$root = [System.Windows.Automation.AutomationElement]::FromHandle($proc.MainWindowHandle)
function Btn($n) { $root.FindFirst([System.Windows.Automation.TreeScope]::Descendants, (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::NameProperty, $n))) }
$gh = Btn 'GitHub'; $mo = Btn 'mod.io'
& "$PSScriptRoot\key-button.ps1" -Name $Name
$sw = [Diagnostics.Stopwatch]::StartNew(); $last = ''
while ($sw.ElapsedMilliseconds -lt 2000) {
  $f = [System.Windows.Automation.AutomationElement]::FocusedElement.Current
  $who = if ($f.ProcessId -eq $proc.Id) { "app:'$($f.Name)'" } else { 'another app' }
  $line = "focus=$who GitHub.enabled=$($gh.Current.IsEnabled) mod.io.enabled=$($mo.Current.IsEnabled)"
  if ($line -ne $last) { '{0,5} ms  {1}' -f $sw.ElapsedMilliseconds, $line; $last = $line }
  Start-Sleep -Milliseconds 10
}
