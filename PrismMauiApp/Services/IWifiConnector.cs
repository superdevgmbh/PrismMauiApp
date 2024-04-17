namespace PrismMauiApp.Services
{
    public interface IWifiConnector
    {
        Task<List<WiFiInfo>> GetAvailableWifis(bool? getSignalStrenth = false);

        Task<bool> ConnectToWifiAsync(string ssid, string password, CancellationToken token = default);

        bool DisconnectWifi(string ssid);
    }
}
