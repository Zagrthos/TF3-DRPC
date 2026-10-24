import ApplicationServices
import Foundation
// Opens the first menu bar extra of a process and presses the menu item at the given title path.
let pid = Int32(CommandLine.arguments[1])!
let path = Array(CommandLine.arguments.dropFirst(2))
func attr(_ e: AXUIElement, _ name: String) -> AnyObject? {
    var value: AnyObject?
    return AXUIElementCopyAttributeValue(e, name as CFString, &value) == .success ? value : nil
}
func find(_ e: AXUIElement, _ title: String) -> AXUIElement? {
    for child in (attr(e, kAXChildrenAttribute) as? [AXUIElement]) ?? [] {
        if (attr(child, kAXRoleAttribute) as? String) == "AXMenuItem" && (attr(child, kAXTitleAttribute) as? String) == title { return child }
        if let hit = find(child, title) { return hit }
    }
    return nil
}
let app = AXUIElementCreateApplication(pid)
guard let extras = attr(app, "AXExtrasMenuBar"), let item = ((attr(extras as! AXUIElement, kAXChildrenAttribute) as? [AXUIElement]) ?? []).first else { print("no extra"); exit(1) }
AXUIElementPerformAction(item, kAXPressAction as CFString)
usleep(400_000)
var current: AXUIElement = item
for (i, title) in path.enumerated() {
    guard let next = find(current, title) else { print("not found: \(title)"); exit(1) }
    if i < path.count - 1 { AXUIElementPerformAction(next, kAXPressAction as CFString); usleep(300_000) }
    else { print("press:", AXUIElementPerformAction(next, kAXPressAction as CFString) == .success) }
    current = next
}
