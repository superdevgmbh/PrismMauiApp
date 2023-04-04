using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using NLog.Extensions.Logging;
using PrismMauiApp.Platforms.Services;
using PrismMauiApp.Services;
using PrismMauiApp.Services.Logging;

namespace PrismMauiApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCompatibility()
            .ConfigureMauiHandlers(handlers =>
            {
                //Xamarin.Forms Renderers

                //#if IOS
                //                handlers.AddCompatibilityRenderer(typeof(Page1ModalPage), typeof(ModalPageCustomRenderer));
                //                handlers.AddCompatibilityRenderer(typeof(TabbedPageRuntimeModal), typeof(ModalPageCustomRenderer));
                //                handlers.AddCompatibilityRenderer(typeof(TabModalPage), typeof(ModalPageCustomRenderer));
                //#endif
            })
            .UsePrism(PrismStartup.Configure)
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        var logFileReader = new NLogFileReader(NLogLoggerConfiguration.LogFilePath);
        builder.Services.AddSingleton<ILogFileReader>(logFileReader);
        builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);
        builder.Services.AddSingleton<IWifiConnector, WifiConnector>();

        builder.Services.AddLogging(configure =>
        {
            configure.SetMinimumLevel(LogLevel.Trace);
            configure.AddNLog();
        });

        return builder.Build();
    }
}
