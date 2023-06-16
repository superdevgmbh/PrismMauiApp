namespace PrismMauiApp.Services
{
    public interface INetworkService
    {
        Task<IEnumerable<string>> ScanAsync(CancellationToken cancellationToken = default);
    }
}