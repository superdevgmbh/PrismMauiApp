namespace PrismMauiApp.Services
{
    public class WiFiInfo
    {
        public string SSID { get; set; }
        public string BSSID { get; set; }
        public int NetworkId { get; set; }
        public int Signal { get; set; }
        public bool IsConnected { get; set; }
    }
}
