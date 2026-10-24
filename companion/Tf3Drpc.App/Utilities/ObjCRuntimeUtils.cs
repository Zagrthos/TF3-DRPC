#if MACCATALYST
using System;
using System.Runtime.InteropServices;

using ObjCRuntime;

namespace Tf3Drpc.App.Utilities;

/// <summary>
/// Messages to AppKit objects, which Mac Catalyst has no bindings for; each overload is one <c>objc_msgSend</c> signature.
/// </summary>
internal static partial class ObjCRuntimeUtils
{
    private const string ObjCLibrary = "/usr/lib/libobjc.dylib";

    /// <summary>
    /// A point size, as AppKit's <c>NSSize</c>.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly record struct NativeSize(double Width, double Height);

    /// <summary>
    /// A point, as AppKit's <c>NSPoint</c>.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly record struct NativePoint(double X, double Y);

    /// <summary>
    /// A rectangle in screen points from the bottom left, as AppKit's <c>NSRect</c>.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly record struct NativeRect(double X, double Y, double Width, double Height);

    public static nint GetClass(string name)
        => Class.GetHandle(name);

    public static nint Send(in nint receiver, string selector)
        => MsgPointer(receiver, Selector.GetHandle(selector));

    public static nint Send(in nint receiver, string selector, in nint argument)
        => MsgPointer(receiver, Selector.GetHandle(selector), argument);

    public static nint Send(in nint receiver, string selector, in nint first, in nint second, in nint third)
        => MsgPointer(receiver, Selector.GetHandle(selector), first, second, third);

    public static nint Send(in nint receiver, string selector, in nint first, in nint second, in nint third, in nint fourth)
        => MsgPointer(receiver, Selector.GetHandle(selector), first, second, third, fourth);

    public static nint Send(in nint receiver, string selector, in nint first, in nint second, in nint third, bool fourth)
        => MsgPointer(receiver, Selector.GetHandle(selector), first, second, third, fourth);

    public static nint Send(in nint receiver, string selector, in nint first, in nint second, uint third, uint fourth)
        => MsgPointer(receiver, Selector.GetHandle(selector), first, second, third, fourth);

    public static nint Send(in nint receiver, string selector, double argument)
        => MsgPointer(receiver, Selector.GetHandle(selector), argument);

    public static void SendInteger(in nint receiver, string selector, in nint argument)
        => MsgVoid(receiver, Selector.GetHandle(selector), argument);

    public static void SendSize(in nint receiver, string selector, in NativeSize argument)
        => MsgVoid(receiver, Selector.GetHandle(selector), argument);

    public static void SendPoint(in nint receiver, string selector, in NativePoint argument)
        => MsgVoid(receiver, Selector.GetHandle(selector), argument);

    // arm64 returns a struct of up to four doubles in registers, so objc_msgSend serves here too (only arm64 is built).
    public static NativeRect GetRect(in nint receiver, string selector)
        => MsgRect(receiver, Selector.GetHandle(selector));

    public static nint GetInteger(in nint receiver, string selector)
        => MsgInteger(receiver, Selector.GetHandle(selector));

    public static bool GetBoolean(in nint receiver, string selector)
        => MsgBoolean(receiver, Selector.GetHandle(selector));

    public static void SendBoolean(in nint receiver, string selector, bool argument)
        => MsgVoid(receiver, Selector.GetHandle(selector), argument);

    [LibraryImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    private static partial nint MsgPointer(nint receiver, nint selector);

    [LibraryImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    private static partial nint MsgPointer(nint receiver, nint selector, nint argument);

    [LibraryImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    private static partial nint MsgPointer(nint receiver, nint selector, nint first, nint second, nint third);

    [LibraryImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    private static partial nint MsgPointer(nint receiver, nint selector, nint first, nint second, nint third, nint fourth);

    [LibraryImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    private static partial nint MsgPointer(nint receiver, nint selector, nint first, nint second, nint third, [MarshalAs(UnmanagedType.U1)] bool fourth);

    [LibraryImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    private static partial nint MsgPointer(nint receiver, nint selector, nint first, nint second, uint third, uint fourth);

    [LibraryImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    private static partial nint MsgPointer(nint receiver, nint selector, double argument);

    [LibraryImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    private static partial void MsgVoid(nint receiver, nint selector, nint argument);

    [LibraryImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    private static partial void MsgVoid(nint receiver, nint selector, NativeSize argument);

    [LibraryImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    private static partial void MsgVoid(nint receiver, nint selector, NativePoint argument);

    [LibraryImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    private static partial void MsgVoid(nint receiver, nint selector, [MarshalAs(UnmanagedType.U1)] bool argument);

    [LibraryImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    private static partial NativeRect MsgRect(nint receiver, nint selector);

    [LibraryImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    private static partial nint MsgInteger(nint receiver, nint selector);

    [LibraryImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    [return: MarshalAs(UnmanagedType.U1)]
    private static partial bool MsgBoolean(nint receiver, nint selector);
}
#endif
