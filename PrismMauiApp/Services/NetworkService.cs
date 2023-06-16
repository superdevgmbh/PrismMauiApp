using PrismMauiApp.Services.Http;

namespace PrismMauiApp.Services
{
    public class NetworkService : INetworkService
    {
        private readonly IApiService apiService;

        public NetworkService(IApiService apiService)
        {
            this.apiService = apiService;
        }

        public async Task<IEnumerable<string>> ScanAsync(CancellationToken cancellationToken = default)
        {
            var ssids = await this.apiService.GetAsync<List<string>>("api/system/network/scan", cancellationToken).ConfigureAwait(false);
            return ssids;
        }
    }
}
