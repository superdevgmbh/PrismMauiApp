using PrismMauiApp.Platforms.Services;
using PrismMauiApp.Services;
using PrismMauiApp.ViewModels;
using PrismMauiApp.Views;

namespace PrismMauiApp;

internal static class PrismStartup
{
    public static void Configure(PrismAppBuilder builder)
    {
        builder.RegisterTypes(RegisterTypes);
        builder.OnInitialized((IContainerProvider obj) =>
        {

        });
        builder.OnAppStart(async navigationService =>
        {
            var result = await navigationService.NavigateAsync("/TabbedMainPage?selectedTab=ViewA");
            if (!result.Success)
            {
                System.Diagnostics.Debugger.Break();
            }
        });
    }

    private static void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<NavigationPage>();
        containerRegistry.RegisterForNavigation<TabbedMainPage, TabbedMainPageViewModel>();
        containerRegistry.RegisterForNavigation<HomePage, HomePageViewModel>();
        containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
        containerRegistry.RegisterForNavigation<ViewA>(App.Pages.ViewA);

        containerRegistry.RegisterInstance(() => SemanticScreenReader.Default);
        containerRegistry.RegisterInstance(() => Connectivity.Current);
        containerRegistry.RegisterSingleton<IWifiConnector, WifiConnector>();
    }
}
