# Invokes a button of the TF3-DRPC window by its UIA name through InvokePattern

param([string]$Name)
Add-Type -AssemblyName UIAutomationClient, UIAutomationTypes
$proc = Get-Process TF3-DRPC | Where-Object { $_.MainWindowHandle -ne 0 } | Select-Object -First 1
if (-not $proc) { throw 'TF3-DRPC window not found; nothing was sent' }
$root = [System.Windows.Automation.AutomationElement]::FromHandle($proc.MainWindowHandle)
$cond = New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::NameProperty, $Name)
$el = $root.FindFirst([System.Windows.Automation.TreeScope]::Descendants, $cond)
"found: $($el.Current.Name) patterns: " + (($el.GetSupportedPatterns() | ForEach-Object { $_.ProgrammaticName }) -join ', ')
$el.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern).Invoke()
"invoked at $(Get-Date -Format HH:mm:ss) in process $($proc.Id)"
