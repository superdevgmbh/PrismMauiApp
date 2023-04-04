using Android.Content;
using Android.Net.Wifi;
using PrismMauiApp.Services;

namespace PrismMauiApp.Platforms.Services
{
    public class WifiConnector : IWifiConnector
    {
        private readonly WifiManager wifiManager;

        public WifiConnector()
        {
            this.wifiManager = (WifiManager)global::Android.App.Application.Context
                        .GetSystemService(Context.WifiService);
        }

        public void ConnectToWifi(string ssid, string password)
        {
            var formattedSsid = $"\"{ssid}\"";
            var formattedPassword = $"\"{password}\"";

            var wifiConfig = new WifiConfiguration
            {
                Ssid = formattedSsid,
                PreSharedKey = formattedPassword
            };

            var addNetwork = this.wifiManager.AddNetwork(wifiConfig);

            var network = this.wifiManager.ConfiguredNetworks
                 .FirstOrDefault(n => n.Ssid == ssid);

            if (network == null)
            {
                Console.WriteLine($"Cannot connect to network: {ssid}");
                return;
            }

            this.wifiManager.Disconnect();

            var result = this.wifiManager.EnableNetwork(network.NetworkId, true);
        }

        public void Disconnect(string ssid)
        {
            var network = this.wifiManager.ConfiguredNetworks
                 .FirstOrDefault(n => n.Ssid == ssid);

            if (network != null)
            {
                this.wifiManager.RemoveNetwork(network.NetworkId);
            }
        }
    }
}
