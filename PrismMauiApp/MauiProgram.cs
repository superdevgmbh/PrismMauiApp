using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using NLog.Extensions.Logging;
using PrismMauiApp.Platforms.Services;
using PrismMauiApp.Services;

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

        #region Microsoft.Extensions.DependencyInjection
        builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);
        builder.Services.AddSingleton<IWifiConnector, WifiConnector>();
        #endregion

        builder.Services.AddLogging(configure =>
        {
            configure.SetMinimumLevel(LogLevel.Trace);
            configure.AddDebug();
            configure.AddNLog();
        });

        return builder.Build();
    }
}
