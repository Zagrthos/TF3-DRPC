using System;
using System.Diagnostics.CodeAnalysis;

using Foundation;

using Microsoft.Maui.ApplicationModel;

using Tf3Drpc.App.Services.Interfaces;

using static Tf3Drpc.App.Utilities.ObjCRuntimeUtils;

namespace Tf3Drpc.App.Platforms.MacCatalyst;

/// <summary>
/// Single instance per user session on macOS: a second start asks the running copy to show its settings window.
/// </summary>
[SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable", Justification = "The service and its target live as long as the app; the container holds it as an instance and never disposes it.")]
internal sealed class MacSingleInstanceService : ISingleInstanceService
{
    private const string ShowSettingsSuffix = ".showSettings";
    private const uint AppleEventClass = 0x61657674; // 'aevt'
    private const uint ReopenEventId = 0x72617070; // 'rapp'
    private readonly ActivationTarget _target;
    private bool _reopenRegistered;

    public MacSingleInstanceService()
    {
        _target = new ActivationTarget(this);
        string? bundleId = NSBundle.MainBundle.BundleIdentifier;

        if (string.IsNullOrEmpty(bundleId))
            return;

        using NSString name = new(bundleId + ShowSettingsSuffix);
        nint center = Send(GetClass("NSDistributedNotificationCenter"), "defaultCenter");
        Send(center, "addObserver:selector:name:object:", _target.Handle, ObjCRuntime.Selector.GetHandle(ActivationTarget.NotifyAction), name.Handle, 0);
    }

    public event EventHandler? ActivationRequested;

    /// <summary>
    /// The handler for MAUI's <c>SceneOnActivated</c> lifecycle hook: a start of the running app from Finder or <c>open</c> sends it a reopen Apple event instead of starting a process.
    /// </summary>
    /// <remarks>Registered here because the handler set at construction is replaced while the app finishes launching.</remarks>
    public void Attach()
    {
        if (_reopenRegistered)
            return;

        _reopenRegistered = true;
        nint manager = Send(GetClass("NSAppleEventManager"), "sharedAppleEventManager");
        Send(manager, "setEventHandler:andSelector:forEventClass:andEventID:", _target.Handle, ObjCRuntime.Selector.GetHandle(ActivationTarget.ReopenAction), AppleEventClass, ReopenEventId);
    }

    /// <summary>
    /// Asks an older running process of this app to show its settings window; returns true when there is one and this process should exit.
    /// </summary>
    public static bool HandOverToRunningInstance()
    {
        string? bundleId = NSBundle.MainBundle.BundleIdentifier;

        if (string.IsNullOrEmpty(bundleId))
            return false;

        int ownId = Environment.ProcessId;
        using NSString id = new(bundleId);
        nint running = Send(GetClass("NSRunningApplication"), "runningApplicationsWithBundleIdentifier:", id.Handle);
        nint count = GetInteger(running, "count");

        for (nint i = 0; i < count; i++)
        {
            nint application = Send(running, "objectAtIndex:", i);
            int processId = (int)GetInteger(application, "processIdentifier");

            // Only an older process (a lower id) that still runs keeps the place, so of two starts at the same moment exactly one exits.
            if (processId >= ownId || GetBoolean(application, "isTerminated"))
                continue;

            using NSString name = new(bundleId + ShowSettingsSuffix);
            nint center = Send(GetClass("NSDistributedNotificationCenter"), "defaultCenter");
            Send(center, "postNotificationName:object:userInfo:deliverImmediately:", name.Handle, 0, 0, true);

            return true;
        }

        return false;
    }

    private void RaiseActivationRequested()
        => MainThread.BeginInvokeOnMainThread(() => ActivationRequested?.Invoke(this, EventArgs.Empty));

    /// <summary>
    /// The Objective-C target of the distributed notification and of the reopen Apple event.
    /// </summary>
    [Register("Tf3DrpcActivationTarget")]
    private sealed class ActivationTarget(MacSingleInstanceService owner) : NSObject
    {
        public const string NotifyAction = "showSettings:";
        public const string ReopenAction = "handleReopen:withReplyEvent:";

        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "The selector has this argument; the target ignores it.")]
        [Export(NotifyAction)]
        public void ShowSettings(NSNotification notification)
            => owner.RaiseActivationRequested();

        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "The selector has these arguments; the target ignores them.")]
        [Export(ReopenAction)]
        public void HandleReopen(NSObject eventDescriptor, NSObject replyEvent)
            => owner.RaiseActivationRequested();
    }
}
