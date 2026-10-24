using UIKit;

namespace Tf3Drpc.App.Platforms.MacCatalyst;

/// <summary>
/// The entry point of the Mac Catalyst app.
/// </summary>
internal static class Program
{
    private static void Main(string[] args)
    {
        if (MacSingleInstanceService.HandOverToRunningInstance())
            return;

        UIApplication.Main(args, null, typeof(AppDelegate));
    }
}
