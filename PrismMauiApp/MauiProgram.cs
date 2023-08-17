using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Caching.InMemory;
using NLog.Extensions.Logging;
using PrismMauiApp.Extensions;
#if ANDROID || IOS
using PrismMauiApp.Platforms;
#endif
using PrismMauiApp.ViewModels;
using PrismMauiApp.ViewModels.Devices;
using PrismMauiApp.Views;

namespace PrismMauiApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {

        ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
        {
            var viewName = viewType.FullName.ReplaceLastOccurrence(".Views.", ".ViewModels.");
            var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;

            var viewModelName = viewName.ReplaceLastOccurrence("Page", viewName.EndsWith("View") ? "Model" : "ViewModel");
            var viewModelFullName = string.Format(CultureInfo.InvariantCulture, "{0}, {1}", viewModelName, viewAssemblyName);
            var type = Type.GetType(viewModelFullName);
            return type;
        });

        var builder = MauiApp.CreateBuilder()
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            //            .UseMauiCompatibility()
            //            .ConfigureMauiHandlers(handlers =>
            //            {
            //                //Xamarin.Forms Renderers

            //                //#if IOS
            //                //                handlers.AddCompatibilityRenderer(typeof(Page1ModalPage), typeof(ModalPageCustomRenderer));
            //                //                handlers.AddCompatibilityRenderer(typeof(TabbedPageRuntimeModal), typeof(ModalPageCustomRenderer));
            //                //                handlers.AddCompatibilityRenderer(typeof(TabModalPage), typeof(ModalPageCustomRenderer));
            //                //#endif
            //            })
            //            .ConfigureLifecycleEvents(builder =>
            //            {
            //#if ANDROID
            //                builder.AddAndroid(android =>
            //                {
            //                    android.OnBackPressed(activity => true);
            //                });
            //#endif
            //            })
            .UsePrism(prism =>
            {
                prism
                    .RegisterTypes(RegisterTypes)
                    .RegisterTypes(RegisterPages)
#if ANDROID || IOS
                    .RegisterTypes(PlatformInitializer.RegisterTypes)
#endif
                    .OnInitialized((IContainerProvider c) =>
                    {

                    })
                    .OnAppStart(async (c, navigationService) =>
                    {
                        var result = await navigationService.NavigateAsync($"/{App.Pages.AppStartPage}");
                        if (!result.Success)
                        {
                            Debug.WriteLine($"{result}");
                            Debugger.Break();
                        }
                    });
            })
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddLogging(configure =>
        {
            configure.SetMinimumLevel(LogLevel.Trace);
            configure.AddNLog();
        });

        var app = builder.Build();

        //CommunityToolkit.Mvvm.DependencyInjection.Ioc.Default.ConfigureServices(app.Services);

        return app;
    }

    private static void RegisterTypes(IContainerRegistry containerRegistry)
    {
        RegisterServices(containerRegistry);
        RegisterPages(containerRegistry);
    }

    private static void RegisterServices(IContainerRegistry containerRegistry)
    {
        var logFileReader = new NLogFileReader(NLogLoggerConfiguration.LogFilePath);
        containerRegistry.RegisterInstance<ILogFileReader>(logFileReader);

        containerRegistry.RegisterSingleton<IIdentityService, IdentityService>();
        containerRegistry.RegisterSingleton<INetworkService, NetworkService>();
        containerRegistry.RegisterSingleton<IDisplayRepository, DisplayRepository>();

        containerRegistry.Register<IConnectivity>(() => Connectivity.Current);
        containerRegistry.Register<ISecureStorage>(() => SecureStorage.Default);
        containerRegistry.Register<IDeviceInfo>(() => DeviceInfo.Current);
        containerRegistry.Register<IAppInfo>(() => AppInfo.Current);

        containerRegistry.RegisterSingleton<IMemoryCache, MemoryCache>();
        containerRegistry.RegisterSingleton<IApiService, ApiService>();
        containerRegistry.RegisterSingleton<IApiServiceConfiguration>(() =>
        {
            return new DefaultApiServiceConfiguration
            {
                Timeout = TimeSpan.FromSeconds(20),
            };
        });

    }

    private static void RegisterPages(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<NavigationPage>();
        containerRegistry.RegisterForNavigation<AppStartPage, AppStartViewModel>(App.Pages.AppStartPage);
        containerRegistry.RegisterForNavigation<TabbedMainPage, TabbedMainViewModel>();
        containerRegistry.RegisterForNavigation<HomePage, HomeViewModel>(App.Pages.HomePage);
        containerRegistry.RegisterForNavigation<MainPage, MainViewModel>(App.Pages.MainPage);
        containerRegistry.RegisterForNavigation<AddNewDevicePage, AddNewDeviceViewModel>(App.Pages.AddNewDevicePage);
        containerRegistry.RegisterForNavigation<ConnectToDevicePage, ConnectToDeviceViewModel>(App.Pages.ConnectToDevicePage);
        containerRegistry.RegisterForNavigation<AboutPage, AboutViewModel>(App.Pages.AboutPage);
    }

    //static MauiAppBuilder RegisterAppServices(this MauiAppBuilder builder)
    //{
    //    // register your own services here!
    //    return builder;
    //}

    //static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
    //{
    //    var serviceCollection = builder.Services;
    //    serviceCollection.RegisterForNavigation<NavigationPage>();
    //    serviceCollection.RegisterForNavigation<TabbedMainPage, TabbedMainViewModel>();
    //    serviceCollection.RegisterForNavigation<HomePage, HomeViewModel>(App.Pages.HomePage);
    //    serviceCollection.RegisterForNavigation<MainPage, MainViewModel>(App.Pages.MainPage);
    //    return builder;
    //}

}
