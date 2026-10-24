import ApplicationServices
import Foundation
// Prints the menu bar extras of a process and, with "open", the entries of its first menu.
let pid = Int32(CommandLine.arguments[1])!
let open = CommandLine.arguments.count > 2
let app = AXUIElementCreateApplication(pid)
func attr(_ e: AXUIElement, _ name: String) -> AnyObject? {
    var value: AnyObject?
    return AXUIElementCopyAttributeValue(e, name as CFString, &value) == .success ? value : nil
}
func describe(_ e: AXUIElement) -> String {
    let title = attr(e, kAXTitleAttribute) as? String ?? ""
    let desc = attr(e, kAXDescriptionAttribute) as? String ?? ""
    let mark = attr(e, "AXMenuItemMarkChar") as? String ?? ""
    let enabled = attr(e, kAXEnabledAttribute) as? Bool ?? false
    return "\(mark.isEmpty ? " " : "✓") \(title)\(desc.isEmpty ? "" : " [\(desc)]")\(enabled ? "" : " (disabled)")"
}
func dump(_ e: AXUIElement, _ depth: Int) {
    for child in (attr(e, kAXChildrenAttribute) as? [AXUIElement]) ?? [] {
        let role = attr(child, kAXRoleAttribute) as? String ?? ""
        if role == "AXMenuItem" { print(String(repeating: "  ", count: depth) + describe(child)) }
        dump(child, role == "AXMenuItem" ? depth + 1 : depth)
    }
}
guard let extras = attr(app, "AXExtrasMenuBar") else { print("no menu bar extra"); exit(1) }
let items = (attr(extras as! AXUIElement, kAXChildrenAttribute) as? [AXUIElement]) ?? []
for item in items { print("extra:", describe(item)) }
if open, let item = items.first {
    AXUIElementPerformAction(item, kAXPressAction as CFString)
    usleep(500_000)
    dump(item, 1)
    let esc = CGEvent(keyboardEventSource: nil, virtualKey: 53, keyDown: true); esc?.post(tap: .cghidEventTap)
    let escUp = CGEvent(keyboardEventSource: nil, virtualKey: 53, keyDown: false); escUp?.post(tap: .cghidEventTap)
}
