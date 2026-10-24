# Keyboard user activates the GitHub button with Space, then the footer button states and the keyboard focus are read during and after the command
Add-Type -AssemblyName UIAutomationClient, UIAutomationTypes
$ErrorActionPreference = 'Stop'
& "$PSScriptRoot\key-button.ps1" -Name GitHub
$proc = Get-Process TF3-DRPC | Where-Object { $_.MainWindowHandle -ne 0 } | Select-Object -First 1
function Show($label) {
  $f = [System.Windows.Automation.AutomationElement]::FocusedElement.Current
  "--- $label"
  "focused: name='$($f.Name)' type=$($f.ControlType.ProgrammaticName) sameProcess=$($f.ProcessId -eq $proc.Id)"
  & "$PSScriptRoot\uia-buttons.ps1" -Filter '^(GitHub|mod\.io|Beenden|Exit)$'
}
Start-Sleep -Milliseconds 500
Show '0.5 s after Space'
Start-Sleep -Seconds 4
Show '4.5 s after Space'
