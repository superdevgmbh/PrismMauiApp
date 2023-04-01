namespace PrismMauiApp.Services
{
    public interface IWifiConnector
    {
        void ConnectToWifi(string ssid, string password);
    }
}
