using System.Net.Security;
using Xamarin.Android.Net;

namespace PrismMauiApp.Platforms
{
    public static class PlatformInitializer
    {
        public static void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var androidMessageHandler = new AndroidMessageHandler
            {
                //ServerCertificateCustomValidationCallback = (httpRequestMessage, certificate, chain, sslPolicyErrors) =>
                //{
                //    return certificate?.Issuer == "CN=WeatherDisplay" || sslPolicyErrors == SslPolicyErrors.None;
                //}
            };

            containerRegistry.RegisterSingleton<HttpMessageHandler>(() => androidMessageHandler);
            containerRegistry.RegisterSingleton<IWifiConnector, WifiConnectorMock>();
        }
    }
}
