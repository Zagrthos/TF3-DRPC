using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using CommunityToolkit.Mvvm.Messaging;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.LifecycleEvents;

using NReco.Logging.File;

using Tf3Drpc.App.Utilities;
using Tf3Drpc.App.Constants;
using Tf3Drpc.App.Extensions;
using Tf3Drpc.App.Messages;

#if WINDOWS
using Tf3Drpc.App.Platforms.Windows;
#elif MACCATALYST
using Tf3Drpc.App.Platforms.MacCatalyst;
#endif

namespace Tf3Drpc.App;

/// <summary>
/// Builds the MAUI app.
/// </summary>
internal static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        PlatformPathService pathService = new();
        PlatformSingleInstanceService singleInstanceService = new();
        PlatformWindowService windowService = new();
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<CompanionApplication>();
#if WINDOWS
        builder.ConfigureMauiHandlers(static handlers => handlers.AddHandler<Microsoft.Maui.Controls.Picker, WindowsPickerHandler>());
#elif MACCATALYST
        builder.ConfigureMauiHandlers(static handlers =>
        {
            handlers.AddHandler<Microsoft.Maui.Controls.Picker, MacPickerHandler>();
            handlers.AddHandler<Microsoft.Maui.Controls.CheckBox, MacCheckBoxHandler>();
        });
#endif
        builder.ConfigureLifecycleEvents(events =>
        {
#if WINDOWS
            events.AddWindows(windows => windows
                .OnAppInstanceActivated(singleInstanceService.HandleActivation)
                .OnWindowCreated(windowService.Attach));
#elif MACCATALYST
            events.AddiOS(ios => ios.SceneOnActivated(_ =>
            {
                windowService.Attach();
                singleInstanceService.Attach();
            }));
#endif
        });

        ConfigureLogging(builder.Logging, pathService);
        builder.Services.AddAppServices(pathService, singleInstanceService, windowService);
        builder.Services.AddAppViewModels();

        return builder.Build();
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "A failing log file must never keep the app from starting.")]
    [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "The service provider owns the file logger provider.")]
    private static void ConfigureLogging(ILoggingBuilder logging, PlatformPathService pathService)
    {
#if DEBUG
        logging.SetMinimumLevel(LogLevel.Debug);
        logging.AddDebug();
#else
        logging.SetMinimumLevel(LogLevel.Information);
#endif

        // The path service already counts an unwritable log folder as an unavailable log file.
        if (!pathService.IsLogDirectoryWritable)
            return;

        LogFileUtils.DeleteOldFiles(pathService.LogDirectory, DateTime.UtcNow);
        string template = System.IO.Path.Combine(pathService.LogDirectory, AppConstants.LogFilePrefix + "{0:yyyy-MM-dd}" + AppConstants.LogFileExtension);

        // The provider is created here, not lazily by the container, so that a failure cannot escape the start of the app.
        try
        {
            FileLoggerOptions options = new()
            {
                FormatLogFileName = static name => string.Format(CultureInfo.InvariantCulture, name, DateTime.Now),
                HandleFileError = _ => ReportLogFileFailure(pathService)
            };
            FileLoggerProvider provider = new(template, options);
            logging.Services.AddSingleton<ILoggerProvider>(_ => provider);
        }
        catch (Exception exception) when (exception is not OutOfMemoryException)
        {
            // The app runs without a log file; the settings window shows the storage problem.
            ReportLogFileFailure(pathService);
        }
    }

    // Runs on the logger's thread and for every failed write; only the first report reaches the window.
    private static void ReportLogFileFailure(PlatformPathService pathService)
    {
        if (pathService.ReportLogFileFailure())
            WeakReferenceMessenger.Default.Send(new LogFileUnavailableMessage());
    }
}
