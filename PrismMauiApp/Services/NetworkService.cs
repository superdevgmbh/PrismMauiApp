namespace PrismMauiApp.Services
{
    public class NetworkService : INetworkService
    {
        private readonly IApiService apiService;

        public NetworkService(IApiService apiService)
        {
            this.apiService = apiService;
        }

        public async Task<string[]> ScanAsync(CancellationToken cancellationToken = default)
        {
            var ssids = await this.apiService.GetAsync<string[]>("api/system/network/wifi/scan", cancellationToken).ConfigureAwait(false);
            return ssids;
        }

        public async Task ConnectToWifiAsync(string ssid, string psk)
        {
            var request = new ConnectToWifiRequest
            {
                SSID = ssid,
                PSK = psk
            };
            await this.apiService.PostAsync("api/system/network/wifi", request).ConfigureAwait(false);
        }

        public class ConnectToWifiRequest
        {
            public string SSID { get; set; }

            public string PSK { get; set; }
        }

    }
}
