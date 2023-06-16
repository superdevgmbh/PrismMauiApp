namespace PrismMauiApp.Services.Login
{
    public interface IIdentityService
    {
        Task LoginAsync(string username, string password);
    }
}