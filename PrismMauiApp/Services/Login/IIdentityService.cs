namespace PrismMauiApp.Services.Login
{
    public interface IIdentityService
    {
        Task<string> LoginAsync(string username, string password);
    }
}