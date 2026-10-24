import ApplicationServices
import CoreGraphics
import Foundation
// Prints the on-screen normal windows of a process, the frontmost window's owner, and with "close" presses the close button.
let pid = Int32(CommandLine.arguments[1])!
let list = CGWindowListCopyWindowInfo([.optionOnScreenOnly, .excludeDesktopElements], kCGNullWindowID) as! [[String: Any]]
let normal = list.filter { ($0[kCGWindowLayer as String] as? Int) == 0 }
let own = normal.filter { ($0[kCGWindowOwnerPID as String] as? Int32) == pid }
print("visible windows:", own.count, own.map { $0[kCGWindowBounds as String]! })
print("front owner is app:", (normal.first?[kCGWindowOwnerPID as String] as? Int32) == pid)
if CommandLine.arguments.count > 2 {
    let app = AXUIElementCreateApplication(pid)
    var windows: AnyObject?
    AXUIElementCopyAttributeValue(app, kAXWindowsAttribute as CFString, &windows)
    for w in (windows as? [AXUIElement]) ?? [] {
        var button: AnyObject?
        if AXUIElementCopyAttributeValue(w, kAXCloseButtonAttribute as CFString, &button) == .success {
            print("close:", AXUIElementPerformAction(button as! AXUIElement, kAXPressAction as CFString) == .success)
        }
    }
}
