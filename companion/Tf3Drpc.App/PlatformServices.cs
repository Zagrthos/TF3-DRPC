// The platform implementations of the services, named once for all platforms; MauiProgram creates
// and ServiceCollectionExtensions registers them through these names.
#if WINDOWS
global using PlatformPathService = Tf3Drpc.App.Platforms.Windows.WindowsPathService;
global using PlatformSingleInstanceService = Tf3Drpc.App.Platforms.Windows.WindowsSingleInstanceService;
global using PlatformTrayService = Tf3Drpc.App.Platforms.Windows.WindowsTrayService;
global using PlatformWindowService = Tf3Drpc.App.Platforms.Windows.WindowsWindowService;
#elif MACCATALYST
global using PlatformPathService = Tf3Drpc.App.Platforms.MacCatalyst.MacPathService;
global using PlatformSingleInstanceService = Tf3Drpc.App.Platforms.MacCatalyst.MacSingleInstanceService;
global using PlatformTrayService = Tf3Drpc.App.Platforms.MacCatalyst.MacTrayService;
global using PlatformWindowService = Tf3Drpc.App.Platforms.MacCatalyst.MacWindowService;
#else
#error Name the platform services of the new platform here.
#endif
