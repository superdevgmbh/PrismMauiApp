namespace PrismMauiApp.Services
{
    public interface INetworkService
    {
        Task<string[]> ScanAsync(CancellationToken cancellationToken = default);

        Task ConnectToWifiAsync(string ssid, string psk);
    }
}