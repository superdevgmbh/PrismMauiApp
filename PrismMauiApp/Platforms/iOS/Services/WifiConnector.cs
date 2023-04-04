using NetworkExtension;
using PrismMauiApp.Services;

namespace PrismMauiApp.Platforms.Services
{
    public class WifiConnector : IWifiConnector
    {
        private readonly NEHotspotConfigurationManager wifiManager;

        public WifiConnector()
        {
            this.wifiManager = new NEHotspotConfigurationManager();
        }

        public void ConnectToWifi(string ssid, string password)
        {
            var wifiConfig = new NEHotspotConfiguration(ssid, password, false);
            this.wifiManager.ApplyConfiguration(wifiConfig, (error) =>
            {
                if (error != null)
                {
                    Console.WriteLine($"Error while connecting to WiFi network {ssid}: {error}");
                }
            });
        }

        public void Disconnect(string ssid)
        {
            this.wifiManager.RemoveConfiguration(ssid);
        }
    }
}
