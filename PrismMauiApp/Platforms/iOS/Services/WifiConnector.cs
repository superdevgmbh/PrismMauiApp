using Intents;
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

        public async Task<bool> ConnectToWifi(string ssid, string password, CancellationToken token = default)
        {
            var wifiConfig = new NEHotspotConfiguration(ssid, password, false);

            try
            {
                await this.wifiManager.ApplyConfigurationAsync(wifiConfig);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DisconnectWifi(string ssid)
        {
            this.wifiManager.RemoveConfiguration(ssid);
            return true;
        }
    }
}
