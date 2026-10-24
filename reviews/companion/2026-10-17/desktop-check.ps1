param([switch]$Preflight, [ValidateSet('Normal', 'LockedLog', 'Tooltips')][string]$Scenario = 'Normal')
$ErrorActionPreference = 'Stop'
$reviewRoot = $PSScriptRoot
$repoRoot = (Resolve-Path (Join-Path $reviewRoot '../../..')).Path
$running = @(Get-Process -Name 'TF3-DRPC', 'TransportFever3' -ErrorAction SilentlyContinue)
if ($Preflight) {
    $running | Select-Object ProcessName, Id | ConvertTo-Json
    if ($running.Count -eq 0) { Write-Output 'No existing companion or game process.' }
    exit 0
}
if ($running.Count -gt 0) { throw 'Close the existing companion/game before this isolated desktop check.' }

Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type -AssemblyName System.Drawing
Add-Type @'
using System;
using System.Runtime.InteropServices;
public static class ReviewWindowNative {
    [StructLayout(LayoutKind.Sequential)] public struct Point { public int X; public int Y; }
    [DllImport("user32.dll")] public static extern uint GetDpiForWindow(IntPtr window);
    [DllImport("user32.dll")] public static extern bool SetWindowPos(IntPtr window, IntPtr after, int x, int y, int width, int height, uint flags);
    [DllImport("user32.dll")] public static extern bool SetCursorPos(int x, int y);
    [DllImport("user32.dll")] public static extern bool GetCursorPos(out Point point);
    [DllImport("user32.dll")] public static extern bool SetPhysicalCursorPos(int x, int y);
    [DllImport("user32.dll")] public static extern bool GetPhysicalCursorPos(out Point point);
    [DllImport("user32.dll")] public static extern int GetSystemMetrics(int index);
    [DllImport("user32.dll")] public static extern void mouse_event(uint flags, uint x, uint y, uint data, UIntPtr extra);
    [DllImport("user32.dll")] public static extern bool SetProcessDPIAware();
    [DllImport("user32.dll")] public static extern IntPtr SetThreadDpiAwarenessContext(IntPtr context);
}
'@
[ReviewWindowNative]::SetProcessDPIAware() | Out-Null
[ReviewWindowNative]::SetThreadDpiAwarenessContext([IntPtr]::new(-4)) | Out-Null
$publish = Join-Path $repoRoot 'companion/artifacts/publish/Tf3Drpc.App/release_win-x64'
$copy = Join-Path $reviewRoot ('artifacts/desktop-' + [Guid]::NewGuid().ToString('N'))
New-Item $copy -ItemType Directory | Out-Null
Get-ChildItem $publish | Where-Object Name -ne DATA | Copy-Item -Destination $copy -Recurse
$data = Join-Path $copy 'DATA'
New-Item $data -ItemType Directory | Out-Null
@{ language = 'en'; preset = 'custom'; templates = @{ details = '{rank}'; state = '{date}'; hover = 'Transport Fever 3' }; storeButton = $true } |
    ConvertTo-Json | Set-Content (Join-Path $data 'settings.json') -Encoding UTF8
$reviewProcess = $null
$lockedLog = $null
$results = [ordered]@{}
$results.scenario = $Scenario
if ($Scenario -eq 'LockedLog') {
    $logs = Join-Path $data 'logs'
    New-Item $logs -ItemType Directory | Out-Null
    $logFile = Join-Path $logs ('TF3-DRPC_' + [DateTime]::Now.ToString('yyyy-MM-dd') + '.log')
    $lockedLog = [System.IO.FileStream]::new($logFile, [System.IO.FileMode]::CreateNew, [System.IO.FileAccess]::ReadWrite, [System.IO.FileShare]::None)
}

function Find-ByType($element, $type) {
    $condition = [System.Windows.Automation.PropertyCondition]::new([System.Windows.Automation.AutomationElement]::ControlTypeProperty, $type)
    $element.FindAll([System.Windows.Automation.TreeScope]::Descendants, $condition)
}
# Rectangles and points are recorded in device-independent pixels, so the results don't depend on the display's scaling.
$script:Scale = 1.0
function ConvertTo-Dip([double]$value) { [Math]::Round($value / $script:Scale, 1) }
function Get-Rect($element) {
    $r = $element.Current.BoundingRectangle
    if ($r.IsEmpty) { return @{ x = $null; y = $null; width = $null; height = $null; offscreen = $element.Current.IsOffscreen } }
    @{ x = (ConvertTo-Dip $r.X); y = (ConvertTo-Dip $r.Y); width = (ConvertTo-Dip $r.Width); height = (ConvertTo-Dip $r.Height); offscreen = $element.Current.IsOffscreen }
}
function Read-Inputs($element) {
    $items = @()
    foreach ($type in @([System.Windows.Automation.ControlType]::ComboBox, [System.Windows.Automation.ControlType]::Edit, [System.Windows.Automation.ControlType]::CheckBox)) {
        foreach ($control in (Find-ByType $element $type)) {
            $label = $control.GetCurrentPropertyValue([System.Windows.Automation.AutomationElement]::LabeledByProperty)
            $labelName = if ($label -is [System.Windows.Automation.AutomationElement]) { $label.Current.Name } else { $null }
            $items += @{ type = $type.ProgrammaticName; name = $control.Current.Name; labelledBy = $labelName; rect = (Get-Rect $control) }
        }
    }
    $items
}
function Find-ProcessControls($type) {
    $pidCondition = [System.Windows.Automation.PropertyCondition]::new([System.Windows.Automation.AutomationElement]::ProcessIdProperty, $reviewProcess.Id)
    foreach ($root in [System.Windows.Automation.AutomationElement]::RootElement.FindAll([System.Windows.Automation.TreeScope]::Children, $pidCondition)) {
        if ($root.Current.ControlType -eq $type) { $root }
        foreach ($child in (Find-ByType $root $type)) { $child }
    }
}
function Set-Size($handle, $width, $height) {
    $scale = [ReviewWindowNative]::GetDpiForWindow($handle) / 96.0
    [ReviewWindowNative]::SetWindowPos($handle, [IntPtr]::Zero, 50, 50, [int]($width * $scale), [int]($height * $scale), 0x0040) | Out-Null
    Start-Sleep -Milliseconds 300
}
function Save-WindowImage($element, [string]$name) {
    $r = $element.Current.BoundingRectangle
    $bitmap = [System.Drawing.Bitmap]::new([int]$r.Width, [int]$r.Height)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    try {
        $graphics.CopyFromScreen([int]$r.X, [int]$r.Y, 0, 0, $bitmap.Size)
        $bitmap.Save((Join-Path $reviewRoot ('artifacts/' + $name + '.png')), [System.Drawing.Imaging.ImageFormat]::Png)
    }
    finally { $graphics.Dispose(); $bitmap.Dispose() }
}
function Move-ReviewPointer([int]$x, [int]$y) {
    $screenWidth = [ReviewWindowNative]::GetSystemMetrics(0)
    $screenHeight = [ReviewWindowNative]::GetSystemMetrics(1)
    [ReviewWindowNative]::mouse_event(0x8001, [uint32]($x * 65535 / ($screenWidth - 1)), [uint32]($y * 65535 / ($screenHeight - 1)), 0, [UIntPtr]::Zero)
}

try {
    $reviewProcess = Start-Process (Join-Path $copy 'TF3-DRPC.exe') -PassThru
    Start-Sleep -Seconds 2
    $second = Start-Process (Join-Path $copy 'TF3-DRPC.exe') -PassThru
    if (-not $second.WaitForExit(15000)) { throw 'Second-instance activation did not finish.' }
    $deadline = [DateTime]::UtcNow.AddSeconds(15)
    do {
        $reviewProcess.Refresh()
        if ($reviewProcess.HasExited) { throw 'Private companion exited unexpectedly.' }
        $handle = $reviewProcess.MainWindowHandle
        if ($handle -ne [IntPtr]::Zero) { break }
        Start-Sleep -Milliseconds 100
    } while ([DateTime]::UtcNow -lt $deadline)
    if ($handle -eq [IntPtr]::Zero) { throw 'No private settings window.' }
    $window = [System.Windows.Automation.AutomationElement]::FromHandle($handle)
    $script:Scale = [ReviewWindowNative]::GetDpiForWindow($handle) / 96.0
    Set-Size $handle 640 900
    $readyDeadline = [DateTime]::UtcNow.AddSeconds(15)
    do {
        $buttons = Find-ByType $window ([System.Windows.Automation.ControlType]::Button)
        $exitButton = @($buttons | Where-Object { $_.Current.Name -notin @('Minimize', 'Maximize', 'Close', 'Restore') })[-1]
        if ($null -ne $exitButton) { break }
        Start-Sleep -Milliseconds 200
    } while ([DateTime]::UtcNow -lt $readyDeadline)
    $results.initialInputs = @(Read-Inputs $window)
    $results.buttonNames = @($buttons | ForEach-Object { $_.Current.Name })
    $results.exitButtonName = $exitButton.Current.Name
    $results.storageWarningVisible = @((Find-ByType $window ([System.Windows.Automation.ControlType]::Text)) | Where-Object { $_.Current.Name -like '*write*folder*' -and -not $_.Current.IsOffscreen }).Count -gt 0
    if ($null -eq $exitButton) {
        $all = $window.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
        @($all | ForEach-Object { @{ name = $_.Current.Name; type = $_.Current.ControlType.ProgrammaticName; rect = (Get-Rect $_) } }) |
            ConvertTo-Json -Depth 6 | Set-Content (Join-Path $reviewRoot 'artifacts/desktop-diagnostic.json') -Encoding UTF8
        throw "No Exit button in private window: root name=$($window.Current.Name), root type=$($window.Current.ControlType.ProgrammaticName), children=$($all.Count), handle=$handle."
    }
    $results.normal = @{ window = (Get-Rect $window); exit = (Get-Rect $exitButton) }
    $secondary = $window.FindFirst([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.PropertyCondition]::new([System.Windows.Automation.AutomationElement]::NameProperty, 'Not connected to Discord'))
    $results.secondaryLabel = @{ rect = (Get-Rect $secondary); name = $secondary.Current.Name }
    $textPattern = $null
    if ($secondary.TryGetCurrentPattern([System.Windows.Automation.TextPattern]::Pattern, [ref]$textPattern)) {
        $results.secondaryLabel.fontSize = $textPattern.DocumentRange.GetAttributeValue([System.Windows.Automation.TextPattern]::FontSizeAttribute)
    }
    Save-WindowImage $window 'native-settings-normal'
    $allControls = $window.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
    foreach ($control in $allControls) {
        $scrollPattern = $null
        if ($control.TryGetCurrentPattern([System.Windows.Automation.ScrollPattern]::Pattern, [ref]$scrollPattern) -and $scrollPattern.Current.VerticallyScrollable) {
            $scrollPattern.SetScrollPercent(-1, 100)
            Start-Sleep -Milliseconds 200
            $checkbox = (Find-ByType $window ([System.Windows.Automation.ControlType]::CheckBox))[0]
            $results.visibleCheckbox = @{ name = $checkbox.Current.Name; rect = (Get-Rect $checkbox) }
            $scrollPattern.SetScrollPercent(-1, 0)
            Start-Sleep -Milliseconds 200
            break
        }
    }
    if ($Scenario -eq 'Tooltips') {
        $combo = (Find-ByType $window ([System.Windows.Automation.ControlType]::ComboBox))[1]
        $expand = $combo.GetCurrentPattern([System.Windows.Automation.ExpandCollapsePattern]::Pattern)
        $cursor = [ReviewWindowNative+Point]::new()
        [ReviewWindowNative]::GetCursorPos([ref]$cursor) | Out-Null
        try {
            $expand.Expand()
            Start-Sleep -Milliseconds 300
            $results.tooltips = @()
            foreach ($wanted in @('Career', 'Network', 'Era', 'Alternating', 'Minimal', 'Custom')) {
                $expand.Collapse()
                Move-ReviewPointer 80 80
                Start-Sleep -Milliseconds 200
                $expand.Expand()
                Start-Sleep -Milliseconds 200
                $item = @(Find-ProcessControls ([System.Windows.Automation.ControlType]::ListItem) | Where-Object { $_.Current.Name -eq $wanted -and -not $_.Current.IsOffscreen })[0]
                if ($null -eq $item) { throw "No visible preset item: $wanted" }
                $r = $item.Current.BoundingRectangle
                $pointerX = [int]($r.X + $r.Width / 2)
                $pointerY = [int]($r.Y + $r.Height / 2)
                Move-ReviewPointer $pointerX $pointerY
                $actualCursor = [ReviewWindowNative+Point]::new()
                [ReviewWindowNative]::GetPhysicalCursorPos([ref]$actualCursor) | Out-Null
                $hit = [System.Windows.Automation.AutomationElement]::FromPoint([System.Windows.Point]::new($pointerX, $pointerY))
                $hitName = if ($hit.Current.ProcessId -eq $reviewProcess.Id) { $hit.Current.Name } else { 'A different process' }
                Start-Sleep -Milliseconds 1800
                $tips = @(Find-ProcessControls ([System.Windows.Automation.ControlType]::ToolTip))
                Save-WindowImage $window ('native-tooltip-' + $wanted)
                $results.tooltips += @{ item = $wanted; hitName = $hitName; cursorX = (ConvertTo-Dip $actualCursor.X); cursorY = (ConvertTo-Dip $actualCursor.Y); targetX = (ConvertTo-Dip $pointerX); targetY = (ConvertTo-Dip $pointerY); rect = (Get-Rect $item); helpText = $item.Current.HelpText; tooltips = @($tips | ForEach-Object { @{ name = $_.Current.Name; text = @((Find-ByType $_ ([System.Windows.Automation.ControlType]::Text)) | ForEach-Object { $_.Current.Name }) } }) }
            }
        }
        finally { $expand.Collapse(); [ReviewWindowNative]::SetCursorPos($cursor.X, $cursor.Y) | Out-Null }
        $results.presetAfterTooltipCheck = (Get-Content (Join-Path $data 'settings.json') -Raw | ConvertFrom-Json).preset
        $results | ConvertTo-Json -Depth 8 | Set-Content (Join-Path $reviewRoot 'artifacts/desktop-tooltip-observations.json') -Encoding UTF8
        $expand.Expand()
        Start-Sleep -Milliseconds 200
        $customItem = @(Find-ProcessControls ([System.Windows.Automation.ControlType]::ListItem) | Where-Object { $_.Current.Name -eq 'Custom' })[0]
        if ($null -ne $customItem) { $customItem.GetCurrentPattern([System.Windows.Automation.SelectionItemPattern]::Pattern).Select() }
        $expand.Collapse()
        Start-Sleep -Milliseconds 200
    }
    $edits = Find-ByType $window ([System.Windows.Automation.ControlType]::Edit)
    if ($edits.Count -ne 3) { throw "Expected three custom template entries, found $($edits.Count)." }
    foreach ($edit in $edits) {
        $pattern = $edit.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern)
        $pattern.SetValue(('W' * 150))
    }
    Set-Size $handle 400 500
    $results.longTemplatesAtMinimum = @{ window = (Get-Rect $window); exit = (Get-Rect $exitButton); scrolls = @((Find-ByType $window ([System.Windows.Automation.ControlType]::Pane)) | ForEach-Object { Get-Rect $_ }) }
    $results.longTextBlocks = @((Find-ByType $window ([System.Windows.Automation.ControlType]::Text)) | Where-Object { $_.Current.Name.StartsWith('WW') } | ForEach-Object { @{ nameLength = $_.Current.Name.Length; rect = (Get-Rect $_) } })
    Save-WindowImage $window 'native-settings-minimum-long-text'
    Set-Size $handle 640 900
    $exitPattern = $exitButton.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern)
    $exitPattern.Invoke()
    $results.windowExitCompleted = $reviewProcess.WaitForExit(15000)
    $saved = Get-Content (Join-Path $data 'settings.json') -Raw | ConvertFrom-Json
    $results.savedTemplateLength = $saved.templates.details.Length
    $results.nativeExe = @{ bytes = (Get-Item (Join-Path $copy 'TF3-DRPC.exe')).Length; secondInstanceExitCode = $second.ExitCode }
    $results.R07 = (@($results.initialInputs | Where-Object { $_.name -eq '' -and $null -eq $_.labelledBy }).Count -eq 6) -and ($results.exitButtonName -eq '')
    $results.R08 = $results.longTemplatesAtMinimum.exit.offscreen -and (-not $results.normal.exit.offscreen)
    if ($null -ne $lockedLog) {
        $results.logBytes = $lockedLog.Length
        $results.R10 = ($lockedLog.Length -eq 0) -and (-not $results.storageWarningVisible)
    }
    $outputName = switch ($Scenario) { 'Normal' { 'latest-desktop-results-windows.json' } 'LockedLog' { 'latest-desktop-locked-log-windows.json' } 'Tooltips' { 'latest-desktop-tooltips-windows.json' } }
    $results | ConvertTo-Json -Depth 8 | Set-Content (Join-Path $reviewRoot ('artifacts/' + $outputName)) -Encoding UTF8
    $results | ConvertTo-Json -Depth 8
}
finally {
    if ($null -ne $reviewProcess) {
        $reviewProcess.Refresh()
        if (-not $reviewProcess.HasExited) { $reviewProcess.Kill(); $reviewProcess.WaitForExit() }
    }
    if ($null -ne $lockedLog) { $lockedLog.Dispose() }
    Remove-Item $copy -Recurse -Force
}
