import ApplicationServices
import Foundation
// Clicks the first menu bar extra of a process with the mouse, then the menu item with the given title.
let pid = Int32(CommandLine.arguments[1])!
let title = CommandLine.arguments[2]
func attr(_ e: AXUIElement, _ name: String) -> AnyObject? {
    var value: AnyObject?
    return AXUIElementCopyAttributeValue(e, name as CFString, &value) == .success ? value : nil
}
func center(_ e: AXUIElement) -> CGPoint {
    var p = CGPoint.zero, s = CGSize.zero
    AXValueGetValue(attr(e, kAXPositionAttribute) as! AXValue, .cgPoint, &p)
    AXValueGetValue(attr(e, kAXSizeAttribute) as! AXValue, .cgSize, &s)
    return CGPoint(x: p.x + s.width / 2, y: p.y + s.height / 2)
}
func click(_ point: CGPoint) {
    for type in [CGEventType.mouseMoved, .leftMouseDown, .leftMouseUp] {
        let e = CGEvent(mouseEventSource: nil, mouseType: type, mouseCursorPosition: point, mouseButton: .left)
        e?.setIntegerValueField(.mouseEventClickState, value: 1)
        e?.post(tap: .cghidEventTap); usleep(80_000)
    }
}
func find(_ e: AXUIElement) -> AXUIElement? {
    for child in (attr(e, kAXChildrenAttribute) as? [AXUIElement]) ?? [] {
        if (attr(child, kAXRoleAttribute) as? String) == "AXMenuItem" && (attr(child, kAXTitleAttribute) as? String) == title { return child }
        if let hit = find(child) { return hit }
    }
    return nil
}
let app = AXUIElementCreateApplication(pid)
let extra = ((attr(attr(app, "AXExtrasMenuBar") as! AXUIElement, kAXChildrenAttribute) as? [AXUIElement]) ?? [])[0]
click(center(extra))
usleep(600_000)
guard let item = find(extra) else { print("menu item not found"); exit(1) }
click(center(item))
print("clicked", title)
