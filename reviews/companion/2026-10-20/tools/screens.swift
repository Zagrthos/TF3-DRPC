import CoreGraphics
var ids = [CGDirectDisplayID](repeating: 0, count: 8); var n: UInt32 = 0
CGGetActiveDisplayList(8, &ids, &n)
for i in 0..<Int(n) { print(ids[i], CGDisplayBounds(ids[i]), CGDisplayIsMain(ids[i]) != 0 ? "main" : "") }
