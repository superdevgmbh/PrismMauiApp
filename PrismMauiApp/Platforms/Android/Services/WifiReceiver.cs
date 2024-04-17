using Android.Content;
using Android.Net.Wifi;
using Android.Widget;

namespace PrismMauiApp.Platforms.Services
{
    [BroadcastReceiver(Enabled = true, Exported = false)]
    public class WifiReceiver : BroadcastReceiver
    {
        private readonly WifiManager wifiManager;
        private readonly List<WiFiInfo> wiFiInfos;
        private readonly List<string> wifiNetworks;
        private readonly AutoResetEvent receiverARE;
        private System.Threading.Timer tmr;
        private const int TIMEOUT_MILLIS = 20000; // 20 seconds timeout
        readonly string connectedSSID;

        public WifiReceiver()
        {
            
        }

        public WifiReceiver(WifiManager wifiManager)
        {
            this.wifiManager = wifiManager;
            this.wifiNetworks = new List<string>();
            this.wiFiInfos = new List<WiFiInfo>();
            this.receiverARE = new AutoResetEvent(false);

            //connectedSSID = ((WifiManager)wifi).ConnectionInfo.SSID.Replace("\"","");
            this.connectedSSID = ((WifiManager)wifiManager).ConnectionInfo.BSSID;
        }

        public List<WiFiInfo> Scan()
        {
            this.tmr = new System.Threading.Timer(this.Timeout, null, TIMEOUT_MILLIS, System.Threading.Timeout.Infinite);
            this.wifiManager.StartScan();
            this.receiverARE.WaitOne();
            return this.wiFiInfos;
        }

        //public string AddNetworkSuggestion()
        //{
        //    var suggestion = new WifiNetworkSuggestion.Builder()
        //    .SetSsid(Static.Secrets.PreferredWifi)
        //    .SetWpa2Passphrase(Static.Secrets.PrefferedWifiPassword)
        //    .Build();

        //    var suggestions = new[] { suggestion };
        //    var status = this.wifi.AddNetworkSuggestions(suggestions);
        //    if (status != NetworkStatus.SuggestionsSuccess)
        //    {
        //        status = this.wifi.RemoveNetworkSuggestions(suggestions);
        //        status = this.wifi.AddNetworkSuggestions(suggestions);
        //    }

        //    var statusText = status switch
        //    {
        //        NetworkStatus.SuggestionsSuccess => "Pomyślnie zasugerowano sieć",
        //        NetworkStatus.SuggestionsErrorAddDuplicate => "Sugestia takiej sieci już istnieje",
        //        NetworkStatus.SuggestionsErrorAddExceedsMaxPerApp => "Przekroczono limit ilości sugestii"
        //    };

        //    //var toast = Toast.MakeText(Application.Context, statusText, ToastLength.Long);
        //    //toast.Show();

        //    return statusText;
        //}


        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action.Equals(WifiManager.ActionWifiNetworkSuggestionPostConnection))
            {
                //wifi suggestion finished
                var toast = Toast.MakeText(Android.App.Application.Context, "Sugestia sieci zakońoczna", ToastLength.Long);
                toast.Show();
            }
            else
            {
                //wifi scan finished
                IList<ScanResult> scanwifinetworks = this.wifiManager.ScanResults;
                foreach (ScanResult wifinetwork in scanwifinetworks)
                {
                    bool isConnected = false;
                    if (wifinetwork.Bssid == this.connectedSSID)
                    {
                        isConnected = true;
                    }
                    var nWF = new WiFiInfo
                    {
                        SSID = wifinetwork.Ssid,
                        BSSID = wifinetwork.Bssid,
                        Signal = wifinetwork.Level,
                        IsConnected = isConnected
                    };
                    this.wiFiInfos.Add(nWF);
                    //wifiNetworks.Add(wifinetwork.Ssid);
                }
            }

            this.receiverARE.Set();
        }

        private void Timeout(object sender)
        {
            // NOTE release scan, which we are using now, or we throw an error?
            this.receiverARE.Set();
        }
    }
}
