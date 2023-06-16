namespace PrismMauiApp.Platforms
{
    public static class PlatformInitializer
    {
        public static void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<HttpMessageHandler, NSUrlSessionHandler>();
        }
    }
}
