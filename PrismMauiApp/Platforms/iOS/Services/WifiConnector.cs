using Intents;
using NetworkExtension;
using PrismMauiApp.Services;

namespace PrismMauiApp.Platforms.Services
{
    public class WifiConnector : IWifiConnector
    {
        private readonly NEHotspotConfigurationManager wifiManager;
        private readonly ILogger<WifiConnector> logger;

        public WifiConnector(ILogger<WifiConnector> logger)
        {
            this.logger = logger;

            this.wifiManager = new NEHotspotConfigurationManager();
        }

        public Task<List<WiFiInfo>> GetAvailableWifis(bool? getSignalStrenth = false)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ConnectToWifiAsync(string ssid, string password, CancellationToken token = default)
        {
            this.logger.LogDebug($"ConnectToWifiAsync: ssid={ssid}");

            var wifiConfig = new NEHotspotConfiguration(ssid, password, false);
            var success = false;

            try
            {
                await this.wifiManager.ApplyConfigurationAsync(wifiConfig);
                success = true;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"ConnectToWifiAsync with ssid={ssid} failed with exception");
            }

            this.logger.LogDebug($"ConnectToWifiAsync with ssid={ssid} -> success={success}");
            return success;
        }

        public bool DisconnectWifi(string ssid)
        {
            this.logger.LogDebug($"DisconnectWifi: ssid={ssid}");

            this.wifiManager.RemoveConfiguration(ssid);
            return true;
        }
    }
}
