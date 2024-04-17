using System.Diagnostics.CodeAnalysis;

namespace PrismMauiApp.Services
{
    [ExcludeFromCodeCoverage]
    public class WifiConnectorMock : IWifiConnector
    {
        public async Task<bool> ConnectToWifiAsync(string ssid, string password, CancellationToken token = default)
        {
            await Task.Delay(1000);
            return true;
        }

        public bool DisconnectWifi(string ssid)
        {
            return true;
        }

        public async Task<List<WiFiInfo>> GetAvailableWifis(bool? getSignalStrenth = false)
        {
            await Task.Delay(1000);

            return new List<WiFiInfo>
            {
                new WiFiInfo
                {
                    SSID = "testwifi1",
                },
                new WiFiInfo
                {
                    SSID = "testwifi2",
                },
                new WiFiInfo
                {
                    SSID = "testwifi3",
                },
                new WiFiInfo
                {
                    SSID = "PiWeatherDisplay_3B05CA",
                },
            };
        }
    }
}
