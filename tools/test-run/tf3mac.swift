// Native helper of the test-run scripts on macOS: window lookup, focus and input through
// CoreGraphics and the Accessibility API. common.ps1 compiles it into artifacts/bin/tf3mac.
//   tf3mac window <pid>           the process's largest window: "<id> <x> <y> <w> <h>" in points
//   tf3mac frontpid               pid of the owner of the frontmost normal window
//   tf3mac activate <pid>         bring the app to the front; exit code 1 if it isn't afterwards
//   tf3mac click <x> <y> [right]  click at global coordinates in points
//   tf3mac key <name> [count]     press a key by name (ESCAPE, RETURN, SPACE, F5, A, D1, ...)
//   tf3mac type <text>            type text with the keys of the current layout (Unicode otherwise)
//   tf3mac pixels <png> <out> [width]   the picture's pixels as raw 8-bit B, G, R, A bytes (sRGB, row after row,
//                                 no padding) in <out>, scaled to [width] if given; prints "<width> <height>"
// Input needs the Accessibility permission, window pictures the Screen Recording permission,
// both for the app that runs the script (Terminal, VS Code).
import ApplicationServices
import Carbon
import Foundation
import ImageIO

func fail(_ message: String) -> Never {
    FileHandle.standardError.write(Data((message + "\n").utf8))
    exit(1)
}

// Normal windows (layer 0) front to back, as CoreGraphics lists them. Without onScreenOnly the list
// also has windows on another Space, such as the game's while another app is in front.
func windows(onScreenOnly: Bool = true) -> [[String: Any]] {
    let options: CGWindowListOption = onScreenOnly ? [.optionOnScreenOnly, .excludeDesktopElements] : [.excludeDesktopElements]
    let list = CGWindowListCopyWindowInfo(options, kCGNullWindowID) as? [[String: Any]] ?? []
    return list.filter { ($0[kCGWindowLayer as String] as? Int) == 0 }
}

func bounds(_ window: [String: Any]) -> CGRect {
    CGRect(dictionaryRepresentation: window[kCGWindowBounds as String] as! CFDictionary) ?? .zero
}

func frontPid() -> Int32? {
    windows().first?[kCGWindowOwnerPID as String] as? Int32
}

func post(_ event: CGEvent?) {
    event?.post(tap: .cghidEventTap)
    usleep(20_000)
}

let keyCodes: [String: CGKeyCode] = [
    "ESCAPE": 53, "RETURN": 36, "ENTER": 36, "SPACE": 49, "TAB": 48, "BACK": 51, "DELETE": 117,
    "LEFT": 123, "RIGHT": 124, "DOWN": 125, "UP": 126, "HOME": 115, "END": 119, "PAGEUP": 116, "PAGEDOWN": 121,
    "F1": 122, "F2": 120, "F3": 99, "F4": 118, "F5": 96, "F6": 97, "F7": 98, "F8": 100, "F9": 101, "F10": 109,
    "F11": 103, "F12": 111,
    // Letters and digits by their key position on a US keyboard, like the Windows scan codes.
    "A": 0, "S": 1, "D": 2, "F": 3, "H": 4, "G": 5, "Z": 6, "X": 7, "C": 8, "V": 9, "B": 11, "Q": 12, "W": 13,
    "E": 14, "R": 15, "Y": 16, "T": 17, "O": 31, "U": 32, "I": 34, "P": 35, "L": 37, "J": 38, "K": 40, "N": 45,
    "M": 46, "D1": 18, "D2": 19, "D3": 20, "D4": 21, "D5": 23, "D6": 22, "D7": 26, "D8": 28, "D9": 25, "D0": 29,
]

let modifierKeys: [(code: CGKeyCode, flag: CGEventFlags)] = [(56, .maskShift), (58, .maskAlternate)]

func postKey(_ code: CGKeyCode, down: Bool, flags: CGEventFlags) {
    let event = CGEvent(keyboardEventSource: nil, virtualKey: code, keyDown: down)
    event?.flags = flags
    post(event)
}

// The key and modifiers (none, Shift, Option, both) that type each character on the current
// keyboard layout, found by translating every key code through the layout.
func layoutKeys() -> [Character: (CGKeyCode, CGEventFlags)] {
    guard let source = TISCopyCurrentKeyboardLayoutInputSource()?.takeRetainedValue(),
        let property = TISGetInputSourceProperty(source, kTISPropertyUnicodeKeyLayoutData)
    else { return [:] }
    let data = Unmanaged<CFData>.fromOpaque(property).takeUnretainedValue() as Data
    // UCKeyTranslate takes the Carbon modifier bits shifted right by 8: Shift 2, Option 8.
    let combinations: [(UInt32, CGEventFlags)] = [(0, []), (2, .maskShift), (8, .maskAlternate), (10, [.maskShift, .maskAlternate])]
    var keys: [Character: (CGKeyCode, CGEventFlags)] = [:]
    data.withUnsafeBytes { (buffer: UnsafeRawBufferPointer) in
        let layout = buffer.bindMemory(to: UCKeyboardLayout.self).baseAddress!
        for (modifierState, flags) in combinations {
            for code in 0..<UInt16(128) {
                var deadKeyState: UInt32 = 0
                var length = 0
                var characters = [UniChar](repeating: 0, count: 4)
                let status = UCKeyTranslate(
                    layout, code, UInt16(kUCKeyActionDown), modifierState, UInt32(LMGetKbdType()),
                    OptionBits(kUCKeyTranslateNoDeadKeysBit), &deadKeyState, characters.count, &length, &characters)
                guard status == noErr, length > 0 else { continue }
                let text = String(utf16CodeUnits: characters, count: length)
                // The first, simplest way wins; control characters aren't typed this way.
                if text.count == 1, let character = text.first, keys[character] == nil, !(character.asciiValue.map { $0 < 32 } ?? false) {
                    keys[character] = (CGKeyCode(code), flags)
                }
            }
        }
    }
    return keys
}

let args = CommandLine.arguments.dropFirst().map { $0 }
guard let command = args.first else { fail("usage: tf3mac window|frontpid|activate|click|key|type|pixels ...") }

switch command {
case "window":
    guard args.count == 2, let pid = Int32(args[1]) else { fail("usage: tf3mac window <pid>") }
    let own = windows(onScreenOnly: false).filter { ($0[kCGWindowOwnerPID as String] as? Int32) == pid }
    guard let window = own.max(by: { bounds($0).width * bounds($0).height < bounds($1).width * bounds($1).height }) else {
        fail("no window of process \(pid)")
    }
    let rect = bounds(window)
    print("\(window[kCGWindowNumber as String] as! Int) \(Int(rect.minX)) \(Int(rect.minY)) \(Int(rect.width)) \(Int(rect.height))")
case "frontpid":
    guard let pid = frontPid() else { fail("no window in front") }
    print(pid)
case "activate":
    guard args.count == 2, let pid = Int32(args[1]) else { fail("usage: tf3mac activate <pid>") }
    if frontPid() != pid {
        // Through the Accessibility API, because NSRunningApplication.activate may be refused
        // to a process that isn't active itself.
        AXUIElementSetAttributeValue(AXUIElementCreateApplication(pid), kAXFrontmostAttribute as CFString, kCFBooleanTrue)
        for _ in 0..<20 where frontPid() != pid { usleep(100_000) }
    }
    if frontPid() != pid { fail("process \(pid) is not in front") }
case "click":
    guard args.count >= 3, let x = Double(args[1]), let y = Double(args[2]) else { fail("usage: tf3mac click <x> <y> [right]") }
    let right = args.count > 3 && args[3] == "right"
    let point = CGPoint(x: x, y: y)
    let (down, up, button): (CGEventType, CGEventType, CGMouseButton) =
        right ? (.rightMouseDown, .rightMouseUp, .right) : (.leftMouseDown, .leftMouseUp, .left)
    post(CGEvent(mouseEventSource: nil, mouseType: .mouseMoved, mouseCursorPosition: point, mouseButton: button))
    usleep(100_000)
    for type in [down, up] {
        // SDL ignores a press without a click count.
        let event = CGEvent(mouseEventSource: nil, mouseType: type, mouseCursorPosition: point, mouseButton: button)
        event?.setIntegerValueField(.mouseEventClickState, value: 1)
        post(event)
        usleep(60_000)
    }
case "key":
    guard args.count >= 2, let code = keyCodes[args[1].uppercased()] else {
        fail("unknown key; known: \(keyCodes.keys.sorted().joined(separator: " "))")
    }
    let count = args.count > 2 ? Int(args[2]) ?? 1 : 1
    for _ in 0..<count {
        post(CGEvent(keyboardEventSource: nil, virtualKey: code, keyDown: true))
        usleep(60_000)
        post(CGEvent(keyboardEventSource: nil, virtualKey: code, keyDown: false))
        usleep(120_000)
    }
case "type":
    guard args.count == 2 else { fail("usage: tf3mac type <text>") }
    let layout = layoutKeys()
    for character in args[1] {
        let units = Array(String(character).utf16)
        // A character on the layout is typed with its own key, as a person types it: in one run a
        // Mac Catalyst text field took real keys but not events with only a Unicode string (the
        // cause wasn't found; later runs took both). The string is kept for SDL and for
        // characters off the layout.
        let (code, flags) = layout[character] ?? (0, [])
        let modifiers = modifierKeys.filter { flags.contains($0.flag) }
        for modifier in modifiers { postKey(modifier.code, down: true, flags: flags) }
        for keyDown in [true, false] {
            let event = CGEvent(keyboardEventSource: nil, virtualKey: code, keyDown: keyDown)
            event?.flags = flags
            event?.keyboardSetUnicodeString(stringLength: units.count, unicodeString: units)
            post(event)
        }
        for modifier in modifiers.reversed() { postKey(modifier.code, down: false, flags: []) }
        usleep(40_000)
    }
case "pixels":
    guard args.count >= 3, args.count <= 4 else { fail("usage: tf3mac pixels <png> <out> [width]") }
    guard let source = CGImageSourceCreateWithURL(URL(fileURLWithPath: args[1]) as CFURL, nil),
        let image = CGImageSourceCreateImageAtIndex(source, 0, nil)
    else { fail("cannot read the picture \(args[1])") }
    let width = args.count == 4 ? Int(args[3]) ?? 0 : image.width
    guard width > 0 else { fail("bad width \(args.last!)") }
    let height = max(1, Int((Double(image.height) * Double(width) / Double(image.width)).rounded()))
    var buffer = [UInt8](repeating: 0, count: width * height * 4)
    // premultipliedFirst with 32-bit little-endian order is B, G, R, A in memory; the first row is the top row.
    let bitmapInfo = CGImageAlphaInfo.premultipliedFirst.rawValue | CGBitmapInfo.byteOrder32Little.rawValue
    let drawn = buffer.withUnsafeMutableBytes { (raw: UnsafeMutableRawBufferPointer) -> Bool in
        guard
            let context = CGContext(
                data: raw.baseAddress, width: width, height: height, bitsPerComponent: 8, bytesPerRow: width * 4,
                space: CGColorSpace(name: CGColorSpace.sRGB) ?? CGColorSpaceCreateDeviceRGB(), bitmapInfo: bitmapInfo)
        else { return false }
        context.interpolationQuality = .high
        context.draw(image, in: CGRect(x: 0, y: 0, width: width, height: height))
        return true
    }
    guard drawn else { fail("cannot make a \(width)x\(height) bitmap") }
    do {
        try Data(buffer).write(to: URL(fileURLWithPath: args[2]))
    } catch {
        fail("cannot write \(args[2]): \(error)")
    }
    print("\(width) \(height)")
default:
    fail("unknown command \(command)")
}
