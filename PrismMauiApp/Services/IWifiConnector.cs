namespace PrismMauiApp.Services
{
    public interface IWifiConnector
    {
        Task<bool> ConnectToWifi(string ssid, string password, CancellationToken token = default);

        bool DisconnectWifi(string ssid);
    }
}
