using PrismMauiApp.Platforms.Services;
using Xamarin.Android.Net;

namespace PrismMauiApp.Platforms
{
    public static class PlatformInitializer
    {
        public static void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<HttpMessageHandler, AndroidMessageHandler>();
            containerRegistry.RegisterSingleton<IWifiConnector, WifiConnector>();
        }
    }
}
