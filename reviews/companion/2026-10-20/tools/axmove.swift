import ApplicationServices
import Foundation
// Moves the first window of a process to the given global position (points).
let pid = Int32(CommandLine.arguments[1])!
var point = CGPoint(x: Double(CommandLine.arguments[2])!, y: Double(CommandLine.arguments[3])!)
let app = AXUIElementCreateApplication(pid)
var windows: AnyObject?
AXUIElementCopyAttributeValue(app, kAXWindowsAttribute as CFString, &windows)
guard let window = (windows as? [AXUIElement])?.first else { print("no window"); exit(1) }
let value = AXValueCreate(.cgPoint, &point)!
print("move:", AXUIElementSetAttributeValue(window, kAXPositionAttribute as CFString, value) == .success)
