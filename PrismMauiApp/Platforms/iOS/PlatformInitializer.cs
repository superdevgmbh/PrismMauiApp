using PrismMauiApp.Platforms.Services;

namespace PrismMauiApp.Platforms
{
    public static class PlatformInitializer
    {
        public static void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<HttpMessageHandler, NSUrlSessionHandler>();
            containerRegistry.RegisterSingleton<IWifiConnector, WifiConnector>();
        }
    }
}
