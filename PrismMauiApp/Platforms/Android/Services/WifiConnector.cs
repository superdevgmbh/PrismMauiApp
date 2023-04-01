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
            throw new NotImplementedException();
        }
    }
}
