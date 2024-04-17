using System.Diagnostics.CodeAnalysis;

namespace PrismMauiApp.Services
{
    [ExcludeFromCodeCoverage]
    public class NetworkServiceMock : INetworkService
    {
        public async Task ConnectToWifiAsync(string ssid, string psk)
        {
            await Task.Delay(1000);
        }

        public async Task<string[]> ScanAsync(CancellationToken cancellationToken = default)
        {
            await Task.Delay(3000);

            return new[]
            {
                "testwifi1",
                "testwifi2",
                "testwifi3",
                "PiWeatherDisplay_1",
                "PiWeatherDisplay_2",
                "PiWeatherDisplay_3",
            };
        }
    }
}
