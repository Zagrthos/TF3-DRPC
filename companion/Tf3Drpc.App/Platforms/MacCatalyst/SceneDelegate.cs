using Foundation;

using Microsoft.Maui;

namespace Tf3Drpc.App.Platforms.MacCatalyst;

/// <summary>
/// The scene delegate that Info.plist names; MAUI creates the window in it.
/// </summary>
[Register("SceneDelegate")]
#pragma warning disable CA1812 // Avoid uninstantiated internal classes
internal sealed class SceneDelegate : MauiUISceneDelegate;
#pragma warning restore CA1812
